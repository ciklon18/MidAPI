using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MisAPI.Configurations;
using MisAPI.Data;
using MisAPI.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        In = ParameterLocation.Header,
        Scheme = "bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            Array.Empty<string>()
        }
    });
});


builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddJwt(builder.Configuration);
builder.Services.AddSingletons(builder.Configuration);
builder.Services.AddServices();



var app = builder.Build();



var dbContext = app.Services.CreateScope().ServiceProvider.GetService<ApplicationDbContext>();

if (dbContext != null)
{
    dbContext.Database.Migrate();
    var migrator = app.Services.GetRequiredService<DatabaseMigrator>();
    migrator.MigrateDatabase();
    
}

app.UseExceptionMiddleware();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();
app.MapControllers();
 
app.Run();

