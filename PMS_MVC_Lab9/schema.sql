IF DB_ID(N'PMS') IS NULL
BEGIN
    CREATE DATABASE [PMS];
END;
GO

USE [PMS];
GO

IF OBJECT_ID(N'dbo.products', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[products] (
        [Id]          INT           IDENTITY (1, 1) NOT NULL,
        [Title]       VARCHAR (50)  NOT NULL,
        [description] VARCHAR (255) NOT NULL,
        [image]       VARCHAR (255) NOT NULL,
        [price]       INT           NOT NULL,
        [quantity]    INT           NOT NULL,
        [status]      VARCHAR (50)  NOT NULL,
        PRIMARY KEY CLUSTERED ([Id] ASC)
    );
END;
GO
