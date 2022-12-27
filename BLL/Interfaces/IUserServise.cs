using BLL.DTO;

namespace BLL.Interfaces
{
    public interface IUserServise
    {
        UserDTO CreatAccount(UserDTO client);
        UserDTO GetAccount(int phoneNumber);
    }
}
