using FIrstMVCWebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace FIrstMVCWebApp.Data
{
        public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
        {   
            public DbSet<User> Users { get; set; }
            public DbSet<Employee> Employees { get; set; }
        } 
}
