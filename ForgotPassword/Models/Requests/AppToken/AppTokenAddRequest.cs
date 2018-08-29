using System.ComponentModel.DataAnnotations;

namespace Models.Requests.App
{
    public class AppTokenAddRequest
    {
        [Required]
        public int UserBaseId { get; set; }

        [Required]
        public string Token { get; set; }

        [Required]
        public int AppTokenTypeId { get; set; }
    }
}