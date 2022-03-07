namespace MyFace.Data
{
    public class HashedPasswordSalt
    {
        public string HashedPassword { get; set; }
        public string Salt { get; set; }

        public HashedPasswordSalt(string hashedPassword, string salt)
        {
            HashedPassword = hashedPassword;
            Salt = salt;
        }
    }
}