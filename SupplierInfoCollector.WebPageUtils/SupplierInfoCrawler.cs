using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Nana.Framework.Utility;
using SupplierInfoCollector.Domain;
using HtmlAgilityPack;
using System.Web;

namespace SupplierInfoCollector.WebPageUtils
{
    public class SupplierInfoCrawler
    {
        private readonly Regex _supplierCountRegex = new Regex(
            "(?<=<strong>Supplier List <span class=\"countNum\">\\().*?(?=\\)</span></strong>)");

        public List<SupplierInfo> GetSupplierInfoList(List<string> listPageURLTemplateList)
        {
            List<SupplierInfo> result = new List<SupplierInfo>();

            string isDebug = ConfigurationManager.AppSettings["isDebug"];

            if ("true" == isDebug)
            {
                string pageHtml1 = File.ReadAllText(".\\TestPage\\pageHtml.txt");
                List<SupplierInfo> list = GetSupplierInfoListInPage(pageHtml1);
                result.AddRange(list);
            }
            else
            {
                foreach (string urlTemplate in listPageURLTemplateList)
                {
                    int startIndex = 0;
                    int supplierCount = 0;
                    var tempSupplierInfoList = new List<SupplierInfo>();

                    do
                    {
                        string page1URL = null;

                        try
                        {
                            page1URL = string.Format(urlTemplate, startIndex);
                            string pageHtml1 = WebPageDownloader.GetPageHtml(page1URL);

                            if (supplierCount == 0)
                            {
                                // 获得供应商数量
                                supplierCount = GetSupplierCount(pageHtml1);
                            }

                            List<SupplierInfo> list = GetSupplierInfoListInPage(pageHtml1);
                            tempSupplierInfoList.AddRange(list);

                            startIndex += 20;

                            Console.WriteLine(string.Format("StartIndex={0}, 已抓取{1}/{2}个供应商概要信息",
                                startIndex, tempSupplierInfoList.Count, supplierCount));
                        }
                        catch (Exception ex)
                        {
                            string message = string.Format(
                                "抓取供应商概要信息失败\r\nurl:{0}\r\nmessage:{1}\r\nstacktrace:{2}",
                                page1URL, ex.Message, ex.StackTrace);
                            Console.WriteLine(message);
                            TxtLogHelper.Instance.Log(message, EnumConfigLogLevel.Error);
                        }
                    } while (startIndex < supplierCount);

                    result.AddRange(tempSupplierInfoList);
                }
            }

            return result;
        }

        public SupplierInfo GetSupplierDetails(SupplierInfo supplierInfo)
        {
            string contactUsPageHtml = null;

            string isDebug = ConfigurationManager.AppSettings["isDebug"];

            if ("true" == isDebug)
            {
                contactUsPageHtml = File.ReadAllText(".\\TestPage\\contactUsPageHtml.txt");

                //string contactUsPageURL = "http://ruiher-hw.manufacturer.globalsources.com/si/6008836364680/ContactUs.htm";
                //contactUsPageHtml = WebPageDownloader.GetPageHtml(contactUsPageURL);
            }
            else
            {
                string contactUsPageURL =
                    supplierInfo.GlobalSourcesHomePageURL.ToLower().Replace("homepage.htm", "ContactUs.htm");

                contactUsPageHtml = WebPageDownloader.GetPageHtml(contactUsPageURL);
            }

            var pageInfo = GetContactUsPageInfo(contactUsPageHtml);

            if (pageInfo.ContainsKey("Other Homepage Address"))
            {
                supplierInfo.CompageHomePageURL = pageInfo["Other Homepage Address"];
            }

            if (pageInfo.ContainsKey("KeyContactPersonName"))
            {
                supplierInfo.Contactor = pageInfo["KeyContactPersonName"];
            }

            if (pageInfo.ContainsKey("KeyContactPersonTitle"))
            {
                supplierInfo.ContactorTitle = pageInfo["KeyContactPersonTitle"];
            }

            if (pageInfo.ContainsKey("KeyContactPersonEmailImgURL"))
            {
                supplierInfo.ContactorEmailImgURL = pageInfo["KeyContactPersonEmailImgURL"];
            }

            if (pageInfo.ContainsKey("Address"))
            {
                supplierInfo.Address = pageInfo["Address"];
            }

            if (pageInfo.ContainsKey("City"))
            {
                supplierInfo.City = pageInfo["City"];
            }

            if (pageInfo.ContainsKey("Country"))
            {
                supplierInfo.Country = pageInfo["Country"];
            }

            if (pageInfo.ContainsKey("State/Province"))
            {
                supplierInfo.StateOrProvince = pageInfo["State/Province"];
            }

            if (pageInfo.ContainsKey("Zip/Postal Code"))
            {
                supplierInfo.PostCode = pageInfo["Zip/Postal Code"];
            }

            if (pageInfo.ContainsKey("Phone Number"))
            {
                supplierInfo.PhoneNumber = pageInfo["Phone Number"];
            }

            if (pageInfo.ContainsKey("Fax Number"))
            {
                supplierInfo.FaxNumber = pageInfo["Fax Number"];
            }

            if (pageInfo.ContainsKey("Mobile"))
            {
                supplierInfo.MobilePhone = pageInfo["Mobile"];
            }

            if (pageInfo.ContainsKey("Registered Company"))
            {
                supplierInfo.CompanyNameZh = pageInfo["Registered Company"];
            }

            if (pageInfo.ContainsKey("Company Registration Address"))
            {
                supplierInfo.AddressZh = pageInfo["Company Registration Address"];
            }

            return supplierInfo;
        }

        private int GetSupplierCount(string pageHtml)
        {
            Match m1 = _supplierCountRegex.Match(pageHtml);
            return int.Parse(m1.Value.Trim());
        }

        private List<SupplierInfo> GetSupplierInfoListInPage(string pageHtml)
        {
            List<SupplierInfo> result = new List<SupplierInfo>();

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(pageHtml);

            try
            {
                var typeNode = doc.DocumentNode.SelectSingleNode(".//p[@class='path mt5']/strong[1]");
                string supplierType = (typeNode == null) ? null : typeNode.InnerText.Trim();

                HtmlNodeCollection supplierNodes = doc.DocumentNode.SelectNodes("//div[@class='kwRelateLink']/div[@id]");

                if (supplierNodes != null && supplierNodes.Count > 0)
                {
                    foreach (var supplierNode in supplierNodes)
                    {
                        string id = supplierNode.Attributes["id"].Value.Replace("HL", "").Replace("A", "");

                        HtmlNode node1 = supplierNode.SelectSingleNode(".//a[@class='supplierTit']");
                        string name = node1.InnerText;
                        string homepageUrl = node1.Attributes["href"].Value.Trim();
                        homepageUrl = homepageUrl.Substring(0, homepageUrl.IndexOf("?"));

                        HtmlNode node2 = supplierNode.SelectSingleNode(".//div[@class='supplierName']/a[1]");
                        string ranking = node2.InnerText;

                        HtmlNode node3 = supplierNode.SelectSingleNode(".//span[@class='memberSince']/img[1]");
                        string num = node3.Attributes["class"].Value.Replace("num", "").Trim();
                        int yearsSince = int.Parse(num);

                        HtmlNode node4 =
                            supplierNode.SelectSingleNode(".//p[@class='supplierProduct_num']/a[@class='fullCatalog']");
                        string fullCatalogUrl = (node4 == null) ? null : node4.Attributes["href"].Value;

                        HtmlNode node5 =
                            supplierNode.SelectSingleNode(
                                ".//p[@class='supplierProduct_num']/a[@class='fullCatalog']/strong");
                        int productCount = 0;
                        if (node5 != null)
                        {
                            productCount = int.Parse(node5.InnerText.Trim());
                        }

                        SupplierInfo sInfo = new SupplierInfo()
                        {
                            GlobalSourcesId = id,
                            Name = name,
                            Ranking = ranking,
                            YearsSince = yearsSince,
                            ProductCount = productCount,
                            GlobalSourcesHomePageURL = homepageUrl,
                            FullCatalogPageURL = fullCatalogUrl,
                            SupplierType = supplierType
                        };

                        result.Add(sInfo);
                    }
                }
            }
            catch (Exception ex)
            {
                string message = string.Format(
                                "抓取供应商概要信息失败\r\nmessage:{0}\r\nstacktrace:{1}", ex.Message, ex.StackTrace);
                Console.WriteLine(message);
                TxtLogHelper.Instance.Log(message, EnumConfigLogLevel.Error);
            }

            return result;
        }

        private Dictionary<string, string> GetContactUsPageInfo(string pageHtml)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(pageHtml);

            var mainContentNodes = doc.DocumentNode.SelectNodes(".//div[@class='contactUs']/div[@class='mt10']/div[@class='proDetCont mt10']//div[@class='clearfix mt5']");

            if (mainContentNodes != null && mainContentNodes.Count > 0)
            {
                foreach (var node in mainContentNodes)
                {
                    var fieldNameNode = node.SelectSingleNode(".//p[@class='fl c6 proDetTit']");
                    var fieldValueNode = node.SelectSingleNode(".//p[@class='fl ml5']");

                    if (fieldNameNode == null || fieldValueNode == null)
                    {
                        continue;
                    }

                    string feildName = fieldNameNode.InnerText.Replace(":", "").Trim();
                    string fieldValue = HtmlAnalyzer.CleanText(fieldValueNode.InnerText.Trim());

                    result[feildName] = fieldValue;
                }
            }

            var nodeList1 = doc.DocumentNode.SelectNodes(".//div[@class='contactUs']/div[@class='mt20']");

            if (nodeList1 != null)
            {
                foreach (var nodeMT20 in nodeList1)
                {
                    var titleNode = nodeMT20.SelectSingleNode(".//h2[@class='proCatTitle']");

                    if (titleNode != null)
                    {
                        var title = titleNode.InnerText.Trim();

                        if (title.Contains("Key Contact Person"))
                        {
                            var childNodes = nodeMT20.ChildNodes;

                            foreach (var node in childNodes)
                            {
                                if (node.Name == "p" && node.Attributes["class"].Value.Contains("contName"))
                                {
                                    result["KeyContactPersonName"] = HtmlAnalyzer.CleanText(node.InnerText.Trim());
                                }

                                if (node.Name == "p" && node.Attributes["class"].Value == "ml10")
                                {
                                    result["KeyContactPersonTitle"] = HtmlAnalyzer.CleanText(node.InnerText.Trim());
                                }

                                if (node.Name == "p" && node.Attributes["class"].Value.Contains("tsEmail"))
                                {
                                    result["KeyContactPersonEmailImgURL"] = node.ChildNodes["img"].Attributes["src"].Value;
                                }
                            }
                        }
                        else if (title.Contains("Verified Business Registration Details"))
                        {
                            var fieldNodes = nodeMT20.SelectNodes(".//div/p");

                            foreach (var fieldNode in fieldNodes)
                            {
                                var fieldTitleNode = fieldNode.SelectSingleNode(".//em");
                                if (fieldTitleNode != null)
                                {
                                    var fieldTitle = HtmlAnalyzer.CleanText(fieldTitleNode.InnerText.Replace(":", ""));

                                    if (fieldTitle.Contains("Registered Company"))
                                    {
                                        HtmlNode textNode = fieldTitleNode.NextSibling;
                                        result["Registered Company"] =
                                            HtmlAnalyzer.CleanText(textNode.InnerText).Replace("\"", "");
                                    }
                                    else if (fieldTitle.Contains("Company Registration Address"))
                                    {
                                        HtmlNode textNode = fieldTitleNode.NextSibling;
                                        result["Company Registration Address"] =
                                            HtmlAnalyzer.CleanText(textNode.InnerText).Replace("\"", "");
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return result;
        }

        
    }
}
