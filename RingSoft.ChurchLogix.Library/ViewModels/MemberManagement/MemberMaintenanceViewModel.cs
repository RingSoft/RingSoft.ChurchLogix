using RingSoft.ChurchLogix.DataAccess;
using RingSoft.ChurchLogix.DataAccess.Model.ChurchLife;
using RingSoft.ChurchLogix.DataAccess.Model.Financial_Management;
using RingSoft.ChurchLogix.DataAccess.Model.MemberManagement;
using RingSoft.DataEntryControls.Engine;
using RingSoft.DbLookup;
using RingSoft.DbLookup.AutoFill;
using RingSoft.DbLookup.Lookup;
using RingSoft.DbLookup.QueryBuilder;
using RingSoft.DbMaintenance;

namespace RingSoft.ChurchLogix.Library.ViewModels.MemberManagement
{
    public class HouseholdToken
    {

    }

    public interface IMemberView : IDbMaintenanceView
    {
        public void RefreshView();

        public bool SetupRecalcFilter(LookupDefinitionBase lookupDefinition);

        public void StartRecalcProcedure(MemberMaintenanceViewModel viewModel
        , LookupDefinitionBase lookupDefinition);

        public void UpdateProcedure(string headerText
        , int headerTotal
        , int headerIndex
        , string detailText
        , int detailTotal
        , int detailIndex);
    }
    public class MemberMaintenanceViewModel : DbMaintenanceViewModel<Member>
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

        private AutoFillSetup _householdAutoFillSetup;

        public AutoFillSetup HouseholdAutoFillSetup
        {
            get { return _householdAutoFillSetup; }
            set
            {
                if (_householdAutoFillSetup == value)
                {
                    return;
                }
                _householdAutoFillSetup = value;
                OnPropertyChanged(null, false);
            }
        }

        private AutoFillValue _householdAutoFillValue;

        public AutoFillValue HouseholdAutoFillValue
        {
            get { return _householdAutoFillValue; }
            set
            {
                if (_householdAutoFillValue == value)
                {
                    return;
                }
                _householdAutoFillValue = value;
                OnPropertyChanged();
            }
        }

        private string? _phone;

        public string? Phone
        {
            get
            {
                return _phone;
            }
            set
            {
                if (_phone == value)
                {
                    return;
                }
                _phone = value;
                OnPropertyChanged();
            }
        }

        private string? _email;

        public string? Email
        {
            get { return _email; }
            set
            {
                if (_email == value)
                {
                    return;
                }
                _email = value;
                View.RefreshView();
                OnPropertyChanged();
            }
        }

        private LookupDefinition<MemberLookup, Member> _householdMembersLookupDefinition;

        public LookupDefinition<MemberLookup, Member> HouseholdMembersLookupDefinition
        {
            get => _householdMembersLookupDefinition;
            set
            {
                if (_householdMembersLookupDefinition == value)
                    return;

                _householdMembersLookupDefinition = value;
                OnPropertyChanged();
            }
        }

        private LookupDefinition<SmallGroupMemberLookup, SmallGroupMember> _smallGroupLookupDefinition;

        public LookupDefinition<SmallGroupMemberLookup, SmallGroupMember> SmallGroupLookupDefinition
        {
            get { return _smallGroupLookupDefinition; }
            set
            {
                if (_smallGroupLookupDefinition == value)
                    return;

                _smallGroupLookupDefinition = value;
                OnPropertyChanged();
            }
        }

        private LookupDefinition<EventMemberLookup, EventMember> _eventLookupDefinition;

        public LookupDefinition<EventMemberLookup, EventMember> EventLookupDefinition
        {
            get { return _eventLookupDefinition; }
            set
            {
                if (_eventLookupDefinition == value)
                    return;

                _eventLookupDefinition = value;
                OnPropertyChanged();
            }
        }

        private LookupDefinition<MemberGivingHistoryLookup, MemberGivingHistory> _historyLookupDefinition;

        public LookupDefinition<MemberGivingHistoryLookup, MemberGivingHistory> HistoryLookupDefinition
        {
            get { return _historyLookupDefinition; }
            set
            {
                if (_historyLookupDefinition == value)
                    return;

                _historyLookupDefinition = value;
                OnPropertyChanged();
            }
        }

        private LookupDefinition<MemberPeriodGivingLookup, MemberPeriodGiving> _monthlyTotalsLookupDefinition;

        public LookupDefinition<MemberPeriodGivingLookup, MemberPeriodGiving> MonthlyTotalsLookupDefinition
        {
            get { return _monthlyTotalsLookupDefinition; }
            set
            {
                if (_monthlyTotalsLookupDefinition == value)
                {
                    return;
                }
                _monthlyTotalsLookupDefinition = value;
                OnPropertyChanged();
            }
        }

        private LookupDefinition<MemberPeriodGivingLookup, MemberPeriodGiving> _yearlyTotalsLookupDefinition;

        public LookupDefinition<MemberPeriodGivingLookup, MemberPeriodGiving> YearlyTotalsLookupDefinition
        {
            get { return _yearlyTotalsLookupDefinition; }
            set
            {
                if (_yearlyTotalsLookupDefinition == value)
                {
                    return;
                }
                _yearlyTotalsLookupDefinition = value;
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
                {
                    return;
                }
                _notes = value;
                OnPropertyChanged();
            }
        }

        #endregion

        public IMemberView View { get; private set; }

        public RelayCommand AddModifyHouseholdLookupCommand { get; }

        public RelayCommand RecalcCommand { get; }

        public PrimaryKeyValue PrimaryKey { get; private set; }

        public AutoFillValue DefaultHouseHoldAutoFillValue { get; private set; }

        private LookupDefinition<MemberLookup, Member> _householdLookupDefinition;

        public MemberMaintenanceViewModel()
        {
            _householdLookupDefinition = AppGlobals.LookupContext.MemberLookup.Clone();
            HouseholdAutoFillSetup = new AutoFillSetup(_householdLookupDefinition);

            HouseholdMembersLookupDefinition = AppGlobals.LookupContext.MemberLookup.Clone();

            AddModifyHouseholdLookupCommand = new RelayCommand(AddModifyHousehold);

            RecalcCommand = new RelayCommand(DoRecalc);

            HistoryLookupDefinition = AppGlobals.LookupContext.MemberGivingHistoryLookup.Clone();
            HistoryLookupDefinition.InitialOrderByColumn = HistoryLookupDefinition
                .GetColumnDefinition(p => p.Date);
            HistoryLookupDefinition.InitialOrderByType = OrderByTypes.Descending;

            MonthlyTotalsLookupDefinition = AppGlobals.LookupContext.MemberPeriodGivingLookup.Clone();
            MonthlyTotalsLookupDefinition.InitialOrderByColumn = MonthlyTotalsLookupDefinition
                .GetColumnDefinition(p => p.Date);
            MonthlyTotalsLookupDefinition.InitialOrderByType = OrderByTypes.Descending;

            YearlyTotalsLookupDefinition = AppGlobals.LookupContext.MemberPeriodGivingLookup.Clone();
            YearlyTotalsLookupDefinition.InitialOrderByColumn = YearlyTotalsLookupDefinition
                .GetColumnDefinition(p => p.Date);
            YearlyTotalsLookupDefinition.InitialOrderByType = OrderByTypes.Descending;

            SmallGroupLookupDefinition = AppGlobals.LookupContext.SmallGroupMemberLookupDefinition.Clone();
            EventLookupDefinition = AppGlobals.LookupContext.EventMemberLookupDefinition.Clone();

            RegisterLookup(HistoryLookupDefinition);
            RegisterLookup(HouseholdMembersLookupDefinition, new HouseholdToken());
            RegisterLookup(SmallGroupLookupDefinition);
            RegisterLookup(EventLookupDefinition);
        }

        protected override void Initialize()
        {
            View = base.View as IMemberView;
            if (View == null)
                throw new Exception($"Member View interface must be of type '{nameof(IMemberView)}'.");

            if (LookupAddViewArgs != null && LookupAddViewArgs.ParentWindowPrimaryKeyValue != null &&  LookupAddViewArgs.ParentWindowPrimaryKeyValue.TableDefinition ==
                AppGlobals.LookupContext.Members)
            {
                var member =
                    AppGlobals.LookupContext.Members.GetEntityFromPrimaryKeyValue(LookupAddViewArgs
                        .ParentWindowPrimaryKeyValue);

                var context = AppGlobals.DataRepository.GetDataContext();
                var table = context.GetTable<Member>();
                member = table.FirstOrDefault(p => p.Id == member.Id);
                if (LookupAddViewArgs.InputParameter is HouseholdToken)
                {
                    DefaultHouseHoldAutoFillValue = member.GetAutoFillValue();
                }
            }


            base.Initialize();
        }

        protected override void PopulatePrimaryKeyControls(Member newEntity, PrimaryKeyValue primaryKeyValue)
        {
            PrimaryKey = primaryKeyValue;
            Id = newEntity.Id;
            View.RefreshView();
            _householdLookupDefinition.FilterDefinition.ClearFixedFilters();
            _householdLookupDefinition.FilterDefinition.AddFixedFilter(
                p => p.Id
                , Conditions.NotEquals, Id);

            var command = GetLookupCommand(LookupCommands.Refresh, primaryKeyValue);
            MonthlyTotalsLookupDefinition.FilterDefinition.ClearFixedFilters();
            MonthlyTotalsLookupDefinition.FilterDefinition.AddFixedFilter(
                p => p.MemberId, Conditions.Equals, newEntity.Id);
            MonthlyTotalsLookupDefinition.FilterDefinition.AddFixedFilter(
                p => p.PeriodType
                , Conditions.Equals
                , (int)PeriodTypes.MonthEnding);

            MonthlyTotalsLookupDefinition.SetCommand(command);

            YearlyTotalsLookupDefinition.FilterDefinition.ClearFixedFilters();
            YearlyTotalsLookupDefinition.FilterDefinition.AddFixedFilter(
                p => p.MemberId, Conditions.Equals, newEntity.Id);
            YearlyTotalsLookupDefinition.FilterDefinition.AddFixedFilter(
                p => p.PeriodType
                , Conditions.Equals
                , (int)PeriodTypes.YearEnding);

            YearlyTotalsLookupDefinition.SetCommand(command);
        }

        protected override void LoadFromEntity(Member entity)
        {
            HouseholdAutoFillValue = entity.Household.GetAutoFillValue();
            Phone = entity.PhoneNumber;
            Email = entity.Email;
            Notes = entity.Notes;
        }

        protected override Member GetEntityData()
        {
            var result = new Member()
            {
                Id = Id,
                Name = KeyAutoFillValue.Text,
                HouseholdId = HouseholdAutoFillValue.GetEntity<Member>().Id,
                PhoneNumber = Phone,
                Email = Email,
                Notes = Notes
            };

            if (result.HouseholdId == 0)
            {
                result.HouseholdId = null;
            }

            return result;
        }

        protected override void ClearData()
        {
            Id = 0;
            HouseholdAutoFillValue = DefaultHouseHoldAutoFillValue;
            Phone = null;
            Email = null;
            Notes = null;
            View.RefreshView();
            var command = GetLookupCommand(LookupCommands.Clear);
            MonthlyTotalsLookupDefinition.SetCommand(command);
            YearlyTotalsLookupDefinition.SetCommand(command);
            PrimaryKey = null;
        }

        private void AddModifyHousehold()
        {
            if (ExecuteAddModifyCommand() == DbMaintenanceResults.Success)
                HouseholdMembersLookupDefinition.SetCommand(GetLookupCommand(LookupCommands.AddModify
                , null, new HouseholdToken()));
        }

        private void DoRecalc()
        {
            var recalcFilter = ViewLookupDefinition.Clone();
            if (!View.SetupRecalcFilter(recalcFilter))
                return;


            View.StartRecalcProcedure(this, recalcFilter);
            var command = GetLookupCommand(LookupCommands.Refresh, PrimaryKey);
            MonthlyTotalsLookupDefinition.SetCommand(command);
            YearlyTotalsLookupDefinition.SetCommand(command);
        }

        public bool StartRecalc(LookupDefinitionBase recalcFilter)
        {
            var lookupData = TableDefinition.LookupDefinition.GetLookupDataMaui(recalcFilter, false);
            var recordCount = lookupData.GetRecordCount();
            var context = SystemGlobals.DataRepository.GetDataContext();
            var memberIndex = 0;
            lookupData.PrintOutput += (sender, args) =>
            {
                foreach (var primaryKeyValue in args.Result)
                {
                    memberIndex++;
                    var memberPrimaryKey = primaryKeyValue;
                    if (memberPrimaryKey.IsValid())
                    {
                        var member = TableDefinition.GetEntityFromPrimaryKeyValue(memberPrimaryKey);
                        member = member.FillOutProperties(false);

                        var periodTotals = context.GetTable<MemberPeriodGiving>()
                            .Where(p => p.MemberId == member.Id);
                        context.RemoveRange(periodTotals);
                        context.Commit("");

                        var memberGivingHistory = context.GetTable<MemberGivingHistory>();
                        var historyRecs 
                            = memberGivingHistory.Where(p => p.MemberId == member.Id)
                                .OrderBy(p => p.Date);

                        var historyIndex = 0;
                        var historyTotal = historyRecs.Count();
                        foreach (var givingHistory in historyRecs)
                        {
                            historyIndex++;
                            View.UpdateProcedure(
                                $"Processing {member.Name} {memberIndex} / {recordCount}"
                                , recordCount
                                , memberIndex
                                , $"Processing History Record {historyIndex} / {historyTotal}"
                                , historyTotal
                                , historyIndex);
                            PostHistory(context, givingHistory);
                        }
                    }
                }
            };
            lookupData.DoPrintOutput(10);
            var result = context.Commit("");
            return result;
        }

        private void PostHistory(IDbContext context, MemberGivingHistory history)
        {
            var monthEnding = new DateTime(history.Date.Year, history.Date.Month, 1);
            monthEnding = monthEnding.AddMonths(1).AddDays(-1);
            var yearEnding = new DateTime(
                history.Date.Year
                , AppGlobals.SystemPreferences.FiscalYearEnd.Value.Month
                , AppGlobals.SystemPreferences.FiscalYearEnd.Value.Day);

            var periodsTable = context.GetTable<MemberPeriodGiving>();
            var monthRec = periodsTable.FirstOrDefault(p => p.MemberId == history.MemberId
                && p.Date == monthEnding
                            && p.PeriodType ==
                            (int)PeriodTypes.MonthEnding);
            if (monthRec == null)
            {
                monthRec = new MemberPeriodGiving
                {
                    MemberId = history.MemberId,
                    Date = monthEnding,
                    PeriodType = (int)PeriodTypes.MonthEnding,
                    TotalGiving = history.Amount,
                };
                context.AddSaveEntity(monthRec, "");
            }
            else
            {
                monthRec.TotalGiving += history.Amount;
                context.SaveNoCommitEntity(monthRec, "");
            }

            var yearRec = periodsTable
                .FirstOrDefault(p => p.MemberId == history.MemberId && p.Date == yearEnding
                                                            && p.PeriodType == (int)PeriodTypes.YearEnding);
            if (yearRec == null)
            {
                yearRec = new MemberPeriodGiving
                {
                    MemberId = history.MemberId,
                    Date = yearEnding,
                    PeriodType = (int)PeriodTypes.YearEnding,
                    TotalGiving = history.Amount,
                };
                context.AddSaveEntity(yearRec, "");
            }
            else
            {
                yearRec.TotalGiving += history.Amount;
                context.SaveNoCommitEntity(yearRec, "");
            }
        }
    }
}
