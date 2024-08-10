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
using RingSoft.DbLookup;
using RingSoft.DbLookup.Controls.WPF;
using RingSoft.DbMaintenance;

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
    /// Interaction logic for BudgetActualMaintenanceWindow.xaml
    /// </summary>
    public partial class BudgetActualMaintenanceWindow : IBudgetActualView
    {
        public override DbMaintenanceTopHeaderControl DbMaintenanceTopHeaderControl => TopHeaderControl;
        public override string ItemText => "Budget Cost";
        public override DbMaintenanceViewModelBase ViewModel => LocalViewModel;
        public override Control MaintenanceButtonsControl => TopHeaderControl;
        public override DbMaintenanceStatusBar DbStatusBar => StatusBar;

        private PostProcedure _procedure;
        public BudgetActualMaintenanceWindow()
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
