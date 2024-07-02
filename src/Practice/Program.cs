using System.Data.SqlClient;
using Dapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Practice.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var connectionString = "Server=tcp:productionserversql.database.windows.net,1433;Initial Catalog=ProductionDb;Persist Security Info=False;User ID=azureuser;Password=max123456789$;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

var connection = new SqlConnection(connectionString);

var defaultUsers = new[]
{
    new User(){
        Id = 1,
        Username = "max"
    },

    new User(){
        Id = 2,
        Username = "alex"
    },

    new User(){
        Id = 3,
        Username = "qasim"
    },

    new User(){
        Id = 4,
        Username = "sadiq"
    },
};
try 
{
    await connection.ExecuteAsync(@"create table Users(
        Id int not null,
        Username nvarchar(60) not null
    )");
}
catch(Exception ex)
{
    System.Console.WriteLine(ex.Message);
}





app.MapGet("/User", async () =>
{
    try
    {
        var user = await connection.QueryAsync<User>(@"select *
                                                from Users");
        return user;
    }
    catch(Exception ex)
    {
        System.Console.WriteLine(ex.Message);
        return null;
    }
});

app.MapPut("/User", async (int userId, User user) =>
{   
    try
    {
        user.Id = userId;
        await connection.ExecuteAsync(@"upsate Users
                                set Username = @Username
                                where Id =@Id", user);
        return Results.Ok(); 
    }
    catch(Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
});

app.MapPost("/User", async (User user) =>
{
    try
    {
        await connection.ExecuteAsync(@"insert into Users
                                (Id, Username)
                                values (@Id, @Username)", user);
        return Results.Ok(); 
    }
    catch(Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
});

app.MapDelete("/User", async (int userId) =>
{
    await connection.ExecuteAsync(@"delete from Users
                                        where Id = @Id", new {
                                            Id = userId
                                        });
});



app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
