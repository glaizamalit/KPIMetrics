using System;
using System.Web;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.Data.SqlClient;
using System.Net;
using System.Net.Sockets;
using System.Net.Mail;
using System.IO;
using Newtonsoft.Json;
using KPIMetrics.Helper;

namespace KPIMetrics.Sso
{
    public class SsoAuth : ActionFilterAttribute, IActionFilter
    {

        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private Models.VRMdbModelsDbContext dbVRM = new Models.VRMdbModelsDbContext();

        void IActionFilter.OnActionExecuting(ActionExecutingContext filterContext)
        {
            // var callerURL = filterContext.HttpContext.Request.Url;

            if (Sso.IsLogged())
            {
                //Get the rights
                if (!IsAuthorized())
                {
                    HttpContext.Current.Response.Redirect("~/Unauthorized");
                }
            }
            else
            {

                try
                {
                    string token = HttpContext.Current.Request.QueryString["tkid"];
                    string isAllow = HttpContext.Current.Request.QueryString["tkallow"];
                    bool isAllowed = isAllow == null ? false : Boolean.Parse(isAllow);
                    bool isAuthen = false;

                    if (string.IsNullOrEmpty(token))
                    {
                        logger.Debug("### Token is null, redirect to SSO, for " +
                            filterContext.HttpContext.Request.HttpMethod + " request ");
                      
                        HttpContext.Current.Session["tkusername"] = null;
                        HttpContext.Current.Session["UserFullName"] = null;                      
                        HttpContext.Current.Response.Redirect(Sso.SsoLoginurl());
                    }
                    else
                    {
                        // send token to IdP (SAML Request)
                        logger.Debug("Token is not null, send SAML request to IdP SSO ");

                        var data = Sso.PostToken(token, isAllowed);
                        //SPHelper.CheckSSOViaService(ref token, ref isAuthen, isAllow);
                        if (data != null)
                        {
                            // registerLink

                            HttpContext.Current.Session["tkusername"] = data[18].Value;
                            HttpContext.Current.Session["tkid"] = data[16].Value;

                            // store user full name
                            string strInitial = HttpContext.Current.Session["tkusername"].ToString();
                            UserHelper uh = new UserHelper();

                            var userItem = uh.GetUserInfoSSO(strInitial);
                            string strUserFullName = "";
                            if (userItem != null)
                            {
                                strUserFullName = userItem.FullName;
                            }
                            HttpContext.Current.Session["UserFullName"] = strUserFullName;

                            isAuthen = true;
                        }
                        logger.Debug("Called CheckSSO method, result: " + isAuthen);
                        if (isAuthen)
                        {
                            string usn = HttpContext.Current.Session["tkusername"].ToString();

                            logger.Info(usn + " - Authenticated.");

                            //TODO: Do we still need to get the token???

                            if (!IsAuthorized())
                            {
                                HttpContext.Current.Response.Redirect("~/Unauthorized");
                            }

                            this.OnActionExecuting(filterContext);
                        }
                        else
                        {
                            logger.Debug("Problem with SSO to verify token");
                            HttpContext.Current.Response.Redirect(Sso.SsoLoginurl());
                        }
                    }
                }
                catch (Exception ex)
                {
                    string usn = HttpContext.Current.Session["tkusername"].ToString();
                    SendMail(usn);
                    logger.Error("Error " + ex.Message);
                    logger.Error("Error " + ex.InnerException);

                    throw ex;
                }
            }

        }

        public Guid getid(string usn)
        {
            Guid userID = new Guid();
            if (usn != null)
            {

                try
                {
                    var connectionString = ConfigurationManager.ConnectionStrings["VRMdb"].ConnectionString;
                    string queryString = "select userID from tblusers where musrLoginID like '%" + usn + "%'";
                    using (var connection = new SqlConnection(connectionString))
                    {
                        var command = new SqlCommand(queryString, connection);
                        connection.Open();
                        var reader = command.ExecuteReader();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                userID = reader.GetGuid(0);
                            }
                        }
                        connection.Close();
                    }
                    return userID;
                }
                catch (Exception e)
                {
                    SendMail(usn);
                    return userID;
                }
            }
            else
            {
                SendMail(usn);
                userID = Guid.Empty;
                return userID;
            }

        }
        public string getIP()
        {
            string myIP = "";
            IPAddress[] localIPs = Dns.GetHostAddresses(Dns.GetHostName());
            foreach (IPAddress addr in localIPs)
            {
                if (addr.AddressFamily == AddressFamily.InterNetwork)
                {

                    myIP = addr.ToString();
                }
            }
            return myIP;

        }

        public void SendMail(string username)
        {
            try
            {
                /*
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(ConfigurationManager.AppSettings["SMTPFrom"]);
                mail.To.Add(ConfigurationManager.AppSettings["SMTPTo"]);
                mail.CC.Add(ConfigurationManager.AppSettings["SMTPCC"]);
                mail.Subject = ConfigurationManager.AppSettings["SMTPSubjectNoAccount"] + " " + DateTime.Now.ToString("yyyy-MM-dd");
                mail.Body = ConfigurationManager.AppSettings["SMTPGreetings"] + "\n\n" + "User with network login ID “" + username.ToUpper() + "” " + ConfigurationManager.AppSettings["SMTPMessageNoAccount"];
                SmtpClient smtp = new SmtpClient(ConfigurationManager.AppSettings["SMTPHost"]);
                smtp.Send(mail);
                */
                // send email disabled
            }
            catch (Exception ep)
            {
                Console.WriteLine("Failed to send email with the following error:");
                Console.WriteLine(ep.Message);
            }
        }

        public bool IsAuthorized()
        {
            /*
            var isAuthorized = HttpContext.Current.Session["IsAuthorized"];

            if (isAuthorized == null)
            {
                string UserInitial = HttpContext.Current.Session["tkusername"].ToString();
                var ResultList = dbVRM.Database.SqlQuery<Models.IsGroupMemberResult>("exec WPAdmin_IsGroupMember {0}", UserInitial);
                if (ResultList.Count() == 0)
                {
                    return false;
                }
                else
                {
                    HttpContext.Current.Session["IsAuthorized"] = "true";
                    return true;
                }
            }else
            {
                return true;
            }
            */

            return true;
        }

    }
}