using System;

namespace Models.Domain.App
{
    public class AppToken
    {
        public int UserBaseId { get; set; }
        public string Token { get; set; }
        public int AppTokenTypeId { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}