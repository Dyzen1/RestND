using RestNDSignalR.Hubs;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSignalR();
var app = builder.Build();

<<<<<<< HEAD
app.MapHub<InventoryHub>("/inventoryHub");
app.Run("http://localhost:5047"); //
=======
app.MapGet("/", () => "Hello World!");
app.MapHub<InventoryHub>("/inventoryHub");

app.Run();
>>>>>>> 552c4bd5460e2d717d68ad1a5f45076c3608055f
