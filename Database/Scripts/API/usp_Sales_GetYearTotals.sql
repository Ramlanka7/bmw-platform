USE BmwSalesDw;
GO

CREATE OR ALTER PROC usp_Sales_GetYearTotals
AS
BEGIN
  SET NOCOUNT ON;
  SELECT d.[Year], SUM(f.SalesUnits) AS Units
  FROM Dimension.FactSales f
  JOIN Dimension.DimDate d ON d.DimDateId = f.DimDateId
  GROUP BY d.[Year]
  ORDER BY d.[Year];
END
GO