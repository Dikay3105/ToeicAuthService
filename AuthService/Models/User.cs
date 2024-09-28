namespace AuthService.Models
{
    public class User
    {
        public int UserID { get; set; }
        public required string Username { get; set; }
        public required string PasswordHash { get; set; }
        public string lastPasswordChange { get; set; }
        public required string Salt { get; set; } // Add this line
        public required string Email { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
