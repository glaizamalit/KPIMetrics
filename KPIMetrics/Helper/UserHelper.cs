using System;
using KPIMetrics.Models;
using System.Net;
using System.IO;
using System.Configuration;
using Newtonsoft.Json;

namespace KPIMetrics.Helper
{
    public class UserHelper
    {
        public UserInfo GetUserInfoSSO(string userid)
        {
            try
            {

                userid = userid != null || userid != "" ? userid.Trim() : userid;
                string urlUserInfo = ConfigurationManager.AppSettings["WebserviceForUserInfo"] + userid + "?json=true";

                var request = (HttpWebRequest)WebRequest.Create(urlUserInfo);

                request.Method = "GET";

                var content = string.Empty;

                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    using (var stream = response.GetResponseStream())
                    {
                        using (var sr = new StreamReader(stream))
                        {
                            content = sr.ReadToEnd();
                        }
                    }
                }
                UserInfo userInfo = new UserInfo();

                userInfo = JsonConvert.DeserializeObject<UserInfo>(content);
                if (userInfo == null)
                {
                    throw new Exception("Initial " + userid + " not found in ADS user records.");
                }

                return userInfo;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}