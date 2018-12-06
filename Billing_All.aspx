<%@ Page Title="Billing" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Billing_All.aspx.cs" Inherits="Billing_All" %>
<%@ MasterType TypeName="MasterPage" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" Runat="Server">
    <style type="text/css">
    /* css for jQuery UI */
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
    </style>
    <style type="text/css">
    .star
    {
    	margin-left: 0px;
    	display: inline-block;
    }
    .star_all
    {
    }
    .star_on
    {
    	display: inline-block;
    	margin: -5px -1px 0px -1px;
    	width: 16px;
    	height: 16px;
    	background-image: url(images/star-on.png);
    }
    .star_off
    {
    	display: inline-block;
    	margin: -5px -2px 0px -2px;
    	width: 16px;
    	height: 16px;
    	background-image: url(images/star-off.png);
    }
    .priority
    {
    	display: inline-block;
    }
    .priority_on
    {
    	margin: 0px -1px;
    }
    .priority_off
    {
    	margin: 0px -2px;
    }
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
    .reporting_filters_note h3
    {
    	background-color: #80A2D0;
    	width: 350px;
    	text-align: center;
    	margin-bottom: 10px;
    }
    .filter_section_note
    {
    	float: left;
    	width: 350px;
    	margin-bottom: 5px;
    	border: solid 1px white;
    	margin-right: 5px;
    }
    .message_high
    {
    	color: Red;
    	font-weight: bold;
    }

    </style>

    <link href="../css/content_general.css" rel="stylesheet" type="text/css" />
    <link href="../css/portal_standard.css" rel="stylesheet" type="text/css" />

    <link href="../css/start/jquery-ui-1.8.18.custom.css" rel="stylesheet" type="text/css" />
    <link href="../css/start/jquery.ui.selectmenu.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery-1.7.1.min.js" type="text/javascript"></script>
    <script src="../js/jquery-ui-1.8.18.custom.min.js" type="text/javascript"></script>
    <script src="../js/jquery-ui-timepicker-addon.js" type="text/javascript"></script>
    <script src="../js/jquery.ui.selectmenu.js" type="text/javascript"></script>

    <link href="../js/jquery.countdown.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery.countdown.pack.js" type="text/javascript"></script>

    <script src="../js/jquery-gh-custom-objects.js" type="text/javascript"></script>


    <script type="text/javascript">
        function Portal_OnLoad() {
            $(document).ready(function() {
                GH_Buttons();
                GH_DropDown();
                GH_DatePicker();
            });
        }
        function Filter_Toggle(btn, field, tgl) {
            // Hide/Show if an option is selected
            // Toggle if no option is selected
            // btn the requester
            // field the div to hide
            // tgl if present, we hide or show based on that 
            $(document).ready(function() {
                //alert($(btn).val().substring(5, $(btn).val().length));
                type = $(btn).val();
                var nme = $(btn).val().substring(5, $(btn).val().length);
                if (tgl == "Hide") { $("#" + field).hide(); $(btn).val("Show " + nme); }
                else if (tgl == "Show") { $("#" + field).show(); $(btn).val("Hide " + nme); }
                else if (type.substring(0, 4) == "Hide") { $("#" + field).hide(); $(btn).val("Show " + nme); }
                else { $("#" + field).show(); $(btn).val("Hide " + nme); }
            });
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Master_Content" Runat="Server">
    <script type="text/javascript">
        Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(BeginRequestHandler);
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(EndRequestHandler);
        function BeginRequestHandler(sender, args) { CountDownStart(); }
        function EndRequestHandler(sender, args) { CountDownStop(); }
        function ActivateAlertDiv(visstring, elem, msg) {
            var adiv = $get(elem);
            adiv.style.visibility = visstring;
            adiv.innerHTML = msg;
        }
        document.body.onload = function() { Portal_OnLoad(); }
    </script>
    <div id="default_container">
        <div id="report_header">
            <div id="msgTitle" style="display: inline;width: 50%;color: DarkRed;font-weight: bold;text-align: left;">
                Greenwood & Hall Billing Report
            </div>            
            <div id="msgTitleRight" style="float: right;width: 50%;color: DarkOrange;font-weight: bold;text-align: right;">
                <asp:Label ID="rpTimeZone" runat="server" Text="Times are UTC Minus the Offset: " />
            </div>
            <hr />
        </div>
        <div id="report_control1">
            <input id="btnFilters01" type="button" value="Hide All Filters" onclick="Filter_Toggle(this,'reporting_filters','');" />
        </div>
        <div id="reporting_filters" class="reporting_filters">
            <div class="clearfix">
                <div id="filter_dates" class="filter_section">
                    <h3>Billing Dates</h3>
                    <div id="datetime_start">
                        <asp:Label ID="Label3" runat="server" Text="Start Time" CssClass="filter_label1" />
                        <asp:TextBox ID="dtStartDate" runat="server" Width="75px" CssClass="ghDatePickerStart" />
                        <asp:TextBox ID="dtStartTime" runat="server" Width="50px" CssClass="ghTimePickerStart" />
                    </div>
                    <div id="datetime_end">
                        <asp:Label ID="Label4" runat="server" Text="End Time" CssClass="filter_label1" />
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
                    </div>
                </div>
            </div>
        </div>
        <div id="report_control2" style="float: right;">
            <input id="btnFilters02" type="button" value="Hide Report - Segments" onclick="Filter_Toggle(this,'capella_dashboard_segments','');" />
        </div>
        <div id="capella_dashboard_segments">
            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>
                    <script type="text/javascript">
                        Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(function(evt, args) { Portal_OnLoad(); });
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
                    <div>
                        <asp:Button ID="Button2" runat="server"
                            Text="Run Report"
                            OnClientClick="this.disabled = true; this.value = 'Running...';" 
                            UseSubmitBehavior="false" 
                            OnClick="GridView_Refresh"
                            />
                        <div style="display: inline-block;">
                            <asp:Label ID="rpElapsed" runat="server" Text="" />
                        </div>
                    </div>
                    <div>
                        Report Date Range: <asp:Label ID="lblDateRange" runat="server" Text="-- none --" ForeColor="Blue" Font-Bold="true" />
                    </div>
                    <style type="text/css">
                        .oracle_section
                        {
            	            margin-bottom: 10px;
            	            clear: both;
                        }
                        .oracle_section:after
                        {
                            content: ".";
                            display: block;
                            height: 0;
                            clear: both;
                            visibility: hidden;
                        }
                        #report_list
                        {
                        	float: left;
                        	width: 525px;
                        }
                        #report_details
                        {
                        	float: left;
                        	margin-left: 10px;
                        	width: 675px;
                        }
                    </style>
                    <div class="oracle_section">
                        <div id="report_list">
                            <asp:GridView ID="gvBillingARC_Dedicated" runat="server" AutoGenerateColumns="false"
                                BackColor="White"
                                AllowSorting="True"
                                ForeColor="Black"
                                GridLines="Vertical"
                                AllowPaging="True"
                                PageSize="50"

                                OnRowDataBound="GridView_RowDataBound"
                                OnSelectedIndexChanged="GridView_IndexChanged"
                                OnPageIndexChanging="GridView_PageIndexChanging"
                                ShowFooter="true"
                                Width="525"
                                
                                CssClass="Portal_GridView_Standard">
                                <AlternatingRowStyle CssClass="Portal_GridView_Standard_Alternate" />
                                <SelectedRowStyle CssClass="Portal_GridView_Standard_Selected" />
                                <Columns>
                                    <asp:BoundField ItemStyle-Width="100" HeaderText="Type" DataField="type" />
                                    <asp:BoundField ItemStyle-Width="90" HeaderText="Calls" DataField="calls" DataFormatString="{0:N}" ItemStyle-HorizontalAlign="Right" FooterStyle-HorizontalAlign="Right" />
                                    <asp:BoundField ItemStyle-Width="102" HeaderText="Minutes" DataField="time_minutes" DataFormatString="{0:N}" ItemStyle-HorizontalAlign="Right" FooterStyle-HorizontalAlign="Right" />
                                </Columns>
                                <PagerSettings Position="Top" />
                                <EmptyDataTemplate>
                                    There are no records to display.
                                </EmptyDataTemplate>
                            </asp:GridView>
                            <br /><br />
                            <asp:GridView ID="gvBillingARC_Standard" runat="server" AutoGenerateColumns="false"
                                BackColor="White"
                                AllowSorting="True"
                                ForeColor="Black"
                                GridLines="Vertical"
                                AllowPaging="True"
                                PageSize="50"

                                OnRowDataBound="GridView_RowDataBound"
                                OnSelectedIndexChanged="GridView_IndexChanged"
                                OnPageIndexChanging="GridView_PageIndexChanging"
                                ShowFooter="true"
                                Width="525"
                                
                                CssClass="Portal_GridView_Standard">
                                <AlternatingRowStyle CssClass="Portal_GridView_Standard_Alternate" />
                                <SelectedRowStyle CssClass="Portal_GridView_Standard_Selected" />
                                <Columns>
                                    <asp:BoundField ItemStyle-Width="100" HeaderText="Type" DataField="type" />
                                    <asp:BoundField ItemStyle-Width="90" HeaderText="Calls" DataField="calls" DataFormatString="{0:N}" ItemStyle-HorizontalAlign="Right" FooterStyle-HorizontalAlign="Right" />
                                    <asp:BoundField ItemStyle-Width="102" HeaderText="Minutes" DataField="time_minutes" DataFormatString="{0:N}" ItemStyle-HorizontalAlign="Right" FooterStyle-HorizontalAlign="Right" />
                                </Columns>

                                <PagerSettings Position="Top" />
                                <EmptyDataTemplate>
                                    There are no records to display.
                                </EmptyDataTemplate>
                            </asp:GridView>
                            <br /><br />
                            <asp:GridView ID="gvBillingARC_Total" runat="server" AutoGenerateColumns="false"
                                BackColor="White"
                                AllowSorting="True"
                                ForeColor="Black"
                                GridLines="Vertical"
                                AllowPaging="True"
                                PageSize="50"

                                OnRowDataBound="GridView_RowDataBound"
                                OnSelectedIndexChanged="GridView_IndexChanged"
                                OnPageIndexChanging="GridView_PageIndexChanging"
                                ShowFooter="true"
                                Width="525"
                                
                                CssClass="Portal_GridView_Standard">
                                <AlternatingRowStyle CssClass="Portal_GridView_Standard_Alternate" />
                                <SelectedRowStyle CssClass="Portal_GridView_Standard_Selected" />
                                <Columns>
                                    <asp:BoundField ItemStyle-Width="100" HeaderText="Type" DataField="type" />
                                    <asp:BoundField ItemStyle-Width="90" HeaderText="Calls" DataField="calls" DataFormatString="{0:N}" ItemStyle-HorizontalAlign="Right" FooterStyle-HorizontalAlign="Right" />
                                    <asp:BoundField ItemStyle-Width="102" HeaderText="Minutes" DataField="time_minutes" DataFormatString="{0:N}" ItemStyle-HorizontalAlign="Right" FooterStyle-HorizontalAlign="Right" />
                                </Columns>
                                <PagerSettings Position="Top" />
                                <EmptyDataTemplate>
                                    There are no records to display.
                                </EmptyDataTemplate>
                            </asp:GridView>
                            <br /><br />
                            <asp:GridView ID="gvBillingARC_Transactions" runat="server" AutoGenerateColumns="false"
                                BackColor="White"
                                AllowSorting="True"
                                ForeColor="Black"
                                GridLines="Vertical"
                                AllowPaging="True"
                                PageSize="50"

                                OnRowDataBound="GridView_RowDataBound"
                                OnSelectedIndexChanged="GridView_IndexChanged"
                                OnPageIndexChanging="GridView_PageIndexChanging"
                                ShowFooter="true"
                                Width="525"
                                
                                CssClass="Portal_GridView_Standard">
                                <AlternatingRowStyle CssClass="Portal_GridView_Standard_Alternate" />
                                <SelectedRowStyle CssClass="Portal_GridView_Standard_Selected" />
                                <Columns>
                                    <asp:BoundField ItemStyle-Width="100" HeaderText="Type" DataField="type" />
                                    <asp:BoundField ItemStyle-Width="90" HeaderText="Count" DataField="count" DataFormatString="{0:N}" ItemStyle-HorizontalAlign="Right" FooterStyle-HorizontalAlign="Right" />
                                    <asp:BoundField ItemStyle-Width="102" HeaderText="Amount" DataField="amount_approved" DataFormatString="{0:N}" ItemStyle-HorizontalAlign="Right" FooterStyle-HorizontalAlign="Right" />
                                </Columns>
                                <PagerSettings Position="Top" />
                                <EmptyDataTemplate>
                                    There are no records to display.
                                </EmptyDataTemplate>
                            </asp:GridView>
                            <asp:Label ID="rpError" runat="server" Text="" ForeColor="Red" Font-Bold="true" />
                            &nbsp;
                        </div>
                        <div id="report_details">
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
            <div id="admin_section">
                <asp:UpdatePanel ID="upAdminSection" runat="server">
                    <Triggers>
                    </Triggers>
                    <ContentTemplate>
                        <asp:Label ID="sqlPrint" runat="server" Text="" />
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </div>
    </div>
    <div class="progressUpdate_wrap">
        <asp:AlwaysVisibleControlExtender ID="AlwaysVisibleControlExtender1" runat="server"
            TargetControlID="UpdateProgress1"
            VerticalSide="Middle"
            HorizontalSide="Left"
            HorizontalOffset="10"
            ScrollEffectDuration=".1"
            />        
        <asp:UpdateProgress ID="UpdateProgress1" runat="server">
            <ProgressTemplate>
                <%--width: 500px;text-align: center;height: 150px;--%>
                <%--margin: 0 auto;width: 250px;text-align: left;padding-top: 50px;--%>
                <div id="progressUpdate" class="progressUpdate" style="background-color: #C6C6C6;width: 500px;height: 150px;">
                    <div style="margin: 0 auto;width: 250px;padding-top: 50px;">
                        <div>Your request is processing, please wait.<br />This process may take several minutes...</div>
                        <div id="sinceCountdown" style="width: 200px;background-color: #C6C6C6;"></div>
                        <div><img id="loading" src="../images/loading/loading6.gif" alt="..." /></div>
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
                    <EmptyDataTemplate>
                        No Errors
                    </EmptyDataTemplate>
                </asp:DetailsView>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder2" Runat="Server">
</asp:Content>

