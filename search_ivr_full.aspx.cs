using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;

public partial class search_ivr_full : System.Web.UI.Page
{
    private Boolean isAdmin = false;
    protected void Page_PreInit(Object sender, EventArgs e)
    {
        Master.PageTitle = "IVR Search Full";
        this.Title = String.Format("{0}: {1}", Master.MyTitle, Page.Title);
        isAdmin = ghUser.identity_is_admin();
    }
    private String sqlStr = Connection.GetConnectionString("Default", ""); // Controlled by Web Config ARC_Live | ARC_Stage
    protected void Page_Load(object sender, EventArgs e)
    {
        //SqlDataSource1.ConnectionString = Connection.GetConnectionString("Default", "");
        //SqlDataSource2.ConnectionString = Connection.GetConnectionString("Default", "");
        if (!IsPostBack)
        {
            //GridView_Data(0, this.Page.User.Identity.Name, GridView1);

            if (Session["userid"] == null)
            {
                //this.Page.User.Identity.Name
                ghUser.identity_get_userid();
            }
            //Label8.Text += "<br />" + this.Page.User.Identity.Name;
            GridView_Refresh();

        }
        // Multi Select
        // http://www.erichynds.com/jquery/jquery-ui-multiselect-widget/
        // http://www.erichynds.com/examples/jquery-ui-multiselect-widget/demos/#position

        // http://www.abeautifulsite.net/blog/2008/04/jquery-multiselect/
        // 

    }
    protected void GridView_Refresh(object sender, EventArgs e)
    {
        GridView_Refresh();
    }
    protected void GridView_Refresh()
    {
        try
        {
            Int32 UserID = Convert.ToInt32(Session["userid"].ToString());
            //Label8.Text = "Test1";
            Error_General.Text = "";
            GridView1.SelectedIndex = -1;
            GridView_Data(UserID, this.Page.User.Identity.Name, GridView1);
            gvTotalCounts_Data(UserID, this.Page.User.Identity.Name, gvTotalCounts);
            //Label8.Text += "<br />Test2";
            DetailsView_Clear();

            btnClearRecord.Visible = false;
            btnDiscardRecord.Visible = false;
            btnClearRecordConfirm.Enabled = true;
            btnClearRecordCancel.Enabled = true;
            ClearRecord.Visible = false;
            lblClearRecordConfirm.Text = "";

            btnDiscardRecord.Visible = false;
            btnDiscardRecordConfirm.Enabled = true;
            btnDiscardRecordCancel.Enabled = true;
            DiscardRecord.Visible = false;
            lblDiscardRecordConfirm.Text = "";


        }
        catch (Exception ex)
        {
            Label8.Text = "Error processing the request";
            Error_Save(ex, "GridView_Refresh");
        }
    }
    protected void GridView1_Export_Excel(object sender, EventArgs e)
    {
    }
    protected void GridView_Data(Int32 UserID, String UserName, GridView gv)
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
                cmdText = "[dbo].[sp_ivr_search_get]";
                //sp_error_log_sql_list
                cmd.CommandText = cmdText;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                #region SQL Parameters
                if (CallID.Text.Trim().Length > 0)
                {
                    cmd.Parameters.Add(new SqlParameter("@sp_callid", CallID.Text));
                    string calldate = DateTime.Parse(CallDate.Text).ToString("yyyyMMdd");
                    cmd.Parameters.Add(new SqlParameter("@sp_calldate", calldate));
                    rpElapsed.Text = calldate;
                }
                else if (CallANI.Text.Trim().Length > 0)
                {
                    cmd.Parameters.Add(new SqlParameter("@sp_callani", CallANI.Text));
                    string calldate = DateTime.Parse(CallDate.Text).ToString("yyyyMMdd");
                    cmd.Parameters.Add(new SqlParameter("@sp_calldate", calldate));
                    rpElapsed.Text = calldate;
                }
                else
                {
                }
                
                #endregion SQL Parameters
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
        #endregion SQL Connection
    }
    protected void gvTotalCounts_Data(Int32 UserID, String UserName, GridView gv)
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
                cmdText = "[dbo].[ivr_processing_records_get_count]";
                //sp_error_log_sql_list
                cmd.CommandText = cmdText;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                #region SQL Parameters
                #endregion SQL Parameters
                #region SQL Processing
                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                ad.Fill(dt);
                gv.DataSource = dt;
                gv.DataBind();
                #endregion SQL Processing
            }
            #endregion SQL Command
        }
        #endregion SQL Connection
    }
    protected void GridView1_DataBound(Object sender, EventArgs e)
    {
        Label8.Text = " Records: [" + GridView1.Rows.Count.ToString() + "]";
        if (GridView1.PageCount > 0)
        {
            Label8.Text += " - Pages: [" + GridView1.PageCount.ToString() + "]";
            Label8.Text += " - Approx Total: [" + (GridView1.PageCount * GridView1.Rows.Count).ToString() + "]";
            // Retrieve the pager row.
            //GridViewRow pagerRow = GridView1.BottomPagerRow;
            GridViewRow pagerRow = GridView1.TopPagerRow;
            // Retrieve the DropDownList and Label controls from the row.
            DropDownList pageList = (DropDownList)pagerRow.Cells[0].FindControl("PageDropDownList");
            Label pageLabel = (Label)pagerRow.Cells[0].FindControl("CurrentPageLabel");
            if (pageList != null)
            {
                // Create the values for the DropDownList control based on 
                // the  total number of pages required to display the data
                // source.
                for (int i = 0; i < GridView1.PageCount; i++)
                {
                    // Create a ListItem object to represent a page.
                    int pageNumber = i + 1;
                    ListItem item = new ListItem(pageNumber.ToString());
                    // If the ListItem object matches the currently selected
                    // page, flag the ListItem object as being selected. Because
                    // the DropDownList control is recreated each time the pager
                    // row gets created, this will persist the selected item in
                    // the DropDownList control.   
                    if (i == GridView1.PageIndex)
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
                int currentPage = GridView1.PageIndex + 1;
                // Update the Label control with the current page information.
                pageLabel.Text = "Page " + currentPage.ToString() +
                  " of " + GridView1.PageCount.ToString();
            }
            if (GridView1.PageIndex > 0)
            {
                pagerRow.Cells[0].FindControl("lnkPrevPage").Visible = true;
                pagerRow.Cells[0].FindControl("lnkFirstPage").Visible = true;
            }
            else
            {
                pagerRow.Cells[0].FindControl("lnkPrevPage").Visible = false;
                pagerRow.Cells[0].FindControl("lnkFirstPage").Visible = false;
            }

            if (GridView1.PageCount != (GridView1.PageIndex + 1))
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
    protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
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

            e.Row.Attributes["onclick"] = Page.ClientScript.GetPostBackClientHyperlink(this.GridView1, "Select$" + e.Row.RowIndex);
        }
    }
    protected void GridView1_IndexChanged_Old(object sender, EventArgs e)
    {
        dtlLabel.Text = "Feature not enabled: " + DateTime.Now.ToString("HH:mm:ss");
    }
    protected void GridView1_IndexChanged(object sender, EventArgs e)
    {
        dtlLabel.Text = GridView1.SelectedIndex.ToString();
        //dtlLabel.Text += " [" + GridView1.SelectedDataKey.Values[0].ToString();
        //dtlLabel.Text += " [" + GridView1.SelectedDataKey.Values[1].ToString();
        try
        {
            Int32 sourceid = Convert.ToInt32(GridView1.SelectedDataKey.Values[0].ToString());
            Int32 recordid = Convert.ToInt32(GridView1.SelectedDataKey.Values[1].ToString());
            String calldate = GridView1.SelectedDataKey.Values[2].ToString();
            String calltime = GridView1.SelectedDataKey.Values[3].ToString();
            String ani = GridView1.SelectedDataKey.Values[4].ToString();
            DetailsView_Data(sourceid, recordid, calldate, calltime, ani, dv_ivr_file_vc);
            DetailsView_Data(sourceid, recordid, calldate, calltime, ani, dv_ivr_file_cc);
            DetailsView_Data(sourceid, recordid, calldate, calltime, ani, dv_ivr_file_rn);
            DetailsView_Data(sourceid, recordid, calldate, calltime, ani, dv_ivr_file_op);
            DetailsView_Data(sourceid, recordid, calldate, calltime, ani, dv_ivr_file_ct);

            DetailsView_Data(sourceid, recordid, calldate, calltime, ani, dv_ivr_file_info);
            if (CallID.Text.Trim().Length > 0)
            {
                //dv_ivr_file_info.Visible = true;
                //DetailsView_Data(sourceid, recordid, calldate, calltime, ani, dv_ivr_file_info);
            }
            else
            {
                //dv_ivr_file_info.Visible = false;
            }
            
            btnClearRecord.Visible = true;
            if (GridView1.SelectedRow.Cells[7].Text.ToString() == "Cleared")
            {
                btnClearRecord.Enabled = false;
            }
            else { btnClearRecord.Enabled = true; }
            btnClearRecordConfirm.Enabled = true;
            btnClearRecordCancel.Enabled = true;
            ClearRecord.Visible = false;
            lblClearRecordConfirm.Text = "";

            btnDiscardRecord.Visible = true;
            btnDiscardRecordConfirm.Enabled = true;
            btnDiscardRecordCancel.Enabled = true;
            DiscardRecord.Visible = false;
            lblDiscardRecordConfirm.Text = "";

            if (CallID.Text.Trim().Length > 0 || CallANI.Text.Trim().Length > 0)
            {
                btnClearRecord.Enabled = false;
                btnDiscardRecord.Enabled = false;
            }
            else { btnDiscardRecord.Enabled = true; }
            Label16.Text += "<br />Good";

        }
        catch (Exception ex)
        {
            Label16.Text += "<br />Error";
            Error_Save(ex, "DetailsView Data Error");
            btnClearRecord.Visible = false;
            btnClearRecordConfirm.Enabled = true;
            btnClearRecordCancel.Enabled = true;
            ClearRecord.Visible = false;
            lblClearRecordConfirm.Text = "";

            btnDiscardRecord.Visible = false;
            btnDiscardRecordConfirm.Enabled = true;
            btnDiscardRecordCancel.Enabled = true;
            DiscardRecord.Visible = false;
            lblDiscardRecordConfirm.Text = "";
        }
    }
    protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        //Session["EventList_GridView_SelectedIndex"] = null;
        //Session["EventList_GridView_PageIndex"] = null;
        Label8.Text = e.NewPageIndex.ToString();
        GridView1.SelectedIndex = -1;
        GridView1.PageIndex = e.NewPageIndex;
        GridView_Data(0, this.Page.User.Identity.Name, GridView1);
    }
    protected void GridView1_PageDropDownList_SelectedIndexChanged(Object sender, EventArgs e)
    {

        // Retrieve the pager row.
        GridViewRow pagerRow = GridView1.TopPagerRow;
        // Retrieve the PageDropDownList DropDownList from the bottom pager row.
        DropDownList pageList = (DropDownList)pagerRow.Cells[0].FindControl("PageDropDownList");
        // Set the PageIndex property to display that page selected by the user.
        GridView1.SelectedIndex = -1;
        GridView1.PageIndex = pageList.SelectedIndex;
        GridView_Data(0, this.Page.User.Identity.Name, GridView1);

    }
    protected void DetailsView_Clear()
    {
        if (dv_ivr_file_vc.Rows.Count > 0) dv_ivr_file_vc.DataBind();
        if (dv_ivr_file_cc.Rows.Count > 0) dv_ivr_file_cc.DataBind();
        if (dv_ivr_file_rn.Rows.Count > 0) dv_ivr_file_rn.DataBind();
        if (dv_ivr_file_op.Rows.Count > 0) dv_ivr_file_op.DataBind();
        if (dv_ivr_file_ct.Rows.Count > 0) dv_ivr_file_ct.DataBind();
    }
    /// <summary>
    /// Control how the DetailsView is handled for each individual section
    /// This providers a much higher level of validation and security
    /// Also allows for full customization of what the update command does
    /// http://www.c-sharpcorner.com/uploadfile/raj1979/using-Asp-Net-detailsview-control-without-sqldatasource/
    /// </summary>
    /// <param name="UserID"></param>
    #region Details View Handling
    /// <summary>
    /// Get the data
    /// </summary>
    /// <param name="UserID"></param>
    protected void DetailsView_Data(Int32 sourceid, Int32 recordid, String calldate, String calltime, String ani, DetailsView dv)
    {
        Label16.Text = String.Format("{0}|{1}|{2}|{3}|{4}"
            , sourceid
            , recordid
            , calldate
            , calltime
            , ani
            );
           
        #region SQL Connection
        using (SqlConnection con = new SqlConnection(sqlStr))
        {
            #region SQL Command
            using (SqlCommand cmd = new SqlCommand("", con))
            {
                if (con.State == ConnectionState.Closed) { con.Open(); }
                cmd.CommandTimeout = 600;
                String cmdText = "";
                if (dv.ID == "dv_ivr_file_vc") { cmdText = "[dbo].[sp_ivr_search_get_vc]"; }
                if (dv.ID == "dv_ivr_file_cc") { cmdText = "[dbo].[sp_ivr_search_get_cc]"; }
                if (dv.ID == "dv_ivr_file_rn") { cmdText = "[dbo].[sp_ivr_search_get_rn]"; }
                if (dv.ID == "dv_ivr_file_op") { cmdText = "[dbo].[sp_ivr_search_get_op]"; }
                if (dv.ID == "dv_ivr_file_ct") { cmdText = "[dbo].[sp_ivr_search_get_ct]"; }
                if (dv.ID == "dv_ivr_file_info") { cmdText = "[dbo].[sp_ivr_search_get_info]"; }
                
                cmd.CommandText = cmdText;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                #region SQL Parameters
                cmd.Parameters.Add(new SqlParameter("@sp_sourceid", sourceid));
                cmd.Parameters.Add(new SqlParameter("@sp_recordid", recordid));
                cmd.Parameters.Add(new SqlParameter("@sp_calldate", calldate));
                cmd.Parameters.Add(new SqlParameter("@sp_calltime", calltime));
                cmd.Parameters.Add(new SqlParameter("@sp_ani", ani));
                #endregion SQL Parameters
                #region SQL Processing
                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                ad.Fill(dt);
                dv.DataSource = dt; //dv_ivr_file_vc.DataSource = dt;
                dv.DataBind(); //dv_ivr_file_vc.DataBind();
                //if (dt.Rows.Count > 0)
                //{
                //    dv.Visible = true;
                //}
                //else
                //{
                //    dv.DataSource = null;
                //    dv.DataBind();
                //    dv.Visible = false;
                //}
                
                dtlLabel.Text += "<br />" + dv.ID;
                #endregion SQL Processing
            }
            #endregion SQL Command
        }
        #endregion SQL Connection
    }
    /// <summary>
    /// 
    /// </summary>
    protected void DetailsView_DataBound(object sender, EventArgs e)
    {
        #region DataBound Action
        DetailsView dv = (DetailsView)sender;
        #region DataBound Action for dv_ivr_file_vc
        if (dv.ID == "dv_ivr_file_vc")
        {
            // Nothing
        }
        #endregion DataBound Action for dv_ivr_file_vc
        #region DataBound Action for dv_ivr_file_cc
        else if (dv.ID == "dv_ivr_file_cc")
        {
            // Nothing
        }
        #endregion DataBound Action for dv_ivr_file_cc
        #region DataBound Action for dv_ivr_file_rn
        else if (dv.ID == "dv_ivr_file_rn")
        {
            // Nothing
        }
        #endregion DataBound Action for dv_ivr_file_rn
        #region DataBound Action for dv_ivr_file_op
        else if (dv.ID == "dv_ivr_file_op")
        {
            // Populate the State/Country Drop Down
            #region State/Country Populate if Edit Mode
            if (dv_ivr_file_op.CurrentMode == DetailsViewMode.Edit)
            {
                Label lblState = (Label)dv_ivr_file_op.FindControl("State");

                DropDownList dvState = (DropDownList)dv_ivr_file_op.FindControl("ddlState");
                DropDownList dvCountry = (DropDownList)dv_ivr_file_op.FindControl("ddlCountry");
                if (dvState != null && dvCountry != null)
                {
                    //dtlLabel.Text = "Found DDL 1";
                    if (lblState != null)
                    {
                        //dtlLabel.Text += " [" + lblState.Text + "]";
                    }
                    try
                    {
                        Populate_StateProvinceCountry(dvState, null, dvCountry);
                    }
                    catch (Exception ex)
                    {
                        Error_Save(ex, "dv_ivr_file_op - DDL State/Country Populate Error");
                    }
                }
                else if (dvState != null)
                {
                    //dtlLabel.Text = "Found DDL 2";
                    try
                    {
                        Populate_StateProvinceCountry(dvState, null, null);
                    }
                    catch (Exception ex)
                    {
                        Error_Save(ex, "dv_ivr_file_op - DDL State Populate Error");
                    }
                }
                else if (dvCountry != null)
                {
                    //dtlLabel.Text = "Found DDL 3";
                    try
                    {
                        Populate_StateProvinceCountry(null, null, dvCountry);
                    }
                    catch (Exception ex)
                    {
                        Error_Save(ex, "dv_ivr_file_op - DDL Country Populate Error");
                    }
                }
                else
                {
                    //dtlLabel.Text = "Did not find DDL";
                    #region DeBug Code
                    //dtlLabel.Text += " [" + dv_ivr_file_op.Rows.Count.ToString() + "]";
                    //foreach (DetailsViewRow dvr in dv_ivr_file_op.Rows)
                    //{
                    //    dtlLabel.Text += "<br />" + dvr.Cells.Count.ToString();
                    //    dtlLabel.Text += " - " + dvr.Cells[0].Text.ToString();
                    //    dtlLabel.Text += " - " + dvr.Cells[0].Controls.Count.ToString();
                    //    dtlLabel.Text += " - " + dvr.Cells[1].Controls.Count.ToString();
                    //    //dtlLabel.Text += " - " + dvr.
                    //    foreach (Control ctl in dvr.Cells[1].Controls)
                    //    {
                    //        dtlLabel.Text += "<br />---" + ctl.ClientID.ToString();
                    //    }

                    //    //dvr.TemplateControl.FindControl
                    //    DropDownList dvState2 = (DropDownList)dvr.Cells[1].TemplateControl.FindControl("ddlState");
                    //    if (dvState2 != null)
                    //    {
                    //        //dtlLabel.Text = "Found DDL";
                    //        dtlLabel.Text += " [" + dvr.ID.ToString() + "]";
                    //        break;
                    //    }
                    //} 
                    #endregion
                }
                //dtlLabel.Text += " finished searching";

            }
            #endregion
            else
            {
                ////dtlLabel.Text = "Other Mode";
            }

        }
        #endregion DataBound Action for dv_ivr_file_op
        #region DataBound Action for dv_ivr_file_ct
        else if (dv.ID == "dv_ivr_file_ct")
        {
        }
        #endregion DataBound Action for dv_ivr_file_ct
        #endregion DataBound Action
    }
    protected void DetailsView_ItemCommand(object sender, DetailsViewCommandEventArgs e)
    {
        #region ItemCommand Action
        if (e.CommandName == "Clear")
        {
            GridView1.SelectedIndex = -1;
            dv_ivr_file_vc.DataBind();
            dv_ivr_file_cc.DataBind();
            dv_ivr_file_rn.DataBind();
            dv_ivr_file_op.DataBind();
            dv_ivr_file_ct.DataBind();
        }
        if (e.CommandName == "Update")
        {
            DetailsView dv = (DetailsView)sender;
            WriteToLabel("new", "Blue", dv.ID + " Update Processed [" + DateTime.Now.ToString("HH:mm:ss") + "]<br />", dtlLabel);
        }
        //dtlLabel.Text = e.CommandName;
        #endregion ItemCommand Action
    }
    protected void DetailsView_ModeChanging(object sender, DetailsViewModeEventArgs e)
    {
        #region ModeChanging Action
        DetailsView dv = (DetailsView)sender;
        dv.ChangeMode(e.NewMode);
        try
        {
            Int32 callid = Convert.ToInt32(GridView1.SelectedDataKey.Values[0].ToString());
            Int32 donorid = Convert.ToInt32(GridView1.SelectedDataKey.Values[1].ToString());
            //DetailsView_Data(callid, donorid, dv);
        }
        catch (Exception ex)
        {
            Error_Save(ex, "DetailsView_ModeChanging");
        }
        if (e.NewMode == DetailsViewMode.Edit)
        {
            dv.AllowPaging = false;
        }
        else
        {
            dv.AllowPaging = true;
        }
        #endregion ModeChanging Action
    }
    /// <summary>
    /// Here we perform the update action
    /// There is some slight validation, all done in the backend code
    /// If we update successfully, we switch the DV to ReadOnly mode
    /// If we fail to update, we leave the data as is and give the user an error
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void DetailsView_ItemUpdating2(object sender, DetailsViewUpdateEventArgs e)
    {
        dtlLabel.Text = "DetailsView_ItemUpdating";
        DetailsView dv = (DetailsView)sender;
        dv.ChangeMode(DetailsViewMode.ReadOnly);
        Int32 callid = Convert.ToInt32(GridView1.SelectedDataKey.Values[0].ToString());
        Int32 donorid = Convert.ToInt32(GridView1.SelectedDataKey.Values[1].ToString());
        //DetailsView_Data(callid, donorid, dv);
    }
    protected void DetailsView_ItemUpdating(object sender, DetailsViewUpdateEventArgs e)
    {
        Boolean update = false;
        String error = "";
        #region ItemUpdating Action
        DetailsView dv = (DetailsView)sender;
        #region ItemUpdating Action for dv_ivr_file_vc
        if (dv.ID == "dv_ivr_file_vc")
        {
            #region Try: dv_ivr_file_vc Update
            try
            {
                DropDownList uptRole = (DropDownList)dv.FindControl("ddlRole");
                DropDownList uptClient = (DropDownList)dv.FindControl("ddlClient");
                String UserID = ((HiddenField)dv.FindControl("UserID")).Value;
                String RoleID = ((HiddenField)dv.FindControl("RoleID")).Value;
                String ClientID = ((HiddenField)dv.FindControl("ClientID")).Value;
                String ModuleID = ((HiddenField)dv.FindControl("ModuleID")).Value;

                if (uptRole != null) { update = true; dtlLabel.Text += "<br />Role: " + uptRole.SelectedValue; }
                if (update)
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText = "[dbo].[user_update_user_credentials]";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@UserID", UserID));
                        cmd.Parameters.Add(new SqlParameter("@Role", uptRole.SelectedValue));
                        cmd.Parameters.Add(new SqlParameter("@RoleOld", RoleID));
                        cmd.Parameters.Add(new SqlParameter("@Client", uptClient.SelectedValue));
                        cmd.Parameters.Add(new SqlParameter("@ClientOld", ClientID));
                        //cmd.Parameters.Add(new SqlParameter("@ModuleOld", ModuleID));
                        DetailsView_UpdateRecord(cmd);
                    }
                }
            }
            #endregion Try: dv_ivr_file_vc Update
            #region Catch: dv_ivr_file_vc Update
            catch (Exception ex)
            {
                Error_Save(ex, "Update User: User Credentials");
                update = false;
                error = "Internal server error during update process.";
            }
            #endregion Catch: dv_ivr_file_vc Update
        }
        #endregion ItemUpdating Action for dv_ivr_file_vc
        #region ItemUpdating Action for dv_ivr_file_cc
        else if (dv.ID == "dv_ivr_file_cc")
        {
            #region Try: dv_ivr_file_cc Update
            try
            {
                DropDownList uptPrefix = (DropDownList)dv.FindControl("ddlPrefix");
                TextBox uptFirstName = (TextBox)dv.FindControl("FirstName");
                TextBox uptMiddleName = (TextBox)dv.FindControl("MiddleName");
                TextBox uptLastName = (TextBox)dv.FindControl("LastName");
                DropDownList uptSuffix = (DropDownList)dv.FindControl("ddlSuffix");

                if (uptFirstName != null
                    && uptFirstName.Text.Length > 0
                    && uptLastName != null
                    && uptLastName.Text.Length > 0
                    )
                {
                    update = true;
                }

                if (uptPrefix != null) { dtlLabel.Text += "<br />Prefix: " + uptPrefix.SelectedValue; } else { update = false; }
                if (uptFirstName != null) { dtlLabel.Text += "<br />FirstName: " + uptFirstName.Text; } else { update = false; }
                if (uptMiddleName != null) { dtlLabel.Text += "<br />MiddleName: " + uptMiddleName.Text; } else { update = false; }
                if (uptLastName != null) { dtlLabel.Text += "<br />LastName: " + uptLastName.Text; } else { update = false; }
                if (uptSuffix != null) { dtlLabel.Text += "<br />Suffix: " + uptSuffix.SelectedValue; } else { update = false; }

                if (update)
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        Int32 UserID = Convert.ToInt32(GridView1.SelectedDataKey.Values[0].ToString());
                        cmd.CommandText = "[dbo].[user_update_user_details]";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@SP_UserID", UserID));
                        cmd.Parameters.Add(new SqlParameter("@uptPrefix", uptPrefix.SelectedValue));
                        cmd.Parameters.Add(new SqlParameter("@uptFirstName", uptFirstName.Text.Trim()));
                        cmd.Parameters.Add(new SqlParameter("@uptMiddleName", uptMiddleName.Text.Trim()));
                        cmd.Parameters.Add(new SqlParameter("@uptLastName", uptLastName.Text.Trim()));
                        cmd.Parameters.Add(new SqlParameter("@uptSuffix", uptSuffix.SelectedValue));
                        DetailsView_UpdateRecord(cmd);
                    }
                }
            }
            #endregion Try: dv_ivr_file_cc Update
            #region Catch: dv_ivr_file_cc Update
            catch (Exception ex)
            {
                Error_Save(ex, "Update User: User Details");
                update = false;
                error = "Internal server error during update process.";
            }
            #endregion Catch: dv_ivr_file_cc Update
        }
        #endregion ItemUpdating Action for dv_ivr_file_cc
        #region ItemUpdating Action for dv_ivr_file_rn
        else if (dv.ID == "dv_ivr_file_rn")
        {
            #region Try: dv_ivr_file_rn Update
            try
            {
                TextBox uptPhoneArea = (TextBox)dv.FindControl("PhoneArea");
                TextBox uptPhonePrefix = (TextBox)dv.FindControl("PhonePrefix");
                TextBox uptPhoneSuffix = (TextBox)dv.FindControl("PhoneSuffix");

                if (uptPhoneArea != null
                    && uptPhoneArea.Text.Length == 3
                    && uptPhonePrefix != null
                    && uptPhonePrefix.Text.Length == 3
                    && uptPhoneSuffix != null
                    && uptPhoneSuffix.Text.Length == 4
                    )
                {
                    update = true;
                }
                else
                {
                    error = "You can not blank out the phone number at this time.";
                }
                if (uptPhoneArea != null) { dtlLabel.Text += "<br />PhoneArea: " + uptPhoneArea.Text; } else { update = false; }
                if (uptPhonePrefix != null) { dtlLabel.Text += "<br />PhonePrefix: " + uptPhonePrefix.Text; } else { update = false; }
                if (uptPhoneSuffix != null) { dtlLabel.Text += "<br />PhoneSuffix: " + uptPhoneSuffix.Text; } else { update = false; }
                if (update)
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        Int32 UserID = Convert.ToInt32(GridView1.SelectedDataKey.Values[0].ToString());
                        cmd.CommandText = "[dbo].[user_update_user_phone]";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@SP_UserID", UserID));
                        cmd.Parameters.Add(new SqlParameter("@uptPhoneArea", uptPhoneArea.Text));
                        cmd.Parameters.Add(new SqlParameter("@uptPhonePrefix", uptPhonePrefix.Text));
                        cmd.Parameters.Add(new SqlParameter("@uptPhoneSuffix", uptPhoneSuffix.Text));
                        DetailsView_UpdateRecord(cmd);
                    }
                }
            }
            #endregion Try: dv_ivr_file_rn Update
            #region Catch: dv_ivr_file_rn Update
            catch (Exception ex)
            {
                Error_Save(ex, "Update User: User Phone");
                update = false;
                error = "Internal server error during update process.";
            }
            #endregion Catch: dv_ivr_file_rn Update
        }
        #endregion ItemUpdating Action for dv_ivr_file_rn
        #region ItemUpdating Action for dv_ivr_file_op
        else if (dv.ID == "dv_ivr_file_op")
        {
            #region Try: dv_ivr_file_op Update
            try
            {
                TextBox uptAddress1 = (TextBox)dv.FindControl("Address1");
                TextBox uptAddress2 = (TextBox)dv.FindControl("Address2");
                TextBox uptAddress3 = (TextBox)dv.FindControl("Address3");
                TextBox uptCity = (TextBox)dv.FindControl("City");
                TextBox uptZip = (TextBox)dv.FindControl("Zip");
                DropDownList uptState = (DropDownList)dv.FindControl("ddlState");
                DropDownList uptCountry = (DropDownList)dv.FindControl("ddlCountry");

                if (uptAddress1 != null
                    && uptAddress2 != null
                    && uptAddress3 != null
                    && uptCity != null
                    && uptState != null
                    && uptZip != null
                    && uptCountry != null
                    )
                {
                    update = true;
                }

                #region This is a Server Side check - If we failed, the code is broken (Possible Browser Related Issue)
                if (uptAddress1 != null) { dtlLabel.Text += "<br />Address1: " + uptAddress1.Text; } else { update = false; }
                if (uptAddress2 != null) { dtlLabel.Text += "<br />Address2: " + uptAddress2.Text; } else { update = false; }
                if (uptAddress3 != null) { dtlLabel.Text += "<br />Address3: " + uptAddress3.Text; } else { update = false; }
                if (uptCity != null) { dtlLabel.Text += "<br />City: " + uptCity.Text; } else { update = false; }
                if (uptState != null) { dtlLabel.Text += "<br />State: " + uptState.SelectedValue; } else { update = false; }
                if (uptZip != null) { dtlLabel.Text += "<br />Zip: " + uptZip.Text; } else { update = false; }
                if (uptCountry != null) { dtlLabel.Text += "<br />Country: " + uptCountry.SelectedValue; } else { update = false; }
                #endregion This is a Server Side check - If we failed, the code is broken (Possible Browser Related Issue)

                if (update)
                {
                    // Here we should process the validation
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        Int32 UserID = Convert.ToInt32(GridView1.SelectedDataKey.Values[0].ToString());
                        cmd.CommandText = "[dbo].[user_update_user_address]";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@SP_UserID", UserID));
                        cmd.Parameters.Add(new SqlParameter("@uptAddress1", uptAddress1.Text));
                        cmd.Parameters.Add(new SqlParameter("@uptAddress2", uptAddress2.Text));
                        cmd.Parameters.Add(new SqlParameter("@uptAddress3", uptAddress3.Text));
                        cmd.Parameters.Add(new SqlParameter("@uptCity", uptCity.Text));
                        cmd.Parameters.Add(new SqlParameter("@uptState", uptState.SelectedValue));
                        cmd.Parameters.Add(new SqlParameter("@uptZip", uptZip.Text));
                        cmd.Parameters.Add(new SqlParameter("@uptCountry", uptCountry.SelectedValue));
                        DetailsView_UpdateRecord(cmd);
                    }
                }
            }
            #endregion Try: dv_ivr_file_op Update
            #region Catch: dv_ivr_file_op Update
            catch (Exception ex)
            {
                Error_Save(ex, "Update User: User Address");
                update = false;
                error = "Internal server error during update process.";
            }
            #endregion Catch: dv_ivr_file_op Update

        }
        #endregion ItemUpdating Action for dv_ivr_file_op
        if (update)
        {
            dv.ChangeMode(DetailsViewMode.ReadOnly);
            Int32 callid = Convert.ToInt32(GridView1.SelectedDataKey.Values[0].ToString());
            Int32 donorid = Convert.ToInt32(GridView1.SelectedDataKey.Values[1].ToString());
            //DetailsView_Data(callid, donorid, dv);
            GridView_Data(0, "", GridView1);
        }
        else
        {
            WriteToLabel("add", "Cyan", "<br /><br />There was an error updating your record.<br />Please review the below message:", dtlLabel);
            WriteToLabel("add", "Red", "<br /><br />" + error, dtlLabel);
        }
        #endregion ItemUpdating Action
    }
    protected void DetailsView_UpdateRecord(SqlCommand cmd)
    {
        #region Using: SqlConnection
        using (SqlConnection con = new SqlConnection(sqlStr))
        {
            using (cmd)
            {
                cmd.Connection = con;
                if (con.State == ConnectionState.Closed) { con.Open(); }
                cmd.CommandTimeout = 600;

                using (SqlDataReader sqlRdr = cmd.ExecuteReader())
                {
                    if (sqlRdr.HasRows)
                    {
                        while (sqlRdr.Read())
                        {
                            try
                            {
                                WriteToLabel("add", "Blue", "<br />" + sqlRdr[0].ToString(), dtlLabel);
                            }
                            catch
                            {
                                WriteToLabel("add", "Red", "<br />" + "oops?", dtlLabel);
                            }
                        }
                    }
                    else
                    {
                        WriteToLabel("add", "Red", "<br />" + "No Rows", dtlLabel);
                    }
                }
            }
        }
        #endregion Using: SqlConnection
    }
    protected void DetailsView_ItemUpdated(object sender, EventArgs e)
    {
        #region ItemUpdated Action
        dtlLabel.Text = "DetailsView_ItemUpdated";
        #endregion ItemUpdated Action
    }
    #endregion Details View Handling
    #region Record Clear
    protected void Processing_Record_Clear(object sender, EventArgs e)
    {
        ClearRecord.Visible = true;
    }
    protected void Processing_Record_Clear_Confirm(object sender, EventArgs e)
    {
        if (Session["userid"] == null) { ghUser.identity_get_userid(); }
        #region Using: SqlConnection
        using (SqlConnection con = new SqlConnection(sqlStr))
        {
            using (SqlCommand cmd = new SqlCommand("", con))
            {
                cmd.CommandText = "[dbo].[sp_ivr_search_clear]";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                #region SQL Parameters
                Int32 sourceid = Convert.ToInt32(GridView1.SelectedDataKey.Values[0].ToString());
                Int32 recordid = Convert.ToInt32(GridView1.SelectedDataKey.Values[1].ToString());
                String calldate = GridView1.SelectedDataKey.Values[2].ToString();
                String calltime = GridView1.SelectedDataKey.Values[3].ToString();
                String ani = GridView1.SelectedDataKey.Values[4].ToString();
                cmd.Parameters.Add(new SqlParameter("@sp_sourceid", sourceid));
                cmd.Parameters.Add(new SqlParameter("@sp_recordid", recordid));
                cmd.Parameters.Add(new SqlParameter("@sp_calldate", calldate));
                cmd.Parameters.Add(new SqlParameter("@sp_calltime", calltime));
                cmd.Parameters.Add(new SqlParameter("@sp_ani", ani));
                cmd.Parameters.Add(new SqlParameter("@sp_userid", Session["userid"].ToString()));
                #endregion SQL Parameters
                if (con.State == ConnectionState.Closed) { con.Open(); }
                int affected = cmd.ExecuteNonQuery();
                btnClearRecordConfirm.Enabled = false;
                btnClearRecordCancel.Enabled = false;
                lblClearRecordConfirm.Text = "Records Updated: " + affected.ToString();
                GridView_Data(0, "", GridView1);
            }
        }
        #endregion Using: SqlConnection
    }
    protected void Processing_Record_Clear_Cancel(object sender, EventArgs e)
    {
        ClearRecord.Visible = false;
    }
    #endregion Record Clear
    #region Record Discard
    protected void Processing_Record_Discard(object sender, EventArgs e)
    {
        DiscardRecord.Visible = true;
    }
    protected void Processing_Record_Discard_Confirm(object sender, EventArgs e)
    {
        if (Session["userid"] == null) { ghUser.identity_get_userid(); }
        #region Using: SqlConnection
        using (SqlConnection con = new SqlConnection(sqlStr))
        {
            using (SqlCommand cmd = new SqlCommand("", con))
            {
                cmd.CommandText = "[dbo].[sp_ivr_search_discard]";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                #region SQL Parameters
                Int32 sourceid = Convert.ToInt32(GridView1.SelectedDataKey.Values[0].ToString());
                Int32 recordid = Convert.ToInt32(GridView1.SelectedDataKey.Values[1].ToString());
                String calldate = GridView1.SelectedDataKey.Values[2].ToString();
                String calltime = GridView1.SelectedDataKey.Values[3].ToString();
                String ani = GridView1.SelectedDataKey.Values[4].ToString();
                cmd.Parameters.Add(new SqlParameter("@sp_sourceid", sourceid));
                cmd.Parameters.Add(new SqlParameter("@sp_recordid", recordid));
                cmd.Parameters.Add(new SqlParameter("@sp_calldate", calldate));
                cmd.Parameters.Add(new SqlParameter("@sp_calltime", calltime));
                cmd.Parameters.Add(new SqlParameter("@sp_ani", ani));
                cmd.Parameters.Add(new SqlParameter("@sp_userid", Session["userid"].ToString()));
                #endregion SQL Parameters
                if (con.State == ConnectionState.Closed) { con.Open(); }
                int affected = cmd.ExecuteNonQuery();
                btnDiscardRecordConfirm.Enabled = false;
                btnDiscardRecordCancel.Enabled = false;
                lblDiscardRecordConfirm.Text = "Records Updated: " + affected.ToString();
                GridView_Data(0, "", GridView1);
                GridView1.SelectedIndex = -1;
            }
        }
        #endregion Using: SqlConnection
    }
    protected void Processing_Record_Discard_Cancel(object sender, EventArgs e)
    {
        DiscardRecord.Visible = false;
    }
    #endregion Record Discard
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
    /// <summary>
    /// Testing
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <param name="e2"></param>
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
        ErrorLog.ErrorLog_Save(ex, dv, "Portal: Oracle", error, spPage, spQS, spURL);
        dv.Visible = true;
        updatePanel2.Update();
    }
}
