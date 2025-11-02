# BMW Sales Data Platform

A complete data and API solution for analyzing **BMW Worldwide Sales (2010–2024)** using **SQL Server** for data warehousing and **.NET 8 Web API** for analytics and reporting.

---

## 🚀 Overview

This project transforms raw BMW sales data from CSV files into a structured SQL Server data warehouse and exposes the processed data through a .NET 8 Web API for reporting and integration.

The process includes:

1. **Database Setup** – Creating the data warehouse and schema structure  
2. **ETL Processing** – Loading and transforming raw CSV data  
3. **Web API** – Providing data access through RESTful endpoints

---

## 🧱 Database Setup

**Database:** `BmwSalesDw`

Schemas:
- **Staging** — Raw imported data  
- **Dimension** — Structured model for dates, models, regions, and powertrain  
- **Analytics** — Curated views for business insights

### Scripts
| Script | Purpose |
|--------|----------|
| `Database/Migrations/V2025.11.01__CreateDatabase.sql` | Creates the database and schemas |
| `Database/Scripts/Load/Insert_W_Sales_Into_BmwSalesRaw.sql` | Loads raw CSV data into staging tables |
| `Database/Scripts/Load/Load_From_BmwSalesRaw_To_Dimensions_And_Fact.sql` | Transforms staging data into dimension and fact tables |

### Data Loading Steps
1. Import the CSV file into `[Staging].[W_Sales_2010_2024]` using the SSMS Import Wizard.  
2. Run `Insert_W_Sales_Into_BmwSalesRaw.sql` to populate the standardized staging table.  
3. Run `Load_From_BmwSalesRaw_To_Dimensions_And_Fact.sql` to load dimension and fact data.

---

## 🌐 Web API

The **.NET 8 Web API** exposes key analytics endpoints that return BMW sales insights from the data warehouse.

### 1️⃣ `/api/sales/years`
Provides total BMW sales by year.

### 2️⃣ `/api/sales/models/top`
Returns the top-selling BMW models for a given year, with support for pagination.

### 3️⃣ `/api/sales/regions/mix`
Shows the regional distribution of BMW sales for a selected year.
