using Microsoft.EntityFrameworkCore;
using RingSoft.ChurchLogix.DataAccess.Model.Financial_Management;
using RingSoft.ChurchLogix.DataAccess.Model.MemberManagement;
using RingSoft.ChurchLogix.Library.ViewModels.Financial_Management;
using RingSoft.DataEntryControls.Engine;
using RingSoft.DbLookup;
using RingSoft.DbLookup.AutoFill;
using RingSoft.DbMaintenance;

namespace RingSoft.ChurchLogix.Library.ViewModels.MemberManagement
{
    public interface IMemberGivingView : IDbMaintenanceView
    {
        void ShowPostProcedure(MemberGivingMaintenanceViewModel viewModel);

        void UpdateProcedure(string text);
    }
    public class MemberGivingMaintenanceViewModel : DbMaintenanceViewModel<MemberGiving>
    {
        #region Properties

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

        private AutoFillSetup _memberAutoFillSetup;

        public AutoFillSetup MemberAutoFillSetup
        {
            get { return _memberAutoFillSetup; }
            set
            {
                if (_memberAutoFillSetup == value)
                    return;

                _memberAutoFillSetup = value;
                OnPropertyChanged();
            }
        }

        private AutoFillValue _memberAutoFillValue;

        public AutoFillValue MemberAutoFillValue
        {
            get { return _memberAutoFillValue; }
            set
            {
                if (_memberAutoFillValue == value)
                    return;

                _memberAutoFillValue = value;
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


        private MemberGivingDetailsManager _detailsManager;

        public MemberGivingDetailsManager DetailsManager
        {
            get { return _detailsManager; }
            set
            {
                if (_detailsManager == value)
                    return;

                _detailsManager = value;
                OnPropertyChanged();
            }
        }

        #endregion

        public UiCommand MemberUiCommand { get; }

        public IMemberGivingView View { get; private set; }

        public RelayCommand PostCommand { get; }

        public MemberGivingMaintenanceViewModel()
        {
            MemberAutoFillSetup = new AutoFillSetup(
                TableDefinition.GetFieldDefinition(p => p.MemberId));

            MemberUiCommand = new UiCommand();

            DetailsManager = new MemberGivingDetailsManager(this);
            RegisterGrid(DetailsManager);

            PostCommand = new RelayCommand((() =>
            {
                if (CheckDirty())
                {
                    View.ShowPostProcedure(this);
                }
            }));

        }

        protected override void Initialize()
        {
            if (base.View is IMemberGivingView memberGivingView)
            {
                View = memberGivingView;
            }
            else
            {
                throw new Exception("Invalid view interface");
            }
            base.Initialize();
        }

        protected override void PopulatePrimaryKeyControls(MemberGiving newEntity, PrimaryKeyValue primaryKeyValue)
        {
            Id = newEntity.Id;
        }

        protected override void LoadFromEntity(MemberGiving entity)
        {
            MemberAutoFillValue = entity.Member.GetAutoFillValue();
            Date = entity.Date;
        }

        protected override MemberGiving GetEntityData()
        {
            var result = new MemberGiving()
            {
                Id = Id,
                MemberId = MemberAutoFillValue.GetEntity<Member>().Id,
                Date = Date,
            };

            return result;
        }

        protected override void ClearData()
        {
            Id = 0;
            MemberAutoFillValue = null;
            Date = DateTime.Today;
            MemberUiCommand.SetFocus();
            Total = 0;
        }

        public bool PostGiving()
        {
            var context = SystemGlobals.DataRepository.GetDataContext();
            if (context != null)
            {
                var fundPeriodTotalsTable = context.GetTable<FundPeriodTotals>();
                var memberPeriodTotalsTable = context.GetTable<MemberPeriodGiving>();
                var givingTable = context.GetTable<MemberGiving>()
                    .Include(p => p.Details);
                var fundTable = context.GetTable<Fund>();
                var totalRecords = givingTable.Count();
                var index = 0;
                var memberGivingsToDelete = new List<MemberGiving>();
                var fundTotals = new List<FundTotal>();

                foreach (var memberGiving in givingTable)
                {
                    index++;
                    //memberGiving.FillOutEntity();
                    memberGiving.UtFillOutEntity();
                    View.UpdateProcedure($"Processing Member Giving {index} / {totalRecords}");

                    foreach (var memberGivingDetail in memberGiving.Details)
                    {
                        var fundHistoryRec = new FundHistory()
                        {
                            Amount = memberGivingDetail.Amount,
                            AmountType = (int)HistoryAmountTypes.Income,
                            Date = memberGiving.Date,
                            FundId = memberGivingDetail.FundId,
                        };

                        if (!context.SaveNoCommitEntity(fundHistoryRec, "Saving Fund History"))
                        {
                            return false;
                        }

                        var memberHistoryRec = new MemberGivingHistory()
                        {
                            MemberId = memberGiving.MemberId,
                            Date = memberGiving.Date,
                            FundId = memberGivingDetail.FundId,
                            Amount = memberGivingDetail.Amount,
                        };

                        if (!context.SaveNoCommitEntity(memberHistoryRec, "Saving Member History"))
                        {
                            return false;
                        }

                        var endDayMonth = new DateTime(memberGiving.Date.Year, memberGiving.Date.Month, 1);
                        endDayMonth = endDayMonth.AddMonths(1);
                        endDayMonth = endDayMonth.AddDays(-1);

                        var endYear = AppGlobals.GetYearEndDate(memberGiving.Date);

                        if (!SaveFundPeriodMonth(fundPeriodTotalsTable, endDayMonth, memberGivingDetail, context))
                            return false;

                        if (!SaveFundPeriodYear(fundPeriodTotalsTable, endYear, memberGivingDetail, context)) return false;

                        if (!SaveMemberPeriodMonth(memberPeriodTotalsTable, endDayMonth, memberGivingDetail, context))
                            return false;

                        if (!SaveMemberPeriodYear(memberPeriodTotalsTable, endYear, memberGivingDetail, context)) return false;

                        if (!SaveFund(fundTable, memberGivingDetail, context, fundTotals))
                        {
                            return false;
                        }
                    }

                    memberGiving.Member = null;
                    memberGivingsToDelete.Add(memberGiving);
                }

                foreach (var fundTotal in fundTotals)
                {
                    var fund = fundTable.FirstOrDefault(p => p.Id == fundTotal.FundId);
                    if (fund != null)
                    {
                        fund.TotalCollected += fundTotal.Total;
                        if (!context.SaveNoCommitEntity(fund, "Saving Fund"))
                        {
                            return false;
                        }
                    }
                }
                
                var detailsToDelete = new List<MemberGivingDetails>();

                foreach (var memberGiving in memberGivingsToDelete)
                {
                    foreach (var memberGivingDetail in memberGiving.Details)
                    {
                        memberGivingDetail.Fund = null;
                        memberGivingDetail.MemberGiving = null;
                        detailsToDelete.Add(memberGivingDetail);
                    }
                }
                context.RemoveRange(detailsToDelete);
                context.RemoveRange(memberGivingsToDelete);
                var result = context.Commit("Committing Data");

                if (result)
                {
                    if (AppGlobals.MainViewModel.MainView != null) 
                        AppGlobals.MainViewModel.MainView.RefreshChart(false);
                    NewCommand.Execute(null);
                    return true;
                }
            }
            return false;
        }

        private static bool SaveFundPeriodYear(
            IQueryable<FundPeriodTotals> fundPeriodTotalsTable
            , DateTime endYear
            , MemberGivingDetails memberGivingDetail,
            IDbContext context)
        {
            var fundYearRec = fundPeriodTotalsTable
                .FirstOrDefault(p => p.Date == endYear
                                     && p.PeriodType == (int)PeriodTypes.YearEnding);

            if (fundYearRec == null)
            {
                fundYearRec = new FundPeriodTotals()
                {
                    FundId = memberGivingDetail.FundId,
                    PeriodType = (int)PeriodTypes.YearEnding,
                    Date = endYear,
                    TotalIncome = memberGivingDetail.Amount,
                };
                if (!context.AddSaveEntity(fundYearRec, "Saving Fund Year Ending"))
                {
                    return false;
                }
            }
            else
            {
                fundYearRec.TotalIncome += memberGivingDetail.Amount;
                if (!context.SaveNoCommitEntity(fundYearRec, "Saving Fund Year Ending"))
                {
                    return false;
                }
            }

            return true;
        }


        private static bool SaveFundPeriodMonth(IQueryable<FundPeriodTotals> fundPeriodTotalsTable, DateTime endDayMonth,
            MemberGivingDetails memberGivingDetail, IDbContext context)
        {
            var fundMonthRec = fundPeriodTotalsTable
                .FirstOrDefault(p => p.Date == endDayMonth
                                     && p.PeriodType == (int)PeriodTypes.MonthEnding);

            if (fundMonthRec == null)
            {
                fundMonthRec = new FundPeriodTotals()
                {
                    FundId = memberGivingDetail.FundId,
                    PeriodType = (int)PeriodTypes.MonthEnding,
                    Date = endDayMonth,
                    TotalIncome = memberGivingDetail.Amount,
                };
                if (!context.AddSaveEntity(fundMonthRec, "Saving Fund Month Ending"))
                {
                    return false;
                }
            }
            else
            {
                fundMonthRec.TotalIncome += memberGivingDetail.Amount;
                if (!context.SaveNoCommitEntity(fundMonthRec, "Saving Fund Month Ending"))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool SaveFund(IQueryable<Fund> fundTable,
            MemberGivingDetails memberGivingDetail, IDbContext context, List<FundTotal> fundTotals)
        {
            var fund = fundTable
                .FirstOrDefault(p => p.Id == memberGivingDetail.FundId);

            var fundTotal = fundTotals.FirstOrDefault(p => p.FundId == fund.Id);
            if (fundTotal == null)
            {
                fundTotal = new FundTotal()
                {
                    FundId = fund.Id,
                };
                fundTotals.Add(fundTotal);
            }

            fundTotal.Total += memberGivingDetail.Amount;
            return true;
        }

        private static bool SaveMemberPeriodMonth(IQueryable<MemberPeriodGiving> memberPeriodTotalsTable,
            DateTime endDayMonth,
            MemberGivingDetails memberGivingDetail, IDbContext context)
        {
            var memberMonthRec = memberPeriodTotalsTable
                .FirstOrDefault(p => p.MemberId == memberGivingDetail.MemberGiving.MemberId
                    && p.Date == endDayMonth
                                     && p.PeriodType == (int)PeriodTypes.MonthEnding);

            if (memberMonthRec == null)
            {
                int memberId = memberGivingDetail.MemberGiving.MemberId;
                memberMonthRec = new MemberPeriodGiving()
                {
                    MemberId = memberId,
                    PeriodType = (int)PeriodTypes.MonthEnding,
                    Date = endDayMonth,
                    TotalGiving = memberGivingDetail.Amount,
                };
                if (!context.AddSaveEntity(memberMonthRec, "Saving Member Month Ending"))
                {
                    return false;
                }
            }
            else
            {
                memberMonthRec.TotalGiving += memberGivingDetail.Amount;
                if (!context.SaveNoCommitEntity(memberMonthRec, "Saving Member Month Ending"))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool SaveMemberPeriodYear(IQueryable<MemberPeriodGiving> memberPeriodTotalsTable, DateTime endYear,
            MemberGivingDetails memberGivingDetail, IDbContext context)
        {
            var memberYearRec = memberPeriodTotalsTable
                .FirstOrDefault(p => p.MemberId == memberGivingDetail.MemberGiving.MemberId
            && p.Date == endYear
                                     && p.PeriodType == (int)PeriodTypes.YearEnding);

            if (memberYearRec == null)
            {
                memberYearRec = new MemberPeriodGiving()
                {
                    MemberId = memberGivingDetail.MemberGiving.MemberId,
                    PeriodType = (int)PeriodTypes.YearEnding,
                    Date = endYear,
                    TotalGiving = memberGivingDetail.Amount,
                };
                if (!context.AddSaveEntity(memberYearRec, "Saving Member Year Ending"))
                {
                    return false;
                }
            }
            else
            {
                memberYearRec.TotalGiving += memberGivingDetail.Amount;
                if (!context.SaveNoCommitEntity(memberYearRec, "Saving Member Year Ending"))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
