using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthService.Models
{
    public class ResetPassword
    {
        [Key]
        public int Id { get; set; }                   // Khóa chính, định danh mã xác nhận

        [Required]
        public int UserId { get; set; }               // Khóa ngoại, ID của người dùng

        [Required]
        [StringLength(100)]
        public string Token { get; set; }             // Mã xác nhận được gửi qua email

        [Required]
        public DateTime ExpirationDate { get; set; }  // Thời gian mã xác nhận hết hạn

        [Required]
        public DateTime CreatedAt { get; set; }       // Thời gian tạo mã xác nhận

        [Required]
        public bool Used { get; set; }                // Trạng thái mã xác nhận đã được sử dụng hay chưa

        // Tham chiếu đến đối tượng người dùng liên kết
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}
