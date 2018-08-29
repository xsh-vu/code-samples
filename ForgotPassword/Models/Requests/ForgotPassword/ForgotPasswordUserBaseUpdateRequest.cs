using System.ComponentModel.DataAnnotations;

namespace Models.Requests.ForgotPassword
{
    public class ForgotPasswordUserBaseUpdateRequest
    {
        [Required]
        public string Password { get; set; }

        [Required]
        public string GUID { get; set; }
    }
}