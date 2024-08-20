using ASPNetCoreMVCSample.Data;
using ASPNetCoreMVCSample.Entities;
using ASPNetCoreMVCSample.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using X.PagedList.Extensions;

namespace ASPNetCoreMVCSample.Controllers
{
    public class ProductController : Controller
    {
        private readonly MyDbContext _db;

        // Constructor injecting MyDbContext
        public ProductController(MyDbContext db)
        {
            _db = db;
        }

        // Action to display a paginated list of products
        public IActionResult Index(int? page)
        {
            int pageSize = 5; // Number of products per page
            int pageNumber = page ?? 1; // Current page number, default to 1 if null

            var products = _db.Products.ToList(); // Fetch all products
            var productmodellist = products.Select(x => new ProductModel
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                Price = x.Price,
                Quantity = x.Quantity,
            }).ToPagedList(pageNumber, pageSize); // Paginate the list

            return View(productmodellist);
        }

        // Action to display a paginated list of products for visitors
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Visitor(int? page)
        {
            int pageSize = 5; 
            int pageNumber = page ?? 1; 

            var products = _db.Products.ToList(); 
            var productmodellist = products.Select(x => new ProductModel
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                Price = x.Price,
                Quantity = x.Quantity,
            }).ToPagedList(pageNumber, pageSize); 

            return View(productmodellist);
        }

        // GET action to display the Add Product form
        [HttpGet]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public IActionResult Add()
        {
            return View();
        }

        // POST action to handle the submission of a new product
        [HttpPost]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> Add(ProductModel productModel)
        {
            try
            {
                // Save uploaded images and get their paths
                List<string> imagePaths = await SaveImagesToDirectory(productModel);

                // Create a new Product entity
                var productEntity = new Product
                {
                    Name = productModel.Name,
                    Description = productModel.Description,
                    Price = productModel.Price,
                    Quantity = productModel.Quantity,
                };

                // Add the new product to the database
                _db.Products.Add(productEntity);
                await _db.SaveChangesAsync();

                // Save image paths associated with the product
                await SaveProductImagesPathsToDb(imagePaths, productEntity);

                // Return a JSON response indicating success and redirect URL
                return Json(new { success = true, redirectUrl = Url.Action("Index") });
            }
            catch (Exception e)
            {
                
                throw;
            }
        }

        // Private method to save image paths to the database
        private async Task SaveProductImagesPathsToDb(List<string> imagePaths, Product productEntity)
        {
            if (imagePaths.Count > 0)
            {
                foreach (var imagePath in imagePaths)
                {
                    var imageEntity = new ProductImage
                    {
                        ProductId = productEntity.Id,
                        ImagePath = imagePath
                    };

                    _db.ProductImages.Add(imageEntity);
                }

                await _db.SaveChangesAsync();
            }
        }

        // Private method to save images to the server directory
        private static async Task<List<string>> SaveImagesToDirectory(ProductModel productModel)
        {
            List<string> imagePaths = new List<string>();

            if (productModel.Images != null && productModel.Images.Count > 0)
            {
                foreach (var imageFile in productModel.Images)
                {
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        // Generate a unique file name to avoid conflicts
                        var fileName = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", fileName);

                        // Save the image file to the server
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(stream);
                        }

                        // Add the image path to the list
                        imagePaths.Add("images/" + fileName);
                    }
                }
            }

            return imagePaths;
        }

        // GET action to display the Edit Product form
        [HttpGet]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public IActionResult Edit(int id)
        {
            // Find the product by id
            var productEntity = _db.Products.Find(id);

            if (productEntity == null)
            {
                return View("Error"); // Return an error view if product not found
            }

            // Map the product entity to a ProductModel
            var productModel = new ProductModel
            {
                Name = productEntity.Name,
                Description = productEntity.Description,
                Price = productEntity.Price,
                Quantity = productEntity.Quantity,
                Id = productEntity.Id,
                ImagePaths = _db.ProductImages.Where(p => p.ProductId == productEntity.Id)
                                               .Select(i => i.ImagePath)
                                               .ToList()
            };

            return View(productModel);
        }

        // POST action to handle the submission of product updates
        [HttpPost]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> Edit(ProductModel productModel)
        {
            // Save uploaded images and get their paths
            List<string> imagePaths = await SaveImagesToDirectory(productModel);

            // Find the product by id
            var productEntity = _db.Products.Find(productModel.Id);

            if (productEntity == null)
            {
                return View("Error"); 
            }

            // Update the product entity
            productEntity.Name = productModel.Name;
            productEntity.Description = productModel.Description;
            productEntity.Price = productModel.Price;
            productEntity.Quantity = productModel.Quantity;

            _db.Update(productEntity); // Update the product in the database
            await _db.SaveChangesAsync();

            
            await SaveProductImagesPathsToDb(imagePaths, productEntity);

            // Return a JSON response indicating success and redirect URL
            return Json(new { success = true, redirectUrl = Url.Action("Index") });
        }

        // POST action to delete a product
        [HttpPost]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public IActionResult Delete(int id)
        {
            // Find the product by id
            var productEntity = _db.Products.Find(id);

            if (productEntity == null)
            {
                return View("Error"); 
            }

            // Remove the product from the database
            _db.Products.Remove(productEntity);
            _db.SaveChanges();

            // Redirect to the index page after deletion
            return RedirectToAction("Index");
        }
    }
}
