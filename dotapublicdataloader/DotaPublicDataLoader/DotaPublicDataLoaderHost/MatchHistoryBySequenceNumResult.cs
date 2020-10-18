using System;
using System.Collections.Generic;

namespace DotaPublicDataLoaderHost
{
    public class MatchHistoryBySequenceNumResult
    {
        public Byte status { get; set; }
        public string statusDetail { get; set; }
        public IList<MatchDetails> matches { get; set; }
    }
}