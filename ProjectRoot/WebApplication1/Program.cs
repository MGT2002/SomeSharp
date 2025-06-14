var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => {
    return "Hello World!";
});

app.MapGet("/square/{num}", (int num) => {
    return num * num;
});

app.Run();
