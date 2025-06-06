using RestNDSignalR.Hubs;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSignalR();
var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.MapHub<InventoryHub>("/inventoryHub");
app.MapHub<DishHub>("/dishHub");


app.Run();
