using ASPNetCoreMVCSample.Data;
using ASPNetCoreMVCSample.Entities;
using ASPNetCoreMVCSample.Models;
using Microsoft.AspNetCore.Mvc;

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
                Name = x.Name,


            }).ToList();



            return View(Categorylist);
        }
        [HttpGet]
        public IActionResult Add() 
        {
            return View();
        }
        [HttpPost]
        public IActionResult Add(Categories categories) 
        {
            var CategoryEntities = new Category
            {
                Name = categories.Name,
            };
            _db.Categories.Add(CategoryEntities);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

    }
}
