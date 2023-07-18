using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoreApi.DAL;
using StoreApi.Data;
using StoreApi.DTO;
using StoreApi.Models;
using System.Security.Claims;

namespace StoreApi.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly StoreDbContext context;
        public UserController(StoreDbContext dbContext)
        {
            context = dbContext;
        }

        //Get endpoint to fetch all the users from the db
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            using (var userRepository = new UserRepository(context))
            {
                var users = await userRepository.GetAll();
                return Ok(users);
            }
        }

        //Get endpoint to fetch a unique user by id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            using (var userRepository = new UserRepository(context))
            {
                User? users = await userRepository.GetById(id);
                if (users != null)
                {
                    return Ok(users);
                }
                else
                {
                    return NotFound("Product not found");
                }
            }
        }

        //Get endpoint to fetch a unique user by id when is authenticated
        [HttpGet("memberaccount")]
        [Authorize]
        public async Task<IActionResult> GetUserById()
        {
            var stringId = User?.FindFirstValue(ClaimTypes.NameIdentifier);
            int jwtId = 0;

            if (stringId != null)
            {
                jwtId = Int32.Parse(stringId);
            }

            if (await context.Users.FindAsync(jwtId) != null)
            {
                using (var userRepository = new UserRepository(context))
                {
                    User? users = await userRepository.GetById(jwtId);
                    if (users != null)
                    {
                        return Ok(users);
                    }
                    else
                    {
                        return NotFound("Product not found");
                    }
                }
            }
            else
            {
                return BadRequest("Error with the JWT");
            }
        }
        //Post endpoint to Add a user into the DB
        [HttpPost]
        public async Task<IActionResult> PostUser(UserDTO request)
        {
            using (var userRepository = new UserRepository(context))
            {
                var newUser = await userRepository.Insert(request);
                if (newUser != null)
                {
                    return Created("Added", request);
                }
                else
                {
                    return BadRequest("Error creating the new user, please verify the information");
                }
            }
        }
        //Put endpoint to update the data stored from an user
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, UserDTO request)
        {
            using (var userRepository = new UserRepository(context))
            {
                User? user = await userRepository.Update(id, request);
                if (user != null)
                {
                    return Ok(user);
                }
                else
                {
                    return BadRequest("Error editing the sser");
                }
            }
        }
        //Put endpoint to update the data stored from an user
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            using (var userRepository = new UserRepository(context))
            {
                if (await userRepository.Delete(id))
                {
                    return Ok();
                }
                else
                {
                    return NotFound("User not found");
                }
            }
        }
    }
}
