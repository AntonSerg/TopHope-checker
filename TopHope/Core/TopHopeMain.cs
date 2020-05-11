using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Windows;
using AngleSharp.Parser;
using AngleSharp.Css;
using AngleSharp.Parser.Html;

namespace TopHope.Core
{
    class TopHopeMain
    {
        private  HttpClient client = null;
        public  HttpClient Client {
            get {
                return this.client;
            }
            set
            {
                this.client = value;
            }
        }

        private string nickname { get; set; }
        private string messagesCount { get; set; }
        private string thxCount { get; set; }
        private string pointsCount { get; set; }
        private string userId { get; set; }
        private string userImage { get; set; }

        public TopHopeMain()
        {
           
        }
        // simple get request for HTML parse
        public async Task<String> getResponseTopHope(string urlArg)
        {

            HttpRequestMessage requestGet = new HttpRequestMessage(HttpMethod.Get, Client.BaseAddress + urlArg);
            requestGet.Headers.Accept.TryParseAdd("text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
            requestGet.Headers.AcceptLanguage.TryParseAdd("ru-RU,ru;q=0.9,en-US;q=0.8,en;q=0.7");
            requestGet.Headers.UserAgent.TryParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/67.0.3396.99 Safari/537.36");
            using (var response = await client.SendAsync(requestGet))
            {
                return await response.Content.ReadAsStringAsync();

            }
        }
        public async Task<bool> getUserInfo()
        {
            string responseMessage;
            responseMessage = await getResponseTopHope("");
            HtmlParser htmlParser = new HtmlParser();
            // Nickname parse
            var document = await htmlParser.ParseAsync(responseMessage);
            var getRequest = document.QuerySelector("a.username.NoOverlay");
            string[] strTemp = getRequest.OuterHtml.Split(@"><".ToCharArray());
            nickname = strTemp[4];
            // MessageCount parse
            getRequest = document.QuerySelector("dl:nth-child(1).pairsJustified");
            strTemp = getRequest.OuterHtml.Split(@"><".ToCharArray());
            messagesCount = strTemp[8];
            // ThanksCount parse
            getRequest = document.QuerySelector("dl:nth-child(2).pairsJustified");
            strTemp = getRequest.OuterHtml.Split(@"><".ToCharArray());
            thxCount = strTemp[8];
            // PointsCount parse
            getRequest = document.QuerySelector("dl:nth-child(3).pairsJustified");
            strTemp = getRequest.OuterHtml.Split(@"><".ToCharArray());
            pointsCount = strTemp[8];
            // UserId parse
            getRequest = document.QuerySelector("a.LogOut");
            strTemp = getRequest.OuterHtml.Split(@"=%".ToCharArray());
            userId = strTemp[2];
            // http request for user link and image link parse
            responseMessage = await getResponseTopHope("members/" + nickname.ToLower() + "." + userId);
            document = await htmlParser.ParseAsync(responseMessage);
            string selector = "div.avatarScaler > a.Av" + userId + "l.OverlayTrigger > img";
            getRequest = document.QuerySelector(selector);
            strTemp = getRequest.OuterHtml.Split(@"=""".ToCharArray());
            userImage = strTemp[2];
            return true;
        }

        public string[] fillUserInfo()
        {
            string[] info = new string[5];
            info[0] = nickname;
            info[1] = messagesCount;
            info[2] = thxCount;
            info[3] = pointsCount;
            info[4] = userImage;
            return info;
        }
        // Annul user information
        public void topHopeMainLogout()
        {
            client = null;
            nickname = "";
            messagesCount = "";
            thxCount = "";
            pointsCount = "";
            userImage = "";
        }

    }
}
