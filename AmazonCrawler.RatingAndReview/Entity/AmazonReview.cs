using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonCrawler.RatingAndReview.Entity
{
    public  class AmazonReview
    {
        public long ItemID { get; set; }
        public string ASIN { get; set; }
        public string ReviewID { get; set; }
        public string Reviewer { get; set; }
        public string ReviewDate { get; set; }
        public string ReviewTitle { get; set; }
        public string ReviewContent { get; set; }
        public float? Rate { get; set; }
        public bool ShouldSerializeRate()
        {
            return Rate != null;
        }
    }
}
