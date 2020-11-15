using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using EmployeeManagement.Utilities;

namespace EmployeeManagement.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [ValidEmailDomainAttribute(allowDomain:"gmail.com",
            ErrorMessage ="Email Domain must be gmail.com")]
        [Remote(action: "IsEmail", controller:"Account")]
        //he following is the model class for the User Registration View. Notice, 
        //we have decorated the Email property with the [Remote]
        //attribute pointing it to the action method that should be invoked when the email value changes.
        public String  Email{ get; set; }
        [Required]
        [DataType(DataType.Password)]
        public String Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password",ErrorMessage ="Password and confirmation password do not match")]
        public String ConfirmPassword { get; set; }
        public string City { get; set; }

    }
}
