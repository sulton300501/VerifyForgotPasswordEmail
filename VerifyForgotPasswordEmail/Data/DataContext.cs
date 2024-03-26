

namespace VerifyForgotPasswordEmail.Data
{
    public class DataContext:DbContext
    {
        public DataContext(DbContextOptions<DataContext> options):base(options)
        {
          
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseNpgsql(@"Host=localhost;Port=5432;Username=postgres;Password=sulton;Database=FutureProjectsDBEmail");
        }

        public DbSet<User> Users =>Set<User>();
    }
}
