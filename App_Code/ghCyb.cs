using CyberSource.Clients;
//using CyberSource.Clients.SoapWebReference;
using CyberSource.Clients.SoapServiceReference;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Web;

/// <summary>
/// ARC Do Refund
/// Will perform a refund
/// Currently only Cybersource is supported
/// </summary>
public class ghCyb
{
    public ghCyb()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    static private String sqlStr = Connection.GetConnectionString("Default", ""); // Controlled by Web Config ARC_Live | ARC_Stage
    /// <summary>
    /// Perform a Follow On Refund
    /// This is based on the user selecting this method
    /// We still validate it; but if the validation fails we just let the user know
    /// </summary>
    /// <param name="cybid">Cybersource Log Auth ID</param>
    /// <param name="amount">Refund Amount</param>
    /// <param name="reason">Refund Reason ID</param>
    /// <param name="reasonnote">Refund Reason Note</param>
    /// <returns></returns>
    #region Processing - Refunds
    static public cybProcess cybRefundFollowOn(int cybid, double amount, string reason, string reasonnotes, System.Web.UI.WebControls.Label lbl)
    {
        cybProcess cybProc = new cybProcess();
        ARC_Cybersource_Log_Refund arcRecord = new ARC_Cybersource_Log_Refund();
        arcRecord.CBAuthID = cybid;
        arcRecord.RefundReason = reason;
        arcRecord.RefundReasonNotes = reasonnotes;

        cybProc.message = "processing...";
        
        #region Process Refund
        try
        {
            /// <summary>
            /// Follow-On transactions only need the RequestID, Token, and Amount of the refund
            /// </summary
            /// 
            bool doRefund = false;
            int callid = 0;
            #region Initiliaze
            RequestMessage request = new RequestMessage();
            #endregion Initiliaze
            #region Get the SQL Record
            #region SqlConnection
            using (SqlConnection con = new SqlConnection(sqlStr))
            {
                #region SqlCommand cmd
                using (SqlCommand cmd = new SqlCommand("", con))
                {
                    #region Populate the SQL Command
                    cmd.CommandTimeout = 600;
                    #region Build cmdText
                    String cmdText = "";
                    cmdText += @"
-- Did this for recurring transactions which are linked differently
IF EXISTS(
		SELECT TOP 1 1
		FROM [dbo].[cybersource_log_auth] [cb] WITH(NOLOCK)
		JOIN [dbo].[donationccinfo] [di] WITH(NOLOCK) ON [di].[id] = [cb].[externalid]
		JOIN [dbo].[callinfo] [ci] WITH(NOLOCK) ON [ci].[callid] = [di].[callid]
		WHERE 1=1
		AND [cb].[id] = @sp_cybid
)
BEGIN
    SELECT
    [cb].[id]
    ,[cb].[status]
    ,[cb].[createdate]
    ,[cb].[requestid]
    ,[cb].[requesttoken]
    ,[cb].[merchantreferencecode]
    ,[di].[callid]
    ,[cb].[externalid]
    ,[di].[donationamount] [amount]
    ,(SELECT SUM([cr].[cccreditreply_amount]) FROM [dbo].[cybersource_log_refund] [cr] WITH(NOLOCK) WHERE [cr].[externalid] = [di].[id] AND [cr].[reasoncode] = '100') [amount_ref]

    ,[di].[ccnum]
    ,[di].[ccexpmonth]
    ,[di].[ccexpyear]
    ,[ci].[fname]
    ,[ci].[lname]
    ,[ci].[address]
    ,[ci].[suitenumber]
    ,[ci].[zip]
    ,[ci].[city]
    ,[ci].[state]

    ,[cb].[source]
    ,[cb].[decision]
    ,[cb].[reasoncode]
    ,[cb].[ccauthreply_amount]
    ,[cb].[cccapturereply_amount]
    ,[cb].[cccontent]
    FROM [dbo].[cybersource_log_auth] [cb] WITH(NOLOCK)
    JOIN [dbo].[donationccinfo] [di] WITH(NOLOCK) ON [di].[id] = [cb].[externalid]
    JOIN [dbo].[callinfo] [ci] WITH(NOLOCK) ON [ci].[callid] = [di].[callid]
    WHERE 1=1
    AND [cb].[id] = @sp_cybid
END
ELSE
BEGIN

    SELECT
    [cb].[id]
    ,[cb].[status]
    ,[cb].[createdate]
    ,[cb].[requestid]
    ,[cb].[requesttoken]
    ,[cb].[merchantreferencecode]
    ,[di].[callid]
    ,[cb].[externalid]
    ,[di].[donationamount] [amount]
    ,(SELECT SUM([cr].[cccreditreply_amount]) FROM [dbo].[cybersource_log_refund] [cr] WITH(NOLOCK) WHERE [cr].[externalid] = [di].[id] AND [cr].[reasoncode] = '100') [amount_ref]

    ,[di].[ccnum]
    ,[di].[ccexpmonth]
    ,[di].[ccexpyear]
    ,[ci].[fname]
    ,[ci].[lname]
    ,[ci].[address]
    ,[ci].[suitenumber]
    ,[ci].[zip]
    ,[ci].[city]
    ,[ci].[state]

    ,[cb].[source]
    ,[cb].[decision]
    ,[cb].[reasoncode]
    ,[cb].[ccauthreply_amount]
    ,[cb].[cccapturereply_amount]
    ,[cb].[cccontent]
    FROM [dbo].[cybersource_log_auth] [cb] WITH(NOLOCK)
	JOIN [dbo].[donation_recurring_log] [drl] WITH(NOLOCK) ON [drl].[recurringid] = [cb].[externalid]
    JOIN [dbo].[donationccinfo] [di] WITH(NOLOCK) ON [di].[id] = [drl].[donationid]
    JOIN [dbo].[callinfo] [ci] WITH(NOLOCK) ON [ci].[callid] = [di].[callid]
    WHERE 1=1
    AND [cb].[id] = @sp_cybid
END
";
                    #endregion Build cmdText
                    cmd.CommandText = cmdText;
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Clear();
                    #endregion Populate the SQL Command
                    #region Populate the SQL Params
                    cmd.Parameters.Add(new SqlParameter("@sp_cybid", cybid));
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
                                    // Ensure we have a valid record for [Follow On]
                                    //cybProc.message = "sql record..." + sqlRdr["status"].ToString();
                                    DateTime dtChargeDate;
                                    DateTime.TryParse(sqlRdr["createdate"].ToString(), out dtChargeDate);
                                    Int32 foLimit = 59; // Days that a Follow On Credit can be performed
                                    callid = Convert.ToInt32(sqlRdr["callid"].ToString());
                                    arcRecord.ExternalID = Convert.ToInt32(sqlRdr["externalid"].ToString());
                                    if (sqlRdr["status"].ToString() == "Settled")
                                    {
                                        // We're good, don't do anything?
                                    }
                                    else if (sqlRdr["status"].ToString() == "Cancelled" && sqlRdr["decision"].ToString() == "ACCEPT")
                                    {
                                        // We're good, don't do anything?
                                    }
                                    else if (sqlRdr["status"].ToString() == "Refunded")
                                    {
                                        // We did this validation, just need to do it again because AGENTS
                                        double dvAmount = 0;
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
                                                    if (dvAmount >= amount)
                                                    {
                                                        doRefund = true;
                                                    }
                                                }
                                            }
                                        }
                                        if (!doRefund)
                                        {
                                            throw new Exception("Invalid Donation/Refund Amounts");
                                        }
                                    }
                                    else
                                    {
                                        throw new Exception("Invalid Donation Status");
                                    }
                                    if (dtChargeDate != null && (DateTime.UtcNow - dtChargeDate).TotalDays < foLimit)
                                    {
                                        // We're good, don't do anything?
                                    }
                                    else
                                    {
                                        cybProc.message = "Error:";
                                        cybProc.message += "<br />cybid: " + cybid.ToString();
                                        cybProc.message += "<br />Charge Date: " + dtChargeDate.ToString();
                                        cybProc.message += "<br />";
                                        doRefund = false;
                                        throw new Exception("Invalid Donation Date");
                                    }
                                    // Add the required data to the request
                                    request.ccCreditService = new CCCreditService();
                                    request.ccCreditService.run = "true";

                                    request.ccCreditService.captureRequestID = sqlRdr["requestid"].ToString().Trim();
                                    request.ccCreditService.captureRequestToken = sqlRdr["requesttoken"].ToString().Trim();
                                    request.merchantReferenceCode = sqlRdr["merchantreferencecode"].ToString().Trim();

                                    PurchaseTotals purchaseTotals = new PurchaseTotals();
                                    purchaseTotals.currency = "USD";
                                    purchaseTotals.grandTotalAmount = amount.ToString();
                                    request.purchaseTotals = purchaseTotals;

                                    doRefund = true;
                                }
                            }
                            else
                            {
                                cybProc.message = "sql No records...";
                            }
                        }
                    }
                    #endregion Process SQL Command - Try
                    #region Process SQL Command - Catch
                    catch (Exception ex)
                    {
                        cybProc.message += "sql error";
                        ErrorLog.ErrorLog_Display(ex, "DoRefund - Follow On", lbl);

                    }
                    #endregion Process SQL Command - Catch
                }
                #endregion SqlCommand cmd
            }
            #endregion SqlConnection

            #endregion Get the SQL Record
            #region Processing CyberSource Attempt
            if (doRefund)
            {
                //ReplyMessage reply = SoapClient.RunTransaction(request);
                //string template = GetTemplate(reply.decision.ToUpper());
                //string content = GetContent(reply);
                Process_Record_Refund(arcRecord, request, lbl);
                cybProc.status = arcRecord.Status;
                cybProc.message += "<br />process complete";
            }
            else
            {

            }
            #endregion Processing CyberSource Attempt
        }
        catch (Exception ex)
        {
            cybProc.message = "error";
            ErrorLog.ErrorLog_Display(ex, "DoRefund - Follow On", lbl);
        }
        finally
        {
            //InstantEmail_RefundReport();
        }
        #endregion


        return cybProc;
    }
    static private void Process_Record_Refund(ARC_Cybersource_Log_Refund arcRecord, RequestMessage request, System.Web.UI.WebControls.Label lbl)
    {
        #region Process Refund
        try
        {
            ReplyMessage reply = SoapClient.RunTransaction(request);

            string template = GetTemplate(reply.decision.ToUpper());
            string content = GetContent(reply);
            arcRecord.ccContent = content;

            if (reply.decision == "ACCEPT") arcRecord.Status = "Refunded";
            else if (reply.decision == "REJECT") arcRecord.Status = "Rejected";
            else arcRecord.Status = "Error";


            arcRecord.decision = reply.decision;
            arcRecord.merchantReferenceCode = reply.merchantReferenceCode;
            arcRecord.reasonCode = reply.reasonCode;
            arcRecord.requestID = reply.requestID;
            arcRecord.requestToken = reply.requestToken;
            if (reply.ccCreditReply != null)
            {
                arcRecord.ccCreditReply_amount = (reply.ccCreditReply.amount != null) ? arcRecord.ccCreditReply_amount = reply.ccCreditReply.amount : "0";
                arcRecord.ccCreditReply_reasonCode = (reply.ccCreditReply.reasonCode != null) ? reply.ccCreditReply.reasonCode : "";
                arcRecord.ccCreditReply_reconciliationID = (reply.ccCreditReply.reconciliationID != null) ? reply.ccCreditReply.reconciliationID : "";
                arcRecord.ccCreditReply_requestDateTime = (reply.ccCreditReply.requestDateTime != null) ? reply.ccCreditReply.requestDateTime.Replace("T", " ").Replace("Z", "") : "";
            }
            arcRecord.msgProcess = "success";            
        }
        catch (Exception ex)
        {
            ErrorLog.ErrorLog_Display(ex, "Refund Processing - Record Process", lbl);
            arcRecord.Status = "error";
            arcRecord.msgProcess = "Process_Record_Refund - error<br />" + ex.Message;
        }
        finally
        {
            // Try to save even if we failed?
            Save_Record_Refund(arcRecord, lbl);
        }
        #endregion Process Refund
    }
    static private void Save_Record_Refund(ARC_Cybersource_Log_Refund arcRecord, System.Web.UI.WebControls.Label lbl)
    {
        #region Processing Start - SQL - Try
        string cmdText = "";
        try
        {
            #region SqlConnection
            String sqlStr = Connection.GetConnectionString("Default", ""); // Controlled by Web Config ARC_Live | ARC_Stage
            using (SqlConnection con = new SqlConnection(sqlStr))
            {
                #region SqlCommand cmd
                using (SqlCommand cmd = new SqlCommand("", con))
                {
                    #region Populate the SQL Command
                    cmd.CommandTimeout = 600;
                    cmd.CommandText = "[dbo].[sp_cybersource_refund]";
                    cmd.CommandType = CommandType.StoredProcedure;
                    #endregion Populate the SQL Command
                    #region Populate the SQL Params
                    cmd.Parameters.Add(new SqlParameter("@Source", "Web"));
                    cmd.Parameters.Add(new SqlParameter("@CBAuthID", arcRecord.CBAuthID));
                    cmd.Parameters.Add(new SqlParameter("@ExternalID", arcRecord.ExternalID));
                    cmd.Parameters.Add(new SqlParameter("@User", HttpContext.Current.User.Identity.Name));
                    cmd.Parameters.Add(new SqlParameter("@Reason", arcRecord.RefundReason));
                    cmd.Parameters.Add(new SqlParameter("@ReasonNotes", arcRecord.RefundReasonNotes));                    
                    cmd.Parameters.Add(new SqlParameter("@Status", arcRecord.Status));
                    cmd.Parameters.Add(new SqlParameter("@CreateDate", arcRecord.CreateDate));

                    cmd.Parameters.Add(new SqlParameter("@decision", arcRecord.decision));
                    cmd.Parameters.Add(new SqlParameter("@merchantReferenceCode", arcRecord.merchantReferenceCode));
                    cmd.Parameters.Add(new SqlParameter("@reasonCode", arcRecord.reasonCode));
                    cmd.Parameters.Add(new SqlParameter("@requestID", arcRecord.requestID));
                    cmd.Parameters.Add(new SqlParameter("@requestToken", arcRecord.requestToken));

                    cmd.Parameters.Add(new SqlParameter("@ccAuthReversalReply_amount", arcRecord.ccAuthReversalReply_amount));
                    cmd.Parameters.Add(new SqlParameter("@ccAuthReversalReply_authorizationCode", arcRecord.ccAuthReversalReply_authorizationCode));
                    cmd.Parameters.Add(new SqlParameter("@ccAuthReversalReply_processorResponse", arcRecord.ccAuthReversalReply_processorResponse));
                    cmd.Parameters.Add(new SqlParameter("@ccAuthReversalReply_reasonCode", arcRecord.ccAuthReversalReply_reasonCode));
                    cmd.Parameters.Add(new SqlParameter("@ccAuthReversalReply_requestDateTime", arcRecord.ccAuthReversalReply_requestDateTime));
                    cmd.Parameters.Add(new SqlParameter("@ccCreditReply_amount", arcRecord.ccCreditReply_amount));
                    cmd.Parameters.Add(new SqlParameter("@ccCreditReply_ownerMerchantID", arcRecord.ccCreditReply_ownerMerchantID));
                    cmd.Parameters.Add(new SqlParameter("@ccCreditReply_reasonCode", arcRecord.ccCreditReply_reasonCode));
                    cmd.Parameters.Add(new SqlParameter("@ccCreditReply_reconciliationID", arcRecord.ccCreditReply_reconciliationID));
                    cmd.Parameters.Add(new SqlParameter("@ccCreditReply_requestDateTime", arcRecord.ccCreditReply_requestDateTime));
                    cmd.Parameters.Add(new SqlParameter("@voidReply_amount", arcRecord.voidReply_amount));
                    cmd.Parameters.Add(new SqlParameter("@voidReply_currency", arcRecord.voidReply_currency));
                    cmd.Parameters.Add(new SqlParameter("@voidReply_reasonCode", arcRecord.voidReply_reasonCode));
                    cmd.Parameters.Add(new SqlParameter("@voidReply_requestDateTime", arcRecord.voidReply_requestDateTime));

                    cmd.Parameters.Add(new SqlParameter("@ccContent", arcRecord.ccContent));
                    cmdText = "\n" + cmd.CommandText;
                    bool cmdFirst = true;
                    foreach (SqlParameter param in cmd.Parameters)
                    {
                        cmdText += "\n" + ((cmdFirst) ? "" : ",") + param.ParameterName + " = " + ((param.Value != null) ? "'" + param.Value.ToString() + "'" : "default");
                        cmdFirst = false;
                    }
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
                                    //arcNewID = sqlRdr["Response"].ToString();
                                    //rplResponse.Text = "Record Updated: " + sqlRdr[0].ToString();
                                }
                            }
                            else
                            {
                                //arcNewID = 0;
                            }
                        }
                    }
                    #endregion Process SQL Command - Try
                    #region Process SQL Command - Catch
                    catch (Exception ex)
                    {
                        ErrorLog.ErrorLog_Display(ex, "Refund Processing - Record Save", lbl);
                        arcRecord.msgSave = "Process_Record_Refund - error";
                    }
                    #endregion Process SQL Command - Catch
                }
                #endregion SqlCommand cmd
            }
            #endregion SqlConnection
        }
        #endregion Processing Start - SQL - Try
        #region Processing Start - SQL - Catch
        catch (Exception ex)
        {
            ErrorLog.ErrorLog_Display(ex, "DoRefund - Follow On", lbl);
        }
        #endregion Processing Start - SQL - Catch
    }
    #endregion Processing - Refunds
    #region Cybersource - Templates
    private static string GetTemplate(string decision)
    {
        // Retrieves the text that corresponds to the decision.
        if ("ACCEPT".Equals(decision))
        {
            return ("The order succeeded. {0}");
        }
        if ("REJECT".Equals(decision))
        {
            return ("Your order was not approved. {0}");
        }
        // ERROR, or an unknown decision
        return ("Your order could not be completed at this time. {0}" + "Please try again later.");
    }
    private static string GetContent(ReplyMessage reply)
    {
        /*
         * This is where you retrieve the content that will be plugged
         * into the template.
         * 
         * The strings returned in this sample are mostly to demonstrate
         * how to retrieve the reply fields.  Your application should
         * display user-friendly messages.
         */

        int reasonCode = int.Parse(reply.reasonCode);
        switch (reasonCode)
        {
            // Success
            case 100:
                return ("Approved");
            //"\nRequest ID: " + reply.requestID +
            //"\nAuthorization Code: " +
            //    reply.ccAuthReply.authorizationCode +
            //"\nCapture Request Time: " +
            //    reply.ccCaptureReply.requestDateTime +
            //"\nCaptured Amount: " +
            //    reply.ccCaptureReply.amount);

            // Missing field(s)
            case 101:
                return (
                    "The following required field(s) are missing: " +
                    EnumerateValues(reply.missingField));

            // Invalid field(s)
            case 102:
                return (
                    "The following field(s) are invalid: " +
                    EnumerateValues(reply.invalidField));

            // Insufficient funds
            case 204:
                return (
                    "Insufficient funds in the account.  Please use a " +
                    "different card or select another form of payment.");

            // add additional reason codes here that you need to handle
            // specifically.

            default:
                // For all other reason codes, return an empty string,
                // in which case, the template will be displayed with no
                // specific content.
                return (String.Empty);
        }
    }
    private static string EnumerateValues(string[] array)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        foreach (string val in array)
        {
            sb.Append(val + "");
        }

        return (sb.ToString());
    }
    #endregion Cybersource - Templates
    #region Processing - Charge
    public sealed class ARC_Cybersource_Charge
    {
        // Class for processing the charge
        public String source;
        public Int32 callid; // reconciliationID
        public Int32 donationid; // reconciliationID
        public String orderid; // merchantReferenceCode
        public String billto_firstname; // billTo.firstName
        public String billto_lastname; // billTo.lastName
        public String billto_streeet1; // billTo.street1
        public String billto_zip; // billTo.postalCode
        public String billto_city; // billTo.city
        public String billto_state; // billTo.state
        public String billto_country; // billTo.country
        public String billto_email; // billTo.email
        /// If valid Email
        /// if (tb8_email.Text.Trim().Length > 5 && tb8_email.Text.Trim().Contains("@") && tb8_email.Text.Trim().Contains("."))
        /// {
        ///     billTo.email = donor.; // tb8_email.Text.Trim();
        /// }
        /// else { billTo.email = "nobody@cybersource.com"; }
        public String card_number; // card.accountNumber
        public String card_month; // card.expirationMonth
        public String card_year; // card.expirationYear
        public Double amount; // item.unitPrice
        /// Sustainer | "RD001" | "ARC Sustainer"
        /// IVR | "DN001" | "ARC Call"
        /// WEB | "DN001" | "ARC Agent Script Donation"
        public String product_sku; // item.productSKU
        public String product_name; // item.productName
    }
    static public cybProcess cybCharge(ARC_Cybersource_Charge donor)
    {
        cybProcess cybProc = new cybProcess();
        cybProc.status = "START";
        cybProc.message = "begin processing...";
        
        #region Insert: CYBERSOURCE
        Boolean doCyberSource = true; // [false] Prevents CS from processing (DeBug)
        Boolean donationApproved = false; // Determine if this is the proper place for this, it is used to determine success/failure
        String ResponseSQL = ""; // Determien proper place for this, it was a label
        String cdChargeStatus = ""; // Determien proper place for this, it was a label
        
        #region CS Request Creation and Type
        RequestMessage request = new RequestMessage();
        request.ccAuthService = new CCAuthService();
        request.ccAuthService.run = "true";
        request.ccCaptureService = new CCCaptureService();
        request.ccCaptureService.run = "true";
        #endregion CS Request Creation and Type
        #region CS Reconcilliation ID
        /// Reconcilliation ID from ExternalID / DonationCCInfo.ID
        string reconciliationID = donor.donationid.ToString(); // sp_donationccinfoid.ToString();
        // Padding Not Used [Used in IVR/Recurring Services ???]
        //int pad = 16; // 9 for AmEx, 16 for others
        //if (sp_ccnum.StartsWith("3")) { pad = 9; }
        //reconciliationID = reconciliationID.PadRight(pad, '0');
        request.ccAuthService.reconciliationID = reconciliationID;
        request.ccCaptureService.reconciliationID = reconciliationID;
        request.merchantReferenceCode = donor.orderid; // sp_orderid = sp_donationccinfoid.ToString().PadLeft(14, '0');;
        #endregion CS Reconcilliation ID
        #region CS billTo
        /// We need to enter default data if non is supplied
        /// We also need to parse the Zip Code against the Zip database
        BillTo billTo = new BillTo();
        billTo.firstName = donor.billto_firstname; // tb7_first_name.Text.Trim();
        billTo.lastName = donor.billto_lastname; // tb7_last_name.Text.Trim();
        billTo.street1 = donor.billto_streeet1; // tb8_address1.Text.Trim();
        billTo.postalCode = donor.billto_zip; // tb8_postal_code.Text.Trim();
        billTo.city = donor.billto_city; // tb8_city.Text.Trim();
        billTo.state = donor.billto_state; // sp_state; // tb8_state.SelectedValue;
        billTo.country = donor.billto_country; // sp_country; // "US";
        billTo.email = donor.billto_email; // tb8_email.Text.Trim()
        request.billTo = billTo;
        #endregion CS billTo
        #region CS Card
        Card card = new Card();
        card.accountNumber = donor.card_number; // sp_ccnum;
        card.expirationMonth = donor.card_month; // tb7_card_month.SelectedValue;
        card.expirationYear = donor.card_year; // tb7_card_year.SelectedValue;
        request.card = card;
        #endregion CS Card
        #region CS Item / Amount
        PurchaseTotals purchaseTotals = new PurchaseTotals();
        purchaseTotals.currency = "USD";
        request.purchaseTotals = purchaseTotals;
        request.item = new Item[1];
        Item item = new Item();
        item.id = "0";
        item.unitPrice = donor.amount.ToString(); // sp_donationamount.ToString();
        item.productSKU = donor.product_sku; //"DN001";
        item.productName = donor.product_name; //"ARC Agent Script Donation";
        request.item[0] = item;
        #endregion CS Item / Amount
        #region CS Process / Reply
        ARC_Cybersource_Log_Auth arcRecord = new ARC_Cybersource_Log_Auth();
        arcRecord.ExternalID = donor.donationid.ToString(); // sp_donationccinfoid.ToString();
        if (doCyberSource)
        {
            try
            {
                ReplyMessage reply = SoapClient.RunTransaction(request);
                string template = GetTemplate(reply.decision.ToUpper());
                string content = "";
                try { content = GetContent(reply); }
                catch { content = "error"; }
                //Log(logRecord + ",CB: " + String.Format(template, content), "record");
                #region Populate the ARC Record
                if (reply.decision == "ACCEPT") { arcRecord.Status = "Settled"; donationApproved = true; }
                // Change me before launching Monday !!!!
                //else if (reply.decision == "REJECT" && sp_ccnum == "4111111111111111x" && tglMode == "Stage") { arcRecord.Status = "Settled"; donationApproved = true; }
                else if (reply.decision == "REJECT") { arcRecord.Status = "Declined"; donationApproved = false; }
                else { arcRecord.Status = "Error"; donationApproved = false; }
                

                ResponseSQL += "<br /><b>CS Status: " + arcRecord.Status + "</b>";

                arcRecord.ccContent = content;
                arcRecord.decision = reply.decision;
                arcRecord.merchantReferenceCode = reply.merchantReferenceCode;
                try
                {
                    arcRecord.reasonCode = Convert.ToInt32(reply.reasonCode);
                }
                catch { }
                arcRecord.requestID = reply.requestID;
                arcRecord.requestToken = reply.requestToken;
                #region reply.ccAuthReply
                if (reply.ccAuthReply != null)
                {
                    arcRecord.ccAuthReply_accountBalance = reply.ccAuthReply.accountBalance;
                    //arcRecord.ccAuthReply_accountBalanceCurrency = String.Empty;
                    //arcRecord.ccAuthReply_accountBalanceSign = String.Empty;
                    arcRecord.ccAuthReply_amount = reply.ccAuthReply.amount;
                    arcRecord.ccAuthReply_authFactorCode = reply.ccAuthReply.authFactorCode;
                    arcRecord.ccAuthReply_authorizationCode = reply.ccAuthReply.authorizationCode;
                    if (reply.ccAuthReply.authorizedDateTime != null)
                    {
                        arcRecord.ccAuthReply_authorizedDateTime = reply.ccAuthReply.authorizedDateTime.Replace("T", " ").Replace("Z", "");
                    }
                    arcRecord.ccAuthReply_avsCode = reply.ccAuthReply.avsCode;
                    arcRecord.ccAuthReply_avsCodeRaw = reply.ccAuthReply.avsCodeRaw;
                    //arcRecord.ccAuthReply_cardCategory = String.Empty;
                    arcRecord.ccAuthReply_cavvResponseCode = reply.ccAuthReply.cavvResponseCode;
                    arcRecord.ccAuthReply_cavvResponseCodeRaw = reply.ccAuthReply.cavvResponseCodeRaw;
                    arcRecord.ccAuthReply_cvCode = reply.ccAuthReply.cvCode;
                    arcRecord.ccAuthReply_cvCodeRaw = reply.ccAuthReply.cvCodeRaw;
                    arcRecord.ccAuthReply_merchantAdviceCode = reply.ccAuthReply.merchantAdviceCode;
                    arcRecord.ccAuthReply_merchantAdviceCodeRaw = reply.ccAuthReply.merchantAdviceCodeRaw;
                    //arcRecord.ccAuthReply_ownerMerchantID = String.Empty;
                    //arcRecord.ccAuthReply_paymentNetworkTransactionID = String.Empty;
                    arcRecord.ccAuthReply_processorResponse = reply.ccAuthReply.processorResponse;
                    try
                    {
                        arcRecord.ccAuthReply_reasonCode = Convert.ToInt32(reply.ccAuthReply.reasonCode);
                    }
                    catch { }
                    arcRecord.ccAuthReply_reconciliationID = reply.ccAuthReply.reconciliationID;
                    arcRecord.ccAuthReply_referralResponseNumber = String.Empty;
                    arcRecord.ccAuthReply_requestAmount = donor.amount.ToString(); // sp_donationamount.ToString();
                    arcRecord.ccAuthReply_requestCurrency = String.Empty;
                }
                #endregion reply.ccAuthReply
                #region reply.ccCaptureReply
                if (reply.ccCaptureReply != null)
                {
                    arcRecord.ccCaptureReply_amount = reply.ccCaptureReply.amount;
                    try
                    {
                        arcRecord.ccCaptureReply_reasonCode = Convert.ToInt32(reply.ccCaptureReply.reasonCode);
                    }
                    catch { }
                    arcRecord.ccCaptureReply_reconciliationID = reply.ccCaptureReply.reconciliationID;
                    arcRecord.ccCaptureReply_requestDateTime = reply.ccCaptureReply.requestDateTime.Replace("T", " ").Replace("Z", "");
                }
                #endregion reply.ccCaptureReply

                #endregion Populate the ARC Record
                cdChargeStatus = arcRecord.Status;
                cybProc.status = arcRecord.Status;
            }
            catch (Exception ex)
            {
                // Depending on the type of error, the user may be able to re-try, or this may be a fatal failure
                cybProc.status = "ERROR";
                cybProc.message = "cybCharge - Catch - doCyberSource";
                cybProc.lblmessage = ErrorLog.ErrorLog_Display_String(ex, "Error: Processing Donation 002");
            }
        }
        else
        {
            // Declined - Not Processed
            donationApproved = false;
            arcRecord.Status = "Declined";
            cdChargeStatus = arcRecord.Status;
            cybProc.status = arcRecord.Status;
        }
        #endregion CS Process / Reply
        #region CS Insert SQL
        #region Save the record to SQL
        if (arcRecord.Status != null)
        {
            //arcRecord.Source = "PORTAL";
            //arcRecord.Source = "WEB"; // Get this from the record type
            //arcRecord.Source = "IVR";
            //arcRecord.Source = "RECURRING";
            arcRecord.Source = donor.source;
            ARC_Cybersource_To_SQL(arcRecord, cybProc);
        }
        #endregion Save the record to SQL
        #endregion CS Insert SQL
        #endregion Insert: CYBERSOURCE
        return cybProc;
    }
    static private void ARC_Cybersource_To_SQL(ARC_Cybersource_Log_Auth arcRecord, cybProcess cybProc)
    {
        #region Processing Start - SQL - Try
        try
        {
            #region SqlConnection
            using (SqlConnection con = new SqlConnection(sqlStr))
            {
                #region SqlCommand cmd
                using (SqlCommand cmd = new SqlCommand("", con))
                {
                    #region Populate the SQL Command
                    cmd.CommandTimeout = 600;
                    cmd.CommandText = "[dbo].[recurring_records_add_cybersource]";
                    cmd.CommandType = CommandType.StoredProcedure;
                    #endregion Populate the SQL Command
                    #region Populate the SQL Params
                    cmd.Parameters.Add(new SqlParameter("@Source", arcRecord.Source));
                    cmd.Parameters.Add(new SqlParameter("@ExternalID", arcRecord.ExternalID));
                    cmd.Parameters.Add(new SqlParameter("@Status", arcRecord.Status));
                    cmd.Parameters.Add(new SqlParameter("@CreateDate", arcRecord.CreateDate));

                    cmd.Parameters.Add(new SqlParameter("@decision", arcRecord.decision));
                    cmd.Parameters.Add(new SqlParameter("@merchantReferenceCode", arcRecord.merchantReferenceCode));
                    cmd.Parameters.Add(new SqlParameter("@reasonCode", arcRecord.reasonCode));
                    cmd.Parameters.Add(new SqlParameter("@requestID", arcRecord.requestID));
                    cmd.Parameters.Add(new SqlParameter("@requestToken", arcRecord.requestToken));

                    cmd.Parameters.Add(new SqlParameter("@ccAuthReply_accountBalance", arcRecord.ccAuthReply_accountBalance));
                    cmd.Parameters.Add(new SqlParameter("@ccAuthReply_accountBalanceCurrency", arcRecord.ccAuthReply_accountBalanceCurrency));
                    cmd.Parameters.Add(new SqlParameter("@ccAuthReply_accountBalanceSign", arcRecord.ccAuthReply_accountBalanceSign));
                    cmd.Parameters.Add(new SqlParameter("@ccAuthReply_amount", arcRecord.ccAuthReply_amount));
                    cmd.Parameters.Add(new SqlParameter("@ccAuthReply_authFactorCode", arcRecord.ccAuthReply_authFactorCode));
                    cmd.Parameters.Add(new SqlParameter("@ccAuthReply_authorizationCode", arcRecord.ccAuthReply_authorizationCode));
                    cmd.Parameters.Add(new SqlParameter("@ccAuthReply_authorizedDateTime", arcRecord.ccAuthReply_authorizedDateTime));
                    cmd.Parameters.Add(new SqlParameter("@ccAuthReply_avsCode", arcRecord.ccAuthReply_avsCode));
                    cmd.Parameters.Add(new SqlParameter("@ccAuthReply_avsCodeRaw", arcRecord.ccAuthReply_avsCodeRaw));
                    cmd.Parameters.Add(new SqlParameter("@ccAuthReply_cardCategory", arcRecord.ccAuthReply_cardCategory));
                    cmd.Parameters.Add(new SqlParameter("@ccAuthReply_cavvResponseCode", arcRecord.ccAuthReply_cavvResponseCode));
                    cmd.Parameters.Add(new SqlParameter("@ccAuthReply_cavvResponseCodeRaw", arcRecord.ccAuthReply_cavvResponseCodeRaw));
                    cmd.Parameters.Add(new SqlParameter("@ccAuthReply_cvCode", arcRecord.ccAuthReply_cvCode));
                    cmd.Parameters.Add(new SqlParameter("@ccAuthReply_cvCodeRaw", arcRecord.ccAuthReply_cvCodeRaw));
                    cmd.Parameters.Add(new SqlParameter("@ccAuthReply_merchantAdviceCode", arcRecord.ccAuthReply_merchantAdviceCode));
                    cmd.Parameters.Add(new SqlParameter("@ccAuthReply_merchantAdviceCodeRaw", arcRecord.ccAuthReply_merchantAdviceCodeRaw));
                    cmd.Parameters.Add(new SqlParameter("@ccAuthReply_ownerMerchantID", arcRecord.ccAuthReply_ownerMerchantID));
                    cmd.Parameters.Add(new SqlParameter("@ccAuthReply_paymentNetworkTransactionID", arcRecord.ccAuthReply_paymentNetworkTransactionID));
                    cmd.Parameters.Add(new SqlParameter("@ccAuthReply_processorResponse", arcRecord.ccAuthReply_processorResponse));
                    cmd.Parameters.Add(new SqlParameter("@ccAuthReply_reasonCode", arcRecord.ccAuthReply_reasonCode));
                    cmd.Parameters.Add(new SqlParameter("@ccAuthReply_reconciliationID", arcRecord.ccAuthReply_reconciliationID));
                    cmd.Parameters.Add(new SqlParameter("@ccAuthReply_referralResponseNumber", arcRecord.ccAuthReply_referralResponseNumber));
                    cmd.Parameters.Add(new SqlParameter("@ccAuthReply_requestAmount", arcRecord.ccAuthReply_requestAmount));
                    cmd.Parameters.Add(new SqlParameter("@ccAuthReply_requestCurrency", arcRecord.ccAuthReply_requestCurrency));
                    cmd.Parameters.Add(new SqlParameter("@ccCaptureReply_amount", arcRecord.ccCaptureReply_amount));
                    cmd.Parameters.Add(new SqlParameter("@ccCaptureReply_reasonCode", arcRecord.ccCaptureReply_reasonCode));
                    cmd.Parameters.Add(new SqlParameter("@ccCaptureReply_reconciliationID", arcRecord.ccCaptureReply_reconciliationID));
                    cmd.Parameters.Add(new SqlParameter("@ccCaptureReply_requestDateTime", arcRecord.ccCaptureReply_requestDateTime));

                    cmd.Parameters.Add(new SqlParameter("@ccContent", arcRecord.ccContent));
                    string cmdText = "\n" + cmd.CommandText;
                    bool cmdFirst = true;
                    foreach (SqlParameter param in cmd.Parameters)
                    {
                        cmdText += "\n" + ((cmdFirst) ? "" : ",") + param.ParameterName + " = " + ((param.Value != null) ? "'" + param.Value.ToString() + "'" : "default");
                        cmdFirst = false;
                    }
                    #endregion Populate the SQL Params
                    #region Process SQL Command - Try
                    try
                    {
                        if (con.State == ConnectionState.Closed) { con.Open(); }
                        Int32 ccauthid = Convert.ToInt32(cmd.ExecuteScalar());
                        cybProc.cybid = ccauthid;
                        // LogSQL(cmd, "sqlPassed");
                    }
                    #endregion Process SQL Command - Try
                    #region Process SQL Command - Catch
                    catch (Exception ex)
                    {
                        cybProc.status = "ERROR";
                        cybProc.message = "ARC_SQL - Catch - SqlCommand";
                        cybProc.lblmessage = ErrorLog.ErrorLog_Display_String(ex, "Error: ARC_Cybersource_To_SQL - Try");

                        // Log_Exception("Error 001", ex, "error", "ARC_Cybersource_To_SQL - Process SQL Command - Try");
                        // LogSQL(cmd, "sqlFailed");
                    }
                    #endregion Process SQL Command - Catch
                }
                #endregion SqlCommand cmd
            }
            #endregion SqlConnection
        }
        #endregion Processing Start - SQL - Try
        #region Processing Start - SQL - Catch
        catch (Exception ex)
        {
            cybProc.status = "ERROR";
            cybProc.message = "ARC_SQL - Catch - Processing";
            cybProc.lblmessage = ErrorLog.ErrorLog_Display_String(ex, "Error: ARC_Cybersource_To_SQL - Catch");
            // Log_Exception("Error 002", ex, "error", "ARC_Cybersource_To_SQL - Processing Start - SQL - Catch");
        }
        #endregion Processing Start - SQL - Catch
    }
    #endregion Processing - Charge
    public sealed class ARC_Cybersource_Log_Auth
    {
        // Class for inserting the log
        public String Source;
        public String ExternalID;
        public String Status;
        public String CreateDate;

        public String decision;
        public String merchantReferenceCode;
        public Int32 reasonCode;
        public String requestID;
        public String requestToken;

        public String ccAuthReply_accountBalance;
        public String ccAuthReply_accountBalanceCurrency;
        public String ccAuthReply_accountBalanceSign;
        public String ccAuthReply_amount;
        public String ccAuthReply_authFactorCode;
        public String ccAuthReply_authorizationCode;
        public String ccAuthReply_authorizedDateTime;
        public String ccAuthReply_avsCode;
        public String ccAuthReply_avsCodeRaw;
        public String ccAuthReply_cardCategory;
        public String ccAuthReply_cavvResponseCode;
        public String ccAuthReply_cavvResponseCodeRaw;
        public String ccAuthReply_cvCode;
        public String ccAuthReply_cvCodeRaw;
        public String ccAuthReply_merchantAdviceCode;
        public String ccAuthReply_merchantAdviceCodeRaw;
        public String ccAuthReply_ownerMerchantID;
        public String ccAuthReply_paymentNetworkTransactionID;
        public String ccAuthReply_processorResponse;
        public Int32 ccAuthReply_reasonCode;
        public String ccAuthReply_reconciliationID;
        public String ccAuthReply_referralResponseNumber;
        public String ccAuthReply_requestAmount;
        public String ccAuthReply_requestCurrency;
        public String ccCaptureReply_amount;
        public Int32 ccCaptureReply_reasonCode;
        public String ccCaptureReply_reconciliationID;
        public String ccCaptureReply_requestDateTime;

        public String ccContent;
    }
    private sealed class ARC_Cybersource_Log_Refund
    {
        public Int32 CBAuthID;
        public Int32 ExternalID;

        public String msgInitialize;
        public String msgProcess;
        public String msgSave;

        public String RefundReason;
        public String RefundReasonNotes;
        public String Status;
        public String CreateDate;

        public String decision;
        public String merchantReferenceCode;
        public String reasonCode;
        public String requestID;
        public String requestToken;

        public String ccAuthReply_accountBalance;
        public String ccAuthReversalReply_amount;
        public String ccAuthReversalReply_authorizationCode;
        public String ccAuthReversalReply_processorResponse;
        public String ccAuthReversalReply_reasonCode;
        public String ccAuthReversalReply_requestDateTime;
        public String ccCreditReply_amount;
        public String ccCreditReply_ownerMerchantID;
        public String ccCreditReply_reasonCode;
        public String ccCreditReply_reconciliationID;
        public String ccCreditReply_requestDateTime;
        public String voidReply_amount;
        public String voidReply_currency;
        public String voidReply_reasonCode;
        public String voidReply_requestDateTime;

        public String ccContent;
    }
    public sealed class cybProcess
    {
        public String message;
        public String status;
        public String lblmessage;
        public Int32 cybid = 0;
    }

}
