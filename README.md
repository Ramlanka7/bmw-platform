# Loading BMW Sales Data into Staging

## Overview

This process loads the **BMW Worldwide Sales (2010–2024)** dataset from the Kaggle CSV file into the SQL Server staging environment for the BMW Sales Data Warehouse.

The import is performed in two steps:

1. **Data import using SSMS Import Wizard**
2. **Conversion script to load into standardized staging table**

Once the wizard import completes successfully, run the conversion script:
Database/Scripts/Load/Load_Tables_From_Staging.sql


3. **ETL script to populate Dimension and Fact tables**  
After the staging table (`Staging.BmwSalesRaw`) is populated, run the ETL script to load data into all core Dimension and Fact tables

This completes the initial data load from raw CSV → standardized staging → dimensional data warehouse.



