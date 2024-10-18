using CMCS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CMCS.Controllers
{
    public class AdminController : Controller
    {
        private readonly YourDbContext _context;

        public AdminController(YourDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [AllowAnonymous] // Allow unauthenticated access to this action
        public IActionResult AdminRegister()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous] // Allow unauthenticated access to this action
        public async Task<IActionResult> Register(AdminRegisterViewModel model)
        {
            Console.WriteLine($"Registering admin: Username: {model.Username}, Email: {model.Email}, Role: {model.Role}");

            if (ModelState.IsValid)
            {
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == model.Email || u.Username == model.Username);

                if (existingUser != null)
                {
                    if (existingUser.Email == model.Email)
                        ModelState.AddModelError("Email", "Email already exists.");
                    if (existingUser.Username == model.Username)
                        ModelState.AddModelError("Username", "Username already exists.");
                    return View(model);
                }

                var newUser = new User
                {
                    Username = model.Username,
                    Email = model.Email,
                    Password = model.Password, // TODO: Hash this password
                    Role = model.Role,
                    Departments = "Admin"
                };

                try
                {
                    await _context.Users.AddAsync(newUser);
                    await _context.SaveChangesAsync(); // Save changes to get the user ID

                    // Create a new Admin entity linked to the new user
                    var newAdmin = new Admin
                    {
                        Role = model.Role,
                        UserId = newUser.UserId // Use the ID from the newly created user
                    };

                    await _context.Admins.AddAsync(newAdmin);
                    await _context.SaveChangesAsync(); // Save the new Admin entry

                    return RedirectToAction("Login", "Admin");
                }
                catch (DbUpdateException dbEx)
                {
                    Console.WriteLine($"Database error: {dbEx.Message}");
                    ModelState.AddModelError("", "Error saving to the database. Please check your inputs.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error saving user: {ex.Message}");
                    ModelState.AddModelError("", "Unexpected error occurred. Please try again.");
                }
            }

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous] // Allow unauthenticated access to this action
        public IActionResult AdminLogin()
        {
            return View("AdminLogin"); // Ensure this matches the view name exactly
        }

        [HttpPost]
        [AllowAnonymous] // Allow unauthenticated access to this action
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Retrieve user based on email
                var user = await _context.Users
                    .Where(u => u.Email == model.Email)
                    .FirstOrDefaultAsync();

                // Check if the user exists and handle password comparison
                if (user != null)
                {
                    // Ensure to handle potential nulls here
                    string password = user.Password ?? string.Empty; // Use empty string if Password is NULL

                    if (password == model.Password) // Implement proper password hashing here!
                    {
                        // You might want to set up authentication and redirect to a dashboard or admin home page
                        Console.WriteLine($"Admin logged in: {user.Email}");
                        return RedirectToAction("ManageClaims", "Admin");
                    }
                }
                // If user is null or password does not match
                ModelState.AddModelError("", "Invalid login attempt.");
            }

            return View(model); // Return the view with model errors if login fails
        }

        // Action to fetch and display claims for approval/rejection
        [HttpGet]
        public async Task<IActionResult> ManageClaims()
        {
            var claims = await _context.Claims
                .Include(c => c.Lecturer) // Assuming Lecturer is a navigation property
                .Select(c => new ClaimViewModel // Using a ViewModel for better clarity
                {
                    ClaimID = c.ClaimID,
                    LecturerId = c.Lecturer.LecturerId, // Assuming Lecturer has a LecturerId property
                    SubmissionDate = c.SubmissionDate,
                    Month = c.Month,
                    HoursWorked = c.HoursWorked,
                    HourlyRate = c.HourlyRate,
                    TaxDeductions = c.TaxDeductions,
                    TotalClaim = (c.HoursWorked * c.HourlyRate) - c.TaxDeductions,
                    Status = c.Status
                })
                .ToListAsync();

            return View(claims);
        }

        [HttpPost]
        public async Task<IActionResult> ApproveRejectClaim(int claimId, string reason, string action)
        {
            var claim = await _context.Claims.FindAsync(claimId);
            if (claim == null)
            {
                return NotFound();
            }

            // Update the claim status based on the action
            claim.Status = action == "approve" ? "Approved" : "Rejected";
            claim.Reason = reason; // Optional reason for rejection/approval

            try
            {
                await _context.SaveChangesAsync();
                return RedirectToAction("ManageClaims");
            }
            catch (DbUpdateException dbEx)
            {
                Console.WriteLine($"Database error: {dbEx.Message}");
                ModelState.AddModelError("", "Error updating the claim. Please try again.");
                return View("ManageClaims"); // Return the view with error
            }
        }


        // Add Demo Claims (existing)
        private void AddDemoClaims()
        {
            var demoClaims = new[]
            {
                new Claim
                {
                    SubmissionDate = DateTime.Now,
                    Month = "October 2024",
                    HoursWorked = 10,
                    HourlyRate = 150,
                    TaxDeductions = 50,
                    Status = "Approved",
                    LecturerID = 1 // Example lecturer ID
                },
                new Claim
                {
                    SubmissionDate = DateTime.Now,
                    Month = "September 2024",
                    HoursWorked = 12,
                    HourlyRate = 160,
                    TaxDeductions = 40,
                    Status = "Pending",
                    LecturerID = 1
                },
                new Claim
                {
                    SubmissionDate = DateTime.Now,
                    Month = "August 2024",
                    HoursWorked = 8,
                    HourlyRate = 140,
                    TaxDeductions = 30,
                    Status = "Rejected",
                    LecturerID = 1
                },
                new Claim
                {
                    SubmissionDate = DateTime.Now,
                    Month = "July 2024",
                    HoursWorked = 15,
                    HourlyRate = 150,
                    TaxDeductions = 20,
                    Status = "Pending",
                    LecturerID = 2
                }
            };

            _context.Claims.AddRange(demoClaims);
            _context.SaveChanges();
        }
    
    }
}
