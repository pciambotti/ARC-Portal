using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using ClosedXML.Excel;
using ClosedXML.Excel.Drawings;
using System.IO;
using System.Net;
using System.Configuration;
public partial class Search : System.Web.UI.Page
{
    private int tempRow = 0;
    private String sqlStrARC = Connection.GetConnectionString("ARC_Production", ""); // ARC_Production || ARC_Stage
    private String sqlStrDE = Connection.GetConnectionString("DE_Production", ""); // DE_Production || DE_Stage
    private String sqlStrInt = Connection.GetConnectionString("PS_Production", ""); // PS_Production | PS_Stage
    private String sqlStrRec = Connection.GetConnectionString("RECORDINGS", ""); // 
    protected void Page_PreInit(Object sender, EventArgs e)
    {
        Master.PageTitle = "Search";
        this.Title = String.Format("{0}: {1}", Master.MyTitle, Page.Title);
        // if (!Request.IsSecureConnection && !Request.IsLocal && !Request.Url.ToString().Contains("192.168.") && !Request.Url.ToString().Contains("mylocal"))
        if (Request.Url.ToString().Contains("portalstage") || Request.IsLocal || Request.Url.ToString().Contains("192.168."))
        {
            cbTests.Checked = false;
        }
        if (Connection.GetDBMode() == "Stage")
        {
            sqlStrARC = Connection.GetConnectionString("ARC_Stage", ""); // ARC_Production || ARC_Stage
            sqlStrDE = Connection.GetConnectionString("DE_Stage", ""); // DE_Production || DE_Stage
            sqlStrInt = Connection.GetConnectionString("PS_Stage", ""); // PS_Production | PS_Stage
        }
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
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (Page.User.IsInRole("System Administrator") == true && Request.IsLocal)
            {
                ddlSort.SelectedIndex = 1;
            }

            rpTimeZone.Text += "-" + ghFunctions.dtUserOffSet.ToString() + " (US Eastern Timezone)";

            dtStartDate.Text = DateTime.UtcNow.AddHours(-ghFunctions.dtUserOffSet).AddDays(-0).ToString("MM/dd/yyyy");
            dtStartTime.Text = "00:00";
            dtEndDate.Text = DateTime.UtcNow.AddHours(-ghFunctions.dtUserOffSet).AddDays(+0).ToString("MM/dd/yyyy");
            dtEndTime.Text = "23:59";

            if (Page.User.IsInRole("System Administrator")) { Load_Checklist(); }
            DDL_Load_DNIS();
            DDL_Load_Dispositions();
            DDL_Load_Designation();
            DDL_Load_Refund_Reason();
            DDL_Load_Charge_Reason();
            if (Page.User.IsInRole("System Administrator") == true && Request.IsLocal)
            {
                dtStartDate.Text = DateTime.Now.AddDays(-7).ToString("MM/dd/yyyy");
                //if (ddlDispositions.Items.Count > 0) { ddlDispositions.SelectedIndex = 0; }
            }
            if (ghUser.identity_is_admin())
            {
                filter_call_length.Visible = true;

                ListItem li = new ListItem();
                li.Text = "Cancelled Settled";
                li.Value = "CancelledSettled";
                ddlDonationStatus.Items.Add(li);
                li = new ListItem();
                li.Text = "Cancelled Settled Unique";
                li.Value = "CancelledSettledUnique";
                ddlDonationStatus.Items.Add(li);

                donation_adu.Visible = true;
            }
            #region Admin / Local Stuff - Load a Record
            if (Page.User.IsInRole("System Administrator") == true && Connection.GetConnectionType() == "Local")
            {
                //ddlDonationStatus.SelectedValue = "Error";
                //ddlDonationStatus.SelectedValue = "CancelledSettled";
                //cbTests.Checked = true;

                // ddlDonationType.SelectedIndex = 2;
                // cbTests.Checked = false;


                                
                if (gvSearchGrid.Rows.Count > 0)
                {
                    int iCell = 0;
                    for (int i = 0; i < gvSearchGrid.HeaderRow.Cells.Count; i++)
                    {
                        if (gvSearchGrid.HeaderRow.Cells[i].Text == "ID")
                        {
                            iCell = i;
                            break;
                        }
                    }
                    foreach (GridViewRow row in gvSearchGrid.Rows)
                    {
                        if (row.Cells[iCell].Text == "2952584")
                        {
                            gvSearchGrid.SelectedIndex = row.RowIndex;
                            break;
                        }
                        if (row.Cells[iCell].Text == "2952585")
                        {
                            gvSearchGrid.SelectedIndex = row.RowIndex;
                            break;
                        }
                        if (row.Cells[iCell].Text == "2952620")
                        {
                            gvSearchGrid.SelectedIndex = row.RowIndex;
                            break;
                        }
                    }
                    if (gvSearchGrid.SelectedIndex >= 0) GridView_IndexChanged(gvSearchGrid, e);
                }
            }
            #endregion Admin / Local Stuff - Load a Record

            Page_Saved_Settings();

            GridView_Data(0, this.Page.User.Identity.Name, gvSearchGrid);
            if (!IsPostBack && Request["callid"] != null)
            {
                if (gvSearchGrid.Rows.Count > 0)
                {
                    gvSearchGrid.SelectedIndex = 0;
                    if (gvSearchGrid.SelectedIndex >= 0) GridView_IndexChanged(gvSearchGrid, e);
                }
            }

            lblSystemMessage.Text += "<br /><br>GCT: " + Connection.GetConnectionType();
        }
        // Multi Select
        // http://www.erichynds.com/jquery/jquery-ui-multiselect-widget/
        // http://www.erichynds.com/examples/jquery-ui-multiselect-widget/demos/#position

        // http://www.abeautifulsite.net/blog/2008/04/jquery-multiselect/
        // 

        Error_General.Text += "<br />Client: " + Page.User.IsInRole("Client").ToString();
        Error_General.Text += "<br />CDR: " + Page.User.IsInRole("CDR Fundraising Group").ToString();
    }
    protected void Page_Saved_Settings()
    {
        if (Session["sort"] != null)
        {
            ddlSort.SelectedValue = Session["sort"].ToString();
        }
    }
    protected void Load_Checklist()
    {
        String strMessage = "";
        String strToCheck = "";
        Int32 errCode = 0;
        String errLast = "";
        Boolean oDebug = false;
        if (Connection.GetDBMode() == "Stage") oDebug = true;
        lblSystemMessage.Text = "Checking resources...";
        #region Cybersource
        strMessage += ("<br />" + Repeat('\t', 1) + "Checking Cybersource");
        strMessage += ("<br />" + Repeat('\t', 2) + "DB Mode: " + Connection.GetDBMode());
        #region Cybersource Keys
        strToCheck = ConfigurationManager.AppSettings["cybs.keysDirectory"];
        if (System.IO.Directory.Exists(strToCheck)) // checking if keys folder exists
        {
            strMessage += ("<br />" + Repeat('\t', 2) + "Key Path OK");
            string strpath = ConfigurationManager.AppSettings["cybs.keysDirectory"];
            string strfile = ConfigurationManager.AppSettings["cybs.merchantID"];
            if (System.IO.File.Exists(strpath + @"\" + strfile + ".p12"))
            {
                strMessage += ("<br />" + Repeat('\t', 2) + "Key File OK");
            }
            else
            {
                strMessage += ("<br />" + Repeat('\t', 2) + "Key File FAILED");
                errCode++;
                errLast = "Cyb Key File Failed";
            }
        }
        else
        {
            strMessage += ("<br />" + Repeat('\t', 2) + "Key Path FAILED");
            strMessage += ("<br />" + Repeat('\t', 3) + "Path: " + strToCheck);
            strMessage += ("<br />" + Repeat('\t', 3) + "PathS: " + Server.MapPath("~"));
            errCode++;
            errLast = "Cyb Key Path Failed";
        }
        #endregion Cybersource Keys
        #region Cybersource Logs
        strToCheck = ConfigurationManager.AppSettings["cybs.logDirectory"];
        if (System.IO.Directory.Exists(strToCheck)) // checking if keys folder exists
        {
            strMessage += ("<br />" + Repeat('\t', 2) + "Log Path OK");
        }
        else
        {
            strMessage += ("<br />" + Repeat('\t', 2) + "Log Path FAILED");
            strMessage += ("<br />" + Repeat('\t', 3) + "Path: " + strToCheck);
            errCode++;
            errLast = "Cyb Log Path Failed";
        }
        #endregion Cybersource Logs
        #region Cybersource Account
        // If DeBug we should be using gnh160325
        // If Live we should be using lms150201
        String cbMerchantID = "";
        String cbProduction = "";
        String cbLog = "";
        // If we are on LIVE DB we expect lms150201
        // If we are on STAGE DB we expect gnh160325
        // If we are on portal. / portalnew. we expect LIVE DB
        // If we are on portal. / portalnew. we expect STAGE DB
        if (Connection.GetDBMode() == "Live")
        {
            cbMerchantID = "lms150201";
            cbProduction = "true";
            cbLog = "false";
        }
        if (Connection.GetDBMode() == "Stage")
        {
            cbMerchantID = "gnh160325";
            cbProduction = "false";
            cbLog = "true";
        }
        #region Merch ID
        strToCheck = ConfigurationManager.AppSettings["cybs.merchantID"];
        if (strToCheck == cbMerchantID)
        {
            strMessage += ("<br />" + Repeat('\t', 2) + "MerchantID OK");
        }
        else
        {
            strMessage += ("<br />" + Repeat('\t', 2) + "MerchantID FAILED");
            strMessage += ("<br />" + Repeat('\t', 3) + "Current: " + strToCheck + " | Expected: " + cbMerchantID);
            errCode++;
            errLast = "Cyb Merch ID Failed";
        }
        #endregion Merch ID
        #region Production
        strToCheck = ConfigurationManager.AppSettings["cybs.sendToProduction"];
        if (strToCheck == cbProduction)
        {
            strMessage += ("<br />" + Repeat('\t', 2) + "Production OK");
        }
        else
        {
            strMessage += ("<br />" + Repeat('\t', 2) + "Production FAILED");
            strMessage += ("<br />" + Repeat('\t', 3) + "Current: " + strToCheck + " | Expected: " + cbProduction);
            errCode++;
            errLast = "Cyb Production Failed";
        }
        #endregion Production
        #region Log
        strToCheck = ConfigurationManager.AppSettings["cybs.enableLog"];
        if (strToCheck == cbLog)
        {
            strMessage += ("<br />" + Repeat('\t', 2) + "Log OK");
        }
        else
        {
            strMessage += ("<br />" + Repeat('\t', 2) + "Log FAILED");
            strMessage += ("<br />" + Repeat('\t', 3) + "Current: " + strToCheck + " | Expected: " + cbLog);
            errCode++;
            errLast = "Cyb Log Failed";
        }
        #endregion Production

        #endregion Cybersource Account
        #endregion Cybersource
        #region Database
        strMessage += ("<br />" + Repeat('\t', 1) + "Checking Database");
        // We need to check both ARC and PORTAL databases
        #region Portal DB
        try
        {
            using (SqlConnection con = new SqlConnection(sqlStrInt))
            {
                if (con.State == ConnectionState.Closed) { con.Open(); }
                if (con.State == ConnectionState.Open)
                {
                    strMessage += ("<br />" + Repeat('\t', 2) + "Portal Database OK");
                    con.Close();
                }
                else
                {
                    strMessage += ("<br />" + Repeat('\t', 2) + "Portal Database FAILED");
                    errCode++;
                    errLast = "Portal Database Failed";
                }
            }
        }
        catch (Exception ex)
        {
            strMessage += ("<br />" + Repeat('\t', 2) + "Portal Database ERROR");
            errCode++;
            errLast = "Portal Database Error\n" + ex.Message;
        }

        #endregion Portal DB
        #region ARC DB
        try
        {
            using (SqlConnection con = new SqlConnection(sqlStrARC))
            {
                try
                {
                    ghFunctions.Donation_Open_Database(con);
                    strMessage += ("<br />" + Repeat('\t', 2) + "ARC Database  OK");

                }
                catch
                {
                    strMessage += ("<br />" + Repeat('\t', 2) + "ARC Database FAILED");
                    errCode++;
                    errLast = "ARC Database Failed";
                }
            }
        }
        catch (Exception ex)
        {
            strMessage += ("<br />" + Repeat('\t', 2) + "ARC Database ERROR");
            errCode++;
            errLast = "ARC Database Error\n" + ex.Message;
        }

        #endregion ARC DB
        #region DataExchange DB
        try
        {
            using (SqlConnection con = new SqlConnection(sqlStrDE))
            {
                try
                {
                    ghFunctions.Donation_Open_Database(con);
                    strMessage += ("<br />" + Repeat('\t', 2) + "DE Database  OK");

                }
                catch
                {
                    strMessage += ("<br />" + Repeat('\t', 2) + "DE Database FAILED");
                    //isvalid = false;
                    errCode++;
                    errLast = "DE Database Failed";
                }
            }
        }
        catch (Exception ex)
        {
            strMessage += ("<br />" + Repeat('\t', 2) + "DE Database ERROR");
            errCode++;
            errLast = "DE Database Error\n" + ex.Message;
        }

        #endregion ARC DB

        #endregion Database
        lblSystemMessage.Text += "<br />" + strMessage.Replace("		", "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;");
        /*
        "Key&nbsp;Path&nbsp;FAILED"
        */

    }
    protected void GridView_Refresh(object sender, EventArgs e)
    {
        try
        {
            sqlPrint.Text = "";
            Error_General.Text = "";
            hdPDcallID.Value = "";
            hdPDcybID.Value = "";
            gvSearchGrid.SelectedIndex = -1;
            GridView_Data(0, this.Page.User.Identity.Name, gvSearchGrid);
            DetailsView_Clear();

            if (ddlDNISList.SelectedIndex != -1 && ddlDNISList.SelectedValue.Length > 0)
            {
                lblResponse.Text = "";
                foreach (ListItem li in ddlDNISList.Items)
                {
                    if (li.Selected)
                    {
                        string strDNIS = "";
                        if (ddlDNISList.SelectedValue.Contains(","))
                        {
                            strDNIS = li.Text.ToString();
                        }
                        else
                        {
                            strDNIS = li.Value.ToString();
                        }

                        if (lblResponse.Text.Length > 0)
                            lblResponse.Text += ", " + strDNIS;
                        else
                            lblResponse.Text += strDNIS;
                    }
                }
                lblResponse.Text = "DNIS List: " + lblResponse.Text;
            }
                
        }
        catch (Exception ex)
        {
            Error_Save(ex, "GridView_Refresh");
        }
    }
    protected void GridView_Refresh_Manual(object sender, EventArgs e)
    {
        try
        {
            // Get the current index, the current page
            int curindex = gvSearchGrid.SelectedIndex;
            int curpage = gvSearchGrid.PageIndex;
            DetailsView_Clear();
            GridView_Data(0, this.Page.User.Identity.Name, gvSearchGrid);
            gvSearchGrid.PageIndex = curpage;
            #region GridView - Select
            if (hdPDcallID.Value.Length > 0)
            {
                if (gvSearchGrid.Rows.Count > 0)
                {
                    int iCell = 0;
                    for (int i = 0; i < gvSearchGrid.HeaderRow.Cells.Count; i++)
                    {
                        if (gvSearchGrid.HeaderRow.Cells[i].Text == "ID")
                        {
                            iCell = i;
                            break;
                        }
                    }
                    foreach (GridViewRow row in gvSearchGrid.Rows)
                    {
                        if (row.Cells[iCell].Text == hdPDcallID.Value)
                        {
                            gvSearchGrid.SelectedIndex = row.RowIndex;
                            break;
                        }
                    }
                    if (gvSearchGrid.SelectedIndex >= 0) GridView_IndexChanged(gvSearchGrid, e);
                }
            }
            else
            {
                gvSearchGrid.SelectedIndex = curindex;
            }
            #endregion GridView - Select
            GridView_IndexChanged(gvSearchGrid, e);
        }
        catch (Exception ex)
        {
            Error_Save(ex, "GridView_Refresh");
        }
    }
    protected void Custom_Export_Excel_SearchGrid_vo(object sender, EventArgs e)
    {
        /// This will be a fully customized export using ClosedXML
        /// We need to add each cell individually
        /// So this will allow us complete control
        /// Use file: F:\ciambotti\greenwoodhall\MiddleWare\sql\dashboard\Dashboard-Export.xlsx
        /// http://stackoverflow.com/questions/12267421/closedxml-working-with-percents-1-decimal-place-and-rounding
        /// https://closedxml.codeplex.com/wikipage?title=Merging%20Cells&referringTitle=Documentation
        /// https://techatplay.wordpress.com/2013/11/05/closedxml-an-easier-way-of-using-openxml/
        /// https://programmershandbook.wordpress.com/2015/03/20/create-closedxml-excel/
        /// ws.Cell("A4").SetValue("25").SetDataType(XLCellValues.Number);
        String nameFile = "ARC-Portal-Search-Results";
        String nameSheet = "Search-Results";

        XLWorkbook wb = new XLWorkbook();
        var ws = wb.Worksheets.Add(nameSheet);
        // Starting Column and Row for Dashboard
        int sRow = 1; int sCol = 1; // A1
        #region Insert - Logo
        ws.Range(sRow, sCol, sRow + 3, sCol + 3).Merge();
        using (WebClient wc = new WebClient())
        {
            byte[] bytes = wc.DownloadData(Server.MapPath("/images/ghnew.png"));
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                System.Drawing.Image img = System.Drawing.Image.FromStream(ms);

                var fIn = img.ToStream(System.Drawing.Imaging.ImageFormat.Png);

                XLPicture pic = new XLPicture
                {
                    NoChangeAspect = false,
                    NoMove = false,
                    NoResize = false,
                    ImageStream = fIn,
                    Name = "Logo"
                };
                XLMarker fMark = new XLMarker { ColumnId = sCol, RowId = sRow };
                pic.AddMarker(fMark);
                fMark = new XLMarker { ColumnId = sCol + 4, RowId = sRow + 3 };
                pic.AddMarker(fMark);
                ws.AddPicture(pic);

                img.Dispose();
                fIn.Dispose();
            }
        }
        sRow = sRow + 4;
        ws.Cell(sRow, sCol).Active = true;
        #endregion Insert - Logo
        var cl = ws.Cell(sRow, sCol);
        var cr = ws.Range(sRow, sCol, sRow, sCol + 1);
        #region Date Range
        cr.Value = "Start Date";
        cr.Merge();
        cr.Style.Font.Bold = true;
        cr.Style.Fill.BackgroundColor = XLColor.LightGray;
        cr.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        cr.Style.Border.OutsideBorderColor = XLColor.DarkGray;

        cr = ws.Range(sRow, sCol + 2, sRow, sCol + 2 + 1);
        cr.Value = dtStartDate.Text + " " + dtStartTime.Text;
        cr.Style.NumberFormat.Format = "YYYY-MM-dd HH:mm:ss";
        cr.Merge();
        cr.Style.Fill.BackgroundColor = XLColor.WhiteSmoke;
        cr.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        cr.Style.Border.OutsideBorderColor = XLColor.DarkGray;
        sRow = sRow + 1;
        cr = ws.Range(sRow, sCol, sRow, sCol + 1);
        cr.Value = "End Date";
        cr.Merge();
        cr.Style.Font.Bold = true;
        cr.Style.Fill.BackgroundColor = XLColor.LightGray;
        cr.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        cr.Style.Border.OutsideBorderColor = XLColor.DarkGray;

        cr = ws.Range(sRow, sCol + 2, sRow, sCol + 2 + 1);
        cr.Value = dtEndDate.Text + " " + dtEndTime.Text;
        cr.Style.NumberFormat.Format = "YYYY-MM-dd HH:mm:ss";
        cr.Merge();
        cr.Style.Fill.BackgroundColor = XLColor.WhiteSmoke;
        cr.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        cr.Style.Border.OutsideBorderColor = XLColor.DarkGray;
        #endregion Date Range
        sRow = sRow + 2;
        #region Grid - Call Dispositions
        cl = ws.Cell(sRow, sCol);
        cl.Value = "Record Details";
        cl.Style.Font.Bold = true;
        cl.Style.Font.FontSize = 12;
        ws.Range(sRow, sCol, sRow, sCol + 5).Merge();
        ws.Range(sRow, sCol, sRow, sCol + 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

        int dRow = sRow + 2; int dCol = sCol; int dColT = dCol;
        GridView gv = gvSearchExport; // gvSearchGrid

        foreach (TableCell cell in gv.HeaderRow.Cells)
        {
            ws.Cell(dRow, dColT).Value = cell.Text;
            ws.Cell(dRow, dColT).Style.Font.Bold = true;
            ws.Cell(dRow, dColT).Style.Fill.BackgroundColor = XLColor.LightGray;
            ws.Cell(dRow, dColT).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell(dRow, dColT).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            ws.Cell(dRow, dColT).Style.Border.OutsideBorderColor = XLColor.DarkGray;
            dColT++;
        }
        dRow++;
        bool altRow = false;
        #region Process each Disposition Row
        foreach (GridViewRow gvRow in gv.Rows)
        {
            dColT = dCol;
            #region Go through Row Cells
            for (int i = 0; i < gvRow.Cells.Count; i++)
            {
                cl = ws.Cell(dRow, dColT);
                if (gvRow.Cells[i].HasControls())
                {
                    string cntrls = "";
                    foreach (Control c in gvRow.Cells[i].Controls)
                    {
                        if (c.GetType() == typeof(Label))
                        {
                            cntrls = ((Label)c).Text;
                        }
                    }
                    if (gv.HeaderRow.Cells[i].Text == "Caller's Zip Code"
                        || gv.HeaderRow.Cells[i].Text == "Date of Call"
                        || gv.HeaderRow.Cells[i].Text == "Time of Call"
                        )
                    {
                        cl.Style.NumberFormat.Format = "@";
                        cl.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    }
                    //var num = decimal.Parse(cntrls.TrimEnd(new char[] { '%', ' ' })) / 100M;
                    //cl.Value = num;
                    //cl.Style.NumberFormat.Format = "0%";
                    cl.Value = cntrls;
                }
                else
                {
                    if (gvRow.Cells[i].Text != "&nbsp;")
                    {
                        //
                        if (gv.HeaderRow.Cells[i].Text == "Amount")
                        {
                            cl.Value = gvRow.Cells[i].Text;
                            cl.Style.NumberFormat.Format = "$#,##0.00";
                        }
                        else if (gv.HeaderRow.Cells[i].Text == "CreateDate")
                        {
                            cl.Value = gvRow.Cells[i].Text;
                            cl.Style.NumberFormat.Format = "MM/dd/yyyy hh:mm";
                        }
                        else
                        {
                            cl.Value = gvRow.Cells[i].Text;
                        }
                        cl.Value = gvRow.Cells[i].Text;
                    }
                }
                if (altRow) { cl.Style.Fill.BackgroundColor = XLColor.White; } else { cl.Style.Fill.BackgroundColor = XLColor.WhiteSmoke; }
                cl.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                cl.Style.Border.OutsideBorderColor = XLColor.DarkGray;

                dColT++;
            }
            #endregion Go through Row Cells
            dRow++;
            if (altRow) altRow = false; else altRow = true;
        }
        #endregion Process each Disposition Row
        #endregion Grid - Call Dispositions
        #region Wrap Up - Save/Download the File
        if (dRow < sRow + 23) dRow = sRow + 23;
        ws.Range(1, 1, dRow, sCol + 2).Style.Alignment.WrapText = true;
        sRow = dRow;

        ws.Rows().AdjustToContents();
        ws.Columns().AdjustToContents();
        // We want 40 width for the logo
        ws.Column(1).Width = 10;
        ws.Column(2).Width = 7.25;
        ws.Column(3).Width = 4.25;
        ws.Column(4).Width = 18.5;

        ws.ShowGridLines = false;

        HttpContext.Current.Response.Clear();
        HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        HttpContext.Current.Response.AddHeader("content-disposition", String.Format(@"attachment;filename={0}{1}.xlsx", nameFile.Replace(" ", "_"), DateTime.Now.ToString("-yyyyMMdd-HHmmss")));

        using (MemoryStream memoryStream = new MemoryStream())
        {
            wb.SaveAs(memoryStream);
            memoryStream.WriteTo(HttpContext.Current.Response.OutputStream);
            memoryStream.Close();
        }

        HttpContext.Current.Response.End();
        #endregion Wrap Up - Save/Download the File
    }
    protected void Custom_Export_Excel_SearchGrid(object sender, EventArgs e)
    {
        /// This will be a fully customized export using ClosedXML
        /// We need to add each cell individually
        /// So this will allow us complete control
        /// Use file: F:\ciambotti\greenwoodhall\MiddleWare\sql\dashboard\Dashboard-Export.xlsx
        /// http://stackoverflow.com/questions/12267421/closedxml-working-with-percents-1-decimal-place-and-rounding
        /// https://closedxml.codeplex.com/wikipage?title=Merging%20Cells&referringTitle=Documentation
        /// https://techatplay.wordpress.com/2013/11/05/closedxml-an-easier-way-of-using-openxml/
        /// https://programmershandbook.wordpress.com/2015/03/20/create-closedxml-excel/
        /// ws.Cell("A4").SetValue("25").SetDataType(XLCellValues.Number);
        String nameFile = "ARC-Portal-Search-Results";
        String nameSheet = "Search-Results";

        XLWorkbook wb = new XLWorkbook();
        Custom_Export_Excel_Dashboard_Details(wb, nameSheet);

        #region Save/Download the File
        HttpContext.Current.Response.Clear();
        HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        HttpContext.Current.Response.AddHeader("content-disposition", String.Format(@"attachment;filename={0}{1}.xlsx", nameFile.Replace(" ", "_"), DateTime.Now.ToString("-yyyyMMdd-HHmmss")));

        using (MemoryStream memoryStream = new MemoryStream())
        {
            wb.SaveAs(memoryStream);
            memoryStream.WriteTo(HttpContext.Current.Response.OutputStream);
            memoryStream.Close();
        }

        HttpContext.Current.Response.End();
        #endregion Save/Download the File
    }
    protected void Custom_Export_Excel_Dashboard_Details(XLWorkbook wb, String nameSheet)
    {
        GridView gv = gvSearchExport; // gvSearchGrid

        var ws = wb.Worksheets.Add(nameSheet);
        // Starting Column and Row for Dashboard
        int sRow = 1; int sCol = 1; // A1
        #region Insert - Logo
        ws.Range(sRow, sCol, sRow + 3, sCol + 2).Merge();
        using (WebClient wc = new WebClient())
        {
            byte[] bytes = wc.DownloadData(Server.MapPath("/images/ghnew.png"));
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                System.Drawing.Image img = System.Drawing.Image.FromStream(ms);

                var fIn = img.ToStream(System.Drawing.Imaging.ImageFormat.Png);

                XLPicture pic = new XLPicture
                {
                    NoChangeAspect = false,
                    NoMove = false,
                    NoResize = false,
                    ImageStream = fIn,
                    Name = "Logo"
                };
                XLMarker fMark = new XLMarker { ColumnId = sCol, RowId = sRow };
                pic.AddMarker(fMark);
                fMark = new XLMarker { ColumnId = sCol + 3, RowId = sRow + 3 };
                pic.AddMarker(fMark);
                ws.AddPicture(pic);

                img.Dispose();
                fIn.Dispose();
            }
        }
        sRow = sRow + 4;
        #endregion Insert - Logo
        var cl = ws.Cell(sRow, sCol);
        var cr = ws.Range(sRow, sCol, sRow, sCol + 0);
        #region Date Range
        cr.Value = "Start Date";
        cr.Merge();
        cr.Style.Font.Bold = true;
        cr.Style.Fill.BackgroundColor = XLColor.LightGray;
        cr.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        cr.Style.Border.OutsideBorderColor = XLColor.DarkGray;

        cr = ws.Range(sRow, sCol + 1, sRow, sCol + 1 + 1);
        cr.Value = dtStartDate.Text + " " + dtStartTime.Text;
        cr.Style.NumberFormat.Format = "YYYY-MM-dd HH:mm:ss";
        cr.Merge();
        cr.Style.Fill.BackgroundColor = XLColor.WhiteSmoke;
        cr.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        cr.Style.Border.OutsideBorderColor = XLColor.DarkGray;
        sRow = sRow + 1;
        cr = ws.Range(sRow, sCol, sRow, sCol + 0);
        cr.Value = "End Date";
        cr.Merge();
        cr.Style.Font.Bold = true;
        cr.Style.Fill.BackgroundColor = XLColor.LightGray;
        cr.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        cr.Style.Border.OutsideBorderColor = XLColor.DarkGray;

        cr = ws.Range(sRow, sCol + 1, sRow, sCol + 1 + 1);
        cr.Value = dtEndDate.Text + " " + dtEndTime.Text;
        cr.Style.NumberFormat.Format = "YYYY-MM-dd HH:mm:ss";
        cr.Merge();
        cr.Style.Fill.BackgroundColor = XLColor.WhiteSmoke;
        cr.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        cr.Style.Border.OutsideBorderColor = XLColor.DarkGray;
        #endregion Date Range
        sRow = sRow + 1;
        #region Grid - Call Details
        cl = ws.Cell(sRow, sCol);
        cl.Value = "Record Details";
        cl.Style.Font.Bold = true;
        cl.Style.Font.FontSize = 12;
        ws.Range(sRow, sCol, sRow, sCol + 5).Merge();
        ws.Range(sRow, sCol, sRow, sCol + 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        int dRow = sRow + 1; int dCol = sCol; int dColT = dCol;
        foreach (TableCell cell in gv.HeaderRow.Cells)
        {
            if (cell.Text == "Call Length" && !ghUser.identity_is_admin()) { continue; }
            if (cell.Text == "Agent Name" && !ghUser.identity_is_admin()) { continue; }
            ws.Cell(dRow, dColT).Value = cell.Text;
            ws.Cell(dRow, dColT).Style.Font.Bold = true;
            ws.Cell(dRow, dColT).Style.Fill.BackgroundColor = XLColor.LightGray;
            ws.Cell(dRow, dColT).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell(dRow, dColT).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            ws.Cell(dRow, dColT).Style.Border.OutsideBorderColor = XLColor.DarkGray;
            dColT++;
        }
        dRow++;
        bool altRow = false;
        #region Process each Row
        foreach (GridViewRow gvRow in gv.Rows)
        {
            dColT = dCol;
            #region Go through Row Cells
            for (int i = 0; i < gvRow.Cells.Count; i++)
            {
                cl = ws.Cell(dRow, dColT);
                if (gvRow.Cells[i].HasControls())
                {
                    string cntrls = "";
                    foreach (Control c in gvRow.Cells[i].Controls)
                    {
                        if (c.GetType() == typeof(Label))
                        {
                            cntrls = ((Label)c).Text;
                        }
                    }
                    if (gv.HeaderRow.Cells[i].Text == "Caller's Zip Code"
                        || gv.HeaderRow.Cells[i].Text == "Date of Call"
                        || gv.HeaderRow.Cells[i].Text == "Time of Call"
                        )
                    {
                        cl.Style.NumberFormat.Format = "@";
                        cl.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    }
                    if (gv.HeaderRow.Cells[i].Text == "Call Length")
                    {
                        if (ghUser.identity_is_admin())
                        {
                            cl.Style.NumberFormat.Format = "HH:mm:ss";
                            cl.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        }
                        else
                        {
                            continue;
                        }
                    }
                    if (gv.HeaderRow.Cells[i].Text == "Agent Name")
                    {
                        if (ghUser.identity_is_admin())
                        {
                            cl.Style.NumberFormat.Format = "HH:mm:ss";
                            cl.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        }
                        else
                        {
                            continue;
                        }
                    }

                    //var num = decimal.Parse(cntrls.TrimEnd(new char[] { '%', ' ' })) / 100M;
                    //cl.Value = num;
                    //cl.Style.NumberFormat.Format = "0%";
                    cl.Value = cntrls;
                }
                else
                {
                    if (gvRow.Cells[i].Text != "&nbsp;")
                    {
                        //
                        if (gv.HeaderRow.Cells[i].Text == "Amount")
                        {
                            cl.Style.NumberFormat.Format = "$#,##0.00";
                            cl.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        }
                        else if (gv.HeaderRow.Cells[i].Text == "Call Date")
                        {
                            cl.Style.NumberFormat.Format = "MM/dd/yyyy hh:mm";
                            cl.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        }
                        else if (gv.HeaderRow.Cells[i].Text == "DNIS" || gv.HeaderRow.Cells[i].Text == "Zip")
                        {
                            cl.Style.NumberFormat.Format = "@";
                            cl.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        }
                        else
                        {
                        }
                        cl.Value = gvRow.Cells[i].Text;
                    }
                }

                if (altRow) { cl.Style.Fill.BackgroundColor = XLColor.White; } else { cl.Style.Fill.BackgroundColor = XLColor.WhiteSmoke; }
                cl.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                cl.Style.Border.OutsideBorderColor = XLColor.DarkGray;

                dColT++;
            }
            #endregion Go through Row Cells
            dRow++;
            if (altRow) altRow = false; else altRow = true;
        }
        #endregion Process each Row
        #endregion Grid - Call Details
        #region Wrap Up
        if (dRow < sRow + 23) dRow = sRow + 23;
        ws.Range(1, 1, dRow, sCol + 2).Style.Alignment.WrapText = true;
        sRow = dRow;

        ws.Rows().AdjustToContents();
        ws.Columns().AdjustToContents();
        // Total Width: 40
        ws.Column(1).Width = 20;
        ws.Column(2).Width = 9;
        ws.Column(3).Width = 11;
        //ws.Columns(1, 3).Width = 11;
        //ws.Column(4).Width = 7;
        //8.43 * 4 == 33.72
        ws.ShowGridLines = false;
        ws.SheetView.FreezeRows(8);
        ws.Cell(9, 1).Active = true;
        #endregion Wrap Up
    }
    protected void Custom_Export_Excel_Details(object sender, EventArgs e)
    {
        /// This will be a fully customized export using ClosedXML
        /// We need to add each cell individually
        /// So this will allow us complete control
        /// Use file: F:\ciambotti\greenwoodhall\MiddleWare\sql\dashboard\Dashboard-Export.xlsx
        /// http://stackoverflow.com/questions/12267421/closedxml-working-with-percents-1-decimal-place-and-rounding
        /// https://closedxml.codeplex.com/wikipage?title=Merging%20Cells&referringTitle=Documentation
        /// https://techatplay.wordpress.com/2013/11/05/closedxml-an-easier-way-of-using-openxml/
        /// https://programmershandbook.wordpress.com/2015/03/20/create-closedxml-excel/
        /// ws.Cell("A4").SetValue("25").SetDataType(XLCellValues.Number);
        String fileName = "Record-Details-";
        fileName += gvSearchGrid.SelectedDataKey["callid"].ToString();
        XLWorkbook wb = new XLWorkbook();
        var ws = wb.Worksheets.Add(fileName);
        // Starting Column and Row for Dashboard
        int sRow = 1; int sCol = 1; // A1
        #region Insert - Logo
        ws.Range(sRow, sCol, sRow + 3, sCol + 3).Merge();
        using (WebClient wc = new WebClient())
        {
            byte[] bytes = wc.DownloadData(Server.MapPath("/images/ghnew.png"));
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                System.Drawing.Image img = System.Drawing.Image.FromStream(ms);

                var fIn = img.ToStream(System.Drawing.Imaging.ImageFormat.Png);

                XLPicture pic = new XLPicture
                {
                    NoChangeAspect = false,
                    NoMove = false,
                    NoResize = false,
                    ImageStream = fIn,
                    Name = "Logo"
                };
                XLMarker fMark = new XLMarker { ColumnId = sCol, RowId = sRow };
                pic.AddMarker(fMark);
                fMark = new XLMarker { ColumnId = sCol + 4, RowId = sRow + 3 };
                pic.AddMarker(fMark);
                ws.AddPicture(pic);

                img.Dispose();
                fIn.Dispose();
            }
        }
        sRow = sRow + 4;
        ws.Cell(sRow, sCol).Active = true;
        #endregion Insert - Logo
        var cl = ws.Cell(sRow, sCol);
        var cr = ws.Range(sRow, sCol, sRow, sCol + 1);
        #region Date Range
        cr.Value = "Start Date";
        cr.Merge();
        cr.Style.Font.Bold = true;
        cr.Style.Fill.BackgroundColor = XLColor.LightGray;
        cr.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        cr.Style.Border.OutsideBorderColor = XLColor.DarkGray;

        cr = ws.Range(sRow, sCol + 2, sRow, sCol + 2 + 1);
        cr.Value = dtStartDate.Text + " " + dtStartTime.Text;
        cr.Style.NumberFormat.Format = "YYYY-MM-dd HH:mm:ss";
        cr.Merge();
        cr.Style.Fill.BackgroundColor = XLColor.WhiteSmoke;
        cr.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        cr.Style.Border.OutsideBorderColor = XLColor.DarkGray;
        sRow = sRow + 1;
        cr = ws.Range(sRow, sCol, sRow, sCol + 1);
        cr.Value = "End Date";
        cr.Merge();
        cr.Style.Font.Bold = true;
        cr.Style.Fill.BackgroundColor = XLColor.LightGray;
        cr.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        cr.Style.Border.OutsideBorderColor = XLColor.DarkGray;

        cr = ws.Range(sRow, sCol + 2, sRow, sCol + 2 + 1);
        cr.Value = dtEndDate.Text + " " + dtEndTime.Text;
        cr.Style.NumberFormat.Format = "YYYY-MM-dd HH:mm:ss";
        cr.Merge();
        cr.Style.Fill.BackgroundColor = XLColor.WhiteSmoke;
        cr.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        cr.Style.Border.OutsideBorderColor = XLColor.DarkGray;
        #endregion Date Range
        sRow = sRow + 2;
        cl = ws.Cell(sRow, sCol);
        cl.Value = "Record Details";
        cl.Style.Font.Bold = true;
        cl.Style.Font.FontSize = 12;
        ws.Range(sRow, sCol, sRow, sCol + 5).Merge();
        ws.Range(sRow, sCol, sRow, sCol + 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

        #region Load the Views
        // Call Details - Left Side
        int dRow = sRow + 2; int dCol = sCol; int dColT = dCol;
        Excel_Export_DetailsView(dvCallDetails, ws, cr, dRow, dCol, dColT);
        // Payment Details
        dRow = tempRow + 1;
        Excel_Export_DetailsView(dvPaymentDetails, ws, cr, dRow, dCol, dColT);
        // Contact Details
        dRow = tempRow + 1;
        Excel_Export_DetailsView(dvContactDetails, ws, cr, dRow, dCol, dColT);
        // Refund Details
        dRow = tempRow + 1;
        Excel_Export_DetailsView(dvRefundDetails, ws, cr, dRow, dCol, dColT);
        // Gift Details
        dRow = tempRow + 1;
        int dRow_Gift = dRow;
        Excel_Export_DetailsView(dvGiftDetails, ws, cr, dRow, dCol, dColT);
        // Sustainer Details
        dRow = tempRow + 1;
        Excel_Export_DetailsView(dvSustainerDetails, ws, cr, dRow, dCol, dColT);
        // Tokenization Details
        dRow = tempRow + 1;
        Excel_Export_DetailsView(dvTokenizationDetails, ws, cr, dRow, dCol, dColT);

        // Interaction -- This is Last
        dRow = tempRow + 1;
        int dRow_Interaction = dRow;
        Excel_Export_DetailsView(dvInteraction, ws, cr, dRow, dCol, dColT);


        // Donor Details - Right Side
        dRow = sRow + 2; dCol = sCol + 6; dColT = dCol;
        Excel_Export_DetailsView(dvDonorDetails, ws, cr, dRow, dCol, dColT);
        // Tribute Details
        dRow = tempRow + 1;
        Excel_Export_DetailsView(dvTributeDetails, ws, cr, dRow, dCol, dColT);
        // ADU File
        dRow = tempRow + 1;
        Excel_Export_DetailsView(dvADUFile, ws, cr, dRow, dCol, dColT);
        // Remove Details
        dRow = tempRow + 1;
        Excel_Export_DetailsView(dvRemoveDetails, ws, cr, dRow, dCol, dColT);

        // Interaction Details -- This is last
        dRow = tempRow + 1;
        if (dRow < dRow_Interaction) dRow = dRow_Interaction;
        Excel_Export_DetailsView(dvInteractionDetails, ws, cr, dRow, dCol, dColT);

        // Gift List
        // Recurring List
        dRow = sRow + 0; dCol = sCol + 12; dColT = dCol;
        tempRow = dRow;
        Excel_Export_GridView(gvRecurringListExport, "Recurring List", ws, cl, cr, dRow, dCol, dColT);

        dRow = tempRow + 1;
        Excel_Export_DetailsView(dvPaymentDetailsRecurring, ws, cr, dRow, dCol, dColT);

        // Gift List
        dRow = tempRow + 1;
        if (dRow < dRow_Gift) dRow = dRow_Gift;
        Excel_Export_GridView(gvGiftList, "Gift List", ws, cl, cr, dRow, dCol, dColT);

        #endregion Load the Views

        #region Wrap Up - Save/Download the File
        if (dRow < sRow + 23) dRow = sRow + 23;
        ws.Range(1, 1, dRow, sCol + 2).Style.Alignment.WrapText = true;
        sRow = dRow;

        ws.Rows().AdjustToContents();
        ws.Columns().AdjustToContents();
        // We want 40 width for the logo
        ws.Column(1).Width = 10;
        ws.Column(2).Width = 7.25;
        ws.Column(3).Width = 4.25;
        ws.Column(4).Width = 18.5;

        ws.Column(6).Width = 10;
        ws.Column(7).Width = 7.25;
        ws.Column(8).Width = 4.25;
        ws.Column(9).Width = 18.5;

        ws.ShowGridLines = false;
        fileName = "ARC-Portal-" + fileName;

        HttpContext.Current.Response.Clear();
        HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        HttpContext.Current.Response.AddHeader("content-disposition", String.Format(@"attachment;filename={0}{1}.xlsx", fileName.Replace(" ", "_"), DateTime.Now.ToString("-yyyyMMdd-HHmmss")));

        using (MemoryStream memoryStream = new MemoryStream())
        {
            wb.SaveAs(memoryStream);
            memoryStream.WriteTo(HttpContext.Current.Response.OutputStream);
            memoryStream.Close();
        }

        HttpContext.Current.Response.End();
        #endregion Wrap Up - Save/Download the File
    }
    protected void Excel_Export_DetailsView(DetailsView dv, IXLWorksheet ws, IXLRange cr, int dRow, int dCol, int dColT)
    {
        #region Details View
        //if (dv.Rows[0].Cells[0].Text != "&nbsp;")
        if (dv.Rows.Count > 0)
        {
            // Each DetailsView has a Header, 2 Columns, Multiple Rows
            // There can be variable information in any place
            cr = ws.Range(dRow, dColT, dRow, dColT + 4);
            cr.Value = dv.HeaderText;
            cr.Merge();
            cr.Style.Font.Bold = true;
            cr.Style.Fill.BackgroundColor = XLColor.LightGray;
            cr.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            cr.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            cr.Style.Border.OutsideBorderColor = XLColor.DarkGray;
            dRow++;
            bool altRow = false;
            #region DV Rows
            // We need to ensure the 1st row is formatted even if no record details
            foreach (TableRow dvRow in dv.Rows)
            {
                dColT = dCol;
                #region DV Cells
                for (int i = 0; i < dvRow.Cells.Count; i++)
                {
                    cr = ws.Range(dRow, dColT, dRow, dColT + 1);
                    if (i == 1) cr = ws.Range(dRow, dColT, dRow, dColT + 2);
                    if (dvRow.Cells[i].HasControls())
                    {
                        string cntrls = "";
                        foreach (Control c in dvRow.Cells[i].Controls)
                        {
                            if (c.GetType() == typeof(Label))
                            {
                                cntrls = ((Label)c).Text;
                            }
                        }
                        var num = cntrls;
                        cr.Value = num;
                    }
                    else
                    {
                        if (dvRow.Cells[i].Text.Contains("No [") && dvRow.Cells[i].Text.Contains("] details;"))
                        {
                            cr = ws.Range(dRow, dColT, dRow, dColT + 4);
                        }
                        if (dvRow.Cells[i].Text != "&nbsp;")
                        {
                            cr.Value = dvRow.Cells[i].Text;
                        }
                    }
                    cr.Merge();
                    if (altRow) { cr.Style.Fill.BackgroundColor = XLColor.White; } else { cr.Style.Fill.BackgroundColor = XLColor.WhiteSmoke; }
                    //cr.Style.Fill.BackgroundColor = XLColor.WhiteSmoke;
                    cr.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    cr.Style.Border.OutsideBorderColor = XLColor.DarkGray;
                    cr.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    //cr.Style.Alignment.WrapText = true; // Does not work due to merged cells
                    dColT++;
                    dColT++;
                }
                #endregion DV Cells
                dRow++;
                if (altRow) altRow = false; else altRow = true;
            }
            #endregion DV Rows
            tempRow = dRow;
        }
        #endregion Details View
    }
    protected void Excel_Export_GridView(GridView gv, String Title, IXLWorksheet ws, IXLCell cl, IXLRange cr, int dRow, int dCol, int dColT)
    {
        if (gv.Rows.Count > 0)
        {
            #region Grid View
            cl = ws.Cell(dRow, dCol);
            cl.Value = Title;
            cl.Style.Font.Bold = true;
            cl.Style.Font.FontSize = 12;
            ws.Range(dRow, dCol, dRow, dCol + 5).Merge();
            ws.Range(dRow, dCol, dRow, dCol + 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            dRow++;
            dRow++;

            foreach (TableCell cell in gv.HeaderRow.Cells)
            {
                if (cell.Text != "&nbsp;")
                {
                    ws.Cell(dRow, dColT).Value = cell.Text;
                    ws.Cell(dRow, dColT).Style.Font.Bold = true;
                    ws.Cell(dRow, dColT).Style.Fill.BackgroundColor = XLColor.LightGray;
                    ws.Cell(dRow, dColT).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Cell(dRow, dColT).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    ws.Cell(dRow, dColT).Style.Border.OutsideBorderColor = XLColor.DarkGray;
                    dColT++;
                }
            }
            dRow++;
            bool altRow = false;
            #region Process each Row
            foreach (GridViewRow gvRow in gv.Rows)
            {
                dColT = dCol;
                #region Go through Row Cells
                for (int i = 0; i < gvRow.Cells.Count; i++)
                {
                    cl = ws.Cell(dRow, dColT);
                    if (gvRow.Cells[i].HasControls())
                    {
                        string cntrls = "";
                        foreach (Control c in gvRow.Cells[i].Controls)
                        {
                            if (c.GetType() == typeof(Label))
                            {
                                cntrls = ((Label)c).Text;
                            }
                        }
                        if (cntrls != "&nbsp;")
                        {
                            //var num = decimal.Parse(cntrls.TrimEnd(new char[] { '%', ' ' })) / 100M;
                            var num = cntrls;
                            cl.Value = num;
                            //cl.Style.NumberFormat.Format = "0%";
                            //cl.Style.NumberFormat.Format = "@";
                            //cl.Value = gvRow.Cells[i].Text;
                        }
                    }
                    else
                    {
                        if (gvRow.Cells[i].Text != "&nbsp;")
                        {
                            if (gv.HeaderRow.Cells[i].Text == "Amount")
                            {
                                cl.Value = gvRow.Cells[i].Text;
                                cl.Style.NumberFormat.Format = "$#,##0.00";
                            }
                            else if (gv.HeaderRow.Cells[i].Text == "CreateDate")
                            {
                                cl.Value = gvRow.Cells[i].Text;
                                cl.Style.NumberFormat.Format = "MM/dd/yyyy hh:mm";
                            }
                            else
                            {
                                cl.Value = gvRow.Cells[i].Text;
                            }
                        }
                    }
                    if (altRow) { cl.Style.Fill.BackgroundColor = XLColor.White; } else { cl.Style.Fill.BackgroundColor = XLColor.WhiteSmoke; }
                    cl.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    cl.Style.Border.OutsideBorderColor = XLColor.DarkGray;

                    dColT++;
                }
                #endregion Go through Row Cells
                dRow++;
                if (altRow) altRow = false; else altRow = true;
            }
            #endregion Process each Row
            #endregion Grid View
            tempRow = dRow;
        }
    }
    protected void GridView_Data(Int32 UserID, String UserName, GridView gv)
    {
        // Validate Date
        // .AddHours(ghFunctions.dtUserOffSet)
        DateTime dtStart = DateTime.Parse(dtStartDate.Text + " " + dtStartTime.Text).AddHours(ghFunctions.dtUserOffSet);
        DateTime dtEnd = DateTime.Parse(dtEndDate.Text + " " + dtEndTime.Text).AddHours(ghFunctions.dtUserOffSet);

        Session["sort"] = ddlSort.SelectedValue;
        Session["length"] = ddlLength.SelectedValue;
        

        #region SQL Connection
        using (SqlConnection con = new SqlConnection(sqlStrARC))
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
                // categoriesAdapter.SelectCommand.Parameters.Add("@CategoryName", SqlDbType.VarChar, 80).Value = "toasters";

                bool qsValid = false;
                Int32 qsCallID = 0;
                if (!IsPostBack && Request["callid"] != null)
                {
                    qsValid = Int32.TryParse(Request["callid"].ToString(), out qsCallID);
                }
                if (qsValid)
                {

                    cmd.Parameters.Add("@sp_top", SqlDbType.Int).Value = 1;
                    cmd.Parameters.Add(new SqlParameter("@sp_startdate", DateTime.UtcNow.AddHours(ghFunctions.dtUserOffSet).AddYears(-1).ToString("MM/dd/yyyy")));
                    cmd.Parameters.Add(new SqlParameter("@sp_starttime", "00:00:00"));
                    cmd.Parameters.Add(new SqlParameter("@sp_enddate", DateTime.UtcNow.AddHours(ghFunctions.dtUserOffSet).AddDays(1).ToString("MM/dd/yyyy")));
                    cmd.Parameters.Add(new SqlParameter("@sp_endtime", "23:59:59"));
                    cmd.Parameters.Add(new SqlParameter("@sp_callid", qsCallID));
                }
                else
                {
                    String sp_source = "Live";
                    if (Page.User.IsInRole("CDR Fundraising Group") && (ddlDNISList.SelectedIndex == -1 || ddlDNISList.SelectedValue.Length == 0))
                    {
                        // For CDRFG we need to ensure a DNIS is selected, otherwise we do not show any results
                        sp_source = "Invalid";
                    }

                    cmd.Parameters.Add("@sp_source", SqlDbType.VarChar, 20).Value = sp_source;
                    cmd.Parameters.Add("@sp_top", SqlDbType.Int).Value = ddlTop.SelectedValue;

                    #region Filter Processing
                    if (dtRecurringDate.Text.Length > 0)
                    {
                        cmd.Parameters.Add(new SqlParameter("@sp_recurringdate", dtRecurringDate.Text));

                    }
                    else
                    {
                        // .AddHours(ghFunctions.dtUserOffSet)
                        cmd.Parameters.Add(new SqlParameter("@sp_startdate", dtStart.ToString("MM/dd/yyyy"))); // dtStartDate.Text));
                        cmd.Parameters.Add(new SqlParameter("@sp_starttime", dtStart.ToString("HH:mm:ss"))); // dtStartTime.Text));
                        cmd.Parameters.Add(new SqlParameter("@sp_enddate", dtEnd.ToString("MM/dd/yyyy"))); // dtEndDate.Text));
                        cmd.Parameters.Add(new SqlParameter("@sp_endtime", dtEnd.ToString("HH:mm:ss"))); // dtEndTime.Text));
                    }
                    if (CallID.Text.Trim().Length > 0)
                    {
                        CallID.Text = CallID.Text.Trim().Replace(" ", "");
                        if (CallID.Text.Contains(","))
                        {
                            // Split this and make sure that all the parts are INTEGERS
                            string[] callidarray = CallID.Text.Split(',');
                            string callidlist = "";
                            int intDonID = 0;
                            if (callidarray.Length > 0)
                            {
                                foreach (string strDonID in callidarray)
                                {
                                    if (Int32.TryParse(strDonID, out intDonID))
                                    {
                                        if (callidlist.Length > 0) { callidlist += "," + strDonID; } else { callidlist = strDonID; }
                                    }
                                }
                            }
                            if (callidlist.Length > 0) cmd.Parameters.Add("@sp_callidlist", SqlDbType.VarChar, 400).Value = callidlist;
                        }
                        else
                        {
                            cmd.Parameters.Add(new SqlParameter("@sp_callid", CallID.Text));
                        }
                    }
                    if (DonationID.Text.Length > 0)
                    {
                        if (DonationID.Text.Contains(","))
                        {
                            // Split this and make sure that all the parts are INTEGERS
                            string[] donationidarray = DonationID.Text.Split(',');
                            string donationidlist = "";
                            int intDonID = 0;
                            if (donationidarray.Length > 0)
                            {
                                foreach(string strDonID in donationidarray)
                                {
                                    if (Int32.TryParse(strDonID, out intDonID))
                                    {
                                        if (donationidlist.Length > 0) { donationidlist += "," + strDonID; } else { donationidlist = strDonID; }
                                    }
                                }
                            }
                            if (donationidlist.Length > 0) cmd.Parameters.Add("@sp_donationidlist", SqlDbType.VarChar, 400).Value = donationidlist;
                        }
                        else
                        {
                            cmd.Parameters.Add(new SqlParameter("@sp_donationid", DonationID.Text));
                        }
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
                    if (ddlDesignation.SelectedIndex != -1 && ddlDesignation.SelectedValue.Length > 0)
                    {
                        string dsgns = "";
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
                            //cmd.Parameters.Add("@sp_designation", SqlDbType.VarChar, -1).Value = dsgns;
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
                        // cmd.Parameters.Add("@sp_dnislist", SqlDbType.VarChar, -1).Value = ddlDNISList.SelectedValue;
                        // Multiple DNIS Selection
                        string dnslst = "";
                        foreach (ListItem li in ddlDNISList.Items)
                        {
                            if (li.Selected)
                            {
                                if (li.Value.Contains(","))
                                {
                                    dnslst += li.Value + ",";
                                }
                                else
                                {
                                    // dnslst += li.Value + ",";
                                    dnslst += "'" + li.Value + "'" + ",";
                                }
                            }
                        }
                        if (dnslst.EndsWith(",")) { dnslst = dnslst.TrimEnd(','); }
                        if (dnslst.Contains("'") && !dnslst.Contains(","))
                        {
                            dnslst = dnslst.Replace("'", "");
                        }
                        if (dnslst.Length > 0)
                        {
                            cmd.Parameters.Add("@sp_dnislist", SqlDbType.VarChar, -1).Value = dnslst;
                        }

                    }
                    if (cbTests.Checked)
                    {
                        cmd.Parameters.Add(new SqlParameter("@sp_exclude_testcc", cbTests.Checked));
                    }
                    if (cbDateRefund.Checked)
                    {
                        cmd.Parameters.Add(new SqlParameter("@sp_refund_dates", cbDateRefund.Checked));
                    }


                    if (dtStartDate.Text.Length > 0)
                    {
                        rpElapsed.Text = "From " + dtStartDate.Text + " " + dtStartTime.Text + " to " + dtEndDate.Text + " " + dtEndTime.Text;
                    }
                    if (hdPDcallID.Value.Length > 0)
                    {
                        cmd.Parameters.Add(new SqlParameter("@sp_chargeid", hdPDcallID.Value));
                        cmd.Parameters.Add(new SqlParameter("@sp_chargecbid", hdPDcybID.Value));
                    }
                    if (ADU.Text.Length > 0)
                    {
                        cmd.Parameters.Add("@sp_adu", SqlDbType.VarChar, 100).Value = ADU.Text.Trim();
                    }

                    cmd.Parameters.Add("@sp_sort", SqlDbType.VarChar, 4).Value = ddlSort.SelectedValue;
                    if (ddlLength.SelectedValue != "0" || ddlLength.SelectedValue != "")
                    {
                        cmd.Parameters.Add("@sp_long_calls", SqlDbType.Int).Value = ddlLength.SelectedValue;
                    }
                    #endregion Filter Processing
                }

                #endregion SQL Parameters
                #region Print SQL
                if (Page.User.IsInRole("System Administrator") == true && Connection.GetConnectionType() == "Local")
                {
                    print_sql(cmd, "append"); // Will print for Admin in Local
                    //String sqlToText = "";
                    //sqlToText += cmd.CommandText.ToString();
                    //foreach (SqlParameter p in cmd.Parameters)
                    //{
                    //    sqlToText += "<br />" + p.ParameterName + " = " + p.Value.ToString() + " [" + p.DbType.ToString() + "]";
                    //}
                    //sqlPrint.Text = "<br />" + sqlToText;
                }
                #endregion Print SQL
                #region SQL Processing
                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                ad.Fill(dt);
                gv.DataSource = dt;
                gv.DataBind();
                gv.Visible = true;
                if (dt.Rows.Count > 0)
                {
                    btnExport.Visible = true;
                    gvSearchExport.DataSource = dt;
                    gvSearchExport.DataBind();
                }
                if (Page.User.IsInRole("CDR Fundraising Group") && (ddlDNISList.SelectedIndex == -1 || ddlDNISList.SelectedValue.Length == 0))
                {
                    // For CDRFG we need to ensure a DNIS is selected, otherwise we do not show any results
                    lblSearchGrid.Text += "<br /><span style='color: DarkRed;'>Invalid request - you MUST select a DNIS or DNIS Group.</span>";
                }
                #endregion SQL Processing
            }
            #endregion SQL Command
        }
        #endregion SQL Connection
    }
    protected void GridView_DataBound(Object sender, EventArgs e)
    {
        GridView gv = (GridView)sender;
        if (gv.ID == "gvSearchGrid")
        {
            #region gvSearchGrid
            lblSearchGrid.Text = " Records: [" + gvSearchGrid.Rows.Count.ToString() + "]";
            if (gvSearchGrid.PageCount > 0)
            {
                lblSearchGrid.Text += " - Pages: [" + gvSearchGrid.PageCount.ToString() + "]";
                lblSearchGrid.Text += " - Approx Total: [" + (gvSearchGrid.PageCount * gvSearchGrid.Rows.Count).ToString() + "]";
                // Retrieve the pager row.
                //GridViewRow pagerRow = gvSearchGrid.BottomPagerRow;
                GridViewRow pagerRow = gvSearchGrid.TopPagerRow;
                // Retrieve the DropDownList and Label controls from the row.
                DropDownList pageList = (DropDownList)pagerRow.Cells[0].FindControl("gvSearchGridPageDropDownList");
                Label pageLabel = (Label)pagerRow.Cells[0].FindControl("CurrentPageLabel");
                if (pageList != null)
                {
                    // Create the values for the DropDownList control based on 
                    // the  total number of pages required to display the data
                    // source.
                    for (int i = 0; i < gvSearchGrid.PageCount; i++)
                    {
                        // Create a ListItem object to represent a page.
                        int pageNumber = i + 1;
                        ListItem item = new ListItem(pageNumber.ToString());
                        // If the ListItem object matches the currently selected
                        // page, flag the ListItem object as being selected. Because
                        // the DropDownList control is recreated each time the pager
                        // row gets created, this will persist the selected item in
                        // the DropDownList control.   
                        if (i == gvSearchGrid.PageIndex)
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
                    int currentPage = gvSearchGrid.PageIndex + 1;
                    // Update the Label control with the current page information.
                    pageLabel.Text = "Page " + currentPage.ToString() +
                      " of " + gvSearchGrid.PageCount.ToString();
                }
                if (gvSearchGrid.PageIndex > 0)
                {
                    pagerRow.Cells[0].FindControl("lnkPrevPage").Visible = true;
                    pagerRow.Cells[0].FindControl("lnkFirstPage").Visible = true;
                }
                else
                {
                    pagerRow.Cells[0].FindControl("lnkPrevPage").Visible = false;
                    pagerRow.Cells[0].FindControl("lnkFirstPage").Visible = false;
                }

                if (gvSearchGrid.PageCount != (gvSearchGrid.PageIndex + 1))
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
            #endregion gvSearchGrid
        }
    }
    protected void GridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        GridView gv = (GridView)sender;
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            //http://aspdotnetfaq.com/Faq/How-to-correctly-highlight-GridView-rows-on-Mouse-Hover-in-ASP-NET.aspx
            // when mouse is over the row, save original color to new attribute, and change it to highlight yellow color
            e.Row.Attributes.Add("onmouseover", "this.style.cursor='pointer';this.originalstyle=this.style.backgroundColor;this.style.backgroundColor='#336699'");

            // when mouse leaves the row, change the bg color to its original value    
            e.Row.Attributes.Add("onmouseout", "this.style.backgroundColor=this.originalstyle;");

            e.Row.Attributes["onclick"] = Page.ClientScript.GetPostBackClientHyperlink(gv, "Select$" + e.Row.RowIndex);
        }
    }
    protected void GridView_IndexChanged(object sender, EventArgs e)
    {
        GridView gv = (GridView)sender;
        if (gv.ID == "gvSearchGrid")
        {
            #region gvSearchGrid
            try
            {
                Admin_Clear();
                if (dvPaymentDetailsRecurring.Rows.Count > 0) dvPaymentDetailsRecurring.DataBind();
                Int32 callid = Convert.ToInt32(gvSearchGrid.SelectedDataKey.Values[0].ToString());
                Int32 donorid = 0; // Convert.ToInt32(gvSearchGrid.SelectedDataKey.Values[1].ToString());
                DetailsView_Data(callid, donorid, dvCallDetails);
                DetailsView_Data(callid, donorid, dvDonorDetails);
                DetailsView_Data(callid, donorid, dvTributeDetails);
                DetailsView_Data(callid, donorid, dvPaymentDetails);
                DetailsView_Data(callid, donorid, dvContactDetails);
                DetailsView_Data(callid, donorid, dvRefundDetails);
                DetailsView_Data(callid, donorid, dvADUFile);
                DetailsView_Data(callid, donorid, dvRemoveDetails);
                DetailsView_Data(callid, donorid, dvGiftDetails);
                GridView_Data2(callid, donorid, gvGiftList, pnlGiftList);
                DetailsView_Data(callid, donorid, dvSustainerDetails);
                GridView_Data2(callid, donorid, gvRecurringList, pnlRecurringList);
                DetailsView_DataExchange_Data(callid, dvInteraction);
                DetailsView_DataExchange_Data(callid, dvInteractionDetails);

                DetailsView_Data(callid, donorid, dvTokenizationDetails);

                DetailsView_Data(callid, donorid, dvHolidayAddress);

                // If Recording Admin
                
                Int32 sp_companyid = 3;
                Int64 sp_interactionid = 0;
                if (dvInteraction.Rows.Count > 1)
                {
                    Int64.TryParse(dvInteraction.DataKey[0].ToString(), out sp_interactionid);
                    if (ghUser.identity_is_recording_admin() && sp_interactionid > 0)
                    {
                        Recording_Fetch_Record(sp_companyid, sp_interactionid); if (gvRecordingsGrid.Rows.Count >= 1) { pnlRecordings.Visible = true; }
                    }
                }
                btnExportDetails.Visible = true;
                //if (Page.User.IsInRole("System Administrator")) pAdminFunctions.Visible = true;
                if (ghUser.identity_is_admin()) pAdminFunctions.Visible = true;
            }
            catch (Exception ex)
            {
                Error_Save(ex, "DetailsView Data Error");
            }
            #endregion gvSearchGrid
        }
        else if (gv.ID == "gvRecurringList")
        {
            lblRecurringDetail.Text = "Fetching records...";
            try
            {
                Admin_Clear();
                int dvCallID = Convert.ToInt32(gv.SelectedDataKey["callid"].ToString());
                int dvCybID = 0;
                if (gv.SelectedDataKey["cbid"].ToString() != "")
                {
                    dvCybID = Convert.ToInt32(gv.SelectedDataKey["cbid"].ToString());
                }
                DetailsView_Data(dvCallID, dvCybID, dvPaymentDetailsRecurring);
            }
            catch (Exception ex)
            {
                Error_Save(ex, "DetailsView Data Error");
            }
        }
        else
        {
            lblRecurringDetail.Text = "Details...";
        }
    }
    protected void GridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GridView gv = (GridView)sender;
        if (gv.ID == "gvSearchGrid")
        {
            #region gvSearchGrid
            Admin_Clear();
            //Session["EventList_GridView_SelectedIndex"] = null;
            //Session["EventList_GridView_PageIndex"] = null;
            lblSearchGrid.Text = e.NewPageIndex.ToString();
            gvSearchGrid.SelectedIndex = -1;
            gvSearchGrid.PageIndex = e.NewPageIndex;
            GridView_Data(0, this.Page.User.Identity.Name, gvSearchGrid);
            #endregion gvSearchGrid
        }
    }
    protected void GridView_PageDropDownList_SelectedIndexChanged(Object sender, EventArgs e)
    {
        DropDownList gv = (DropDownList)sender;
        if (gv.ID == "gvSearchGridPageDropDownList")
        {
            #region gvSearchGrid
            // Retrieve the pager row.
            GridViewRow pagerRow = gvSearchGrid.TopPagerRow;
            // Retrieve the PageDropDownList DropDownList from the bottom pager row.
            DropDownList pageList = (DropDownList)pagerRow.Cells[0].FindControl("gvSearchGridPageDropDownList");
            // Set the PageIndex property to display that page selected by the user.
            gvSearchGrid.SelectedIndex = -1;
            gvSearchGrid.PageIndex = pageList.SelectedIndex;
            GridView_Data(0, this.Page.User.Identity.Name, gvSearchGrid);
            #endregion gvSearchGrid
        }

    }
    protected void Recording_Fetch_Record(Int32 sp_companyid, Int64 sp_interactionid)
    {
        GridView gv = gvRecordingsGrid;
        // if (oDeBug)
        // lblSystemMessage.Text = "<br />Fetching recordings...";
        gv.DataSource = null;
        gv.DataBind();
        if (sp_companyid == 3) // if(sp_callid > 0)
        {
            lblSystemMessage.Text += "<br />Have Five9 callid...";
            DateTime dtStart = DateTime.UtcNow;
            #region Get the Recording - From MiddleWare
            #region SQL Connection
            using (SqlConnection con = new SqlConnection(sqlStrDE))
            {
                #region SQL Command
                using (SqlCommand cmd = new SqlCommand("", con))
                {
                    ghFunctions.Donation_Open_Database(con);
                    cmd.CommandTimeout = 600;
                    #region Build cmdText
                    String cmdText = "";
                    cmdText += @"
SELECT
[i].[companyid]
,[i].[interactionid]
,[fcr].[callid]
,[fcr].[recordingid]
,[fcr].[agentid]
,[fcr].[datecreated]
,[fcr].[daterecorded]
,[fcr].[filename]
,[fcr].[pathlocal]
,[fcr].[pathurl] [pathurl_old]
,CASE
	WHEN [fcr].[pathurl] LIKE '%cloudapp.net%' THEN REPLACE([fcr].[pathurl],'http://recordingvm.cloudapp.net','https://recordings.greenwoodhall.com')
	ELSE [fcr].[pathurl]
END [pathurl]
FROM [dbo].[interactions] [i] WITH(NOLOCK)
JOIN [dbo].[five9_call_recording] [fcr] WITH(NOLOCK) ON [fcr].[companyid] = [i].[companyid] AND [fcr].[interactionid] = [i].[interactionid]
WHERE [i].[companyid] = @sp_companyid
AND [i].[interactionid] = @sp_interactionid
                            ";
                    #endregion Build cmdText
                    cmd.CommandText = cmdText;
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Clear();
                    #region SQL Parameters
                    cmd.Parameters.Add("@sp_companyid", SqlDbType.Int).Value = sp_companyid;
                    cmd.Parameters.Add("@sp_interactionid", SqlDbType.BigInt).Value = sp_interactionid;
                    #endregion SQL Parameters
                    #region SQL Command Processing
                    SqlDataAdapter ad = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    ad.Fill(dt);
                    gv.DataSource = dt;
                    gv.DataBind();
                    if (dt.Rows.Count > 0)
                    {
                        lblRecordingsGrid.Text = "Call Recordings";
                        lblSystemMessage.Text += "<br />Have recording(s)";
                    }
                    else
                    {
                        lblSystemMessage.Text += "<br />No recordings";
                        gv.DataSource = null;
                        gv.DataBind();
                        lblRecordingsGrid.Text = "";
                    }
                    #endregion SQL Command Processing
                }
                #endregion SQL Command
            }
            #endregion SQL Connection
            #endregion Get the Recording - From MiddleWare
            DateTime dtEnd = DateTime.UtcNow;
            String dtDuration = ghFunctions.SecondsTo((dtEnd - dtStart).TotalSeconds);
            // lblSystemMessage.Text += "<br />Recording Fetch time: " + dtDuration;
        }

    }
    protected void DetailsView_Clear()
    {
        if (dvCallDetails.Rows.Count > 0) { dvCallDetails.DataBind(); dvCallDetails.Visible = false; }
        if (dvDonorDetails.Rows.Count > 0) { dvDonorDetails.DataBind(); dvDonorDetails.Visible = false; }
        if (dvTributeDetails.Rows.Count > 0) { dvTributeDetails.DataBind(); dvTributeDetails.Visible = false; }
        if (dvPaymentDetails.Rows.Count > 0) { dvPaymentDetails.DataBind(); dvPaymentDetails.Visible = false; }
        if (dvContactDetails.Rows.Count > 0) { dvContactDetails.DataBind(); dvContactDetails.Visible = false; }
        if (dvRefundDetails.Rows.Count > 0) { dvRefundDetails.DataBind(); dvRefundDetails.Visible = false; }
        if (dvADUFile.Rows.Count > 0) { dvADUFile.DataBind(); dvADUFile.Visible = false; }
        if (dvGiftDetails.Rows.Count > 0) { dvGiftDetails.DataBind(); dvGiftDetails.Visible = false; }
        if (dvSustainerDetails.Rows.Count > 0) { dvSustainerDetails.DataBind(); dvSustainerDetails.Visible = false; }
        if (dvPaymentDetailsRecurring.Rows.Count > 0) { dvPaymentDetailsRecurring.DataBind(); dvPaymentDetailsRecurring.Visible = false; }
        if (dvInteraction.Rows.Count > 0) { dvInteraction.DataBind(); dvInteraction.Visible = false; }
        if (dvInteractionDetails.Rows.Count > 0) { dvInteractionDetails.DataBind(); dvInteractionDetails.Visible = false; }
        if (dvTokenizationDetails.Rows.Count > 0) { dvTokenizationDetails.DataBind(); dvTokenizationDetails.Visible = false; }
        if (dvHolidayAddress.Rows.Count > 0) { dvHolidayAddress.DataBind(); dvHolidayAddress.Visible = false; }
        if (dvRemoveDetails.Rows.Count > 0) { dvRemoveDetails.DataBind(); dvRemoveDetails.Visible = false; }
        if (gvGiftList.Rows.Count > 0) { gvGiftList.DataBind(); }
        if (gvRecurringList.Rows.Count > 0) { gvRecurringList.SelectedIndex = -1; gvRecurringList.DataBind(); }
        if (gvRecordingsGrid.Rows.Count > 0) { gvRecordingsGrid.DataBind(); pnlRecordings.Visible = false; }
        pnlGiftList.Visible = false;
        pnlRecurringList.Visible = false;
        btnExportDetails.Visible = false;
        Admin_Clear();
    }
    protected void DDL_Load_Dispositions()
    {
        ListBox ddl = ddlDispositions;
        #region Overall Try
        try
        {
            #region SQL Connection
            using (SqlConnection con = new SqlConnection(sqlStrARC))
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
        DropDownList ddl2 = ddlSustainerDesignation;
        #region Overall Try
        try
        {
            #region SQL Connection
            using (SqlConnection con = new SqlConnection(sqlStrARC))
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
                    if (ddl2 != null)
                    {
                        ddl2.DataTextField = "Text";
                        ddl2.DataValueField = "Value";

                        ddl2.Items.Clear();
                        ddl2.DataSource = dt;
                        ddl2.DataBind();
                        if (ddl2.Items.Count > 0)
                        {
                            ddl2.Items.Remove(ddl2.Items[0]);
                        }
                    }

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
            using (SqlConnection con = new SqlConnection(sqlStrARC))
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
                    // Error_General.Text += "<br />CDR: " + Page.User.IsInRole("CDR Fundraising Group").ToString();
                    if (Page.User.IsInRole("CDR Fundraising Group")) { cmd.Parameters.Add("@sp_source", SqlDbType.VarChar, 25).Value = "CDRFG"; }
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

                    if (Page.User.IsInRole("CDR Fundraising Group"))
                    {
                        ddl.SelectedIndex = 0;
                    }

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
    protected void DDL_Load_Refund_Reason()
    {
        DropDownList ddl = ddlRefundReason;
        //ListBox ddl = ddlDesignation;
        #region Overall Try
        try
        {
            #region SQL Connection
            using (SqlConnection con = new SqlConnection(sqlStrARC))
            {
                #region SQL Command
                using (SqlCommand cmd = new SqlCommand("", con))
                {
                    if (con.State == ConnectionState.Closed) { con.Open(); }
                    #region Populate the SQL Command
                    cmd.CommandTimeout = 600;
                    cmd.CommandText = "[dbo].[portal_call_search_get_ddl_refund_reason]";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Clear();
                    #endregion Populate the SQL Command
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
    protected void DDL_Load_Charge_Reason()
    {
        DropDownList ddl = ddlAdminChargeReason;
        #region Overall Try
        try
        {
            #region SQL Connection
            using (SqlConnection con = new SqlConnection(sqlStrARC))
            {
                #region SQL Command
                using (SqlCommand cmd = new SqlCommand("", con))
                {
                    if (con.State == ConnectionState.Closed) { con.Open(); }
                    #region Populate the SQL Command
                    cmd.CommandTimeout = 600;
                    cmd.CommandText = "[dbo].[portal_call_search_get_ddl_charge_reason]";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Clear();
                    #endregion Populate the SQL Command
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
    #region Details View Handling
    /// <summary>
    /// Get the data
    /// </summary>
    protected void DetailsView_Data(Int32 CallID, Int32 DonorID, DetailsView dv)
    {
        #region SQL Connection
        using (SqlConnection con = new SqlConnection(sqlStrARC))
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
                if (dv.ID == "dvPaymentDetailsRecurring") { cmdText = "[dbo].[portal_call_search_get_payment_recurring]"; }


                if (dv.ID == "dvTokenizationDetails")
                {
                    #region Build cmdText
                    cmdText = "";
                    cmdText += @"
SELECT
TOP 1
[ct].[tokenid]
,[ct].[callid]
,REPLICATE('*',14) + RIGHT([ct].[subscriptionid],8) [subscriptionid]
,[ct].[status]
,[ct].[createdate]
FROM [dbo].[call] [c] WITH(NOLOCK)
JOIN [dbo].[donationccinfo] [di] WITH(NOLOCK) ON [di].[callid] = [c].[callid]
JOIN [dbo].[cybersource_log_auth] [cb] WITH(NOLOCK) ON [cb].[externalid] = [di].[id]
JOIN [dbo].[cybersource_tokenization] [ct] WITH(NOLOCK) ON [ct].[callid] = [c].[callid] AND [ct].[donationid] = [di].[id] AND [ct].[authid] = [cb].[id]
WHERE 1=1
AND [di].[callid] = @sp_callid

                            ";
                    cmdText += "\r";
                    #endregion Build cmdText
                    cmd.CommandText = cmdText;
                    cmd.CommandType = CommandType.Text;
                }
                else if (dv.ID == "dvHolidayAddress")
                {
                    #region Build cmdText
                    cmdText = "";
                    cmdText += @"
SELECT
TOP 1
[ha].[callid]
,[ha].[fname]
,[ha].[lname]
,CASE
	WHEN [ha].[prefix] = 0 OR [ha].[prefix] IS NULL THEN ''
	ELSE (SELECT TOP 1 [pt].[AddressPrefix] FROM [dbo].[prefix] [pt] WITH(NOLOCK) WHERE [pt].[PrefixValue] = [ha].[prefix])
END [prefix]

--,[ha].[companyyn]
,[ha].[companyname]
,CASE
	WHEN [ha].[companytypeid] = 0 OR [ha].[companytypeid] IS NULL THEN ''
	ELSE (SELECT TOP 1 [ct].[companyTypeName] FROM [dbo].[companyTypeLookup] [ct] WITH(NOLOCK) WHERE [ct].[companyTypeID] = [ha].[companytypeid])
END [companytype]
,[ha].[address]
,CASE
	WHEN [ha].[suitetype] = 0 OR [ha].[suitetype] IS NULL THEN ''
	ELSE (SELECT TOP 1 [st].[SuiteType] FROM [dbo].[suitetype] [st] WITH(NOLOCK) WHERE [st].[SuiteTypeValue] = [ha].[suitetype])
END [suitetype]
,[ha].[suitenumber]
,[ha].[zip]
,[ha].[city]
,[ha].[state]
,[ha].[country]
FROM [dbo].[callinfo_alternate] [ha] WITH(NOLOCK)
WHERE 1=1
AND [ha].[callid] = @sp_callid                            ";
                    cmdText += "\r";
                    #endregion Build cmdText
                    cmd.CommandText = cmdText;
                    cmd.CommandType = CommandType.Text;
                }
                else
                {
                    cmd.CommandText = cmdText;
                    cmd.CommandType = CommandType.StoredProcedure;
                }
                cmd.Parameters.Clear();
                #region SQL Parameters
                cmd.Parameters.Add(new SqlParameter("@sp_callid", CallID));                
                if (dv.ID == "dvPaymentDetails")
                {
                    if (hdPDcybID.Value.Length > 0 && hdPDcybID.Value != "0" && hdPDcallID.Value == CallID.ToString())
                    {
                        Int32 cbid = Convert.ToInt32(hdPDcybID.Value);
                        cmd.Parameters.Add(new SqlParameter("@sp_cbid", cbid));
                    }
                    else if (gvSearchGrid.SelectedDataKey.Values[1].ToString().Length > 0)
                    {
                        Int32 cbid = Convert.ToInt32(gvSearchGrid.SelectedDataKey.Values[1].ToString());
                        cmd.Parameters.Add(new SqlParameter("@sp_cbid", cbid));
                    }
                }
                if (dv.ID == "dvPaymentDetailsRecurring")
                {
                    cmd.Parameters.Add(new SqlParameter("@sp_cbid", DonorID));
                }
                #endregion SQL Parameters
                #region SQL Processing
                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                ad.Fill(dt);
                dv.DataSource = dt;
                dv.DataBind();
                dv.Visible = true;
                #endregion SQL Processing

            }
            #endregion SQL Command
        }
        #endregion SQL Connection
    }
    protected void DetailsView_DataExchange_Data(Int32 CallID, DetailsView dv)
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
                #region dvInteraction
                if (dv.ID == "dvInteraction") {
                    cmdText = @"
SELECT
TOP 1
[i].[companyid]
,[i].[interactionid]
,[ia].[callid] [callid_arc]
,[fc].[callid] [callid_five9]
,[fa].[agentid]
,[fa].[five9id] [agentid_five9]
FROM [dbo].[interactions] [i] WITH(NOLOCK)
JOIN [dbo].[interactions_arc] [ia] WITH(NOLOCK) ON [ia].[companyid] = [i].[companyid] AND [ia].[interactionid] = [i].[interactionid]
LEFT OUTER JOIN [dbo].[five9_call] [fc] WITH(NOLOCK) ON [fc].[interactionid] = [i].[interactionid] AND [fc].[companyid] = [i].[companyid]

LEFT OUTER JOIN [dbo].[five9_call_disposition] [fcd] WITH(NOLOCK) ON [fcd].[companyid] = [i].[companyid] AND [fcd].[interactionid] = [i].[interactionid]

LEFT OUTER JOIN [dbo].[five9_item] [fid] WITH(NOLOCK) ON [fid].[typeid] = 103000000 AND [fid].[itemid] = [fcd].[dispositionid]
LEFT OUTER JOIN [dbo].[five9_agent] [fa] WITH(NOLOCK) ON [fa].[agentid] = [fcd].[agentid]

WHERE [ia].[callid] = @sp_arc_callid
";
                }
                #endregion
                #region dvInteractionDetails
                if (dv.ID == "dvInteractionDetails")
                {
                    cmdText = @"
SELECT
TOP 1
[i].[companyid]
,[i].[interactionid]
,[i].[originator]
,[i].[destinator]
,[fa].[fullname] [agent_fullname]
,[ia].[dispositionname] [disposition_arc]
,[fid].[name] [disposition_five9]
,[fic].[name] [campaign]
,[fis].[name] [skill]
,[fit].[name] [type]
,[fim].[name] [mediatype]
,[fc].[datestart]
FROM [dbo].[interactions] [i] WITH(NOLOCK)
JOIN [dbo].[interactions_arc] [ia] WITH(NOLOCK) ON [ia].[companyid] = [i].[companyid] AND [ia].[interactionid] = [i].[interactionid]
LEFT OUTER JOIN [dbo].[five9_call] [fc] WITH(NOLOCK) ON [fc].[interactionid] = [i].[interactionid] AND [fc].[companyid] = [i].[companyid]
LEFT OUTER JOIN [dbo].[five9_call_disposition] [fcd] WITH(NOLOCK) ON [fcd].[companyid] = [i].[companyid] AND [fcd].[interactionid] = [i].[interactionid]
LEFT OUTER JOIN [dbo].[five9_item] [fid] WITH(NOLOCK) ON [fid].[typeid] = 103000000 AND [fid].[itemid] = [fcd].[dispositionid]
LEFT OUTER JOIN [dbo].[five9_agent] [fa] WITH(NOLOCK) ON [fa].[agentid] = [fcd].[agentid]
LEFT OUTER JOIN [dbo].[five9_item] [fic] WITH(NOLOCK) ON [fic].[typeid] = 101000000 AND [fic].[itemid] = [fc].[campaignid]
LEFT OUTER JOIN [dbo].[five9_item] [fis] WITH(NOLOCK) ON [fis].[typeid] = 102000000 AND [fis].[itemid] = [fc].[skillid]
LEFT OUTER JOIN [dbo].[five9_item] [fit] WITH(NOLOCK) ON [fit].[typeid] = 104000000 AND [fit].[itemid] = [fc].[typeid]
LEFT OUTER JOIN [dbo].[five9_item] [fim] WITH(NOLOCK) ON [fim].[typeid] = 107000000 AND [fim].[itemid] = [fc].[mediatypeid]
WHERE [ia].[callid] = @sp_arc_callid
";
                }
                #endregion
                cmd.CommandText = cmdText;
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Clear();
                #region SQL Parameters
                cmd.Parameters.Add(new SqlParameter("@sp_arc_callid", CallID));
                #endregion SQL Parameters
                #region SQL Processing
                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                ad.Fill(dt);
                dv.DataSource = dt;
                dv.DataBind();
                dv.Visible = true;
                #endregion SQL Processing

            }
            #endregion SQL Command
        }
        #endregion SQL Connection
    }
    protected void DetailsView_ItemCommand(object sender, DetailsViewCommandEventArgs e)
    {
        Admin_Clear();
        //if (Page.User.IsInRole("System Administrator")) pAdminFunctions.Visible = true;
        if (ghUser.identity_is_admin()) pAdminFunctions.Visible = true;
        DetailsView dv = (DetailsView)sender;
        if (dv.ID == "dvPaymentDetails")
        {
            if (e.CommandName == "Refund")
            {
                Refund_Start(dv, e);
            }
            if (e.CommandName == "Charge")
            {
                Charge_Start(dv, e);
            }
        }
        else if (dv.ID == "dvPaymentDetailsRecurring")
        {
            if (e.CommandName == "Refund")
            {
                Refund_Start(dv, e);
            }
        }
        else if (dv.ID == "dvSustainerDetails")
        {
            if (e.CommandName == "Modify")
            {
                Modify_Sustainer(dv, e);
            }
        }
        else if (dv.ID == "dvDonorDetails")
        {
            if (e.CommandName == "Modify")
            {
                Modify_Donor(dv, e);
            }
        }
    }
    protected void DetailsView_DataBound(object sender, EventArgs e)
    {
        #region DataBound Action
        DetailsView dv = (DetailsView)sender;
        #region DataBound Action for dvDonationDetails
        if (dv.ID == "dvDonationDetails")
        {
            // Nothing
        }
        #endregion DataBound Action for dvDonationDetails
        #region DataBound Action for dvDonorDetails
        else if (dv.ID == "dvDonorDetails")
        {
            // dvDonorDetails
            #region State/Country Populate if Edit Mode
            if (dv.CurrentMode == DetailsViewMode.Edit)
            {
                DropDownList dvState = (DropDownList)dv.FindControl("donorStateUS");
                DropDownList dvProvince = (DropDownList)dv.FindControl("donorStateCA");
                DropDownList dvCountry = (DropDownList)dv.FindControl("donorCountry");
                TextBox dvStateOther = (TextBox)dv.FindControl("donorStateOther");
                Label lblSummary = (Label)dv.FindControl("lblSummary");
                #region State
                if (dvState != null)
                {
                    HiddenField lblState = (HiddenField)dv.FindControl("donorStateCurrent");
                    try
                    {
                        Popuplate_DropDown_FromXML(dvState, "state");
                        if (dvState.Items.Count > 0)
                        {
                            dvState.SelectedIndex = -1;
                            foreach (ListItem li in dvState.Items)
                            {
                                if (li.Value == lblState.Value)
                                {
                                    li.Selected = true;
                                    break;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Error_Save(ex, dv.ID.ToString() + " - DDL State Populate Error");
                    }
                }
                #endregion State
                #region Province
                if (dvProvince != null)
                {
                    HiddenField lblProvince = (HiddenField)dv.FindControl("donorStateCurrent");
                    try
                    {
                        Popuplate_DropDown_FromXML(dvProvince, "province");
                        if (dvProvince.Items.Count > 0)
                        {
                            dvProvince.SelectedIndex = -1;
                            foreach (ListItem li in dvProvince.Items)
                            {
                                if (li.Value == lblProvince.Value)
                                {
                                    li.Selected = true;
                                    break;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Error_Save(ex, dv.ID.ToString() + " - DDL Province Populate Error");
                    }
                }
                #endregion State
                #region Country
                if (dvCountry != null)
                {
                    HiddenField lblCountry = (HiddenField)dv.FindControl("donorCountryCurrent");
                    try
                    {
                        Popuplate_DropDown_FromXML(dvCountry, "country");
                        if (dvCountry.Items.Count > 0)
                        {
                            dvCountry.SelectedIndex = -1;
                            foreach (ListItem li in dvCountry.Items)
                            {
                                if (li.Value == lblCountry.Value)
                                {
                                    li.Selected = true;
                                    break;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Error_Save(ex, dv.ID.ToString() + " - DDL Country Populate Error");
                    }
                }
                #endregion Country
                #region Load JavaScript
                // Does not work; does not hide drop down lists that have jquery-select on them
                //String scriptText = "";
                //scriptText = "dvCountrySwitch('country','stateus','stateca','stateother');";
                //scriptText = scriptText.Replace("country", dvCountry.ClientID.ToString());
                //scriptText = scriptText.Replace("stateus", dvState.ClientID.ToString());
                //scriptText = scriptText.Replace("stateca", dvProvince.ClientID.ToString());
                //scriptText = scriptText.Replace("stateother", dvStateOther.ClientID.ToString());
                //lblSummary.Text = scriptText;
                //ScriptManager.RegisterClientScriptBlock(dtlLabel, this.Page.GetType(), "dvCountrySwitch_Donor", scriptText, true);
                #endregion Load JavaScript
            }
            #endregion
        }
        #endregion DataBound Action for dvDonorDetails
        #region DataBound Action for dvTributeDetails
        else if (dv.ID == "dvTributeDetails")
        {
            // dvTributeDetails
            #region State/Country Populate if Edit Mode
            if (dv.CurrentMode == DetailsViewMode.Edit || dv.Rows.Count == 1)
            {
                #region State
                DropDownList dvState = (DropDownList)dv.FindControl("tributeStateUS");
                if (dvState != null)
                {
                    HiddenField lblState = (HiddenField)dv.FindControl("tributeStateCurrent");
                    try
                    {
                        Popuplate_DropDown_FromXML(dvState, "state");
                        if (lblState != null)
                        {
                            if (dvState.Items.Count > 0)
                            {
                                dvState.SelectedIndex = -1;
                                foreach (ListItem li in dvState.Items)
                                {
                                    if (li.Value == lblState.Value)
                                    {
                                        li.Selected = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Error_Save(ex, dv.ID.ToString() + " - DDL State Populate Error");
                    }
                }
                #endregion State
                #region Province
                DropDownList dvProvince = (DropDownList)dv.FindControl("tributeStateCA");
                if (dvProvince != null)
                {
                    HiddenField lblProvince = (HiddenField)dv.FindControl("tributeStateCurrent");
                    try
                    {
                        Popuplate_DropDown_FromXML(dvProvince, "province");
                        if (lblProvince != null)
                        {
                            if (dvProvince.Items.Count > 0)
                            {
                                dvProvince.SelectedIndex = -1;
                                foreach (ListItem li in dvProvince.Items)
                                {
                                    if (li.Value == lblProvince.Value)
                                    {
                                        li.Selected = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Error_Save(ex, dv.ID.ToString() + " - DDL Province Populate Error");
                    }
                }
                #endregion State
                #region Country
                DropDownList dvCountry = (DropDownList)dv.FindControl("tributeCountry");
                if (dvCountry != null)
                {
                    HiddenField lblCountry = (HiddenField)dv.FindControl("tributeCountryCurrent");
                    try
                    {
                        Popuplate_DropDown_FromXML(dvCountry, "country");
                        if (lblCountry != null)
                        {
                            if (dvCountry.Items.Count > 0)
                            {
                                dvCountry.SelectedIndex = -1;
                                foreach (ListItem li in dvCountry.Items)
                                {
                                    if (li.Value == lblCountry.Value)
                                    {
                                        li.Selected = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Error_Save(ex, dv.ID.ToString() + " - DDL Country Populate Error");
                    }
                }
                #endregion Country
            }
            #endregion
            Error_General.Text = dv.Rows.Count.ToString();
        }
        #endregion DataBound Action for dvTributeDetails
        #region DataBound Action for dvPaymentDetails
        else if (dv.ID == "dvPaymentDetails")
        {
            // Populate the State/Country Drop Down
            #region State/Country Populate if Edit Mode
            if (dvPaymentDetails.CurrentMode == DetailsViewMode.Edit)
            {
                Label lblState = (Label)dvPaymentDetails.FindControl("State");

                DropDownList dvState = (DropDownList)dvPaymentDetails.FindControl("ddlState");
                DropDownList dvCountry = (DropDownList)dvPaymentDetails.FindControl("ddlCountry");
                if (dvState != null && dvCountry != null)
                {
                    //dtlLabel.Text = "Found DDL 1";
                    if (lblState != null)
                    {
                        //dtlLabel.Text += " [" + lblState.Text + "]";
                    }
                    try
                    {
                        Popuplate_DropDown_FromXML(dvState, "state");
                        Popuplate_DropDown_FromXML(dvCountry, "country");
                    }
                    catch (Exception ex)
                    {
                        Error_Save(ex, "dvPaymentDetails - DDL State/Country Populate Error");
                    }
                }
                else if (dvState != null)
                {
                    //dtlLabel.Text = "Found DDL 2";
                    try
                    {
                        Popuplate_DropDown_FromXML(dvState, "state");
                    }
                    catch (Exception ex)
                    {
                        Error_Save(ex, "dvPaymentDetails - DDL State Populate Error");
                    }
                }
                else if (dvCountry != null)
                {
                    //dtlLabel.Text = "Found DDL 3";
                    try
                    {
                        Popuplate_DropDown_FromXML(dvCountry, "country");
                    }
                    catch (Exception ex)
                    {
                        Error_Save(ex, "dvPaymentDetails - DDL Country Populate Error");
                    }
                }
                else
                {
                    //dtlLabel.Text = "Did not find DDL";
                    #region DeBug Code
                    //dtlLabel.Text += " [" + dvPaymentDetails.Rows.Count.ToString() + "]";
                    //foreach (DetailsViewRow dvr in dvPaymentDetails.Rows)
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
        #endregion DataBound Action for dvPaymentDetails
        #region DataBound Action for dvContact
        else if (dv.ID == "dvContact")
        {
            try
            {
                // Label aphone = (Label)dvContact.FindControl("aphone");
                // if (aphone != null) { if (aphone.Text.Length == 0) { dv.Fields[5].Visible = false; dv.Fields[6].Visible = false; } else { dv.Fields[5].Visible = true; dv.Fields[6].Visible = true; } }
            }
            catch
            {
            }
        }
        #endregion DataBound Action for dvContact
        #endregion DataBound Action
    }
    protected void DetailsView_ModeChanging(object sender, DetailsViewModeEventArgs e)
    {
        #region ModeChanging Action
        try
        {
            // AdminToggle_Hide();
            DetailsView dv = (DetailsView)sender;
            dv.ChangeMode(e.NewMode);

            Int32 callid = Convert.ToInt32(gvSearchGrid.SelectedDataKey.Values[0].ToString());
            Int32 donorid = 0; // Convert.ToInt32(gvSearchGrid.SelectedDataKey.Values[1].ToString());
            // Int32 donationid = Convert.ToInt32(((Label)dvDonationDetails.FindControl("DonationID")).Text.ToString());
            // Int32 donorid = Convert.ToInt32(((HiddenField)dvDonationDetails.FindControl("DonorID")).Value.ToString());
            DetailsView_Data(callid, donorid, dv);

            if (e.NewMode == DetailsViewMode.Edit)
            {
                dv.AllowPaging = false;
            }
            else
            {
                dv.AllowPaging = true;
            }
        }
        catch (Exception ex)
        {
            Error_Save(ex, "DetailsView_ModeChanging");
        }
        #endregion ModeChanging Action
    }
    protected void DetailsView_ItemUpdating(object sender, DetailsViewUpdateEventArgs e)
    {
        Boolean update = false;
        String error = "";
        //Int32 donationid = Convert.ToInt32(gvResults.SelectedDataKey.Values[0].ToString());
        //Int32 donorid = Convert.ToInt32(gvResults.SelectedDataKey.Values[1].ToString());
        //Int32 donationid = Convert.ToInt32(((Label)dvDonationDetails.FindControl("DonationID")).Text.ToString());
        //Int32 donorid = Convert.ToInt32(((HiddenField)dvDonationDetails.FindControl("DonorID")).Value.ToString());
        Int32 callid = Convert.ToInt32(gvSearchGrid.SelectedDataKey.Values[0].ToString());
        Int32 donorid = 0; // Convert.ToInt32(gvSearchGrid.SelectedDataKey.Values[1].ToString());
        DetailsView dv = (DetailsView)sender;
        if (update)
        {

            #region ItemUpdating Action
            
            #region ItemUpdating Action for dvDonationDetails
            if (dv.ID == "dvDonationDetails")
            {
                #region Try: dvDonationDetails Update
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
                #endregion Try: dvDonationDetails Update
                #region Catch: dvDonationDetails Update
                catch (Exception ex)
                {
                    Error_Save(ex, "Update User: User Credentials");
                    update = false;
                    error = "Internal server error during update process.";
                }
                #endregion Catch: dvDonationDetails Update
            }
            #endregion ItemUpdating Action for dvDonationDetails
            #region ItemUpdating Action for dvDonorDetails
            else if (dv.ID == "dvDonorDetails")
            {
                #region Try: dvDonorDetails Update
                try
                {
                    String dFirstName = ((TextBox)dv.FindControl("FirstName")).Text.ToString().Trim();
                    String dLastName = ((TextBox)dv.FindControl("LastName")).Text.ToString().Trim();
                    String dAddress1 = ((TextBox)dv.FindControl("Address1")).Text.ToString().Trim();
                    String dAddress2 = ((TextBox)dv.FindControl("Address2")).Text.ToString().Trim();
                    String dCity = ((TextBox)dv.FindControl("City")).Text.ToString().Trim();
                    String dState = "";
                    String dZip = ((TextBox)dv.FindControl("Zip")).Text.ToString().Trim();
                    String dCountry = ((DropDownList)dv.FindControl("donorCountry")).Text.ToString().Trim();
                    if (dCountry == "US") { dState = ((DropDownList)dv.FindControl("donorStateUS")).Text.ToString().Trim(); }
                    else if (dCountry == "CA") { dState = ((DropDownList)dv.FindControl("donorStateCA")).Text.ToString().Trim(); }
                    else { dState = ((TextBox)dv.FindControl("donorStateOther")).Text.ToString().Trim(); }

                    if (dFirstName != null
                        && dFirstName.Length > 0
                        )
                    {
                        update = true;
                    }
                    else
                    {
                        error = "Field Validation failed";
                    }
                    if (update)
                    {
                        using (SqlCommand cmd = new SqlCommand())
                        {

                            cmd.CommandText = "[dbo].[donation_update_donor]";
                            cmd.CommandType = CommandType.StoredProcedure;
                            if (Session["userid"] == null) { ghUser.identity_get_userid(); }
                            cmd.Parameters.Add(new SqlParameter("@sp_userid", Session["userid"]));
                            cmd.Parameters.Add(new SqlParameter("@sp_donorid", donorid));
                            cmd.Parameters.Add(new SqlParameter("@sp_donationid", callid));
                            cmd.Parameters.Add(new SqlParameter("@sp_firstname", dFirstName));
                            cmd.Parameters.Add(new SqlParameter("@sp_lastname", dLastName));
                            cmd.Parameters.Add(new SqlParameter("@sp_address1", dAddress1));
                            cmd.Parameters.Add(new SqlParameter("@sp_address2", dAddress2));
                            cmd.Parameters.Add(new SqlParameter("@sp_city", dCity));
                            cmd.Parameters.Add(new SqlParameter("@sp_zip", dZip));
                            cmd.Parameters.Add(new SqlParameter("@sp_state", dState));
                            cmd.Parameters.Add(new SqlParameter("@sp_country", dCountry));

                            DetailsView_UpdateRecord(cmd);
                        }
                    }
                }
                #endregion Try: dvDonorDetails Update
                #region Catch: dvDonorDetails Update
                catch (Exception ex)
                {
                    Error_Save(ex, "Update Donation: Donor");
                    update = false;
                    error = "Internal server error during update process.";
                }
                #endregion Catch: dvDonorDetails Update
            }
            #endregion ItemUpdating Action for dvDonorDetails
            #region ItemUpdating Action for dvTributeDetails
            else if (dv.ID == "dvTributeDetails")
            {
                #region Try: dvTributeDetails Update
                try
                {
                    Int32 trTypeID = Convert.ToInt32(((DropDownList)dv.FindControl("ddlTribute")).SelectedValue.ToString());
                    String trFirstName = ((TextBox)dv.FindControl("FirstName")).Text.ToString().Trim();
                    String trLastName = ((TextBox)dv.FindControl("LastName")).Text.ToString().Trim();
                    String trSndFirst = ((TextBox)dv.FindControl("SndFirst")).Text.ToString().Trim();
                    String trSndLast = ((TextBox)dv.FindControl("SndLast")).Text.ToString().Trim();
                    String trRcpFirst = ((TextBox)dv.FindControl("RcpFirst")).Text.ToString().Trim();
                    String trRcpLast = ((TextBox)dv.FindControl("RcpLast")).Text.ToString().Trim();
                    String trAddress1 = ((TextBox)dv.FindControl("Address1")).Text.ToString().Trim();
                    String trAddress2 = ((TextBox)dv.FindControl("Address2")).Text.ToString().Trim();
                    String trCity = ((TextBox)dv.FindControl("City")).Text.ToString().Trim();
                    String trState = "";
                    String trZip = ((TextBox)dv.FindControl("Zip")).Text.ToString().Trim();
                    String trCountry = ((DropDownList)dv.FindControl("tributeCountry")).Text.ToString().Trim();
                    if (trCountry == "US") { trState = ((DropDownList)dv.FindControl("tributeStateUS")).Text.ToString().Trim(); }
                    else if (trCountry == "CA") { trState = ((DropDownList)dv.FindControl("tributeStateCA")).Text.ToString().Trim(); }
                    else { trState = ((TextBox)dv.FindControl("tributeStateOther")).Text.ToString().Trim(); }
                    String trMessage = ((TextBox)dv.FindControl("Message")).Text.ToString().Trim();

                    if (trFirstName != null
                        && trFirstName.Length > 0
                        )
                    {
                        update = true;
                    }
                    else
                    {
                        error = "Field Validation failed";
                    }
                    if (update)
                    {
                        using (SqlCommand cmd = new SqlCommand())
                        {
                            Int32 seqid = Convert.ToInt32(((HiddenField)dv.FindControl("SeqID")).Value.ToString());

                            cmd.CommandText = "[dbo].[donation_update_tribute]";
                            cmd.CommandType = CommandType.StoredProcedure;

                            if (Session["userid"] == null) { ghUser.identity_get_userid(); }
                            cmd.Parameters.Add(new SqlParameter("@sp_userid", Session["userid"]));
                            cmd.Parameters.Add(new SqlParameter("@sp_donationid", callid));
                            cmd.Parameters.Add(new SqlParameter("@sp_typeid", trTypeID));
                            cmd.Parameters.Add(new SqlParameter("@sp_seqid", seqid));
                            cmd.Parameters.Add(new SqlParameter("@sp_firstname", trFirstName));
                            cmd.Parameters.Add(new SqlParameter("@sp_lastname", trLastName));
                            cmd.Parameters.Add(new SqlParameter("@sp_sndfirst", trSndFirst));
                            cmd.Parameters.Add(new SqlParameter("@sp_sndlast", trSndLast));
                            cmd.Parameters.Add(new SqlParameter("@sp_rcpfirst", trRcpFirst));
                            cmd.Parameters.Add(new SqlParameter("@sp_rcplast", trRcpLast));
                            cmd.Parameters.Add(new SqlParameter("@sp_address1", trAddress1));
                            cmd.Parameters.Add(new SqlParameter("@sp_address2", trAddress2));
                            cmd.Parameters.Add(new SqlParameter("@sp_city", trCity));
                            cmd.Parameters.Add(new SqlParameter("@sp_zip", trZip));
                            cmd.Parameters.Add(new SqlParameter("@sp_state", trState));
                            cmd.Parameters.Add(new SqlParameter("@sp_country", trCountry));
                            cmd.Parameters.Add(new SqlParameter("@sp_message", trMessage));

                            DetailsView_UpdateRecord(cmd);
                        }
                    }
                }
                #endregion Try: dvTributeDetails Update
                #region Catch: dvTributeDetails Update
                catch (Exception ex)
                {
                    Error_Save(ex, "Update Donation: Tribute");
                    update = false;
                    error = "Internal server error during update process.";
                }
                #endregion Catch: dvTributeDetails Update
            }
            #endregion ItemUpdating Action for dvTributeDetails
            #region ItemUpdating Action for dvPaymentDetails
            else if (dv.ID == "dvPaymentDetails")
            {
                #region Try: dvPaymentDetails Update
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
                            Int32 UserID = 0;// Convert.ToInt32(gvResults.SelectedDataKey.Values[0].ToString());
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
                #endregion Try: dvPaymentDetails Update
                #region Catch: dvPaymentDetails Update
                catch (Exception ex)
                {
                    Error_Save(ex, "Update User: User Address");
                    update = false;
                    error = "Internal server error during update process.";
                }
                #endregion Catch: dvPaymentDetails Update

            }
            #endregion ItemUpdating Action for dvPaymentDetails
            #region ItemUpdating Action for dvContact
            else if (dv.ID == "dvContact")
            {
                #region Try: dvContact Update
                try
                {
                    String Email = ((TextBox)dv.FindControl("email")).Text.ToString().Trim();
                    String eSeqID = ((HiddenField)dv.FindControl("eSeqID")).Value.ToString().Trim();

                    String Phone = ((TextBox)dv.FindControl("phone")).Text.ToString().Trim();
                    String pSeqID = ((HiddenField)dv.FindControl("pSeqID")).Value.ToString().Trim();

                    //String aniPhone = ((TextBox)dv.FindControl("aphone")).Text.ToString().Trim();
                    //String apSeqID = ((HiddenField)dv.FindControl("apSeqID")).Value.ToString().Trim();

                    if ((Email != null && Email.Length > 0)
                        || (Phone != null && Phone.Length > 0))
                    {
                        update = true;
                    }
                    else
                    {
                        error = "Field Validation failed";
                    }
                    if (update)
                    {
                        using (SqlCommand cmd = new SqlCommand())
                        {

                            cmd.CommandText = "[dbo].[donation_update_contact]";
                            cmd.CommandType = CommandType.StoredProcedure;
                            if (Session["userid"] == null) { ghUser.identity_get_userid(); }
                            cmd.Parameters.Add(new SqlParameter("@sp_userid", Session["userid"]));
                            cmd.Parameters.Add(new SqlParameter("@sp_donorid", donorid));
                            cmd.Parameters.Add(new SqlParameter("@sp_donationid", callid));
                            cmd.Parameters.Add(new SqlParameter("@sp_email", Email));
                            cmd.Parameters.Add(new SqlParameter("@sp_eseqid", eSeqID));
                            cmd.Parameters.Add(new SqlParameter("@sp_phone", Phone));
                            cmd.Parameters.Add(new SqlParameter("@sp_pseqid", pSeqID));
                            //cmd.Parameters.Add(new SqlParameter("@sp_pahone", aniPhone));
                            //cmd.Parameters.Add(new SqlParameter("@sp_paseqid", apSeqID));

                            DetailsView_UpdateRecord(cmd);
                        }
                    }
                }
                #endregion Try: dvContact Update
                #region Catch: dvContact Update
                catch (Exception ex)
                {
                    Error_Save(ex, "Update Donation: Donor");
                    update = false;
                    error = "Internal server error during update process.";
                }
                #endregion Catch: dvContact Update
            }
            #endregion ItemUpdating Action for dvContact
            if (update)
            {
                dv.ChangeMode(DetailsViewMode.ReadOnly);
                DetailsView_Data(callid, donorid, dv);
                //GridView_Data(0, "", gvResults);
            }
            else
            {
                WriteToLabel("add", "Cyan", "<br /><br />There was an error updating your record.<br />Please review the below message:", dtlLabel);
                WriteToLabel("add", "Red", "<br /><br />" + error, dtlLabel);
            }
            #endregion ItemUpdating Action
        }
        else
        {
            WriteToLabel("new", "Red", "Update function not ready.", lblDonorDetails);
            WriteToLabel("new", "Red", "<br /><br />" + "Update function not ready.", dtlLabel);
            dv.ChangeMode(DetailsViewMode.ReadOnly);
            DetailsView_Data(callid, donorid, dv);
        }

    }
    protected void DetailsView_ItemUpdated(object sender, EventArgs e)
    {
        #region ItemUpdated Action
        dtlLabel.Text = "DetailsView_ItemUpdated";
        #endregion ItemUpdated Action
    }
    protected void DetailsView_UpdateRecord(SqlCommand cmd)
    {
        #region Using: SqlConnection
       
        using (SqlConnection con = new SqlConnection(sqlStrARC))
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
                                for (int i = 0; i < sqlRdr.FieldCount; i++)
                                {
                                    WriteToLabel("add", "Blue", "<br />" + sqlRdr.GetName(i) + ": " + sqlRdr[i].ToString(), dtlLabel);
                                }
                                //WriteToLabel("add", "Blue", "<br />" + sqlRdr.GetName(0) + ": " + sqlRdr[0].ToString(), dtlLabel);
                                //if (sqlRdr.FieldCount >= 1) { WriteToLabel("add", "Blue", "<br />" + sqlRdr.GetName(1) + ": " + sqlRdr[1].ToString(), dtlLabel); }
                                //if (sqlRdr.FieldCount >= 1) { WriteToLabel("add", "Blue", "<br />" + sqlRdr.GetName(1) + ": " + sqlRdr[1].ToString(), dtlLabel); }
                                //if (sqlRdr.FieldCount >= 1) { WriteToLabel("add", "Blue", "<br />" + sqlRdr.GetName(1) + ": " + sqlRdr[1].ToString(), dtlLabel); }
                                //if (sqlRdr.FieldCount >= 1) { WriteToLabel("add", "Blue", "<br />" + sqlRdr.GetName(1) + ": " + sqlRdr[1].ToString(), dtlLabel); }
                                //if (sqlRdr.FieldCount >= 1) { WriteToLabel("add", "Blue", "<br />" + sqlRdr.GetName(1) + ": " + sqlRdr[1].ToString(), dtlLabel); }
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

    protected void GridView_Data2(Int32 CallID, Int32 DonorID, GridView gv, Panel pnl)
    {
        gv.SelectedIndex = -1;
        #region SQL Connection
        using (SqlConnection con = new SqlConnection(sqlStrARC))
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
                if (dt.Rows.Count > 0)
                {
                    pnl.Visible = true;
                    gv.Visible = true;
                    if (gv.ID == "gvRecurringList")
                    {
                        gvRecurringListExport.DataSource = dt;
                        gvRecurringListExport.DataBind();
                        gvRecurringListExport.Visible = true;
                    }
                }
                else
                {
                    pnl.Visible = false;
                    gv.Visible = false;
                    if (gv.ID == "gvRecurringList")
                    {
                        gvRecurringListExport.DataSource = null;
                        gvRecurringListExport.DataBind();
                        gvRecurringListExport.Visible = false;
                    }
                }

                #endregion SQL Processing

            }
            #endregion SQL Command
        }
        #endregion SQL Connection
    }
    #endregion Details View Handling
    #region Admin Functions
    protected void Admin_Clear()
    {
        pAdminFunctions.Visible = false;
        Donor_Clear();
        Sustainer_Clear();
        Refund_Clear();
        Charge_Clear();
    }
    protected void Charge_Clear()
    {
        pAdminCharge.Visible = false;
        pAdminChargeSubmit.Visible = false;
        pAdminChargeResponse.Visible = false;
        pAdminChargeAmount.Visible = false;
    }
    protected void Donor_Clear()
    {
        // Hide all panels related to donor
        pAdminDonor.Visible = false;
        pAdminDonorDetails.Visible = false;
        pAdminDonorSubmit.Visible = false;
        pAdminDonorResponse.Visible = false;
    }
    protected void Sustainer_Clear()
    {
        // Hide all panels related to sustainer
        pAdminSustainer.Visible = false;
    }
    protected void Refund_Clear()
    {
        // Hide all panels related to refund
        pAdminRefundResponse.Visible = false;
        pAdminRefundSubmit.Visible = false;
        pAdminRefundAmount.Visible = false;
        pAdminRefundType.Visible = false;
        pAdminRefund.Visible = false;
    }
    #region Refund Processing
    /// <summary>
    /// Verify if the record is properly set for refund
    /// Verify the user has access to do refunds
    /// Load the information needed for the refund
    /// Determine if "Follow On" or "Stand Alone" refund is needed
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Refund_FakeIt(object sender, EventArgs e)
    {
    }
    protected void Refund_Start(object sender, EventArgs e)
    {
        #region Refund - Start
        // dvPaymentDetails | dvPaymentDetailsRecurring
        DetailsView dv = (DetailsView)sender;
        pAdminRefund.Visible = true;
        tbRefundAmount.Text = "";
        tbRefundNote.Text = "";
        ddlRefundReason.SelectedIndex = -1;
        #region Refund - Start - Try
        // Get the CallID the CybersourceID than get the rest from SQL
        // This will ensure we are processing with the latest information
        try
        {
            lblRefundTemp.Text = "";
            lblRefundProcessing.Text = "Processing start...";
            int dvCallID = Convert.ToInt32(dv.DataKey["callid"].ToString());
            int dvCybID = Convert.ToInt32(dv.DataKey["cbid"].ToString());
            double dvAmount = 0;

            //lblRefundTemp.Text += "<br />dv CallID " + dvCallID.ToString();
            //lblRefundTemp.Text += "<br />dv CybID " + dvCybID.ToString();
            // Now get the stuff from SQL
            #region SqlConnection
            using (SqlConnection con = new SqlConnection(sqlStrARC))
            {
                #region SqlCommand cmd
                using (SqlCommand cmd = new SqlCommand("", con))
                {
                    #region Populate the SQL Command
                    cmd.CommandTimeout = 600;
                    #region Build cmdText
                    String cmdText = "";
                    if (dv.ID == "dvPaymentDetails")
                    {
                        cmdText += @"
                                SELECT
                                [cb].[id]
                                ,[cb].[createdate]
                                ,[cb].[status]
                                ,[cb].[decision]
                                ,[di].[callid]
                                ,[di].[donationamount] [amount]
                                ,(SELECT SUM([cr].[cccreditreply_amount]) FROM [dbo].[cybersource_log_refund] [cr] WITH(NOLOCK) WHERE [cr].[externalid] = [di].[id] AND [cr].[reasoncode] = '100') [amount_ref]
                                FROM [dbo].[cybersource_log_auth] [cb] WITH(NOLOCK)
                                JOIN [dbo].[donationccinfo] [di] WITH(NOLOCK) ON [di].[id] = [cb].[externalid]
                                WHERE 1=1
                                AND [cb].[id] = @sp_cybid
                            ";
                    }
                    if (dv.ID == "dvPaymentDetailsRecurring")
                    {
                        cmdText += @"
                                SELECT
                                [cb].[id]
                                ,[cb].[createdate]
                                ,[cb].[status]
                                ,[cb].[decision]
                                ,[di].[callid]
                                ,[di].[donationamount] [amount]
                                ,(SELECT SUM([cr].[cccreditreply_amount]) FROM [dbo].[cybersource_log_refund] [cr] WITH(NOLOCK) WHERE [cr].[externalid] = [di].[id] AND [cr].[reasoncode] = '100') [amount_ref]
                                FROM [dbo].[cybersource_log_auth] [cb] WITH(NOLOCK)
                                JOIN [dbo].[donation_recurring_log] [drl] WITH(NOLOCK) ON [drl].[recurringid] = [cb].[externalid]
                                JOIN [dbo].[donationccinfo] [di] WITH(NOLOCK) ON [di].[id] = [drl].[donationid]
                                WHERE 1=1
                                AND [cb].[id] = @sp_cybid
                            ";

                    }
                    #endregion Build cmdText
                    cmd.CommandText = cmdText;
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Clear();
                    #endregion Populate the SQL Command
                    #region Populate the SQL Params
                    cmd.Parameters.Add(new SqlParameter("@sp_cybid", dvCybID));
                    #endregion Populate the SQL Params
                    #region Process SQL Command - Try
                    try
                    {
                        if (con.State == ConnectionState.Closed) { con.Open(); }
                        using (SqlDataReader sqlRdr = cmd.ExecuteReader())
                        {
                            if (sqlRdr.HasRows)
                            {
                                while (sqlRdr.Read())
                                {
                                    // We just check to see if the transaction is valid
                                    // If so; let the user hit the submit button
                                    // When the user hits submit; we fetch from SQL again for processing
                                    // This will prevent the user from sitting on the data and causing errors
                                    // To validate:
                                    //        ensure donation status is settled or refund processed with amount left
                                    //        ensure donation date is within follow-on/stand-alone date range
                                    #region Populate Refund Data
                                    bool doRefund = false;
                                    if (sqlRdr["status"].ToString() == "Settled")
                                    {
                                        //lblRefundTemp.Text += "<br />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;ok for processing";
                                        doRefund = true;
                                        Double.TryParse(sqlRdr["amount"].ToString(), out dvAmount);
                                    }
                                    else if (sqlRdr["status"].ToString() == "Cancelled" && sqlRdr["decision"].ToString() == "ACCEPT")
                                    {
                                        //lblRefundTemp.Text += "<br />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;ok for processing";
                                        doRefund = true;
                                        Double.TryParse(sqlRdr["amount"].ToString(), out dvAmount);
                                    }
                                    else if (sqlRdr["status"].ToString() == "Refunded")
                                    {
                                        // Check remaining amount                                        
                                        
                                        double dvAmountRef = 0;
                                        Double.TryParse(sqlRdr["amount_ref"].ToString(), out dvAmountRef);
                                        if (dvAmountRef > 0)
                                        {
                                            Double.TryParse(sqlRdr["amount"].ToString(), out dvAmount);
                                            if (dvAmount > 0)
                                            {
                                                if ((dvAmount - dvAmountRef) > 0)
                                                {
                                                    dvAmount = dvAmount - dvAmountRef;
                                                    //lblRefundTemp.Text += "<br />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;ok for processing partial";
                                                    doRefund = true;
                                                }
                                            }
                                        }
                                        if (!doRefund)
                                        {
                                            lblRefundTemp.Text += "<br />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;NOT ok for processing partial";
                                        }
                                    }
                                    //lblRefundTemp.Text += "<br />sql callid " + sqlRdr["callid"].ToString();
                                    //lblRefundTemp.Text += "<br />sql donationid " + sqlRdr["externalid"].ToString();
                                    //lblRefundTemp.Text += "<br />sql ccnum " + sqlRdr["ccnum"].ToString().Substring(sqlRdr["ccnum"].ToString().Length - 4, 4);
                                    //lblRefundTemp.Text += "<br />sql status " + sqlRdr["status"].ToString();
                                    //lblRefundTemp.Text += "<br />sql decision " + sqlRdr["decision"].ToString();
                                    //lblRefundTemp.Text += "<br />sql createdate " + sqlRdr["createdate"].ToString();
                                    if (doRefund)
                                    {
                                        lblRefundProcessing.Text += "<br />Transaction in valid status; may process refund.";
                                        btnRefundSubmit.Enabled = true;
                                        DateTime dtChargeDate;
                                        DateTime.TryParse(sqlRdr["createdate"].ToString(), out dtChargeDate);
                                        Int32 foLimit = 59; // Days that a Follow On Credit can be performed
                                        if (dtChargeDate != null && (DateTime.UtcNow - dtChargeDate).TotalDays < foLimit)
                                        {
                                            //lblRefundTemp.Text += "<br />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;ok follow on";
                                            pAdminRefundType.Visible = true;
                                            pAdminRefundFO.Visible = true;
                                            pAdminRefundSA.Visible = false;
                                            lblRefundAmount.Text = "Refund Amount:";
                                            lblRefundType.Text = "Follow On";
                                            pAdminRefundAmount.Visible = true;
                                        }
                                        else
                                        {
                                            lblRefundTemp.Text += "<br />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;do stand alone";
                                            lblRefundTemp.Text += "<br />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;This is not configured yet, so sit still.";
                                            pAdminRefundType.Visible = true;
                                            pAdminRefundSA.Visible = true;
                                            pAdminRefundFO.Visible = false;
                                            lblRefundAmount.Text = "Credit Amount:";
                                            lblRefundType.Text = "Stand Alone";
                                            btnRefundSubmit.Enabled = false;
                                            pAdminRefundAmount.Visible = false;
                                        }
                                        
                                        pAdminRefundSubmit.Visible = true;
                                        lblRefCallID.Text = sqlRdr["callid"].ToString();
                                        // This should be the remaining amount when handling partial refunds
                                        lblCurrentAmount.Text = dvAmount.ToString();
                                        lblCurrentCYBID.Text = dvCybID.ToString();
                                    }
                                    else
                                    {
                                        btnRefundSubmit.Enabled = false;
                                        lblRefundProcessing.Text += "<br />Transaction not valid, refund not doable.";
                                        pAdminRefundSubmit.Visible = true;
                                    }
                                    #endregion Populate Refund Data
                                }
                            }
                            else
                            {
                                lblRefundTemp.Text += "<br />sql No records...";
                            }
                        }
                    }
                    #endregion Process SQL Command - Try
                    #region Process SQL Command - Catch
                    catch (Exception ex)
                    {
                        lblErrorDV.Text = "*** ERROR IN PROCESSING -- SEE BOTTOM OF PAGE ***";
                        lblErrorDV.Text += "<br />*** TRY LOGIN OUT/IN AND RE-ATTEMPT ***";
                        Error_Display(ex, "Refund - SQL - Catch", msgLabel);
                    }
                    #endregion Process SQL Command - Catch
                }
                #endregion SqlCommand cmd
            }
            #endregion SqlConnection
        }
        #endregion Refund - Start - Try
        #region Refund - Start - Catch
        catch (Exception ex)
        {
            lblErrorDV.Text = "*** ERROR IN PROCESSING -- SEE BOTTOM OF PAGE ***";
            lblErrorDV.Text += "<br />*** TRY LOGIN OUT/IN AND RE-ATTEMPT ***";
            Error_Display(ex, "Refund - Start - Catch", msgLabel);
        }
        #endregion Refund - Start - Catch
        #endregion Refund - Start
    }
    protected void CreditTry(object sender, EventArgs e)
    {
        bool doRefund = true;
        int dvCallID = Convert.ToInt32(dvPaymentDetails.DataKey["callid"].ToString());
        //int dvCybID = Convert.ToInt32(dvPaymentDetails.DataKey["cbid"].ToString());
        int dvCybID = Convert.ToInt32(lblCurrentCYBID.Text);
        dtlLabel.Text = "";
        lblRefundResponse.Text = "";
        //lblRefundProcessing.Text = "";
        #region Validate Request
        // Validate the refund amount
        // Process the refund
        double amountcurrent = 0;
        Double.TryParse(lblCurrentAmount.Text, out amountcurrent);
        double amountrefund = 0;
        Double.TryParse(tbRefundAmount.Text, out amountrefund);
        if (amountrefund <= 0)
        {
            lblRefundResponse.Text += "<li>refund amount can not be zero</li>";
            doRefund = false;
        }
        else if (amountrefund > amountcurrent)
        {
            lblRefundResponse.Text += "<li>refund amount exceeds current amount</li>";
            doRefund = false;
        }
        if (ddlRefundReason.SelectedIndex < 1)
        {
            lblRefundResponse.Text += "<li>refund reason is required</li>";
            doRefund = false;
        }
        if (tbRefundNote.Text.Trim().Length <= 0)
        {
            lblRefundResponse.Text += "<li>refund note is required</li>";
            doRefund = false;
        }
        if (tbRefundPassword.Text.Trim().Length <= 0)
        {
            lblRefundResponse.Text += "<li>refund password is required</li>";
            doRefund = false;
        }
        else if (tbRefundPassword.Text.Trim() != "arcref2017")
        {
            lblRefundResponse.Text += "<li>refund password is invalid</li>";
            doRefund = false;
        }
        #endregion Validate Request
        if (!doRefund)
        {
            lblRefundResponse.Text = String.Format("<br /><div style='color: darkred;font-weight: bold;'>*** FAILED ****<div style='margin-left: 25px;'><ul>{0}</ul></div></div>", lblRefundResponse.Text);
        }
        else
        {
            // We move the refund processing to a class to keep this page cleaner
            // All we care about is the result; did it process? what was the response?
            ghCyb.cybProcess doRefundCyb = new ghCyb.cybProcess();
            doRefundCyb = ghCyb.cybRefundFollowOn(dvCybID, amountrefund, ddlRefundReason.SelectedItem.Text, tbRefundNote.Text.Trim(), lblRefundResponse);

            //lblRefundResponse.Text += String.Format("<br />... do refund: {0}|{1}", amountcurrent, amountrefund);
            lblRefundResponse.Text += "<br />Status: " + doRefundCyb.status;
            lblRefundResponse.Text += "<br />Message: " + doRefundCyb.message;
            if (doRefundCyb.status == "Refunded")
            {
                // Everything worked good; we should refresh things.. just know that when you do, some stuff will go poof
                GridView_Refresh_Manual(sender, e);
                pAdminRefund.Visible = true;
                pAdminRefundResponse.Visible = true;

            }
        }
        lblRefundResponse.Text += "<br />If there are any  issues; contact IT.";
        pAdminRefundResponse.Visible = true;
    }
    protected void Refund_Cancel(object sender, EventArgs e)
    {
        Refund_Clear();
    }
    #endregion Refund Processing
    #region Charge Processing
    protected void Charge_Start(object sender, EventArgs e)
    {
        #region Charge - Start
        Label lbl = lblAdminCharge;
        pAdminCharge.Visible = true;
        pAdminChargeSubmit.Visible = true;
        pAdminChargeAmount.Visible = true;
        lblAdminChargeProcessing.Text = "";
        hdPDcallID.Value = "";
        hdPDcybID.Value = "";
        #region Charge - Start - Try
        try
        {
            lblAdminChargeTemp.Text = "";
            lblAdminCharge.Text = "Processing start...";
            int dvCallID = Convert.ToInt32(dvPaymentDetails.DataKey["callid"].ToString());
            int dvCybID = 0;
            if (dvPaymentDetails.DataKey["cbid"].ToString().Length > 0) dvCybID = Convert.ToInt32(dvPaymentDetails.DataKey["cbid"].ToString());
            double dvAmount = 0;
            #region SqlConnection
            using (SqlConnection con = new SqlConnection(sqlStrARC))
            {
                #region SqlCommand cmd
                using (SqlCommand cmd = new SqlCommand("", con))
                {
                    #region Populate the SQL Command
                    cmd.CommandTimeout = 600;
                    #region Build cmdText
                    String cmdText = "";
                    cmdText += @"
                                SELECT
                                [cb].[id]
                                ,[cb].[createdate]
                                ,[cb].[status]
                                ,[di].[callid]
                                ,[di].[donationamount] [amount]
                                ,[ir].[callid] [ir_callid]
                                FROM [dbo].[call] [c] WITH(NOLOCK)
                                JOIN [dbo].[callinfo] [ci] WITH(NOLOCK) ON [ci].[callid] = [c].[callid]
                                JOIN [dbo].[donationccinfo] [di] WITH(NOLOCK) ON [di].[callid] = [c].[callid]
                                LEFT OUTER JOIN [dbo].[cybersource_log_auth] [cb] WITH(NOLOCK) ON [cb].[externalid] = [di].[id]
                                LEFT OUTER JOIN [dbo].[cybersource_reason_code] [cbr] WITH(NOLOCK) ON [cbr].[code] = [cb].[reasoncode]
                                LEFT OUTER JOIN [dbo].[ivr_record] [ir] WITH(NOLOCK) ON [ir].[callid] = [c].[callid]
                                WHERE 1=1
                                AND [c].[callid] = @sp_callid
                                AND ([cb].[id] = @sp_cybid OR [cb].[id] IS NULL)
                            ";
                    #endregion Build cmdText
                    cmd.CommandText = cmdText;
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Clear();
                    #endregion Populate the SQL Command
                    #region Populate the SQL Params
                    cmd.Parameters.Add(new SqlParameter("@sp_callid", dvCallID));
                    cmd.Parameters.Add(new SqlParameter("@sp_cybid", dvCybID));
                    #endregion Populate the SQL Params
                    #region Process SQL Command - Try
                    try
                    {
                        if (con.State == ConnectionState.Closed) { con.Open(); }
                        using (SqlDataReader sqlRdr = cmd.ExecuteReader())
                        {
                            if (sqlRdr.HasRows)
                            {
                                while (sqlRdr.Read())
                                {
                                    // Populate the fields with the relevant information
                                    // Allow the user to change the charge amount
                                    // If we have an existing cyb auth; we need to cancel it properly
                                    // Currently we only allow this to happen if there is no auth or the status is blank or error
                                    #region Populate Charge Data
                                    bool chargeready = false;
                                    //if (sqlRdr["status"].ToString().Trim() == "" || sqlRdr["status"].ToString().ToLower() == "error" || sqlRdr["status"].ToString().ToLower() == "declined")
                                    if (sqlRdr["status"].ToString().Trim() == "" || sqlRdr["status"].ToString().ToLower() == "error")
                                    {
                                        chargeready = true;
                                    }
                                    if (chargeready)
                                    {
                                        tbAdminChargeAmount.Text = sqlRdr["amount"].ToString();
                                        hfAdminChargeAmountCurrent.Value = sqlRdr["amount"].ToString();
                                        // IF [ir].[callid] == NULL then CALL else IVR else else RECURRING?
                                        if(sqlRdr["ir_callid"].ToString().Length > 0)
                                        {
                                            lblAdminChargeType.Text = "IVR";
                                        }
                                        else
                                        {
                                            lblAdminChargeType.Text = "WEB";
                                            //lblAdminChargeType.Text = "RECURRING"; // How?
                                        }
                                        
                                        lblAdminChargeProcessing.Text += "<br />Ready to charge...";
                                        btnAdminChargeSubmit.Enabled = true;
                                    }
                                    #endregion Populate Charge Data
                                }
                            }
                            else
                            {
                                lblAdminChargeTemp.Text += "<br />sql No records...";
                                lblAdminChargeTemp.Text += "<br />callid: " + dvCallID.ToString();
                                lblAdminChargeTemp.Text += "<br />cbyid: " + dvCybID.ToString();
                            }
                        }
                    }
                    #endregion Process SQL Command - Try
                    #region Process SQL Command - Catch
                    catch (Exception ex)
                    {
                        lblErrorDV.Text = "*** ERROR IN PROCESSING -- SEE BOTTOM OF PAGE ***";
                        lblErrorDV.Text += "<br />*** TRY LOGIN OUT/IN AND RE-ATTEMPT ***";
                        Error_Display(ex, "Charge - SQL - Catch", msgLabel);
                    }
                    #endregion Process SQL Command - Catch
                }
                #endregion SqlCommand cmd
            }
            #endregion SqlConnection
        }
        #endregion Charge - Start - Try
        #region Charge - Start - Catch
        catch (Exception ex)
        {
            lblErrorDV.Text = "*** ERROR IN PROCESSING -- SEE BOTTOM OF PAGE ***";
            lblErrorDV.Text += "<br />*** TRY LOGIN OUT/IN AND RE-ATTEMPT ***";
            Error_Display(ex, "Charge - Start - Catch", msgLabel);
        }
        #endregion Charge - Start - Catch
        #endregion Charge - Start
    }
    protected void Charge_Cancel(object sender, EventArgs e)
    {
        Charge_Clear();
    }
    protected void ChargeTry(object sender, EventArgs e)
    {
        bool doCharge = true;
        String msgCharge = "";
        lblAdminChargeProcessing.Text = "";
        lblAdminChargeResponse.Text = "";
        //pAdminChargeResponse.Visible = true;
        int dvCallID = Convert.ToInt32(dvPaymentDetails.DataKey["callid"].ToString());
        int dvCybID = 0;
        if (dvPaymentDetails.DataKey["cbid"].ToString().Length > 0) dvCybID = Convert.ToInt32(dvPaymentDetails.DataKey["cbid"].ToString());
        #region Charge - Start - Try
        try
        {
            #region Validate Request
            // Validate the charge request
            double amountcharge = 0;
            Double.TryParse(tbAdminChargeAmount.Text, out amountcharge);
            double amountcurrent = 0;
            Double.TryParse(hfAdminChargeAmountCurrent.Value, out amountcurrent);
            if (amountcharge <= 0)
            {
                msgCharge += "<li>charge amount can not be zero</li>";
                doCharge = false;
            }
            else if (amountcharge > amountcurrent)
            {
                msgCharge += "<li>charge amount is invalid</li>";
                doCharge = false;
            }
            if (ddlAdminChargeReason.SelectedIndex < 1)
            {
                msgCharge += "<li>charge reason is required</li>";
                doCharge = false;
            }
            if (tbAdminChargeNote.Text.Trim().Length <= 0)
            {
                msgCharge += "<li>charge note is required</li>";
                doCharge = false;
            }

            #endregion Validate Request
            #region Validation Failed - Cancel Charge
            if (!doCharge)
            {
                lblAdminChargeResponse.Text = String.Format("<br /><div style='color: darkred;font-weight: bold;'>*** FAILED ****<div style='margin-left: 25px;'><ul>{0}</ul></div></div>", msgCharge);
                lblAdminChargeProcessing.Text += "<br />done with errors...";
                pAdminChargeResponse.Visible = true;
            }
            #endregion Validation Failed - Cancel Charge
            #region Validation Passed - Process Charge
            else
            {
                ghCyb.ARC_Cybersource_Charge cybDonor = new ghCyb.ARC_Cybersource_Charge();
                ghCyb.cybProcess cybProcessCharge = new ghCyb.cybProcess();
                
                cybDonor.callid = 0;
                #region SqlConnection
                using (SqlConnection con = new SqlConnection(sqlStrARC))
                {
                    #region Charge - Gathering
                    #region SqlCommand cmd
                    using (SqlCommand cmd = new SqlCommand("", con))
                    {
                        #region Populate the SQL Command
                        cmd.CommandTimeout = 600;
                        #region Build cmdText
                        String cmdText = "";
                        cmdText += @"
                                    SELECT
                                    [c].[CallID]

                                    ,[di].[id] [donationid]

                                    ,[di].[DonationAmount] [amount]
                                    ,[ci].[fname]
                                    ,[ci].[lname]
                                    ,[ci].[address]
                                    ,[ci].[suitenumber]
                                    ,[ci].[zip]
                                    ,[ci].[city]
                                    ,[ci].[state]
                                    ,[ci].[email]

                                    ,[cb].[id] [cbid]
                                    ,[cb].[status]
                                    ,[cb].[decision]

                                    ,[di].[ccnum]
                                    ,[di].[ccexpmonth]
                                    ,[di].[ccexpyear]
                                    ,CASE
	                                    WHEN LEFT([di].[ccnum],1) = '4' THEN 'Visa'
	                                    WHEN LEFT([di].[ccnum],1) = '5' THEN 'MC'
	                                    WHEN LEFT([di].[ccnum],1) = '3' THEN 'AmEx'
	                                    WHEN LEFT([di].[ccnum],1) = '6' THEN 'DC'
	                                    ELSE 'Other'
                                    END [cctype]

                                    FROM [dbo].[call] [c] WITH(NOLOCK)
                                    JOIN [dbo].[callinfo] [ci] WITH(NOLOCK) ON [ci].[callid] = [c].[callid]
                                    JOIN [dbo].[donationccinfo] [di] WITH(NOLOCK) ON [di].[callid] = [c].[callid]
                                    LEFT OUTER JOIN [dbo].[cybersource_log_auth] [cb] WITH(NOLOCK) ON [cb].[externalid] = [di].[id]
                                    WHERE 1=1
                                    AND [c].[callid] = @sp_callid
                                    AND ([cb].[id] = @sp_cybid OR [cb].[id] IS NULL)
                            ";

                        #endregion Build cmdText
                        cmd.CommandText = cmdText;
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Clear();
                        #endregion Populate the SQL Command
                        #region Populate the SQL Params
                        cmd.Parameters.Add(new SqlParameter("@sp_callid", dvCallID));
                        cmd.Parameters.Add(new SqlParameter("@sp_cybid", dvCybID));
                        #endregion Populate the SQL Params
                        #region Process SQL Command - Try
                        try
                        {
                            if (con.State == ConnectionState.Closed) { con.Open(); }
                            using (SqlDataReader sqlRdr = cmd.ExecuteReader())
                            {
                                if (sqlRdr.HasRows)
                                {
                                    while (sqlRdr.Read())
                                    {
                                        // Here we do some soft validation as we gather the needed data
                                        #region Populate Charge Data
                                        bool chargeready = false;
                                        //if (sqlRdr["status"].ToString().Trim() == "" || sqlRdr["status"].ToString().ToLower() == "error" || sqlRdr["status"].ToString().ToLower() == "declined")
                                        if (sqlRdr["status"].ToString().Trim() == "" || sqlRdr["status"].ToString().ToLower() == "error")
                                        {
                                            chargeready = true;
                                        }
                                        if (chargeready)
                                        {
                                            cybDonor.source = lblAdminChargeType.Text;
                                            cybDonor.callid = Convert.ToInt32(sqlRdr["callid"].ToString());
                                            cybDonor.donationid = Convert.ToInt32(sqlRdr["donationid"].ToString());
                                            //sp_orderid = sp_donationccinfoid.ToString().PadLeft(14, '0');
                                            cybDonor.orderid = cybDonor.donationid.ToString().PadLeft(14, '0');
                                            cybDonor.billto_firstname = (sqlRdr["fname"].ToString().Length > 0) ? sqlRdr["fname"].ToString() : "Anonymous";
                                            cybDonor.billto_lastname = (sqlRdr["lname"].ToString().Length > 0) ? sqlRdr["lname"].ToString() : "Anonymous";
                                            cybDonor.billto_streeet1 = (sqlRdr["address"].ToString().Length > 0) ? sqlRdr["address"].ToString() : "Anonymous";
                                            cybDonor.billto_city = (sqlRdr["city"].ToString().Length > 0) ? sqlRdr["city"].ToString() : "Anonymous";
                                            cybDonor.billto_zip = sqlRdr["zip"].ToString();
                                            cybDonor.billto_state = (sqlRdr["state"].ToString().Length > 0) ? sqlRdr["state"].ToString() : "Anonymous";
                                            cybDonor.billto_country = "US";
                                            /// If valid Email
                                            string tmpemail = sqlRdr["email"].ToString().Trim();
                                            if (tmpemail.Length > 5 && tmpemail.Contains("@") && tmpemail.Contains("."))
                                            {
                                                cybDonor.billto_email = tmpemail;
                                            }
                                            else { cybDonor.billto_email = "nobody@cybersource.com"; }

                                            cybDonor.card_number = sqlRdr["ccnum"].ToString();
                                            cybDonor.card_month = sqlRdr["ccexpmonth"].ToString();
                                            cybDonor.card_year = sqlRdr["ccexpyear"].ToString();
                                            /// Sustainer | "RD001" | "ARC Sustainer"
                                            /// IVR | "DN001" | "ARC Call"
                                            /// WEB | "DN001" | "ARC Agent Script Donation"
                                            cybDonor.amount = amountcharge;
                                            cybDonor.product_sku = "DN001";
                                            cybDonor.product_name = "ARC Agent Portal Donation";
                                        }
                                        else
                                        {
                                            lblAdminChargeProcessing.Text += "<br />chargeready == false...";
                                            doCharge = false;
                                            cybProcessCharge.status = "FAILED";
                                            cybProcessCharge.message = "Was not able to properly gather the data from the database.";
                                        }
                                        #endregion Populate Charge Data
                                    }
                                }
                                else
                                {
                                    lblAdminChargeProcessing.Text += "<br />sql No records...";
                                    doCharge = false;
                                }
                            }
                        }
                        #endregion Process SQL Command - Try
                        #region Process SQL Command - Catch
                        catch (Exception ex)
                        {
                            lblErrorDV.Text = "*** ERROR IN PROCESSING -- SEE BOTTOM OF PAGE ***";
                            lblErrorDV.Text += "<br />*** TRY LOGIN OUT/IN AND RE-ATTEMPT ***";
                            Error_Display(ex, "Charge - SQL - Catch", msgLabel);
                            doCharge = false;
                        }
                        #endregion Process SQL Command - Catch
                    }
                    #endregion SqlCommand cmd
                    #endregion Charge - Gathering
                    #region Charge - Processing
                    // Done gathering data, process it
                    if (doCharge)
                    {
                        cybProcessCharge = ghCyb.cybCharge(cybDonor);
                    }
                    #endregion Charge - Processing
                    #region Charge - Log
                    #region SqlCommand cmd
                    using (SqlCommand cmd = new SqlCommand("", con))
                    {
                        #region Populate the SQL Command
                        cmd.CommandTimeout = 600;
                        cmd.CommandText = "[dbo].[sp_cybersource_charge_log]";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Clear();
                        #endregion Populate the SQL Command
                        #region Populate the SQL Params
                        cmd.Parameters.Add(new SqlParameter("@sp_callid", dvCallID));
                        cmd.Parameters.Add(new SqlParameter("@sp_donationid", cybDonor.donationid));
                        cmd.Parameters.Add(new SqlParameter("@sp_authid", cybProcessCharge.cybid));
                        cmd.Parameters.Add(new SqlParameter("@sp_amount", cybDonor.amount));
                        cmd.Parameters.Add(new SqlParameter("@sp_amount_original", hfAdminChargeAmountCurrent.Value));
                        cmd.Parameters.Add(new SqlParameter("@sp_status", cybProcessCharge.status));
                        cmd.Parameters.Add(new SqlParameter("@sp_user", Page.User.Identity.Name));
                        cmd.Parameters.Add(new SqlParameter("@sp_reason", ddlAdminChargeReason.SelectedValue));
                        cmd.Parameters.Add(new SqlParameter("@sp_notes", tbAdminChargeNote.Text));
                        if (dvCybID > 0)
                        {
                            cmd.Parameters.Add(new SqlParameter("@sp_cybid", dvCybID));
                        }
                        #endregion Populate the SQL Params
                        #region Process SQL Command - Try
                        try
                        {
                            if (con.State == ConnectionState.Closed) { con.Open(); }
                            Int32 LogID = Convert.ToInt32(cmd.ExecuteScalar());
                        }
                        #endregion Process SQL Command - Try
                        #region Process SQL Command - Catch
                        catch (Exception ex)
                        {
                            lblErrorDV.Text = "*** ERROR IN PROCESSING -- SEE BOTTOM OF PAGE ***";
                            lblErrorDV.Text += "<br />*** TRY LOGIN OUT/IN AND RE-ATTEMPT ***";
                            Error_Display(ex, "Charge - SQL - Catch", msgLabel);
                            doCharge = false;
                        }
                        #endregion Process SQL Command - Catch
                    }
                    #endregion SqlCommand cmd
                    #endregion Charge - Log
                }
                #endregion SqlConnection
                #region Charge - Cleanup
                if (cybProcessCharge.status == "ERROR")
                {
                    lblAdminChargeProcessing.Text += "<br />Error in processing...";
                    lblAdminChargeProcessing.Text += "<br />" + cybProcessCharge.status;
                    lblAdminChargeProcessing.Text += "<br />" + cybProcessCharge.message;
                    msgLabel.Text = cybProcessCharge.lblmessage;
                }
                else
                {
                    lblAdminChargeResponse.Text += "<br />done...";
                    lblAdminChargeResponse.Text += "<br />" + cybProcessCharge.status;
                    lblAdminChargeResponse.Text += "<br />" + cybProcessCharge.message;
                    lblAdminChargeResponse.Text += "<br />" + cybProcessCharge.cybid.ToString();
                    hdPDcybID.Value = cybProcessCharge.cybid.ToString();
                    hdPDcallID.Value = dvCallID.ToString();
                    GridView_Refresh_Manual(sender, e);
                    //GridView_IndexChanged(gvSearchGrid, e);
                    ddlAdminChargeReason.SelectedIndex = -1;
                    tbAdminChargeNote.Text = "";
                    pAdminCharge.Visible = true;
                    pAdminChargeResponse.Visible = true;
                    // charge log
                    // dvCybID
                }
                #endregion Charge - Cleanup
            }
            #endregion Validation Passed - Process Charge
            lblAdminChargeProcessing.Text += "<br />done...";
            lblAdminChargeProcessing.Text += "<br />need to do the charge log...";
            // When we do the log; we also cancel previous charges...

        }
        #endregion Charge - Start - Try
        #region Charge - Start - Catch
        catch (Exception ex)
        {
            lblErrorDV.Text = "*** ERROR IN PROCESSING -- SEE BOTTOM OF PAGE ***";
            lblErrorDV.Text += "<br />*** TRY LOGIN OUT/IN AND RE-ATTEMPT ***";
            Error_Display(ex, "Charge - Start - Catch", msgLabel);
        }
        #endregion Charge - Start - Catch
    }
    protected void ChargeTry_Cybersource(object sender, EventArgs e)
    {

    }

    #endregion Charge Processing
    #region Modify
    #region Modify - Sustainer
    protected void Modify_Sustainer(object sender, EventArgs e)
    {
        Label lblModify = lblSustainerModify;
        btnSustainerUpdate.Enabled = false;
        lblModify.Text = "";
        pAdminSustainer.Visible = true;
        pAdminSustainerDetails.Visible = true;
        pAdminSustainerSubmit.Visible = true;
        pAdminSustainerResponse.Visible = true;
        // Pull the sustainer information from SQL
        // We won't trust the DetailsView in-case someone else updated it
        // We just get the CallID from the current page; the rest is from SQL
        try
        {
            int dvCallID = Convert.ToInt32(dvSustainerDetails.DataKey["callid"].ToString());
            #region SqlConnection
            using (SqlConnection con = new SqlConnection(sqlStrARC))
            {
                #region SqlCommand cmd
                using (SqlCommand cmd = new SqlCommand("", con))
                {
                    #region Populate the SQL Command
                    cmd.CommandTimeout = 600;
                    #region Build cmdText
                    String cmdText = "";
                    cmdText += @"
                                SELECT
                                TOP 1
                                [dr].[callid]

                                ,[dr].[donationid]
                                ,[dr].[status]
                                ,[dr].[frequency]
                                ,[dr].[chargedate]
                                ,[dr].[startdate]
                                ,[dr].[receiptfrequency]
                                ,[dr].[createdate]
                                ,[dr].[modifieddate]
                                ,[dr].[processed]
                                ,[dr].[processedstatus]

                                ,[di].[designationid]
                                ,[ds].[displayname] [designation]

                                ,[di].[CCNum] [ccnumber]
                                ,[di].[CCExpMonth] [ccexpmonth]
                                ,[di].[CCExpYear] [ccexpyear]
                                ,[di].[DonationAmount] [amount]

                                ,[ci].[Fname] [firstname]
                                ,[ci].[LName] [lastname]
                                ,[ci].[Address] [address1]
                                ,[ci].[Zip] [postalcode]
                                ,[ci].[City] [city]
                                ,[ci].[State] [state]
                                ,'US' [country]

                                ,[ci].[Email] [email]

                                FROM [dbo].[call] [c] WITH(NOLOCK)
                                JOIN [dbo].[donation_recurring] [dr] WITH(NOLOCK) ON [dr].[callid] = [c].[callid]
                                JOIN [dbo].[callinfo] [ci] WITH(NOLOCK) ON [ci].[callid] = [dr].[callid]
                                JOIN [dbo].[donationccinfo] [di] WITH(NOLOCK) ON [di].[id] = [dr].[donationid]
                                JOIN [dbo].[cybersource_log_auth] [cb] WITH(NOLOCK) ON [cb].[externalid] = [di].[id]
                                LEFT OUTER JOIN [dbo].[designation] [ds] WITH(NOLOCK) ON [ds].[designationid] = [di].[designationid]
                                WHERE 1=1
                                AND [c].[callid] = @sp_callid
                            ";
                    #endregion Build cmdText
                    cmd.CommandText = cmdText;
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Clear();
                    #endregion Populate the SQL Command
                    #region Populate the SQL Params
                    cmd.Parameters.Add(new SqlParameter("@sp_callid", dvCallID));
                    #endregion Populate the SQL Params
                    #region Process SQL Command - Try
                    try
                    {
                        if (con.State == ConnectionState.Closed) { con.Open(); }
                        using (SqlDataReader sqlRdr = cmd.ExecuteReader())
                        {
                            if (sqlRdr.HasRows)
                            {
                                while (sqlRdr.Read())
                                {
                                    // Populate the sustainer data

                                    #region Populate Sustainer Data
                                    ddlSustainerStatus.SelectedValue = sqlRdr["status"].ToString();

                                    #region Sustainer Date
                                    DateTime dtChargeDate;
                                    if (DateTime.TryParse(sqlRdr["chargedate"].ToString(), out dtChargeDate))
                                    {
                                        if (dtChargeDate.Day == 1)
                                        {
                                            ddlSustainerDate.SelectedIndex = 0;
                                        }
                                        else
                                        {
                                            ddlSustainerDate.SelectedIndex = 1;
                                        }
                                        
                                    }
                                    else
                                    {
                                        ddlSustainerDate.SelectedIndex = -1;
                                    }
                                    #endregion Sustainer Date
                                    #region Sustainer Designation
                                    if (sqlRdr["designation"].ToString() != "")
                                    {
                                        lblSustainerDesignation.Text = sqlRdr["designation"].ToString();
                                        //// ddlSustainerDesignation.ClearSelection();
                                        ////ddlSustainerDesignation.SelectedIndex = -1;
                                        //foreach (ListItem li in ddlSustainerDesignation.Items)
                                        //{
                                        //    if (li.Text == sqlRdr["designation"].ToString())
                                        //    {
                                        //        li.Selected = true;
                                        //        break;
                                        //    }
                                        //}
                                    }
                                    #endregion Sustainer Designation
                                    lblSustainerFrequency.Text = sqlRdr["frequency"].ToString();
                                    #region Sustainer Receipt
                                    if (sqlRdr["receiptfrequency"].ToString() != "")
                                    {
                                        lblSustainerReceipt.Text = sqlRdr["receiptfrequency"].ToString();
                                        //ddlSustainerReceipt.ClearSelection();
                                        //foreach (ListItem li in ddlSustainerReceipt.Items)
                                        //{
                                        //    if (li.Text == sqlRdr["receiptfrequency"].ToString())
                                        //    {
                                        //        li.Selected = true;
                                        //        break;
                                        //    }
                                        //}
                                    }
                                    #endregion Sustainer Receipt
                                    //lblModify.Text = "Verify:";
                                    //lblModify.Text += "<br />" + sqlRdr["callid"].ToString();
                                    //lblModify.Text += "<br />" + sqlRdr["donationid"].ToString();
                                    //lblModify.Text += "<br />" + sqlRdr["status"].ToString();
                                    #endregion Populate Sustainer Data
                                    btnSustainerUpdate.Enabled = true;
                                }
                            }
                            else
                            {
                                lblModify.Text += "<br />sql No records...";
                            }
                        }
                    }
                    #endregion Process SQL Command - Try
                    #region Process SQL Command - Catch
                    catch (Exception ex)
                    {
                        lblModify.Text = "*** ERROR IN PROCESSING -- SEE BOTTOM OF PAGE ***";
                        lblModify.Text += "<br />*** TRY LOGIN OUT/IN AND RE-ATTEMPT ***";
                        Error_Display(ex, "Refund - SQL - Catch", msgLabel);
                    }
                    #endregion Process SQL Command - Catch
                }
                #endregion SqlCommand cmd
            }
            #endregion SqlConnection
        }
        catch (Exception ex)
        {
            lblModify.Text += "<br />*** ERROR IN PROCESSING -- SEE BOTTOM OF PAGE ***";
            lblModify.Text += "<br />*** TRY LOGIN OUT/IN AND RE-ATTEMPT ***";
            Error_Display(ex, "Refund - Start - Catch", msgLabel);

        }
        //lblModify.Text = "... modify?";
    }
    protected void Modify_Sustainer_Cancel(object sender, EventArgs e)
    {
        // Hide all panels related to refund
        Sustainer_Clear();
    }
    protected void Modify_Sustainer_Submit(object sender, EventArgs e)
    {
        /// Process Sustainer Update
        /// We currently only allow the Status and Recurring Date to be modified
        /// All other fields are read only
        try
        {
            Label lblModify = lblSustainerModify;

            lblModify.Text = "<hr />Processing...";
            Boolean doUpdate = false;
            Boolean didUpdate = false;
            Boolean doUpdateStatus = false;
            Boolean doUpdateDate = false;
            DetailsView dv = dvSustainerDetails;
            Int32 callid = 0;
            Int32 donationid = 0;

            //DropDownList dvState = (DropDownList)dv.FindControl("donorStateUS");
            //DropDownList dvProvince = (DropDownList)dv.FindControl("donorStateCA");
            //DropDownList dvCountry = (DropDownList)dv.FindControl("donorCountry");
            //TextBox dvStateOther = (TextBox)dv.FindControl("donorStateOther");


            HiddenField currentCallID = (HiddenField)dv.FindControl("callid");
            Int32.TryParse(currentCallID.Value, out callid);
            HiddenField currentDonationID = (HiddenField)dv.FindControl("donationid");
            Int32.TryParse(currentDonationID.Value, out donationid);

            Label currentStatusName = (Label)dv.FindControl("status");
            HiddenField currentStatus = (HiddenField)dv.FindControl("current_status");
            HiddenField currentDateField = (HiddenField)dv.FindControl("current_date");
            DateTime currentDate = DateTime.UtcNow;
            DateTime newDate = DateTime.UtcNow;
            if (currentDateField != null)
            {
                if (DateTime.TryParse(currentDateField.Value, out currentDate))
                {
                    newDate = currentDate;
                    if (currentDate.Day.ToString("00") != ddlSustainerDate.SelectedValue)
                    {
                        newDate = DateTime.Parse(currentDate.Year.ToString() + "-" + currentDate.Month.ToString() + "-" + ddlSustainerDate.SelectedValue);
                    }
                }
            }
            // ChargeDate
            // Acceptable change:

            lblModify.Text += String.Format("<br />Status Name: {0} | {1} ", currentStatusName.Text, ddlSustainerStatus.SelectedItem.Text);
            lblModify.Text += String.Format("<br />Status Value: {0} | {1} ", currentStatus.Value, ddlSustainerStatus.SelectedValue);
            if (currentStatusName.Text != ddlSustainerStatus.SelectedItem.Text) { doUpdateStatus = true; doUpdate = true; }


            // lblModify.Text += String.Format("<br />Designation: {0} | {1} ", currentStatusName.Text, ddlSustainerStatus.SelectedItem.Text);
            // lblModify.Text += String.Format("<br />Date: {0} | {1} ", currentStatusName.Text, ddlSustainerStatus.SelectedItem.Text);
            lblModify.Text += String.Format("<br />Receipt Name: {0} | {1} ", currentDate.Day.ToString("00"), ddlSustainerDate.SelectedValue);
            lblModify.Text += String.Format("<br />Receipt Value: {0} | {1} ", currentDateField.Value, newDate);
            if (currentDate != newDate) { doUpdateDate = true; doUpdate = true; }

            // doUpdate = false;
            if (doUpdate)
            {
                lblModify.Text += "<br />Updating...";
                #region SqlConnection
                using (SqlConnection con = new SqlConnection(sqlStrARC))
                {
                    ghFunctions.Donation_Open_Database(con);
                    // if (con.State == ConnectionState.Closed) { con.Open(); }
                    if (doUpdateStatus)
                    {
                        #region SqlCommand cmd
                        using (SqlCommand cmd = new SqlCommand("", con))
                        {
                            #region Populate the SQL Command
                            cmd.CommandTimeout = 600;
                            #region Build cmdText
                            String cmdText = "";
                            if (doUpdate)
                            {
                                cmdText += @"
UPDATE [dbo].[donation_recurring]
	SET [status] = @sp_status_new -- New Status
WHERE [callid] = @sp_callid
AND [donationid] = @sp_donationid
AND [status] = @sp_status_old -- Old Status
                            ";
                            }
                            #endregion Build cmdText
                            cmd.CommandText = cmdText;
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.Clear();
                            #endregion Populate the SQL Command
                            #region Populate the SQL Params
                            cmd.Parameters.Add("@sp_callid", SqlDbType.Int).Value = callid;
                            cmd.Parameters.Add("@sp_donationid", SqlDbType.Int).Value = donationid;
                            cmd.Parameters.Add("@sp_status_new", SqlDbType.Int).Value = ddlSustainerStatus.SelectedValue;
                            cmd.Parameters.Add("@sp_status_old", SqlDbType.Int).Value = currentStatus.Value;

                            // cmd.Parameters.Add("@sp_charge_new", SqlDbType.DateTime).Value = callid;
                            // cmd.Parameters.Add("@sp_charge_old", SqlDbType.DateTime).Value = callid;
                            #endregion Populate the SQL Params
                            #region Process SQL Command - Try
                            try
                            {
                                // Need to update this so we have a log of sorts
                                var rowsUpdated = cmd.ExecuteNonQuery();
                                if (rowsUpdated == 1)
                                {
                                    lblModify.Text += "<br /><br />Updated Sustainer Status";
                                    didUpdate = true;
                                }
                                else
                                {
                                    lblModify.Text += "<br /><br />Update FAILED! Contact IT for Details.";
                                }

                            }
                            #endregion Process SQL Command - Try
                            #region Process SQL Command - Catch
                            catch (Exception ex)
                            {
                                lblErrorDV.Text = "*** ERROR IN PROCESSING -- SEE BOTTOM OF PAGE ***";
                                lblErrorDV.Text += "<br />*** TRY LOGIN OUT/IN AND RE-ATTEMPT ***";
                                Error_Display(ex, "Refund - SQL - Catch", msgLabel);
                            }
                            #endregion Process SQL Command - Catch
                        }
                        #endregion SqlCommand cmd
                    }
                    if (doUpdateDate)
                    {
                        #region SqlCommand cmd
                        using (SqlCommand cmd = new SqlCommand("", con))
                        {
                            #region Populate the SQL Command
                            cmd.CommandTimeout = 600;
                            #region Build cmdText
                            String cmdText = "";
                            if (doUpdate)
                            {
                                cmdText += @"
UPDATE [dbo].[donation_recurring]
	SET [chargedate] = @sp_charge_new -- New Charge Date
WHERE [callid] = @sp_callid
AND [donationid] = @sp_donationid
AND [chargedate] = @sp_charge_old -- Old Charge Date
                            ";

                            }
                            #endregion Build cmdText
                            cmd.CommandText = cmdText;
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.Clear();
                            #endregion Populate the SQL Command
                            #region Populate the SQL Params
                            cmd.Parameters.Add("@sp_callid", SqlDbType.Int).Value = callid;
                            cmd.Parameters.Add("@sp_donationid", SqlDbType.Int).Value = donationid;
                            cmd.Parameters.Add("@sp_charge_new", SqlDbType.DateTime).Value = newDate;
                            cmd.Parameters.Add("@sp_charge_old", SqlDbType.DateTime).Value = currentDate;

                            // cmd.Parameters.Add("@sp_charge_new", SqlDbType.DateTime).Value = callid;
                            // cmd.Parameters.Add("@sp_charge_old", SqlDbType.DateTime).Value = callid;
                            #endregion Populate the SQL Params
                            #region Process SQL Command - Try
                            try
                            {
                                // Need to update this so we have a log of sorts
                                var rowsUpdated = cmd.ExecuteNonQuery();
                                if (rowsUpdated == 1)
                                {
                                    lblModify.Text += "<br /><br />Updated Charge Date.";
                                    didUpdate = true;
                                }
                                else
                                {
                                    lblModify.Text += "<br /><br />Update FAILED! Contact IT for Details.";
                                }

                            }
                            #endregion Process SQL Command - Try
                            #region Process SQL Command - Catch
                            catch (Exception ex)
                            {
                                lblErrorDV.Text = "*** ERROR IN PROCESSING -- SEE BOTTOM OF PAGE ***";
                                lblErrorDV.Text += "<br />*** TRY LOGIN OUT/IN AND RE-ATTEMPT ***";
                                Error_Display(ex, "Sustainer - SQL - Catch", msgLabel);
                            }
                            #endregion Process SQL Command - Catch
                        }
                        #endregion SqlCommand cmd

                    }
                }
                #endregion SqlConnection

            }
            else
            {
                lblModify.Text += "<br />Nothing to Update!";
            }

            if (didUpdate)
            {
                DetailsView_Data(callid, 0, dv);
            }
        }
        catch (Exception ex)
        {
        }
        finally
        {
            
        }
    }
    #endregion Modify - Sustainer
    #region Modify - Donor
    protected void Modify_Donor(object sender, EventArgs e)
    {
        pAdminDonor.Visible = true;
        pAdminDonorDetails.Visible = true;
        pAdminDonorSubmit.Visible = true;
        pAdminDonorResponse.Visible = true;
        lblDonorModify.Text = "... modify?";
    }
    #endregion Modify - Donor
    #endregion Modify
    #endregion Admin Functions
    protected string label_address(String address, String city, String state, String zip, String country)
    {
        String rtrn = "";
        if (address.Length > 0 || city.Length > 0 || state.Length > 0)
        {
            rtrn = address;
            rtrn += ", " + city;
            rtrn += ", " + state;
            rtrn += " " + zip;
            rtrn += " " + country;
        }
        return rtrn.Trim();
    }
    protected System.Drawing.Color label_status_color(String status)
    {
        System.Drawing.Color rtrn = System.Drawing.Color.Blue;
        if (status == "Declined") rtrn = System.Drawing.Color.Red;
        if (status.Contains("Declined") || status.Contains("Cancel") || status.Contains("Reject")) rtrn = System.Drawing.Color.Red;
        return rtrn;
    }
    protected string label_phone(String callid, String phone, String ani)
    {
        String rtrn = phone;
        if (phone.Length == 0)
        {
            rtrn = ani;
            if (ani.Length == 0)
            {
                // rtrn = de_get_phone_from_callid(callid);

                /// Performance hindering
                /// We need to add ANI to [call] TABLE
                /// Populate as much as we can from [callinfo] rest from [dataexchange]
                /// For IVR records pull from [ivr_record]
            }
        }
        return rtrn.Trim();
    }
    protected string de_get_phone_from_callid(String callid)
    {
        String rtrn = callid;
        #region Get the ANI - From DataExchange
        #region SQL Connection
        using (SqlConnection con = new SqlConnection(sqlStrDE))
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
	                                TOP 1
	                                [fc].[call.ani]
                                    FROM [dbo].[interactions] [i] WITH(NOLOCK)
                                    JOIN [dbo].[interactions_arc] [ia] WITH(NOLOCK) ON [ia].[companyid] = [i].[companyid] AND [ia].[interactionid] = [i].[interactionid]
                                    JOIN [dbo].[five9_calls] [fc] WITH(NOLOCK) ON [fc].[interactionid] = [i].[interactionid] AND [fc].[companyid] = [i].[companyid]
                                    WHERE [ia].[arc.callid] = @sp_arc_callid
                            ";
                #endregion Build cmdText
                cmd.CommandText = cmdText;
                cmd.CommandType = CommandType.Text;
                #region SQL Parameters
                cmd.Parameters.Clear();
                cmd.Parameters.Add(new SqlParameter("@sp_arc_callid", callid));
                #endregion SQL Parameters
                #region SQL Processing
                var rtrn_ani = cmd.ExecuteScalar();
                if (rtrn_ani != null) rtrn = rtrn_ani.ToString();
                #endregion SQL Processing

            }
            #endregion SQL Command
        }
        #endregion SQL Connection
        #endregion Get the ANI - From DataExchange

        return rtrn.Trim();
    }
    protected string de_get_agent_from_callid(String callid)
    {
        String rtrn = callid;
        #region Get the ANI - From DataExchange
        #region SQL Connection
        using (SqlConnection con = new SqlConnection(sqlStrDE))
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
TOP 1
[fa].[fullname] [agent_fullname]
FROM [dbo].[interactions] [i] WITH(NOLOCK)
JOIN [dbo].[interactions_arc] [ia] WITH(NOLOCK) ON [ia].[companyid] = [i].[companyid] AND [ia].[interactionid] = [i].[interactionid]
LEFT OUTER JOIN [dbo].[five9_call] [fc] WITH(NOLOCK) ON [fc].[interactionid] = [i].[interactionid] AND [fc].[companyid] = [i].[companyid]
LEFT OUTER JOIN [dbo].[five9_call_disposition] [fcd] WITH(NOLOCK) ON [fcd].[companyid] = [i].[companyid] AND [fcd].[interactionid] = [i].[interactionid]
LEFT OUTER JOIN [dbo].[five9_agent] [fa] WITH(NOLOCK) ON [fa].[agentid] = [fcd].[agentid]
WHERE [ia].[callid] = @sp_arc_callid
                            ";
                #endregion Build cmdText
                cmd.CommandText = cmdText;
                cmd.CommandType = CommandType.Text;
                #region SQL Parameters
                cmd.Parameters.Clear();
                cmd.Parameters.Add(new SqlParameter("@sp_arc_callid", callid));
                #endregion SQL Parameters
                #region SQL Processing
                var rtrn_ani = cmd.ExecuteScalar();
                if (rtrn_ani != null) rtrn = rtrn_ani.ToString();
                #endregion SQL Processing

            }
            #endregion SQL Command
        }
        #endregion SQL Connection
        #endregion Get the ANI - From DataExchange

        return rtrn.Trim();
    }
    protected string label_amount_convert(string amount, string amount_ref)
    {
        double amnt = 0;
        double amnt_ref = 0;
        if (amount_ref.Length > 0)
        {
            Double.TryParse(amount, out amnt);
            Double.TryParse(amount_ref, out amnt_ref);
            amnt = amnt - amnt_ref;
            amount = amnt.ToString();
        }
        return amount;
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
    protected bool refund_visible_button(string status, string decision)
    {
        //Page.User.IsInRole("System Administrator")
        // if ((Page.User.IsInRole("System Administrator") || Page.User.Identity.Name == "nciambotti2@greenwoodhall.com" || Page.User.Identity.Name == "cstevenson2@greenwoodhall.com")
        // if (ghUser.identity_is_admin())
        bool rtrn = false;
        if ((ghUser.identity_is_admin())
            && decision == "ACCEPT" && (status == "Cancelled" || status == "Approved" || status == "Settled" || status == "Refunded")
            )
        {
            rtrn = true;
        }
        return rtrn;
    }
    protected bool charge_visible_button(string status, string decision)
    {
        //Page.User.IsInRole("System Administrator")
        bool rtrn = false;
        if ((Page.User.IsInRole("System Administrator") || Page.User.Identity.Name == "nciambotti2@greenwoodhall.com")
            && decision != "ACCEPT" && (status != "Settled" && status != "Refunded")
            && decision.Length > 0
            )
        {
            rtrn = true;
        }
        return rtrn;
    }
    protected bool dv_visible_button(string status)
    {
        //Page.User.IsInRole("System Administrator")
        bool rtrn = false;
        if (Page.User.IsInRole("System Administrator") || Page.User.IsInRole("System Administrator"))
        {
            rtrn = true;
        }
        return rtrn;
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

        //Error_Save(ex, error);
        //ErrorLog.ErrorLog(ex);
        //pnlError.Visible = true;
    }
    protected string Repeat(char character, int numberOfIterations)
    {
        return "".PadLeft(numberOfIterations, character);
    }
    protected void print_sql(SqlCommand cmd, String type)
    {
        ghFunctions.print_sql(cmd, sqlPrint, type);
        //lblGraphStatsHeaderNote.Text = "Last Refreshed: " + DateTime.Now.ToString("hh:mm:ss tt");
    }
    protected void Popuplate_DropDown_FromXML(DropDownList ddl, String Name)
    {
        DataSet myDs = new DataSet();
        myDs.ReadXml(Server.MapPath(@"StateCountry.xml"));
        if (myDs.Tables.Count > 0)
        {
            ddl.DataSource = myDs.Tables[Name]; //state, province, country
            ddl.DataValueField = "code";
            ddl.DataTextField = "name";
            ddl.DataBind();
        }
        else
        {
            //This is a fatal error.. can not load the State/Province/Country
        }
        myDs.Dispose();
    }
    protected void Dump()
    {
        #region dv find control and stuff
        //string decision = dvPaymentDetails.Rows[2].Cells[0].Text;

        //lblRefundTemp.Text += "<br />CallID " + callid.ToString();
        //int i = 0;
        //lblRefundTemp.Text += "<br />" + dvPaymentDetails.Rows[i].Cells[0].Text + " " + dvPaymentDetails.Rows[i].Cells[1].Text;
        //i = 1;
        //lblRefundTemp.Text += "<br />" + dvPaymentDetails.Rows[i].Cells[0].Text + " " + dvPaymentDetails.Rows[i].Cells[1].Text;
        //if (dvPaymentDetails.Rows[i].Cells[0].Text == "cbid")
        //{
        //    // Error check here
        //    lblRefundTemp.Text += "<br />" + dvPaymentDetails.Rows[i].Cells[0].Text + " " + ((Label)dvPaymentDetails.Rows[i].FindControl("lbl_cbid")).Text;
        //}
        //i = 2;
        //lblRefundTemp.Text += "<br />" + dvPaymentDetails.Rows[i].Cells[0].Text + " " + dvPaymentDetails.Rows[i].Cells[1].Text;
        //i = 3;
        //lblRefundTemp.Text += "<br />" + dvPaymentDetails.Rows[i].Cells[0].Text + " " + dvPaymentDetails.Rows[i].Cells[1].Text;
        #endregion
        #region old refund processing
        #region Old Code
        ////lblCallID.Text = sqlRdr["callid"].ToString();
        //lblExternalID.Text = sqlRdr["ExternalID"].ToString();
        //lblStatus.Text = sqlRdr["Status"].ToString();
        //CreateDate.Text = sqlRdr["CreateDate"].ToString();
        //RequestID.Text = sqlRdr["requestID"].ToString();
        //RequestToken.Text = sqlRdr["requestToken"].ToString();
        //ReferenceNum.Text = sqlRdr["merchantReferenceCode"].ToString();
        //Amount.Text = sqlRdr["ccAuthReply_amount"].ToString();
        //AmountOriginal.Value = sqlRdr["ccAuthReply_amount"].ToString();
        //// Last 4 of Card Number
        //CardNumber.Text = sqlRdr["ccnum"].ToString().Substring(sqlRdr["ccnum"].ToString().Length - 4, 4);
        //FirstName.Text = sqlRdr["fname"].ToString();
        //LastName.Text = sqlRdr["lname"].ToString();
        //DateTime dtChargeDate;
        //DateTime.TryParse(CreateDate.Text, out dtChargeDate);
        //if (dtChargeDate != null)
        //{
        //    dtlLabel.Text = (dtChargeDate - DateTime.UtcNow).TotalDays.ToString();
        //    if ((DateTime.UtcNow - dtChargeDate).TotalDays > foLimit)
        //    {
        //        RefundType.Value = "Stand Alone";
        //        pnl_standalone.Visible = true;
        //        CardNumberFull.Value = sqlRdr["ccnum"].ToString();
        //        CardMonth.Text = sqlRdr["ccexpmonth"].ToString();
        //        CardYear.Text = sqlRdr["ccexpyear"].ToString();
        //        /*
        //         * 001 == Visa == 2
        //         * 002 == MasterCard == 3
        //         * 003 == American Express == 4
        //         * 004 == Discover == 5
        //         */
        //        if (CardNumberFull.Value.Length > 1)
        //        {
        //            switch (CardNumberFull.Value.Substring(0, 1))
        //            {
        //                case "4":
        //                    CardType.Text = "Visa";
        //                    CardTypeFull.Value = "001";
        //                    break;
        //                case "5":
        //                    CardType.Text = "MasterCard";
        //                    CardTypeFull.Value = "002";
        //                    break;
        //                case "3":
        //                    CardType.Text = "American Express";
        //                    CardTypeFull.Value = "003";
        //                    break;
        //                case "6":
        //                    CardType.Text = "Discover";
        //                    CardTypeFull.Value = "004";
        //                    break;
        //            }
        //        }
        //        Address1.Text = sqlRdr["address"].ToString();
        //        Address2.Text = sqlRdr["suitenumber"].ToString();
        //        //Address3.Text = sqlRdr[""].ToString();
        //        City.Text = sqlRdr["city"].ToString();
        //        ddlState.Text = sqlRdr["state"].ToString();
        //        Zip.Text = sqlRdr["zip"].ToString();
        //        ddlCountry.Text = "USA";
        //    }
        //    else
        //    {
        //        RefundType.Value = "Follow On";
        //    }
        //}
        //if (sqlRdr["Status"].ToString() == "Settled" || sqlRdr["Status"].ToString() == "Approved")
        //{
        //    btnRefundSubmit.Enabled = true;
        //}
        //else
        //{
        //    dtlLabel.Text = "The record is not in a valid refundable status.";
        //    dtlLabel.ForeColor = System.Drawing.Color.Red;
        //}
        #endregion Old Code

        #endregion
    }

}
