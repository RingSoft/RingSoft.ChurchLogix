using RingSoft.App.Library;
using RingSoft.ChurchLogix.DataAccess;
using RingSoft.ChurchLogix.DataAccess.Model.Financial_Management;
using RingSoft.DbLookup;
using RingSoft.DbLookup.AutoFill;
using RingSoft.DbLookup.Lookup;
using RingSoft.DbLookup.QueryBuilder;
using RingSoft.DbMaintenance;

namespace RingSoft.ChurchLogix.Library.ViewModels.Financial_Management
{
    public interface IFundPeriodView : IDbMaintenanceView
    {
        void RefreshView();
    }
    public class FundPeriodTotalsViewModel : AppDbMaintenanceViewModel<FundPeriodTotals>
    {
        private AutoFillSetup _fundAutoFillSetup;

        public AutoFillSetup FundAutoFillSetup
        {
            get { return _fundAutoFillSetup; }
            set
            {
                if (_fundAutoFillSetup == value)
                {
                    return;
                }
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

        private string _periodType;

        public string PeriodType
        {
            get {return _periodType;}
            set
            {
                if (_periodType == value)
                    return;

                _periodType = value;
                OnPropertyChanged();
            }
        }

        private DateTime _dateEnding;

        public DateTime DateEnding
        {
            get { return _dateEnding; }
            set
            {
                if (_dateEnding == value)
                    return;

                _dateEnding = value;
                OnPropertyChanged();
            }
        }

        private double _totalIncome;

        public double TotalIncome
        {
            get { return _totalIncome; }
            set
            {
                if (_totalIncome == value)
                    return;

                _totalIncome = value;
                OnPropertyChanged();
                RefreshDifference();
            }
        }

        private double _totalExpenses;

        public double TotalExpenses
        {
            get { return _totalExpenses; }
            set
            {
                if (_totalExpenses == value)
                    return;

                _totalExpenses = value;
                OnPropertyChanged();
                RefreshDifference();
            }
        }

        private double _difference;

        public double Difference
        {
            get { return _difference; }
            set
            {
                if (_difference == value)
                    return;

                _difference = value;
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

        public IFundPeriodView View { get; private set; }

        public FundPeriodTotalsViewModel()
        {
            FundAutoFillSetup = new AutoFillSetup(
                TableDefinition.GetFieldDefinition(p => p.FundId));

            FundHistoryLookupDefinition = AppGlobals.LookupContext.FundHistoryLookup.Clone();
        }

        protected override void Initialize()
        {
            if (base.View is IFundPeriodView view)
            {
                View = view;
            }
            else
            {
                throw new Exception($"View must implement IFundPeriodView.");
            }

            ReadOnlyMode = true;
            base.Initialize();
        }

        protected override void PopulatePrimaryKeyControls(FundPeriodTotals newEntity, PrimaryKeyValue primaryKeyValue)
        {
            var periodType = (PeriodTypes)newEntity.PeriodType;
            DateEnding = newEntity.Date;
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

            DateTime? beginDate = null;
            DateTime ?endDate = null;

            switch (periodType)
            {
                case PeriodTypes.MonthEnding:
                    beginDate = new DateTime(DateEnding.Year, DateEnding.Month, 1);
                    endDate = beginDate.Value.AddMonths(1).AddDays(-1);
                    break;
                case PeriodTypes.YearEnding:
                    beginDate = new DateTime(DateEnding.Year, 1, 1);
                    endDate = new DateTime(DateEnding.Year, 12, 31);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            FundHistoryLookupDefinition.FilterDefinition.ClearFixedFilters();
            FundHistoryLookupDefinition.FilterDefinition.AddFixedFilter(
                p => p.FundId, Conditions.Equals, newEntity.FundId);
            FundHistoryLookupDefinition.FilterDefinition.AddFixedFilter(
                p => p.Date, Conditions.GreaterThanEquals, beginDate.Value);
            FundHistoryLookupDefinition.FilterDefinition.AddFixedFilter(
                p => p.Date, Conditions.LessThanEquals, endDate.Value);

            var command = GetLookupCommand(LookupCommands.Refresh, primaryKeyValue);
            FundHistoryLookupDefinition.SetCommand(command);
        }

        protected override void LoadFromEntity(FundPeriodTotals entity)
        {
            FundAutoFillValue = entity.Fund.GetAutoFillValue();
            TotalIncome = entity.TotalIncome;
            TotalExpenses = entity.TotalExpenses;
        }

        protected override FundPeriodTotals GetEntityData()
        {
            throw new NotImplementedException();
        }

        protected override void ClearData()
        {
            FundAutoFillValue = null;
            var command = GetLookupCommand(LookupCommands.Clear);
            FundHistoryLookupDefinition.SetCommand(command);

            PeriodType = string.Empty;
            DateEnding = DateTime.Today;
            TotalIncome = TotalExpenses = Difference = 0;
        }

        private void RefreshDifference()
        {
            Difference = TotalIncome - TotalExpenses;
            View.RefreshView();
        }
    }
}
