using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EmployeeManagement.Controllers
{
    public class DepartmentController : Controller
    {
        // GET: /<controller>/
        public string List()
        {
            return "List";
        }
        public string Details()
        {
            return "details";
        }
    }
}
