using AuthService.Data;
using AuthService.Dto;
using AuthService.Interfaces;
using AuthService.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly AuthDbContext _context;
        private readonly IMapper _mapper;

        public UserController(IUserRepository userRepository, AuthDbContext context, IMapper mapper)
        {
            _userRepository = userRepository;
            _context = context;
            _mapper = mapper;
        }

        // Get all users
        [HttpGet]
        public ActionResult<IEnumerable<User>> GetUsers()
        {
            var users = _mapper.Map<List<UserDto>>(_userRepository.GetUsers());
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(new
            {
                EC = 0,
                EM = "Get all users success",
                DT = users
            });
        }

        // Get user by ID
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UserDto), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<UserDto>> GetUserById(int id)
        {
            var user = await _userRepository.GetUserById(id);
            if (user == null)
            {
                return NotFound(new
                {
                    EC = -1,  // Error Code
                    EM = $"User with ID {id} not found"
                });
            }

            var userDto = _mapper.Map<UserDto>(user);

            return Ok(new
            {
                EC = 0,  // Error Code
                EM = "User found",
                DT = userDto
            });
        }


        // Add new user
        [HttpPost]
        public IActionResult AddUser([FromBody] User newUser)
        {
            if (newUser == null)
            {
                return BadRequest(ModelState);
            }

            var checkUsername = _userRepository.GetUsers()
                .Where(u => u.Username.Trim().ToUpper() == newUser.Username.Trim().ToUpper()
               )
                .FirstOrDefault();

            if (checkUsername != null)
            {
                ModelState.AddModelError("", "Username already used");
                return StatusCode(422, new
                {
                    EC = -1,
                    EM = ModelState

                });
            }

            var checkEmail = _userRepository.GetUsers()
                            .Where(u => u.Email.Trim().ToUpper() == newUser.Email.Trim().ToUpper())
                            .FirstOrDefault();

            if (checkEmail != null)
            {
                ModelState.AddModelError("", "Email already used");
                return StatusCode(422, new
                {
                    EC = -1,
                    EM = ModelState

                });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userMap = _mapper.Map<User>(newUser);
            if (!_userRepository.AddUser(newUser))
            {
                ModelState.AddModelError("", "Something went wrong when saving new user");
                return StatusCode(422, new
                {
                    EC = -1,
                    EM = ModelState

                });
            }
            return Ok(new
            {
                EC = 0,
                EM = "Successfully added user"
            });
        }

        //update user
        [HttpPut]
        public IActionResult UpdateUser([FromBody] UpdateUserModel model)
        {
            if (model == null)
            {
                return BadRequest(new { EC = -1, EM = "Invalid user data" });
            }

            // Kiểm tra sự tồn tại của người dùng
            var checkExistUser = _userRepository.GetUsers()
                .FirstOrDefault(u => u.UserID == model.UserID);

            if (checkExistUser == null)
            {
                return NotFound(new { EC = -1, EM = "No user found" });
            }

            // Kiểm tra xem có thay đổi gì không
            if (checkExistUser.FirstName == model.FirstName && checkExistUser.LastName == model.LastName)
            {
                return StatusCode(422, new { EC = -1, EM = "Nothing need to change" });
            }

            // Kiểm tra model có hợp lệ không
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Thực hiện cập nhật người dùng
            if (!_userRepository.UpdateUser(model))
            {
                return StatusCode(500, new { EC = -1, EM = "Something went wrong when updating user" });
            }

            // Trả về thành công
            return Ok(new { EC = 0, EM = "Successfully updated user" });
        }


        // Delete user by ID
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteUser(int id)
        //{
        //    var user = await _userRepository.GetUserById(id);
        //    if (user == null)
        //    {
        //        return NotFound(new
        //        {
        //            EC = -1,  // Error Code
        //            EM = $"User with ID {id} not found"
        //        });
        //    }

        //    var result = await _userRepository.DeleteUser(id);
        //    return Ok(new
        //    {
        //        EC = 0,
        //        EM = result
        //    });
        //}

        // Lấy roles của user dựa trên UserID
        [HttpGet("roles/{userId}")]
        public async Task<IActionResult> GetRolesByUserId(int userId)
        {
            var roles = await _userRepository.GetRolesByUserId(userId);

            if (roles == null || !roles.Any())
            {
                return NotFound(new
                {
                    EC = -1,
                    EM = "Roles not found for this user"
                });
            }

            var rolesDto = _mapper.Map<List<Role>>(roles);

            return Ok(new
            {
                EC = 0,
                EM = "Roles retrieved successfully",
                DT = rolesDto
            });
        }
    }
}
