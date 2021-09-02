namespace TxSpareParts.Core.Entities
{
    public class Star
    {
        public string ID { get; set; }
        public string CompanyID { get; set; }
        public string UserID { get; set; }
        public virtual Company Company { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }     
    }
}