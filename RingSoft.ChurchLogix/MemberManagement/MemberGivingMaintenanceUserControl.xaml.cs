using RingSoft.App.Controls;
using RingSoft.App.Library;
using RingSoft.ChurchLogix.Library.ViewModels.MemberManagement;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using RingSoft.DataEntryControls.WPF;
using RingSoft.DbLookup.Controls.WPF;
using RingSoft.DbMaintenance;

namespace RingSoft.ChurchLogix.MemberManagement
{
    public class PostProcedure : AppProcedure
    {
        private ProcessingSplashWindow _splashWindow;

        public override ISplashWindow SplashWindow => _splashWindow;

        private MemberGivingMaintenanceViewModel _memberGivingMaintenanceViewModel;
        public PostProcedure(MemberGivingMaintenanceViewModel memberGivingMaintenanceViewModel)
        {
            _memberGivingMaintenanceViewModel = memberGivingMaintenanceViewModel;
        }

        protected override void ShowSplash()
        {
            _splashWindow = new ProcessingSplashWindow("Posting Member Giving");
            _splashWindow.ShowInTaskbar = false;
            _splashWindow.ShowDialog();
        }

        protected override bool DoProcess()
        {
            var result = _memberGivingMaintenanceViewModel.PostGiving();
            _splashWindow.CloseSplash();
            return result;
        }
    }
    public class MemberGivingHeaderControl : DbMaintenanceCustomPanel
    {
        public DbMaintenanceButton PostGivingButton { get; set; }

        static MemberGivingHeaderControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MemberGivingHeaderControl), new FrameworkPropertyMetadata(typeof(MemberGivingHeaderControl)));
        }

        public MemberGivingHeaderControl()
        {

        }

        public override void OnApplyTemplate()
        {
            PostGivingButton = GetTemplateChild(nameof(PostGivingButton)) as DbMaintenanceButton;

            base.OnApplyTemplate();
        }
    }
    /// <summary>
    /// Interaction logic for MemberGivingMaintenanceUserControl.xaml
    /// </summary>
    public partial class MemberGivingMaintenanceUserControl : IMemberGivingView
    {
        private PostProcedure _procedure;

        public MemberGivingMaintenanceUserControl()
        {
            InitializeComponent();

            TopHeaderControl.Loaded += (sender, args) =>
            {
                if (TopHeaderControl.CustomPanel is MemberGivingHeaderControl memberGivingHeaderControl)
                {
                    memberGivingHeaderControl.PostGivingButton.Command =
                        LocalViewModel.PostCommand;

                    memberGivingHeaderControl.PostGivingButton.ToolTip.HeaderText = "Post Giving (Ctrl + E, Ctrl + O)";
                    memberGivingHeaderControl.PostGivingButton.ToolTip.DescriptionText = "Post Giving";
                }
            };

            var hotKey = new HotKey(LocalViewModel.PostCommand);
            hotKey.AddKey(Key.E);
            hotKey.AddKey(Key.O);
            AddHotKey(hotKey);
        }

        protected override DbMaintenanceViewModelBase OnGetViewModel()
        {
            return LocalViewModel;
        }

        protected override Control OnGetMaintenanceButtons()
        {
            return TopHeaderControl;
        }

        protected override DbMaintenanceStatusBar OnGetStatusBar()
        {
            return StatusBar;
        }

        protected override string GetTitle()
        {
            return "Member Giving";
        }

        public void ShowPostProcedure(MemberGivingMaintenanceViewModel viewModel)
        {
            _procedure = new PostProcedure(viewModel);
            _procedure.Start();
        }

        public void UpdateProcedure(string text)
        {
            _procedure.SplashWindow.SetProgress(text);
        }
    }
}
