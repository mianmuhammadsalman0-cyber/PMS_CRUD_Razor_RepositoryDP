using Microsoft.Data.SqlClient;
using System.Data;
using Product_Managemnet_System_Razor_Core.Models;

namespace Product_Managemnet_System_Razor_Core.Data
{
    /// <summary>
    /// Repository class implementing data access for Products using ADO.NET
    /// Demonstrates the Repository Design Pattern for clean separation of concerns.
    /// </summary>
    public class ProductRepository
    {
        private readonly string _connectionString;

        // Constructor: Inject IConfiguration to read connection string from appsettings.json
        public ProductRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Database connection string 'DefaultConnection' was not found in appsettings.json.");
        }

        /// <summary>
        /// Reads all products from database ordered by newest first.
        /// </summary>
        public async Task<List<Product>> GetAllAsync()
        {
            var productList = new List<Product>();

            const string selectQuery = """
                SELECT Id, Title, [description], [image], price, quantity, [status]
                FROM dbo.products
                ORDER BY Id DESC;
                """;

            await using var connection = new SqlConnection(_connectionString);
            await using var command = new SqlCommand(selectQuery, connection);

            await connection.OpenAsync();
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                productList.Add(MapProduct(reader));
            }

            return productList;
        }

        /// <summary>
        /// Retrieves a single product by its Primary Key (Id).
        /// Returns null if product is not found.
        /// </summary>
        public async Task<Product?> GetByIdAsync(int id)
        {
            const string queryById = """
                SELECT Id, Title, [description], [image], price, quantity, [status]
                FROM dbo.products
                WHERE Id = @Id;
                """;

            await using var connection = new SqlConnection(_connectionString);
            await using var command = new SqlCommand(queryById, connection);

            // Parameterized query to prevent SQL Injection
            command.Parameters.Add("@Id", SqlDbType.Int).Value = id;

            await connection.OpenAsync();
            await using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return MapProduct(reader);
            }

            return null;
        }

        /// <summary>
        /// Inserts a new product record into the database and returns the generated Id.
        /// Uses OUTPUT INSERTED.Id to retrieve the identity value directly.
        /// </summary>
        public async Task<int> CreateAsync(Product product)
        {
            const string insertQuery = """
                INSERT INTO dbo.products (Title, [description], [image], price, quantity, [status])
                OUTPUT INSERTED.Id
                VALUES (@Title, @Description, @Image, @Price, @Quantity, @Status);
                """;

            await using var connection = new SqlConnection(_connectionString);
            await using var command = new SqlCommand(insertQuery, connection);

            AddProductParameters(command, product);

            await connection.OpenAsync();
            object? newId = await command.ExecuteScalarAsync();

            return Convert.ToInt32(newId);
        }

        /// <summary>
        /// Updates an existing product record in the database.
        /// Returns true if at least one row was affected.
        /// </summary>
        public async Task<bool> UpdateAsync(Product product)
        {
            const string updateQuery = """
                UPDATE dbo.products
                SET Title = @Title,
                    [description] = @Description,
                    [image] = @Image,
                    price = @Price,
                    quantity = @Quantity,
                    [status] = @Status
                WHERE Id = @Id;
                """;

            await using var connection = new SqlConnection(_connectionString);
            await using var command = new SqlCommand(updateQuery, connection);

            AddProductParameters(command, product);
            command.Parameters.Add("@Id", SqlDbType.Int).Value = product.Id;

            await connection.OpenAsync();
            int rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        /// <summary>
        /// Deletes a product from database by its Id.
        /// Returns true if deletion was successful.
        /// </summary>
        public async Task<bool> DeleteAsync(int id)
        {
            const string deleteQuery = """
                DELETE FROM dbo.products
                WHERE Id = @Id;
                """;

            await using var connection = new SqlConnection(_connectionString);
            await using var command = new SqlCommand(deleteQuery, connection);

            command.Parameters.Add("@Id", SqlDbType.Int).Value = id;

            await connection.OpenAsync();
            int rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        /// <summary>
        /// Helper: Maps a single row from SqlDataReader to a Product model instance.
        /// </summary>
        private static Product MapProduct(SqlDataReader reader)
        {
            return new Product
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Title = reader.GetString(reader.GetOrdinal("Title")),
                description = reader.GetString(reader.GetOrdinal("description")),
                image = reader.GetString(reader.GetOrdinal("image")),
                price = reader.GetInt32(reader.GetOrdinal("price")),
                quantity = reader.GetInt32(reader.GetOrdinal("quantity")),
                status = reader.GetString(reader.GetOrdinal("status"))
            };
        }

        /// <summary>
        /// Helper: Sets all parameters on SqlCommand from Product entity to prevent SQL injection.
        /// </summary>
        private static void AddProductParameters(SqlCommand command, Product product)
        {
            command.Parameters.Add("@Title", SqlDbType.VarChar, 50).Value = product.Title ?? string.Empty;
            command.Parameters.Add("@Description", SqlDbType.VarChar, 255).Value = product.description ?? string.Empty;
            command.Parameters.Add("@Image", SqlDbType.VarChar, 255).Value = product.image ?? string.Empty;
            command.Parameters.Add("@Price", SqlDbType.Int).Value = product.price;
            command.Parameters.Add("@Quantity", SqlDbType.Int).Value = product.quantity;

            // Safe status: if 'Active' or 'A', ensure it's compatible with both VARCHAR(1) and VARCHAR(50)
            string statusVal = string.IsNullOrWhiteSpace(product.status) ? "A" : product.status;
            command.Parameters.Add("@Status", SqlDbType.VarChar, 50).Value = statusVal;
        }
    }
}
