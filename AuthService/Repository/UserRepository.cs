using AuthService.Data;
using AuthService.Interfaces;
using AuthService.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly AuthDbContext _context;

        public UserRepository(AuthDbContext context)
        {
            _context = context;
        }
        public ICollection<User> GetUsers()
        {
            return _context.Users.ToList();
        }
        public async Task<User> GetUserById(int id)
        {
            return await (_context.Users.FindAsync(id));
        }
        //public bool DeleteUser(User user)
        //{
        //    _context.Users.Remove(user);
        //    return Save();

        //}
        public bool AddUser(User newUser)
        {
            _context.Users.Add(newUser);
            return Save();
        }

        //get user by email
        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _context.Users
                                 .FirstOrDefaultAsync(u => u.Email == email);
        }

        public bool UpdateUser(UpdateUserModel model)
        {
            // Lấy thông tin người dùng dựa trên UserID
            var existingUser = _context.Users.FirstOrDefault(u => u.UserID == model.UserID);

            if (existingUser == null)
            {
                // Nếu không tìm thấy người dùng, trả về false
                return false;
            }

            // Cập nhật các thuộc tính (chỉ cập nhật nếu giá trị mới không null)
            existingUser.FirstName = model.FirstName ?? existingUser.FirstName;
            existingUser.LastName = model.LastName ?? existingUser.LastName;
            // Cập nhật thêm các thuộc tính khác nếu có...

            // Cập nhật đối tượng người dùng
            _context.Users.Update(existingUser);

            // Lưu thay đổi vào cơ sở dữ liệu
            return Save();
        }




        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }
        public async Task<List<Role>> GetRolesByUserId(int userId)
        {
            var roles = await (from ur in _context.UserRoles
                               join r in _context.Roles on ur.RoleID equals r.RoleID
                               where ur.UserID == userId
                               select r).ToListAsync();

            return roles;
        }
    }
}
