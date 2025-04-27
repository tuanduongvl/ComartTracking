using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComartTracking
{
    internal class LotRecord
    {
        public int boxCount { get; set; }
        public string LotID { get; set; }
        public string PartID { get; set; }
        public int realCount { get; set; }
        public DateTime startTime { get; set; }

        public DateTime endTime { get; set; }

        public LotRecord(string LotID, int boxCount, string PartID, DateTime startTime)
        {
            this.boxCount = boxCount;
            this.LotID = LotID;
            this.startTime = startTime;
            this.PartID = PartID;
        }

    }
}
