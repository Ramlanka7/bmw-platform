USE BmwSalesDw;
GO

IF OBJECT_ID('dbo.usp_Sales_GetTopModels','P') IS NOT NULL
    DROP PROC dbo.usp_Sales_GetTopModels;
GO

CREATE PROC dbo.usp_Sales_GetTopModels
  @Year SMALLINT = NULL,
  @Page INT = 1,
  @PageSize INT = 10
AS
BEGIN
  SET NOCOUNT ON;

  DECLARE @Agg TABLE
  (
      [Year]  SMALLINT NOT NULL,
      Model   NVARCHAR(100) NOT NULL,
      Series  NVARCHAR(50)  NULL,
      Units   INT NOT NULL
  );

  INSERT INTO @Agg ([Year], Model, Series, Units)
  SELECT 
      d.[Year], 
      m.Model, 
      m.Series, 
      SUM(f.SalesUnits) AS Units
  FROM Dimension.FactSales f
  JOIN Dimension.DimDate d  ON d.DimDateId  = f.DimDateId
  JOIN Dimension.DimModel m ON m.DimModelId = f.DimModelId
  WHERE (@Year IS NULL OR d.[Year] = @Year)
  GROUP BY d.[Year], m.Model, m.Series;

  -- 1st result set: total
  SELECT COUNT(*) AS Total FROM @Agg;

  -- 2nd result set: page
  SELECT [Year], Model, Series, Units
  FROM @Agg
  ORDER BY Units DESC
  OFFSET (@Page - 1) * @PageSize ROWS FETCH NEXT @PageSize ROWS ONLY;
END
GO
