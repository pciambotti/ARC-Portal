<%@ Page Title="Search" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Search.aspx.cs" Inherits="Search" %>
<%@ MasterType TypeName="MasterPage" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="Server">
    <link href="css/content_general.css" rel="stylesheet" type="text/css" />
    <link href="css/user.css" rel="stylesheet" type="text/css" />
    <link href="css/user_list.css" rel="stylesheet" type="text/css" />
    <link href="css/portal_standard.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
    /* css for timepicker */
    .ui-timepicker-div .ui-widget-header { margin-bottom: 8px; }
    .ui-timepicker-div dl { text-align: left; }
    .ui-timepicker-div dl dt { height: 25px; margin-bottom: -25px; }
    .ui-timepicker-div dl dd { margin: 0 10px 10px 65px; }
    #ui-timepicker-div td { font-size: 50%; }
    .ui-tpicker-grid-label { background: none; border: none; margin: 0; padding: 0; }
    .ui-widget{font-size: 12px;}
    .ui-button-text, .ui-button, .ui-button-text {font-size: 10px !important;}

    .ui-selectmenu-menu, .ui-selectmenu, .ui-selectmenu-text {font-size: 10px !important;}
    .wrap {display: inline-block;}
    
    #ui-datepicker-div{font-size: 12px;}
    #ui-datepicker-div{margin-left: 30px;}

    .ui-multiselect-menu
    {
    	width: 275px !important;
    }
    </style>
    <script src="js/jquery-gh-custom-objects.js?v=<%=ghFunctions.portalVersion %>" type="text/javascript"></script>
    <link href="js/jquery-ui-1.11.4.start/jquery-ui.css?v=<%=ghFunctions.portalVersion %>" rel="stylesheet" type="text/css" />
    <link href="css/start/jquery.ui.selectmenu.css?v=<%=ghFunctions.portalVersion %>" rel="stylesheet" type="text/css" />
    <link href="js/jquery.countdown.css?v=<%=ghFunctions.portalVersion %>" rel="stylesheet" type="text/css" />
    <link href="js/jquery.multiselect.css?v=<%=ghFunctions.portalVersion %>" rel="stylesheet" type="text/css" />
    <link href="js/jquery.multiselect.filter.css?v=<%=ghFunctions.portalVersion %>" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="js/jquery.js?v=<%=ghFunctions.portalVersion %>"></script>
    <script type="text/javascript" src="js/jquery-ui-1.11.4.start/jquery-ui.js?v=<%=ghFunctions.portalVersion %>"></script>
    <script src="js/jquery.multiselect.js?v=<%=ghFunctions.portalVersion %>" type="text/javascript"></script>
    <script src="js/jquery.multiselect.filter.js?v=<%=ghFunctions.portalVersion %>" type="text/javascript"></script>
    <script src="js/jquery.plugin.min.js?v=<%=ghFunctions.portalVersion %>" type="text/javascript"></script>
    <script src="js/jquery-ui-timepicker-addon.js?v=<%=ghFunctions.portalVersion %>" type="text/javascript"></script>
    <script src="js/jquery.countdown.min.js?v=<%=ghFunctions.portalVersion %>" type="text/javascript"></script>
    <%--
    <link href="css/start/jquery-ui-1.8.18.custom.css" rel="stylesheet" type="text/css" />
    <link href="css/start/jquery.ui.selectmenu.css" rel="stylesheet" type="text/css" />
    <script src="js/jquery-1.7.1.min.js" type="text/javascript"></script>
    <script src="js/jquery-ui-1.8.18.custom.min.js" type="text/javascript"></script>
    <script src="js/jquery-ui-timepicker-addon.js" type="text/javascript"></script>
    <link href="js/jquery.countdown.css" rel="stylesheet" type="text/css" />
    <script src="js/jquery.plugin.min.js" type="text/javascript"></script>
    <script src="js/jquery.countdown.min.js" type="text/javascript"></script>
    <script src="js/jquery-gh-custom-objects.js" type="text/javascript"></script>
    <link href="js/jquery.multiselect.css" rel="stylesheet" type="text/css" />
    <script src="js/jquery.multiselect.js" type="text/javascript"></script>

    --%>
    <script type="text/javascript">
        function OnLoad_Function() {
            GH_Buttons();
            GH_Select();
            GH_SelectMultiple();
            //GH_DatePicker();
            GH_DatePickerToday();
            $("audio").mediaelementplayer({
                alwaysShowControls: true
            });

            $('.ghDatePickerStart').each(function (i, obj) {
                getMyDay(this);
            });
            $('.ghDatePickerEnd').each(function (i, obj) {
                getMyDay(this);
            });
            $(document).ready(function () {
                $('.multiselectfilter').multiselect().multiselectfilter();
            });

            // $("select").selectmenu({ width: 'auto' });
        }
        function Filter_Toggle(btn, field, nme) {
            $(document).ready(function() {
                //alert(type + "\n\r" + field);
                type = $(btn).val();
                if (type.substring(0, 4) == "Hide") {
                    $("#" + field).hide();
                    $(btn).val("Show " + nme);
                } else {
                    $("#" + field).show();
                    $(btn).val("Hide " + nme);
                }
            });
        }
        function Filter_Toggle_Hide(btn, field, nme) {
            $(document).ready(function() {
                //alert(type + "\n\r" + field);
                $("#" + field).hide();
                $(btn).val("Show " + nme);
            });
        }
        function CountDownStart() {
            $(document).ready(function() {
                var newDate = new Date();
                $('#sinceCountdown').countdown({ since: newDate, compact: true,
                    layout: '{desc}: <b>{hnn}{sep}{mnn}{sep}{snn}</b>',
                    description: 'Elapsed Time'
                });
            });
        }
        function CountDownStop() {
            $(document).ready(function() {
                $('#sinceCountdown').countdown('destroy');
            });
        }
    </script>
    <style type="text/css">
    .filter_label1
    {
    	width: 100px;
    	text-align: right;
    	display: inline-block;
    	font-weight: bold;
    }
    .filter_call_type span
    {
    }
    .reporting_filters h3
    {
    	background-color: #80A2D0;
    	width: 300px;
    	text-align: center;
    	margin-bottom: 10px;
    }
    .filter_section
    {
    	float: left;
    	width: 300px;
    	margin-bottom: 5px;
    	border: solid 1px white;
    	margin-right: 5px;
    	height: 110px;
    }
    .message_high
    {
    	color: Red;
    	font-weight: bold;
    }
    </style>
    <%--
        mp3 player stuff
    --%>
	<script src="js/src/js/me-namespace.js" type="text/javascript"></script>
	<script src="js/src/js/me-utility.js" type="text/javascript"></script>
	<script src="js/src/js/me-i18n.js" type="text/javascript"></script>
	<script src="js/src/js/me-plugindetector.js" type="text/javascript"></script>
	<script src="js/src/js/me-featuredetection.js" type="text/javascript"></script>
	<script src="js/src/js/me-mediaelements.js" type="text/javascript"></script>
	<script src="js/src/js/me-shim.js" type="text/javascript"></script>
	
	<script src="js/src/js/mep-library.js" type="text/javascript"></script>
	<script src="js/src/js/mep-player.js" type="text/javascript"></script>
	<script src="js/src/js/mep-feature-playpause.js" type="text/javascript"></script>
	<script src="js/src/js/mep-feature-progress.js" type="text/javascript"></script>
	<script src="js/src/js/mep-feature-time.js" type="text/javascript"></script>
	<script src="js/src/js/mep-feature-speed.js" type="text/javascript"></script>	
	<script src="js/src/js/mep-feature-tracks.js" type="text/javascript"></script>
	<script src="js/src/js/mep-feature-volume.js" type="text/javascript"></script>
	<script src="js/src/js/mep-feature-stop.js" type="text/javascript"></script>
	<script src="js/src/js/mep-feature-fullscreen.js" type="text/javascript"></script>
	<link rel="stylesheet" href="js/src/css/mediaelementplayer.css" />
	<link rel="stylesheet" href="js/src/css/mejs-skins.css" />	
    <style type="text/css">
        .mejs-container2
        {
            /* To Center the Audio Controls */
            margin: 0px auto;
        }
    </style>
    <style type="text/css">
    .audio_link a {
	    display:block;
	    cursor:pointer;
	    /*position:absolute;*/
	    top:9px;
	    right:0;
	    float:right;
	    height:27px;
	    width:27px;
	    background:url(images/dl.png);
	    text-indent: 100%;
	    white-space: nowrap;
	    overflow: hidden;
    }
    .audio_link a:hover {
	    background-position: 27px 0; 
    }
    </style>
    <%--
        mp3 player stuff
    --%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Master_Content" runat="Server">
    <script type="text/javascript">
        document.body.onload = function() { OnLoad_Function(); }
        Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(BeginRequestHandler);
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(EndRequestHandler);
        function BeginRequestHandler(sender, args) {
            //var elem = args.get_postBackElement();
            //ActivateAlertDiv('visible', 'AlertDiv', elem.value + ' processing...');
            //alert("Updating...");
            CountDownStart();
        }
        function EndRequestHandler(sender, args) {
            //ActivateAlertDiv('hidden', 'AlertDiv', '');
            CountDownStop();
        }
        function ActivateAlertDiv(visstring, elem, msg) {
            var adiv = $get(elem);
            adiv.style.visibility = visstring;
            adiv.innerHTML = msg;
        }
    </script>
    <asp:Panel ID="pnlMain" runat="server" DefaultButton="btnRunReport">
        <div id="default_container">
            <div id="report_header">
                <div id="msgTitle" style="display: inline;width: 50%;color: DarkRed;font-weight: bold;text-align: left;">
                    Greenwood & Hall Client Search Tool
                </div>            
                <div id="msgTitleRight" style="float: right;width: 50%;color: DarkOrange;font-weight: bold;text-align: right;">
                    <asp:Label ID="rpTimeZone" runat="server" Text="Times are UTC Minus the Offset: " />
                </div>
                <hr />
            </div>
            <div id="report_control1">
                <input id="btnFilters01" type="button" value="Hide All Filters" onclick="Filter_Toggle(this,'reporting_filters','All Filters');" />
            </div>
            <div id="reporting_filters" class="reporting_filters">
                <div class="clearfix">
                    <div id="filter_type" class="filter_section">
                        <h3>Donation Type</h3>
                        <div id="call_type">
                            <asp:Label ID="Label2" runat="server" Text="Source" CssClass="filter_label1" />
                            <asp:DropDownList ID="ddlDonationType" runat="server" Width="128px" Width2="250px">
                                <asp:ListItem Value="" Text="All" />
                                <asp:ListItem Value="103010" Text="Call" />
                                <asp:ListItem Value="103020" Text="IVR" />
                            </asp:DropDownList>
                        </div>
                        <div id="call_id">
                            <asp:Label ID="Label1" runat="server" Text="Call ID" CssClass="filter_label1" />
                            <asp:TextBox ID="CallID" runat="server" Width="125px" />
                        </div>
                        <div id="call_status">
                            <asp:Label ID="Label9" runat="server" Text="Disposition" CssClass="filter_label1" />
                            <asp:ListBox ID="ddlDispositions" runat="server" SelectionMode="Multiple" Width="130px" Width2="300px" multiple="multiple" CssClass="multiselect" />
                        </div>
                        <div id="donation_id">
                            <asp:Label ID="Label17" runat="server" Text="Donation ID" CssClass="filter_label1" />
                            <asp:TextBox ID="DonationID" runat="server" Width="125px" />
                        </div>
                    </div>
                    <div id="filter_to" class="filter_section">
                        <h3>Donation Source</h3>
                        <div id="donation_designation">
                            <asp:Label ID="lblDesignation" runat="server" Text="Active Designation" CssClass="filter_label1" />
                            <asp:ListBox ID="ddlDesignation" runat="server" SelectionMode="Multiple" Width="130px" Width2="300px" multiple="multiple" CssClass="multiselect" />
                        </div>
                        <div id="donation_designation_all" runat="server" visible="false">
                            <asp:Label ID="lblDesignationAll" runat="server" Text="Active Designation" CssClass="filter_label1" />
                            <asp:ListBox ID="ddlDesignationAll" runat="server" SelectionMode="Multiple" Width="130px" Width2="300px" multiple="multiple" CssClass="multiselect" />
                        </div>
                        <div id="donation_source" runat="server" visible="false">
                            <asp:Label ID="Label6" runat="server" Text="Donation Source" CssClass="filter_label1" />
                            <asp:DropDownList ID="ddlDonationSource" runat="server" Width="180px" Width2="200px" >
                                <asp:ListItem Value="" Text="All" />
                                <asp:ListItem Value="103201" Text="Web" />
                                <asp:ListItem Value="103202" Text="Phone" />
                                <asp:ListItem Value="103203" Text="Check" />
                                <asp:ListItem Value="103204" Text="Other" />
                                <asp:ListItem Value="103205" Text="IVR" />
                                <asp:ListItem Value="103206" Text="Pledge" />

                            </asp:DropDownList>
                        </div>
                        <div id="donor_type" runat="server" visible="false">
                            <asp:Label ID="Label7" runat="server" Text="Donor Type" CssClass="filter_label1" />
                            <asp:DropDownList ID="ddlDonorType" runat="server" Width="180px" Width2="200px" >
                                <asp:ListItem Value="" Text="All" />
                                <asp:ListItem Value="101010" Text="Web" />
                                <asp:ListItem Value="101020" Text="Phone" />
                                <asp:ListItem Value="101030" Text="Mail" />
                                <asp:ListItem Value="101040" Text="Web - Recurring" />
                            </asp:DropDownList>
                        </div>
                        <div id="donation_status">
                            <asp:Label ID="Label16" runat="server" Text="Status" CssClass="filter_label1" />
                            <asp:DropDownList ID="ddlDonationStatus" runat="server" Width="180px" Width2="200px" >
                                <asp:ListItem Value="" Text="All" />
                                <asp:ListItem Value="Settled" Text="Settled" />
                                <asp:ListItem Value="Approved" Text="Approved" />
                                <asp:ListItem Value="Declined" Text="Declined" />
                                <asp:ListItem Value="Refunded" Text="Refunded" />
                                <asp:ListItem Value="Error" Text="Error" />
                            </asp:DropDownList>
                        </div>
                        <div id="donation_dnis_list">
                            <asp:Label ID="lblDNISList" runat="server" Text="DNIS List" CssClass="filter_label1" />
                            <asp:ListBox ID="ddlDNISList" runat="server" SelectionMode="Multiple" Width="180px" Width2="500px" multiple="multiple" CssClass="multiselect multiselectfilter" />
                        </div>
                    </div>
                    <div id="filter_options" class="filter_section">
                        <h3>Options</h3>
                        <div id="donation_max">
                            <asp:Label ID="Label13" runat="server" Text="Max Records" CssClass="filter_label1" />
                            <asp:DropDownList ID="ddlTop" runat="server" Width="60" Width2="60">
                                <asp:ListItem Text="All" Value="0" />
                                <asp:ListItem Text="25" Value="25" />
                                <asp:ListItem Text="50" Value="50" Selected="True" />
                                <asp:ListItem Text="100" Value="100" />
                                <asp:ListItem Text="250" Value="250" />
                                <asp:ListItem Text="500" Value="500" />
                            </asp:DropDownList>
                        </div>
                        <div>
                            <div style="margin-left: 15px;">
                                It is recommend you use a max if searching for specific records.
                            </div>
                        </div>
                        <div id="donation_exclude_tests">
                            <asp:Label ID="Label18" runat="server" Text="Exclude Test CC" CssClass="filter_label1" Width="125px" />
                            <asp:CheckBox ID="cbTests" runat="server" Checked="true" />
                        </div>
                        <div id="refund_date">
                            <asp:Label ID="Label27" runat="server" Text="Use Refund Dates" CssClass="filter_label1" Width="125px" />
                            <asp:CheckBox ID="cbDateRefund" runat="server" Checked="false" />
                        </div>
                    </div>
                    <div id="filter_options2" class="filter_section">
                        <h3>Options</h3>
                        <div id="donation_sort">
                            <asp:Label ID="lblSort" runat="server" Text="Sort" CssClass="filter_label1" />
                            <asp:DropDownList ID="ddlSort" runat="server" Width="130" Width2="150">
                                <asp:ListItem Value="ASC" Text="Oldest to Newest" Selected="True" />
                                <asp:ListItem Value="DESC" Text="Newest to Oldest" />
                            </asp:DropDownList>
                        </div>
                        <div id="filter_call_length" runat="server" visible="false">
                            <asp:Label ID="lblLength" runat="server" Text="Call Length" CssClass="filter_label1" />
                            <asp:DropDownList ID="ddlLength" runat="server" Width="130" Width2="150">
                                <asp:ListItem Value="0" Text="" Selected="True" />
                                <asp:ListItem Value="5" Text=">= 5 Minutes" />
                                <asp:ListItem Value="10" Text=">= 10 Minutes" />
                                <asp:ListItem Value="15" Text=">= 15 Minutes" />
                                <asp:ListItem Value="20" Text=">= 20 Minutes" />
                            </asp:DropDownList>
                        </div>
                    </div>
                </div>
                <div class="clearfix">
                    <div id="filter_dates" class="filter_section">
                        <h3>Donation Dates</h3>
                        <div id="datetime_start">
                            <asp:Label ID="Label3" runat="server" Text="Start Time" CssClass="filter_label1" />
                            <asp:TextBox ID="dtStartDate" runat="server" Width="75px" CssClass="ghDatePickerStart" onchange="getMyDay(this);" />
                            <asp:TextBox ID="dtStartTime" runat="server" Width="50px" CssClass="ghTimePickerStart" />
                            <div style="position: relative;">
                                <div style="position: absolute;bottom: 2px;left: 242px;">
                                    <asp:Label ID="lblStartDate" runat="server" Text="" CssClass="lblStartDate" />
                                </div>
                            </div>
                        </div>
                        <div id="datetime_end">
                            <asp:Label ID="Label4" runat="server" Text="End Time" CssClass="filter_label1" />
                            <asp:TextBox ID="dtEndDate" runat="server" Width="75px" CssClass="ghDatePickerEnd" onchange="getMyDay(this);" />
                            <asp:TextBox ID="dtEndTime" runat="server" Width="50px" CssClass="ghTimePickerEnd" />
                            <div style="position: relative;">
                                <div style="position: absolute;bottom: 2px;left: 242px;">
                                    <asp:Label ID="lblEndDate" runat="server" Text="" CssClass="lblStartDate" />
                                </div>
                            </div>
                        </div>
                        <div style="text-align: center;">
                            <a href="JavaScript:SelectDate('Yesterday')" style="color: Blue;">Yesterday</a>&nbsp;&nbsp;
                            <a href="JavaScript:SelectDate('Today')" style="color: Blue;">Today</a>&nbsp;&nbsp;
                            <a href="JavaScript:SelectDate('LastWeek')" style="color: Blue;">Last Week</a>&nbsp;&nbsp;
                            <a href="JavaScript:SelectDate('ThisWeek')" style="color: Blue;">This Week</a><br />
                            <a href="JavaScript:SelectDate('LastMonth')" style="color: Blue;">Last Month</a>&nbsp;&nbsp;
                            <a href="JavaScript:SelectDate('ThisMonth')" style="color: Blue;">This Month</a>&nbsp;&nbsp;
                            <a href="JavaScript:SelectDate('YearToDate')" style="color: Blue;">Year To Date</a>&nbsp;&nbsp;
                            <br />
                            <a href="JavaScript:SelectDateVariable('DayPrevious', '<%=dtStartDate.ClientID %>', '<%=dtEndDate.ClientID %>')" style="color: Blue;"><< Previous Day</a>&nbsp;&nbsp;
                            <a href="JavaScript:SelectDateVariable('DayNext', '<%=dtStartDate.ClientID %>', '<%=dtEndDate.ClientID %>')" style="color: Blue;">Next Day >></a>&nbsp;&nbsp;
                        </div>
                    </div>
                    <div id="filter_from" class="filter_section">
                        <h3>Donation Details</h3>
                        <div id="call_from">
                            <asp:Label ID="Label5" runat="server" Text="Name" CssClass="filter_label1" />
                            <asp:TextBox ID="Name" runat="server" Width="125px" />
                        </div>
                        <div id="Div1">
                            <asp:Label ID="Label10" runat="server" Text="Email" CssClass="filter_label1" />
                            <asp:TextBox ID="Email" runat="server" Width="125px" />
                        </div>
                        <div id="Div2">
                            <asp:Label ID="Label11" runat="server" Text="Phone" CssClass="filter_label1" />
                            <asp:TextBox ID="Phone" runat="server" Width="125px" />
                        </div>
                        <div id="Div3">
                            <asp:Label ID="Label12" runat="server" Text="Credit Card" CssClass="filter_label1" />
                            <asp:TextBox ID="Card" runat="server" Width="125px" />
                        </div>
                    </div>
                    <div id="filter_other" class="filter_section">
                        <h3>Other Details</h3>
                        <div id="donation_amount">
                            <asp:Label ID="Label14" runat="server" Text="Amount" CssClass="filter_label1" />
                            <asp:TextBox ID="Amount" runat="server" Width="125px" />
                        </div>
                        <div id="donation_amount_type">
                            <asp:Label ID="Label15" runat="server" Text="Amount Filter" CssClass="filter_label1" />
                            <asp:DropDownList ID="ddlAmountType" runat="server" Width="127" Width2="150">
                                <asp:ListItem Text="Equal to" Value="Equal to" Selected="True" />
                                <asp:ListItem Text="Equal or Greater" Value="Equal or Greater" />
                                <asp:ListItem Text="Equal or Less" Value="Equal or Less" />
                            </asp:DropDownList>
                        </div>
                        <div id="donation_dnis">
                            <asp:Label ID="lblDNIS" runat="server" Text="DNIS" CssClass="filter_label1" />
                            <asp:TextBox ID="DNIS" runat="server" Width="75px" />
                        </div>
                        <div id="donation_adu" runat="server" visible="false">
                            <asp:Label ID="lblADU" runat="server" Text="ADU File" CssClass="filter_label1" />
                            <asp:TextBox ID="ADU" runat="server" Width="150px" />
                        </div>
                    </div>
                    <div id="filter_sustainer" class="filter_section">
                        <h3>Sustainer Records</h3>
                        <div id="recurring_datetime">
                            <asp:Label ID="Label19" runat="server" Text="Transaction Date" CssClass="filter_label1" />
                            <asp:TextBox ID="dtRecurringDate" runat="server" Width="75px" CssClass="ghDatePickerSustainerStart" />
                            <a href="JavaScript:ClearField('<% Response.Write(dtRecurringDate.ClientID); %>')" style="color: Blue;">Clear Value</a>&nbsp;&nbsp;
                            <script type="text/javascript">
                                function ClearField(fld) {
                                    $(document).ready(function () {
                                        $("#" + fld).val("");
                                    });
                                }
                            </script>
                        </div>
                        <div style="color: blue;">
                            If this field has a value, the Donation Date field will be ignored.
                        </div>
                        <div id="blankme" runat="server" visible="false">
                        <div id="recurring_datetime_start">
                            <asp:Label ID="Label21" runat="server" Text="End Time" CssClass="filter_label1" />
                            <asp:TextBox ID="TextBox2" runat="server" Width="75px" CssClass="ghDatePickerEnd clearme" />
                            <asp:TextBox ID="TextBox5" runat="server" Width="50px" CssClass="ghTimePickerEnd clearme" />
                        </div>
                        <div id="recurring_datetime_end">
                            <asp:Label ID="Label20" runat="server" Text="End Time" CssClass="filter_label1" />
                            <asp:TextBox ID="TextBox3" runat="server" Width="75px" CssClass="ghDatePickerEnd clearme" />
                            <asp:TextBox ID="TextBox4" runat="server" Width="50px" CssClass="ghTimePickerEnd clearme" />
                        </div>
                        <div style="text-align: center;">
                            <a href="JavaScript:SelectDateVariable('Yesterday', '<% Response.Write(TextBox2.ClientID); %>', '<% Response.Write(TextBox3.ClientID); %>')" style="color: Blue;">Yesterday</a>&nbsp;&nbsp;
                            <a href="JavaScript:SelectDateVariable('Today', '<% Response.Write(TextBox2.ClientID); %>', '<% Response.Write(TextBox3.ClientID); %>')" style="color: Blue;">Today</a>&nbsp;&nbsp;
                            <a href="JavaScript:SelectDateVariable('LastWeek', '<% Response.Write(TextBox2.ClientID); %>', '<% Response.Write(TextBox3.ClientID); %>')" style="color: Blue;">Last Week</a>&nbsp;&nbsp;
                            <a href="JavaScript:SelectDateVariable('ThisWeek', '<% Response.Write(TextBox2.ClientID); %>', '<% Response.Write(TextBox3.ClientID); %>')" style="color: Blue;">This Week</a><br />
                            <a href="JavaScript:SelectDateVariable('LastMonth', '<% Response.Write(TextBox2.ClientID); %>', '<% Response.Write(TextBox3.ClientID); %>')" style="color: Blue;">Last Month</a>&nbsp;&nbsp;
                            <a href="JavaScript:SelectDateVariable('ThisMonth', '<% Response.Write(TextBox2.ClientID); %>', '<% Response.Write(TextBox3.ClientID); %>')" style="color: Blue;">This Month</a>&nbsp;&nbsp;
                            <a href="JavaScript:SelectDateVariable('YearToDate', '<% Response.Write(TextBox2.ClientID); %>', '<% Response.Write(TextBox3.ClientID); %>')" style="color: Blue;">Year To Date</a>&nbsp;&nbsp;
                        </div>
                        </div>
                    </div>
                </div>
                <div style="color: Blue;">
                    All filter options work together; you can clear any text or date fields to remove them from the query.
                </div>
            </div>
            <div id="search_result" class="clearfix">
                <div id="search_grid" style="display: inline-block;vertical-align: top;">
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                        <Triggers>
                            <asp:PostBackTrigger ControlID="btnExport" />
                        </Triggers>
                        <ContentTemplate>
                        <script type="text/javascript">
                            Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(function(evt, args) { OnLoad_Function(); });
                            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(EndRequestHandler);
                            function EndRequestHandler(sender, args) {
                                var Error = args.get_error();

                                if (Error != null) {
                                    alert("An error occurred while processing request. Please try again.");
                                } else {
                                    $('#<%= btnRunReport.ClientID  %> ').find("[value=0]").attr("disabled", "disabled");
                                    $('#<%= btnRunReport.ClientID  %> ').find("[value=0]").attr("aria-disabled", "true");
                                }
                            }
                        </script>
                        <div>
                            <%--Filter_Toggle_Hide(btnFilters01,'reporting_filters','All Filters');--%>
                            <asp:Button ID="btnRunReport" runat="server"
                                Text="Search"
                                OnClientClick="this.disabled = true; this.value = 'Running...';" 
                                UseSubmitBehavior="false" 
                                OnClick="GridView_Refresh"
                                />
                            <div style="display: inline-block;margin-left: 15px;">
                                <asp:Button ID="btnExport" runat="server" Text="Export to Grid Excel" OnClick="Custom_Export_Excel_SearchGrid" Visible="false" />
                            </div>
                            <div id="msgOther" runat="server" style="display: inline;width: 100%;color: DarkRed;font-weight: bold;">
                                <asp:Label ID="rpMessage" runat="server" Text="" />
                                <br />
                            </div>
                            <div style="min-height: 14px;">
                                <asp:Label ID="rpElapsed" runat="server" Text="" />
                                <asp:Label ID="lblResponse" runat="server" Text="" />
                            </div>
                        </div>
                        <div id="grid" style="display: inline-block;width: 1205px;vertical-align: top;">
                            <asp:GridView ID="gvSearchGrid" runat="server" AutoGenerateColumns="False"
                                BackColor="White"
                                BorderStyle="None"
                                BorderWidth="1px"
                                BorderColor="#DEDFDE"
                                AllowSorting="True"
                                ForeColor="Black"
                                GridLines="Vertical"
                                AllowPaging="true" PageSize="25"
                                DataKeyNames="callid,cbid"

                                OnDataBound="GridView_DataBound"
                                OnRowDataBound="GridView_RowDataBound"
                                OnSelectedIndexChanged="GridView_IndexChanged"
                                OnPageIndexChanging="GridView_PageIndexChanging"

                                CssClass="Portal_GridView_Standard"
                                Width="1200"
                                >
                                <AlternatingRowStyle CssClass="Portal_GridView_Standard_Alternate" />
                                <SelectedRowStyle CssClass="Portal_GridView_Standard_Selected" />
                                <PagerSettings Position="Top" />
                                <PagerStyle VerticalAlign="Bottom" HorizontalAlign="Center" CssClass="Portal_Gridview_Pager" />
                                <RowStyle VerticalAlign="Top" />
                                <PagerTemplate>
                                    <table class="Portal_Gridview_Pager">
                                        <tr>
                                            <td>
                                                <div style="display: inline-block;margin-left: 0px;">
                                                    <asp:Label ID="MessageLabel" runat="server" Text="Page:" ForeColor="Black" Font-Bold="true" />
                                                    <asp:DropDownList ID="gvSearchGridPageDropDownList" runat="server"
                                                      AutoPostBack="true"
                                                      OnSelectedIndexChanged="GridView_PageDropDownList_SelectedIndexChanged" 
                                                      Width="35px" Width2="35px"
                                                    />
                                                </div>
                                                <div style="display: inline-block;margin-left: 15px;">
                                                    <asp:LinkButton ID="lnkFirstPage" CommandName="Page" CommandArgument="First" runat="server">[First]</asp:LinkButton>
                                                    <asp:LinkButton ID="lnkPrevPage"  CommandName="Page" CommandArgument="Prev"  runat="server">[Prev]</asp:LinkButton>
                                                    <asp:LinkButton ID="lnkNextPage"  CommandName="Page" CommandArgument="Next"  runat="server">[Next]</asp:LinkButton>
                                                    <asp:LinkButton ID="lnkLastPage"  CommandName="Page" CommandArgument="Last"  runat="server">[Last]</asp:LinkButton>
                                                </div>
                                                <div style="display: inline-block;margin-left: 15px;">
                                                    <asp:Label ID="CurrentPageLabel" runat="server" Text="" ForeColor="Black" Font-Bold="true" />
                                                </div>
                                            </td>
                                        </tr>                    
                                    </table>
                                </PagerTemplate> 
                                <Columns>
                                    <asp:BoundField HeaderText="DonationID" DataField="DonationID" Visible="false" />
                                    <asp:TemplateField HeaderText="Type" ItemStyle-HorizontalAlign="Left" >
                                        <ItemTemplate>
                                            <asp:Label ID="type" runat="server" Text='<%# Eval("type").ToString() %>' />
                                            <asp:Label ID="agent_name" runat="server" Visible='<%# ghUser.identity_is_admin() %>' Text='<%# "<br />" + de_get_agent_from_callid(Eval("callid").ToString()) %>' ForeColor="Blue" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField HeaderText="ID" DataField="CallID" />
                                    <asp:TemplateField HeaderText="Dialed Line">
                                        <ItemTemplate>
                                            <asp:Label ID="DNIS" runat="server" Text='<%# Eval("DNIS").ToString() %>' />
                                            <asp:Label ID="DNIS_Description" runat="server" Text='<%# Eval("DNIS_Description").ToString().Length > 0 ? "<br />" + Eval("DNIS_Description").ToString() : "" %>' ForeColor="Blue" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Disposition">
                                        <ItemTemplate>
                                            <asp:Label ID="Disposition" runat="server" Text='<%# Eval("disposition").ToString() %>' />
                                            <asp:Label ID="Status" runat="server" Text='<%# (Eval("donation_status").ToString().Length > 0 ? "<br />" + Eval("donation_status").ToString() : "") %>' ForeColor='<%# label_status_color(Eval("donation_status").ToString()) %>' />
                                            <asp:Label ID="Label8" runat="server" Text='<%# (Eval("sustainer_status").ToString().Length > 0 ? "<br />" + Eval("sustainer_status").ToString() : "") %>' ForeColor='<%# label_status_color(Eval("sustainer_status").ToString()) %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Phone">
                                        <ItemTemplate>
                                            <asp:Label ID="Phone" runat="server" Text='<%# label_phone(Eval("callid").ToString(), Eval("phone").ToString(), Eval("ani").ToString()) %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField HeaderText="Name" DataField="Name" />
                                    <asp:TemplateField HeaderText="Address">
                                        <ItemTemplate>
                                            <asp:Label ID="address" runat="server" Text='<%# label_address(Eval("address").ToString(), Eval("city").ToString(), Eval("state").ToString(), Eval("zip").ToString(), Eval("country").ToString()) %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField HeaderText="Amount" DataField="donation_amount" DataFormatString="{0:C}" ItemStyle-HorizontalAlign="Right" />
                                    <asp:TemplateField HeaderText="Created" ItemStyle-CssClass="LastCol" ItemStyle-HorizontalAlign="Right" >
                                        <ItemTemplate>
                                            <asp:Label ID="createdate" runat="server" Text='<%# ghFunctions.date_label_full_short(Eval("call_createdate").ToString()) %>' />
                                            <asp:Label ID="call_length" runat="server" Visible='<%# ghUser.identity_is_admin() %>' Text='<%# "<br />" + ghFunctions.SecondsTo(Eval("time_seconds").ToString()) %>' ForeColor="Blue" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:CommandField SelectText="Select" ShowSelectButton="true" ItemStyle-CssClass="HideButton" HeaderStyle-CssClass="HideButton" />
                                </Columns>
                                <EmptyDataTemplate>No Records For Selected Filters</EmptyDataTemplate>
                            </asp:GridView>
                            <asp:Label ID="lblSearchGrid" runat="server" Text="Label"></asp:Label>
                            <asp:GridView ID="gvSearchExport" runat="server" AutoGenerateColumns="False" Visible="false">
                                <Columns>
                                    <asp:BoundField HeaderText="Type" DataField="type" />
                                    <asp:BoundField HeaderText="Call ID" DataField="callid" />
                                    <asp:BoundField HeaderText="Donation ID" DataField="donationid" />
                                    <asp:BoundField HeaderText="DNIS" DataField="dnis" />
                                    <asp:BoundField HeaderText="Disposition" DataField="disposition" />
                                    <asp:BoundField HeaderText="DispID" DataField="dispositionid" />
                                    <asp:BoundField HeaderText="DNIS Description" DataField="dnis_description" />
                                    <asp:BoundField HeaderText="DNIS Line" DataField="dnis_line" />
                                    <asp:BoundField HeaderText="DNIS Phone" DataField="dnis_phonenumber" />
                                    <asp:TemplateField HeaderText="Phone">
                                        <ItemTemplate>
                                            <asp:Label ID="Phone" runat="server" Text='<%# label_phone(Eval("callid").ToString(), Eval("phone").ToString(), Eval("ani").ToString()) %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField HeaderText="Donation" DataField="donation_status" />
                                    <asp:BoundField HeaderText="Sustainer" DataField="sustainer_status" />
                                    <asp:BoundField HeaderText="Amount" DataField="donation_amount" />

                                    <asp:BoundField HeaderText="Caller Name" DataField="name" />
                                    <asp:BoundField HeaderText="Company" DataField="companyname" />

                                    <asp:BoundField HeaderText="Phone" DataField="phone" />
                                    <asp:BoundField HeaderText="OptIn" DataField="phone_optin" />
                                    <asp:BoundField HeaderText="Type" DataField="phone_type" />

                                    <asp:BoundField HeaderText="Phone2" DataField="phone2" />
                                    <asp:BoundField HeaderText="OptIn" DataField="phone2_optin" />
                                    <asp:BoundField HeaderText="Type" DataField="phone2_type" />

                                    <asp:BoundField HeaderText="Receipt OptIn" DataField="emailreceipt_optin" />
                                    <asp:BoundField HeaderText="Receipt Email" DataField="email" />

                                    <asp:BoundField HeaderText="Email OptIn" DataField="email_optin" />
                                    <asp:BoundField HeaderText="Email" DataField="email2" />

                                    <asp:BoundField HeaderText="Address" DataField="address" />
                                    <asp:BoundField HeaderText="City" DataField="city" />
                                    <asp:BoundField HeaderText="State" DataField="state" />
                                    <asp:BoundField HeaderText="Zip" DataField="zip" />
                                    <asp:BoundField HeaderText="Country" DataField="country" />

                                    <asp:BoundField HeaderText="Sustainer Frequency" DataField="sustainer_frequency" />
                                    <asp:BoundField HeaderText="Sustainer Receipt" DataField="sustainer_receipt_frequency" />
                                    <asp:BoundField HeaderText="Sustainer Start Date" DataField="sustainer_startdate" />
                                    <asp:BoundField HeaderText="Sustainer Processed" DataField="sustainer_processed" />

                                    <asp:BoundField HeaderText="Call Date" DataField="call_createdate" />

                                    <asp:TemplateField HeaderText="Call Length" >
                                        <ItemTemplate>
                                            <asp:Label ID="call_length" runat="server" Visible='<%# ghUser.identity_is_admin() %>' Text='<%# ghFunctions.SecondsTo(Eval("time_seconds").ToString()) %>' ForeColor="Blue" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Agent Name" >
                                        <ItemTemplate>
                                            <asp:Label ID="agent_name" runat="server" Visible='<%# ghUser.identity_is_admin() %>' Text='<%# de_get_agent_from_callid(Eval("callid").ToString()) %>' ForeColor="Blue" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
                <div id="search_details" style="display: block;vertical-align: top;" class="clearfix">
                    <div style="float: left;">
                        <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                            <Triggers>
                                <asp:PostBackTrigger ControlID="btnExportDetails" />
                            </Triggers>
                            <ContentTemplate>
                                <div>
                                    <asp:Button ID="btnExportDetails" runat="server" Text="Export to Details Excel" OnClick="Custom_Export_Excel_Details" Visible="false" />
                                    <div>
                                        <asp:Label ID="lblErrorDV" runat="server" Text="" ForeColor="DarkRed" Font-Bold="true" />
                                    </div>
                                </div>
                                <asp:Panel ID="pnlRecordings" runat="server" CssClass="clearfix" Visible="false">
                                    <div style="background-color: #3A4875;padding: 3px 0px;width: 455px;">
                                        <asp:Label ID="lblRecordingsGrid" runat="server" Text="" Font-Bold="true" ForeColor="White" style="margin-left: 2px;" />
                                    </div>
                                    <asp:GridView ID="gvRecordingsGrid" runat="server" AutoGenerateColumns="False"
                                        BackColor="White"
                                        BorderStyle="None"
                                        BorderWidth="1px"
                                        BorderColor="#DEDFDE"
                                        AllowSorting="True"
                                        ForeColor="Black"
                                        GridLines="Vertical"
                                        AllowPaging="true" PageSize="25"
                                        DataKeyNames="recordingid"

                                        CssClass="Portal_GridView_Standard"
                                        Width="455"
                                        >
                                        <AlternatingRowStyle CssClass="Portal_GridView_Standard_Alternate" />
                                        <SelectedRowStyle CssClass="Portal_GridView_Standard_Selected" />
                                        <PagerSettings Position="Top" />
                                        <PagerStyle VerticalAlign="Bottom" HorizontalAlign="Center" CssClass="Portal_Gridview_Pager" />
                                        <Columns>
                                            <asp:BoundField HeaderText="ID" DataField="recordingid" />
                                            <asp:TemplateField HeaderText="Playback" ItemStyle-Width="340px">
                                                <ItemTemplate>
                                                    <%--
                                                        Tutorial: How To Style the HTML 5 Audio Player
                                                        https://serversideup.net/style-the-html-5-audio-element/
                                                        https://serversideup.net/customize-html-audio-css-amplitudejs/
                                                        https://open.521dimensions.com/amplitudejs
                                                        https://serversideup.net/style-html5-audio-tag-with-amplitudejs-1-building-a-single-song-player/

                                                        Other tutorial
                                                        http://www.catswhocode.com/blog/mastering-the-html5-audio-property
                                                        Simple:
                                                        http://www.alexkatz.me/html5-audio/building-a-custom-html5-audio-player-with-javascript/

                                                    --%>
                                                    <table>
                                                        <tr>
                                                            <td style="border-color: transparent;">
                                                                <audio id="<%# Eval("recordingid") %>" class="recording_player" controls="controls" width="300">
                                                                    <source src='<%# Eval("pathurl") %>' type="audio/mp3" />
                                                                    Browser Compatibility Issue
                                                                </audio>
                                                            </td>
                                                            <td style="border-color: transparent;">
                                                                <div class="audio_link">
                                                                    <a class="" href="<%# Eval("pathurl") %>">Download</a>
                                                                </div>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Date" ItemStyle-CssClass="LastCol" >
                                                <ItemTemplate>
                                                    <asp:Label ID="daterecorded" runat="server" Text='<%# ghFunctions.date_label(Eval("daterecorded").ToString()) %>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:CommandField SelectText="Select" ShowSelectButton="true" ItemStyle-CssClass="HideButton" HeaderStyle-CssClass="HideButton" />
                                        </Columns>
                                    </asp:GridView>
                                </asp:Panel>
                                <div class="clearfix">
                                    <div style="display: inline-block;vertical-align: top;">
                                        <asp:DetailsView ID="dvCallDetails" runat="server" DataKeyNames="callid"
                                            AutoGenerateRows="False" ForeColor="Black" GridLines="Vertical"
                                            HeaderText="Donation Details"
                                            CssClass="User_List_Details_Border"
                                            Width="400px"
                                            >
                                            <HeaderStyle CssClass="User_List_Details_Header" />
                                            <RowStyle BackColor="#CCCCCC" CssClass="User_List_Details_Row" />
                                            <FieldHeaderStyle Width="100px" />
                                            <EmptyDataRowStyle BackColor="White" />
                                            <Fields>
                                                <asp:BoundField HeaderText="CallID" DataField="callid" ReadOnly="true" />
                                                <asp:BoundField HeaderText="DonationID" DataField="donationid" ReadOnly="true" />
                                                <asp:BoundField HeaderText="Name" DataField="Name" ReadOnly="true" />
                                                <asp:BoundField HeaderText="Type" DataField="Type" ReadOnly="true" />
                                                <asp:TemplateField HeaderText="Amount">
                                                    <ItemTemplate>
			                                            <asp:Label ID="lblAmount" runat="server" Text='<%# label_amount_convert(Eval("amount").ToString(), Eval("amount_ref").ToString()) %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField HeaderText="Designation" DataField="DisplayName" ReadOnly="true" />
                                                <asp:BoundField HeaderText="DesignationID" DataField="DesignationID" ReadOnly="true" />
                                                <asp:BoundField HeaderText="DispositionID" DataField="DispositionID" ReadOnly="true" />
                                                <asp:BoundField HeaderText="DNIS" DataField="DNIS" ReadOnly="true" />
                                                <asp:BoundField HeaderText="ANI" DataField="ANI" ReadOnly="true" />
                                                <asp:TemplateField HeaderText="Call Length" >
                                                    <ItemTemplate>
                                                        <asp:Label ID="call_length" runat="server" Text='<%# ghFunctions.date_label_length(Eval("call_createdate").ToString(), Eval("call_enddate").ToString()) %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Created" >
                                                    <ItemTemplate>
                                                        <asp:Label ID="call_createdate" runat="server" Text='<%# ghFunctions.date_label_full(Eval("call_createdate").ToString()) %>' /> Offset
                                                        <br /><asp:Label ID="call_createdate_utc" runat="server" Text='<%# ghFunctions.date_label_full_utc(Eval("call_createdate").ToString()) %>' /> Original Time
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Ended" >
                                                    <ItemTemplate>
                                                        <asp:Label ID="call_enddate" runat="server" Text='<%# ghFunctions.date_label_full(Eval("call_enddate").ToString()) %>' /> Offset
                                                        <br /><asp:Label ID="call_enddate_utc" runat="server" Text='<%# ghFunctions.date_label_full_utc(Eval("call_enddate").ToString()) %>' /> Original Time
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Fields>
                                            <EmptyDataTemplate>No [Call] details;</EmptyDataTemplate>
                                        </asp:DetailsView>
                                        <div style="margin-top: 5px;">
                                            <asp:DetailsView ID="dvPaymentDetails" runat="server" DataKeyNames="callid,cbid"
                                                AutoGenerateRows="False" ForeColor="Black" GridLines="Vertical"
                                                HeaderText="Payment Details"
                                                CssClass="User_List_Details_Border"
                                                OnItemCommand="DetailsView_ItemCommand"
                                                Width="400px"
                                                >
                                                <HeaderStyle CssClass="User_List_Details_Header" />
                                                <RowStyle BackColor="#CCCCCC" CssClass="User_List_Details_Row" />
                                                <FieldHeaderStyle Width="100px" />
                                                <EmptyDataRowStyle BackColor="White" />
                                                <Fields>
                                                    <asp:BoundField HeaderText="Transaction Type" DataField="donationtype" ReadOnly="true" />
                                                    <asp:BoundField HeaderText="Payment Type" DataField="paymenttype" ReadOnly="true" />
                                                    <asp:BoundField HeaderText="CallID" DataField="CallID" ReadOnly="true" Visible="false" />
                                                    <asp:TemplateField HeaderText="Transaction">
                                                        <ItemTemplate>
                                                            <div style="display: inline;">
			                                                    <asp:Label ID="lbl_cbid" runat="server" Text='<%# Eval("cbid") %>' />
                                                            </div>
                                                            <div style="display: inline;margin-left: 10px;">
                                                                <asp:LinkButton ID="btnDVRefund" CommandName="Refund" CommandArgument="Refund" runat="server" Visible='<%# refund_visible_button(Eval("status").ToString(),Eval("decision").ToString())%>'>Refund</asp:LinkButton>
                                                                <asp:LinkButton ID="btnDVCharge" CommandName="Charge" CommandArgument="Charge" runat="server" Visible='<%# charge_visible_button(Eval("status").ToString(),Eval("decision").ToString())%>'>Charge</asp:LinkButton>
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:BoundField HeaderText="status" DataField="status" ReadOnly="true" />
                                                    <asp:BoundField HeaderText="decision" DataField="decision" ReadOnly="true" />
                                                    <asp:BoundField HeaderText="reasoncode" DataField="reasoncode" ReadOnly="true" />
                                                    <asp:BoundField HeaderText="reasondescription" DataField="reasondescription" ReadOnly="true" />
                                                    <asp:BoundField HeaderText="authed" DataField="authed" ReadOnly="true" />
                                                    <asp:BoundField HeaderText="cctype" DataField="cctype" ReadOnly="true" />
                                                    <asp:BoundField HeaderText="ccnum" DataField="ccnum" ReadOnly="true" />
                                                    <asp:BoundField HeaderText="Reference" DataField="merchantreferencecode" ReadOnly="true" />
                                                    <asp:BoundField HeaderText="Response" DataField="ccContent" ReadOnly="true" />
                                                    <asp:TemplateField HeaderText="Created" >
                                                        <ItemTemplate>
                                                            <asp:Label ID="call_createdate" runat="server" Text='<%# ghFunctions.date_label_full(Eval("CreateDate").ToString()) %>' /> Offset
                                                            <br /><asp:Label ID="call_createdate_utc" runat="server" Text='<%# ghFunctions.date_label_full_utc(Eval("CreateDate").ToString()) %>' /> Original Time
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Fields>
                                                <EmptyDataTemplate>No [Payment] details;</EmptyDataTemplate>
                                            </asp:DetailsView>
                                            <asp:HiddenField ID="hdPDcybID" runat="server" />
                                            <asp:HiddenField ID="hdPDcallID" runat="server" />
                                        </div>
                                    </div>
                                    <div style="display: inline-block;vertical-align: top;">
                                        <asp:DetailsView ID="dvDonorDetails" runat="server" DataKeyNames="CallID"
                                            AutoGenerateRows="False" ForeColor="Black" GridLines="Vertical"
                                            HeaderText="Donor Details"
                                            CssClass="User_List_Details_Border"
                                            OnItemCommand="DetailsView_ItemCommand"
                                            Width="400px"

                            OnDataBound="DetailsView_DataBound"
                            OnModeChanging="DetailsView_ModeChanging"
                            OnItemUpdating="DetailsView_ItemUpdating"
                            OnItemUpdated="DetailsView_ItemUpdated"

                                            >
                                            <HeaderStyle CssClass="User_List_Details_Header" />
                                            <RowStyle BackColor="#CCCCCC" CssClass="User_List_Details_Row" />
                                            <FieldHeaderStyle Width="100px" />
                                            <EmptyDataRowStyle BackColor="White" />
                                            <Fields>
                                                <asp:TemplateField HeaderText="CallID">
                                                    <ItemTemplate>
                                                        <div style="display: inline;">
			                                                <asp:Label Font-Bold="true" ID="callid" runat="server" Text='<%# Eval("CallID") %>' />
                                                        </div>
                                                        <div style="display: inline;margin-left: 10px;">
                                                            <span style="margin-right: 25px;">
                                                                <asp:Button ID="donor_edit" runat="server" CommandName="Edit" Text="Edit" Visible='<%# ghUser.identity_is_admin() %>' />
                                                            </span>
                                                        </div>
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <div style="width: 300px;margin: 10px 0px;">
			                                                <asp:Label Font-Bold="true" ID="callid" runat="server" Text='<%# Eval("CallID") %>' />
                                                            <asp:Button ID="donor_update" runat="server" CommandName="Update" Text="Save" ValidationGroup="vsDonor" />
                                                            <asp:Button ID="donor_cancel" runat="server" CommandName="Cancel" Text="Cancel" />
                                                            <div style="margin-left: 15px;">
                                                                <asp:Label ID="lblSummary" runat="server" Text="" />
                                                                <asp:ValidationSummary ID="summaryDonor" ValidationGroup="vsDonor" runat="server" />
                                                            </div>
                                                        </div>
                                                    </EditItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField HeaderText="Amount" DataField="Amount" ReadOnly="true" />
                                                <asp:BoundField HeaderText="Type" DataField="type" ReadOnly="true" />
                                                <asp:TemplateField HeaderText="Name">
                                                    <ItemTemplate>
                                                        <asp:Label ID="name" runat="server" Text='<%# Eval("fname") + " " + Eval("lname") %>' />
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:TextBox ID="fname" runat="server" Text='<%# Eval("fname") %>' Width="100px" />
                                                        <asp:TextBox ID="lname" runat="server" Text='<%# Eval("lname") %>' Width="100px" />
                                                        <asp:RequiredFieldValidator ValidationGroup="vsDonorDetails" ID="vsfname" runat="server" ControlToValidate="fname" Text="*" ErrorMessage="First Name is required" SetFocusOnError="false" />
                                                        <asp:RequiredFieldValidator ValidationGroup="vsDonorDetails" ID="vslname" runat="server" ControlToValidate="lname" Text="*" ErrorMessage="First Name is required" SetFocusOnError="false" />
                                                    </EditItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Address">
                                                    <ItemTemplate>
                                                        <asp:Label ID="Address" runat="server" Text='<%# Eval("Address") %>' />
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:TextBox ID="Address" runat="server" Text='<%# Eval("Address") %>' />
                                                        <asp:RequiredFieldValidator ValidationGroup="vsDonorDetails" ID="vsAddress" runat="server" ControlToValidate="Address" Text="*" ErrorMessage="Address is required" SetFocusOnError="false" />
                                                    </EditItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="SuiteNumber">
                                                    <ItemTemplate>
                                                        <asp:Label ID="SuiteNumber" runat="server" Text='<%# Eval("SuiteNumber") %>' />
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:TextBox ID="SuiteNumber" runat="server" Text='<%# Eval("SuiteNumber") %>' />
                                                    </EditItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Zip">
                                                    <ItemTemplate>
                                                        <asp:Label ID="Zip" runat="server" Text='<%# Eval("Zip") %>' />
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:TextBox ID="Zip" runat="server" Text='<%# Eval("Zip") %>' />
                                                        <asp:RequiredFieldValidator ValidationGroup="vsDonorDetails" ID="vsZip" runat="server" ControlToValidate="Zip" Text="*" ErrorMessage="Zip is required" SetFocusOnError="false" />
                                                    </EditItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="City">
                                                    <ItemTemplate>
                                                        <asp:Label ID="City" runat="server" Text='<%# Eval("City") %>' />
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:TextBox ID="City" runat="server" Text='<%# Eval("City") %>' />
                                                        <asp:RequiredFieldValidator ValidationGroup="vsDonorDetails" ID="vsCity" runat="server" ControlToValidate="City" Text="*" ErrorMessage="City is required" SetFocusOnError="false" />
                                                    </EditItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="State">
                                                    <ItemTemplate>
                                                        <asp:Label ID="State" runat="server" Text='<%# Eval("State") %>' />
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:TextBox ID="State" runat="server" Text='<%# Eval("State") %>' />
                                                        <asp:RequiredFieldValidator ValidationGroup="vsDonorDetails" ID="vsState" runat="server" ControlToValidate="State" Text="*" ErrorMessage="State is required" SetFocusOnError="false" />
                                                    </EditItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Country">
                                                    <ItemTemplate>
                                                        <asp:Label ID="Country" runat="server" Text='<%# Eval("Country") %>' />
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:TextBox ID="Country" runat="server" Text='<%# Eval("Country") %>' />
                                                        <asp:RequiredFieldValidator ValidationGroup="vsDonorDetails" ID="vsCountry" runat="server" ControlToValidate="Country" Text="*" ErrorMessage="Country is required" SetFocusOnError="false" />
                                                    </EditItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Phone">
                                                    <ItemTemplate>
                                                        <asp:Label ID="hphone" runat="server" Text='<%# Eval("hphone") %>' />
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:TextBox ID="hphone" runat="server" Text='<%# Eval("hphone") %>' />
                                                        <asp:RequiredFieldValidator ValidationGroup="vsDonorDetails" ID="vshphone" runat="server" ControlToValidate="hphone" Text="*" ErrorMessage="Phone is required" SetFocusOnError="false" />
                                                    </EditItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Email" Visible="false">
                                                    <ItemTemplate>
                                                        <asp:Label ID="ackaddress" runat="server" Text='<%# Eval("ackaddress") %>' />
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:TextBox ID="ackaddress" runat="server" Text='<%# Eval("ackaddress") %>' />
                                                    </EditItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Email">
                                                    <ItemTemplate>
                                                        <asp:Label ID="Email" runat="server" Text='<%# Eval("Email") %>' />
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:TextBox ID="Email" runat="server" Text='<%# Eval("Email") %>' />
                                                    </EditItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Phone Type">
                                                    <ItemTemplate>
                                                        <asp:Label ID="phone_type" runat="server" Text='<%# Eval("phone_type") %>' />
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:TextBox ID="phone_type" runat="server" Text='<%# Eval("phone_type") %>' />
                                                    </EditItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Phone OptIn">
                                                    <ItemTemplate>
                                                        <asp:Label ID="phone_optin" runat="server" Text='<%# Eval("phone_optin") %>' />
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:CheckBox ID="phone_optin" runat="server" Checked='<%# Eval("phone_optin") %>' />
                                                    </EditItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Phone 2">
                                                    <ItemTemplate>
                                                        <asp:Label ID="phone2" runat="server" Text='<%# Eval("phone2") %>' />
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:TextBox ID="phone2" runat="server" Text='<%# Eval("phone2") %>' />
                                                    </EditItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Phone 2 Type">
                                                    <ItemTemplate>
                                                        <asp:Label ID="phone2_type" runat="server" Text='<%# Eval("phone2_type") %>' />
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:TextBox ID="phone2_type" runat="server" Text='<%# Eval("phone2_type") %>' />
                                                    </EditItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Phone 2 OptIn">
                                                    <ItemTemplate>
                                                        <asp:Label ID="phone2_optin" runat="server" Text='<%# Eval("phone2_optin") %>' />
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:CheckBox ID="phone2_optin" runat="server" Checked='<%# Eval("phone2_optin") %>' />
                                                    </EditItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField HeaderText="" DataField="email2" ReadOnly="true" />
                                                <asp:TemplateField HeaderText="Email 2">
                                                    <ItemTemplate>
                                                        <asp:Label ID="email2" runat="server" Text='<%# Eval("email2") %>' />
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:TextBox ID="email2" runat="server" Text='<%# Eval("email2") %>' />
                                                    </EditItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Email Receipt">
                                                    <ItemTemplate>
                                                        <asp:Label ID="receipt_email" runat="server" Text='<%# Eval("receipt_email") %>' />
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:CheckBox ID="receipt_email" runat="server" Checked='<%# Eval("receipt_email") %>' />
                                                    </EditItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Company">
                                                    <ItemTemplate>
                                                        <asp:Label ID="companyName" runat="server" Text='<%# Eval("companyName") %>' />
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:TextBox ID="companyName" runat="server" Text='<%# Eval("companyName") %>' />
                                                    </EditItemTemplate>
                                                </asp:TemplateField>
                                            </Fields>
                                            <EmptyDataTemplate>No [Call Info] details;</EmptyDataTemplate>
                                        </asp:DetailsView>
                                        <div>
                                            <asp:Label ID="lblDonorDetails" runat="server" Text="" />
                                        </div>
                                        <div style="margin-top: 5px;">
                                            <asp:DetailsView ID="dvHolidayAddress" runat="server" DataKeyNames="callid"
                                                AutoGenerateRows="False" ForeColor="Black" GridLines="Vertical"
                                                HeaderText="Holiday Shipping Address Details"
                                                CssClass="User_List_Details_Border"
                                                OnItemCommand="DetailsView_ItemCommand"
                                                Width="400px"
                                                >
                                                <HeaderStyle CssClass="User_List_Details_Header" />
                                                <RowStyle BackColor="#CCCCCC" CssClass="User_List_Details_Row" />
                                                <FieldHeaderStyle Width="100px" />
                                                <EmptyDataRowStyle BackColor="White" />
                                                <Fields>
                                                    <asp:BoundField HeaderText="First Name" DataField="fname" ReadOnly="true" />
                                                    <asp:BoundField HeaderText="Last Name" DataField="lname" ReadOnly="true" />
                                                    <asp:BoundField HeaderText="Prefix" DataField="prefix" ReadOnly="true" />                                                
                                                    <asp:BoundField HeaderText="Company" DataField="companyname" ReadOnly="true" />
                                                    <asp:BoundField HeaderText="Company Type" DataField="companytype" ReadOnly="true" />
                                                    <asp:BoundField HeaderText="Address" DataField="address" ReadOnly="true" />
                                                    <asp:BoundField HeaderText="Suite Type" DataField="suitetype" ReadOnly="true" />
                                                    <asp:BoundField HeaderText="Suite Number" DataField="suitenumber" ReadOnly="true" />
                                                    <asp:BoundField HeaderText="Zip" DataField="zip" ReadOnly="true" />
                                                    <asp:BoundField HeaderText="City" DataField="city" ReadOnly="true" />
                                                    <asp:BoundField HeaderText="State" DataField="state" ReadOnly="true" />
                                                    <asp:BoundField HeaderText="Country" DataField="country" ReadOnly="true" />
                                                </Fields>
                                                <EmptyDataTemplate>No [Alternate Address] details;</EmptyDataTemplate>
                                            </asp:DetailsView>
                                        </div>
                                        <div style="margin-top: 5px;">
                                            <asp:DetailsView ID="dvTributeDetails" runat="server" DataKeyNames="CallID"
                                                AutoGenerateRows="False" ForeColor="Black" GridLines="Vertical"
                                                HeaderText="Tribute Details"
                                                CssClass="User_List_Details_Border"

                                                Width="400px"
                                                >
                                                <HeaderStyle CssClass="User_List_Details_Header" />
                                                <RowStyle BackColor="#CCCCCC" CssClass="User_List_Details_Row" />
                                                <FieldHeaderStyle Width="100px" />
                                                <EmptyDataRowStyle BackColor="White" />
                                                <Fields>
                                                    <asp:BoundField HeaderText="callid" DataField="callid" ReadOnly="true" />
                                                    <asp:BoundField HeaderText="type" DataField="type" ReadOnly="true" />
                                                    <asp:BoundField HeaderText="imoihoid" DataField="imoihoid" ReadOnly="true" />
                                                    <asp:BoundField HeaderText="postmarkdate" DataField="postmarkdate" ReadOnly="true" />
                                                    <asp:BoundField HeaderText="honmemdesc" DataField="honmemdesc" ReadOnly="true" />
                                                    <asp:BoundField HeaderText="honmemtitle" DataField="honmemtitle" ReadOnly="true" />
                                                    <asp:BoundField HeaderText="honmemfirstname" DataField="honmemfirstname" ReadOnly="true" />
                                                    <asp:BoundField HeaderText="honmemmiddlename" DataField="honmemmiddlename" ReadOnly="true" />
                                                    <asp:BoundField HeaderText="honmemlastname" DataField="honmemlastname" ReadOnly="true" />
                                                    <asp:BoundField HeaderText="honmemsuffix" DataField="honmemsuffix" ReadOnly="true" />
                                                    <asp:BoundField HeaderText="honmemstreet" DataField="honmemstreet" ReadOnly="true" />
                                                    <asp:BoundField HeaderText="honmemaddr2" DataField="honmemaddr2" ReadOnly="true" />
                                                    <asp:BoundField HeaderText="honmemaddr3" DataField="honmemaddr3" ReadOnly="true" />
                                                    <asp:BoundField HeaderText="honmemcity" DataField="honmemcity" ReadOnly="true" />
                                                    <asp:BoundField HeaderText="honmemstate" DataField="honmemstate" ReadOnly="true" />
                                                    <asp:BoundField HeaderText="honmemzip" DataField="honmemzip" ReadOnly="true" />
                                                    <asp:BoundField HeaderText="honmemcountry" DataField="honmemcountry" ReadOnly="true" />
                                                    <asp:BoundField HeaderText="honmemcountryzip" DataField="honmemcountryzip" ReadOnly="true" />
                                                    <asp:BoundField HeaderText="honormemorium" DataField="honormemorium" ReadOnly="true" />
                                                    <asp:BoundField HeaderText="honmemoccation" DataField="honmemoccation" ReadOnly="true" />
                                                    <asp:BoundField HeaderText="honmemfirstname2" DataField="honmemfirstname2" ReadOnly="true" />
                                                    <asp:BoundField HeaderText="honmemlastname2" DataField="honmemlastname2" ReadOnly="true" />
                                                    <asp:BoundField HeaderText="closing" DataField="closing" ReadOnly="true" />
                                                    <asp:BoundField HeaderText="signature" DataField="signature" ReadOnly="true" />
                                                </Fields>
                                                <EmptyDataTemplate>No [Tribute] details;</EmptyDataTemplate>
                                            </asp:DetailsView>
                                        </div>
                                    </div>
                                </div>
                                <div class="clearfix">
                                    <div style="display: inline-block;vertical-align: top;margin-top: 5px;">
                                        <asp:DetailsView ID="dvContactDetails" runat="server" DataKeyNames="CallID"
                                            AutoGenerateRows="True" ForeColor="Black" GridLines="Vertical"
                                            HeaderText="Donor Contact"
                                            CssClass="User_List_Details_Border"

                                            Width="400px"
                                            >
                                            <HeaderStyle CssClass="User_List_Details_Header" />
                                            <RowStyle BackColor="#CCCCCC" CssClass="User_List_Details_Row" />
                                            <FieldHeaderStyle Width="100px" />
                                            <EmptyDataRowStyle BackColor="White" />
                                            <Fields>
                                            </Fields>
                                            <EmptyDataTemplate>No [Contact] details;</EmptyDataTemplate>
                                        </asp:DetailsView>
                                        <div style="margin-top: 5px;">
                                            <asp:DetailsView ID="dvRefundDetails" runat="server" DataKeyNames="CallID"
                                                AutoGenerateRows="False" ForeColor="Black" GridLines="Vertical"
                                                HeaderText="Refund Details"
                                                CssClass="User_List_Details_Border"

                                                Width="400px"
                                                >
                                                <HeaderStyle CssClass="User_List_Details_Header" />
                                                <RowStyle BackColor="#CCCCCC" CssClass="User_List_Details_Row" />
                                                <FieldHeaderStyle Width="100px" />
                                                <EmptyDataRowStyle BackColor="White" />
                                                <Fields>
                                                    <asp:BoundField HeaderText="CallID" DataField="CallID" ReadOnly="true" />
                                                    <asp:BoundField HeaderText="Type" DataField="Type" ReadOnly="true" />
                                                    <asp:BoundField HeaderText="Amount" DataField="Amount" ReadOnly="true" />
                                                    <asp:BoundField HeaderText="User" DataField="user" ReadOnly="true" />
                                                    <asp:BoundField HeaderText="Reason" DataField="reason" ReadOnly="true" />
                                                    <asp:BoundField HeaderText="Status" DataField="status" ReadOnly="true" />
                                                    <asp:BoundField HeaderText="RefundDate" DataField="createdate" ReadOnly="true" />
                                                    <asp:BoundField HeaderText="Decision" DataField="decision" ReadOnly="true" />
                                                    <asp:BoundField HeaderText="Reference" DataField="merchantreferencecode" ReadOnly="true" />
                                                    <asp:BoundField HeaderText="Response" DataField="ccContent" ReadOnly="true" />
                                                </Fields>
                                                <EmptyDataTemplate>No [Refund] details;</EmptyDataTemplate>
                                            </asp:DetailsView>
                                        </div>
                                    </div>
                                    <div style="display: inline-block;vertical-align: top;margin-top: 5px;">
                                        <asp:DetailsView ID="dvADUFile" runat="server" DataKeyNames="CallID"
                                            AutoGenerateRows="False" ForeColor="Black" GridLines="Vertical"
                                            HeaderText="ADU File"
                                            CssClass="User_List_Details_Border"

                                            Width="400px"
                                            >
                                            <HeaderStyle CssClass="User_List_Details_Header" />
                                            <RowStyle BackColor="#CCCCCC" CssClass="User_List_Details_Row" />
                                            <FieldHeaderStyle Width="100px" />
                                            <EmptyDataRowStyle BackColor="White" />
                                            <Fields>
                                                <asp:BoundField HeaderText="File Name" DataField="FileUpToARC" ReadOnly="true" />
                                            </Fields>
                                            <EmptyDataTemplate>No [ADU] details;</EmptyDataTemplate>
                                        </asp:DetailsView>
                                        <div style="margin-top: 5px;">
                                            <asp:DetailsView ID="dvRemoveDetails" runat="server" DataKeyNames="CallID"
                                                AutoGenerateRows="False" ForeColor="Black" GridLines="Vertical"
                                                HeaderText="Donor Details"
                                                CssClass="User_List_Details_Border"

                                                Width="400px"
                                                >
                                                <HeaderStyle CssClass="User_List_Details_Header" />
                                                <RowStyle BackColor="#CCCCCC" CssClass="User_List_Details_Row" />
                                                <FieldHeaderStyle Width="100px" />
                                                <EmptyDataRowStyle BackColor="White" />
                                                <Fields>
                                                    <asp:TemplateField HeaderText="Name">
                                                        <ItemTemplate>
                                                            <asp:Label ID="name" runat="server" Text='<%# Eval("fname") + " " + Eval("lname") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:BoundField HeaderText="Address" DataField="address" ReadOnly="true" />
                                                    <asp:BoundField HeaderText="SuiteNumber" DataField="suitenumber" ReadOnly="true" />
                                                    <asp:BoundField HeaderText="Zip" DataField="zip" ReadOnly="true" />
                                                    <asp:BoundField HeaderText="City" DataField="city" ReadOnly="true" />
                                                    <asp:BoundField HeaderText="State" DataField="state" ReadOnly="true" />
                                                    <asp:BoundField HeaderText="Phone" DataField="hphone" ReadOnly="true" />
                                                    <asp:BoundField HeaderText="NO Mail" DataField="no_mail" ReadOnly="true" />
                                                    <asp:BoundField HeaderText="NO Phone" DataField="no_phone" ReadOnly="true" />
                                                    <asp:BoundField HeaderText="NO Email" DataField="no_email" ReadOnly="true" />
                                                    <asp:BoundField HeaderText="Email OptIn" DataField="email_optin" ReadOnly="true" />
                                                    <asp:BoundField HeaderText="Email" DataField="email" ReadOnly="true" />
                                                </Fields>
                                                <EmptyDataTemplate>No [Remove] details;</EmptyDataTemplate>
                                            </asp:DetailsView>
                                        </div>
                                    </div>
                                </div>
                                <div class="clearfix">
                                    <div style="display: inline-block;vertical-align: top;margin-top: 5px;">
                                        <asp:DetailsView ID="dvGiftDetails" runat="server" DataKeyNames="CallID"
                                            AutoGenerateRows="True" ForeColor="Black" GridLines="Vertical"
                                            HeaderText="Gift Details"
                                            CssClass="User_List_Details_Border"

                                            Width="400px"
                                            >
                                            <HeaderStyle CssClass="User_List_Details_Header" />
                                            <RowStyle BackColor="#CCCCCC" CssClass="User_List_Details_Row" />
                                            <FieldHeaderStyle Width="100px" />
                                            <EmptyDataRowStyle BackColor="White" />
                                            <Fields>
                                            </Fields>
                                            <EmptyDataTemplate>No [Gift] details;</EmptyDataTemplate>
                                        </asp:DetailsView>
                                    </div>
                                    <asp:Panel style="display: inline-block;vertical-align: top;" id="pnlGiftList" runat="server">
                                        <div>
                                            <h3 id="lblGiftList" runat="server" visible="true">Gift List</h3>
                                            <asp:GridView ID="gvGiftList" runat="server" AutoGenerateColumns="False"
                                                BackColor="White"
                                                BorderStyle="None"
                                                BorderWidth="1px"
                                                BorderColor="#DEDFDE"
                                                AllowSorting="True"
                                                ForeColor="Black"
                                                GridLines="Vertical"
                                                AllowPaging="true" PageSize="25"

                                                CssClass="Portal_GridView_Standard"
                                                Width="400"
                                                >
                                                <AlternatingRowStyle CssClass="Portal_GridView_Standard_Alternate" />
                                                <SelectedRowStyle CssClass="Portal_GridView_Standard_Selected" />
                                                <PagerSettings Position="Top" />
                                                <PagerStyle VerticalAlign="Bottom" HorizontalAlign="Center" CssClass="Portal_Gridview_Pager" />
                                                <Columns>
                                                    <asp:BoundField HeaderText="id" DataField="giftcatalogselectedid" />
                                                    <%--<asp:BoundField HeaderText="selectedid" DataField="selectedoptionid" Visible="false" />--%>
                                                    <asp:BoundField HeaderText="skuname" DataField="skuname" />
                                                    <%--<asp:BoundField HeaderText="sku" DataField="sku" Visible="false" />--%>
                                                    <asp:BoundField HeaderText="quantity" DataField="quantity" />
                                                    <asp:BoundField HeaderText="amount" DataField="amount" />
                                                </Columns>
                                                <EmptyDataTemplate>No [Gifts] details;</EmptyDataTemplate>
                                            </asp:GridView>
                                        </div>
                                    </asp:Panel>
                                </div>
                                <div class="clearfix">
                                    <div style="display: inline-block;vertical-align: top;margin-top: 5px;">
                                        <asp:DetailsView ID="dvSustainerDetails" runat="server" DataKeyNames="callid"
                                            AutoGenerateRows="False" ForeColor="Black" GridLines="Vertical"
                                            HeaderText="Sustainer Details"
                                            CssClass="User_List_Details_Border"
                                            OnItemCommand="DetailsView_ItemCommand"
                                            Width="400px"
                                            >
                                            <HeaderStyle CssClass="User_List_Details_Header" />
                                            <RowStyle BackColor="#CCCCCC" CssClass="User_List_Details_Row" />
                                            <FieldHeaderStyle Width="100px" />
                                            <EmptyDataRowStyle BackColor="White" />
                                            <Fields>
                                                <asp:TemplateField HeaderText="CallID">
                                                    <ItemTemplate>
                                                        <div style="display: inline;">
			                                                <asp:Label ID="lbl_cbid" runat="server" Text='<%# Eval("CallID") %>' />
                                                            <asp:HiddenField ID="callid" runat="server" Value='<%# Eval("callid") %>' />
                                                            <asp:HiddenField ID="donationid" runat="server" Value='<%# Eval("donationid") %>' />
                                                            <asp:HiddenField ID="current_status" runat="server" Value='<%# Eval("status") %>' />
                                                            <asp:HiddenField ID="current_date" runat="server" Value='<%# Eval("chargedate") %>' />
                                                        </div>
                                                        <div style="display: inline;margin-left: 10px;">
                                                            <asp:LinkButton ID="btnDVRefund" CommandName="Modify" CommandArgument="Modify" runat="server" Visible='<%# ghUser.identity_is_admin() %>'>Modify</asp:LinkButton>
                                                        </div>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField HeaderText="DonationID" DataField="DonationID" ReadOnly="true" />
                                                <asp:TemplateField HeaderText="Status">
                                                    <ItemTemplate>
                                                        <asp:Label ID="status" runat="server" Text='<%# ghFunctions.status_recurring_record(Eval("status").ToString()) %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField HeaderText="Frequency" DataField="Frequency" ReadOnly="true" />
                                                <asp:BoundField HeaderText="ChargeDate" DataField="ChargeDate" ReadOnly="true" />
                                                <asp:BoundField HeaderText="StartDate" DataField="StartDate" ReadOnly="true" />
                                                <asp:BoundField HeaderText="ReceiptFrequency" DataField="receiptfrequency" ReadOnly="true" />
                                                <asp:BoundField HeaderText="Processed" DataField="processed" ReadOnly="true" />
                                                <asp:BoundField HeaderText="ProcessedStatus" DataField="processedstatus" ReadOnly="true" />
                                                <asp:TemplateField HeaderText="ProcessedStatus">
                                                    <ItemTemplate>
                                                        <asp:Label ID="processedstatus" runat="server" Text='<%# ghFunctions.status_recurring_record(Eval("processedstatus").ToString()) %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Created" >
                                                    <ItemTemplate>
                                                        <asp:Label ID="createdate" runat="server" Text='<%# ghFunctions.date_label_full(Eval("createdate").ToString()) %>' /> Offset
                                                        <br /><asp:Label ID="createdate_utc" runat="server" Text='<%# ghFunctions.date_label_full_utc(Eval("createdate").ToString()) %>' /> Original Time
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Modified" >
                                                    <ItemTemplate>
                                                        <asp:Label ID="modifieddate" runat="server" Text='<%# ghFunctions.date_label_full(Eval("modifieddate").ToString()) %>' /> Offset
                                                        <br /><asp:Label ID="modifieddate_utc" runat="server" Text='<%# ghFunctions.date_label_full_utc(Eval("modifieddate").ToString()) %>' /> Original Time
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Fields>
                                            <EmptyDataTemplate>No [Sustainer] details;</EmptyDataTemplate>
                                        </asp:DetailsView>
                                    </div>
                                    <asp:Panel style="display: inline-block;vertical-align: top;" id="pnlRecurringList" runat="server" visible="false">
                                        <div>
                                            <h3 id="lblRecurringList" runat="server" visible="true">Recurring List</h3>
                                            <div style="font-weight: bold;color: darkred;">
                                                Select a record to see it's related payment transaction
                                            </div>
                                            <asp:GridView ID="gvRecurringList" runat="server" AutoGenerateColumns="False"
                                                BackColor="White"
                                                BorderStyle="None"
                                                BorderWidth="1px"
                                                BorderColor="#DEDFDE"
                                                AllowSorting="True"
                                                ForeColor="Black"
                                                GridLines="Vertical"
                                                AllowPaging="true" PageSize="25"
                                                DataKeyNames="callid,cbid"

                                                OnDataBound="GridView_DataBound"
                                                OnRowDataBound="GridView_RowDataBound"
                                                OnSelectedIndexChanged="GridView_IndexChanged"
                                                OnPageIndexChanging="GridView_PageIndexChanging"

                                                CssClass="Portal_GridView_Standard"
                                                Width="400"
                                                >
                                                <AlternatingRowStyle CssClass="Portal_GridView_Standard_Alternate" />
                                                <SelectedRowStyle CssClass="Portal_GridView_Standard_Selected" />
                                                <PagerSettings Position="Top" />
                                                <PagerStyle VerticalAlign="Bottom" HorizontalAlign="Center" CssClass="Portal_Gridview_Pager" />
                                                <Columns>
                                                    <asp:TemplateField HeaderText="Status">
                                                        <ItemTemplate>
                                                            <asp:Label ID="createdate" runat="server" Text='<%# ghFunctions.status_recurring_log(Eval("status").ToString()) %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="ChargeDate">
                                                        <ItemTemplate>
                                                            <asp:Label ID="createdate" runat="server" Text='<%# ghFunctions.date_label_short_noconvert(Eval("chargedate").ToString()) %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:BoundField HeaderText="Amount" DataField="Amount" DataFormatString="{0:C}" />
                                                    <asp:BoundField HeaderText="ADU File" DataField="FileUpToARC" ItemStyle-CssClass="LastCol" />
                                                    <asp:CommandField SelectText="Select" ShowSelectButton="true" HeaderText="Select" ItemStyle-CssClass="HideButton" HeaderStyle-CssClass="HideButton" />
                                                </Columns>
                                                <EmptyDataTemplate>No [Recurring] records;</EmptyDataTemplate>
                                            </asp:GridView>
                                            <asp:GridView ID="gvRecurringListExport" runat="server" AutoGenerateColumns="False" Visible="false">
                                                <Columns>
                                                    <asp:TemplateField HeaderText="Status">
                                                        <ItemTemplate>
                                                            <asp:Label ID="createdate" runat="server" Text='<%# ghFunctions.status_recurring_log(Eval("status").ToString()) %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="ChargeDate">
                                                        <ItemTemplate>
                                                            <asp:Label ID="createdate" runat="server" Text='<%# ghFunctions.date_label_short(Eval("chargedate").ToString()) %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:BoundField HeaderText="Amount" DataField="Amount" />
                                                    <asp:BoundField HeaderText="ADU File" DataField="FileUpToARC" />
                                                </Columns>
                                            </asp:GridView>
                                            <div style="margin-top: 5px;">
                                                <asp:DetailsView ID="dvPaymentDetailsRecurring" runat="server" DataKeyNames="callid,cbid"
                                                    AutoGenerateRows="False" ForeColor="Black" GridLines="Vertical"
                                                    HeaderText="Payment Details [Recurring]"
                                                    CssClass="User_List_Details_Border"
                                                    OnItemCommand="DetailsView_ItemCommand"
                                                    Width="400px"
                                                    >
                                                    <HeaderStyle CssClass="User_List_Details_Header" />
                                                    <RowStyle BackColor="#CCCCCC" CssClass="User_List_Details_Row" />
                                                    <FieldHeaderStyle Width="100px" />
                                                    <EmptyDataRowStyle BackColor="White" />
                                                    <Fields>
                                                        <asp:BoundField HeaderText="CallID" DataField="CallID" ReadOnly="true" />
                                                        <asp:TemplateField HeaderText="cbid">
                                                            <ItemTemplate>
                                                                <div style="display: inline;">
			                                                        <asp:Label ID="lbl_cbid" runat="server" Text='<%# Eval("cbid") %>' />
                                                                </div>
                                                                <div style="display: inline;margin-left: 10px;">
                                                                    <asp:LinkButton ID="btnDVRefund" CommandName="Refund" CommandArgument="Refund" runat="server" Visible='<%# refund_visible_button(Eval("status").ToString(),Eval("decision").ToString())%>'>Refund</asp:LinkButton>
                                                                    <asp:LinkButton ID="btnDVCharge" CommandName="Charge" CommandArgument="Charge" runat="server" Visible='<%# charge_visible_button(Eval("status").ToString(),Eval("decision").ToString())%>'>Charge</asp:LinkButton>
                                                                </div>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:BoundField HeaderText="status" DataField="status" ReadOnly="true" />
                                                        <asp:BoundField HeaderText="decision" DataField="decision" ReadOnly="true" />
                                                        <asp:BoundField HeaderText="reasoncode" DataField="reasoncode" ReadOnly="true" />
                                                        <asp:BoundField HeaderText="reasondescription" DataField="reasondescription" ReadOnly="true" />
                                                        <asp:BoundField HeaderText="authed" DataField="authed" ReadOnly="true" />
                                                        <asp:BoundField HeaderText="cctype" DataField="cctype" ReadOnly="true" />
                                                        <asp:BoundField HeaderText="ccnum" DataField="ccnum" ReadOnly="true" />
                                                        <asp:BoundField HeaderText="Reference" DataField="merchantreferencecode" ReadOnly="true" />
                                                        <asp:BoundField HeaderText="Response" DataField="ccContent" ReadOnly="true" />
                                                        <asp:TemplateField HeaderText="Created" >
                                                            <ItemTemplate>
                                                                <asp:Label ID="createdate" runat="server" Text='<%# ghFunctions.date_label_full(Eval("createdate").ToString()) %>' /> Offset
                                                                <br /><asp:Label ID="createdate_utc" runat="server" Text='<%# ghFunctions.date_label_full_utc(Eval("createdate").ToString()) %>' /> Original Time
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Charged" >
                                                            <ItemTemplate>
                                                                <asp:Label ID="chargedate_utc" runat="server" Text='<%# ghFunctions.date_label_full_utc(Eval("chargedate").ToString()) %>' /> Original Time
                                                            </ItemTemplate>
                                                        </asp:TemplateField>

                                                    </Fields>
                                                    <EmptyDataTemplate>No [Payment] details;</EmptyDataTemplate>
                                                </asp:DetailsView>
                                            </div>
                                            <div>
                                                <asp:Label ID="lblRecurringDetail" runat="server" Text="" />
                                            </div>
                                        </div>
                                    </asp:Panel>
                                </div>
                                <div class="clearfix">
                                    <div style="display: inline-block;vertical-align: top;margin-top: 5px;">
                                        <asp:DetailsView ID="dvTokenizationDetails" runat="server" DataKeyNames="tokenid"
                                            AutoGenerateRows="False" ForeColor="Black" GridLines="Vertical"
                                            HeaderText="Tokenization Details"
                                            CssClass="User_List_Details_Border"
                                            OnItemCommand="DetailsView_ItemCommand"
                                            Width="400px"
                                            >
                                            <HeaderStyle CssClass="User_List_Details_Header" />
                                            <RowStyle BackColor="#CCCCCC" CssClass="User_List_Details_Row" />
                                            <FieldHeaderStyle Width="100px" />
                                            <EmptyDataRowStyle BackColor="White" />
                                            <Fields>
                                                <asp:TemplateField HeaderText="TokenID">
                                                    <ItemTemplate>
                                                        <div style="display: inline;">
			                                                <asp:Label ID="lbl_tokenid" runat="server" Text='<%# Eval("tokenid") %>' />
                                                        </div>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField HeaderText="SubscriptionID" DataField="SubscriptionID" ReadOnly="true" />
                                                <asp:TemplateField HeaderText="Status">
                                                    <ItemTemplate>
                                                        <asp:Label ID="status" runat="server" Text='<%# ghFunctions.status_tokenization_record(Eval("status").ToString()) %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Created" >
                                                    <ItemTemplate>
                                                        <asp:Label ID="createdate" runat="server" Text='<%# ghFunctions.date_label_full(Eval("createdate").ToString()) %>' /> Offset
                                                        <br /><asp:Label ID="createdate_utc" runat="server" Text='<%# ghFunctions.date_label_full_utc(Eval("createdate").ToString()) %>' /> Original Time
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Fields>
                                            <EmptyDataTemplate>No [Tokenization] details;</EmptyDataTemplate>
                                        </asp:DetailsView>
                                    </div>
                                </div>
                                <div class="clearfix">
                                    <div style="display: inline-block;vertical-align: top;margin-top: 5px;">
                                        <asp:DetailsView ID="dvInteraction" runat="server" DataKeyNames="interactionid"
                                            AutoGenerateRows="True" ForeColor="Black" GridLines="Vertical"
                                            HeaderText="Interaction"
                                            CssClass="User_List_Details_Border"

                                            Width="400px"
                                            >
                                            <HeaderStyle CssClass="User_List_Details_Header" />
                                            <RowStyle BackColor="#CCCCCC" CssClass="User_List_Details_Row" />
                                            <FieldHeaderStyle Width="100px" />
                                            <EmptyDataRowStyle BackColor="White" />
                                            <Fields>
                                            </Fields>
                                            <EmptyDataTemplate>No [Interaction] details;</EmptyDataTemplate>
                                        </asp:DetailsView>
                                    </div>
                                    <div style="display: inline-block;vertical-align: top;margin-top: 5px;">
                                        <asp:DetailsView ID="dvInteractionDetails" runat="server" DataKeyNames="interactionid"
                                            AutoGenerateRows="True" ForeColor="Black" GridLines="Vertical"
                                            HeaderText="Interaction Details"
                                            CssClass="User_List_Details_Border"

                                            Width="400px"
                                            >
                                            <HeaderStyle CssClass="User_List_Details_Header" />
                                            <RowStyle BackColor="#CCCCCC" CssClass="User_List_Details_Row" />
                                            <FieldHeaderStyle Width="100px" />
                                            <EmptyDataRowStyle BackColor="White" />
                                            <Fields>
                                                <asp:TemplateField HeaderText="datestart" >
                                                    <ItemTemplate>
                                                        <asp:Label ID="datestart" runat="server" Text='<%# ghFunctions.date_label_full(DataBinder.GetPropertyValue(Container.DataItem, "datestart").ToString()) %>' /> Offset
                                                        <br /><asp:Label ID="datestart_utc" runat="server" Text='<%# ghFunctions.date_label_full_utc(DataBinder.GetPropertyValue(Container.DataItem, "datestart").ToString()) %>' /> Original Time
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Fields>
                                            <EmptyDataTemplate>No [Interaction] details;</EmptyDataTemplate>
                                        </asp:DetailsView>
                                    </div>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <div style="float: left;margin-left: 5px;" class="admin_section">
                        <asp:UpdatePanel ID="UpdatePanel4" runat="server">
                            <ContentTemplate>
                                <style type="text/css">
                                    .admin_section .user_label
                                    {
                                        width: 100px;
                                    }
                                    .admin_section .user_section
                                    {
                                        width: 390px;
                                        padding-bottom: 5px;
                                    }
                                    .admin_section .user_section h2
                                    {
                                        padding-top: 2px;
                                        padding-bottom: 2px;
                                    }
                                </style>
                                <asp:Panel ID="pAdminFunctions" runat="server" Visible="false">
                                    <div class="user_section" style="min-height: 15px;padding: 0px;margin-bottom: 1px;">
                                        <h2 style="">Admin Functions</h2>
                                    </div>
                                    <asp:Panel ID="pAdminWarningADU" runat="server" Visible="false">
                                        <div class="user_section" style="">
                                            <h2>ADU Warning</h2>
                                            <div style="margin: 5px;color: blue;">
                                                This record has already been sent to ADU
                                                <div style="margin-left: 10px;">
                                                    This means that any update will not be reflected on the initial transmission to the ADU.
                                                    <br />If the update is intended to update ARC; please note you will have to do that separately.
                                                </div>
                                            </div>
                                        </div>
                                    </asp:Panel>
                                    <asp:Panel ID="pAdminRefund" runat="server" Visible="false">
                                        <div class="user_section" style="">
                                            <h2>Refund Processing</h2>
                                            <div style="margin: 5px;color: blue;">
                                                Refunds are processed against the card on file.
                                                <div style="margin-left: 10px;">
                                                A refund is also processed against the original transaction.
                                                <br />If the transaction is more than 60 days old; a credit will be issued against the card on file.
                                                <br />If a different card needs to be credited; the card on file needs to be updated first.
                                                </div>
                                                Partial refunds are possible; enter an amount lower than the total.
                                                <div style="margin-left: 10px">
                                                    The system will verify the original donation amount against a modified amount.
                                                    <br />Any successful refunds will be deducted from original amount.
                                                </div>
                                            </div>
                                        </div>
                                        <asp:Panel ID="pAdminRefundType" runat="server" Visible="false">
                                            <div class="user_section" style="">
                                                <h2><asp:Label ID="lblRefundType" runat="server" Text="" /></h2>
                                                <div style="margin: 5px;">
                                                    <asp:Panel ID="pAdminRefundFO" runat="server" Visible="false">
                                                        The refund is done against the original transaction
                                                        <br />Below is the details of the original transaction
                                                    </asp:Panel>
                                                    <asp:Panel ID="pAdminRefundSA" runat="server" Visible="false">
                                                        The credit is done against the credit card on file
                                                        <br />Below is the details of the information on file
                                                    </asp:Panel>
                                                </div>
                                                <div class="user_line clearfix">
                                                    <div class="user_label">
                                                        CallID:
                                                    </div>
                                                    <div class="user_field">
                                                        <asp:Label ID="lblRefCallID" runat="server" Text="" />
                                                    </div>
                                                </div>
                                                <div class="user_line clearfix">
                                                    <div class="user_label">
                                                        Donation Amount:
                                                    </div>
                                                    <div class="user_field">
                                                        <asp:Label ID="lblCurrentAmount" runat="server" Text="" />
                                                    </div>
                                                </div>
                                                <div class="user_line clearfix">
                                                    <div class="user_label">
                                                        Transaction ID:
                                                    </div>
                                                    <div class="user_field">
                                                        <asp:Label ID="lblCurrentCYBID" runat="server" Text="" />
                                                    </div>
                                                </div>
                                            </div>
                                        </asp:Panel>
                                        <asp:Panel ID="pAdminRefundAmount" runat="server" Visible="false">
                                            <div class="user_section" style="">
                                                <h2>Refund Amount & Reason</h2>
                                                <div class="user_line clearfix">
                                                    <div class="user_label">
                                                        <asp:Label ID="lblRefundAmount" runat="server" Text="Refund Amount:" />
                                                    </div>
                                                    <div class="user_field">
                                                        <asp:TextBox ID="tbRefundAmount" runat="server" Width="100px" Text="" />
                                                        <asp:HiddenField ID="AmountOriginal" runat="server" />
                                                        <asp:HiddenField ID="RefundType" runat="server" />
                                                        <div style="color: blue;">
                                                            Amount must not exceed donation amount
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="user_line clearfix">
                                                    <div class="user_label">
                                                        Refund Reason:
                                                    </div>
                                                    <div class="user_field">
                                                        <asp:DropDownList ID="ddlRefundReason" runat="server" Width="250px">
                                                        </asp:DropDownList>
                                                        <div style="color: blue;">
                                                            Note is required
                                                        </div>
                                                        <asp:TextBox ID="tbRefundNote" runat="server" Width="250px" TextMode="MultiLine" MaxLength="99" />
                                                    </div>
                                                </div>
                                                <div class="user_line clearfix">
                                                    <div class="user_label">
                                                        Password:
                                                    </div>
                                                    <div class="user_field">
                                                        <asp:TextBox ID="tbRefundPassword" runat="server" Width="100px" TextMode="Password" Text="" />
                                                        <div style="color: blue;">
                                                            Please enter the refund password.
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </asp:Panel>
                                        <asp:Panel ID="pAdminRefundSubmit" runat="server" Visible="false">
                                            <div class="user_section" style="">
                                                <h2>Submit Refund</h2>
                                                <div class="user_line clearfix user_last">
                                                    <div class="user_submit">
                                                        <asp:Button ID="btnRefundSubmit" runat="server"
                                                            Text="Process Refund/Credit"
                                                            OnClientClick="this.disabled = true; this.value = 'Processing...';$('#processing').show();" 
                                                            UseSubmitBehavior="false" 
                                                            OnClick="CreditTry" Enabled="false"
                                                            />
                                                        <asp:Button ID="btnRefundCancel" runat="server" Text="Cancel" OnClick="Refund_Cancel" />
                                                        <asp:Button ID="Button1" runat="server" Text="Refresh" OnClick="Refund_FakeIt" Visible="false" />
                                                    </div>
                                                    <div id="processing" class="donate-alert" style="display: none;">Please wait while the refund processes.</div>
                                                </div>
                                                <div style="margin-left: 15px;">
                                                    <asp:Label ID="lblRefundProcessing" runat="server" Text="" />
                                                </div>
                                            </div>
                                        </asp:Panel>
                                        <asp:Panel ID="pAdminRefundResponse" runat="server" Visible="false">
                                            <div class="user_section" style="">
                                                <h2>Submit Refund</h2>
                                                <div class="user_line clearfix user_last">
                                                    <div style="margin: 5px;">
                                                        <asp:Label ID="lblRefundResponse" runat="server" Text="" />
                                                    </div>
                                                </div>
                                            </div>
                                        </asp:Panel>
                                        <div style="margin: 5px;color: darkred;">
                                            <asp:Label ID="lblRefundTemp" runat="server" Text="" />
                                        </div>
                                    </asp:Panel>
                                    <asp:Panel ID="pAdminCharge" runat="server" Visible="false">
                                        <div class="user_section" style="">
                                            <h2>Charge Processing</h2>
                                            <div style="margin: 5px;color: blue;">
                                                Process a charge for a non-settled transaction
                                                <div style="margin-left: 10px;">
                                                    The transaction data used will be what is visible for the record now
                                                    <br />If a different card or address needs to be used; change that first
                                                    <br />The only change from this section is the amount
                                                    <br />A reason and note is required for a charge
                                                </div>
                                            </div>
                                            <div style="margin: 5px;">
                                                <asp:Label ID="lblAdminCharge" runat="server" Text="" />
                                            </div>
                                        </div>
                                        <asp:Panel ID="pAdminChargeAmount" runat="server" Visible="false">
                                            <div class="user_section" style="">
                                                <h2>Charge Amount & Reason</h2>
                                                <div class="user_line clearfix">
                                                    <div class="user_label">
                                                        Charge Type:
                                                    </div>
                                                    <div class="user_field">
                                                        <asp:Label ID="lblAdminChargeType" runat="server" Text="" />
                                                    </div>
                                                </div>
                                                <div class="user_line clearfix">
                                                    <div class="user_label">
                                                        Charge Amount:
                                                    </div>
                                                    <div class="user_field">
                                                        <asp:TextBox ID="tbAdminChargeAmount" runat="server" Width="100px" Text="" />
                                                        <asp:HiddenField ID="hfAdminChargeAmountCurrent" runat="server" />
                                                        <div style="color: blue;">
                                                            Amount must not exceed donation amount
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="user_line clearfix">
                                                    <div class="user_label">
                                                        Charge Reason:
                                                    </div>
                                                    <div class="user_field">
                                                        <asp:DropDownList ID="ddlAdminChargeReason" runat="server" Width="250px">
                                                        </asp:DropDownList>
                                                        <div style="color: blue;">
                                                            Note is required
                                                        </div>
                                                        <asp:TextBox ID="tbAdminChargeNote" runat="server" Width="250px" TextMode="MultiLine" MaxLength="99" />
                                                    </div>
                                                </div>
                                            </div>
                                        </asp:Panel>
                                        <asp:Panel ID="pAdminChargeSubmit" runat="server" Visible="false">
                                            <div class="user_section" style="">
                                                <h2>Submit Charge</h2>
                                                <div class="user_line clearfix user_last">
                                                    <div class="user_submit">
                                                        <asp:Button ID="btnAdminChargeSubmit" runat="server"
                                                            Text="Process Charge"
                                                            OnClientClick="this.disabled = true; this.value = 'Processing...';$('#charging').show();" 
                                                            UseSubmitBehavior="false" 
                                                            OnClick="ChargeTry" Enabled="false"
                                                            />
                                                        <asp:Button ID="btnAdminChargeCancel" runat="server" Text="Cancel" OnClick="Charge_Cancel" />
                                                    </div>
                                                    <div id="charging" class="donate-alert" style="display: none;">Please wait while the charge is processed...</div>
                                                </div>
                                                <div style="margin-left: 15px;">
                                                    <asp:Label ID="lblAdminChargeProcessing" runat="server" Text="" />
                                                </div>
                                            </div>
                                        </asp:Panel>
                                        <asp:Panel ID="pAdminChargeResponse" runat="server" Visible="false">
                                            <div class="user_section" style="">
                                                <h2>Submit Charge</h2>
                                                <div class="user_line clearfix user_last">
                                                    <div style="margin: 5px;">
                                                        <asp:Label ID="lblAdminChargeResponse" runat="server" Text="" />
                                                    </div>
                                                </div>
                                            </div>
                                        </asp:Panel>
                                        <div style="margin: 5px;color: darkred;">
                                            <asp:Label ID="lblAdminChargeTemp" runat="server" Text="" />
                                        </div>
                                    </asp:Panel>
                                    <asp:Panel ID="pAdminDonor" runat="server" Visible="false">
                                        <div class="user_section" style="">
                                            <h2>Donor Updates</h2>
                                            <div style="margin: 5px;color: blue;">
                                                Perform updates and maintenance on donor record
                                            </div>
                                            <div style="margin: 5px;">
                                                <asp:Label ID="lblDonorModify" runat="server" Text="" />
                                            </div>
                                        </div>
                                        <asp:Panel ID="pAdminDonorDetails" runat="server" Visible="false">
                                            <div class="user_section" style="">
                                                <h2>Sustainer Details</h2>
                                                <div class="user_line clearfix">
                                                    <div class="user_label">
                                                        First Name:
                                                    </div>
                                                    <div class="user_field">
                                                        <asp:TextBox ID="tbAdminDonorFirstName" runat="server" Width="125px" />
                                                    </div>
                                                </div>
                                                <div class="user_line clearfix">
                                                    <div class="user_label">
                                                        Last Name:
                                                    </div>
                                                    <div class="user_field">
                                                        <asp:TextBox ID="tbAdminDonorLastName" runat="server" Width="125px" />
                                                    </div>
                                                </div>
                                                <div class="user_line clearfix">
                                                    <div class="user_label">
                                                        Address 1:
                                                    </div>
                                                    <div class="user_field">
                                                        <asp:TextBox ID="tbAdminDonorAddress1" runat="server" Width="125px" />
                                                    </div>
                                                </div>
                                                <div class="user_line clearfix">
                                                    <div class="user_label">
                                                        Address 2:
                                                    </div>
                                                    <div class="user_field">
                                                        <asp:TextBox ID="tbAdminDonorAddress2" runat="server" Width="125px" />
                                                    </div>
                                                </div>
                                                <div class="user_line clearfix">
                                                    <div class="user_label">
                                                        Suite:
                                                    </div>
                                                    <div class="user_field">
                                                        <asp:TextBox ID="tbAdminDonorSuite" runat="server" Width="125px" />
                                                    </div>
                                                </div>
                                                <div class="user_line clearfix">
                                                    <div class="user_label">
                                                        Zip Code:
                                                    </div>
                                                    <div class="user_field">
                                                        <asp:TextBox ID="tbAdminDonorZip" runat="server" Width="125px" />
                                                    </div>
                                                </div>
                                                <div class="user_line clearfix">
                                                    <div class="user_label">
                                                        City:
                                                    </div>
                                                    <div class="user_field">
                                                        <asp:TextBox ID="tbAdminDonorCity" runat="server" Width="125px" />
                                                    </div>
                                                </div>
                                                <div class="user_line clearfix">
                                                    <div class="user_label">
                                                        State:
                                                    </div>
                                                    <div class="user_field">
                                                        <asp:TextBox ID="tbAdminDonorState" runat="server" Width="125px" />
                                                    </div>
                                                </div>
                                                <div class="user_line clearfix">
                                                    <div class="user_label">
                                                        Country:
                                                    </div>
                                                    <div class="user_field">
                                                        <asp:TextBox ID="tbAdminDonorCountry" runat="server" Width="125px" />
                                                    </div>
                                                </div>
                                                <div class="user_line clearfix">
                                                    <div class="user_label">
                                                        Phone:
                                                    </div>
                                                    <div class="user_field">
                                                        <asp:TextBox ID="tbAdminDonorPhone" runat="server" Width="125px" />
                                                    </div>
                                                </div>
                                                <div class="user_line clearfix">
                                                    <div class="user_label">
                                                        Phone Optin:
                                                    </div>
                                                    <div class="user_field user_radio">
                                                        <asp:CheckBox ID="tbAdminDonorPhoneOptin" runat="server" />
                                                    </div>
                                                </div>
                                                <div class="user_line clearfix">
                                                    <div class="user_label">
                                                        Email:
                                                    </div>
                                                    <div class="user_field">
                                                        <asp:TextBox ID="tbAdminDonorEmail" runat="server" Width="125px" />
                                                    </div>
                                                </div>
                                                <div class="user_line clearfix">
                                                    <div class="user_label">
                                                        Receipt Optin:
                                                    </div>
                                                    <div class="user_field user_radio">
                                                        <asp:CheckBox ID="CheckBox1" runat="server" />
                                                    </div>
                                                </div>
                                                <div class="user_line clearfix">
                                                    <div class="user_label">
                                                        News Optin:
                                                    </div>
                                                    <div class="user_field user_radio">
                                                        <asp:CheckBox ID="CheckBox2" runat="server" />
                                                    </div>
                                                </div>
                                            </div>
                                        </asp:Panel>
                                        <asp:Panel ID="pAdminDonorSubmit" runat="server" Visible="false">
                                            <div class="user_section" style="">
                                                <h2>Submit Update</h2>
                                                <div class="user_line clearfix user_last">
                                                    <div class="user_submit">
                                                        <asp:Button ID="Button5" runat="server" Text="Cancel" OnClick="Modify_Sustainer_Cancel" />
                                                    </div>
                                                    <div id="processing_sustainer2" class="donate-alert" style="display: none;">Please wait while the refund processes.</div>
                                                </div>
                                                <div style="margin-left: 15px;">
                                                    <asp:Label ID="Label24" runat="server" Text="" />
                                                </div>
                                            </div>
                                        </asp:Panel>
                                        <asp:Panel ID="pAdminDonorResponse" runat="server" Visible="false">
                                            <div class="user_section" style="">
                                                <h2>Submit Response</h2>
                                                <div class="user_line clearfix user_last">
                                                    <div style="margin: 5px;">
                                                        <asp:Label ID="Label25" runat="server" Text="" />
                                                    </div>
                                                </div>
                                            </div>
                                        </asp:Panel>
                                        <div style="margin: 5px;color: darkred;">
                                            <asp:Label ID="Label26" runat="server" Text="" />
                                        </div>
                                    </asp:Panel>
                                    <asp:Panel ID="pAdminSustainer" runat="server" Visible="false">
                                        <div class="user_section" style="">
                                            <h2>Sustainer Function</h2>
                                            <div style="margin: 5px;color: blue;">
                                                Perform updates and maintenance on sustainer record
                                                <div style="margin-left: 10px">
                                                    Make changes to sustainer designation, date, etc.
                                                    <br />Also cancel or re-activate the sustainer record.
                                                </div>
                                            </div>
                                        </div>
                                        <asp:Panel ID="pAdminSustainerDetails" runat="server" Visible="false">
                                            <div class="user_section" style="">
                                                <h2>Sustainer Details</h2>
                                                <div class="user_line clearfix">
                                                    <div class="user_label">
                                                        Sustainer Status:
                                                    </div>
                                                    <div class="user_field">
                                                        <asp:DropDownList ID="ddlSustainerStatus" runat="server" Width="250px">
                                                            <asp:ListItem Value="301001" Text="New Record: Settled"></asp:ListItem>
                                                            <asp:ListItem Value="301002" Text="Processed: Settled"></asp:ListItem>
                                                            <asp:ListItem Value="301003" Text="Processed: Rejected"></asp:ListItem>
                                                            <asp:ListItem Value="301004" Text="Processed: Error"></asp:ListItem>
                                                            <asp:ListItem Value="301005" Text="Cancelled by System"></asp:ListItem>
                                                            <asp:ListItem Value="301006" Text="Cancelled by Donor"></asp:ListItem>
                                                            <asp:ListItem Value="301007" Text="Cancelled by Admin"></asp:ListItem>
                                                            <asp:ListItem Value="301008" Text="Deleted by Admin"></asp:ListItem>
                                                        </asp:DropDownList>
                                                    </div>
                                                </div>
                                                <div class="user_line clearfix">
                                                    <div class="user_label">
                                                        Designation:
                                                    </div>
                                                    <div class="user_field">
                                                        <asp:Label ID="lblSustainerDesignation" runat="server" Text="" />
                                                        <asp:DropDownList ID="ddlSustainerDesignation" runat="server" Width="250px" Visible="false" />
                                                    </div>
                                                </div>
                                                <div class="user_line clearfix">
                                                    <div class="user_label">
                                                        Recurring Frequency:
                                                    </div>
                                                    <div class="user_field">
                                                        <asp:Label ID="lblSustainerFrequency" runat="server" Text="" />
                                                    </div>
                                                </div>
                                                <div class="user_line clearfix">
                                                    <div class="user_label">
                                                        Recurring Date:
                                                    </div>
                                                    <div class="user_field">
                                                        <asp:DropDownList ID="ddlSustainerDate" runat="server" Width="250px">
                                                            <asp:ListItem Value="01" Text="1st of every month"></asp:ListItem>
                                                            <asp:ListItem Value="15" Text="15th of every month"></asp:ListItem>
                                                        </asp:DropDownList>
                                                    </div>
                                                </div>
                                                <div class="user_line clearfix">
                                                    <div class="user_label">
                                                        Receipt Frequency:
                                                    </div>
                                                    <div class="user_field">
                                                        <asp:Label ID="lblSustainerReceipt" runat="server" Text="" />
                                                        <asp:DropDownList ID="ddlSustainerReceipt" runat="server" Width="250px" Visible="false">
                                                            <asp:ListItem Value="MONTHLY" Text="MONTHLY"></asp:ListItem>
                                                            <asp:ListItem Value="YEARLY" Text="YEARLY"></asp:ListItem>
                                                        </asp:DropDownList>
                                                    </div>
                                                </div>
                                                <div>
                                                    <asp:Label ID="lblSustainerDetails" runat="server" Text="" />
                                                </div>
                                            </div>
                                        </asp:Panel>
                                        <asp:Panel ID="pAdminSustainerSubmit" runat="server" Visible="false">
                                            <div class="user_section" style="">
                                                <h2>Sustainer Update</h2>
                                                <div class="user_line clearfix user_last">
                                                    <div class="user_submit">
                                                        <asp:Button ID="btnSustainerUpdate" runat="server"
                                                            Text="Update Sustainer"
                                                            OnClientClick="this.disabled = true; this.value = 'Processing...';$('#processing_sustainer').show();" 
                                                            UseSubmitBehavior="false" 
                                                            OnClick="Modify_Sustainer_Submit" Enabled="false"
                                                            />
                                                        <asp:Button ID="btnSustainerCancel" runat="server" Text="Cancel" OnClick="Modify_Sustainer_Cancel" />
                                                    </div>
                                                    <div id="processing_sustainer" class="donate-alert" style="display: none;">Please wait while the update processes...</div>
                                                </div>
                                                <div style="margin-left: 15px;">
                                                    <asp:Label ID="Label22" runat="server" Text="" />
                                                </div>
                                            </div>
                                        </asp:Panel>
                                        <asp:Panel ID="pAdminSustainerResponse" runat="server" Visible="false">
                                            <div class="user_section" style="">
                                                <h2>Submit Response</h2>
                                                <div class="user_line clearfix user_last">
                                                    <div style="margin: 5px;">
                                                        <asp:Label ID="Label23" runat="server" Text="" />
                                                    </div>
                                                </div>
                                            </div>
                                        </asp:Panel>
                                        <div style="margin: 5px;color: darkred;">
                                            <asp:Label ID="lblSustainerModify" runat="server" Text="" />
                                        </div>
                                    </asp:Panel>
                                </asp:Panel>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </div>
                <div>
                    <asp:UpdatePanel ID="upSystemMessages" runat="server">
                        <ContentTemplate>
                            <asp:Label Font-Bold="true" ID="dtlLabel" runat="server" Text="" />
                            <div style="width: 575px">
                                <div>
                                    <asp:Label ID="sqlPrint" runat="server" Text="" />
                                </div>
                                <asp:Label ID="Error_General" runat="server" Text="General Error Label" />
                                <asp:Label ID="lblSystemMessage" runat="server" Text="" />
                                <div class="user_error_standard">
                                </div>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>
        </div>
    </asp:Panel>
    <div class="progressUpdate_wrap">
        <asp:AlwaysVisibleControlExtender ID="AlwaysVisibleControlExtender1" runat="server"
            TargetControlID="UpdateProgress1"
            VerticalSide="Middle"
            HorizontalSide="Left"
            HorizontalOffset="10"
            ScrollEffectDuration=".1"
            />        
        <asp:UpdateProgress ID="UpdateProgress1" runat="server" DisplayAfter="0">
            <ProgressTemplate>
                <div id="progressUpdate" class="progressUpdate" style="background-color: #C6C6C6;width: 500px;height: 150px;">
                    <div style="margin: 0 auto;width: 250px;padding-top: 50px;">
                        <div>Your request is processing, please wait.<br />This process may take several minutes...</div>
                        <div id="sinceCountdown" style="width: 200px;background-color: #C6C6C6;"></div>
                        <div><img id="loading" src="images/loading/loading6.gif" alt="..." /></div>
                    </div>
                </div>
            </ProgressTemplate>
        </asp:UpdateProgress>
    </div>
    <div class="error_view clearfix" style="margin-top: 25px;">
        <asp:UpdatePanel ID="updatePanel2" runat="server">
            <ContentTemplate>
                <asp:Label ID="msgLabel" runat="server" Text="" />
                <asp:DetailsView ID="ErrorView" runat="server" AutoGenerateRows="True" ForeColor="Black"
                    GridLines="Vertical" HeaderText="Error Display" CssClass="Portal_DetailView_Standard">
                    <HeaderStyle CssClass="Portal_DetailView_Standard_Header" />
                    <RowStyle CssClass="Portal_DetailView_Standard_Row" />
                    <FieldHeaderStyle Width="100px" />
                    <Fields>
                    </Fields>
                    <EmptyDataTemplate>No Errors to Report;</EmptyDataTemplate>
                </asp:DetailsView>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">
</asp:Content>
