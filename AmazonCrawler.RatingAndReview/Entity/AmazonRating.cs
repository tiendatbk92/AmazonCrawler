
namespace AmazonCrawler.RatingAndReview.Entity
{
    public class AmazonRating
    {
        public long ItemID { get; set; }
        public string ASIN { get; set; }
        public int? NumberOfRatings { get; set; }
        public int? NumberOfReviews { get; set; }
        public float? TotalRate { get; set; }
        public float? _1Star { get; set; }
        public float? _2Star { get; set; }
        public float? _3Star { get; set; }
        public float? _4Star { get; set; }
        public float? _5Star { get; set; }

        public bool ShouldSerializeNumberOfRatings()
        {
            return NumberOfRatings != null;
        }
        public bool ShouldSerializeNumberOfReviews()
        {
            return NumberOfReviews != null;
        }
        public bool ShouldSerializeTotalRate()
        {
            return TotalRate != null;
        }
        public bool ShouldSerialize_1Star()
        {
            return _1Star != null;
        }
        public bool ShouldSerialize_2Star()
        {
            return _2Star != null;
        }
        public bool ShouldSerialize_3Star()
        {
            return _3Star != null;
        }
        public bool ShouldSerialize_4Star()
        {
            return _4Star != null;
        }
        public bool ShouldSerialize_5Star()
        {
            return _5Star != null;
        }
    }
}
