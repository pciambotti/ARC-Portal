using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;

public partial class Reporting_Daily : System.Web.UI.Page
{
    protected void Page_PreInit(Object sender, EventArgs e)
    {
        Master.PageTitle = "Reporting Daily";
        this.Title = String.Format("{0}: {1}", Master.MyTitle, Page.Title);
    }
    private String sqlStr = Connection.GetConnectionString("Default", ""); // Controlled by Web Config ARC_Live | ARC_Stage
    protected void Page_Load(object sender, EventArgs e)
    {
        //SqlDataSource1.ConnectionString = Connection.GetConnectionString("Default", "");
        //SqlDataSource2.ConnectionString = Connection.GetConnectionString("Default", "");
        if (!IsPostBack)
        {
            //GridView_Data(0, this.Page.User.Identity.Name, GridView1);
        }
        Label lblPower = (Label)Master.FindControl("lblPower");

        if (lblPower != null)
        {
            lblPower.Text = "Powered by Greenwood & Hall";
        }
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
        }
        catch (Exception ex)
        {
            Label8.Text = "Oops";
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
                cmdText = "[dbo].[portal_donation_search_get_list]";
                cmdText = "[dbo].[portal_donation_totals]";
                cmdText = "[dbo].[portal_custom_daily_file]";
                //sp_error_log_sql_list
                cmd.CommandText = cmdText;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                #region SQL Parameters
                ////cmd.Parameters.Add(new SqlParameter("@SP_UserID", UserID));
                ////cmd.Parameters.Add(new SqlParameter("@SP_UserName", UserName));
                //cmd.Parameters.Add(new SqlParameter("@Source", "Live"));
                //cmd.Parameters.Add(new SqlParameter("@SP_Top", ddlTop.SelectedValue));
                //cmd.Parameters.Add(new SqlParameter("@SP_StartDate", dtStartDate.Text));
                //cmd.Parameters.Add(new SqlParameter("@SP_StartTime", dtStartTime.Text));
                //cmd.Parameters.Add(new SqlParameter("@SP_EndDate", dtEndDate.Text));
                //cmd.Parameters.Add(new SqlParameter("@SP_EndTime", dtEndTime.Text));

                //cmd.Parameters.Add(new SqlParameter("@SP_Confirmation", Confirmation.Text));
                //cmd.Parameters.Add(new SqlParameter("@SP_DonationType", ddlDonationType.SelectedValue));
                //cmd.Parameters.Add(new SqlParameter("@SP_DonationSource", ddlDonationSource.SelectedValue));
                //cmd.Parameters.Add(new SqlParameter("@SP_DonationStatus", ddlDonationStatus.SelectedValue));
                //cmd.Parameters.Add(new SqlParameter("@SP_DonorType", ddlDonorType.SelectedValue));

                //cmd.Parameters.Add(new SqlParameter("@SP_Name", Name.Text));
                //cmd.Parameters.Add(new SqlParameter("@SP_Email", Email.Text));
                //cmd.Parameters.Add(new SqlParameter("@SP_Phone", Phone.Text));
                //cmd.Parameters.Add(new SqlParameter("@SP_Card", Card.Text));

                //try
                //{
                //    Convert.ToDouble(Amount.Text);
                //    cmd.Parameters.Add(new SqlParameter("@SP_Amount", Amount.Text));
                //    cmd.Parameters.Add(new SqlParameter("@SP_AmountType", ddlAmountType.SelectedValue));
                //}
                //catch { }
                #endregion SQL Parameters
                #region SQL Processing
                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                ad.Fill(dt);
                gv.DataSource = dt; //DetailsView1.DataSource = dt;
                gv.DataBind(); //DetailsView1.DataBind();
                //dtlLabel.Text += "<br />" + gv.ID;
                #endregion SQL Processing
            }
            #endregion SQL Command
        }
        #endregion SQL Connection
    }
    protected void GridView1_DataBound(Object sender, EventArgs e)
    {
        Label8.Text = " Records: [" + GridView1.Rows.Count.ToString() + "]";
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
            Int32 donationid = Convert.ToInt32(GridView1.SelectedDataKey.Values[0].ToString());
            Int32 donorid = Convert.ToInt32(GridView1.SelectedDataKey.Values[1].ToString());
            DetailsView_Data(donationid, donorid, DetailsView1);
            DetailsView_Data(donationid, donorid, DetailsView2);
            DetailsView_Data(donationid, donorid, DetailsView3);
            DetailsView_Data(donationid, donorid, DetailsView4);
            DetailsView_Data(donationid, donorid, DetailsView5);
        }
        catch (Exception ex)
        {
            Error_Save(ex, "DetailsView Data Error");
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
    protected void DetailsView_Data(Int32 DonationID, Int32 DonorID, DetailsView dv)
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
                if (dv.ID == "DetailsView1") { cmdText = "[dbo].[portal_donation_search_get_details]"; }
                if (dv.ID == "DetailsView2") { cmdText = "[dbo].[portal_donation_search_get_donor]"; }
                if (dv.ID == "DetailsView3") { cmdText = "[dbo].[portal_donation_search_get_tribute]"; }
                if (dv.ID == "DetailsView4") { cmdText = "[dbo].[portal_donation_search_get_payment]"; }
                if (dv.ID == "DetailsView5") { cmdText = "[dbo].[portal_donation_search_get_contact]"; }
                cmd.CommandText = cmdText;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                #region SQL Parameters
                cmd.Parameters.Add(new SqlParameter("@SP_DonationID", DonationID));
                cmd.Parameters.Add(new SqlParameter("@SP_DonorID", DonorID));
                #endregion SQL Parameters
                #region SQL Processing
                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                ad.Fill(dt);
                dv.DataSource = dt; //DetailsView1.DataSource = dt;
                dv.DataBind(); //DetailsView1.DataBind();
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
        #region DataBound Action for DetailsView1
        if (dv.ID == "DetailsView1")
        {
            // Nothing
        }
        #endregion DataBound Action for DetailsView1
        #region DataBound Action for DetailsView2
        else if (dv.ID == "DetailsView2")
        {
            // Nothing
        }
        #endregion DataBound Action for DetailsView2
        #region DataBound Action for DetailsView3
        else if (dv.ID == "DetailsView3")
        {
            // Nothing
        }
        #endregion DataBound Action for DetailsView3
        #region DataBound Action for DetailsView4
        else if (dv.ID == "DetailsView4")
        {
            // Populate the State/Country Drop Down
            #region State/Country Populate if Edit Mode
            if (DetailsView4.CurrentMode == DetailsViewMode.Edit)
            {
                Label lblState = (Label)DetailsView4.FindControl("State");

                DropDownList dvState = (DropDownList)DetailsView4.FindControl("ddlState");
                DropDownList dvCountry = (DropDownList)DetailsView4.FindControl("ddlCountry");
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
                        Error_Save(ex, "DetailsView4 - DDL State/Country Populate Error");
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
                        Error_Save(ex, "DetailsView4 - DDL State Populate Error");
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
                        Error_Save(ex, "DetailsView4 - DDL Country Populate Error");
                    }
                }
                else
                {
                    //dtlLabel.Text = "Did not find DDL";
                    #region DeBug Code
                    //dtlLabel.Text += " [" + DetailsView4.Rows.Count.ToString() + "]";
                    //foreach (DetailsViewRow dvr in DetailsView4.Rows)
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
        #endregion DataBound Action for DetailsView4
        #region DataBound Action for DetailsView5
        else if (dv.ID == "DetailsView5")
        {
        }
        #endregion DataBound Action for DetailsView5
        #endregion DataBound Action
    }
    protected void DetailsView_ItemCommand(object sender, DetailsViewCommandEventArgs e)
    {
        #region ItemCommand Action
        if (e.CommandName == "Clear")
        {
            GridView1.SelectedIndex = -1;
            DetailsView1.DataBind();
            DetailsView2.DataBind();
            DetailsView3.DataBind();
            DetailsView4.DataBind();
            DetailsView5.DataBind();
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
            Int32 donationid = Convert.ToInt32(GridView1.SelectedDataKey.Values[0].ToString());
            Int32 donorid = Convert.ToInt32(GridView1.SelectedDataKey.Values[1].ToString());
            DetailsView_Data(donationid, donorid, dv);
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
        Int32 donationid = Convert.ToInt32(GridView1.SelectedDataKey.Values[0].ToString());
        Int32 donorid = Convert.ToInt32(GridView1.SelectedDataKey.Values[1].ToString());
        DetailsView_Data(donationid, donorid, dv);
    }
    protected void DetailsView_ItemUpdating(object sender, DetailsViewUpdateEventArgs e)
    {
        Boolean update = false;
        String error = "";
        #region ItemUpdating Action
        DetailsView dv = (DetailsView)sender;
        #region ItemUpdating Action for DetailsView1
        if (dv.ID == "DetailsView1")
        {
            #region Try: DetailsView1 Update
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
            #endregion Try: DetailsView1 Update
            #region Catch: DetailsView1 Update
            catch (Exception ex)
            {
                Error_Save(ex, "Update User: User Credentials");
                update = false;
                error = "Internal server error during update process.";
            }
            #endregion Catch: DetailsView1 Update
        }
        #endregion ItemUpdating Action for DetailsView1
        #region ItemUpdating Action for DetailsView2
        else if (dv.ID == "DetailsView2")
        {
            #region Try: DetailsView2 Update
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
            #endregion Try: DetailsView2 Update
            #region Catch: DetailsView2 Update
            catch (Exception ex)
            {
                Error_Save(ex, "Update User: User Details");
                update = false;
                error = "Internal server error during update process.";
            }
            #endregion Catch: DetailsView2 Update
        }
        #endregion ItemUpdating Action for DetailsView2
        #region ItemUpdating Action for DetailsView3
        else if (dv.ID == "DetailsView3")
        {
            #region Try: DetailsView3 Update
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
            #endregion Try: DetailsView3 Update
            #region Catch: DetailsView3 Update
            catch (Exception ex)
            {
                Error_Save(ex, "Update User: User Phone");
                update = false;
                error = "Internal server error during update process.";
            }
            #endregion Catch: DetailsView3 Update
        }
        #endregion ItemUpdating Action for DetailsView3
        #region ItemUpdating Action for DetailsView4
        else if (dv.ID == "DetailsView4")
        {
            #region Try: DetailsView4 Update
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
            #endregion Try: DetailsView4 Update
            #region Catch: DetailsView4 Update
            catch (Exception ex)
            {
                Error_Save(ex, "Update User: User Address");
                update = false;
                error = "Internal server error during update process.";
            }
            #endregion Catch: DetailsView4 Update

        }
        #endregion ItemUpdating Action for DetailsView4
        if (update)
        {
            dv.ChangeMode(DetailsViewMode.ReadOnly);
            Int32 donationid = Convert.ToInt32(GridView1.SelectedDataKey.Values[0].ToString());
            Int32 donorid = Convert.ToInt32(GridView1.SelectedDataKey.Values[1].ToString());
            DetailsView_Data(donationid, donorid, dv);
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
    }

}
