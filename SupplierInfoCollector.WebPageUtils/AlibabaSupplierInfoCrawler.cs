using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using HtmlAgilityPack;
using Nana.Framework.Utility;
using SupplierInfoCollector.Domain;

namespace SupplierInfoCollector.WebPageUtils
{
    public class AlibabaSupplierInfoCrawler
    {
        private readonly Regex Grade_Regex = new Regex("(?<=sale_).*?(?=\\.gif)");
        private CookieContainer _cookieContainer = null;
        
        public List<AlibabaSupplierInfo> GetAlibabaSupplierInfoList(AlibabaProductType productType)
        {
            List<AlibabaSupplierInfo> result = new List<AlibabaSupplierInfo>();

            string isDebug = ConfigurationManager.AppSettings["isDebug"];

            if ("true" == isDebug)
            {
                string pageHtml1 = File.ReadAllText(".\\TestPage\\AlibabaProductListPage.html");
                int pageCount = GetPageCount(pageHtml1);
                List<AlibabaSupplierInfo> list = GetSupplierInfoListInPage(pageHtml1);
                result.AddRange(list);
            }
            else
            {
                int pageIndex = 1;
                int pageCount = 0;

                try
                {
                    do
                    {
                        string page1URL = string.Format("{0}&uniqfield=userid&pageSize=20&beginPage={1}", productType.ListPageURL, pageIndex);
                        string pageHtml1 = WebPageDownloader.GetPageHtml(page1URL, Encoding.GetEncoding("GBK"));

                        if (pageCount == 0)
                        {
                            pageCount = GetPageCount(pageHtml1);
                        }

                        List<AlibabaSupplierInfo> list = GetSupplierInfoListInPage(pageHtml1);
                        foreach (var item in list)
                        {
                            item.AddProductType(productType);       //关联产品类型
                        }
                        result.AddRange(list);

                        pageIndex++;

                        Console.WriteLine("已抓取" + result.Count + "个" + productType.Name + "供应商概要信息");

                    } while (pageIndex <= pageCount);
                }
                catch (Exception ex)
                {
                    string message = string.Format("抓取供Alibaba供应商概要信息失败\r\nmessage:{0}\r\nstacktrace:{1}", ex.Message,
                        ex.StackTrace);
                    Console.WriteLine(message);
                    TxtLogHelper.Instance.Log(message, EnumConfigLogLevel.Error);
                }
            }

            return result;
        }

        public AlibabaSupplierInfo GetSupplierDetailsFromWeb(AlibabaSupplierInfo supplierInfo)
        {
            try
            {
                string isDebug = ConfigurationManager.AppSettings["isDebug"];

                if ("true" == isDebug)
                {
                    //string pageHtml1 = File.ReadAllText(".\\TestPage\\AlibabaProductListPage.txt");
                    string pageHtml1 = WebPageDownloader.GetPageHtml("http://shop1387558760455.1688.com/page/contactinfo.htm", Encoding.GetEncoding("GBK"));

                    SetDetailInfo(supplierInfo, pageHtml1);
                }
                else
                {
                    if (_cookieContainer == null)
                    {
                        _cookieContainer = WebPageDownloader.CreateCookieContainer(
@"ali_apache_track=""c_ms=1|c_mid=b2b-57046111|c_lid=%E8%94%A1%E5%9F%8E%E6%9D%A8""; expires=Tue, 09 May 2017 14:52:30 GMT; path=/; domain=.1688.com
ali_beacon_id=223.21.135.25.13943415505.354934.2; expires=Fri, 20 Mar 2026 12:00:03 GMT; path=/; domain=.1688.com
cna=YuSiC4sHcFQCAd8VhxlVbzyo; expires=Thu, 31 Dec 2037 16:00:03 GMT; path=/; domain=.1688.com
JSESSIONID=ZwlFpg1-EPDRoafC9fFGbZwrUC-IAkC1YO-frLM; path=/; domain=.1688.com
__cn_logon__=true; path=/; domain=.1688.com
__cn_logon_id__=%E8%94%A1%E5%9F%8E%E6%9D%A8; path=/; domain=.1688.com
ali_apache_tracktmp=""c_w_signed=Y""; path=/; domain=.1688.com
userID=""DepCf%2BTD4o2PKDm1G8jeKZ%2Bc5PNLdoPtK3Mn4Nb7pyU6sOlEpJKl9g%3D%3D""; path=/; domain=.1688.com
LoginUmid=""iCsysNWhyw0tRpBbZIcfy8BuPKWyZUzkJpQ1HrWNM1bmkdAiNY8Cqw%3D%3D""; path=/; domain=.1688.com
cn_tmp=""Z28mC+GqtZ1HPf7Xr0cQoKFETgo7w9c25Oy/h3q2w9nlOh3rxpmtSRkcLoUKUsOzfl8i4jX0E8YGMzQYSpGrpZxQ3E9FbAodxTDoxqVRVgTTSBpYK1zMaOzGgLIQNUM3y5f6/qgto2MNqnVspHaH9BbjPGmHIMSA02j4BKm+5IBRSJewbjf8NbAhyLxK5hPyxPLfVRolfcDElLqON1O7AnLly3tyiW86cFTyZbafglQ=""; path=/; domain=.1688.com; HttpOnly
last_mid=b2b-57046111; expires=Mon, 09 Mar 2015 13:33:18 GMT; path=/; domain=.1688.com
_cn_slid_=""usdgReOV%2BH""; expires=Mon, 09 Mar 2015 13:33:18 GMT; path=/; domain=.1688.com
__last_loginid__=""%E8%94%A1%E5%9F%8E%E6%9D%A8""; expires=Mon, 09 Mar 2015 13:33:18 GMT; path=/; domain=.1688.com
login=""kFeyVBJLQQI%3D""; path=/; domain=.1688.com
_csrf_token=1394371995013; path=/; domain=.1688.com
ali_ab=223.21.135.25.1394342961745.3; expires=Wed, 06 Mar 2024 13:33:19 GMT; path=/; domain=.1688.com
userIDNum=""C%2FnKuuDqZ3I6sOlEpJKl9g%3D%3D""; path=/; domain=.1688.com
_nk_=""uu2VEv9Ylos%3D""; path=/; domain=.1688.com
__rn_refer_login_id__=%E8%94%A1%E5%9F%8E%E6%9D%A8; expires=Sun, 09 Mar 2014 15:33:19 GMT; path=/; domain=.1688.com
__rn_alert__=false; expires=Sun, 09 Mar 2014 15:33:19 GMT; path=/; domain=.1688.com
_tmp_ck_0=""oP%2BYOcZGEy5G3VtO6jI4VmXjvgIzb3rD1cpbBoNgB0vS3zERjhRDdN%2Fpvuf1tIMirbpASCDzzb7k%2FDOBX0XSpW4Dk98U58jti79g9cG5Xv%2FoQGhmap6HKh5Z9garz%2BkaJpow7UZXqpl%2B6uoidRCUo7OTm%2B8rEpha13tvyhFwY3LqN0gZ%2FyATixrz8CXsdS%2FbC40JXetEQn6AOsucdyFaaFzDXrZCFWlapu9iPdyAy%2FtxZOEKP0u%2B9eZzRvdIv7bWsNpGeNO%2BFbn5jJRkzzJThBUZmbC3QJC%2B%2F8tmZSr0Ksi%2Bvd%2BDDdet%2BHaSmBkgiGhm3ZPrSd%2BE5LMrtF2hTPqFc5npFjXL2L0P6mv4UYTtoQxEMCi00YOF4LqIJPF%2FYydeHzrogMfbH3qO%2FY8TK5aCuaAK4sE1zBwzuNqapvIO49TK%2F6sdjA2JX%2FR9EvulgIWCouPdaRZDEwEOKZ03UzX2iQIATyPzfll5rjKA0mES93R%2FyL0HaGjmow%3D%3D""; path=/; domain=.1688.com
alicnweb=lastlogonid%3D%25E8%2594%25A1%25E5%259F%258E%25E6%259D%25A8%7Ctouch_tb_at%3D1394371988477; expires=Fri, 31 Dec 2049 16:00:00 GMT; path=/; domain=.1688.com");
                    }

                    string pageHtml1 = WebPageDownloader.GetPageHtml(
                        supplierInfo.AlibabaHomeURL + "/page/contactinfo.htm", 
                        Encoding.GetEncoding("GBK"),
                        _cookieContainer);
                    SetDetailInfo(supplierInfo, pageHtml1);
                }

                Console.WriteLine(string.Format("已抓取{0}({1})供应商详细信息", supplierInfo.CompanyName, supplierInfo.Id));
            }
            catch (Exception ex)
            {
                string message = string.Format("抓取供Alibaba供应商{2}({3})详细信息失败\r\nmessage:{0}\r\nstacktrace:{1}", 
                    ex.Message, ex.StackTrace, supplierInfo.CompanyName, supplierInfo.Id);
                Console.WriteLine(message);
                TxtLogHelper.Instance.Log(message, EnumConfigLogLevel.Error);
            }

            return supplierInfo;
        }

        private int GetPageCount(string pageHtml)
        {
            int result = 0;

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(pageHtml);

            var node = doc.DocumentNode.SelectSingleNode(".//span[@class='total-page']");
            var nodeTxt = node.InnerText.Replace("共", "").Replace("页", "").Trim();
            int.TryParse(nodeTxt, out result);

            return result;
        }

        private List<AlibabaSupplierInfo> GetSupplierInfoListInPage(string pageHtml)
        {
            List<AlibabaSupplierInfo> result = new List<AlibabaSupplierInfo>();

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(pageHtml);

            var supplierNodeList = doc.DocumentNode.SelectNodes(".//ul[@id='sw_maindata_asyncload']/li[@class='sm-offerShopwindow']");

            foreach (var supplierNode in supplierNodeList)
            {
                AlibabaSupplierInfo supplierInfo = new AlibabaSupplierInfo();

                var companyLinkNode = supplierNode.SelectSingleNode(".//div[@class='sm-offerShopwindow-company fd-clr']/a[@memberid]");
                supplierInfo.MemberID = companyLinkNode.Attributes["memberid"].Value.Trim();
                supplierInfo.CompanyName = HtmlAnalyzer.CleanText(companyLinkNode.InnerText);
                supplierInfo.AlibabaHomeURL = companyLinkNode.Attributes["href"].Value.Trim();

                var yearNode = supplierNode.SelectSingleNode(".//div[@class='sm-offerShopwindow-summary fd-clr']/a[1]/em");
                int yearsInAlibaba = 0;
                if (yearNode != null && int.TryParse(yearNode.InnerText.Trim(), out yearsInAlibaba))
                {
                    supplierInfo.YearsOnAlibaba = yearsInAlibaba;
                }

                result.Add(supplierInfo);
            }

            return result;
        }

        private void SetDetailInfo(AlibabaSupplierInfo supplierInfo, string pageHtml)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(pageHtml);

            var nameNode = doc.DocumentNode.SelectSingleNode(".//div[@class='m-content']/dl/dd/a");
            if (nameNode == null)
            {
                return;
            }
            supplierInfo.Contractor = HtmlAnalyzer.CleanText(nameNode.InnerText);

            var gradeNode = doc.DocumentNode.SelectSingleNode(".//dl[@class='supply-grade']/dd/a/img[@alt='供应等级']");
            if (gradeNode != null)
            {
                string imageUrl = gradeNode.Attributes["src"].Value;
                Match m1 = Grade_Regex.Match(imageUrl);
                supplierInfo.SupplierRank = m1.Value;
            }
            
            var ratingNode = doc.DocumentNode.SelectSingleNode(".//div[@class='certify']/div/dl[@class='medal']/dd/a/img");
            if (ratingNode != null)
            {
                supplierInfo.BusinessRating = ratingNode.Attributes["alt"].Value;
            }

            var fieldNodes = doc.DocumentNode.SelectNodes(".//div[@class='fd-clr']/div[@class='fd-left']/dl");

            foreach (var fieldNode in fieldNodes)
            {
                var dt = fieldNode.SelectSingleNode(".//dt");
                var dd = fieldNode.SelectSingleNode(".//dd");

                string fieldName = HtmlAnalyzer.CleanText(dt.InnerText);
                string fieldValue = HtmlAnalyzer.CleanText(dd.InnerText);

                if (fieldName.Contains("移动电话"))
                {
                    supplierInfo.MobilePhone = fieldValue;
                    continue;
                }
                if (fieldName.Contains("电话"))
                {
                    supplierInfo.CompanyPhone = fieldValue;
                    continue;
                }
                if (fieldName.Contains("传真"))
                {
                    supplierInfo.CompanyFax = fieldValue;
                    continue;
                }
                if (fieldName.Contains("地址"))
                {
                    supplierInfo.CompanyAddress = fieldValue;
                    continue;
                }
                if (fieldName.Contains("公司主页"))
                {
                    var node = dd.SelectSingleNode(".//div/a[@class='outsite']");
                    if (node != null)
                    {
                        supplierInfo.WebsiteURL = node.Attributes["href"].Value;
                    }
                    continue;
                }
            }

        }
    }
}
