using BLL.DTO;
using BLL.Interfaces;
using static Accounting_of_goods.ConsoleController;

namespace Accounting_of_goods
{
    public static class Verify
    {
        public static UserDTO SigningUp(IUserServise userServise)
        {
            string firstName = GetStrint("Input you first name: ");
            string secontName = GetStrint("Input you secont name: ");
            int phoneNumber = (int)GetNumber("Input you phone number: ");
            var client = new UserDTO()
            {
                FirstName = firstName,
                LastName = secontName,
                UserStatus = DAL.Statuses.UserStatus.Customer,
                PhoneNumber = phoneNumber
            };
            client = userServise.CreatAccount(client);
            return client;
        }

        public static UserDTO SigningIn(IUserServise userServise)
        {
            int phoneNumber = (int)GetNumber("Input your phone number: ");
            var client = userServise.GetAccount(phoneNumber);
            if (client == null)
            {
                Write("Didn't find your account, you must sing up");
            }
            else
            {
                Write($"\nHello {client.FirstName} {client.LastName}\n");
            }
            return client;
        }
    }
}
