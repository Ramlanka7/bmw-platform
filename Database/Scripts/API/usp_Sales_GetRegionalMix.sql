CREATE OR ALTER PROC usp_Sales_GetRegionalMix
  @Year SMALLINT = NULL
AS
BEGIN
  SET NOCOUNT ON;
  SELECT d.[Year], g.Region, SUM(f.SalesUnits) AS Units
  FROM Dimension.FactSales f
  JOIN Dimension.DimDate d ON d.DimDateId = f.DimDateId
  JOIN Dimension.DimGeo  g ON g.DimGeoId  = f.DimGeoId
  WHERE (@Year IS NULL OR d.[Year] = @Year)
  GROUP BY d.[Year], g.Region
  ORDER BY d.[Year], Units DESC;
END
GO