using Microsoft.EntityFrameworkCore;
using WebApplication2.DBContext;
using WebApplication2.DTO;
using WebApplication2.Extension;

namespace WebApplication2.Repositries
{
    public class RepoUser:IRepoUser
    {
        private readonly IApplicationDbContext _context;

        public RepoUser(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<RegisterUserDTO> RegistorUser(RegisterUserDTO registerUserDTO)
        {
            registerUserDTO.Password=registerUserDTO.Password.ComputeSha256Hash();
            await _context.Users.AddAsync(registerUserDTO);
            await _context.SaveChangesAsync();
            return registerUserDTO;
        }
        public async Task<int> loginUser(loginDTO logindto)
        {
            logindto.Password = logindto.Password.ComputeSha256Hash();
            var responce = await _context.Users.Where(x => x.UserName == logindto.Username && x.Password == logindto.Password).FirstOrDefaultAsync();
            if (responce != null)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }
}
