using ASPNetCoreMVCSample.Entities;
using Microsoft.EntityFrameworkCore;

namespace ASPNetCoreMVCSample.Data
{
    public class MyDbContext:DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options) :  base(options) 
        {
            
        }

       public DbSet<Product> Products { get; set; }
       public DbSet<Category> Categories { get; set; }



    }
}
