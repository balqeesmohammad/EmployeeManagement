using System;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using EmployeeManagement.Models;
using EmployeeManagement.ViewModels;
using Microsoft.AspNetCore.DataProtection;
using EmployeeManagement.Security;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EmployeeManagement.Controllers
{
    // [Route("[Controller]/[action]")]
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IEmployeeRepository _employeeRepository;//1
        private readonly AppDbContext context;
       
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IDataProtector protector;

        // private readonly IHostingEnvironment hostingEnvironment;
        private readonly ILogger<HomeController> logger;

        public HomeController(IEmployeeRepository employeeRepository,
                                AppDbContext context,
                                IWebHostEnvironment webHostEnvironment,
                          //injecte __inface__ dependency //2///***/ IHosting Environment this services using to store photo in images file 
                              ILogger<HomeController> logger,
                              // to encryption  
                              IDataProtectionProvider dataProtectionProvider,
                              DataProtectionPurposeStrings dataProtectionPurposeStrings)
        {
            
            _employeeRepository = employeeRepository;
            this.context = context;
            this.webHostEnvironment = webHostEnvironment;
          //  this.hostingEnvironment = hostingEnvironment;
            this.logger = logger;
            /// to encryption 
            protector = dataProtectionProvider.CreateProtector(dataProtectionPurposeStrings.EmployeeIdRouteValue);

        }//1+2= this is constructor injection 
        /*
        * **
        * 
        * 
        *  public HomeController()//injecte __inface__ dependency //2
       {
           _employeeRepository = new MockEmployeeRepository();

       }*/

        //  [Route("~/Home")]
        //  [Route("~/")]
        //[Route("Home/index")]

        [AllowAnonymous]

        public ViewResult Index()
        {
            // return Json(new { id = 1 ,name="balqees" });

            var model = _employeeRepository.GetAllEmployee()
                // to encryption 
                .Select(e => 
                {
                    e.EncryptedId = protector.Protect(e.Id.ToString());
                    return e;
                });
            // end encryption 
            return View(model);
        }
        // [Route("{id?}")]
        [AllowAnonymous]

        public ViewResult Details(string Id)
        {
            //throw new Exception("Error in details view ");
            logger.LogTrace("trace");
            logger.LogDebug("debugg");
            logger.LogInformation("information");
            logger.LogWarning("warning");
            logger.LogError("Error");
            logger.LogCritical("critical");


           int employeeId = Convert.ToInt32(protector.Unprotect(Id));


            Employee employee = _employeeRepository.GetEmployee(employeeId);
            if (employee == null)
            {
                Response.StatusCode = 404;
                return View("EmployeeNotFound", employeeId);
            }
            HomeDetailsViewModel homeDetailsViewModel = new HomeDetailsViewModel()
            {
                Employee = employee,
                PageTitle = "Employee Details page "
            };



            // return Json(new { id = 1 ,name="balqees" });

            //   Employee model = _employeeRepository.GetEmployee(3);
            ////   ViewBag.Employee = model;
            //  ViewBag.EmployeeTitle = "Employee Details ";


            //ViewData["Employee"] = model;
            //ViewData["EmployeeTitle"] = "Employee Details ";
            return View(homeDetailsViewModel);

        }


       
        [HttpGet]
        public ViewResult Create()
        {
            return View();
        }

        [HttpGet]

        public ViewResult Edit(int id)
        {
            Employee employee = _employeeRepository.GetEmployee(id);
            EmployeeEditViewModel employeeEditViewModel = new EmployeeEditViewModel
            {
                Id = employee.Id,
                Name = employee.Name,
                Email = employee.Email,
                Department = employee.Department,
                ExistingPhotoPath = employee.PhotoPath

            };
            return View(employeeEditViewModel);
        }

        [HttpPost]
        public IActionResult Edit(EmployeeEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                Employee employee = _employeeRepository.GetEmployee(model.Id);
                employee.Name = model.Name;
                employee.Email = model.Email;
                employee.Department = model.Department;
                if (model.Photos != null)
                {
                    if (model.ExistingPhotoPath !=null)
                    {
                        string filePath = Path.Combine(webHostEnvironment.WebRootPath, "images",
                            model.ExistingPhotoPath);
                        System.IO.File.Delete(filePath);
                    }
                    employee.PhotoPath = ProcessUploadFile(model);
                }
            
                _employeeRepository.Update(employee);
                return RedirectToAction("index");
            }
            return View();
        }

        private string ProcessUploadFile(EmployeeCreateViewModel model)
        {
            String uniqueFileName = null;

            if (model.Photos != null && model.Photos.Count > 0)
            {
                foreach (var photo in model.Photos)
                {
                    string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "images");
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + photo.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        photo.CopyTo(fileStream);
                    }
                    
                }
            }

            return uniqueFileName;
        }

        //public async Task<IActionResult> DeleteEmployee(int id, bool? saveChangesError = false)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    Employee employee = _employeeRepository.GetEmployee(id);

        //    //var student = await _context.Students
        //    //    .AsNoTracking()
        //    //    .FirstOrDefaultAsync(m => m.ID == id);
        //    if (employee == null)
        //    {
        //        return NotFound();
        //    }

        //    if (saveChangesError.GetValueOrDefault())
        //    {

        //        // ViewBag.ErrorMessage = "";

        //        ViewData["ErrorMessage"] =
        //            "Delete failed. Try again, and if the problem persists " +
        //            "see your system administrator.";
        //    }

        //    return View(employee);
        //}



        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            try
            {
                Employee employee = _employeeRepository.GetEmployee(id);
                context.Remove(employee);
                await context.SaveChangesAsync();
                return View();

               
            }
            catch (DbUpdateException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                return RedirectToAction(nameof(Index), new { id = id, saveChangesError = true });
            }
        }





       

        [HttpPost]

        public IActionResult Create(EmployeeCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                String uniqueFileName = ProcessUploadFile(model);
                //String uniqueFileName = null;
                //// If the Photos property on the incoming model object is not null and if count > 0,
                //// then the user has selected at least one file to upload

                //if (model.Photos!=null&& model.Photos.Count>0)
                //{// Loop thru each selected file
                //    foreach (var photo in model.Photos)
                //    {
                //        // The file must be uploaded to the images folder in wwwroot
                //        // To get the path of the wwwroot folder we are using the injected
                //        // IHostingEnvironment service provided by ASP.NET Core
                //        string uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "images");//store photo in images folder
                //        // To make sure the file name is unique we are appending a new
                //        // GUID value and and an underscore to the file name
                //        uniqueFileName = Guid.NewGuid().ToString() + "_" + photo.FileName;   //to unique name for each photo using Guid class 
                //        string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                //        // Use CopyTo() method provided by IFormFile interface to
                //        // copy the file to wwwroot/images folder
                //        photo.CopyTo(new FileStream(filePath, FileMode.Create));
                //    }

                //}

                Employee newEmployee = new Employee
                {
                    Name = model.Name,
                Email = model.Email,
                Department =model.Department,
                PhotoPath = uniqueFileName
                };
                _employeeRepository.Add(newEmployee);
                return RedirectToAction("details", new { id = newEmployee.Id });
            }
            return View();
        }
    }   
}
