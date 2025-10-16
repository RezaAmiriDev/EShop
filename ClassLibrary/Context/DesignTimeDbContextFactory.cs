using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using ClassLibrary.Models; // namespace مربوط به MobiContext

public class MobiContextFactory : IDesignTimeDbContextFactory<MobiContext>
{
    public MobiContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<MobiContext>();

        // همین کانکشن رشته‌ی development را اینجا قرار بده
        var connectionString = "Server=.;Database=StoreDB;Trusted_Connection=True;TrustServerCertificate=True;";

        optionsBuilder.UseSqlServer(connectionString);

        return new MobiContext(optionsBuilder.Options);
    }
}
