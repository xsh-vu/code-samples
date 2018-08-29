using System.ComponentModel.DataAnnotations;

namespace Models.Requests.App
{
    public class AppTokenTypeUpdateRequest : AppTokenTypeAddRequest
    {
        [Required]
        public int Id { get; set; }
    }
}