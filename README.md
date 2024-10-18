# Programming-2B_PROG6212_POEPart2

# CMCS Management System

This README provides instructions on how to set up and run the CMCS Management System application using C# MVC with MySQL. It covers database setup, application environment, and running unit tests.

## Table of Contents

- [Requirements](#requirements)
- [Database Setup](#database-setup)
- [Application Setup](#application-setup)
- [Running the Application](#running-the-application)
- [Running Unit Tests](#running-unit-tests)
- [Contributing](#contributing)

## Requirements

- **MySQL Server**: Ensure you have MySQL Server installed. You can download it from [MySQL's official website](https://dev.mysql.com/downloads/mysql/).
- **Visual Studio**: Download and install [Visual Studio Community](https://visualstudio.microsoft.com/vs/community/) (ensure to include the ASP.NET and web development workload).
- **.NET SDK**: Make sure you have the .NET SDK installed. You can download it from [.NET's official website](https://dotnet.microsoft.com/download).

## Database Setup

### 1. Create the Database

1. Open MySQL Workbench or any MySQL client.
2. Create a new database named `cmcs`.
3. Execute the following SQL commands to create the necessary tables:

   ```sql
   CREATE TABLE Users (
       UserID INT AUTO_INCREMENT PRIMARY KEY,
       Username VARCHAR(255) NOT NULL,
       Name VARCHAR(255) NOT NULL,
       Surname VARCHAR(255) NOT NULL,
       Address VARCHAR(255),
       Email VARCHAR(255) NOT NULL,
       Password VARCHAR(255) NOT NULL,
       isApproved BOOLEAN DEFAULT FALSE
   );

   CREATE TABLE Lecturer (
       LecturerId INT AUTO_INCREMENT PRIMARY KEY,
       UserId INT,
       FOREIGN KEY (UserId) REFERENCES Users(UserID)
   );

   CREATE TABLE Departments (
       DepartmentId INT AUTO_INCREMENT PRIMARY KEY,
       DepartmentName VARCHAR(255) NOT NULL
   );

   CREATE TABLE Claim (
       claimID INT AUTO_INCREMENT NOT NULL,
       lecturerID INT NOT NULL, 
       adminID INT, 
       documentPath VARCHAR(255),
       hoursWorked DECIMAL(10, 2) NOT NULL, 
       hourlyRate DECIMAL(10, 2) NOT NULL, 
       month VARCHAR(20) NOT NULL,
       status ENUM('Pending', 'Approved', 'Rejected') DEFAULT 'Pending',
       submissionDate DATETIME DEFAULT CURRENT_TIMESTAMP,
       taxDeductions DECIMAL(10, 2) DEFAULT 0.00,
       TotalClaim DECIMAL(10, 2),
       Reason VARCHAR(255),
       PRIMARY KEY (claimID),
       FOREIGN KEY (lecturerID) REFERENCES Lecturer(lecturerID) ON DELETE CASCADE,
       FOREIGN KEY (adminID) REFERENCES Admin(adminID) ON DELETE SET NULL
   );

   CREATE TABLE Document (
       DocumentId INT AUTO_INCREMENT PRIMARY KEY,
       ClaimId INT,
       FilePath VARCHAR(255),
       FOREIGN KEY (ClaimId) REFERENCES Claim(claimID)
   );

   CREATE TABLE Admin (
       AdminId INT AUTO_INCREMENT PRIMARY KEY,
       UserId INT,
       FOREIGN KEY (UserId) REFERENCES Users(UserID)
   );

   CREATE TABLE Approval (
       ApprovalId INT AUTO_INCREMENT PRIMARY KEY,
       ClaimId INT,
       AdminId INT,
       IsApproved BOOLEAN,
       FOREIGN KEY (ClaimId) REFERENCES Claim(claimID),
       FOREIGN KEY (AdminId) REFERENCES Admin(adminId)
   );

2. Update Connection String
Open the appsettings.json file in your project and update the connection string to point to your MySQL database:
{
  "ConnectionStrings": {
    "DefaultConnection": "server=localhost;database=cmcs;uid=root;password=YOUR_PASSWORD"
  }
}

Application Setup
1. Clone the Repository
Open your terminal or command prompt.

Clone the repository to your local machine:

2. Open the Project in Visual Studio
Open Visual Studio.
Click on File > Open > Project/Solution and select the .sln file in the cloned project directory.
3. Restore Dependencies
Open the NuGet Package Manager Console (Tools > NuGet Package Manager > Package Manager Console) and run the following command to restore the necessary packages:
Update-Package -Reinstall

Running the Application
1. Build the Project
In Visual Studio, go to Build > Build Solution or press Ctrl + Shift + B.
2. Run the Application
To run the application, press F5 or click on the Start button in Visual Studio.
The application should start, and you can access it in your web browser at http://localhost:5000 (or whichever port is configured).
Running Unit Tests
1. Navigate to Test Project
Ensure you have the test project included in your solution.
2. Run the Tests
In Visual Studio, open the Test Explorer (Test > Windows > Test Explorer).
Click on Run All to execute all tests or select specific tests to run.
3. View Results
The Test Explorer will display the results of the tests, including any that have failed and their reasons.
