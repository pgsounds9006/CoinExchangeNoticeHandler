using CoinExchangeNoticeHandler.Abstracts;
using Microsoft.Extensions.Logging;

namespace CoinExchangeNoticeHandler
{
    public class NotifyMessageHandler(
        INotifier<BithumbNotifyMessage> notifier,
        ILogger<NotifyMessageHandler> logger)
    {
        public void Subscribe()
        {
            notifier.Notify += OnMessageReceived;
        }

        private void OnMessageReceived(object? sender, BithumbNotifyMessage e)
        {
            logger.LogInformation("[Bithumb]새로운 공지 발생:{Message} - {EventAt}", e.Message, e.EventAt);
        }
    }
}
