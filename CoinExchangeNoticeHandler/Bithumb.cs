using System.ComponentModel.DataAnnotations.Schema;

namespace CoinExchangeNoticeHandler
{
    public class Bithumb
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public DateTime CreatedAt { get; set; }
    }
}
