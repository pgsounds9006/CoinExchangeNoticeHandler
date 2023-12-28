namespace CoinExchangeNoticeHandler
{
    public class BithumbNotifyMessage
    {
        public string Message { get; set; } = "";
        public DateTime EventAt { get; set; } = DateTime.Now;
    }
}
