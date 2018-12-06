using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Web.Security;

using System.Security.Principal;
public partial class MasterPage : System.Web.UI.MasterPage
{
    public virtual String MyTitle
    {
        get { return "Admin Portal"; }
    }
    public virtual String PageTitle
    {
        get;
        set;
    }
    private String sqlStr = Connection.GetConnectionString("PS_Production", ""); // PS_Production | PS_Stage
    protected void Page_PreInit(object sender, EventArgs e)
    {
        // ghFunctions.portalVersion = "20170130.001";
        // ghFunctions.portalVersion = ghFunctions.getPortalVersion();
        ghFunctions.portalVersion = System.Configuration.ConfigurationManager.AppSettings["portal_version"];

        // Master Page does not have a PreInit
        //SecureCheck();
        //if (ghFunctions.dtUserOffSet == 0)
        //{
        //    /// Switch this to a user determined variable
        //    /// Possibly in the MasterPage
        //    Int32 dtOffSet = 5;
        //    DateTime dtCurrent = DateTime.Now;
        //    System.TimeZone localZone = System.TimeZone.CurrentTimeZone;
        //    if (localZone.IsDaylightSavingTime(dtCurrent))
        //    {
        //        dtOffSet = 4;
        //    }
        //    else
        //    {
        //        dtOffSet = 5;
        //    }
        //    ghFunctions.dtUserOffSet = dtOffSet;
        //}
    }
    protected void SecureCheck()
    {
        //if (!Request.IsSecureConnection && !Request.IsLocal && !Request.Url.ToString().Contains("192.168.2"))
        if (!Request.IsSecureConnection && !Request.IsLocal && !Request.Url.ToString().Contains("192.168.") && !Request.Url.ToString().Contains("ciambotti-dsk"))
        {
            String redir = Request.Url.ToString().Replace("http:", "https:");
            Response.Redirect(@redir);
        }
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        SecureCheck();
        if (Session["userstatus"] == null)
        {
            Response.Redirect("logout.aspx", false);
        }
        if (Session["userid"] == null || Session["UserFullName"] == null)
        {
            identity_get_user();
        }
        if (Session["UserFullName"] != null)
        {
            UserFullName.Text = Session["UserFullName"].ToString();
        }
        else { UserFullName.Text = "n/a"; }
        ghFunctions.portalVersion = System.Configuration.ConfigurationManager.AppSettings["portal_version"];
        #region dtUserOffSet
        if (ghFunctions.dtUserOffSet == 0)
        {
            /// Switch this to a user determined variable
            /// Possibly in the MasterPage
            Int32 dtOffSet = 5;
            DateTime dtCurrent = DateTime.Now;
            System.TimeZone localZone = System.TimeZone.CurrentTimeZone;
            if (localZone.IsDaylightSavingTime(dtCurrent))
            {
                dtOffSet = 4;
            }
            else
            {
                dtOffSet = 5;
            }
            ghFunctions.dtUserOffSet = dtOffSet;
        }
        #endregion dtUserOffSet
        #region ARC Color
        lblMasterHeader.Text = "Portal - ARC";
        masterColor = "#EEEEEE";
        if (System.Configuration.ConfigurationManager.AppSettings["DBMode"] == "Stage")
        {
            lblMasterHeader.Text = "Stage - Portal - ARC";
            masterColor = "orange";
        }
        #endregion ARC Color
        if (!IsPostBack)
        {
            //Literal1.Text = Page.User.Identity.IsAuthenticated.ToString();
            //Literal2.Text = Page.User.Identity.Name;
            #region Perform certain action if user is authenticated
            if (Page.User.Identity.IsAuthenticated)
            {
                Literal3.Text = "";
                #region Load the Menu
                //Menue_Load();
                PopulateMenu(this.Page.User.Identity.Name);
                #endregion Load the Menu
                #region If User is a Sys Admin we add more menu options
                #endregion If User is a Sys Admin we add more menu options
                if (Page.User.IsInRole("System Administrator") == true)
                {
                    Literal3.Text += "System Administrator<br />";
                    Session["UserRole"] = "System Administrator";
                    Panel1.Visible = true;
                }
                if (Page.User.IsInRole("Administrator") == true)
                {
                    Literal3.Text += "Administrator<br />";
                    Session["UserRole"] = "Administrator";
                }
                if (Page.User.IsInRole("Manager") == true)
                {
                    Literal3.Text += "Manager<br />";
                    Session["UserRole"] = "Manager";
                }
                if (Page.User.IsInRole("Advisor") == true)
                {
                    Literal3.Text += "Advisor<br />";
                    Session["UserRole"] = "Advisor";
                }
                if (Page.User.IsInRole("Agent") == true)
                {
                    Literal3.Text += "Agent<br />";
                    Session["UserRole"] = "Agent";
                }
                if (Page.User.IsInRole("Client") == true)
                {
                    Literal3.Text += "Client";
                    Session["UserRole"] = "Client";
                    if (Page.User.IsInRole("Capella") == true)
                    {
                        Literal3.Text += " | Capella";
                        Session["UserClient"] = "Capella";
                    }
                    else if (Page.User.IsInRole("Troy") == true)
                    {
                        Literal3.Text += " | Troy";
                        Session["UserClient"] = "Troy";
                    }
                    else if (Page.User.IsInRole("Strayer") == true)
                    {
                        Literal3.Text += " | Strayer";
                        Session["UserClient"] = "Strayer";
                    }
                    else if (Page.User.IsInRole("Full Sail") == true)
                    {
                        Literal3.Text += " | Full Sail";
                        Session["UserClient"] = "Full Sail";
                    }
                    Literal3.Text += "<br />";
                }
                DBMode.Text = Connection.GetDBMode();
                if (DBMode.Text == "Stage") { DBMode.ForeColor = System.Drawing.Color.Red; } else { DBMode.ForeColor = System.Drawing.Color.Green; }
                DBMode.Text += "|" + Connection.GetConnectionType();
            }
            #endregion Perform certain action if user is authenticated
            //Simple Menu
            //G:\Documents and Settings\All Users\Desktop\Admin\Software\Web\dotNet\CSSFriendly_Adapter_1.0\CSSFriendly_1.0\Web\WalkThru
            //MenuLoad();
            //PopulateMenu(DeBug_Footer, Menu1, mi);

            // The header bar that labels the menu
            MessageLabel.Text = String.Format("Page: {0} - {1}"
                , MyTitle
                , PageTitle
                );
            // Highlight Active Menu..?
            lblTitleHeader.Text = " - " + PageTitle;
            Menu_Selected_Check(Menu1, PageTitle);
            /// This will verify that the user has access to the page
            /// This is done by checking that the page is in the Menu
            /// Change this so it's done through a query: is user allowed to see page
            /// This way we do not need to have the item in the menu section in order for the user to see it
            if (!verify_access() && Page.User.IsInRole("System Administrator") == false)
            {
                // Response.Redirect("access.aspx");
                Response.Redirect("~/access.aspx?p=" + PageTitle, true);
                // Server.Transfer("~/access.aspx", false); // This crashes
            }
            string currentPage = new System.IO.FileInfo(HttpContext.Current.Request.Url.LocalPath).Name;
            if (!(currentPage.ToLower().Contains("user_profile.aspx")) && Session["userstatus"] != null && Session["userstatus"].ToString() == "5")
            {
                Response.Redirect("user_profile.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
            }
        }
    }
    public string masterColor { get; set; }
    protected bool verify_access()
    {
        bool access = false;
        string currentPage = new System.IO.FileInfo(HttpContext.Current.Request.Url.LocalPath).Name;

        //DeBug_Footer.Text = "Current: " + currentPage;
        if (currentPage.ToLower().Contains("access.aspx") || currentPage.ToLower().Contains("default.aspx") || currentPage.ToLower().Contains("user_profile.aspx"))
        {
            access = true;
        }
        else
        {
            foreach (MenuItem mi in Menu1.Items)
            {
                if (verify_access_items(mi, currentPage))
                {
                    access = true;
                    break;
                }
                //DeBug_Footer.Text += "<br />" + mi.NavigateUrl.ToLower();
            }
        }
        //DeBug_Footer.Text += "<br />URL: " + HttpContext.Current.Request.Url.AbsolutePath;
        //DeBug_Footer.Text += "<br />IsFile: " + HttpContext.Current.Request.Url.IsFile.ToString();
        //DeBug_Footer.Text += "<br />Access: " + access.ToString();
        return access;

    }
    protected bool verify_access_items(MenuItem mi, String cp)
    {
        if (mi.NavigateUrl.ToLower().Contains(cp.ToLower()))
        {
            return true;
        }
        else if (mi.ChildItems.Count > 0)
        {
            foreach (MenuItem mc in mi.ChildItems)
            {
                if(verify_access_items(mc, cp))
                {
                    return true;
                }
            }
            return false;
        }
        else
        {
            return false;
        }
    }
    public void PopulateMenu(String UserName)
    {
        try
        {
            //UserName
            //http://www.codemyne.net/articles/Dynamic-Menu-using-Database-in-aspdotnet.aspx?visitid=25&type=2
            Menu1.Items.Clear();

            DataSet ds = new DataSet();

            #region SQL Connection
            if (Connection.GetDBMode() == "Stage")
            {
                sqlStr = Connection.GetConnectionString("PS_Stage", ""); // PS_Production | PS_Stage
            }
            using (SqlConnection con = new SqlConnection(sqlStr))
            {
                using (SqlCommand cmd = new SqlCommand("", con))
                {
                    if (con.State == ConnectionState.Closed) { con.Open(); }
                    cmd.CommandTimeout = 600;
                    cmd.CommandText = "[dbo].[user_menu_get_list]";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Clear();
                    #region SQL Parameters
                    cmd.Parameters.Add(new SqlParameter("@Source", "Web"));
                    cmd.Parameters.Add(new SqlParameter("@SP_UserName", UserName));
                    cmd.Parameters.Add(new SqlParameter("@SP_ModuleID", 3)); // Donation ARC Portal
                    #endregion SQL Parameters
                    #region SQL Processing
                    SqlDataAdapter ad = new SqlDataAdapter(cmd);
                    ad.Fill(ds, "Menu");
                    #endregion SQL Processing
                }
            }
            #endregion SQL Connection

            ds.DataSetName = "Menus";
            ds.Tables[0].TableName = "Menu";

            DataRelation relation = new DataRelation("Parentchild", ds.Tables["Menu"].Columns["MenuID"], ds.Tables["Menu"].Columns["ParentId"], true);
            relation.Nested = true;
            ds.Relations.Add(relation);


            XmlDataSource xmlDataSource = new XmlDataSource();
            xmlDataSource.ID = "XmlSource1";
            xmlDataSource.EnableCaching = false;

            xmlDataSource.Data = ds.GetXml();

            //Reformat the xmldatasource from the dataset to fit menu into xml format
            xmlDataSource.TransformFile = Server.MapPath("~/Menu.xslt");
            //assigning the path to start read all MenuItem under MenuItems
            xmlDataSource.XPath = "MenuItems/MenuItem";
            //Finally, bind the source to the Menu1 control
            Menu1.DataSource = xmlDataSource;
            Menu1.DataBind();
        }
        catch (Exception ex)
        {
            //Error_Save(ex, "Error: Loading Menu");
        }
    }
    protected void Menue_Load()
    {
        #region If User is a Sys Admin we add more menu options
        // Eventually this is replaced by a SQL Based Menu
        MenuItem mi = new MenuItem();
        MenuItem ci1 = new MenuItem();
        MenuItem ci2 = new MenuItem();
        #region Client: Capella
        if (Page.User.IsInRole("Capella") == true)
        {
            #region Load Client Menu: Capella
            mi = new MenuItem();
            mi.NavigateUrl = "~/Capella_Dashboard.aspx";
            mi.Text = "Capella";

            ci1 = new MenuItem();
            ci1.NavigateUrl = "~/Capella_Dashboard.aspx";
            ci1.Text = "Capella Dashboard";
            mi.ChildItems.Add(ci1);

            ci1 = new MenuItem();
            ci1.NavigateUrl = "~/Capella_Call_History.aspx";
            ci1.Text = "Capella Call History";
            mi.ChildItems.Add(ci1);

            Menu1.Items.Add(mi);
            #endregion Load Client Menu: Capella
        }
        #endregion Client: Capella
        #region Client: Strayer
        if (Page.User.IsInRole("Strayer") == true)
        {
            #region Load Client Menu: Strayer
            mi = new MenuItem();
            mi.NavigateUrl = "~/clients/strayer/Dashboard.aspx";
            mi.Text = "Strayer";

            ci1 = new MenuItem();
            ci1.NavigateUrl = "~/clients/strayer/Outcomes.aspx";
            ci1.Text = "Outcomes";
            mi.ChildItems.Add(ci1);

            ci1 = new MenuItem();
            ci1.NavigateUrl = "~/clients/strayer/Inbound_Detail.aspx";
            ci1.Text = "Inbound Detail";
            mi.ChildItems.Add(ci1);

            ci1 = new MenuItem();
            ci1.NavigateUrl = "~/clients/strayer/Transfers.aspx";
            ci1.Text = "Transfers";
            mi.ChildItems.Add(ci1);

            //ci1 = new MenuItem();
            //ci1.NavigateUrl = "~/clients/strayer/Transfer_Summary.aspx";
            //ci1.Text = "Transfer Summary";
            //mi.ChildItems.Add(ci1);

            ci1 = new MenuItem();
            ci1.NavigateUrl = "~/clients/strayer/Hourly.aspx";
            ci1.Text = "Hourly";
            mi.ChildItems.Add(ci1);

            ci1 = new MenuItem();
            ci1.NavigateUrl = "~/clients/strayer/Queue_Stats.aspx";
            ci1.Text = "Queue Stats";
            mi.ChildItems.Add(ci1);

            Menu1.Items.Add(mi);
            #endregion Load Client Menu: Troy
        }
        #endregion Client: Strayer
        #region Client: Troy
        if (Page.User.IsInRole("Troy") == true)
        {
            #region Load Client Menu: Troy
            mi = new MenuItem();
            mi.NavigateUrl = "~/clients/troy/Dashboard.aspx";
            mi.Text = "Troy";

            ci1 = new MenuItem();
            ci1.NavigateUrl = "~/clients/troy/Dashboard.aspx";
            ci1.Text = "Troy Dashboard";
            mi.ChildItems.Add(ci1);

            ci1 = new MenuItem();
            ci1.NavigateUrl = "~/clients/troy/Call_History.aspx";
            ci1.Text = "Troy Call History";
            mi.ChildItems.Add(ci1);

            Menu1.Items.Add(mi);
            #endregion Load Client Menu: Troy
        }
        #endregion Client: Troy
        #region Client: Full Sail
        if (Page.User.IsInRole("Full Sail") == true)
        {
            #region Load Client Menu: Full Sail
            mi = new MenuItem();
            mi.NavigateUrl = "~/clients/fullsail/Dashboard.aspx";
            mi.Text = "Full Sail";

            ci1 = new MenuItem();
            ci1.NavigateUrl = "~/clients/fullsail/Dashboard.aspx";
            ci1.Text = "Full Sail Dashboard";
            mi.ChildItems.Add(ci1);

            ci1 = new MenuItem();
            ci1.NavigateUrl = "~/clients/fullsail/Call_History.aspx";
            ci1.Text = "Full Sail Call History";
            mi.ChildItems.Add(ci1);

            Menu1.Items.Add(mi);
            #endregion Load Client Menu: Full Sail
        }
        #endregion Client: Full Sail
        #region Users: Manager, Administrator, System Administrator
        if (Page.User.IsInRole("Manager") == true
            || Page.User.IsInRole("Administrator") == true
            || Page.User.IsInRole("System Administrator") == true
            || (Page.User.Identity.Name.Contains("@greenwoodhall.com") == true && Page.User.IsInRole("Advisor") == true)
            )
        {
            MenuItem m0 = new MenuItem();
            m0 = new MenuItem();
            m0.Text = "Clients";
            #region Load all clients
            #region Load Client Menu: Internal
            mi = new MenuItem();
            mi.NavigateUrl = "~/clients/internal/Dashboard.aspx";
            mi.Text = "Internal";

            ci1 = new MenuItem();
            ci1.NavigateUrl = "~/clients/internal/Dashboard.aspx";
            ci1.Text = "Dashboard";
            mi.ChildItems.Add(ci1);

            ci1 = new MenuItem();
            ci1.NavigateUrl = "~/clients/internal/Call_History.aspx";
            ci1.Text = "Call History";
            mi.ChildItems.Add(ci1);

            ci1 = new MenuItem();
            ci1.NavigateUrl = "~/clients/internal/Billing.aspx";
            ci1.Text = "Billing";
            mi.ChildItems.Add(ci1);

            m0.ChildItems.Add(mi);
            //Menu1.Items.Add(mi);
            #endregion Load Client Menu: Internal
            #region Load Client Menu: Capella
            mi = new MenuItem();
            mi.NavigateUrl = "~/Capella_Dashboard.aspx";
            mi.Text = "Capella";

            ci1 = new MenuItem();
            ci1.NavigateUrl = "~/Capella_Dashboard.aspx";
            ci1.Text = "Capella Dashboard";
            mi.ChildItems.Add(ci1);

            ci1 = new MenuItem();
            ci1.NavigateUrl = "~/Capella_Call_History.aspx";
            ci1.Text = "Capella Call History";
            mi.ChildItems.Add(ci1);

            ci1 = new MenuItem();
            ci1.NavigateUrl = "~/Capella_Billing.aspx";
            ci1.Text = "Capella Billing";
            mi.ChildItems.Add(ci1);

            m0.ChildItems.Add(mi);
            //Menu1.Items.Add(mi);
            #endregion Load Client Menu: Capella
            #region Load Client Menu: RMCAD
            //mi = new MenuItem();
            //mi.NavigateUrl = "~/rmcad/RMCAD_Dashboard.aspx";
            //mi.Text = "RMCAD";

            //ci1 = new MenuItem();
            //ci1.NavigateUrl = "~/rmcad/RMCAD_Dashboard.aspx";
            //ci1.Text = "RMCAD Dashboard";
            //mi.ChildItems.Add(ci1);

            //ci1 = new MenuItem();
            //ci1.NavigateUrl = "~/rmcad/RMCAD_Call_History.aspx";
            //ci1.Text = "RMCAD Call History";
            //mi.ChildItems.Add(ci1);

            //ci1 = new MenuItem();
            //ci1.NavigateUrl = "~/rmcad/RMCAD_Billing.aspx";
            //ci1.Text = "RMCAD Billing";
            //mi.ChildItems.Add(ci1);

            //m0.ChildItems.Add(mi);
            //Menu1.Items.Add(mi);
            #endregion Load Client Menu: RMCAD
            #region Load Client Menu: Troy
            mi = new MenuItem();
            mi.NavigateUrl = "~/clients/troy/Dashboard.aspx";
            mi.Text = "Troy";

            ci1 = new MenuItem();
            ci1.NavigateUrl = "~/clients/troy/Dashboard.aspx";
            ci1.Text = "Troy Dashboard";
            mi.ChildItems.Add(ci1);

            ci1 = new MenuItem();
            ci1.NavigateUrl = "~/clients/troy/Call_History.aspx";
            ci1.Text = "Troy Call History";
            mi.ChildItems.Add(ci1);

            //ci1 = new MenuItem();
            //ci1.NavigateUrl = "~/clients/troy/Billing.aspx";
            //ci1.Text = "Troy Billing";
            //mi.ChildItems.Add(ci1);

            m0.ChildItems.Add(mi);
            //Menu1.Items.Add(mi);
            #endregion Load Client Menu: Troy
            #region Load Client Menu: Strayer
            mi = new MenuItem();
            mi.NavigateUrl = "~/clients/strayer/Dashboard.aspx";
            mi.Text = "Strayer";

            ci1 = new MenuItem();
            ci1.NavigateUrl = "~/clients/strayer/Outcomes.aspx";
            ci1.Text = "Outcomes";
            mi.ChildItems.Add(ci1);

            ci1 = new MenuItem();
            ci1.NavigateUrl = "~/clients/strayer/Inbound_Detail.aspx";
            ci1.Text = "Inbound Detail";
            mi.ChildItems.Add(ci1);

            ci1 = new MenuItem();
            ci1.NavigateUrl = "~/clients/strayer/Transfers.aspx";
            ci1.Text = "Transfers";
            mi.ChildItems.Add(ci1);

            //ci1 = new MenuItem();
            //ci1.NavigateUrl = "~/clients/strayer/Transfer_Summary.aspx";
            //ci1.Text = "Transfer Summary";
            //mi.ChildItems.Add(ci1);

            ci1 = new MenuItem();
            ci1.NavigateUrl = "~/clients/strayer/Hourly.aspx";
            ci1.Text = "Hourly";
            mi.ChildItems.Add(ci1);

            ci1 = new MenuItem();
            ci1.NavigateUrl = "~/clients/strayer/Queue_Stats.aspx";
            ci1.Text = "Queue Stats";
            mi.ChildItems.Add(ci1);

            m0.ChildItems.Add(mi);
            //Menu1.Items.Add(mi);
            #endregion Load Client Menu: Strayer
            #region Load Client Menu: Full Sail
            mi = new MenuItem();
            mi.NavigateUrl = "~/clients/fullsail/Dashboard.aspx";
            mi.Text = "Full Sail";

            ci1 = new MenuItem();
            ci1.NavigateUrl = "~/clients/fullsail/Dashboard.aspx";
            ci1.Text = "Full Sail Dashboard";
            mi.ChildItems.Add(ci1);

            ci1 = new MenuItem();
            ci1.NavigateUrl = "~/clients/fullsail/Call_History.aspx";
            ci1.Text = "Full Sail Call History";
            mi.ChildItems.Add(ci1);

            m0.ChildItems.Add(mi);
            //Menu1.Items.Add(mi);
            #endregion Load Client Menu: Full Sail
            #endregion Load all clients
            Menu1.Items.Add(m0);
        }
        #endregion Users: Manager, Administrator, System Administrator

        if (Page.User.IsInRole("System Administrator") != true && Page.User.IsInRole("Administrator") != true)
        {
            #region Load: User Profile
            mi = new MenuItem();
            mi.NavigateUrl = "~/User_Profile.aspx";
            mi.Text = "User Profile";

            Menu1.Items.Add(mi);
            #endregion Load: User Profile
        }
        if (Page.User.IsInRole("System Administrator") == true || Page.User.IsInRole("Administrator") == true)
        {
            #region Load: User Menu
            mi = new MenuItem();
            mi.NavigateUrl = "~/User_List.aspx";
            mi.Text = "Users";

            ci1 = new MenuItem();
            ci1.NavigateUrl = "~/User_List.aspx";
            ci1.Text = "User List";
            mi.ChildItems.Add(ci1);

            ci1 = new MenuItem();
            ci1.NavigateUrl = "~/User_Log.aspx";
            ci1.Text = "User Log";
            mi.ChildItems.Add(ci1);

            ci1 = new MenuItem();
            ci1.NavigateUrl = "~/User_Add.aspx";
            ci1.Text = "User Add";
            mi.ChildItems.Add(ci1);

            ci1 = new MenuItem();
            ci1.NavigateUrl = "~/User_Profile.aspx";
            ci1.Text = "User Profile";
            mi.ChildItems.Add(ci1);

            ci1 = new MenuItem();
            ci1.NavigateUrl = "~/User_Menu.aspx";
            ci1.Text = "User Menu";
            mi.ChildItems.Add(ci1);

            Menu1.Items.Add(mi);
            #endregion Load: User Menu
            
        }
        if (Page.User.IsInRole("System Administrator") == true)
        {
            #region Load: Tasks/Notes
            mi = new MenuItem();
            mi.NavigateUrl = "~/Tasks.aspx";
            mi.Text = "Tasks";

            ci1 = new MenuItem();
            ci1.NavigateUrl = "~/Notes.aspx";
            ci1.Text = "Notes";
            mi.ChildItems.Add(ci1);

            Menu1.Items.Add(mi);
            #endregion Load: Tasks/Notes

            #region Load: Sys Admin
            mi = new MenuItem();
            mi.NavigateUrl = "~/SystemAdministrator.aspx";
            mi.Text = "Sys Admin";
            #region Load: Emails
            ci1 = new MenuItem();
            ci1.NavigateUrl = "~/emails/";
            ci1.Text = "Emails";

            ci2 = new MenuItem();
            ci2.NavigateUrl = "~/emails/portal_email_password_reminder.html";
            ci2.Text = "Password Resetting Email";
            ci2.Target = "_blank";

            ci1.ChildItems.Add(ci2);
            mi.ChildItems.Add(ci1);
            #endregion Load: Emails
            #region Load: Offline Content
            ci1 = new MenuItem();
            ci1.NavigateUrl = "~/offline/Resetting.aspx";
            ci1.Text = "Password Resetting Tool";
            ci1.Target = "_blank";
            mi.ChildItems.Add(ci1);
            #endregion Load: Offline Content
            #region Load: Logs
            ci1 = new MenuItem();
            ci1.NavigateUrl = "~/Logs_User_Activity.aspx";
            ci1.Text = "Logs";
            ci1.Target = "_blank";

            ci2 = new MenuItem();
            ci2.NavigateUrl = "~/Logs_User_Activity.aspx";
            ci2.Text = "User Logs";
            ci1.ChildItems.Add(ci2);

            ci2 = new MenuItem();
            ci2.NavigateUrl = "~/Logs.aspx";
            ci2.Text = "Event Logs";
            ci1.ChildItems.Add(ci2);

            ci2 = new MenuItem();
            ci2.NavigateUrl = "~/Logs_Admin_SQL.aspx";
            ci2.Text = "SQL Logs";
            ci1.ChildItems.Add(ci2);

            ci2 = new MenuItem();
            ci2.NavigateUrl = "~/Logs_Admin_DOTNET.aspx";
            ci2.Text = ".NET Logs";
            ci1.ChildItems.Add(ci2);

            mi.ChildItems.Add(ci1);
            #endregion Load: Logs
            #region Load HTML Pages
            ci1 = new MenuItem();
            ci1.NavigateUrl = "~/ReportServlet.html";
            ci1.Text = "ReportServlet";
            mi.ChildItems.Add(ci1);
            #endregion

            Menu1.Items.Add(mi);
            #endregion Load: Sys Admin
        }
        #endregion If User is a Sys Admin we add more menu options
    }
    protected void Menu_Selected_CheckItem(MenuItem mi, String val)
    {
        foreach (MenuItem ci in mi.ChildItems)
        {
            if (ci.Text == val)
            {
                ci.Selected = true;
                MessageLabel.Text = String.Format("Menu: {0}"
                    , Page.Title.ToString()
                    );
            }
            else if (ci.ChildItems.Count > 0)
            {
                Menu_Selected_CheckItem(ci, val);
            }
        }
    }
    protected void Menu_Selected_Check(Menu men, String val)
    {
        foreach (MenuItem mi in men.Items)
        {
            if (mi.Text == val)
            {
                mi.Selected = true;
                MessageLabel.Text = String.Format("Menu: {0}"
                    , Page.Title.ToString()
                    );
                break;
            }
            else if (mi.ChildItems.Count > 0)
            {
                Menu_Selected_CheckItem(mi, val);
            }
        }
    }

    protected void Menu_Selected()
    {
        try
        {
            if (Menu1.Items.Count > 0)
            {
                foreach (MenuItem mi in Menu1.Items)
                {
                    if (mi.Text == Page.Title && mi.Selectable == true)
                    {
                        mi.Selected = true;
                        MessageLabel.Text = String.Format("Menu: {0}"
                            , Page.Title.ToString()
                            );
                        break;
                    }
                    if (mi.ChildItems.Count > 0)
                    {

                    }
                }
            }
        }
        catch
        {
            MessageLabel.Text = String.Format("Page: {0}"
                , Page.Title.ToString()
                );
        }
    }
    protected void Pop_Cookie()
    {
        Panel1.Visible = true;

        System.Text.StringBuilder output = new System.Text.StringBuilder();
        HttpCookie aCookie;
        string subkeyName;
        string subkeyValue;

        //for (int i = 0; i < Request.Cookies.Count; i++)
        //{
        //    aCookie = Request.Cookies[i];
        //    output.Append("Cookie name = " + Server.HtmlEncode(aCookie.Name)
        //        + "<br />");
        //    output.Append("Cookie value = " + Server.HtmlEncode(aCookie.Value)
        //        + "<br /><br />");
        //}
        //DeBug_Footer.Text += "<hr />" + output.ToString();
        //output = new System.Text.StringBuilder();

        //for (int i = 0; i < Request.Cookies.Count; i++)
        //{
        //    aCookie = Request.Cookies[i];

        //    output.Append("Name = " + aCookie.Name + "<br />");
        //    output.Append("Expires = " + aCookie.Expires.ToString() + "<br />");
        //    output.Append("Secure = " + aCookie.Secure.ToString() + "<br />");
        //    if (aCookie.HasKeys)
        //    {
        //        for (int j = 0; j < aCookie.Values.Count; j++)
        //        {
        //            subkeyName = Server.HtmlEncode(aCookie.Values.AllKeys[j]);
        //            subkeyValue = Server.HtmlEncode(aCookie.Values[j]);
        //            output.Append("Subkey name = " + subkeyName + "<br />");
        //            output.Append("Subkey value = " + subkeyValue + "<br /><br />");
        //        }
        //    }
        //    else
        //    {
        //        output.Append("Value = " + Server.HtmlEncode(aCookie.Value) +
        //            "<br /><br />");
        //    }
        //}
        DeBug_Footer.Text += "<hr />" + output.ToString();
        output = new System.Text.StringBuilder();
        for (int i = 0; i < Request.Cookies.Count; i++)
        {
            aCookie = Request.Cookies[i];
            output.Append("Name = " + aCookie.Name + "<br />");
            output.Append("Expires = " + aCookie.Expires.ToString() + "<br />");
            output.Append("Secure = " + aCookie.Secure.ToString() + "<br />");
            if (aCookie.HasKeys)
            {
                System.Collections.Specialized.NameValueCollection CookieValues =
                    aCookie.Values;
                string[] CookieValueNames = CookieValues.AllKeys;
                for (int j = 0; j < CookieValues.Count; j++)
                {
                    subkeyName = Server.HtmlEncode(CookieValueNames[j]);
                    subkeyValue = Server.HtmlEncode(CookieValues[j]);
                    output.Append("Subkey name = " + subkeyName + "<br />");
                    output.Append("Subkey value = " + subkeyValue +
                        "<br /><br />");
                }
            }
            else
            {
                output.Append("Value = " + Server.HtmlEncode(aCookie.Value) +
                    "<br /><br />");
            }
        }
        DeBug_Footer.Text += "<hr />" + output.ToString();


    }
    protected void Pop_Var()
    {

        String tst = "";

        String tst2 = Request.ServerVariables["PATH_TRANSLATED"].ToString().Replace(Request.ServerVariables["APPL_PHYSICAL_PATH"].ToString(), "");
        //String tst2 = Request.ServerVariables["SCRIPT_NAME"].ToString();
        DeBug_Footer.Text += "<hr />" + tst2 + "<hr />";

        //c:\inetpub\wwwroot\webservice\Portal\
        //c:\inetpub\wwwroot\webservice\portal\Default.aspx

        tst = "<table>";
        try
        {
            foreach (string key in Request.ServerVariables.AllKeys)
            {
                tst += String.Format("<tr><td>{0}</td><td>{1}</td></tr>", key, Request.ServerVariables[key]);
            }
            tst += "</table>";
        }
        catch
        {
            for (int i = 0; i < Request.ServerVariables.Count; i++)
            {
                tst += String.Format("<tr><td>{0}</td><td>{1}</td></tr>", i, Request.ServerVariables[i].ToString());
            }
            tst += "</table>";
        }

        DeBug_Footer.Text += tst;
    }
    protected void OnClick(Object sender, MenuEventArgs e)
    {
        //if (e.Item.Parent.Text != "")
        //{
        //    MessageLabel.Text = "You selected " + e.Item.Parent.Text + " > " + e.Item.Text + ".";
        //}
        //else
        //{
        //    MessageLabel.Text = "You selected " + e.Item.Text + ".";
        //}
        MessageLabel.Text = " >> " + e.Item.Text + ".";
        e.Item.Selected = true;
    }
    protected void MenuLoad()
    {
        MenuItem mi = new MenuItem();
        mi.Text = "New";
        mi.Selected = true;

        MenuItem miC = new MenuItem();
        miC.Text = "Child";

        mi.ChildItems.Add(miC);

        //PortalMenu.Items.Add(mi);
    }
    public bool Menu_Item_Toggle_Selected(String mPage, String cPage)
    {
        if (mPage == cPage)
        {
            return false;
        }
        else
        {
            return false;
        }
    }
    protected void Get_Children(MenuItem newMenuItem, DataRow masterRow)
    {
        Label label_error = DeBug_Footer;
        try
        {
            #region Load Children
            foreach (DataRow childRow in masterRow.GetChildRows("Children"))
            {
                MenuItem childItem = new MenuItem((string)childRow["Text"], (string)childRow["Text"], string.Empty, (string)childRow["NavigateURL"], (string)childRow["Target"]);
                newMenuItem.ChildItems.Add(childItem);
                if (childRow["Children"] != null)
                {
                    if ((string)childRow["Children"] == "true")
                    {
                        Get_Children(childItem, childRow);
                    }
                }
            }
            #endregion
        }
        catch (Exception ex)
        {
            Error_Catch(ex, "Menu - Error Loading - Step 6", label_error);
        }
    }

    protected void Error_Catch(Exception ex, String error, Label lbl)
    {
        Panel1.Visible = true;
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
    }
    protected void identity_get_user()
    {
        // Get the logged in users userid
        // This should be retrieved during the login process
        // if (Session["userid"] == null) { identity_get_userid(); }
        // cmd.Parameters.Add(new SqlParameter("@sp_actor", Session["userid"].ToString()));
        try
        {
            #region SQL Connection
            if (Connection.GetDBMode() == "Stage")
            {
                sqlStr = Connection.GetConnectionString("PS_Stage", ""); // PS_Production | PS_Stage
            }
            using (SqlConnection con = new SqlConnection(sqlStr))
            {
                #region SQL Command
                using (SqlCommand cmd = new SqlCommand("", con))
                {
                    if (con.State == ConnectionState.Closed) { con.Open(); }
                    cmd.CommandTimeout = 600;
                    // cmdText = "[portal_user].[dbo].[user_get_userid]";
                    // cmd.CommandType = CommandType.StoredProcedure;
                    String cmdText = "";
                    cmdText = @"
                                SELECT
                                [u].[userid]
                                ,[u].[firstname]
                                ,[u].[lastname]
                                FROM [portal_user].[dbo].[user] [u] WITH(NOLOCK)
                                WHERE 1=1
                                AND [u].[username] = @sp_username";

                    cmd.CommandText = cmdText;
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Clear();
                    #region SQL Parameters
                    //cmd.Parameters.Add(new SqlParameter("@UserID", UserID));
                    cmd.Parameters.Add(new SqlParameter("@sp_username", this.Page.User.Identity.Name));
                    #endregion SQL Parameters
                    #region SQL Processing
                    using (SqlDataReader sqlRdr = cmd.ExecuteReader())
                    {
                        if (sqlRdr.HasRows)
                        {
                            while (sqlRdr.Read())
                            {
                                Session["userid"] = sqlRdr["userid"].ToString();
                                String NameFirst = sqlRdr["firstname"].ToString();
                                String NameLast = sqlRdr["lastname"].ToString();
                                String NameFull = String.Format("{0} {1}", NameFirst, NameLast).Trim();
                                Session["UserFullName"] = NameFull;
                                //Label8.Text += "<br />UserID: " + Session["userid"].ToString();
                            }
                        }
                    }
                    #endregion SQL Processing
                }
                #endregion SQL Command
            }
            #endregion SQL Connection
        }
        catch (Exception ex)
        {
            Error_Catch(ex, "User Get UserID", DeBug_Footer);
            //Error_Save(ex, "User Get UserID");
        }
    }

}
