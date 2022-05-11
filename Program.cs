using Microsoft.EntityFrameworkCore;
using XmlImport;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllersWithViews();

//��� codeFirst ��������� ����� microsoft.entityframeworkcore.tools
//����� � ������� Nuget ������ Add-Migration InitialDatabase
//Update-Database






var app = builder.Build();

// Configure the HTTP request pipeline.
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
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
