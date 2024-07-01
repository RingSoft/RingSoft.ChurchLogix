using RingSoft.App.Library;
using RingSoft.DbLookup;
using RingSoft.DbLookup.DataProcessor;

namespace RingSoft.ChurchLogix.Library
{
    public class DataRepository : SystemDataRepository
    {
        public override IDbContext GetDataContext()
        {
            //return AppGlobals.GetNewDbContext();
            throw new NotImplementedException();
        }

        public override IDbContext GetDataContext(DbDataProcessor dataProcessor)
        {
            var platform = DbPlatforms.Sqlite;

            if (dataProcessor is SqlServerDataProcessor)
            {
                platform = DbPlatforms.SqlServer;
            }
            //return AppGlobals.GetNewDbContext(platform);

            throw new NotImplementedException();
        }
    }
}
