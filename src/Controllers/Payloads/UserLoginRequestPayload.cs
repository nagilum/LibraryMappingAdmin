namespace DotNetLibraryAdmin.Controllers.Payloads
{
    public class UserLoginRequestPayload
    {
        /// <summary>
        /// User's username.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// User's password.
        /// </summary>
        public string Password { get; set; }
    }
}