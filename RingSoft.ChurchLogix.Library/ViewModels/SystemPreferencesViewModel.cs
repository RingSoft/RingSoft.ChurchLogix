using RingSoft.App.Library;
using RingSoft.ChurchLogix.DataAccess.Model;
using RingSoft.ChurchLogix.DataAccess.Model.Financial_Management;
using RingSoft.ChurchLogix.DataAccess.Model.MemberManagement;
using RingSoft.DataEntryControls.Engine;
using RingSoft.DbLookup;
using RingSoft.DbMaintenance;

namespace RingSoft.ChurchLogix.Library.ViewModels
{
    public interface ISystemPreferencesView : IDbMaintenanceView
    {
        void CloseWindow();

        void ShowPostProcedure(SystemPreferencesViewModel viewModel);

        void UpdateProcedure(string text);
    }
    public class SystemPreferencesViewModel : AppDbMaintenanceViewModel<SystemPreferences>
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

        private DateTime _fiscalYearEnd;

        public DateTime FiscalYearEnd
        {
            get { return _fiscalYearEnd; }
            set
            {
                if (_fiscalYearEnd == value)
                    return;

                _fiscalYearEnd = value;
                OnPropertyChanged();
            }
        }

        public DateTime OriginalFYEnd { get; private set; }

        public RelayCommand OkCommand { get; }

        public RelayCommand CancelCommand { get; }

        public ISystemPreferencesView View { get; private set; }

        private bool _qResult;

        public SystemPreferencesViewModel()
        {
            OkCommand = new RelayCommand(OnOk);

            CancelCommand = new RelayCommand(OnCancel);
        }

        protected override void Initialize()
        {
            if (base.View is ISystemPreferencesView systemPreferencesView)
            {
                View = systemPreferencesView;
            }
            base.Initialize();
            OnGotoNextButton();
            NextCommand.IsEnabled = false;
            PreviousCommand.IsEnabled = false;
        }

        protected override void PopulatePrimaryKeyControls(SystemPreferences newEntity, PrimaryKeyValue primaryKeyValue)
        {
            Id = newEntity.Id;
        }

        protected override void LoadFromEntity(SystemPreferences entity)
        {
            FiscalYearEnd = entity.FiscalYearEnd.Value;
            OriginalFYEnd = FiscalYearEnd;
        }

        protected override SystemPreferences GetEntityData()
        {
            var result = new SystemPreferences()
            {
                Id = Id,
                FiscalYearEnd = FiscalYearEnd,
            };

            return result;
        }

        protected override void ClearData()
        {
        }

        protected override bool SaveEntity(SystemPreferences entity)
        {
            CheckConvert();
            if (_qResult)
            {
                View.ShowPostProcedure(this);
            }
            AppGlobals.SystemPreferences = entity;
            return base.SaveEntity(entity);
        }

        private async void CheckConvert()
        {
            _qResult = false;
            var message =
                $"Do you wish to rebuild Period Totals tables with the new fiscal year of {FiscalYearEnd.ToShortDateString()}?";
            var caption = "Rebuild For New Fiscal Year?";
            var result = await ControlsGlobals.UserInterface.ShowYesNoMessageBox(message, caption);
            switch (result)
            {
                case MessageBoxButtonsResult.Yes:
                    _qResult = true;
                    break;
                case MessageBoxButtonsResult.No:
                case MessageBoxButtonsResult.Cancel:
                    _qResult = false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public bool ChangeFY()
        {
            var context = SystemGlobals.DataRepository.GetDataContext();
            var needsRecalculation = false;
            var fundPeriodTable = context.GetTable<FundPeriodTotals>();
            var fundPeriodRecs = fundPeriodTable
                .Where(p => p.PeriodType == (int)PeriodTypes.YearEnding
                && p.Date == FiscalYearEnd);
            var budgetPeriodTable = context.GetTable<BudgetPeriodTotals>();
            var budgetPeriodRecs = budgetPeriodTable
                .Where(p => p.PeriodType == (int)PeriodTypes.YearEnding
                            && p.Date == FiscalYearEnd);
            var memberPeriodTable = context.GetTable<MemberPeriodGiving>();
            var memberPeriodRecs = memberPeriodTable
                .Where(p => p.PeriodType == (int)PeriodTypes.YearEnding
                            && p.Date == FiscalYearEnd);

            if (!fundPeriodRecs.Any()
                || !budgetPeriodRecs.Any()
                || !memberPeriodRecs.Any())
            {
                var beginDate = FiscalYearEnd.AddYears(-1)
                    .AddDays(1);
                var endDate = FiscalYearEnd;

                var existFundPeriods = fundPeriodTable.Where(
                    p => p.Date >= OriginalFYEnd
                         && p.PeriodType == (int)PeriodTypes.YearEnding);

                var existBudgetPeriods = budgetPeriodTable.Where(
                    p => p.Date == OriginalFYEnd
                         && p.PeriodType >= (int)PeriodTypes.YearEnding);

                var existMemberPeriods = memberPeriodTable.Where(
                    p => p.Date == OriginalFYEnd
                         && p.PeriodType >= (int)PeriodTypes.YearEnding);

                context.RemoveRange(existFundPeriods);
                context.RemoveRange(existBudgetPeriods);
                context.RemoveRange(existMemberPeriods);

                var historyFound = true;
                while (historyFound)
                {
                    var fundHistory = context.GetTable<FundHistory>()
                        .Where(p => p.Date >= beginDate
                                    && p.Date <= endDate);
                    var memberHistory = context.GetTable<MemberGivingHistory>()
                        .Where(p => p.Date >= beginDate
                                    && p.Date <= endDate);

                    if (!fundHistory.Any() && !memberHistory.Any() && endDate >= DateTime.Today)
                    {
                        historyFound = false;
                    }
                    else
                    {
                        var index = 0;
                        var total = fundHistory.Count();
                        foreach (var history in fundHistory)
                        {
                            index++;
                            View.UpdateProcedure($"Processing Fund History Record {index} / {total}");
                            PostFundHistory(context, history, endDate);
                        }

                        index = 0;
                        total = memberHistory.Count();
                        foreach (var memberGivingHistory in memberHistory)
                        {
                            index++;
                            View.UpdateProcedure($"Processing Member Giving History Record {index} / {total}");
                            PostMemberGivingHistory(context, memberGivingHistory, endDate);
                        }

                        beginDate = beginDate.AddYears(1);
                        endDate = endDate.AddYears(1);
                    }
                }
            }

            return context.Commit("");
        }
        private async void OnOk()
        {
            if (DoSave() == DbMaintenanceResults.Success)
            {
                View.CloseWindow();
            }

        }

        private void PostFundHistory(IDbContext context, FundHistory fundHistory, DateTime fyEndDate)
        {
            var totalIncome = 0.0;
            var totalExpenses = 0.0;
            switch ((HistoryAmountTypes)fundHistory.AmountType)
            {
                case HistoryAmountTypes.Income:
                    totalIncome = fundHistory.Amount;
                    break;
                case HistoryAmountTypes.Expense:
                    totalExpenses = fundHistory.Amount;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            if (fundHistory.FundId != null)
            {
                var fundPeriodTotals = context.GetTable<FundPeriodTotals>();
                var fundYearlyTotals = fundPeriodTotals.FirstOrDefault(
                    p => p.FundId == fundHistory.FundId
                         && p.PeriodType == (int)PeriodTypes.YearEnding
                         && p.Date == fyEndDate);

                if (fundYearlyTotals == null)
                {
                    fundYearlyTotals = new FundPeriodTotals
                    {
                        FundId = fundHistory.FundId.GetValueOrDefault(),
                        PeriodType = (int)PeriodTypes.YearEnding,
                        Date = fyEndDate,
                        TotalIncome = totalIncome,
                        TotalExpenses = totalExpenses,
                    };
                    context.AddSaveEntity(fundYearlyTotals, "");
                }
                else
                {
                    fundYearlyTotals.TotalIncome += totalIncome;
                    fundYearlyTotals.TotalExpenses += totalExpenses;
                    context.SaveNoCommitEntity(fundYearlyTotals, "");
                }
            }

            if (fundHistory.BudgetId != null)
            {
                var budgetPeriodTotals = context.GetTable<BudgetPeriodTotals>();
                var budgetYearlyTotals = budgetPeriodTotals.FirstOrDefault(
                    p => p.BudgetId == fundHistory.BudgetId
                         && p.PeriodType == (int)PeriodTypes.YearEnding
                         && p.Date == fyEndDate);

                if (budgetYearlyTotals == null)
                {
                    budgetYearlyTotals = new BudgetPeriodTotals()
                    {
                        BudgetId = fundHistory.BudgetId.GetValueOrDefault(),
                        PeriodType = (int)PeriodTypes.YearEnding,
                        Date = fyEndDate,
                        Total = fundHistory.Amount,
                    };
                    context.AddSaveEntity(budgetYearlyTotals, "");
                }
                else
                {
                    budgetYearlyTotals.Total += fundHistory.Amount;
                    context.SaveNoCommitEntity(budgetYearlyTotals, "");
                }

            }
        }

        private void PostMemberGivingHistory(IDbContext context, MemberGivingHistory memberGivingHistory
        , DateTime fyEndDate)
        {
            var memberPeriodTotals = context.GetTable<MemberPeriodGiving>();
            var memberYearlyTotals = memberPeriodTotals.FirstOrDefault(
                p => p.MemberId == memberGivingHistory.MemberId
                     && p.PeriodType == (int)PeriodTypes.YearEnding
                     && p.Date == fyEndDate);

            if (memberYearlyTotals == null)
            {
                memberYearlyTotals = new MemberPeriodGiving()
                {
                    MemberId = memberGivingHistory.MemberId,
                    PeriodType = (int)PeriodTypes.YearEnding,
                    Date = fyEndDate,
                    TotalGiving = memberGivingHistory.Amount,
                };
                context.AddSaveEntity(memberYearlyTotals, "");
            }
            else
            {
                memberYearlyTotals.TotalGiving += memberGivingHistory.Amount;
                context.SaveNoCommitEntity(memberYearlyTotals, "");
            }
        }

        private void OnCancel()
        {
            View.CloseWindow();
        }
    }
}
