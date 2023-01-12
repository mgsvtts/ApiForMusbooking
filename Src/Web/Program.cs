using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Persistence.Repositories;
using Presentation;
using Services;
using Services.Abstractions;
using Web.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
                .AddApplicationPart(typeof(AssemblyReference).Assembly);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("Database").Replace("~", builder.Environment.ContentRootPath);
builder.Services.AddDbContext<ApplicationContext>(options =>
                options.UseSqlite(connectionString));

builder.Services.AddTransient<HandleExceptionsMiddleware>();
builder.Services.AddScoped<IRepositoryManager, RepositoryManager>();
builder.Services.AddScoped<IServiceObjectService, ServiceObjectService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

    app.UseSwagger();

    app.UseSwaggerUI();
}

app.UseMiddleware<HandleExceptionsMiddleware>();

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();
