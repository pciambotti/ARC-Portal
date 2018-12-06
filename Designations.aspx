<%@ Page Title="Designations" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Designations.aspx.cs" Inherits="Designations" MaintainScrollPositionOnPostback="true" %>
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
            <span style="font-weight: bold;">Designations</span>
            <br />Designation report and admin
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
            <div id="designation_grid" style="display: inline-block;vertical-align: top;">
                <asp:UpdatePanel ID="upDesignationGrid" runat="server">
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
                        <div id="grid" style="display: inline-block;width: 515px;vertical-align: top;">
                            <asp:GridView ID="gvDesignationList" runat="server" AutoGenerateColumns="False"
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
                                DataKeyNames="designationid"

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
                                    <asp:BoundField HeaderText="ID" DataField="designationid" />
                                    <asp:BoundField HeaderText="Display Name" DataField="displayname" />
                                    <asp:BoundField HeaderText="pagelocationid" DataField="pagelocationid" Visible="false" />
                                    <asp:BoundField HeaderText="merchantid" DataField="merchantid" Visible="false" />
                                    <asp:BoundField HeaderText="Fund" DataField="fundcode" />
                                        <asp:TemplateField HeaderText="Status">
                                            <ItemTemplate>
                                                <asp:Label ID="status" runat="server" Text='<%# (Eval("status").ToString() == "A") ? "Active" : "De-active"  %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    <asp:BoundField HeaderText="Sort" DataField="sort" Visible="true" />
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
            <div id="designation_details" style="display: inline-block;vertical-align: top;">
                <asp:UpdatePanel ID="upDesignationDetails" runat="server">
                    <ContentTemplate>
                        <asp:Label Font-Bold="true" ID="dtlLabel" runat="server" Text="" />
                        <div>
                            <asp:Button ID="btnDesignationAdd" runat="server" Text="Add New Designation" OnClick="Designation_Add_Click" Visible="false" />
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
                                <asp:DetailsView ID="dvDesignationDetails" runat="server" DataKeyNames="designationid"
                                    AutoGenerateRows="False" ForeColor="Black" GridLines="Vertical"
                                    HeaderText="Designation Details"
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
                                        <asp:TemplateField HeaderText="ID">
                                            <ItemTemplate>
                                                <asp:Label ID="designationid" runat="server" Text='<%# Eval("designationid") %>' />
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:Label ID="designationid" runat="server" Text='<%# Eval("designationid") %>' />
                                            </EditItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Old Name">
                                            <ItemTemplate>
                                                <asp:Label ID="displayname" runat="server" Text='<%# Eval("displayname") %>' />
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:HiddenField ID="displayname_crnt" runat="server" Value='<%# Eval("displayname") %>' />
                                                <asp:Textbox ID="displayname" runat="server" Text='<%# Eval("displayname") %>' TextMode="MultiLine" Height="25px" Width="375px" />
                                                <asp:RequiredFieldValidator
                                                    ControlToValidate="displayname"
                                                    Text="*"
                                                    SetFocusOnError="false"
                                                    ErrorMessage="Old Name is required"
                                                    InitialValue=""
                                                    ValidationGroup="vgDesignationInsert"
                                                    ID="rfv_displayname"
                                                    runat="server"
                                                    />
                                                <br />Old Name should be all CAPs for consistency
                                            </EditItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Sort">
                                            <ItemTemplate>
                                                <asp:Label ID="sort" runat="server" Text='<%# Eval("sort") %>' />
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:HiddenField ID="sort_crnt" runat="server" Value='<%# Eval("sort") %>' />
                                                <asp:Textbox ID="sort" runat="server" Text='<%# Eval("sort") %>' Width="25px" MaxLength="3" />
                                                <asp:RequiredFieldValidator
                                                    ControlToValidate="sort"
                                                    Text="*"
                                                    SetFocusOnError="false"
                                                    ErrorMessage="sort is required"
                                                    InitialValue=""
                                                    ValidationGroup="vgDesignationInsert"
                                                    ID="rfv_sort"
                                                    runat="server"
                                                    />
                                                <asp:RegularExpressionValidator
                                                    ControlToValidate="sort"
                                                    Text="*"
                                                    SetFocusOnError="false"
                                                    ErrorMessage="sort must be 1-3 digits long"
                                                    ValidationExpression="^.*[0-9]{1,3}$"
                                                    ValidationGroup="vgDesignationInsert"
                                                    ID="rev_sort"
                                                    runat="server"
                                                 />
                                                Default value should be higher than current designations
                                            </EditItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Fund Code">
                                            <ItemTemplate>
                                                <asp:Label ID="fundcode" runat="server" Text='<%# Eval("fundcode") %>' />
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:HiddenField ID="fundcode_crnt" runat="server" Value='<%# Eval("fundcode") %>' />
                                                <asp:Textbox ID="fundcode" runat="server" Text='<%# Eval("fundcode") %>' Width="50px" Enabled="false" MaxLength="7" />
                                                <div style="display: inline;margin-left: 5px;">
                                                    Only IT may change
                                                </div>
                                            </EditItemTemplate>
                                            <InsertItemTemplate>
                                                <asp:Textbox ID="fundcode" runat="server" Text='<%# Eval("fundcode") %>' Width="50px" MaxLength="7" />
                                                <asp:RequiredFieldValidator
                                                    ControlToValidate="fundcode"
                                                    Text="*"
                                                    SetFocusOnError="false"
                                                    ErrorMessage="fundcode is required"
                                                    InitialValue=""
                                                    ValidationGroup="vgDesignationInsert"
                                                    ID="rfv_fundcode"
                                                    runat="server"
                                                    />
                                                <asp:RegularExpressionValidator
                                                    ControlToValidate="fundcode"
                                                    Text="*"
                                                    SetFocusOnError="false"
                                                    ErrorMessage="fundcode must be 4-7 digits long"
                                                    ValidationExpression="^.*[0-9-]{4,7}$"
                                                    ValidationGroup="vgDesignationInsert"
                                                    ID="rev_fundcode"
                                                    runat="server"
                                                 />
                                                Also known as AP Code - 4 digits or 6 with a dash
                                            </InsertItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="ARC Status">
                                            <ItemTemplate>
                                                <asp:Label ID="status" runat="server" Text='<%# (Eval("status").ToString() == "A") ? "Active" : "De-active"  %>' />
                                                <div>
                                                    Indicates the status is active or not within ARC
                                                </div>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:HiddenField ID="status_crnt" runat="server" Value='<%# Eval("status") %>' />
                                                <asp:DropDownList ID="status" runat="server" SelectedValue='<%# Eval("status") %>'>
                                                    <asp:ListItem Value="A" Text="Active" />
                                                    <asp:ListItem Value="D" Text="De-activate" />
                                                    <asp:ListItem Value="" Text="Other" />
                                                </asp:DropDownList>
                                            </EditItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Show Online">
                                            <ItemTemplate>
                                                <asp:CheckBox ID="status_online" runat="server" Checked='<%# eval_checkbox_checked(Eval("status_online")) %>' Enabled="false" />
                                                <span style="margin-left: 5px;">
                                                    Designation shows up in the Agent Script or not
                                                </span>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:HiddenField ID="status_online_crnt" runat="server" Value='<%# Eval("status_online") %>' />
                                                <asp:CheckBox ID="status_online" runat="server" Checked='<%# eval_checkbox_checked(Eval("status_online")) %>' />
                                                <span style="margin-left: 5px;">
                                                    Designation shows up in the Agent Script or not
                                                </span>
                                                <br />If the designation is specific to 1 or more DNIS leave this unchecked
                                            </EditItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Pull ADU">
                                            <ItemTemplate>
                                                <asp:CheckBox ID="status_adu" runat="server" Checked='<%# eval_checkbox_checked(Eval("status_adu")) %>' Enabled="false" />
                                                <span style="margin-left: 5px;">
                                                    Records with this designation are pulled for ADU or not
                                                </span>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:HiddenField ID="status_adu_crnt" runat="server" Value='<%# Eval("status_adu") %>' />
                                                <asp:CheckBox ID="status_adu" runat="server" Checked='<%# eval_checkbox_checked(Eval("status_adu")) %>' Enabled="false" />
                                                <div style="display: inline;margin-left: 5px;">
                                                    Records with this designation are pulled for ADU or not
                                                    <br />Only IT may change
                                                </div>
                                            </EditItemTemplate>
                                            <InsertItemTemplate>
                                                <asp:CheckBox ID="status_adu" runat="server" Checked='<%# eval_checkbox_checked(Eval("status_adu")) %>' />
                                                <div style="display: inline;margin-left: 5px;">
                                                    Records with this designation are pulled for ADU or not
                                                </div>
                                            </InsertItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Name in Script">
                                            <ItemTemplate>
                                                <asp:Label ID="name" runat="server" Text='<%# Eval("name") %>' />
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:HiddenField ID="name_crnt" runat="server" Value='<%# Eval("name") %>' />
                                                <asp:Textbox ID="name" runat="server" Text='<%# Eval("name") %>' TextMode="MultiLine" Height="35px" Width="375px" />
                                                <asp:RequiredFieldValidator
                                                    ControlToValidate="name"
                                                    Text="*"
                                                    SetFocusOnError="false"
                                                    ErrorMessage="name is required"
                                                    InitialValue=""
                                                    ValidationGroup="vgDesignationInsert"
                                                    ID="rfv_name"
                                                    runat="server"
                                                    />
                                            </EditItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Name in Script (sp)">
                                            <ItemTemplate>
                                                <asp:Label ID="name_spanish" runat="server" Text='<%# Eval("name_spanish") %>' />
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:HiddenField ID="name_spanish_crnt" runat="server" Value='<%# Eval("name_spanish") %>' />
                                                <asp:Textbox ID="name_spanish" runat="server" Text='<%# Eval("name_spanish") %>' TextMode="MultiLine" Height="35px" Width="375px" />
                                                <asp:RequiredFieldValidator
                                                    ControlToValidate="name_spanish"
                                                    Text="*"
                                                    SetFocusOnError="false"
                                                    ErrorMessage="name_spanish is required"
                                                    InitialValue=""
                                                    ValidationGroup="vgDesignationInsert"
                                                    ID="rfv_name_spanish"
                                                    runat="server"
                                                    />
                                            </EditItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Script Continue">
                                            <ItemTemplate>
                                                <asp:Label ID="continue" runat="server" Text='<%# Eval("continue") %>' />
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:HiddenField ID="continue_crnt" runat="server" Value='<%# Eval("continue") %>' />
                                                <asp:Textbox ID="continue" runat="server" Text='<%# Eval("continue") %>' Width="50px" />
                                                <asp:RequiredFieldValidator
                                                    ControlToValidate="continue"
                                                    Text="*"
                                                    SetFocusOnError="false"
                                                    ErrorMessage="continue is required"
                                                    InitialValue=""
                                                    ValidationGroup="vgDesignationInsert"
                                                    ID="rfv_continue"
                                                    runat="server"
                                                    />
                                                Default value is A4
                                            </EditItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Description in Script">
                                            <ItemTemplate>
                                                <asp:Label ID="description" runat="server" Text='<%# Eval("description") %>' />
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:HiddenField ID="description_crnt" runat="server" Value='<%# Eval("description") %>' />
                                                <asp:Textbox ID="description" runat="server" Text='<%# Eval("description") %>' TextMode="MultiLine" Height="50px" Width="375px" />
                                                <asp:RequiredFieldValidator
                                                    ControlToValidate="description"
                                                    Text="*"
                                                    SetFocusOnError="false"
                                                    ErrorMessage="description is required"
                                                    InitialValue=""
                                                    ValidationGroup="vgDesignationInsert"
                                                    ID="rfv_description"
                                                    runat="server"
                                                    />
                                            </EditItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Description in Script (sp)">
                                            <ItemTemplate>
                                                <asp:Label ID="description_spanish" runat="server" Text='<%# Eval("description_spanish") %>' />
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:HiddenField ID="description_spanish_crnt" runat="server" Value='<%# Eval("description_spanish") %>' />
                                                <asp:Textbox ID="description_spanish" runat="server" Text='<%# Eval("description_spanish") %>' TextMode="MultiLine" Height="50px" Width="375px" />
                                                <asp:RequiredFieldValidator
                                                    ControlToValidate="description_spanish"
                                                    Text="*"
                                                    SetFocusOnError="false"
                                                    ErrorMessage="description_spanish is required"
                                                    InitialValue=""
                                                    ValidationGroup="vgDesignationInsert"
                                                    ID="rfv_description_spanish"
                                                    runat="server"
                                                    />
                                            </EditItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Agent Note (top)">
                                            <ItemTemplate>
                                                <asp:Label ID="agentnote_top" runat="server" Text='<%# Eval("agentnote_top") %>' />
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:HiddenField ID="agentnote_top_crnt" runat="server" Value='<%# Eval("agentnote_top") %>' />
                                                <asp:Textbox ID="agentnote_top" runat="server" Text='<%# Eval("agentnote_top") %>' TextMode="MultiLine" Height="50px" Width="375px" />
                                            </EditItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Agent Note (bottom)">
                                            <ItemTemplate>
                                                <asp:Label ID="agentnote_bottom" runat="server" Text='<%# Eval("agentnote_bottom") %>' />
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:HiddenField ID="agentnote_bottom_crnt" runat="server" Value='<%# Eval("agentnote_bottom") %>' />
                                                <asp:Textbox ID="agentnote_bottom" runat="server" Text='<%# Eval("agentnote_bottom") %>' TextMode="MultiLine" Height="50px" Width="375px" />
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
                                                <asp:Button ID="Button4" runat="server" CommandName="Insert" Text="Insert" ValidationGroup="vgDesignationInsert"  />
                                                <asp:Button ID="cancel" runat="server" Text="Cancel" CommandName="Cancel" />
                                                <div>
                                                    <asp:ValidationSummary ID="vsDesignationInsert" ValidationGroup="vgDesignationInsert" HeaderText="Validation Failed:" CssClass="portal_validation_summary"  runat="server" />
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
