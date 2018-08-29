using Data.Structured;
using System.ComponentModel.DataAnnotations;


namespace Models.Requests
{
    public class UserSubscriptionUpdateMultiLock
    {
        [Required]
        [GenericTable(System.Data.SqlDbType.Int, "Id")]
        public int Id { get; set; }
    }
}
