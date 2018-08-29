using System.ComponentModel.DataAnnotations;

namespace Models.Requests.ForgotPassword
{
    public class ForgotPasswordAppTokenAddRequest
    {
        [Required]
        public string Email { get; set; }
    }
}