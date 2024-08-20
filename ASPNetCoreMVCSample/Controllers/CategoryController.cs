using ASPNetCoreMVCSample.Data;
using ASPNetCoreMVCSample.Entities;
using ASPNetCoreMVCSample.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using X.PagedList.Extensions;

namespace ASPNetCoreMVCSample.Controllers
{
    public class CategoryController : Controller
    {
        private readonly MyDbContext _db;

        // Constructor injecting MyDbContext
        public CategoryController(MyDbContext db)
        {
            _db = db;
        }

       
        public IActionResult Index()
        {
            // Fetch all categories from the database
            var catg = _db.Categories.ToList();

            // Map to a list of Category models
            var Categorylist = catg.Select(x => new Categories
            {
                Id = x.Id,
                Name = x.Name,
            }).ToList();

            // Return the view with the list of categories
            return View(Categorylist);
        }

        
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Visitor(int? page)
        {
            int pageSize = 5; // Number of categories per page
            int pageNumber = page ?? 1; 

            // Fetch and paginate the list of categories
            var categories = _db.Categories
                .Select(x => new Categories
                {
                    Id = x.Id,
                    Name = x.Name
                })
                .ToPagedList(pageNumber, pageSize); // Paginate the list

            // Return the view with the paginated list
            return View(categories);
        }

        // GET action to display the Add Category form
        [HttpGet]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public IActionResult Add()
        {
            return View();
        }

        
        [HttpPost]
        [Authorize(Roles = "Admin, SuperAdmin")]
        public IActionResult Add(Categories categories)
        {
            // Check if the model state is valid
            if (ModelState.IsValid)
            {
                // Create a new Category entity
                var categoryEntity = new Category
                {
                    Name = categories.Name,
                };

                // Add the new category to the database
                _db.Categories.Add(categoryEntity);
                _db.SaveChanges();

                // Redirect to the Index action after successful addition
                return RedirectToAction("Index");
            }

            // If the model state is invalid, return the view with the same model to show validation errors
            return View(categories);
        }

        // POST action to delete a category
        [HttpPost]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public IActionResult Delete(int id)
        {
            // Find the category by id
            var categoryEntity = _db.Categories.Find(id);

            if (categoryEntity == null)
            {
                // Return an error view if the category is not found
                return View("Error");
            }

            // Remove the category from the database
            _db.Categories.Remove(categoryEntity);
            _db.SaveChanges();

            // Redirect to the Index action after successful deletion
            return RedirectToAction("Index");
        }
    }
}
