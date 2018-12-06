using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;

public partial class ActionLog : System.Web.UI.Page
{
    private String sqlStr = Connection.GetConnectionString("PS_Production", ""); // PS_Production | PS_Stage
    protected void Page_PreInit(Object sender, EventArgs e)
    {
        Master.PageTitle = "Action Log";
        this.Title = String.Format("{0}: {1}", Master.MyTitle, Page.Title);
        if (Connection.GetDBMode() == "Stage")
        {
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
            //lookup_zipcode(); -- Why is this here?
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
            GV_Data_Query(UserID, this.Page.User.Identity.Name, gvSearchResults);
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
        GV_Data_Query(UserID, this.Page.User.Identity.Name, gvSearchResults);
        GridViewExportUtil.Export("IRV-Call-Records.xls", this.gvSearchResults);
    }
    protected void GV_Data_Query(Int32 UserID, String UserName, GridView gv)
    {
        // Change to this section should be duplicated to this section: GV_Data_Query_Counts
        #region SQL Connection
        try
        {
            using (SqlConnection con = new SqlConnection(sqlStr))
            {
                #region SQL Command
                using (SqlCommand cmd = new SqlCommand("", con))
                {
                    if (con.State == ConnectionState.Closed) { con.Open(); }
                    cmd.CommandTimeout = 600;
                    #region SQL String Builder
                    string sqlBuilder = "";
                    sqlBuilder += "" + "\n";
                    sqlBuilder += "SELECT" + "\n";
                    sqlBuilder += "TOP (@sp_top)" + "\n";
                    sqlBuilder += "[al].[logid]" + "\n";
                    sqlBuilder += ",[al].[sourceid]" + "\n";
                    sqlBuilder += ",[al].[recordid]" + "\n";
                    sqlBuilder += ",[al].[calldate]" + "\n";
                    sqlBuilder += ",[al].[calltime]" + "\n";
                    sqlBuilder += ",[al].[ani]" + "\n";
                    sqlBuilder += ",[al].[actor]" + "\n";
                    sqlBuilder += ",[al].[action]" + "\n";
                    sqlBuilder += ",[al].[createdate]" + "\n";
                    sqlBuilder += ",CASE [al].[action]" + "\n";
                    sqlBuilder += "	WHEN 100098 THEN 'Discard'" + "\n";
                    sqlBuilder += "	WHEN 101098 THEN 'Undo Discard'" + "\n";
                    sqlBuilder += "	WHEN 100099 THEN 'Clear'" + "\n";
                    sqlBuilder += "	WHEN 101099 THEN 'Undo Clear'" + "\n";
                    sqlBuilder += "	ELSE 'Other'" + "\n";
                    sqlBuilder += "END [action_name]" + "\n";
                    sqlBuilder += ",LTRIM(RTRIM([u].[firstname] + ' ' + [u].[lastname])) [actor_name]" + "\n";
                    sqlBuilder += "FROM [dbo].[ivr_record_action_log] [al] WITH(NOLOCK)" + "\n";
                    sqlBuilder += "LEFT OUTER JOIN [66.135.60.195].[portal_user].[dbo].[user] [u] WITH(NOLOCK) ON [u].[userid] = [al].[actor]" + "\n";
                    sqlBuilder += "WHERE 1=1" + "\n";
                    sqlBuilder += "AND [al].[createdate] BETWEEN @sp_datestart AND @sp_dateend" + "\n";
                    sqlBuilder += "ORDER BY [al].[logid] DESC" + "\n";
                    sqlBuilder += "" + "\n";
                    sqlBuilder += "" + "\n";
                    #endregion SQL String Builder
                    String cmdText = "";
                    cmdText = sqlBuilder;// "[dbo].[sp_ivr_search_get]";
                    cmd.CommandText = cmdText;
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Clear();
                    #region SQL Parameters
                    //dtStartDate
                    DateTime startdate = DateTime.UtcNow.AddDays(-5);
                    DateTime enddate = DateTime.UtcNow.AddDays(-0);
                    if (dtStartDate.Text.Length > 0 && dtStartTime.Text.Length > 0)
                    {
                        startdate = DateTime.Parse(dtStartDate.Text.Trim() + " " + dtStartTime.Text.Trim());
                    }
                    else
                    {
                        dtStartDate.Text = startdate.ToString("MM/dd/yyyy");
                    }
                    if (dtEndDate.Text.Length > 0 && dtEndTime.Text.Length > 0)
                    {
                        enddate = DateTime.Parse(dtEndDate.Text.Trim() + " " + dtEndTime.Text.Trim());
                    }
                    else
                    {
                        dtEndDate.Text = enddate.ToString("MM/dd/yyyy");
                    }
                    cmd.Parameters.Add(new SqlParameter("@sp_top", Convert.ToInt32(ddlTop.SelectedValue.ToString())));
                    cmd.Parameters.Add(new SqlParameter("@sp_datestart", startdate));
                    cmd.Parameters.Add(new SqlParameter("@sp_dateend", enddate));
                    #endregion SQL Parameters
                    print_sql(cmd); // Will print for Admin in Local
                    #region SQL Processing
                    SqlDataAdapter ad = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    ad.Fill(dt);
                    gv.DataSource = dt; //dv_ivr_file_vc.DataSource = dt;
                    gv.DataBind(); //dv_ivr_file_vc.DataBind();
                    //dtlLabel.Text += "<br />" + gv.ID;
                    #endregion SQL Processing
                }
                #endregion SQL Command
            }
        }
        catch (Exception ex)
        {
            Error_Save(ex, "GV_Data_Query");
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
    protected void GridView_IndexChanged(object sender, EventArgs e)
    {
        dtlLabel.Text = "Feature not enabled: " + DateTime.Now.ToString("HH:mm:ss");
    }
    protected void GridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        //Session["EventList_GridView_SelectedIndex"] = null;
        //Session["EventList_GridView_PageIndex"] = null;
        Label8.Text = e.NewPageIndex.ToString();
        gvSearchResults.SelectedIndex = -1;
        gvSearchResults.PageIndex = e.NewPageIndex;
        GV_Data_Query(0, this.Page.User.Identity.Name, gvSearchResults);
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
        GV_Data_Query(0, this.Page.User.Identity.Name, gvSearchResults);

    }
    protected void DetailsView_Clear()
    {
    }
    /// <summary>
    /// Testing
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <param name="e2"></param>
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
    protected void lookup_zipcode()
    {
        // Get the logged in users userid
        // This should be retrieved during the login process
        try
        {
            #region SQL Connection
            using (SqlConnection con = new SqlConnection(sqlStr))
            {
                #region SqlCommand cmd
                using (SqlCommand cmd = new SqlCommand("", con))
                {
                    #region Populate the SQL Command
                    string sqlBuilder = "";
                    sqlBuilder += "SELECT" + "\n";
                    sqlBuilder += "TOP 1" + "\n";
                    sqlBuilder += "[zip]" + "\n";
                    sqlBuilder += ",[latitude]" + "\n";
                    sqlBuilder += ",[longitude]" + "\n";
                    sqlBuilder += ",[city]" + "\n";
                    sqlBuilder += ",[state]" + "\n";
                    sqlBuilder += ",[abbr]" + "\n";
                    sqlBuilder += "FROM [dbo].[zipData]" + "\n";
                    sqlBuilder += "WHERE [zip] = @sp_postalcode" + "\n";
                    sqlBuilder += "";
                    cmd.CommandTimeout = 600;
                    cmd.CommandText = sqlBuilder;
                    cmd.CommandType = CommandType.Text;
                    #endregion Populate the SQL Command
                    #region Populate the SQL Params
                    cmd.Parameters.Add(new SqlParameter("@sp_postalcode", "10459"));
                    string cmdText = "\n" + cmd.CommandText;
                    bool cmdFirst = true;
                    foreach (SqlParameter param in cmd.Parameters)
                    {
                        cmdText += "\n" + ((cmdFirst) ? "" : ",") + param.ParameterName + " = " + ((param.Value != null) ? "'" + param.Value.ToString() + "'" : "default");
                        cmdFirst = false;
                    }
                    #endregion Populate the SQL Params
                    #region Process SQL Command - Try
                    Label8.Text = "Results:";
                    try
                    {
                        Label8.Text += "|Try";
                        if (con.State == ConnectionState.Closed) { con.Open(); }
                        using (SqlDataReader sqlRdr = cmd.ExecuteReader())
                        {
                            Label8.Text += "|Reader";
                            if (sqlRdr.HasRows)
                            {
                                Label8.Text += "|Rows";
                                while (sqlRdr.Read())
                                {
                                    Label8.Text += "<br />" + sqlRdr["city"].ToString();
                                    Label8.Text += "<br />" + sqlRdr["abbr"].ToString();
                                }
                            }
                            else
                            {
                                Label8.Text += "|Blank";
                                //arcNewID = 0;
                            }
                        }
                    }
                    #endregion Process SQL Command - Try
                    #region Process SQL Command - Catch
                    catch (Exception ex)
                    {
                    }
                    #endregion Process SQL Command - Catch
                }
                #endregion SqlCommand cmd
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
