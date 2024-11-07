using AuthService.Data;
using AuthService.Interfaces;
using AuthService.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Repository
{
    public class RoleRepository : IRoleRepository
    {
        private readonly AuthDbContext _context;

        public RoleRepository(AuthDbContext context)
        {
            _context = context;
        }

        public async Task<List<Role>> GetRolesByUserId(int userId)
        {
            var roles = await (from userRole in _context.UserRoles
                               join role in _context.Roles on userRole.RoleID equals role.RoleID
                               where userRole.UserID == userId
                               select role).ToListAsync();

            return roles;
        }
    }
}
