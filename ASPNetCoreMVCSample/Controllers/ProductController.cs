using ASPNetCoreMVCSample.Data;
using ASPNetCoreMVCSample.Entities;
using ASPNetCoreMVCSample.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace ASPNetCoreMVCSample.Controllers
{
    public class ProductController : Controller
    {
        MyDbContext _db;

        public ProductController(MyDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {

            var products = _db.Products.ToList();
            var productmodellist = products.Select(x => new ProductModel
            {
                Id = x.Id,  
                Name = x.Name,
                Description = x.Description,
                Price = x.Price,
                Quantity = x.Quantity,

            }).ToList();
            return View(productmodellist);
        }
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Add(ProductModel productModel)
        {
            var productentities = new Product
            {
                Name = productModel.Name,
                Description = productModel.Description,
                Price = productModel.Price,
                Quantity = productModel.Quantity,
            };
           _db.Products.Add(productentities);
            _db.SaveChanges();
            return RedirectToAction("Index");   
            
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            //look for product with this id
            var productEntity = _db.Products.Find(id);

            /*if this product is not found with that id
            return not found*/
            if (productEntity==null)
            {
                return View("Error");
            }
            //copy from product Entity to product model
            var productModel = new ProductModel();
            productModel.Name= productEntity.Name;
            productModel.Description= productEntity.Description;
            productModel.Price= productEntity.Price;
            productModel.Quantity= productEntity.Quantity;
            productModel.Id= productEntity.Id;
            
            return View(productModel);
        
        }
        [HttpPost]
        public IActionResult Edit(ProductModel productModel) 
        {
            //look for product with this id
            var productEntity = _db.Products.Find(productModel.Id);

            /*if this product is not found with that id
            return not found*/
            if (productEntity == null)
            {
                return View("Error");
            }
            //lets update it
            productEntity.Name = productModel.Name;
            productEntity.Description = productModel.Description;
            productEntity.Price = productModel.Price;
            productEntity.Quantity = productModel.Quantity;
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult Delete(int id) 
        {
            var productEntity = _db.Products.Find(id);

            if (productEntity == null)
            {
                return View("Error");
            }
            _db.Products.Remove(productEntity);
            _db.SaveChanges();
            return RedirectToAction("Index");

        }


    }
}
