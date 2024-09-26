namespace AuthService.Models
{
    public class UpdateUserModel
    {
        public int UserID { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
    }
}
