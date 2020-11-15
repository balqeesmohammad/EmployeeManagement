using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Models
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>().HasData( 
            new Employee
            {
                Id =1,
                Name = "Balqees",
                Email ="Balbal@gmail.com",
                Department= Dept.IT
            }
            
            );
        }
    }
}
