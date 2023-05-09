using Ligg.Infrastructure.DataModels;
using Ligg.Infrastructure.Extensions;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Ligg.Infrastructure.Utilities.DataParserUtil;

namespace Ligg.Infrastructure.Helpers
{
    public class HttpClientHelper
    {


        private static readonly string _typeFullName = System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName;
        private static readonly object LockObj = new object();
        private static string BaseUrl;

        private static string Token = "";
        private static HttpClient _client =null;
        //private static HttpClient _client = new HttpClient();
        public static void Initialize(string baseUrl)
        {

            if (_client == null)
            {
                lock (LockObj)
                {
                    if (_client == null)
                    {
                        BaseUrl = baseUrl;
                        _client = new HttpClient();
                    }
                }
            }
            //return _client;
        }
        public static string Logon(string url, string usrCode, string usrPass)
        {
            if (BaseUrl.IsNullOrEmpty()) throw new ArgumentException(_typeFullName + ".logon error: BaseUrl can't be empty");

            url = BaseUrl + "/" + url + "?account=" + usrCode + "&password=" + usrPass;
            try
            {
                HttpContent content = new StringContent("");
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                var response = _client.PostAsync(url, content);
                var responseResult = response.Result;
                var statusCode = responseResult.StatusCode;
                if (statusCode == HttpStatusCode.OK)
                {
                    var contentResult = responseResult.Content.ReadAsStringAsync().Result;
                    var result = JsonHelper.ConvertToGeneric<TResult>(contentResult);
                    if (result != null)
                        if (result.Flag == 1)
                        {
                            var token = result?.Message;
                            Token = token;
                            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("LrdClient", Token); //error //Client.DefaultRequestHeaders.Add("Authorization", "erd " + Token);
                            return "{\"Success\":true}";
                        }
                        else
                        {
                            return "{\"Success\":false, \"Message\":\"" + result.Message+"\"}";
                        }

                }
                return "{\"Success\":false, \"Message\":\"" + statusCode.ToString() + "\"}";
            }
            catch (Exception ex)
            {
                throw new ArgumentException(_typeFullName + ".logon error: " + ex.Message);
            }
        }

        public static string Get(string url, string[] paramArr)
        {
            var query = TextDataHelper.GetHttpClientParamString(paramArr);
            url = BaseUrl + "/" + url + (string.IsNullOrEmpty(query) ? "" : "?" + query);
            try
            {
                var response = _client.GetAsync(url);
                var responseResult = response.Result;
                if (responseResult.StatusCode == HttpStatusCode.OK)
                {
                    string responseResultString = responseResult.Content.ReadAsStringAsync().Result;
                    var result = JsonHelper.ConvertToGeneric<TResult>(responseResultString);
                    if (result != null)
                        if (result.Flag == 1)
                        {
                            var rstData = responseResultString.GetSubStringSurroundedByTwoDifferentIdentifiers("data\":", ",\"flag\"");
                            if (rstData.StartsWith("\"")& rstData.EndsWith("\""))
                                rstData = rstData.Replace("\"",string.Empty);
                            return rstData;
                        }
                        else
                        {
                            var msg =result.Message;
                            //throw new ArgumentException(msg);
                            return "";
                        }
                    throw new ArgumentException("illegal visit");
                }
                else throw new ArgumentException(responseResult.ReasonPhrase);
            }
            catch (Exception ex)
            {
                throw new ArgumentException(_typeFullName + ".Get error: " + ex.Message); 
            }
        }

        public static string Post(string url, string data = "", string[] paramArr = null)
        {
            var query = TextDataHelper.GetHttpClientParamString(paramArr);
            url = BaseUrl + "/" + url + (string.IsNullOrEmpty(query) ? "" : "?" + query);
            HttpContent body = new StringContent("");
            if (data != "") body = new StringContent(data, Encoding.UTF8, "application/json");
            try
            {

                var response = _client.PostAsync(url, body);
                var responseResult = response.Result;
                var contentResult = responseResult.Content.ReadAsStringAsync().Result;
                if (responseResult.StatusCode == HttpStatusCode.OK)
                {
                    string responseResultString = responseResult.Content.ReadAsStringAsync().Result;
                    var result = JsonHelper.ConvertToGeneric<TResult>(responseResultString);
                    if (result != null)
                    {
                        var msg = result.Message;
                        var rst = new UniversalResult(true,"");
                        if (result.Flag == 1)
                        {
                            return "{\"Success\":true,\"Message\":\"" + msg + "\"}";
                        }
                        else
                        {
                            
                            return "{\"Success\":false,\"Message\":\"" + msg+ "\"}";
                            //throw new ArgumentException(msg);
                        }
                    }

                    throw new ArgumentException("illegal visit");
                }
                else throw new ArgumentException(responseResult.ReasonPhrase);
            }
            catch (Exception ex)
            {
                throw new ArgumentException(_typeFullName + ".Post error: " + ex.Message);
            }
        }



    }
}