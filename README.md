# Product Management System (PMS)

A web application built with **ASP.NET Core 10 Razor Pages** that demonstrates the **Repository Design Pattern** combined with **ADO.NET** for fast, direct database access to **SQL Server**. The UI is built with **Bootstrap 5** and supports full product CRUD operations, including image upload and cleanup.

---

## Features

- **Full CRUD**
  - **Create** — add a product with image, title, description, price, quantity, and status.
  - **Read** — browse products in a clean table and open a details page for any product.
  - **Update** — edit product fields and optionally replace the product image.
  - **Delete** — remove a product and delete its image from disk.
- **Repository Pattern** — all data access is isolated in `ProductRepository`, using `SqlConnection`, `SqlCommand`, and `SqlDataReader`.
- **Image management** — `ProductImageService` validates file type and size (max 5 MB), generates unique GUID file names, saves under `wwwroot/images/products`, and removes files on delete/replace.
- **Async everywhere** — repository and page handlers are fully asynchronous.
- **Validation** — Data Annotations on the model plus jQuery Unobtrusive Validation on the client.
- **Responsive UI** — Bootstrap 5 layout with Bootstrap Icons, alerts, and status badges.

---

## Technology Stack

| Layer | Technology |
| --- | --- |
| Framework | ASP.NET Core 10 (Razor Pages) |
| Language | C# |
| Database | Microsoft SQL Server / LocalDB |
| Data access | ADO.NET (`Microsoft.Data.SqlClient` 7.1.1) |
| Frontend | HTML5, Bootstrap 5, Bootstrap Icons, jQuery |
| Pattern | Repository Design Pattern + dependency injection |

---

## Project Structure

```
PMS_CRUD_Razor_RepositoryDP/
├── ProductManagementSystem.slnx              # Solution file
├── run.bat                                   # Build & run helper for Windows
└── PMS_CRUD_Razor_RepositoryDP-main/
    ├── README.md
    └── Product_Managemnet_System_Razor_Core/
        ├── Data/
        │   ├── ProductRepository.cs          # ADO.NET data access (Repository Pattern)
        │   └── ProductImageService.cs        # Image validation, saving, deletion
        ├── Models/
        │   └── Product.cs                    # Entity + validation attributes
        ├── Pages/
        │   ├── Products/                     # Index, Create, Edit, Details, Delete
        │   └── Shared/_Layout.cshtml
        ├── Properties/launchSettings.json
        ├── wwwroot/images/products/          # Uploaded product images
        ├── appsettings.json                  # Connection string & logging config
        ├── Program.cs                        # DI, middleware, routing
        ├── pms script.txt                    # SQL script (database + table)
        └── Product_Managemnet_System_Razor_Core.csproj
```

---

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- SQL Server (LocalDB, Express, or full edition)
- Visual Studio 2022+ (optional) or any editor

### 1. Create the database

Run the script in `pms script.txt` using SQL Server Management Studio, Azure Data Studio, or Visual Studio's SQL Server Object Explorer:

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

### 2. Configure the connection string

Edit `appsettings.json` in `Product_Managemnet_System_Razor_Core` and point `DefaultConnection` at your SQL Server instance:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=PMS;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

Examples for other instances:

```json
"Server=.\\SQLEXPRESS;Database=PMS;Trusted_Connection=True;TrustServerCertificate=True;"
"Server=localhost,1433;Database=PMS;User Id=sa;Password=YourPassword;TrustServerCertificate=True;"
```

### 3. Build and run

From the project folder `Product_Managemnet_System_Razor_Core`:

```bash
dotnet restore
dotnet build
dotnet run
```

Or double-click `run.bat` in the repository root (Windows).

Then open: **http://localhost:5000** — the root URL redirects to `/Products`.

---

## Application Routes

| Page | Route | Description |
| --- | --- | --- |
| Product list | `/Products` | Table of all products |
| Create | `/Products/Create` | Add a new product with image upload |
| Details | `/Products/Details/{id}` | Read-only product view |
| Edit | `/Products/Edit/{id}` | Update product and replace image |
| Delete | `/Products/Delete/{id}` | Delete product and its image |

---

## How the Repository Pattern Is Used

1. `Program.cs` registers `ProductRepository` and `ProductImageService` as scoped services.
2. Razor Page models receive `ProductRepository` through constructor injection — pages never open a database connection directly.
3. `ProductRepository` builds parameterized ADO.NET commands (`@Id`, `@Title`, …) and maps rows to `Product` objects.
4. Swapping the data source only requires a new repository implementation, with no page changes.

```csharp
// Pages/Products/Index.cshtml.cs (abridged)
public class IndexModel : PageModel
{
    private readonly ProductRepository _productRepo;

    public IndexModel(ProductRepository productRepo) => _productRepo = productRepo;

    public List<Product> Products { get; set; } = new();

    public async Task OnGetAsync() => Products = await _productRepo.GetAllAsync();
}
```

---

## Security Notes

- All SQL statements use **parameterized queries**, preventing SQL injection.
- Uploads are restricted by extension and a 5 MB size limit.
- Store real credentials in user secrets or environment variables instead of committing them (`appsettings.Development.json` is not tracked).

---

## License

This project is provided for learning and demonstration purposes.
