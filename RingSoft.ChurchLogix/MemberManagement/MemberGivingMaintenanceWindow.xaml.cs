using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using RingSoft.App.Controls;
using RingSoft.App.Library;
using RingSoft.ChurchLogix.Library.ViewModels.Financial_Management;
using RingSoft.ChurchLogix.Library.ViewModels.MemberManagement;
using RingSoft.DbLookup.Controls.WPF;
using RingSoft.DbMaintenance;

namespace RingSoft.ChurchLogix.MemberManagement
{


    /// <summary>
    /// Interaction logic for MemberGivingMaintenanceWindow.xaml
    /// </summary>
    public partial class MemberGivingMaintenanceWindow : IMemberGivingView
    {
        public override DbMaintenanceViewModelBase ViewModel => LocalViewModel;
        public override Control MaintenanceButtonsControl => TopHeaderControl;
        public override DbMaintenanceStatusBar DbStatusBar => StatusBar;
        public override DbMaintenanceTopHeaderControl DbMaintenanceTopHeaderControl => TopHeaderControl;
        public override string ItemText => "Member Giving";

        private PostProcedure _procedure;


        public MemberGivingMaintenanceWindow()
        {
            InitializeComponent();
            TopHeaderControl.Loaded += (sender, args) =>
            {
                if (TopHeaderControl.CustomPanel is MemberGivingHeaderControl memberGivingHeaderControl)
                {
                    memberGivingHeaderControl.PostGivingButton.Command =
                        LocalViewModel.PostCommand;

                    memberGivingHeaderControl.PostGivingButton.ToolTip.HeaderText = "Post Giving (Alt + O)";
                    memberGivingHeaderControl.PostGivingButton.ToolTip.DescriptionText = "Post Giving (Alt + O)";
                }
            };
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
