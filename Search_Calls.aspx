<%@ Page Title="Search Calls" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Search_Calls.aspx.cs" Inherits="Search_Calls" %>
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
            //GH_DatePicker();
            GH_DatePickerToday();
            $("audio").mediaelementplayer({
                alwaysShowControls: true
            });
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
    <script type="text/javascript">
        $(function () {
        });
    </script>
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
                <div id="filter_options" class="filter_section">
                    <h3>Options</h3>
                    <div id="donation_max">
                        <asp:Label ID="Label13" runat="server" Text="Max Records" CssClass="filter_label1" />
                        <asp:DropDownList ID="ddlTop" runat="server" Width="60" Width2="60">
                            <asp:ListItem Text="All" Value="1000" />
                            <asp:ListItem Text="5" Value="5" Selected="True" />
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
                                $('#<%= btnRefresh.ClientID  %> ').find("[value=0]").attr("disabled", "disabled");
                                $('#<%= btnRefresh.ClientID  %> ').find("[value=0]").attr("aria-disabled", "true");
                            }
                        }
                    </script>
                    <div>
                        <%--Filter_Toggle_Hide(btnFilters01,'reporting_filters','All Filters');--%>
                        <asp:Button ID="btnRefresh" runat="server"
                            Text="Search"
                            OnClientClick="this.disabled = true; this.value = 'Running...';" 
                            UseSubmitBehavior="false" 
                            OnClick="GridView_Refresh"
                            />
                        <div style="display: inline-block;margin-left: 15px;">
                            <asp:Button ID="btnExport" runat="server" Text="Export to Excel" OnClick="Custom_Export_Excel_SearchGrid" Visible="false" />
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
                            DataKeyNames="companyid,interactionid"

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
                                <asp:BoundField HeaderText="companyid" DataField="companyid" />
                                <asp:BoundField HeaderText="interactionid" DataField="interactionid" />
                                <asp:BoundField HeaderText="interactiontype" DataField="interactiontype" />
                                <asp:BoundField HeaderText="call.call_id" DataField="call.call_id" />
                                <asp:BoundField HeaderText="call.campaign_name" DataField="call.campaign_name" />
                                <asp:BoundField HeaderText="call.skill_name" DataField="call.skill_name" />
                                <asp:BoundField HeaderText="call.length" DataField="call.length" />
                                <asp:BoundField HeaderText="agent.full_name" DataField="agent.full_name" />
                                <asp:BoundField HeaderText="call.disposition" DataField="call.disposition_name"  />
                                <asp:BoundField HeaderText="ia.callid" DataField="ia.callid" />
                                <asp:BoundField HeaderText="arc.disposition" DataField="arc.disposition_name" />
                                <asp:TemplateField HeaderText="recordings">
                                    <ItemTemplate>
                                        <asp:Label ID="recordings" runat="server" Text='0' />
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="created" ItemStyle-CssClass="LastCol" >
                                    <ItemTemplate>
                                        <asp:Label ID="createdate" runat="server" Text='<%# ghFunctions.date_label(Eval("createdate").ToString()) %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:CommandField SelectText="Select" ShowSelectButton="true" ItemStyle-CssClass="HideButton" HeaderStyle-CssClass="HideButton" />
                            </Columns>
                            <EmptyDataTemplate>No Records For Selected Filters</EmptyDataTemplate>
                        </asp:GridView>
                        <asp:Label ID="lblSearchGrid" runat="server" Text="Label" />
                        <asp:Label ID="lblRecordings" runat="server" Text="Label" />
                        <asp:Label ID="lblCache" runat="server" Text="Label" />
                        <asp:GridView ID="gvSearchExport" runat="server" AutoGenerateColumns="False" Visible="false">
                            <Columns>
                                <asp:BoundField HeaderText="companyid" DataField="companyid" />
                                <asp:BoundField HeaderText="interactionid" DataField="interactionid" />
                                <asp:BoundField HeaderText="interactiontype" DataField="interactiontype" />
                                <asp:BoundField HeaderText="call.call_id" DataField="call.call_id" />
                                <asp:BoundField HeaderText="call.campaign_name" DataField="call.campaign_name" />
                                <asp:BoundField HeaderText="call.skill_name" DataField="call.skill_name" />
                                <asp:BoundField HeaderText="call.length" DataField="call.length" />
                                <asp:BoundField HeaderText="fcd.call_id" DataField="fcd.call_id" />
                                <asp:BoundField HeaderText="agent.full_name" DataField="agent.full_name" />
                                <asp:BoundField HeaderText="call.disposition_name" DataField="call.disposition_name"  />
                                <asp:BoundField HeaderText="ia.callid" DataField="ia.callid" />
                                <asp:BoundField HeaderText="arc.disposition_name" DataField="arc.disposition_name" />
                                <asp:BoundField HeaderText="createdate" DataField="createdate"  />
                            </Columns>
                        </asp:GridView>
                    </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
            <div id="search_details" style="display: block;vertical-align: top;" class="clearfix">
                <div style="float: left;width: 803px;">
                    <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                        <Triggers>
                            <asp:PostBackTrigger ControlID="btnExportDetails" />
                        </Triggers>
                        <ContentTemplate>
                            <div>
                                <asp:Button ID="btnExportDetails" runat="server" Text="Export to Excel" OnClick="Custom_Export_Excel_Details" Visible="false" />
                                <div>
                                    <asp:Label ID="lblErrorDV" runat="server" Text="" ForeColor="DarkRed" Font-Bold="true" />
                                </div>
                            </div>
                            <div class="clearfix">
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
                                                                <source src='<%# Eval("RecordingURL") %>' type="audio/mp3" />
                                                                Browser Compatibility Issue
                                                            </audio>
                                                        </td>
                                                        <td style="border-color: transparent;">
                                                            <div class="audio_link">
                                                                <a class="" href="<%# Eval("RecordingURL") %>">Download</a>
                                                            </div>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Date" ItemStyle-CssClass="LastCol" >
                                            <ItemTemplate>
                                                <asp:Label ID="DateCreated" runat="server" Text='<%# ghFunctions.date_label(Eval("DateCreated").ToString()) %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:CommandField SelectText="Select" ShowSelectButton="true" ItemStyle-CssClass="HideButton" HeaderStyle-CssClass="HideButton" />
                                    </Columns>
                                </asp:GridView>
                            </div>
                            <div class="clearfix">
                                <div style="display: inline-block;vertical-align: top;margin-top: 5px;">
                                    <asp:DetailsView ID="dvInteraction" runat="server" DataKeyNames="companyid,interactionid"
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
                                        </Fields>
                                        <EmptyDataTemplate>No [Interaction] details;</EmptyDataTemplate>
                                    </asp:DetailsView>
                                </div>
                                <div style="display: inline-block;vertical-align: top;margin-top: 5px;">
                                    <asp:DetailsView ID="dvInteractionARC" runat="server" DataKeyNames="companyid,interactionid"
                                        AutoGenerateRows="True" ForeColor="Black" GridLines="Vertical"
                                        HeaderText="ARC Interaction"
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
                            </div>
                            <div class="clearfix">
                                <div style="display: inline-block;vertical-align: top;margin-top: 5px;">
                                    <asp:DetailsView ID="dvFive9Calls" runat="server" DataKeyNames="companyid,interactionid"
                                        AutoGenerateRows="True" ForeColor="Black" GridLines="Vertical"
                                        HeaderText="Five9 Call"
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
                                    <%--Should be a GridView - get the template from Search.aspx--%>
                                    <asp:DetailsView ID="dvFive9CallsDisposition" runat="server" DataKeyNames="companyid,interactionid"
                                        AutoGenerateRows="True" ForeColor="Black" GridLines="Vertical"
                                        HeaderText="Five9 Call Disposition"
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
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
                <div style="float: left;margin-left: 5px;" class="admin_section">
                    <asp:UpdatePanel ID="UpdatePanel4" runat="server">
                        <ContentTemplate>
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
                    <EmptyDataTemplate>No Errors to Report;</EmptyDataTemplate>
                </asp:DetailsView>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">
</asp:Content>
