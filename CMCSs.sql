-- Create the CMCS database and switch to it
CREATE DATABASE CMCSs;
USE CMCSs;

-- Create the Users table
CREATE TABLE IF NOT EXISTS Users (
    userID INT AUTO_INCREMENT NOT NULL,
    username VARCHAR(50) NOT NULL,
    password VARCHAR(50) NOT NULL,  -- Plain password for now
    email VARCHAR(255) NOT NULL UNIQUE,
    departments VARCHAR(100), -- Department for lecturers (null for non-lecturers)
    role ENUM('Lecturer', 'Programme Coordinator', 'Academic Manager') NOT NULL,
    isApproved BOOLEAN DEFAULT FALSE,
    PRIMARY KEY (userID)
);

-- Create the Lecturer table (one-to-one relation with Users)
CREATE TABLE IF NOT EXISTS Lecturer (
    lecturerID INT AUTO_INCREMENT NOT NULL,
    userID INT NOT NULL,  -- Foreign key to Users table
    department VARCHAR(100) NOT NULL, -- Department specific to lecturers
    PRIMARY KEY (lecturerID),
    FOREIGN KEY (userID) REFERENCES Users(userID) ON DELETE CASCADE -- Cascade deletes to clean related lecturer data
);
insert into lecturer(lecturerID, userID, Department)
values(3, 4, 'Information Technology');

-- Create the Programme Coordinator table (one-to-one relation with Users)
CREATE TABLE IF NOT EXISTS ProgrammeCoordinator (
    coordinatorID INT AUTO_INCREMENT NOT NULL,
    userID INT NOT NULL,  -- Foreign key to Users table
    department VARCHAR(100) NOT NULL, -- Department they manage
    PRIMARY KEY (coordinatorID),
    FOREIGN KEY (userID) REFERENCES Users(userID) ON DELETE CASCADE
);

-- Create the Academic Manager table (one-to-one relation with Users)
CREATE TABLE IF NOT EXISTS AcademicManager (
    managerID INT AUTO_INCREMENT NOT NULL,
    userID INT NOT NULL,  -- Foreign key to Users table
    department VARCHAR(100) NOT NULL, -- Department they manage
    PRIMARY KEY (managerID),
    FOREIGN KEY (userID) REFERENCES Users(userID) ON DELETE CASCADE
);

-- Create the Admin table (includes Programme Coordinators and Academic Managers)
CREATE TABLE IF NOT EXISTS Admin (
    adminID INT AUTO_INCREMENT NOT NULL,
    role ENUM('Programme Coordinator', 'Academic Manager') NOT NULL,
    userID INT NOT NULL, -- Foreign key to Users table
    PRIMARY KEY (adminID),
    FOREIGN KEY (userID) REFERENCES Users(userID) ON DELETE CASCADE
);

-- Create the Claim table (submitted by lecturers, approved by admins)
CREATE TABLE IF NOT EXISTS Claim (
    claimID INT AUTO_INCREMENT NOT NULL,
    lecturerID INT NOT NULL, -- Foreign key to Lecturer
    adminID INT,  -- Foreign key to Admin (nullable, pending approval)
    documentPath VARCHAR(255),
    hoursWorked DECIMAL(10, 2) NOT NULL, -- Changed from INT to DECIMAL
    hourlyRate DECIMAL(10, 2) NOT NULL,  -- Added this column for hourly rate
    month VARCHAR(20) NOT NULL,
    status ENUM('Pending', 'Approved', 'Rejected') DEFAULT 'Pending',
    submissionDate DATETIME DEFAULT CURRENT_TIMESTAMP,
    taxDeductions DECIMAL(10, 2) DEFAULT 0.00,
    -- TotalClaim DECIMAL(10, 2),
    -- Reason VARCHAR(255),
    PRIMARY KEY (claimID),
    FOREIGN KEY (lecturerID) REFERENCES Lecturer(lecturerID) ON DELETE CASCADE,
    FOREIGN KEY (adminID) REFERENCES Admin(adminID) ON DELETE SET NULL -- Admin might be null at first
);

ALTER TABLE Claim
ADD COLUMN TotalClaim DECIMAL(10, 2);

UPDATE Claim
SET TotalClaim = HourlyRate * HoursWorked;

ALTER TABLE Claim ADD COLUMN Reason VARCHAR(255);

-- ALTER TABLE Claim ADD COLUMN HoursRate DECIMAL(10, 2);

-- Create the Document table (links to Claim)
CREATE TABLE IF NOT EXISTS Document (
    documentID INT AUTO_INCREMENT NOT NULL,
    claimID INT NOT NULL, -- Foreign key to Claim
    documentName VARCHAR(100) NOT NULL,
    documentType VARCHAR(50),
    filePath VARCHAR(255) NOT NULL, -- Path to uploaded file
    PRIMARY KEY (documentID),
    FOREIGN KEY (claimID) REFERENCES Claim(claimID) ON DELETE CASCADE
);

-- Create the Approval table (links to Claim and Admin)
CREATE TABLE IF NOT EXISTS Approval (
    approvalID INT AUTO_INCREMENT NOT NULL,
    approvalDate DATETIME DEFAULT CURRENT_TIMESTAMP,
    status ENUM('Approved', 'Rejected') NOT NULL,
    comments TEXT,
    claimID INT NOT NULL, -- Foreign key to Claim
    adminID INT NOT NULL, -- Foreign key to Admin (the one approving)
    PRIMARY KEY (approvalID),
    FOREIGN KEY (claimID) REFERENCES Claim(claimID) ON DELETE CASCADE,
    FOREIGN KEY (adminID) REFERENCES Admin(adminID) ON DELETE CASCADE
);

-- Sample Inserts (Users with plain passwords)
INSERT INTO Users (username, password, email, departments, role, isApproved) VALUES
('lecturer1', 'password1', 'lecturer1@example.com', 'Executive Education', 'Lecturer', TRUE),
('admin1', 'password2', 'admin1@example.com', NULL, 'Programme Coordinator', TRUE);

-- Sample Inserts (Admin)
INSERT INTO Admin (role, userID) VALUES
('Programme Coordinator', 2);  -- Matches with 'admin1'

-- Sample Inserts (Lecturer)
INSERT INTO Lecturer (userID, department) VALUES
(1, 'Executive Education');  -- Matches with 'lecturer1'

-- Sample Inserts (Claims)
INSERT INTO Claim (lecturerID, documentPath, hoursWorked, hourlyRate, month, status, taxDeductions) VALUES
(1, 'documents/claim1.pdf', 10, 200.00, 'September', 'Pending', 30.00);

-- ALTER TABLE Claim ADD COLUMN HoursRate DECIMAL(10,2);

-- ALTER TABLE Claim MODIFY COLUMN LecturerID INT NULL;

-- Sample Selects for testing
SELECT * FROM Users;
SELECT * FROM Admin;
SELECT * FROM Lecturer;
SELECT * FROM Claim;
SELECT * FROM Document;
SELECT * FROM Approval;
SHOW COLUMNS FROM Claim;

-- Insert sample data into the Claim table with TotalClaim calculation
INSERT INTO Claim (lecturerID, adminID, documentPath, hoursWorked, hourlyRate, month, status, submissionDate, taxDeductions, TotalClaim, Reason)
VALUES (1, NULL, '/documents/claim1.pdf', 35.50, 200.00, 'September', 'Pending', '2024-09-15 10:30:00', 0.00, 35.50 * 200.00, 'Extra lectures for exam prep');

INSERT INTO Claim (lecturerID, adminID, documentPath, hoursWorked, hourlyRate, month, status, submissionDate, taxDeductions, TotalClaim, Reason)
VALUES (2, NULL, '/documents/claim2.pdf', 40.00, 180.00, 'September', 'Pending', '2024-09-16 11:00:00', 0.00, 40.00 * 180.00, 'Guest lecture session');

INSERT INTO Claim (lecturerID, adminID, documentPath, hoursWorked, hourlyRate, month, status, submissionDate, taxDeductions, TotalClaim, Reason)
VALUES (3, 1, '/documents/claim3.pdf', 20.00, 150.00, 'August', 'Approved', '2024-08-25 09:15:00', 0.00, 20.00 * 150.00, 'Consulting hours');

INSERT INTO Claim (lecturerID, adminID, documentPath, hoursWorked, hourlyRate, month, status, submissionDate, taxDeductions, TotalClaim, Reason)
VALUES (4, NULL, '/documents/claim4.pdf', 50.00, 210.00, 'October', 'Pending', '2024-10-01 14:45:00', 0.00, 50.00 * 210.00, 'Workshop participation');

INSERT INTO Claim (lecturerID, adminID, documentPath, hoursWorked, hourlyRate, month, status, submissionDate, taxDeductions, TotalClaim, Reason)
VALUES (5, 2, '/documents/claim5.pdf', 60.00, 175.00, 'July', 'Rejected', '2024-07-20 08:30:00', 0.00, 60.00 * 175.00, 'Additional tutorials');


/*SELECT * FROM Lecturer WHERE LecturerID =3;
INSERT INTO Lecturer (LecturerID,Department)
VALUES
(123, 'Lecturer1', 'Lecturer1@gmail.com', 'Information Technology');

SELECT L.LecturerID, U.UserID, U.Username, U.Email
FROM Lecturer L
JOIN Users U ON L.UserID = U.UserID
WHERE L.LecturerID = 2 AND U.UserID = 3;

-- Check if the lecturer exists
SELECT * FROM Lecturer WHERE lecturerID = 2;


