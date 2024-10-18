using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using CMCS.Controllers;
using CMCS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Globalization;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace TestSample
{
    [TestClass]
    public class ClaimControllerTests
    {
        private Mock<YourDbContext> _mockContext; // Use your actual DbContext name
        private YourDbContext _context;
        private ClaimController _controller;

        [TestInitialize]
        public void Setup()
        {
            // Use an in-memory database for testing
            var options = new DbContextOptionsBuilder<YourDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            // Initialize your DbContext with in-memory options
            _context = new YourDbContext(options);

            // Pass the context to your controller
            _controller = new ClaimController(_context);
        }


        [TestMethod]
        public async Task SubmitClaim_ValidModel_ReturnsRedirectToClaimPage()
        {
            // Arrange
            var model = new ClaimViewModel
            {
                Month = new DateTime(1, 5, 1).ToString("MMMM", CultureInfo.InvariantCulture), // May
                HoursWorked = 10,
                HourlyRate = 100,
                TaxDeductions = 50,
                Document = null // Example: Set to null for simplicity
            };

            // Create a fake logged-in user
            var claimsIdentity = new ClaimsIdentity(new List<System.Security.Claims.Claim>
    {
        new System.Security.Claims.Claim(ClaimTypes.NameIdentifier, "1")
    });
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            // Simulate user context in the controller
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            // Set up your mock for the Lecturer and Claims DbSet
            var lecturer = new Lecturer { UserId = 1, LecturerId = 1 };

            // Mocking the Lecturers DbSet
            var mockSetLecturers = new Mock<DbSet<Lecturer>>();
            mockSetLecturers.Setup(m => m.FirstOrDefaultAsync(It.IsAny<Expression<Func<Lecturer, bool>>>(), CancellationToken.None))
                            .ReturnsAsync(lecturer);

            // Mocking the Claims DbSet
            var mockSetClaims = new Mock<DbSet<CMCS.Models.Claim>>();
            _mockContext.Setup(m => m.Claims).Returns(mockSetClaims.Object);

            // Now assign the mocked context to your controller
            _mockContext.Setup(c => c.Lecturers).Returns(mockSetLecturers.Object);

            // Act
            var result = await _controller.SubmitClaim(model) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result); // Ensure we got a result
            Assert.AreEqual("ClaimPage", result.ActionName); // Ensure it redirects to "ClaimPage"
            mockSetClaims.Verify(c => c.Add(It.IsAny<CMCS.Models.Claim>()), Times.Once); // Verify claim was added
            _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once); // Verify save changes
        }



        [TestMethod]
        public void MonthNumber_ShouldReturnCorrectMonthName()
        {
            // Arrange
            var monthNumber = 5; // May

            // Act
            var monthName = new DateTime(1, monthNumber, 1).ToString("MMMM", CultureInfo.InvariantCulture);

            // Assert
            Assert.AreEqual("May", monthName);
        }
    }
}