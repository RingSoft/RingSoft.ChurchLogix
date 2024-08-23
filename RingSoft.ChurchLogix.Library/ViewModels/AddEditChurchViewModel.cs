using Microsoft.EntityFrameworkCore;
using RingSoft.App.Interop;
using RingSoft.App.Library;
using RingSoft.ChurchLogix.DataAccess;
using RingSoft.ChurchLogix.MasterData;
using RingSoft.ChurchLogix.Sqlite;
using RingSoft.ChurchLogix.SqlServer;
using RingSoft.DataEntryControls.Engine;
using RingSoft.DbLookup.DataProcessor;
using RingSoft.DbLookup.EfCore;

namespace RingSoft.ChurchLogix.Library.ViewModels
{
    public enum SetFocusControls
    {
        ChurchName = 0,
        FileName = 1
    }

    public enum ChurchProcesses
    {
        Add = 0,
        Edit = 1,
        Connect = 2
    }

    public interface IAddEditChurchView : IDbLoginView
    {
        void SetFocus(SetFocusControls control);

        void SetPlatform();

        Church Church { get; set; }

        ChurchProcesses ChurchProcess { get; set; }
    }


    public class AddEditChurchViewModel : DbLoginViewModel<Church>
    {
        public new IAddEditChurchView View { get; private set; }
        protected override string TestTable => "SystemMaster";
        public AddEditChurchViewModel()
        {
            DbName = "ChurchLogix";
        }

        public override void Initialize(IDbLoginView view, DbLoginProcesses loginProcess, SqliteLoginViewModel sqliteLoginViewModel,
            SqlServerLoginViewModel sqlServerLoginViewModel, Church entity)
        {
            if (view is IAddEditChurchView addEditChurchView)
            {
                View = addEditChurchView;
            }

            base.Initialize(view, loginProcess, sqliteLoginViewModel, sqlServerLoginViewModel, entity);
        }

        public override void LoadFromEntity(Church entity)
        {
            EntityName = entity.Name;
            DbPlatform = (DbPlatforms)entity.Platform;
            var directory = entity.FilePath;
            if (!directory.IsNullOrEmpty() && !directory.EndsWith("\\"))
            {
                directory += "\\";
            }

            SqliteLoginViewModel.FilenamePath = $"{directory}{entity.FileName}";
            SqlServerLoginViewModel.Server = entity.Server;
            SqlServerLoginViewModel.Database = entity.Database;
            if (entity.AuthenticationType != null)
                SqlServerLoginViewModel.SecurityType = (SecurityTypes)entity.AuthenticationType.Value;
            SqlServerLoginViewModel.UserName = entity.Username;
            SqlServerLoginViewModel.Password = entity.Password.DecryptDatabasePassword();

        }

        protected override void ShowEntityNameFailure()
        {
            var message = $"Church Name must have a value";
            ControlsGlobals.UserInterface.ShowMessageBox(message, "Invalid Church Name", RsMessageBoxIcons.Exclamation);
            View.SetFocus(SetFocusControls.ChurchName);
        }

        protected override void SaveEntity(Church entity)
        {
            if (Object != null)
            {
                entity.Id = Object.Id;
            }
            entity.Name = EntityName;
            entity.FilePath = SqliteLoginViewModel.FilePath;
            entity.FileName = SqliteLoginViewModel.FileName;
            entity.Platform = (byte)DbPlatform;
            entity.Server = SqlServerLoginViewModel.Server;
            entity.Database = SqlServerLoginViewModel.Database;
            entity.AuthenticationType = (byte)SqlServerLoginViewModel.SecurityType;
            entity.Username = SqlServerLoginViewModel.UserName;
            entity.Password = SqlServerLoginViewModel.Password.EncryptDatabasePassword();
        }

        protected override bool PreDataCopy(ref LookupContext context, ref DbDataProcessor destinationProcessor, ITwoTierProcedure procedure)
        {
            DbContext destinationDbContext = null;
            IChurchLogixDbContext sourceDbContext = null;
            context = AppGlobals.LookupContext;
            switch (DbPlatform)
            {
                case DbPlatforms.Sqlite:
                    destinationProcessor = AppGlobals.LookupContext.SqliteDataProcessor;
                    LoadDbDataProcessor(destinationProcessor);
                    var sqliteHomeLogixDbContext = new ChurchLogixSqliteDbContext();
                    sqliteHomeLogixDbContext.SetLookupContext(AppGlobals.LookupContext);
                    destinationDbContext = sqliteHomeLogixDbContext;
                    break;
                case DbPlatforms.SqlServer:
                    var sqlServerProcessor = AppGlobals.LookupContext.SqlServerDataProcessor;
                    destinationProcessor = sqlServerProcessor;
                    LoadDbDataProcessor(destinationProcessor);
                    var sqlServerContext = new ChurchLogixSqlServerDbContext();
                    sqlServerContext.SetLookupContext(AppGlobals.LookupContext);
                    destinationDbContext = sqlServerContext;

                    break;
                default:
                    throw new ArgumentOutOfRangeException();

            }

            AppGlobals.DbPlatform = OriginalDbPlatform;
            switch (OriginalDbPlatform)
            {
                case DbPlatforms.Sqlite:
                    sourceDbContext = new ChurchLogixSqliteDbContext();
                    sourceDbContext.SetLookupContext(AppGlobals.LookupContext);
                    break;
                case DbPlatforms.SqlServer:
                    sourceDbContext = new ChurchLogixSqlServerDbContext();
                    sourceDbContext.SetLookupContext(AppGlobals.LookupContext);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            AppGlobals.LoadDataProcessor(Object, OriginalDbPlatform);
            switch (OriginalDbPlatform)
            {
                case DbPlatforms.Sqlite:
                    break;
                case DbPlatforms.SqlServer:
                    if (!AppGlobals.AllowMigrate(AppGlobals.LookupContext.SqlServerDataProcessor))
                    {
                        var message = "You do not have the permission to copy this database's data.";
                        var caption = "Copy Error";
                        procedure.ShowMessage(message, caption, RsMessageBoxIcons.Exclamation);
                        return false;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            switch (OriginalDbPlatform)
            {
                case DbPlatforms.Sqlite:
                    break;
                case DbPlatforms.SqlServer:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            var dropResult = destinationProcessor.DropDatabase();
            if (dropResult.ResultCode != GetDataResultCodes.Success)
            {
                procedure.ShowError(dropResult.Message, "Error Dropping Database");
                return false;
            }

            procedure.SetCursor(WindowCursorTypes.Wait);
            sourceDbContext = AppGlobals.GetNewDbContext();
            sourceDbContext.SetLookupContext(AppGlobals.LookupContext);
            AppGlobals.LookupContext.Initialize(sourceDbContext, OriginalDbPlatform);


            var result = AppGlobals.MigrateContext(AppGlobals.GetNewDbContext().DbContext);
            if (!result.IsNullOrEmpty())
            {
                procedure.SetCursor(WindowCursorTypes.Default);
                procedure.ShowError(result, "File Access Error");
                return false;
            }

            result = AppGlobals.MigrateContext(destinationDbContext);
            if (!result.IsNullOrEmpty())
            {
                procedure.SetCursor(WindowCursorTypes.Default);
                procedure.ShowError(result, "File Access Error");
                return false;
            }
            return true;
        }
    }
}
