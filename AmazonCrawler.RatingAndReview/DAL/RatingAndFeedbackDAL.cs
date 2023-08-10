using AmazonCrawler.Common;
using Dapper;
using AmazonCrawler.RatingAndReview.Entity;
using System.Data;
using System.Data.SqlClient;

namespace AmazonCrawler.RatingAndReview.DAL
{
    public class RatingAndReviewDAL
    {
        public async Task<List<AmazonItem>> ListItemsGetRatingAndReview(DateTime startDate, int top)
        {
            using (var db = new SqlConnection(DbConnectionString.GetConnection(DbConnectionString.ConnectionName.DataWarehouse)))
            {
                var param = new DynamicParameters();
                param.Add("@StartDate", startDate);
                param.Add("@Top", top);
                db.Open();
                var result = await db.QueryAsync<AmazonItem>("Amz_List_Items_Get_Rating_And_Review", param, commandType: CommandType.StoredProcedure, commandTimeout: Int32.MaxValue);
                db.Close();
                db.Dispose();
                return result.ToList();
            }
        }

        public async Task<int> SaveItemRating(string xml)
        {
            using (var db = new SqlConnection(DbConnectionString.GetConnection(DbConnectionString.ConnectionName.DataWarehouse)))
            {
                var param = new DynamicParameters();
                param.Add("@xml", xml);
                db.Open();
                var result = await db.ExecuteAsync("Amz_Save_Item_Rating", param, commandType: CommandType.StoredProcedure, commandTimeout: Int32.MaxValue);
                db.Close();
                db.Dispose();
                return result;
            }
        }

        public async Task<bool> SaveItemReviews(string xml)
        {
            using (var db = new SqlConnection(DbConnectionString.GetConnection(DbConnectionString.ConnectionName.DataWarehouse)))
            {
                var param = new DynamicParameters();
                param.Add("@xml", xml);
                param.Add("@NextPage", dbType: DbType.Boolean, direction: ParameterDirection.Output);
                db.Open();
                var result = await db.ExecuteAsync("Amz_Save_Item_Reviews", param, commandType: CommandType.StoredProcedure, commandTimeout: Int32.MaxValue);
                db.Close();
                db.Dispose();
                return param.Get<Boolean>("@NextPage");
            }
        }

        public async Task<int> SaveCrawledDate(long itemId, string asin)
        {
            using (var db = new SqlConnection(DbConnectionString.GetConnection(DbConnectionString.ConnectionName.DataWarehouse)))
            {
                var param = new DynamicParameters();
                param.Add("@ItemID", itemId);
                param.Add("@ASIN", asin);
                db.Open();
                var result = await db.ExecuteAsync("Amz_Save_Crawled_Date", param, commandType: CommandType.StoredProcedure, commandTimeout: Int32.MaxValue);
                db.Close();
                db.Dispose();
                return result;
            }
        }

        public async Task<int> PopulateItemNeedGetRatingAndReview()
        {
            using (var db = new SqlConnection(DbConnectionString.GetConnection(DbConnectionString.ConnectionName.DataWarehouse)))
            {
                var param = new DynamicParameters();
                db.Open();
                var result = await db.ExecuteAsync("Amz_Populate_Item_Need_Get_Rating_And_Review", param, commandType: CommandType.StoredProcedure, commandTimeout: Int32.MaxValue);
                db.Close();
                db.Dispose();
                return result;
            }
        }
    }
}
