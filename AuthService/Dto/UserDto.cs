namespace AuthService.Dto
{
    public class UserDto
    {
        public int UserID { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
