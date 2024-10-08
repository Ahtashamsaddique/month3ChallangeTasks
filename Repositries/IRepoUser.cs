using WebApplication2.DTO;

namespace WebApplication2.Repositries
{
    public interface IRepoUser
    {
        Task<int> loginUser(loginDTO logindto);
        Task<RegisterUserDTO> RegistorUser(RegisterUserDTO registerUserDTO);
    }
}
