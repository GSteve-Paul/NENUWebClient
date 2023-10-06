using HtmlAgilityPack;
using Microsoft.Maui.Primitives;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using MauiNenuHttpClient.Exception;
using Jurassic;

namespace MauiNenuHttpClient
{
    public class NenuClient : HttpClient
    {
        private string _Username;
        private string _Password;
        private bool _IsLogin;

        // form params
        private string lt, dllt, execution, _eventId, rmShown, pwdDefaultEncryptSalt;


        public string Username { get { return _Username; } set { _Username = value; } }
        public string Password { get { return _Password; } set { _Password = value; } }
        public bool IsLogin { get { return _IsLogin; } }

        public NenuClient() : base(new HttpClientHandler()
        {
            CookieContainer = new CookieContainer(),
            UseCookies = true,
            AllowAutoRedirect = false,
            ServerCertificateCustomValidationCallback = (sender, certificate, chain, sslPolicyErrors) => { return true; }
        })
        {
            _IsLogin = false;
            this.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/100.0.0.0 Safari/537.36");
        }

        public NenuClient(string username, string password) : this()
        {
            _Username = username;
            _Password = password;
        }

        private void GetFormParameters()
        {
            string url = "https://authserver.nenu.edu.cn/authserver/login?service=http%3A%2F%2Fdsjx.nenu.edu.cn%3A80%2F";
            HttpResponseMessage resp = this.GetAsync(url).Result;
            string content = resp.Content.ReadAsStringAsync().Result;
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(content);
            HtmlNode node;
            //lt
            node = htmlDoc.DocumentNode.SelectSingleNode("//*[@id=\"casLoginForm\"]/input[1]");
            lt = node.GetAttributeValue("value", "");
            //dllt
            node = htmlDoc.DocumentNode.SelectSingleNode("//*[@id=\"casLoginForm\"]/input[2]");
            dllt = node.GetAttributeValue("value", "");
            //execution
            node = htmlDoc.DocumentNode.SelectSingleNode("//*[@id=\"casLoginForm\"]/input[3]");
            execution = node.GetAttributeValue("value", "");
            //_eventId
            node = htmlDoc.DocumentNode.SelectSingleNode("//*[@id=\"casLoginForm\"]/input[4]");
            _eventId = node.GetAttributeValue("value", "");
            //rmShown
            node = htmlDoc.DocumentNode.SelectSingleNode("//*[@id=\"casLoginForm\"]/input[5]");
            rmShown = node.GetAttributeValue("value", "");
            //pwdDefaultEncryptedSalt
            node = htmlDoc.DocumentNode.SelectSingleNode("//*[@id=\"casLoginForm\"]/input[6]");
            pwdDefaultEncryptSalt = node.GetAttributeValue("value", "");
        }

        private string GetEncryptedPassword(string password, string mask)
        {
            //get the javascript code
            string url = "https://authserver.nenu.edu.cn/authserver/custom/js/encrypt.js";
            HttpResponseMessage resp = this.GetAsync(url).Result;
            string jsCode = resp.Content.ReadAsStringAsync().Result;

            //run the javascript code
            ScriptEngine engine = new ScriptEngine();
            engine.Execute(jsCode);
            return engine.CallGlobalFunction<string>("encryptAES",password,mask);
        }

        private void LoginRequest(string username,string password)
        {
            try
            {
                GetFormParameters();
            }
            catch (AggregateException) 
            {
                throw new InvalidNetworkException();
            }
            password = GetEncryptedPassword(password, pwdDefaultEncryptSalt);
            try
            {
                List<KeyValuePair<string, string>> formData = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("username", username),
                    new KeyValuePair<string, string>("password", password),
                    new KeyValuePair<string, string>("lt", lt),
                    new KeyValuePair<string, string>("dllt", dllt),
                    new KeyValuePair<string, string>("execution", execution),
                    new KeyValuePair<string, string>("_eventId", _eventId),
                    new KeyValuePair<string, string>("rmShown",rmShown)
                };

                HttpResponseMessage resp;
                string url = "https://authserver.nenu.edu.cn/authserver/login?service=http%3A%2F%2Fdsjx.nenu.edu.cn%3A80%2F";
                HttpContent loginRequestContent = new FormUrlEncodedContent(formData);
                resp = this.PostAsync(url, loginRequestContent).Result;//login 302

                url = resp.Headers.GetValues("Location").ElementAt(0);
                resp = this.GetAsync(url).Result;//ticket 302

                url = resp.Headers.GetValues("Location").ElementAt(0);
                resp = this.GetAsync(url).Result;//jsession 302

                url = "http://dsjx.nenu.edu.cn/framework/main.jsp";
                resp = this.GetAsync(url).Result;//dsjx 302

                url = "http://dsjx.nenu.edu.cn/Logon.do?method=logonBySSO";
                resp = this.PostAsync(url, new StringContent("")).Result;//logonBySSO

            }
            catch(InvalidOperationException)
            {
                throw new InvalidUsernamePasswordException();   
            }
        }

        public void Login()
        {
            LoginRequest(_Username, _Password);
            _IsLogin = true;
        }

        public void Login(string username,string password)
        {
            _Username = username;
            _Password = password;
            Login();
        }
    }
}
