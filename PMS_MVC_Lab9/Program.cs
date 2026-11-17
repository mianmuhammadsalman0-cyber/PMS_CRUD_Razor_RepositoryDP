using PMS_MVC_Lab9.Data;

var builder = WebApplication.CreateBuilder(args);

// MVC (Controllers + Views) and the repository used by ProductsController.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<ProductRepository>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Products}/{action=Index}/{id?}");

app.Run();
