using System.ComponentModel;

namespace ASPNetCoreMVCSample.Models
{
    public class ProductModel
    {
        public int Id { get; set; } 
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal  Price { get; set; }
        public int Quantity { get; set; }
        public List<string> ImagePaths { get; set; }
        [DisplayName("Images")]
        public List<IFormFile> Images { get; set; } // Property for image uploads
    }
}
