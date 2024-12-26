﻿using RingSoft.App.Controls;
using RingSoft.ChurchLogix.Library.ViewModels.Financial_Management;
using System.Windows;
using System.Windows.Controls;
using RingSoft.DbLookup.Controls.WPF;
using RingSoft.DbMaintenance;
using RingSoft.DbLookup;

namespace RingSoft.ChurchLogix.FinancialManagement
{
    public class BudgetHeaderControl : DbMaintenanceCustomPanel
    {
        public DbMaintenanceButton EditCostButton { get; set; }

        static BudgetHeaderControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BudgetHeaderControl), new FrameworkPropertyMetadata(typeof(BudgetHeaderControl)));
        }

        public BudgetHeaderControl()
        {

        }

        public override void OnApplyTemplate()
        {
            EditCostButton = GetTemplateChild(nameof(EditCostButton)) as DbMaintenanceButton;

            base.OnApplyTemplate();
        }
    }
    /// <summary>
    /// Interaction logic for BudgetMaintenanceUserControl.xaml
    /// </summary>
    public partial class BudgetMaintenanceUserControl : IBudgetView
    {
        public BudgetMaintenanceUserControl()
        {
            InitializeComponent();
            TopHeaderControl.Loaded += (sender, args) =>
            {
                if (TopHeaderControl.CustomPanel is BudgetHeaderControl budgetHeaderControl)
                {
                    budgetHeaderControl.EditCostButton.Command =
                        LocalViewModel.EnterCostsCommand;

                    budgetHeaderControl.EditCostButton.ToolTip.HeaderText = "Enter Costs (Alt + E)";
                    budgetHeaderControl.EditCostButton.ToolTip.DescriptionText = "Enter Costs (Alt + E)";

                    if (!LocalViewModel.TableDefinition.HasRight(RightTypes.AllowEdit))
                    {
                        budgetHeaderControl.EditCostButton.Visibility = Visibility.Collapsed;
                    }

                    budgetHeaderControl.EditCostButton.Command = LocalViewModel.EnterCostsCommand;
                }
            };

            RegisterFormKeyControl(NameControl);
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
            return "Budget Item";
        }
    }
}
