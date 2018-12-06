using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using System.Data;
using System.Data.SqlClient;
public partial class Login : System.Web.UI.Page
{
    private bool m_bIsTerminating = false;
    private String sqlStr = Connection.GetConnectionString("PS_Production", ""); // PS_Production | PS_Stage
    private String mode = Connection.GetSiteMode();
    private String mode_db = Connection.GetDBMode();
    private String pageRedi = "Dashboard.aspx";
    protected void Page_PreInit(object sender, EventArgs e)
    {
        ghUser.SecureCheck();
        ghFunctions.portalVersion = System.Configuration.ConfigurationManager.AppSettings["portal_version"];
        if (Connection.GetDBMode() == "Stage")
        {
            sqlStr = Connection.GetConnectionString("PS_Stage", ""); // PS_Production | PS_Stage
        }
    }
    protected void SecureCheck()
    {
        if (!Request.IsSecureConnection && !Request.IsLocal && !Request.Url.ToString().Contains("192.168.") && !Request.Url.ToString().Contains("mylocal"))
        {
            String redir = Request.Url.ToString().Replace("http:", "https:");
            //String toDomainStage = "hihportal.telethongiving2.com";
            //redir = redir.Replace("hihportal.telethongiving2.com", toDomainStage);
            Response.Redirect(@redir);
        }
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack && !IsCallback)
        {
            Label3.Text = mode;
            if (mode == "Maintenance") { Label3.ForeColor = System.Drawing.Color.Red; } else { Label3.ForeColor = System.Drawing.Color.Green; }
            DBMode.Text = mode_db;
            if (mode_db == "Stage") { DBMode.ForeColor = System.Drawing.Color.Red; } else { DBMode.ForeColor = System.Drawing.Color.Green; }

            // if (Connection.userIP().Contains("192.168.2")) { Panel3.Visible = true; }

            if (User.Identity.IsAuthenticated)
            {
                // We are already logged in, redirect to default page
                Login_Authenticated_Redirect("LoggedIn");
            }
            if (Request["t"] != null)
            {
                if (Request["t"].ToString() == "1")
                {
                    Panel1.Visible = true;
                    Panel2.Visible = false;
                }
                else
                {
                    Panel1.Visible = false;
                    Panel2.Visible = true;
                }
            }
            //ReturnUrl
            if (Request["ReturnUrl"] != null)
            {
                lblLoginMessage.Text = "You are not logged in, please login to access this web application.";
                lblLoginMessage.Text += " You will be redirected after login in to: ";
                //lblLoginMessage.Text += "<br />" + Request["ReturnUrl"].ToString();
                String page = Request["ReturnUrl"].ToString();
                page = page.Substring(page.LastIndexOf("/") + 1, page.Length - 1 - page.LastIndexOf("/"));
                lblLoginMessage.Text += "<br />" + page;

            }
            // lblLoginMessage.Text += "<br />" + FormsAuthentication.LoginUrl;
            // lblLoginMessage.Text += "<br />" + FormsAuthentication.DefaultUrl;
        }
        else
        {
            if (lblLoginMessage.Text.Length > 0) { lblLoginMessage.Text = ""; }
        }
    }

    #region Handle the Login Request
    protected void Login_Submit(object sender, EventArgs e)
    {
        System.Threading.Thread.Sleep(50);
        if (mode == "Maintenance" && (Username.Text != "nciambotti@greenwoodhall.com" || Username.Text != "cstevenson@greenwoodhall.com"))
        {
            news_content_inner.Visible = false;
            maintenance_mode.Visible = true;
            lblLoginMessage.Text = "<br />Portal is currently disabled for Maintenance.";
        }
        else
        {
            // Initialize FormsAuthentication, for what it's worth
            Login_Submit_SQL();
        }
    }
    protected void Login_Submit_SQL()
    {
        String sqlResponse = "";
        String sqlResponseFull = "";
        String sqlUserName = "";
        String sqlRole = "";
        String sqlSessionID = null;
        if (Session["sessionkey"] != null)
        {
            sqlSessionID = Session["sessionkey"].ToString();
        }
        try
        {
            #region SQL Connection
            using (SqlConnection con = new SqlConnection(sqlStr))
            {
                #region SQL Command
                using (SqlCommand cmd = new SqlCommand("", con))
                {
                    if (con.State == ConnectionState.Closed) { con.Open(); }
                    cmd.CommandTimeout = 600;
                    String cmdText = "";
                    cmdText = "[dbo].[user_login_authenticate_new]";
                    cmd.CommandText = cmdText;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Clear();
                    #region SQL Parameters
                    //cmd.Parameters.Add(new SqlParameter("@UserID", UserID));
                    cmd.Parameters.Add(new SqlParameter("@SP_UserName", Username.Text.ToLower()));
                    cmd.Parameters.Add(new SqlParameter("@SP_Password", Password.Text));
                    cmd.Parameters.Add(new SqlParameter("@SP_SessionKey", sqlSessionID));
                    #endregion SQL Parameters
                    #region SQL Parameters - Session
                    string myHost = System.Net.Dns.GetHostName();
                    string myIP = System.Net.Dns.GetHostEntry(myHost).AddressList[0].ToString();
                    cmd.Parameters.Add(new SqlParameter("@SP_ServerName", myHost));
                    cmd.Parameters.Add(new SqlParameter("@SP_ServerIP", myIP));
                    cmd.Parameters.Add(new SqlParameter("@SP_ServerPort", Request.ServerVariables["SERVER_PORT"].ToString()));
                    cmd.Parameters.Add(new SqlParameter("@SP_ServerSecure", Request.ServerVariables["SERVER_PORT_SECURE"].ToString()));
                    cmd.Parameters.Add(new SqlParameter("@SP_ServerDomain", Request.ServerVariables["SERVER_NAME"].ToString()));
                    cmd.Parameters.Add(new SqlParameter("@SP_ServerPage", Request.ServerVariables["SCRIPT_NAME"].ToString()));
                    cmd.Parameters.Add(new SqlParameter("@SP_ServerURL", Request.Url.ToString()));
                    // Don Stats
                    cmd.Parameters.Add(new SqlParameter("@SP_UserHostAddress", Request.UserHostAddress.ToString()));
                    cmd.Parameters.Add(new SqlParameter("@SP_UserHostName", Request.UserHostName.ToString()));
                    cmd.Parameters.Add(new SqlParameter("@SP_UserAgent", Request.UserAgent.ToString()));
                    cmd.Parameters.Add(new SqlParameter("@SP_UserDateTime", UserStart.Value.ToString()));
                    cmd.Parameters.Add(new SqlParameter("@SP_ServerDateTime", DateTime.Now.ToString()));

                    #endregion SQL Parameters - Session
                    #region SQL Processing
                    using (SqlDataReader sqlRdr = cmd.ExecuteReader())
                    {
                        if (sqlRdr.HasRows)
                        {
                            while (sqlRdr.Read())
                            {
                                sqlResponse = sqlRdr["Response"].ToString();
                                sqlResponseFull = sqlRdr["ResponseFull"].ToString();
                                sqlUserName = sqlRdr["UserName"].ToString();

                                Session["username"] = sqlRdr["username"].ToString();
                                Session["userid"] = sqlRdr["userid"].ToString();
                                Session["userstatus"] = sqlRdr["userstatus"].ToString();
                                Session["sessionkey"] = sqlRdr["SessionKey"].ToString();
                                Session["userfullname"] = sqlRdr["fullname"].ToString();

                                if (sqlRole.Length > 0)
                                {
                                    sqlRole += "," + sqlRdr["Role"].ToString();
                                }
                                else
                                {
                                    sqlRole = sqlRdr["Role"].ToString();
                                }
                                if (sqlRdr["client"].ToString() != "")
                                {
                                    Session["clientname"] = sqlRdr["client"].ToString();
                                    Session["clientcache"] = DateTime.UtcNow.ToString(); // How long we cache it for
                                    // ghUser.identity_get_client();
                                }
                                lblLoginMessage.Text = "<br />" + sqlRdr["Role"].ToString();
                            }
                        }
                    }
                    //lblLoginMessage.Text += "<br />" + dv.ID;
                    #endregion SQL Processing
                }
                #endregion SQL Command
            }
            #endregion SQL Connection
        }
        catch (Exception ex)
        {
            news_content_inner.Visible = false;
            Error_Display(ex, "Login Attempt", lblLoginMessage);
            Error_Save(ex, "Login Attempt");
        }
        finally
        {
            if (sqlResponse == "Success")
            {
                Login_Authenticate_Success(sqlUserName, sqlRole);
            }
            else
            {
                news_content_inner.Visible = false;
                lblLoginMessage.Text += "<br />" + sqlResponse;
                lblLoginMessage.Text += "<br />" + sqlResponseFull;
            }
        }
    }
    protected void Login_Authenticate_Success(String UserName, String UserRole)
    {
        //FormsAuthentication.RedirectFromLoginPage("Debug", false);
        //UserRole = "Admin";
        FormsAuthenticationTicket tkt;
        String cookiestr;
        HttpCookie ck;
        Double expMnts;
        expMnts = 1440;
        tkt = new FormsAuthenticationTicket(1, Username.Text.ToLower(), DateTime.Now,
                                            DateTime.Now.AddMinutes(expMnts), chkPersistCookie.Checked,
                                            UserRole, FormsAuthentication.FormsCookiePath);
        cookiestr = FormsAuthentication.Encrypt(tkt);
        ck = new HttpCookie(FormsAuthentication.FormsCookieName, cookiestr);
        //ck.Domain = "192.168.2.107";
        if (chkPersistCookie.Checked) ck.Expires = tkt.Expiration;
        Response.Cookies.Add(ck);

        Login_Authenticated_Redirect("Auth");
        //String strRedirect;
        //strRedirect = Request["ReturnUrl"];
        //if (strRedirect == null)
        //    strRedirect = "Default.aspx";
        //Response.Redirect(strRedirect, false);
        //HttpContext.Current.ApplicationInstance.CompleteRequest();
        //m_bIsTerminating = true;
        //// remember to end the method here if
        //// there is more code in it
        //return;        
    }
    /// <summary>
    /// Ensure the redirect does not cause an error exception
    /// </summary>
    protected void Login_Authenticated_Redirect(String strType)
    {
        /// Need to figure out a better way to get the redirect page
        /// For some reason IsInRole doesn't seem to work from here
        /// Could be too early in the session cycle
        String ReturnUrl;
        String strRedirect;
        ReturnUrl = Request["ReturnUrl"];
        if (ReturnUrl != null)
        {
            if (strType == "LoggedIn")
            {
                strRedirect = "Default.aspx?ReturnUrl=" + ReturnUrl;
                strRedirect = "Dashboard.aspx";
                strRedirect = "Search.aspx";
                if (Page.User.IsInRole("CDR Fundraising Group")) { strRedirect = "Search.aspx"; }
            }
            else
            {
                strRedirect = Request["ReturnUrl"];
            }
        }
        else
        {
            strRedirect = "Default.aspx";
            strRedirect = "Dashboard.aspx";
            strRedirect = "Search.aspx";
            // strRedirect = pageRedi;
            if (Page.User.IsInRole("CDR Fundraising Group")) { strRedirect = "Search.aspx"; }
        }
        Response.Redirect(strRedirect, false);
        HttpContext.Current.ApplicationInstance.CompleteRequest();
        m_bIsTerminating = true;
    }
    #endregion Handle the Login Request

    #region Handle the Password Reminder Request
    protected void ForgotPassword_Click(object sender, EventArgs e)
    {
        Panel1.Visible = false;
        Panel2.Visible = true;
    }
    protected void ForgotPassword_Cancel(object sender, EventArgs e)
    {
        //PopupControlExtender1.Cancel();
        Panel1.Visible = true;
        Panel2.Visible = false;
    }
    protected void ForgotPassword_Submit(object sender, EventArgs e)
    {
        // Verify the username they entered exists
        // Generate the Resetting Key
        // Send an email to the user
        // Respond back with a message
        #region Overal Password Help Tool Try
        try
        {
            #region SQL Connection
            using (SqlConnection con = new SqlConnection(sqlStr))
            {
                #region SQL Command
                using (SqlCommand cmd = new SqlCommand("", con))
                {
                    if (con.State == ConnectionState.Closed) { con.Open(); }
                    cmd.CommandTimeout = 600;
                    String cmdText = "";
                    cmdText = "[dbo].[user_password_reminder]";
                    cmd.CommandText = cmdText;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Clear();
                    #region SQL Parameters
                    cmd.Parameters.Add(new SqlParameter("@SP_UserName", Username_Retrieve.Text));
                    #endregion SQL Parameters
                    #region SQL Processing
                    using (SqlDataReader sqlRdr = cmd.ExecuteReader())
                    {
                        if (sqlRdr.HasRows)
                        {
                            while (sqlRdr.Read())
                            {
                                // If we generated a key, send the email
                                // If there was some kind of error, report on it
                                Label2.Text = "Response:<br />";
                                Label2.Text += sqlRdr["Response"].ToString() + "<br />";
                                Label2.Text += sqlRdr["ResponseFull"].ToString() + "<br />";
                                Label2.Text += sqlRdr["UserID"].ToString() + "<br />";
                                Label2.Text += sqlRdr["KeyValue"].ToString() + "<br />";
                                Label2.Text += sqlRdr["KeyExpiresPST"].ToString() + "<br />";
                                try
                                {
                                    //2012-01-27 22:45:45.873
                                    if (sqlRdr["Response"].ToString() == "Success")
                                    {
                                        //DateTime keyExpire = DateTime.Parse("2012-01-27 22:45:45.873");
                                        Label2.Text = "Request processed successfully:";
                                        DateTime keyExpire = DateTime.Parse(sqlRdr["KeyExpiresPST"].ToString());
                                        String keyValue = sqlRdr["KeyValue"].ToString();
                                        String spName = sqlRdr["Name"].ToString();
                                        String spEmail = sqlRdr["Email"].ToString();
                                        ForgotPassword_Email(keyValue, keyExpire, spName, spEmail);
                                    }
                                    else
                                    {
                                        Label2.Text = "Problem processing request:<br />";
                                        Label2.Text += sqlRdr["Response"].ToString() + "<br />";
                                        Label2.Text += sqlRdr["ResponseFull"].ToString() + "<br />";
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Error_Display(ex, "Password Reset", Label2);
                                }
                            }
                        }

                    }
                    //lblLoginMessage.Text += "<br />" + dv.ID;
                    #endregion SQL Processing

                }
                #endregion SQL Command
            }
            #endregion SQL Connection
        }
        #endregion Overal Password Help Tool Try
        #region Overal Password Help Tool Catch
        catch (Exception ex)
        {
            Error_Save(ex, "Pasword Reset");
            Error_Display(ex, "Password Reset", Label2);
        }

        #endregion Overal Password Help Tool Catch

    }
    protected void ForgotPassword_Email(String key, DateTime expire, String rtr_Name, String rtr_Email)
    {
        using (Chilkat.MailMan mailman = new Chilkat.MailMan())
        {
            //mailman.UnlockComponent("SGREENWOODMAILQ_FuY9K2d92R8F");
            //mailman.SmtpHost = Connection.GetSmtpHost();

            mailman.UnlockComponent("SGREENWOODMAILQ_FuY9K2d92R8F");
            mailman.SmtpHost = Connection.SmtpHost();
            mailman.SmtpPort = Int32.Parse(Connection.SmtpPort());
            mailman.SmtpUsername = Connection.SmtpUsername();
            mailman.SmtpPassword = Connection.SmtpPassword();
            mailman.StartTLS = true;

            using (Chilkat.Email email = new Chilkat.Email())
            {
                email.Subject = "Greenwood & Hall Portal - Password Reset";

                String openEmail = Server.MapPath(".") + @"\emails\portal_email_password_reminder.html";

                System.IO.StreamReader rdr = new System.IO.StreamReader(System.IO.File.OpenRead(openEmail));
                String htmlBody = rdr.ReadToEnd(); rdr.Close(); rdr.Dispose();
                String resetLink = "https://portal.greenwoodhall.com/offline/resetting.aspx?k=" + key;
                htmlBody = htmlBody.Replace("{ResetLink}", resetLink);

                //{ResetLink}
                //{Expire_Date}
                htmlBody = htmlBody.Replace("{Expire_Date}", expire.ToString("MM/dd/yyyy"));
                //{Expire_Time}
                htmlBody = htmlBody.Replace("{Expire_Time}", expire.ToString("HH:mm:ss"));
                //{Expire_Zone}
                htmlBody = htmlBody.Replace("{Expire_Zone}", "PST");
                //{RequestTime}
                htmlBody = htmlBody.Replace("{RequestTime}", DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"));
                //{RequestIP} myIP
                htmlBody = htmlBody.Replace("{RequestIP}", Connection.userIP());
                //{ScriptTimeZone}
                htmlBody = htmlBody.Replace("{ScriptTimeZone}", "PST");

                email.SetHtmlBody(htmlBody);
                //email.AddTo("Pehuen Test", "nciambotti@greenwoodhall.com");
                email.AddTo(rtr_Name, rtr_Email);

                email.BounceAddress = "do.not.reply@greenwoodhall.com";
                email.FromName = "Greenwood & Hall Portal";
                email.FromAddress = "do.not.reply@greenwoodhall.com";
                email.ReplyTo = "do.not.reply@greenwoodhall.com";
                bool success;
                success = mailman.SendEmail(email);
                if (success)
                {
                    Label2.Text += "<br />Email Sent";
                    //Label2.Text += "<br />" + mailman.LastSmtpStatus;

                    lblLoginMessage.Text = Label2.Text;
                    lblLoginMessage.Text += "<br />Please check your inbox within a few minutes for a link to reset your password.";

                    Panel1.Visible = true;
                    Panel2.Visible = false;
                }
                else
                {
                    Label2.Text += "<br />Email Failed, internal server error, please try again.";
                }
                //Label2.Text += "<br />" + Connection.GetSmtpHost();
                //Label2.Text += "<br />" + DateTime.Now.ToString("MM/dd/yyyy");
                //Label2.Text += "<br />" + DateTime.Now.ToString("HH:mm:ss");
            }
        }
    }

    #endregion Handle the Password Reminder Request
    protected void Error_Save(Exception ex, String error)
    {
        string sPath = HttpContext.Current.Request.Url.AbsolutePath;
        string[] strarry = sPath.Split('/');
        int lengh = strarry.Length;
        String spPage = strarry[lengh - 1];
        String spURL = HttpContext.Current.Request.Url.ToString();
        String spQS = HttpContext.Current.Request.Url.Query.ToString();
        if (error == null) { error = "General Error"; }

        DetailsView dv = null; // ErrorView;

        ErrorLog.ErrorLog_Save(ex, dv, "Greenwood & Hall Portal", error, spPage, spQS, spURL);
    }
    protected void Error_Display(Exception ex, String error, Label lbl)
    {
        lbl.Text = String.Format("<table class='table_error'>"
            + "<tr><td>Error<td/><td>{0}</td></tr>"
            + "<tr><td>Message<td/><td>{1}</td></tr>"
            + "<tr><td>StackTrace<td/><td>{2}</td></tr>"
            + "<tr><td>Source<td/><td>{3}</td></tr>"
            + "<tr><td>InnerException<td/><td>{4}</td></tr>"
            + "<tr><td>Data<td/><td>{5}</td></tr>"
            + "</table>"
            , error //0
            , ex.Message //1
            , ex.StackTrace //2
            , ex.Source //3
            , ex.InnerException //4
            , ex.Data //5
            , ex.HelpLink
            , ex.TargetSite
            );

        //ErrorLog.ErrorLog(ex);

    }

}
