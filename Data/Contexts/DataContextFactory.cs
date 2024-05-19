using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;

namespace Data.Contexts;

public class DataContextFactory : IDesignTimeDbContextFactory<DataContext>
{
    
    public DataContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<DataContext>();
        //todo
        optionsBuilder.UseSqlServer("Server=tcp:sqlserver-silicon-nor.database.windows.net,1433;Initial Catalog=verification_requests_database;Persist Security Info=False;User ID=SqlAdmin;Password=changeme$1234;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");

        return new DataContext(optionsBuilder.Options);
    }
}
