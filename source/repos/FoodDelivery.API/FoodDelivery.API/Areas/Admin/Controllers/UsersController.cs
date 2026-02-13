using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FoodDelivery.Domain.Entities;
using FoodDelivery.Domain.IRepository;
using FoodDelivery.Application.DTOs.Users; 

namespace FoodDelivery.API.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class UsersController : ControllerBase
    {
        private readonly IRepository<User> _userRepo;

        public UsersController(IRepository<User> userRepo)
        {
            _userRepo = userRepo;
        }

        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAll()
        {
            var users = await _userRepo.GetAsync();

            var response = users.Select(u => new UserDto(
                u.Id,
                u.Name,
                u.Email,
                u.Phone,
                u.Role,
                u.CreatedAt
            ));

            return Ok(response);
        }

       
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetById(int id)
        {
            var user = await _userRepo.GetOneAsync(u => u.Id == id);

            if (user == null)
                return NotFound($"User with ID {id} not found.");

            var response = new UserDto(
                user.Id,
                user.Name,
                user.Email,
                user.Phone,
                user.Role,
                user.CreatedAt
            );

            return Ok(response);
        }

        
        [HttpPost]
        public async Task<ActionResult<UserDto>> Create([FromBody] CreateUserDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

           

            var user = new User
            {
                Name = request.Name,
                Email = request.Email,
                Phone = request.Phone,
                PasswordHash = request.Password,
                Role = request.Role,
                CreatedAt = DateTime.UtcNow
            };

            await _userRepo.AddAsync(user);
            await _userRepo.CommitAsync();

            var response = new UserDto(
                user.Id,
                user.Name,
                user.Email,
                user.Phone,
                user.Role,
                user.CreatedAt
            );

            return CreatedAtAction(nameof(GetById), new { id = user.Id }, response);
        }

        
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateUserDto request)
        {
            if (id != request.Id)
                return BadRequest("ID mismatch");

            var user = await _userRepo.GetOneAsync(u => u.Id == id);

            if (user == null)
                return NotFound();

           
            user.Name = request.Name;
            user.Phone = request.Phone;
            user.Role = request.Role; 

            _userRepo.Update(user);
            await _userRepo.CommitAsync();

            return NoContent();
        }

        
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userRepo.GetOneAsync(u => u.Id == id);

            if (user == null)
                return NotFound();

            _userRepo.Delete(user);
            await _userRepo.CommitAsync();

            return NoContent();
        }
    }
}