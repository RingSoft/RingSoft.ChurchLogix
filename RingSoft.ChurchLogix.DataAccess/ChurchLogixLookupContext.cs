using Microsoft.EntityFrameworkCore;
using RingSoft.App.Library;
using RingSoft.ChurchLogix.DataAccess.Model;
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
        public TableDefinition<Member> Members { get; set; }
        public TableDefinition<Group> Groups { get; set; }
        public TableDefinition<StaffGroup> StaffGroups { get; set; }

        public TableDefinition<Fund> Funds { get; set; }
        public TableDefinition<BudgetItem> Budgets { get; set; }
        public TableDefinition<FundHistory> FundHistory { get; set; }
        public TableDefinition<FundPeriodTotals> FundPeriodTotals { get; set; }
        public TableDefinition< BudgetPeriodTotals> BudgetPeriodTotals { get; set; }

        public LookupDefinition<StaffLookup, StaffPerson> StaffLookup { get; set; }
        public LookupDefinition<MemberLookup, Member> MemberLookup { get; set; }
        public LookupDefinition<GroupsLookup, Group> GroupsLookup { get; set; }
        public LookupDefinition<StaffGroupsLookup, StaffGroup> StaffGroupsLookup { get; set;}

        public LookupDefinition<FundLookup, Fund> FundsLookup { get; set; }
        public LookupDefinition<BudgetLookup, BudgetItem> BudgetsLookup { get; set; }
        public LookupDefinition<FundHistoryLookup, FundHistory> FundHistoryLookup { get; set; }
        public LookupDefinition<FundPeriodLookup, FundPeriodTotals> FundPeriodLookup { get; set; }
        public LookupDefinition<BudgetPeriodTotalsLookup, BudgetPeriodTotals> BudgetPeriodLookup { get; set; }

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

            if (e.AllowView && e.TableDefinition == Members)
            {
                if (!Members.HasSpecialRight((int)MemberSpecialRights.AllowViewGiving))
                {
                    
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

            MemberLookup = new LookupDefinition<MemberLookup, Member>(Members);

            MemberLookup.AddVisibleColumnDefinition(
                p => p.Name
                , "Member"
                , p => p.Name, 99);

            Members.HasLookupDefinition(MemberLookup);

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

            FundHistoryLookup.AddVisibleColumnDefinition(
                p => p.Fund
                , "Fund"
                , p => p.Amount, 40);

            FundHistoryLookup.AddVisibleColumnDefinition(
                p => p.AmountType
                , "Type"
                , p => p.AmountType, 30);

            FundHistoryLookup.AddVisibleColumnDefinition(
                p => p.Amount
                , "Amount"
                , p => p.Amount, 30);

            FundHistory.HasLookupDefinition(FundHistoryLookup);

            FundPeriodLookup = new LookupDefinition<FundPeriodLookup, FundPeriodTotals>(FundPeriodTotals);

            FundPeriodLookup.Include(p => p.Fund)
                .AddVisibleColumnDefinition(p => p.Fund
                    , "Fund"
                    , p => p.Description, 40);

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
                    , p => p.Name, 50);

            BudgetPeriodLookup.AddVisibleColumnDefinition(
                p => p.Date
                , "Date Ending"
                , p => p.Date, 25);

            BudgetPeriodLookup.AddVisibleColumnDefinition(
                p => p.Total
                , "Total"
                , p => p.Total, 25);

            BudgetPeriodTotals.HasLookupDefinition(BudgetPeriodLookup);
        }

        protected override void SetupModel()
        {
            Members.PriorityLevel = 100;
            Groups.PriorityLevel = 100;
            Funds.PriorityLevel = 100;
            Budgets.PriorityLevel = 200;
            Staff.PriorityLevel = 200;
            StaffGroups.PriorityLevel = 300;
            FundHistory.PriorityLevel = 300;
            FundPeriodTotals.PriorityLevel = 300;
            BudgetPeriodTotals.PriorityLevel = 300;

            Staff.GetFieldDefinition(p => p.Notes).IsMemo();

            Members.GetFieldDefinition(p => p.Notes).IsMemo();

            Funds.GetFieldDefinition(p => p.Notes).IsMemo();
            Budgets.GetFieldDefinition(p => p.Notes).IsMemo();

            FundHistory.GetFieldDefinition(p => p.AmountType)
                .IsEnum<HistoryAmountTypes>();

            FundPeriodTotals.GetFieldDefinition(p => p.PeriodType)
                .IsEnum<PeriodTypes>();

            BudgetPeriodTotals.GetFieldDefinition(p => p.PeriodType)
                .IsEnum<PeriodTypes>();
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
