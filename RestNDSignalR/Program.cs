using RestNDSignalR.Hubs;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSignalR();
var app = builder.Build();
app.MapHub<InventoryHub>("/inventoryHub");
<<<<<<< HEAD
app.Run("http://localhost:5047");
=======
app.MapHub<DishHub>("/dishHub");


app.Run();
>>>>>>> 4b2e2f0cbcea4c9fee549741708659cc87b288af
