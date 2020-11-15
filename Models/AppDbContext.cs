using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Models
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {   
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)              
            //base class constractor for DbContext
        {

         }

        public DbSet<Employee> Employees { get; set; }
        /*
         * Foreign key with Cascade DELETE

            In Entity Framework Core, by default the foreign keys in AspNetUserRoles table have Cascade DELETE behaviour.
            This means, if a record in the parent table (AspNetRoles) is deleted, then the corresponding records in the child table
            (AspNetUserRoles ) are automatically be deleted.

            Foreign key with NO ACTION ON DELETE

            What if you want to customise this default behaviour. We do not want to allow a role to be deleted, 
            if there are rows in the child table (AspNetUserRoles) which point to a role in the parent table (AspNetRoles).

            To achieve this, modify foreign keys DeleteBehavior to Restrict. We do this in OnModelCreating() 
            method of AppDbContext class
*/
        protected override void OnModelCreating(ModelBuilder modelBuilder) 
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Seed();

            foreach (var foreignKey in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }
        
    }
}






