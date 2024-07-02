using Microsoft.EntityFrameworkCore.Design;

namespace RingSoft.ChurchLogix.Sqlite
{
    public class ContextDesignTimeFactory : IDesignTimeDbContextFactory<ChurchLogixSqliteDbContext>
    {
        ChurchLogixSqliteDbContext IDesignTimeDbContextFactory<ChurchLogixSqliteDbContext>.CreateDbContext(string[] args)
        {
            return new ChurchLogixSqliteDbContext(){IsDesignTime = true};
        }
    }
}
