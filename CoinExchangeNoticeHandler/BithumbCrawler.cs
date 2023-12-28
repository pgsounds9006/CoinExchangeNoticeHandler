using System.Text.RegularExpressions;
using AngleSharp;
using AngleSharp.Text;
using CoinExchangeNoticeHandler.Abstracts;
using Microsoft.Extensions.Logging;

namespace CoinExchangeNoticeHandler
{
    public partial class BithumbNoticeCrawler : INotifier<BithumbNotifyMessage>
    {
        private readonly AppDbContext dbContext;
        private readonly ILogger<BithumbNoticeCrawler>? logger;
        private readonly IBrowsingContext browsingContext;
        public event EventHandler<BithumbNotifyMessage>? Notify;

        public BithumbNoticeCrawler(AppDbContext dbContext, ILogger<BithumbNoticeCrawler>? logger = null)
        {
            this.dbContext = dbContext;
            this.logger = logger;

            var cfg = Configuration.Default.WithDefaultLoader();
            browsingContext = BrowsingContext.New(cfg);
        }

        public static string Uri => "https://cafe.bithumb.com/view/boards/43";
        public static TimeSpan Interval => TimeSpan.FromSeconds(
            3 + (new Random().Next(0, 1000) / 1000) // 1초 랜덤
        );
        public static CancellationTokenSource TimeOut => new(TimeSpan.FromSeconds(1));

        public async Task RunAsync(CancellationToken appCancellation = default)
        {
            try
            {
                while (true)
                {
                    await Work();
                    await Task.Delay(Interval, appCancellation);
                }
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                logger?.LogError("{ExceptionMessage}", ex.Message);
                throw;
            }
        }

        public async Task Work()
        {
            logger?.LogInformation("Start");
            try
            {
                var document = await browsingContext.OpenAsync(Uri, TimeOut.Token);
                var rows = document.QuerySelectorAll("#dataTables > tbody > tr");

                foreach (var row in rows)
                {
                    var titleElem = row.QuerySelector("td:nth-child(2)");
                    if (titleElem == null) continue;

                    var aElem = titleElem.QuerySelector("a");
                    if (aElem == null) continue;

                    var aElemOnclickContent = aElem.GetAttribute("onclick");
                    if (aElemOnclickContent == null) continue;

                    var postNumRegex = OnclickRegex();
                    if (!postNumRegex.Match(aElemOnclickContent).Groups[1].Success) continue;

                    int postNum = postNumRegex.Match(aElemOnclickContent).Groups[1].Value.ToInteger(default);
                    if (postNum == default) continue;

                    var content = titleElem.TextContent.Trim();
                    var match = TitleRegex().Match(content);

                    var tag = match.Groups["Tag"];
                    var title = match.Groups["Title"];

                    if (!tag.Success || !title.Success) continue;

                    if (dbContext.Bithumb.Any(o => o.Id == postNum)) continue;

                    Notify?.Invoke(this, new BithumbNotifyMessage
                    {
                        Message = $"[{tag.Value}]{title.Value}",
                        EventAt = DateTime.Now,
                    });

                    dbContext.Bithumb.Add(new Bithumb
                    {
                        Id = postNum,
                        CreatedAt = DateTime.Now,
                        Title = content,
                    });

                    dbContext.SaveChanges();
                }
            }
            catch (OperationCanceledException)
            {
                logger?.LogInformation("Timeout reached");
            }
            catch (Exception ex)
            {
                logger?.LogError("{ExceptionMessage}", ex.Message);
                throw;
            }
        }

        [GeneratedRegex(@"\[(?<Tag>.+)\] (?<Title>.+)")]
        private static partial Regex TitleRegex();
        [GeneratedRegex(@"toDetailOrUrl\(event, \'(\d+)\'")]
        private static partial Regex OnclickRegex();
    }
}
