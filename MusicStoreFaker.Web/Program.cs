using Microsoft.EntityFrameworkCore;
using MusicStoreFaker.Data;
using MusicStoreFaker.Core.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<LocaleDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("LocaleDb")));

builder.Services.AddSingleton<LocaleDataProvider>();
builder.Services.AddSingleton<ILocaleDataProvider>(sp => sp.GetRequiredService<LocaleDataProvider>());
builder.Services.AddSingleton<ICoverGenerator, CoverGenerator>();
builder.Services.AddSingleton<IAudioGenerator>(sp =>
{
    var env = sp.GetRequiredService<IWebHostEnvironment>();
    string sfPath = Path.Combine(env.WebRootPath, "soundfonts", "TimGM6mb.sf2");
    return new AudioGenerator(sfPath);
});

builder.Services.AddScoped<ISongGenerator, SongGenerator>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("DevCors", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<LocaleDbContext>();
    await SeedData.InitializeAsync(context);

    var localeProvider = scope.ServiceProvider.GetRequiredService<LocaleDataProvider>();
    localeProvider.LoadData(context);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("DevCors");
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();