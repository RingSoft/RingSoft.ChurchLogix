using RingSoft.App.Library;
using RingSoft.ChurchLogix.DataAccess;
using RingSoft.ChurchLogix.DataAccess.Model.Financial_Management;
using RingSoft.DataEntryControls.Engine;
using RingSoft.DbLookup;
using RingSoft.DbLookup.Lookup;
using RingSoft.DbLookup.QueryBuilder;

namespace RingSoft.ChurchLogix.Library.ViewModels.Financial_Management
{
    public interface IFundView
    {
        void RefreshView();

        bool SetupRecalcFilter(LookupDefinitionBase lookupDefinition);

        void StartRecalcProcedure(FundsMaintenanceViewModel viewModel
            , LookupDefinitionBase lookupDefinition);

        void UpdateProcedure(string headerText
            , int headerTotal
            , int headerIndex
            , string detailText
            , int detailTotal
            , int detailIndex);

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

        public RelayCommand RecalcCommand { get; }

        public PrimaryKeyValue PrimaryKey { get; private set; }

        public FundsMaintenanceViewModel()
        {
            FundHistoryLookupDefinition = AppGlobals.LookupContext.FundHistoryLookup.Clone();
            FundHistoryLookupDefinition.InitialOrderByColumn = FundHistoryLookupDefinition
                .GetColumnDefinition(p => p.Date);
            FundHistoryLookupDefinition.InitialOrderByType = OrderByTypes.Descending;
            RegisterLookup(FundHistoryLookupDefinition);

            FundMonthlyTotalsLookupDefinition = AppGlobals.LookupContext.FundPeriodLookup.Clone();
            FundMonthlyTotalsLookupDefinition.InitialOrderByColumn = FundMonthlyTotalsLookupDefinition
                .GetColumnDefinition(p => p.Date);
            FundMonthlyTotalsLookupDefinition.InitialOrderByType = OrderByTypes.Descending;

            FundYearlyTotalsLookupDefinition = AppGlobals.LookupContext.FundPeriodLookup.Clone();
            FundYearlyTotalsLookupDefinition.InitialOrderByColumn = FundYearlyTotalsLookupDefinition
                .GetColumnDefinition(p => p.Date);
            FundYearlyTotalsLookupDefinition.InitialOrderByType = OrderByTypes.Descending;

            RecalcCommand = new RelayCommand(DoRecalc);
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

        //protected override Fund GetEntityFromDb(Fund newEntity, PrimaryKeyValue primaryKeyValue)
        //{
        //    var context = SystemGlobals.DataRepository.GetDataContext();
        //    var table = context.GetTable<Fund>();
        //    return table.FirstOrDefault(p => p.Id == newEntity.Id);
        //}

        protected override void PopulatePrimaryKeyControls(Fund newEntity, PrimaryKeyValue primaryKeyValue)
        {
            PrimaryKey = primaryKeyValue;
            Id = newEntity.Id;

            var command = GetLookupCommand(LookupCommands.Refresh, primaryKeyValue);

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
            FundMonthlyTotalsLookupDefinition.SetCommand(command);
            FundYearlyTotalsLookupDefinition.SetCommand(command);
            PrimaryKey = null;
        }

        private void UpdateDiffValues()
        {
            GoalDif = TotalCollected - Goal;
            FundDiff = TotalCollected - TotalSpent;
            View.RefreshView();
        }

        private void DoRecalc()
        {
            var recalcFilter = ViewLookupDefinition.Clone();
            if (!View.SetupRecalcFilter(recalcFilter))
                return;


            View.StartRecalcProcedure(this, recalcFilter);
            var command = GetLookupCommand(LookupCommands.Refresh, PrimaryKey);
            FundMonthlyTotalsLookupDefinition.SetCommand(command);
            FundYearlyTotalsLookupDefinition.SetCommand(command);
        }

        public bool StartRecalc(LookupDefinitionBase recalcFilter)
        {
            var lookupData = TableDefinition.LookupDefinition.GetLookupDataMaui(recalcFilter, false);
            var recordCount = lookupData.GetRecordCount();
            var context = SystemGlobals.DataRepository.GetDataContext();
            var fundIndex = 0;
            lookupData.PrintOutput += (sender, args) =>
            {
                foreach (var primaryKeyValue in args.Result)
                {
                    fundIndex++;
                    var fundPrimaryKey = primaryKeyValue;
                    if (fundPrimaryKey.IsValid())
                    {
                        var fund = TableDefinition.GetEntityFromPrimaryKeyValue(fundPrimaryKey);
                        fund = fund.FillOutProperties(false);
                        var periodTotals = context.GetTable<FundPeriodTotals>()
                            .Where(p => p.FundId == fund.Id);
                        context.RemoveRange(periodTotals);
                        context.Commit("");

                        var fundHistory = context.GetTable<FundHistory>();
                        var historyRecs
                            = fundHistory.Where(p => p.FundId == fund.Id)
                                .OrderBy(p => p.Date);

                        var historyIndex = 0;
                        var historyTotal = historyRecs.Count();
                        foreach (var historyRec in historyRecs)
                        {
                            historyIndex++;
                            View.UpdateProcedure(
                                $"Processing {fund.Description} {fundIndex} / {recordCount}"
                                , recordCount
                                , fundIndex
                                , $"Processing History Record {historyIndex} / {historyTotal}"
                                , historyTotal
                                , historyIndex);
                            PostPeriodTotals(historyRec, context);

                        }

                    }
                }
            };
            lookupData.DoPrintOutput(10);
            var result = context.Commit("");
            return result;
        }

        private void PostPeriodTotals(FundHistory history, IDbContext context)
        {
            var monthEnding = new DateTime(history.Date.Year, history.Date.Month, 1);
            monthEnding = monthEnding.AddMonths(1).AddDays(-1);
            var yearEnding = new DateTime(
                history.Date.Year
                , AppGlobals.SystemPreferences.FiscalYearEnd.Value.Month
                , AppGlobals.SystemPreferences.FiscalYearEnd.Value.Day);

            var periodsTable = context.GetTable<FundPeriodTotals>();
            var monthRec = periodsTable.FirstOrDefault(p => p.FundId == history.FundId
                                                            && p.Date == monthEnding
                                                            && p.PeriodType ==
                                                            (int)PeriodTypes.MonthEnding);

            var totalGiving = 0.0;
            var totalExpenses = 0.0;
            switch ((HistoryAmountTypes)history.AmountType)
            {
                case HistoryAmountTypes.Income:
                    totalGiving = history.Amount;
                    break;
                case HistoryAmountTypes.Expense:
                    totalExpenses = history.Amount;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            if (monthRec == null)
            {
                monthRec = new FundPeriodTotals()
                {
                    FundId = history.FundId.GetValueOrDefault(),
                    Date = monthEnding,
                    PeriodType = (int)PeriodTypes.MonthEnding,
                    TotalIncome = totalGiving,
                    TotalExpenses = totalExpenses
                };
                context.AddSaveEntity(monthRec, "");
            }
            else
            {
                monthRec.TotalIncome += totalGiving;
                monthRec.TotalExpenses += totalExpenses;
                context.SaveNoCommitEntity(monthRec, "");
            }
            var yearRec = periodsTable.FirstOrDefault(p => p.FundId == history.FundId
                                                            && p.Date == yearEnding
                                                            && p.PeriodType ==
                                                            (int)PeriodTypes.YearEnding);
            if (yearRec == null)
            {
                yearRec = new FundPeriodTotals()
                {
                    FundId = history.FundId.GetValueOrDefault(),
                    Date = yearEnding,
                    PeriodType = (int)PeriodTypes.YearEnding,
                    TotalIncome = totalGiving,
                    TotalExpenses = totalExpenses
                };
                context.AddSaveEntity(yearRec, "");
            }
            else
            {
                yearRec.TotalIncome += totalGiving;
                yearRec.TotalExpenses += totalExpenses;
                context.SaveNoCommitEntity(yearRec, "");
            }

        }
    }
}
