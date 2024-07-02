using Microsoft.EntityFrameworkCore;
using RingSoft.App.Library;
using RingSoft.ChurchLogix.DataAccess;
using RingSoft.ChurchLogix.DataAccess.Model;
using RingSoft.ChurchLogix.Library.ViewModels;
using RingSoft.ChurchLogix.MasterData;
using RingSoft.DbLookup;
using RingSoft.DbLookup.EfCore;
using System.Runtime.InteropServices;

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

        public static DbPlatforms DbPlatform { get; set; }

        public static event EventHandler<AppProgressArgs> AppSplashProgress;

        public static void InitSettings()
        {
            RingSoftAppGlobals.AppTitle = "ChurchLogix";
            RingSoftAppGlobals.AppCopyright = "©2024 by Peter Ringering";
            RingSoftAppGlobals.PathToDownloadUpgrade = MasterDbContext.ProgramDataFolder;
            RingSoftAppGlobals.AppGuid = "cd59af5f-799d-4203-8bfa-f4fdbe35c49c";
            RingSoftAppGlobals.AppVersion = 252;
            SystemGlobals.ProgramDataFolder = MasterDbContext.ProgramDataFolder;
        }

        public static void Initialize()
        {
            DataRepository ??= new DataRepository();
            var test = SystemGlobals.DataRepository;
            SystemGlobals.ConvertAllDatesToUniversalTime = true;

            AppSplashProgress?.Invoke(null, new AppProgressArgs("Initializing Database Structure."));

            LookupContext = new ChurchLogixLookupContext();
            LookupContext.SqliteDataProcessor.FilePath = MasterDbContext.ProgramDataFolder;
            LookupContext.SqliteDataProcessor.FileName = MasterDbContext.DemoDataFileName;

            //SystemGlobals.ItemRightsFactory = new DevLogixRightsFactory();

            if (!UnitTesting)
            {
                AppSplashProgress?.Invoke(null, new AppProgressArgs("Connecting to the Master Database."));

                MasterDbContext.ConnectToMaster();

                var defaultChurch = MasterDbContext.GetDefaultChurch();
                if (defaultChurch != null)
                {
                    //if (LoginToOrganization(defaultOrganization).IsNullOrEmpty())
                    //    LoggedInOrganization = defaultOrganization;
                }
            }

        }

        public static string LoginToChurch(Church church)
        {
            AppSplashProgress?.Invoke(null, new AppProgressArgs($"Migrating the {church.Name} Database."));
            DbPlatform = (DbPlatforms)church.Platform;
            LookupContext.SetProcessor(DbPlatform);
            var context = SystemGlobals.DataRepository.GetDataContext();
            context.SetLookupContext(LookupContext);
            LoadDataProcessor(organization);
            SystemMaster systemMaster = null;
            DbContext migrateContext = context.DbContext;
            var migrateResult = string.Empty;

            switch ((DbPlatforms)church.Platform)
            {
                case DbPlatforms.Sqlite:
                    if (!organization.FilePath.EndsWith('\\'))
                        organization.FilePath += "\\";

                    LookupContext.Initialize(context, DbPlatform);

                    var newFile = !File.Exists($"{organization.FilePath}{organization.FileName}");

                    if (newFile == false)
                    {
                        try
                        {
                            var file = new FileInfo($"{organization.FilePath}{organization.FileName}");
                            file.IsReadOnly = false;
                        }
                        catch (Exception e)
                        {
                            var message = $"Can't access Organization file path: {organization.FilePath}.  You must run this program as administrator.";
                            return message;
                        }
                        migrateResult = MigrateContext(migrateContext);
                        if (!migrateResult.IsNullOrEmpty())
                        {
                            return migrateResult;
                        }

                        systemMaster = context.SystemMaster.FirstOrDefault();
                        if (systemMaster != null) organization.Name = systemMaster.OrganizationName;
                    }
                    else
                    {
                        migrateResult = MigrateContext(migrateContext);
                        if (!migrateResult.IsNullOrEmpty())
                        {
                            return migrateResult;
                        }

                        context.DbContext.Database.Migrate();
                        systemMaster = new SystemMaster { OrganizationName = organization.Name };
                        context.DbContext.AddNewEntity(context.SystemMaster, systemMaster, "Saving SystemMaster");

                    }

                    break;
                case DbPlatforms.SqlServer:

                    var databases = RingSoftAppGlobals.GetSqlServerDatabaseList(organization.Server);
                    {
                        var migrate = true;
                        if (databases.IndexOf(organization.Database) < 0)
                        {
                            migrateResult = MigrateContext(migrateContext);
                            if (!migrateResult.IsNullOrEmpty())
                            {
                                return migrateResult;
                            }
                        }
                        else
                        {
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

                    if (databases.IndexOf(organization.Database) >= 0)
                    {
                        systemMaster = context.SystemMaster.FirstOrDefault();
                        if (systemMaster != null) organization.Name = systemMaster.OrganizationName;
                    }

                    if (systemMaster == null)
                    {
                        systemMaster = new SystemMaster { OrganizationName = organization.Name };
                        context.DbContext.AddNewEntity(context.SystemMaster, systemMaster, "Saving SystemMaster");
                    }
                    LookupContext.Initialize(context, DbPlatform);

                    break;
                case DbPlatforms.MySql:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            SystemGlobals.Rights = new AppRights(new DevLogixRights());

            AppSplashProgress?.Invoke(null, new AppProgressArgs($"Connecting to the {organization.Name} Database."));
            //var selectQuery = new SelectQuery(LookupContext.SystemMaster.TableName);
            //LookupContext.DataProcessor.GetData(selectQuery, false);
            DataAccessGlobals.SetupSysPrefs();
            return string.Empty;
        }

    }
}
