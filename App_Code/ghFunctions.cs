using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Xml;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using System.Data;
/// <summary>
/// Utility functions for various usage
/// </summary>
public class ghFunctions
{
    public ghFunctions()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    static public string portalVersion { get; set; }
    static public String getPortalVersion()
    {
        return System.Configuration.ConfigurationManager.AppSettings["portal_version"];
    }
    private static readonly DateTime EpochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    static public Int32 dtUserOffSet { get; set; }
    static public DateTime dtConverted(DateTime dt)
    {
        // Convert ETC to UTC
        //Int32 dtOffSet = 5;
        //DateTime dtCurrent = DateTime.Now;
        //System.TimeZone localZone = System.TimeZone.CurrentTimeZone;
        //if (localZone.IsDaylightSavingTime(dtCurrent))
        //{
        //    dtOffSet = 4;
        //}
        //else
        //{
        //    dtOffSet = 5;
        //}
        //if (dtUserOffSet != 0) dtOffSet = dtUserOffSet;
        //return dt.AddHours(dtOffSet);
        return dt.AddHours(dtUserOffSet);

    }
    static public DateTime dtConvertedBack(DateTime dt)
    {
        //// Convert UTC to ETC (All time should be UTC)
        //Int32 dtOffSet = -5;
        //DateTime dtCurrent = DateTime.Now;
        //System.TimeZone localZone = System.TimeZone.CurrentTimeZone;
        //if (localZone.IsDaylightSavingTime(dtCurrent))
        //{
        //    dtOffSet = -4;
        //}
        //else
        //{
        //    dtOffSet = -5;
        //}
        //if (dtUserOffSet != 0) dtOffSet = dtUserOffSet;

        //return dt.AddHours(-dtOffSet);
        return dt.AddHours(-dtUserOffSet);

    }
    static public DateTime dtOffSetForward(DateTime dt)
    {

        /// Convert DateTime based on User Offset
        /// 

        //Int32 dtOffSet = -5;
        //DateTime dtCurrent = DateTime.Now;
        //System.TimeZone localZone = System.TimeZone.CurrentTimeZone;
        //if (localZone.IsDaylightSavingTime(dtCurrent))
        //{
        //    dtOffSet = -4;
        //}
        //else
        //{
        //    dtOffSet = -5;
        //}
        //if (dtUserOffSet != 0) dtOffSet = dtUserOffSet;
        //return dt.AddHours(-dtOffSet);

        return dt.AddHours(dtUserOffSet);
    }
    static public DateTime dtOffSetBack(DateTime dt)
    {
        /// Convert DateTime based on User Offset
        /// Backwards - Minus

        //Int32 dtOffSet = -5;
        //DateTime dtCurrent = DateTime.Now;
        //System.TimeZone localZone = System.TimeZone.CurrentTimeZone;
        //if (localZone.IsDaylightSavingTime(dtCurrent))
        //{
        //    dtOffSet = -4;
        //}
        //else
        //{
        //    dtOffSet = -5;
        //}
        //if (dtUserOffSet != 0) dtOffSet = dtUserOffSet;
        //return dt.AddHours(-dtOffSet);

        return dt.AddHours(-dtUserOffSet);
    }
    static public String EpochTo_FormatTime_Local(String Epoch)
    {
        var utcDate = DateTime.Now.ToUniversalTime();
        long baseTicks = 621355968000000000;
        long tickResolution = 10000000;
        long epoch = Convert.ToInt32(Epoch);
        long epochTicks = (epoch * tickResolution) + baseTicks;

        int TZ = -7;
        return new DateTime(epochTicks, DateTimeKind.Utc).AddHours(TZ).ToString("MM/dd/yyyy hh:mm:ss tt");
    }
    static public string DateToFormatTime(DateTime dt)
    {
        return dt.ToString("MM/dd/yyyy hh:mm:ss tt");
    }
    static public DateTime EpochToDate_Local(String Epoch)
    {
        var utcDate = DateTime.Now.ToUniversalTime();
        long baseTicks = 621355968000000000;
        long tickResolution = 10000000;
        long epoch = Convert.ToInt32(Epoch);
        long epochTicks = (epoch * tickResolution) + baseTicks;

        int TZ = -7;
        return new DateTime(epochTicks, DateTimeKind.Utc).AddHours(TZ);
    }
    static public long EpochFromDate_Local(DateTime Date)
    {
        TimeSpan elapsedTime = Date - EpochStart;
        return (long)elapsedTime.TotalSeconds;
    }
    static public Double DoubleFromString(String Value)
    {
        Double rtrn = Convert.ToDouble(Value);
        return rtrn;
    }
    static public String SecondsTo(Double Seconds)
    {
        TimeSpan time = TimeSpan.FromSeconds(Seconds);

        String rtrn = String.Format("{0}:{1}:{2}",
            Math.Floor(time.TotalHours).ToString().PadLeft(2, '0'),
            time.Minutes.ToString().PadLeft(2, '0'),
            time.Seconds.ToString().PadLeft(2, '0'));

        return rtrn;
    }
    static public String SecondsTo(String strSeconds)
    {
        String rtrn = "";
        try
        {
            Double Seconds;
            Double.TryParse(strSeconds, out Seconds);

            rtrn = SecondsTo(Seconds);
        }
        catch
        {
            rtrn = strSeconds;
        }

        return rtrn;
    }
    static public String MillisecondsTo(Double mSeconds)
    {
        String rtrn = "";
        try
        {
            TimeSpan time = TimeSpan.FromMilliseconds(mSeconds);

            rtrn = String.Format("{0}:{1}:{2}",
                     ((int)time.TotalHours < 10) ? ((int)time.TotalHours).ToString().PadLeft(2, '0') : ((int)time.TotalHours).ToString()
                     , time.Minutes.ToString().PadLeft(2, '0')
                     , time.Seconds.ToString().PadLeft(2, '0')
                     );
        }
        catch
        {
            rtrn = "error";
        }

        return rtrn;
    }
    static public string date_label(string date)
    {
        //Eval("timestamp_start").ToString()
        DateTime dt;
        if (date.Length > 0)
        {
            Boolean dtResponse;
            dtResponse = DateTime.TryParse(date, out dt);
            if (dtResponse)
            {
                DateTime dtNow = DateTime.UtcNow;
                dtNow = dtConvertedBack(dtNow);
                dt = dtConvertedBack(dt);
                if (dt.ToString("MM/dd/yyyy") == dtNow.ToString("MM/dd/yyyy"))
                {
                    return dt.ToString("hh:mm:ss tt");
                }
                else
                {
                    return dt.ToString("MM/dd/yyyy");
                }
            }
            else
            {
                return date;
            }
        }
        else
        {
            return "";
        }
    }
    static public string date_label_full_short(string date)
    {
        if (date.Length > 0)
        {
            DateTime dt;
            dt = DateTime.Parse(date);
            dt = dtConvertedBack(dt);
            return dt.ToString("MM/dd hh:mm tt");
            // return dt.ToString("MM/dd/yyyy hh:mm:ss tt");
        }
        else
        {
            return "";
        }
    }
    static public string date_label_full(string date)
    {
        if (date.Length > 0)
        {
            DateTime dt;
            dt = DateTime.Parse(date);
            dt = dtConvertedBack(dt);
            return dt.ToString("MM/dd/yyyy hh:mm:ss tt");
        }
        else
        {
            return "";
        }
    }
    static public string date_label_full_noconvert(string date)
    {
        DateTime dt;
        if (date.Length > 0)
        {
            Boolean dtResponse;
            dtResponse = DateTime.TryParse(date, out dt);
            if (dtResponse)
            {
                return dt.ToString("MM/dd/yyyy hh:mm:ss tt");
            }
            else
            {
                return date;
            }
        }
        else
        {
            return "";
        }
    }
    static public string date_label_full_tz(string date, string tz)
    {
        //TimeZoneInfo.Local.StandardName
        if (date.Length > 0)
        {
            DateTime dt;
            dt = DateTime.Parse(date);

            TimeZoneInfo est;
            try { est = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"); }
            catch {est = TimeZoneInfo.Local;}

            
            dt = dt.AddHours(est.BaseUtcOffset.Hours);
            
            //dt = dtConvertedBack(dt);
            string rtrn = dt.ToString("MM/dd/yyyy hh:mm:ss tt");
            rtrn += " " + est.StandardName;
            return rtrn;
        }
        else
        {
            return "";
        }
    }
    static public string date_label_full_utc(string date)
    {
        if (date.Length > 0)
        {
            DateTime dt;
            dt = DateTime.Parse(date);
            return dt.ToString("MM/dd/yyyy hh:mm:ss tt");
        }
        else
        {
            return "";
        }
    }
    static public string date_label_only(string date)
    {
        if (date.Length > 0)
        {
            DateTime dt;
            dt = DateTime.Parse(date);
            dt = dtConvertedBack(dt);
            return dt.ToString("MM/dd/yy HH:mm");
        }
        else
        {
            return "";
        }
    }
    static public string date_label_only_date(string date)
    {
        if (date.Length > 0)
        {
            DateTime dt;
            dt = DateTime.Parse(date);
            dt = dtConvertedBack(dt);
            return dt.ToString("MM/dd/yyyy");
        }
        else
        {
            return "";
        }
    }
    static public string date_label_length(string date_start, string date_end)
    {
        //TimeZoneInfo.Local.StandardName
        if (date_start.Length > 0 && date_end.Length > 0)
        {
            // SecondsTo
            DateTime dtStart;
            DateTime dtEnd;
            if (DateTime.TryParse(date_start, out dtStart) && DateTime.TryParse(date_end, out dtEnd))
            {
                String dtDuration = ghFunctions.SecondsTo((dtEnd - dtStart).TotalSeconds);
                return dtDuration;
            }
            else
            {
                return "00:00:00";
            }
        }
        else
        {
            return "00:00:00";
        }
    }
    static public string date_label_mil(string date)
    {
        if (date.Length > 0)
        {
            DateTime dt;
            dt = DateTime.Parse(date);
            dt = dtConvertedBack(dt);
            return dt.ToString("MM/dd/yyyy HH:mm:ss");
        }
        else
        {
            return "";
        }
    }
    static public string date_label_short(string date)
    {
        if (date.Length > 0)
        {
            DateTime dt;
            dt = DateTime.Parse(date);
            dt = dtConvertedBack(dt);
            return dt.ToString("MM/dd/yyyy");
        }
        else
        {
            return "";
        }
    }
    static public string date_label_short_noconvert(string date)
    {
        if (date.Length > 0)
        {
            DateTime dt;
            dt = DateTime.Parse(date);
            return dt.ToString("MM/dd/yyyy");
        }
        else
        {
            return "";
        }
    }
    static public string xml_from_string(string value)
    {
        // http://forums.asp.net/t/1145533.aspx
        // http://stackoverflow.com/questions/2149051/how-to-display-formatted-xml
        // Wrap the response in <XMP>string</XMP>
        string regex = "(<password>.*</password>)";
        value = Regex.Replace(value, regex, "<password>***</password>");
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(value);
        XmlElement newXML = doc.DocumentElement;
        return xml_from_doc(doc.DocumentElement);
    }
    static public string xml_from_doc(XmlNode xmlNode)
    {
        StringBuilder builder = new StringBuilder();
        // We will use stringWriter to push the formated xml into our StringBuilder bob.
        builder.Append("\n");
        using (StringWriter stringWriter = new StringWriter(builder))
        {
            // We will use the Formatting of our xmlTextWriter to provide our indentation.
            using (XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter))
            {
                xmlTextWriter.Formatting = Formatting.Indented;
                xmlNode.WriteTo(xmlTextWriter);
            }
        }
        return builder.ToString();
    }
    static public string header_label_payment(string field, string type)
    {
        string rtrn = field;
        if (field.Length > 0 && type.Length > 0)
        {
            if (type == "PayPal")
            {
                if (field == "CC_Type")
                {
                    rtrn = "Token";
                }
                if (field == "CC_Number")
                {
                    rtrn = "PayerID";
                }
                if (field == "CC_Exp")
                {
                    rtrn = "TransactionID";
                }
            }
        }
        return rtrn;
    }
    static public void WriteToLabel(String type, String color, String msg, Label lbl)
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
    protected string xml_to_string(string xml)
    {
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(xml);
        //return xmlDoc.OuterXml.ToString();

        return "<XMP>" + xmlDoc.OuterXml.ToString().Replace("<", "\n<").Replace("\n</", "</") + "</XMP>";
    }
    protected string ind(int x)
    {
        return new string(' ', x);
    }
    protected string xml_format(string xml)
    {
        //xml = xml.ToString().Replace("<", "[").Replace(">", "]").Replace("[", "<span style='color: blue;'>[").Replace("]", "]</span>").Replace("\r", "<br />");
        string newXML = xml;
        newXML = newXML.Replace("\n", "").Replace("\r", "");
        if (newXML.Contains("<GetCallResponse") || newXML.Contains("<leadResponse"))
        {
            newXML = newXML.Replace("<?xml", "\n<?xml");
            newXML = newXML.Replace("<soap:Envelope", "\n" + ind(0) + "<soap:Envelope").Replace("</soap:Envelope", "\n" + ind(0) + "</soap:Envelope");
            newXML = newXML.Replace("<soap:Body", "\n" + ind(2) + "<soap:Body").Replace("</soap:Body", "\n" + ind(2) + "</soap:Body");

            newXML = newXML.Replace("<GetCallResponse", "\n" + ind(4) + "<GetCallResponse");
            newXML = newXML.Replace("</GetCallResponse", "\n" + ind(4) + "</GetCallResponse");
            newXML = newXML.Replace("<leadResponse", "\n" + ind(4) + "<leadResponse");
            newXML = newXML.Replace("</leadResponse", "\n" + ind(4) + "</leadResponse");

            newXML = newXML.Replace("<GetCallResult", "\n" + ind(6) + "<GetCallResult");
            newXML = newXML.Replace("</GetCallResult", "\n" + ind(6) + "</GetCallResult");
            newXML = newXML.Replace("<leadResult", "\n" + ind(6) + "<leadResult");
            newXML = newXML.Replace("</leadResult", "\n" + ind(6) + "</leadResult");

            newXML = newXML.Replace("<code", "\n" + ind(8) + "<code");
            newXML = newXML.Replace("<message", "\n" + ind(8) + "<message");
            newXML = newXML.Replace("<callid", "\n" + ind(8) + "<callid");
            newXML = newXML.Replace("<ani", "\n" + ind(8) + "<ani");
            newXML = newXML.Replace("<school", "\n" + ind(8) + "<school");
            newXML = newXML.Replace("<media", "\n" + ind(8) + "<media");

        }
        else if (newXML.Contains("<lead") || newXML.Contains("<GetCall"))
        {
            newXML = newXML.Replace("<?xml", "\n<?xml");
            newXML = newXML.Replace("<soap:Envelope", "\n" + ind(0) + "<soap:Envelope").Replace("</soap:Envelope", "\n" + ind(0) + "</soap:Envelope");
            newXML = newXML.Replace("<soap:Body", "\n" + ind(2) + "<soap:Body").Replace("</soap:Body", "\n" + ind(2) + "</soap:Body");
            newXML = newXML.Replace("<s:Envelope", "\n" + ind(0) + "<s:Envelope").Replace("</s:Envelope", "\n" + ind(0) + "</s:Envelope");
            newXML = newXML.Replace("<s:Body", "\n" + ind(2) + "<s:Body").Replace("</s:Body", "\n" + ind(2) + "</s:Body");

            newXML = newXML.Replace("<lead", "\n" + ind(4) + "<lead");
            newXML = newXML.Replace("</lead", "\n" + ind(4) + "</lead");
            newXML = newXML.Replace("<GetCall", "\n" + ind(4) + "<GetCall");
            newXML = newXML.Replace("</GetCall", "\n" + ind(4) + "</GetCall");

            newXML = newXML.Replace("<username", "\n" + ind(6) + "<username");
            newXML = newXML.Replace("<password", "\n" + ind(6) + "<password");
            newXML = newXML.Replace("<ani", "\n" + ind(6) + "<ani");
            newXML = newXML.Replace("<dnis", "\n" + ind(6) + "<dnis");
            newXML = newXML.Replace("<school", "\n" + ind(6) + "<school");
            newXML = newXML.Replace("<media", "\n" + ind(6) + "<media");
            newXML = newXML.Replace("<id", "\n" + ind(6) + "<id");
            newXML = newXML.Replace("<datetime", "\n" + ind(6) + "<datetime");


        }
        else if (newXML.Contains("<soap:Fault"))
        {
            newXML = newXML.Replace("<?xml", "\n<?xml");
            newXML = newXML.Replace("<soap:Envelope", "\n" + ind(0) + "<soap:Envelope").Replace("</soap:Envelope", "\n" + ind(0) + "</soap:Envelope");
            newXML = newXML.Replace("<soap:Body", "\n" + ind(2) + "<soap:Body").Replace("</soap:Body", "\n" + ind(2) + "</soap:Body");
            newXML = newXML.Replace("<soap:Fault", "\n" + ind(4) + "<soap:Fault").Replace("</soap:Fault", "\n" + ind(4) + "</soap:Fault");
            newXML = newXML.Replace("<soap:Code", "\n" + ind(6) + "<soap:Code").Replace("</soap:Code", "\n" + ind(6) + "</soap:Code");
            newXML = newXML.Replace("<soap:Value", "\n" + ind(8) + "<soap:Value");
            newXML = newXML.Replace("<soap:Reason", "\n" + ind(6) + "<soap:Reason").Replace("</soap:Reason", "\n" + ind(6) + "</soap:Reason");
            newXML = newXML.Replace("<soap:Text", "\n" + ind(8) + "<soap:Text").Replace("</soap:Text", "\n" + ind(8) + "</soap:Text");
            newXML = newXML.Replace("<soap:Detail", "\n" + ind(6) + "<soap:Detail").Replace("</soap:Detail", "\n" + ind(6) + "</soap:Detail");
        }
        else
        {
            newXML = newXML.Replace("<s:Envelope", "\n" + ind(0) + "<s:Envelope").Replace("</s:Envelope", "\n" + ind(0) + "</s:Envelope");
            newXML = newXML.Replace("<s:Body", "\n" + ind(2) + "<s:Body").Replace("</s:Body", "\n" + ind(2) + "</s:Body");
            newXML = newXML.Replace("<s:Header", "\n" + ind(2) + "<s:Header").Replace("</s:Header", "\n" + ind(2) + "</s:Header");
            newXML = newXML.Replace("<a:Action", "\n" + ind(4) + "<a:Action").Replace("</a:Action", "\n" + ind(4) + "</a:Action");
            newXML = newXML.Replace("<a:MessageID", "\n" + ind(6) + "<a:MessageID");
            newXML = newXML.Replace("<a:ReplyTo", "\n" + ind(6) + "<a:ReplyTo").Replace("</a:ReplyTo", "\n" + ind(6) + "</a:ReplyTo");
            newXML = newXML.Replace("<a:Address", "\n" + ind(8) + "<a:Address");
            newXML = newXML.Replace("<a:To", "\n" + ind(6) + "<a:To");
        }
        //http://stackoverflow.com/questions/1359412/c-sharp-remove-text-in-between-delimiters-in-a-string-regex
        //string input = "Give [Me Some] Purple (And More) Elephants";
        //string regex = "(\\[.*\\])|(\".*\")|('.*')|(\\(.*\\))";
        string regex = "(<password>.*</password>)";
        //string output = Regex.Replace(input, regex, "");
        newXML = Regex.Replace(newXML, regex, "<password>***</password>");
        return newXML;
    }
    static public void Donation_Open_Database(SqlConnection con)
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
    static public string since_label(string date, Label lblError)
    {
        string rtrn = "";
        try
        {
            //Eval("timestamp_start").ToString()
            DateTime dt; DateTime dt2;
            if (date.Length > 0)
            {
                // Date comes in as 20150606 - so turn it into a date
                string dtY = date.Substring(0, 4);
                string dtM = date.Substring(4, 2);
                string dtD = date.Substring(6, 2);
                dt = DateTime.Parse(dtM + "/" + dtD + "/" + dtY);
                DateTime dtNow = DateTime.UtcNow;
                double days = (dtNow - dt).TotalDays;
                rtrn = days.ToString("#");
                //rtrn = dtD + "/" + dtM + "/" + dtY;
                //rtrn += "<br />" + dt.ToString("MM/dd/yyyy");
            }
            else
            {
                rtrn = "error";
            }
        }
        catch (Exception ex)
        {
            rtrn = date.Length.ToString();
            lblError.Text = "<hr />" + date;
            lblError.Text += "<br />" + ex.Message;
            lblError.Text += "<br />" + ex.StackTrace;
        }
        return rtrn;
    }
    /// <summary>
    /// Recurring Status
    /// </summary>
    /// <param name="status"></param>
    /// <returns></returns>
    static public string status_recurring_record(string status)
    {
        string rtrn = status;
        if (status.Length > 0)
        {
            switch (status)
            {
                case "1": rtrn = "New"; break;
                case "2": rtrn = "Settled"; break;
                case "0": rtrn = "Declined"; break;
                case "-1": rtrn = "Cancelled"; break;
                case "301001": rtrn = "New Record: Settled"; break;
                case "301002": rtrn = "Processed: Settled"; break;
                case "301003": rtrn = "Processed: Rejected"; break;
                case "301004": rtrn = "Processed: Error"; break;
                case "301005": rtrn = "Cancelled by System"; break;
                case "301006": rtrn = "Cancelled by Donor"; break;
                case "301007": rtrn = "Cancelled by Admin"; break;
                default: rtrn = status; break;
            }
        }
        return rtrn;
    }
    static public string status_recurring_log(string status)
    {
        string rtrn = status;
        if (status.Length > 0)
        {
            switch (status)
            {
                case "1": rtrn = "Started"; break;
                case "2": rtrn = "Settled"; break;
                case "3": rtrn = "Settled (ADU)"; break;
                case "0": rtrn = "Declined"; break;
                case "302000": rtrn = "New Record: Settled"; break;
                case "302001": rtrn = "Processing Start"; break;
                case "302002": rtrn = "Processed: Settled"; break;
                case "302003": rtrn = "Processed: Settled [ADU]"; break;
                case "302004": rtrn = "Processed: Rejected"; break;
                case "302005": rtrn = "Processed: Error"; break;
                case "302006": rtrn = "Processed: Skipped"; break;

                default: rtrn = status; break;
            }
        }
        return rtrn;
    }
    static public string status_tokenization_record(string status)
    {
        string rtrn = status;
        if (status.Length > 0)
        {
            switch (status)
            {
                case "10340001": rtrn = "Success"; break;
                default: rtrn = status; break;
            }
        }
        return rtrn;
    }
    static public void print_sql(SqlCommand cmd, Label lblPrint, String type)
    {
        #region Print SQL
        if (HttpContext.Current.User.IsInRole("System Administrator") == true && Connection.GetConnectionType() == "Local")
        {
            String sqlToText = "";
            sqlToText += cmd.CommandText.ToString().Replace("\n", "<br />").Replace("\r", "<br />"); // Replaces the line breaks from SQL to <br /> for printing
            sqlToText = sqlToText.Replace("<br /><br />", "<br />");
            int cnt = 0;
            foreach (SqlParameter p in cmd.Parameters)
            {
                string pname = "";
                string pvalue = "";
                string ptype = "";
                string prefix = "";
                if (!String.IsNullOrEmpty(p.ParameterName)) { pname = p.ParameterName; }
                if (p.Value != null) { pvalue = p.Value.ToString(); }
                ptype = p.DbType.ToString(); // if (p.DbType != null) { ptype = p.DbType.ToString(); }
                if (cnt > 0) prefix = ",";
                sqlToText += "<br />" + prefix + pname + " = '" + pvalue + "' -- [" + ptype + "]";
                cnt++;
            }
            // new == we make this a new write | else we append
            if (type == "new") { lblPrint.Text = ""; }
            lblPrint.Text = String.Format("<hr />Print: {0}<br />{1}{2}", DateTime.UtcNow.ToString(), sqlToText, lblPrint.Text);
        }
        #endregion Print SQL
    }
    /// <summary>
    /// New Date Functions
    /// </summary>
    static public string dtGetFirstDay()
    {
        DateTime dtNow = DateTime.UtcNow.AddHours(-dtUserOffSet);

        DateTime dt = new DateTime(dtNow.Year, dtNow.Month, 1);
        string rtrn = dt.ToString("MM/dd/yyyy"); // yyyy-MM-dd

        return rtrn;
    }
    static public string dtGetFirstDayFrom(DateTime dtFrom)
    {
        DateTime dtNow = dtFrom.AddHours(-dtUserOffSet);

        DateTime dt = new DateTime(dtNow.Year, dtNow.Month, 1);
        string rtrn = dt.ToString("MM/dd/yyyy"); // yyyy-MM-dd

        return rtrn;
    }
    static public string dtFromString_Date_NoOffset(string date)
    {
        DateTime dt;
        if (date.Length > 0)
        {
            Boolean dtResponse;
            dtResponse = DateTime.TryParse(date, out dt);
            if (dtResponse)
            {
                //dt = dt.AddHours(-dtUserOffSet);
                return dt.ToString("MMddyy");
            }
            else
            {
                return date;
            }
        }
        else
        {
            return "";
        }
    }
    static public string dtFromString_Date(string date)
    {
        DateTime dt;
        if (date.Length > 0)
        {
            Boolean dtResponse;
            dtResponse = DateTime.TryParse(date, out dt);
            if (dtResponse)
            {
                dt = dt.AddHours(-dtUserOffSet);
                return dt.ToString("MMddyy");
            }
            else
            {
                return date;
            }
        }
        else
        {
            return "";
        }
    }
    static public string dtFromString_Time(string date)
    {
        DateTime dt;
        if (date.Length > 0)
        {
            Boolean dtResponse;
            dtResponse = DateTime.TryParse(date, out dt);
            if (dtResponse)
            {
                dt = dt.AddHours(-dtUserOffSet);
                return dt.ToString("HH:mm");
            }
            else
            {
                return date;
            }
        }
        else
        {
            return "";
        }
    }
    static public string dtFromString_Custom(string date, string format, bool offset)
    {
        DateTime dt;
        if (date.Length > 0)
        {
            Boolean dtResponse;
            dtResponse = DateTime.TryParse(date, out dt);
            if (dtResponse)
            {
                if (offset) dt = dt.AddHours(-dtUserOffSet);
                if (format.Length == 0)
                {
                    format = "MM/dd/YYYY";
                }
                return dt.ToString(format);
            }
            else
            {
                return date;
            }
        }
        else
        {
            return "";
        }
    }
    static public string GetSafeFilename(string filename) { return string.Join("_", filename.Split(Path.GetInvalidFileNameChars())); }
    static public DateTime dtConvertFromFive9(String dtFive9)
    {
        // Convert TimeStamp to DateTime
        // 20160523070451932
        DateTime dtParse;
        String dtParseTry;
        //Request[key];
        dtParseTry = dtFive9.Substring(0, 4);
        dtParseTry += "-" + dtFive9.Substring(4, 2);
        dtParseTry += "-" + dtFive9.Substring(6, 2);
        dtParseTry += " " + dtFive9.Substring(8, 2);
        dtParseTry += ":" + dtFive9.Substring(10, 2);
        dtParseTry += ":" + dtFive9.Substring(12, 2);
        if (dtFive9.Length >= 18) dtParseTry += "." + dtFive9.Substring(14, 3);
        if (DateTime.TryParse(dtParseTry, out dtParse))
        {
            //strTime = dtParse.ToString("yyyy-MM-dd HH:ss:mm.ms tt");
            //strTime = dtParse.ToString("d");
        }
        else
        {
            //strTime = dtParseTry;
        }
        return dtParse;
    }
    static public void print_loadtime(Label lbl, String query, DateTime dtStart, DateTime dtEnd)
    {
        bool printMe = false;
        if (HttpContext.Current.User.IsInRole("System Administrator") == true && HttpContext.Current.User.Identity.Name == "nciambotti@greenwoodhall.com") printMe = true;

        if (printMe)
        {
            // sqlPrint | lblResults | lblMessage | lblLoadTime
            TimeSpan t = (dtEnd - dtStart);
            String tLabel = String.Format("{0:D2}h:{1:D2}m:{2:D2}s:{3:D3}ms", t.Hours, t.Minutes, t.Seconds, t.Milliseconds);
            lbl.Text += String.Format("Query: {0} | Time: {1}<br />", query, tLabel);

        }
    }
}
