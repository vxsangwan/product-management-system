using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

using API.Data;


var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(connectionString));

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(
    options => {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }
).AddJwtBearer(
    options => {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    }
);

builder.Services.AddAuthorization(
    options => {
        options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
        options.AddPolicy("RequireUserRole", policy => policy.RequireRole("User"));
        options.AddPolicy("RequireHumanResourcesRole", policy => policy.RequireRole("HumanResources"));
        // ...
    }
);

// ---


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); // API Documentation


// ---

var app = builder.Build();

using (var scope = app.Services.CreateScope()){

    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    if(!(await roleManager.RoleExistsAsync("User"))){
        await roleManager.CreateAsync(new IdentityRole("User"));
    }

    if(!(await roleManager.RoleExistsAsync("Admin"))){
        await roleManager.CreateAsync(new IdentityRole("Admin"));
    }

    if(!(await roleManager.RoleExistsAsync("HumanResources"))){
        await roleManager.CreateAsync(new IdentityRole("HumanResources"));
    }
}


if(app.Environment.IsDevelopment()){
    app.UseSwagger();
    app.UseSwaggerUI();
}

if(!app.Environment.IsDevelopment()){
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();