# Product Management System (PMS) - Razor Pages & ADO.NET

A web application developed in **ASP.NET Core (.NET 10)** demonstrating the **Repository Design Pattern** and **ADO.NET** for high-performance direct database operations with **MS SQL Server**. Frontend pages are designed using **Razor Pages** and **Bootstrap 5**.

---

## 🚀 Key Features

- **Full CRUD Operations**:
  - **Create**: Add new products with image upload, title, description, price, quantity, and status.
  - **Read**: List products in a clean tabular view and inspect full details of any product.
  - **Update**: Edit product information and optionally replace the product image.
  - **Delete**: Safely delete products along with corresponding disk image cleanup.
- **Repository Pattern**: Separation of data access logic into `ProductRepository` using ADO.NET `SqlConnection`, `SqlCommand`, and `SqlDataReader`.
- **Image File Management**: `ProductImageService` validates file type/size, generates unique GUID file names, and stores images in `wwwroot/images/products`.
- **Validation**: Data Annotations for server-side validation and jQuery Unobtrusive validation for client-side feedback.
- **Responsive UI**: Styled using Bootstrap 5 with alert messages and status indicators.

---

## 🛠️ Technology Stack

- **Framework**: ASP.NET Core 10.0 (Razor Pages)
- **Language**: C#
- **Database**: Microsoft SQL Server
- **Data Access**: ADO.NET (`Microsoft.Data.SqlClient`)
- **Frontend Styling**: HTML5, Bootstrap 5, Bootstrap Icons

---

## ⚙️ How to Setup & Run

### 1. Database Setup
Run the SQL queries from `pms script.txt` in SQL Server Management Studio (SSMS) or Visual Studio SQL Server Object Explorer to create the database and table:
```sql
CREATE DATABASE PMS;
GO
USE PMS;
GO
CREATE TABLE [dbo].[products] (
    [Id]          INT           IDENTITY (1, 1) NOT NULL,
    [Title]       VARCHAR (50)  NOT NULL,
    [description] VARCHAR (255) NOT NULL,
    [image]       VARCHAR (255) NOT NULL,
    [price]       INT           NOT NULL,
    [quantity]    INT           NOT NULL,
    [status]      VARCHAR (50)  NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);
```

### 2. Configure Connection String
Open `appsettings.json` in `Product_Managemnet_System_Razor_Core` and verify your connection string:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=PMS;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

### 3. Build & Run
Run the application using .NET CLI or Visual Studio:
```bash
dotnet restore
dotnet build
dotnet run
```
Then navigate to: `http://localhost:5000` (or the launch port shown in the console).
