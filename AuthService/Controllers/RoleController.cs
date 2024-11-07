using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers
{
    //[ApiController]
    //[Route("api/[controller]")]
    public class RoleController : ControllerBase
    {
        //private readonly IRoleRepository _roleRepository;

        //public RoleController(IRoleRepository roleRepository)
        //{
        //    _roleRepository = roleRepository;
        //}

        //[HttpGet("{userId}/roles")]
        //public IActionResult GetUserRoles(int userId)
        //{
        //    // Lấy các roles của user từ repository
        //    List<Role> roles = _roleRepository.GetRolesByUserId(userId);

        //    // Kiểm tra nếu không tìm thấy role nào cho user
        //    if (roles == null || roles.Count == 0)
        //    {
        //        return NotFound(new { Message = "No roles found for this user." });
        //    }

        //    // Trả về kết quả roles nếu tìm thấy
        //    return Ok(roles);
        //}
    }
}
