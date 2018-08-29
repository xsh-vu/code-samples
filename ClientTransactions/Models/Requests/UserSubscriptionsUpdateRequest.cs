using System.ComponentModel.DataAnnotations;


namespace Models.Requests
{
    public class UserSubscriptionsUpdateRequest
    {
        [Required]
        public string CustomerId { get; set; }
        [Required]
        public string CurrentCard { get; set; }
        [Required]
        public int CardExpMonth { get; set; }
        [Required]
        public int CardExpYear { get; set; }
    }
}
