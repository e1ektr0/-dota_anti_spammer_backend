namespace DotaAntiSpammerCommon.Models
{
    public class Hero
    {
        public int Id { get; set; }
        public int Games { get; set; }
        public decimal WinRate { get; set; }
        public decimal PickRate { get; set; }
        public decimal FirstPickRate { get; set; }
        public decimal LastPickRate { get; set; }

        public bool Spam => PickRate > 90;

        public int? PickStage
        {
            get
            {
                if (FirstPickRate > 90)
                    return 1;
                if (LastPickRate > 90)
                    return 3;
                if (FirstPickRate + LastPickRate < 10)
                    return 2;
                return null;
            }
        }
    }
}