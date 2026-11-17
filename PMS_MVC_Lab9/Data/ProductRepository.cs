using System.Data;
using Microsoft.Data.SqlClient;
using PMS_MVC_Lab9.Models;

namespace PMS_MVC_Lab9.Data
{
    /// <summary>
    /// Repository Design Pattern: all SQL Server access for products lives here.
    /// The controllers never open a connection directly.
    /// </summary>
    public class ProductRepository
    {
        private readonly string _connectionString;

        public ProductRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "Connection string 'DefaultConnection' was not found in appsettings.json.");
        }

        public async Task<List<Product>> GetAllAsync()
        {
            const string sql = """
                SELECT Id, Title, [description], [image], price, quantity, [status]
                FROM dbo.products
                ORDER BY Id DESC;
                """;

            await using var connection = new SqlConnection(_connectionString);
            await using var command = new SqlCommand(sql, connection);

            await connection.OpenAsync();
            await using var reader = await command.ExecuteReaderAsync();

            var products = new List<Product>();
            while (await reader.ReadAsync())
            {
                products.Add(MapProduct(reader));
            }

            return products;
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            const string sql = """
                SELECT Id, Title, [description], [image], price, quantity, [status]
                FROM dbo.products
                WHERE Id = @Id;
                """;

            await using var connection = new SqlConnection(_connectionString);
            await using var command = new SqlCommand(sql, connection);
            command.Parameters.Add("@Id", SqlDbType.Int).Value = id;

            await connection.OpenAsync();
            await using var reader = await command.ExecuteReaderAsync();

            return await reader.ReadAsync() ? MapProduct(reader) : null;
        }

        public async Task<int> CreateAsync(Product product)
        {
            const string sql = """
                INSERT INTO dbo.products (Title, [description], [image], price, quantity, [status])
                OUTPUT INSERTED.Id
                VALUES (@Title, @Description, @Image, @Price, @Quantity, @Status);
                """;

            await using var connection = new SqlConnection(_connectionString);
            await using var command = new SqlCommand(sql, connection);
            AddProductParameters(command, product);

            await connection.OpenAsync();
            return Convert.ToInt32(await command.ExecuteScalarAsync());
        }

        public async Task<bool> UpdateAsync(Product product)
        {
            const string sql = """
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
            await using var command = new SqlCommand(sql, connection);
            AddProductParameters(command, product);
            command.Parameters.Add("@Id", SqlDbType.Int).Value = product.Id;

            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            const string sql = "DELETE FROM dbo.products WHERE Id = @Id;";

            await using var connection = new SqlConnection(_connectionString);
            await using var command = new SqlCommand(sql, connection);
            command.Parameters.Add("@Id", SqlDbType.Int).Value = id;

            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync() > 0;
        }

        // Parameterized queries protect the app from SQL injection.
        private static void AddProductParameters(SqlCommand command, Product product)
        {
            command.Parameters.Add("@Title", SqlDbType.VarChar, 50).Value = product.Title;
            command.Parameters.Add("@Description", SqlDbType.VarChar, 255).Value = product.Description;
            command.Parameters.Add("@Image", SqlDbType.VarChar, 255).Value = product.Image;
            command.Parameters.Add("@Price", SqlDbType.Int).Value = product.Price;
            command.Parameters.Add("@Quantity", SqlDbType.Int).Value = product.Quantity;
            command.Parameters.Add("@Status", SqlDbType.VarChar, 50).Value = product.Status;
        }

        private static Product MapProduct(SqlDataReader reader)
        {
            return new Product
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Title = reader.GetString(reader.GetOrdinal("Title")),
                Description = reader.GetString(reader.GetOrdinal("description")),
                Image = reader.GetString(reader.GetOrdinal("image")),
                Price = reader.GetInt32(reader.GetOrdinal("price")),
                Quantity = reader.GetInt32(reader.GetOrdinal("quantity")),
                Status = reader.GetString(reader.GetOrdinal("status"))
            };
        }
    }
}
