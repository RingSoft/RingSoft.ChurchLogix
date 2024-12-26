using RingSoft.App.Controls;
using RingSoft.ChurchLogix.Library.ViewModels.MemberManagement;
using RingSoft.DataEntryControls.WPF;
using RingSoft.DbLookup.Lookup;
using System.Windows;
using System.Windows.Controls;
using RingSoft.DbLookup.Controls.WPF;
using RingSoft.DbMaintenance;
using RingSoft.ChurchLogix.DataAccess.Model.MemberManagement;
using RingSoft.DbLookup;

namespace RingSoft.ChurchLogix.MemberManagement
{

    public class RecalcProcedure : TwoTierProcessingProcedure
    {
        private MemberMaintenanceViewModel _viewModel;
        private LookupDefinitionBase _lookupDefinition;
        public RecalcProcedure(Window ownerWindow
            , MemberMaintenanceViewModel memberMaintenanceViewModel
            , LookupDefinitionBase lookupDefinition) : base(ownerWindow, "Recalculating Member(s)")
        {
            _viewModel = memberMaintenanceViewModel;
            _lookupDefinition = lookupDefinition;
        }

        public override bool DoProcedure()
        {
            var result = _viewModel.StartRecalc(_lookupDefinition);
            return result;
        }
    }
    public class MemberHeaderControl : DbMaintenanceCustomPanel
    {
        public DbMaintenanceButton RecalculateButton { get; set; }

        static MemberHeaderControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MemberHeaderControl)
                , new FrameworkPropertyMetadata(typeof(MemberHeaderControl)));
        }

        public MemberHeaderControl()
        {

        }

        public override void OnApplyTemplate()
        {
            RecalculateButton = GetTemplateChild(nameof(RecalculateButton)) as DbMaintenanceButton;

            base.OnApplyTemplate();
        }
    }
    /// <summary>
    /// Interaction logic for MemberMaintenanceUserControl.xaml
    /// </summary>
    public partial class MemberMaintenanceUserControl : IMemberView
    {
        private RecalcProcedure _procedure;

        public MemberMaintenanceUserControl()
        {
            InitializeComponent();

            TopHeaderControl.Loaded += (sender, args) =>
            {
                if (TopHeaderControl.CustomPanel is MemberHeaderControl memberHeaderControl)
                {
                    memberHeaderControl.RecalculateButton.Command =
                        LocalViewModel.RecalcCommand;

                    memberHeaderControl.RecalculateButton.ToolTip.HeaderText = "Recalculate Member Totals (Alt + R)";
                    memberHeaderControl.RecalculateButton.ToolTip.DescriptionText =
                        "Recalculate Member Totals (Alt + R)";

                    if (!LocalViewModel.TableDefinition.HasSpecialRight((int)MemberSpecialRights.AllowViewGiving))
                    {
                        memberHeaderControl.RecalculateButton.Visibility = Visibility.Collapsed;
                    }
                }
            };

            RegisterFormKeyControl(NameControl);

            Loaded += (sender, args) =>
            {
                if (!LocalViewModel.TableDefinition.HasSpecialRight((int)MemberSpecialRights.AllowViewGiving))
                {
                    HistoryTab.Visibility = Visibility.Collapsed;
                    MonthlyTab.Visibility = Visibility.Collapsed;
                    YearlyTab.Visibility = Visibility.Collapsed;
                }
            };
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
            return "Member";
        }

        public void RefreshView()
        {
            MainWindow.ProcessSendEmailLink(SendEmailControl, LocalViewModel.Email);
        }

        public bool SetupRecalcFilter(LookupDefinitionBase lookupDefinition)
        {
            var genericInput = new GenericReportLookupFilterInput
            {
                LookupDefinitionToFilter = lookupDefinition,
                CodeNameToFilter = "Member",
                KeyAutoFillValue = LocalViewModel.KeyAutoFillValue,
                ProcessText = "Recalculate"
            };
            var genericWindow = new GenericReportFilterWindow(genericInput);
            genericWindow.Owner = OwnerWindow;
            genericWindow.ShowInTaskbar = false;
            genericWindow.ShowDialog();
            return genericWindow.ViewModel.DialogReesult;
        }

        public void StartRecalcProcedure(MemberMaintenanceViewModel viewModel
            , LookupDefinitionBase lookupDefinition)
        {
            _procedure = new RecalcProcedure(OwnerWindow, viewModel, lookupDefinition);
            _procedure.Start();

        }

        public void UpdateProcedure(string headerText
            , int headerTotal
            , int headerIndex
            , string detailText
            , int detailTotal
            , int detailIndex)
        {
            _procedure.SetProgress(headerTotal
                , headerIndex
                , headerText
                , detailTotal
                , detailIndex
                , detailText);
        }
    }
}
