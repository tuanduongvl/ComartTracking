using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComartTracking
{
    internal class BoxRecord
    {
        public string BoxID { get; set; }
        public string LotID { get; set; }
        public string PartID { get; set; }
        public DateTime DateTime { get; set; }
        public int BoxNo { get; set; }
        public BoxRecord(string boxID, string lotID, string partID, DateTime dateTime, int boxNo)
        {
            BoxID = boxID;
            LotID = lotID;
            PartID = partID;
            DateTime = dateTime;
            BoxNo = boxNo;
        }
    }
}
