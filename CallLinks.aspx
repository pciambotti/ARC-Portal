﻿<%@ Page Title="Call Links" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="CallLinks.aspx.cs" Inherits="CallLinks" %>
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
            <span style="font-weight: bold;">Active Calls</span>
            <br />List of active call records, oldest to newest
            <br /><span style="color: DarkRed;">Server time is UTC.</span>
            <hr />
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
                            <asp:Button ID="btnSearch" runat="server"
                                Text="Search"
                                OnClientClick="this.disabled = true; this.value = 'Running...';FixExport();" 
                                UseSubmitBehavior="false" 
                                OnClick="GridView_Refresh"
                                />
                            &nbsp;|&nbsp;&nbsp;&nbsp;&nbsp;
                            <span style="font-weight: bold;color: blue;">Click on "Link" column to open the agent script, anywhere else on the row to open the record.</span>
                            <asp:Button ID="btnExport" runat="server"
                                Text="Export to Excel"
                                OnClientClick="this.disabled = true; this.value = 'Running...';" 
                                UseSubmitBehavior="false" 
                                OnClick="GridView_Export_Excel"
                                Visible="false"
                                />
                            <div style="display: inline-block;">
                                <asp:Label ID="rpElapsed" runat="server" Text="" />
                            </div>
                            <div style="display: inline-block;">
                                <asp:Label ID="lblAllRecordConfirm" runat="server" Text="" />
                            </div>
                        </div>
                        <div id="grid" style="display: inline-block;width: 800px;vertical-align: top;">
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
                                DataKeyNames="companyid,interactionid"

                                OnDataBound="GridView_DataBound"
                                OnRowDataBound="GridView_RowDataBound"
                                OnSelectedIndexChanged="GridView_IndexChanged"
                                OnPageIndexChanging="GridView_PageIndexChanging"

                                CssClass="Portal_GridView_Standard"
                                Width="775"
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
                                    <asp:BoundField HeaderText="interactionid" DataField="interactionid" />
                                    <asp:BoundField HeaderText="author" DataField="fullname" />
                                    <asp:BoundField HeaderText="campaign_name" DataField="campaign" />
                                    <asp:TemplateField HeaderText="Started">
                                        <ItemTemplate>
                                            <asp:Label ID="call_createdate" runat="server" Text='<%# ghFunctions.date_label(Eval("datestart").ToString()) %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="DNIS">
                                        <ItemTemplate>
                                            <asp:Label ID="call_dnis" runat="server" Text='<%# get_Script_DNIS(DataBinder.GetPropertyValue(Container.DataItem, "destinator").ToString()) %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Age">
                                        <ItemTemplate>
                                            <asp:Label ID="age" runat="server" Text='<%# ghFunctions.SecondsTo(Eval("age").ToString()) %>' />
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
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
            <div id="search_details" style="display: inline-block;vertical-align: top;">
                <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                    <ContentTemplate>
                    <asp:Label Font-Bold="true" ID="dtlLabel" runat="server" Text="" />
                    <div style="width: 400px">
                        <asp:Label ID="Error_General" runat="server" Text="General Error Label" />
                        <div class="user_error_standard">
                        </div>
                        <div>
                            <div>
                                <asp:DetailsView ID="dvAgentScript" runat="server" DataKeyNames="five9_callid"
                                    AutoGenerateRows="False" ForeColor="Black" GridLines="Vertical"
                                    HeaderText="Agent Script Link"
                                    CssClass="User_List_Details_Border"

                                    Width="400px"
                                    >
                                    <HeaderStyle CssClass="User_List_Details_Header" />
                                    <RowStyle BackColor="#CCCCCC" CssClass="User_List_Details_Row" />
                                    <FieldHeaderStyle Width="100px" />
                                    <EmptyDataRowStyle BackColor="White" />
                                    <Fields>
                                        <asp:TemplateField HeaderText="Link" ItemStyle-CssClass="LastCol" ShowHeader="false">
                                            <ItemTemplate>
                                                <div style="text-align: center;">
                                                     <asp:HyperLink runat="server"
                                                         Text='<%#string.Format("{0} | {1}",DataBinder.GetPropertyValue(Container.DataItem, "interactionid"),DataBinder.GetPropertyValue(Container.DataItem, "five9_callid")) %>'
                                                         Target='_blank'
                                                         NavigateUrl='<%# string.Format(get_Script_URL() + "?ghsource=_AgentPOP&agent.station_id={0}&agent.first_agent={1}&agent.id={2}&agent.full_name={3}&agent.station_type={4}&agent.user_name={5}&call.ani={6}&call.bill_time={7}&call.call_id={8}&call.campaign_id={9}&call.campaign_name={10}&call.comments={11}&call.disposition_id={12}&call.disposition_name={13}&call.dnis={14}&call.end_timestamp={15}&call.handle_time={16}&call.hold_time={17}&call.length={18}&call.mediatype={19}&call.number={20}&call.park_time={21}&call.queue_time={22}&call.session_id={23}&call.skill_id={24}&call.skill_name={25}&call.start_timestamp={26}&call.tcpa_date_of_consent={27}&call.type_name={28}&call.type={29}&call.wrapup_time={30}",
                                                            (DataBinder.GetPropertyValue(Container.DataItem, "agent_stationid").ToString())
                                                            ,(DataBinder.GetPropertyValue(Container.DataItem, "agent_five9id").ToString())
                                                            ,(DataBinder.GetPropertyValue(Container.DataItem, "agent_five9id").ToString())
                                                            ,(DataBinder.GetPropertyValue(Container.DataItem, "agent_fullname").ToString())
                                                            ,(DataBinder.GetPropertyValue(Container.DataItem, "agent_stationtype").ToString())
                                                            ,(DataBinder.GetPropertyValue(Container.DataItem, "agent_username").ToString())
                                                            ,(DataBinder.GetPropertyValue(Container.DataItem, "originator").ToString())
                                                            ,(DataBinder.GetPropertyValue(Container.DataItem, "bill_time").ToString())
                                                            ,(DataBinder.GetPropertyValue(Container.DataItem, "five9_callid").ToString())
                                                            ,(DataBinder.GetPropertyValue(Container.DataItem, "five9_campaignid").ToString())
                                                            ,(DataBinder.GetPropertyValue(Container.DataItem, "five9_campaign").ToString())
                                                            ,(DataBinder.GetPropertyValue(Container.DataItem, "comments").ToString())
                                                            ,(DataBinder.GetPropertyValue(Container.DataItem, "five9_dispositionid").ToString())
                                                            ,(DataBinder.GetPropertyValue(Container.DataItem, "five9_disposition").ToString())
                                                            ,(get_URL_DNIS(DataBinder.GetPropertyValue(Container.DataItem, "destinator").ToString()))                                                    
                                                            ,(DataBinder.GetPropertyValue(Container.DataItem, "dateend").ToString())
                                                            ,(DataBinder.GetPropertyValue(Container.DataItem, "handle_time").ToString())
                                                            ,(DataBinder.GetPropertyValue(Container.DataItem, "hold_time").ToString())
                                                            ,(DataBinder.GetPropertyValue(Container.DataItem, "length").ToString())
                                                            ,(DataBinder.GetPropertyValue(Container.DataItem, "five9_mediatype").ToString())
                                                            ,(DataBinder.GetPropertyValue(Container.DataItem, "originator").ToString())
                                                            ,(DataBinder.GetPropertyValue(Container.DataItem, "park_time").ToString())
                                                            ,(DataBinder.GetPropertyValue(Container.DataItem, "queue_time").ToString())
                                                            ,(DataBinder.GetPropertyValue(Container.DataItem, "sessionid").ToString())
                                                            ,(DataBinder.GetPropertyValue(Container.DataItem, "five9_skillid").ToString())
                                                            ,(DataBinder.GetPropertyValue(Container.DataItem, "five9_skill").ToString())
                                                            ,(DataBinder.GetPropertyValue(Container.DataItem, "datestart").ToString())
                                                            ,(DataBinder.GetPropertyValue(Container.DataItem, "tcpa_date_of_consent").ToString())
                                                            ,(DataBinder.GetPropertyValue(Container.DataItem, "five9_type").ToString())
                                                            ,(DataBinder.GetPropertyValue(Container.DataItem, "five9_typeid").ToString())
                                                            ,(DataBinder.GetPropertyValue(Container.DataItem, "wrapup_time").ToString())
                                                             )
                                                            %>'
                                                         />
                                                </div>
                                                <div>
                                                    DNIS: <%# DataBinder.GetPropertyValue(Container.DataItem, "destinator").ToString() %>
                                                    <br />DNIS: <%# get_URL_DNIS(DataBinder.GetPropertyValue(Container.DataItem, "destinator").ToString()) %>
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Fields>
                                    <EmptyDataTemplate>No [Interaction] details;</EmptyDataTemplate>
                                </asp:DetailsView>
                            </div>
                            <div>
                                <asp:DetailsView ID="dvInteraction" runat="server" DataKeyNames="arc_callid"
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
                            <div>
                                <asp:DetailsView ID="dvInteractionDetails" runat="server" DataKeyNames="arc_callid"
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
