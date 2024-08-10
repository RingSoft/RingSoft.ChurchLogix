using RingSoft.App.Library;
using RingSoft.ChurchLogix.DataAccess;
using RingSoft.ChurchLogix.DataAccess.Model.Financial_Management;
using RingSoft.DbLookup;
using RingSoft.DbLookup.AutoFill;
using RingSoft.DbLookup.Lookup;
using RingSoft.DbLookup.QueryBuilder;

namespace RingSoft.ChurchLogix.Library.ViewModels.Financial_Management
{
    public class BudgetPeriodTotalsViewModel : AppDbMaintenanceViewModel<BudgetPeriodTotals>
    {
        private AutoFillSetup _budgetAutoFillSetup;

        public AutoFillSetup BudgetAutoFillSetup
        {
            get { return _budgetAutoFillSetup; }
            set
            {
                if (_budgetAutoFillSetup == value)
                {
                    return;
                }
                _budgetAutoFillSetup = value;
                OnPropertyChanged();
            }
        }

        private AutoFillValue _budgetAutoFillValue;

        public AutoFillValue BudgetAutoFillValue
        {
            get { return _budgetAutoFillValue; }
            set
            {
                if (_budgetAutoFillValue == value)
                    return;

                _budgetAutoFillValue = value;
                OnPropertyChanged();
            }
        }

        private string _periodType;

        public string PeriodType
        {
            get { return _periodType; }
            set
            {
                if (_periodType == value)
                    return;

                _periodType = value;
                OnPropertyChanged();
            }
        }

        private DateTime _date;

        public DateTime Date
        {
            get { return _date; }
            set
            {
                if (_date == value)
                    return;

                _date = value;
                OnPropertyChanged();
            }
        }

        private double _total;

        public double Total
        {
            get { return _total; }
            set
            {
                if (_total == value)
                    return;

                _total = value;
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

        public BudgetPeriodTotalsViewModel()
        {
            BudgetAutoFillSetup = new AutoFillSetup(
                TableDefinition.GetFieldDefinition(p => p.BudgetId));

            FundHistoryLookupDefinition = AppGlobals.LookupContext.FundHistoryLookup.Clone();
        }

        protected override void Initialize()
        {
            if (Processor is IAppDbMaintenanceProcessor appDbMaintenanceProcessor)
            {
                appDbMaintenanceProcessor.WindowReadOnlyMode = true;
            }

            base.Initialize();
        }

        protected override void PopulatePrimaryKeyControls(BudgetPeriodTotals newEntity, PrimaryKeyValue primaryKeyValue)
        {
            var periodType = (PeriodTypes)newEntity.PeriodType;
            var enumTranslation =
                TableDefinition
                    .GetFieldDefinition(p => p.PeriodType)
                    .EnumTranslation;

            if (enumTranslation != null)
            {
                var enumText = enumTranslation
                    .TypeTranslations
                    .FirstOrDefault(p => p.NumericValue == newEntity.PeriodType)
                    .TextValue;

                PeriodType = enumText;
            }

            Date = newEntity.Date;

            DateTime? beginDate = null;
            DateTime? endDate = null;

            switch (periodType)
            {
                case PeriodTypes.MonthEnding:
                    beginDate = new DateTime(Date.Year, Date.Month, 1);
                    endDate = beginDate.Value.AddMonths(1).AddDays(-1);
                    break;
                case PeriodTypes.YearEnding:
                    beginDate = new DateTime(Date.Year, 1, 1);
                    endDate = new DateTime(Date.Year, 12, 31);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            FundHistoryLookupDefinition.FilterDefinition.ClearFixedFilters();
            FundHistoryLookupDefinition.FilterDefinition.AddFixedFilter(
                p => p.BudgetId, Conditions.Equals, newEntity.BudgetId);
            FundHistoryLookupDefinition.FilterDefinition.AddFixedFilter(
                p => p.Date, Conditions.GreaterThanEquals, beginDate.Value);
            FundHistoryLookupDefinition.FilterDefinition.AddFixedFilter(
                p => p.Date, Conditions.LessThanEquals, endDate.Value);

            var command = GetLookupCommand(LookupCommands.Refresh, primaryKeyValue);
            FundHistoryLookupDefinition.SetCommand(command);
        }

        protected override void LoadFromEntity(BudgetPeriodTotals entity)
        {
            BudgetAutoFillValue = entity.BudgetItem.GetAutoFillValue();
            Total = entity.Total;
        }

        protected override BudgetPeriodTotals GetEntityData()
        {
            throw new NotImplementedException();
        }

        protected override void ClearData()
        {
            BudgetAutoFillValue = null;
            PeriodType = string.Empty;
            Date = DateTime.Today;
            Total = 0;

            var command = GetLookupCommand(LookupCommands.Clear);
            FundHistoryLookupDefinition.SetCommand(command);
        }
    }
}
