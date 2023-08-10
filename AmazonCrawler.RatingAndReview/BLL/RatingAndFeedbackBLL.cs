using AmazonCrawler.RatingAndReview.DAL;
using AmazonCrawler.RatingAndReview.Entity;

namespace AmazonCrawler.RatingAndReview.BLL
{
    public class RatingAndReviewBLL
    {
        readonly RatingAndReviewDAL RatingAndReviewDAL = new RatingAndReviewDAL();

        public async Task<List<AmazonItem>> ListItemsGetRatingAndReview(DateTime startDate, int top)
        {
            return await RatingAndReviewDAL.ListItemsGetRatingAndReview(startDate, top);
        }

        public async Task<int> SaveItemRating(string xml)
        {
            return await RatingAndReviewDAL.SaveItemRating(xml);
        }

        public async Task<bool> SaveItemReviews(string xml)
        {
            return await RatingAndReviewDAL.SaveItemReviews(xml);
        }

        public async Task<int> SaveCrawledDate(long itemId, string asin)
        {
            return await RatingAndReviewDAL.SaveCrawledDate(itemId, asin);
        }

        public async Task<int> PopulateItemNeedGetRatingAndReview()
        {
            return await RatingAndReviewDAL.PopulateItemNeedGetRatingAndReview();
        }
    }
}
