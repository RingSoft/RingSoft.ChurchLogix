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
using RingSoft.DbLookup;
using RingSoft.DbLookup.Controls.WPF;
using RingSoft.DbMaintenance;

namespace RingSoft.ChurchLogix.FinancialManagement
{
    public class BudgetHeaderControl : DbMaintenanceCustomPanel
    {
        public DbMaintenanceButton PostCostButton { get; set; }

        static BudgetHeaderControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BudgetHeaderControl), new FrameworkPropertyMetadata(typeof(BudgetHeaderControl)));
        }

        public BudgetHeaderControl()
        {

        }

        public override void OnApplyTemplate()
        {
            PostCostButton = GetTemplateChild(nameof(PostCostButton)) as DbMaintenanceButton;

            base.OnApplyTemplate();
        }
    }

    /// <summary>
    /// Interaction logic for BudgetMaintenanceWindow.xaml
    /// </summary>
    public partial class BudgetMaintenanceWindow
    {
        public override DbMaintenanceTopHeaderControl DbMaintenanceTopHeaderControl => TopHeaderControl;
        public override string ItemText => "Budget Item";
        public override DbMaintenanceViewModelBase ViewModel => LocalViewModel;
        public override Control MaintenanceButtonsControl => TopHeaderControl;
        public override DbMaintenanceStatusBar DbStatusBar => StatusBar;

        public BudgetMaintenanceWindow()
        {
            InitializeComponent();
            TopHeaderControl.Loaded += (sender, args) =>
            {
                if (TopHeaderControl.CustomPanel is BudgetHeaderControl budgetHeaderControl)
                {
                    budgetHeaderControl.PostCostButton.Command =
                        LocalViewModel.PostCostsCommand;

                    budgetHeaderControl.PostCostButton.ToolTip.HeaderText = "Post Costs (Alt + O)";
                    budgetHeaderControl.PostCostButton.ToolTip.DescriptionText = "Post Costs (Alt + U)";

                    if (!LocalViewModel.TableDefinition.HasRight(RightTypes.AllowEdit))
                    {
                        budgetHeaderControl.PostCostButton.Visibility = Visibility.Collapsed;
                    }

                    budgetHeaderControl.PostCostButton.Command = LocalViewModel.PostCostsCommand;
                }
            };

            RegisterFormKeyControl(NameControl);
        }
    }
}
