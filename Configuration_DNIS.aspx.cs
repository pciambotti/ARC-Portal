using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
public partial class Configuration_DNIS : System.Web.UI.Page
{
    protected void Page_PreInit(Object sender, EventArgs e)
    {
        Master.PageTitle = "Configuration DNIS";
        this.Title = String.Format("{0}: {1}", Master.MyTitle, Page.Title);
    }
    private String sqlStr = Connection.GetConnectionString("Default", ""); // Controlled by Web Config ARC_Live | ARC_Stage
    private String sqlStrDE = Connection.GetConnectionString("DE_Production", "");
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            DNIS_List_Refresh();
            #region Enable IT commands
            if (this.Page.User.Identity.Name == "nciambotti@greenwoodhall.com"
                || this.Page.User.Identity.Name == "cstevenson@greenwoodhall.com"
                )
            {
                btnDNISAdd.Visible = true;
            }
            #endregion
        }
    }
    protected void Refresh_Button(object sender, EventArgs e)
    {
        DNIS_List_Refresh();
    }
    protected void DNIS_List_Refresh()
    {
        try
        {
            if (gvDNISList.SelectedIndex >= 0) gvDNISList.SelectedIndex = -1;
            DNIS_List_Fetch();
            Session["gridrefresh"] = DateTime.UtcNow.ToString();
        }
        catch (Exception ex)
        {
            Label8.Text = "Error processing the request";
            Error_Save(ex, "DNIS_List_Refresh");
        }
    }
    protected void DNIS_List_Fetch()
    {
        GridView gv = gvDNISList;
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
                    #region Build cmdText
                    String cmdText = "";
                    cmdText = @"
                                SELECT
                                [d].[line]
                                ,[d].[company]
                                ,[d].[city]
                                ,[d].[state]
                                ,[d].[type]
                                ,[d].[dnis]
                                ,[d].[phonenumber]
                                ,[d].[languageid]
                                ,[d].[isactive]
                                ,[d].[sourcecode_onetime]
                                ,[d].[sourcecode_sustainer]
                                FROM [dbo].[dnis] [d]
                                WHERE 1=1
                            ";
                    if (ddlStatus.SelectedValue == "A")
                    {
                        cmdText += "AND [d].[isactive] = '1'\r";
                    }
                    else if (ddlStatus.SelectedValue == "D")
                    {
                        cmdText += "AND [d].[isactive] = '0'\r";
                    }
                    else
                    {

                    }
                        
                    cmdText += "ORDER BY [d].[line], [d].[company], [d].[phonenumber]\r";
                    cmdText += "\r";
                    #endregion Build cmdText
                    cmd.CommandText = cmdText;
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Clear();
                    #region SQL Parameters
                    cmd.Parameters.Add("@sp_status", SqlDbType.VarChar, 1).Value = ddlStatus.SelectedValue;
                    #endregion SQL Parameters
                    print_sql(cmd, "append"); // Will print for Admin in Local
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
    protected void DNIS_List_Select_DNIS(int sp_dnisid)
    {
        try
        {
            if (sp_dnisid > 0)
            {
                gvDNISList.SelectedIndex = -1;
                if (gvDNISList.Rows.Count > 0)
                {
                    // http://stackoverflow.com/questions/19823803/getting-datakeynames-for-row-on-button-click/19824029
                    foreach (GridViewRow gvr in gvDNISList.Rows)
                    {
                        if (gvDNISList.DataKeys[gvr.RowIndex].Values[0].ToString() == sp_dnisid.ToString())
                        {
                            gvDNISList.SelectedIndex = gvr.RowIndex;
                            DetailsView_Data(dvDNISDetails);
                            break;
                        }
                    }
                }

            }
        }
        catch (Exception ex)
        {
            Label8.Text = "Error DNIS_List_Select_DNIS";
            Error_Catch(ex, "DNIS_List_Select_DNIS", msgLabel);
        }
    }
    protected void GridView_DataBound(Object sender, EventArgs e)
    {
        // get the GridView from the sender
        GridView gv = (GridView)sender;

        #region gvDNISList
        Label8.Text = " Records: [" + gv.Rows.Count.ToString() + "]";
        if (gv.PageCount > 0)
        {
            Label8.Text += " - Pages: [" + gv.PageCount.ToString() + "]";
            Label8.Text += " - Approx Total: [" + (gv.PageCount * gv.Rows.Count).ToString() + "]";
            // Retrieve the pager row.
            GridViewRow pagerRow = gv.TopPagerRow;
            // Retrieve the DropDownList and Label controls from the row.
            DropDownList pageList = (DropDownList)pagerRow.Cells[0].FindControl("PageDropDownList");
            Label pageLabel = (Label)pagerRow.Cells[0].FindControl("CurrentPageLabel");
            if (pageList != null)
            {
                // Create the values for the DropDownList control based on 
                // the  total number of pages required to display the data
                // source.
                for (int i = 0; i < gv.PageCount; i++)
                {
                    // Create a ListItem object to represent a page.
                    int pageNumber = i + 1;
                    ListItem item = new ListItem(pageNumber.ToString());
                    // If the ListItem object matches the currently selected
                    // page, flag the ListItem object as being selected. Because
                    // the DropDownList control is recreated each time the pager
                    // row gets created, this will persist the selected item in
                    // the DropDownList control.   
                    if (i == gv.PageIndex)
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
                int currentPage = gv.PageIndex + 1;
                // Update the Label control with the current page information.
                pageLabel.Text = "Page " + currentPage.ToString() +
                  " of " + gv.PageCount.ToString();
            }
            if (gv.PageIndex > 0)
            {
                pagerRow.Cells[0].FindControl("lnkPrevPage").Visible = true;
                pagerRow.Cells[0].FindControl("lnkFirstPage").Visible = true;
            }
            else
            {
                pagerRow.Cells[0].FindControl("lnkPrevPage").Visible = false;
                pagerRow.Cells[0].FindControl("lnkFirstPage").Visible = false;
            }

            if (gv.PageCount != (gv.PageIndex + 1))
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
        #endregion gvDNISList
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

            e.Row.Attributes["onclick"] = Page.ClientScript.GetPostBackClientHyperlink(this.gvDNISList, "Select$" + e.Row.RowIndex);
        }
    }
    protected void GridView_IndexChanged(object sender, EventArgs e)
    {
        dtlLabel.Text = "Fetching details: " + DateTime.Now.ToString("HH:mm:ss");
        if (dvDNISDetails.CurrentMode == DetailsViewMode.Insert || dvDNISDetails.CurrentMode == DetailsViewMode.Edit)
        {
            dvDNISDetails.ChangeMode(DetailsViewMode.ReadOnly);
        }
        DetailsView_Data(dvDNISDetails);
    }
    protected void GridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        //Session["EventList_GridView_SelectedIndex"] = null;
        //Session["EventList_GridView_PageIndex"] = null;
        Label8.Text = e.NewPageIndex.ToString();
        gvDNISList.SelectedIndex = -1;
        gvDNISList.PageIndex = e.NewPageIndex;
        DNIS_List_Fetch();
    }
    protected void GridView_PageDropDownList_SelectedIndexChanged(Object sender, EventArgs e)
    {

        // Retrieve the pager row.
        GridViewRow pagerRow = gvDNISList.TopPagerRow;
        // Retrieve the PageDropDownList DropDownList from the bottom pager row.
        DropDownList pageList = (DropDownList)pagerRow.Cells[0].FindControl("PageDropDownList");
        // Set the PageIndex property to display that page selected by the user.
        gvDNISList.SelectedIndex = -1;
        gvDNISList.PageIndex = pageList.SelectedIndex;
        DNIS_List_Fetch();

    }
    protected void GridView_Data2(Int32 CallID, Int32 DonorID, GridView gv, Panel pnl)
    {
        gv.SelectedIndex = -1;
        #region SQL Connection
        using (SqlConnection con = new SqlConnection(sqlStr))
        {
            #region SQL Command
            using (SqlCommand cmd = new SqlCommand("", con))
            {
                if (con.State == ConnectionState.Closed) { con.Open(); }
                cmd.CommandTimeout = 600;
                #region Build cmdText
                String cmdText = "";
                if (gv.ID == "gvGiftList") { cmdText = "[dbo].[portal_call_search_get_gift_list]"; }
                if (gv.ID == "gvRecurringList") { cmdText = "[dbo].[portal_call_search_get_recurring_list]"; }
                #endregion Build cmdText
                cmd.CommandText = cmdText;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                #region SQL Parameters
                cmd.Parameters.Add(new SqlParameter("@sp_callid", CallID));
                #endregion SQL Parameters
                #region SQL Processing
                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                ad.Fill(dt);
                gv.DataSource = dt;
                gv.DataBind();
                if (dt.Rows.Count > 0)
                {
                    pnl.Visible = true;
                    gv.Visible = true;
                    //gvRecurringListExport.DataSource = dt;
                    //gvRecurringListExport.DataBind();
                }
                else
                {
                    pnl.Visible = false;
                    gv.Visible = false;
                    //gvRecurringListExport.DataSource = null;
                    //gvRecurringListExport.DataBind();
                }

                #endregion SQL Processing

            }
            #endregion SQL Command
        }
        #endregion SQL Connection
    }
    protected void DetailsView_Clear()
    {
        if (dvDNISDetails.Rows.Count > 0) { dvDNISDetails.DataBind(); }
    }
    protected void DetailsView_Data(DetailsView dv)
    {
        GridView gv = gvDNISList;
        String sp_phonenumber = "";
        try
        {
            if (gv.SelectedIndex != -1)
            {
                sp_phonenumber = gv.SelectedDataKey["phonenumber"].ToString();
            }
            if (sp_phonenumber.Length > 0)
            {
                #region SQL Connection
                using (SqlConnection con = new SqlConnection(sqlStr))
                {
                    #region SQL Command
                    using (SqlCommand cmd = new SqlCommand("", con))
                    {
                        if (con.State == ConnectionState.Closed) { con.Open(); }
                        cmd.CommandTimeout = 600;
                        #region Build cmdText
                        String cmdText = "";
                        #region dvDNISDetails
                        if (dv.ID == "dvDNISDetails")
                        {
                            cmdText = @"
                                SELECT
                                [d].[line]
                                ,[d].[company]
                                ,[d].[city]
                                ,[d].[state]
                                ,[d].[type]
                                ,[d].[dnis]
                                ,[d].[phonenumber]
                                ,[d].[languageid]
                                ,[d].[isactive]
                                ,[d].[sourcecode_onetime]
                                ,[d].[sourcecode_sustainer]
                                FROM [dbo].[dnis] [d]
                                WHERE 1=1
AND [d].[phonenumber] = @sp_phonenumber
                                        ";
                        }
                        #endregion dvDNISDetails
                        #endregion Build cmdText
                        cmd.CommandText = cmdText;
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Clear();
                        #region SQL Parameters
                        cmd.Parameters.Add("@sp_phonenumber", SqlDbType.VarChar, 20).Value = sp_phonenumber;
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
            else
            {
                lblErrorMsg.Text = "Somethign went wrong.. no Company or Interaction";
            }
        }
        catch (Exception ex)
        {
            lblErrorMsg.Text = "Somethign went wrong.. Error with Company or Interaction";
            Error_Catch(ex, "DetailsView_Data | " + dv.ID, msgLabel);
        }
    }
    protected void DNIS_Add_Click(object sender, EventArgs e)
    {
        string msgLog = "";
        #region Try Stuff
        try
        {
            /// Need to switch the DNIS Details grid to Insert
            /// 
            msgResults.Text = "<br />Change View Status to Insert";
            dvDNISDetails.ChangeMode(DetailsViewMode.Insert);
            msgLog += String.Format("<li>{0}</li>", "Doe changing");
        }
        #endregion Try Stuff
        #region Catch Stuff
        catch (Exception ex)
        {
            Error_Catch(ex, "DNIS_Add_Click", msgDebug);
        }
        #endregion Catch Stuff
        if (msgLog.Length > 0) msgResults.Text += String.Format("<br />{0}", msgLog);
    }
    protected void DetailsView_DataBound(object sender, EventArgs e)
    {
        string msgLog = "";
        #region Try Stuff
        try
        {
            #region DataBound Action
            DetailsView dv = (DetailsView)sender;
            #region DataBound Action for dvDisposition
            if (dv.ID == "dvDNISDetails")
            {
                if (dv.CurrentMode == DetailsViewMode.ReadOnly)
                {
                    if (Page.User.IsInRole("System Administrator") || Page.User.IsInRole("Administrator"))
                    {
                        Button btn = (Button)dv.FindControl("modify");
                        if (btn != null)
                        {
                            btn.Visible = true;
                        }
                    }
                }
                if (dv.CurrentMode == DetailsViewMode.Edit)
                {
                    if (Page.User.IsInRole("System Administrator") || Page.User.IsInRole("Administrator"))
                    {
                        Button btn = (Button)dv.FindControl("update");
                        if (btn != null)
                        {
                            btn.Enabled = true;
                        }
                        #region Enable IT commands
                        if (this.Page.User.Identity.Name == "nciambotti@greenwoodhall.com"
                            || this.Page.User.Identity.Name == "nciambotti@greenwoodhall.com"
                            )
                        {
                            TextBox fundcode = (TextBox)dv.FindControl("fundcode");
                            if (fundcode != null) { fundcode.Enabled = true; }
                            CheckBox status_adu = (CheckBox)dv.FindControl("status_adu");
                            if (status_adu != null) { status_adu.Enabled = true; }
                            TextBox sp_continue = (TextBox)dv.FindControl("continue");
                            if (sp_continue != null) { sp_continue.Enabled = true; }
                        }
                        #endregion
                    }
                }

            }
            #endregion DataBound Action for dvDisposition
            #endregion DataBound Action
        }
        #endregion Try Stuff
        #region Catch Stuff
        catch (Exception ex)
        {
            Error_Catch(ex, "NewVoid", msgDebug);
        }
        #endregion Catch Stuff
        if (msgLog.Length > 0) msgResults.Text += String.Format("<br />{0}", msgLog);
    }
    protected void DetailsView_ModeChanging(object sender, DetailsViewModeEventArgs e)
    {
        try
        {
            #region ModeChanging Action
            DetailsView dv = (DetailsView)sender;
            dv.ChangeMode(e.NewMode);
            DetailsView_Data(dv);
            if (e.NewMode == DetailsViewMode.Edit)
            {
                #region ModeChanging Action for dvDNISDetails
                lblItemMsg.Text = "Inside";
                if (dv.ID == "dvDNISDetails")
                {
                    if (dv.CurrentMode == DetailsViewMode.Edit)
                    {
                        DropDownList ddlStatus = (DropDownList)dv.FindControl("status");
                        HiddenField crntStatus = (HiddenField)dv.FindControl("status_crnt");
                        if (ddlStatus != null && crntStatus != null)
                        {
                            ddlStatus.SelectedValue = crntStatus.Value;
                        }
                    }
                }


                if (dv.ID == "dvDisposition")
                {
                    dv.AllowPaging = false;
                    ListBox lb = (ListBox)dv.FindControl("status");
                    String status = ((HiddenField)dv.FindControl("status_current")).Value;
                    if (lb != null)
                    {
                        //DDL_Load_Status_Manual(lb);
                        foreach (ListItem li in lb.Items)
                        {
                            if (li.Value == status)
                            {
                                li.Selected = true;
                                break;
                            }
                        }
                    }
                }
                #endregion ModeChanging Action for dvDNISDetails
            }
            else
            {
                //dv.AllowPaging = true;
            }
            #endregion ModeChanging Action
        }
        catch (Exception ex)
        {
            Error_Catch(ex, "DetailsView_ModeChanging Error", lblErrorMsg);
        }
    }
    protected void DetailsView_ItemCommand(object sender, DetailsViewCommandEventArgs e)
    {
        try
        {
            // Admin_Clear();
            //if (Page.User.IsInRole("System Administrator")) pAdminFunctions.Visible = true;
            //if (ghUser.identity_is_admin()) pAdminFunctions.Visible = true;
            DetailsView dv = (DetailsView)sender;
            #region ItemCommand Action
            if (dv.ID == "dvInteractionARC")
            {
                if (e.CommandName == "Refund")
                {
                    //Refund_Start(dv, e);
                }
            }
            else if (dv.ID == "dvPaymentDetailsRecurring")
            {
                if (e.CommandName == "Refund")
                {
                    //Refund_Start(dv, e);
                }
            }
            else if (dv.ID == "dvSustainerDetails")
            {
                if (e.CommandName == "Modify")
                {
                    //Modify_Sustainer(dv, e);
                }
            }
            else if (e.CommandName == "Clear")
            {
                gvDNISList.SelectedIndex = -1;
                DetailsView_Clear();
            }
            else if (e.CommandName == "Cancel")
            {
                dv.ChangeMode(DetailsViewMode.ReadOnly);
                dv.DataBind();
            }
            if (e.CommandName == "Update")
            {
                WriteToLabel("new", "Blue", dv.ID + " Update Processed [" + DateTime.Now.ToString("HH:mm:ss") + "]<br />", lblItemMsg);
            }
            if (e.CommandName == "Edit")
            {
                dtlLabel.Text = "Edit: Test";
            }
            else if (e.CommandName == "Insert")
            {
                msgResults.Text += "<br />Insert Complete1";
            }
            //Label1.Text = e.CommandName;
            #endregion ItemCommand Action
        }
        catch (Exception ex)
        {
            Error_Catch(ex, "DetailsView_ItemCommand Error", lblErrorMsg);
        }
    }
    protected void DetailsView_ItemUpdating(object sender, DetailsViewUpdateEventArgs e)
    {
        Int32 sp_userid = 0;
        try
        {
            if (Session["userid"] == null) { ghUser.identity_get_userid(); }
            Int32.TryParse(Session["userid"].ToString(), out sp_userid);
            Boolean update = false;
            String error = "";
            String errMsg = "";
            #region ItemUpdating Action
            DetailsView dv = (DetailsView)sender;
            #region ItemUpdating Action for dvDisposition
            if (dv.ID == "dvDNISDetails")
            {
                #region Try: DetailsView1 Update
                try
                {
                    /// Get the values from the DV EditTemplate
                    /// 
                    #region Get Values
                    Int32 sp_dnisid = 0;
                    Int32.TryParse(((Label)dv.FindControl("dnisid")).Text, out sp_dnisid);
                    if (sp_dnisid == 0) { errMsg += "<br />Did not capture dnisid"; throw new Exception("Capture field error"); }

                    Int32 sp_externaltypeid = 1; // DNIS
                    Int32 sp_externalid = sp_dnisid;
                    Int32 sp_action = 0;
                    Int32 sp_action_result = 0;
                    String sp_notes = ""; // Use this for agent entered, and also to/from statuses
                    DateTime sp_createdate = DateTime.UtcNow;

                    Int32 sp_itemtypeid = 0;
                    Int32 sp_itemid = sp_dnisid;
                    Int32 sp_languageid = 0; // Spanish

                    Int32 sp_sort = 999;
                    Int32.TryParse(((TextBox)dv.FindControl("sort")).Text.Trim(), out sp_sort);
                    Int32 sp_sort_crnt = 0;
                    Int32.TryParse(((HiddenField)dv.FindControl("sort_crnt")).Value, out sp_sort_crnt);

                    String sp_displayname = ((TextBox)dv.FindControl("displayname")).Text.Trim();
                    String sp_displayname_crnt = ((HiddenField)dv.FindControl("displayname_crnt")).Value;
                    String sp_fundcode = ((TextBox)dv.FindControl("fundcode")).Text;
                    String sp_fundcode_crnt = ((HiddenField)dv.FindControl("fundcode_crnt")).Value;
                    String sp_status = ((DropDownList)dv.FindControl("status")).SelectedValue;
                    String sp_status_crnt = ((HiddenField)dv.FindControl("status_crnt")).Value;
                    String sp_name = ((TextBox)dv.FindControl("name")).Text.Trim();
                    String sp_name_crnt = ((HiddenField)dv.FindControl("name_crnt")).Value;
                    String sp_name_spanish = ((TextBox)dv.FindControl("name_spanish")).Text.Trim();
                    String sp_name_spanish_crnt = ((HiddenField)dv.FindControl("name_spanish_crnt")).Value;

                    // Status
                    //Int32 sp_status_online_crnt = (strStatusOnline.Length > 0) ? Convert.ToInt32(strStatusOnline) : 0;
                    Boolean sp_status_online = ((CheckBox)dv.FindControl("status_online")).Checked;
                    String strStatusOnline = ((HiddenField)dv.FindControl("status_online_crnt")).Value;
                    Boolean sp_status_online_crnt = (strStatusOnline.Length > 0) ? Convert.ToBoolean(strStatusOnline) : false;
                    //Int32 sp_status_adu = (((CheckBox)dv.FindControl("status_adu")).Checked) ? 0 : 1;
                    //String strStatusADU = ((HiddenField)dv.FindControl("status_adu_crnt")).Value;
                    //Int32 sp_status_adu_crnt = (strStatusADU.Length > 0) ? Convert.ToInt32(strStatusADU) : 0;
                    Boolean sp_status_adu = ((CheckBox)dv.FindControl("status_adu")).Checked;
                    String strStatusADU = ((HiddenField)dv.FindControl("status_adu_crnt")).Value;
                    Boolean sp_status_adu_crnt = (strStatusOnline.Length > 0) ? Convert.ToBoolean(strStatusOnline) : false;

                    String sp_continue = ((TextBox)dv.FindControl("continue")).Text.Trim();
                    String sp_continue_crnt = ((HiddenField)dv.FindControl("continue_crnt")).Value;
                    String sp_description = ((TextBox)dv.FindControl("description")).Text.Trim();
                    String sp_description_crnt = ((HiddenField)dv.FindControl("description_crnt")).Value;
                    String sp_description_spanish = ((TextBox)dv.FindControl("description_spanish")).Text.Trim();
                    String sp_description_spanish_crnt = ((HiddenField)dv.FindControl("description_spanish_crnt")).Value;
                    String sp_agentnote_top = ((TextBox)dv.FindControl("agentnote_top")).Text.Trim();
                    String sp_agentnote_top_crnt = ((HiddenField)dv.FindControl("agentnote_top_crnt")).Value;
                    String sp_agentnote_bottom = ((TextBox)dv.FindControl("agentnote_bottom")).Text.Trim();
                    String sp_agentnote_bottom_crnt = ((HiddenField)dv.FindControl("agentnote_bottom_crnt")).Value;
                    #endregion Get Values
                    /// Determien if we have an update to perform
                    /// 
                    #region Check for Changes
                    if (sp_dnisid > 0
                        && (sp_displayname != sp_displayname_crnt
                        || sp_sort != sp_sort_crnt
                        || sp_fundcode != sp_fundcode_crnt
                        || sp_status != sp_status_crnt
                        || sp_name != sp_name_crnt
                        || sp_name_spanish != sp_name_spanish_crnt
                        || sp_status_online != sp_status_online_crnt
                        || sp_status_adu != sp_status_adu_crnt
                        || sp_continue != sp_continue_crnt
                        || sp_description != sp_description_crnt
                        || sp_description_spanish != sp_description_spanish_crnt
                        || sp_agentnote_top != sp_agentnote_top_crnt
                        || sp_agentnote_bottom != sp_agentnote_bottom_crnt
                        ))
                    {
                        /// Something changed, so flag for an update
                        update = true;
                    }
                    #endregion Check for Changes
                    else
                    {
                        /// There are no changes, so just close it and add a message that nothing was updated
                        dv.ChangeMode(DetailsViewMode.ReadOnly);
                        dtlLabel.Text = "Nothing to update";
                        DetailsView_Data(dv);
                        /// We do not need the gridview updated here?
                        /// GridView_Data(0, "", gvDispositions);
                    }

                    if (update)
                    {
                        /// We have an update, let's do it
                        /// 
                        #region Update dvDNISDetails
                        #region Using: SqlConnection
                        using (SqlConnection con = new SqlConnection(sqlStr))
                        {
                            Donation_Open_Database(con);
                            /// Updating the record is a multi part process
                            /// We need to update the table: [dnis]
                            /// We need to update/insert into table: [item_description]
                            /// We need to inserto into table: [history_action]

                            // First update the DNIS
                            #region SQL Command
                            using (SqlCommand cmd = new SqlCommand("", con))
                            {
                                #region Build cmdText
                                String cmdText = "";
                                cmdText = @"
                                    UPDATE [dbo].[dnis]
                                    SET [displayname] = @sp_displayname
                                    ,[sort] = @sp_sort
                                    --,[pagelocationid] = @sp_pagelocationid
                                    --,[merchantid] = @sp_merchantid
                                    ,[fundcode] = @sp_fundcode
                                    ,[status] = @sp_status

                                    ,[name] = @sp_name
                                    ,[status_online] = @sp_status_online
                                    ,[status_adu] = @sp_status_adu
                                    ,[continue] = @sp_continue
                                    ,[description] = @sp_description
                                    ,[agentnote_top] = @sp_agentnote_top
                                    ,[agentnote_bottom] = @sp_agentnote_bottom
                                    WHERE [dnisid] = @sp_dnisid
                            ";
                                cmdText += "\r";
                                #endregion Build cmdText
                                #region SQL Parameters
                                cmd.CommandText = cmdText;
                                cmd.CommandType = CommandType.Text;
                                cmd.Parameters.Clear();
                                cmd.Parameters.Add("@sp_dnisid", SqlDbType.Int).Value = sp_dnisid;
                                cmd.Parameters.Add("@sp_displayname", SqlDbType.VarChar, 4000).Value = sp_displayname;
                                cmd.Parameters.Add("@sp_sort", SqlDbType.Int).Value = sp_sort;                                
                                cmd.Parameters.Add("@sp_fundcode", SqlDbType.VarChar, 7).Value = sp_fundcode;
                                cmd.Parameters.Add("@sp_status", SqlDbType.VarChar, 1).Value = sp_status;
                                cmd.Parameters.Add("@sp_name", SqlDbType.VarChar, 4000).Value = sp_name;
                                cmd.Parameters.Add("@sp_status_online", SqlDbType.Bit).Value = sp_status_online;
                                cmd.Parameters.Add("@sp_status_adu", SqlDbType.Bit).Value = sp_status_adu;
                                cmd.Parameters.Add("@sp_continue", SqlDbType.VarChar, 10).Value = sp_continue;
                                cmd.Parameters.Add("@sp_description", SqlDbType.VarChar, 4000).Value = sp_description;
                                cmd.Parameters.Add("@sp_agentnote_top", SqlDbType.VarChar, 4000).Value = sp_agentnote_top;
                                cmd.Parameters.Add("@sp_agentnote_bottom", SqlDbType.VarChar, 4000).Value = sp_agentnote_bottom;
                                #endregion SQL Parameters
                                // print_sql(cmd, "append"); // Will print for Admin in Local
                                #region SQL Processing
                                int sqlNonQuery = cmd.ExecuteNonQuery();
                                if (sqlNonQuery == 1)
                                {
                                    // Good
                                }
                                else
                                {
                                    // Bad
                                    throw new Exception("Error trying to update dnis");
                                }
                                #endregion SQL Processing

                            }
                            #endregion SQL Command
                            // Next update/add the Locolization (Spanish Name)
                            if (sp_name_spanish != sp_name_spanish_crnt)
                            {
                                #region SQL Command
                                using (SqlCommand cmd = new SqlCommand("", con))
                                {
                                    #region Build cmdText
                                    String cmdText = "";
                                    cmdText = @"
                                    IF EXISTS(	SELECT TOP 1
				                                    1
			                                    FROM [dbo].[item_description] [id] WITH(NOLOCK)
			                                    WHERE [id].[itemtypeid] = @sp_itemtypeid
			                                    AND [id].[itemid] = @sp_itemid
			                                    AND [id].[languageid] = @sp_languageid
			                                    )
                                    BEGIN
	                                    UPDATE [dbo].[item_description]
	                                    SET [description] = @sp_description
	                                    ,[modifieddate] = @sp_modifieddate
	                                    WHERE [itemtypeid] = @sp_itemtypeid
	                                    AND [itemid] = @sp_itemid
	                                    AND [languageid] = @sp_languageid
                                    END
                                    ELSE
                                    BEGIN
	                                    INSERT INTO [dbo].[item_description]
	                                    ([itemtypeid], [itemid], [languageid], [description], [modifieddate])
	                                    SELECT
	                                    @sp_itemtypeid
	                                    ,@sp_itemid
	                                    ,@sp_languageid
	                                    ,@sp_description
	                                    ,@sp_modifieddate
                                    END
                            ";
                                    cmdText += "\r";
                                    #endregion Build cmdText
                                    #region SQL Parameters
                                    cmd.CommandText = cmdText;
                                    cmd.CommandType = CommandType.Text;
                                    cmd.Parameters.Clear();
                                    sp_itemtypeid = 1; // DNIS Name
                                    cmd.Parameters.Add("@sp_itemtypeid", SqlDbType.Int).Value = sp_itemtypeid;
                                    cmd.Parameters.Add("@sp_itemid", SqlDbType.Int).Value = sp_itemid;
                                    cmd.Parameters.Add("@sp_languageid", SqlDbType.Int).Value = sp_languageid;
                                    cmd.Parameters.Add("@sp_description", SqlDbType.NVarChar, 4000).Value = sp_name_spanish;
                                    cmd.Parameters.Add("@sp_modifieddate", SqlDbType.DateTime).Value = sp_createdate;
                                    #endregion SQL Parameters
                                    // print_sql(cmd, "append"); // Will print for Admin in Local
                                    #region SQL Processing
                                    int sqlNonQuery = cmd.ExecuteNonQuery();
                                    if (sqlNonQuery == 1)
                                    {
                                        // Good
                                    }
                                    else
                                    {
                                        // Bad
                                        throw new Exception("Error trying to update/insert DNIS name");
                                    }
                                    #endregion SQL Processing

                                }
                                #endregion SQL Command
                            }
                            // Next update/add the Locolization (Spanish Description)
                            if (sp_description_spanish != sp_description_spanish_crnt)
                            {
                                #region SQL Command
                                using (SqlCommand cmd = new SqlCommand("", con))
                                {
                                    /// Updating the record is a multi part process
                                    /// We need to update the table: [dnis]
                                    /// We need to update/insert into table: [item_description]
                                    /// We need to inserto into table: [history_action]
                                    #region Build cmdText
                                    String cmdText = "";
                                    cmdText = @"
                                    IF EXISTS(	SELECT TOP 1
				                                    1
			                                    FROM [dbo].[item_description] [id] WITH(NOLOCK)
			                                    WHERE [id].[itemtypeid] = @sp_itemtypeid
			                                    AND [id].[itemid] = @sp_itemid
			                                    AND [id].[languageid] = @sp_languageid
			                                    )
                                    BEGIN
	                                    UPDATE [dbo].[item_description]
	                                    SET [description] = @sp_description
	                                    ,[modifieddate] = @sp_modifieddate
	                                    WHERE [itemtypeid] = @sp_itemtypeid
	                                    AND [itemid] = @sp_itemid
	                                    AND [languageid] = @sp_languageid
                                    END
                                    ELSE
                                    BEGIN
	                                    INSERT INTO [dbo].[item_description]
	                                    ([itemtypeid], [itemid], [languageid], [description], [modifieddate])
	                                    SELECT
	                                    @sp_itemtypeid
	                                    ,@sp_itemid
	                                    ,@sp_languageid
	                                    ,@sp_description
	                                    ,@sp_modifieddate
                                    END
                            ";
                                    cmdText += "\r";
                                    #endregion Build cmdText
                                    #region SQL Parameters
                                    cmd.CommandText = cmdText;
                                    cmd.CommandType = CommandType.Text;
                                    cmd.Parameters.Clear();
                                    sp_itemtypeid = 2; // DNIS Description
                                    cmd.Parameters.Add("@sp_itemtypeid", SqlDbType.Int).Value = sp_itemtypeid;
                                    cmd.Parameters.Add("@sp_itemid", SqlDbType.Int).Value = sp_itemid;
                                    cmd.Parameters.Add("@sp_languageid", SqlDbType.Int).Value = sp_languageid;
                                    cmd.Parameters.Add("@sp_description", SqlDbType.NVarChar, 4000).Value = sp_description_spanish;
                                    cmd.Parameters.Add("@sp_modifieddate", SqlDbType.DateTime).Value = sp_createdate;
                                    #endregion SQL Parameters
                                    // print_sql(cmd, "append"); // Will print for Admin in Local
                                    #region SQL Processing
                                    int sqlNonQuery = cmd.ExecuteNonQuery();
                                    if (sqlNonQuery == 1)
                                    {
                                        // Good
                                    }
                                    else
                                    {
                                        // Bad
                                        throw new Exception("Error trying to update/insert DNIS description");
                                    }
                                    #endregion SQL Processing

                                }
                                #endregion SQL Command
                            }
                            // Next insert the History Action(s)
                            #region SQL Command
                            using (SqlCommand cmd = new SqlCommand("", con))
                            {
                                /// Updating the record is a multi part process
                                /// We need to update the table: [dnis]
                                /// We need to update/insert into table: [item_description]
                                /// We need to inserto into table: [history_action]
                                #region Build cmdText
                                String cmdText = "";
                                cmdText = @"
                                    INSERT INTO [dbo].[history_action]
                                    ([userid], [externaltypeid], [externalid], [action], [action_result], [notes], [createdate])
                                    SELECT
                                    @sp_userid
                                    ,@sp_externaltypeid
                                    ,@sp_externalid
                                    ,@sp_action
                                    ,@sp_action_result
                                    ,@sp_notes
                                    ,@sp_createdate
                            ";
                                cmdText += "\r";
                                #endregion Build cmdText
                                #region SQL Parameters
                                cmd.CommandText = cmdText;
                                cmd.CommandType = CommandType.Text;
                                cmd.Parameters.Clear();
                                cmd.Parameters.Add("@sp_userid", SqlDbType.Int).Value = sp_userid;
                                cmd.Parameters.Add("@sp_externaltypeid", SqlDbType.Int).Value = sp_externaltypeid;
                                cmd.Parameters.Add("@sp_externalid", SqlDbType.Int).Value = sp_externalid;
                                sp_action = 1; // Update Generic (description, etc)
                                               //sp_action = 2; // Change Status
                                               //sp_action = 3; // Change Online Status
                                               //sp_action = 4; // Change ADU Status
                                               //sp_action = 5; // 
                                               //sp_action = 6; // 
                                cmd.Parameters.Add("@sp_action", SqlDbType.Int).Value = sp_action;
                                sp_action_result = 1; // 0 == failed, 1 == good
                                cmd.Parameters.Add("@sp_action_result", SqlDbType.Int).Value = sp_action_result;
                                cmd.Parameters.Add("@sp_notes", SqlDbType.VarChar, 4000).Value = sp_notes;
                                cmd.Parameters.Add("@sp_createdate", SqlDbType.DateTime).Value = sp_createdate;
                                #endregion SQL Parameters
                                // print_sql(cmd, "append"); // Will print for Admin in Local
                                #region SQL Processing
                                // var sqlScalar = cmd.ExecuteScalar();
                                int sqlNonQuery = cmd.ExecuteNonQuery();
                                if (sqlNonQuery == 1)
                                {
                                    // Good
                                }
                                else
                                {
                                    // Bad
                                    throw new Exception("Error trying to insert history action(s)");
                                }
                                #endregion SQL Processing
                            }
                            #endregion SQL Command
                        }
                        #endregion Using: SqlConnection
                        #endregion Update dvDNISDetails
                    }
                }
                #endregion Try: DetailsView1 Update
                #region Catch: DetailsView1 Update
                catch (Exception ex)
                {
                    Error_Catch(ex, "Catch: DetailsView1 Update", lblErrorMsg);
                    update = false;
                    error = "Internal server error during update process.";
                }
                #endregion Catch: DetailsView1 Update
            }
            #endregion ItemUpdating Action for dvDisposition
            if (update)
            {
                dv.ChangeMode(DetailsViewMode.ReadOnly);
                DetailsView_Data(dv);
                DNIS_List_Fetch();
            }
            else
            {
                if (errMsg.Length > 0)
                {
                    WriteToLabel("add", "DarkRed", "<br /><br />There was an error updating your record.<br />Please review the below message:", lblItemMsg);
                    WriteToLabel("add", "Red", "<br /><br />" + errMsg, lblItemMsg);
                }
                else
                {
                    WriteToLabel("add", "DarkRed", "<br /><br />Nothing to update.", lblItemMsg);
                }
            }
            #endregion ItemUpdating Action
        }
        catch (Exception ex)
        {
            Error_Catch(ex, "DetailsView_ItemUpdating", lblErrorMsg);

        }
    }
    protected void DetailsView_ItemUpdated(object sender, EventArgs e)
    {
        string msgLog = "";
        #region Try Stuff
        try
        {
            #region ItemUpdated Action
            dtlLabel.Text = "DetailsView_ItemUpdated";
            #endregion ItemUpdated Action
        }
        #endregion Try Stuff
        #region Catch Stuff
        catch (Exception ex)
        {
            Error_Catch(ex, "NewVoid", msgDebug);
        }
        #endregion Catch Stuff
        if (msgLog.Length > 0) msgResults.Text += String.Format("<br />{0}", msgLog);
    }
    protected void DetailsView_UpdateRecord(SqlCommand cmd)
    {
        string msgLog = "";
        #region Try Stuff
        try
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
                                    WriteToLabel("add", "Blue", "<br />" + sqlRdr[0].ToString(), lblItemMsg);
                                }
                                catch
                                {
                                    WriteToLabel("add", "Red", "<br />" + "oops?", lblItemMsg);
                                }
                            }
                        }
                        else
                        {
                            WriteToLabel("add", "Red", "<br />" + "No Rows", lblItemMsg);
                        }
                    }
                }
            }
            #endregion Using: SqlConnection
        }
        #endregion Try Stuff
        #region Catch Stuff
        catch (Exception ex)
        {
            Error_Catch(ex, "NewVoid", msgDebug);
        }
        #endregion Catch Stuff
        if (msgLog.Length > 0) msgResults.Text += String.Format("<br />{0}", msgLog);
    }
    // DetailsView_ItemInserting
    protected void DetailsView_ItemInserting(object sender, DetailsViewInsertEventArgs e)
    {
        String msgLog = "";
        bool err = false;
        try
        {
            DetailsView dv = (DetailsView)sender;
            bool doprocess = false;
            msgResults.Text = "<br />Attempting Insert";
            Int32 sp_dnisid = 0;
            DateTime sp_createdate = DateTime.UtcNow;
            #region Get Values
            Int32 sp_sort = Convert.ToInt32(((TextBox)dv.FindControl("sort")).Text.Trim());
            String sp_displayname = ((TextBox)dv.FindControl("displayname")).Text.Trim();
            String sp_fundcode = ((TextBox)dv.FindControl("fundcode")).Text;
            String sp_status = ((DropDownList)dv.FindControl("status")).SelectedValue;
            String sp_name = ((TextBox)dv.FindControl("name")).Text.Trim();
            String sp_name_spanish = ((TextBox)dv.FindControl("name_spanish")).Text.Trim();
            Boolean sp_status_online = ((CheckBox)dv.FindControl("status_online")).Checked;
            Boolean sp_status_adu = ((CheckBox)dv.FindControl("status_adu")).Checked;
            String sp_continue = ((TextBox)dv.FindControl("continue")).Text.Trim();
            String sp_description = ((TextBox)dv.FindControl("description")).Text.Trim();
            String sp_description_spanish = ((TextBox)dv.FindControl("description_spanish")).Text.Trim();
            String sp_agentnote_top = ((TextBox)dv.FindControl("agentnote_top")).Text.Trim();
            String sp_agentnote_bottom = ((TextBox)dv.FindControl("agentnote_bottom")).Text.Trim();
            #endregion Get Values
            msgResults.Text += "<br />Values:";
            msgResults.Text += "<br />sp_displayname:" + sp_displayname.ToString();
            msgResults.Text += "<br />sp_fundcode:" + sp_fundcode.ToString();
            msgResults.Text += "<br />sp_status:" + sp_status.ToString();
            msgResults.Text += "<br />sp_name:" + sp_name.ToString();
            msgResults.Text += "<br />sp_name_spanish:" + sp_name_spanish.ToString();
            msgResults.Text += "<br />sp_status_online:" + sp_status_online.ToString();
            msgResults.Text += "<br />sp_status_adu:" + sp_status_adu.ToString();
            msgResults.Text += "<br />sp_continue:" + sp_continue.ToString();
            msgResults.Text += "<br />sp_description:" + sp_description.ToString();
            msgResults.Text += "<br />sp_description_spanish:" + sp_description_spanish.ToString();
            msgResults.Text += "<br />sp_agentnote_top:" + sp_agentnote_top.ToString();
            msgResults.Text += "<br />sp_agentnote_bottom:" + sp_agentnote_bottom.ToString();
            doprocess = true;
            if (doprocess)
            {
                #region SQL Connection
                using (SqlConnection con = new SqlConnection(sqlStr))
                {
                    Donation_Open_Database(con);
                    bool doinsert = false;
                    bool doexists = false;
                    bool doerror = false;
                    #region SQL Command
                    using (SqlCommand cmd = new SqlCommand("", con))
                    {
                        #region Build cmdText
                        String cmdText = "";
                        cmdText = @"
IF EXISTS(SELECT TOP 1 1 FROM [dbo].[dnis] [d] WHERE [d].[name] = @sp_name)
BEGIN
	SELECT 'EXISTS' [result],'DNIS already exists' [message]
END
ELSE
BEGIN
	SELECT 'INSERT' [result],'Ok to insert' [message]
END
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
                        cmd.Parameters.Add("@sp_name", SqlDbType.VarChar, 255).Value = sp_name;
                        #endregion SQL Command Parameters
                        #region SQL Command Processing
                        var chckResults = cmd.ExecuteScalar();
                        if (chckResults != null && chckResults.ToString().Length > 0)
                        {
                            // Determine whether we can insert the record or not
                            if (chckResults.ToString() == "INSERT")
                            {
                                doinsert = true;
                            }
                            else if (chckResults.ToString() == "EXISTS")
                            {
                                doexists = true;
                            }
                            else
                            {
                                msgLog += String.Format("<li>{0}</li>", "Did not recognize message from database");
                            }
                        }
                        else
                        {
                            // Something funky will rodgers
                            doerror = true;
                            msgLog += String.Format("<li>{0}</li>", "Did not get expected result from 1st query");

                        }
                        #endregion SQL Command Processing
                    }
                    #endregion SQL Command
                    if (doinsert)
                    {
                        // We are good to insert
                        #region SQL Command
                        using (SqlCommand cmd = new SqlCommand("", con))
                        {
                            #region Build cmdText
                            String cmdText = "";
                            cmdText = @"
INSERT INTO [dbo].[dnis]
           ([DisplayName]
           ,[PageLocationID]
           ,[MerchantID]
           ,[FundCode]
           ,[Status]
           ,[name]
           ,[status_online]
           ,[status_adu]
           ,[continue]
           ,[description]
           ,[agentnote_top]
           ,[agentnote_bottom]
           ,[sort])
     VALUES
		(@sp_displayname
		,@sp_pageLocationid
		,@sp_merchantid
		,@sp_fundcode
		,@sp_status
		,@sp_name
		,@sp_status_online
		,@sp_status_adu
		,@sp_continue
		,@sp_description
		,@sp_agentnote_top
		,@sp_agentnote_bottom
		,@sp_sort
		)
SELECT SCOPE_IDENTITY()
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
                            int sp_pageLocationid = 52; // legacy
                            string sp_merchantid = "069981"; // legacy
                            cmd.Parameters.Add("@sp_displayname", SqlDbType.VarChar, 100).Value = sp_displayname;
                            cmd.Parameters.Add("@sp_pageLocationid", SqlDbType.Int).Value = sp_pageLocationid;
                            cmd.Parameters.Add("@sp_merchantid", SqlDbType.VarChar, 10).Value = sp_merchantid;
                            cmd.Parameters.Add("@sp_fundcode", SqlDbType.VarChar, 7).Value = sp_fundcode;
                            cmd.Parameters.Add("@sp_status", SqlDbType.VarChar, 1).Value = sp_status;
                            cmd.Parameters.Add("@sp_name", SqlDbType.VarChar, 255).Value = sp_name;
                            cmd.Parameters.Add("@sp_status_online", SqlDbType.Bit).Value = sp_status_online;
                            cmd.Parameters.Add("@sp_status_adu", SqlDbType.Bit).Value = sp_status_adu;
                            cmd.Parameters.Add("@sp_continue", SqlDbType.VarChar, 10).Value = sp_continue;
                            cmd.Parameters.Add("@sp_description", SqlDbType.VarChar, 4000).Value = sp_description;
                            cmd.Parameters.Add("@sp_agentnote_top", SqlDbType.VarChar, 4000).Value = sp_agentnote_top;
                            cmd.Parameters.Add("@sp_agentnote_bottom", SqlDbType.VarChar, 4000).Value = sp_agentnote_bottom;
                            cmd.Parameters.Add("@sp_sort", SqlDbType.Int).Value = sp_sort;
                            cmd.Parameters.Add("@sp_authorid", SqlDbType.Int).Value = Session["userid"].ToString();
                            #endregion SQL Command Parameters
                            #region SQL Command Processing
                            var chckResults = cmd.ExecuteScalar();
                            if (chckResults != null && chckResults.ToString() != "0")
                            {
                                // We inserted the ticket
                                sp_dnisid = Convert.ToInt32(chckResults.ToString());
                                msgLog += String.Format("<li>{0}</li>", "Inserted DNIS.");
                            }
                            else
                            {
                                // There was a problem inserting the ticket
                                sp_dnisid = -1;
                                err = true;
                                msgLog += String.Format("<li>{0}</li>", "Failed to get a DNIS id.");
                            }
                            #endregion SQL Command Processing

                        }
                        #endregion SQL Command
                        if (sp_dnisid > 0)
                        {
                            int sp_itemtypeid = 0;
                            int sp_languageid = 0; // Spanish
                            // Insert the Item Description(s)
                            if (sp_name_spanish.Length > 0)
                            {
                                sp_itemtypeid = 1; // DNIS Name
                                #region SQL Command
                                using (SqlCommand cmd = new SqlCommand("", con))
                                {
                                    #region Build cmdText
                                    String cmdText = "";
                                    cmdText = @"
INSERT INTO [dbo].[item_description]
           ([itemtypeid]
           ,[itemid]
           ,[languageid]
           ,[description]
           ,[modifieddate])
     VALUES
		(@sp_itemtypeid
		,@sp_itemid
		,@sp_languageid
		,@sp_item_description
		,@sp_modifieddate
		)
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
                                    cmd.Parameters.Add("@sp_itemtypeid", SqlDbType.Int).Value = sp_itemtypeid;
                                    cmd.Parameters.Add("@sp_itemid", SqlDbType.Int).Value = sp_dnisid;
                                    cmd.Parameters.Add("@sp_languageid", SqlDbType.Int).Value = sp_languageid;
                                    cmd.Parameters.Add("@sp_item_description", SqlDbType.VarChar, 8000).Value = sp_name_spanish;
                                    cmd.Parameters.Add("@sp_modifieddate", SqlDbType.DateTime).Value = DateTime.UtcNow;
                                    #endregion SQL Command Parameters
                                    #region SQL Command Processing
                                    int records = cmd.ExecuteNonQuery();
                                    if (records == 1)
                                    {
                                        // We inserted the record
                                        msgLog += String.Format("<li>{0}</li>", "Inserted Spanish Name.");
                                    }
                                    else
                                    {
                                        // There was a problem inserting the ticket
                                        err = true;
                                        msgLog += String.Format("<li>{0}</li>", "FAILED to insert Spanish Name.");
                                    }
                                    #endregion SQL Command Processing
                                }
                                #endregion SQL Command
                            }
                            if (sp_description_spanish.Length > 0)
                            {
                                sp_itemtypeid = 2; // DNIS Description
                                #region SQL Command
                                using (SqlCommand cmd = new SqlCommand("", con))
                                {
                                    #region Build cmdText
                                    String cmdText = "";
                                    cmdText = @"
INSERT INTO [dbo].[item_description]
           ([itemtypeid]
           ,[itemid]
           ,[languageid]
           ,[description]
           ,[modifieddate])
     VALUES
		(@sp_itemtypeid
		,@sp_itemid
		,@sp_languageid
		,@sp_item_description
		,@sp_modifieddate
		)
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
                                    cmd.Parameters.Add("@sp_itemtypeid", SqlDbType.Int).Value = sp_itemtypeid;
                                    cmd.Parameters.Add("@sp_itemid", SqlDbType.Int).Value = sp_dnisid;
                                    cmd.Parameters.Add("@sp_languageid", SqlDbType.Int).Value = sp_languageid;
                                    cmd.Parameters.Add("@sp_item_description", SqlDbType.VarChar, 8000).Value = sp_description_spanish;
                                    cmd.Parameters.Add("@sp_modifieddate", SqlDbType.DateTime).Value = DateTime.UtcNow;
                                    #endregion SQL Command Parameters
                                    #region SQL Command Processing
                                    int records = cmd.ExecuteNonQuery();
                                    if (records == 1)
                                    {
                                        // We inserted the record
                                        msgLog += String.Format("<li>{0}</li>", "Inserted Spanish Description.");
                                    }
                                    else
                                    {
                                        // There was a problem inserting the ticket
                                        err = true;
                                        msgLog += String.Format("<li>{0}</li>", "FAILED to insert Spanish Description.");
                                    }
                                    #endregion SQL Command Processing
                                }
                                #endregion SQL Command
                            }
                        }
                    }
                    if (doexists)
                    {
                        // The record already exists.. let them know
                        msgLog += String.Format("<li>{0}</li>", "This DNIS already exists.");
                    }
                    if (doerror)
                    {
                        err = true;
                        msgLog += String.Format("<li>{0}</li>", "Error trying to remove lock");
                    }
                }
                #endregion SQL Connection
            }
            if (!err && sp_dnisid > 0)
            {
                // Success...
                dv.ChangeMode(DetailsViewMode.ReadOnly);
                dv.DataBind();
                DNIS_List_Refresh();
                DNIS_List_Select_DNIS(sp_dnisid);
            }

        }
        catch (Exception ex)
        {
            Error_Catch(ex, "DetailsView_ItemUpdating", lblErrorMsg);

        }
    }
    protected void DetailsView_ItemInserted(object sender, EventArgs e)
    {
        string msgLog = "";
        #region Try Stuff
        try
        {
            #region ItemInserted
            msgResults.Text = "<br />Insert Complete3";
            #endregion ItemInserted
        }
        #endregion Try Stuff
        #region Catch Stuff
        catch (Exception ex)
        {
            Error_Catch(ex, "DetailsView_ItemInserted", msgDebug);
        }
        #endregion Catch Stuff
        if (msgLog.Length > 0) msgResults.Text += String.Format("<br />{0}", msgLog);
    }
    protected bool eval_checkbox_checked(object eval)
    {
        bool rtrn = false;
        if (eval != null)
        {
            if (eval.ToString() == "True")
            {
                rtrn = true;
            }

        }
        return rtrn;
    }
    protected bool refund_visible_label(string status)
    {
        bool rtrn = true;
        if ((this.Page.User.Identity.Name == "nciambotti@greenwoodhall.com"
            || this.Page.User.Identity.Name == "cstevenson@greenwoodhall.com"
            )
            && (status == "Approved" || status == "Settled")
            )
        {
            rtrn = false;
        }
        return rtrn;
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
    protected void print_sql(SqlCommand cmd, String type)
    {
        ghFunctions.print_sql(cmd, sqlPrint, type);
        //lblGraphStatsHeaderNote.Text = "Last Refreshed: " + DateTime.Now.ToString("hh:mm:ss tt");
    }
    protected void Donation_Open_Database(SqlConnection con)
    {
        bool trySql = true;
        while (trySql)
        {
            try
            {
                if (con.State != ConnectionState.Open) { con.Close(); con.Open(); }
                trySql = false;
            }
            catch (Exception ex)
            {
                if (ex.Message.ToLower().Contains("timeout") || ex.Message.ToLower().Contains("time out"))
                {
                    // Pause .5 seconds and try again
                    System.Threading.Thread.Sleep(1000);
                }
                else
                {
                    // throw the exception
                    trySql = false;
                    throw ex;
                }
            }
        }
    }
}
