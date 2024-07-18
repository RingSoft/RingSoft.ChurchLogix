using RingSoft.App.Interop;
using RingSoft.App.Library;
using RingSoft.ChurchLogix.MasterData;
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
            throw new NotImplementedException();
        }
    }
}
