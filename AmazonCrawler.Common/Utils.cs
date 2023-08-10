using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace AmazonCrawler.Common
{
    public class Utils
    {
        public static T DeserializeXML<T>(string Xml)
        {

            XmlSerializer ser;
            ser = new XmlSerializer(typeof(T));
            StringReader stringReader;
            stringReader = new StringReader(Xml);
            XmlTextReader xmlReader;
            xmlReader = new XmlTextReader(stringReader);
            T obj;
            obj = (T)ser.Deserialize(xmlReader);
            xmlReader.Close();
            stringReader.Close();
            return obj;
        }

        //Serializes the <i>Obj</i> to an XML string.
        public static string SerializeXML<T>(object Obj)
        {
            try
            {
                XmlSerializer ser;
                ser = new XmlSerializer(typeof(T));
                MemoryStream memStream;
                memStream = new MemoryStream();
                XmlTextWriter xmlWriter;
                xmlWriter = new XmlTextWriter(memStream, Encoding.UTF8);
                xmlWriter.Namespaces = true;
                ser.Serialize(xmlWriter, Obj);
                xmlWriter.Close();
                memStream.Close();
                string xml;
                xml = Encoding.UTF8.GetString(memStream.GetBuffer());
                xml = xml.Substring(xml.IndexOf(Convert.ToChar(60)));
                xml = xml.Substring(0, (xml.LastIndexOf(Convert.ToChar(62)) + 1));
                return xml.Replace("<?xml version=\"1.0\" encoding=\"utf-8\"?>", "")
                    .Replace("xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"", "")
                    .Replace("xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"", "").Replace("xsi:nil=\"true\"", "");
            }
            catch (Exception ex)
            {
                Utils.WriteToLogs(ex.ToString());
                return "";
            }
        }

        public static void WriteToLogs(object Erros)
        {
            string absolutePath = AppSettings.GetString("Log");
            string fileName = DateTime.Now.Day.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Year.ToString() + ".txt";
            if (!Directory.Exists(absolutePath))
            {
                try
                {
                    Directory.CreateDirectory(absolutePath);
                }
                catch (Exception ex)
                {

                }
            }
            string pathString = absolutePath = Path.Combine(absolutePath, fileName);
            if (!File.Exists(pathString))
            {
                using (FileStream fs = File.Create(pathString))
                {
                    fs.Close();
                    using (StreamWriter sw = new StreamWriter(pathString))
                    {
                        sw.WriteLine("[_" + DateTime.Now + "_]");
                        sw.WriteLine(Erros);
                    }
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(pathString))
                {
                    sw.WriteLine("========================================================================");
                    sw.WriteLine("[_" + DateTime.Now + "_]");
                    sw.WriteLine(Erros);
                }
                return;
            }
        }
    }
}
