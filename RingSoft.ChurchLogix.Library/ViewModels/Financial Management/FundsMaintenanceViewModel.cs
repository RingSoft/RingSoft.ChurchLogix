using RingSoft.App.Library;
using RingSoft.ChurchLogix.DataAccess;
using RingSoft.ChurchLogix.DataAccess.Model.Financial_Management;
using RingSoft.DbLookup;
using RingSoft.DbLookup.Lookup;
using RingSoft.DbLookup.QueryBuilder;

namespace RingSoft.ChurchLogix.Library.ViewModels.Financial_Management
{
    public interface IFundView
    {
        void RefreshView();
    }
    public class FundsMaintenanceViewModel : AppDbMaintenanceViewModel<Fund>
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

        private double _goal;

        public double Goal
        {
            get { return _goal; }
            set
            {
                if (_goal == value)
                {
                    return;
                }
                _goal = value;
                OnPropertyChanged();
                UpdateDiffValues();
            }
        }


        private double _totalCollected;

        public double TotalCollected
        {
            get { return _totalCollected; }
            set
            {
                if (_totalCollected == value)
                    return;
                _totalCollected = value;
                OnPropertyChanged();
                UpdateDiffValues();
            }
        }

        private double _goalDif;

        public double GoalDif
        {
            get { return _goalDif; }
            set
            {
                if (_goalDif == value)
                    return;

                _goalDif = value;
                OnPropertyChanged();
            }
        }

        private double _totalSpent;

        public double TotalSpent
        {
            get { return _totalSpent; }
            set
            {
                if (_totalSpent == value)
                    return;

                _totalSpent = value;
                OnPropertyChanged();
                UpdateDiffValues();
            }
        }

        private double _fundDiff;

        public double FundDiff
        {
            get
            {
                return _fundDiff;
            }
            set
            {
                if (_fundDiff == value)
                    return;

                _fundDiff = value;
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

        private LookupDefinition<FundPeriodLookup, FundPeriodTotals> _fundMonthlyTotalsLookupDefinition;

        public LookupDefinition<FundPeriodLookup, FundPeriodTotals> FundMonthlyTotalsLookupDefinition
        {
            get { return _fundMonthlyTotalsLookupDefinition; }
            set
            {
                if (_fundMonthlyTotalsLookupDefinition == value)
                {
                    return;
                }
                _fundMonthlyTotalsLookupDefinition = value;
                OnPropertyChanged();
            }
        }

        private LookupDefinition<FundPeriodLookup, FundPeriodTotals> _fundYearlyTotalsLookupDefinition;

        public LookupDefinition<FundPeriodLookup, FundPeriodTotals> FundYearlyTotalsLookupDefinition
        {
            get { return _fundYearlyTotalsLookupDefinition; }
            set
            {
                if (_fundYearlyTotalsLookupDefinition == value)
                {
                    return;
                }
                _fundYearlyTotalsLookupDefinition = value;
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


        public IFundView View { get; private set; }

        public FundsMaintenanceViewModel()
        {
            FundHistoryLookupDefinition = AppGlobals.LookupContext.FundHistoryLookup.Clone();
            FundHistoryLookupDefinition.InitialOrderByColumn = FundHistoryLookupDefinition
                .GetColumnDefinition(p => p.Date);
            FundHistoryLookupDefinition.InitialOrderByType = OrderByTypes.Descending;

            FundMonthlyTotalsLookupDefinition = AppGlobals.LookupContext.FundPeriodLookup.Clone();
            FundMonthlyTotalsLookupDefinition.InitialOrderByColumn = FundMonthlyTotalsLookupDefinition
                .GetColumnDefinition(p => p.Date);
            FundMonthlyTotalsLookupDefinition.InitialOrderByType = OrderByTypes.Descending;

            FundYearlyTotalsLookupDefinition = AppGlobals.LookupContext.FundPeriodLookup.Clone();
            FundYearlyTotalsLookupDefinition.InitialOrderByColumn = FundYearlyTotalsLookupDefinition
                .GetColumnDefinition(p => p.Date);
            FundYearlyTotalsLookupDefinition.InitialOrderByType = OrderByTypes.Descending;
        }

        protected override void Initialize()
        {
            if (base.View is IFundView fundView)
            {
                View = fundView;
            }
            else
            {
                throw new Exception($"Maintenance window must implement {nameof(IFundView)} interface.");
            }
            base.Initialize();
        }


        protected override void PopulatePrimaryKeyControls(Fund newEntity, PrimaryKeyValue primaryKeyValue)
        {
            Id = newEntity.Id;

            FundHistoryLookupDefinition.FilterDefinition.ClearFixedFilters();
            FundHistoryLookupDefinition.FilterDefinition.AddFixedFilter(
                p => p.FundId, Conditions.Equals, newEntity.Id);

            var command = GetLookupCommand(LookupCommands.Refresh, primaryKeyValue);
            FundHistoryLookupDefinition.SetCommand(command);

            FundMonthlyTotalsLookupDefinition.FilterDefinition.ClearFixedFilters();
            FundMonthlyTotalsLookupDefinition.FilterDefinition.AddFixedFilter(
                p => p.FundId, Conditions.Equals, newEntity.Id);
            FundMonthlyTotalsLookupDefinition.FilterDefinition.AddFixedFilter(
                p => p.PeriodType
                , Conditions.Equals
                , (int)PeriodTypes.MonthEnding);

            FundMonthlyTotalsLookupDefinition.SetCommand(command);

            FundYearlyTotalsLookupDefinition.FilterDefinition.ClearFixedFilters();
            FundYearlyTotalsLookupDefinition.FilterDefinition.AddFixedFilter(
                p => p.FundId, Conditions.Equals, newEntity.Id);
            FundYearlyTotalsLookupDefinition.FilterDefinition.AddFixedFilter(
                p => p.PeriodType
                , Conditions.Equals
                , (int)PeriodTypes.YearEnding);

            FundYearlyTotalsLookupDefinition.SetCommand(command);

        }

        protected override void LoadFromEntity(Fund entity)
        {
            Goal = entity.Goal;
            TotalCollected = entity.TotalCollected;
            TotalSpent = entity.TotalSpent;
            Notes = entity.Notes;
        }

        protected override Fund GetEntityData()
        {
            var context = SystemGlobals.DataRepository.GetDataContext();
            var table = context.GetTable<Fund>();
            var existFund = table.FirstOrDefault(p => p.Id == Id);

            var result = new Fund
            {
                Id = Id,
                Description = KeyAutoFillValue.Text,
                Goal = Goal,
                Notes = Notes,
            };

            if (existFund != null)
            {
                result.TotalCollected = existFund.TotalCollected;
                result.TotalSpent = existFund.TotalSpent;
            }

            return result;
        }

        protected override void ClearData()
        {
            Id = 0;
            GoalDif = 0;
            FundDiff = 0;
            TotalCollected = 0;
            Goal = 0;
            TotalSpent = 0;
            Notes = null;

            var command = GetLookupCommand(LookupCommands.Clear);
            FundHistoryLookupDefinition.SetCommand(command);
            FundMonthlyTotalsLookupDefinition.SetCommand(command);
            FundYearlyTotalsLookupDefinition.SetCommand(command);
        }

        private void UpdateDiffValues()
        {
            GoalDif = TotalCollected - Goal;
            FundDiff = TotalCollected - TotalSpent;
            View.RefreshView();
        }
    }
}
