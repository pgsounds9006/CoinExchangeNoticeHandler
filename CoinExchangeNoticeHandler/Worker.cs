using Microsoft.Extensions.Hosting;

namespace CoinExchangeNoticeHandler
{
    public class Worker(
        BithumbNoticeCrawler bithumbNoticeCrawler,
        NotifyMessageHandler notifyMessageHandler)
        : IHostedService
    {
        public Task StartAsync(CancellationToken cancellationToken)
        {
            notifyMessageHandler.Subscribe();
            _ = bithumbNoticeCrawler.RunAsync(cancellationToken);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
