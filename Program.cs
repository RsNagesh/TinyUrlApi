using Exam.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader());
});

var app = builder.Build();

app.UseCors("AllowAll");


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/urls", async (UrlCreateRequest request, AppDbContext db) =>
{
    if (string.IsNullOrWhiteSpace(request.OriginalUrl))
    {
        return Results.BadRequest("OriginalUrl is required.");
    }

    string shortCode = GenerateShortCode();

    var url = new Url
    {
        OriginalUrl = request.OriginalUrl,
        ShortCode = shortCode,
        IsPrivate = request.IsPrivate
    };

    db.Urls.Add(url);
    await db.SaveChangesAsync();

    var baseUrl = "https://localhost:5001/";
    var shortUrl = $"{baseUrl}{shortCode}";

    return Results.Ok(new { shortUrl, url.Id, url.OriginalUrl, url.IsPrivate });
});

app.MapGet("/{shortCode}", async (string shortCode, AppDbContext db) =>
{
    var url = await db.Urls.FirstOrDefaultAsync(u => u.ShortCode == shortCode);

    if (url == null)
        return Results.NotFound("Short URL not found");

    url.Clicks++;
    await db.SaveChangesAsync();
    return Results.Redirect(url.OriginalUrl);
});

app.MapGet("/urls", async (AppDbContext db) =>
{
    var urls = await db.Urls
        .Select(u => new
        {
            Id = u.Id.ToString(),
            u.OriginalUrl,
            ShortUrl = $"https://localhost:7120/{u.ShortCode}",
            u.ShortCode,
            Clicks = u.Clicks,  
            u.IsPrivate,
            u.CreatedAt
        })
        .ToListAsync();

    return Results.Ok(urls);
});


app.MapDelete("/urls/{shortCode}", async (string shortCode, AppDbContext db) =>
{
    var url = await db.Urls.FirstOrDefaultAsync(u => u.ShortCode == shortCode);

    if (url == null)
        return Results.NotFound("Short URL not found");

    db.Urls.Remove(url);
    await db.SaveChangesAsync();

    return Results.Ok($"Short URL {shortCode} deleted successfully");
});

string GenerateShortCode()
{
    var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    var random = new Random();
    return new string(Enumerable.Repeat(chars, 6)
        .Select(s => s[random.Next(s.Length)]).ToArray());
}

app.UseAuthorization();

app.MapControllers();

app.Run();
