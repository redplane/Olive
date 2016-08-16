---------------------------------------------------------------------------------------------------
-- DROP TABLES
---------------------------------------------------------------------------------------------------
DROP TABLE RealTimeConnection;
DROP TABLE Diary;
DROP TABLE MedicalImage;
DROP TABLE ExperimentNote;
DROP TABLE MedicalNote;
DROP TABLE PrescriptionImage;
DROP TABLE Prescription;
DROP TABLE MedicalRecord;
DROP TABLE MedicalCategory;
DROP TABLE AccountCode;
DROP TABLE Allergy;
DROP TABLE Addiction;
DROP TABLE SugarBlood;
DROP TABLE BloodPressure;
DROP TABLE Heartbeat;
DROP TABLE Notification;
DROP TABLE Appointment;
DROP TABLE Rating;
DROP TABLE RelationshipRequest;
DROP TABLE Relation;
DROP TABLE Patient;
DROP TABLE Doctor;
DROP TABLE Place;
DROP TABLE Specialty;
DROP TABLE [Message];
DROP TABLE Person;
DROP TABLE JunkFile;
---------------------------------------------------------------------------------------------------

-- Specialty table.
CREATE TABLE Specialty
(
	Id					INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	Name				NVARCHAR(32) NOT NULL 
)

-- Country table.
CREATE TABLE Place
(
	Id					INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	City				NVARCHAR(64) NOT NULL,
	Country				NVARCHAR(64) NOT NULL,
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
	PhotoUrl			NVARCHAR(MAX),
	PhotoPhysicPath		NVARCHAR(MAX)
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
CREATE TABLE AccountCode
(
	Owner				INT NOT NULL,
	Code				VARCHAR(36),
	Type				TINYINT NOT NULL,
	Expired				DATETIME NOT NULL,
	
	FOREIGN KEY (Owner) REFERENCES Person(Id),
	PRIMARY KEY (Owner, Type)
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
	LastModifiedNote	NVARCHAR(128),
	[Status]			TINYINT NOT NULL

	FOREIGN KEY (Maker) REFERENCES Person(Id),
	FOREIGN KEY (Dater) REFERENCES Person(Id)
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
	Voters				INT				NOT NULL,
	PlaceId				INT				NOT NULL,
	ProfilePdf			NVARCHAR(32)

	FOREIGN KEY (Id) REFERENCES Person(Id),
	FOREIGN KEY (SpecialtyId) REFERENCES Specialty(Id),
	FOREIGN KEY (PlaceId) REFERENCES Place(Id)
);

-- Patient information.
CREATE TABLE Patient
(
	Id					INT NOT NULL PRIMARY KEY,
	Weight				FLOAT,
	Height				FLOAT

	FOREIGN KEY (Id) REFERENCES Person(Id)
)

-- This table is for storing relationship requests.
CREATE TABLE RelationshipRequest
(
	Id					INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	Source				INT NOT NULL,
	Target				INT NOT NULL,
	Content				NVARCHAR(128),
	Created				FLOAT NOT NULL,
	LastModified		FLOAT,

	FOREIGN KEY (Source) REFERENCES Patient(Id),
	FOREIGN KEY (Target) REFERENCES Doctor(Id)	
)

-- Relation table
CREATE TABLE Relation
(
	Id					INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	Source				INT NOT NULL,
	Target				INT NOT NULL,
	Created				FLOAT NOT NULL,
	
	FOREIGN KEY (Source) REFERENCES Patient(Id),
	FOREIGN KEY (Target) REFERENCES Doctor(Id)			
);

CREATE TABLE Diary
(
	Id						INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	Owner					INT NOT NULL,
	Target					INT NOT NULL,
	Time					FLOAT NOT NULL,
	Note					NVARCHAR(512),
	Created					FLOAT NOT NULL,
	LastModified			FLOAT

	FOREIGN KEY (Owner) REFERENCES Doctor(Id),
	FOREIGN KEY (Target) REFERENCES Patient(Id)
)

CREATE TABLE MedicalCategory
(
	Id						INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	Name					NVARCHAR(32),
	Created					FLOAT NOT NULL,
	LastModified			FLOAT
)

-- Medical record table
CREATE TABLE MedicalRecord
(
	Id						INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	Owner					INT NOT NULL,
	Creator					INT NOT NULL,
	Category				INT NOT NULL,
	Name					NVARCHAR(64),
	Info					NVARCHAR(MAX),
	Time					FLOAT NOT NULL,
	Available				BIT NOT NULL DEFAULT 1,
	Created					FLOAT NOT NULL,
	LastModified			FLOAT,

	FOREIGN KEY (Owner) REFERENCES Person(Id) ,
	FOREIGN KEY (Creator) REFERENCES Person(Id),
	FOREIGN KEY (Category) REFERENCES MedicalCategory(Id)
)

-- Medical image table
CREATE TABLE MedicalImage
(
	Id						INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	MedicalRecordId			INT NOT NULL,
	Owner					INT NOT NULL,
	Creator					INT NOT NULL,
	Image					NVARCHAR(32) NOT NULL,
	FullPath				NVARCHAR(MAX) NOT NULL,
	Created					FLOAT NOT NULL,
	Available				BIT NOT NULL DEFAULT 1
	FOREIGN KEY (MedicalRecordId) REFERENCES MedicalRecord(Id),
	FOREIGN KEY (Owner) REFERENCES Person(Id)					
)

-- Medical note table
CREATE TABLE MedicalNote
(
	Id						INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	MedicalRecordId			INT NOT NULL,
	Creator					INT NOT NULL,
	Owner					INT NOT NULL,
	Note					NVARCHAR(128),
	Time					FLOAT NOT NULL,
	Created					FLOAT NOT NULL,
	LastModified			FLOAT,

	FOREIGN KEY (Owner) REFERENCES Person(Id),
	FOREIGN KEY (Creator) REFERENCES Person(Id),
	FOREIGN KEY (MedicalRecordId) REFERENCES MedicalRecord(Id), 
)

CREATE TABLE Prescription
(
	Id						INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	Creator					INT NOT NULL,
	Owner					INT NOT NULL,
	MedicalRecordId			INT NOT NULL,
	[From]					FLOAT NOT NULL,
	[To]					FLOAT NOT NULL,
	Name					NVARCHAR(32),
	Medicine				NVARCHAR(MAX),
	Note					NVARCHAR(128),
	Available				BIT NOT NULL DEFAULT 1,
	Created					FLOAT NOT NULL,
	LastModified			FLOAT,

	FOREIGN KEY (MedicalRecordId)	REFERENCES MedicalRecord(Id),
	FOREIGN KEY (Owner)				REFERENCES Person(Id),
	FOREIGN KEY (Creator)			REFERENCES Person(Id)
)

-- Prescription image table
CREATE TABLE PrescriptionImage
(
	Id						INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	PrescriptionId			INT NOT NULL,
	MedicalRecordId			INT NOT NULL,
	Image					NVARCHAR(32) NOT NULL,
	FullPath				NVARCHAR(MAX) NOT NULL,
	Owner					INT NOT NULL,
	Creator					INT NOT NULL,
	Created					FLOAT NOT NULL,
	Available				BIT NOT NULL DEFAULT 1

	FOREIGN KEY (MedicalRecordId) REFERENCES MedicalRecord(Id),
	FOREIGN KEY (PrescriptionId) REFERENCES Prescription(Id),
	FOREIGN KEY (Creator) REFERENCES Person(Id),
	FOREIGN KEY (Owner) REFERENCES Patient(Id)					
)

CREATE TABLE ExperimentNote
(
	Id						INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	MedicalRecordId			INT NOT NULL,
	Owner					INT NOT NULL,
	Creator					INT NOT NULL,
	Name					NVARCHAR(32) NOT NULL,
	Info					NVARCHAR(MAX),
	Time					FLOAT NOT NULL,
	Created					FLOAT NOT NULL,
	LastModified			FLOAT,
	
	FOREIGN KEY (MedicalRecordId) REFERENCES MedicalRecord(Id),
	FOREIGN KEY (Owner) REFERENCES Person(Id),
	FOREIGN KEY (Creator) REFERENCES Person(Id)
)

CREATE TABLE Rating 
(
	Maker					INT NOT NULL,
	Target					INT NOT NULL,
	Value					INT NOT NULL,
	Comment					NVARCHAR(128),
	Created					FLOAT NOT NULL,
	LastModified			FLOAT,

	FOREIGN KEY (Maker) REFERENCES Patient(Id),
	FOREIGN KEY (Target) REFERENCES Doctor(Id),
	PRIMARY KEY (Maker, Target)
)

CREATE TABLE JunkFile
(
	Id						INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	FullPath				NVARCHAR(MAX)
)

----------------------------------------------------------
-- Notifications
----------------------------------------------------------
CREATE TABLE Notification
(
	Id						INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	Type					TINYINT NOT NULL,
	Topic					TINYINT NOT NULL,
	Container				INT,
	ContainerType			TINYINT,
	Broadcaster				INT NOT NULL,
	Recipient				INT NOT NULL,
	Record					INT NOT NULL,
	Message					NVARCHAR(512),
	Created					FLOAT NOT NULL,
	IsSeen					BIT NOT NULL

	FOREIGN KEY (Broadcaster) REFERENCES Person(Id),
	FOREIGN KEY (Recipient) REFERENCES Person(Id)
)

----------------------------------------------------------
-- Real time connection
----------------------------------------------------------
CREATE TABLE RealTimeConnection
(
	Id						INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	Owner					INT NOT NULL,
	ConnectionId			NVARCHAR(MAX),
	Created					FLOAT NOT NULL,
)

----------------------------------------------------------
-- Messaging
----------------------------------------------------------
CREATE TABLE [Message]
(
	Id						INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	Broadcaster				INT NOT NULL,
	Recipient				INT NOT NULL,
	Content					NVARCHAR(512),
	Created					FLOAT NOT NULL,
	IsSeen					BIT NOT NULL,

	FOREIGN KEY (Broadcaster) REFERENCES Person(Id),
	FOREIGN KEY (Recipient) REFERENCES Person(Id)
)
