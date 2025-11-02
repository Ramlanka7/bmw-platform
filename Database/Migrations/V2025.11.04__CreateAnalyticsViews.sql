/* =========================================================
   V2025.11.04__CreateAnalyticsViews.sql
   - Creates Analytics schema (if missing)
   - Creates curated BI views (read-only)
   ========================================================= */

IF DB_ID('BmwSalesDw') IS NULL
BEGIN
    RAISERROR('Database BmwSalesDw not found. Run prior migrations first.', 16, 1);
    RETURN;
END
GO
USE BmwSalesDw;
GO

IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'Analytics')
    EXEC('CREATE SCHEMA Analytics;');
GO

-- Drop and recreate views to stay idempotent
IF OBJECT_ID('Analytics.vwTopModelsByYear','V') IS NOT NULL
    DROP VIEW Analytics.vwTopModelsByYear;
GO
CREATE VIEW Analytics.vwTopModelsByYear AS
SELECT
    d.[Year],
    m.Model,
    m.Series,
    SUM(f.SalesUnits) AS Units
FROM Dimension.FactSales f
JOIN Dimension.DimDate d        ON d.DimDateId = f.DimDateId
JOIN Dimension.DimModel m       ON m.DimModelId = f.DimModelId
GROUP BY d.[Year], m.Model, m.Series;
GO

IF OBJECT_ID('Analytics.vwRegionalMix','V') IS NOT NULL
    DROP VIEW Analytics.vwRegionalMix;
GO
CREATE VIEW Analytics.vwRegionalMix AS
SELECT
    d.[Year],
    g.Region,
    SUM(f.SalesUnits) AS Units
FROM Dimension.FactSales f
JOIN Dimension.DimDate d  ON d.DimDateId = f.DimDateId
JOIN Dimension.DimGeo  g  ON g.DimGeoId  = f.DimGeoId
GROUP BY d.[Year], g.Region;
GO
