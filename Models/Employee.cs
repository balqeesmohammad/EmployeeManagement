using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Models
{
    public class Employee
    {
        public int Id { get; set; }

        //to hold encryption value 
        [NotMapped]
        public string EncryptedId { get; set; }


        [Required, MaxLength(50, ErrorMessage = "Name cannot exceed 50 characters")]
        [MinLength(5,ErrorMessage ="the name must a bove of 5 digits")]
        public  String Name { get; set; }

        [Display(Name = "Office Email")]
        [RegularExpression(@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$",ErrorMessage = "Invalid email format")]
        [Required]
        public string Email { get; set; }
        [Required]
        public Dept?  Department { get; set; }
        public String PhotoPath { get; set; }

    }
}
