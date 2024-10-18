using AutoMapper;
using BusinessObject.Models;
using BusinessObject.ResourceModel.ViewModel.User;
using DataAccess.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EShopAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginVM model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userRepository.Login(model);
            if (user == null)
            {
                return Unauthorized("Username or password is incorrect!");
            }

            return Ok(user);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterVM model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userRepository.Register(model);
            if (user == null)
            {
                return BadRequest("User has been already existed or error when creating user!");
            }

            return Ok(user);
        }

        [HttpGet]
        public async Task<IActionResult> GetUser()
        {
            var users = await _userRepository.GetUsers();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserByID(string id)
        {
            var user = await _userRepository.GetUserByID(id);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        //[HttpGet("member")]
        //public async Task<IActionResult> GetMemberByEmailAndPass([FromQuery] string email, [FromQuery] string password)
        //{
        //    var user = await _userRepository.GetMemberByEmailAndPass(email, password);
        //    if (user == null)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(user);
        //}

        [HttpPut("update")]
        public async Task<IActionResult> UpdateUser([FromBody] UserUpdateVM user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var updatedUser = await _userRepository.UpdateUser(user);
            if (updatedUser == null)
            {
                return BadRequest();
            }
            return Ok(updatedUser);
        }
    }
}
