<%@ Page Title="Reporting Stats" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Reporting_Stats.aspx.cs" Inherits="Reporting_Stats" %>
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
    <script type="text/javascript">
        function OnLoad_Function() {
            GH_Buttons();
            GH_DropDown();
            //GH_DatePicker();
            GH_DatePicker_FullMonth();
            GH_TimePicker();

            $(document).ready(function () {
                toggle_option_loop();
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
            $(document).ready(function () {
                //alert($(btn).attr("src"));
                //alert($(btn).val().substring(5, $(btn).val().length));
                if (tgl == "") { tgl = $(btn).val(); }
                var nme = $(btn).val().substring(5, $(btn).val().length);
                if (tgl == "hide") { $("#" + field).hide(); $(btn).val("show"); $(btn).attr("src", "images/down_white_green.jpg"); }
                else { $("#" + field).show(); $(btn).val("hide"); $(btn).attr("src", "images/up_white_red.jpg"); }
            });
        }
        function toggle_option_loop() {
            $(document).ready(function () {
                $("#report_count").val("0");
                btn = "#ctl00_Master_Content_cbMonthlyCounts"; var btnChecked = $(btn).prop("checked"); if (btnChecked) { toggle_option(btn); }
                btn = "#ctl00_Master_Content_cbCallCounts"; var btnChecked = $(btn).prop("checked"); if (btnChecked) { toggle_option(btn); }
                btn = "#ctl00_Master_Content_cbHourlySats"; var btnChecked = $(btn).prop("checked"); if (btnChecked) { toggle_option(btn); }
                btn = "#ctl00_Master_Content_cbDispositions"; var btnChecked = $(btn).prop("checked"); if (btnChecked) { toggle_option(btn); }
                btn = "#ctl00_Master_Content_cbDesignations"; var btnChecked = $(btn).prop("checked"); if (btnChecked) { toggle_option(btn); }
                btn = "#ctl00_Master_Content_cbCreditCard"; var btnChecked = $(btn).prop("checked"); if (btnChecked) { toggle_option(btn); }
                btn = "#ctl00_Master_Content_cbDNIS"; var btnChecked = $(btn).prop("checked"); if (btnChecked) { toggle_option(btn); }
                btn = "#ctl00_Master_Content_cbDNISDesignation"; var btnChecked = $(btn).prop("checked"); if (btnChecked) { toggle_option(btn); }
                btn = "#ctl00_Master_Content_cbLines"; var btnChecked = $(btn).prop("checked"); if (btnChecked) { toggle_option(btn); }
                btn = "#ctl00_Master_Content_cbLinesDesignations"; var btnChecked = $(btn).prop("checked"); if (btnChecked) { toggle_option(btn); }
            });

        }
        function toggle_option(btn) {
            // Toggles Filter Options based on which report is selected
            // We also use this to limit how many reports the user selects
            $(document).ready(function () {
                //alert($(btn).attr("src"));
                //alert($(btn).val().substring(5, $(btn).val().length));
                //alert(btn);
                var msg = "";
                var btnChecked = $(btn).prop("checked");
                var btnName = $(btn).attr('id');
                var rprtCount = Number($("#report_count").val());
                if (btnChecked) { rprtCount += 1; } else { rprtCount -= 1; }
                if (rprtCount < 0) { rprtCount = 0; }

                if (rprtCount > 3) {
                    $(btn).attr("checked", false); // Unchecks it
                    rprtCount -= 1;
                    //alert("You may only select up to 3 reports.");
                    msg = "<br />" + "You may only select up to 3 reports at once.";
                } else {
                    if (btnName.indexOf("cbMonthlyCounts") > 0) {
                        if (btnChecked) {
                            $("#grid_monthlycounts").show();
                            msg += "<br />Monthly Count report selected.";
                        } else {
                            $("#grid_monthlycounts").hide();
                        }
                    } else if (btnName.indexOf("cbCallCounts") > 0) {
                        if (btnChecked) {
                            $("#grid_callcounts").show();
                            msg += "<br />Call Count report selected.";
                        } else {
                            $("#grid_callcounts").hide();
                        }
                    } else if (btnName.indexOf("cbHourlySats") > 0) {
                        if (btnChecked) {
                            $("#grid_hourlystats").show();
                            msg += "<br />Hourly Stats report selected.";
                        } else {
                            $("#grid_hourlystats").hide();
                        }
                    } else if (btnName.indexOf("cbDispositions") > 0) {
                        if (btnChecked) {
                            $("#grid_dispositions").show();
                            msg += "<br />Dispositions report selected.";
                            // $("#filter_options").show();
                            $("#filter_options_dispositions").show();
                            var tglCount = Number($("#tgl_show_options").val()); tglCount++; $("#tgl_show_options").val(tglCount);
                        } else {
                            $("#grid_dispositions").hide();
                            var tglCount = Number($("#tgl_show_options").val()); tglCount--; $("#tgl_show_options").val(tglCount);
                            if (tglCount == 0) $("#filter_options").hide(); // Can't hide this if another report needs it...
                            $("#filter_options_dispositions").hide();
                        }
                    } else if (btnName.indexOf("cbDesignations") > 0) {
                        if (btnChecked) {
                            $("#grid_designations").show();
                            msg += "<br />Designations report selected.";
                            // $("#filter_options").show();
                            $("#filter_options_designations").show();
                            var tglCount = Number($("#tgl_show_options").val()); tglCount++; $("#tgl_show_options").val(tglCount);
                        } else {
                            $("#grid_designations").hide();
                            var tglCount = Number($("#tgl_show_options").val()); tglCount--; $("#tgl_show_options").val(tglCount);
                            if (tglCount == 0) $("#filter_options").hide(); // Can't hide this if another report needs it...
                            $("#filter_options_designations").hide();
                        }
                    } else if (btnName.indexOf("cbCreditCard") > 0) {
                        if (btnChecked) {
                            $("#grid_creditcard").show();
                            msg += "<br />Credit Card report selected.";
                        } else {
                            $("#grid_creditcard").hide();
                        }
                    } else if (btnName.indexOf("cbDNISDesignation") > 0) {
                        if (btnChecked) {
                            $("#grid_dnisdesignation").show();
                            msg += "<br />DNIS by Designation report selected.";
                        } else {
                            $("#grid_dnisdesignation").hide();
                        }
                    } else if (btnName.indexOf("cbDNIS") > 0) {
                        if (btnChecked) {
                            $("#grid_dnis").show();
                            msg += "<br />DNIS report selected.";
                        } else {
                            $("#grid_dnis").hide();
                        }
                    } else if (btnName.indexOf("cbLinesDesignations") > 0) {
                        if (btnChecked) {
                            $("#grid_linesdesignations").show();
                            msg += "<br />Phone Line by Designations report selected.";
                        } else {
                            $("#grid_linesdesignations").hide();
                        }
                    } else if (btnName.indexOf("cbLines") > 0) {
                        if (btnChecked) {
                            $("#grid_lines").show();
                            msg += "<br />Phone Line report selected.";
                        } else {
                            $("#grid_lines").hide();
                        }
                    } 

                    // 
                }
                //alert(btnName + "\r" + btnName + "\r" + rprtCount);
                // msg += "<br />" + btnName + "<br />" + btnName + "<br />" + rprtCount;
                //alert($(btn + " input:checked").val());
                $("#report_count").val(rprtCount);
                $("#msgFilters").html(msg)
            });
        }

        // 
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
    .report_header
    {
        font-weight: bold;
        color: #80A2D0;
        background-color: white;
        width: 710px;
    }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Master_Content" Runat="Server">
    <script type="text/javascript">
        // document.body.onload = function() { OnLoad_Function(); }
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
            <div id="msgTitle" style="display: inline;width: 50%;color: DarkRed;font-weight: bold;text-align: left;">
                Greenwood & Hall Client Reporting Tool
            </div>            
            <div id="msgTitleRight" style="float: right;width: 50%;color: DarkOrange;font-weight: bold;text-align: right;">
                <asp:Label ID="rpTimeZone" runat="server" Text="Times are UTC Minus the Offset: " />
            </div>
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
                        <br />
                        <a href="JavaScript:SelectDateVariable('DayPrevious', '<%=dtStartDate.ClientID %>', '<%=dtEndDate.ClientID %>')" style="color: Blue;"><< Previous Day</a>&nbsp;&nbsp;
                        <a href="JavaScript:SelectDateVariable('DayNext', '<%=dtStartDate.ClientID %>', '<%=dtEndDate.ClientID %>')" style="color: Blue;">Next Day >></a>&nbsp;&nbsp;
                    </div>
                </div>
                <div id="filter_reports" class="filter_section" runat="server" visible="false">
                    <h3>Select Report(s)</h3>
                    <div style="display: inline-block;vertical-align: top;">
                        <div id="filter_monthlycounts" runat="server" visible="false">
                            <asp:CheckBox ID="cbMonthlyCounts" runat="server" Checked="false" onclick="toggle_option(this);" /> Monthly Counts
                        </div>
                        <div id="filter_callcounts" runat="server" visible="false">
                            <asp:CheckBox ID="cbCallCounts" runat="server" Checked="false" onclick="toggle_option(this);" /> Call/Record Counts                            
                        </div>
                        <div id="filter_hourlystats" runat="server" visible="false">
                            <asp:CheckBox ID="cbHourlySats" runat="server" Checked="false" onclick="toggle_option(this);" /> Call/Record Hourly                            
                        </div>
                        <div id="filter_dispositions" runat="server" visible="false">
                            <asp:CheckBox ID="cbDispositions" runat="server" Checked="false" onclick="toggle_option(this);" /> Dispositions                            
                        </div>
                        <div id="filter_designations" runat="server" visible="false">
                            <asp:CheckBox ID="cbDesignations" runat="server" Checked="false" onclick="toggle_option(this);" /> Designations                            
                        </div>
                    </div>                    
                    <div style="display: inline-block;vertical-align: top;">
                        <div id="filter_creditcard" runat="server" visible="false">
                            <asp:CheckBox ID="cbCreditCard" runat="server" Checked="false" onclick="toggle_option(this);" /> Credit Card Processing                            
                        </div>
                        <div id="filter_dnis" runat="server" visible="false">
                            <asp:CheckBox ID="cbDNIS" runat="server" Checked="false" onclick="toggle_option(this);" /> DNIS                            
                        </div>
                        <div id="filter_dnisdesignation" runat="server" visible="false">
                            <asp:CheckBox ID="cbDNISDesignation" runat="server" Checked="false" onclick="toggle_option(this);" /> DNIS -> Designation                            
                        </div>
                        <div id="filter_lines" runat="server" visible="false">
                            <asp:CheckBox ID="cbLines" runat="server" Checked="false" onclick="toggle_option(this);" /> Lines
                        </div>
                        <div id="filter_lines_designations" runat="server" visible="false">
                            <asp:CheckBox ID="cbLinesDesignations" runat="server" Checked="false" onclick="toggle_option(this);" /> Lines -> Designations
                        </div>
                    </div>
                </div>
                <div id="filter_info" class="filter_section">
                    You may select up to 3 reports to run at the same time.
                    <span id="msgFilters" style="color: darkred;" />
                </div>
                <input type="hidden" id="report_count" value="0" />
                <input type="hidden" id="tgl_show_options" value="0" />
            </div>
            <div class="clearfix">
                <div id="filter_options" class="filter_section" style="display: none;">
                    <h3>Options</h3>
                    <div id="filter_options_dispositions" style="display: none;">
                        <asp:Label ID="Label2" runat="server" Text="Disposition" CssClass="filter_label1" />
                        <asp:DropDownList ID="ddlDisposition" runat="server" Width="128px" Width2="250px" >
                            <asp:ListItem Value="0" Text="All" />
                            <asp:ListItem Value="103010" Text="General Donation" />
                            <asp:ListItem Value="103020" Text="In Honor Donation" />
                            <asp:ListItem Value="103030" Text="In Memory Donation" />
                            <asp:ListItem Value="103040" Text="Purchase" />
                            <asp:ListItem Value="103050" Text="Other" />
                            <asp:ListItem Value="103060" Text="Dedication" />
                            <asp:ListItem Value="103070" Text="Recurring - General" />
                            <asp:ListItem Value="103080" Text="Recurring - General Honor" />
                            <asp:ListItem Value="103090" Text="Recurring - General Memory" />
                            <asp:ListItem Value="103100" Text="Recurring - Team" />
                            <asp:ListItem Value="103110" Text="Recurring - Team Honor" />
                            <asp:ListItem Value="103120" Text="Recurring - Team Memory" />
                            <asp:ListItem Value="103130" Text="Recurring - Star" />
                            <asp:ListItem Value="103140" Text="Recurring - Star Honor" />
                            <asp:ListItem Value="103150" Text="Recurring - Star Memory" />
                            <asp:ListItem Value="103160" Text="Recurring - Repeat" />
                        </asp:DropDownList>
                    </div>
                    <div id="filter_options_designations" style="display: none;">
                        <asp:Label ID="Label5" runat="server" Text="Designation" CssClass="filter_label1" />
                        <asp:DropDownList ID="ddlDesignation" runat="server" Width="128px" Width2="250px" >
                            <asp:ListItem Value="0" Text="All" />
                            <asp:ListItem Value="103010" Text="General Donation" />
                            <asp:ListItem Value="103020" Text="In Honor Donation" />
                            <asp:ListItem Value="103030" Text="In Memory Donation" />
                            <asp:ListItem Value="103040" Text="Purchase" />
                            <asp:ListItem Value="103050" Text="Other" />
                            <asp:ListItem Value="103060" Text="Dedication" />
                            <asp:ListItem Value="103070" Text="Recurring - General" />
                            <asp:ListItem Value="103080" Text="Recurring - General Honor" />
                            <asp:ListItem Value="103090" Text="Recurring - General Memory" />
                            <asp:ListItem Value="103100" Text="Recurring - Team" />
                            <asp:ListItem Value="103110" Text="Recurring - Team Honor" />
                            <asp:ListItem Value="103120" Text="Recurring - Team Memory" />
                            <asp:ListItem Value="103130" Text="Recurring - Star" />
                            <asp:ListItem Value="103140" Text="Recurring - Star Honor" />
                            <asp:ListItem Value="103150" Text="Recurring - Star Memory" />
                            <asp:ListItem Value="103160" Text="Recurring - Repeat" />
                        </asp:DropDownList>
                    </div>
                    <div id="filter_options_other01" style="display: none;">
                        <asp:Label ID="Label11" runat="server" Text="Phone" CssClass="filter_label1" />
                        <asp:TextBox ID="Phone" runat="server" Width="125px" />
                    </div>
                    <div id="filter_options_other02" style="display: none;">
                        <asp:Label ID="Label12" runat="server" Text="Credit Card" CssClass="filter_label1" />
                        <asp:TextBox ID="Card" runat="server" Width="125px" />
                    </div>
                </div>
                <div id="filter_type" class="filter_section" runat="server" visible="false">
                    <h3>Donation Type</h3>
                    <div id="call_id">
                        <asp:Label ID="Label1" runat="server" Text="Donation ID" CssClass="filter_label1" />
                        <asp:TextBox ID="Confirmation" runat="server" Width="125px" />
                    </div>
                    <div id="call_status">
                        <asp:Label ID="Label9" runat="server" Text="Status" CssClass="filter_label1" />
                        <asp:DropDownList ID="ddlDonationStatus" runat="server" Width="128px" Width2="250px" >
                            <asp:ListItem Value="0" Text="All" />
                            <asp:ListItem Value="104010" Text="Submited" />
                            <asp:ListItem Value="104020" Text="Approved" />
                            <asp:ListItem Value="104030" Text="Error" />
                            <asp:ListItem Value="104040" Text="Decline" />
                            <asp:ListItem Value="104050" Text="Refund Req" />
                            <asp:ListItem Value="104060" Text="Refund Proc" />
                            <asp:ListItem Value="104070" Text="Cancelled" />
                            <asp:ListItem Value="104080" Text="Settled" />
                            <asp:ListItem Value="104090" Text="Test" />
                            <asp:ListItem Value="104100" Text="Imported" />
                            <asp:ListItem Value="104110" Text="PayPal Submited" />
                            <asp:ListItem Value="104120" Text="PayPal Complete" />
                            <asp:ListItem Value="104130" Text="PayPal Cancelled" />
                            <asp:ListItem Value="104140" Text="Refund Cancelled" />
                            <asp:ListItem Value="104150" Text="Review" />
                            <asp:ListItem Value="104160" Text="Cancelled - Duplicate" />
                            <asp:ListItem Value="104170" Text="Chargeback Entered" />
                            <asp:ListItem Value="104180" Text="Refund Declined/Rejected" />
                            <asp:ListItem Value="104190" Text="Recurring Approved" />
                            <asp:ListItem Value="104200" Text="Recurring Declined" />
                            <asp:ListItem Value="104210" Text="Recurring Canceled" />
                            <asp:ListItem Value="104220" Text="Shipping Address Confirmed" />
                            <asp:ListItem Value="104230" Text="Shipping Address Edited" />
                        </asp:DropDownList>
                    </div>
                </div>
                <div id="filter_to" class="filter_section" runat="server" visible="false">
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
            </div>
            <div style="color: Blue;">
                All filter options work together; you can clear any text or date fields to remove them from the query.
                <br /><br />
            </div>
        </div>
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
                                    $('#<%= btnRefresh.ClientID  %> ').find("[value=0]").attr("disabled", "disabled");
                                    $('#<%= btnRefresh.ClientID  %> ').find("[value=0]").attr("aria-disabled", "true");
                                }
                            }
                        </script>
                        <div id="submit-buttons">
                            <asp:Button ID="btnRefresh" runat="server"
                                Text="Run/Refresh Stats"
                                OnClientClick="this.disabled = true; this.value = 'Running...';" 
                                UseSubmitBehavior="false" 
                                OnClick="GridView_Refresh"
                                />
                            <div style="display: inline-block;">
                                <asp:Label ID="rpElapsed" runat="server" Text="" />
                            </div>
                            <asp:Button ID="btnExport" runat="server" Text="Export to Excel" OnClick="CallRecords_Export_Excel" Visible="false" />
                        </div>
                        <div>
                            <asp:Label ID="lblFilterDetails" runat="server" Text="" ForeColor="Blue" />
                        </div>
                        <div id="grid" style="display: inline-block;width: 715px;vertical-align: top;">
                            <div id="grid_monthlycounts" style="display: none;">
                                <h3 class="report_header">Monthly Count</h3>
                                <asp:GridView ID="gvMonthlyCounts" runat="server" AutoGenerateColumns="false"
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

                                    CssClass="Portal_GridView_Standard"
                                    Width="710"
                                    >
                                    <AlternatingRowStyle CssClass="Portal_GridView_Standard_Alternate" />
                                    <SelectedRowStyle CssClass="Portal_GridView_Standard_Selected" />
                                    <PagerSettings Position="Top" />
                                    <PagerStyle VerticalAlign="Bottom" HorizontalAlign="Center" CssClass="Portal_Gridview_Pager" />
                                    <Columns>
                                        <asp:BoundField HeaderText="year" DataField="year" />
                                        <asp:BoundField HeaderText="month" DataField="month" />
                                        <asp:BoundField HeaderText="month_name" DataField="month_name" />
                                        <asp:BoundField HeaderText="calls_total" DataField="calls_total" />
                                        <asp:BoundField HeaderText="calls_main" DataField="calls_main" />
                                        <asp:BoundField HeaderText="calls_drtv" DataField="calls_drtv" />
                                        <asp:BoundField HeaderText="donations" DataField="donations" />
                                        <asp:BoundField HeaderText="donations_main" DataField="donations_main" />
                                        <asp:BoundField HeaderText="donations_drtv" DataField="donations_drtv" />
                                        <asp:BoundField HeaderText="amount_approved" DataField="amount_approved" DataFormatString="{0:C}" />
                                    </Columns>
                                    <EmptyDataTemplate>
                                        No Records For Selected Filters
                                    </EmptyDataTemplate>
                                </asp:GridView>
                            </div>
                            <div id="grid_callcounts" style="display: none;">
                                <h3 class="report_header">Call/Record Counts</h3>
                                <asp:GridView ID="gvCallRecords" runat="server" AutoGenerateColumns="False"
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

                                    CssClass="Portal_GridView_Standard"
                                    Width="710"
                                    >
                                    <AlternatingRowStyle CssClass="Portal_GridView_Standard_Alternate" />
                                    <SelectedRowStyle CssClass="Portal_GridView_Standard_Selected" />
                                    <PagerSettings Position="Top" />
                                    <PagerStyle VerticalAlign="Bottom" HorizontalAlign="Center" CssClass="Portal_Gridview_Pager" />
                                    <Columns>
                                        <asp:BoundField HeaderText="calls" DataField="count" />
                                        <asp:BoundField HeaderText="donations" DataField="count_donation" />
                                        <asp:BoundField HeaderText="approved" DataField="amount_approved" DataFormatString="{0:C}" />
                                        <asp:BoundField HeaderText="total" DataField="amount_total" DataFormatString="{0:C}" />
                                        <asp:TemplateField HeaderText="first">
                                            <ItemTemplate>
                                                <asp:Label ID="first" runat="server" Text='<%# ghFunctions.date_label(Eval("first").ToString()) %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="last" ItemStyle-CssClass="LastCol" >
                                            <ItemTemplate>
                                                <asp:Label ID="last" runat="server" Text='<%# ghFunctions.date_label(Eval("last").ToString()) %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                    <EmptyDataTemplate>
                                        No Records For Selected Filters
                                    </EmptyDataTemplate>
                                </asp:GridView>
                            </div>
                            <div id="grid_hourlystats" style="display: none;">
                                <h3 class="report_header">Call/Record Hourly Stats w/Amounts</h3>
                                <asp:Label ID="lblCall_StatsHourly" runat="server" Text="" ForeColor="DarkRed" />
                                <asp:GridView ID="gvCall_StatsHourly" runat="server" AutoGenerateColumns="False"
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

                                    CssClass="Portal_GridView_Standard"
                                    Width="710"
                                    >
                                    <AlternatingRowStyle CssClass="Portal_GridView_Standard_Alternate" />
                                    <SelectedRowStyle CssClass="Portal_GridView_Standard_Selected" />
                                    <PagerSettings Position="Top" />
                                    <PagerStyle VerticalAlign="Bottom" HorizontalAlign="Center" CssClass="Portal_Gridview_Pager" />
                                    <Columns>
                                        <asp:BoundField HeaderText="day" DataField="day" />
                                        <asp:BoundField HeaderText="hour" DataField="hour" />
                                        <asp:BoundField HeaderText="calls" DataField="count" />
                                        <asp:BoundField HeaderText="donations" DataField="count_donation" />
                                        <asp:BoundField HeaderText="approved" DataField="amount_approved" DataFormatString="{0:C}" />
                                        <asp:BoundField HeaderText="total" DataField="amount_total" DataFormatString="{0:C}" />
                                    </Columns>
                                    <EmptyDataTemplate>
                                        No Records For Selected Filters
                                    </EmptyDataTemplate>
                                </asp:GridView>
                            </div>
                            <div id="grid_dispositions" style="display: none;">
                                <h3 class="report_header">Dispositions</h3>
                                <asp:GridView ID="gvDispositions" runat="server" AutoGenerateColumns="False"
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

                                    CssClass="Portal_GridView_Standard"
                                    Width="710"
                                    >
                                    <AlternatingRowStyle CssClass="Portal_GridView_Standard_Alternate" />
                                    <SelectedRowStyle CssClass="Portal_GridView_Standard_Selected" />
                                    <PagerSettings Position="Top" />
                                    <PagerStyle VerticalAlign="Bottom" HorizontalAlign="Center" CssClass="Portal_Gridview_Pager" />
                                    <Columns>
                                        <asp:BoundField HeaderText="disposition" DataField="disposition" />
                                        <asp:BoundField HeaderText="count" DataField="count" />
                                        <asp:BoundField HeaderText="donations" DataField="count_donation" />
                                        <asp:BoundField HeaderText="approved" DataField="amount_approved" DataFormatString="{0:C}" />
                                        <asp:BoundField HeaderText="total" DataField="amount_total" DataFormatString="{0:C}" />
                                        <asp:TemplateField HeaderText="first">
                                            <ItemTemplate>
                                                <asp:Label ID="first" runat="server" Text='<%# ghFunctions.date_label(Eval("first").ToString()) %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="last" ItemStyle-CssClass="LastCol" >
                                            <ItemTemplate>
                                                <asp:Label ID="last" runat="server" Text='<%# ghFunctions.date_label(Eval("last").ToString()) %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField HeaderText="avg_settle" DataField="avg_settle" DataFormatString="{0:C}" />
                                        <asp:BoundField HeaderText="avg_decline" DataField="avg_decline" DataFormatString="{0:C}" />
                                    </Columns>
                                    <EmptyDataTemplate>
                                        No Records For Selected Filters
                                    </EmptyDataTemplate>
                                </asp:GridView>
                            </div>
                            <div id="grid_designations" style="display: none;">
                                <h3 class="report_header">Designations</h3>
                                <asp:GridView ID="gvDesignations" runat="server" AutoGenerateColumns="False"
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

                                    CssClass="Portal_GridView_Standard"
                                    Width="710"
                                    >
                                    <AlternatingRowStyle CssClass="Portal_GridView_Standard_Alternate" />
                                    <SelectedRowStyle CssClass="Portal_GridView_Standard_Selected" />
                                    <PagerSettings Position="Top" />
                                    <PagerStyle VerticalAlign="Bottom" HorizontalAlign="Center" CssClass="Portal_Gridview_Pager" />
                                    <Columns>
                                        <asp:BoundField HeaderText="designation" DataField="designation" />
                                        <asp:BoundField HeaderText="count" DataField="count" />
                                        <asp:BoundField HeaderText="donations" DataField="count_donation" />
                                        <asp:BoundField HeaderText="approved" DataField="amount_approved" DataFormatString="{0:C}" />
                                        <asp:BoundField HeaderText="total" DataField="amount_total" DataFormatString="{0:C}" />
                                        <asp:TemplateField HeaderText="first">
                                            <ItemTemplate>
                                                <asp:Label ID="first" runat="server" Text='<%# ghFunctions.date_label(Eval("first").ToString()) %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="last" ItemStyle-CssClass="LastCol" >
                                            <ItemTemplate>
                                                <asp:Label ID="last" runat="server" Text='<%# ghFunctions.date_label(Eval("last").ToString()) %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                    <EmptyDataTemplate>
                                        No Records For Selected Filters
                                    </EmptyDataTemplate>
                                </asp:GridView>
                            </div>
                            <div id="grid_creditcard" style="display: none;">
                                <h3 class="report_header">Credit Card Processing</h3>
                                <asp:GridView ID="gvCreditCard" runat="server" AutoGenerateColumns="False"
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

                                    CssClass="Portal_GridView_Standard"
                                    Width="710"
                                    >
                                    <AlternatingRowStyle CssClass="Portal_GridView_Standard_Alternate" />
                                    <SelectedRowStyle CssClass="Portal_GridView_Standard_Selected" />
                                    <PagerSettings Position="Top" />
                                    <PagerStyle VerticalAlign="Bottom" HorizontalAlign="Center" CssClass="Portal_Gridview_Pager" />
                                    <Columns>
                                        <asp:BoundField HeaderText="source" DataField="source" />
                                        <asp:BoundField HeaderText="status" DataField="status" />
                                        <asp:BoundField HeaderText="donations" DataField="count_donation" />
                                        <asp:BoundField HeaderText="amount" DataField="amount" DataFormatString="{0:C}" />
                                        <asp:BoundField HeaderText="avg" DataField="avg" DataFormatString="{0:C}" />
                                        <asp:TemplateField HeaderText="first">
                                            <ItemTemplate>
                                                <asp:Label ID="first" runat="server" Text='<%# ghFunctions.date_label(Eval("first").ToString()) %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="last" ItemStyle-CssClass="LastCol" >
                                            <ItemTemplate>
                                                <asp:Label ID="last" runat="server" Text='<%# ghFunctions.date_label(Eval("last").ToString()) %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                    <EmptyDataTemplate>
                                        No Records For Selected Filters
                                    </EmptyDataTemplate>
                                </asp:GridView>
                            </div>
                            <div id="grid_dnis" style="display: none;">
                                <h3 class="report_header">DNIS</h3>
                                <asp:GridView ID="gvDNIS" runat="server" AutoGenerateColumns="False"
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

                                    CssClass="Portal_GridView_Standard"
                                    Width="710"
                                    >
                                    <AlternatingRowStyle CssClass="Portal_GridView_Standard_Alternate" />
                                    <SelectedRowStyle CssClass="Portal_GridView_Standard_Selected" />
                                    <PagerSettings Position="Top" />
                                    <PagerStyle VerticalAlign="Bottom" HorizontalAlign="Center" CssClass="Portal_Gridview_Pager" />
                                    <Columns>
                                        <asp:BoundField HeaderText="dnis" DataField="dnis" />
                                        <asp:BoundField HeaderText="line" DataField="line" />
                                        <asp:BoundField HeaderText="company" DataField="company" />
                                        <asp:BoundField HeaderText="count" DataField="count" />
                                        <asp:BoundField HeaderText="donations" DataField="count_donation" />
                                        <asp:BoundField HeaderText="approved" DataField="amount_approved" DataFormatString="{0:C}" />
                                        <asp:TemplateField HeaderText="first">
                                            <ItemTemplate>
                                                <asp:Label ID="first" runat="server" Text='<%# ghFunctions.date_label(Eval("first").ToString()) %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="last" ItemStyle-CssClass="LastCol" >
                                            <ItemTemplate>
                                                <asp:Label ID="last" runat="server" Text='<%# ghFunctions.date_label(Eval("last").ToString()) %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                    <EmptyDataTemplate>
                                        No Records For Selected Filters
                                    </EmptyDataTemplate>
                                </asp:GridView>
                            </div>
                            <div id="grid_dnisdesignation" style="display: none;">
                                <h3 class="report_header">DNIS -> Designation</h3>
                                <asp:GridView ID="gvDNIS_Designation" runat="server" AutoGenerateColumns="False"
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

                                    CssClass="Portal_GridView_Standard"
                                    Width="710"
                                    >
                                    <AlternatingRowStyle CssClass="Portal_GridView_Standard_Alternate" />
                                    <SelectedRowStyle CssClass="Portal_GridView_Standard_Selected" />
                                    <PagerSettings Position="Top" />
                                    <PagerStyle VerticalAlign="Bottom" HorizontalAlign="Center" CssClass="Portal_Gridview_Pager" />
                                    <Columns>
                                        <asp:BoundField HeaderText="DNIS" DataField="dnis" />
                                        <asp:BoundField HeaderText="Line" DataField="line" />
                                        <asp:BoundField HeaderText="Designation" DataField="designation" />
                                        <asp:BoundField HeaderText="Donations" DataField="count_donation" />
                                        <asp:BoundField HeaderText="Amount" DataField="amount_donation" DataFormatString="{0:C}" />
                                    </Columns>
                                    <EmptyDataTemplate>
                                        No Records For Selected Filters
                                    </EmptyDataTemplate>
                                </asp:GridView>
                            </div>
                            <div id="grid_lines" style="display: none;">
                                <h3 class="report_header">Phone Lines</h3>
                                <asp:GridView ID="gvLines" runat="server" AutoGenerateColumns="False"
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

                                    CssClass="Portal_GridView_Standard"
                                    Width="710"
                                    >
                                    <AlternatingRowStyle CssClass="Portal_GridView_Standard_Alternate" />
                                    <SelectedRowStyle CssClass="Portal_GridView_Standard_Selected" />
                                    <PagerSettings Position="Top" />
                                    <PagerStyle VerticalAlign="Bottom" HorizontalAlign="Center" CssClass="Portal_Gridview_Pager" />
                                    <Columns>
                                        <asp:BoundField HeaderText="Line" DataField="line" />
                                        <asp:BoundField HeaderText="Calls" DataField="count" />
                                        <asp:BoundField HeaderText="Donations" DataField="count_donation" />
                                        <asp:BoundField HeaderText="Amount" DataField="amount_approved" DataFormatString="{0:C}" />
                                    </Columns>
                                    <EmptyDataTemplate>
                                        No Records For Selected Filters
                                    </EmptyDataTemplate>
                                </asp:GridView>
                            </div>
                            <div id="grid_linesdesignations" style="display: none;">
                                <h3 class="report_header">Phone Lines by Designations</h3>
                                <asp:GridView ID="gvLinesDesignations" runat="server" AutoGenerateColumns="False"
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

                                    CssClass="Portal_GridView_Standard"
                                    Width="710"
                                    >
                                    <AlternatingRowStyle CssClass="Portal_GridView_Standard_Alternate" />
                                    <SelectedRowStyle CssClass="Portal_GridView_Standard_Selected" />
                                    <PagerSettings Position="Top" />
                                    <PagerStyle VerticalAlign="Bottom" HorizontalAlign="Center" CssClass="Portal_Gridview_Pager" />
                                    <Columns>
                                        <asp:BoundField HeaderText="Line" DataField="line" />
                                        <asp:BoundField HeaderText="Designation" DataField="designation" />
                                        <asp:BoundField HeaderText="Calls" DataField="count" Visible="false" />
                                        <asp:BoundField HeaderText="Donations" DataField="count_donation" />
                                        <asp:BoundField HeaderText="Amount" DataField="amount_approved" DataFormatString="{0:C}" />
                                    </Columns>
                                    <EmptyDataTemplate>
                                        No Records For Selected Filters
                                    </EmptyDataTemplate>
                                </asp:GridView>
                            </div>
                            <hr />
                            <asp:Label ID="Error_General" runat="server" Text="" />
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
            <div id="search_details" style="display: inline-block;vertical-align: top;">
                <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                    <ContentTemplate>
                        <div style="display: none;">
                            Details Section is not complete.
                            <br />Additional Detail sections will be added soon.
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

