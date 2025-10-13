
CREATE DATABASE IF NOT EXISTS scholarshipdb CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE scholarshipdb;

DROP TABLE IF EXISTS Applications;
DROP TABLE IF EXISTS Scholarships;
DROP TABLE IF EXISTS Applicants;

CREATE TABLE Applicants (
  ApplicantId INT AUTO_INCREMENT PRIMARY KEY,
  Name VARCHAR(100) NOT NULL,
  Gender VARCHAR(20),
  DOB DATETIME NULL,
  District VARCHAR(100),
  Province VARCHAR(100),
  School VARCHAR(120),
  Email VARCHAR(120),
  Phone VARCHAR(50),
  Address VARCHAR(200)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE Scholarships (
  ScholarshipId INT AUTO_INCREMENT PRIMARY KEY,
  ScholarshipName VARCHAR(100) NOT NULL,
  Description VARCHAR(500),
  EligibilityCriteria VARCHAR(500),
  FundingAmount DECIMAL(12,2) NOT NULL DEFAULT 0,
  Sponsor VARCHAR(120),
  Deadline DATETIME NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE Applications (
  ApplicationModelId INT AUTO_INCREMENT PRIMARY KEY,
  ApplicantId INT NOT NULL,
  ScholarshipId INT NOT NULL,
  Status VARCHAR(30) NOT NULL DEFAULT 'Pending',
  CONSTRAINT FK_App_Applicant FOREIGN KEY (ApplicantId) REFERENCES Applicants(ApplicantId) ON DELETE CASCADE,
  CONSTRAINT FK_App_Scholarship FOREIGN KEY (ScholarshipId) REFERENCES Scholarships(ScholarshipId) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

INSERT INTO Applicants (Name, Gender, District, Province, Email) VALUES
('Anton Peter', 'Male', 'Chuave', 'Chimbu', 'anton@example.com'),
('Sine Mau', 'Female', 'Chuave', 'Chimbu', 'sine@example.com');

INSERT INTO Scholarships (ScholarshipName, Description, EligibilityCriteria, FundingAmount, Sponsor, Deadline) VALUES
('Chuave STEM 2025', 'Support for STEM students', 'STEM studies, GPA >= 3.0', 2500, 'Chuave District', '2025-12-31'),
('Teacher Training 2025', 'Support for teacher training', 'Education major, Chuave resident', 2000, 'Chuave District', '2025-11-30');

INSERT INTO Applications (ApplicantId, ScholarshipId, Status) VALUES (1,1,'Pending'),(2,2,'Pending');
