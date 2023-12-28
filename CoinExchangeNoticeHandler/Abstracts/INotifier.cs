namespace CoinExchangeNoticeHandler.Abstracts
{
    public interface INotifier<T>
    {
        event EventHandler<T>? Notify;
    }
}
