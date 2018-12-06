using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CyberSource.Clients;
//using CyberSource.Clients.SoapWebReference;
using CyberSource.Clients.SoapServiceReference;

using System.IO;

using System.Data;
using System.Data.SqlClient;

public partial class CallRefunds : System.Web.UI.Page
{
    bool oDebug = true;
    protected void Page_PreInit(Object sender, EventArgs e)
    {
        Master.PageTitle = "Process Refund";
        this.Title = String.Format("{0}: {1}", Master.MyTitle, Page.Title);
    }
    private String sqlStr = Connection.GetConnectionString("Default", ""); // Controlled by Web Config ARC_Live | ARC_Stage
    private String msgRefund = "";
    private Int32 foLimit = 59; // Days that a Follow On Credit can be performed
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (Page.User.Identity.IsAuthenticated)
            {
                if (Page.User.IsInRole("System Administrator") == true || Page.User.IsInRole("Administrator") == true || Page.User.IsInRole("Manager") == true)
                {
                    // Properly Authenticated
                    if (Request["cbid"] != null)
                    {
                        Record_Get(Convert.ToInt32(Request["cbid"]));
                        pnl_ReplyDetails.Visible = false;
                    }
                    else
                    {
                        dtlLabel.Text = "The record is not valid; please go back and try again.";
                        dtlLabel.ForeColor = System.Drawing.Color.Red;
                        pnl_refund.Visible = false;
                    }
                }
                else
                {
                    // User does not have access
                    dtlLabel.Text = "You do not have access to process refunds.";
                    dtlLabel.ForeColor = System.Drawing.Color.Red;
                    pnl_refund.Visible = false;
                }
            }
            else
            {
                // User does not have access
                dtlLabel.Text = "You are not properly authenticated.";
                dtlLabel.ForeColor = System.Drawing.Color.Red;
                pnl_refund.Visible = false;
            }
        }
    }
    protected void CreditTry(object sender, EventArgs e)
    {
        dtlLabel.Text = "";
        lblTemplate.Text = "";
        msgRefund = "";
        ErrorView.DataSource = null;
        ProcessRefund_FollowOn();
        //InstantEmail_RefundReport();
        pnl_ReplyDetails.Visible = true;
    }
    protected void Record_Get(Int32 CBAuthID)
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
                    cmd.CommandText = "[dbo].[sp_cybersource_get_record]";
                    cmd.CommandType = CommandType.StoredProcedure;
                    #endregion Populate the SQL Command
                    #region Populate the SQL Params
                    cmd.Parameters.Add(new SqlParameter("@Source", "Web"));
                    cmd.Parameters.Add(new SqlParameter("@CBAuthID", CBAuthID));
                    #region Log the SQL Command Text
                    string cmdText = "\n" + cmd.CommandText;
                    bool cmdFirst = true;
                    foreach (SqlParameter param in cmd.Parameters)
                    {
                        cmdText += "\n" + ((cmdFirst) ? "" : ",") + param.ParameterName + " = " + ((param.Value != null) ? "'" + param.Value.ToString() + "'" : "default");
                        cmdFirst = false;
                    }
                    #endregion Log the SQL Command Text
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
                                    lblCBAuthID.Text = CBAuthID.ToString();
                                    lblCallID.Text = sqlRdr["callid"].ToString();
                                    lblExternalID.Text = sqlRdr["ExternalID"].ToString();
                                    lblStatus.Text = sqlRdr["Status"].ToString();
                                    CreateDate.Text = sqlRdr["CreateDate"].ToString();
                                    RequestID.Text = sqlRdr["requestID"].ToString();
                                    RequestToken.Text = sqlRdr["requestToken"].ToString();
                                    ReferenceNum.Text = sqlRdr["merchantReferenceCode"].ToString();
                                    Amount.Text = sqlRdr["ccAuthReply_amount"].ToString();
                                    AmountOriginal.Value = sqlRdr["ccAuthReply_amount"].ToString();
                                    // Last 4 of Card Number
                                    CardNumber.Text = sqlRdr["ccnum"].ToString().Substring(sqlRdr["ccnum"].ToString().Length - 4, 4);
                                    FirstName.Text = sqlRdr["fname"].ToString();
                                    LastName.Text = sqlRdr["lname"].ToString();
                                    DateTime dtChargeDate;
                                    DateTime.TryParse(CreateDate.Text, out dtChargeDate);
                                    if (dtChargeDate != null)
                                    {
                                        dtlLabel.Text = (dtChargeDate - DateTime.UtcNow).TotalDays.ToString();
                                        if ((DateTime.UtcNow - dtChargeDate).TotalDays > foLimit)
                                        {
                                            RefundType.Value = "Stand Alone";
                                            pnl_standalone.Visible = true;
                                            CardNumberFull.Value = sqlRdr["ccnum"].ToString();
                                            CardMonth.Text = sqlRdr["ccexpmonth"].ToString();
                                            CardYear.Text = sqlRdr["ccexpyear"].ToString();
                                            /*
                                             * 001 == Visa == 2
                                             * 002 == MasterCard == 3
                                             * 003 == American Express == 4
                                             * 004 == Discover == 5
                                             */
                                            if (CardNumberFull.Value.Length > 1)
                                            {
                                                switch (CardNumberFull.Value.Substring(0, 1))
                                                {
                                                    case "4":
                                                        CardType.Text = "Visa";
                                                        CardTypeFull.Value = "001";
                                                        break;
                                                    case "5":
                                                        CardType.Text = "MasterCard";
                                                        CardTypeFull.Value = "002";
                                                        break;
                                                    case "3":
                                                        CardType.Text = "American Express";
                                                        CardTypeFull.Value = "003";
                                                        break;
                                                    case "6":
                                                        CardType.Text = "Discover";
                                                        CardTypeFull.Value = "004";
                                                        break;
                                                }
                                            }
                                            Address1.Text = sqlRdr["address"].ToString();
                                            Address2.Text = sqlRdr["suitenumber"].ToString();
                                            //Address3.Text = sqlRdr[""].ToString();
                                            City.Text = sqlRdr["city"].ToString();
                                            ddlState.Text = sqlRdr["state"].ToString();
                                            Zip.Text = sqlRdr["zip"].ToString();
                                            ddlCountry.Text = "USA";
                                        }
                                        else
                                        {
                                            RefundType.Value = "Follow On";
                                        }
                                    }

                                    if (sqlRdr["Status"].ToString() == "Settled" || sqlRdr["Status"].ToString() == "Approved")
                                    {
                                        btnRefundSubmit.Enabled = true;
                                    }
                                    else
                                    {
                                        dtlLabel.Text = "The record is not in a valid refundable status.";
                                        dtlLabel.ForeColor = System.Drawing.Color.Red;
                                    }
                                }
                            }
                            else
                            {
                                dtlLabel.Text = "No records...";
                            }
                        }
                        //Log(cmdText.Replace("\n", " "), "sqlPassed");
                        //sqlMsg.Text = cmdText.Replace("\n", "<br />") + "<br />";
                    }
                    #endregion Process SQL Command - Try
                    #region Process SQL Command - Catch
                    catch (Exception ex)
                    {
                        msgLabel.Text = "Oops";
                        Error_Save(ex, "RunTransaction");
                        //Log_Exception("Error 001", ex, "standard", "Step 1 Catch");
                        //Log(cmdText, "sqlFailed");
                        sqlMsg.Text = cmdText.Replace("\n", "<br />") + "<br />";
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
            msgLabel.Text = "Oops";
            Error_Save(ex, "RunTransaction");
            //Log_Exception("Error 001", ex, "standard", "Step 1 Catch");
        }
        #endregion Processing Start - SQL - Catch
    }
    protected void ProcessRefund_FollowOn()
    {
        WriteToLabel("add", "Red", "<br />" + "Processing: Follow-On Transaction - Start", dtlLabel);
        #region Process Refund
        try
        {
            /// <summary>
            /// Follow-On transactions only need the RequestID, Token, and Amount of the refund
            /// </summary
            #region Processing CyberSource Attempt
            RequestMessage request = new RequestMessage();
            request.ccCreditService = new CCCreditService();
            request.ccCreditService.run = "true";

            DateTime dtChargeDate;
            DateTime.TryParse(CreateDate.Text, out dtChargeDate);
            if (dtChargeDate == null)
            {
                // Throw an Exception since the date is not valid
                throw new Exception("Invalid Donation Date");
            }
            //dtlLabel.Text = (dtChargeDate - DateTime.UtcNow).TotalDays.ToString();
            #region Stand-Alone Credit
            if ((DateTime.UtcNow - dtChargeDate).TotalDays > foLimit)
            {
                // Stand Alone
                BillTo billTo = new BillTo();
                billTo.firstName = FirstName.Text;
                billTo.lastName = LastName.Text;
                billTo.street1 = Address1.Text;
                billTo.postalCode = Zip.Text;
                billTo.city = City.Text;
                billTo.state = ddlState.Text;
                billTo.country = ddlCountry.Text;

                billTo.email = "nobody@cybersource.com";

                request.billTo = billTo;

                Card card = new Card();
                card.accountNumber = CardNumberFull.Value;
                card.expirationMonth = CardMonth.Text;
                card.expirationYear = CardYear.Text;
                if (CardTypeFull.Value.Length > 0)
                {
                    card.cardType = CardTypeFull.Value;
                }
                request.card = card;

            }
            #endregion Stand-Alone Credit
            #region Follow On Credit
            else
            {
                // Follow On
                // Credit Required Fields
                request.ccCreditService.captureRequestID = RequestID.Text.Trim();
                request.ccCreditService.captureRequestToken = RequestToken.Text.Trim();
            }
            #endregion Follow On Credit


            request.merchantReferenceCode = ReferenceNum.Text.Trim();

            PurchaseTotals purchaseTotals = new PurchaseTotals();
            purchaseTotals.currency = "USD";
            purchaseTotals.grandTotalAmount = Amount.Text.Trim();
            request.purchaseTotals = purchaseTotals;

            //Attempt processing the request, handle excepts
            Console.WriteLine("     ");
            WriteToLabel("add", "Red", "<br />" + "Sending Request", dtlLabel);
            #endregion Processing CyberSource Attempt
            #region RunTransaction: Try
            try
            {
                ARC_Cybersource_Log_Refund arcRecord = new ARC_Cybersource_Log_Refund();

                ReplyMessage reply = SoapClient.RunTransaction(request);

                string template = GetTemplate(reply.decision.ToUpper());
                string content = GetContent(reply);
                lblTemplate.Text = String.Format(template, content);
                arcRecord.ccContent = content;
                arcRecord.ExternalID = lblExternalID.Text;
                arcRecord.CBAuthID = lblCBAuthID.Text;

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
                //ProcessRefund_SQLUpdate()

                rplDecision.Text = arcRecord.decision;
                rplMerchantReferenceCode.Text = arcRecord.merchantReferenceCode;
                rplReasonCode.Text = arcRecord.reasonCode;
                rplRequestID.Text = arcRecord.requestID;
                rplRequestToken.Text = arcRecord.requestToken;

                rplAmount.Text = arcRecord.ccCreditReply_amount;
                rplReasonCode2.Text = arcRecord.ccCreditReply_reasonCode;
                rplReconciliationID.Text = arcRecord.ccCreditReply_reconciliationID;
                rplReconciliationID.ForeColor = System.Drawing.Color.Blue;
                rplReconciliationID.Font.Bold = true;
                rplRequestDateTime.Text = arcRecord.ccCreditReply_requestDateTime;

                Record_Save(arcRecord);
                Record_Get(Convert.ToInt32(Request["cbid"]));
            }
            #endregion RunTransaction: Try
            #region RunTransaction: Catch
            catch (Exception ex)
            {
                msgLabel.Text = "Oops";
                rplDecision.Text = "!! ERROR - NOT COMPLETED !!";
                msgRefund = ex.Message;
                Error_Save(ex, "RunTransaction");
            }
            #endregion RunTransaction: Catch
        }
        catch (Exception ex)
        {
            msgLabel.Text = "Oops";
            rplDecision.Text = "!! ERROR - NOT COMPLETED !!";
            msgRefund = ex.Message;
            Error_Save(ex, "Processing_CyberSource");
        }
        finally
        {
            InstantEmail_RefundReport();
        }
        #endregion
        WriteToLabel("add", "Red", "<br />" + "Processing: Follow-On Transaction - End", dtlLabel);
    }
    protected bool InstantEmail_RefundReport()
    {
        #region Chilkat MailMan
        using (Chilkat.MailMan mailman = new Chilkat.MailMan())
        {
            mailman.UnlockComponent("SGREENWOODMAILQ_FuY9K2d92R8F");
            mailman.SmtpHost = Connection.GetSmtpHost();
            #region Chilkat Email
            using (Chilkat.Email email = new Chilkat.Email())
            {
                int tzOffSet = (DateTime.Now.IsDaylightSavingTime()) ? -4 : -5;
                DateTime dt = DateTime.Parse(CreateDate.Text);
                DateTime dtNow = DateTime.UtcNow;
                email.AddHeaderField("X-Priority", "1 (High)");
                email.Subject = "ARC Portal - Refund Report";
                email.AddHeaderField("DataExchange-Automated", "LMS_JobNotice JobID:03");
                email.AddHeaderField("Portal-Automated", "Script-ARCRR");

                String emailPath = "emails/";
                String emailFile = "portal_arc_refund_processed.html";
                String htmlBody = "";

                //System.IO.StreamReader rdr = new StreamReader(Server.MapPath("~") + emailPath + emailFile);
                System.IO.StreamReader rdr = new StreamReader(Server.MapPath(emailPath + emailFile));
                htmlBody = rdr.ReadToEnd();
                rdr.Close();
                rdr.Dispose();

                #region Populate Lead Email Fields
                htmlBody = htmlBody.Replace("{agent_name}", Page.User.Identity.Name);
                htmlBody = htmlBody.Replace("{attempt_on}", " at " + dtNow.ToString("MM/dd/yyyy HH:mm:ss") + " - UTC");

                htmlBody = htmlBody.Replace("{refund_type}", RefundType.Value);
                htmlBody = htmlBody.Replace("{callid}", lblCallID.Text);
                htmlBody = htmlBody.Replace("{donation_date}", dt.ToString("MM/dd/yyyy HH:mm:ss"));
                htmlBody = htmlBody.Replace("{donationccinfoid}", lblExternalID.Text);
                htmlBody = htmlBody.Replace("{refund_status}", rplDecision.Text);
                htmlBody = htmlBody.Replace("{refund_reason}", refundReason.Text);
                
                htmlBody = htmlBody.Replace("{amount_donation}", AmountOriginal.Value);
                htmlBody = htmlBody.Replace("{amount_refund}", Amount.Text);
                htmlBody = htmlBody.Replace("{first_name}", FirstName.Text);
                htmlBody = htmlBody.Replace("{last_name}", LastName.Text);
                htmlBody = htmlBody.Replace("{card_num}", CardNumber.Text);

                htmlBody = htmlBody.Replace("{refund_message}", msgRefund);
                

                #endregion Populate Lead Email Fields
                #region Populate Standard Email Fields
                htmlBody = htmlBody.Replace("{script_time}", dtNow.ToString());
                if (DateTime.Now.IsDaylightSavingTime())
                {
                    htmlBody = htmlBody.Replace("{script_timezone}", String.Format("GMT {0} {1}", tzOffSet, "Eastern Daylight Time"));
                }
                else
                {
                    htmlBody = htmlBody.Replace("{script_timezone}", String.Format("GMT {0} {1}", tzOffSet, "Eastern Standard Time"));
                }
                #endregion Populate Standard Email Fields

                email.SetHtmlBody(htmlBody);
                if (Connection.GetConnectionType() == "Local")
                {
                    email.AddTo("Pehuen Test 1", "nciambotti@greenwoodhall.com");
                    email.AddTo("Carrie Stevenson", "cstevenson@greenwoodhall.com");
                }
                else
                {
                    email.AddTo("Pehuen ARC", "nciambotti@greenwoodhall.com");
                    email.AddTo("Carrie Stevenson", "cstevenson@greenwoodhall.com");
                    //email.AddCC("MIS", "mis@greenwoodhall.com");
                    //email.AddBcc("Pehuen Scripts", "nciambotti@greenwoodhall.com");
                }
                email.BounceAddress = "do.not.reply@greenwoodhall.com";
                email.FromName = "ARC Portal";
                email.FromAddress = "do.not.reply@greenwoodhall.com";
                email.ReplyTo = "do.not.reply@greenwoodhall.com";
                bool success;
                success = mailman.SendEmail(email);
                if (success)
                {
                    // Update the action
                    return true;
                }
                else
                {
                    // Do not update the action, the system will pick it up again
                    return false;
                }
            }
            #endregion Chilkat Email
        }
        #endregion Chilkat MailMan
    }

    public sealed class ARC_Cybersource_Log_Refund
    {
        public String CBAuthID;
        public String ExternalID;
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
    protected void Record_Save(ARC_Cybersource_Log_Refund arcRecord)
    {
        #region Processing Start - SQL - Try
        string cmdText = "";
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
                    cmd.CommandText = "[dbo].[sp_cybersource_refund]";
                    cmd.CommandType = CommandType.StoredProcedure;
                    #endregion Populate the SQL Command
                    #region Populate the SQL Params
                    cmd.Parameters.Add(new SqlParameter("@Source", "Web"));
                    cmd.Parameters.Add(new SqlParameter("@CBAuthID", arcRecord.CBAuthID));
                    cmd.Parameters.Add(new SqlParameter("@ExternalID", arcRecord.ExternalID));
                    cmd.Parameters.Add(new SqlParameter("@User", Page.User.Identity.Name));
                    string strReason = refundReason.SelectedValue;
                    if (strReason == "Other" && refundReasonOther.Text.Length > 0)
                    {
                        strReason = refundReasonOther.Text;
                    }
                    cmd.Parameters.Add(new SqlParameter("@Reason", strReason));
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
                                    rplResponse.Text = "Record Updated: " + sqlRdr[0].ToString();
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
                        msgLabel.Text = "Oops";
                        Error_Save(ex, "RunTransaction");
                        //Log_Exception("Error 001", ex, "standard", "Step 1 Catch");
                        //Log(cmdText, "sqlFailed");
                        sqlMsg.Text = cmdText.Replace("\n", "<br />") + "<br />";
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
            msgLabel.Text = "Oops";
            sqlMsg.Text = cmdText.Replace("\n", "<br />") + "<br />";
            Error_Save(ex, "RunTransaction");
            //Log_Exception("Error 001", ex, "standard", "Step 1 Catch");
        }
        #endregion Processing Start - SQL - Catch
    }

    private static string GetTemplate(string decision)
    {
        // Retrieves the text that corresponds to the decision.
        if ("ACCEPT".Equals(decision))
        {
            return ("The order succeeded.{0}");
        }
        if ("REJECT".Equals(decision))
        {
            return ("Your order was not approved.{0}");
        }
        // ERROR, or an unknown decision
        return ("Your order could not be completed at this time.{0}" + "Please try again later.");
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
