using AmazonCrawler.Common;
using HtmlAgilityPack;
using AmazonCrawler.RatingAndReview.BLL;
using AmazonCrawler.RatingAndReview.Entity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;

namespace AmazonCrawler.RatingAndReview
{
    class Program
    {
        private static int blockItems = AppSettings.GetInt32("BlockItems") ?? 0;
        private static int waitForDelay = AppSettings.GetInt32("WaitForDelay") ?? 0;
        private static string Url = "https://www.amazon.com/product-reviews/{0}/reviewerType=all_reviews?sortBy=recent&pageNumber={1}&formatType=current_format";
        private static int maxItemEmptyData = AppSettings.GetInt32("MaxItemEmptyData") ?? 0;
        private static IWebDriver driver = new ChromeDriver();
        private static string cookie = string.Empty;

        static async Task Main(string[] args)
        {
            var continueItem = true;
            var bs = new RatingAndReviewBLL();
            var startDate = DateTime.Now;
            var countItemEmptyData = 0;

            await bs.PopulateItemNeedGetRatingAndReview();

            while (continueItem)
            {
                var listAmazonItems = await bs.ListItemsGetRatingAndReview(startDate, blockItems);

                if (listAmazonItems.Count > 0)
                {
                    foreach (var amazonItem in listAmazonItems)
                    {
                        var nextPage = true;
                        var pageNumber = 1;
                        var crawled = false;
                        while (nextPage)
                        {
                            var html = GetHtml(amazonItem.ASIN, pageNumber);

                            HtmlDocument document = new HtmlDocument();
                            document.LoadHtml(html);

                            if (pageNumber == 1)
                            {
                                var amazonRating = GetAmazonRating(amazonItem, document);
                                if (amazonRating != null && amazonRating.NumberOfRatings != null)
                                {
                                    var xmlRating = Utils.SerializeXML<AmazonRating>(amazonRating);
                                    var re = await bs.SaveItemRating(xmlRating);
                                    countItemEmptyData = 0;
                                    crawled = true;
                                }
                                else
                                {
                                    crawled = false;
                                    countItemEmptyData++;
                                }
                            }

                            var amazonReviews = GetAmazonReviews(amazonItem, document);
                            if (amazonReviews != null && amazonReviews.Count > 0)
                            {
                                var xmlReviews = Utils.SerializeXML<List<AmazonReview>>(amazonReviews);
                                nextPage = await bs.SaveItemReviews(xmlReviews);
                            }
                            else
                            {
                                nextPage = false;
                            }

                            if (countItemEmptyData >= maxItemEmptyData)
                            {
                                return;
                            }

                            if (nextPage)
                            {
                                pageNumber++;
                                Thread.Sleep(waitForDelay * 1000);
                            }
                        }

                        if (crawled)
                        {
                            await bs.SaveCrawledDate(amazonItem.ItemID, amazonItem.ASIN);
                        }

                        if (countItemEmptyData >= maxItemEmptyData)
                        {
                            return;
                        }

                        Thread.Sleep(waitForDelay * 1000);
                    }

                    if (listAmazonItems.Count < blockItems)
                    {
                        continueItem = false;
                    }
                }
                else
                {
                    continueItem = false;
                }
            }
            driver.Close();
        }

        //private static string GetHtml(string asin, int pageNumber)
        //{
        //    var url = string.Format(Url, asin, pageNumber);
        //    WebRequest req = HttpWebRequest.Create(url);
        //    req.Method = "GET";
        //    req.ContentType = "text/html";
        //    req.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/114.0.0.0 Safari/537.36");
        //    req.Headers.Add("Accept-Language", "en-US, en;q=0.5");
        //    if(!string.IsNullOrEmpty(cookie))
        //    {
        //        req.Headers.Add("Cookie", cookie);
        //    }
        //    string source;
        //    HttpWebResponse respone = (HttpWebResponse)req.GetResponse();
        //    using (StreamReader reader = new StreamReader(respone.GetResponseStream()))
        //    {
        //        source = reader.ReadToEnd();
        //    }
        //    cookie = respone.Headers[HttpResponseHeader.SetCookie];
        //    var aaa = respone.Cookies.ToList();
        //    return source;
        //}

        private static string GetHtml(string asin, int pageNumber)
        {
            //if (pageNumber == 1)
            //{
            //    var url = string.Format(Url, asin, pageNumber);
            //    driver.Navigate().GoToUrl(url);
            //    var html = driver.PageSource;
            //    return html;
            //}
            //else
            //{
            //    var aaa = driver.PageSource;
            //    string xpathLocate = "//a[contains(text(), 'Next page')]";
            //    var lastPageButton = driver.FindElement(By.XPath(xpathLocate));
            //    Actions actions = new Actions(driver);
            //    actions.MoveToElement(lastPageButton);
            //    actions.Perform();
            //    lastPageButton.Click();
            //    WebUtils.WaitForElement(driver, "histogramTable");
            //    var html = driver.PageSource;
            //    return html;
            //}
            var url = string.Format(Url, asin, pageNumber);
            driver.Navigate().GoToUrl(url);
            var html = driver.PageSource;
            return html;

        }

        private static AmazonRating GetAmazonRating(AmazonItem amazonItem, HtmlDocument document)
        {
            var amazonRating = new AmazonRating();
            amazonRating.ItemID = amazonItem.ItemID;
            amazonRating.ASIN = amazonItem.ASIN;

            // Avg Star
            var averageStarRatingNode = document.DocumentNode.SelectSingleNode("//i[contains(@data-hook, 'average-star-rating')]//span");
            if (averageStarRatingNode != null)
            {
                var strAvgStar = averageStarRatingNode.InnerText.Trim();
                if (!string.IsNullOrEmpty(strAvgStar))
                {
                    float avgStart;
                    float.TryParse(strAvgStar.Replace(" out of 5 stars", ""), out avgStart);
                    amazonRating.TotalRate = avgStart;
                }
            }

            // Number Of Ratings
            var numberOfRatingsNode = document.DocumentNode.SelectSingleNode("//div[contains(@data-hook, 'total-review-count')]//span");
            if (numberOfRatingsNode != null)
            {
                var strNumberOfRatings = numberOfRatingsNode.InnerText.Trim();
                if (!string.IsNullOrEmpty(strNumberOfRatings))
                {
                    int numberOfRatings;
                    int.TryParse(strNumberOfRatings.Replace(" global rating", "").Replace(" global ratings", "").Replace(",", ""), out numberOfRatings);
                    amazonRating.NumberOfRatings = numberOfRatings;
                }
            }

            // Number Of Reviews
            var numberOfReviewsNode = document.DocumentNode.SelectSingleNode("//div[contains(@data-hook, 'cr-filter-info-review-rating-count')]");
            if (numberOfReviewsNode != null)
            {
                var strNumberOfReviews = numberOfReviewsNode.InnerText.Trim();
                if (!string.IsNullOrEmpty(strNumberOfReviews))
                {
                    int numberOfReviews;
                    var strReviews = strNumberOfReviews.Split(",").Last().Trim();
                    int.TryParse(strReviews.Replace(" with review", "").Replace(" with reviews", "").Replace(",", ""), out numberOfReviews);
                    amazonRating.NumberOfReviews = numberOfReviews;
                }
            }

            // Detail Ratings
            var trs = document.DocumentNode.SelectNodes("//table[@id='histogramTable']//tr");
            if (trs != null)
            {
                foreach (var tr in trs)
                {
                    string trHtml = tr.InnerHtml;
                    HtmlDocument trDoc = new HtmlDocument();
                    trDoc.LoadHtml(trHtml);
                    var cols = trDoc.DocumentNode.SelectNodes("//td");
                    if (cols != null && cols.Count >= 3)
                    {
                        var name = cols[0].InnerText.Trim().ToLower().Substring(0, 6);
                        var strRating = cols[2].InnerText.Trim();
                        if (!string.IsNullOrEmpty(strRating))
                        {
                            float rating;
                            float.TryParse(strRating.Replace("%", ""), out rating);

                            switch (name)
                            {
                                case "5 star":
                                    amazonRating._5Star = rating;
                                    break;
                                case "4 star":
                                    amazonRating._4Star = rating;
                                    break;
                                case "3 star":
                                    amazonRating._3Star = rating;
                                    break;
                                case "2 star":
                                    amazonRating._2Star = rating;
                                    break;
                                case "1 star":
                                    amazonRating._1Star = rating;
                                    break;
                            }
                        }
                    }
                }
            }

            return amazonRating;
        }

        private static List<AmazonReview> GetAmazonReviews(AmazonItem amazonItem, HtmlDocument document)
        {
            var reviewNodes = document.DocumentNode.SelectNodes("//div[@id='cm_cr-review_list']//div[contains(@data-hook, 'review')]");

            if (reviewNodes != null && reviewNodes.Count > 0)
            {
                var amazonReviews = new List<AmazonReview>();
                foreach (var reviewNode in reviewNodes)
                {
                    string reviewHtml = reviewNode.InnerHtml;
                    HtmlDocument reviewDoc = new HtmlDocument();
                    reviewDoc.LoadHtml(reviewHtml);

                    var amazonReview = new AmazonReview();
                    amazonReview.ItemID = amazonItem.ItemID;
                    amazonReview.ASIN = amazonItem.ASIN;

                    amazonReview.ReviewID = reviewNode.Id;

                    // Reviewer
                    var reviewerNode = reviewDoc.DocumentNode.SelectSingleNode("//div[contains(@data-hook, 'genome-widget')]//div[contains(@class, 'a-profile-content')]//span[contains(@class, 'a-profile-name')]");
                    if (reviewerNode != null)
                    {
                        amazonReview.Reviewer = reviewerNode.InnerText.Trim();
                    }

                    // Rate
                    var ratingNode = reviewDoc.DocumentNode.SelectSingleNode("//i[contains(@data-hook, 'review-star-rating')]//span");
                    if (ratingNode != null)
                    {
                        var strRate = ratingNode.InnerText.Trim();
                        if (!string.IsNullOrEmpty(strRate))
                        {
                            float rate;
                            var re = float.TryParse(strRate.Replace(" out of 5 stars", ""), out rate);
                            if (re)
                            {
                                amazonReview.Rate = rate;
                            }
                        }
                    }

                    // Review Title
                    var titleNodes = reviewDoc.DocumentNode.SelectNodes("//a[contains(@data-hook, 'review-title')]//span");
                    if (titleNodes != null && titleNodes.Count > 0)
                    {
                        var titleNode = titleNodes.Last();
                        amazonReview.ReviewTitle = titleNode.InnerText.Trim();
                    }

                    // Review Date
                    var dateNode = reviewDoc.DocumentNode.SelectSingleNode("//span[contains(@data-hook, 'review-date')]");
                    if (dateNode != null)
                    {
                        var strDate = dateNode.InnerText.Trim();
                        var lastChar = strDate.LastIndexOf(" on ");
                        if (lastChar > 0)
                        {
                            amazonReview.ReviewDate = strDate.Substring(lastChar + 4);
                        }
                    }

                    // Review Content
                    var contentNode = reviewDoc.DocumentNode.SelectSingleNode("//span[contains(@data-hook, 'review-body')]//span");
                    if (contentNode != null)
                    {
                        amazonReview.ReviewContent = contentNode.InnerText.Trim();
                    }

                    amazonReviews.Add(amazonReview);
                }
                return amazonReviews;
            }
            else
            {
                return null;
            }
        }
    }
}
