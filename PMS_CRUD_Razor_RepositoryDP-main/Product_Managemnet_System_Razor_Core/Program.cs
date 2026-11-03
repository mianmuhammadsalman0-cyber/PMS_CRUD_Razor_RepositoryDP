using Product_Managemnet_System_Razor_Core.Data;

var builder = WebApplication.CreateBuilder(args);

// Add Razor Pages services to the DI container
builder.Services.AddRazorPages();

// Register application services with Scoped lifetime
builder.Services.AddScoped<ProductRepository>();
builder.Services.AddScoped<ProductImageService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

// Default route redirects directly to Products listing page
app.MapGet("/", () => Results.Redirect("/Products"));
app.MapRazorPages();

app.Run();
