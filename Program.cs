using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using TodoApi;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", 
        builder =>
        {
            builder.AllowAnyOrigin() // מאפשר לכל המקורות לגשת
                   .AllowAnyMethod() // מאפשר כל שיטה (GET, POST, וכו')
                   .AllowAnyHeader(); // מאפשר כל כותרת
        });
});
builder.Services.AddEndpointsApiExplorer(); 
builder.Services.AddSwaggerGen();//הוספת הswagger
builder.Services.AddDbContext<ToDoDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("ToDoDB"), 
        new MySqlServerVersion(new Version(8, 0, 21))));
var app = builder.Build();
app.UseCors("AllowAllOrigins"); 
app.UseSwagger(); // הפעלת Swagger
app.UseSwaggerUI(c => 
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"); 
    c.RoutePrefix = string.Empty; 
});
app.MapGet("/tasks", async (ToDoDbContext db) =>
{
    return await db.Items.ToListAsync();
});
app.MapPost("/addTask", async (ToDoDbContext db, Item task) =>
{
    db.Items.Add(task);
    await db.SaveChangesAsync();
    return Results.Created($"/tasks/{task.Id}", task);
});
app.MapPost("/tasks/{id}", async (int id, ToDoDbContext db, Item updatedTask) =>
{
    var task = await db.Items.FindAsync(id);
    if (task is null) return Results.NotFound();

    task.Name=updatedTask.Name;
    task.IsComplete = updatedTask.IsComplete;
    await db.SaveChangesAsync();
    return Results.NoContent();
});
app.MapDelete("/tasks/{id}", async (int id, ToDoDbContext db) =>
{
    var task = await db.Items.FindAsync(id);
    if (task is null) return Results.NotFound();

    db.Items.Remove(task);
    await db.SaveChangesAsync();
    return Results.NoContent();
});
app.MapGet("/", () => "Hello World!");

app.Run();
