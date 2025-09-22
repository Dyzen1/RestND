using RestNDSignalR.Hubs;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSignalR();
var app = builder.Build();

app.MapHub<InventoryHub>("/inventoryHub");
app.MapHub<DishHub>("/dishHub");
app.MapHub<TableHub>("/tableHub");
app.MapHub<MainHub>("/mainHub");
app.MapHub<EmployeeHub>("/employeeHub");
app.MapHub<DishTypeHub>("/dishtypeHub");


app.Run();
