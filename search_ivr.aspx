<%@ Page Title="IVR Search" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="search_ivr.aspx.cs" Inherits="search_ivr" %>
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
<%--    <link href="css/start/jquery-ui-1.8.18.custom.css" rel="stylesheet" type="text/css" />
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
    <script src="js/jquery.multiselect.js" type="text/javascript"></script>--%>
    <script type="text/javascript">
        function OnLoad_Function() {
            GH_Buttons();
            GH_Select();
            GH_SelectMultiple();
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
    <script type="text/javascript">
        function FixExport() {
            //$('#<%= btnExport.ClientID  %> ').find("[value=0]").attr("disabled", "disabled");
            //$('#<%= btnExport.ClientID  %> ').find("[value=0]").attr("aria-disabled", "true");
        }
    </script>
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
    <div id="default_container">
        <div id="report_header">
            <span style="font-weight: bold;">IVR Record Search Tool</span>
            <br />Currently this tool will only display records the system considers invalid
            <br />All records must have a VC file; currently that is the only restriction
            <br />CC/RN files should come before VC; so records should also have CC or RN file
            <br /><span style="color: DarkRed;">Cleared records will display here until they are processed.</span>
            <hr />
        </div>
        <div id="report_control1">
            <input id="btnFilters01" type="button" value="Hide All Filters" onclick="Filter_Toggle(this,'reporting_filters','All Filters');" />
        </div>
        <div id="reporting_filters" class="reporting_filters">
            <div class="clearfix">
                <div id="filter_type" class="filter_section">
                    <h3>Call Type</h3>
                    <div id="call_type">
                        <asp:Label ID="lblCallType" runat="server" Text="Type" CssClass="filter_label1" />
                        <asp:ListBox ID="ddlCallType" runat="server" SelectionMode="Multiple" Width="130px" Width2="300px" multiple="multiple" CssClass="multiselect">
                            <asp:ListItem Value="completed" Text="Completed" />
                            <asp:ListItem Value="pending" Text="Pending" />
                            <asp:ListItem Value="invalid" Text="Invalid" />
                            <asp:ListItem Value="cleared" Text="Cleared" />
                            <asp:ListItem Value="discarded" Text="Discarded" />
                        </asp:ListBox>
                    </div>
                </div>
                <div id="filter_received" class="filter_section">
                    <h3>Received Files</h3>
                    <div id="donation_source">
                        <asp:Label ID="lblReceivedFiles" runat="server" Text="File Types" CssClass="filter_label1" />
                        <asp:ListBox ID="ddlFileTypes" runat="server" SelectionMode="Multiple" Width="130px" Width2="300px" multiple="multiple" CssClass="multiselect">
                            <asp:ListItem Value="VC" Text="VC" />
                            <asp:ListItem Value="CC" Text="CC" />
                            <asp:ListItem Value="RANI" Text="RANI" />
                            <asp:ListItem Value="OP" Text="OP" />
                            <asp:ListItem Value="CT" Text="CT" />
                        </asp:ListBox>
                    </div>
                </div>
                <div id="filter_missing" class="filter_section">
                    <h3>Missing Files</h3>
                    <div id="donation_source">
                        <asp:Label ID="lblMissingFiles" runat="server" Text="File Types" CssClass="filter_label1" />
                        <asp:ListBox ID="ddlFileMissing" runat="server" SelectionMode="Multiple" Width="130px" Width2="300px" multiple="multiple" CssClass="multiselect">
                            <asp:ListItem Value="VC" Text="VC" />
                            <asp:ListItem Value="CC" Text="CC" />
                            <asp:ListItem Value="RANI" Text="RANI" />
                            <asp:ListItem Value="OP" Text="OP" />
                            <asp:ListItem Value="CT" Text="CT" />
                        </asp:ListBox>
                    </div>
                </div>
            </div>
            <div class="clearfix">
                <div id="filter_dates" class="filter_section">
                    <h3>Call Dates</h3>
                    <div id="datetime_start">
                        <asp:Label ID="Label3" runat="server" Text="Start Time" CssClass="filter_label1" />
                        <asp:TextBox ID="dtStartDate" runat="server" Width="75px" CssClass="ghDatePickerStart clearme" />
                        <asp:TextBox ID="dtStartTime" runat="server" Width="50px" CssClass="ghTimePickerStart" />
                    </div>
                    <div id="datetime_end">
                        <asp:Label ID="Label4" runat="server" Text="End Time" CssClass="filter_label1" />
                        <asp:TextBox ID="dtEndDate" runat="server" Width="75px" CssClass="ghDatePickerEnd clearme" />
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
                    <h3>Options</h3>
                    <div id="donation_max">
                        <asp:Label ID="lblMaxRecords" runat="server" Text="Max Records" CssClass="filter_label1" />
                        <asp:DropDownList ID="ddlTop" runat="server">
                            <asp:ListItem Value="100" Text="100" />
                            <asp:ListItem Value="500" Text="500" />
                            <asp:ListItem Value="1000" Text="1000" Selected="True" />
                            <asp:ListItem Value="2500" Text="2500" />
                            <asp:ListItem Value="5000" Text="5000" />
                            <asp:ListItem Value="10000" Text="10000" />
                        </asp:DropDownList>
                    </div>
                    <div>
                        <div style="margin-left: 15px;">
                            It is recommend you use a max if searching for specific records.
                        </div>
                    </div>
                </div>
                <div id="Div4" class="filter_section">
                    <h3>&nbsp;</h3>
                </div>
            </div>
            <div style="color: Blue;">
                All filter options work together; you can clear any text or date fields to remove them from the query.
                <br /><br />
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
                            $('#<%= btnSearch.ClientID  %> ').find("[value=0]").attr("disabled", "disabled");
                            $('#<%= btnSearch.ClientID  %> ').find("[value=0]").attr("aria-disabled", "true");
                        }
                        }
                    </script>
                    <div>
                        <asp:GridView ID="gvTotalCounts" runat="server" AutoGenerateColumns="False"
                            BackColor="White"
                            BorderStyle="None"
                            BorderWidth="1px"
                            BorderColor="#DEDFDE"
                            AllowSorting="True"
                            ForeColor="Black"
                            GridLines="Vertical"
                            CellPadding="20"
                            CellSpacing="20"

                            CssClass="Portal_GridView_Standard"
                            Width="510"
                            >
                            <AlternatingRowStyle CssClass="Portal_GridView_Standard_Alternate" />
                            <SelectedRowStyle CssClass="Portal_GridView_Standard_Selected" />
                            <PagerSettings Position="Top" />
                            <PagerStyle VerticalAlign="Bottom" HorizontalAlign="Center" CssClass="Portal_Gridview_Pager" />
                            <Columns>
                                <asp:BoundField HeaderText="Since" DataField="since" />
                                <asp:BoundField HeaderText="Total" DataField="total" />
                                <asp:BoundField HeaderText="Completed" DataField="completed" />
                                <asp:BoundField HeaderText="Pending" DataField="pending" />
                                <asp:BoundField HeaderText="Invalid" DataField="invalid" />
                                <asp:BoundField HeaderText="Cleared" DataField="cleared" />
                                <asp:BoundField HeaderText="Discarded" DataField="discarded" />
                            </Columns>
                            <EmptyDataTemplate>
                                No Records For Selected Filters
                            </EmptyDataTemplate>
                        </asp:GridView>
                    </div>
                    <div>
                    <style type="text/css">
                        .btn_submit2:after
                        {
                        	content: " | ";
                        }
                    </style>
                        <div class="btn_submit" style="display: inline-block;width: 75px;">
                            <asp:Button ID="btnSearch" runat="server"
                                Text="Search"
                                OnClientClick="this.disabled = true; this.value = 'Running...';FixExport();" 
                                UseSubmitBehavior="false" 
                                OnClick="GridView_Refresh"
                                />
                        </div>
                        &nbsp;|&nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:Button ID="btnExport" runat="server"
                            Text="Export to Excel"
                            OnClientClick="this.disabled = true; this.value = 'Running...';" 
                            UseSubmitBehavior="false" 
                            OnClick="GridView_Export_Excel"
                            Visible="false"
                            />
                        <span id="divClearAllRecord" runat="server" Visible="false">
                            &nbsp;|&nbsp;&nbsp;&nbsp;&nbsp;<asp:Button ID="btnClearAllRecord" runat="server" Text="Clear All Records" OnClick="Processing_All_Clear" />
                        </span>
                        
                        <span id="divUndoClearAllRecord" runat="server" Visible="false">
                            &nbsp;|&nbsp;&nbsp;&nbsp;&nbsp;<asp:Button ID="btnUndoClearAllRecord" runat="server" Text="Undo Clear All Records" OnClick="Processing_All_UndoClear" />
                        </span>
                        <span id="divDiscardAllRecord" runat="server" Visible="false">
                            &nbsp;|&nbsp;&nbsp;&nbsp;&nbsp;<asp:Button ID="btnDiscardAllRecord" runat="server" Text="Discard All Record" OnClick="Processing_All_Discard" />
                        </span>
                        <span id="divUndoDiscardAllRecord" runat="server" Visible="false">
                            &nbsp;|&nbsp;&nbsp;&nbsp;&nbsp;<asp:Button ID="btnUndoDiscardAllRecord" runat="server" Text="Undo Discard All Record" OnClick="Processing_All_UndoDiscard" />
                        </span>
                        <div style="display: inline-block;">
                            <asp:Label ID="rpElapsed" runat="server" Text="" />
                        </div>
                        <div style="display: inline-block;">
                            <asp:Label ID="lblAllRecordConfirm" runat="server" Text="" />
                        </div>
                        
                    </div>
                    <div>
                        <asp:GridView ID="gvQueryTotalCounts" runat="server" AutoGenerateColumns="False"
                            BackColor="White"
                            BorderStyle="None"
                            BorderWidth="1px"
                            BorderColor="#DEDFDE"
                            AllowSorting="True"
                            ForeColor="Black"
                            GridLines="Vertical"
                            CellPadding="20"
                            CellSpacing="20"

                            CssClass="Portal_GridView_Standard"
                            Width="510"
                            >
                            <AlternatingRowStyle CssClass="Portal_GridView_Standard_Alternate" />
                            <SelectedRowStyle CssClass="Portal_GridView_Standard_Selected" />
                            <PagerSettings Position="Top" />
                            <PagerStyle VerticalAlign="Bottom" HorizontalAlign="Center" CssClass="Portal_Gridview_Pager" />
                            <Columns>
                                <asp:BoundField HeaderText="Since" DataField="since" />
                                <asp:BoundField HeaderText="Total" DataField="total" />
                                <asp:BoundField HeaderText="Completed" DataField="completed" />
                                <asp:BoundField HeaderText="Pending" DataField="pending" />
                                <asp:BoundField HeaderText="Invalid" DataField="invalid" />
                                <asp:BoundField HeaderText="Cleared" DataField="cleared" />
                                <asp:BoundField HeaderText="Discarded" DataField="discarded" />
                            </Columns>
                            <EmptyDataTemplate>
                                No Records For Selected Filters
                            </EmptyDataTemplate>
                        </asp:GridView>
                    </div>
                    <div>
                        <asp:Panel ID="ExpiredAllRecord" runat="server" Visible="false">
                            <div style="color: DarkRed;font-weight: bold;">
                            <br />The time between the last query search and the clear/discard was too long.
                            <br />Please refresh the search before performing this function
                            <br />You have 2 minutes to perform this function after performing a query search.
                            <br />&nbsp;
                            </div>
                        </asp:Panel>
                        <asp:Panel ID="ClearAllRecord" runat="server" Visible="false">
                            <div style="color: DarkRed;font-weight: bold">
                            <br />Performing this action may not be reversible; be sure you query is accurate and the numbers match.
                            <br />You are clearing all records for processing
                            <br />This will process the record as is and not wait for any other file or data.
                            <br />&nbsp;
                            </div>
                            <asp:Button ID="btnClearAllConfirm" runat="server" Text="Confirm Clear" OnClick="Processing_All_Clear_Confirm" />
                            <asp:Button ID="btnClearAllCancel" runat="server" Text="Cancel Clear" OnClick="Processing_All_Clear_Cancel" />
                            <br /><asp:Label ID="lblClearAllConfirm" runat="server" Text="" />
                        </asp:Panel>
                        <asp:Panel ID="UndoClearAllRecord" runat="server" Visible="false">
                            <div style="color: DarkRed;font-weight: bold">
                            <br />You are reversing the clearing of all records
                            <br />This will put the record back into Pending/Invalid status based on other filters/criteria
                            <br />&nbsp;
                            </div>
                            <asp:Button ID="btnUndoClearAllConfirm" runat="server" Text="Confirm Undo Clear" OnClick="Processing_All_UndoClear_Confirm" />
                            <asp:Button ID="btnUndoClearAllCancel" runat="server" Text="Cancel Undo Clear" OnClick="Processing_All_UndoClear_Cancel" />
                            <br /><asp:Label ID="lblUndoClearAllConfirm" runat="server" Text="" ForeColor="DarkRed" Font-Bold="true" />
                        </asp:Panel>
                        <asp:Panel ID="DiscardAllRecord" runat="server" Visible="false">
                            <div style="color: DarkRed;font-weight: bold">
                            <br />Performing this action may not reversible; be sure you query is accurate and the numbers match.
                            <br />You are discarding all records from processing
                            <br />This will remove the record from the invalid pool and this list.
                            <br />Discarded records do not show up by default in the grid.</span>
                            <br />&nbsp;
                            </div>
                            <asp:Button ID="btnDiscardAllConfirm" runat="server" Text="Confirm Discard" OnClick="Processing_All_Discard_Confirm" />
                            <asp:Button ID="btnDiscardAllCancel" runat="server" Text="Cancel Discard" OnClick="Processing_All_Discard_Cancel" />
                            <br /><asp:Label ID="lblDiscardAllConfirm" runat="server" Text="" ForeColor="Blue" Font-Bold="true" />
                        </asp:Panel>
                        <asp:Panel ID="UndoDiscardAllRecord" runat="server" Visible="false">
                            <div style="color: DarkRed;font-weight: bold">
                            <br />You are reversing the discard of all records
                            <br />This will put the record back into Pending/Invalid status based on other filters/criteria
                            <br />&nbsp;
                            </div>
                            <asp:Button ID="btnUndoDiscardAllConfirm" runat="server" Text="Confirm Undo Discard" OnClick="Processing_All_UndoDiscard_Confirm" />
                            <asp:Button ID="btnUndoDiscardAllCancel" runat="server" Text="Cancel Undo Discard" OnClick="Processing_All_UndoDiscard_Cancel" />
                            <br /><asp:Label ID="lblUndoDiscardAllConfirm" runat="server" Text="" ForeColor="Blue" Font-Bold="true" />
                        </asp:Panel>
                    </div>
                    <div id="grid" style="display: inline-block;width: 515px;vertical-align: top;">
                        <asp:GridView ID="gvSearchResults" runat="server" AutoGenerateColumns="False"
                            BackColor="White"
                            BorderStyle="None"
                            BorderWidth="1px"
                            BorderColor="#DEDFDE"
                            AllowSorting="True"
                            ForeColor="Black"
                            GridLines="Vertical"
                            CellPadding="20"
                            CellSpacing="20"
                            AllowPaging="true" PageSize="100"
                            DataKeyNames="sourceid,recordid,calldate,calltime,ani"

                            OnDataBound="GridView_DataBound"
                            OnRowDataBound="GridView_RowDataBound"
                            OnSelectedIndexChanged="GridView_IndexChanged"
                            OnPageIndexChanging="GridView_PageIndexChanging"

                            CssClass="Portal_GridView_Standard"
                            Width="510"
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
                                <asp:BoundField HeaderText="sourceid" DataField="sourceid" />
                                <asp:BoundField HeaderText="recordid" DataField="recordid" />
                                <asp:BoundField HeaderText="calldate" DataField="calldate" />
                                <asp:BoundField HeaderText="calltime" DataField="calltime" />
                                <asp:BoundField HeaderText="ani" DataField="ani" />
                                <asp:BoundField HeaderText="dnis" DataField="dnis" />
                                <asp:BoundField HeaderText="files" DataField="files" />
                                <asp:TemplateField HeaderText="imported" ItemStyle-CssClass="LastCol">
                                    <ItemTemplate>
                                        <asp:Label ID="since" runat="server" Text='<%# ghFunctions.date_label(Eval("started").ToString()) %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField HeaderText="status" DataField="status" />
                                <asp:TemplateField HeaderText="since" ItemStyle-CssClass="LastCol">
                                    <ItemTemplate>
                                        <asp:Label ID="since" runat="server" Text='<%# ghFunctions.since_label(Eval("calldate").ToString(), msgLabel) %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:CommandField SelectText="Select" ShowSelectButton="true" ItemStyle-CssClass="HideButton" HeaderStyle-CssClass="HideButton" />
                            </Columns>
                            <EmptyDataTemplate>
                                No Records For Selected Filters
                            </EmptyDataTemplate>
                        </asp:GridView>
                        <asp:Label ID="Label8" runat="server" Text="" />
                    </div>
                    <div id="admin_section_old" style="width: 500px;">
                        
                    </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
            <div id="search_details" style="display: inline-block;vertical-align: top;">
                <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                    <ContentTemplate>
                    <div>
                        <span style="font-weight: bold;">Select a record to show it's details below</span>
                        <br /><asp:Label ID="Label16" runat="server" Text="" />
                        <br /><asp:Button ID="btnClearRecord" runat="server" Text="Clear Record" OnClick="Processing_Record_Clear" Visible="false" />
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:Button ID="btnDiscardRecord" runat="server" Text="Discard Record" OnClick="Processing_Record_Discard" Visible="false" />
                        <asp:Panel ID="ClearRecord" runat="server">
                            You are clearing the record for processing
                            <br />This will process the record as is and not wait for any other file or data.
                            <br />
                            <asp:Button ID="btnClearRecordConfirm" runat="server" Text="Confirm" OnClick="Processing_Record_Clear_Confirm" />
                            <asp:Button ID="btnClearRecordCancel" runat="server" Text="Cancel" OnClick="Processing_Record_Clear_Cancel" />
                            <br /><asp:Label ID="lblClearRecordConfirm" runat="server" Text="" />
                        </asp:Panel>
                        <asp:Panel ID="DiscardRecord" runat="server">
                            You are discarding the record from processing
                            <br />This will remove the record from the invalid pool and this list.
                            <br /><span style="color: DarkRed;">Once you discard the record it will no longer show up on this list.</span>
                            <br />
                            <asp:Button ID="btnDiscardRecordConfirm" runat="server" Text="Confirm" OnClick="Processing_Record_Discard_Confirm" />
                            <asp:Button ID="btnDiscardRecordCancel" runat="server" Text="Cancel" OnClick="Processing_Record_Discard_Cancel" />
                            <br /><asp:Label ID="lblDiscardRecordConfirm" runat="server" Text="" />
                        </asp:Panel>
                    </div>
                    <asp:DetailsView ID="dv_ivr_file_vc" runat="server"
                        AutoGenerateRows="True" ForeColor="Black" GridLines="Vertical"
                        HeaderText="VC File"
                        CssClass="User_List_Details_Border"

                        OnDataBound="DetailsView_DataBound"
                        OnItemCommand="DetailsView_ItemCommand"
                        OnModeChanging="DetailsView_ModeChanging"
                        OnItemUpdating="DetailsView_ItemUpdating"
                        OnItemUpdated="DetailsView_ItemUpdated"
                        
                        Width="400px"
                        >
                        <HeaderStyle CssClass="User_List_Details_Header" />
                        <RowStyle BackColor="#CCCCCC" CssClass="User_List_Details_Row" />
                        <FieldHeaderStyle Width="100px" />
                        <EmptyDataRowStyle BackColor="White" />
                        <Fields>
                        </Fields>
                        <EmptyDataTemplate>
                            No VC File Record;
                        </EmptyDataTemplate>
                    </asp:DetailsView>
                    <asp:DetailsView ID="dv_ivr_file_cc" runat="server"
                        AutoGenerateRows="True" ForeColor="Black" GridLines="Vertical"
                        HeaderText="CC File"
                        CssClass="User_List_Details_Border"

                        OnDataBound="DetailsView_DataBound"
                        OnItemCommand="DetailsView_ItemCommand"
                        OnModeChanging="DetailsView_ModeChanging"
                        OnItemUpdating="DetailsView_ItemUpdating"
                        OnItemUpdated="DetailsView_ItemUpdated"
                        
                        Width="400px"
                        >
                        <HeaderStyle CssClass="User_List_Details_Header" />
                        <RowStyle BackColor="#CCCCCC" CssClass="User_List_Details_Row" />
                        <FieldHeaderStyle Width="100px" />
                        <EmptyDataRowStyle BackColor="White" />
                        <Fields>
                        </Fields>
                        <EmptyDataTemplate>
                            No CC File Record;
                        </EmptyDataTemplate>
                    </asp:DetailsView>
                    <asp:DetailsView ID="dv_ivr_file_rn" runat="server"
                        AutoGenerateRows="True" ForeColor="Black" GridLines="Vertical"
                        HeaderText="RANI File"
                        CssClass="User_List_Details_Border"

                        OnDataBound="DetailsView_DataBound"
                        OnItemCommand="DetailsView_ItemCommand"
                        OnModeChanging="DetailsView_ModeChanging"
                        OnItemUpdating="DetailsView_ItemUpdating"
                        OnItemUpdated="DetailsView_ItemUpdated"
                        
                        Width="400px"
                        >
                        <HeaderStyle CssClass="User_List_Details_Header" />
                        <RowStyle BackColor="#CCCCCC" CssClass="User_List_Details_Row" />
                        <FieldHeaderStyle Width="100px" />
                        <EmptyDataRowStyle BackColor="White" />
                        <Fields>
                        </Fields>
                        <EmptyDataTemplate>
                            No RANI File Record;
                        </EmptyDataTemplate>
                    </asp:DetailsView>
                    <asp:DetailsView ID="dv_ivr_file_op" runat="server"
                        AutoGenerateRows="False" ForeColor="Black" GridLines="Vertical"
                        HeaderText="OP File"
                        CssClass="User_List_Details_Border"

                        OnDataBound="DetailsView_DataBound"
                        OnItemCommand="DetailsView_ItemCommand"
                        OnModeChanging="DetailsView_ModeChanging"
                        OnItemUpdating="DetailsView_ItemUpdating"
                        OnItemUpdated="DetailsView_ItemUpdated"
                        
                        Width="400px"
                        >
                        <HeaderStyle CssClass="User_List_Details_Header" />
                        <RowStyle BackColor="#CCCCCC" CssClass="User_List_Details_Row" />
                        <FieldHeaderStyle Width="100px" />
                        <EmptyDataRowStyle BackColor="White" />
                        <Fields>
                            <asp:BoundField HeaderText="CallID" DataField="CallID" ReadOnly="true" />
                            <asp:HyperLinkField
                                HeaderText="cbid"
                                DataNavigateUrlFields="cbid"
                                DataNavigateUrlFormatString="~/CallRefunds.aspx?cbid={0}"
                                DataTextField="cbid"
                                Target="_blank"
                                />
                            <asp:BoundField HeaderText="status" DataField="status" ReadOnly="true" />
                            <asp:BoundField HeaderText="decision" DataField="decision" ReadOnly="true" />
                            <asp:BoundField HeaderText="reasoncode" DataField="reasoncode" ReadOnly="true" />
                            <asp:BoundField HeaderText="authed" DataField="authed" ReadOnly="true" />
                        </Fields>
                        <EmptyDataTemplate>
                            No OP File Record;
                        </EmptyDataTemplate>
                    </asp:DetailsView>
                    <asp:DetailsView ID="dv_ivr_file_ct" runat="server"
                        AutoGenerateRows="True" ForeColor="Black" GridLines="Vertical"
                        HeaderText="CT File"
                        CssClass="User_List_Details_Border"

                        OnDataBound="DetailsView_DataBound"
                        OnItemCommand="DetailsView_ItemCommand"
                        
                        Width="400px"
                        >
                        <HeaderStyle CssClass="User_List_Details_Header" />
                        <RowStyle BackColor="#CCCCCC" CssClass="User_List_Details_Row" />
                        <FieldHeaderStyle Width="100px" />
                        <EmptyDataRowStyle BackColor="White" />
                        <Fields>
                        </Fields>
                        <EmptyDataTemplate>
                            No CT File Record;
                        </EmptyDataTemplate>
                    </asp:DetailsView>
                    <asp:DetailsView ID="dv_ivr_file_main" runat="server"
                        AutoGenerateRows="True" ForeColor="Black" GridLines="Vertical"
                        HeaderText="MAIN File"
                        CssClass="User_List_Details_Border"

                        OnDataBound="DetailsView_DataBound"
                        OnItemCommand="DetailsView_ItemCommand"
                        
                        Width="400px"
                        >
                        <HeaderStyle CssClass="User_List_Details_Header" />
                        <RowStyle BackColor="#CCCCCC" CssClass="User_List_Details_Row" />
                        <FieldHeaderStyle Width="100px" />
                        <EmptyDataRowStyle BackColor="White" />
                        <Fields>
                        </Fields>
                        <EmptyDataTemplate>
                            No MAIN File Record;
                        </EmptyDataTemplate>
                    </asp:DetailsView>
                    <asp:DetailsView ID="dv_ivr_file_drtv" runat="server"
                        AutoGenerateRows="True" ForeColor="Black" GridLines="Vertical"
                        HeaderText="DRTV File"
                        CssClass="User_List_Details_Border"

                        OnDataBound="DetailsView_DataBound"
                        OnItemCommand="DetailsView_ItemCommand"
                        
                        Width="400px"
                        >
                        <HeaderStyle CssClass="User_List_Details_Header" />
                        <RowStyle BackColor="#CCCCCC" CssClass="User_List_Details_Row" />
                        <FieldHeaderStyle Width="100px" />
                        <EmptyDataRowStyle BackColor="White" />
                        <Fields>
                        </Fields>
                        <EmptyDataTemplate>
                            No DRTV File Record;
                        </EmptyDataTemplate>
                    </asp:DetailsView>
                    <asp:DetailsView ID="dv_ivr_file_record" runat="server"
                        AutoGenerateRows="True" ForeColor="Black" GridLines="Vertical"
                        HeaderText="RECORD File"
                        CssClass="User_List_Details_Border"

                        OnDataBound="DetailsView_DataBound"
                        OnItemCommand="DetailsView_ItemCommand"
                        
                        Width="400px"
                        >
                        <HeaderStyle CssClass="User_List_Details_Header" />
                        <RowStyle BackColor="#CCCCCC" CssClass="User_List_Details_Row" />
                        <FieldHeaderStyle Width="100px" />
                        <EmptyDataRowStyle BackColor="White" />
                        <Fields>
                        </Fields>
                        <EmptyDataTemplate>
                            No RECORD File Record;
                        </EmptyDataTemplate>
                    </asp:DetailsView>
                    <asp:Label Font-Bold="true" ID="dtlLabel" runat="server" Text="" />
                    <div style="width: 575px">
                        <asp:Label ID="Error_General" runat="server" Text="General Error Label" />
                        <div class="user_error_standard">
                            <asp:ValidationSummary ID="vsCredentials" runat="server"
                                ForeColor="DarkRed" DisplayMode="BulletList" ShowMessageBox="false" 
                                HeaderText="Credentials Validation Failed:"
                                ValidationGroup="vsCredentials"
                                />
                            <asp:ValidationSummary ID="vsDetails" runat="server"
                                ForeColor="DarkRed" DisplayMode="BulletList" ShowMessageBox="false" 
                                HeaderText="Details Validation Failed:"
                                ValidationGroup="vsDetails"
                                />
                            <asp:ValidationSummary ID="vsContact" runat="server"
                                ForeColor="DarkRed" DisplayMode="BulletList" ShowMessageBox="false" 
                                HeaderText="Contact Validation Failed:"
                                ValidationGroup="vsContact"
                                />
                            <asp:ValidationSummary ID="vsAddress" runat="server"
                                ForeColor="DarkRed" DisplayMode="BulletList" ShowMessageBox="false" 
                                HeaderText="Address Validation Failed:"
                                ValidationGroup="vsAddress"
                                />
                        </div>
                    </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
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
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">
</asp:Content>
