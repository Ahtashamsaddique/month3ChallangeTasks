
using Microsoft.EntityFrameworkCore;
using WebApplication2.DTO;

namespace WebApplication2.DBContext
{
    public interface IApplicationDbContext
    {
        DbSet<RegisterUserDTO> Users { get; set; }

        Task<int> SaveChangesAsync();
    }
}
