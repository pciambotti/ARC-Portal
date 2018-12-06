using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;

public partial class Search_Old : System.Web.UI.Page
{
    protected void Page_PreInit(Object sender, EventArgs e)
    {
        Master.PageTitle = "Search";
        this.Title = String.Format("{0}: {1}", Master.MyTitle, Page.Title);
    }
    private String sqlStr = Connection.GetConnectionString("Default", ""); // Controlled by Web Config ARC_Live | ARC_Stage
    protected void Page_Load(object sender, EventArgs e)
    {
        //SqlDataSource1.ConnectionString = Connection.GetConnectionString("Default", "");
        //SqlDataSource2.ConnectionString = Connection.GetConnectionString("Default", "");
        if (!IsPostBack)
        {
            GridView_Data(0, this.Page.User.Identity.Name, GridView1);
            DDL_Load_Dispositions();
            DDL_Load_Designation();
            DDL_Load_DNIS();
        }
        // Multi Select
        // http://www.erichynds.com/jquery/jquery-ui-multiselect-widget/
        // http://www.erichynds.com/examples/jquery-ui-multiselect-widget/demos/#position

        // http://www.abeautifulsite.net/blog/2008/04/jquery-multiselect/
        // 

    }
    protected bool refund_visible_label(string status)
    {
        //Eval("status").ToString() == "Approved" ? false : Eval("status").ToString() == "Settled" ? false : true
        //Eval("status").ToString() == "Approved" || Eval("status").ToString() == "Settled" ? true : false
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
    protected bool refund_visible_link(string status)
    {
        bool rtrn = false;
        if ((this.Page.User.Identity.Name == "nciambotti@greenwoodhall.com"
            || this.Page.User.Identity.Name == "cstevenson@greenwoodhall.com"
            )
            && (status == "Approved" || status == "Settled")
            )
        {
            rtrn = true;
        }
        return rtrn;
    }
    protected void GridView_Refresh(object sender, EventArgs e)
    {
        try
        {
            Label8.Text = "Test1";
            Error_General.Text = "";
            GridView1.SelectedIndex = -1;
            GridView_Data(0, this.Page.User.Identity.Name, GridView1);
            Label8.Text += "<br />Test2";
            DetailsView_Clear();

            if (ddlDNISList.SelectedIndex != -1 && ddlDNISList.SelectedValue.Length > 0)
            {
                lblResponse.Text = "";
                foreach (ListItem li in ddlDNISList.Items)
                {
                    if (li.Selected)
                    {
                        if (lblResponse.Text.Length > 0)
                            lblResponse.Text += ", " + li.Value.ToString();
                        else
                            lblResponse.Text += li.Value.ToString();
                    }
                }
                lblResponse.Text = "DNIS List: " + lblResponse.Text;
            }
                
        }
        catch (Exception ex)
        {
            Label8.Text = "Oops";
            Error_Save(ex, "GridView_Refresh");
        }
    }
    protected void GridView_Export_Excel(object sender, EventArgs e)
    {
        //GridView_DataBound(GridView1);
        //lblAdminNotes.Text = GridView1.Controls[0].Controls[0].ToString();
        GridViewExportUtil.Export("ARC-Portal-Search.xls", this.GridView2);
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
                cmdText = "[dbo].[portal_call_search_get_list]";
                //sp_error_log_sql_list
                cmd.CommandText = cmdText;
                cmd.CommandType = CommandType.StoredProcedure;
                #region SQL Parameters
                cmd.Parameters.Clear();
                cmd.Parameters.Add(new SqlParameter("@sp_source", "Live"));
                cmd.Parameters.Add(new SqlParameter("@sp_top", ddlTop.SelectedValue));
                //dtRecurringDate
                if (dtRecurringDate.Text.Length > 0)
                {
                    cmd.Parameters.Add(new SqlParameter("@sp_recurringdate", dtRecurringDate.Text));

                }
                else
                {
                    cmd.Parameters.Add(new SqlParameter("@sp_startdate", dtStartDate.Text));
                    cmd.Parameters.Add(new SqlParameter("@sp_starttime", dtStartTime.Text));
                    cmd.Parameters.Add(new SqlParameter("@sp_enddate", dtEndDate.Text));
                    cmd.Parameters.Add(new SqlParameter("@sp_endtime", dtEndTime.Text));
                }
                if (CallID.Text.Length > 0)
                {
                    cmd.Parameters.Add(new SqlParameter("@sp_callid", CallID.Text));
                }
                if (DonationID.Text.Length > 0)
                {
                    cmd.Parameters.Add(new SqlParameter("@sp_donationid", DonationID.Text));
                }
                if (ddlDonationType.SelectedIndex != -1 && ddlDonationType.SelectedValue.Length > 0)
                {
                    cmd.Parameters.Add(new SqlParameter("@sp_donationtype", ddlDonationType.SelectedValue));
                }
                string dsps = "";
                if (ddlDispositions.SelectedIndex != -1 && ddlDispositions.SelectedValue.Length > 0)
                {
                    foreach (ListItem li in ddlDispositions.Items)
                    {
                        if (li.Selected)
                        {
                            dsps += li.Value + ",";
                        }
                    }
                    if (dsps.EndsWith(",")) { dsps = dsps.TrimEnd(','); }
                    if (dsps.Length > 0)
                    {
                        cmd.Parameters.Add(new SqlParameter("@sp_disposition", dsps));
                    }
                }
                string dsgns = "";
                if (ddlDesignation.SelectedIndex != -1 && ddlDesignation.SelectedValue.Length > 0)
                {
                    foreach (ListItem li in ddlDesignation.Items)
                    {
                        if (li.Selected)
                        {
                            dsgns += li.Value + ",";
                        }
                    }
                    if (dsgns.EndsWith(",")) { dsgns = dsgns.TrimEnd(','); }
                    if (dsgns.Length > 0)
                    {
                        cmd.Parameters.Add(new SqlParameter("@sp_designation", dsgns));
                    }
                }
                //ddlDesignation
                if (ddlDonationSource.SelectedIndex != -1 && ddlDonationSource.SelectedValue.Length > 0)
                {
                    cmd.Parameters.Add(new SqlParameter("@sp_donationsource", ddlDonationSource.SelectedValue));
                }
                if (ddlDonorType.SelectedIndex != -1 && ddlDonorType.SelectedValue.Length > 0)
                {
                    cmd.Parameters.Add(new SqlParameter("@sp_donortype", ddlDonorType.SelectedValue));
                }

                if (Name.Text.Length > 0)
                {
                    cmd.Parameters.Add(new SqlParameter("@sp_name", Name.Text.Trim()));
                }
                if (Email.Text.Length > 0)
                {
                    cmd.Parameters.Add(new SqlParameter("@sp_email", Email.Text.Trim()));
                }
                if (Phone.Text.Length > 0)
                {
                    cmd.Parameters.Add(new SqlParameter("@sp_phone", Phone.Text.Trim()));
                }
                if (Card.Text.Length > 0)
                {
                    cmd.Parameters.Add(new SqlParameter("@sp_card", Card.Text.Trim()));
                }
                if (ddlDonationStatus.SelectedIndex != -1 && ddlDonationStatus.SelectedValue.Length > 0)
                {
                    cmd.Parameters.Add(new SqlParameter("@sp_status", ddlDonationStatus.SelectedValue));
                }
                try
                {
                    if (Amount.Text.Length > 0)
                    {
                        Convert.ToDouble(Amount.Text);
                        cmd.Parameters.Add(new SqlParameter("@sp_amount", Amount.Text));
                        cmd.Parameters.Add(new SqlParameter("@sp_amounttype", ddlAmountType.SelectedValue));
                    }
                }
                catch { }
                if (DNIS.Text.Length > 0)
                {
                    cmd.Parameters.Add(new SqlParameter("@sp_dnis", DNIS.Text.Trim()));
                }
                if (ddlDNISList.SelectedIndex != -1 && ddlDNISList.SelectedValue.Length > 0)
                {
                    cmd.Parameters.Add(new SqlParameter("@sp_dnislist", ddlDNISList.SelectedValue));
                }
                if (cbTests.Checked)
                {
                    cmd.Parameters.Add(new SqlParameter("@sp_exclude_testcc", cbTests.Checked));
                }


                if (dtStartDate.Text.Length > 0)
                {
                    rpElapsed.Text = "From " + dtStartDate.Text + " " + dtStartTime.Text + " to " + dtEndDate.Text + " " + dtEndTime.Text;
                }
                
                //rpElapsed.Text += " dispos: " + dsps;
                
                #endregion SQL Parameters
                #region Print SQL
                if (Page.User.IsInRole("System Administrator") == true && Connection.GetConnectionType() == "Local")
                {
                    String sqlToText = "";
                    sqlToText += cmd.CommandText.ToString();
                    foreach (SqlParameter p in cmd.Parameters)
                    {
                        sqlToText += "<br />" + p.ParameterName + " = " + p.Value.ToString() + " [" + p.DbType.ToString() + "]";
                    }
                    sqlPrint.Text = "<br />" + sqlToText;
                }
                #endregion Print SQL
                #region SQL Processing
                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                ad.Fill(dt);
                gv.DataSource = dt;
                gv.DataBind();
                //dtlLabel.Text += "<br />" + gv.ID;
                if (dt.Rows.Count > 0)
                {
                    btnExport.Visible = true;
                    GridView2.DataSource = dt;
                    GridView2.DataBind();
                }
                #endregion SQL Processing
            }
            #endregion SQL Command
        }
        #endregion SQL Connection
    }
    protected void GridView_DataBound(Object sender, EventArgs e)
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

            e.Row.Attributes["onclick"] = Page.ClientScript.GetPostBackClientHyperlink(this.GridView1, "Select$" + e.Row.RowIndex);
        }
    }
    protected void GridView_IndexChanged_Old(object sender, EventArgs e)
    {
        dtlLabel.Text = "Feature not enabled: " + DateTime.Now.ToString("HH:mm:ss");
    }
    protected void GridView_IndexChanged(object sender, EventArgs e)
    {
        dtlLabel.Text = GridView1.SelectedIndex.ToString();
        //dtlLabel.Text += " [" + GridView1.SelectedDataKey.Values[0].ToString();
        //dtlLabel.Text += " [" + GridView1.SelectedDataKey.Values[1].ToString();
        try
        {
            Int32 callid = Convert.ToInt32(GridView1.SelectedDataKey.Values[0].ToString());
            Int32 donorid = 0; // Convert.ToInt32(GridView1.SelectedDataKey.Values[1].ToString());
            DetailsView_Data(callid, donorid, dvCallDetails);
            DetailsView_Data(callid, donorid, dvDonorDetails);
            DetailsView_Data(callid, donorid, dvTributeDetails);
            DetailsView_Data(callid, donorid, dvPaymentDetails);
            DetailsView_Data(callid, donorid, dvContactDetails);
            DetailsView_Data(callid, donorid, dvRefundDetails);
            DetailsView_Data(callid, donorid, dvADUFile);
            DetailsView_Data(callid, donorid, dvRemoveDetails);
            DetailsView_Data(callid, donorid, dvGiftDetails);
            GridView_Data2(callid, donorid, gvGiftList, lblGiftList);
            DetailsView_Data(callid, donorid, dvSustainerDetails);
            GridView_Data2(callid, donorid, gvRecurringList, lblRecurringList);
        }
        catch (Exception ex)
        {
            Error_Save(ex, "DetailsView Data Error");
        }
    }
    protected void GridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        //Session["EventList_GridView_SelectedIndex"] = null;
        //Session["EventList_GridView_PageIndex"] = null;
        Label8.Text = e.NewPageIndex.ToString();
        GridView1.SelectedIndex = -1;
        GridView1.PageIndex = e.NewPageIndex;
        GridView_Data(0, this.Page.User.Identity.Name, GridView1);
    }
    protected void GridView_PageDropDownList_SelectedIndexChanged(Object sender, EventArgs e)
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
        if (dvCallDetails.Rows.Count > 0) dvCallDetails.DataBind();
        if (dvDonorDetails.Rows.Count > 0) dvDonorDetails.DataBind();
        if (dvTributeDetails.Rows.Count > 0) dvTributeDetails.DataBind();
        if (dvPaymentDetails.Rows.Count > 0) dvPaymentDetails.DataBind();
        if (dvContactDetails.Rows.Count > 0) dvContactDetails.DataBind();
        if (dvRefundDetails.Rows.Count > 0) dvRefundDetails.DataBind();
        if (dvADUFile.Rows.Count > 0) dvADUFile.DataBind();
        if (dvGiftDetails.Rows.Count > 0) dvGiftDetails.DataBind();
        if (gvGiftList.Rows.Count > 0) gvGiftList.DataBind();
        if (dvSustainerDetails.Rows.Count > 0) dvSustainerDetails.DataBind();
        if (gvRecurringList.Rows.Count > 0) gvRecurringList.DataBind();
        lblGiftList.Visible = false;
        lblRecurringList.Visible = false;


    }
    protected void DDL_Load_Dispositions()
    {
        ListBox ddl = ddlDispositions;
        #region Overall Try
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
                    cmd.CommandText = "[dbo].[portal_call_search_get_ddl_dispositions]";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Clear();
                    #region SQL Parameters
                    #endregion SQL Parameters
                    #region SQL Processing
                    SqlDataAdapter ad = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    ad.Fill(dt);

                    ddl.DataTextField = "Text";
                    ddl.DataValueField = "Value";

                    ddl.Items.Clear();
                    ddl.DataSource = dt;
                    ddl.DataBind();

                    #endregion SQL Processing
                }
                #endregion SQL Command
            }
            #endregion SQL Connection
        }
        #endregion Overall Try
        #region Overall Catch
        catch (Exception ex)
        {
            Error_Save(ex, "DDL Projects");
        }
        #endregion Overall Catch
    }
    protected void DDL_Load_Designation()
    {
        ListBox ddl = ddlDesignation;
        #region Overall Try
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
                    cmd.CommandText = "[dbo].[portal_call_search_get_ddl_designation]";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Clear();
                    #region SQL Parameters
                    #endregion SQL Parameters
                    #region SQL Processing
                    SqlDataAdapter ad = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    ad.Fill(dt);

                    ddl.DataTextField = "Text";
                    ddl.DataValueField = "Value";

                    ddl.Items.Clear();
                    ddl.DataSource = dt;
                    ddl.DataBind();

                    #endregion SQL Processing
                }
                #endregion SQL Command
            }
            #endregion SQL Connection
        }
        #endregion Overall Try
        #region Overall Catch
        catch (Exception ex)
        {
            Error_Save(ex, "DDL Projects");
        }
        #endregion Overall Catch
    }
    protected void DDL_Load_DNIS()
    {
        ListBox ddl = ddlDNISList;
        #region Overall Try
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
                    cmd.CommandText = "[dbo].[portal_call_search_get_ddl_dnis]";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Clear();
                    #region SQL Parameters
                    #endregion SQL Parameters
                    #region SQL Processing
                    SqlDataAdapter ad = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    ad.Fill(dt);

                    ddl.DataTextField = "Text";
                    ddl.DataValueField = "Value";

                    ddl.Items.Clear();
                    ddl.DataSource = dt;
                    ddl.DataBind();

                    #endregion SQL Processing
                }
                #endregion SQL Command
            }
            #endregion SQL Connection
        }
        #endregion Overall Try
        #region Overall Catch
        catch (Exception ex)
        {
            Error_Save(ex, "DDL Projects");
        }
        #endregion Overall Catch
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
    protected void DetailsView_Data(Int32 CallID, Int32 DonorID, DetailsView dv)
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
                if (dv.ID == "dvCallDetails") { cmdText = "[dbo].[portal_call_search_get_details]"; }
                if (dv.ID == "dvDonorDetails") { cmdText = "[dbo].[portal_call_search_get_donor]"; }
                if (dv.ID == "dvTributeDetails") { cmdText = "[dbo].[portal_call_search_get_tribute]"; }
                if (dv.ID == "dvPaymentDetails") { cmdText = "[dbo].[portal_call_search_get_payment]"; }
                if (dv.ID == "dvContactDetails") { cmdText = "[dbo].[portal_call_search_get_contact]"; }
                if (dv.ID == "dvRefundDetails") { cmdText = "[dbo].[portal_call_search_get_refund]"; }
                if (dv.ID == "dvADUFile") { cmdText = "[dbo].[portal_call_search_get_adufile]"; }
                if (dv.ID == "dvRemoveDetails") { cmdText = "[dbo].[portal_call_search_get_remove]"; }
                if (dv.ID == "dvGiftDetails") { cmdText = "[dbo].[portal_call_search_get_gift_details]"; }
                if (dv.ID == "dvSustainerDetails") { cmdText = "[dbo].[portal_call_search_get_sustainer_details]"; }


                cmd.CommandText = cmdText;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                #region SQL Parameters
                cmd.Parameters.Add(new SqlParameter("@sp_callid", CallID));
                
                if (dv.ID == "dvPaymentDetails")
                {
                    if (GridView1.SelectedDataKey.Values[1].ToString().Length > 0)
                    {
                        Int32 cbid = Convert.ToInt32(GridView1.SelectedDataKey.Values[1].ToString());
                        cmd.Parameters.Add(new SqlParameter("@sp_cbid", cbid));
                    }

                }
                #endregion SQL Parameters
                #region SQL Processing
                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                ad.Fill(dt);
                dv.DataSource = dt; //dvCallDetails.DataSource = dt;
                dv.DataBind(); //dvCallDetails.DataBind();
                dtlLabel.Text += "<br />" + dv.ID;
                #endregion SQL Processing

            }
            #endregion SQL Command
        }
        #endregion SQL Connection
    }
    
    protected void GridView_Data2(Int32 CallID, Int32 DonorID, GridView gv, System.Web.UI.HtmlControls.HtmlGenericControl lbl)
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
                if (gv.ID == "gvGiftList") { cmdText = "[dbo].[portal_call_search_get_gift_list]"; }
                if (gv.ID == "gvRecurringList") { cmdText = "[dbo].[portal_call_search_get_recurring_list]"; }

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
                dtlLabel.Text += "<br />" + gv.ID + "|" + gv.Rows.Count.ToString();
                if (gv.Rows.Count > 0) { lbl.Visible = true; gv.Visible = true; }
                else
                {
                    lbl.Visible = false;
                    gv.Visible = false;
                }

                #endregion SQL Processing

            }
            #endregion SQL Command
        }
        #endregion SQL Connection
    }
    #endregion Details View Handling

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
    }

}
