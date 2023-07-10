using Domain;
using Domain.Identity;
using Repository.Interface;
using Repository;
using Services;
using Services.Implementation;
using Services.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Repository.Implementation;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                       throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

builder.Services.AddRazorPages();

EmailSettings emailService = new EmailSettings();

builder.Services.Configure<StripeSettings>("Stripe", builder.Configuration.GetSection("Stripe"));
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped(typeof(IUserRepository), typeof(UserRepository));
builder.Services.AddScoped(typeof(IOrderRepository), typeof(OrderRepository));
builder.Services.AddTransient<IShoppingCartService, ShoppingCartService>();
builder.Services.AddTransient<IOrderService, Services.Implementation.OrderService>();
builder.Services.AddTransient<ITicketService, Services.Implementation.TicketService>();
builder.Services.AddTransient<IUserService, Services.Implementation.UserService>();
builder.Services.AddScoped<EmailSettings>(es => emailService);
builder.Services.AddScoped<IEmailService, EmailService>(email => new EmailService(emailService));
builder.Services.AddScoped<IBackgroundEmailSender, BackgroundEmailSender>();
builder.Services.AddHostedService<ConsumeScopedHostedService>();    

var app = builder.Build();

StripeConfiguration.SetApiKey(builder.Configuration.GetSection("Stripe")["SecretKey"]);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();