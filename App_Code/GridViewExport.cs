using System;
using System.Data;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ClosedXML.Excel;
public class GridViewExportUtil
{
    /// <summary>
    /// Tried multiple versions of the Export to Excel features, this was the best suited:
    /// http://forums.asp.net/t/1255489.aspx
    /// 
    /// Will try more in the future to be able to customize the export.
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="gv"></param>
    public static void ClosedXMLExport(string fileName, GridView gv)
    {
        // This works nicely but only for GridView
        // Need to figure out a way to make this more friendly for Panels
        // Or create custom GridViews for all the data
        #region DataTable - GridView
        DataTable dt = new DataTable(fileName);
        foreach (TableCell cell in gv.HeaderRow.Cells)
        {
            dt.Columns.Add(cell.Text);
        }
        foreach (GridViewRow row in gv.Rows)
        {
            GridViewExportUtil.PrepareControlForExport(row);
            dt.Rows.Add();
            for (int i = 0; i < row.Cells.Count; i++)
            {
                if (row.Cells[i].HasControls())
                {
                    string cntrls = "";
                    foreach (Control c in row.Cells[i].Controls)
                    {
                        if (c.GetType() == typeof(Label))
                        {
                            cntrls = ((Label)c).Text;
                        }
                        //cntrls += c.GetType() + "|";
                    }
                    cntrls.TrimEnd('|');
                    dt.Rows[dt.Rows.Count - 1][i] = cntrls;// row.Cells[i].Controls.Count.ToString();
                }
                else
                {
                    dt.Rows[dt.Rows.Count - 1][i] = row.Cells[i].Text;
                }
            }
        }
        #endregion DataTable - GridView

        // Dashboard-Reporting-
        XLWorkbook wb = new XLWorkbook();
        var ws = wb.Worksheets.Add(dt);

        foreach (IXLWorksheet workSheet in wb.Worksheets)
        {
            foreach (IXLTable table in workSheet.Tables)
            {
                workSheet.Table(table.Name).ShowAutoFilter = false;
                workSheet.Columns().AdjustToContents();
            }
        }

        fileName = "Dashboard-Reporting-" + fileName;

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
    }
    public static void ClosedXMLExport_Custom(string fileName, GridView gv)
    {
        XLWorkbook wb = new XLWorkbook();
        var ws = wb.Worksheets.Add(fileName);

        #region Wrap Up - Save/Download the File
        fileName = "Dashboard-Reporting-" + fileName;

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
    public static void Export(string fileName, GridView gv)
    {
        fileName = String.Format("{0}{1}.xls"
            , fileName.Replace(".xls","")
            , DateTime.Now.ToString("-yyyyMMdd-HHmmss")
            );

        HttpContext.Current.Response.Clear();
        HttpContext.Current.Response.AddHeader(
            "content-disposition", string.Format("attachment; filename={0}", fileName));
        HttpContext.Current.Response.ContentType = "application/ms-excel";

        using (StringWriter sw = new StringWriter())
        {
            using (HtmlTextWriter htw = new HtmlTextWriter(sw))
            {
                // We want to spit stuff out as 'text' into Excel
                foreach (GridViewRow r in gv.Rows)
                {
                    if (r.RowType == DataControlRowType.DataRow)
                    {
                        for (int columnIndex = 0; columnIndex < r.Cells.Count; columnIndex++)
                        {
                            r.Cells[columnIndex].Attributes.Add("class", "text");
                        }
                    }
                }
                //  Create a form to contain the grid 
                Table table = new Table();

                //  add the header row to the table 
                if (gv.HeaderRow != null)
                {
                    GridViewExportUtil.PrepareControlForExport(gv.HeaderRow);
                    table.Rows.Add(gv.HeaderRow);
                }

                //  add each of the data rows to the table 
                foreach (GridViewRow row in gv.Rows)
                {
                    GridViewExportUtil.PrepareControlForExport(row);
                    table.Rows.Add(row);
                    
                    
                }

                //  add the footer row to the table 
                if (gv.FooterRow != null)
                {
                    GridViewExportUtil.PrepareControlForExport(gv.FooterRow);
                    table.Rows.Add(gv.FooterRow);
                }

                //  render the table into the htmlwriter 
                table.RenderControl(htw);
                // Second part of sending to Excel as text
                string style = @"<style> .text { mso-number-format:\@; } </style> ";
                HttpContext.Current.Response.Write(style);
                //  render the htmlwriter into the response 
                HttpContext.Current.Response.Write(sw.ToString());
                HttpContext.Current.Response.End();
            }
        }
    }
    public static void ExportPanel(string fileName, Panel pn)
    {
        fileName = String.Format("{0}{1}.xls", fileName.Replace(".xls", ""), DateTime.Now.ToString("-yyyyMMdd-HHmmss"));

        HttpContext.Current.Response.Clear();
        HttpContext.Current.Response.AddHeader("content-disposition", string.Format("attachment; filename={0}", fileName));
        HttpContext.Current.Response.ContentType = "application/ms-excel";

        using (StringWriter sw = new StringWriter())
        {
            using (HtmlTextWriter htw = new HtmlTextWriter(sw))
            {
                GridViewExportUtil.PrepareControlForExport(pn);
                pn.RenderControl(htw);
                // Second part of sending to Excel as text
                string style = @"<style> .text { mso-number-format:\@; } </style> ";
                HttpContext.Current.Response.Write(style);
                //  render the htmlwriter into the response 
                HttpContext.Current.Response.Write(sw.ToString());
                HttpContext.Current.Response.End();
            }
        }
    }
    /// <summary> 
    /// Replace any of the contained controls with literals 
    /// </summary> 
    /// <param name="control"></param> 
    private static void PrepareControlForExport(Control control)
    {
        for (int i = 0; i < control.Controls.Count; i++)
        {
            Control current = control.Controls[i];
            if (current is LinkButton)
            {
                control.Controls.Remove(current);
                control.Controls.AddAt(i, new LiteralControl((current as LinkButton).Text));
            }
            else if (current is ImageButton)
            {
                control.Controls.Remove(current);
                control.Controls.AddAt(i, new LiteralControl((current as ImageButton).AlternateText));
            }
            else if (current is HyperLink)
            {
                control.Controls.Remove(current);
                control.Controls.AddAt(i, new LiteralControl((current as HyperLink).Text));
            }
            else if (current is DropDownList)
            {
                control.Controls.Remove(current);
                control.Controls.AddAt(i, new LiteralControl((current as DropDownList).SelectedItem.Text));
            }
            else if (current is CheckBox)
            {
                control.Controls.Remove(current);
                control.Controls.AddAt(i, new LiteralControl((current as CheckBox).Checked ? "True" : "False"));
            }

            if (current.HasControls())
            {
                GridViewExportUtil.PrepareControlForExport(current);
            }
        }
    }
}