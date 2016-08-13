using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace SupplierInfoCollector.WebPageUtils
{
    public class WebPageDownloader
    {
        public static string GetPageHtml(string url)
        {
            return GetPageHtml(url, Encoding.UTF8);
        }

        public static string GetPageHtml(string url, Encoding encoding)
        {
            return GetPageHtml(url, encoding, null);
        }

        public static string GetPageHtml(string url, Encoding encoding, CookieContainer cookieContainer)
        {
            var request = CreateWebRequest(url);

            if (cookieContainer != null)
            {
                request.CookieContainer = cookieContainer;
            }

            HttpWebResponse response = null;
            Stream st = null;
            StreamReader sr = null;
            string pageHtml = null;

            try
            {
                response = (HttpWebResponse)request.GetResponse();

                if (cookieContainer != null)
                {
                    response.Cookies = cookieContainer.GetCookies(request.RequestUri);
                }

                st = new GZipStream(response.GetResponseStream(), CompressionMode.Decompress);
                sr = new StreamReader(st, encoding);
                pageHtml = sr.ReadToEnd();
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }

                if (st != null)
                {
                    st.Close();
                }

                if (sr != null)
                {
                    sr.Close();
                }
            }

            return pageHtml;
        }

        public static HttpWebRequest CreateWebRequest(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);

            // Set Head
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:16.0) Gecko/20100101 Firefox/16.0";
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            request.Headers.Add("Accept-Encoding", "gzip, deflate");
            request.Headers.Add("Accept-Language", "zh-cn,zh;q=0.8,en-us;q=0.5,en;q=0.3");
            request.KeepAlive = true;

            return request;
        }

        /// <summary>
        /// 此方法目前有问题
        /// </summary>
        /// <param name="loginURL"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static CookieContainer SimulateLogin(string loginURL, Dictionary<string, string> parameters)
        {
            CookieContainer cookieContainer = new CookieContainer();

            HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(loginURL);
            
            myHttpWebRequest.ContentType = "application/x-www-form-urlencoded";
            myHttpWebRequest.Method = "POST";
            myHttpWebRequest.CookieContainer = cookieContainer;
            string postContent = "";
            if (parameters.Count > 0)
            {
                foreach (var item in parameters)
                {
                    postContent += HttpUtility.UrlEncode(item.Key) + "=" + HttpUtility.UrlEncode(item.Value) + "&";
                }
                postContent = postContent.Substring(0, postContent.Length - 1);
            }

            var postContentData = Encoding.UTF8.GetBytes(postContent);

            myHttpWebRequest.ContentLength = postContentData.Length;

            //写入request content
            Stream myRequestStream = myHttpWebRequest.GetRequestStream();
            myRequestStream.Write(postContentData, 0, postContentData.Length);
            myRequestStream.Close();


            HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();
            myHttpWebResponse.Cookies = cookieContainer.GetCookies(myHttpWebRequest.RequestUri); //获取一个包含url的Cookie集合的CookieCollection 
            Stream myResponseStream = myHttpWebResponse.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("GBK"));
            string outdata = myStreamReader.ReadToEnd(); //把数据从HttpWebResponse的Response流中读出 
            myStreamReader.Close();
            myResponseStream.Close();

            return cookieContainer;
        }

        /// <summary>
        /// 根据Firefox中的cookie text创建cookie
        /// </summary>
        /// <param name="cookieText"></param>
        /// <returns></returns>
        public static CookieContainer CreateCookieContainer(string cookieText)
        {
            CookieContainer result = new CookieContainer();

            string[] cookieStrArray1 = cookieText.Split(new string[] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries);

            foreach (var cookieStr in cookieStrArray1)
            {
                var keyValueStrArray = cookieStr.Split(new string[] {";"}, StringSplitOptions.RemoveEmptyEntries);

                System.Net.Cookie cookie = new Cookie();

                for (int i=0; i< keyValueStrArray.Length; i++)
                {
                    var keyValueStr = keyValueStrArray[i];

                    if (keyValueStr.IndexOf("=") < 0)
                    {
                        continue;
                    }

                    string key = keyValueStr.Substring(0, keyValueStr.IndexOf("=")).Trim();
                    string value = "";
                    if (keyValueStr.IndexOf("=") < keyValueStr.Length - 1)
                    {
                        value = keyValueStr.Substring(keyValueStr.IndexOf("=") + 1,
                            keyValueStr.Length - keyValueStr.IndexOf("=") - 1).Trim();
                    }

                    if (i == 0)
                    {
                        cookie.Name = key;
                        cookie.Value = value;
                    }
                    else if (key == "domain")
                    {
                        cookie.Domain = value;
                    }
                    else if (key == "path")
                    {
                        cookie.Path = value;
                    }
                }

                result.Add(cookie);
            }

            return result;
        }


    }
}
