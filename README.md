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


---

# BMW Sales Data — MCP Server

This is the **MCP Server** for the BMW Sales Data Platform.  
It enables AI assistants (e.g., ChatGPT or other LLMs) to call specialized tools for querying the BMW sales warehouse via the Model Context Protocol (MCP) over JSON-RPC.

---

## 🔍 What it does

- Exposes three tools via MCP:
  1. `getYearTotals` — returns total sales per year  
  2. `getTopModels` — returns top-selling BMW models (filter by year, with pagination)  
  3. `getRegionalMix` — returns sales by region for a given year  
- Uses SQL Server stored procedures behind the scenes to fetch data from `BmwSalesDw`  
- Runs as a console application, communicating over **stdin/stdout** using JSON-RPC (MCP transport)  
- Allows seamless integration with LLMs and agent workflows, providing structured data access in a standard format  


Example calls:

Initialize handshake: 
{"jsonrpc":"2.0","id":"1","method":"initialize","params":{}}

List available tools: 
{"jsonrpc":"2.0","id":"2","method":"tools/list","params":{}}

Call getYearTotals tool: 
{"jsonrpc":"2.0","id":"3","method":"tools/call","params":{"name":"getYearTotals","arguments":{}}}

Call getTopModels tool: 
{"jsonrpc":"2.0","id":"4","method":"tools/call","params":{"name":"getTopModels","arguments":{"year":2024,"page":1,"pageSize":10}}}

Call getRegionalMix tool: 
{"jsonrpc":"2.0","id":"5","method":"tools/call","params":{"name":"getRegionalMix","arguments":{"year":2024}}}


🔗 Integrating via ChatGPT or LLM client

To integrate your MCP server with ChatGPT or another LLM that supports MCC (Model Context Protocol):

Host the MCP server so it’s reachable (or use a local client that spawns the process via stdio)

Use the MCP handshake (initialize) and tool list (tools/list) to register the tools

The LLM will automatically send tools/call requests based on user prompts

The server returns structured JSON results which the model uses to reply to the user



