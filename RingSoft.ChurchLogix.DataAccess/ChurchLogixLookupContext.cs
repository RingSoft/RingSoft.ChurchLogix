using Microsoft.EntityFrameworkCore;
using RingSoft.App.Library;
using RingSoft.ChurchLogix.DataAccess.Model;
using RingSoft.ChurchLogix.DataAccess.Model.Financial_Management;
using RingSoft.ChurchLogix.DataAccess.Model.MemberManagement;
using RingSoft.ChurchLogix.DataAccess.Model.StaffManagement;
using RingSoft.DbLookup;
using RingSoft.DbLookup.AutoFill;
using RingSoft.DbLookup.DataProcessor;
using RingSoft.DbLookup.EfCore;
using RingSoft.DbLookup.Lookup;
using RingSoft.DbLookup.ModelDefinition;
using RingSoft.DbLookup.ModelDefinition.FieldDefinitions;

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

        public LookupDefinition<StaffLookup, StaffPerson> StaffLookup { get; set; }
        public LookupDefinition<MemberLookup, Member> MemberLookup { get; set; }
        public LookupDefinition<GroupsLookup, Group> GroupsLookup { get; set; }
        public LookupDefinition<StaffGroupsLookup, StaffGroup> StaffGroupsLookup { get; set;}

        public LookupDefinition<FundLookup, Fund> FundsLookup { get; set; }

        private DbContext _dbContext;
        private DbDataProcessor _dbDataProcessor;
        private bool _initialized;


        public ChurchLogixLookupContext()
        {
            SqliteDataProcessor = new SqliteDataProcessor();
            SqlServerDataProcessor = new SqlServerDataProcessor();
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
        }

        protected override void SetupModel()
        {
            Members.PriorityLevel = 100;
            Groups.PriorityLevel = 100;
            Staff.PriorityLevel = 200;
            StaffGroups.PriorityLevel = 300;
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
