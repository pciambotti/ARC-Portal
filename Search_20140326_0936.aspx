<%@ Page Title="Search" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Search_20140326_0936.aspx.cs" Inherits="Search_20140326_0936" %>
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
    <link href="css/start/jquery-ui-1.8.18.custom.css" rel="stylesheet" type="text/css" />
    <link href="css/start/jquery.ui.selectmenu.css" rel="stylesheet" type="text/css" />
    <script src="js/jquery-1.7.1.min.js" type="text/javascript"></script>
    <script src="js/jquery-ui-1.8.18.custom.min.js" type="text/javascript"></script>
    <script src="js/jquery-ui-timepicker-addon.js" type="text/javascript"></script>
    <script src="js/jquery.ui.selectmenu.js" type="text/javascript"></script>
    <link href="js/jquery.countdown.css" rel="stylesheet" type="text/css" />
    <script src="js/jquery.countdown.pack.js" type="text/javascript"></script>
    <script src="js/jquery-gh-custom-objects.js" type="text/javascript"></script>
    <link href="js/jquery.multiselect.css" rel="stylesheet" type="text/css" />
    <script src="js/jquery.multiselect.js" type="text/javascript"></script>
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
            Donation Search Tool
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
                    <div id="donation_source">
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
                    <div id="donor_type">
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
                </div>
                <div id="filter_options" class="filter_section">
                    <h3>Options</h3>
                    <div id="donation_max">
                        <asp:Label ID="Label13" runat="server" Text="Max Records" CssClass="filter_label1" />
                        <asp:DropDownList ID="ddlTop" runat="server" Width="60" Width2="60">
                            <asp:ListItem Text="All" Value="0" Selected="True" />
                            <asp:ListItem Text="25" Value="25" />
                            <asp:ListItem Text="50" Value="50" />
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
                </div>
            </div>
            <div class="clearfix">
                <div id="filter_dates" class="filter_section">
                    <h3>Donation Dates</h3>
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
                <div id="Div4" class="filter_section">
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
                                $('#<%= Button2.ClientID  %> ').find("[value=0]").attr("disabled", "disabled");
                                $('#<%= Button2.ClientID  %> ').find("[value=0]").attr("aria-disabled", "true");
                            }
                        }
                    </script>
                    <div>
                        <%--Filter_Toggle_Hide(btnFilters01,'reporting_filters','All Filters');--%>
                        <asp:Button ID="Button2" runat="server"
                            Text="Search"
                            OnClientClick="this.disabled = true; this.value = 'Running...';" 
                            UseSubmitBehavior="false" 
                            OnClick="GridView_Refresh"
                            />
                        <div style="display: inline-block;">
                            <asp:Label ID="rpElapsed" runat="server" Text="" />
                        </div>
                        <asp:Button ID="btnExport" runat="server" Text="Export to Excel" OnClick="GridView_Export_Excel" Visible="false" />
                    </div>
                    <div id="grid" style="display: inline-block;width: 1015px;vertical-align: top;">
                        <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False"
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
                            Width="1010"
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
                                <asp:BoundField HeaderText="DonationID" DataField="DonationID" />
                                <asp:BoundField HeaderText="ID" DataField="CallID" />
                                <asp:BoundField HeaderText="DNIS" DataField="DNIS" />
                                <asp:BoundField HeaderText="Name" DataField="Name" />
                                <asp:BoundField HeaderText="Address" DataField="Address" />
                                <asp:BoundField HeaderText="City" DataField="City" />
                                <asp:BoundField HeaderText="Zip" DataField="Zip" />
                                <asp:BoundField HeaderText="State" DataField="State" />
                                <asp:BoundField HeaderText="Email" DataField="AckAddress" />
                                <asp:BoundField HeaderText="Disposition" DataField="Disposition" />
                                <asp:BoundField HeaderText="Amount" DataField="Amount" DataFormatString="{0:C}" />
                                <asp:BoundField HeaderText="Created" DataField="CreateDate" DataFormatString="{0:MM/dd/yyyy HH:mm:ss}" Visible="false" />
                                <asp:BoundField HeaderText="Status" DataField="Status"  />
                                <asp:CommandField SelectText="Select" ShowSelectButton="true" ItemStyle-CssClass="HideButton" HeaderStyle-CssClass="HideButton" />
                                <asp:TemplateField HeaderText="Created" ItemStyle-CssClass="LastCol" >
                                    <ItemTemplate>
                                        <asp:Label ID="CurrentPageLabel" runat="server" Text='<%# DateTime.Parse(Eval("CreateDate").ToString()).ToString("MM/dd/yyyy") == DateTime.Today.ToString("MM/dd/yyyy") ? DateTime.Parse(Eval("CreateDate").ToString()).ToString("hh:mm:ss tt") : DateTime.Parse(Eval("CreateDate").ToString()).ToString("MM/dd/yyyy hh:mm:ss tt") %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <EmptyDataTemplate>
                                No Records For Selected Filters
                            </EmptyDataTemplate>
                        </asp:GridView>
                        <asp:Label ID="Label8" runat="server" Text="Label"></asp:Label>
                        <asp:GridView ID="GridView2" runat="server" AutoGenerateColumns="False"
                            BackColor="White"
                            BorderStyle="None"
                            BorderWidth="1px"
                            BorderColor="#DEDFDE"
                            AllowSorting="True"
                            ForeColor="Black"
                            GridLines="Vertical"
                            AllowPaging="false"

                            CssClass="Portal_GridView_Standard"
                            Visible="false"
                            >
                            <AlternatingRowStyle CssClass="Portal_GridView_Standard_Alternate" />
                            <Columns>
                                <asp:BoundField HeaderText="DonationID" DataField="DonationID" />
                                <asp:BoundField HeaderText="ID" DataField="CallID" />
                                <asp:BoundField HeaderText="DNIS" DataField="DNIS" />
                                <asp:BoundField HeaderText="Name" DataField="Name" />
                                <asp:BoundField HeaderText="Address" DataField="Address" />
                                <asp:BoundField HeaderText="City" DataField="City" />
                                <asp:BoundField HeaderText="Zip" DataField="Zip" />
                                <asp:BoundField HeaderText="State" DataField="State" />
                                <asp:BoundField HeaderText="Email" DataField="AckAddress" />
                                <asp:BoundField HeaderText="Disposition" DataField="Disposition" />
                                <asp:BoundField HeaderText="Amount" DataField="Amount" DataFormatString="{0:C}" />
                                <asp:BoundField HeaderText="Created" DataField="CreateDate" DataFormatString="{0:MM/dd/yyyy HH:mm:ss}" Visible="false" />
                                <asp:BoundField HeaderText="Status" DataField="Status"  />
                                <asp:TemplateField HeaderText="Created" ItemStyle-CssClass="LastCol" >
                                    <ItemTemplate>
                                        <asp:Label ID="CurrentPageLabel" runat="server" Text='<%# DateTime.Parse(Eval("CreateDate").ToString()).ToString("MM/dd/yyyy") == DateTime.Today.ToString("MM/dd/yyyy") ? DateTime.Parse(Eval("CreateDate").ToString()).ToString("hh:mm:ss tt") : DateTime.Parse(Eval("CreateDate").ToString()).ToString("MM/dd/yyyy hh:mm:ss tt") %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
            <div id="search_details" style="display: block;vertical-align: top;">
                <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                    <ContentTemplate>
                    <div class="clearfix">
                        <div style="display: inline-block;vertical-align: top;">
                            <asp:DetailsView ID="dvCallDetails" runat="server" DataKeyNames="CallID"
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
                                <asp:BoundField HeaderText="CallID" DataField="CallID" ReadOnly="true" />
                                <asp:BoundField HeaderText="DonationID" DataField="id" ReadOnly="true" />
                                <asp:BoundField HeaderText="Name" DataField="Name" ReadOnly="true" />
                                <asp:BoundField HeaderText="Type" DataField="Type" ReadOnly="true" />
                                <asp:BoundField HeaderText="Amount" DataField="Amount" ReadOnly="true" />
                                <asp:BoundField HeaderText="Designation" DataField="DisplayName" ReadOnly="true" />
                                <asp:BoundField HeaderText="DesignationID" DataField="DesignationID" ReadOnly="true" />
                                <asp:BoundField HeaderText="DispositionID" DataField="DispositionID" ReadOnly="true" />
                                <asp:BoundField HeaderText="CreateDate" DataField="CreateDate" ReadOnly="true" />
                            </Fields>
                            <EmptyDataTemplate>
                                No [Call] details;
                            </EmptyDataTemplate>
                        </asp:DetailsView>
                            <asp:DetailsView ID="dvPaymentDetails" runat="server" DataKeyNames="CallID"
                                AutoGenerateRows="False" ForeColor="Black" GridLines="Vertical"
                                HeaderText="Payment Details"
                                CssClass="User_List_Details_Border"

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
			                                <asp:Label ID="lbl_cbid" runat="server" Text='<%# Eval("cbid") %>'
			                                Visible='<%# refund_visible_label(Eval("status").ToString())%>'
			                                />

		                                    <a style="display:block;"
		                                        runat="server"
		                                        Visible='<%# refund_visible_link(Eval("status").ToString())%>'
		                                        href='<%# "CallRefunds.aspx?cbid=" + Eval("cbid") %>'
        			                            
		                                        >
		                                        <%# DataBinder.Eval(Container.DataItem, "cbid")%>
		                                    </a>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:HyperLinkField
                                        HeaderText="cbid2"
                                        DataNavigateUrlFields="cbid"
                                        DataNavigateUrlFormatString="~/CallRefunds.aspx?cbid={0}"
                                        DataTextField="cbid"
                                        Target="_blank"
                                        Visible="false"
                                        />
                                    <asp:BoundField HeaderText="status" DataField="status" ReadOnly="true" />
                                    <asp:BoundField HeaderText="decision" DataField="decision" ReadOnly="true" />
                                    <asp:BoundField HeaderText="reasoncode" DataField="reasoncode" ReadOnly="true" />
                                    <asp:BoundField HeaderText="authed" DataField="authed" ReadOnly="true" />
                                    <asp:BoundField HeaderText="ccnum" DataField="ccnum" ReadOnly="true" />
                                    <asp:BoundField HeaderText="Reference" DataField="merchantreferencecode" ReadOnly="true" />
                                    <asp:BoundField HeaderText="Response" DataField="ccContent" ReadOnly="true" />
                                    <asp:BoundField HeaderText="CreateDate" DataField="CreateDate" ReadOnly="true" />
                                </Fields>
                                <EmptyDataTemplate>
                                    No [Payment] details;
                                </EmptyDataTemplate>
                            </asp:DetailsView>
                        </div>
                        <div style="display: inline-block;vertical-align: top;">
                            <asp:DetailsView ID="dvDonorDetails" runat="server" DataKeyNames="CallID"
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
                                    <asp:BoundField HeaderText="Amount" DataField="amount" ReadOnly="true" />
                                    <asp:BoundField HeaderText="Type" DataField="type" ReadOnly="true" />
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
                                    <asp:BoundField HeaderText="Email" DataField="ackaddress" ReadOnly="true" />
                                    <asp:BoundField HeaderText="Phone Type" DataField="phone_type" ReadOnly="true" />
                                    <asp:BoundField HeaderText="Phone OptIn" DataField="phone_optin" ReadOnly="true" />
                                    <asp:BoundField HeaderText="Phone 2" DataField="phone2" ReadOnly="true" />
                                    <asp:BoundField HeaderText="Phone 2 Type" DataField="phone2_type" ReadOnly="true" />
                                    <asp:BoundField HeaderText="Phone 2 OptIn" DataField="phone2_optin" ReadOnly="true" />
                                    <asp:BoundField HeaderText="Email 2" DataField="email2" ReadOnly="true" />
                                    <asp:BoundField HeaderText="Email Receipt" DataField="receipt_email" ReadOnly="true" />
                                </Fields>
                                <EmptyDataTemplate>
                                    No [Call Info] details;
                                </EmptyDataTemplate>
                            </asp:DetailsView>
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
                                <EmptyDataTemplate>
                                    No [Tribute] details;
                                </EmptyDataTemplate>
                            </asp:DetailsView>
                        </div>
                    </div>
                    <div class="clearfix">
                        <div style="display: inline-block;vertical-align: top;">
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
                                <EmptyDataTemplate>
                                    No [Contact] details;
                                </EmptyDataTemplate>
                            </asp:DetailsView>
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
                                    <asp:BoundField HeaderText="CreateDate" DataField="LoginDateTime" ReadOnly="true" />
                                    <asp:BoundField HeaderText="User" DataField="user" ReadOnly="true" />
                                    <asp:BoundField HeaderText="Reason" DataField="reason" ReadOnly="true" />
                                    <asp:BoundField HeaderText="Status" DataField="status" ReadOnly="true" />
                                    <asp:BoundField HeaderText="RefundDate" DataField="createdate" ReadOnly="true" />
                                    <asp:BoundField HeaderText="Decision" DataField="decision" ReadOnly="true" />
                                    <asp:BoundField HeaderText="Reference" DataField="merchantreferencecode" ReadOnly="true" />
                                    <asp:BoundField HeaderText="Response" DataField="ccContent" ReadOnly="true" />
                                </Fields>
                                <EmptyDataTemplate>
                                    No [Refund] details;
                                </EmptyDataTemplate>
                            </asp:DetailsView>
                        </div>
                        <div style="display: inline-block;vertical-align: top;">
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
                                <EmptyDataTemplate>
                                    No [ADU] files;
                                </EmptyDataTemplate>
                            </asp:DetailsView>
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
                                <EmptyDataTemplate>
                                    No [Remove] details;
                                </EmptyDataTemplate>
                            </asp:DetailsView>
                        </div>
                    </div>
                    <div>
                        <div>
                            <asp:Label ID="sqlPrint" runat="server" Text="" />
                        </div>
                    </div>
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
