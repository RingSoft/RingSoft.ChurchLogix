using Microsoft.EntityFrameworkCore;
using RingSoft.App.Library;
using RingSoft.ChurchLogix.DataAccess.Model;
using RingSoft.ChurchLogix.DataAccess.Model.MemberManagement;
using RingSoft.ChurchLogix.DataAccess.Model.StaffManagement;
using RingSoft.DbLookup.DataProcessor;
using RingSoft.DbLookup.EfCore;
using RingSoft.DbLookup.Lookup;
using RingSoft.DbLookup.ModelDefinition;

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

        public LookupDefinition<StaffLookup, StaffPerson> StaffLookup { get; set; }
        public LookupDefinition<MemberLookup, Member> MemberLookup { get; set; }

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
                , p => p.Name, 99);

            Staff.HasLookupDefinition(StaffLookup);

            MemberLookup = new LookupDefinition<MemberLookup, Member>(Members);

            MemberLookup.AddVisibleColumnDefinition(
                p => p.Name
                , "Member"
                , p => p.Name, 99);

            Members.HasLookupDefinition(MemberLookup);
        }

        protected override void SetupModel()
        {
            Staff.PriorityLevel = 100;
        }
    }
}
