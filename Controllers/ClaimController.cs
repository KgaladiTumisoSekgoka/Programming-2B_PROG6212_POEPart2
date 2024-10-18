using CMCS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Globalization;
using System.IO;
using System.Linq; 
using System.Security.Claims;
using System.Threading.Tasks;

namespace CMCS.Controllers
{
    [Authorize]
    public class ClaimController : Controller
    {
        private readonly YourDbContext _context; 

        public ClaimController(YourDbContext context) // Inject your DbContext
        {
            _context = context;
        }

        public IActionResult ClaimPage()
        {
            return View();
        }

        // Action to handle claim submission
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitClaim(ClaimViewModel model)
        {
            if (!User.Identity.IsAuthenticated)
            {
                // Log the authentication failure for debugging
                Console.WriteLine("User is not authenticated.");
                return RedirectToAction("Login", "Account");
            }

            if (ModelState.IsValid)
            {
                // Retrieve the user ID from the logged-in user
                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdString))
                {
                    ModelState.AddModelError("", "Unable to retrieve user ID.");
                    return View("ClaimPage", model);
                }

                var userId = int.Parse(userIdString);

                // Check if the lecturer exists and is linked to the current user
                var lecturerExists = await _context.Lecturers
                    .AnyAsync(l => l.UserId == userId);

                if (!lecturerExists)
                {
                    ModelState.AddModelError("", "You are not authorized to submit claims as a lecturer.");
                    return View("ClaimPage", model);
                }

                // Calculate the total amount and net amount
                var totalAmount = model.HoursWorked * model.HourlyRate;
                var netAmount = totalAmount - model.TaxDeductions;

                // Check if model.Month is a string, convert it to an integer
                int monthNumber;
                if (!int.TryParse(model.Month.ToString(), out monthNumber))
                {
                    ModelState.AddModelError("", "Invalid month value.");
                    return View("ClaimPage", model); // Handle the error
                }

                // Convert the month number to the month name (e.g., "January", "February", etc.)
                var monthName = new DateTime(1, monthNumber, 1).ToString("MMMM", CultureInfo.InvariantCulture);

                // Create a new Claim object
                var claim = new CMCS.Models.Claim
                {
                    SubmissionDate = DateTime.Now,
                    Month = monthName, // Use the month name as a string
                    HoursWorked = model.HoursWorked,
                    HourlyRate = model.HourlyRate,
                    TaxDeductions = model.TaxDeductions,
                    Status = "Pending",
                    LecturerID = (await _context.Lecturers.FirstOrDefaultAsync(l => l.UserId == userId)).LecturerId,
                    TotalClaim = netAmount
                };



                // Optionally handle file upload if a document is included
                if (model.Document != null && model.Document.Length > 0)
                {
                    var filePath = Path.Combine("wwwroot/uploads", model.Document.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.Document.CopyToAsync(stream);
                    }

                    claim.DocumentPath = filePath; // Save the file path in the claim object
                }
                else
                {
                    claim.DocumentPath = null; // No document uploaded
                }

                try
                {
                    // Save the claim to the database
                    _context.Claims.Add(claim);
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"An error occurred while saving the claim: {ex.Message}");
                    return View("ClaimPage", model);
                }

                TempData["SuccessMessage"] = "Claim submitted successfully!";
                return RedirectToAction("ClaimPage");
            }

            // If the model state is invalid, return to the ClaimPage
            return View("ClaimPage", model);
        }



        // Action to manage claims
        public IActionResult ManageClaims()
        {
            var claims = _context.Claims
                .Where(c => c.LecturerID != null) // Filter out claims with NULL LecturerID
                .ToList();

            // Check if claims are empty and add demo claims if needed
            if (!claims.Any())
            {
                AddDemoClaims();
                claims = _context.Claims.ToList(); // Refresh claims list
            }

            return View(claims);
        }

        // Method to add demo claims
        private void AddDemoClaims()
        {
            var demoClaims = new[]
            {
                new CMCS.Models.Claim // Use fully qualified name here
                {
                    SubmissionDate = DateTime.Now,
                    Month = "October 2024",
                    HoursWorked = 10,
                    HourlyRate = 150,
                    TaxDeductions = 50,
                    Status = "Approved",
                    LecturerID = 1 // Example lecturer ID
                },
                new CMCS.Models.Claim // Use fully qualified name here
                {
                    SubmissionDate = DateTime.Now,
                    Month = "September 2024",
                    HoursWorked = 12,
                    HourlyRate = 160,
                    TaxDeductions = 40,
                    Status = "Pending",
                    LecturerID = 1 // Example lecturer ID
                },
                new CMCS.Models.Claim // Use fully qualified name here
                {
                    SubmissionDate = DateTime.Now,
                    Month = "August 2024",
                    HoursWorked = 8,
                    HourlyRate = 140,
                    TaxDeductions = 30,
                    Status = "Rejected",
                    LecturerID = 1 // Example lecturer ID
                },
                new CMCS.Models.Claim // Use fully qualified name here
                {
                    SubmissionDate = DateTime.Now,
                    Month = "July 2024",
                    HoursWorked = 15,
                    HourlyRate = 150,
                    TaxDeductions = 20,
                    Status = "Pending",
                    LecturerID = 2 // Another example lecturer ID
                }
            };

            // Add demo claims to the database
            _context.Claims.AddRange(demoClaims);
            _context.SaveChanges();
        }

        // Action to approve a claim
        [HttpPost]
        public async Task<IActionResult> ApproveClaim(int id)
        {
            var claim = await _context.Claims.FindAsync(id);
            if (claim == null)
            {
                return NotFound();
            }

            claim.Status = "Approved"; // Update status
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(ManageClaims));
        }

        // Action to reject a claim
        [HttpPost]
        public async Task<IActionResult> RejectClaim(int id)
        {
            var claim = await _context.Claims.FindAsync(id);
            if (claim == null)
            {
                return NotFound();
            }

            claim.Status = "Rejected"; // Update status
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(ManageClaims));
        }

        // Action to approve or reject a claim
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveRejectClaim(int claimId, string action, string reason)
        {
            var claim = await _context.Claims.FindAsync(claimId);
            if (claim == null)
            {
                return NotFound();
            }

            // Handle approval
            if (action == "approve")
            {
                claim.Status = "Approved"; // Update the status
                claim.Reason = reason; // Optionally store the reason for approval
                TempData["SuccessMessage"] = "Claim approved successfully!";
            }
            // Handle rejection
            else if (action == "reject")
            {
                claim.Status = "Rejected"; // Update the status
                claim.Reason = reason; // Store the reason for rejection
                TempData["SuccessMessage"] = "Claim rejected successfully!";
            }

            await _context.SaveChangesAsync(); // Save changes to the database
            return RedirectToAction("ManageClaims"); // Redirect back to the manage claims page
        }
    }
}
