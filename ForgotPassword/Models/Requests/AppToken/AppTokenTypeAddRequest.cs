using System.ComponentModel.DataAnnotations;

namespace Models.Requests.App
{
    public class AppTokenTypeAddRequest
    {
        [Required]
        public string TypeName { get; set; }
        public string TypeDescription { get; set; }
    }
}