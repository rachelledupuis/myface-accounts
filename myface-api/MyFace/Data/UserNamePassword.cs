namespace MyFace.Data
{
    public class UserNamePassword
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public UserNamePassword(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }
}