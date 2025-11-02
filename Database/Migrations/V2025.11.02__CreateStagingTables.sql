/* =========================================================
   V2025.11.02__CreateStagingTables.sql
   - Creates Staging schema (if missing)
   - Creates Staging.BmwSalesRaw
   ========================================================= */

IF DB_ID('BmwSalesDw') IS NULL
BEGIN
    RAISERROR('Database BmwSalesDw not found. Run V2025.11.01__CreateDatabase.sql first.', 16, 1);
    RETURN;
END
GO
USE BmwSalesDw;
GO

IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'Staging')
    EXEC('CREATE SCHEMA Staging;');
GO

IF OBJECT_ID('Staging.BmwSalesRaw','U') IS NOT NULL
    DROP TABLE Staging.BmwSalesRaw;
GO

CREATE TABLE Staging.BmwSalesRaw
(
    BmwSalesRawId   BIGINT IDENTITY(1,1) PRIMARY KEY,
    YearTxt         NVARCHAR(10)   NULL,
    MonthTxt        NVARCHAR(20)   NULL,    -- "1..12" or "January.."
    Model           NVARCHAR(100)  NULL,
    Series          NVARCHAR(50)   NULL,
    BodyStyle       NVARCHAR(50)   NULL,
    FuelType        NVARCHAR(50)   NULL,
    Transmission    NVARCHAR(50)   NULL,
    Engine          NVARCHAR(100)  NULL,
    Region          NVARCHAR(100)  NULL,
    Country         NVARCHAR(100)  NULL,
    SalesUnitsTxt   NVARCHAR(50)   NULL,
    SourceFile      NVARCHAR(260)  NULL,
    ImportedAtUtc   DATETIME2(0)   NOT NULL CONSTRAINT DF_BmwSalesRaw_ImportedAtUtc DEFAULT (SYSUTCDATETIME())
);
GO
