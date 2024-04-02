using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MatchingJob.Entities;
using MatchingJob.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.IdentityModel.Tokens;
using MatchingJob.DAL;
using MatchingJob.BLL;

namespace MatchingJob.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IUserRepository _userRepo;

        public UsersController(AppDbContext context, IUserRepository userRepository)
        {
            _context = context;
            _userRepo = userRepository;
        }


        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetById(Guid id)
        {
            try
            {
                var user = await _userRepo.findById(id);
                if (user == null)
                {
                    return NotFound(new APIResponse
                    {
                        Success = false,
                        Message = "User not found"
                    });
                }
                return Ok(new APIResponse
                {
                    Success = true,
                    Message = "Get user by id successfully!",
                    Data = user
                });
            }
            catch
            {
                return BadRequest(new APIResponse
                {
                    Success = false,
                    Message = Constants.SOMETHING_WENT_WRONG
                });
            }
        }

        [Route("/getUsersByName")]
        [HttpGet]
        public async Task<ActionResult<User>> GetUsersByName(string name)
        {
            try
            {
                var user = await _userRepo.getUsersByName(name);
                if (user == null)
                {
                    return NotFound(new APIResponse
                    {
                        Success = false,
                        Message = "User not found"
                    });
                }
                return Ok(new APIResponse
                {
                    Success = true,
                    Message = "Get user by id successfully!",
                    Data = user
                });
            }
            catch
            {
                return BadRequest(new APIResponse
                {
                    Success = false,
                    Message = Constants.SOMETHING_WENT_WRONG
                });
            }
        }


        // POST: api/Users
        [HttpPost]
        public async Task<ActionResult<User>> create(UsersDTO userDTO)
        {
            try
            {
                var user = await _userRepo.save(userDTO);
                if (user == null)
                {
                    return BadRequest(new APIResponse
                    {
                        Success = false,
                        Message = "Create new user not successfully!"
                    });
                }
                else
                {
                    
                    return Ok(new APIResponse
                    {
                        Success = true,
                        Message = "Create new user successfully!",
                        Data = user
                    });
                }
            }
            catch(Exception e) 
            {
                Console.WriteLine(e);
                return BadRequest(new APIResponse
                {
                    Success = false,
                    Message = Constants.SOMETHING_WENT_WRONG,
                }); 
            }
        }


        // PUT: api/Users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, UsersDTO userDTO)
        {
            try
            {
                var user = await _context.Users.SingleOrDefaultAsync(u => u.Id == id);
                if (user == null) return NotFound();
                if (id != user.Id)
                {
                    return BadRequest();
                }

                //user = ConvertDTOToEntity(userDTO);

                user.FirstName = userDTO.FirstName;
                user.LastName = userDTO.LastName;
                user.Email = userDTO.Email;
                user.Password = userDTO.Password;
                user.UserName = userDTO.UserName;

                // Thiết lập trạng thái của đối tượng user thành Modified
                _context.Entry(user).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsersExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                // Update xong thi tra ve trang thai nocontent
                return NoContent();
            }
            catch
            {
                return BadRequest(new APIResponse
                {
                    Success = false,
                    Message = Constants.SOMETHING_WENT_WRONG
                });
            }
        }

        // PUT: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> SoftDelete(Guid id)
        {
            try
            {
                var users = await _context.Users.FindAsync(id);
                if (users == null)
                {
                    return NotFound();
                }
                users.IsDeleted = true;
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch
            {
                return BadRequest();
            }
        }

        [Route("/Search")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> search(string? keyword, string? sortBy, bool? gender, int pageNumber)
        {
            try
            {
                var listUsers = await _userRepo.search(keyword,sortBy,gender,pageNumber);
                if (listUsers.IsNullOrEmpty())
                {
                    return Ok(new APIResponse
                    {
                        Success = true,
                        Message = "Don't have any users in DB"
                    });
                }
                return Ok(new APIResponse
                {
                    Success = true,
                    Message = "Get all users successfully",
                    Data = listUsers
                });
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest(new APIResponse
                {
                    Success = false,
                    Message = Constants.SOMETHING_WENT_WRONG
                });
            }
        }

        private bool UsersExists(Guid id)
        {
            return _context.Users.Any(e => e.Id == id);
        }

    }
}
