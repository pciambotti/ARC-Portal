<%@ Page Title="IVR Search Full" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="search_ivr_full.aspx.cs" Inherits="search_ivr_full" %>
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
    <link href="js/jquery.multiselect.css" rel="stylesheet" type="text/css" />
    <script src="js/jquery-1.7.1.min.js" type="text/javascript"></script>
    <script src="js/jquery-ui-1.8.18.custom.min.js" type="text/javascript"></script>
    <script src="js/jquery-ui-timepicker-addon.js" type="text/javascript"></script>
    <script src="js/jquery.ui.selectmenu.js" type="text/javascript"></script>
    <link href="js/jquery.countdown.css" rel="stylesheet" type="text/css" />
    <script src="js/jquery.countdown.pack.js" type="text/javascript"></script>
    <script src="js/jquery-gh-custom-objects.js" type="text/javascript"></script>
    <script src="js/jquery.multiselect.js" type="text/javascript"></script>
    <script type="text/javascript">
        function OnLoad_Function() {
            GH_Buttons();
            //GH_DropDown();
            GH_DatePicker();

            //GH_Button();
            //GH_Select();
            //GH_SelectMultiple();

            $('select').each(function(i, obj) {
                var w = $(this).width() + 25;
                if ($(this).attr("Width2")) {
                    w = $(this).attr("Width2");
                }
                if ($(this).attr("multiple") != "multiple") {
                    $(this).selectmenu({
                        menuWidth: w,
                        wrapperElement: "<div class='wrap' />"
                    });
                };
            });

            $("#<%=ddlDispositions.ClientID %>").multiselect({
                minWidth: 125
                , height: 275
                , classes: "multieselect"
                , header: true
                , noneSelectedText: 'Select'
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

    </script>
    <style type="text/css">
    .ui-multiselect-menu
    {
    	width: 275px !important;
    }
    </style>
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
        <asp:Panel ID="Panel1" runat="server" Visible="false">
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
                            <asp:ListItem Value="0" Text="All" />
                            <asp:ListItem Value="103010" Text="Call" />
                            <asp:ListItem Value="103020" Text="IVR" />
                        </asp:DropDownList>
                    </div>
                    <div id="call_id">
                        <asp:Label ID="Label1" runat="server" Text="Call ID" CssClass="filter_label1" />
                        <asp:TextBox ID="CallID2" runat="server" Width="125px" />
                    </div>
                    <div id="call_status">
                        <asp:Label ID="Label9" runat="server" Text="Disposition" CssClass="filter_label1" />
                        <asp:ListBox ID="ddlDispositions" runat="server" SelectionMode="Multiple" Width="130px" Width2="300px" multiple="multiple">
                            <asp:ListItem Value="0" Text="* Donations *" />
                            <asp:ListItem Value="1" Text="Complaint on Policies" />
                            <asp:ListItem Value="2" Text="No Whisper" />
                            <asp:ListItem Value="3" Text="Wanted to Donate Blood/Questions" />
                            <asp:ListItem Value="4" Text="Wanted to Volunteer" />
                            <asp:ListItem Value="5" Text="Wanted to Donate Goods" />
                            <asp:ListItem Value="6" Text="Wanted to Sponsor an Event" />
                            <asp:ListItem Value="7" Text="Wanted Information on Red Cross" />
                            <asp:ListItem Value="8" Text="Wanted Donation/Receipt Info" />
                            <asp:ListItem Value="9" Text="Hung Up" />
                            <asp:ListItem Value="10" Text="Information Only" />
                            <asp:ListItem Value="11" Text="Needed Local Chapter Info" />
                            <asp:ListItem Value="12" Text="Needed Help" />
                            <asp:ListItem Value="13" Text="Needed to Locate Military Personnel" />
                            <asp:ListItem Value="14" Text="Needed Media Inquiry" />
                            <asp:ListItem Value="15" Text="Wanted Course/Class Information" />
                            <asp:ListItem Value="16" Text="Wanted Information About Use of Funds" />
                            <asp:ListItem Value="17" Text="Wanted to Donate Planned Gift" />
                            <asp:ListItem Value="18" Text="Referred" />
                            <asp:ListItem Value="19" Text="Wanted to Donate Memorial Gift" />
                            <asp:ListItem Value="20" Text="Training" />
                            <asp:ListItem Value="21" Text="Not GE-Canada" />
                            <asp:ListItem Value="22" Text="Wrong Number" />
                            <asp:ListItem Value="23" Text="Research" />
                            <asp:ListItem Value="24" Text="Memorial-Honorary Gift" />
                            <asp:ListItem Value="25" Text="Wanted to Find Loved Ones" />
                            <asp:ListItem Value="26" Text="Wants Explanation of Different Funds" />
                            <asp:ListItem Value="27" Text="Wants to be Removed From Mailing List" />
                            <asp:ListItem Value="28" Text="Wanted to Donate Planned Gift" />
                            <asp:ListItem Value="29" Text="Message" />
                            <asp:ListItem Value="30" Text="Canceled Order" />
                            <asp:ListItem Value="31" Text="Gift Matching" />
                            <asp:ListItem Value="32" Text="2009 Holiday Gift Catalog Donation" />
                            <asp:ListItem Value="33" Text="Transfer to DOC" />
                            <asp:ListItem Value="34" Text="2010 Holiday Giving Catalog" />
                            <asp:ListItem Value="35" Text="2011 Spring Storms Telethon" />
                            <asp:ListItem Value="36" Text="2011 Holiday Giving Catalog" />
                        </asp:ListBox>
                    </div>
                </div>
                <div id="filter_to" class="filter_section">
                    <h3>Donation Source</h3>
                    <div id="donation_to_source">
                        <asp:Label ID="Label6" runat="server" Text="Donation Source" CssClass="filter_label1" />
                        <asp:DropDownList ID="ddlDonationSource" runat="server" Width="180px" Width2="200px" >
                            <asp:ListItem Value="0" Text="All" />
                            <asp:ListItem Value="103201" Text="Web" />
                            <asp:ListItem Value="103202" Text="Phone" />
                            <asp:ListItem Value="103203" Text="Check" />
                            <asp:ListItem Value="103204" Text="Other" />
                            <asp:ListItem Value="103205" Text="IVR" />
                            <asp:ListItem Value="103206" Text="Pledge" />

                        </asp:DropDownList>
                    </div>
                    <div id="donor_to_source">
                        <asp:Label ID="Label7" runat="server" Text="Donor Type" CssClass="filter_label1" />
                        <asp:DropDownList ID="ddlDonorType" runat="server" Width="180px" Width2="200px" >
                            <asp:ListItem Value="0" Text="All" />
                            <asp:ListItem Value="101010" Text="Web" />
                            <asp:ListItem Value="101020" Text="Phone" />
                            <asp:ListItem Value="101030" Text="Mail" />
                            <asp:ListItem Value="101040" Text="Web - Recurring" />
                        </asp:DropDownList>
                    </div>
                </div>
                <div id="filter_options" class="filter_section">
                    <h3>Options</h3>
                    <div id="donation_max">
                        <asp:Label ID="Label13" runat="server" Text="Max Records" CssClass="filter_label1" />
                        <asp:DropDownList ID="ddlTop" runat="server" Width="60" Width2="60">
                            <asp:ListItem Text="25" Value="25" />
                            <asp:ListItem Text="50" Value="50" />
                            <asp:ListItem Text="100" Value="100" Selected="True" />
                            <asp:ListItem Text="250" Value="250" />
                            <asp:ListItem Text="500" Value="500" />
                            <asp:ListItem Text="All" Value="0" />
                        </asp:DropDownList>
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
                            <asp:ListItem Text="Equal to" Value="0" Selected="True" />
                            <asp:ListItem Text="Equal or Greater" Value="1" />
                            <asp:ListItem Text="Equal or Less" Value="2" />
                        </asp:DropDownList>
                    </div>
                </div>
            </div>
            <div style="color: Blue;">
                All filter options work together; you can clear any text or date fields to remove them from the query.
                <br /><br />
            </div>
        </div>
        </asp:Panel>
        <div id="search_result" class="clearfix">
            <div id="search_grid" style="display: inline-block;vertical-align: top;">
                <asp:UpdatePanel ID="UpdatePanel1" runat="server">
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
                        <div style="display: inline-block;vertical-align: top;">
                            <asp:Button ID="Button1" runat="server"
                                Text="Search"
                                OnClientClick="this.disabled = true; this.value = 'Running...';" 
                                UseSubmitBehavior="false" 
                                OnClick="GridView_Refresh"
                                />
                            <asp:Button ID="Button2" runat="server"
                                Text="Search"
                                OnClientClick="Filter_Toggle(btnFilters01,'reporting_filters','All Filters');this.disabled = true; this.value = 'Running...';" 
                                UseSubmitBehavior="false" 
                                OnClick="GridView_Refresh"
                                Visible="false"
                                />
                            <asp:Button ID="btnExport" runat="server" Text="Export to Excel" OnClick="GridView1_Export_Excel" Visible="false" />
                        </div>
                        <div style="display: inline-block;vertical-align: top;">
                            <asp:Label ID="rpElapsed" runat="server" Text="" />
                        </div>
                        <div style="display: inline-block;">
                            Call ID: <asp:TextBox ID="CallID" runat="server" Width="125px" />
                            Call ANI: <asp:TextBox ID="CallANI" runat="server" Width="125px" />
                            <br />Call Date: <asp:TextBox ID="CallDate" runat="server" Width="75px" CssClass="ghDatePickerStart" />
                        </div>
                    </div>
                    <div id="grid" style="display: inline-block;width: 515px;vertical-align: top;">
                        <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False"
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

                            OnDataBound="GridView1_DataBound"
                            OnRowDataBound="GridView1_RowDataBound"
                            OnSelectedIndexChanged="GridView1_IndexChanged"
                            OnPageIndexChanging="GridView1_PageIndexChanging"

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
                                                  OnSelectedIndexChanged="GridView1_PageDropDownList_SelectedIndexChanged" 
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
                                <asp:BoundField HeaderText="imported" DataField="started" DataFormatString="{0:MM/dd/yyyy HH:mm:ss}" ItemStyle-CssClass="LastCol" />
                                <asp:BoundField HeaderText="status" DataField="status" />
                                <asp:CommandField SelectText="Select" ShowSelectButton="true" ItemStyle-CssClass="HideButton"
                                    HeaderStyle-CssClass="HideButton" />
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
                    <div>
                        <span style="font-weight: bold;">Select a record to show it's details below</span>
                        <br /><asp:Label ID="Label16" runat="server" Text="" />
                        <br /><asp:Button ID="btnClearRecord" runat="server" Text="Clear Record" OnClick="Processing_Record_Clear" Visible="false" />
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:Button ID="btnDiscardRecord" runat="server" Text="Discard Record" OnClick="Processing_Record_Discard" Visible="false" />
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
                    <asp:DetailsView ID="dv_ivr_file_info" runat="server"
                        AutoGenerateRows="True" ForeColor="Black" GridLines="Vertical"
                        HeaderText="Record Info"
                        CssClass="User_List_Details_Border"

                        OnDataBound="DetailsView_DataBound"
                        
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
