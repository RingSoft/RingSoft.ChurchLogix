using RingSoft.App.Library;
using RingSoft.ChurchLogix.DataAccess.Model.Financial_Management;
using RingSoft.DataEntryControls.Engine;
using RingSoft.DbLookup;
using RingSoft.DbLookup.AutoFill;

namespace RingSoft.ChurchLogix.Library.ViewModels.Financial_Management
{
    public interface IBudgetActualView
    {
        void ShowPostProcedure(BudgetActualMaintenanceViewModel viewModel);

        void UpdateProcedure(string text);
    }
    public class BudgetActualMaintenanceViewModel : AppDbMaintenanceViewModel<BudgetActual>
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

        private AutoFillSetup _budgetAutoFillSetup;

        public AutoFillSetup BudgetAutoFillSetup
        {
            get { return _budgetAutoFillSetup; }
            set
            {
                if (_budgetAutoFillSetup == value)
                    return;

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

        public UiCommand BudgetAutoFillCommand { get; }

        public RelayCommand PostCostsCommand { get; }

        public IBudgetActualView View { get; private set; }


        private bool? _valSave;

        public BudgetActualMaintenanceViewModel()
        {
            BudgetAutoFillSetup = new AutoFillSetup
                (TableDefinition.GetFieldDefinition(p => p.BudgetId));

            BudgetAutoFillCommand = new UiCommand();
            PostCostsCommand = new RelayCommand((() =>
            {
                if (CheckDirty())
                {
                    View.ShowPostProcedure(this);
                }
            }));
        }

        protected override void Initialize()
        {
            if (base.View is IBudgetActualView view)
            {
                View = view;
            }
            else
            {
                throw new Exception($"Must implement {nameof(IBudgetActualView)}");
            }
            base.Initialize();
        }

        protected override void PopulatePrimaryKeyControls(BudgetActual newEntity, PrimaryKeyValue primaryKeyValue)
        {
            Id = newEntity.Id;
        }

        protected override void LoadFromEntity(BudgetActual entity)
        {
            BudgetAutoFillValue = entity.Budget.GetAutoFillValue();
            Date = entity.Date;
            Amount = entity.Amount;
        }

        protected override BudgetActual GetEntityData()
        {
            return new BudgetActual
            {
                Id = Id,
                BudgetId = BudgetAutoFillValue.GetEntity<BudgetItem>().Id,
                Date = Date,
                Amount = Amount,
            };
        }

        protected override bool ValidateEntity(BudgetActual entity)
        {
            if (entity.Id == 0)
            {
                CheckSave(entity);
                if (_valSave != null)
                {
                    var result = _valSave.Value;
                    _valSave = null;
                    return result;
                }
            }

            return base.ValidateEntity(entity);
        }

        private async void CheckSave(BudgetActual entity)
        {
            if (_valSave == null)
            {
                var context = SystemGlobals.DataRepository.GetDataContext();
                var actualTable = context.GetTable<BudgetActual>();
                var actualRecord = actualTable.FirstOrDefault(
                    p => p.BudgetId == entity.BudgetId
                    && p.Date == entity.Date
                    && p.Amount.Equals(entity.Amount));

                if (actualRecord != null)
                {
                    var message =
                        "There is a record in this table that matches this data.  Are you sure you wish to save?";
                    var caption = "Confirm Save";
                    if (await ControlsGlobals.UserInterface.ShowYesNoMessageBox(message, caption) ==
                        MessageBoxButtonsResult.Yes)
                    {
                        _valSave = true;
                    }
                    else
                    {
                        _valSave = false;
                    }
                }

                if (_valSave == null)
                {
                    var fundHistoryTable = context.GetTable<FundHistory>();
                    var fundHistoryRecord = fundHistoryTable.FirstOrDefault(
                        p => p.BudgetId == entity.BudgetId
                             && p.Date == entity.Date
                             && p.Amount.Equals(entity.Amount));

                    if (fundHistoryRecord != null)
                    {
                        var message =
                            "There is a record in the Fund History table that matches this data.  Are you sure you wish to save?";
                        var caption = "Confirm Save";
                        if (await ControlsGlobals.UserInterface.ShowYesNoMessageBox(message, caption) ==
                            MessageBoxButtonsResult.Yes)
                        {
                            _valSave = true;
                        }
                        else
                        {
                            _valSave = false;
                        }
                    }

                }
            }
        }
        protected override void ClearData()
        {
            Id = 0;
            BudgetAutoFillValue = null;
            Date = DateTime.Today;
            Amount = 0;

            BudgetAutoFillCommand.SetFocus();
        }

        public bool PostCosts()
        {
            var context = SystemGlobals.DataRepository.GetDataContext();
            if (context != null)
            {
                var fundPeriodTotalsTable = context.GetTable<FundPeriodTotals>();
                var budgetPeriodTotalsTable = context.GetTable<BudgetPeriodTotals>();
                var actualTable = context.GetTable<BudgetActual>();
                var totalRecords = actualTable.Count();
                var index = 0;
                var budgetsToDelete = new List<BudgetActual>();
                foreach (var budgetActual in actualTable)
                {
                    index++;
                    budgetActual.FillOutEntity();
                    View.UpdateProcedure($"Processing Budget Cost {index} / {totalRecords}");

                    var fundHistoryRec = new FundHistory()
                    {
                        Amount = budgetActual.Amount,
                        AmountType = (int)HistoryAmountTypes.Expense,
                        BudgetId = budgetActual.BudgetId,
                        Date = budgetActual.Date,
                        FundId = budgetActual.Budget.FundId,
                    };

                    if (!context.SaveNoCommitEntity(fundHistoryRec, "Saving Fund History"))
                    {
                        return false;
                    }

                    var endDayMonth = new DateTime(budgetActual.Date.Year, budgetActual.Date.Month, 1);
                    endDayMonth = endDayMonth.AddMonths(1);
                    endDayMonth = endDayMonth.AddDays(-1);

                    var endYear = new DateTime(budgetActual.Date.Year, 12, 31);

                    if (!SaveFundPeriodMonth(fundPeriodTotalsTable, endDayMonth, budgetActual, context)) 
                        return false;

                    if (!SaveFundPeriodYear(fundPeriodTotalsTable, endYear, budgetActual, context)) return false;

                    if (!SaveBudgetPeriodMonth(budgetPeriodTotalsTable, endDayMonth, budgetActual, context))
                        return false;

                    if (!SaveBudgetPeriodYear(budgetPeriodTotalsTable, endYear, budgetActual, context)) return false;

                    budgetActual.Budget = null;
                    budgetsToDelete.Add(budgetActual);
                }

                context.RemoveRange(budgetsToDelete);
                var result = context.Commit("Committing Data");

                if (result)
                {
                    NewCommand.Execute(null);
                    return true;
                }
            }

            return false;
        }

        private static bool SaveFundPeriodYear(IQueryable<FundPeriodTotals> fundPeriodTotalsTable, DateTime endYear, BudgetActual budgetActual,
            IDbContext context)
        {
            var fundYearRec = fundPeriodTotalsTable
                .FirstOrDefault(p => p.Date == endYear
                                     && p.PeriodType == (int)PeriodTypes.YearEnding);

            if (fundYearRec == null)
            {
                fundYearRec = new FundPeriodTotals()
                {
                    FundId = budgetActual.Budget.FundId,
                    PeriodType = (int)PeriodTypes.YearEnding,
                    Date = endYear,
                    TotalExpenses = budgetActual.Amount,
                };
                if (!context.AddSaveEntity(fundYearRec, "Saving Fund Year Ending"))
                {
                    return false;
                }
            }
            else
            {
                fundYearRec.TotalExpenses += budgetActual.Amount;
                if (!context.SaveNoCommitEntity(fundYearRec, "Saving Fund Year Ending"))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool SaveFundPeriodMonth(IQueryable<FundPeriodTotals> fundPeriodTotalsTable, DateTime endDayMonth,
            BudgetActual budgetActual, IDbContext context)
        {
            var fundMonthRec = fundPeriodTotalsTable
                .FirstOrDefault(p => p.Date == endDayMonth
                                     && p.PeriodType == (int)PeriodTypes.MonthEnding);

            if (fundMonthRec == null)
            {
                fundMonthRec = new FundPeriodTotals()
                {
                    FundId = budgetActual.Budget.FundId,
                    PeriodType = (int)PeriodTypes.MonthEnding,
                    Date = endDayMonth,
                    TotalExpenses = budgetActual.Amount,
                };
                if (!context.AddSaveEntity(fundMonthRec, "Saving Fund Month Ending"))
                {
                    return false;
                }
            }
            else
            {
                fundMonthRec.TotalExpenses += budgetActual.Amount;
                if (!context.SaveNoCommitEntity(fundMonthRec, "Saving Fund Month Ending"))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool SaveBudgetPeriodMonth(IQueryable<BudgetPeriodTotals> budgetPeriodTotalsTable, DateTime endDayMonth,
            BudgetActual budgetActual, IDbContext context)
        {
            var budgetMonthRec = budgetPeriodTotalsTable
                .FirstOrDefault(p => p.Date == endDayMonth
                                     && p.PeriodType == (int)PeriodTypes.MonthEnding);

            if (budgetMonthRec == null)
            {
                budgetMonthRec = new BudgetPeriodTotals()
                {
                    BudgetId = budgetActual.BudgetId,
                    PeriodType = (int)PeriodTypes.MonthEnding,
                    Date = endDayMonth,
                    Total = budgetActual.Amount,
                };
                if (!context.AddSaveEntity(budgetMonthRec, "Saving Budget Month Ending"))
                {
                    return false;
                }
            }
            else
            {
                budgetMonthRec.Total += budgetActual.Amount;
                if (!context.SaveNoCommitEntity(budgetMonthRec, "Saving Budget Month Ending"))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool SaveBudgetPeriodYear(IQueryable<BudgetPeriodTotals> budgetPeriodTotalsTable, DateTime endYear,
            BudgetActual budgetActual, IDbContext context)
        {
            var budgetYearRec = budgetPeriodTotalsTable
                .FirstOrDefault(p => p.Date == endYear
                                     && p.PeriodType == (int)PeriodTypes.YearEnding);

            if (budgetYearRec == null)
            {
                budgetYearRec = new BudgetPeriodTotals()
                {
                    BudgetId = budgetActual.BudgetId,
                    PeriodType = (int)PeriodTypes.YearEnding,
                    Date = endYear,
                    Total = budgetActual.Amount,
                };
                if (!context.AddSaveEntity(budgetYearRec, "Saving Budget Year Ending"))
                {
                    return false;
                }
            }
            else
            {
                budgetYearRec.Total += budgetActual.Amount;
                if (!context.SaveNoCommitEntity(budgetYearRec, "Saving Budget Year Ending"))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
