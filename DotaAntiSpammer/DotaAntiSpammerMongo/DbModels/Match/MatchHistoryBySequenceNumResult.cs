using System.Collections.Generic;

namespace DotaAntiSpammerMongo.Models.Match
{
    public class MatchHistoryBySequenceNumResult
    {
        public byte status { get; set; }
        public string statusDetail { get; set; }
        public IList<MatchDetails> matches { get; set; }
    }
}