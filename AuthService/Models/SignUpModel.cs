namespace AuthService.Models
{
    public class SignUpModel
    {
        public required string Username { get; set; }
        public required string PasswordHash { get; set; }
        public required string Email { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
    }
}
