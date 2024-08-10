﻿using System;
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
