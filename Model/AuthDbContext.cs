using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using _210081D_Wong_Yee_Jin.ViewModels;

namespace _210081D_Wong_Yee_Jin.Model
{
    public class AuthDbContext: IdentityDbContext<ApplicationUser>
    {
        private readonly IConfiguration _configuration;

        public AuthDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString = _configuration.GetConnectionString("AuthConnectionString");
            optionsBuilder.UseSqlServer(connectionString);
        }

        public DbSet<AuditLog> AuditLogDB { get; set; }

        public DbSet<PasswordHistory> PasswordHistoryDB { get; set; }
    }
}
