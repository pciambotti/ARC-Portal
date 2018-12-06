using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
public partial class CallLinks : System.Web.UI.Page
{
    // Controlled by Web Config 
    private String sqlStrARC = Connection.GetConnectionString("ARC_Production", ""); // ARC_Production | ARC_Stage
    private String sqlStrDE = Connection.GetConnectionString("DE_Production", ""); // DE_Production || DE_Stage
    private String sqlStr = Connection.GetConnectionString("PS_Production", ""); // PS_Production | PS_Stage
    protected void Page_PreInit(Object sender, EventArgs e)
    {
        Master.PageTitle = "Call Links";
        this.Title = String.Format("{0}: {1}", Master.MyTitle, Page.Title);

        if (Connection.GetDBMode() == "Stage")
        {
            sqlStrARC = Connection.GetConnectionString("ARC_Stage", ""); // ARC_Production || ARC_Stage
            sqlStrDE = Connection.GetConnectionString("DE_Stage", ""); // DE_Production || DE_Stage
            sqlStr = Connection.GetConnectionString("PS_Stage", ""); // PS_Production | PS_Stage
        }
    }
    protected void Page_Load(object sender, EventArgs e)
    {

        if (Session["userid"] == null) { identity_get_userid(); }
        if (!IsPostBack)
        {
            //DateTime sp_datestart = DateTime.Parse("05/18/2015 00:00:00");
            //DateTime sp_dateend = DateTime.Parse(DateTime.UtcNow.ToString("MM/dd/yyyy 23:59:59"));
            //dtStartDate.Text = sp_datestart.ToString("MM/dd/yyyy");

            //Label8.Text += "<br />" + this.Page.User.Identity.Name;
            GridView_Refresh();
            // Clear Date/Time Fields


        }
    }
    protected void GridView_Refresh(object sender, EventArgs e)
    {
        GridView_Refresh();
    }
    protected void GridView_Refresh()
    {
        try
        {
            Int32 UserID = Convert.ToInt32(Session["userid"].ToString()); // This shouldn't be here, should be global
            #region Populate the grids
            // This populates the mind grid with record list
            Search_Data_Query(UserID, this.Page.User.Identity.Name, gvSearchResults);
            #endregion Populate the grids
            // Clear the "All" Panels
            gvSearchResults.SelectedIndex = -1;
            Session["gridrefresh"] = DateTime.UtcNow.ToString();
        }
        catch (Exception ex)
        {
            Label8.Text = "Error processing the request";
            Error_Save(ex, "GridView_Refresh");
        }
    }
    protected void GridView_Export_Excel(object sender, EventArgs e)
    {
        Int32 UserID = Convert.ToInt32(Session["userid"].ToString());
        gvSearchResults.AllowPaging = false;
        Search_Data_Query(UserID, this.Page.User.Identity.Name, gvSearchResults);
        GridViewExportUtil.Export("IRV-Call-Records.xls", this.gvSearchResults);
    }
    protected void Search_Data_Query(Int32 UserID, String UserName, GridView gv)
    {
        // Change to this section should be duplicated to this section: Search_Data_Query_Counts

        
        #region SQL Connection
        try
        {
            #region SQL Connection
            using (SqlConnection con = new SqlConnection(sqlStrDE))
            {
                ghFunctions.Donation_Open_Database(con);
                #region SQL Command
                using (SqlCommand cmd = new SqlCommand("", con))
                {
                    #region Build cmdText
                    String cmdText = "";
                    cmdText += @"
SELECT
TOP 500
[i].[companyid]
,[i].[interactionid]
,[fic].[name] [campaign]
,[fa].[fullname]
,[i].[originator]
,[i].[destinator]
,[ia].[dispositionname]
,[fc].[datestart]
,DATEDIFF(s,[i].[datestart],GETUTCDATE()) [age]
,[fa].[agentid]
FROM [dbo].[interactions] [i] WITH(NOLOCK)
LEFT OUTER JOIN [dbo].[interactions_arc] [ia] WITH(NOLOCK) ON [ia].[companyid] = [i].[companyid] AND [ia].[interactionid] = [i].[interactionid]
LEFT OUTER JOIN [dbo].[five9_call] [fc] WITH(NOLOCK) ON [fc].[companyid] = [i].[companyid] AND [fc].[interactionid] = [i].[interactionid]
LEFT OUTER JOIN [dbo].[five9_item] [fic] WITH(NOLOCK) ON [fic].[typeid] = 101000000 AND [fic].[itemid] = [fc].[campaignid]
LEFT OUTER JOIN [dbo].[five9_item] [fis] WITH(NOLOCK) ON [fis].[typeid] = 102000000 AND [fis].[itemid] = [fc].[skillid]
LEFT OUTER JOIN [dbo].[five9_call_agent] [fca] WITH(NOLOCK) ON [fca].[companyid] = [i].[companyid] AND [fca].[interactionid] = [i].[interactionid]
LEFT OUTER JOIN [dbo].[five9_agent] [fa] WITH(NOLOCK) ON [fa].[agentid] = [fca].[agentid]
WHERE 1=1
AND ([fic].[name] LIKE 'ARC%' OR [fic].[name] LIKE 'American%')
AND [ia].[interactionid] IS NOT NULL
AND [ia].[dispositionid] = -1
ORDER BY [i].[interactionid] DESC
--AND DATEDIFF(d,[i].[datestart],GETUTCDATE()) < 10
--AND ([ia].[interactionid] IS NULL OR [ia].[dispositionid] = -1)
--AND DATEDIFF(d,[i].[datestart],GETUTCDATE()) < 10
--AND [fa].[agentid] NOT IN (100000000)
                            ";
                    cmdText += "\r";
                    #endregion Build cmdText
                    #region SQL Command Config
                    cmd.CommandTimeout = 600;
                    cmd.CommandText = cmdText;
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Clear();
                    #endregion SQL Command Config
                    #region SQL Command Parameters
                    //cmd.Parameters.Add("@sp_callid", SqlDbType.Int).Value = 2016;
                    #endregion SQL Command Parameters
                    print_sql(cmd); // Will print for Admin in Local
                    #region SQL Command Processing
                    SqlDataAdapter ad = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    ad.Fill(dt);
                    gv.DataSource = dt; //dv_ivr_file_vc.DataSource = dt;
                    gv.DataBind(); //dv_ivr_file_vc.DataBind();
                    //dtlLabel.Text += "<br />" + gv.ID;
                    #endregion SQL Command Processing
                }
                #endregion SQL Command

            }
            #endregion SQL Connection
        }
        catch (Exception ex)
        {
            Error_Save(ex, "Search_Data_Query");
        }
        #endregion SQL Connection
    }
    protected void GridView_DataBound(Object sender, EventArgs e)
    {
        Label8.Text = " Records: [" + gvSearchResults.Rows.Count.ToString() + "]";
        if (gvSearchResults.PageCount > 0)
        {
            Label8.Text += " - Pages: [" + gvSearchResults.PageCount.ToString() + "]";
            Label8.Text += " - Approx Total: [" + (gvSearchResults.PageCount * gvSearchResults.Rows.Count).ToString() + "]";
            // Retrieve the pager row.
            //GridViewRow pagerRow = gvSearchResults.BottomPagerRow;
            GridViewRow pagerRow = gvSearchResults.TopPagerRow;
            // Retrieve the DropDownList and Label controls from the row.
            DropDownList pageList = (DropDownList)pagerRow.Cells[0].FindControl("PageDropDownList");
            Label pageLabel = (Label)pagerRow.Cells[0].FindControl("CurrentPageLabel");
            if (pageList != null)
            {
                // Create the values for the DropDownList control based on 
                // the  total number of pages required to display the data
                // source.
                for (int i = 0; i < gvSearchResults.PageCount; i++)
                {
                    // Create a ListItem object to represent a page.
                    int pageNumber = i + 1;
                    ListItem item = new ListItem(pageNumber.ToString());
                    // If the ListItem object matches the currently selected
                    // page, flag the ListItem object as being selected. Because
                    // the DropDownList control is recreated each time the pager
                    // row gets created, this will persist the selected item in
                    // the DropDownList control.   
                    if (i == gvSearchResults.PageIndex)
                    {
                        item.Selected = true;
                    }
                    // Add the ListItem object to the Items collection of the 
                    // DropDownList.
                    pageList.Items.Add(item);
                }
            }
            if (pageLabel != null)
            {
                // Calculate the current page number.
                int currentPage = gvSearchResults.PageIndex + 1;
                // Update the Label control with the current page information.
                pageLabel.Text = "Page " + currentPage.ToString() +
                  " of " + gvSearchResults.PageCount.ToString();
            }
            if (gvSearchResults.PageIndex > 0)
            {
                pagerRow.Cells[0].FindControl("lnkPrevPage").Visible = true;
                pagerRow.Cells[0].FindControl("lnkFirstPage").Visible = true;
            }
            else
            {
                pagerRow.Cells[0].FindControl("lnkPrevPage").Visible = false;
                pagerRow.Cells[0].FindControl("lnkFirstPage").Visible = false;
            }

            if (gvSearchResults.PageCount != (gvSearchResults.PageIndex + 1))
            {
                pagerRow.Cells[0].FindControl("lnkNextPage").Visible = true;
                pagerRow.Cells[0].FindControl("lnkLastPage").Visible = true;
            }
            else
            {
                pagerRow.Cells[0].FindControl("lnkNextPage").Visible = false;
                pagerRow.Cells[0].FindControl("lnkLastPage").Visible = false;
            }
        }
    }
    protected void GridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            //e.Row.Attributes["onmouseover"] = "this.style.cursor='hand';this.style.textDecoration='none';this.style.background='Gray';";
            //e.Row.Attributes["onmouseout"] = "this.style.textDecoration='none';this.style.background=this.originalstyle;";

            //http://aspdotnetfaq.com/Faq/How-to-correctly-highlight-GridView-rows-on-Mouse-Hover-in-ASP-NET.aspx
            // when mouse is over the row, save original color to new attribute, and change it to highlight yellow color
            e.Row.Attributes.Add("onmouseover",
                "this.style.cursor='pointer';this.originalstyle=this.style.backgroundColor;this.style.backgroundColor='#336699'");

            // when mouse leaves the row, change the bg color to its original value    
            e.Row.Attributes.Add("onmouseout",
                "this.style.backgroundColor=this.originalstyle;");

            e.Row.Attributes["onclick"] = Page.ClientScript.GetPostBackClientHyperlink(this.gvSearchResults, "Select$" + e.Row.RowIndex);
        }
    }
    protected void GridView_IndexChanged_Old(object sender, EventArgs e)
    {
        dtlLabel.Text = "Feature not enabled: " + DateTime.Now.ToString("HH:mm:ss");

    }
    protected void GridView_IndexChanged(object sender, EventArgs e)
    {
        GridView gv = (GridView)sender;
        if (gv.ID == "gvSearchResults")
        {
            #region gvSearchGrid
            try
            {
                // Admin_Clear();
                Int32 companyid = Convert.ToInt32(gvSearchResults.SelectedDataKey.Values[0].ToString());
                Int32 interactionid = Convert.ToInt32(gvSearchResults.SelectedDataKey.Values[1].ToString());
                DetailsView_DataExchange_Data(interactionid, dvAgentScript);
                DetailsView_DataExchange_Data(interactionid, dvInteraction);
                DetailsView_DataExchange_Data(interactionid, dvInteractionDetails);

                // btnExportDetails.Visible = true;
            }
            catch (Exception ex)
            {
                Error_Save(ex, "DetailsView Data Error");
            }
            #endregion gvSearchGrid
        }
        else
        {
            dtlLabel.Text = "Details...";
        }
    }
    protected void DetailsView_DataExchange_Data(Int32 interactionid, DetailsView dv)
    {
        #region SQL Connection
        using (SqlConnection con = new SqlConnection(sqlStrDE))
        {
            #region SQL Command
            using (SqlCommand cmd = new SqlCommand("", con))
            {
                if (con.State == ConnectionState.Closed) { con.Open(); }
                cmd.CommandTimeout = 600;
                String cmdText = "";
                #region dvAgentScript
                if (dv.ID == "dvAgentScript")
                {
                    cmdText = @"
SELECT
TOP (@sp_top)

[i].[companyid]
,[i].[interactionid]
,[ia].[interactionid] [ia_interactionid]
,[fc].[interactionid] [fc_interactionid]



,[fca].[stationid] [agent_stationid]
,[fa].[five9id] [agent_five9id]
,[fa].[fullname] [agent_fullname]
,[fca].[stationtype] [agent_stationtype]
,[fa].[username] [agent_username]
,[i].[originator] -- ani
,[fct].[bill_time]
,[fc].[callid] [five9_callid]
,[fic].[five9id] [five9_campaignid]
,[fic].[name] [five9_campaign]
,'' [comments]
,[fid].[five9id] [five9_dispositionid]
,[fid].[name] [five9_disposition]
,[i].[destinator] -- dnis
,REPLACE(REPLACE(REPLACE(REPLACE(CONVERT(varchar,[fc].[dateend],121),'-',''),':',''),' ',''),'.','') [dateend]
,[fct].[handle_time]
,[fct].[hold_time]
,[fct].[length]
,[fim].[name] [five9_mediatype]
,[fct].[park_time]
,[fct].[queue_time]
,[fc].[sessionid]
,[fis].[five9id] [five9_skillid]
,[fis].[name] [five9_skill]
--,[fc].[datestart]
,REPLACE(REPLACE(REPLACE(REPLACE(CONVERT(varchar,[fc].[datestart],121),'-',''),':',''),' ',''),'.','') [datestart]
,'' [tcpa_date_of_consent]
,[fit].[five9id] [five9_typeid]
,[fit].[name] [five9_type]
,[fct].[wrapup_time]
FROM [dbo].[interactions] [i] WITH(NOLOCK)
LEFT OUTER JOIN [dbo].[interactions_arc] [ia] WITH(NOLOCK) ON [ia].[companyid] = [i].[companyid] AND [ia].[interactionid] = [i].[interactionid]
LEFT OUTER JOIN [dbo].[five9_call] [fc] WITH(NOLOCK) ON [fc].[companyid] = [i].[companyid] AND [fc].[interactionid] = [i].[interactionid]
LEFT OUTER JOIN [dbo].[five9_item] [fic] WITH(NOLOCK) ON [fic].[typeid] = 101000000 AND [fic].[itemid] = [fc].[campaignid]
LEFT OUTER JOIN [dbo].[five9_item] [fis] WITH(NOLOCK) ON [fis].[typeid] = 102000000 AND [fis].[itemid] = [fc].[skillid]
LEFT OUTER JOIN [dbo].[five9_call_agent] [fca] WITH(NOLOCK) ON [fca].[companyid] = [i].[companyid] AND [fca].[interactionid] = [i].[interactionid]
LEFT OUTER JOIN [dbo].[five9_agent] [fa] WITH(NOLOCK) ON [fa].[agentid] = [fca].[agentid]
LEFT OUTER JOIN [dbo].[five9_call_time] [fct] WITH(NOLOCK) ON [fct].[companyid] = [i].[companyid] AND [fct].[interactionid] = [i].[interactionid]
LEFT OUTER JOIN [dbo].[five9_call_disposition] [fcd] WITH(NOLOCK) ON [fcd].[companyid] = [i].[companyid] AND [fcd].[interactionid] = [i].[interactionid]
LEFT OUTER JOIN [dbo].[five9_item] [fid] WITH(NOLOCK) ON [fid].[typeid] = 103000000 AND [fid].[itemid] = [fcd].[dispositionid]
LEFT OUTER JOIN [dbo].[five9_item] [fim] WITH(NOLOCK) ON [fim].[typeid] = 107000000 AND [fim].[itemid] = [fc].[mediatypeid]
LEFT OUTER JOIN [dbo].[five9_item] [fit] WITH(NOLOCK) ON [fit].[typeid] = 104000000 AND [fit].[itemid] = [fc].[typeid]
WHERE 1=1
AND [i].[interactionid] = @sp_interactionid
ORDER BY [i].[interactionid] DESC, [fcd].[datecreated] DESC



                            ";
                }
                #endregion dvAgentScript
                #region dvInteraction
                if (dv.ID == "dvInteraction")
                {
                    cmdText = @"
SELECT
TOP (@sp_top)
[i].[companyid]
,[i].[interactionid]
,[fc].[callid] [five9_callid]
,[ia].[callid] [arc_callid]
,[fic].[name] [campaign]
,[fis].[name] [skill]
,[ia].[dispositionname] [arc_disposition]

,[fa].[fullname] [agent_fullname]

,[fca].[agentid]


,[i].[originator]
,[i].[destinator]

,[fc].[datestart]
,[fc].[dateend]

,[fc].[sessionid]

FROM [dbo].[interactions] [i] WITH(NOLOCK)
LEFT OUTER JOIN [dbo].[interactions_arc] [ia] WITH(NOLOCK) ON [ia].[companyid] = [i].[companyid] AND [ia].[interactionid] = [i].[interactionid]
JOIN [dbo].[five9_call] [fc] WITH(NOLOCK) ON [fc].[companyid] = [i].[companyid] AND [fc].[interactionid] = [i].[interactionid]
JOIN [dbo].[five9_item] [fic] WITH(NOLOCK) ON [fic].[typeid] = 101000000 AND [fic].[itemid] = [fc].[campaignid]
JOIN [dbo].[five9_item] [fis] WITH(NOLOCK) ON [fis].[typeid] = 102000000 AND [fis].[itemid] = [fc].[skillid]
LEFT OUTER JOIN [dbo].[five9_call_agent] [fca] WITH(NOLOCK) ON [fca].[companyid] = [i].[companyid] AND [fca].[interactionid] = [i].[interactionid]
LEFT OUTER JOIN [dbo].[five9_agent] [fa] WITH(NOLOCK) ON [fa].[agentid] = [fca].[agentid]
WHERE 1=1
AND [i].[interactionid] = @sp_interactionid
ORDER BY [i].[interactionid] DESC
";
                }
                #endregion
                #region dvInteractionDetails
                if (dv.ID == "dvInteractionDetails")
                {
                    cmdText = @"
SELECT
TOP (@sp_top)
[i].[companyid]
,[i].[interactionid]
,[fc].[callid] [five9_callid]
,[ia].[callid] [arc_callid]
,[fic].[name] [campaign]
,[fis].[name] [skill]
,[ia].[dispositionname] [arc_disposition]

,[fa].[fullname] [agent_fullname]

,[i].[originator]
,[i].[destinator]

,[fc].[datestart]
,[fc].[dateend]

,[fc].[sessionid]

FROM [dbo].[interactions] [i] WITH(NOLOCK)
LEFT OUTER JOIN [dbo].[interactions_arc] [ia] WITH(NOLOCK) ON [ia].[companyid] = [i].[companyid] AND [ia].[interactionid] = [i].[interactionid]
JOIN [dbo].[five9_call] [fc] WITH(NOLOCK) ON [fc].[companyid] = [i].[companyid] AND [fc].[interactionid] = [i].[interactionid]
JOIN [dbo].[five9_item] [fic] WITH(NOLOCK) ON [fic].[typeid] = 101000000 AND [fic].[itemid] = [fc].[campaignid]
JOIN [dbo].[five9_item] [fis] WITH(NOLOCK) ON [fis].[typeid] = 102000000 AND [fis].[itemid] = [fc].[skillid]
LEFT OUTER JOIN [dbo].[five9_call_agent] [fca] WITH(NOLOCK) ON [fca].[companyid] = [i].[companyid] AND [fca].[interactionid] = [i].[interactionid]
LEFT OUTER JOIN [dbo].[five9_agent] [fa] WITH(NOLOCK) ON [fa].[agentid] = [fca].[agentid]
WHERE 1=1
AND [i].[interactionid] = @sp_interactionid
ORDER BY [i].[interactionid] DESC
";
                }
                #endregion
                cmd.CommandText = cmdText;
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Clear();
                #region SQL Parameters
                cmd.Parameters.Add("@sp_interactionid", SqlDbType.Int).Value = interactionid;
                cmd.Parameters.Add("@sp_top", SqlDbType.Int).Value = 1;
                #endregion SQL Parameters
                #region SQL Processing
                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                ad.Fill(dt);
                dv.DataSource = dt;
                dv.DataBind();
                #endregion SQL Processing

            }
            #endregion SQL Command
        }
        #endregion SQL Connection
    }

    protected void GridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        //Session["EventList_GridView_SelectedIndex"] = null;
        //Session["EventList_GridView_PageIndex"] = null;
        Label8.Text = e.NewPageIndex.ToString();
        gvSearchResults.SelectedIndex = -1;
        gvSearchResults.PageIndex = e.NewPageIndex;
        Search_Data_Query(0, this.Page.User.Identity.Name, gvSearchResults);
    }
    protected void GridView_PageDropDownList_SelectedIndexChanged(Object sender, EventArgs e)
    {

        // Retrieve the pager row.
        GridViewRow pagerRow = gvSearchResults.TopPagerRow;
        // Retrieve the PageDropDownList DropDownList from the bottom pager row.
        DropDownList pageList = (DropDownList)pagerRow.Cells[0].FindControl("PageDropDownList");
        // Set the PageIndex property to display that page selected by the user.
        gvSearchResults.SelectedIndex = -1;
        gvSearchResults.PageIndex = pageList.SelectedIndex;
        Search_Data_Query(0, this.Page.User.Identity.Name, gvSearchResults);

    }
    protected void DetailsView_Clear()
    {
    }
    protected void WriteToLabel(String type, String color, String msg, Label lbl)
    {
        if (lbl != null)
        {
            String spanBlue = "<span style='color: Blue;'>{0}</span>";
            String spanRed = "<span style='color: Blue;'>{0}</span>";
            String spanWhite = "<span style='color: Blue;'>{0}</span>";

            String spanColor = "<span style='color: " + color + ";'>{0}</span>";
            if (type == "add")
            {
                lbl.Text += String.Format(spanColor, msg);
            }
            else if (type == "append")
            {
                lbl.Text = String.Format(spanColor, msg) + lbl.Text;
            }
            else
            {
                lbl.Text = String.Format(spanColor, msg);
            }
        }
    }
    protected void sqlupdated(object sender, SqlDataSourceStatusEventArgs e, SqlDataSourceCommandEventArgs e2)
    {
        //e.AffectedRows;
        //e.Command;
        //e.Exception;
        //e.ExceptionHandled;

        //e2.Cancel
        //e2.Command
    }
    protected void Populate_StateProvinceCountry(DropDownList ddlState, DropDownList ddlProvince, DropDownList ddlCountry)
    {
        DataSet myDs = new DataSet();
        myDs.ReadXml(Server.MapPath(@"StateCountry.xml"));
        if (myDs.Tables.Count > 0)
        {
            for (int x = 0; x <= myDs.Tables.Count - 1; x++)
            {
                if (myDs.Tables[x].TableName == "state" && ddlState != null)
                {
                    ddlState.DataSource = myDs.Tables[x];
                    ddlState.DataValueField = "code";
                    ddlState.DataTextField = "name";
                    ddlState.DataBind();
                }
                else if (myDs.Tables[x].TableName == "province" && ddlProvince != null)
                {
                    ddlProvince.DataSource = myDs.Tables[x];
                    ddlProvince.DataValueField = "code";
                    ddlProvince.DataTextField = "name";
                    ddlProvince.DataBind();
                }
                else if (myDs.Tables[x].TableName == "country" && ddlCountry != null)
                {
                    ddlCountry.DataSource = myDs.Tables[x];
                    ddlCountry.DataValueField = "code";
                    ddlCountry.DataTextField = "name";
                    ddlCountry.DataBind();
                    if (!IsPostBack) { ddlCountry.SelectedIndex = 1; }
                }
            }
        }
        else
        {
            //This is a fatal error.. can not load the State/Province/Country
        }
        myDs.Dispose();
    }
    protected void Error_Catch(Exception ex, String error, Label lbl)
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
    protected void Error_Save(Exception ex, String error)
    {
        string sPath = HttpContext.Current.Request.Url.AbsolutePath;
        string[] strarry = sPath.Split('/');
        int lengh = strarry.Length;
        String spPage = strarry[lengh - 1];
        String spURL = HttpContext.Current.Request.Url.ToString();
        String spQS = HttpContext.Current.Request.Url.Query.ToString();
        if (error == null) { error = "General Error"; }

        DetailsView dv = ErrorView;

        ErrorLog.ErrorLog_Save(ex, dv, "Portal: ARC", error, spPage, spQS, spURL);
    }
    protected string get_Script_URL()
    {
        String urlScript = "";
        urlScript = @"http://localhost:84/script_main.aspx";

        if (Request.Url.ToString().Contains("portalstage"))
        {
            urlScript = @"https://ivrstage.archelpnow.com/script_main.aspx";
        }
        else if (Request.Url.ToString().Contains("portal"))
        {
            urlScript = @"https://ivr.archelpnow.com/script_main.aspx";
        }
        else if (Request.Url.ToString().Contains("192.168"))
        {
            urlScript = @"http://192.168.1.6:84/script_main.aspx";
        }
        return urlScript;
    }
    protected string get_URL_DNIS(String dnis)
    {
        String urlDNIS = "";
        #region SQL Connection
        using (SqlConnection con = new SqlConnection(sqlStrARC))
        {
            ghFunctions.Donation_Open_Database(con);
            #region SQL Command
            using (SqlCommand cmd = new SqlCommand("", con))
            {
                #region Build cmdText
                String cmdText = "";
                cmdText += @"
SELECT
TOP 1
--CASE LEN(@sp_dnis) WHEN 4 THEN [d].[dnis] ELSE [d].[line] END [number]
CASE
	WHEN LEN([d].[line]) = 10 THEN [d].[line]
	ELSE [d].[dnis]
END [line]
FROM [dbo].[dnis] [d] WITH(NOLOCK)
WHERE 1=1
-- AND [d].[dnis] = @sp_dnis
AND CASE LEN(@sp_dnis) WHEN 4 THEN [d].[dnis] ELSE [d].[line] END = @sp_dnis
                            ";
                cmdText += "\r";
                #endregion Build cmdText
                #region SQL Command Config
                cmd.CommandTimeout = 600;
                cmd.CommandText = cmdText;
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Clear();
                #endregion SQL Command Config
                #region SQL Command Parameters
                cmd.Parameters.Add("@sp_dnis", SqlDbType.VarChar, 10).Value = dnis;
                #endregion SQL Command Parameters
                #region SQL Command Processing
                var cmdScalar = cmd.ExecuteScalar();
                if (cmdScalar != null && cmdScalar.ToString() != "")
                {
                    urlDNIS = cmdScalar.ToString();
                }
                #endregion SQL Command Processing
            }
            #endregion SQL Command
            if (urlDNIS == "")
            {
                #region SQL Command
                using (SqlCommand cmd = new SqlCommand("", con))
                {
                    #region Build cmdText
                    String cmdText = "";
                    cmdText += @"
SELECT
TOP 1
--CASE LEN(@sp_dnis) WHEN 4 THEN [d].[dnis] ELSE [d].[line] END [number]
CASE
	WHEN LEN([d].[line]) = 10 THEN [d].[line]
	ELSE [d].[dnis]
END [line]
FROM [dbo].[dnis] [d] WITH(NOLOCK)
WHERE 1=1
AND RIGHT([d].[phoneNumber],4) = @sp_dnis
                            ";
                    cmdText += "\r";
                    #endregion Build cmdText
                    #region SQL Command Config
                    cmd.CommandTimeout = 600;
                    cmd.CommandText = cmdText;
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Clear();
                    #endregion SQL Command Config
                    #region SQL Command Parameters
                    cmd.Parameters.Add("@sp_dnis", SqlDbType.VarChar, 10).Value = dnis;
                    #endregion SQL Command Parameters
                    #region SQL Command Processing
                    var cmdScalar = cmd.ExecuteScalar();
                    if (cmdScalar != null && cmdScalar.ToString() != "")
                    {
                        urlDNIS = cmdScalar.ToString();
                    }
                    #endregion SQL Command Processing
                }
                #endregion SQL Command
            }
            if (urlDNIS == "")
            {
                urlDNIS = dnis;
            }

        }
        #endregion SQL Connection

        return urlDNIS;
    }
    protected string get_Script_DNIS(String dnis)
    {
        String scriptDNIS = "";
        #region SQL Connection
        using (SqlConnection con = new SqlConnection(sqlStrARC))
        {
            ghFunctions.Donation_Open_Database(con);
            #region SQL Command
            using (SqlCommand cmd = new SqlCommand("", con))
            {
                #region Build cmdText
                String cmdText = "";
                cmdText += @"
SELECT
TOP 1
[d].[company] + ' [' + [d].[line] + '] ' + CASE WHEN [d].[languageid] = 0 THEN 'English' ELSE 'Spanish' END + ' - [' + [d].[dnis] + ']' [dnis]
FROM [dbo].[dnis] [d] WITH(NOLOCK)
WHERE 1=1
-- AND [d].[dnis] = @sp_dnis
AND CASE LEN(@sp_dnis) WHEN 4 THEN [d].[dnis] ELSE [d].[line] END = @sp_dnis
                            ";
                cmdText += "\r";
                #endregion Build cmdText
                #region SQL Command Config
                cmd.CommandTimeout = 600;
                cmd.CommandText = cmdText;
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Clear();
                #endregion SQL Command Config
                #region SQL Command Parameters
                cmd.Parameters.Add("@sp_dnis", SqlDbType.VarChar, 10).Value = dnis;
                #endregion SQL Command Parameters
                #region SQL Command Processing
                var cmdScalar = cmd.ExecuteScalar();
                if (cmdScalar != null && cmdScalar.ToString() != "")
                {
                    scriptDNIS = cmdScalar.ToString();
                }
                #endregion SQL Command Processing
            }
            #endregion SQL Command
            if (scriptDNIS == "")
            {
                #region SQL Command
                using (SqlCommand cmd = new SqlCommand("", con))
                {
                    #region Build cmdText
                    String cmdText = "";
                    cmdText += @"
SELECT
TOP 1
[d].[company] + ' [' + [d].[line] + '] ' + CASE WHEN [d].[languageid] = 0 THEN 'English' ELSE 'Spanish' END + ' - [' + [d].[dnis] + ']' [dnis]
FROM [dbo].[dnis] [d] WITH(NOLOCK)
WHERE 1=1
AND CASE LEN(@sp_dnis) WHEN 4 THEN RIGHT([d].[phoneNumber],4) ELSE [d].[phoneNumber] END = @sp_dnis
                            ";
                    cmdText += "\r";
                    #endregion Build cmdText
                    #region SQL Command Config
                    cmd.CommandTimeout = 600;
                    cmd.CommandText = cmdText;
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Clear();
                    #endregion SQL Command Config
                    #region SQL Command Parameters
                    cmd.Parameters.Add("@sp_dnis", SqlDbType.VarChar, 10).Value = dnis;
                    #endregion SQL Command Parameters
                    #region SQL Command Processing
                    var cmdScalar = cmd.ExecuteScalar();
                    if (cmdScalar != null && cmdScalar.ToString() != "")
                    {
                        scriptDNIS = cmdScalar.ToString();
                    }
                    #endregion SQL Command Processing
                }
                #endregion SQL Command
            }
            if (scriptDNIS == "")
            {
                scriptDNIS = dnis;
            }

        }
        #endregion SQL Connection

        return scriptDNIS;
    }
    protected void identity_get_userid()
    {
        // Get the logged in users userid
        // This should be retrieved during the login process
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
                    cmdText = "[portal_user].[dbo].[user_get_userid]";
                    cmd.CommandText = cmdText;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Clear();
                    #region SQL Parameters
                    //cmd.Parameters.Add(new SqlParameter("@UserID", UserID));
                    cmd.Parameters.Add(new SqlParameter("@sp_username", this.Page.User.Identity.Name));
                    #endregion SQL Parameters
                    print_sql(cmd); // Will print for Admin in Local
                    #region SQL Processing
                    using (SqlDataReader sqlRdr = cmd.ExecuteReader())
                    {
                        if (sqlRdr.HasRows)
                        {
                            while (sqlRdr.Read())
                            {
                                Session["userid"] = sqlRdr["userid"].ToString();
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
            Error_Save(ex, "User Get UserID");
        }
    }
    protected void print_sql(SqlCommand cmd)
    {
        print_sql(cmd, sqlPrint, "append");
    }
    protected void print_sql(SqlCommand cmd, Label lblPrint, String type)
    {
        #region Print SQL
        if (Page.User.IsInRole("System Administrator") == true && Connection.GetConnectionType() == "Local")
        {
            String sqlToText = "";
            sqlToText += cmd.CommandText.ToString().Replace("\n", "<br />");
            foreach (SqlParameter p in cmd.Parameters)
            {
                sqlToText += "<br />" + p.ParameterName + " = " + p.Value.ToString() + " [" + p.DbType.ToString() + "]";
            }
            // new == we make this a new write | else we append
            if (type == "new") { lblPrint.Text = ""; }
            //lblPrint.Text = "<hr />" + sqlToText + "<hr />" + lblPrint.Text;
            lblPrint.Text = String.Format("<hr />Print: {0}<br />{1}{2}", DateTime.UtcNow.ToString(), sqlToText, lblPrint.Text);
        }
        #endregion Print SQL
    }
}
