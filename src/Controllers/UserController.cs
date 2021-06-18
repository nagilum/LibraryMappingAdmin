using DotNetLibraryAdmin.Controllers.Payloads;
using DotNetLibraryAdmin.Database;
using DotNetLibraryAdmin.Database.Tables;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace DotNetLibraryAdmin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        /// <summary>
        /// Attempt to log the user in.
        /// </summary>
        /// <param name="payload">User credentials.</param>
        /// <returns>User token.</returns>
        [HttpPost]
        [Route("login")]
        public async Task<ActionResult> Login(UserLoginRequestPayload payload)
        {
            User user;

            try
            {
                user = await new DatabaseContext()
                    .Users
                    .FirstOrDefaultAsync(n => n.Username == payload.Username &&
                                              !n.Deleted.HasValue);

                if (user == null)
                {
                    throw new UnauthorizedAccessException();
                }
            }
            catch
            {
                return this.Unauthorized();
            }

            if (!BCrypt.Net.BCrypt.Verify(payload.Password, user.Password))
            {
                return this.Unauthorized();
            }

            return this.Ok(new
            {
                token = user.GetToken()
            });
        }

        /// <summary>
        /// Create a new user.
        /// </summary>
        /// <param name="payload">User credentials and secret.</param>
        /// <returns>Success.</returns>
        [HttpPost]
        [Route("create")]
        public async Task<ActionResult> Create(UserCreateRequestPayload payload)
        {
            // Check that the secret is correct.
            var secret = Config.Get("secret");

            if (secret == null)
            {
                return this.BadRequest(new
                {
                    message = "Config is missing."
                });
            }

            if (payload.Secret != secret)
            {
                return this.Unauthorized();
            }

            // Check that both username and password is included.
            if (payload.Username == null ||
                payload.Password == null)
            {
                return this.BadRequest(new
                {
                    message = "Both 'username' and 'password' are required."
                });
            }

            User user;

            await using var db = new DatabaseContext();

            try
            {
                user = await db.Users
                    .FirstOrDefaultAsync(n => n.Username == payload.Username &&
                                              !n.Deleted.HasValue);

                if (user != null)
                {
                    throw new Exception("Username unavailable.");
                }
            }
            catch (Exception ex)
            {
                return this.BadRequest(new
                {
                    message = ex.Message
                });
            }

            try
            {
                user = new User
                {
                    Created = DateTimeOffset.Now,
                    Updated = DateTimeOffset.Now,
                    Username = payload.Username,
                    Password = BCrypt.Net.BCrypt.HashPassword(payload.Password)
                };

                await db.Users.AddAsync(user);
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return this.BadRequest(new
                {
                    message = ex.Message
                });
            }

            return this.Ok();
        }
    }
}