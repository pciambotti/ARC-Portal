<%@ Page Title="Dashboard" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Dashboard_Calls.aspx.cs" Inherits="Dashboard_Calls" %>
<%@ MasterType TypeName="MasterPage" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register TagPrefix="chart" Namespace="ChartDirector" Assembly="netchartdir" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="Server">
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
    	width: 500px !important;
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
    <script type="text/javascript">
        function OnLoad_Function() {
            GH_Buttons();
            //GH_DropDown();
            GH_DatePickerToday();
            GH_Select();
            GH_SelectMultiple();


            // $(".ghDatePicker").bind("onchange", getMyDay(this));

            // onchange="getMyDay(this);" 
            // keydown="getMyDay(this);"
            $('.ghDatePickerStart').each(function (i, obj) {
                getMyDay(this);
            });
            $('.ghDatePickerEnd').each(function (i, obj) {
                getMyDay(this);
            });

            $(document).ready(function () {
                $('.multiselectfilter').multiselect().multiselectfilter();

                // Need a better answer to this issue
                // The auto postback got disabled with the latest jquery updates I did
                // This was the work-around
                $("#<% = ddlCallClients.ClientID %>").selectmenu({
                    change: function (event, ui) {
                        setTimeout('__doPostBack(\'<% = ddlCallClients.ClientID %>\',\'\')', 0);
                    }
                });
            });
        }
        $(document).ready(function () {
            // Turn all Submit Buttons to jQuery themed buttons
        });

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
    </script>
    <script type="text/javascript">
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
    <script type="text/javascript">
        function testMe() {
            alert("Works");
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
    	min-height: 110px;
    }
    .filter_row
    {
    	margin-bottom: 3px;
    }
    .filter_top
    {
    	vertical-align: top;
    }
    .filter_submit
    {
    	text-align: center;
    	margin-top: 15px;
    }
    .h125
    {
    	height: 125px;
    }
    .message_high
    {
    	color: Red;
    	font-weight: bold;
    }
    </style>
    <style type="text/css">
        .report-controls
        {
	        display: inline-block;width: 525px;margin: 0px;
        }
        .report-timers
        {
	        display: inline-block;
	        width: 475px;
	        margin: 0px;
	        text-align: right;
        }
        .report-timers-nextin
        {
	        display: inline-block;width: 55px;text-align: center;background-color: #C6C6C6;	
        }
    </style>
    <script type="text/javascript">
        function RefreshCountdownStart(field, scnds) {
            $(document).ready(function() {
                var newDate = new Date();
                newDate.setSeconds(newDate.getSeconds() + scnds);
                //alert(scnds);
                $('#' + field).countdown({ until: newDate, compact: true,
                    layout: '<b>{hnn}{sep}{mnn}{sep}{snn}</b>{desc}',
                    description: ''
                });
            });
        }
        function RefreshCountdownStop(field) {
            $(document).ready(function() {
                $('#' + field).countdown('destroy');
            });
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Master_Content" Runat="Server">
    <script type="text/javascript">
        Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(function(evt, args) { OnLoad_Function(); });
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
        <div id="default_container"  style="margin-left: 5px;">
            <div id="report_header" style="color: DarkRed;font-weight: bold;">
                <div id="msgTitle" style="display: inline;width: 50%;color: DarkRed;font-weight: bold;text-align: left;">
                    Greenwood & Hall Client Reporting Dashboard
                </div>            
                <div id="msgTitleRight" style="float: right;width: 50%;color: DarkOrange;font-weight: bold;text-align: right;">
                    <asp:Label ID="rpTimeZone" runat="server" Text="Times are UTC Minus the Offset: " />
                </div>
                <hr />
            </div>
            <div id="report_control1">
                <input id="btnFilters01" type="button" value="Hide All Filters" onclick="Filter_Toggle(this,'reporting_filters','All Filters');" />
                <div style="display: inline-block;color: DarkRed;font-weight: bold;">
                    Info
                </div>
            </div>
            <div id="reporting_filters" class="reporting_filters">
                <div class="clearfix">
                    <div id="filter_dates" class="filter_section" runat="server" visible="true">
                        <h3>Call Dates</h3>
                        <div id="datetime_start" class="clear_both">
                            <asp:Label ID="lblDateStart" runat="server" Text="Start Time" CssClass="filter_label1" />
                            <asp:TextBox ID="dtStartDate" runat="server" Width="75px" CssClass="ghDatePickerStart" onchange="getMyDay(this);" />
                            <asp:TextBox ID="dtStartTime" runat="server" Width="50px" CssClass="ghTimePickerStart" />
                            <div style="position: relative;">
                                <div style="position: absolute;bottom: 2px;left: 242px;">
                                    <asp:Label ID="lblStartDate" runat="server" Text="" CssClass="lblStartDate" />
                                </div>
                            </div>
                        </div>
                        <div id="datetime_end">
                            <asp:Label ID="lblDateEnd" runat="server" Text="End Time" CssClass="filter_label1" />
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
                    <div id="filter_clients" class="filter_section" runat="server">
                        <h3>Client</h3>
                        <div id="call_clients" style="margin-left: 15px;">
                            <asp:UpdatePanel ID="UpdatePanel4" runat="server">
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="ddlCallClients" />
                                </Triggers>
                                <ContentTemplate>
                                    <asp:ListBox ID="ddlCallClients" runat="server" Width="250px" Width2="300px" OnSelectedIndexChanged="Client_Index_Changed"  AutoPostBack="true" />
                                    <asp:ListSearchExtender ID="lse" runat="server"
                                        TargetControlID="ddlCallClients"
                                        PromptText="Start typing"
                                        PromptPosition="Top"
                                        QueryPattern="Contains"
                                        />
                                    <div style="margin-left: 10px;">
                                        <asp:Label ID="lblClientCampaigns" runat="server" Text="" />
                                        <asp:Label ID="lblClientCampaigns2" runat="server" Text="" />
                                        <asp:Label ID="lblClientCampaigns3" runat="server" Text="" />
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>

                        </div>
                    </div>
                    <div id="filter_to" class="filter_section">
                        <h3>List Filters</h3>
                        <asp:UpdatePanel ID="upListFilters" runat="server">
                            <Triggers>
                            </Triggers>
                            <ContentTemplate>
                                <div id="call_campaign">
                                    <asp:Label ID="lblCallCampaign" runat="server" Text="Campaigns" CssClass="filter_label1" />
                                    <asp:ListBox ID="ddlCallCampaigns" runat="server" SelectionMode="Multiple" Width="130px" Width2="500px" multiple="multiple" CssClass="multiselect multiselectfilter" />
                                    <span>
                                        <asp:Label ID="lblCampaignsCount" runat="server" Text="" />
                                    </span>
                                </div>
                                <div id="call_skill">
                                    <asp:Label ID="lblCallSkill" runat="server" Text="Skills" CssClass="filter_label1" />
                                    <asp:ListBox ID="ddlCallSkills" runat="server" SelectionMode="Multiple" Width="130px" Width2="500px" multiple="multiple" CssClass="multiselect multiselectfilter" />
                                    <span>
                                        <asp:Label ID="lblSkillsCount" runat="server" Text="" />
                                    </span>
                                </div>
                                <div id="call_agent" style="">
                                    <asp:Label ID="lblCallAgent" runat="server" Text="Agents" CssClass="filter_label1" />
                                    <asp:ListBox ID="ddlCallAgents" runat="server" SelectionMode="Multiple" Width="130px" Width2="500px" multiple="multiple" CssClass="multiselect multiselectfilter" />
                                    <span>
                                        <asp:Label ID="lblAgentsCount" runat="server" Text="" />
                                    </span>
                                </div>

                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <div id="filter_stats" class="filter_section">
                        <h3 style="margin-bottom: 3px;">Total Stats</h3>
                        <div id="dashboard_stats" style="width: 100%;">
                            <div style="margin: 0px auto;width: 225px">
                                <asp:UpdatePanel ID="upStatsDashboard" runat="server">
                                    <ContentTemplate>
                                        <asp:GridView ID="gvStatsDashboard" runat="server" AutoGenerateColumns="False" CssClass="center">
                                            <Columns>
                                                <asp:BoundField HeaderText="count" DataField="count" ItemStyle-HorizontalAlign="Right" ItemStyle-Width="75" HeaderStyle-HorizontalAlign="Center" />
                                                <asp:BoundField HeaderText="datestart" DataField="datestart" ItemStyle-HorizontalAlign="Right" ItemStyle-Width="75" HeaderStyle-HorizontalAlign="Center" />
                                                <asp:BoundField HeaderText="dateend" DataField="dateend" ItemStyle-HorizontalAlign="Right" ItemStyle-Width="75" HeaderStyle-HorizontalAlign="Center" />
                                            </Columns>
                                        </asp:GridView>

                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                        </div>
                        <h3 style="margin-top: 3px;margin-bottom: 3px;">Query Stats</h3>
                        <div id="query_stats" style="width: 100%;">
                            <div style="margin: 0px auto;width: 225px;">
                                <asp:UpdatePanel ID="upStatsQuery" runat="server">
                                    <ContentTemplate>
                                        <asp:GridView ID="gvStatsQuery" runat="server" AutoGenerateColumns="False" CssClass="center">
                                            <Columns>
                                                <asp:BoundField HeaderText="count" DataField="count" ItemStyle-HorizontalAlign="Right" ItemStyle-Width="75" HeaderStyle-HorizontalAlign="Center" />
                                                <asp:BoundField HeaderText="datestart" DataField="datestart" ItemStyle-HorizontalAlign="Right" ItemStyle-Width="75" HeaderStyle-HorizontalAlign="Center" />
                                                <asp:BoundField HeaderText="dateend" DataField="dateend" ItemStyle-HorizontalAlign="Right" ItemStyle-Width="75" HeaderStyle-HorizontalAlign="Center" />
                                            </Columns>
                                        </asp:GridView>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                        </div>
                    </div>
                </div>
                <div style="color: DarkRed;font-weight: bold;display: none;">
                    Select a client > select a campaign > click Run Report
                </div>
            </div>
            <div id="report_result" class="clearfix">
                <div id="search_grid" style="display: inline-block;vertical-align: top;">
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                        <Triggers>
                            <asp:PostBackTrigger ControlID="btnExportFull" />
                            <asp:PostBackTrigger ControlID="btnCallDispositions" />
                            <asp:PostBackTrigger ControlID="btnCCPerformance" />
                            <asp:PostBackTrigger ControlID="btnInterval" />
                            <asp:AsyncPostBackTrigger ControlID="btnRunReport" />
                        </Triggers>
                        <ContentTemplate>
                            <asp:Timer runat="server" ID="gvCountReport_Timer1" Interval="10000" OnTick="gvCountReport_TimerTick" Enabled="false"></asp:Timer>
                            <script type="text/javascript">
                                //Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(function(evt, args) { OnLoad_Function(); });
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
                            <div id="msgOther" runat="server" style="display: inline;width: 100%;color: DarkRed;font-weight: bold;">
                                <asp:Label ID="rpMessage" runat="server" Text="Select a client > select a campaign > click Run Report" />
                                <br />
                            </div>
                            <div style="width: 100%;vertical-align: top;height: 30px;">
                                <div class="report-controls" style="vertical-align: top;height: 30px;">
                                    <div style="display: inline-block;margin-left: 0px;">
                                        <asp:Button ID="btnRunReport" runat="server"
                                            Text="Run Report"
                                            OnClientClick="this.disabled = true; this.value = 'Running...';" 
                                            OnClick="GridView_Refresh"
                                            UseSubmitBehavior="false"
                                            />
                                    </div>
                                    <div style="display: inline-block;margin-left: 25px;">
                                        <asp:Button ID="btnDashboardReport" runat="server" Text="Selective Reporting" OnClick="GoTo_Other_Dashboard" Visible="true" />
                                    </div>
                                    <div style="display: inline-block;margin-left: 25px;">
                                        <asp:Button ID="btnExportFull" runat="server" Text="Export full Report" OnClick="Custom_Export_Excel_Dashboard" Visible="false" />
                                    </div>
                                    <div style="display: inline-block;margin-left: 25px;">
                                        <asp:Label ID="lblWeekDay" runat="server" Text="" />
                                    </div>
                                </div>
                                <div id="gvCountReport_Timer" runat="server" class="report-timers">
                                    <div>
                                        Refreshed @ <asp:Label ID="gvCR_lstRefresh" runat="server" Text="00:00:00" />
                                         | Next in <div class="report-timers-nextin"><span id="gvCR_refreshCountdown"></span></div>
                                         | Auto Status <asp:Label ID="gvCR_lblTimer" Width="45px" runat="server" Text="active" ForeColor="Blue" />
                                        <asp:CheckBox ID="gvCR_tglTimer" runat="server" Text="Toggle Auto Refresh" Checked="true" AutoPostBack="true" OnCheckedChanged="gvReport_TimerToggle" />
                                    </div>
                                    <div style="font-weight: bold;">
                                        <asp:Label ID="rpElapsed" runat="server" Text="" />
                                    </div>
                                </div>
                            </div>
                            <div id="dashboard">
                                <style type="text/css">
                                    .dash_label
                                    {
                                        margin: 5px 0px;
                                        font-weight: bold;
                                        font-size: 14px;
                                        text-align: center;
                                    }
                                    .dash_section
                                    {
                                        display: inline-block;
                                        width: 300px;
                                        height: 400px;
                                        vertical-align: top;
                                    }
                                    .dash_section_inner w400
                                    {
                                        width: 400px;
                                    }
                                    .dash_section_inner
                                    {
                                        height: 380px;
                                        vertical-align: top;
                                        border-style: solid;
                                        border-width: 2px;
                                        border-color: lightgrey;
                                    }
                                    .dash_section_inner_half
                                    {
                                        height: 170px;
                                        vertical-align: top;
                                        border-style: solid;
                                        border-width: 2px;
                                        border-color: lightgrey;
                                    }
                                    .center
                                    {
                                        text-align: center;
                                    }
                                    .dash_table
                                    {
                                        width: 95%;
                                        margin: 5px;
                                
                                    }
                                    .dash_table thead, .dash_grid th
                                    {
                                        background-color: lightgrey;
                                    }
                                    .dash_table td, .dash_grid td
                                    {
                                        background-color: whitesmoke;
                                    }
                                    .dash_table, .dash_grid
                                    {
                                        text-align: center;
                                    }
                                    .dash_table table, .dash_table th, .dash_table td, .dash_grid table, .dash_grid tbody, .dash_grid td
                                    {
                                        border-style: solid;
                                        border-width: 1px;
                                        border-color: darkgrey;
                                    }
                                    .mtop
                                    {
                                        margin-top: 5px;
                                    }
                                    .left, .left table, .left th, .left td
                                    {
                                        text-align: left !important;
                                    }
                                    .dash_grid
                                    {
                                        margin: 5px;
                                        width: 95%;
                                    }
                                </style>
                                <div id="support_level" class="dash_section">
                                    <div class="dash_label" style="">
                                        Support Level
                                    </div>
                                    <div class="dash_section_inner center" style="">
                                        <chart:WebChartViewer id="chartSupportLevel" runat="server" />
                                    </div>
                                </div>
                                <div id="performance" class="dash_section" style="position: relative;width: 300px;">
                                    <div style="position: absolute; right: 5px;top: 0px;">
                                        <asp:Button ID="btnCCPerformance" runat="server" Text="Export" OnClick="Custom_Export_Excel_Dashboard" Visible="false" />
                                    </div>
                                    <asp:Panel ID="pnlCCPerformanceExport" runat="server" Visible="true">
                                        <div class="dash_label" style="font-weight: bold;">
                                            Call Center Performance
                                        </div>
                                        <div class="dash_section_inner" style="">
                                            <asp:Panel ID="pnlCCPerformance" runat="server" Visible="false">
                                                <%--<table class="dash_table" style="" cellpadding="0" cellspacing="0">
                                                    <thead>
                                                        <tr>
                                                            <th style="width: 80%;background-color: lightgrey;">
                                                                <asp:Label ID="lblAnswerPerf" runat="server" Text="Answered Within 90 Seconds" />
                                                            </th>
                                                            <th style="width: 20%;background-color: lightgrey;">%</th>
                                                        </tr>
                                                    </thead>
                                                    <tr>
                                                        <td style="background-color: whitesmoke;">
                                                            <asp:Label ID="cntAnswerPerf" runat="server" Text="#" />
                                                        </td>
                                                        <td style="background-color: whitesmoke;">
                                                            <asp:Label ID="perAnswerPerf" runat="server" Text="%" />
                                                        </td>
                                                    </tr>
                                                </table>--%>
                                                <table class="dash_table mtop" style="" cellpadding="0" cellspacing="0">
                                                    <thead>
                                                        <tr>
                                                            <th style="width: 80%;background-color: lightgrey;">
                                                                <asp:Label ID="lblTotalCalls" runat="server" Text="Total Calls" />
                                                            </th>
                                                            <th style="width: 20%;background-color: lightgrey;">%</th>
                                                        </tr>
                                                    </thead>
                                                    <tr>
                                                        <td style="background-color: whitesmoke;">
                                                            <asp:Label ID="cntTotalCalls" runat="server" Text="#" />
                                                        </td>
                                                        <td style="background-color: whitesmoke;">
                                                            <asp:Label ID="perTotalCalls" runat="server" Text="%" />
                                                        </td>
                                                    </tr>

                                                </table>
                                                <table class="dash_table mtop" style="" cellpadding="0" cellspacing="0">
                                                    <thead>
                                                        <tr>
                                                            <th style="width: 80%;background-color: lightgrey;">
                                                                <asp:Label ID="lblTotalAnswered" runat="server" Text="Total Answered" />
                                                            </th>
                                                            <th style="width: 20%;background-color: lightgrey;">%</th>
                                                        </tr>
                                                    </thead>
                                                    <tr>
                                                        <td style="background-color: whitesmoke;">
                                                            <asp:Label ID="cntTotalAnswered" runat="server" Text="#" />
                                                        </td>
                                                        <td style="background-color: whitesmoke;">
                                                            <asp:Label ID="perTotalAnswered" runat="server" Text="%" />
                                                        </td>
                                                    </tr>
                                                </table>
                                                <table class="dash_table mtop" style="" cellpadding="0" cellspacing="0">
                                                    <thead>
                                                        <tr>
                                                            <th style="width: 80%;background-color: lightgrey;">
                                                                <asp:Label ID="lblTotalAbandoned" runat="server" Text="Total Abandoned" />
                                                            </th>
                                                            <th style="width: 20%;background-color: lightgrey;">%</th>
                                                        </tr>
                                                    </thead>
                                                    <tr>
                                                        <td style="background-color: whitesmoke;">
                                                            <asp:Label ID="cntTotalAbandoned" runat="server" Text="#" />
                                                        </td>
                                                        <td style="background-color: whitesmoke;">
                                                            <asp:Label ID="perTotalAbandoned" runat="server" Text="%" />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </asp:Panel>
                                            <div class="dash_label" style="font-weight: bold;margin-top: 5px;">
                                                Abandon Intervals
                                            </div>
                                            <asp:Panel ID="pnlIntervalAbandon" runat="server" Visible="false">
                                                <table class="dash_table" style="" cellpadding="0" cellspacing="0">
                                                    <thead>
                                                        <tr>
                                                            <th style="width: 80%;background-color: lightgrey;">
                                                                <asp:Label ID="lblAbandoned90" runat="server" Text="Abandoned Within 90 Seconds" />
                                                            </th>
                                                            <th style="width: 20%;background-color: lightgrey;">%</th>
                                                        </tr>
                                                    </thead>
                                                    <tr>
                                                        <td style="background-color: whitesmoke;">
                                                            <asp:Label ID="cntAbandoned90" runat="server" Text="#" />
                                                        </td>
                                                        <td style="background-color: whitesmoke;">
                                                            <asp:Label ID="perAbandoned90" runat="server" Text="%" />
                                                        </td>
                                                    </tr>
                                                </table>
                                                <table class="dash_table mtop" style="" cellpadding="0" cellspacing="0">
                                                    <thead>
                                                        <tr>
                                                            <th style="width: 80%;background-color: lightgrey;">
                                                                <asp:Label ID="lblAbandoned120" runat="server" Text="Abandoned 90 to 120 Seconds" />
                                                            </th>
                                                            <th style="width: 20%;background-color: lightgrey;">%</th>
                                                        </tr>
                                                    </thead>
                                                    <tr>
                                                        <td style="background-color: whitesmoke;">
                                                            <asp:Label ID="cntAbandoned120" runat="server" Text="#" />
                                                        </td>
                                                        <td style="background-color: whitesmoke;">
                                                            <asp:Label ID="perAbandoned120" runat="server" Text="%" />
                                                        </td>
                                                    </tr>
                                                </table>
                                                <table class="dash_table mtop" style="" cellpadding="0" cellspacing="0">
                                                    <thead>
                                                        <tr>
                                                            <th style="width: 80%;background-color: lightgrey;">
                                                                <asp:Label ID="lblAbandoned120p" runat="server" Text="Abandoned Over 120 Seconds" />
                                                            </th>
                                                            <th style="width: 20%;background-color: lightgrey;">%</th>
                                                        </tr>
                                                    </thead>
                                                    <tr>
                                                        <td style="background-color: whitesmoke;">
                                                            <asp:Label ID="cntAbandoned120p" runat="server" Text="#" />
                                                        </td>
                                                        <td style="background-color: whitesmoke;">
                                                            <asp:Label ID="perAbandoned120p" runat="server" Text="%" />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </asp:Panel>
                                            <div class="dash_label" style="font-weight: bold;margin-top: 5px;">
                                                Answer Intervals
                                            </div>
                                            <asp:Panel ID="pnlIntervalAnswer" runat="server" Visible="false">
                                                <table class="dash_table" style="" cellpadding="0" cellspacing="0">
                                                    <thead>
                                                        <tr>
                                                            <th style="width: 80%;background-color: lightgrey;">
                                                                <asp:Label ID="lblAnswered90" runat="server" Text="Answered Within 90 Seconds" />
                                                            </th>
                                                            <th style="width: 20%;background-color: lightgrey;">%</th>
                                                        </tr>
                                                    </thead>
                                                    <tr>
                                                        <td style="background-color: whitesmoke;">
                                                            <asp:Label ID="cntAnswered90" runat="server" Text="#" />
                                                        </td>
                                                        <td style="background-color: whitesmoke;">
                                                            <asp:Label ID="perAnswered90" runat="server" Text="%" />
                                                        </td>
                                                    </tr>
                                                </table>
                                                <table class="dash_table mtop" style="" cellpadding="0" cellspacing="0">
                                                    <thead>
                                                        <tr>
                                                            <th style="width: 80%;background-color: lightgrey;">
                                                                <asp:Label ID="lblAnswered120" runat="server" Text="Answered 90 to 120 Seconds" />
                                                            </th>
                                                            <th style="width: 20%;background-color: lightgrey;">%</th>
                                                        </tr>
                                                    </thead>
                                                    <tr>
                                                        <td style="background-color: whitesmoke;">
                                                            <asp:Label ID="cntAnswered120" runat="server" Text="#" />
                                                        </td>
                                                        <td style="background-color: whitesmoke;">
                                                            <asp:Label ID="perAnswered120" runat="server" Text="%" />
                                                        </td>
                                                    </tr>
                                                </table>
                                                <table class="dash_table mtop" style="" cellpadding="0" cellspacing="0">
                                                    <thead>
                                                        <tr>
                                                            <th style="width: 80%;background-color: lightgrey;">
                                                                <asp:Label ID="lblAnswered120p" runat="server" Text="Answered Over 120 Seconds" />
                                                            </th>
                                                            <th style="width: 20%;background-color: lightgrey;">%</th>
                                                        </tr>
                                                    </thead>
                                                    <tr>
                                                        <td style="background-color: whitesmoke;">
                                                            <asp:Label ID="cntAnswered120p" runat="server" Text="#" />
                                                        </td>
                                                        <td style="background-color: whitesmoke;">
                                                            <asp:Label ID="perAnswered120p" runat="server" Text="%" />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </asp:Panel>
                                        </div>
                                    </asp:Panel>
                                </div>
                                <div id="intervals" class="dash_section" style="position: relative;width: 300px;">
                                    <div style="position: absolute; right: 5px;top: 0px;">
                                        <asp:Button ID="btnInterval" runat="server" Text="Export" OnClick="Panel_Export_Excel" Visible="false" />
                                    </div>
                                    <asp:Panel ID="pnlIntervalExport" runat="server" Visible="true">
                                        <div class="dash_label" style="font-weight: bold;">
                                            Call Times
                                        </div>
                                        <div class="dash_section_inner" style="">
                                            <asp:Panel ID="pnlCallTimes" runat="server" Visible="false">
                                                <table class="dash_table mtop" style="" cellpadding="0" cellspacing="0">
                                                    <thead>
                                                        <tr>
                                                            <th style="width: 100%;background-color: lightgrey;" colspan="2">
                                                                <asp:Label ID="lblSpeedAnswer" runat="server" Text="Speed of Answer" />
                                                            </th>
                                                        </tr>
                                                    </thead>
                                                    <tr>
                                                        <td style="background-color: whitesmoke;">
                                                            <asp:Label ID="avgSpeedAnswer" runat="server" Text="##:##:##" /> <span style="color: blue;font-weight: bold;">avg</span>
                                                        </td>
                                                        <td style="background-color: whitesmoke;">
                                                            <asp:Label ID="totalSpeedAnswer" runat="server" Text="##:##:##" /> <span style="color: darkorange;font-weight: bold;">total</span>
                                                        </td>
                                                    </tr>
                                                </table>
                                                <table class="dash_table mtop" style="" cellpadding="0" cellspacing="0">
                                                    <thead>
                                                        <tr>
                                                            <th style="width: 100%;background-color: lightgrey;" colspan="2">
                                                                <asp:Label ID="lblTalkTime" runat="server" Text="Talk Time" />
                                                            </th>
                                                        </tr>
                                                    </thead>
                                                    <tr>
                                                        <td style="background-color: whitesmoke;">
                                                            <asp:Label ID="avgTalkTime" runat="server" Text="##:##:##" /> <span style="color: blue;font-weight: bold;">avg</span>
                                                        </td>
                                                        <td style="background-color: whitesmoke;">
                                                            <asp:Label ID="totalTalkTime" runat="server" Text="##:##:##" /> <span style="color: darkorange;font-weight: bold;">total</span>
                                                        </td>
                                                    </tr>
                                                </table>
                                                <table class="dash_table mtop" style="" cellpadding="0" cellspacing="0">
                                                    <thead>
                                                        <tr>
                                                            <th style="width: 100%;background-color: lightgrey;" colspan="2">
                                                                <asp:Label ID="lblAbandonedTime" runat="server" Text="Abandoned Time" />
                                                            </th>
                                                        </tr>
                                                    </thead>
                                                    <tr>
                                                        <td style="background-color: whitesmoke;">
                                                            <asp:Label ID="avgAbandonedTime" runat="server" Text="##:##:##" /> <span style="color: blue;font-weight: bold;">avg</span>
                                                        </td>
                                                        <td style="background-color: whitesmoke;">
                                                            <asp:Label ID="totalAbandonedTime" runat="server" Text="##:##:##" /> <span style="color: darkorange;font-weight: bold;">total</span>
                                                        </td>
                                                    </tr>
                                                </table>
                                                <table class="dash_table mtop" style="" cellpadding="0" cellspacing="0">
                                                    <thead>
                                                        <tr>
                                                            <th style="width: 100%;background-color: lightgrey;" colspan="2">
                                                                <asp:Label ID="lblAfterCallTime" runat="server" Text="After Call Time" />
                                                            </th>
                                                        </tr>
                                                    </thead>
                                                    <tr>
                                                        <td style="background-color: whitesmoke;">
                                                            <asp:Label ID="avgAfterCallTime" runat="server" Text="##:##:##" /> <span style="color: blue;font-weight: bold;">avg</span>
                                                        </td>
                                                        <td style="background-color: whitesmoke;">
                                                            <asp:Label ID="totalAfterCallTime" runat="server" Text="##:##:##" /> <span style="color: darkorange;font-weight: bold;">total</span>
                                                        </td>
                                                    </tr>
                                                </table>
                                                <table class="dash_table mtop" style="" cellpadding="0" cellspacing="0">
                                                    <thead>
                                                        <tr>
                                                            <th style="width: 100%;background-color: lightgrey;" colspan="2">
                                                                <asp:Label ID="lblQueueTime" runat="server" Text="Queue Time" />
                                                            </th>
                                                        </tr>
                                                    </thead>
                                                    <tr>
                                                        <td style="background-color: whitesmoke;">
                                                            <asp:Label ID="avgQueueTime" runat="server" Text="##:##:##" /> <span style="color: blue;font-weight: bold;">avg</span>
                                                        </td>
                                                        <td style="background-color: whitesmoke;">
                                                            <asp:Label ID="totalQueueTime" runat="server" Text="##:##:##" /> <span style="color: darkorange;font-weight: bold;">total</span>
                                                        </td>
                                                    </tr>
                                                </table>
                                                <table class="dash_table mtop" style="" cellpadding="0" cellspacing="0">
                                                    <thead>
                                                        <tr>
                                                            <th style="width: 100%;background-color: lightgrey;" colspan="2">
                                                                <asp:Label ID="lblHandleTime" runat="server" Text="Handle Time" />
                                                            </th>
                                                        </tr>
                                                    </thead>
                                                    <tr>
                                                        <td style="background-color: whitesmoke;">
                                                            <asp:Label ID="avgHandleTime" runat="server" Text="##:##:##" /> <span style="color: blue;font-weight: bold;">avg</span>
                                                        </td>
                                                        <td style="background-color: whitesmoke;">
                                                            <asp:Label ID="totalHandleTime" runat="server" Text="##:##:##" /> <span style="color: darkorange;font-weight: bold;">total</span>
                                                        </td>
                                                    </tr>
                                                </table>
                                                <table class="dash_table mtop" style="" cellpadding="0" cellspacing="0">
                                                    <thead>
                                                        <tr>
                                                            <th style="width: 100%;background-color: lightgrey;" colspan="2">
                                                                <asp:Label ID="lblIVRTime" runat="server" Text="IVR Time" />
                                                            </th>
                                                        </tr>
                                                    </thead>
                                                    <tr>
                                                        <td style="background-color: whitesmoke;">
                                                            <asp:Label ID="avgIVRTime" runat="server" Text="##:##:##" /> <span style="color: blue;font-weight: bold;">avg</span>
                                                        </td>
                                                        <td style="background-color: whitesmoke;">
                                                            <asp:Label ID="totalIVRTime" runat="server" Text="##:##:##" /> <span style="color: darkorange;font-weight: bold;">total</span>
                                                        </td>
                                                    </tr>
                                                </table>
                                                <table class="dash_table mtop" style="" cellpadding="0" cellspacing="0">
                                                    <thead>
                                                        <tr>
                                                            <th style="width: 100%;background-color: lightgrey;" colspan="2">
                                                                <asp:Label ID="lblThirdPartyTime" runat="server" Text="Third Party Time" />
                                                            </th>
                                                        </tr>
                                                    </thead>
                                                    <tr>
                                                        <td style="background-color: whitesmoke;">
                                                            <asp:Label ID="avgThirdPartyTime" runat="server" Text="##:##:##" /> <span style="color: blue;font-weight: bold;">avg</span>
                                                        </td>
                                                        <td style="background-color: whitesmoke;">
                                                            <asp:Label ID="totalThirdPartyTime" runat="server" Text="##:##:##" /> <span style="color: darkorange;font-weight: bold;">total</span>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </asp:Panel>
                                        </div>
                                        <div class="dash_label" style="font-weight: bold;margin-top: 15px;display: none;">
                                            Answer Intervals
                                        </div>
                                    </asp:Panel>
                                </div>
                                <div id="disposition" class="dash_section" style="width: 330px;">
                                    <div class="dash_label" style="position: relative;font-weight: bold;">
                                        Count Breakdown
                                        <div style="position: absolute; right: 5px;bottom: 0px;">
                                            <asp:Button ID="btnCallDispositions" runat="server" Text="Export" OnClick="GridView_Export_Excel" Visible="false" />
                                        </div>
                                    </div>
                                    <div class="dash_section_inner" style="overflow: scroll;">
                                        <div>
                                            <asp:GridView ID="gvCallDispositions"
                                                runat="server"
                                                AutoGenerateColumns="False"
                                                AllowSorting="False"
                                                GridLines="Vertical"
                                                CellPadding="0"
                                                CellSpacing="0"

                                                CssClass="dash_grid"
                                                >
                                                <AlternatingRowStyle CssClass="Portal_GridView_Standard_Alternate" />
                                                <SelectedRowStyle CssClass="Portal_GridView_Standard_Selected" />
                                                <Columns>
                                                    <asp:BoundField HeaderText="Disposition" DataField="dispositionname" HeaderStyle-CssClass="left" ItemStyle-CssClass="left" HeaderStyle-Width="64%" HeaderStyle-BackColor="lightgrey" ItemStyle-BackColor="whitesmoke" />
                                                    <asp:BoundField HeaderText="Count" DataField="total_calls" HeaderStyle-CssClass="center" ItemStyle-CssClass="center" HeaderStyle-Width="18%" HeaderStyle-BackColor="lightgrey" ItemStyle-BackColor="whitesmoke" />
                                                    <asp:TemplateField HeaderText="%" HeaderStyle-CssClass="center" ItemStyle-CssClass="center"  HeaderStyle-Width="18%"  HeaderStyle-BackColor="lightgrey" ItemStyle-BackColor="whitesmoke">
                                                        <ItemTemplate>
                                                            <asp:Label ID="dCount" runat="server" Text='<%# dispo_percent(Eval("total_calls").ToString()) %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>                                       
                                                </Columns>
                                                <PagerSettings Position="Top" />
                                                <EmptyDataTemplate>
                                                </EmptyDataTemplate>
                                            </asp:GridView>
                                            <div style="margin: 5px;">
                                                <asp:Label ID="totalDispos" runat="server" Text="" />
                                            </div>
                                        </div>
                                        <div>
                                        <asp:GridView ID="gvCampaignCount"
                                            runat="server"
                                            AutoGenerateColumns="False"
                                            AllowSorting="True"
                                            GridLines="Vertical"
                                            CellPadding="0"
                                            CellSpacing="0"
                                            CssClass="dash_grid"
                                            Visible="false"
                                            >
                                            <AlternatingRowStyle CssClass="Portal_GridView_Standard_Alternate" />
                                            <SelectedRowStyle CssClass="Portal_GridView_Standard_Selected" />
                                            <Columns>
                                                <asp:BoundField HeaderText="Campaign" DataField="campaign" HeaderStyle-CssClass="left" ItemStyle-CssClass="left" HeaderStyle-Width="64%" />
                                                <asp:BoundField HeaderText="Count" DataField="total_calls" HeaderStyle-CssClass="center" ItemStyle-CssClass="center" HeaderStyle-Width="18%" />
                                                <asp:TemplateField HeaderText="%" HeaderStyle-CssClass="center" ItemStyle-CssClass="center"  HeaderStyle-Width="18%">
                                                    <ItemTemplate>
                                                        <asp:Label ID="total_calls" runat="server" Text='<%# campaign_percent(Eval("total_calls").ToString()) %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>                                       
                                            </Columns>
                                            <PagerSettings Position="Top" />
                                            <EmptyDataTemplate>
                                            </EmptyDataTemplate>
                                        </asp:GridView>
                                            <div style="margin: 5px;">
                                                <asp:Label ID="Label1" runat="server" Text="" />
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div>
                                <asp:HiddenField ID="cntSupportLevel" runat="server" />
                                <asp:HiddenField ID="hfDispoCount" runat="server" />
                                <asp:HiddenField ID="hfCampaignCount" runat="server" />                                
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>
            <div id="admin_section">
                <asp:UpdatePanel ID="upAdminSection" runat="server">
                    <Triggers>
                    </Triggers>
                    <ContentTemplate>
                        <div>
                            <asp:Label ID="lblLoadTime" runat="server" Text="<br />Load Time" />
                        </div>
                        <div>
                            <asp:Label ID="lblMessage" runat="server" Text="" />
                        </div>
                        <div>
                            <asp:Label ID="sqlPrint" runat="server" Text="" />
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
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
                <asp:Label ID="sqlCmnd" runat="server" Text="" />
                <asp:Label ID="msgLabel" runat="server" Text="" />
                <asp:DetailsView ID="ErrorView" runat="server" AutoGenerateRows="True" ForeColor="Black"
                    GridLines="Vertical" HeaderText="Error Display" CssClass="Portal_DetailView_Standard">
                    <HeaderStyle CssClass="Portal_DetailView_Standard_Header" />
                    <RowStyle CssClass="Portal_DetailView_Standard_Row" />
                    <FieldHeaderStyle Width="100px" />
                    <Fields>
                    </Fields>
                    <EmptyDataTemplate>
                        No Errors to Report;
                    </EmptyDataTemplate>
                </asp:DetailsView>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder2" Runat="Server">
</asp:Content>

