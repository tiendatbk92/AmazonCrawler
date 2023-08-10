using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonCrawler.Common
{
    public class DbConnectionString
    {
        #region Constants Database
        private const string DataR1Inventory = "st_Inventory";
        private const string DataR1Identity = "st_Identity";
        private const string DataR1Payment = "st_Payment";
        private const string DataR1Email = "st_Email";
        private const string DataR1Stemp = "st_Temp";
        private const string DataR1Print = "st_Print";
        private const string DataR1ECommerce = "st_ECommerce";
        private const string DataR1Report = "st_Report";
        private const string DataR1DataFeed = "st_DataFeed";
        private const string DataWarehouse = "DataWarehouse";
        #endregion
        #region ConnectionNameDb
        public enum ConnectionName
        {
            st_Inventory = 1,
            st_Identity = 2,
            st_Payment = 3,
            st_Email = 4,
            st_Temp = 5,
            st_Print = 6,
            st_ECommerce = 7,
            st_Report = 8,
            st_DataFeed = 9,
            DataWarehouse = 10
        }
        #endregion
        public static string GetConnection(ConnectionName connectionName)
        {
            var connectionString = string.Empty;

            switch (connectionName)
            {
                case ConnectionName.st_Inventory:
                    try
                    {
                        connectionString = AppSettings.GetConnection(DataR1Inventory);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(string.Format("Generate connection string fail:{0}", ex.Message));
                    }
                    break;
                case ConnectionName.st_Identity:
                    try
                    {
                        connectionString = AppSettings.GetConnection(DataR1Identity);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(string.Format("Generate connection string fail:{0}", ex.Message));
                    }
                    break;
                case ConnectionName.st_Payment:
                    try
                    {
                        connectionString = AppSettings.GetConnection(DataR1Payment);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(string.Format("Generate connection string fail:{0}", ex.Message));
                    }
                    break;
                case ConnectionName.st_Email:
                    try
                    {
                        connectionString = AppSettings.GetConnection(DataR1Email);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(string.Format("Generate connection string fail:{0}", ex.Message));
                    }
                    break;
                case ConnectionName.st_Temp:
                    try
                    {
                        connectionString = AppSettings.GetConnection(DataR1Stemp);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(string.Format("Generate connection string fail:{0}", ex.Message));
                    }
                    break;
                case ConnectionName.st_Print:
                    try
                    {
                        connectionString = AppSettings.GetConnection(DataR1Print);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(string.Format("Generate connection string fail:{0}", ex.Message));
                    }
                    break;
                case ConnectionName.st_ECommerce:
                    try
                    {
                        connectionString = AppSettings.GetConnection(DataR1ECommerce);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(string.Format("Generate connection string fail:{0}", ex.Message));
                    }
                    break;
                case ConnectionName.st_Report:
                    try
                    {
                        connectionString = AppSettings.GetConnection(DataR1Report);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(string.Format("Generate connection string fail:{0}", ex.Message));
                    }
                    break;
                case ConnectionName.st_DataFeed:
                    try
                    {
                        connectionString = AppSettings.GetConnection(DataR1DataFeed);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(string.Format("Generate connection string fail:{0}", ex.Message));
                    }
                    break;
                case ConnectionName.DataWarehouse:
                    try
                    {
                        connectionString = AppSettings.GetConnection(DataWarehouse);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(string.Format("Generate connection string fail:{0}", ex.Message));
                    }
                    break;
            }
            return connectionString;
        }

    }
}
