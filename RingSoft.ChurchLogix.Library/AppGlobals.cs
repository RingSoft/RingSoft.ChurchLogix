using Microsoft.EntityFrameworkCore;
using RingSoft.App.Interop;
using RingSoft.App.Library;
using RingSoft.ChurchLogix.DataAccess;
using RingSoft.ChurchLogix.DataAccess.Model;
using RingSoft.ChurchLogix.DataAccess.Model.StaffManagement;
using RingSoft.ChurchLogix.Library.ViewModels;
using RingSoft.ChurchLogix.MasterData;
using RingSoft.ChurchLogix.Sqlite;
using RingSoft.ChurchLogix.SqlServer;
using RingSoft.DataEntryControls.Engine;
using RingSoft.DbLookup;
using RingSoft.DbLookup.DataProcessor;
using RingSoft.DbLookup.EfCore;

namespace RingSoft.ChurchLogix.Library
{
    public class AppProgressArgs
    {
        public string ProgressText { get; }

        public AppProgressArgs(string progressText)
        {
            ProgressText = progressText;
        }
    }
    public class AppGlobals
    {
        public static ChurchLogixLookupContext LookupContext { get; private set; }

        public static bool UnitTesting { get; set; }

        public static DataRepository DataRepository { get; set; }

        public static MainViewModel MainViewModel { get; set; }

        public static Church LoggedInChurch { get; set; }

        public static StaffPerson LoggedInStaffPerson { get; set; }

        public static DbPlatforms DbPlatform { get; set; }

        public static SystemPreferences SystemPreferences { get; set; }

        public static event EventHandler<AppProgressArgs> AppSplashProgress;

        public static void InitSettings()
        {
            RingSoftAppGlobals.AppTitle = "ChurchLogix";
            RingSoftAppGlobals.AppCopyright = "©2024 by Peter Ringering";
            RingSoftAppGlobals.PathToDownloadUpgrade = MasterDbContext.ProgramDataFolder;
            RingSoftAppGlobals.AppGuid = "cd59af5f-799d-4203-8bfa-f4fdbe35c49c";
            RingSoftAppGlobals.AppVersion = 334;
            SystemGlobals.ProgramDataFolder = MasterDbContext.ProgramDataFolder;
        }

        public static void Initialize()
        {
            if (!UnitTesting)
            {
                DataRepository ??= new DataRepository();
            }

            var test = SystemGlobals.DataRepository;

            AppSplashProgress?.Invoke(null, new AppProgressArgs("Initializing Database Structure."));

            LookupContext = new ChurchLogixLookupContext();
            LookupContext.SqliteDataProcessor.FilePath = MasterDbContext.ProgramDataFolder;
            LookupContext.SqliteDataProcessor.FileName = MasterDbContext.DemoDataFileName;

            SystemGlobals.ItemRightsFactory = new ChurchLogixRightsFactory();

            if (!UnitTesting)
            {
                AppSplashProgress?.Invoke(null, new AppProgressArgs("Connecting to the Master Database."));

                MasterDbContext.ConnectToMaster();

                var defaultChurch = MasterDbContext.GetDefaultChurch();
                if (defaultChurch != null)
                {
                    if (LoginToChurch(defaultChurch).IsNullOrEmpty())
                        LoggedInChurch = defaultChurch;
                }
            }

        }

        public static string LoginToChurch(Church church)
        {
            AppSplashProgress?.Invoke(null, new AppProgressArgs($"Migrating the {church.Name} Database."));
            DbPlatform = (DbPlatforms)church.Platform;
            LookupContext.SetProcessor(DbPlatform);
            var context = GetNewDbContext();
            context.SetLookupContext(LookupContext);
            LoadDataProcessor(church);
            SystemMaster systemMaster = null;
            DbContext migrateContext = context.DbContext;
            var migrateResult = string.Empty;

            switch ((DbPlatforms)church.Platform)
            {
                case DbPlatforms.Sqlite:
                    if (!church.FilePath.EndsWith('\\'))
                        church.FilePath += "\\";

                    LookupContext.Initialize(context, DbPlatform);

                    var newFile = !File.Exists($"{church.FilePath}{church.FileName}");

                    if (newFile == false)
                    {
                        try
                        {
                            var file = new FileInfo($"{church.FilePath}{church.FileName}");
                            file.IsReadOnly = false;
                        }
                        catch (Exception e)
                        {
                            var message = $"Can't access Church file path: {church.FilePath}.  You must run this program as administrator.";
                            return message;
                        }
                        migrateResult = MigrateContext(migrateContext);
                        if (!migrateResult.IsNullOrEmpty())
                        {
                            return migrateResult;
                        }

                        systemMaster = context.SystemMaster.FirstOrDefault();
                        if (systemMaster != null) church.Name = systemMaster.ChurchName;
                    }
                    else
                    {
                        migrateResult = MigrateContext(migrateContext);
                        if (!migrateResult.IsNullOrEmpty())
                        {
                            return migrateResult;
                        }

                        context.DbContext.Database.Migrate();
                        systemMaster = new SystemMaster
                        {
                            ChurchName = church.Name,
                            AppGuid = RingSoftAppGlobals.AppGuid,
                        };
                        context.DbContext.AddNewEntity(context.SystemMaster, systemMaster, "Saving SystemMaster");
                    }

                    break;
                case DbPlatforms.SqlServer:

                    var databases = RingSoftAppGlobals.GetSqlServerDatabaseList(church.Server);
                    {
                        var migrate = true;
                        if (databases.IndexOf(church.Database) < 0)
                        {
                            migrateResult = MigrateContext(migrateContext);
                            if (!migrateResult.IsNullOrEmpty())
                            {
                                return migrateResult;
                            }
                        }
                        else
                        {
                            var context1 = SystemGlobals.DataRepository.GetDataContext();
                            var sysMasterTable = context1.GetTable<SystemMaster>();
                            try
                            {
                                var sysMasterRecord = sysMasterTable.FirstOrDefault();
                                var appGuid = sysMasterRecord.AppGuid;
                            }
                            catch (Exception e)
                            {
                                MigrateContext(migrateContext);
                            }

                            migrate = AllowMigrate();

                            if (migrate)
                            {
                                migrateResult = MigrateContext(migrateContext);
                                if (!migrateResult.IsNullOrEmpty())
                                {
                                    return migrateResult;
                                }
                            }
                        }
                    }

                    if (databases.IndexOf(church.Database) >= 0)
                    {
                        systemMaster = context.SystemMaster.FirstOrDefault();
                        if (systemMaster != null) church.Name = systemMaster.ChurchName;
                    }

                    if (systemMaster == null)
                    {
                        systemMaster = new SystemMaster
                        {
                            ChurchName = church.Name,
                            AppGuid = RingSoftAppGlobals.AppGuid,
                        };
                        context.DbContext.AddNewEntity(context.SystemMaster, systemMaster, "Saving SystemMaster");
                    }
                    LookupContext.Initialize(context, DbPlatform);

                    break;
                case DbPlatforms.MySql:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            SetupSystem(church);

            return string.Empty;
        }

        public static void SetupSystem(Church church)
        {
            SystemGlobals.Rights = new AppRights(new ChurchLogixRights());

            AppSplashProgress?.Invoke(null, new AppProgressArgs($"Connecting to the {church.Name} Database."));
            var sysContext = SystemGlobals.DataRepository.GetDataContext();
            var sysPrefsTable = sysContext.GetTable<SystemPreferences>();
            
            var sysPrefsRecord = sysPrefsTable.FirstOrDefault();
            if (sysPrefsRecord == null)
            {
                sysPrefsRecord = new SystemPreferences();
                sysContext.SaveEntity(sysPrefsRecord, "Creating New System Preferences Record");
            }

            SystemPreferences = sysContext.GetTable<SystemPreferences>().FirstOrDefault();
            if (SystemPreferences != null)
            {
                if (!SystemPreferences.FiscalYearEnd.HasValue)
                {
                    SystemPreferences.FiscalYearEnd = new DateTime(DateTime.Today.Year, 12, 31);
                    sysContext.SaveEntity(SystemPreferences, "");
                }
            }
        }

        public static DateTime GetYearEndDate(DateTime currentDate)
        {
            DateTime? result = null;
            if (SystemPreferences.FiscalYearEnd.HasValue)
            {
                var yearEndDate = SystemPreferences.FiscalYearEnd.Value;
                if (currentDate > yearEndDate)
                {
                    while (currentDate > yearEndDate)
                    {
                        yearEndDate = yearEndDate.AddYears(1);
                    }

                    result = yearEndDate;
                }
                else
                {
                    if (currentDate.Year < SystemPreferences.FiscalYearEnd.Value.Year)
                    {
                        result = new DateTime(currentDate.Year
                            , SystemPreferences.FiscalYearEnd.Value.Month
                            , SystemPreferences.FiscalYearEnd.Value.Day);
                    }
                    else
                    {
                        result = SystemPreferences.FiscalYearEnd.Value;
                    }
                }
            }
            else
            {
                result = new DateTime(DateTime.Today.Year, 12, 31);
            }

            return result.Value;
        }

        public static bool AllowMigrate(DbDataProcessor processor = null)
        {
            var migrate = true;
            var context = DataRepository.GetDataContext();
            var table = context.GetTable<SystemMaster>();
            var sysMaster = new SystemMaster()
            {
                ChurchName = Guid.NewGuid().ToString(),
                AppGuid = RingSoftAppGlobals.AppGuid,
            };

            var sysMasters = new List<SystemMaster>();
            sysMasters.Add(sysMaster);

            context.AddRange(sysMasters);
            migrate = context.Commit("Checking System Master");
            if (migrate)
            {
                context.RemoveRange(sysMasters);
                migrate = context.Commit("Checking Migrate");
            }

            return migrate;
        }

        public static string MigrateContext(DbContext migrateContext)
        {
            try
            {
                migrateContext.Database.Migrate();
            }
            catch (Exception e)
            {
                return e.Message;
            }

            return string.Empty;
        }



        public static IChurchLogixDbContext GetNewDbContext(DbPlatforms? platform = null)
        {
            if (platform == null)
            {
                platform = DbPlatform;
            }
            switch (platform)
            {
                case DbPlatforms.Sqlite:
                    var sqliteResult = new ChurchLogixSqliteDbContext();
                    sqliteResult.SetLookupContext(AppGlobals.LookupContext);
                    return sqliteResult;
                case DbPlatforms.SqlServer:
                    var result = new ChurchLogixSqlServerDbContext();
                    result.SetLookupContext(AppGlobals.LookupContext);
                    return result;
                //case DbPlatforms.MySql:
                //    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static void LoadDataProcessor(Church church, DbPlatforms? platform = null)
        {
            if (platform == null)
            {
                platform = (DbPlatforms)church.Platform;
            }

            switch (platform)
            {
                case DbPlatforms.Sqlite:
                    LookupContext.SqliteDataProcessor.FilePath = church.FilePath;
                    LookupContext.SqliteDataProcessor.FileName = church.FileName;
                    break;
                case DbPlatforms.SqlServer:
                    LookupContext.SqlServerDataProcessor.Server = church.Server;
                    LookupContext.SqlServerDataProcessor.Database = church.Database;
                    LookupContext.SqlServerDataProcessor.SecurityType = (SecurityTypes)church.AuthenticationType;
                    LookupContext.SqlServerDataProcessor.UserName = church.Username;
                    LookupContext.SqlServerDataProcessor.Password = church.Password.DecryptDatabasePassword();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

    }
}
