using RingSoft.App.Controls;
using RingSoft.ChurchLogix.DataAccess.Model.MemberManagement;
using RingSoft.ChurchLogix.Library.ViewModels.Financial_Management;
using RingSoft.ChurchLogix.Library.ViewModels.MemberManagement;
using RingSoft.ChurchLogix.MemberManagement;
using RingSoft.DataEntryControls.WPF;
using RingSoft.DbLookup.Controls.WPF;
using RingSoft.DbLookup.Lookup;
using RingSoft.DbMaintenance;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using RingSoft.DbLookup;

namespace RingSoft.ChurchLogix.FinancialManagement
{
    public class RecalcProcedure : TwoTierProcessingProcedure
    {
        private FundsMaintenanceViewModel _viewModel;
        private LookupDefinitionBase _lookupDefinition;
        public RecalcProcedure(Window ownerWindow
            , FundsMaintenanceViewModel fundMaintenanceViewModel
            , LookupDefinitionBase lookupDefinition) : base(ownerWindow, "Recalculating Fund(s)")
        {
            _viewModel = fundMaintenanceViewModel;
            _lookupDefinition = lookupDefinition;
        }

        public override bool DoProcedure()
        {
            var result = _viewModel.StartRecalc(_lookupDefinition);
            return result;
        }
    }
    public class FundHeaderControl : DbMaintenanceCustomPanel
    {
        public DbMaintenanceButton RecalculateButton { get; set; }

        static FundHeaderControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FundHeaderControl)
                , new FrameworkPropertyMetadata(typeof(FundHeaderControl)));
        }

        public FundHeaderControl()
        {

        }

        public override void OnApplyTemplate()
        {
            RecalculateButton = GetTemplateChild(nameof(RecalculateButton)) as DbMaintenanceButton;

            base.OnApplyTemplate();
        }
    }

    /// <summary>
    /// Interaction logic for FundMaintenanceWindow.xaml
    /// </summary>
    public partial class FundMaintenanceWindow : IFundView
    {
        public override DbMaintenanceTopHeaderControl DbMaintenanceTopHeaderControl => TopHeaderControl;
        public override string ItemText => "Fund";
        public override DbMaintenanceViewModelBase ViewModel => LocalViewModel;
        public override Control MaintenanceButtonsControl => TopHeaderControl;
        public override DbMaintenanceStatusBar DbStatusBar => StatusBar;

        private RecalcProcedure _procedure;
        public FundMaintenanceWindow()
        {
            InitializeComponent();
            RegisterFormKeyControl(NameControl);

            TopHeaderControl.Loaded += (sender, args) =>
            {
                if (TopHeaderControl.CustomPanel is FundHeaderControl fundHeaderControl)
                {
                    fundHeaderControl.RecalculateButton.Command =
                        LocalViewModel.RecalcCommand;

                    fundHeaderControl.RecalculateButton.ToolTip.HeaderText = "Recalculate Fund Totals (Alt + R)";
                    fundHeaderControl.RecalculateButton.ToolTip.DescriptionText =
                        "Recalculate Fund Totals (Alt + R)";
                }
            };

        }

        public void RefreshView()
        {
            if (LocalViewModel.GoalDif > 0)
            {
                GoalDifReadOnlyBox.Foreground = Brushes.LightGreen;
            }
            else if (LocalViewModel.GoalDif < 0)
            {
                GoalDifReadOnlyBox.Foreground = Brushes.LightPink;
            }
            else if (LocalViewModel.GoalDif == 0)
            {
                GoalDifReadOnlyBox.Foreground = Brushes.Black;
            }

            if (LocalViewModel.FundDiff > 0)
            {
                FundDifReadOnlyBox.Foreground = Brushes.LightGreen;
            }
            else if (LocalViewModel.FundDiff < 0)
            {
                FundDifReadOnlyBox.Foreground = Brushes.LightPink;
            }
            else if (LocalViewModel.FundDiff == 0)
            {
                FundDifReadOnlyBox.Foreground = Brushes.Black;
            }
        }

        public bool SetupRecalcFilter(LookupDefinitionBase lookupDefinition)
        {
            var genericInput = new GenericReportLookupFilterInput
            {
                LookupDefinitionToFilter = lookupDefinition,
                CodeNameToFilter = "Fund",
                KeyAutoFillValue = LocalViewModel.KeyAutoFillValue,
                ProcessText = "Recalculate"
            };
            var genericWindow = new GenericReportFilterWindow(genericInput);
            genericWindow.Owner = this;
            genericWindow.ShowInTaskbar = false;
            genericWindow.ShowDialog();
            return genericWindow.ViewModel.DialogReesult;
        }

        public void StartRecalcProcedure(FundsMaintenanceViewModel viewModel
            , LookupDefinitionBase lookupDefinition)
        {
            _procedure = new RecalcProcedure(this, viewModel, lookupDefinition);
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
