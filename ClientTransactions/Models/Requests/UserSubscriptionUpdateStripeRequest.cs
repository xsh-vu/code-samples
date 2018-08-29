using System.ComponentModel.DataAnnotations;


namespace Models.Requests
{
    public class UserSubscriptionUpdateStripeRequest
    {
        [Required]
        public string SourceTokenOrExistingSourceId { get; set; }
        [Required]
        public string CustomerId { get; set; }
    }
}
