-- Create a new database called 'DatabaseName'
-- Connect to the 'master' database to run this snippet
USE master
GO
-- Create the new database if it does not exist already
IF NOT EXISTS (
    SELECT name
        FROM sys.databases
        WHERE name = N'DatabaseName'
)
CREATE DATABASE DatabaseName
GO


CREATE TABLE Cryptocurrencies (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Symbol NVARCHAR(50) NOT NULL,
    AvailableExchanges NVARCHAR(MAX),
    CurrencyBase NVARCHAR(50),
    CurrencyQuote NVARCHAR(50)
)