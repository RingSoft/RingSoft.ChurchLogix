using System.Windows;
using RingSoft.App.Controls;
using RingSoft.App.Library;
using RingSoft.ChurchLogix.Library.ViewModels;
using RingSoft.ChurchLogix.MasterData;

namespace RingSoft.ChurchLogix
{
    /// <summary>
    /// Interaction logic for AddEditChurchWindow.xaml
    /// </summary>
    public partial class AddEditChurchWindow : IAddEditChurchView
    {
        public Church Church { get; set; }
        public ChurchProcesses ChurchProcess { get; set; }
        public bool DataCopied { get; set; }

        private TwoTierProcedure _procedure;

        public AddEditChurchWindow(DbLoginProcesses loginProcess, Church church = null)
        {
            InitializeComponent();
            Church = church;

            SqliteLogin.Loaded += (sender, args) => ViewModel.Initialize(this, loginProcess, SqliteLogin.ViewModel,
                SqlServerLogin.ViewModel, church);
            SqlServerLogin.Loaded += (sender, args) => ViewModel.Initialize(this, loginProcess, SqliteLogin.ViewModel,
                SqlServerLogin.ViewModel, church);

            switch (loginProcess)
            {
                case DbLoginProcesses.Add:
                case DbLoginProcesses.Edit:
                    break;
                case DbLoginProcesses.Connect:
                    ChurchNameLabel.Visibility = Visibility.Collapsed;
                    ChurchNameTextBox.Visibility = Visibility.Collapsed;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(loginProcess), loginProcess, null);
            }

        }

        public void CloseWindow()
        {
            Close();
        }

        public void SetPlatform()
        {
            SqliteLogin.Visibility = Visibility.Collapsed;
            SqlServerLogin.Visibility = Visibility.Collapsed;
            switch (ViewModel.DbPlatform)
            {
                case DbPlatforms.Sqlite:
                    SqliteLogin.Visibility = Visibility.Visible;
                    break;
                case DbPlatforms.SqlServer:
                    SqlServerLogin.Visibility = Visibility.Visible;
                    break;
                case DbPlatforms.MySql:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void SetFocus(SetFocusControls control)
        {
            switch (control)
            {
                case SetFocusControls.ChurchName:
                    ChurchNameTextBox.Focus();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(control), control, null);
            }

        }

        public bool DoCopyProcedure()
        {
            _procedure = new TwoTierProcedure();
            _procedure.DoProcedure += (sender, args) =>
            {
                args.Result = ViewModel.CopyData(_procedure);
            };
            var result = _procedure.Start();
            return result;
        }

    }
}
