/* =========================================================
   V2025.11.02__CreateDimensionTables.sql
   - Creates Dimension schema (if missing)
   - Creates Dim tables & FactSales with {TableName}Id PKs
   ========================================================= */

IF DB_ID('BmwSalesDw') IS NULL
BEGIN
    RAISERROR('Database BmwSalesDw not found. Run prior migration first.', 16, 1);
    RETURN;
END
GO
USE BmwSalesDw;
GO

IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'Dimension')
    EXEC('CREATE SCHEMA Dimension;');
GO

-- DimDate
IF OBJECT_ID('Dimension.DimDate','U') IS NOT NULL
    DROP TABLE Dimension.DimDate;
GO
CREATE TABLE Dimension.DimDate
(
    DimDateId   INT IDENTITY(1,1) PRIMARY KEY,
    DateKey     INT          NOT NULL,            -- YYYYMMDD
    [Date]      DATE         NOT NULL,
    [Year]      SMALLINT     NOT NULL,
    [Month]     TINYINT      NOT NULL CHECK ([Month] BETWEEN 1 AND 12),
    MonthName   NVARCHAR(20) NOT NULL
);
GO
CREATE UNIQUE INDEX UX_DimDate_DateKey ON Dimension.DimDate(DateKey);
GO

-- DimModel
IF OBJECT_ID('Dimension.DimModel','U') IS NOT NULL
    DROP TABLE Dimension.DimModel;
GO
CREATE TABLE Dimension.DimModel
(
    DimModelId  INT IDENTITY(1,1) PRIMARY KEY,
    Model       NVARCHAR(100) NOT NULL,
    Series      NVARCHAR(50)  NULL,
    BodyStyle   NVARCHAR(50)  NULL
);
GO
CREATE UNIQUE INDEX UX_DimModel_Business ON Dimension.DimModel(Model, Series, BodyStyle);
GO

-- DimPowertrain
IF OBJECT_ID('Dimension.DimPowertrain','U') IS NOT NULL
    DROP TABLE Dimension.DimPowertrain;
GO
CREATE TABLE Dimension.DimPowertrain
(
    DimPowertrainId INT IDENTITY(1,1) PRIMARY KEY,
    FuelType        NVARCHAR(50)   NULL,
    Engine          NVARCHAR(100)  NULL,
    Transmission    NVARCHAR(50)   NULL
);
GO
CREATE UNIQUE INDEX UX_DimPowertrain_Business ON Dimension.DimPowertrain(FuelType, Engine, Transmission);
GO

-- DimGeo
IF OBJECT_ID('Dimension.DimGeo','U') IS NOT NULL
    DROP TABLE Dimension.DimGeo;
GO
CREATE TABLE Dimension.DimGeo
(
    DimGeoId  INT IDENTITY(1,1) PRIMARY KEY,
    Region    NVARCHAR(100) NULL,
    Country   NVARCHAR(100) NULL
);
GO
CREATE UNIQUE INDEX UX_DimGeo_Business ON Dimension.DimGeo(Region, Country);
GO

-- FactSales
IF OBJECT_ID('Dimension.FactSales','U') IS NOT NULL
    DROP TABLE Dimension.FactSales;
GO
CREATE TABLE Dimension.FactSales
(
    FactSalesId       BIGINT IDENTITY(1,1) PRIMARY KEY,
    DimDateId         INT     NOT NULL,
    DimModelId        INT     NOT NULL,
    DimPowertrainId   INT     NOT NULL,
    DimGeoId          INT     NOT NULL,
    SalesUnits        INT     NOT NULL CONSTRAINT CK_FactSales_SalesUnits_NonNegative CHECK (SalesUnits >= 0),

    CONSTRAINT FK_FactSales_DimDate
        FOREIGN KEY (DimDateId) REFERENCES Dimension.DimDate(DimDateId),
    CONSTRAINT FK_FactSales_DimModel
        FOREIGN KEY (DimModelId) REFERENCES Dimension.DimModel(DimModelId),
    CONSTRAINT FK_FactSales_DimPowertrain
        FOREIGN KEY (DimPowertrainId) REFERENCES Dimension.DimPowertrain(DimPowertrainId),
    CONSTRAINT FK_FactSales_DimGeo
        FOREIGN KEY (DimGeoId) REFERENCES Dimension.DimGeo(DimGeoId)
);
GO

-- Helpful indexes
CREATE INDEX IX_FactSales_DimDate       ON Dimension.FactSales(DimDateId);
CREATE INDEX IX_FactSales_DimModel      ON Dimension.FactSales(DimModelId);
CREATE INDEX IX_FactSales_DimPowertrain ON Dimension.FactSales(DimPowertrainId);
CREATE INDEX IX_FactSales_DimGeo        ON Dimension.FactSales(DimGeoId);
GO
