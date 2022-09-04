-----------------------------------------------------------------------------------------
CREATE DATABASE DeploymentHistory
GO
USE DeploymentHistory
GO
CREATE TABLE Applications
(
    Id int IDENTITY(1,1) NOT NULL,
    Name nvarchar(100) NOT NULL CONSTRAINT UQ_Application_Name UNIQUE,
    RepoUrl nvarchar(300) NOT NULL,
    StoryRegEx nvarchar(50) NOT NULL,
    IsDisabled bit NOT NULL CONSTRAINT DF_Applications_IsDisabled DEFAULT 0,

    CONSTRAINT PK_Applications_Id PRIMARY KEY CLUSTERED ([Id])
)
GO
CREATE TABLE Deployments
(
    Id int IDENTITY(1,1) NOT NULL,
    CommitId nvarchar(100) NOT NULL,
    AppId int NOT NULL,
	BranchName nvarchar(100) NOT NULL CONSTRAINT DF_Deployments_BranchName DEFAULT 'master',
    Timestamp datetime NOT NULL,

    CONSTRAINT PK_Deployments_Id PRIMARY KEY CLUSTERED ([Id]),
    CONSTRAINT FK_Deployments_AppId FOREIGN KEY ([AppId]) REFERENCES [dbo].[Applications]([Id])
)

GO
ALTER TABLE Deployments
ADD CONSTRAINT UQ_Deployments_CommitId_AppId_BranchName_Timestamp UNIQUE(CommitId, AppId, Timestamp, BranchName)
GO
