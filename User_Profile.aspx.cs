#region Initial Config
        lblInformation.Text = "Initialized: Start";
        lblInformation.ForeColor = System.Drawing.Color.Blue;
        reload_script.NavigateUrl = Request.RawUrl;
        if (Request.RawUrl.IndexOf("?") > 0)
        {
            reload_script2.NavigateUrl = Request.RawUrl.Substring(0, Request.RawUrl.IndexOf("?"));
        }
        else
        {
            reload_script2.NavigateUrl = Request.RawUrl;
        }
        #endregion Initial Config
        loadDuration = (DateTime.UtcNow - loadStart).TotalMilliseconds;
        loadTime = ghFunctions.MillisecondsTo(loadDuration);
        loadMessage += String.Format("<br />Config Time: {0}", loadTime);
        #region !IsPostBack
        if (!IsPostBack)
        {
            // This is needed to do the initial setup of the script
            /// This one we need to call even if a failed query string
            /// This will handle the show/hide field part of the script
            requiredScriptSetup();
            ResponseSQL.Text = Connection.GetConnectionType();
            cdCompanyID.Text = companyid.ToString();
            bool vPassed = true;
            bool qValid = true;
            /// First things first we make sure the page is configured properly
            /// This checks DataBase connections
            /// File paths
            /// Log paths (CC Processing)
            vPassed = validateConfig();
            if (vPassed)
            {
                /// Validate the query string - if this is not a valid querystring we report an error
                /// If there is no query string, we provide an agent [login] screen
                #region Validate Query String
                /// Simple QS validation
                /// String.IsNullOrEmpty()
                if (Request["agent.id"] == null || Request["agent.id"].ToString() == "ReplaceMe" || Request["agent.id"].ToString() == "998")
                {
                    // Show the "New Call" screen
                    // Remove this for production?
                    HiddenField_Toggle("sectionA0", "show");
                    HiddenField_Toggle("sectionA1", "hide");
                    HiddenField_Toggle("sectionA2", "hide");
                    ResponseSQL.Text = "This is not a valid agent id, please enter your agent id and name.";
                    DDL_Load_DNIS();
                    if (Request.IsLocal || Request.Url.ToString().Contains("192.168."))
                    {
                        /// This adds 2 checkbos to the agent [login] that allows for some pre-defined admin functions
                        /// They allow for easier/quicker testing
                        /// The testing is a bit dirty as it fills in data
                        pnlDeBug.Visible = true;
                    }
                    pnlNewCall.Visible = true;
                    txtAgentID.Text = "";
                    qValid = false;
                }
                else
                {
                    if (process_Querystring())
                    {
                        /// This script setup is done if we validated the querystring
                        validatedScriptSetup();
                        // setupDispositionList(); // This is a test in progress

                        //English | Spanish
                        #region Call Greeting

                        dGreetingStandard.Visible = true;
                        dGreetingHoliday.Visible = false;
                        dGreetingDRTV.Visible = false;
                        dGreetingDynamic.Visible = false;
                        // Holiday Catalog Greeting: 9496082824 English / 9496082857 Spanish
                        if (cdCompany.Text.Length > 0)
                        {
                            if (cdCompany.Text == "DRTV")
                            {
                                dGreetingStandard.Visible = false;
                                dGreetingDRTV.Visible = true;
                            }
                            else if (ShowHolidayCatalog && (cdDNIS.Text == "2824" || cdDNIS.Text == "2857"))
                            {
                                // Need to fix the above check since the DNIS will be 10 digit not 4
                                dGreetingStandard.Visible = false;
                                dGreetingHoliday.Visible = true;
                            }
                        }
                        #endregion Call Greeting
                        dGreetingStandard_Spanish.InnerHtml = dGreetingStandard_Spanish.InnerHtml.Replace("{agent_name}", cdAgentName.Text);
                        dGreetingStandard_English.InnerHtml = dGreetingStandard_English.InnerHtml.Replace("{agent_name}", cdAgentName.Text);

                        dGreetingHoliday_English.InnerHtml = dGreetingHoliday_English.InnerHtml.Replace("{agent_name}", cdAgentName.Text);
                        dGreetingHoliday_Spanish.InnerHtml = dGreetingHoliday_Spanish.InnerHtml.Replace("{agent_name}", cdAgentName.Text);

                        dGreetingDRTV_Spanish.InnerHtml = dGreetingDRTV_Spanish.InnerHtml.Replace("{agent_name}", cdAgentName.Text);
                        dGreetingDRTV_English.InnerHtml = dGreetingDRTV_English.InnerHtml.Replace("{agent_name}", cdAgentName.Text);
                    }
                    else
                    {
                        qValid = false;
                    }
                }
                #endregion Validate Query String
                #region Valid Query String
                if (qValid)
                {
                    /// If we have a valid query string, we go through the process of initiating the call
                    /// This will create/update a CALL record in ARC
                    /// As well as create the INTERACTION in DataExchange
                    /// 
                    Populate_DropDownList_All();

                    Session.Remove("PostData");
                    Initiate_Call(); // Here we create the CALL  and INTERACTION records
                    Populate_CallInfo(); // Review whether this can be removed and all moved to the Initiate_Call sub

                    #region Admin / DeBug ?
                    /// This should be higher in the page?
                    /// So it does not over-write what we get from Initiate Call?
                    if (Request["t"] != null)
                    {
                        if (Request["t"].ToString() == "arctest234")
                        {
                            // DeBug_Populate_Continue_Donation_OneTime();
                        }
                        else if (Request["t"].ToString() == "txtDon")
                        {
                            Populate_Test_Data();
                        }
                        else if (Request["t"].ToString() == "txtSus")
                        {
                            Populate_Test_Data_Sustainer();
                        }
                        else if (Request["t"].ToString() == "txtHol")
                        {
                            Populate_Test_Data_Holiday();
                        }
                    }
                    #endregion Admin / DeBug ?
                }
                #endregion Valid Query String
                #region InValid Query String
                else
                {
                    /// The querystring is not valid, show the agent login panel
                    if (Request.IsLocal || Request.Url.ToString().Contains("192.168.") || (Request["t"] != null && Request["t"].ToString() == "arctest234"))
                    {
                        txtAgentID.Text = "2032613";
                        txtAgentName.Text = "Pehuen Ciambotti";
                        ddlNewDNIS.SelectedIndex = 0;
                        chkTestData.Checked = false;
                        chkDeBugData.Checked = false;
                        
                    }
                    else if (Session["agentname"] != null)
                    {
                        // cdAgentName.Text =
                        txtAgentName.Text = Session["agentname"].ToString();
                    }
                    if (Session["agentdnis"] != null)
                    {
                        // ["agentdnis"] = call.dnis;
                        try
                        {
                            ddlNewDNIS.SelectedValue = Session["agentdnis"].ToString();
                        }
                        catch { }
                    }

                    if (Request["t"] != null && Request["t"].ToString() == "arctest234")
                    {
                        chkTestData.Checked = false;
                        chkDeBugData.Checked = false;
                    }

                    HiddenField_Toggle("sectionA0", "show");
                    HiddenField_Toggle("sectionA1", "hide");
                    HiddenField_Toggle("sectionA2", "hide");
                    if (Request["agent.id"] == null)
                    {
                        ResponseSQL.Text = "Please enter your Five9 AgentID and Full Name.";
                        ResponseSQL.Text += "<br />Select a DNIS if you would like to test a non default one.";
                    }
                    else
                    {
                        ResponseSQL.Text = "This is not a valid agent id, please enter your agent id and name..";
                        ResponseSQL.Text += "<br />Error validating the query string.";
                    }
                    
                }
                #endregion InValid Query String
            }
            else
            {
                qValid = false;
                HiddenField_Toggle("sectionA0", "show");
                HiddenField_Toggle("sectionA1", "hide");
                HiddenField_Toggle("sectionA2", "hide");
                /// Browsers:
                /// FireFox
                /// UserAgent|Mozilla/5.0 (Windows NT 6.1; WOW64; rv:37.0) Gecko/20100101 Firefox/37.0
                /// Chrome
                /// UserAgent|Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/54.0.2840.71 Safari/537.36
                /// IE
                /// UserAgent|Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko

                if (!Request.UserAgent.ToString().Contains("Chrome") && !Request.UserAgent.ToString().Contains("Firefox"))
                {
                    ResponseSQL.ForeColor = System.Drawing.Color.Red;
                    ResponseSQL.Text += "<br />Your browser is not supported.";
                    ResponseSQL.Text += "<br />You must use Chrome or Firefox browsers.";
                    ResponseSQL.Text += "<br />You can copy the below link and manually open it on one of the approved browsers.";
                    ResponseSQL.Text += "<br />";
                    chrome_link.Visible = true;
                    chrome_link.NavigateUrl = reload_script.NavigateUrl;
                }
                else
                {
                    ResponseSQL.Text += "<br />Internal page error; contact IT.";
                }
                // ResponseSQL.Text += "<br />Internal page error; contact IT.";

            }
        }
        #endregion !IsPostBack
        loadDuration = (DateTime.UtcNow - loadStart).TotalMilliseconds;
        loadTime = ghFunctions.MillisecondsTo(loadDuration);
        loadMessage += String.Format("<br />!IsPostBack Time: {0}", loadTime);
        if (IsPostBack)
        {
            Add_Hidden_Controls();
            Show_Controls_PostBack();
        }
        loadDuration = (DateTime.UtcNow - loadStart).TotalMilliseconds;
        loadTime = ghFunctions.MillisecondsTo(loadDuration);
        loadMessage += String.Format("<br />IsPostBack Time: {0}", loadTime);
        if (!IsPostBack)
        {
            #region Site Mode
            if (tglMode == "Live")
            {
                cdMode.Text = "LIVE";
                lblMode.Text = "Live";
                lblMode.ForeColor = System.Drawing.Color.Blue;
                lblJRE.ForeColor = System.Drawing.Color.Blue;
            }
            else if (tglMode == "Test" || tglMode == "Stage")
            {
                cdMode.Text = "TEST";
                lblMode.Text = "Testing";
                //lblMode.Font.Size = 24;
            }
            else if (tglMode == "Maintenance")
            {
                cdMode.Text = "Maintenance";
                lblMode.Text = "Maintenance Mode";
            }
            else
            {
            }
            lblMode.Text += " - " + ghFunctions.portalVersion;
            #endregion Site Mode
            // lblInformation.Text += String.Format("<br />{0}|{1}", "UserHostAddress", Request.UserHostAddress.ToString());
            // lblInformation.Text += String.Format("<br />{0}|{1}", "UserAgent", Request.UserAgent.ToString());
        }

        // HiddenField_Toggle("sectionA0", "show"); -- This works
        loadDuration = (DateTime.UtcNow - loadStart).TotalMilliseconds;
        loadTime = ghFunctions.MillisecondsTo(loadDuration);
        loadMessage += String.Format("<br />Load Time: {0}", loadTime);
        loadMessage += String.Format("<br />TotalMilliseconds: {0}", loadDuration);
        lblInformation.Text += loadMessage;
        // lblQueryTime.Text += loadMessage;
        //if (Request["ghsource"] != null && (Request["ghsource"].ToString() == "_Ansafone" || Request["ghsource"].ToString() == "_Endicott"))
        //{
        //    faq_drtv.Visible = false;
        //    faq_globe.Visible = false;
        //}
        //if (cdCompany.Text == "Harvey Telethon")
        //{
        //    faq_drtv.Visible = false;
        //    faq_globe.Visible = false;
        //}
        if (cdCompany.Text != "Globetrotters" && cdCompany.Text != "DRTV")
        {
            faq_drtv.Visible = false;
            faq_globe.Visible = false;
            if ("1" == "1")
            {
                cdTelethonMode.Text = "True"; // Telethon Mode
            }
        }



    }
    /// <summary>
    /// This will validate the configuration
    /// Check Cyb Path
    /// Check if we are using Stage vs Production
    /// </summary>
    /// <returns></returns>
    protected sealed class Variables_Interactions
    {
        #region Variables
        public Int32 sp_companyid;
        public Int32 sp_interactiontype;
        public Int32 sp_resourcetype;
        public Int32 sp_resourceid;
        public Int32 sp_status;

        public Int64 sp_interactionid;
        public Int64 sp_callid;

        public DateTime sp_datestart;
        public DateTime sp_dateend;
        public Int32 sp_offset;

        public String sp_originator;
        public String sp_destinator;

        public String sp_sessionid;

        public Int64 sp_campaignid;
        public String sp_campaignname;
        public Int64 sp_campaignid_five9id;
        public Int64 sp_skillid;
        public String sp_skillname;
        public Int64 sp_skillid_five9id;
        public Int64 sp_dispositionid;
        public String sp_dispositionname;
        public Int64 sp_dispositionid_five9id;
        public Int64 sp_typeid;
        public String sp_typename;
        public Int64 sp_typeid_five9id;
        public Int64 sp_agentid;
        public String sp_agent;

        public Int64 sp_agent_five9id;

        public String sp_agent_firstname;
        public String sp_agent_lastname;
        public String sp_agent_name;

        public Int64 sp_stationid;
        public String sp_stationtype;

        public Int64 sp_mediatypeid;
        public String sp_mediatype;


        public Int32 sp_abandoned;
        public Int32 sp_contacted;
        public Int32 sp_conferences;
        public Int32 sp_holds;
        public Int32 sp_parks;
        public Int32 sp_recordings;
        public Int32 sp_transfers;
        public Int32 sp_voicemails;
        public Int32 sp_mw_recordings;

        public Int64 sp_length_prusing System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceProcess;
using System.IO;
using System.Timers;

using System.Data;
using System.Data.SqlClient;

using System.Text;
using System.Net;
using System.Configuration;

using System.Xml;

using System.Collections.Generic;

namespace Donation_SUV_2017_Receipts
{
    /// <summary>
    /// Send receipts based on approved donations
    /// * Service runs every minute
    /// * Service emails [daily] count
    /// * Service emails on error
    /// </summary>
    public partial class Service1 : ServiceBase
    {
        /// <summary>
        /// Initial settings and config variables
        /// </summary>
        #region Log/Run Cycle Settings
        int EmailCycle = 24; // Hours
        int RunCycle = 3; // Minimum Hours to sleep before running again
        int ReceiptsMaxDay = Convert.ToInt32(ConfigurationSettings.AppSettings["ReceiptsMaxDay"]);
        int ReceiptsDailyTotal = 0;
        bool logToggle = false;
        bool runToggle = false; // This will ensure we run if the  RunCycle is not met yet (we started the service within les than that)
        bool runLoop = false;
        bool running = false;
        bool logSleep = false;
        bool runOnStart = Convert.ToBoolean(ConfigurationSettings.AppSettings["runOnStart"].ToString()); // Hard Coded Run Once
        bool oDebug = Convert.ToBoolean(ConfigurationSettings.AppSettings["runDebug"].ToString()); // Hard Coded Run Once
        DateTime dtStart = DateTime.Now;
        DateTime dtLoop = DateTime.Now;
        DateTime dtEmail = DateTime.Now;
        DateTime dtLog = DateTime.Now;
        Int32 dtDay = DateTime.Now.DayOfYear;
        DateTime dtReportDate;
        //DateTime dtReportDate = (ConfigurationSettings.AppSettings["runCustomReportDate"].ToString().Length > 0) ? DateTime.Parse(ConfigurationSettings.AppSettings["runCustomReportDate"].ToString()) : DateTime.Now;

        #endregion Log/Run Cycle Settings
        #region Hard Coded and Soft Coded initial values
        private System.Timers.Timer timer = null;
        string serviceName = ConfigurationSettings.AppSettings["serviceName"];
        string logging = ConfigurationSettings.AppSettings["logging"].ToUpper(); //Login Type
        string servicepath = ConfigurationSettings.AppSettings["servicePath"]; //If File - Path
        string logfilepath = ConfigurationSettings.AppSettings["servicePath"] + ConfigurationSettings.AppSettings["logfilePath"]; //If File - Path
        string logfilename = ConfigurationSettings.AppSettings["logfileName"]; //If File - Name
        long logfilemaxsize; //Will clear the file once maxsize is reached
        int logInterval = 60;
        int cntrLoop = 0;
        int cntrFiles = 0;
        int epochID = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;

        String lnBreak = "--------------------------------------------------------------------";

        #endregion Hard Coded and Soft Coded initial values
        #region Seald Classe(s)
        Process_Count pCounts = new Process_Count();
        public sealed class Process_Count
        {
            public int Loops;
            public int Errors;
            public int RcrdTotal;
            public int RcrdProcessed;
            public int RcrdSuccess;
            public int RcrdFailed;
            public int RcrdError;
            public int RcrdErrorSql;
            public int getError;
            public DateTime Start = DateTime.Now;
        }
        Error_Message errMsg = new Error_Message();
        public sealed class Error_Message
        {
            public int Count;
            public String Error;
            public String Message;
            public String Source;
            public String StackTrace;
            public DateTime TimeStamp;
        }
        #endregion
        private String sqlStr = Connection.GetConnectionString("Default", "");
        public Service1()
        {
     <!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Hand In Hand - 2017 - General Donor Receipt</title>
    <style type="text/css">
        body
        {
            text-align: left;
            font-family: Arial, Helvetica, sans-serif;
            margin: 0px;
            padding: 0px;
            color: #000000;
            font-size: 12px;
        }
        #container
        {
            margin-left: auto;
            margin-right: auto;
            width: 600px;
        }
        #container p
        {
            margin-bottom: 5px;
            padding: 0px;
        }
        div.clearBreak
        {
            clear: both;
            display: block;
            line-height: 1px;
            font-size: 1px;
            margin-top: -1px;
        }
        #legal
        {
            font-size: 10px;
            color: #000000;
            font-family: Arial;
            text-align: left;
        }
        .noborder a
        {
        	outline: none;
        }
        .noborder a img
        {
        	outline: none;
            padding-top: 10px;
        }
        .noborder img
        {
        	border: 0;
        }

    </style>
</head>
<body>
    <div id="container">
        <div>
            <br />Hand in Hand Hurricane Relief
            <br />Comic Relief USA
            <br />488 Madison Avenue,
            <br />10<sup>th</sup> Floor
            <br />New York, 
            <br />New York, 
            <br />10022
            <br />&nbsp;
        </div>
        <div>
            Hand in Hand Hurricane Relief <b>Donor Receipt</b>
            <br /><b>Confirmation No: {Confirmation}</b>
            <br />&nbsp;
        </div>
        <div>
            Dear {FullName},
            <br />
            <br />Thank you so much for your generous donation to the Hand in Hand Hurricane Relief Fund at Comic Relief USA. Your donation comes at a critical moment, and is being rushed to rebuild communities that have been devastated by the recent hurricanes, Harvey and Irma.
            <br/>
            <br/>Your employer may also match your donation; check with your human resources department to see if your company has a matching gift program.
            <br />
            <br />To learn more about the impact your contribution is making, visit comicrelief.org. Or follow Comic Relief USA on social media at @comicreliefusa on 
            Twitter and Instagram - or search "Comic Relief USA" on Facebook.
            <br />            
            <br />&nbsp;
        </div>
        <div>
            <strong>
            <span style="color: #ff0000;">
                THIS IS YOUR RECEIPT - PLEASE RETAIN IT FOR YOUR RECORDS
            </span>
            </strong>
        </div>
        <div>
            <br />Transaction Date: {Date} at {Time}
            <br />Total Gift Amount: {Amount} USD
            <br />Tracking Code: {Confirmation}
            <br />First Name: {FirstName}
            <br />Last Name: {LastName}
            <br />Street: {Address1}
            <br />City: {City}
            <br />State: {State}
            <br />Zip: {Zip}
            <br />&nbsp;
        </div>
        <div>
            If you have any questions regarding your donation, please contact our Donor Services team at <a href="mailto:HandInHand@ComicRelief.org">HandInHand@ComicRelief.org</a> or 1-877-574-2637. Please reference the confirmation number above.
            <br />
            <br />Thank you so much,
            <br />
            <br />The Hand in Hand Team and Comic Relief USA
            <br />&nbsp;
        </div>
        <div id="legal">
            he Hand in Hand Hurricane Relief Fund is a program of Comic Relief Inc., a U.S. 501(c)(3) tax-exempt organization. Comic Relief Inc. affirms that no goods or services were provided in exchange for your contribution. Comic Relief Inc.'s Federal Tax ID Number is 01-0885377. Your donation is tax-deductible to the extent allowed by U.S. law. This receipt is not valid if your donation is refunded for any reason.
            <br />&nbsp;
        </div>
        <div style="text-align: center;">
            <div>
                <a href="https://handinhand2017.com/" target="_blank">
                  