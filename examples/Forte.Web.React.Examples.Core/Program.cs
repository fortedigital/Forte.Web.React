using Forte.Web.React;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddReact(nodeOptions =>
{
    // optional transformations
}, nodeServiceOptions =>
{
    // optional transformations
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

var dir = app.Environment.WebRootPath;
var js = Directory.GetFiles(Path.Combine(dir, "Client/dist/assets")).First(f => f.EndsWith(".js"));
app.UseReact(new[] { js }, new Version(18, 2, 0), strictMode: true);

app.Run();