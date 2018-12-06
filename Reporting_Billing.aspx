<%@ Page Title="Billing" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Reporting_Billing.aspx.cs" Inherits="Reporting_Billing" %>
<%@ MasterType TypeName="MasterPage" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register TagPrefix="chart" Namespace="ChartDirector" Assembly="netchartdir" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="Server">
    <link href="css/content_general.css" rel="stylesheet" type="text/css" />
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
        .dash_table td, .dash_grid tr
        {
            background-color: whitesmoke;
        }
        tr.dash_grid_alternate
        {
            background-color: #FFFFFF;
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
            min-height: 100px;
            max-height: 800px;
            overflow: scroll;
            vertical-align: top;
            border-style: solid;
            border-width: 2px;
            border-color: lightgrey;
        }
    </style>
    <script type="text/javascript">
        function toggleIncludeReports(cntrl) {
            $(document).ready(function () {
                
                if ($(cntrl).is(':checked'))
                {
                    $("#<%=cbReportMMS.ClientID %>").prop('checked', true);
                    $("#<%=cbReportRNO.ClientID %>").prop('checked', true);
                    $("#<%=cbReportMainMMS.ClientID %>").prop('checked', true);
                    $("#<%=cbReportMainRNO.ClientID %>").prop('checked', true);
                }
                else
                {
                    $("#<%=cbReportMMS.ClientID %>").prop('checked', false);
                    $("#<%=cbReportRNO.ClientID %>").prop('checked', false);
                    $("#<%=cbReportMainMMS.ClientID %>").prop('checked', false);
                    $("#<%=cbReportMainRNO.ClientID %>").prop('checked', false);
                }
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
                    Greenwood & Hall Client DRTV Reporting
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
                    <div id="filter_dates" class="filter_section">
                        <h3>Call Dates</h3>
                        <div id="datetime_start">
                            <asp:Label ID="lblDateStart" runat="server" Text="Start Time" CssClass="filter_label1" />
                            <asp:TextBox ID="dtStartDate" runat="server" Width="75px" CssClass="ghDatePickerStart" />
                            <asp:TextBox ID="dtStartTime" runat="server" Width="55px" CssClass="ghTimePickerStart" />
                        </div>
                        <div id="datetime_end">
                            <asp:Label ID="lblDateEnd" runat="server" Text="End Time" CssClass="filter_label1" />
                            <asp:TextBox ID="dtEndDate" runat="server" Width="75px" CssClass="ghDatePickerEnd" />
                            <asp:TextBox ID="dtEndTime" runat="server" Width="55px" CssClass="ghTimePickerEnd" />
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
                            <a href="JavaScript:SelectDateVariable('DayPrevious', '<% Response.Write(dtStartDate.ClientID); %>', '<% Response.Write(dtEndDate.ClientID); %>')" style="color: Blue;"><< Previous Day</a>&nbsp;&nbsp;
                            <a href="JavaScript:SelectDateVariable('DayNext', '<% Response.Write(dtStartDate.ClientID); %>', '<% Response.Write(dtEndDate.ClientID); %>')" style="color: Blue;">Next Day >></a>&nbsp;&nbsp;
                        </div>
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
                            <asp:PostBackTrigger ControlID="btnReportDRTVMMS" />
                            <asp:PostBackTrigger ControlID="btnReportDRTVMMScsv" />
                            <asp:PostBackTrigger ControlID="btnReportDRTVRNO" />
                            <asp:PostBackTrigger ControlID="btnReportDRTVRNOcsv" />
                            <asp:PostBackTrigger ControlID="btnReportDRTVCallHandling" />
                            <asp:PostBackTrigger ControlID="btnReportMainMMS" />
                            <asp:PostBackTrigger ControlID="btnReportMainMMScsv" />
                            <asp:PostBackTrigger ControlID="btnReportMainRNO" />
                            <asp:PostBackTrigger ControlID="btnReportMainRNOcsv" />
                            <asp:PostBackTrigger ControlID="btnReportDRTVMasterFile" />
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
                                        $('#<%= btnRunReport.ClientID  %> ').find("[value=0]").attr("disabled", "disabled");
                                        $('#<%= btnRunReport.ClientID  %> ').find("[value=0]").attr("aria-disabled", "true");
                                    }
                                }
                            </script>
                            <div id="msgOther" runat="server" style="display: inline;width: 100%;color: DarkRed;font-weight: bold;">
                                <asp:Label ID="rpMessage" runat="server" Text="" />
                                <br />
                            </div>
                            <div id="report_controls" style="width: 100%;vertical-align: top;height: 30px;">
                                <div class="report-controls" style="vertical-align: top;height: 30px;">
                                    <asp:Button ID="btnRunReport" runat="server"
                                        Text="Refresh Reports"
                                        OnClientClick="this.disabled = true; this.value = 'Running...';" 
                                        UseSubmitBehavior="false" 
                                        OnClick="Dashboard_Refresh"
                                        />
                                    <div style="display: inline-block;">
                                        Include reports:
                                        <asp:CheckBox ID="cbReportMMS" runat="server" Checked="false" /> MMS
                                        <asp:CheckBox ID="cbReportRNO" runat="server" Checked="false" /> RNO
                                        <asp:CheckBox ID="cbReportMainMMS" runat="server" Checked="false" /> Main Line MMS
                                        <asp:CheckBox ID="cbReportMainRNO" runat="server" Checked="false" /> Main Line RNO
                                        <asp:CheckBox ID="cbReportToggle" runat="server" Checked="false" onclick="toggleIncludeReports(this);" /> Toggle MMS/RNO
                                        <asp:CheckBox ID="cbReportCallHandling" runat="server" Checked="true" /> Call Handling
                                        <asp:CheckBox ID="cbReportMasterFile" runat="server" Checked="false" /> Master File
                                    </div>
                                    <div style="display: inline-block;">
                                        <div style="color: DarkRed;font-weight: bold;">
                                            <asp:Label ID="rpElapsed" runat="server" Text="" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div id="report_drtv_mms" runat="server" visible="true" class="" style="margin-top: 25px;width: 1236px;">
                                <div class="dash_label" style="position: relative;">
                                    DRTV MMS <asp:Label ID="lblReportDRTVMMS" runat="server" Text="" ForeColor="Blue" Font-Bold="true" />
                                    <div style="position: absolute; right: 5px;bottom: 0px;">
                                        <asp:Button ID="btnReportDRTVMMS" runat="server" Text="Export as Excel" OnClick="Dashboard_Data_Export" Visible="false" />
                                        <asp:Button ID="btnReportDRTVMMScsv" runat="server" Text="Export as CSV" OnClick="Dashboard_Data_Export" Visible="false" />
                                    </div>
                                </div>
                                <div class="dash_grid_section" style="">
                                    <asp:UpdatePanel ID="upReportDRTVMMS" runat="server">
                                        <Triggers>
                                        </Triggers>
                                        <ContentTemplate>
                                            <asp:GridView ID="gvReportDRTVMMS"
                                                runat="server"
                                                AutoGenerateColumns="False"
                                                AllowSorting="True"
                                                GridLines="Vertical"
                                                CellPadding="0"
                                                CellSpacing="0"
                                                AllowPaging="false" PageSize="100"
                                                CssClass="dash_grid"
                                                OnRowDataBound="Dashboard_Data_RowDataBound"
                                                >
                                                <AlternatingRowStyle CssClass="dash_grid_alternate" />
                                                <SelectedRowStyle CssClass="dash_grid_selected" />
                                                <Columns>
                                                    <asp:TemplateField HeaderText="Date of Call" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="logindatetime_date" runat="server" Text='<%# ghFunctions.dtFromString_Date(Eval("logindatetime").ToString()) %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Time of Call" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="logindatetime_time" runat="server" Text='<%# ghFunctions.dtFromString_Time(Eval("logindatetime").ToString()) %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:BoundField HeaderText="Phone Number Dialed" DataField="phonenumber" />
                                                    <asp:TemplateField HeaderText="Station Code" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="stationcode" runat="server" Text="" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Product Code" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="productcode" runat="server" Text="ARC" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Creative Code" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="creativecode" runat="server" Text="" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Caller's Zip Code" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="zipcode" runat="server" Text='<%# Eval("zip") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Call Unit" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="callunit" runat="server" Text="1" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="DB Disposition" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="disposition_db" runat="server" Text='<%# Eval("disposition").ToString() %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Disposition" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="disposition" runat="server" Text='<%# report_drtv_disposition(Eval("disposition").ToString(), "dsp", "drtv") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Donation Amount" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="amount" runat="server" Text='<%# String.Format("{0:#}",Eval("amount")) %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Revenue Disposition" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="revenue_disposition" runat="server" Text='<%# report_drtv_disposition(Eval("disposition").ToString(), "rev", "drtv") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:BoundField HeaderText="Transaction ID" DataField="callid" />
                                                    <asp:BoundField HeaderText="Caller Number" DataField="ani" />
                                                </Columns>
                                                <EmptyDataTemplate>
                                                </EmptyDataTemplate>
                                            </asp:GridView>
                                            <asp:GridView ID="gvReportDRTVMMSExport" runat="server" AutoGenerateColumns="False" Visible="false" >
                                                <Columns>
                                                    <asp:TemplateField HeaderText="Date of Call" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="logindatetime_date" runat="server" Text='<%# ghFunctions.dtFromString_Date(Eval("logindatetime").ToString()) %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Time of Call" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="logindatetime_time" runat="server" Text='<%# ghFunctions.dtFromString_Time(Eval("logindatetime").ToString()) %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:BoundField HeaderText="Phone Number Dialed" DataField="phonenumber" />
                                                    <asp:TemplateField HeaderText="Station Code" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="stationcode" runat="server" Text="" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Product Code" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="productcode" runat="server" Text="ARC" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Creative Code" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="creativecode" runat="server" Text="" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Caller's Zip Code" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="zipcode" runat="server" Text='<%# Eval("zip") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Call Unit" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="callunit" runat="server" Text="1" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Disposition" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="disposition" runat="server" Text='<%# report_drtv_disposition(Eval("disposition").ToString(), "dsp", "drtv") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Donation Amount" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="amount" runat="server" Text='<%# String.Format("{0:#}",Eval("amount")) %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Revenue Disposition" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="revenue_disposition" runat="server" Text='<%# report_drtv_disposition(Eval("disposition").ToString(), "rev", "drtv") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:BoundField HeaderText="Transaction ID" DataField="callid" />
                                                </Columns>
                                            </asp:GridView>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </div>
                            </div>
                            <div id="report_main_mms" runat="server" visible="true" class="" style="margin-top: 25px;width: 1236px;">
                                <div class="dash_label" style="position: relative;">
                                    Main MMS <asp:Label ID="lblReportMainMMS" runat="server" Text="" ForeColor="Blue" Font-Bold="true" />
                                    <div style="position: absolute; right: 5px;bottom: 0px;">
                                        <asp:Button ID="btnReportMainMMS" runat="server" Text="Export as Excel" OnClick="Dashboard_Data_Export" Visible="false" />
                                        <asp:Button ID="btnReportMainMMScsv" runat="server" Text="Export as CSV" OnClick="Dashboard_Data_Export" Visible="false" />
                                    </div>
                                </div>
                                <div class="dash_grid_section" style="">
                                    <asp:UpdatePanel ID="upReportMainMMS" runat="server">
                                        <Triggers>
                                        </Triggers>
                                        <ContentTemplate>
                                            <asp:GridView ID="gvReportMainMMS"
                                                runat="server"
                                                AutoGenerateColumns="False"
                                                AllowSorting="True"
                                                GridLines="Vertical"
                                                CellPadding="0"
                                                CellSpacing="0"
                                                AllowPaging="false" PageSize="100"
                                                CssClass="dash_grid"
                                                OnRowDataBound="Dashboard_Data_RowDataBound"
                                                >
                                                <AlternatingRowStyle CssClass="dash_grid_alternate" />
                                                <SelectedRowStyle CssClass="dash_grid_selected" />
                                                <Columns>
                                                    <asp:TemplateField HeaderText="Date of Call" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="logindatetime_date" runat="server" Text='<%# ghFunctions.dtFromString_Date(Eval("logindatetime").ToString()) %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Time of Call" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="logindatetime_time" runat="server" Text='<%# ghFunctions.dtFromString_Time(Eval("logindatetime").ToString()) %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:BoundField HeaderText="Phone Number Dialed" DataField="phonenumber" />
                                                    <asp:TemplateField HeaderText="Station Code" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="stationcode" runat="server" Text="" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Product Code" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="productcode" runat="server" Text="ARC" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Creative Code" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="creativecode" runat="server" Text="" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Caller's Zip Code" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="zipcode" runat="server" Text='<%# Eval("zip") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Call Unit" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="callunit" runat="server" Text="1" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="DB Disposition" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="disposition_db" runat="server" Text='<%# Eval("disposition").ToString() %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Disposition" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="disposition" runat="server" Text='<%# report_drtv_disposition(Eval("disposition").ToString(), "dsp", "main") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Donation Amount" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="amount" runat="server" Text='<%# String.Format("{0:#}",Eval("amount")) %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Revenue Disposition" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="revenue_disposition" runat="server" Text='<%# report_drtv_disposition(Eval("disposition").ToString(), "rev", "main") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:BoundField HeaderText="Transaction ID" DataField="callid" />
                                                    <asp:BoundField HeaderText="Caller Number" DataField="ani" />
                                                </Columns>
                                                <EmptyDataTemplate>
                                                </EmptyDataTemplate>
                                            </asp:GridView>
                                            <asp:GridView ID="gvReportMainMMSExport" runat="server" AutoGenerateColumns="False" Visible="false" >
                                                <Columns>
                                                    <asp:TemplateField HeaderText="Date of Call" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="logindatetime_date" runat="server" Text='<%# ghFunctions.dtFromString_Date(Eval("logindatetime").ToString()) %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Time of Call" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="logindatetime_time" runat="server" Text='<%# ghFunctions.dtFromString_Time(Eval("logindatetime").ToString()) %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:BoundField HeaderText="Phone Number Dialed" DataField="phonenumber" />
                                                    <asp:TemplateField HeaderText="Station Code" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="stationcode" runat="server" Text="" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Product Code" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="productcode" runat="server" Text="ARC" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Creative Code" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="creativecode" runat="server" Text="" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Caller's Zip Code" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="zipcode" runat="server" Text='<%# Eval("zip") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Call Unit" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="callunit" runat="server" Text="1" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Disposition" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="disposition" runat="server" Text='<%# report_drtv_disposition(Eval("disposition").ToString(), "dsp", "main") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Donation Amount" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="amount" runat="server" Text='<%# String.Format("{0:#}",Eval("amount")) %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Revenue Disposition" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="revenue_disposition" runat="server" Text='<%# report_drtv_disposition(Eval("disposition").ToString(), "rev", "main") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:BoundField HeaderText="Transaction ID" DataField="callid" />
                                                </Columns>
                                            </asp:GridView>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </div>
                            </div>
                            <div id="report_drtv_rno" runat="server" visible="true" class="" style="margin-top: 25px;width: 1236px;">
                                <div class="dash_label" style="position: relative;">
                                    DRTV RNO <asp:Label ID="lblReportDRTVRNO" runat="server" Text="" ForeColor="Blue" Font-Bold="true" />
                                    <div style="position: absolute; right: 5px;bottom: 0px;">
                                        <asp:Button ID="btnReportDRTVRNO" runat="server" Text="Export as Excel" OnClick="Dashboard_Data_Export" Visible="false" />
                                        <asp:Button ID="btnReportDRTVRNOcsv" runat="server" Text="Export as CSV" OnClick="Dashboard_Data_Export" Visible="false" />
                                    </div>
                                </div>
                                <div class="dash_grid_section" style="">
                                    <asp:UpdatePanel ID="upReportDRTVRNO" runat="server">
                                        <Triggers>
                                        </Triggers>
                                        <ContentTemplate>
                                            <asp:GridView ID="gvReportDRTVRNO"
                                                runat="server"
                                                AutoGenerateColumns="False"
                                                AllowSorting="True"
                                                GridLines="Vertical"
                                                CellPadding="0"
                                                CellSpacing="0"
                                                AllowPaging="false" PageSize="100"
                                                CssClass="dash_grid"
                                                OnRowDataBound="Dashboard_Data_RowDataBound"
                                                >
                                                <AlternatingRowStyle CssClass="dash_grid_alternate" />
                                                <SelectedRowStyle CssClass="dash_grid_selected" />
                                                <Columns>
                                                    <asp:TemplateField HeaderText="Date of Call" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="logindatetime_date" runat="server" Text='<%# ghFunctions.dtFromString_Date(Eval("logindatetime").ToString()) %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Time of Call" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="logindatetime_time" runat="server" Text='<%# ghFunctions.dtFromString_Time(Eval("logindatetime").ToString()) %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:BoundField HeaderText="Phone Number Dialed" DataField="phonenumber" />
                                                    <asp:TemplateField HeaderText="Station Code" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="stationcode" runat="server" Text="" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Product Code" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="productcode" runat="server" Text="ARC" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Creative Code" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="creativecode" runat="server" Text="" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Caller's Zip Code" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="zipcode" runat="server" Text='<%# Eval("zip") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Call Unit" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="callunit" runat="server" Text="1" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="DB Disposition" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="disposition_db" runat="server" Text='<%# Eval("disposition").ToString() %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Disposition" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="disposition" runat="server" Text='<%# report_drtv_disposition(Eval("disposition").ToString(), "dsp", "drtv") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Donation Amount" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="amount" runat="server" Text='<%# String.Format("{0:#}",Eval("amount")) %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Revenue Disposition" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="revenue_disposition" runat="server" Text='<%# report_drtv_disposition(Eval("disposition").ToString(), "rev", "drtv") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:BoundField HeaderText="Transaction ID" DataField="callid" />
                                                    <asp:BoundField HeaderText="Caller Number" DataField="ani" />
                                                </Columns>
                                                <EmptyDataTemplate>
                                                </EmptyDataTemplate>
                                            </asp:GridView>
                                            <asp:GridView ID="gvReportDRTVRNOExport" runat="server" AutoGenerateColumns="False" Visible="false" >
                                                <Columns>
                                                    <asp:TemplateField HeaderText="Date of Call" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="logindatetime_date" runat="server" Text='<%# ghFunctions.dtFromString_Date(Eval("logindatetime").ToString()) %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Time of Call" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="logindatetime_time" runat="server" Text='<%# ghFunctions.dtFromString_Time(Eval("logindatetime").ToString()) %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:BoundField HeaderText="Phone Number Dialed" DataField="phonenumber" />
                                                    <asp:TemplateField HeaderText="Station Code" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="stationcode" runat="server" Text="" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Product Code" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="productcode" runat="server" Text="ARC" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Creative Code" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="creativecode" runat="server" Text="" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Caller's Zip Code" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="zipcode" runat="server" Text='<%# Eval("zip") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Call Unit" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="callunit" runat="server" Text="1" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Disposition" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="disposition" runat="server" Text='<%# report_drtv_disposition(Eval("disposition").ToString(), "dsp", "drtv") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Donation Amount" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="amount" runat="server" Text='<%# String.Format("{0:#}",Eval("amount")) %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Revenue Disposition" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="revenue_disposition" runat="server" Text='<%# report_drtv_disposition(Eval("disposition").ToString(), "rev", "drtv") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:BoundField HeaderText="Transaction ID" DataField="callid" />
                                                </Columns>
                                            </asp:GridView>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </div>
                            </div>
                            <div id="report_main_rno" runat="server" visible="true" class="" style="margin-top: 25px;width: 1236px;">
                                <div class="dash_label" style="position: relative;">
                                    Main RNO <asp:Label ID="lblReportMainRNO" runat="server" Text="" ForeColor="Blue" Font-Bold="true" />
                                    <div style="position: absolute; right: 5px;bottom: 0px;">
                                        <asp:Button ID="btnReportMainRNO" runat="server" Text="Export as Excel" OnClick="Dashboard_Data_Export" Visible="false" />
                                        <asp:Button ID="btnReportMainRNOcsv" runat="server" Text="Export as CSV" OnClick="Dashboard_Data_Export" Visible="false" />
                                    </div>
                                </div>
                                <div class="dash_grid_section" style="">
                                    <asp:UpdatePanel ID="upReportMainRNO" runat="server">
                                        <Triggers>
                                        </Triggers>
                                        <ContentTemplate>
                                            <asp:GridView ID="gvReportMainRNO"
                                                runat="server"
                                                AutoGenerateColumns="False"
                                                AllowSorting="True"
                                                GridLines="Vertical"
                                                CellPadding="0"
                                                CellSpacing="0"
                                                AllowPaging="false" PageSize="100"
                                                CssClass="dash_grid"
                                                OnRowDataBound="Dashboard_Data_RowDataBound"
                                                >
                                                <AlternatingRowStyle CssClass="dash_grid_alternate" />
                                                <SelectedRowStyle CssClass="dash_grid_selected" />
                                                <Columns>
                                                    <asp:TemplateField HeaderText="Date of Call" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="logindatetime_date" runat="server" Text='<%# ghFunctions.dtFromString_Date(Eval("logindatetime").ToString()) %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Time of Call" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="logindatetime_time" runat="server" Text='<%# ghFunctions.dtFromString_Time(Eval("logindatetime").ToString()) %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:BoundField HeaderText="Phone Number Dialed" DataField="phonenumber" />
                                                    <asp:TemplateField HeaderText="Station Code" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="stationcode" runat="server" Text="" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Product Code" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="productcode" runat="server" Text="ARC" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Creative Code" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="creativecode" runat="server" Text="" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Caller's Zip Code" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="zipcode" runat="server" Text='<%# Eval("zip") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Call Unit" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="callunit" runat="server" Text="1" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="DB Disposition" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="disposition_db" runat="server" Text='<%# Eval("disposition").ToString() %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Disposition" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="disposition" runat="server" Text='<%# report_drtv_disposition(Eval("disposition").ToString(), "dsp", "main") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Donation Amount" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="amount" runat="server" Text='<%# String.Format("{0:#}",Eval("amount")) %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Revenue Disposition" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="revenue_disposition" runat="server" Text='<%# report_drtv_disposition(Eval("disposition").ToString(), "rev", "main") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:BoundField HeaderText="Transaction ID" DataField="callid" />
                                                    <asp:BoundField HeaderText="Caller Number" DataField="ani" />
                                                </Columns>
                                                <EmptyDataTemplate>
                                                </EmptyDataTemplate>
                                            </asp:GridView>
                                            <asp:GridView ID="gvReportMainRNOExport" runat="server" AutoGenerateColumns="False" Visible="false" >
                                                <Columns>
                                                    <asp:TemplateField HeaderText="Date of Call" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="logindatetime_date" runat="server" Text='<%# ghFunctions.dtFromString_Date(Eval("logindatetime").ToString()) %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Time of Call" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="logindatetime_time" runat="server" Text='<%# ghFunctions.dtFromString_Time(Eval("logindatetime").ToString()) %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:BoundField HeaderText="Phone Number Dialed" DataField="phonenumber" />
                                                    <asp:TemplateField HeaderText="Station Code" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="stationcode" runat="server" Text="" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Product Code" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="productcode" runat="server" Text="ARC" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Creative Code" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="creativecode" runat="server" Text="" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Caller's Zip Code" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="zipcode" runat="server" Text='<%# Eval("zip") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Call Unit" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="callunit" runat="server" Text="1" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Disposition" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="disposition" runat="server" Text='<%# report_drtv_disposition(Eval("disposition").ToString(), "dsp", "main") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Donation Amount" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="amount" runat="server" Text='<%# String.Format("{0:#}",Eval("amount")) %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Revenue Disposition" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="revenue_disposition" runat="server" Text='<%# report_drtv_disposition(Eval("disposition").ToString(), "rev", "main") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:BoundField HeaderText="Transaction ID" DataField="callid" />
                                                </Columns>
                                            </asp:GridView>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </div>
                            </div>
                            <div id="report_drtv_callhandling" runat="server" visible="true" class="" style="margin-top: 25px;width: 1236px;">
                                <div class="dash_label" style="position: relative;">
                                    DRTV Call Handling <asp:Label ID="lblReportDRTVCallHandling" runat="server" Text="" ForeColor="Blue" Font-Bold="true" />
                                    <div style="position: absolute; right: 5px;bottom: 0px;">
                                        <asp:Button ID="btnReportDRTVCallHandling" runat="server" Text="Export as Excel" OnClick="Dashboard_Data_Export" Visible="false" />
                                    </div>
                                </div>
                                <div class="dash_grid_section" style="">
                                    <asp:UpdatePanel ID="upReportDRTVCallHandling" runat="server">
                                        <Triggers>
                                        </Triggers>
                                        <ContentTemplate>
                                            <asp:GridView ID="gvReportDRTVCallHandlingCalls" runat="server" AutoGenerateColumns="False" AllowSorting="True" GridLines="Vertical" CellPadding="0" CellSpacing="0" AllowPaging="false" PageSize="100" CssClass="dash_grid" OnRowDataBound="Dashboard_Data_RowDataBound" ShowFooter="true">
                                                <AlternatingRowStyle CssClass="dash_grid_alternate" />
                                                <SelectedRowStyle CssClass="dash_grid_selected" />
                                                <Columns>
                                                    <asp:TemplateField HeaderText="Date" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="date" runat="server" Text='<%# ghFunctions.dtFromString_Custom(Eval("date").ToString(), "yyyy/MM/dd", false) %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:BoundField HeaderText="IVR Calls" DataField="ivr_calls" />
                                                    <asp:BoundField HeaderText="Calls" DataField="calls" />
                                                    <asp:BoundField HeaderText="Contacted" DataField="contacted" />
                                                    <asp:BoundField HeaderText="Abandoned" DataField="abandoned" />
                                                    <asp:TemplateField HeaderText="Average Speed of Answer" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="answer_avg" runat="server" Text='<%# ghFunctions.SecondsTo(Eval("answer_avg").ToString()) %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Average Talk Time" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="talk_avg" runat="server" Text='<%# ghFunctions.SecondsTo(Eval("talk_avg").ToString()) %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Average IVR Duration" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="ivr_duration_avg" runat="server" Text='<%# ghFunctions.SecondsTo(Eval("ivr_duration_avg").ToString()) %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                                <EmptyDataTemplate>
                                                </EmptyDataTemplate>
                                            </asp:GridView>
                                            <asp:GridView ID="gvReportDRTVCallHandlingCallsExport" runat="server" AutoGenerateColumns="False" Visible="false" ShowFooter="true" OnRowDataBound="Dashboard_Data_RowDataBound" >
                                                <Columns>
                                                    <asp:TemplateField HeaderText="Date" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="date" runat="server" Text='<%# ghFunctions.dtFromString_Custom(Eval("date").ToString(), "yyyy/MM/dd", false) %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:BoundField HeaderText="IVR Calls" DataField="ivr_calls" />
                                                    <asp:BoundField HeaderText="Calls" DataField="calls" />
                                                    <asp:BoundField HeaderText="Contacted" DataField="contacted" />
                                                    <asp:BoundField HeaderText="Abandoned" DataField="abandoned" />
                                                    <asp:TemplateField HeaderText="Average Speed of Answer" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="answer_avg" runat="server" Text='<%# ghFunctions.SecondsTo(Eval("answer_avg").ToString()) %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Average Talk Time" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="talk_avg" runat="server" Text='<%# ghFunctions.SecondsTo(Eval("talk_avg").ToString()) %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Average IVR Duration" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="ivr_duration_avg" runat="server" Text='<%# ghFunctions.SecondsTo(Eval("ivr_duration_avg").ToString()) %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                            </asp:GridView>
                                            <br />
                                            <asp:GridView ID="gvReportDRTVCallHandlingDon" runat="server" AutoGenerateColumns="False" AllowSorting="True" GridLines="Vertical" CellPadding="0" CellSpacing="0" AllowPaging="false" PageSize="100" CssClass="dash_grid" OnRowDataBound="Dashboard_Data_RowDataBound" ShowFooter="true">
                                                <AlternatingRowStyle CssClass="dash_grid_alternate" />
                                                <SelectedRowStyle CssClass="dash_grid_selected" />
                                                <Columns>
                                                    <asp:TemplateField HeaderText="Date" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="date" runat="server" Text='<%# ghFunctions.dtFromString_Custom(Eval("date").ToString(), "yyyy/MM/dd", false) %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:BoundField HeaderText="Calls" DataField="calls" />
                                                    <asp:BoundField HeaderText="Donation Calls" DataField="calls_donation" />
                                                    <asp:TemplateField HeaderText="Conversion Rate" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="conversion_donations" runat="server" Text='<%# report_drtv_callhandling_conversion(Eval("calls").ToString(), Eval("calls_donation").ToString()) %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Non-Donation Calls" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="conversion_donations" runat="server" Text='<%# report_drtv_callhandling_nondoncalls(Eval("calls").ToString(), Eval("calls_donation").ToString()) %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:BoundField HeaderText="Credit Card Donations" DataField="donations_cc" />
                                                    <asp:TemplateField HeaderText="CC Conversion Rate" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="conversion_donations" runat="server" Text='<%# report_drtv_callhandling_conversion(Eval("calls_donation").ToString(), Eval("donations_cc").ToString()) %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Average Donation" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="amount_donation_avg" runat="server" Text='<%# String.Format("{0:#0}",Eval("donations_avg")) %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:BoundField HeaderText="Pledge" DataField="pledge" />
                                                    <asp:TemplateField HeaderText="Average Pledge" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="amount_pledge_avg" runat="server" Text='<%# String.Format("{0:#0}",Eval("pledge_avg")) %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                                <EmptyDataTemplate>
                                                </EmptyDataTemplate>
                                            </asp:GridView>
                                            <asp:GridView ID="gvReportDRTVCallHandlingDonExport" runat="server" AutoGenerateColumns="False" Visible="false" ShowFooter="true" OnRowDataBound="Dashboard_Data_RowDataBound" >
                                                <Columns>
                                                    <asp:TemplateField HeaderText="Date" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="date" runat="server" Text='<%# ghFunctions.dtFromString_Custom(Eval("date").ToString(), "yyyy/MM/dd", false) %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:BoundField HeaderText="Calls" DataField="calls" />
                                                    <asp:BoundField HeaderText="Donation Calls" DataField="calls_donation" />
                                                    <asp:TemplateField HeaderText="Conversion Rate" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="conversion_donations" runat="server" Text='<%# report_drtv_callhandling_conversion(Eval("calls").ToString(), Eval("calls_donation").ToString()) %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Non-Donation Calls" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="conversion_donations" runat="server" Text='<%# report_drtv_callhandling_nondoncalls(Eval("calls").ToString(), Eval("calls_donation").ToString()) %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:BoundField HeaderText="Credit Card Donations" DataField="donations_cc" />
                                                    <asp:TemplateField HeaderText="CC Conversion Rate" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="conversion_donations" runat="server" Text='<%# report_drtv_callhandling_conversion(Eval("calls_donation").ToString(), Eval("donations_cc").ToString()) %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Average Donation" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="amount_donation_avg" runat="server" Text='<%# String.Format("{0:#0}",Eval("donations_avg")) %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:BoundField HeaderText="Pledge" DataField="pledge" />
                                                    <asp:TemplateField HeaderText="Average Pledge" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="amount_pledge_avg" runat="server" Text='<%# String.Format("{0:#0}",Eval("pledge_avg")) %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                            </asp:GridView>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </div>
                            </div>
                            <div id="report_drtv_masterfile" runat="server" visible="true" class="" style="margin-top: 25px;width: 1236px;">
                                <div class="dash_label" style="position: relative;">
                                    DRTV Master File <asp:Label ID="lblReportDRTVMasterFile" runat="server" Text="" ForeColor="Blue" Font-Bold="true" />
                                    <div style="position: absolute; right: 5px;bottom: 0px;">
                                        <asp:Button ID="btnReportDRTVMasterFile" runat="server" Text="Export as Excel" OnClick="Dashboard_Data_Export" Visible="false" />
                                    </div>
                                </div>
                                <div class="dash_grid_section" style="">
                                    <asp:UpdatePanel ID="upReportDRTVMasterFile" runat="server">
                                        <Triggers>
                                        </Triggers>
                                        <ContentTemplate>
                                            <asp:GridView ID="gvReportDRTVMasterFile"
                                                runat="server"
                                                AutoGenerateColumns="False"
                                                AllowSorting="True"
                                                GridLines="Vertical"
                                                CellPadding="0"
                                                CellSpacing="0"
                                                AllowPaging="false" PageSize="100"
                                                CssClass="dash_grid"
                                                OnRowDataBound="Dashboard_Data_RowDataBound"
                                                ShowFooter="true"
                                                >
                                                <AlternatingRowStyle CssClass="dash_grid_alternate" />
                                                <SelectedRowStyle CssClass="dash_grid_selected" />
                                                <Columns>
                                                    <asp:TemplateField HeaderText="Date of Call" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="logindatetime_date" runat="server" Text='<%# ghFunctions.dtFromString_Date(Eval("logindatetime").ToString()) %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Time of Call" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="logindatetime_time" runat="server" Text='<%# ghFunctions.dtFromString_Time(Eval("logindatetime").ToString()) %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:BoundField HeaderText="Phone Number Dialed" DataField="dnis" />
                                                    <asp:TemplateField HeaderText="Network" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="Network" runat="server" Text='' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:BoundField HeaderText="Agent Time" DataField="duration" />
                                                    <asp:BoundField HeaderText="Call ID" DataField="callid" />
                                                    <asp:BoundField HeaderText="Transaction ID" DataField="donationid" />
                                                    <asp:BoundField HeaderText="Title" DataField="title" />
                                                    <asp:BoundField HeaderText="First Name" DataField="fname" />
                                                    <asp:BoundField HeaderText="Last Name" DataField="lname" />
                                                    <asp:BoundField HeaderText="Address Line 1" DataField="address" />
                                                    <asp:BoundField HeaderText="Address Line 2" DataField="suitenumber" />
                                                    <asp:BoundField HeaderText="City" DataField="city" />
                                                    <asp:BoundField HeaderText="State" DataField="state" />
                                                    <asp:BoundField HeaderText="Zip Code" DataField="zip" />
                                                    <asp:BoundField HeaderText="Country" DataField="country" />
                                                    <asp:BoundField HeaderText="Telephone" DataField="phonenumber" />
                                                    <asp:BoundField HeaderText="Cell Phone" DataField="phone2" />
                                                    <asp:BoundField HeaderText="Disposition" DataField="disposition" />
                                                    <asp:BoundField HeaderText="Payment Method" DataField="payment" />
                                                    <asp:BoundField HeaderText="Gift Amount" DataField="donationamount" />
                                                    <asp:BoundField HeaderText="CC Type" DataField="cctype" />
                                                    <asp:BoundField HeaderText="Email Address" DataField="email" />
                                                    <asp:BoundField HeaderText="Do Not Mail" DataField="receiveupdatesyn" />
                                                    <asp:BoundField HeaderText="Do Not Email" DataField="receipt_email" />
                                                </Columns>
                                                <EmptyDataTemplate>
                                                </EmptyDataTemplate>
                                            </asp:GridView>
                                            <asp:GridView ID="gvReportDRTVMasterFileExport" runat="server" AutoGenerateColumns="False" Visible="false" ShowFooter="true" OnRowDataBound="Dashboard_Data_RowDataBound" >
                                                <Columns>
                                                    <asp:TemplateField HeaderText="Date of Call" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="logindatetime_date" runat="server" Text='<%# ghFunctions.dtFromString_Date(Eval("logindatetime").ToString()) %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Time of Call" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="logindatetime_time" runat="server" Text='<%# ghFunctions.dtFromString_Time(Eval("logindatetime").ToString()) %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:BoundField HeaderText="Phone Number Dialed" DataField="dnis" />
                                                    <asp:TemplateField HeaderText="Network" HeaderStyle-CssClass="center" ItemStyle-CssClass="center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="Network" runat="server" Text='' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:BoundField HeaderText="Agent Time" DataField="duration" />
                                                    <asp:BoundField HeaderText="Call ID" DataField="callid" />
                                                    <asp:BoundField HeaderText="Transaction ID" DataField="donationid" />
                                                    <asp:BoundField HeaderText="Title" DataField="title" />
                                                    <asp:BoundField HeaderText="First Name" DataField="fname" />
                                                    <asp:BoundField HeaderText="Last Name" DataField="lname" />
                                                    <asp:BoundField HeaderText="Address Line 1" DataField="address" />
                                                    <asp:BoundField HeaderText="Address Line 2" DataField="suitenumber" />
                                                    <asp:BoundField HeaderText="City" DataField="city" />
                                                    <asp:BoundField HeaderText="State" DataField="state" />
                                                    <asp:BoundField HeaderText="Zip Code" DataField="zip" />
                                                    <asp:BoundField HeaderText="Country" DataField="country" />
                                                    <asp:BoundField HeaderText="Telephone" DataField="phonenumber" />
                                                    <asp:BoundField HeaderText="Cell Phone" DataField="phone2" />
                                                    <asp:BoundField HeaderText="Disposition" DataField="disposition" />
                                                    <asp:BoundField HeaderText="Payment Method" DataField="payment" />
                                                    <asp:BoundField HeaderText="Gift Amount" DataField="donationamount" />
                                                    <asp:BoundField HeaderText="CC Type" DataField="cctype" />
                                                    <asp:BoundField HeaderText="Email Address" DataField="email" />
                                                    <asp:BoundField HeaderText="Do Not Mail" DataField="receiveupdatesyn" />
                                                    <asp:BoundField HeaderText="Do Not Email" DataField="receipt_email" />
                                                </Columns>
                                            </asp:GridView>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </div>
                            </div>

                            <div id="hidden_fields">
                                <asp:HiddenField ID="cntSupportLevel" runat="server" />
                                <asp:HiddenField ID="hfDispoCount" runat="server" />
                                <asp:HiddenField ID="hfCampaignCount" runat="server" />                                
                                <div style="margin: 5px;">
                                    <asp:Label ID="msgResults" runat="server" Text="" />
                                    <asp:Label ID="msgDebug" runat="server" Text="" />
                                </div>
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