using Microsoft.EntityFrameworkCore;
using WebApplication2.DTO;
namespace WebApplication2.DBContext
{
    public class ApplicationDbContext: DbContext, IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
            
        }

        public async Task<int> SaveChangesAsync()
        {
            return await base.SaveChangesAsync();
        }

        public DbSet<RegisterUserDTO> Users { get; set; }
    }
}

