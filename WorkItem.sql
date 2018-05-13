
CREATE DATABASE GenericDb
GO

USE GenericDb
GO


CREATE TABLE WorkItems
(	
	uId bigint Primary Key Identity not null,
	ID int,
	Title VARCHAR(200), /*ToDo: Verificar tamanho máximo permitido pela ms*/
	WorkItemType VARCHAR(200),
	IterationPath VARCHAR(200),
	AreaPath VARCHAR (200),
	State VARCHAR (200),	
	Guid UNIQUEIDENTIFIER
)


CREATE TABLE ErrorLogs
(
	LogId INT PRIMARY KEY IDENTITY ,
	Error VARCHAR(1000) NOT NULL,
	Date DATETIME DEFAULT GETDATE()
)
