/* =========================================================
   Load BMW Sales CSV into Staging.BmwSalesRaw
   ========================================================= */

USE BmwSalesDw;
GO

-- Optional: clear staging before reload
-- TRUNCATE TABLE Staging.BmwSalesRaw;
-- GO

INSERT INTO Staging.BmwSalesRaw
(
    [Year],
    [Model],
    [Region],
    [Color],
    [FuelType],
    [Transmission],
    [EngineSizeL],
    [MileageKM],
    [PriceUSD],
    [SalesVolume],
    [SalesClass],
    [SourceFile]
)
SELECT
    TRY_CONVERT(SMALLINT,  [Year])                  AS [Year],
    CAST([Model] AS NVARCHAR(100))                  AS [Model],
    CAST([Region] AS NVARCHAR(100))                 AS [Region],
    CAST([Color] AS NVARCHAR(50))                   AS [Color],
    CAST([Fuel_Type] AS NVARCHAR(50))               AS [FuelType],
    CAST([Transmission] AS NVARCHAR(50))            AS [Transmission],
    TRY_CONVERT(FLOAT,      [Engine_Size_L])        AS [EngineSizeL],
    TRY_CONVERT(INT,        [Mileage_KM])           AS [MileageKM],
    TRY_CONVERT(INT,        [Price_USD])            AS [PriceUSD],
    TRY_CONVERT(INT,        [Sales_Volume])         AS [SalesVolume],
    CAST([Sales_Classification] AS NVARCHAR(50))    AS [SalesClass],
    N'BMW_Sales_2010_2024.csv'                      AS [SourceFile]
FROM Staging.[W_Sales_2010_2024]
-- (optional) skip rows that are completely empty / invalid year
WHERE TRY_CONVERT(SMALLINT, [Year]) IS NOT NULL;
GO

