namespace WebAuthentication.Modules
{
    public interface IPasswordHasher
    {
        string HashPassword(string password);

        bool Verify(string password, string hash);
    }

    public class PasswordHasher : IPasswordHasher
    {
        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, GenerateSalt());
        }

        public bool Verify(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }

        private static string GenerateSalt()
        {
            return BCrypt.Net.BCrypt.GenerateSalt(10);
        }
    }
}