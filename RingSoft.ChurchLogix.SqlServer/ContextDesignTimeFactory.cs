using Microsoft.EntityFrameworkCore.Design;

namespace RingSoft.ChurchLogix.SqlServer
{
    public class ContextDesignTimeFactory : IDesignTimeDbContextFactory<ChurchLogixSqlServerDbContext>
    {
        ChurchLogixSqlServerDbContext IDesignTimeDbContextFactory<ChurchLogixSqlServerDbContext>.CreateDbContext(string[] args)
        {
            return new ChurchLogixSqlServerDbContext(){IsDesignTime = true};
        }
    }
}
