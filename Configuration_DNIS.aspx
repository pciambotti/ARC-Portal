<%@ Page Title="Configuration DNIS" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Configuration_DNIS.aspx.cs" Inherits="Configuration_DNIS" MaintainScrollPositionOnPostback="true" %>
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
    <style type="text/css">
        .User_List_Details_Row td
        {
            vertical-align: top;
            padding-bottom: 5px;
        }
        .portal_validation_summary
        {
            color: darkred;
        	font-weight: bold;
        }
        .portal_validation_summary ul
        {
        	font-weight: normal;
        	margin-left: 15px;
        }
    </style>
<script type="text/javascript">
    window.scrollTo = function( x,y ) 
    {
        /* This prevents scroll up, however MaintainScroll also works... */
        /*return true;*/
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
            <span style="font-weight: bold;">DNIS Configuration</span>
            <br />DNIS admin
            <hr />
        </div>
        <div id="report_control1">
            <input id="btnFilters01" type="button" value="Hide All Filters" onclick="Filter_Toggle(this,'reporting_filters','All Filters');" />
        </div>
        <div id="reporting_filters" class="reporting_filters">
            <div class="clearfix">
                <div id="filter_status" class="filter_section">
                    <h3>Status</h3>
                    <div id="call_type">
                        <asp:Label ID="lblStatus" runat="server" Text="Status" CssClass="filter_label1" />
                        <asp:DropDownList ID="ddlStatus" runat="server" Width="128px" Width2="250px">
                            <asp:ListItem Value="A" Text="Active" />
                            <asp:ListItem Value="D" Text="De-active" />
                            <asp:ListItem Value="" Text="All" />
                        </asp:DropDownList>
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
            </div>
            <div style="color: Blue;">
                All filter options work together; you can clear any text or date fields to remove them from the query.
                <br /><br />
            </div>
        </div>
        <div id="search_result" class="clearfix">
            <div id="dnis_grid" style="display: inline-block;vertical-align: top;">
                <asp:UpdatePanel ID="upDNISGrid" runat="server">
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
                            <asp:Button ID="btnRefresh" runat="server"
                                Text="Refresh List"
                                OnClientClick="this.disabled = true; this.value = 'Running...';FixExport();" 
                                UseSubmitBehavior="false" 
                                OnClick="Refresh_Button"
                                />
                            &nbsp;|&nbsp;&nbsp;&nbsp;&nbsp;
                            <asp:Button ID="btnExport" runat="server"
                                Text="Export to Excel"
                                OnClientClick="this.disabled = true; this.value = 'Running...';" 
                                UseSubmitBehavior="false" 
                                Visible="false"
                                />
                            <div style="display: inline-block;">
                                <asp:Label ID="rpElapsed" runat="server" Text="" />
                            </div>
                            <div style="display: inline-block;">
                                <asp:Label ID="lblAllRecordConfirm" runat="server" Text="" />
                            </div>
                        </div>
                        <div id="grid" style="display: inline-block;width: 615px;vertical-align: top;">
                            <asp:GridView ID="gvDNISList" runat="server" AutoGenerateColumns="False"
                                BackColor="White"
                                BorderStyle="None"
                                BorderWidth="1px"
                                BorderColor="#DEDFDE"
                                AllowSorting="True"
                                ForeColor="Black"
                                GridLines="Vertical"
                                CellPadding="20"
                                CellSpacing="20"
                                AllowPaging="true" PageSize="100" DataKeyNames="phonenumber"

                                OnDataBound="GridView_DataBound"
                                OnRowDataBound="GridView_RowDataBound"
                                OnSelectedIndexChanged="GridView_IndexChanged"
                                OnPageIndexChanging="GridView_PageIndexChanging"

                                CssClass="Portal_GridView_Standard"
                                Width="610"
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
                                    <asp:TemplateField HeaderText="DNIS">
                                        <ItemTemplate>
                                            <asp:Label ID="Line" runat="server" Text='<%# Eval("line").ToString() %>' />
                                            <asp:Label ID="BR" runat="server" Text='<%# Eval("line").ToString().Length > 0 ? "<br />" : "" %>' />
                                            <asp:Label ID="Company" runat="server" Text='<%# Eval("company").ToString().Length > 0 ? "" + Eval("company").ToString() : "" %>' ForeColor="Blue" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField HeaderText="City" DataField="city" Visible="false" />
                                    <asp:BoundField HeaderText="DNIS" DataField="dnis" Visible="false" />
                                    <asp:BoundField HeaderText="PhoneNumber" DataField="phonenumber" Visible="false" />
                                    <asp:TemplateField HeaderText="Number">
                                        <ItemTemplate>
                                            <asp:Label ID="phonenumber" runat="server" Text='<%# Eval("phonenumber").ToString() %>' />
                                            <asp:Label ID="dnis" runat="server" Text='<%# Eval("dnis").ToString().Length > 0 ? "<br />" + Eval("dnis").ToString() : "" %>' ForeColor="Blue" />
                                            <asp:Label ID="city" runat="server" Text='<%# Eval("city").ToString().Length > 0 ? "<br />" + Eval("city").ToString() : "" %>' ForeColor="DarkRed" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Language">
                                        <ItemTemplate>
                                            <asp:Label ID="languageid" runat="server" Text='<%# Eval("languageid").ToString() == "0" ? "English" : "Spanish" %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Status">
                                        <ItemTemplate>
                                            <asp:Label ID="isactive" runat="server" Text='<%# Eval("isactive").ToString() == "1" ? "Active" : "Disabled" %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="SourceCode">
                                        <ItemTemplate>
                                            <asp:Label ID="SourceCode_OneTime" runat="server" Text='<%# Eval("sourcecode_onetime").ToString().Length > 0 ? "One Time: " + Eval("sourcecode_onetime").ToString() : "" %>' />
                                            <asp:Label ID="SourceCode_Sustainer" runat="server" Text='<%# Eval("sourcecode_sustainer").ToString().Length > 0 ? "<br />Sustainer: " + Eval("sourcecode_sustainer").ToString() : "" %>' />
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
            <div id="dnis_details" style="display: inline-block;vertical-align: top;">
                <asp:UpdatePanel ID="upDNISDetails" runat="server">
                    <ContentTemplate>
                        <asp:Label Font-Bold="true" ID="dtlLabel" runat="server" Text="" />
                        <div>
                            <asp:Button ID="btnDNISAdd" runat="server" Text="Add New DNIS" OnClick="DNIS_Add_Click" Visible="false" />
                        </div>
                        <div style="width: 575px">
                            <div>
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

                            </div>
                            <div style="margin-top: 5px;">
                                <asp:DetailsView ID="dvDNISDetails" runat="server"
                                    AutoGenerateRows="False" ForeColor="Black" GridLines="Vertical"
                                    HeaderText="DNIS Details"
                                    CssClass="User_List_Details_Border"
                                    Width="550px"

                                    OnDataBound="DetailsView_DataBound"
                                    OnItemCommand="DetailsView_ItemCommand"
                                    OnModeChanging="DetailsView_ModeChanging"
                                    OnItemUpdating="DetailsView_ItemUpdating"
                                    OnItemUpdated="DetailsView_ItemUpdated"
                                    OnItemInserting="DetailsView_ItemInserting"
                                    OnItemInserted="DetailsView_ItemInserted"
                                    >
                                    <HeaderStyle CssClass="User_List_Details_Header" />
                                    <RowStyle BackColor="#CCCCCC" CssClass="User_List_Details_Row" />
                                    <FieldHeaderStyle Width="150px" />
                                    <EmptyDataRowStyle BackColor="White" />
                                    <Fields>
                                        <asp:TemplateField HeaderText="Line">
                                            <ItemTemplate>
                                                <asp:Label ID="line" runat="server" Text='<%# Eval("line") %>' />
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:Label ID="line" runat="server" Text='<%# Eval("line") %>' />
                                            </EditItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Company">
                                            <ItemTemplate>
                                                <asp:Label ID="company" runat="server" Text='<%# Eval("company") %>' />
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:Label ID="company" runat="server" Text='<%# Eval("comapny") %>' />
                                            </EditItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="City">
                                            <ItemTemplate>
                                                <asp:Label ID="city" runat="server" Text='<%# Eval("city") %>' />
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:Label ID="city" runat="server" Text='<%# Eval("city") %>' />
                                            </EditItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="State">
                                            <ItemTemplate>
                                                <asp:Label ID="state" runat="server" Text='<%# Eval("state") %>' />
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:Label ID="state" runat="server" Text='<%# Eval("state") %>' />
                                            </EditItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Type">
                                            <ItemTemplate>
                                                <asp:Label ID="type" runat="server" Text='<%# Eval("type") %>' />
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:Label ID="type" runat="server" Text='<%# Eval("type") %>' />
                                            </EditItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="DNIS">
                                            <ItemTemplate>
                                                <asp:Label ID="dnis" runat="server" Text='<%# Eval("dnis") %>' />
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:Label ID="dnis" runat="server" Text='<%# Eval("dnis") %>' />
                                            </EditItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="PhoneNumber">
                                            <ItemTemplate>
                                                <asp:Label ID="phonenumber" runat="server" Text='<%# Eval("phonenumber") %>' />
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:Label ID="phonenumber" runat="server" Text='<%# Eval("phonenumber") %>' />
                                            </EditItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Language">
                                            <ItemTemplate>
                                                <asp:Label ID="languageid" runat="server" Text='<%# Eval("languageid") %>' />
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:Label ID="languageid" runat="server" Text='<%# Eval("languageid") %>' />
                                            </EditItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Active">
                                            <ItemTemplate>
                                                <asp:Label ID="isactive" runat="server" Text='<%# Eval("isactive") %>' />
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:Label ID="isactive" runat="server" Text='<%# Eval("isactive") %>' />
                                            </EditItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="OneTime">
                                            <ItemTemplate>
                                                <asp:Label ID="sourcecode_onetime" runat="server" Text='<%# Eval("sourcecode_onetime") %>' />
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:Label ID="sourcecode_onetime" runat="server" Text='<%# Eval("sourcecode_onetime") %>' />
                                            </EditItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Sustainer">
                                            <ItemTemplate>
                                                <asp:Label ID="sourcecode_sustainer" runat="server" Text='<%# Eval("sourcecode_sustainer") %>' />
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:Label ID="sourcecode_sustainer" runat="server" Text='<%# Eval("sourcecode_sustainer") %>' />
                                            </EditItemTemplate>
                                        </asp:TemplateField>


                                        <asp:TemplateField HeaderText="">
                                            <ItemTemplate>
                                                <div style="width: 300px;margin: 5px 0px;">
                                                    <asp:Button ID="modify" runat="server" Text="Modify" CommandName="Edit" Visible="false" />
                                                    <asp:Button ID="clear" runat="server" Text="Clear Selected" CommandName="Clear" />
                                                </div>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <div style="width: 300px;margin: 5px 0px;">
                                                    <asp:Button ID="update" runat="server" Text="Save" CommandName="Update" Enabled="false" />
                                                    <asp:Button ID="cancel" runat="server" Text="Cancel" CommandName="Cancel" />
                                                </div>
                                            </EditItemTemplate>
                                            <InsertItemTemplate>
                                                <asp:Button ID="Button4" runat="server" CommandName="Insert" Text="Insert" ValidationGroup="vgDNISInsert"  />
                                                <asp:Button ID="cancel" runat="server" Text="Cancel" CommandName="Cancel" />
                                                <div>
                                                    <asp:ValidationSummary ID="vsDNISInsert" ValidationGroup="vgDNISInsert" HeaderText="Validation Failed:" CssClass="portal_validation_summary"  runat="server" />
                                                </div>
                                            </InsertItemTemplate>
                                        </asp:TemplateField>
                                    </Fields>
                                    <EmptyDataTemplate>No [Payment] details;</EmptyDataTemplate>
                                </asp:DetailsView>
                            </div>

                            <div class="user_error_standard">
                                <asp:Label ID="lblItemMsg" runat="server" Text="" />
                                <asp:Label ID="lblErrorMsg" runat="server" Text="" />
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
                        <asp:Label ID="msgResults" runat="server" Text="" />
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
                <asp:Label ID="msgDebug" runat="server" Text="" />
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
