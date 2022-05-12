CREATE DATABASE DeploymentsHistory;
USE DeploymentsHistory;
CREATE TABLE Applications
(
	Id int PRIMARY KEY identity NOT NULL,
	Name nvarchar(100) unique NOT NULL,
	RepoUrl nvarchar(300) NOT NULL,
	StoryRegEx nvarchar(50) NOT NULL,
	BranchName nvarchar(50) NOT NULL DEFAULT 'master',
	IsDisabled bit NOT NULL DEFAULT 0
)

CREATE TABLE Deployments
(
	Id int PRIMARY KEY identity NOT NULL,
	CommitId nvarchar(100) NOT NULL,
	AppId int REFERENCES Applications(Id) NOT NULL,
	Timestamp datetime NOT NULL
)

ALTER TABLE Deployments
	ADD CONSTRAINT uq_Deployments UNIQUE(CommitId, AppId, Timestamp)

ALTER TABLE Applications
	ADD CONSTRAINT uq_Applications_Name UNIQUE(Name, BranchName)

-- USE DeploymentsHistory;
-- SELECT *
-- FROM Applications