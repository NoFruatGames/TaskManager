using TransferDataTypes.Messages;
namespace TMServer.Data
{
    internal class Profile
    {
        private static int id = 0;
        public int Id { get; init; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public Profile()
        {
            ++id;
            Id = id;
        }

        public Profile(CreateAccountMessage message)
        {
            Username = message.Username;
            Password = message.Password;
            Email = message.Email;
            ++id;
            Id = id;
        }
    }
}
