using Microsoft.EntityFrameworkCore;
using RingSoft.App.Library;
using RingSoft.ChurchLogix.DataAccess.Model;
using RingSoft.ChurchLogix.DataAccess.Model.ChurchLife;
using RingSoft.ChurchLogix.DataAccess.Model.Financial_Management;
using RingSoft.ChurchLogix.DataAccess.Model.MemberManagement;
using RingSoft.ChurchLogix.DataAccess.Model.StaffManagement;
using RingSoft.DataEntryControls.Engine;
using RingSoft.DbLookup;
using RingSoft.DbLookup.AutoFill;
using RingSoft.DbLookup.DataProcessor;
using RingSoft.DbLookup.EfCore;
using RingSoft.DbLookup.Lookup;
using RingSoft.DbLookup.ModelDefinition;
using RingSoft.DbLookup.ModelDefinition.FieldDefinitions;
using RingSoft.DbLookup.QueryBuilder;

namespace RingSoft.ChurchLogix.DataAccess
{
    public class ChurchLogixLookupContext : LookupContext
    {
        public override DbDataProcessor DataProcessor => _dbDataProcessor;
        protected override DbContext DbContext => _dbContext;
        public SqliteDataProcessor SqliteDataProcessor { get; }
        public SqlServerDataProcessor SqlServerDataProcessor { get; }

        public TableDefinition<SystemMaster> SystemMaster { get; set; }
        public TableDefinition<SystemPreferences> SystemPreferences { get; set; }
        public TableDefinition<StaffPerson> Staff { get; set; }
        public TableDefinition<Group> Groups { get; set; }
        public TableDefinition<StaffGroup> StaffGroups { get; set; }

        public TableDefinition<Member> Members { get; set; }
        public TableDefinition<MemberGivingHistory> MembersGivingHistory { get; set; }
        public TableDefinition<MemberPeriodGiving> MembersPeriodGiving { get; set; }
        public TableDefinition<MemberGiving> MembersGiving { get; set; }
        public TableDefinition<MemberGivingDetails> MembersGivingDetails { get; set; }

        public TableDefinition<Fund> Funds { get; set; }
        public TableDefinition<BudgetItem> Budgets { get; set; }
        public TableDefinition<FundHistory> FundHistory { get; set; }
        public TableDefinition<FundPeriodTotals> FundPeriodTotals { get; set; }
        public TableDefinition< BudgetPeriodTotals> BudgetPeriodTotals { get; set; }
        public TableDefinition<BudgetActual> BudgetActuals { get; set; }
        
        public TableDefinition<Event> Events { get; set; }
        public TableDefinition<EventMember> EventsMember { get; set; }
        public TableDefinition<Role> Roles { get; set; }

        public LookupDefinition<SystemPreferencesLookup, SystemPreferences> SystemPreferencesLookup { get; set; }
        public LookupDefinition<StaffLookup, StaffPerson> StaffLookup { get; set; }
        public LookupDefinition<GroupsLookup, Group> GroupsLookup { get; set; }
        public LookupDefinition<StaffGroupsLookup, StaffGroup> StaffGroupsLookup { get; set;}

        public LookupDefinition<MemberLookup, Member> MemberLookup { get; set; }
        public LookupDefinition<MemberGivingHistoryLookup, MemberGivingHistory> MemberGivingHistoryLookup { get; set; }
        public LookupDefinition<MemberPeriodGivingLookup, MemberPeriodGiving> MemberPeriodGivingLookup { get; set; }
        public LookupDefinition<MemberGivingLookup, MemberGiving> MemberGivingLookup { get; set; }
        public LookupDefinition<MemberGivingDetailsLookup, MemberGivingDetails> MemberGivingDetailsLookup { get; set; }

        public LookupDefinition<FundLookup, Fund> FundsLookup { get; set; }
        public LookupDefinition<BudgetLookup, BudgetItem> BudgetsLookup { get; set; }
        public LookupDefinition<FundHistoryLookup, FundHistory> FundHistoryLookup { get; set; }
        public LookupDefinition<FundPeriodLookup, FundPeriodTotals> FundPeriodLookup { get; set; }
        public LookupDefinition<BudgetPeriodTotalsLookup, BudgetPeriodTotals> BudgetPeriodLookup { get; set; }
        public LookupDefinition<BudgetActualsLookup, BudgetActual> BudgetActualsLookupDefinition { get; set; }

        public LookupDefinition<EventLookup, Event> EventLookupDefinition { get; set; }
        public LookupDefinition<EventMemberLookup, EventMember> EventMemberLookupDefinition { get; set; }
        public LookupDefinition<RoleLookup, Role> RoleLookupDefinition { get; set; }

        private DbContext _dbContext;
        private DbDataProcessor _dbDataProcessor;
        private bool _initialized;


        public ChurchLogixLookupContext()
        {
            SqliteDataProcessor = new SqliteDataProcessor();
            SqlServerDataProcessor = new SqlServerDataProcessor();
            CanProcessTableEvent += ChurchLogixLookupContext_CanProcessTableEvent;
        }

        private void ChurchLogixLookupContext_CanProcessTableEvent(object? sender, CanProcessTableArgs e)
        {
            if (!e.TableDefinition.HasRight(RightTypes.AllowView))
            {
                e.AllowView = false;
            }

            if (e.AllowView)
            {
                if (e.TableDefinition == MembersGiving
                    || e.TableDefinition == MembersGivingHistory
                    || e.TableDefinition == MembersPeriodGiving)
                {
                    if (!Members.HasSpecialRight((int)MemberSpecialRights.AllowViewGiving))
                    {
                        e.AllowView = false;
                    }
                }
            }

            if (!e.TableDefinition.HasRight(RightTypes.AllowEdit))
            {
                e.AllowEdit = false;
            }

            if (e.DeleteMode)
            {
                if (!e.TableDefinition.HasRight(RightTypes.AllowDelete))
                {
                    e.AllowDelete = false;
                }

                if (e.LookupDefinition != null && e.LookupDefinition.TableDefinition == Staff)
                {
                    var lookupToCheck = e.LookupDefinition.Clone();
                    var context = SystemGlobals.DataRepository.GetDataContext();
                    var table = context.GetTable<StaffPerson>();
                    var masterUser = table.FirstOrDefault();
                    if (masterUser != null)
                    {
                        var field = Staff
                            .GetFieldDefinition(p => p.Id);
                        lookupToCheck.FilterDefinition.AddFixedFilter(field, Conditions.Equals, masterUser.Id);
                        //var lookupUi = new LookupUserInterface()
                        //{
                        //    PageSize = 10,
                        //};
                        var lookupData =
                            lookupToCheck.TableDefinition.LookupDefinition.GetLookupDataMaui(lookupToCheck, false);
                        var count = lookupData.GetRecordCount();
                        if (count > 0)
                        {
                            e.AllowDelete = false;
                            var message = $"Deleting the master user '{masterUser.Name}' is not allowed.";
                            var caption = "Delete Denied!";
                            ControlsGlobals.UserInterface.ShowMessageBox(message, caption, RsMessageBoxIcons.Exclamation);
                        }
                    }
                }
            }

            if (!e.TableDefinition.HasRight(RightTypes.AllowAdd))
            {
                e.AllowAdd = false;
            }
        }

        public void SetProcessor(DbPlatforms platform)
        {
            switch (platform)
            {
                case DbPlatforms.Sqlite:
                    _dbDataProcessor = SqliteDataProcessor;
                    break;
                case DbPlatforms.SqlServer:
                    _dbDataProcessor = SqlServerDataProcessor;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(platform), platform, null);
            }
        }

        public void Initialize(IChurchLogixDbContext dbContext, DbPlatforms dbPlatform)
        {
            dbContext.SetLookupContext(this);
            _dbContext = dbContext.DbContext;

            SetProcessor(dbPlatform);
            if (_initialized)
            {
                return;
            }
            Initialize();
            _initialized = true;
        }


        protected override void InitializeLookupDefinitions()
        {
            SystemPreferencesLookup =
                new LookupDefinition<SystemPreferencesLookup, SystemPreferences>(SystemPreferences);

            SystemPreferencesLookup.AddVisibleColumnDefinition(
                p => p.Id
                , "Id"
                , p => p.Id, 99);

            SystemPreferences.HasLookupDefinition(SystemPreferencesLookup);

            StaffLookup = new LookupDefinition<StaffLookup, StaffPerson>(Staff);

            StaffLookup.AddVisibleColumnDefinition(
                p => p.Name
                , "Staff Person"
                , p => p.Name, 34);

            StaffLookup.AddVisibleColumnDefinition(
                p => p.Phone
                , "Phone Number"
                , p => p.PhoneNumber, 33);

            StaffLookup.AddVisibleColumnDefinition(
                p => p.Email
                , "Email Address"
                , p => p.Email, 33);

            Staff.HasLookupDefinition(StaffLookup);


            GroupsLookup = new LookupDefinition<GroupsLookup, Group>(Groups);

            GroupsLookup.AddVisibleColumnDefinition(
                p => p.Name
                , "Group"
                , p => p.Name, 99);

            Groups.HasLookupDefinition(GroupsLookup);

            StaffGroupsLookup = new LookupDefinition<StaffGroupsLookup, StaffGroup>(StaffGroups);

            StaffGroupsLookup.Include(p => p.StaffPerson)
                .AddVisibleColumnDefinition(p => p.StaffPerson
                    , "Staff Person"
                    , p => p.Name, 50);

            StaffGroupsLookup.Include(p => p.Group)
                .AddVisibleColumnDefinition(p => p.Group
                    , "Group"
                    , p => p.Name, 50);

            StaffGroups.HasLookupDefinition(StaffGroupsLookup);

            MemberLookup = new LookupDefinition<MemberLookup, Member>(Members);

            MemberLookup.AddVisibleColumnDefinition(
                p => p.Name
                , "Member"
                , p => p.Name, 99);

            Members.HasLookupDefinition(MemberLookup);

            MemberGivingHistoryLookup =
                new LookupDefinition<MemberGivingHistoryLookup, MemberGivingHistory>(MembersGivingHistory);

            MemberGivingHistoryLookup.Include(p => p.Member)
                .AddVisibleColumnDefinition(p => p.Member
                    , "Member"
                    , p => p.Name, 35);

            MemberGivingHistoryLookup.Include(p => p.Fund)
                .AddVisibleColumnDefinition(p => p.Fund
                    , "Fund"
                    , p => p.Description, 35);

            MemberGivingHistoryLookup.AddVisibleColumnDefinition(
                p => p.Date
                , "Date"
                , p => p.Date, 15);


            MemberGivingHistoryLookup.AddVisibleColumnDefinition(
                p => p.Amount
                , "Amount"
                , p => p.Amount, 15);

            MembersGivingHistory.HasLookupDefinition(MemberGivingHistoryLookup);

            MemberPeriodGivingLookup =
                new LookupDefinition<MemberPeriodGivingLookup, MemberPeriodGiving>(MembersPeriodGiving);

            MemberPeriodGivingLookup.Include(p => p.Member)
                .AddVisibleColumnDefinition(p => p.Member
                    , "Member"
                    , p => p.Name, 60);

            MemberPeriodGivingLookup.AddVisibleColumnDefinition(
                p => p.PeriodType
                , "Period Type"
                , p => p.PeriodType, 10);

            MemberPeriodGivingLookup.AddVisibleColumnDefinition(
                p => p.Date
                , "Date Ending"
                , p => p.Date, 15);

            MemberPeriodGivingLookup.AddVisibleColumnDefinition(
                p => p.Total
                , "Total Giving"
                , p => p.TotalGiving, 15);

            MembersPeriodGiving.HasLookupDefinition(MemberPeriodGivingLookup);

            MemberGivingLookup = new LookupDefinition<MemberGivingLookup, MemberGiving>(MembersGiving);

            MemberGivingLookup.Include(p => p.Member)
                .AddVisibleColumnDefinition(p => p.Member
                    , "Member"
                    , p => p.Name, 70);

            MemberGivingLookup.AddVisibleColumnDefinition(p => p.Date
                , "Date"
                , p => p.Date, 30);

            MembersGiving.HasLookupDefinition(MemberGivingLookup);

            MemberGivingDetailsLookup =
                new LookupDefinition<MemberGivingDetailsLookup, MemberGivingDetails>(MembersGivingDetails);

            MemberGivingDetailsLookup.Include(p => p.MemberGiving)
                .Include(p => p.Member)
                .AddVisibleColumnDefinition(p => p.Member
                    , "Member"
                    , p => p.Name, 40);

            MemberGivingDetailsLookup.Include(p => p.Fund)
                .AddVisibleColumnDefinition(p => p.Fund
                    , "Fund"
                    , p => p.Description, 40);

            MemberGivingDetailsLookup.AddVisibleColumnDefinition(
                p => p.Amount
                , "Amount"
                , p => p.Amount, 20);

            MembersGivingDetails.HasLookupDefinition(MemberGivingDetailsLookup);

            FundsLookup = new LookupDefinition<FundLookup, Fund>(Funds);

            FundsLookup.AddVisibleColumnDefinition(
                p => p.Description
                , "Fund"
                , p => p.Description, 40);

            FundsLookup.AddVisibleColumnDefinition(
                p => p.TotalCollected
                , "Total Collected"
                , p => p.TotalCollected, 20);

            FundsLookup.AddVisibleColumnDefinition(
                p => p.TotalSpent
                , "Total Spent"
                , p => p.TotalSpent, 20);

            FundsLookup.AddVisibleColumnDefinition(
                    p => p.Difference, "Difference", new FundsDifferenceFormula(), 15, "")
                .HasDecimalFieldType(DecimalFieldTypes.Currency)
                .DoShowNegativeValuesInRed()
                .DoShowPositiveValuesInGreen();

            Funds.HasLookupDefinition(FundsLookup);

            BudgetsLookup = new LookupDefinition<BudgetLookup, BudgetItem>(Budgets);

            BudgetsLookup.AddVisibleColumnDefinition(
                P => P.Name
                , "Name"
                , p => p.Name, 50);

            BudgetsLookup.AddVisibleColumnDefinition(
                p => p.Amount
                , "Amount"
                , p => p.Amount, 50);

            Budgets.HasLookupDefinition(BudgetsLookup);

            FundHistoryLookup = new LookupDefinition<FundHistoryLookup, FundHistory>(FundHistory);

            FundHistoryLookup.Include(p => p.Fund)
                .AddVisibleColumnDefinition(
                p => p.Fund
                , "Fund"
                , p => p.Description, 30);

            FundHistoryLookup.Include(p => p.BudgetItem)
                .AddVisibleColumnDefinition(
                    p => p.Budget
                    , "Budget"
                    , p => p.Name, 30);

            FundHistoryLookup.AddVisibleColumnDefinition(
                p => p.Date
                , "Date"
                , p => p.Date, 15);


            FundHistoryLookup.AddVisibleColumnDefinition(
                p => p.AmountType
                , "Type"
                , p => p.AmountType, 10);

            FundHistoryLookup.AddVisibleColumnDefinition(
                p => p.Amount
                , "Amount"
                , p => p.Amount, 15);

            FundHistory.HasLookupDefinition(FundHistoryLookup);

            FundPeriodLookup = new LookupDefinition<FundPeriodLookup, FundPeriodTotals>(FundPeriodTotals);

            FundPeriodLookup.Include(p => p.Fund)
                .AddVisibleColumnDefinition(p => p.Fund
                    , "Fund"
                    , p => p.Description, 30);

            FundPeriodLookup.AddVisibleColumnDefinition(
                p => p.PeriodType
                , "Period Type"
                , p => p.PeriodType, 10);

            FundPeriodLookup.AddVisibleColumnDefinition(
                p => p.Date
                , "Date Ending"
                , p => p.Date, 15);

            FundPeriodLookup.AddVisibleColumnDefinition(
                p => p.TotalIncome
                , "Total Income"
                , p => p.TotalIncome, 15);

            FundPeriodLookup.AddVisibleColumnDefinition(
                p => p.TotalExpenses
                , "Total Expenses"
                , p => p.TotalExpenses, 15);

            FundPeriodLookup.AddVisibleColumnDefinition(
                    p => p.Difference, "Difference"
                    , new FundPeriodDifferenceFormula(), 15, "")
                .HasDecimalFieldType(DecimalFieldTypes.Currency)
                .DoShowNegativeValuesInRed()
                .DoShowPositiveValuesInGreen();

            FundPeriodTotals.HasLookupDefinition(FundPeriodLookup);

            BudgetPeriodLookup = new LookupDefinition<BudgetPeriodTotalsLookup, BudgetPeriodTotals>(BudgetPeriodTotals);

            BudgetPeriodLookup.Include(p => p.BudgetItem)
                .AddVisibleColumnDefinition(p => p.BudgetItem
                    , "Fund"
                    , p => p.Name, 40);
            BudgetPeriodLookup.AddVisibleColumnDefinition(
                p => p.PeriodType
                , "Period Type"
                , p => p.PeriodType, 10);

            BudgetPeriodLookup.AddVisibleColumnDefinition(
                p => p.Date
                , "Date Ending"
                , p => p.Date, 25);

            BudgetPeriodLookup.AddVisibleColumnDefinition(
                p => p.Total
                , "Total"
                , p => p.Total, 25);

            BudgetPeriodTotals.HasLookupDefinition(BudgetPeriodLookup);

            BudgetActualsLookupDefinition = new LookupDefinition<BudgetActualsLookup, BudgetActual>(BudgetActuals);

            BudgetActualsLookupDefinition.Include(p => p.Budget)
                .AddVisibleColumnDefinition(
                    p => p.Budget
                    , "Budget"
                    , p => p.Name, 50);

            BudgetActualsLookupDefinition.AddVisibleColumnDefinition(
                p => p.Date
                , "Date"
                , p => p.Date, 25);

            BudgetActualsLookupDefinition.AddVisibleColumnDefinition(
                p => p.Amount
                , "Amount"
                , p => p.Amount, 25);

            BudgetActuals.HasLookupDefinition(BudgetActualsLookupDefinition);

            EventLookupDefinition = new LookupDefinition<EventLookup, Event>(Events);

            EventLookupDefinition.AddVisibleColumnDefinition(
                p => p.Name
                , "Name"
                , p => p.Name, 50);

            EventLookupDefinition.AddVisibleColumnDefinition(
                p => p.BeginDate
                , "Begin Date/Time"
                , p => p.BeginDate, 25);

            EventLookupDefinition.AddVisibleColumnDefinition(
                p => p.EndDate
                , "End Date/Time"
                , p => p.EndDate, 25);

            Events.HasLookupDefinition(EventLookupDefinition);

            EventMemberLookupDefinition = new LookupDefinition<EventMemberLookup, EventMember>(EventsMember);

            EventMemberLookupDefinition.Include(p => p.Event)
                .AddVisibleColumnDefinition(
                    p => p.Event
                    , "Event"
                    , p => p.Name, 40);

            EventMemberLookupDefinition.Include(p => p.Member)
                .AddVisibleColumnDefinition(
                    p => p.Member
                    , "Member"
                    , p => p.Name, 40);

            EventMemberLookupDefinition.AddVisibleColumnDefinition(
                p => p.AmountPaid
                , "Amount Paid"
                , p => p.AmountPaid, 20);

            EventsMember.HasLookupDefinition(EventMemberLookupDefinition);

            RoleLookupDefinition = new LookupDefinition<RoleLookup, Role>(Roles);

            RoleLookupDefinition.AddVisibleColumnDefinition(
                p => p.Name
                , "Name"
                , p => p.Name, 99);

            Roles.HasLookupDefinition(RoleLookupDefinition);
        }

        protected override void SetupModel()
        {
            Members.PriorityLevel = 100;
            Groups.PriorityLevel = 100;
            Funds.PriorityLevel = 100;
            Events.PriorityLevel = 100;
            Roles.PriorityLevel = 100;
            EventsMember.PriorityLevel = 200;
            MembersGivingHistory.PriorityLevel = 200;
            MembersPeriodGiving.PriorityLevel = 200;
            MembersGiving.PriorityLevel = 200;
            Budgets.PriorityLevel = 200;
            Staff.PriorityLevel = 200;
            StaffGroups.PriorityLevel = 300;
            FundHistory.PriorityLevel = 300;
            FundPeriodTotals.PriorityLevel = 300;
            BudgetPeriodTotals.PriorityLevel = 300;
            BudgetActuals.PriorityLevel = 300;
            MembersGivingDetails.PriorityLevel = 300;

            Staff.GetFieldDefinition(p => p.Notes).IsMemo();

            Members.GetFieldDefinition(p => p.Notes).IsMemo();

            MembersGivingHistory.GetFieldDefinition(p => p.Amount)
                .HasDecimalFieldType(DecimalFieldTypes.Currency);

            MembersPeriodGiving.GetFieldDefinition(p => p.PeriodType)
                .IsEnum<PeriodTypes>();

            MembersPeriodGiving.GetFieldDefinition(p => p.TotalGiving)
                .HasDecimalFieldType(DecimalFieldTypes.Currency);

            MembersGivingDetails.GetFieldDefinition(p => p.Amount)
                .HasDecimalFieldType(DecimalFieldTypes.Currency);

            Funds.GetFieldDefinition(p => p.Notes).IsMemo();
            Funds.GetFieldDefinition(p => p.Goal).HasDecimalFieldType(DecimalFieldTypes.Currency);

            Budgets.GetFieldDefinition(p => p.Notes).IsMemo();
            Budgets.GetFieldDefinition(p => p.Amount).HasDecimalFieldType(DecimalFieldTypes.Currency);

            FundHistory.GetFieldDefinition(p => p.AmountType)
                .IsEnum<HistoryAmountTypes>();
            FundHistory.GetFieldDefinition(p => p.Amount)
                .HasDecimalFieldType(DecimalFieldTypes.Currency);

            FundPeriodTotals.GetFieldDefinition(p => p.PeriodType)
                .IsEnum<PeriodTypes>();
            FundPeriodTotals.GetFieldDefinition(p => p.TotalIncome)
                .HasDecimalFieldType(DecimalFieldTypes.Currency);
            FundPeriodTotals.GetFieldDefinition(p => p.TotalExpenses)
                .HasDecimalFieldType(DecimalFieldTypes.Currency);

            BudgetPeriodTotals.GetFieldDefinition(p => p.PeriodType)
                .IsEnum<PeriodTypes>();
            BudgetPeriodTotals.GetFieldDefinition(p => p.Total)
                .HasDecimalFieldType(DecimalFieldTypes.Currency);

            BudgetActuals.GetFieldDefinition(p => p.Amount)
                .HasDecimalFieldType(DecimalFieldTypes.Currency);

            Events.GetFieldDefinition(
                p => p.Notes).IsMemo();
            Events.GetFieldDefinition(
                    p => p.BeginDate).HasDateType(DbDateTypes.DateTime)
                .DoConvertToLocalTime();
            Events.GetFieldDefinition(
                    p => p.EndDate).HasDateType(DbDateTypes.DateTime)
                .DoConvertToLocalTime();
            Events.GetFieldDefinition(
                p => p.MemberCost).HasDecimalFieldType(DecimalFieldTypes.Currency);
            Events.GetFieldDefinition(
                p => p.TotalCost).HasDecimalFieldType(DecimalFieldTypes.Currency);
            Events.GetFieldDefinition(
                p => p.TotalPaid).HasDecimalFieldType(DecimalFieldTypes.Currency);

            EventsMember.GetFieldDefinition(
                p => p.AmountPaid).HasDecimalFieldType(DecimalFieldTypes.Currency);
        }

        public override UserAutoFill GetUserAutoFill(string userName)
        {
            var context = SystemGlobals.DataRepository.GetDataContext();
            var table = context.GetTable<StaffPerson>();
            var staffPerson = table.FirstOrDefault(p => p.Name == userName);
            if (staffPerson != null)
            {
                var userAutoFill = new UserAutoFill
                {
                    AutoFillSetup = new AutoFillSetup(StaffLookup.Clone()),
                    AutoFillValue = staffPerson.GetAutoFillValue()
                };
                return userAutoFill;
            }
            return base.GetUserAutoFill(userName);
        }
    }
}
