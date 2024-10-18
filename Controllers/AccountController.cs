using CMCS.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using SystemSecurityClaims = System.Security.Claims;

namespace CMCS.Controllers
{
    public class AccountController : Controller
    {
        private readonly YourDbContext _context;

        public AccountController(YourDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Register()
        {
            //return View();
            var model = new RegisterViewModel(); // or appropriate model for registration
            return View(model); // This should match the model type used in the view
        }
        [HttpPost]
        public IActionResult Register(RegisterViewModel model) // Change to RegisterViewModel
        {
            // Log user details (avoiding sensitive info like passwords)
            Console.WriteLine($"Registering user: {model.Username}, Email: {model.Email}, Role: {model.Role}");

            if (ModelState.IsValid)
            {
                // Create a new User from the model
                var user = new User
                {
                    Username = model.Username,
                    Email = model.Email,
                    Password = model.Password, // Make sure to hash the password before saving
                    Role = model.Role,
                    Departments = model.Departments // Assuming you're storing the department in the User model
                };

                // Add the user to the Users table
                _context.Users.Add(user);
                try
                {
                    // Save changes to get the user ID
                    _context.SaveChanges();

                    // Check if the user is a lecturer and insert into the Lecturer table
                    if (user.Role == "Lecturer")
                    {
                        // Make sure department info is provided
                        if (user.Departments == null)
                        {
                            ModelState.AddModelError("", "Department is required for Lecturer role.");
                            return View(model); // Return the same model back to the view
                        }

                        var lecturer = new Lecturer
                        {
                            Department = user.Departments, // Assuming correct mapping
                            UserId = user.UserId // Use the ID from the saved user
                        };

                        // Add the lecturer to the Lecturer table
                        _context.Lecturers.Add(lecturer);
                        _context.SaveChanges();
                    }

                    return RedirectToAction("Login");
                }
                catch (DbUpdateException dbEx)
                {
                    // Log the exception
                    Console.WriteLine($"Database error: {dbEx.Message}");
                    ModelState.AddModelError("", "Error saving to the database. Please check your inputs.");
                }
                catch (Exception ex)
                {
                    // Log any other exceptions
                    Console.WriteLine($"Error saving user: {ex.Message}");
                    ModelState.AddModelError("", "Unexpected error occurred. Please try again.");
                }
            }
            return View(model); // Return the model back to the view in case of error
        }


        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(User loginUser) // Change to async Task<IActionResult>
        {
            var user = _context.Users
                .FirstOrDefault(u => u.Email == loginUser.Email && u.Username == loginUser.Username && u.Password == loginUser.Password);

            if (user != null)
            {
                // Create claims for the authenticated user
                var claims = new List<SystemSecurityClaims.Claim>
        {
            new SystemSecurityClaims.Claim(SystemSecurityClaims.ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new SystemSecurityClaims.Claim(SystemSecurityClaims.ClaimTypes.Name, user.Username) // Assuming you want to use the username
        };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                // Handle successful login
                return RedirectToAction("Index", "Home"); // Redirect to your main page
            }

            ModelState.AddModelError("", "Invalid login attempt.");
            return View(loginUser);
        }
    }
}
