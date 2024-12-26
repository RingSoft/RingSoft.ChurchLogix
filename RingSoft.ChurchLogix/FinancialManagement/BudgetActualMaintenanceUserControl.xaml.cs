using RingSoft.App.Controls;
using RingSoft.App.Library;
using RingSoft.ChurchLogix.Library.ViewModels.Financial_Management;
using System.Windows;
using System.Windows.Controls;
using RingSoft.DbLookup.Controls.WPF;
using RingSoft.DbMaintenance;
using RingSoft.DbLookup;

namespace RingSoft.ChurchLogix.FinancialManagement
{
    public class PostProcedure : AppProcedure
    {
        private ProcessingSplashWindow _splashWindow;

        public override ISplashWindow SplashWindow => _splashWindow;

        private BudgetActualMaintenanceViewModel _actualMaintenanceViewModel;
        public PostProcedure(BudgetActualMaintenanceViewModel actualMaintenanceViewModel)
        {
            _actualMaintenanceViewModel = actualMaintenanceViewModel;
        }

        protected override void ShowSplash()
        {
            _splashWindow = new ProcessingSplashWindow("Posting Budget Costs to History");
            _splashWindow.ShowInTaskbar = false;
            _splashWindow.ShowDialog();
        }

        protected override bool DoProcess()
        {
            var result = _actualMaintenanceViewModel.PostCosts();
            _splashWindow.CloseSplash();
            return result;
        }
    }

    public class BudgetActualHeaderControl : DbMaintenanceCustomPanel
    {
        public DbMaintenanceButton PostCostButton { get; set; }

        static BudgetActualHeaderControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BudgetActualHeaderControl), new FrameworkPropertyMetadata(typeof(BudgetActualHeaderControl)));
        }

        public BudgetActualHeaderControl()
        {

        }

        public override void OnApplyTemplate()
        {
            PostCostButton = GetTemplateChild(nameof(PostCostButton)) as DbMaintenanceButton;

            base.OnApplyTemplate();
        }
    }

    /// <summary>
    /// Interaction logic for BudgetActualMaintenanceUserControl.xaml
    /// </summary>
    public partial class BudgetActualMaintenanceUserControl : IBudgetActualView
    {
        private PostProcedure _procedure;
        public BudgetActualMaintenanceUserControl()
        {
            InitializeComponent();
            TopHeaderControl.Loaded += (sender, args) =>
            {
                if (TopHeaderControl.CustomPanel is BudgetActualHeaderControl budgetHeaderControl)
                {
                    budgetHeaderControl.PostCostButton.Command =
                        LocalViewModel.PostCostsCommand;

                    budgetHeaderControl.PostCostButton.ToolTip.HeaderText = "Post Costs (Alt + O)";
                    budgetHeaderControl.PostCostButton.ToolTip.DescriptionText = "Post Costs (Alt + O)";

                    if (!LocalViewModel.TableDefinition.HasRight(RightTypes.AllowEdit))
                    {
                        budgetHeaderControl.PostCostButton.Visibility = Visibility.Collapsed;
                    }

                    budgetHeaderControl.PostCostButton.Command = LocalViewModel.PostCostsCommand;
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
            return "Budget Cost";
        }

        public void ShowPostProcedure(BudgetActualMaintenanceViewModel viewModel)
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
