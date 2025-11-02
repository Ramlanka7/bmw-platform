USE BmwSalesDw;
GO

/* 0) Build normalized, aggregated set into a temp table */
IF OBJECT_ID('tempdb..#N') IS NOT NULL DROP TABLE #N;
CREATE TABLE #N
(
    [Year]       SMALLINT      NOT NULL,
    DateKey      INT           NOT NULL,
    [Date]       DATE          NOT NULL,
    [Month]      TINYINT       NOT NULL,
    MonthName    NVARCHAR(20)  NOT NULL,
    Model        NVARCHAR(100) NULL,
    Region       NVARCHAR(100) NULL,
    FuelType     NVARCHAR(50)  NULL,
    Transmission NVARCHAR(50)  NULL,
    EngineTxt    NVARCHAR(100) NULL,
    SalesUnits   INT           NOT NULL
);

WITH S AS (
    SELECT
        TRY_CONVERT(SMALLINT, [Year])             AS [Year],
        NULLIF(LTRIM(RTRIM([Model])), '')         AS Model,
        NULLIF(LTRIM(RTRIM([Region])), '')        AS Region,
        NULLIF(LTRIM(RTRIM([FuelType])), '')      AS FuelType,
        NULLIF(LTRIM(RTRIM([Transmission])), '')  AS Transmission,
        TRY_CONVERT(FLOAT, [EngineSizeL])         AS EngineSizeL,
        TRY_CONVERT(INT, [SalesVolume])           AS SalesVolume
    FROM Staging.BmwSalesRaw
),
G AS (
    SELECT
        [Year], Model, Region, FuelType, Transmission, EngineSizeL,
        SUM(ISNULL(SalesVolume,0)) AS SalesUnits
    FROM S
    WHERE [Year] IS NOT NULL AND Model IS NOT NULL AND Region IS NOT NULL
    GROUP BY [Year], Model, Region, FuelType, Transmission, EngineSizeL
)
INSERT INTO #N ([Year], DateKey, [Date], [Month], MonthName, Model, Region, FuelType, Transmission, EngineTxt, SalesUnits)
SELECT
    g.[Year],
    CONVERT(INT, CONCAT(g.[Year], '0101'))                            AS DateKey,
    DATEFROMPARTS(g.[Year], 1, 1)                                     AS [Date],
    CAST(1 AS TINYINT)                                                AS [Month],
    CAST(N'January' AS NVARCHAR(20))                                   AS MonthName,
    g.Model,
    g.Region,
    g.FuelType,
    g.Transmission,
    CAST(NULLIF(STR(ISNULL(g.EngineSizeL,NULL), 10, 2),'         .  ') AS NVARCHAR(100)) AS EngineTxt,
    g.SalesUnits
FROM G g;

/* 1) DimDate */
MERGE Dimension.DimDate AS tgt
USING (
    SELECT DISTINCT DateKey, [Date], [Year], [Month], MonthName FROM #N
) AS src
ON tgt.DateKey = src.DateKey
WHEN NOT MATCHED BY TARGET THEN
  INSERT (DateKey, [Date], [Year], [Month], MonthName)
  VALUES (src.DateKey, src.[Date], src.[Year], src.[Month], src.MonthName)
;

/* 2) DimModel */
MERGE Dimension.DimModel AS tgt
USING (SELECT DISTINCT Model FROM #N) AS src
ON ISNULL(tgt.Model,'') = ISNULL(src.Model,'')
WHEN NOT MATCHED BY TARGET THEN
  INSERT (Model, Series, BodyStyle) VALUES (src.Model, NULL, NULL)
;

/* 3) DimPowertrain */
MERGE Dimension.DimPowertrain AS tgt
USING (
    SELECT DISTINCT FuelType, EngineTxt, Transmission FROM #N
) AS src
ON ISNULL(tgt.FuelType,'')     = ISNULL(src.FuelType,'')
AND ISNULL(tgt.Engine,'')       = ISNULL(src.EngineTxt,'')
AND ISNULL(tgt.Transmission,'') = ISNULL(src.Transmission,'')
WHEN NOT MATCHED BY TARGET THEN
  INSERT (FuelType, Engine, Transmission) VALUES (src.FuelType, src.EngineTxt, src.Transmission)
;

/* 4) DimGeo */
MERGE Dimension.DimGeo AS tgt
USING (SELECT DISTINCT Region FROM #N) AS src
ON ISNULL(tgt.Region,'') = ISNULL(src.Region,'') AND ISNULL(tgt.Country,'') = N''
WHEN NOT MATCHED BY TARGET THEN
  INSERT (Region, Country) VALUES (src.Region, NULL)
;

/* 5) FactSales */
;WITH K AS (
    SELECT
        n.DateKey,
        dm.DimModelId,
        dp.DimPowertrainId,
        dg.DimGeoId,
        n.SalesUnits
    FROM #N n
    JOIN Dimension.DimModel      dm ON ISNULL(dm.Model,'') = ISNULL(n.Model,'')
    JOIN Dimension.DimPowertrain dp ON ISNULL(dp.FuelType,'')     = ISNULL(n.FuelType,'')
                                   AND ISNULL(dp.Engine,'')       = ISNULL(n.EngineTxt,'')
                                   AND ISNULL(dp.Transmission,'') = ISNULL(n.Transmission,'')
    JOIN Dimension.DimGeo        dg ON ISNULL(dg.Region,'') = ISNULL(n.Region,'')
                                   AND ISNULL(dg.Country,'') = N''
)
MERGE Dimension.FactSales AS tgt
USING K AS src
ON  tgt.DimDateId       = (SELECT DimDateId FROM Dimension.DimDate WHERE DateKey = src.DateKey)
AND tgt.DimModelId      = src.DimModelId
AND tgt.DimPowertrainId = src.DimPowertrainId
AND tgt.DimGeoId        = src.DimGeoId
WHEN MATCHED THEN
  UPDATE SET tgt.SalesUnits = src.SalesUnits
WHEN NOT MATCHED BY TARGET THEN
  INSERT (DimDateId, DimModelId, DimPowertrainId, DimGeoId, SalesUnits)
  VALUES ((SELECT DimDateId FROM Dimension.DimDate WHERE DateKey = src.DateKey),
          src.DimModelId, src.DimPowertrainId, src.DimGeoId, src.SalesUnits)
;

/* 6) QA */
SELECT d.[Year], SUM(f.SalesUnits) AS Units
FROM Dimension.FactSales f
JOIN Dimension.DimDate d ON d.DimDateId = f.DimDateId
GROUP BY d.[Year]
ORDER BY d.[Year];

-- Optional: DROP TABLE #N;  -- temp table will auto-drop at session end
