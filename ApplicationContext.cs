using Microsoft.EntityFrameworkCore;

namespace XmlImport
{
    public class ApplicationContext:DbContext
    {
        public DbSet<Master> Masters { get; set; }

        public ApplicationContext()
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=idsdb;Username=postgres;Password=885510");
        }

    }
}
