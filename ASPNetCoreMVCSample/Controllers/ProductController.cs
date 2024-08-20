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
        MyDbContext _db;

        public ProductController(MyDbContext db)
        {
            _db = db;
        }

        public IActionResult Index(int? page)
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

            }).ToPagedList(pageNumber,pageSize );

            return View(productmodellist);
        }
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

        [HttpGet]
        [Authorize(Roles = "Admin,SuperAdmin")]

        public IActionResult Add()
        {
            return View();
        }
        [HttpPost]
        [Authorize(Roles = "Admin,SuperAdmin")]

        public async Task<IActionResult> Add(ProductModel productModel)
        {
            try
			{
				List<string> imagePaths = await SaveImagesToDirectory(productModel);
				var productEntity = new Product
				{
					Name = productModel.Name,
					Description = productModel.Description,
					Price = productModel.Price,
					Quantity = productModel.Quantity,
				};
				_db.Products.Add(productEntity);
				_db.SaveChanges();
				await SaveProductImagesPathsToDb(imagePaths, productEntity);
				//return RedirectToAction("Index");
				return Json(new { success = true, redirectUrl = Url.Action("Index") });
			}
			catch (Exception e)
            {
                throw;
            }
               
            
        }

		private async Task SaveProductImagesPathsToDb(List<string> imagePaths, Product productEntity)
		{
			// Associate the uploaded image file paths with the product
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

		private static async Task<List<string>> SaveImagesToDirectory(ProductModel productModel)
		{
			// Create a list to store the file paths associated with the product
			List<string> imagePaths = new List<string>();

			// Handle the uploaded images
			if (productModel.Images != null && productModel.Images.Count > 0)
			{
				foreach (var imageFile in productModel.Images)
				{
					if (imageFile != null && imageFile.Length > 0)
					{
						// Save the image to a location of your choice, e.g., a folder on the server
						// You can generate a unique file name to avoid overwriting existing images
						var fileName = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
						var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", fileName);

						using (var stream = new FileStream(filePath, FileMode.Create))
						{
							await imageFile.CopyToAsync(stream);
						}

						// Store the file path or other information in your database for reference
						// You can associate the file with the product being added
						// Store the file path in the list
						imagePaths.Add("images/" + fileName);
					}
				}
			}

			return imagePaths;
		}

		[HttpGet]
        [Authorize(Roles = "Admin,SuperAdmin")]

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
            productModel.ImagePaths= _db.ProductImages.Where(p=>p.ProductId==productModel.Id)
                .Select(i=>i.ImagePath).ToList();
            
            return View(productModel);
        
        }
        [HttpPost]
        [Authorize(Roles = "Admin,SuperAdmin")]

        public async Task<IActionResult> Edit(ProductModel productModel) 
        {
			List<string> imagePaths = await SaveImagesToDirectory(productModel);
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
			await SaveProductImagesPathsToDb(imagePaths, productEntity);
			//return RedirectToAction("Index");
			return Json(new { success = true, redirectUrl = Url.Action("Index") });
			
        }
        [HttpPost]
        [Authorize(Roles = "Admin,SuperAdmin")]

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
