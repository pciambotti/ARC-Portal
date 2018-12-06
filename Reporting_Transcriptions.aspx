<%@ Page Title="Reporting Stats" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Reporting_Transcriptions.aspx.cs" Inherits="Reporting_Transcriptions" %>
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

    </style>
    <link href="css/start/jquery-ui-1.8.18.custom.css" rel="stylesheet" type="text/css" />
    <link href="css/start/jquery.ui.selectmenu.css" rel="stylesheet" type="text/css" />
    <script src="js/jquery-1.7.1.min.js" type="text/javascript"></script>
    <script src="js/jquery-ui-1.8.18.custom.min.js" type="text/javascript"></script>
    <script src="js/jquery-ui-timepicker-addon.js" type="text/javascript"></script>
    <script src="js/jquery.ui.selectmenu.js" type="text/javascript"></script>
    <link href="js/jquery.countdown.css" rel="stylesheet" type="text/css" />
    <script src="js/jquery.countdown.pack.js" type="text/javascript"></script>
    <script src="js/jquery-gh-custom-objects.js" type="text/javascript"></script>
    <script type="text/javascript">
        function OnLoad_Function() {
            GH_Buttons();
            GH_DropDown();
            GH_DatePicker();
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
    </script>
    <script type="text/javascript">
        function Filter_Toggle_Dashboard(btn, field, tgl) {
            // images/up_white_red.jpg
            // images/down_white_green.jpg
            // Hide/Show if an option is selected
            // Toggle if no option is selected
            // btn the requester
            // field the div to hide
            // tgl if present, we hide or show based on that
            $(document).ready(function() {
                //alert($(btn).attr("src"));
                //alert($(btn).val().substring(5, $(btn).val().length));
                if (tgl == "") { tgl = $(btn).val(); }
                var nme = $(btn).val().substring(5, $(btn).val().length);
                if (tgl == "hide") { $("#" + field).hide(); $(btn).val("show"); $(btn).attr("src", "images/down_white_green.jpg"); }
                else { $("#" + field).show(); $(btn).val("hide"); $(btn).attr("src", "images/up_white_red.jpg"); }
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
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Master_Content" Runat="Server">
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
    <div id="default_container">
        <div id="report_header">
            Transcription Reporting Tool
            <hr />
        </div>
        <div id="report_control1">
            <input id="btnFilters01" type="button" value="Hide All Filters" onclick="Filter_Toggle(this,'reporting_filters','All Filters');" />
        </div>
        <div id="reporting_filters" class="reporting_filters">
            <div class="clearfix">
                <div id="filter_dates" class="filter_section">
                    <h3>Record Dates</h3>
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
                <div id="filter_options" class="filter_section">
                    <h3>Filter Options</h3>
                    <div id="filter_clientid">
                        <asp:Label ID="lblClientID" runat="server" Text="Client" CssClass="filter_label1" />
                        <asp:DropDownList ID="ddlClientID" runat="server" AppendDataBoundItems="true" Width="150px" Width2="200px" />
                    </div>
                    <div id="filter_status">
                        <asp:Label ID="lblStatus" runat="server" Text="Status" CssClass="filter_label1" />
                        <asp:DropDownList ID="ddlStatus" runat="server" Width="150px" Width2="200px">
                            <asp:ListItem Text="All" Value="0" />
                        </asp:DropDownList>
                    </div>
                </div>
            </div>
            <div style="color: Blue;">
                All filter options work together; you can clear any text or date fields to remove them from the query.
                <br /><br />
            </div>
        </div>
        <div id="search_result" class="clearfix">
            <div id="search_grid" style="float: left;width: 415px;vertical-align: top;">
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
                                    $('#<%= Button2.ClientID  %> ').find("[value=0]").attr("disabled", "disabled");
                                    $('#<%= Button2.ClientID  %> ').find("[value=0]").attr("aria-disabled", "true");
                                }
                            }
                        </script>
                        <div>
                            <%--Filter_Toggle_Hide(btnFilters01,'reporting_filters','All Filters');--%>
                            <asp:Button ID="Button2" runat="server"
                                Text="Run/Refresh Stats"
                                OnClientClick="this.disabled = true; this.value = 'Running...';" 
                                UseSubmitBehavior="false" 
                                OnClick="GridView_Refresh"
                                />
                            <asp:Button ID="btnExport" runat="server" Text="Export to Excel" OnClick="GridView_Export_Excel" Visible="false" />
                            <div style="display: inline-block;">
                                <asp:Label ID="rpElapsed" runat="server" Text="" />
                            </div>
                        </div>
                        <div>
                            <asp:Label ID="lblFilterDetails" runat="server" Text="" ForeColor="Blue" />
                        </div>
                        <div id="grid">
                            <br />
                            <span style="font-weight: bold;color: DarkRed;">Transcription Counts</span>
                            <asp:CheckBox ID="cbDateBreakdown" runat="server" /> Enable date breakdown
                            <asp:Label ID="Label8" runat="server" Text="" />
                            <asp:GridView ID="gvTranscriptions_Counts" runat="server" AutoGenerateColumns="False"
                                BackColor="White"
                                BorderStyle="None"
                                BorderWidth="1px"
                                BorderColor="#DEDFDE"
                                ForeColor="Black"
                                GridLines="Vertical"
                                CellPadding="20"
                                CellSpacing="20"
                                AllowPaging="False"

                                OnDataBound="GridView_DataBound"
                                OnRowDataBound="GridView_RowDataBound"
                                OnSelectedIndexChanged="GridView_IndexChanged"

                                CssClass="Portal_GridView_Standard"
                                Width="375"
                                >
                                <AlternatingRowStyle CssClass="Portal_GridView_Standard_Alternate" />
                                <SelectedRowStyle CssClass="Portal_GridView_Standard_Selected" />
                                <PagerSettings Position="Top" />
                                <PagerStyle VerticalAlign="Bottom" HorizontalAlign="Center" CssClass="Portal_Gridview_Pager" />
                                <Columns>
                                    <asp:BoundField HeaderText="date" DataField="date" />
                                    <asp:BoundField HeaderText="clientid" DataField="clientid" />
                                    <asp:BoundField HeaderText="status" DataField="status" />
                                    <asp:BoundField HeaderText="count" DataField="count" />
                                    <%--<asp:BoundField HeaderText="approved" DataField="amount_approved" DataFormatString="{0:C}" />
                                    <asp:BoundField HeaderText="total" DataField="amount_total" DataFormatString="{0:C}" />
                                    <asp:BoundField HeaderText="last" DataField="last" />--%>
                                    <asp:CommandField SelectText="Select" ShowSelectButton="true" ItemStyle-CssClass="HideButton" HeaderStyle-CssClass="HideButton" />
                                </Columns>
                                <EmptyDataTemplate>
                                    No Records For Selected Filters
                                </EmptyDataTemplate>
                            </asp:GridView>
                            <br />
                        </div>
                        <hr style="width: 375px;" />
                        <asp:Label ID="Error_General" runat="server" Text="" />
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
            <div id="search_details" style="float: left;vertical-align: top;">
                <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                    <ContentTemplate>
                        <div id="grid_list">
                            <h3>List of Transactions</h3>
                            <br />&nbsp;
                            <asp:GridView ID="gvTranscriptions_List" runat="server" AutoGenerateColumns="False"
                                BackColor="White"
                                BorderStyle="None"
                                BorderWidth="1px"
                                BorderColor="#DEDFDE"
                                ForeColor="Black"
                                GridLines="Vertical"
                                AllowPaging="true"
                                PageSize="50"

                                CssClass="Portal_GridView_Standard"
                                >
                                <AlternatingRowStyle CssClass="Portal_GridView_Standard_Alternate" />
                                <SelectedRowStyle CssClass="Portal_GridView_Standard_Selected" />
                                <PagerSettings Position="Top" />
                                <PagerStyle VerticalAlign="Bottom" HorizontalAlign="Center" CssClass="Portal_Gridview_Pager" />
                                <Columns>
                                    <asp:BoundField HeaderText="transid" DataField="transid" />
                                    <asp:BoundField HeaderText="taskid" DataField="taskid" />
                                    <asp:BoundField HeaderText="recordid" DataField="recordid" />
                                    <asp:BoundField HeaderText="lang" DataField="lang" />
                                    <asp:BoundField HeaderText="tarfile" DataField="tarfile" />
                                    <asp:BoundField HeaderText="callid" DataField="callid" />
                                    <asp:BoundField HeaderText="calldate" DataField="calldate" />
                                    <asp:BoundField HeaderText="calltime" DataField="calltime" />
                                    <asp:BoundField HeaderText="ani" DataField="ani" />
                                    <asp:BoundField HeaderText="dnis" DataField="dnis" />
                                </Columns>
                                <EmptyDataTemplate>
                                    No Records For Selected Filters
                                </EmptyDataTemplate>
                            </asp:GridView>
                            <asp:Label ID="lblDetailsList" runat="server" Text="" />
                        </div>
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
        <asp:UpdateProgress ID="UpdateProgress1" runat="server" DisplayAfter="0">
            <ProgressTemplate>
                <%--width: 500px;text-align: center;height: 150px;--%>
                <%--margin: 0 auto;width: 250px;text-align: left;padding-top: 50px;--%>
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

