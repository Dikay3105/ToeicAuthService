using AuthService.Models;

namespace AuthService.Interfaces
{
    public interface IUserRepository
    {
        ICollection<User> GetUsers();
        Task<User> GetUserById(int id);
        //bool DeleteUser(User user);
        Task<List<Role>> GetRolesByUserId(int userId);
        bool AddUser(User newUser);
        bool UpdateUser(UpdateUserModel model);
        bool Save();

    }
}
