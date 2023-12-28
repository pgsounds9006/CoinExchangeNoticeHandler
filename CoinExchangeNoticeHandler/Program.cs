using CoinExchangeNoticeHandler;
using CoinExchangeNoticeHandler.Abstracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder();

builder.Services.AddDbContext<AppDbContext>(options =>
    {
        options.UseSqlite(AppDbContext.ConnectionString);
    },
    contextLifetime: ServiceLifetime.Singleton,
    optionsLifetime: ServiceLifetime.Singleton
);
builder.Services.AddSingleton<BithumbNoticeCrawler>();
builder.Services.AddSingleton<INotifier<BithumbNotifyMessage>>(sp =>
    sp.GetRequiredService<BithumbNoticeCrawler>());
builder.Services.AddSingleton<NotifyMessageHandler>();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();

var dbContext = host.Services.GetRequiredService<AppDbContext>();
dbContext.Bithumb.RemoveRange(dbContext.Bithumb);
dbContext.SaveChanges();
dbContext.Database.Migrate();
dbContext.Database.OpenConnection();

await host.RunAsync();
