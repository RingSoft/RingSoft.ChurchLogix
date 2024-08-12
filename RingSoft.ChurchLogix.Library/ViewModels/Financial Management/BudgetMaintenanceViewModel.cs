using RingSoft.App.Library;
using RingSoft.ChurchLogix.DataAccess;
using RingSoft.ChurchLogix.DataAccess.Model.Financial_Management;
using RingSoft.DataEntryControls.Engine;
using RingSoft.DbLookup;
using RingSoft.DbLookup.AutoFill;
using RingSoft.DbLookup.Lookup;
using RingSoft.DbLookup.QueryBuilder;
using RingSoft.DbMaintenance;

namespace RingSoft.ChurchLogix.Library.ViewModels.Financial_Management
{
    public interface IBudgetView : IDbMaintenanceView
    {
    }
    public class BudgetMaintenanceViewModel : AppDbMaintenanceViewModel<BudgetItem>
    {
        private int _id;

        public int Id
        {
            get { return _id; }
            set
            {
                if (_id == value)
                {
                    return;
                }
                _id = value;
                OnPropertyChanged();
            }
        }

        private AutoFillSetup _fundAutoFillSetup;

        public AutoFillSetup FundAutoFillSetup
        {
            get { return _fundAutoFillSetup; }
            set
            {
                if (_fundAutoFillSetup == value)
                    return;

                _fundAutoFillSetup = value;
                OnPropertyChanged();
            }
        }

        private AutoFillValue _fundAutoFillValue;

        public AutoFillValue FundAutoFillValue
        {
            get { return _fundAutoFillValue; }
            set
            {
                if (_fundAutoFillValue == value)
                    return;

                _fundAutoFillValue = value;
                OnPropertyChanged();
            }
        }

        private double _amount;

        public double Amount
        {
            get { return _amount; }
            set
            {
                if (_amount == value)
                    return;

                _amount = value;
                OnPropertyChanged();
            }
        }


        private string? _notes;

        public string? Notes
        {
            get { return _notes; }
            set
            {
                if (_notes == value)
                    return;

                _notes = value;
                OnPropertyChanged();
            }
        }

        private LookupDefinition<FundHistoryLookup, FundHistory> _fundHistoryLookupDefinition;

        public LookupDefinition<FundHistoryLookup, FundHistory> FundHistoryLookupDefinition
        {
            get { return _fundHistoryLookupDefinition; }
            set
            {
                if (_fundHistoryLookupDefinition == value)
                    return;

                _fundHistoryLookupDefinition = value;
                OnPropertyChanged();
            }
        }

        private LookupDefinition<BudgetPeriodTotalsLookup, BudgetPeriodTotals> _budgetMonthlyTotalsLookupDefinition;

        public LookupDefinition<BudgetPeriodTotalsLookup, BudgetPeriodTotals> BudgetMonthlyTotalsLookupDefinition
        {
            get { return _budgetMonthlyTotalsLookupDefinition; }
            set
            {
                if (_budgetMonthlyTotalsLookupDefinition == value)
                {
                    return;
                }
                _budgetMonthlyTotalsLookupDefinition = value;
                OnPropertyChanged();
            }
        }


        public IBudgetView View { get; private set; }
        public RelayCommand EnterCostsCommand { get; }

        public BudgetMaintenanceViewModel()
        {
            FundAutoFillSetup = new AutoFillSetup(TableDefinition.GetFieldDefinition(p => p.FundId));
            EnterCostsCommand = new RelayCommand(EnterCosts);

            FundHistoryLookupDefinition = AppGlobals.LookupContext.FundHistoryLookup.Clone();
            FundHistoryLookupDefinition.InitialOrderByColumn = FundHistoryLookupDefinition
                .GetColumnDefinition(p => p.Date);
            FundHistoryLookupDefinition.InitialOrderByType = OrderByTypes.Descending;

            BudgetMonthlyTotalsLookupDefinition = AppGlobals.LookupContext.BudgetPeriodLookup.Clone();
            BudgetMonthlyTotalsLookupDefinition.InitialOrderByColumn = BudgetMonthlyTotalsLookupDefinition
                .GetColumnDefinition(p => p.Date);
            BudgetMonthlyTotalsLookupDefinition.InitialOrderByType = OrderByTypes.Descending;
        }

        protected override void Initialize()
        {
            if (base.View is IBudgetView budgetView)
            {
                View = budgetView;
            }
            else
            {
                throw new Exception("Maintenance window must implement IBudgetView");
            }
            base.Initialize();
        }

        protected override void PopulatePrimaryKeyControls(BudgetItem newEntity, PrimaryKeyValue primaryKeyValue)
        {
            Id = newEntity.Id;

            FundHistoryLookupDefinition.FilterDefinition.ClearFixedFilters();
            FundHistoryLookupDefinition.FilterDefinition.AddFixedFilter(
                p => p.BudgetId, Conditions.Equals, newEntity.Id);

            var command = GetLookupCommand(LookupCommands.Refresh, primaryKeyValue);
            FundHistoryLookupDefinition.SetCommand(command);

            BudgetMonthlyTotalsLookupDefinition.FilterDefinition.ClearFixedFilters();
            BudgetMonthlyTotalsLookupDefinition.FilterDefinition.AddFixedFilter(
                p => p.BudgetId, Conditions.Equals, newEntity.Id);
            BudgetMonthlyTotalsLookupDefinition.FilterDefinition.AddFixedFilter(
                p => p.PeriodType
                , Conditions.Equals
                , (int)PeriodTypes.MonthEnding);

            BudgetMonthlyTotalsLookupDefinition.SetCommand(command);
        }

        protected override void LoadFromEntity(BudgetItem entity)
        {
            FundAutoFillValue = entity.Fund.GetAutoFillValue();
            Amount = entity.Amount;
            Notes = entity.Notes;
        }

        protected override BudgetItem GetEntityData()
        {
            var result = new BudgetItem
            {
                Id = Id,
                Name = KeyAutoFillValue.Text,
                FundId = FundAutoFillValue.GetEntity<Fund>().Id,
                Amount = Amount,
                Notes = Notes,
            };

            return result;
        }

        protected override void ClearData()
        {
            Id = 0;
            FundAutoFillValue = null;
            Amount = 0;
            Notes = null;

            var command = GetLookupCommand(LookupCommands.Clear);
            FundHistoryLookupDefinition.SetCommand(command);
            BudgetMonthlyTotalsLookupDefinition.SetCommand(command);
        }

        private void EnterCosts()
        {
            SystemGlobals.TableRegistry.ShowWindow(AppGlobals.LookupContext.BudgetActuals);
        }
    }
}
