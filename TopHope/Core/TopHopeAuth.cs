using System;
using System.Collections.Generic;
using System.Windows;
using System.Net.Http;
using System.Threading.Tasks;
using AngleSharp.Parser;
using AngleSharp.Css;
using AngleSharp.Parser.Html;

namespace TopHope.Core
{
    class TopHopeAuth
    {
        private HttpClient client;
        private readonly string url = "https://tophope.ru";
        // Needed HTML code from response
        private string responseHtmlCode;
        // xF token for logining user(pick from HTML code when you are login in)
        private string XfToken { get; set; }

        public async Task<bool> Authorization(string login, string password)
        {
           
            // Collection for POST Request
            Dictionary<string, string> keyRequests = new Dictionary<string, string>
            {
                { "login" , login },
                { "register", "0" },
                { "password", password },
                { "remember", "1" },
                { "cookie_check", "1" },
                { "redirect", "/" },
                { "_xfToken", "" }



            };
            // content of POST request /login=...
            var content = new FormUrlEncodedContent(keyRequests);
            // Adding headers to request
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, this.url + "/login/login");
            request.Headers.Accept.TryParseAdd("text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
            request.Headers.AcceptLanguage.TryParseAdd("ru-RU,ru;q=0.9,en-US;q=0.8,en;q=0.7");
            request.Headers.UserAgent.TryParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/67.0.3396.99 Safari/537.36");
            request.Headers.Host = "tophope.ru";
            request.Headers.Referrer = new Uri(this.url + "/");
            request.Content = content;
            // HTTP POST REQUEST
            using (var response = await client.SendAsync(request))
            {
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    responseHtmlCode = await response.Content.ReadAsStringAsync();
                    return true;

                }
                else
                {
                    MessageBox.Show("Govno", "Response");
                    new Exception("SomethingWrong with status_code AUTH");
                    return false;
                }

            }



        }

        public async Task clientCreate()
        {
            XfToken = null;
            // Handler for HTTPClient
            HttpClientHandler handler = new HttpClientHandler
            {
                CookieContainer = new System.Net.CookieContainer(),
                UseCookies = true,
                AllowAutoRedirect = true
            };
            // Decoder for HTML code
            if (handler.SupportsAutomaticDecompression)
            {
                handler.AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate;
            }
            client = new HttpClient(handler);
            client.BaseAddress = new Uri(this.url);
            // GET request to get cookies from site, without it you can't login in
            client = await GetRequstForCookies(client);
        }
        // GET Request for cookies with your session for login in
        private async Task<HttpClient> GetRequstForCookies(HttpClient client)
        {
            HttpRequestMessage requestGet = new HttpRequestMessage(HttpMethod.Get, this.url + "/login");
            requestGet.Headers.Accept.TryParseAdd("text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
            requestGet.Headers.AcceptLanguage.TryParseAdd("ru-RU,ru;q=0.9,en-US;q=0.8,en;q=0.7");
            requestGet.Headers.UserAgent.TryParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/67.0.3396.99 Safari/537.36");
            using (var response = await client.SendAsync(requestGet))
            {
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return client;
                }
                else
                {
                    throw new Exception("GetRequestFromCookies HTTpStatusCode != 200");
                }

            }
        }
        // Logout
        public async void Logout()
        {
            // get xFToken for logout
            await xfTokenGet();
            if (XfToken == null)
            {
                // probably you are not login in if xFToken == null
                MessageBox.Show("You are not login in yet!");
                return;
            }
            // Creating get request => adding headers etc.
            HttpRequestMessage requestGet = new HttpRequestMessage(HttpMethod.Get, this.url + "/logout/?_xfToken=" + XfToken);
            requestGet.Headers.Accept.TryParseAdd("text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
            requestGet.Headers.AcceptLanguage.TryParseAdd("ru-RU,ru;q=0.9,en-US;q=0.8,en;q=0.7");
            requestGet.Headers.UserAgent.TryParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/67.0.3396.99 Safari/537.36");
            // GET request for logout
            using (var response = await client.SendAsync(requestGet))
            {
                
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    MessageBox.Show("You are successfully logged out");
                    XfToken = null;
                    return;
                }
                else
                {
                    throw new Exception("GetRequestFromCookies HTTpStatusCode != 200");
                }

            }
        }
        // Parsing xFToken from responseHtmlCode
        public async Task xfTokenGet()
        {
            HtmlParser parser = new HtmlParser();
            var document = await parser.ParseAsync(responseHtmlCode);
            // CSSSelection from <href a class=LogOut ...>
            var getRequest = document.QuerySelector("a.LogOut");
            if (getRequest == null)
            {
                return;
            }
            // Split html code and select our XFToken
            string[] str = getRequest.OuterHtml.Split(@"=*""".ToCharArray());
            XfToken = str[3];
            return;
        }

        public async Task<HttpClient> checkLogin()
        {
            await xfTokenGet();
            if (XfToken == null)
            {
                MessageBox.Show("Sorry, you are not login in.\nWrong Login/Password!", "Login failed");
                return null;
            }
            else
            {
                MessageBox.Show("Login successful!", "Login succeed");
                return client;
            }
        }

        public async Task<bool> checkerLogin()
        {
            await xfTokenGet();
            if (XfToken == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
