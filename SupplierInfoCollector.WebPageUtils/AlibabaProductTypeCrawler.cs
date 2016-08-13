using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using SupplierInfoCollector.Domain;

namespace SupplierInfoCollector.WebPageUtils
{
    public class AlibabaProductTypeCrawler
    {
        private const string ALIBABA_HOMEPAGE = "http://www.1688.com/";

        public List<AlibabaProductType> GetAlibabaProductTypes()
        {
            List<AlibabaProductType> result = new List<AlibabaProductType>();

            string isDebug = ConfigurationManager.AppSettings["isDebug"];

            if ("true" == isDebug)
            {
                string pageHtml = File.ReadAllText(".\\TestPage\\AlibabaHomepage.html");

                result = this.GetProductTypesInPageHtml(pageHtml);
            }
            else
            {
                string pageHtml = WebPageDownloader.GetPageHtml(ALIBABA_HOMEPAGE, Encoding.GetEncoding("GBK"));

                result = this.GetProductTypesInPageHtml(pageHtml);
            }

            return result;
        }


        private List<AlibabaProductType> GetProductTypesInPageHtml(string html)
        {
            List<AlibabaProductType> result = new List<AlibabaProductType>();

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);

            HtmlNode jiajuNode = doc.DocumentNode.SelectSingleNode(".//div[@id='jiaju']");
            string title1 = jiajuNode.Attributes["data-short-title"].Value.Trim();

            var catCellCollection = jiajuNode.SelectNodes(".//div[@class='cat-cell']");

            foreach (var catCell in catCellCollection)
            {
                string title2 = catCell.SelectSingleNode(".//h3[@class='cat-title']/a").InnerText.Trim();

                var typeCollection = catCell.SelectNodes(".//ul/li/a");
                foreach (var type in typeCollection)
                {
                    AlibabaProductType productType = new AlibabaProductType();
                    productType.Name = type.InnerText.Trim();
                    productType.CategoryPath = string.Format("/{0}/{1}/{2}", title1, title2, productType.Name);
                    productType.ListPageURL = type.Attributes["href"].Value;

                    result.Add(productType);
                }
            }

            return result;
        }
    }
}
