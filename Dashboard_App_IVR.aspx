<%@ Page Title="Dashboard IVR" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Dashboard_App_IVR.aspx.cs" Inherits="Dashboard_App_IVR" %>
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
    	width: 275px !important;
    }

    </style>
    <link href="css/start/jquery-ui-1.8.18.custom.css" rel="stylesheet" type="text/css" />
    <link href="css/start/jquery.ui.selectmenu.css" rel="stylesheet" type="text/css" />
    <script src="js/jquery-1.7.1.min.js" type="text/javascript"></script>
    <script src="js/jquery-ui-1.8.18.custom.min.js" type="text/javascript"></script>
    <script src="js/jquery-ui-timepicker-addon.js" type="text/javascript"></script>
    <script src="js/jquery.ui.selectmenu.js" type="text/javascript"></script>
    <link href="js/jquery.countdown.css" rel="stylesheet" type="text/css" />
    <script src="js/jquery.plugin.min.js" type="text/javascript"></script>
    <script src="js/jquery.countdown.min.js" type="text/javascript"></script>
    <script src="js/jquery-gh-custom-objects.js" type="text/javascript"></script>
    <link href="js/jquery.multiselect.css" rel="stylesheet" type="text/css" />
    <script src="js/jquery.multiselect.js" type="text/javascript"></script>
    <script type="text/javascript">
        function OnLoad_Function() {
            GH_Buttons();
            //GH_DropDown();
            GH_DatePickerToday();
            GH_Select();
            GH_SelectMultiple();
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
            .dash_selective_section_left
            {
                display: inline-block;
                width: 718px;
                height: 600px;
                vertical-align: middle;
                text-align: center;
            }
            .dash_selective_section_right
            {
                display: inline-block;
                width: 518px;
                height: 600px;
                vertical-align: middle;
                text-align: center;
            }
            .dash_selective_section_inner
            {
                height: 260px; /* 26+4+10==40 */
                vertical-align: top;
                border-style: solid;
                border-width: 2px;
                border-color: lightgrey;
                margin: 5px 0px;
                margin-bottom: 7px;
            }
        </style>
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
                margin-top: 15px;
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
        <style type="text/css">
            .dash_grid_section
            {
                margin: 5px;
                min-height: 260px;
                max-height: 800px;
                overflow: scroll;
                vertical-align: top;
                border-style: solid;
                border-width: 2px;
                border-color: lightgrey;
            }
        </style>
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
    <asp:Panel ID="pnlMain" runat="server" DefaultButton="Button2">
        <div id="default_container"  style="margin-left: 5px;">
            <div id="report_header" style="color: DarkRed;font-weight: bold;">
                Greenwood & Hall Client Reporting Dashboard
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
                    <div id="filter_dates" class="filter_section">
                        <h3>Record Dates</h3>
                        <div id="datetime_start">
                            <asp:Label ID="lblDateStart" runat="server" Text="Start Time" CssClass="filter_label1" />
                            <asp:TextBox ID="dtStartDate" runat="server" Width="75px" CssClass="ghDatePickerStart" />
                            <asp:TextBox ID="dtStartTime" runat="server" Width="50px" CssClass="ghTimePickerStart" />
                        </div>
                        <div id="datetime_end">
                            <asp:Label ID="lblDateEnd" runat="server" Text="End Time" CssClass="filter_label1" />
                            <asp:TextBox ID="dtEndDate" runat="server" Width="75px" CssClass="ghDatePickerEnd" />
                            <asp:TextBox ID="dtEndTime" runat="server" Width="50px" CssClass="ghTimePickerEnd" />
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
                    <div id="filter_options" class="filter_section">
                        <h3>Options</h3>
                        <div id="file_name">
                            <asp:Label ID="lblFileName" runat="server" Text="File Name" CssClass="filter_label1" />
                            <asp:TextBox ID="tbFileName" runat="server" Width="125px" />
                        </div>
                    </div>
                    <div id="service_status" class="filter_section">
                        <asp:UpdatePanel ID="UpdatePanel3" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <h3>Service Status</h3>
                                <style type="text/css">
                                    .filter_section .service_section
                                    {
                                        margin: 0px 15px;text-align: center;
                                    }
                                </style>
                                <div id="service_name" class="service_section" style="font-weight: bold;">
                                    <asp:Label ID="lblServiceName" runat="server" Text="Name" />
                                </div>
                                <div id="service_label" class="service_section" style="margin-top: 2px;">
                                    <asp:Panel ID="service_panel" runat="server" BackColor="Red">
                                        <asp:Label ID="lblServiceStatus" runat="server" Text="RUNNING" />
                                    </asp:Panel>
                                </div>
                                <div id="service_date" style="margin-top: 2px;">
                                    <asp:Label ID="lblSrvDateLabel" runat="server" Text="Update Time" CssClass="filter_label1" />
                                    <asp:Label ID="lblServiceDate" runat="server" Text="2016-04-08 11:02:00" />
                                </div>
                                <div id="service_message" class="service_section" style="text-align: left;margin-top: 2px;">
                                    <asp:Label ID="lblServiceMsg" runat="server" Text="" />
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </div>
                <div style="color: DarkRed;font-weight: bold;display: none;">
                    Select a date range > click Run Report
                    <br />
                </div>
            </div>
            <div id="report_result" class="clearfix">
                <div id="search_grid" style="display: inline-block;vertical-align: top;">
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                        <Triggers>
                            <asp:PostBackTrigger ControlID="btnExportFull" />
                            <asp:PostBackTrigger ControlID="btnCallDetails" />
                        </Triggers>
                        <ContentTemplate>
                            <script type="text/javascript">
                                //Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(function(evt, args) { OnLoad_Function(); });
                                Sys.WebForms.PageRequestManager.getInstance().add_endRequest(EndRequestHandler);
                                function EndRequestHandler(sender, args) {
                                    var Error = args.get_error();

                                    if (Error != null) {
                                        alert("An error occurred while processing request. Please try again.");
                                    } else {
                                        $('#<%= Button2.ClientID  %> ').find("[value=0]").attr("disabled", "disabled");
                                        $('#<%= Button2.ClientID  %> ').find("[value=0]").attr("aria-disabled", "true");
                                    }
                                }
                            </script>
                            <div id="msgOther" runat="server" style="display: inline;width: 100%;color: DarkRed;font-weight: bold;">
                                <asp:Label ID="rpMessage" runat="server" Text="Select a client > select a campaign > click Run Report" />
                                <br />
                            </div>
                            <div style="width: 100%;vertical-align: top;height: 30px;">
                                <div class="report-controls" style="vertical-align: top;height: 30px;">
                                    <asp:Button ID="Button2" runat="server"
                                        Text="Run Report"
                                        OnClientClick="this.disabled = true; this.value = 'Running...';" 
                                        UseSubmitBehavior="false" 
                                        OnClick="GridView_Refresh"
                                        />
                                    <div style="display: inline-block;margin-left: 25px;">
                                        <asp:Button ID="btnExportFull" runat="server" Text="Export full Report" OnClick="Custom_Export_Excel_Dashboard" Visible="false" />
                                    </div>
                                    <div style="display: inline-block;">
                                        <div style="color: DarkRed;font-weight: bold;">
                                            <asp:Label ID="rpElapsed" runat="server" Text="" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div id="selective">
                                <div class="dash_selective_section_left" style="">
                                    <div class="dash_label" style="">
                                        Counts
                                    </div>
                                    <div class="dash_selective_section_inner" style="">
                                        <chart:WebChartViewer id="chartCallType" runat="server" />
                                    </div>
                                    <div class="dash_label" style="">
                                        File Counts
                                    </div>
                                    <div class="dash_selective_section_inner" style="">
                                        <chart:WebChartViewer id="chartDesignationCount" runat="server" />
                                    </div>
                                </div>
                                <div class="dash_selective_section_right" style="">
                                    <div class="dash_label" style="">
                                        Record Status
                                    </div>
                                    <div class="dash_selective_section_inner" style="overflow: scroll;">
                                        <asp:GridView ID="gvRecordStatus"
                                            runat="server"
                                            AutoGenerateColumns="False"
                                            AllowSorting="True"
                                            GridLines="Vertical"
                                            CellPadding="0"
                                            CellSpacing="0"

                                            CssClass="dash_grid">
                                            <AlternatingRowStyle CssClass="Portal_GridView_Standard_Alternate" />
                                            <SelectedRowStyle CssClass="Portal_GridView_Standard_Selected" />
                                            <Columns>
                                                <asp:BoundField HeaderText="total" DataField="total" />
                                                <asp:BoundField HeaderText="completed" DataField="completed" />
                                                <asp:BoundField HeaderText="pending" DataField="pending" />
                                                <asp:BoundField HeaderText="invalid" DataField="invalid" />
                                                <asp:BoundField HeaderText="cleared" DataField="cleared" />
                                                <asp:BoundField HeaderText="discarded" DataField="discarded" />
                                                <asp:BoundField HeaderText="cc_pending" DataField="cc_pending" Visible="false" />
                                                <asp:BoundField HeaderText="cc_amount" DataField="cc_amount" Visible="false" />
                                                <asp:BoundField HeaderText="file_vc" DataField="file_vc" Visible="false" />
                                                <asp:BoundField HeaderText="file_cc" DataField="file_cc" Visible="false" />
                                                <asp:BoundField HeaderText="file_rani" DataField="file_rani" Visible="false" />
                                                <asp:BoundField HeaderText="file_opt" DataField="file_opt" Visible="false" />
                                                <asp:BoundField HeaderText="file_cat" DataField="file_cat" Visible="false" />
                                                <asp:BoundField HeaderText="processed" DataField="processed" Visible="false" />
                                            </Columns>
                                            <PagerSettings Position="Top" />
                                            <EmptyDataTemplate>
                                            </EmptyDataTemplate>
                                        </asp:GridView>
                                    </div>
                                    <div class="dash_label" style="">
                                        File Details
                                    </div>
                                    <div class="dash_selective_section_inner" style="overflow: scroll;">
                                        <asp:GridView ID="gvCampaignCount"
                                            runat="server"
                                            AutoGenerateColumns="False"
                                            AllowSorting="True"
                                            GridLines="Vertical"
                                            CellPadding="0"
                                            CellSpacing="0"

                                            CssClass="dash_grid">
                                            <AlternatingRowStyle CssClass="Portal_GridView_Standard_Alternate" />
                                            <SelectedRowStyle CssClass="Portal_GridView_Standard_Selected" />
                                            <Columns>
                                                <asp:BoundField HeaderText="filename" DataField="filename" ItemStyle-CssClass="left" />
                                                <asp:BoundField HeaderText="start" DataField="start" ItemStyle-CssClass="left" />
                                                <asp:BoundField HeaderText="prsd" DataField="processed" />
                                                <asp:BoundField HeaderText="dup" DataField="duplicate" />
                                                <asp:BoundField HeaderText="invld" DataField="invalid" />
                                                <asp:BoundField HeaderText="crpt" DataField="corrupted" />
                                                <asp:BoundField HeaderText="err" DataField="error" />
                                                <asp:BoundField HeaderText="errnt" DataField="errornet" />
                                                <asp:BoundField HeaderText="othr" DataField="other" />
                                            </Columns>
                                            <PagerSettings Position="Top" />
                                            <EmptyDataTemplate>
                                            </EmptyDataTemplate>
                                        </asp:GridView>
                                        <asp:GridView ID="gvFileErrors"
                                            runat="server"
                                            AutoGenerateColumns="False"
                                            AllowSorting="True"
                                            GridLines="Vertical"
                                            CellPadding="0"
                                            CellSpacing="0"

                                            CssClass="dash_grid">
                                            <AlternatingRowStyle CssClass="Portal_GridView_Standard_Alternate" />
                                            <SelectedRowStyle CssClass="Portal_GridView_Standard_Selected" />
                                            <Columns>
                                                <asp:BoundField HeaderText="filename" DataField="filename" ItemStyle-CssClass="left" />
                                    <asp:TemplateField HeaderText="logdate" ItemStyle-CssClass="left" >
                                        <ItemTemplate>
                                            <asp:Label ID="logdate" runat="server" Text='<%# ghFunctions.date_label_short(Eval("logdate").ToString()) %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                                <asp:BoundField HeaderText="recordid" DataField="recordid" />
                                                <asp:BoundField HeaderText="calldate" DataField="calldate" />
                                                <asp:BoundField HeaderText="calltime" DataField="calltime" />
                                                <asp:BoundField HeaderText="ani" DataField="ani" />
                                                <asp:BoundField HeaderText="log" DataField="log" />
                                            </Columns>
                                            <PagerSettings Position="Top" />
                                            <EmptyDataTemplate>
                                            </EmptyDataTemplate>
                                        </asp:GridView>
                                    </div>
                                </div>
                            </div>
                            <div id="dashboard" runat="server" visible="false">
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
                                                <table class="dash_table" style="" cellpadding="0" cellspacing="0">
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
                                                </table>
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
                                                <table class="dash_table mtop" style="" cellpadding="0" cellspacing="0">
                                                    <thead>
                                                        <tr>
                                                            <th style="width: 100%;background-color: lightgrey;">
                                                                <asp:Label ID="lblSpeedAnswer" runat="server" Text="Average Speed of Answer" />
                                                            </th>
                                                        </tr>
                                                    </thead>
                                                    <tr>
                                                        <td style="background-color: whitesmoke;">
                                                            <asp:Label ID="avgSpeedAnswer" runat="server" Text="##:##:##" />
                                                        </td>
                                                    </tr>
                                                </table>
                                                <table class="dash_table mtop" style="" cellpadding="0" cellspacing="0">
                                                    <thead>
                                                        <tr>
                                                            <th style="width: 100%;background-color: lightgrey;">
                                                                <asp:Label ID="lblTalkTime" runat="server" Text="Average Talk Time" />
                                                            </th>
                                                        </tr>
                                                    </thead>
                                                    <tr>
                                                        <td style="background-color: whitesmoke;">
                                                            <asp:Label ID="avgTalkTime" runat="server" Text="##:##:##" />
                                                        </td>
                                                    </tr>
                                                </table>
                                                <table class="dash_table mtop" style="" cellpadding="0" cellspacing="0">
                                                    <thead>
                                                        <tr>
                                                            <th style="width: 100%;background-color: lightgrey;">
                                                                <asp:Label ID="lblAbandonedTime" runat="server" Text="Average Abandoned Time" />
                                                            </th>
                                                        </tr>
                                                    </thead>
                                                    <tr>
                                                        <td style="background-color: whitesmoke;">
                                                            <asp:Label ID="avgAbandonedTime" runat="server" Text="##:##:##" />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </asp:Panel>
                                        </div>
                                    </asp:Panel>
                                </div>
                                <div id="intervals" class="dash_section" style="position: relative;width: 300px;">
                                    <div style="position: absolute; right: 5px;top: 0px;">
                                    </div>
                                    <asp:Panel ID="pnlIntervalExport" runat="server" Visible="true">
                                        <div class="dash_label" style="font-weight: bold;">
                                            Abandon Intervals
                                        </div>
                                        <div class="dash_section_inner_half" style="">
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
                                        </div>
                                        <div class="dash_label" style="font-weight: bold;margin-top: 15px;">
                                            Answer Intervals
                                        </div>
                                        <div class="dash_section_inner_half" style="">
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
                                <div id="disposition" class="dash_section" style="width: 330px;">
                                    <div class="dash_label" style="position: relative;font-weight: bold;">
                                        Call Disposition Detail
                                        <div style="position: absolute; right: 5px;bottom: 0px;">
                                        </div>
                                    </div>
                                    <div class="dash_section_inner" style="overflow: scroll;">
                                        <asp:GridView ID="gvCallDispositions"
                                            runat="server"
                                            AutoGenerateColumns="False"
                                            AllowSorting="False"
                                            GridLines="Vertical"
                                            CellPadding="0"
                                            CellSpacing="0"

                                            CssClass="dash_grid">
                                            <AlternatingRowStyle CssClass="Portal_GridView_Standard_Alternate" />
                                            <SelectedRowStyle CssClass="Portal_GridView_Standard_Selected" />
                                            <Columns>
                                                <asp:BoundField HeaderText="Disposition" DataField="disposition" HeaderStyle-CssClass="left" ItemStyle-CssClass="left" HeaderStyle-Width="64%" HeaderStyle-BackColor="lightgrey" ItemStyle-BackColor="whitesmoke" />
                                                <asp:BoundField HeaderText="Count" DataField="count" HeaderStyle-CssClass="center" ItemStyle-CssClass="center" HeaderStyle-Width="18%" HeaderStyle-BackColor="lightgrey" ItemStyle-BackColor="whitesmoke" />
                                                <asp:TemplateField HeaderText="%" HeaderStyle-CssClass="center" ItemStyle-CssClass="center"  HeaderStyle-Width="18%"  HeaderStyle-BackColor="lightgrey" ItemStyle-BackColor="whitesmoke">
                                                    <ItemTemplate>
                                                        <asp:Label ID="dCount" runat="server" Text='<%# dispo_percent(Eval("count").ToString()) %>' />
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
                                </div>
                            </div>
                            <div id="grid_details" class="" style="margin-top: 25px;">
                                <div class="dash_label" style="position: relative;">
                                    Record Detail
                                    <div style="position: absolute; right: 5px;bottom: 0px;">
                                        <asp:Button ID="btnCallDetails" runat="server" Text="Export" OnClick="Custom_Export_Excel_RecordDetails" Visible="false" />
                                    </div>
                                </div>
                                <div class="dash_grid_section" style="">
                                    <asp:UpdatePanel ID="UpdatePanel5" runat="server">
                                        <Triggers>
                                        </Triggers>
                                        <ContentTemplate>
                                            <asp:GridView ID="gvCallDetails"
                                                runat="server"
                                                AutoGenerateColumns="False"
                                                AllowSorting="True"
                                                GridLines="Vertical"
                                                CellPadding="0"
                                                CellSpacing="0"
                                                AllowPaging="true" PageSize="100"
                                                OnPageIndexChanging="GridView_PageIndexChanging"
                                                OnDataBound="GridView_DataBound"
                                                CssClass="dash_grid"
                                                >
                                                <AlternatingRowStyle CssClass="Portal_GridView_Standard_Alternate" />
                                                <SelectedRowStyle CssClass="Portal_GridView_Standard_Selected" />
                                                <PagerSettings Position="Top" />
                                                <PagerStyle VerticalAlign="Bottom" HorizontalAlign="Center" CssClass="Portal_Gridview_Pager" />
                                                <PagerTemplate>
                                                    <table class="Portal_Gridview_Pager">
                                                        <tr>
                                                            <td>
                                                                <div style="display: inline-block;margin-left: 0px;">
                                                                    <asp:Label ID="MessageLabel" runat="server" Text="Page:" ForeColor="Black" Font-Bold="true" />
                                                                    <asp:DropDownList ID="PageDropDownList" runat="server"
                                                                        AutoPostBack="true"
                                                                        OnSelectedIndexChanged="GridView_PageDropDownList_SelectedIndexChanged" 
                                                                        Width="35px" Width2="35px"
                                                                        data-theme="none"
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
                                                    <asp:BoundField HeaderText="recordid" DataField="recordid" />
                                                    <asp:BoundField HeaderText="calldate" DataField="calldate" />
                                                    <asp:BoundField HeaderText="calltime" DataField="calltime" />
                                                    <asp:BoundField HeaderText="ani" DataField="ani" />
                                                    <asp:BoundField HeaderText="status" DataField="status" />
                                                    <asp:BoundField HeaderText="callid" DataField="callid" />
                                                    <asp:BoundField HeaderText="donationccinfoid" DataField="donationccinfoid" />
                                                    <asp:BoundField HeaderText="file_vc" DataField="file_vc" />
                                                    <asp:BoundField HeaderText="file_cc" DataField="file_cc" />
                                                    <asp:BoundField HeaderText="file_rani" DataField="file_rani" />
                                                    <asp:BoundField HeaderText="file_opt" DataField="file_opt" />
                                                    <asp:BoundField HeaderText="file_cat" DataField="file_cat" />
                                                    <asp:BoundField HeaderText="started" DataField="started" DataFormatString="{0:MM/dd/yyyy HH:mm}" />
                                                    <asp:BoundField HeaderText="completed" DataField="completed" DataFormatString="{0:MM/dd/yyyy HH:mm}" />
                                                </Columns>
                                                <PagerSettings Position="Top" />
                                                <EmptyDataTemplate>
                                                </EmptyDataTemplate>
                                            </asp:GridView>
                                            <asp:GridView ID="gvCallDetailsExport" runat="server" AutoGenerateColumns="False" Visible="false" >
                                                <Columns>
                                                    <asp:BoundField HeaderText="recordid" DataField="recordid" />
                                                    <asp:BoundField HeaderText="calldate" DataField="calldate" />
                                                    <asp:BoundField HeaderText="calltime" DataField="calltime" />
                                                    <asp:BoundField HeaderText="ani" DataField="ani" />
                                                    <asp:BoundField HeaderText="status" DataField="status" />
                                                    <asp:BoundField HeaderText="callid" DataField="callid" />
                                                    <asp:BoundField HeaderText="donationccinfoid" DataField="donationccinfoid" />
                                                    <asp:BoundField HeaderText="file_vc" DataField="file_vc" />
                                                    <asp:BoundField HeaderText="file_cc" DataField="file_cc" />
                                                    <asp:BoundField HeaderText="file_rani" DataField="file_rani" />
                                                    <asp:BoundField HeaderText="file_opt" DataField="file_opt" />
                                                    <asp:BoundField HeaderText="file_cat" DataField="file_cat" />
                                                    <asp:BoundField HeaderText="started" DataField="started" />
                                                    <asp:BoundField HeaderText="completed" DataField="completed" />
                                                </Columns>
                                            </asp:GridView>
                                            <div style="margin: 5px;">
                                                <asp:Label ID="lblCallDetails" runat="server" Text="" />
                                            </div>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
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
                            <asp:Label ID="lblLoadTime" runat="server" Text="Load Time" />
                        </div>
                        <div>                            
                            <asp:Label ID="lblMessage" runat="server" Text="" />
                            <asp:Label ID="lblResults" runat="server" Text="" />
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

