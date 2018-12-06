using System;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
public partial class _sample : System.Web.UI.Page
{
    private String sqlStrARC = Connection.GetConnectionString("Default", ""); // Controlled by Web Config ARC_Live | ARC_Stage
    private String sqlStrDE = Connection.GetConnectionString("DE_Stage", ""); // DE_Production || DE_Stage
    protected void Page_Load(object sender, EventArgs e)
    {
        // This looks cool:
        // http://stackoverflow.com/questions/19823803/getting-datakeynames-for-row-on-button-click/19824029
    }
    protected void NewVoid(object sender, EventArgs e)
    {
        String msgLog = "";
        bool err = false;
        #region Try Stuff
        try
        {
            msgResults.Text = "DoingStuff";
            msgLog += String.Format("<li>{0}</li>", "StuffDone");
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
    protected void NewSQL(object sender, EventArgs e)
    {
        String msgLog = "";
        #region CMD Types
        #region SQL Connection
        using (SqlConnection con = new SqlConnection(sqlStrDE))
        {
            ghFunctions.Donation_Open_Database(con);
            bool dodelete = false;
            bool doexit = false;
            bool doerror = false;
            bool unlocked = false;
            bool err = false;
            #region SQL Command
            using (SqlCommand cmd = new SqlCommand("", con))
            {
                #region Build cmdText
                String cmdText = "";
                cmdText = @"
IF EXISTS(SELECT TOP 1 1 FROM [dataexchange_ticket].[dbo].[ticket_lock] [tl] WHERE [tl].[ticketid] = @sp_ticketid)
BEGIN
	IF EXISTS(SELECT TOP 1 1 FROM [dataexchange_ticket].[dbo].[ticket_lock] [tl] WHERE [tl].[ticketid] = @sp_ticketid AND  [tl].[authorid] = @sp_authorid)
	BEGIN
		SELECT 'DELETE' [result], 'Move the user to the ticket page, already locked by them.' [message]
	END
	ELSE
	BEGIN
		SELECT 'LOCKED' [result],'Ticket is locked by someone else' [message]
	END
END
ELSE
BEGIN
	SELECT 'UNLOCKED' [result],'Ticket is not locked' [message]
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
                cmd.Parameters.Add("@sp_ticketid", SqlDbType.Int).Value = sp_ticketid;
                cmd.Parameters.Add("@sp_authorid", SqlDbType.Int).Value = Session["userid"].ToString();
                #endregion SQL Command Parameters
                #region SQL Command Processing
                var chckResults = cmd.ExecuteScalar();
                if (chckResults != null && chckResults.ToString().Length > 0)
                {
                    // We inserted the ticket
                    if (chckResults.ToString() == "DELETE")
                    {
                        dodelete = true;
                    }
                    else if (chckResults.ToString() == "LOCKED")
                    {
                        doexit = true;
                    }
                    else if (chckResults.ToString() == "LOCKED")
                    {
                        doexit = true;
                    }
                    else
                    {
                        msgLog += String.Format("<li>{0}</li>", "Did not recognize message from database");
                    }
                }
                else
                {
                    doerror = true;
                    // Something funky will rodgers
                    msgLog += String.Format("<li>{0}</li>", "Did not get expected result from 1st query");

                }
                #endregion SQL Command Processing
            }
            #endregion SQL Command
            if (dodelete)
            {
                // We are good to delete
                #region SQL Command
                using (SqlCommand cmd = new SqlCommand("", con))
                {
                    #region Build cmdText
                    String cmdText = "";
                    cmdText = @"
DELETE FROM [dataexchange_ticket].[dbo].[ticket_lock]
WHERE [ticketid] = @sp_ticketid
AND [authorid] = @sp_authorid
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
                    cmd.Parameters.Add("@sp_ticketid", SqlDbType.Int).Value = sp_ticketid;
                    cmd.Parameters.Add("@sp_authorid", SqlDbType.Int).Value = Session["userid"].ToString();
                    #endregion SQL Command Parameters
                    #region SQL Command Processing
                    int records = cmd.ExecuteNonQuery();
                    if (records == 1)
                    {
                        // We deleted the record
                        doexit = true;
                    }
                    else
                    {
                        // Something went fonky
                        doexit = false;
                    }
                    #endregion SQL Command Processing
                }
                #endregion SQL Command
            }
            if (doexit)
            {
                unlocked = true;
            }
            if (doerror)
            {
                err = true;
                msgLog += String.Format("<li>{0}</li>", "Error trying to remove lock");
            }
        }
        #endregion SQL Connection
        #endregion CMD Types
    }
    protected Database_Insert Ticket_Add_Simple(Int32 sp_campaignid, String sp_title, String sp_description, String sp_key)
    {
        Database_Insert rtrn = new Database_Insert();
        #region SQL Connection
        using (SqlConnection con = new SqlConnection(sqlStrDE))
        {
            ghFunctions.Donation_Open_Database(con);
            #region SQL Command
            using (SqlCommand cmd = new SqlCommand("", con))
            {
                #region Build cmdText
                String cmdText = "";
                cmdText = @"
	INSERT INTO [dataexchange_ticket].[dbo].[ticket]
		([campaignid]
		,[title]
		,[description]
		,[key]
		)
		VALUES
		(@sp_campaignid
		,@sp_title
		,@sp_description
		,@sp_key)

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
                cmd.Parameters.Add("@sp_campaignid", SqlDbType.Int).Value = sp_campaignid;
                cmd.Parameters.Add("@sp_title", SqlDbType.VarChar, 400).Value = sp_title;
                cmd.Parameters.Add("@sp_description", SqlDbType.VarChar, 4000).Value = sp_description;
                cmd.Parameters.Add("@sp_key", SqlDbType.VarChar, 64).Value = sp_key;
                #endregion SQL Command Parameters
                #region SQL Command Processing
                var chckResults = cmd.ExecuteScalar();
                if (chckResults != null && chckResults.ToString() != "0")
                {
                    // We inserted the ticket
                    rtrn.status = true;
                    rtrn.identity = Convert.ToInt32(chckResults.ToString());
                    rtrn.records = 1;
                    rtrn.response = "Success";
                }
                else
                {
                    // There was a problem inserting the ticket
                    rtrn.status = false;
                    rtrn.response = "Error";
                    rtrn.message = "Invalid Identity";
                }
                #endregion SQL Command Processing
            }
            #endregion SQL Command
        }
        #endregion SQL Connection
        return rtrn;
    }
    protected Database_Insert Ticket_Add_Date_Start(Int32 sp_ticketid, String datestart)
    {
        Database_Insert rtrn = new Database_Insert();
        #region SQL Connection
        using (SqlConnection con = new SqlConnection(sqlStrDE))
        {
            ghFunctions.Donation_Open_Database(con);
            if (datestart.Length > 0)
            {
                #region SQL Command
                using (SqlCommand cmd = new SqlCommand("", con))
                {
                    #region Build cmdText
                    String cmdText = "";
                    cmdText = @"
	INSERT INTO [dataexchange_ticket].[dbo].[ticket_date]
		([ticketid]
		,[typeid]
		,[date]
		,[status]
		)
		VALUES
		(@sp_ticketid
		,@sp_typeid
		,@sp_date
		,@sp_status)
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
                    cmd.Parameters.Add("@sp_ticketid", SqlDbType.Int).Value = sp_ticketid;
                    cmd.Parameters.Add("@sp_typeid", SqlDbType.Int).Value = 1010001; // Ticket Start
                    cmd.Parameters.Add("@sp_date", SqlDbType.DateTime).Value = datestart;
                    cmd.Parameters.Add("@sp_status", SqlDbType.Bit).Value = true;
                    #endregion SQL Command Parameters
                    #region SQL Command Processing
                    int records = cmd.ExecuteNonQuery();
                    if (records == 1)
                    {
                        // We inserted the ticket
                        rtrn.status = true;
                        rtrn.records = 1;
                        rtrn.response = "Success";
                    }
                    else
                    {
                        // There was a problem inserting the ticket
                        rtrn.status = false;
                        rtrn.response = "Error";
                        rtrn.message = "Problem inserting the Ticket Start Date";
                    }
                    #endregion SQL Command Processing
                }
                #endregion SQL Command
            }
        }
        #endregion SQL Connection
        return rtrn;
    }
    protected Database_Insert Ticket_Add_Date_End(Int32 sp_ticketid, String dateend)
    {
        Database_Insert rtrn = new Database_Insert();
        #region SQL Connection
        using (SqlConnection con = new SqlConnection(sqlStrDE))
        {
            ghFunctions.Donation_Open_Database(con);
            if (dateend.Length > 0)
            {
                #region SQL Command
                using (SqlCommand cmd = new SqlCommand("", con))
                {
                    #region Build cmdText
                    String cmdText = "";
                    cmdText = @"
	INSERT INTO [dataexchange_ticket].[dbo].[ticket_date]
		([ticketid]
		,[typeid]
		,[date]
		,[status]
		)
		VALUES
		(@sp_ticketid
		,@sp_typeid
		,@sp_date
		,@sp_status)
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
                    cmd.Parameters.Add("@sp_ticketid", SqlDbType.Int).Value = sp_ticketid;
                    cmd.Parameters.Add("@sp_typeid", SqlDbType.Int).Value = 1010002; // Ticket End
                    cmd.Parameters.Add("@sp_date", SqlDbType.DateTime).Value = dateend;
                    cmd.Parameters.Add("@sp_status", SqlDbType.Bit).Value = true;
                    #endregion SQL Command Parameters

                    #region SQL Command Processing
                    int records = cmd.ExecuteNonQuery();
                    if (records == 1)
                    {
                        // We inserted the ticket
                        rtrn.status = true;
                        rtrn.records = 1;
                        rtrn.response = "Success";
                    }
                    else
                    {
                        // There was a problem inserting the ticket
                        rtrn.status = false;
                        rtrn.response = "Error";
                        rtrn.message = "Problem inserting the Ticket Start Date";
                    }
                    #endregion SQL Command Processing
                }
                #endregion SQL Command
            }
        }
        #endregion SQL Connection
        return rtrn;
    }
    protected Database_Insert Ticket_Add_Item(Int32 sp_ticketid, Int32 sp_typeid, Int32 sp_itemid)
    {
        /// Will add or update the priority
        /// We need to do an associated log
        Database_Insert rtrn = new Database_Insert();
        rtrn.response = "Initiated";
        if (sp_typeid > 0)
        {
            #region SQL Connection
            using (SqlConnection con = new SqlConnection(sqlStrDE))
            {
                ghFunctions.Donation_Open_Database(con);
                bool doinsert = false;
                bool doupdate = false;
                #region SQL Command
                using (SqlCommand cmd = new SqlCommand("", con))
                {
                    #region Build cmdText
                    String cmdText = "";
                    cmdText = @"
IF EXISTS(SELECT TOP 1 1 FROM [dataexchange_ticket].[dbo].[ticket_item] [ti] WHERE [ti].[ticketid] = @sp_ticketid AND  [ti].[typeid] = @sp_typeid)
BEGIN
	IF EXISTS(SELECT TOP 1 1 FROM [dataexchange_ticket].[dbo].[ticket_item] [ti] WHERE [ti].[ticketid] = @sp_ticketid AND  [ti].[typeid] = @sp_typeid AND  [ti].[itemid] = @sp_itemid)
	BEGIN
		SELECT 'SAME' [result], 'No Update No Log' [message]
	END
	ELSE
	BEGIN
		SELECT 'UPDATE' [result],'Log the update' [message]
	END

END
ELSE
BEGIN
	SELECT 'INSERT' [result],'Log the insert' [message]
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
                    cmd.Parameters.Add("@sp_ticketid", SqlDbType.Int).Value = sp_ticketid;
                    cmd.Parameters.Add("@sp_typeid", SqlDbType.Int).Value = sp_typeid;
                    cmd.Parameters.Add("@sp_itemid", SqlDbType.Int).Value = sp_itemid;
                    cmd.Parameters.Add("@sp_authorid", SqlDbType.Int).Value = Session["userid"].ToString();
                    cmd.Parameters.Add("@sp_dateadded", SqlDbType.DateTime).Value = DateTime.UtcNow;
                    #endregion SQL Command Parameters
                    #region SQL Command Processing
                    var chckResults = cmd.ExecuteScalar();
                    if (chckResults != null && chckResults.ToString().Length > 0)
                    {
                        // We inserted the ticket
                        rtrn.status = true;
                        rtrn.records = 1;
                        rtrn.response = chckResults.ToString();
                        if (chckResults.ToString() == "UPDATE") { doupdate = true; }
                        else if (chckResults.ToString() == "INSERT") { doinsert = true; }
                    }
                    else
                    {
                        // There was a problem inserting the ticket
                        rtrn.status = false;
                        rtrn.response = "Error";
                        rtrn.message = "";
                    }
                    #endregion SQL Command Processing
                }
                #endregion SQL Command
                if (doinsert)
                {
                    #region SQL Command
                    using (SqlCommand cmd = new SqlCommand("", con))
                    {
                        #region Build cmdText
                        String cmdText = "";
                        cmdText = @"
INSERT INTO [dataexchange_ticket].[dbo].[ticket_item]
	([ticketid]
	,[typeid]
	,[itemid]
	,[authorid]
	,[dateadded]
	)
	VALUES
	(@sp_ticketid
	,@sp_typeid
	,@sp_itemid
	,@sp_authorid
	,@sp_dateadded)
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
                        cmd.Parameters.Add("@sp_ticketid", SqlDbType.Int).Value = sp_ticketid;
                        cmd.Parameters.Add("@sp_typeid", SqlDbType.Int).Value = sp_typeid;
                        cmd.Parameters.Add("@sp_itemid", SqlDbType.Int).Value = sp_itemid;
                        cmd.Parameters.Add("@sp_authorid", SqlDbType.Int).Value = Session["userid"].ToString();
                        cmd.Parameters.Add("@sp_dateadded", SqlDbType.DateTime).Value = DateTime.UtcNow;
                        #endregion SQL Command Parameters
                        #region SQL Command Processing
                        int cmdNon = cmd.ExecuteNonQuery();
                        if (cmdNon > 0)
                        {
                            // We inserted the ticket
                            rtrn.status = true;
                            rtrn.records = cmdNon;
                            rtrn.response = "Inserted";
                        }
                        else
                        {
                            // There was a problem inserting the ticket
                            rtrn.status = false;
                            rtrn.response = "Failed";
                            rtrn.message = "Failed to insert record";
                        }
                        #endregion SQL Command Processing
                    }
                    #endregion SQL Command
                }
                if (doupdate)
                {
                    #region SQL Command
                    using (SqlCommand cmd = new SqlCommand("", con))
                    {
                        #region Build cmdText
                        String cmdText = "";
                        cmdText = @"
UPDATE [dataexchange_ticket].[dbo].[ticket_item]
	SET [itemid] = @sp_itemid
	,[authorid] = @sp_authorid
	,[dateadded] = @sp_dateadded
WHERE [ticketid] = @sp_ticketid
AND  [typeid] = @sp_typeid
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
                        cmd.Parameters.Add("@sp_ticketid", SqlDbType.Int).Value = sp_ticketid;
                        cmd.Parameters.Add("@sp_typeid", SqlDbType.Int).Value = sp_typeid;
                        cmd.Parameters.Add("@sp_itemid", SqlDbType.Int).Value = sp_itemid;
                        cmd.Parameters.Add("@sp_authorid", SqlDbType.Int).Value = Session["userid"].ToString();
                        cmd.Parameters.Add("@sp_dateadded", SqlDbType.DateTime).Value = DateTime.UtcNow;
                        #endregion SQL Command Parameters
                        #region SQL Command Processing
                        int cmdNon = cmd.ExecuteNonQuery();
                        if (cmdNon > 0)
                        {
                            // We inserted the ticket
                            rtrn.status = true;
                            rtrn.records = cmdNon;
                            rtrn.response = "Updated";
                        }
                        else
                        {
                            // There was a problem inserting the ticket
                            rtrn.status = false;
                            rtrn.response = "Failed";
                            rtrn.message = "Failed to update record";
                        }
                        #endregion SQL Command Processing
                    }
                    #endregion SQL Command
                }

            }
            #endregion SQL Connection
        }
        return rtrn;
    }
    protected Database_Insert Ticket_Add_Comment(Int32 sp_typeid, Int32 sp_ticketid, Boolean sp_status, String sp_note)
    {
        Database_Insert rtrn = new Database_Insert();
        #region SQL Connection
        using (SqlConnection con = new SqlConnection(sqlStrDE))
        {
            ghFunctions.Donation_Open_Database(con);
            #region SQL Command
            using (SqlCommand cmd = new SqlCommand("", con))
            {
                #region Build cmdText
                String cmdText = "";
                cmdText = @"
INSERT INTO [dataexchange_ticket].[dbo].[ticket_note]
	([typeid]
	,[ticketid]
	,[authorid]
	,[status]
	,[note]
	,[dateadded]
	)
	VALUES
	(@sp_typeid
	,@sp_ticketid
	,@sp_authorid
	,@sp_status
	,@sp_note
	,@sp_dateadded)

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
                cmd.Parameters.Add("@sp_typeid", SqlDbType.Int).Value = sp_typeid;
                cmd.Parameters.Add("@sp_ticketid", SqlDbType.Int).Value = sp_ticketid;
                cmd.Parameters.Add("@sp_authorid", SqlDbType.Int).Value = Session["userid"].ToString();
                cmd.Parameters.Add("@sp_status", SqlDbType.Bit).Value = sp_status;
                cmd.Parameters.Add("@sp_note", SqlDbType.VarChar, 4000).Value = sp_note;
                cmd.Parameters.Add("@sp_dateadded", SqlDbType.DateTime).Value = DateTime.UtcNow;
                #endregion SQL Command Parameters
                #region SQL Command Processing
                var chckResults = cmd.ExecuteScalar();
                if (chckResults != null && chckResults.ToString() != "0")
                {
                    // We inserted the ticket
                    rtrn.status = true;
                    rtrn.identity = Convert.ToInt32(chckResults.ToString());
                    rtrn.records = 1;
                    rtrn.response = "Success";
                }
                else
                {
                    // There was a problem inserting the ticket
                    rtrn.status = false;
                    rtrn.response = "Error";
                    rtrn.message = "Invalid Identity";
                }
                #endregion SQL Command Processing
            }
            #endregion SQL Command
        }
        #endregion SQL Connection
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
    protected void Cell_Format_01(IXLWorksheet ws, Int32 row, Int32 col, Label lbl, Label lbl2, Label lbl3)
    {
        // ws.Cell(row, col)
        ws.Cell(row, col).Value = lbl.Text;
        ws.Cell(row, col).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        ws.Cell(row, col).Style.Border.OutsideBorderColor = XLColor.DarkGray;
        ws.Cell(row, col + 1).Value = "%";
        ws.Cell(row, col + 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        ws.Cell(row, col + 1).Style.Border.OutsideBorderColor = XLColor.DarkGray;
        ws.Range(row, col, row, col + 1).Style.Font.Bold = true;
        ws.Range(row, col, row, col + 1).Style.Fill.BackgroundColor = XLColor.LightGray;
        row++;
        ws.Cell(row, col).Value = lbl2.Text;
        ws.Cell(row, col).Style.NumberFormat.Format = "#,##0";
        ws.Cell(row, col).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        ws.Cell(row, col).Style.Border.OutsideBorderColor = XLColor.DarkGray;
        var num = decimal.Parse(lbl3.Text.TrimEnd(new char[] { '%', ' ' })) / 100M;
        ws.Cell(row, col + 1).Value = num;
        ws.Cell(row, col + 1).Style.NumberFormat.Format = "0%";
        ws.Cell(row, col + 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        ws.Cell(row, col + 1).Style.Border.OutsideBorderColor = XLColor.DarkGray;
        ws.Range(row, col, row, col + 1).Style.Fill.BackgroundColor = XLColor.WhiteSmoke;

    }
    protected void Cell_Format_02(IXLWorksheet ws, Int32 row, Int32 col, Label lbl, Label lbl2)
    {
        // ws.Cell(row, col)
        ws.Cell(row, col).Value = lbl.Text;
        ws.Range(row, col, row, col + 1).Merge();
        ws.Range(row, col, row, col + 1).Style.Font.Bold = true;
        ws.Range(row, col, row, col + 1).Style.Fill.BackgroundColor = XLColor.LightGray;
        ws.Range(row, col, row, col + 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        ws.Range(row, col, row, col + 1).Style.Border.OutsideBorderColor = XLColor.DarkGray;

        row++;
        ws.Cell(row, col).Value = lbl2.Text;
        ws.Range(row, col, row, col + 1).Merge();
        ws.Range(row, col, row, col + 1).Style.NumberFormat.Format = "HH:mm:ss";
        ws.Range(row, col, row, col + 1).Style.Font.Bold = true;
        ws.Range(row, col, row, col + 1).Style.Fill.BackgroundColor = XLColor.WhiteSmoke;
        ws.Range(row, col, row, col + 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        ws.Range(row, col, row, col + 1).Style.Border.OutsideBorderColor = XLColor.DarkGray;
    }

}