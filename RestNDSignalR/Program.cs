using RestNDSignalR.Hubs;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSignalR();
var app = builder.Build();
app.MapHub<InventoryHub>("/inventoryHub");
app.Run("http://localhost:5047");