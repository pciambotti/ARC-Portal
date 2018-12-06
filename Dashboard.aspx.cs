using ChartDirector;
using ClosedXML.Excel;
using ClosedXML.Excel.Drawings;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
public partial class Dashboard : System.Web.UI.Page
{
    //private String sqlStr = Connection.GetConnectionString("MiddleWare", "");
    private String sqlStr = Connection.GetConnectionString("Default", ""); // Controlled by Web Config ARC_Live | ARC_Stage
    private String sqlStrInt = Connection.GetConnectionString("PS_Production", ""); // PS_Production | PS_Stage
    private Int32 countCalls = 0;
    private String strClient = "ARC"; //ddlCallClients.SelectedValue.PadLeft(3, '0')
    protected void Page_PreInit(Object sender, EventArgs e)
    {
        Master.PageTitle = "Dashboard";
        this.Title = String.Format("{0}: {1}", Master.MyTitle, Page.Title);

        if (Connection.GetDBMode() == "Stage")
        {
            sqlStrInt = Connection.GetConnectionString("PS_Stage", ""); // PS_Production | PS_Stage
        }
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        lblResults.Text = "<br />";
        if (!IsPostBack)
        {
            #region Chart Image
            if (Session["imgNameSel"] == null || !IsPostBack)
            {
                // Get the Image Name for Charts
                //this.Page.User.Identity.Name
                //if (Page.User.IsInRole("System Administrator") == true && Page.User.Identity.Name == "nciambotti@greenwoodhall.com")
                //userid
                if (Session["userid"] == null) { identity_get_userid(); }
                string uName = Page.User.Identity.Name;
                if (uName.IndexOf("@") > 0)
                {
                    uName = uName.Substring(0, uName.IndexOf("@"));
                }

                string aName = "";
                if (Page.User.IsInRole("System Administrator") == true) aName = "SA";
                else if (Page.User.IsInRole("Administrator") == true) aName = "AM";
                else if (Page.User.IsInRole("Manager") == true) aName = "MA";
                else if (Page.User.IsInRole("Advisor") == true) aName = "AD";
                else if (Page.User.IsInRole("Agent") == true) aName = "AG";
                else if (Page.User.IsInRole("Client") == true) aName = "CL";
                else aName = "AO";
                string uID = Session["userid"].ToString().PadLeft(2, '0');

                String imgName = "";
                imgName = String.Format("{0}_{1}_{2}_[client]_dashboard_selective_[type].png", uName, uID, aName);
                Session["imgNameSel"] = imgName;
            }
            #endregion Chart Image
            dtStartDate.Text = DateTime.Now.AddDays(-3).ToString("MM/dd/yyyy");
            dtStartTime.Text = "00:00";
            dtEndDate.Text = DateTime.Now.ToString("MM/dd/yyyy");
            dtEndTime.Text = "23:59";

            #region Event Dates
            if (DateTime.UtcNow.AddHours(-ghFunctions.dtUserOffSet) < DateTime.Parse("2017-09-24 18:59"))
            {
                dtStartDate.Text = "09/23/2017";
                dtStartTime.Text = "19:00";
                dtEndDate.Text = DateTime.UtcNow.AddHours(-ghFunctions.dtUserOffSet).AddDays(+0).ToString("MM/dd/yyyy");
                dtEndTime.Text = "23:59";
            }
            else if (DateTime.UtcNow.AddHours(-ghFunctions.dtUserOffSet) < DateTime.Parse("2017-09-25 02:00"))
            {
                dtStartDate.Text = "09/24/2017";
                dtStartTime.Text = "19:00";
                dtEndDate.Text = DateTime.UtcNow.AddHours(-ghFunctions.dtUserOffSet).AddDays(+0).ToString("MM/dd/yyyy");
                dtEndTime.Text = "23:59";
            }
            #endregion Event Dates

            Dashboard_Refresh();
        }
        // If we lose session, it can cause errors.
        if (Session["userid"] == null)
        {
            Response.Redirect("~/Dashboard.aspx");
        }
    }
    protected void Custom_Export_Excel_Dashboard(object sender, EventArgs e)
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
        String nameFile = "Dashboard-Reporting-Full-Export";
        String nameSheet = "Call-Statistics";
        XLWorkbook wb = new XLWorkbook();
        var ws = wb.Worksheets.Add(nameSheet);
        // Starting Column and Row for Dashboard
        int sRow = 1; int sCol = 1; // A1
        #region Insert - Logo
        ws.Range(sRow, sCol, sRow + 3, sCol + 3).Merge();
        using (WebClient wc = new WebClient())
        {
            // Logo
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
        #region Insert - Chart - Calls per Hour
        ws.Cell(sRow, sCol).Value = "Calls per Hour";
        ws.Cell(sRow, sCol).Style.Font.Bold = true;
        ws.Cell(sRow, sCol).Style.Font.FontSize = 12;
        ws.Range(sRow, sCol, sRow, sCol + 8).Merge();
        ws.Range(sRow, sCol, sRow, sCol + 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        using (WebClient wc = new WebClient())
        {
            // Calls per Hour Chart
            string iNameCH = Session["imgNameSel"].ToString().Replace("[type]", "ch").Replace("[client]", strClient);

            byte[] bytes = wc.DownloadData(Server.MapPath("/offline/charts/" + iNameCH));
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                System.Drawing.Image img = System.Drawing.Image.FromStream(ms);

                var fIn = img.ToStream(System.Drawing.Imaging.ImageFormat.Png);

                XLPicture pic = new XLPicture
                {
                    NoChangeAspect = true,
                    NoMove = true,
                    NoResize = true,
                    ImageStream = fIn,
                    Name = "Calls per Hour"
                };
                XLMarker fMark = new XLMarker { ColumnId = sCol, RowId = sRow + 2 };
                pic.AddMarker(fMark);
                fMark = new XLMarker { ColumnId = sCol + 9, RowId = sRow + 15 };
                pic.AddMarker(fMark);
                ws.AddPicture(pic);

                img.Dispose();
                fIn.Dispose();
            }
        }
        #endregion Insert - Chart - Calls per Hour
        #region Insert - Chart - Call Types
        ws.Cell(sRow, sCol + 10).Value = "Call Types";
        ws.Cell(sRow, sCol + 10).Style.Font.Bold = true;
        ws.Cell(sRow, sCol + 10).Style.Font.FontSize = 12;
        ws.Range(sRow, sCol + 10, sRow, sCol + 10 + 3).Merge();
        ws.Range(sRow, sCol + 10, sRow, sCol + 10 + 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        using (WebClient wc = new WebClient())
        {
            // Call Type Chart
            string iNameCT = Session["imgNameSel"].ToString().Replace("[type]", "ct").Replace("[client]", strClient);

            byte[] bytes = wc.DownloadData(Server.MapPath("/offline/charts/" + iNameCT));
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                System.Drawing.Image img = System.Drawing.Image.FromStream(ms);

                var fIn = img.ToStream(System.Drawing.Imaging.ImageFormat.Png);

                XLPicture pic = new XLPicture
                {
                    NoChangeAspect = true,
                    NoMove = true,
                    NoResize = true,
                    ImageStream = fIn,
                    Name = "Call Types"
                };
                XLMarker fMark = new XLMarker { ColumnId = sCol + 10, RowId = sRow + 2 };
                pic.AddMarker(fMark);
                fMark = new XLMarker { ColumnId = sCol + 10 + 4, RowId = sRow + 15 };
                pic.AddMarker(fMark);
                ws.AddPicture(pic);

                img.Dispose();
                fIn.Dispose();
            }
        }
        #endregion Insert - Chart - CallsCall Types
        sRow = sRow + 16;
        #region Insert - Chart - Designation Counts
        ws.Cell(sRow, sCol).Value = "Designation Counts";
        ws.Cell(sRow, sCol).Style.Font.Bold = true;
        ws.Cell(sRow, sCol).Style.Font.FontSize = 12;
        ws.Range(sRow, sCol, sRow, sCol + 8).Merge();
        ws.Range(sRow, sCol, sRow, sCol + 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        using (WebClient wc = new WebClient())
        {
            // Designation Counts
            string iNameAH = Session["imgNameSel"].ToString().Replace("[type]", "dc").Replace("[client]", strClient);

            byte[] bytes = wc.DownloadData(Server.MapPath("/offline/charts/" + iNameAH));
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                System.Drawing.Image img = System.Drawing.Image.FromStream(ms);

                var fIn = img.ToStream(System.Drawing.Imaging.ImageFormat.Png);

                XLPicture pic = new XLPicture
                {
                    NoChangeAspect = true,
                    NoMove = true,
                    NoResize = true,
                    ImageStream = fIn,
                    Name = "Designation Count"
                };
                XLMarker fMark = new XLMarker { ColumnId = sCol, RowId = sRow + 2 };
                pic.AddMarker(fMark);
                fMark = new XLMarker { ColumnId = sCol + 9, RowId = sRow + 15 };
                pic.AddMarker(fMark);
                ws.AddPicture(pic);

                img.Dispose();
                fIn.Dispose();
            }
        }
        #endregion Insert - Chart - Designation Counts
        #region Insert - Chart - DNIS Counts
        ws.Cell(sRow, sCol + 10).Value = "DNIS Counts";
        ws.Cell(sRow, sCol + 10).Style.Font.Bold = true;
        ws.Cell(sRow, sCol + 10).Style.Font.FontSize = 12;
        ws.Range(sRow, sCol + 10, sRow, sCol + 10 + 3).Merge();
        ws.Range(sRow, sCol + 10, sRow, sCol + 10 + 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        using (WebClient wc = new WebClient())
        {
            // DNIS Counts Chart
            string iNameCT = Session["imgNameSel"].ToString().Replace("[type]", "ds").Replace("[client]", strClient);

            byte[] bytes = wc.DownloadData(Server.MapPath("/offline/charts/" + iNameCT));
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                System.Drawing.Image img = System.Drawing.Image.FromStream(ms);

                var fIn = img.ToStream(System.Drawing.Imaging.ImageFormat.Png);

                XLPicture pic = new XLPicture
                {
                    NoChangeAspect = true,
                    NoMove = true,
                    NoResize = true,
                    ImageStream = fIn,
                    Name = "DNIS Counts"
                };
                XLMarker fMark = new XLMarker { ColumnId = sCol + 10, RowId = sRow + 2 };
                pic.AddMarker(fMark);
                fMark = new XLMarker { ColumnId = sCol + 10 + 4, RowId = sRow + 15 };
                pic.AddMarker(fMark);
                ws.AddPicture(pic);

                img.Dispose();
                fIn.Dispose();
            }
        }
        #endregion Insert - Chart - DNIS Counts
        sRow = sRow + 16;
        int dRow = sRow + 2; int dCol = sCol; int dColT = dCol;
        #region Wrap Up
        if (dRow < sRow + 23) dRow = sRow + 23;
        ws.Range(1, 1, dRow, sCol + 2).Style.Alignment.WrapText = true;
        sRow = dRow;

        //ws.Cell(sRow + 1, sCol + 3).Value = "Done";

        ws.Rows().AdjustToContents();
        ws.Columns().AdjustToContents();
        ws.Columns(1, 4).Width = 10;
        ws.Columns(5, 9).Width = 11;
        ws.Columns(11, 14).Width = 18;
        ws.ShowGridLines = false;
        ws.Cell(5, 3).Active = true;
        #endregion Wrap Up
        nameSheet = "Call-Details";
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
    protected void Custom_Export_Excel_CallDetails(object sender, EventArgs e)
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
        String nameFile = "Dashboard-Reporting-Details-Export";
        String nameSheet = "Call-Details";
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
        sRow = sRow + 1;
        #region Grid - Call Details
        cl = ws.Cell(sRow, sCol);
        cl.Value = "Call Details";
        cl.Style.Font.Bold = true;
        cl.Style.Font.FontSize = 12;
        ws.Range(sRow, sCol, sRow, sCol + 5).Merge();
        ws.Range(sRow, sCol, sRow, sCol + 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        int dRow = sRow + 1; int dCol = sCol; int dColT = dCol;
        GridView gv = gvCallDetailsExport;
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
        ws.Columns(1, 3).Width = 11;
        ws.Column(4).Width = 7;
        //8.43 * 4 == 33.72
        ws.ShowGridLines = false;
        ws.SheetView.FreezeRows(8);
        ws.Cell(9, 1).Active = true;
        #endregion Wrap Up
    }
    protected void GridView_Export_Excel(object sender, EventArgs e)
    {
        Button btn = (Button)sender;
        GridView gv = null;
        String lbl = "";
        if (btn.ID == "btnCallDetails")
        {
            gv = this.gvCallDetailsExport;
            lbl = "Call-Details";
        }

        if (gv != null)
        {
            rpMessage.Text = "Exporting [" + lbl + "] Grid";
            GridViewExportUtil.ClosedXMLExport("" + lbl + "", gv);
        }
        else
        {
            rpMessage.Text = "Un-configured export click.";
        }
    }
    protected void GridView_Export_Excel_Old(object sender, EventArgs e)
    {
        Button btn = (Button)sender;
        GridView gv = null;
        String lbl = "";
        if (btn.ID == "btnCallDispositions")
        {
            gv = this.gvCallDispositions;
            lbl = "Call-Disposition-Details";
        }
        else if (btn.ID == "btnCallDetails")
        {
            gv = this.gvCallDetailsExport;
            lbl = "Call-Details";
        }


        if (gv != null)
        {
            rpMessage.Text = "Exporting [" + lbl + "] Grid";
            GridViewExportUtil.Export("Dashboard-Reporting-" + lbl + ".xls", gv);
        }
        else
        {
            rpMessage.Text = "Un-configured export click.";
        }
    }
    protected void Panel_Export_Excel(object sender, EventArgs e)
    {
        Button btn = (Button)sender;
        Panel pl = null;
        String lbl = "";
        if (btn.ID == "btnCCPerformance")
        {
            pl = this.pnlCCPerformanceExport;
            lbl = "Call-Center-Performance";
        }
        else if (btn.ID == "btnInterval")
        {
            pl = this.pnlIntervalExport;
            lbl = "Call-Interval";
        }

        if (pl != null)
        {
            rpMessage.Text = "Exporting [" + lbl + "] Grid";
            GridViewExportUtil.ExportPanel("Dashboard-Reporting-" + lbl + ".xls", pl);
        }
        else
        {
            rpMessage.Text = "Un-configured export click.";
        }
    }
    protected void GridView_Refresh(object sender, EventArgs e)
    {
        Dashboard_Refresh();
    }
    protected void Dashboard_Refresh()
    {
        try
        {
            Data_Call_Type();
            Data_Call_by_Hour();
            Data_Designation_Count();
            Data_DNIS_Counts();
            Data_Call_Details();

            btnExportFull.Visible = true;
        }
        catch (Exception ex)
        {
            Error_Catch(ex, "Dashboard - Refresh", msgLabel);
        }
    }
    protected void Data_Call_Type()
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
                cmdText = "";
                cmdText += @"
SELECT
CASE
	WHEN [di].[callid] IS NOT NULL AND [d].[displayname] = 'Sustainer' AND [ir].[callid] IS NULL THEN 'Sustainer Web'
	WHEN [di].[callid] IS NOT NULL AND [d].[displayname] = 'Sustainer' AND [ir].[callid] IS NOT NULL THEN 'Sustainer IVR'
	WHEN [d].[displayname] = 'Sustainer' THEN [d].[displayname]
	WHEN [di].[callid] IS NOT NULL AND [ir].[callid] IS NULL THEN 'Donation Web'
	WHEN [di].[callid] IS NOT NULL AND [ir].[callid] IS NOT NULL THEN 'Donation IVR'
	WHEN [di].[callid] IS NOT NULL THEN 'Donation'
	WHEN [d].[displayname] LIKE '%pledge%' THEN [d].[displayname]
	WHEN [ir].[callid] IS NOT NULL THEN 'IVR Question'
	ELSE 'Web Call [Question]'
END [source]
,COUNT([c].[callid]) [count]
FROM [dbo].[call] [c] WITH(NOLOCK)
LEFT OUTER JOIN [dbo].[disposition] [d] WITH(NOLOCK) ON [d].[dispositionid] = [c].[dispositionid]
LEFT OUTER JOIN [dbo].[donationccinfo] [di] WITH(NOLOCK) ON [di].[callid] = [c].[callid]
LEFT OUTER JOIN [dbo].[ivr_record] [ir] WITH(NOLOCK) ON [ir].[callid] = [c].[callid]
WHERE 1=1
AND [c].[logindatetime] BETWEEN @sp_date_start AND @sp_date_end
GROUP BY
CASE
	WHEN [di].[callid] IS NOT NULL AND [d].[displayname] = 'Sustainer' AND [ir].[callid] IS NULL THEN 'Sustainer Web'
	WHEN [di].[callid] IS NOT NULL AND [d].[displayname] = 'Sustainer' AND [ir].[callid] IS NOT NULL THEN 'Sustainer IVR'
	WHEN [d].[displayname] = 'Sustainer' THEN [d].[displayname]
	WHEN [di].[callid] IS NOT NULL AND [ir].[callid] IS NULL THEN 'Donation Web'
	WHEN [di].[callid] IS NOT NULL AND [ir].[callid] IS NOT NULL THEN 'Donation IVR'
	WHEN [di].[callid] IS NOT NULL THEN 'Donation'
	WHEN [d].[displayname] LIKE '%pledge%' THEN [d].[displayname]
	WHEN [ir].[callid] IS NOT NULL THEN 'IVR Question'
	ELSE 'Web Call [Question]'
END
,CASE
	WHEN [di].[callid] IS NOT NULL AND [d].[displayname] = 'Sustainer' AND [ir].[callid] IS NULL THEN 4 --'Sustainer Web'
	WHEN [di].[callid] IS NOT NULL AND [d].[displayname] = 'Sustainer' AND [ir].[callid] IS NOT NULL THEN 5 --'Sustainer IVR'
	WHEN [d].[displayname] = 'Sustainer' THEN 6 -- [d].[displayname]
	WHEN [di].[callid] IS NOT NULL AND [ir].[callid] IS NULL THEN 1 --'Donation Web'
	WHEN [di].[callid] IS NOT NULL AND [ir].[callid] IS NOT NULL THEN 2 --'Donation IVR'
	WHEN [di].[callid] IS NOT NULL THEN 3 -- 'Donation'
	WHEN [d].[displayname] = 'Pledge' THEN 7.0 -- [d].[displayname]
	WHEN [d].[displayname] = 'Pledge [Sustainer]' THEN 7.1 -- [d].[displayname]
	WHEN [d].[displayname] LIKE '%pledge%' THEN 7.2 -- [d].[displayname]
	WHEN [ir].[callid] IS NOT NULL THEN 8 -- 'IVR Call'
	ELSE 9 -- 'Web Call'
END 
ORDER BY
CASE
	WHEN [di].[callid] IS NOT NULL AND [d].[displayname] = 'Sustainer' AND [ir].[callid] IS NULL THEN 4 --'Sustainer Web'
	WHEN [di].[callid] IS NOT NULL AND [d].[displayname] = 'Sustainer' AND [ir].[callid] IS NOT NULL THEN 5 --'Sustainer IVR'
	WHEN [d].[displayname] = 'Sustainer' THEN 6 -- [d].[displayname]
	WHEN [di].[callid] IS NOT NULL AND [ir].[callid] IS NULL THEN 1 --'Donation Web'
	WHEN [di].[callid] IS NOT NULL AND [ir].[callid] IS NOT NULL THEN 2 --'Donation IVR'
	WHEN [di].[callid] IS NOT NULL THEN 3 -- 'Donation'
	WHEN [d].[displayname] = 'Pledge' THEN 7.0 -- [d].[displayname]
	WHEN [d].[displayname] = 'Pledge [Sustainer]' THEN 7.1 -- [d].[displayname]
	WHEN [d].[displayname] LIKE '%pledge%' THEN 7.2 -- [d].[displayname]
	WHEN [ir].[callid] IS NOT NULL THEN 8 -- 'IVR Call'
	ELSE 9 -- 'Web Call'
END
                            ";
                #endregion Build cmdText
                cmd.CommandText = cmdText;
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Clear();
                #region SQL Parameters
                DateTime dtFrom = DateTime.Parse(dtStartDate.Text + " " + dtStartTime.Text);
                DateTime dtTo = DateTime.Parse(dtEndDate.Text + " " + dtEndTime.Text);
                cmd.Parameters.Add(new SqlParameter("@sp_date_start", dtConverted(dtFrom)));
                cmd.Parameters.Add(new SqlParameter("@sp_date_end", dtConverted(dtTo)));
                #endregion SQL Parameters
                //print_sql(cmd); // Will print for Admin in Local
                DateTime dtStart = DateTime.UtcNow;
                #region SQL Processing - Reader
                //SqlDataAdapter ad = new SqlDataAdapter(cmd);
                //DataTable dt = new DataTable();
                //ad.Fill(dt);
                //int dCount = DataTable_Row_Sum_Values(dt, 1);
                using (SqlDataReader sqlRdr = cmd.ExecuteReader())
                {
                    if (sqlRdr.HasRows)
                    {
                        DBTable chartTable = new DBTable(sqlRdr);
                        int dCount = Chart_Data_Count(chartTable);
                        cntCallTypes.Value = dCount.ToString();
                        Chart_Call_Type(chartTable);
                    }
                }
                #endregion SQL Processing - Reader
                // DateTime dtStart = DateTime.UtcNow;
                ghFunctions.print_loadtime(lblResults, "txt: data_call_type", dtStart, DateTime.UtcNow);
            }
            #endregion SQL Command
        }
        #endregion SQL Connection
    }
    protected Int32 Chart_Data_Count(DBTable table)
    {
        int chartCount = 0;
        double[] data = table.getCol(1);
        foreach (double cnt in data)
        {
            chartCount += Convert.ToInt32(cnt);
        }
        return chartCount;
    }
    protected void Chart_Call_Type(DBTable table)
    {
        // The data for the pie chart
        //double[] data = { 89, 18, 15, 12, 8, 54, 35 };
        double[] data = table.getCol(1);

        // The labels for the pie chart
        //string[] labels = { "Labor", "Licenses", "Taxes", "Legal", "Insurance", "Facilities", "Production" };
        string[] labels = table.getColAsString(0);

        // Create a PieChart object of size 450 x 270 pixels
        PieChart c = new PieChart(504, 260);
        // Transparent Background
        c.setBackground(Chart.Transparent);
        //c.setBackground(0xcccccc); 

        // Set the center of the pie at (150, 100) and the radius to 80 pixels
        c.setPieSize(155, 130, 120);

        // add a legend box where the top left corner is at (330, 50)
        c.addLegend(280, 10).setText("{label}: {value} ({percent|0}%)");
        c.getLegend().addKey2(10000 * 99, "Total Calls: " + cntCallTypes.Value, 0xcccccc);


        // modify the sector label format to show percentages only
        //c.setLabelFormat("{percent}%");
        // Disable the sector labels by setting the color to Transparent
        c.setLabelStyle("", 8, Chart.Transparent);

        // Set the pie data and the pie labels
        c.setData(data, labels);

        // Use rounded edge shading, with a 1 pixel white (FFFFFF) border
        c.setSectorStyle(Chart.RoundedEdgeShading, 0xffffff, 1);

        // Output the chart
        chartCallType.Image = c.makeWebImage(Chart.PNG);
        // Include tool tip for the chart
        chartCallType.ImageMap = c.getHTMLImageMap("", "", "title='{label}: {value} Calls ({percent|0}%)'");

        string iName = Session["imgNameSel"].ToString().Replace("[type]", "ct").Replace("[client]", strClient);
        c.makeChart(Server.MapPath("/offline/charts/" + iName));
    }
    protected void Data_Call_by_Hour()
    {
        #region SQL Connection
        using (SqlConnection con = new SqlConnection(sqlStr))
        {
            #region SQL Command
            using (SqlCommand cmd = new SqlCommand("", con))
            {
                if (con.State == ConnectionState.Closed) { con.Open(); }
                cmd.CommandTimeout = 600;
                cmd.CommandText = "[dbo].[sp_dashboard_get_calls_by_hour]";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                #region SQL Parameters
                DateTime dtFrom = DateTime.Parse(dtStartDate.Text + " " + dtStartTime.Text);
                DateTime dtTo = DateTime.Parse(dtEndDate.Text + " " + dtEndTime.Text);
                cmd.Parameters.Add(new SqlParameter("@sp_date_start", dtConverted(dtFrom)));
                cmd.Parameters.Add(new SqlParameter("@sp_date_end", dtConverted(dtTo)));
                #endregion SQL Parameters
                //print_sql(cmd); // Will print for Admin in Local
                DateTime dtStart = DateTime.UtcNow;
                #region SQL Processing - Reader
                using (SqlDataReader sqlRdr = cmd.ExecuteReader())
                {
                    if (sqlRdr.HasRows)
                    {
                        DBTable chartTable = new DBTable(sqlRdr);
                        Chart_Call_by_Hour(chartTable);
                    }
                }
                #endregion SQL Processing - Reader
                ghFunctions.print_loadtime(lblResults, "sp: sp_dashboard_get_calls_by_hour", dtStart, DateTime.UtcNow);

            }
            #endregion SQL Command
        }
        #endregion SQL Connection
    }
    protected void Chart_Call_by_Hour(DBTable table)
    {

        // The data for the line chart
        //double[] data = { 0, 0, 5, 5, 10, 10, 15, 15, 20, 20, 750, 1250, 2500, 25000, 25000, 2500, 1250, 750, 15, 10, 10, 5, 5, 0, 0 };
        double[] data = table.getCol(2);

        // The labels for the line chart
        //string[] labels = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23", "24" };
        string[] labels = table.getColAsString(1);

        // Create a XYChart object of size 250 x 250 pixels
        XYChart c = new XYChart(700, 255);

        // Transparent Background
        c.setBackground(Chart.Transparent);
        //c.setBackground(0xcccccc);

        // Set the plotarea at (30, 20) and of size 200 x 200 pixels
        c.setPlotArea(60, 20, 620, 200, Chart.Transparent, -1, -1, 0xcccccc, 0xcccccc);

        // Set the plotarea at (55, 58) and of size 520 x 195 pixels, with white background. Turn on
        // both horizontal and vertical grid lines with light grey color (0xcccccc)
        //c.setPlotArea(55, 58, 520, 195, 0xffffff, -1, -1, 0xcccccc, 0xcccccc);

        // Add a line chart layer using the given data
        c.addLineLayer(data);
        //c.addLineLayer(data, 0xff0000, "Calls");
        //c.addLineLayer(data, 0xff0000);
        //LineLayer dt = c.addLineLayer2(Chart.DotLine);
        //dt.addDataSet(data, 0xff0000, "Calls");


        // Set the labels on the x axis.
        c.xAxis().setLabels(labels);

        // Display 1 out of 3 labels on the x-axis.
        c.xAxis().setLabelStep(1);


        // Output the chart
        chartCallsHour.Image = c.makeWebImage(Chart.PNG);
        // Include tool tip for the chart
        chartCallsHour.ImageMap = c.getHTMLImageMap("", "", "title='Hour {xLabel}: Calls {value}'");

        string iName = Session["imgNameSel"].ToString().Replace("[type]", "ch").Replace("[client]", strClient);
        c.makeChart(Server.MapPath("/offline/charts/" + iName));

    }
    protected void Data_Designation_Count()
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
                cmdText = "";
                cmdText += @"
                            SELECT
                            [di].[designationid]
                            ,[ds].[displayname]
                            ,COUNT([c].[callid]) [count]
                            ,SUM([di].[donationamount]) [amount]
                            FROM [dbo].[call] [c] WITH(NOLOCK)
                            LEFT OUTER JOIN [dbo].[donationccinfo] [di] WITH(NOLOCK) ON [di].[callid] = [c].[callid]
                            LEFT OUTER JOIN [dbo].[designation] [ds] WITH(NOLOCK) ON [ds].[designationid] = [di].[designationid]
                            WHERE 1=1
                            AND [c].[logindatetime] BETWEEN @sp_date_start AND @sp_date_end
                            AND [di].[callid] IS NOT NULL
                            GROUP BY [di].[designationid], [ds].[displayname]
                            ";
                #endregion Build cmdText
                cmd.CommandText = cmdText;
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Clear();
                #region SQL Parameters
                DateTime dtFrom = DateTime.Parse(dtStartDate.Text + " " + dtStartTime.Text);
                DateTime dtTo = DateTime.Parse(dtEndDate.Text + " " + dtEndTime.Text);
                cmd.Parameters.Add(new SqlParameter("@sp_date_start", dtConverted(dtFrom)));
                cmd.Parameters.Add(new SqlParameter("@sp_date_end", dtConverted(dtTo)));
                #endregion SQL Parameters
                //print_sql(cmd); // Will print for Admin in Local
                DateTime dtStart = DateTime.UtcNow;
                #region SQL Processing - Reader
                using (SqlDataReader sqlRdr = cmd.ExecuteReader())
                {
                    if (sqlRdr.HasRows)
                    {
                        DBTable chartTable = new DBTable(sqlRdr);
                        Chart_Designation_Count(chartTable);
                    }
                }
                #endregion SQL Processing - Reader
                ghFunctions.print_loadtime(lblResults, "txt: data_designation_count", dtStart, DateTime.UtcNow);

            }
            #endregion SQL Command
        }
        #endregion SQL Connection
    }
    protected void Chart_Designation_Count(DBTable table)
    {
        // The labels (Designation) Element
        string[] labels = table.getColAsString(1);
        // The Count Data Element
        double[] data01 = table.getCol(2);
        // The Amount Data Element
        double[] data02 = table.getCol(3);

        #region Bar Labels
        // Create a XYChart object of size 700 x 255 pixels
        XYChart c = new XYChart(700, 255);
        // Transparent Background
        c.setBackground(Chart.Transparent);

        // Set default text color to dark grey (0x333333)
        c.setColor(Chart.TextColor, 0x333333);

        // Set the plotarea at (70, 20) and of size 500 x 300 pixels, with transparent background and
        // border and light grey (0xcccccc) horizontal grid lines
        //c.setPlotArea(60, 20, 620, 200, Chart.Transparent, -1, -1, 0xcccccc, 0xcccccc);
        //c.setPlotArea(60, 20, 620, 200, Chart.Transparent, -1, Chart.Transparent, 0xcccccc);
        int pArea = 590;
        c.setPlotArea(50, 30, pArea, 180, Chart.Transparent, -1, Chart.Transparent, 0xcccccc);

        // Set the x and y axis stems to transparent and the label font to 12pt Arial
        c.xAxis().setColors(Chart.Transparent);
        c.xAxis().setLabelStyle("Arial", 10).setMaxWidth(pArea / labels.Length);
        c.yAxis().setColors(Chart.Transparent);
        c.yAxis().setLabelStyle("Arial", 10);
        c.yAxis2().setColors(Chart.Transparent);
        c.yAxis2().setLabelStyle("Arial", 10);

        #region Basic Single Bar
        // Add a blue (0x6699bb) bar chart layer using the given data
        //BarLayer layer = c.addBarLayer(data01, 0x6699bb);
        #endregion
        #region Stacked Bar
        // Add a stacked bar layer
        //BarLayer layer = c.addBarLayer2(Chart.Stack);
        // Add the three data sets to the bar layer
        //layer.addDataSet(data01, 0x6699bb, "Count");
        //layer.addDataSet(data02, 0xbbdd88, "Amount");
        #endregion
        #region Multi Bar
        // Add a multi-bar layer with 3 data sets and 3 pixels 3D depth
        BarLayer layer = c.addBarLayer2(Chart.Side);
        layer.addDataSet(data01, 0xbbdd88, "Count");
        layer.addDataSet(data02, 0x6699bb, "Amount").setUseYAxis2();
        // Use bar gradient lighting with the light intensity from 0.8 to 1.3
        layer.setBorderColor(Chart.Transparent, Chart.barLighting(0.8, 1.3));
        // Set rounded corners for bars
        layer.setRoundedCorners();
        // Display labela on top of bars using 12pt Arial font
        layer.setAggregateLabelStyle("Arial", 10);
        layer.setAggregateLabelFormat("{value|0,}");
        //layer.getDataSet(1).setDataLabelFormat("{value}");
        //layer.getDataSet(1).setDataLabelStyle("Arial", 10).setAlignment(Chart.Top);
        #endregion
        #region Dual Y-Axis
        //BarLayer layerCount = c.addBarLayer(data01, 0xbbdd88);
        //BarLayer layerAmount = c.addBarLayer(data02, 0x6699bb);
        //// Use bar gradient lighting with the light intensity from 0.8 to 1.3
        //layerCount.setBorderColor(Chart.Transparent, Chart.barLighting(0.8, 1.3));
        //// Set rounded corners for bars
        //layerCount.setRoundedCorners();
        //// Display labela on top of bars using 12pt Arial font
        //layerCount.setAggregateLabelStyle("Arial", 12);
        //// set to the secondary (right) y axis
        //layerAmount.setUseYAxis2();
        //// Use bar gradient lighting with the light intensity from 0.8 to 1.3
        //layerAmount.setBorderColor(Chart.Transparent, Chart.barLighting(0.8, 1.3));
        //// Set rounded corners for bars
        //layerAmount.setRoundedCorners();
        //// Display labela on top of bars using 12pt Arial font
        //layerAmount.setAggregateLabelStyle("Arial", 12);
        #endregion


        // Set the labels on the x axis.
        c.xAxis().setLabels(labels);

        // For the automatic y-axis labels, set the minimum spacing to 40 pixels.
        c.yAxis().setTickDensity(40);
        c.yAxis2().setTickDensity(40);

        // Add a title to the y axis using dark grey (0x555555) 14pt Arial Bold font
        c.yAxis().setTitle("Count", "Arial Bold", 12, 0x555555);
        c.yAxis().setTitlePos(Chart.Top);
        c.yAxis2().setTitle("Amount", "Arial Bold", 12, 0x555555);
        c.yAxis2().setTitlePos(Chart.Top);
        // Dirty way to get the max value from data01 / data02 to customize the label
        double data01Max = 0;
        foreach (double val in data01) { if (val > data01Max) data01Max = val; if (data01Max > 1000) break; }
        if (data01Max > 1000) c.yAxis().setLabelFormat("{={value}/1000|0}K");

        double data02Max = 0;
        foreach (double val in data02) { if (val > data02Max) data02Max = val; if (data02Max > 1000) break; }
        if (data02Max > 1000) c.yAxis2().setLabelFormat("{={value}/1000|0}K");

        //.setText("{={value}/1000|0}K");

        #endregion Bar Labels

        // Output the chart
        chartDesignationCount.Image = c.makeWebImage(Chart.PNG);
        // Include tool tip for the chart
        chartDesignationCount.ImageMap = c.getHTMLImageMap("", "", "title='Designation {xLabel}: {value}'");

        string iName = Session["imgNameSel"].ToString().Replace("[type]", "dc").Replace("[client]", strClient);
        c.makeChart(Server.MapPath("/offline/charts/" + iName));


    }
    protected void Data_DNIS_Counts()
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
                cmdText = "";
                cmdText += @"
SELECT
ISNULL([ds].[company],'Other') [dnis]
,COUNT([c].[callid]) [count]
,MIN([c].[logindatetime]) [first]
,MAX([c].[logindatetime]) [last]
,YEAR(MAX([c].[logindatetime])) [lyear]
FROM [dbo].[call] [c] WITH(NOLOCK)
LEFT OUTER JOIN [dbo].[dnis] [ds] WITH(NOLOCK) ON [ds].[dnis] = [c].[dnis]
WHERE 1=1
AND [c].[logindatetime] BETWEEN @sp_date_start AND @sp_date_end
GROUP BY [ds].[company]
ORDER BY [ds].[company]
                            ";
                #endregion Build cmdText
                cmd.CommandText = cmdText;
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Clear();
                #region SQL Parameters
                DateTime dtFrom = DateTime.Parse(dtStartDate.Text + " " + dtStartTime.Text);
                DateTime dtTo = DateTime.Parse(dtEndDate.Text + " " + dtEndTime.Text);
                cmd.Parameters.Add("@sp_date_start", SqlDbType.DateTime).Value = dtConverted(dtFrom);
                cmd.Parameters.Add("@sp_date_end", SqlDbType.DateTime).Value = dtConverted(dtTo);
                // cmd.Parameters.Add(new SqlParameter("@sp_date_start", dtConverted(dtFrom)));
                // cmd.Parameters.Add(new SqlParameter("@sp_date_end", dtConverted(dtTo)));
                #endregion SQL Parameters
                //print_sql(cmd); // Will print for Admin in Local
                DateTime dtStart = DateTime.UtcNow;
                #region SQL Processing - Reader
                using (SqlDataReader sqlRdr = cmd.ExecuteReader())
                {
                    if (sqlRdr.HasRows)
                    {
                        DBTable chartTable = new DBTable(sqlRdr);
                        int dCount = Chart_Data_Count(chartTable);
                        cntDNISCounts.Value = dCount.ToString();
                        Chart_DNIS_Counts(chartTable);
                    }
                }
                #endregion SQL Processing - Reader
                ghFunctions.print_loadtime(lblResults, "txt: data_dnis_counts", dtStart, DateTime.UtcNow);
            }
            #endregion SQL Command
        }
        #endregion SQL Connection
    }
    protected void Chart_DNIS_Counts(DBTable table)
    {
        // The data for the pie chart
        //double[] data = { 89, 18, 15, 12, 8, 54, 35 };
        double[] data = table.getCol(1);

        // The labels for the pie chart
        //string[] labels = { "Labor", "Licenses", "Taxes", "Legal", "Insurance", "Facilities", "Production" };
        string[] labels = table.getColAsString(0);

        // Create a PieChart object of size 450 x 270 pixels
        PieChart c = new PieChart(504, 260);
        // Transparent Background
        c.setBackground(Chart.Transparent);
        //c.setBackground(0xcccccc); 

        // Set the center of the pie at (150, 100) and the radius to 80 pixels
        c.setPieSize(155, 130, 120);

        // add a legend box where the top left corner is at (330, 50)
        c.addLegend(280, 10).setText("{label}: {value} ({percent|0}%)");
        c.getLegend().addKey2(10000 * 99, "Total Calls: " + cntDNISCounts.Value, 0xcccccc);
        // c.getLegend().setReverse();

        // modify the sector label format to show percentages only
        //c.setLabelFormat("{percent}%");
        // Disable the sector labels by setting the color to Transparent
        c.setLabelStyle("", 8, Chart.Transparent);

        // Set the pie data and the pie labels
        c.setData(data, labels);

        // Use rounded edge shading, with a 1 pixel white (FFFFFF) border
        c.setSectorStyle(Chart.RoundedEdgeShading, 0xffffff, 1);

        // Output the chart
        chartDNISCounts.Image = c.makeWebImage(Chart.PNG);
        // Include tool tip for the chart
        chartDNISCounts.ImageMap = c.getHTMLImageMap("", "", "title='{label}: {value} Calls ({percent|0}%)'");

        string iName = Session["imgNameSel"].ToString().Replace("[type]", "ds").Replace("[client]", strClient);
        c.makeChart(Server.MapPath("/offline/charts/" + iName));
    }
    protected void Data_Call_Details()
    {
        GridView gv = gvCallDetails;
        GridView gvEx = gvCallDetailsExport;
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
                cmdText = "";
                cmdText += @"
SELECT
TOP (@sp_top)
[c].[callid]
,[c].[LoginDateTime] [call_createdate]
,CASE
	WHEN [di].[callid] IS NOT NULL AND [d].[displayname] = 'Sustainer' AND [ir].[callid] IS NULL THEN 'Sustainer Web'
	WHEN [di].[callid] IS NOT NULL AND [d].[displayname] = 'Sustainer' AND [ir].[callid] IS NOT NULL THEN 'Sustainer IVR'
	WHEN [d].[displayname] = 'Sustainer' THEN [d].[displayname]
	WHEN [di].[callid] IS NOT NULL AND [ir].[callid] IS NULL THEN 'Donation Web'
	WHEN [di].[callid] IS NOT NULL AND [ir].[callid] IS NOT NULL THEN 'Donation IVR'
	WHEN [di].[callid] IS NOT NULL THEN 'Donation'
	WHEN [d].[displayname] LIKE '%pledge%' THEN [d].[displayname]
	WHEN [ir].[callid] IS NOT NULL THEN 'IVR Question'
	ELSE 'Web Call [Question]'
END [type]
,[c].[dispositionid]
,[d].[displayname] [disposition]
--,(SELECT CASE WHEN LEN([d].[line]) = 0 THEN [d].[Company] ELSE [d].[Company] + ' [' + [d].[Line] + ']' END [Description] FROM [dbo].[dnis] [d] WITH(NOLOCK) WHERE [d].[dnis] = [c].[dnis]) [dnis_description]
,[c].[dnis]
,CASE
	WHEN [ds].[dnis] IS NOT NULL THEN [ds].[company] + ' [' + [ds].[line] + '] [' + [ds].[phonenumber] + ']'
	ELSE ''
END [dnis_description]
,[ds].[company] [dnis_company]
,[ds].[line] [dnis_line]
,[ds].[phonenumber] [dnis_phonenumber]
--,[c].[ani] -- ANI!?!?!?!?!?
,[di].[id] [donationid]
,[cb].[status] [donation_status]
,[di].[DonationAmount] [donation_amount]
,dbo.fn_titlecase([ci].[Fname] + ' ' + [ci].[Lname]) [name]
,dbo.fn_titlecase([ci].[address]) [address]
,dbo.fn_titlecase([ci].[city]) [city]
,[ci].[state]
,[ci].[zip]
,[ci].[country]
,CASE
	WHEN LEN([ci].[companyname]) > 0 THEN dbo.fn_titlecase([ci].[companyname])
	ELSE NULL
END [companyname]

,CASE
	WHEN [ci].[phone_optin] IS NOT NULL THEN [ci].[phone_optin]
	WHEN LEN([ci].[hphone]) > 9 THEN 0
	ELSE NULL
END [phone_optin]
,CASE
	WHEN LEN([ci].[hphone]) > 9 THEN [ci].[hphone]
	ELSE NULL
END [phone]
,CASE
	WHEN [ci].[phone_type] IS NOT NULL THEN [ci].[phone_type]
	WHEN LEN([ci].[hphone]) > 9 THEN 'H'
	ELSE NULL
END [phone_type]


,CASE
	WHEN LEN([ci].[phone2]) > 9 AND [ci].[phone2_optin] IS NOT NULL THEN [ci].[phone2_optin]
	WHEN LEN([ci].[phone2]) > 9 THEN 0
	ELSE NULL
END [phone2_optin]
,CASE
	WHEN LEN([ci].[phone2]) > 9 THEN [ci].[phone2]
	ELSE NULL
END [phone2]
,CASE
	WHEN LEN([ci].[phone2]) > 9 AND [ci].[phone2_type] IS NOT NULL THEN [ci].[phone2_type]
	WHEN LEN([ci].[phone2]) > 9 THEN 'H'
	ELSE NULL
END [phone2_type]

,CASE
	WHEN [ci].[receipt_email] = 1 OR ([ci].[email] LIKE '%@%.%' AND [ci].[receipt_email] IS NULL) THEN 1
	ELSE 0
END [emailreceipt_optin]
,CASE
	WHEN [ci].[email] LIKE '%@%.%' THEN LOWER([ci].[email])
	ELSE NULL
END [email]

,CASE
	WHEN [ci].[receiveupdatesyn] = 1 OR ([ci].[email2] LIKE '%@%.%' AND [ci].[receiveupdatesyn] IS NULL) THEN 1
	ELSE 0
END [email_optin]
,CASE
	WHEN [ci].[email2] LIKE '%@%.%' THEN LOWER([ci].[email2])
	ELSE NULL
END [email2]

,CASE [r].[status]
	WHEN 301001 THEN 'New Record: Settled'
	WHEN 301002 THEN 'Processed: Settled'
	WHEN 301003 THEN 'Processed: Rejected'
	WHEN 301004 THEN 'Processed: Error'
	WHEN 301005 THEN 'Cancelled by System'
	WHEN 301006 THEN 'Cancelled by Donor'
	WHEN 301007 THEN 'Cancelled by Admin'
	ELSE CONVERT(varchar(25),[r].[status])
END [sustainer_status]
,[r].[frequency] [sustainer_frequency]
,[r].[receiptfrequency] [sustainer_receipt_frequency]
,[r].[startdate] [sustainer_startdate]
,[r].[processed] [sustainer_processed]
FROM [dbo].[call] [c] WITH(NOLOCK)
LEFT OUTER JOIN [dbo].[disposition] [d] WITH(NOLOCK) ON [d].[dispositionid] = [c].[dispositionid]
LEFT OUTER JOIN [dbo].[callinfo] [ci] WITH(NOLOCK) ON [ci].[callid] = [c].[callid]
LEFT OUTER JOIN [dbo].[donationccinfo] [di] WITH(NOLOCK) ON [di].[callid] = [c].[callid]
LEFT OUTER JOIN [dbo].[cybersource_log_auth] [cb] WITH(NOLOCK) ON [cb].[externalid] = [di].[id]
LEFT OUTER JOIN [dbo].[ivr_record] [ir] WITH(NOLOCK) ON [ir].[callid] = [c].[callid]
LEFT OUTER JOIN [dbo].[dnis] [ds] WITH(NOLOCK) ON [ds].[dnis] = [c].[dnis]
LEFT OUTER JOIN [dbo].[donation_recurring] [r] WITH(NOLOCK) ON [r].[callid] = [c].[callid]
WHERE 1=1
AND [c].[logindatetime] BETWEEN @sp_date_start AND @sp_date_end
ORDER BY [c].[callid] DESC

                            ";
                #endregion Build cmdText
                cmd.CommandText = cmdText;
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Clear();
                #region SQL Parameters
                int sp_top = 500; // Switch from 50k to 500 to prevent chrome crash
                cmd.Parameters.Add(new SqlParameter("@sp_top", sp_top));
                DateTime dtFrom = DateTime.Parse(dtStartDate.Text + " " + dtStartTime.Text);
                DateTime dtTo = DateTime.Parse(dtEndDate.Text + " " + dtEndTime.Text);
                cmd.Parameters.Add(new SqlParameter("@sp_date_start", dtConverted(dtFrom)));
                cmd.Parameters.Add(new SqlParameter("@sp_date_end", dtConverted(dtTo)));
                #endregion SQL Parameters

                #region SQL Processing - GridView
                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                ad.Fill(dt);
                int dCount = dt.Rows.Count;
                gv.DataSource = dt;
                gv.DataBind();
                if (dCount > 0)
                {
                    btnCallDetails.Visible = true;
                    if (gv.Rows.Count != gvEx.Rows.Count)
                    {
                        gvEx.DataSource = dt;
                        gvEx.DataBind();
                    }
                }
                #endregion SQL Processing - GridView

            }
            #endregion SQL Command
        }
        #endregion SQL Connection
    }

    protected void GridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GridView gv = (GridView)sender;
        if (gv != null)
        {
            gv.SelectedIndex = -1;
            gv.PageIndex = e.NewPageIndex;
            //GridView_Data(0, this.Page.User.Identity.Name, gvResults);
            //donation_admin.Visible = false;
            Data_Call_Details();
        }
    }
    protected void GridView_PageDropDownList_SelectedIndexChanged(Object sender, EventArgs e)
    {
        // Work with the Page Template Drop Down List
        DropDownList ddl = (DropDownList)sender;
        GridView gv = (GridView)ddl.GetParentOfType(typeof(GridView)); //gvCallDetails;
        //lblCallDetails.Text = "DDL: " + ddl.ID + "| GV: " + gv.ID; // DeBug to show the ID of the DropDownList and GridView
        if (gv != null)
        {
            // Retrieve the pager row.
            GridViewRow pagerRow = gv.TopPagerRow;
            // Retrieve the PageDropDownList DropDownList from the bottom pager row.
            DropDownList pageList = (DropDownList)pagerRow.Cells[0].FindControl("PageDropDownList");
            // Set the PageIndex property to display that page selected by the user.
            gv.SelectedIndex = -1;
            gv.PageIndex = pageList.SelectedIndex;
            //GridView_Data(0, this.Page.User.Identity.Name, gvResults);
            Data_Call_Details();
        }
    }
    protected void Data_Call_Counts()
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
                #region Build cmdText
                cmdText = "";
                cmdText += @"SELECT
                        COUNT([f].[CallId]) [total_calls]
                        ,COUNT(
	                        CASE
		                        WHEN [f].[disposition] <> 'Abandon' AND [f].[QueueTime] > 1000 AND [f].[QueueTime] < 90000 THEN 1
		                        WHEN [f].[disposition] <> 'Abandon' AND [f].[QueueTime] < 1000 AND [f].[QueueTime] < 90 THEN 1
		                        ELSE NULL
	                        END
                        ) [answered_90]
                        ,COUNT(
	                        CASE
		                        WHEN [f].[disposition] <> 'Abandon' AND [f].[QueueTime] > 1000 AND [f].[QueueTime] > 90000 AND [f].[QueueTime] <= 120000 THEN 1
		                        WHEN [f].[disposition] <> 'Abandon' AND [f].[QueueTime] < 1000 AND [f].[QueueTime] > 90 AND [f].[QueueTime] <= 120 THEN 1
		                        ELSE NULL
	                        END
                        ) [answered_120]
                        ,COUNT(
	                        CASE
		                        WHEN [f].[disposition] <> 'Abandon' AND [f].[QueueTime] > 1000 AND [f].[QueueTime] > 120000 THEN 1
		                        WHEN [f].[disposition] <> 'Abandon' AND [f].[QueueTime] < 1000 AND [f].[QueueTime] > 120 THEN 1
		                        ELSE NULL
	                        END
                        ) [answered_120p]
                        ,COUNT(
	                        CASE
		                        WHEN [f].[disposition] = 'Abandon' AND [f].[QueueTime] > 1000 AND [f].[QueueTime] < 90000 THEN 1
		                        WHEN [f].[disposition] = 'Abandon' AND [f].[QueueTime] < 1000 AND [f].[QueueTime] < 90 THEN 1
		                        ELSE NULL
	                        END
                        ) [abandon_90]
                        ,COUNT(
	                        CASE
		                        WHEN [f].[disposition] = 'Abandon' AND [f].[QueueTime] > 1000 AND [f].[QueueTime] > 90000 AND [f].[QueueTime] <= 120000 THEN 1
		                        WHEN [f].[disposition] = 'Abandon' AND [f].[QueueTime] < 1000 AND [f].[QueueTime] > 90 AND [f].[QueueTime] <= 120 THEN 1
		                        ELSE NULL
	                        END
                        ) [abandon_120]
                        ,COUNT(
	                        CASE
		                        WHEN [f].[disposition] = 'Abandon' AND [f].[QueueTime] > 1000 AND [f].[QueueTime] > 120000 THEN 1
		                        WHEN [f].[disposition] = 'Abandon' AND [f].[QueueTime] < 1000 AND [f].[QueueTime] > 120 THEN 1
		                        ELSE NULL
	                        END
                        ) [abandon_120p]
                        ,[dbo].[ConvertMilliSecondsToHHMMss] (AVG(CAST(((CASE WHEN [f].[disposition] <> 'Abandon' AND [f].[QueueTime] > 1000 THEN [f].[QueueTime] WHEN [f].[disposition] <> 'Abandon' AND [f].[QueueTime] < 1000 THEN [f].[QueueTime] * 1000 END)) AS BIGINT))) [avgQueueTime] 
                        ,[dbo].[ConvertMilliSecondsToHHMMss] (AVG(CAST(((CASE  WHEN [f].[disposition] <> 'Abandon' AND [f].[TalkTime] > 1000 THEN [f].[TalkTime] WHEN [f].[disposition] <> 'Abandon' AND [f].[TalkTime] < 1000 THEN [f].[TalkTime] * 1000 END)) AS BIGINT))) [avgTalkTime] 
                        ,[dbo].[ConvertMilliSecondsToHHMMss] (AVG(CAST(((CASE WHEN [f].[disposition] = 'Abandon' AND [f].[QueueTime] > 1000 THEN [f].[QueueTime] WHEN [f].[disposition] = 'Abandon' AND [f].[QueueTime] < 1000 THEN [f].[QueueTime] * 1000 END)) AS BIGINT))) [avgAbandonTime] 
                        FROM [MiddleWare].[dbo].[Five9Qlikview] [f] WITH(NOLOCK)
                        ";
                cmdText += "WHERE [f].[ClientId]=1\r";
                cmdText += "AND [f].[calldate] BETWEEN @sp_date_start AND @sp_date_end\r";
                cmdText += "AND [f].[campaign] IN (SELECT [CampaignName] FROM @sp_campaigns)";
                #endregion Build cmdText
                cmd.CommandText = cmdText;
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Clear();
                #region SQL Parameters
                DateTime dtFrom = DateTime.Parse(dtStartDate.Text + " " + dtStartTime.Text);
                DateTime dtTo = DateTime.Parse(dtEndDate.Text + " " + dtEndTime.Text);
                lblMessage.Text = "Date Range: " + dtFrom.ToString("yyyy-MM-dd HH:mm:ss") + " to " + dtTo.ToString("yyyy-MM-dd HH:mm:ss");
                cmd.Parameters.Add(new SqlParameter("@sp_date_start", dtConverted(dtFrom)));
                cmd.Parameters.Add(new SqlParameter("@sp_date_end", dtConverted(dtTo)));
                DataTable clientCampaigns = dt_Client_Campaigns();
                SqlParameter tvpParam = cmd.Parameters.AddWithValue("@sp_campaigns", clientCampaigns);
                tvpParam.SqlDbType = SqlDbType.Structured;
                tvpParam.TypeName = "dbo.CampaignTableType";
                #endregion SQL Parameters
                //print_sql(cmd); // Will print for Admin in Local
                double totalCalls = 0;
                double answered_90 = 0;
                double answered_120 = 0;
                double answered_120p = 0;
                double totalAnswered = 0;
                double abandon_90 = 0;
                double abandon_120 = 0;
                double abandon_120p = 0;
                double totalAbandoned = 0;
                DateTime dtStart = DateTime.UtcNow;
                #region SQL Processing - Reader
                using (SqlDataReader sqlRdr = cmd.ExecuteReader())
                {
                    if (sqlRdr.HasRows)
                    {
                        while (sqlRdr.Read())
                        {
                            #region process sqlRdr
                            totalCalls += Convert.ToDouble(sqlRdr["total_calls"].ToString());
                            countCalls = Convert.ToInt32(totalCalls);

                            answered_90 += Convert.ToDouble(sqlRdr["answered_90"].ToString());
                            answered_120 += Convert.ToDouble(sqlRdr["answered_120"].ToString());
                            answered_120p += Convert.ToDouble(sqlRdr["answered_120p"].ToString());
                            totalAnswered = answered_90 + answered_120 + answered_120p;

                            abandon_90 += Convert.ToDouble(sqlRdr["abandon_90"].ToString());
                            abandon_120 += Convert.ToDouble(sqlRdr["abandon_120"].ToString());
                            abandon_120p += Convert.ToDouble(sqlRdr["abandon_120p"].ToString());
                            totalAbandoned = abandon_90 + abandon_120 + abandon_120p;

                            cntTotalCalls.Text = totalCalls.ToString();
                            cntTotalAnswered.Text = totalAnswered.ToString();
                            cntTotalAbandoned.Text = totalAbandoned.ToString();

                            perTotalCalls.Text = String.Format("{0:P0}", GetPercent(totalCalls, totalCalls));
                            perTotalAnswered.Text = String.Format("{0:P0}", GetPercent(totalAnswered, totalCalls));
                            perTotalAbandoned.Text = String.Format("{0:P0}", GetPercent(totalAbandoned, totalCalls));
                            //return 

                            cntAnswerPerf.Text = answered_90.ToString();
                            perAnswerPerf.Text = String.Format("{0:P0}", GetPercent(answered_90, totalAnswered));
                            cntSupportLevel.Value = (GetPercent(answered_90, totalAnswered) * 100).ToString();

                            cntAnswered90.Text = answered_90.ToString();
                            perAnswered90.Text = String.Format("{0:P0}", GetPercent(answered_90, totalAnswered));
                            cntAnswered120.Text = answered_120.ToString();
                            perAnswered120.Text = String.Format("{0:P0}", GetPercent(answered_120, totalAnswered));
                            cntAnswered120p.Text = answered_120p.ToString();
                            perAnswered120p.Text = String.Format("{0:P0}", GetPercent(answered_120p, totalAnswered));

                            cntAbandoned90.Text = abandon_90.ToString();
                            perAbandoned90.Text = String.Format("{0:P0}", GetPercent(abandon_90, totalAbandoned));
                            cntAbandoned120.Text = abandon_120.ToString();
                            perAbandoned120.Text = String.Format("{0:P0}", GetPercent(abandon_120, totalAbandoned));
                            cntAbandoned120p.Text = abandon_120p.ToString();
                            perAbandoned120p.Text = String.Format("{0:P0}", GetPercent(abandon_120p, totalAbandoned));


                            avgSpeedAnswer.Text = String.IsNullOrEmpty(sqlRdr["avgQueueTime"].ToString()) ? "00:00:00" : sqlRdr["avgQueueTime"].ToString();
                            avgTalkTime.Text = String.IsNullOrEmpty(sqlRdr["avgTalkTime"].ToString()) ? "00:00:00" : sqlRdr["avgTalkTime"].ToString();
                            avgAbandonedTime.Text = String.IsNullOrEmpty(sqlRdr["avgAbandonTime"].ToString()) ? "00:00:00" : sqlRdr["avgAbandonTime"].ToString();
                            #endregion process sqlRdr
                        }
                    }
                    else
                    {
                        cntSupportLevel.Value = "-1";
                    }
                }
                #endregion SQL Processing - Reader
                ghFunctions.print_loadtime(lblResults, "txt: data_call_counts", dtStart, DateTime.UtcNow);

            }
            #endregion SQL Command
        }
        #endregion SQL Connection
    }
    protected void Data_Call_Dispositions()
    {
        GridView gv = gvCallDispositions;
        #region SQL Connection
        using (SqlConnection con = new SqlConnection(sqlStr))
        {
            #region SQL Command
            using (SqlCommand cmd = new SqlCommand("", con))
            {
                if (con.State == ConnectionState.Closed) { con.Open(); }
                cmd.CommandTimeout = 600;
                String cmdText = "";
                #region Build cmdText
                cmdText = "";
                cmdText += "SELECT\r";
                cmdText += "[f].[Disposition]\r";
                cmdText += ",COUNT([f].[CallId]) [count]\r";
                cmdText += "FROM [MiddleWare].[dbo].[Five9Qlikview] [f] WITH(NOLOCK)\r";
                cmdText += "WHERE [f].[ClientId] = 1\r";
                cmdText += "AND [f].[calldate] BETWEEN @sp_date_start AND @sp_date_end\r";
                cmdText += "AND [f].[campaign] IN (SELECT [CampaignName] FROM @sp_campaigns)";
                cmdText += "GROUP BY [f].[Disposition]\r";
                cmdText += "ORDER BY [f].[Disposition]\r";
                #endregion Build cmdText

                cmd.CommandText = cmdText;
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Clear();

                #region SQL Parameters
                DateTime dtFrom = DateTime.Parse(dtStartDate.Text + " " + dtStartTime.Text);
                DateTime dtTo = DateTime.Parse(dtEndDate.Text + " " + dtEndTime.Text);
                cmd.Parameters.Add(new SqlParameter("@sp_date_start", dtConverted(dtFrom)));
                cmd.Parameters.Add(new SqlParameter("@sp_date_end", dtConverted(dtTo)));
                DataTable clientCampaigns = dt_Client_Campaigns();
                SqlParameter tvpParam = cmd.Parameters.AddWithValue("@sp_campaigns", clientCampaigns);
                tvpParam.SqlDbType = SqlDbType.Structured;
                tvpParam.TypeName = "dbo.CampaignTableType";
                #endregion SQL Parameters
                #region SQL Processing - GridView
                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                ad.Fill(dt);
                int dCount = DataTable_Row_Sum_Values(dt, 1);
                hfDispoCount.Value = dCount.ToString();
                totalDispos.Text = "Total Disposition Count: " + dCount.ToString();
                gv.DataSource = dt;
                gv.DataBind();
                //if (dCount > 0) btnCallDispositions.Visible = true;
                #endregion SQL Processing - GridView
            }
            #endregion SQL Command
        }
        #endregion SQL Connection

    }
    protected Int32 DataTable_Row_Sum_Values(DataTable dt, Int32 col)
    {
        int dtSum = 0;
        foreach (DataRow r in dt.Rows)
        {
            dtSum += Convert.ToInt32(r[col].ToString());
        }
        return dtSum;
    }
    protected void GridView_DataBound(Object sender, EventArgs e)
    {
        GridView gv = (GridView)sender;
        if (gv.PageCount > 0)
        {
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
    }

    protected DataTable dt_Client_Campaigns()
    {
        // We create a table variable
        // To pass the campaigns selected
        // Need to see if this breaks with large selection

        DataTable clientCampaigns = new DataTable();// = CategoriesDataTable.GetChanges(DataRowState.Added);
        clientCampaigns.Columns.Add(new DataColumn("CampaignName", typeof(String)));
        DataRow dr;
        foreach (ListItem li in ddlCallCampaigns.Items)
        {
            if (li.Selected)
            {
                dr = clientCampaigns.NewRow(); dr["CampaignName"] = li.Text; clientCampaigns.Rows.Add(dr);
            }
        }
        return clientCampaigns;
    }
    protected void Data_Call_Type_Old()
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
                #region Build cmdText
                cmdText = "";
                cmdText += @"
                            SELECT
                            [f].[calltype]
                            ,COUNT([f].[CallId]) [count]
                            FROM [MiddleWare].[dbo].[Five9Qlikview] [f] WITH(NOLOCK)
                            WHERE [f].[ClientId]=1
                            AND [f].[calldate] BETWEEN @sp_date_start AND @sp_date_end
                            AND [f].[campaign] IN (SELECT [CampaignName] FROM @sp_campaigns)
                            GROUP BY [f].[calltype]
                            ";
                #endregion Build cmdText
                cmd.CommandText = cmdText;
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Clear();
                #region SQL Parameters
                cmd.Parameters.Add(new SqlParameter("@sp_clientid", "0"));
                DateTime dtFrom = DateTime.Parse(dtStartDate.Text + " " + dtStartTime.Text);
                DateTime dtTo = DateTime.Parse(dtEndDate.Text + " " + dtEndTime.Text);
                cmd.Parameters.Add(new SqlParameter("@sp_date_start", dtConverted(dtFrom)));
                cmd.Parameters.Add(new SqlParameter("@sp_date_end", dtConverted(dtTo)));
                DataTable clientCampaigns = dt_Client_Campaigns();
                SqlParameter tvpParam = cmd.Parameters.AddWithValue("@sp_campaigns", clientCampaigns);
                tvpParam.SqlDbType = SqlDbType.Structured;
                tvpParam.TypeName = "dbo.CampaignTableType";
                #endregion SQL Parameters
                //print_sql(cmd); // Will print for Admin in Local
                DateTime dtStart = DateTime.UtcNow;
                #region SQL Processing - Reader
                using (SqlDataReader sqlRdr = cmd.ExecuteReader())
                {
                    if (sqlRdr.HasRows)
                    {
                        DBTable chartTable = new DBTable(sqlRdr);
                        Chart_Call_Type(chartTable);
                    }
                }
                #endregion SQL Processing - Reader
                ghFunctions.print_loadtime(lblResults, "txt: data_call_type_old", dtStart, DateTime.UtcNow);
            }
            #endregion SQL Command
        }
        #endregion SQL Connection
    }
    protected void Data_Call_by_Hour_Old()
    {
        #region SQL Connection
        using (SqlConnection con = new SqlConnection(sqlStr))
        {
            #region SQL Command
            using (SqlCommand cmd = new SqlCommand("", con))
            {
                if (con.State == ConnectionState.Closed) { con.Open(); }
                cmd.CommandTimeout = 600;
                cmd.CommandText = "[dbo].[sp_dashboard_get_calls_by_hour]";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                #region SQL Parameters
                cmd.Parameters.Add(new SqlParameter("@sp_clientid", "0"));
                DateTime dtFrom = DateTime.Parse(dtStartDate.Text + " " + dtStartTime.Text);
                DateTime dtTo = DateTime.Parse(dtEndDate.Text + " " + dtEndTime.Text);
                cmd.Parameters.Add(new SqlParameter("@sp_date_start", dtConverted(dtFrom)));
                cmd.Parameters.Add(new SqlParameter("@sp_date_end", dtConverted(dtTo)));
                DataTable clientCampaigns = dt_Client_Campaigns();
                SqlParameter tvpParam = cmd.Parameters.AddWithValue("@sp_campaigns", clientCampaigns);
                tvpParam.SqlDbType = SqlDbType.Structured;
                tvpParam.TypeName = "dbo.CampaignTableType";
                #endregion SQL Parameters
                //print_sql(cmd); // Will print for Admin in Local
                DateTime dtStart = DateTime.UtcNow;
                #region SQL Processing - Reader
                using (SqlDataReader sqlRdr = cmd.ExecuteReader())
                {
                    if (sqlRdr.HasRows)
                    {
                        DBTable chartTable = new DBTable(sqlRdr);
                        Chart_Call_by_Hour(chartTable);
                        Chart_Average_SA_by_Hour(chartTable);
                    }
                }
                #endregion SQL Processing - Reader
                ghFunctions.print_loadtime(lblResults, "txt: data_call_by_hour_old", dtStart, DateTime.UtcNow);
            }
            #endregion SQL Command
        }
        #endregion SQL Connection
    }
    protected void Data_Support_Level()
    {
        double sprtlvl = 0;
        Double.TryParse(cntSupportLevel.Value, out sprtlvl);
        //double sprtlvl = Convert.ToDouble(cntSupportLevel.Value);

        int chartIndex = 0;
        // The value to display on the meter
        double value = sprtlvl;

        // The background and border colors of the meters
        //int[] bgColor = { 0x88CCFF, 0xFFDDDD };
        int[] bgColor = { 0xf5f5f5, 0xFFDDDD };
        int[] borderColor = { 0xcccccc, 0x880000 };

        // Create an AngularMeter object of size 300 x 200 pixels with transparent background
        // AngularMeter m = new AngularMeter(300, 200, Chart.Transparent);
        AngularMeter m = new AngularMeter(250, 200, Chart.Transparent);

        // Center at (150, 150), scale radius = 124 pixels, scale angle -90 to +90 degrees
        //m.setMeter(150, 150, 124, -90, 90);
        m.setMeter(125, 150, 100, -90, 90);

        // Background gradient color with brighter color at the center
        double[] bgGradient = { 0, m.adjustBrightness(bgColor[chartIndex], 3), 0.75, bgColor[chartIndex] }
            ;

        // Add a scale background of 148 pixels radius using the background gradient, with a 13 pixel
        // thick border
        m.addScaleBackground(124, m.relativeRadialGradient(bgGradient), 13, borderColor[chartIndex]);

        // Meter scale is 0 - 100, with major tick every 20 units, minor tick every 10 units, and micro tick every 5 units
        //m.setScale(0, 100, 20, 10, 5);
        // Meter scale is 0 - 100, with out major tick, minor tick every 10 units, and micro tick every 5 units
        m.setScale(0, 100, 0, 10, 5);

        // Set the scale label style to 15pt Arial Italic. Set the major/minor/micro tick lengths to
        // 16/16/10 pixels pointing inwards, and their widths to 2/1/1 pixels.
        m.setLabelStyle("Arial Italic", 16);
        m.setTickLength(-16, -16, -10);
        m.setLineWidth(0, 2, 1, 1);

        // Demostrate different types of color scales and putting them at different positions
        double[] smoothColorScale = { 0, 0xff0000, 25, 0xdddd00, 50, 0x3333ff, 75, 0x0088ff, 100, 0x00ff00 };
        Int32 vColor = 0x00ff00;
        if (value < 25) vColor = 0xff0000;
        if (value >= 25 && value < 50) vColor = 0xdddd00;
        if (value >= 50 && value < 75) vColor = 0x3333ff;
        if (value >= 75 && value < 90) vColor = 0x0088ff;

        if (chartIndex == 0)
        {
            // Add the smooth color scale at the default position
            m.addColorScale(smoothColorScale);
            // Add a red (0xff0000) triangular pointer starting from 38% and ending at 60% of scale
            // radius, with a width 6 times the default
            m.addPointer2(value, 0x595959, -1, Chart.TriangularPointer2, 0.38, 0.6, 6);
        }
        else
        {
            // Add the smooth color scale starting at radius 124 with zero width and ending at radius
            // 124 with 16 pixels inner width
            m.addColorScale(smoothColorScale, 124, 0, 124, -16);
            // Add a red (0xff0000) pointer
            m.addPointer2(value, 0xFF0000);
        }

        // Configure a large "pointer cap" to be used as the readout circle at the center. The cap
        // radius and border width is set to 33% and 4% of the meter scale radius. The cap color is dark
        // blue (0x000044). The border color is light blue (0x66bbff) with a 60% brightness gradient
        // effect.
        m.setCap2(Chart.Transparent, 0xbfbfbf, 0x666666, 0.6, 0, 0.33, 0.04);

        // Add value label at the center with light blue (0x66ddff) 28pt Arial Italic font
        m.addText(125, 150, m.formatValue(value, "{value|0}"), "Arial Italic", 24, vColor, Chart.Center).setMargin(0);

        // Output the chart
        // http://www.advsofteng.com/doc/cdnetdoc/outputfile.htm
        chartSupportLevel.Image = m.makeWebImage(Chart.PNG);
        string iName = Session["imgNameSel"].ToString().Replace("[type]", "sl").Replace("[client]", strClient);
        m.makeChart(Server.MapPath("/offline/charts/" + iName));
    }
    protected void Chart_Average_SA_by_Hour(DBTable table)
    {
        // The data for the line chart
        //double[] data = { 60, 0, 5, 5, 10, 10, 15, 15, 20, 20, 75, 125, 250, 250, 250, 250, 125, 75, 15, 10, 10, 5, 5, 0, 0 };
        //string[] data = { "00:00", "00:15", "00:10", "00:25", "01:15", "02:05", "00:25", "01:15", "00:02", "00:37", "00:35", "00:20", "00:15", "00:40", "00:55", "00:15", "00:05", "00:15", "00:02", "00:01", "00:32", "00:45", "00:65", "00:20", "00:15" };
        double[] data = table.getCol(3);
        

        // The labels for the line chart
        //string[] labels = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23", "24" };
        string[] labels = table.getColAsString(1);

        // Create a XYChart object of size 250 x 250 pixels
        XYChart c = new XYChart(700, 255);

        // Transparent Background
        c.setBackground(Chart.Transparent);
        //c.setBackground(0xcccccc);

        // Set the plotarea at (30, 20) and of size 200 x 200 pixels
        c.setPlotArea(60, 20, 620, 200, Chart.Transparent, -1, -1, 0xcccccc, 0xcccccc);

        // Set the plotarea at (55, 58) and of size 520 x 195 pixels, with white background. Turn on
        // both horizontal and vertical grid lines with light grey color (0xcccccc)
        //c.setPlotArea(55, 58, 520, 195, 0xffffff, -1, -1, 0xcccccc, 0xcccccc);

        // Add a line chart layer using the given data
        c.addLineLayer(data);

        c.yAxis().setLabelFormat("{value|nn:ss}");
        //c.xAxis().setLabelFormat("{value|hh:nn:ss:fff}");

        // Set the labels on the x axis.
        c.xAxis().setLabels(labels);

        // Display 1 out of 3 labels on the x-axis.
        c.xAxis().setLabelStep(1);


        // Output the chart
        chartDesignationCount.Image = c.makeWebImage(Chart.PNG);
        // Include tool tip for the chart
        chartDesignationCount.ImageMap = c.getHTMLImageMap("", "", "title='Hour {xLabel}: Calls {value}'");

        string iName = Session["imgNameSel"].ToString().Replace("[type]", "ah").Replace("[client]", strClient);
        c.makeChart(Server.MapPath("/offline/charts/" + iName));

    }
    protected void GoTo_Dashboard_Report(object sender, EventArgs e)
    {
        Response.Redirect("~/Dashboard.aspx");
        //or
        //Server.Transfer("~/Dashboard.aspx");
    }
    /// <summary>
    /// This functions should be put into their own file
    /// 
    /// </summary>
    protected Double GetPercent(double left, double right)
    {
        double rtrn = 0;
        if (left > 0 && right > 0)
        {
            rtrn = left / right;
        }
        return rtrn;
    }
    protected String dispo_percent(String count)
    {
        double cnt = Convert.ToInt32(count);
        double total = Convert.ToInt32(hfDispoCount.Value);

        return String.Format("{0:P2}", GetPercent(cnt,total));
    }
    protected String campaign_percent(String count)
    {
        double cnt = Convert.ToInt32(count);
        double total = Convert.ToInt32(hfCampaignCount.Value);

        return String.Format("{0:P2}", GetPercent(cnt, total));
    }
    
    protected String EpochTo_FormatTime_Local(String Epoch)
    {
        var utcDate = DateTime.Now.ToUniversalTime();
        long baseTicks = 621355968000000000;
        long tickResolution = 10000000;
        long epoch = Convert.ToInt32(Epoch);
        long epochTicks = (epoch * tickResolution) + baseTicks;

        int TZ = -7;
        return new DateTime(epochTicks, DateTimeKind.Utc).AddHours(TZ).ToString("MM/dd/yyyy hh:mm:ss tt");
    }
    protected string DateToFormatTime(DateTime dt)
    {
        return dt.ToString("MM/dd/yyyy hh:mm:ss tt");
    }
    protected DateTime EpochToDate_Local(String Epoch)
    {
        var utcDate = DateTime.Now.ToUniversalTime();
        long baseTicks = 621355968000000000;
        long tickResolution = 10000000;
        long epoch = Convert.ToInt32(Epoch);
        long epochTicks = (epoch * tickResolution) + baseTicks;

        int TZ = -7;
        return new DateTime(epochTicks, DateTimeKind.Utc).AddHours(TZ);
    }
    protected String SecondsTo(Double Seconds)
    {
        TimeSpan time = TimeSpan.FromSeconds(Seconds);

        String rtrn = String.Format("{0}:{1}:{2}",
            Math.Floor(time.TotalHours).ToString().PadLeft(2, '0'),
            time.Minutes.ToString().PadLeft(2, '0'),
            time.Seconds.ToString().PadLeft(2, '0'));

        return rtrn;
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

        //ErrorLog.ErrorLog_Save(ex, dv, "Ameriprise Admin Portal", error, spPage, spQS, spURL);
    }
    public DateTime dtConverted(DateTime dt)
    {
        // Convert the supplied DT back to UTC based on PST
        Int32 dtOffSet = 7;
        DateTime dtCurrent = DateTime.Now;
        System.TimeZone localZone = System.TimeZone.CurrentTimeZone;
        if (localZone.IsDaylightSavingTime(dtCurrent))
        {
            dtOffSet = 7;
        }
        else
        {
            dtOffSet = 8;
        }
        dtOffSet = 0;
        return dt.AddHours(dtOffSet);
    }
    protected void print_sql(SqlCommand cmd)
    {
        print_sql(cmd, sqlPrint, "append");
    }
    protected void print_sql(SqlCommand cmd, String type)
    {
        //ghFunctions.print_sql(cmd, sqlPrint, type);
        print_sql(cmd, sqlPrint, type);
    }
    protected void print_sql(SqlCommand cmd, Label lblPrint, String type)
    {
        #region Print SQL
        // 
        // Connection.GetConnectionType() == "Local"
        if (Page.User.IsInRole("System Administrator") == true && Page.User.Identity.Name == "nciambotti@greenwoodhall.com")
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
    protected String call_details_type()
    {
        String rtrn = "type";

        return rtrn;
    }

    #region Identity Functions
    protected void identity_get_userid()
    {
        // Get the logged in users userid
        // This should be retrieved during the login process
        // if (Session["userid"] == null) { identity_get_userid(); }
        // cmd.Parameters.Add(new SqlParameter("@sp_actor", Session["userid"].ToString()));
        try
        {
            #region SQL Connection
            using (SqlConnection con = new SqlConnection(sqlStrInt))
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
                    //print_sql(cmd, "append"); // Will print for Admin in Local
                    DateTime dtStart = DateTime.UtcNow;
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
                    ghFunctions.print_loadtime(lblResults, "sp: identity_get_userid", dtStart, DateTime.UtcNow);
                }
                #endregion SQL Command
            }
            #endregion SQL Connection
        }
        catch (Exception ex)
        {
            //Error_Catch(ex, "User Get UserID", DeBug_Footer);
            //Error_Save(ex, "User Get UserID");
        }
    }
    protected string identity_get_username(string userid)
    {
        // Get the username from userid
        string username = userid;
        if (userid == "0" || userid == "")
        {
            username = "system";
        }
        else
        {
            try
            {
                #region SQL Connection
                using (SqlConnection con = new SqlConnection(sqlStrInt))
                {
                    #region SQL Command
                    using (SqlCommand cmd = new SqlCommand("", con))
                    {
                        if (con.State == ConnectionState.Closed) { con.Open(); }
                        cmd.CommandTimeout = 600;
                        String cmdText = "";
                        cmdText = "SELECT TOP 1 [u].[username] FROM [portal_user].[dbo].[user] [u] WITH(NOLOCK) WHERE [u].[userid] = @sp_userid";
                        cmd.CommandText = cmdText;
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Clear();
                        #region SQL Parameters
                        //cmd.Parameters.Add(new SqlParameter("@UserID", UserID));
                        cmd.Parameters.Add(new SqlParameter("@sp_userid", userid));
                        #endregion SQL Parameters
                        print_sql(cmd, "append"); // Will print for Admin in Local
                        DateTime dtStart = DateTime.UtcNow;
                        #region SQL Processing
                        using (SqlDataReader sqlRdr = cmd.ExecuteReader())
                        {
                            if (sqlRdr.HasRows)
                            {
                                while (sqlRdr.Read())
                                {
                                    username = sqlRdr["username"].ToString();
                                    username = username.Replace("@greenwoodhall.com", "");
                                }
                            }
                        }
                        #endregion SQL Processing
                        ghFunctions.print_loadtime(lblResults, "txt: identity_get_username", dtStart, DateTime.UtcNow);
                    }
                    #endregion SQL Command
                }
                #endregion SQL Connection
            }
            catch (Exception ex)
            {
                //Error_Catch(ex, "User Get UserID", DeBug_Footer);
                Error_Save(ex, "User Get UserID");
            }
        }
        return username;
    }
    protected bool identity_is_admin()
    {
        if (Page.User.IsInRole("System Administrator") == true
            || Page.User.IsInRole("Administrator") == true
            || Page.User.IsInRole("Manager") == true
            || (Page.User.IsInRole("Advisor") == true && Page.User.Identity.Name != "agent2014")
            )
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    protected bool identity_is_admin_super()
    {
        if (Page.User.IsInRole("System Administrator") == true)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    protected bool identity_can_edit()
    {
        if (Page.User.IsInRole("System Administrator") == true
            || Page.User.IsInRole("Administrator") == true
            || (Page.User.IsInRole("Advisor") == true && Page.User.Identity.Name != "agent2014")
            )
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    #endregion Identity Functions
}