---------------------------------------------------------------------------------------------------
-- DROP TABLES
---------------------------------------------------------------------------------------------------
DROP TABLE Doctor;
DROP TABLE Relation;
DROP TABLE Appointment;
DROP TABLE Specialty;
DROP TABLE Heartbeat;
DROP TABLE Sugarblood;
DROP TABLE BloodPressure;
DROP TABLE Patient;
DROP TABLE Allergy;
DROP TABLE ActivationCode;
DROP TABLE Addiction;

DROP TABLE City;
DROP TABLE Country;
DROP TABLE MedicalImage;
DROP TABLE PrescriptedMedicine;
DROP TABLE Prescription;
DROP TABLE ExperimentInfo;
DROP TABLE ExperimentNote;
DROP TABLE MedicalRecord;
DROP TABLE Person;
---------------------------------------------------------------------------------------------------

-- Specialty table.
CREATE TABLE Specialty
(
	Id					INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	Name				NVARCHAR(32) NOT NULL 
)

-- Country table.
CREATE TABLE Country
(
	Id					INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	Name				NVARCHAR(64) NOT NULL
)

-- City table.
CREATE TABLE City
(
	Id					INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	CountryId			INT NOT NULL,
	Name				NVARCHAR(64) NOT NULL,
	CountryName			NVARCHAR(64) NOT NULL

	FOREIGN KEY (CountryId) REFERENCES Country(Id)
)

-- Person table.
CREATE TABLE Person
(
	Id					INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	Email				VARCHAR(64) NOT NULL,
	Password			VARCHAR(16),
	FirstName			NVARCHAR(32) NOT NULL,
	LastName			NVARCHAR(32) NOT NULL,
	FullName			NVARCHAR(96) NOT NULL,
	Birthday			FLOAT,
	Phone				VARCHAR(15),
	Gender				TINYINT DEFAULT 0,
	Role				TINYINT				NOT NULL,
	Created				FLOAT				NOT NULL,
	LastModified		FLOAT,
	Status				TINYINT				NOT NULL,
	Address				NVARCHAR(128),
	Photo				VARCHAR(32)
);

-- Heartbeat table.
CREATE TABLE Heartbeat
(
	Id					INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	Owner				INT NOT NULL,
	[Rate]				FLOAT DEFAULT 0 NOT NULL,
	[Time]				FLOAT NOT NULL,
	Note				NVARCHAR(128),
	Created				FLOAT NOT NULL,
	LastModified		FLOAT

	FOREIGN KEY (Owner) REFERENCES Person(Id)
)

-- Sugarblood table
CREATE TABLE SugarBlood
(
	Id					INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	Owner				INT NOT NULL,
	Value				FLOAT NOT NULL,
	[Time]				FLOAT NOT NULL,
	Note				NVARCHAR(128),
	Created				FLOAT NOT NULL,
	LastModified		FLOAT

	FOREIGN KEY (Owner) REFERENCES Person(Id)
)

-- BloodPressure table
CREATE TABLE BloodPressure
(
	Id					INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	Owner				INT NOT NULL,
	Systolic 			INT NOT NULL, -- Maximum bloodpressure
	Diastolic			INT NOT NULL, -- Minimum bloodpressure 
	[Time]				FLOAT NOT NULL,
	Note				NVARCHAR(128),
	Created				FLOAT NOT NULL,
	LastModified		FLOAT

	FOREIGN KEY (Owner) REFERENCES Person(Id)
)

-- Activation code table.
CREATE TABLE ActivationCode
(
	Owner				INT NOT NULL PRIMARY KEY,
	Code				VARCHAR(10),
	Expired				DATETIME NOT NULL,
	FOREIGN KEY (Owner) REFERENCES Person(Id)
)

-- Appointment table.
CREATE TABLE Appointment
(
	Id					INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	Maker				INT NOT NULL,
	MakerFirstName		NVARCHAR(32) NOT NULL,
	MakerLastName		NVARCHAR(32) NOT NULL,
	Dater				INT NOT NULL,
	DaterFirstName		NVARCHAR(32) NOT NULL,
	DaterLastName		NVARCHAR(32) NOT NULL,
	[From]				FLOAT NOT NULL,
	[To]				FLOAT NOT NULL,
	Note				NVARCHAR(128) NOT NULL,
	Created				FLOAT NOT NULL,
	LastModified		FLOAT,
	[Status]			TINYINT NOT NULL

	FOREIGN KEY (Maker) REFERENCES Person(Id),
	FOREIGN KEY (Dater) REFERENCES Person(Id)
);

-- Relation table
CREATE TABLE Relation
(
	Id					INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	Source				INT NOT NULL,
	SourceFirstName		NVARCHAR(32) NOT NULL,
	SourceLastName		NVARCHAR(32) NOT NULL,
	Target				INT NOT NULL,
	TargetFirstName		NVARCHAR(32) NOT NULL,
	TargetLastName		NVARCHAR(32) NOT NULL,
	Type				TINYINT NOT NULL,
	Created				FLOAT NOT NULL,
	Status				TINYINT NOT NULL,

	FOREIGN KEY (Source) REFERENCES Person(Id),
	FOREIGN KEY (Target) REFERENCES Person(Id)			
);

-- Allergy table.
CREATE TABLE Allergy
(
	Id					INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	Owner				INT NOT NULL,
	Name				NVARCHAR(32) NOT NULL,
	Cause				NVARCHAR(128) NOT NULL,
	Note				NVARCHAR(128),
	Created				FLOAT NOT NULL,
	LastModified		FLOAT,

	FOREIGN KEY (Owner) REFERENCES Person(Id)
);

-- Addiction table.
CREATE TABLE Addiction
(
	Id					INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	Owner				INT NOT NULL,
	Cause				NVARCHAR(128) NOT NULL,
	Note				NVARCHAR(128),
	Created				FLOAT NOT NULL,
	LastModified		FLOAT,

	FOREIGN KEY (Owner) REFERENCES Person(Id)
);

-- Doctor information.
CREATE TABLE Doctor
(
	Id					INT NOT NULL PRIMARY KEY,
	Rank				FLOAT,
	SpecialtyId			INT NOT NULL,
	SpecialtyName		NVARCHAR(32) NOT NULL,
	Voters				INT			NOT NULL,
	Money				INT			NOT NULL,
	CityId				INT			NOT NULL,

	FOREIGN KEY (Id) REFERENCES Person(Id),
	FOREIGN KEY (SpecialtyId) REFERENCES Specialty(Id),
	FOREIGN KEY (CityId) REFERENCES City(Id)
);

-- Patient information.
CREATE TABLE Patient
(
	Id					INT NOT NULL PRIMARY KEY,
	Money				INT		NOT NULL,
	Weight				FLOAT,
	Height				FLOAT

	FOREIGN KEY (Id) REFERENCES Person(Id)
)

-- Medical record table
CREATE TABLE MedicalRecord
(
	Id						INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	Owner					INT NOT NULL,
	Summary					NVARCHAR (128),
	Tests					NVARCHAR (128),
	AdditionalMorbidities	NVARCHAR (128),
	DifferentialDiagnosis	NVARCHAR (128),
	OtherPathologies		NVARCHAR (128),
	Time					FLOAT NOT NULL,
	Created					FLOAT NOT NULL,
	LastModified			FLOAT,

	FOREIGN KEY (Owner) REFERENCES Person(Id) 
)

-- Medical image table
CREATE TABLE MedicalImage
(
	Id						INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	MedicalRecordId			INT NOT NULL,
	Owner					INT NOT NULL,
	Image					NVARCHAR(32) NOT NULL,
	Created					FLOAT NOT NULL,
	
	FOREIGN KEY (MedicalRecordId) REFERENCES MedicalRecord(Id),
	FOREIGN KEY (Owner) REFERENCES Person(Id)					
)

CREATE TABLE Prescription
(
	Id						INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	Owner					INT NOT NULL,
	MedicalRecordId			INT NOT NULL,
	[From]					FLOAT NOT NULL,
	[To]					FLOAT NOT NULL,
	Note					NVARCHAR(128),
	Created					FLOAT NOT NULL,
	LastModified			FLOAT,

	FOREIGN KEY (MedicalRecordId)	REFERENCES MedicalRecord(Id),
	FOREIGN KEY (Owner)				REFERENCES Person(Id)
)

CREATE TABLE PrescriptedMedicine
(
	Id						INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	PrescriptionId			INT NOT NULL,
	Owner					INT NOT NULL,
	MedicineName			NVARCHAR(32) NOT NULL,
	Quantity				FLOAT NOT NULL,
	Unit					NVARCHAR(16) NOT NULL,
	Note					NVARCHAR(128),
	Expired					FLOAT NOT NULL,

	FOREIGN KEY (PrescriptionId) REFERENCES Prescription(Id),
	FOREIGN KEY (Owner) REFERENCES Person(Id)
)

CREATE TABLE ExperimentNote
(
	Id						INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	MedicalRecordId			INT NOT NULL,
	Owner					INT NOT NULL,
	Name					NVARCHAR(32) NOT NULL,
	Created					FLOAT NOT NULL,
	LastModified			FLOAT,
	
	FOREIGN KEY (MedicalRecordId) REFERENCES MedicalRecord(Id),
	FOREIGN KEY (Owner) REFERENCES Person(Id)
)

CREATE TABLE ExperimentInfo
(
	ExperimentId			INT NOT NULL,
	[Key]					NVARCHAR(32) NOT NULL,
	Value					FLOAT NOT NULL,

	FOREIGN KEY (ExperimentId) REFERENCES ExperimentNote(Id),
	PRIMARY KEY (ExperimentId, [Key])
)

SELECT * FROM ExperimentNote
INNER JOIN ExperimentInfo ON ExperimentNote.Id = ExperimentInfo.ExperimentId
SELECT * FROM Person
SELECT * FROM Country
WHERE Country.Name = 'Bac giang'
SELECT * FROM City
---------------------------------------------------------------------------------------------------
-- DELETE ALL RECORDS
---------------------------------------------------------------------------------------------------
DELETE FROM Person;
DELETE FROM Doctor;
DELETE FROM Patient;


---------------------------------------------------------------------------------------------------
-- PROCEDURES
---------------------------------------------------------------------------------------------------

---------------------------------------------------------------------------------------------------
-- END PROCEDURES
---------------------------------------------------------------------------------------------------
SELECT * FROM Heartbeat WHERE Heartbeat.Time >= 1466787397566 AND Heartbeat.Time <= 1467392197566
ORDER BY Heartbeat.Time ASC


DELETE FROM Heartbeat

SELECT * FROM Person
INNER JOIN Doctor ON Person.Id = Doctor.CityId
WHERE Person.Id = 26

SELECT * FROM Person
WHERE Person.Email = 'patient26@gmail.com'

UPDATE Person
SET Email = 'patient26@gmail.com'
WHERE Person.Id = 77

SELECT * FROM Relation