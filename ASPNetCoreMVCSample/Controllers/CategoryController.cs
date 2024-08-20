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
        MyDbContext _db;

        public CategoryController(MyDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var catg = _db.Categories.ToList();
            var Categorylist = catg.Select(x => new Categories
            {
                Id = x.Id,
                Name = x.Name,

            }).ToList();

            return View(Categorylist);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Visitor(int? page)
        {
            int pageSize = 5;
            int pageNumber = page ?? 1;

            var categories = _db.Categories
                .Select(x => new Categories
                {
                    Id = x.Id,
                    Name = x.Name
                })
                .ToPagedList(pageNumber, pageSize);

            return View(categories);
        }

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
            if (ModelState.IsValid)
            {
                var categoryEntity = new Category
                {
                    Name = categories.Name,
                };
                _db.Categories.Add(categoryEntity);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            // If the model state is invalid, return the view with the same model to show validation errors
            return View(categories);
        }

    }
}