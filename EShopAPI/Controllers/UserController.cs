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
        private readonly IMapper _mapper;

        public UserController(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
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

        //[HttpGet("members")]
        //public async Task<IActionResult> GetMembers()
        //{
        //    var users = await _userRepository.GetMembers();
        //    return Ok(users);
        //}

        //[HttpGet("member/{id}")]
        //public async Task<IActionResult> GetMemberByID(string id)
        //{
        //    var user = await _userRepository.GetMemberByID(id);
        //    if (user == null)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(user);
        //}

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

        //[HttpPost("insert")]
        //public async Task<IActionResult> InsertMember([FromBody] User member)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    await _userRepository.InsertMember(member);
        //    return Ok();
        //}

        //[HttpPut("update")]
        //public async Task<IActionResult> UpdateMember([FromBody] User member)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    await _userRepository.UpdateMember(member);
        //    return Ok();
        //}
    }
}
