using ASPNetCoreMVCSample.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace ASPNetCoreMVCSample.Controllers
{
    public class ProductController : Controller
    {
        public IActionResult Index()
        {
            List<ProductModel> products = new List<ProductModel> 
            {
               new ProductModel 
               {
                  Name = "indomie",
                  Description="instance noodles",
                  Price= 234.43m,
                  Quantity= 10
               },
                new ProductModel 
               {
                  Name = "popcorn",
                  Description="creamy flakes",
                  Price= 200.99m,
                  Quantity= 4
               },
                new ProductModel 
               {
                  Name = "milk",
                  Description="diary new",
                  Price= 567.10m,
                  Quantity= 26
               }



            };
            
            return View(products);
        }
    }
}
