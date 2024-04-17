using EmployeeDetails.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EmployeeDetails.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly EmployeeDbContext _employeeDbContext;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public EmployeeController(EmployeeDbContext employeeDbContext, IWebHostEnvironment hostingEnvironment)
        {
            _employeeDbContext = employeeDbContext;
            _webHostEnvironment = hostingEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            var employeelist = await _employeeDbContext.Employees.ToListAsync();
            return View(employeelist);
        }
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return RedirectToAction("Index");
                }

                var employe = await _employeeDbContext.Employees.SingleOrDefaultAsync(m => m.Id == id);

                if (employe == null)
                {
                    return RedirectToAction("Index");
                }

                if (employe.City_id != null) // Add null check here
                {
                    employe.City = await _employeeDbContext.Cities.SingleOrDefaultAsync(c => c.Id == employe.City_id);
                }

                return View(employe);
            }
            catch (Exception ex)
            {
                return View("Error");
            }
        }




        [HttpGet]
        public async Task<IActionResult> Create()
        {
            try
            {
                var cities = await _employeeDbContext.Cities.ToListAsync();

                var cityList = cities
                    .OrderBy(u => u.City_Name)
                    .Select(u => new City
                    {
                        Id = u.Id,
                        City_Name = u.City_Name
                    })
                    .ToList();

                ViewBag.CityList = cityList;

                return View();
            }
            catch (Exception ex)
            {
                throw;
            }
        }




        [HttpPost]
        public async Task<IActionResult> Create(Employee employee, IFormFile ResumePath)
        {
            if (ResumePath != null && ResumePath.Length > 0)
            {
                try
                {
                    // Ensure the upload directory exists, create it if it doesn't
                    var uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }

                    // Generate a unique file name to avoid conflicts
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ResumePath.FileName);
                    var filePath = Path.Combine(uploadPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await ResumePath.CopyToAsync(stream);
                    }

                    employee.ResumePath = filePath;
                }
                catch (Exception ex)
                {
                    // Handle any exceptions related to file handling
                    ModelState.AddModelError("", $"An error occurred while uploading the file: {ex.Message}");
                    // Return the view with the model to show error messages
                    return View(employee);
                }
            }

            try
            {
                _employeeDbContext.Employees.Add(employee);
                await _employeeDbContext.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Proper error handling should be implemented
                ModelState.AddModelError("", $"An error occurred while saving employee details: {ex.Message}");
                // Return the view with the model to show error messages
                return View(employee);
            }
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var employee = await _employeeDbContext.Employees
                .FirstOrDefaultAsync(e => e.Id == id);

            if (employee == null)
            {
                return NotFound();
            }

            _employeeDbContext.Employees.Remove(employee);
            await _employeeDbContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Edit(int id)
        {
            var employee = await _employeeDbContext.Employees.FindAsync(id);

            if (employee == null)
            {
                return NotFound();
            }

            var cities = await _employeeDbContext.Cities.ToListAsync();
            var cityList = cities
                          .OrderBy(u => u.City_Name)
                          .Select(u => new City
                          {
                              Id = u.Id,
                              City_Name = u.City_Name
                          })
                          .ToList();

            ViewBag.CityList = cityList;

            var resumePath = employee.ResumePath;

            // Pass the employee model and resumePath to the view
            ViewBag.ResumePath = resumePath;

            return View(employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Employee employee, IFormFile ResumePath)
        {
            if (id != employee.Id)
            {
                return NotFound();
            }
            

            if (ModelState.IsValid || ResumePath!=null)
            {
                try
                {
                    // Check if the employee with the given id exists
                    var existingEmployee = await _employeeDbContext.Employees.FindAsync(id);
                    if (existingEmployee == null)
                    {
                        return NotFound();
                    }

                    // Check if a new resume file is uploaded
                    if (ResumePath != null && ResumePath.Length > 0)
                    {
                        // Handle uploading of new resume file
                        var uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                        if (!Directory.Exists(uploadPath))
                        {
                            Directory.CreateDirectory(uploadPath);
                        }
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ResumePath.FileName);
                        var filePath = Path.Combine(uploadPath, fileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await ResumePath.CopyToAsync(stream);
                        }
                        employee.ResumePath = filePath;
                    }

                    // Update other properties of the employee object
                    _employeeDbContext.Entry(existingEmployee).CurrentValues.SetValues(employee);
                    await _employeeDbContext.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Log the error or handle it appropriately
                    throw;
                }
            }

            // If ModelState is not valid, return the view with validation errors
            return View(employee);
        }
      }
    }


