namespace TxSpareParts.Core.Entities
{
    public partial class Like
    {
        public string Id { get; set; }
        public string productID { get; set; }
        public string userID { get; set; }        
        public virtual ApplicationUser ApplicationUser { get; set; }
        public virtual Product Product { get; set; }
    }
}