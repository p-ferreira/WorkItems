
CREATE DATABASE GenericDataBase
GO

USE GenericDataBase
GO


CREATE TABLE WorkItems
(	
	ID int,
	Title VARCHAR(200), /*ToDo: Verificar tamanho m�ximo permitido pela ms*/
	WorkItemType VARCHAR(200),
	IterationPath VARCHAR(200),
	AreaPath VARCHAR (200),
	State VARCHAR (200),	
	uId UNIQUEIDENTIFIER
)


CREATE TABLE ErrorLogs
(
	LogId INT PRIMARY KEY IDENTITY ,
	Error VARCHAR(1000) NOT NULL,
	Date DATETIME DEFAULT GETDATE()
)
