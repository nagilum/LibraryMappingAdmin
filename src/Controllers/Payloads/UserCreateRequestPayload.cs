namespace DotNetLibraryAdmin.Controllers.Payloads
{
    public class UserCreateRequestPayload
    {
        /// <summary>
        /// User's username.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// User's password.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Secret password.
        /// </summary>
        public string Secret { get; set; }
    }
}