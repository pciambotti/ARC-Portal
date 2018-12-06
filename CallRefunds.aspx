<%@ Page Title="Process Refund" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="CallRefunds.aspx.cs" Inherits="CallRefunds" %>
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

    <script src="js/arc_validate.js" type="text/javascript"></script>
    <script type="text/javascript">
        function OnLoad_Function() {
            GH_Button();
            GH_Select();
            $(document).ready(function() {
                $('.ghDatePicker').each(function(i, obj) {
                    $(this).datepicker();
                    if ($(this).val() == "") {
                        $(this).datepicker('setDate', '-1d');
                    }
                });
                $('.ghTimePicker').each(function(i, obj) {
                    $(this).timepicker({});
                    if ($(this).val() == "") {
                        $(this).val("00:00");
                    }
                });
                $('.ghTimePickerEnd').each(function(i, obj) {
                    $(this).timepicker({});
                    if ($(this).val() == "") {
                        $(this).val("23:59");
                    }
                });
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
        function PageInvalid() {
            $('#<%=btnRefundSubmit.ClientID %>').attr("disabled", false);
            $('#<%=btnRefundSubmit.ClientID %>').val("Process Refund/Credit");
            $('#processing').hide();
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
    .user_section
    {
    	width: 600px;
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
        <div>
            Credit Attempt
            <br /><asp:Label ID="dtlLabel" runat="server" Text="" />
            <br /><asp:Label ID="lblTemplate" runat="server" Text="" />
        </div>
        <div class="refund_details">
            <asp:Panel ID="pnl_refund" runat="server">
            <asp:Panel ID="pnl_CreditDetails" runat="server">
                <div id="view_credentials" class="user_section">
                    <h2>Follow On Credit</h2>
                    This indicates that the charge was recent enough to do a credit against the original charge.
                    <div class="user_line clearfix">
                        <div class="user_label">
                            CallID:
                        </div>
                        <div class="user_field">
                            <asp:Label ID="lblCallID" runat="server" Text="" />
                        </div>
                    </div>
                    <div class="user_line clearfix">
                        <div class="user_label">
                            Auth ID:
                        </div>
                        <div class="user_field">
                            <asp:Label ID="lblCBAuthID" runat="server" Text="" />
                        </div>
                    </div>
                    <div class="user_line clearfix">
                        <div class="user_label">
                            ExternalID:
                        </div>
                        <div class="user_field">
                            <asp:Label ID="lblExternalID" runat="server" Text="" />
                        </div>
                    </div>
                    <div class="user_line clearfix">
                        <div class="user_label">
                            Status:
                        </div>
                        <div class="user_field">
                            <asp:Label ID="lblStatus" runat="server" Text="" />
                        </div>
                    </div>
                    <div class="user_line clearfix">
                        <div class="user_label">
                            Donation Date:
                        </div>
                        <div class="user_field">
                            <asp:Label ID="CreateDate" runat="server" Text="" />
                        </div>
                    </div>
                    <div class="user_line clearfix">
                        <div class="user_label">
                            RequestID:
                        </div>
                        <div class="user_field">
                            <asp:Label ID="RequestID" runat="server" Text="" />
                        </div>
                    </div>
                    <div class="user_line clearfix">
                        <div class="user_label">
                            RequestToken:
                        </div>
                        <div class="user_field">
                            <asp:Label ID="RequestToken" runat="server" Text="" />
                        </div>
                    </div>
                    <div class="user_line clearfix">
                        <div class="user_label">
                            Reference Num:
                        </div>
                        <div class="user_field">
                            <asp:Label ID="ReferenceNum" runat="server" Text="" />
                        </div>
                    </div>
                    <div class="user_line clearfix">
                        <div class="user_label">
                            Amount:
                        </div>
                        <div class="user_field">
                            <asp:TextBox ID="Amount" runat="server" Width="50px" />
                            <asp:HiddenField ID="AmountOriginal" runat="server" />
                            <asp:HiddenField ID="RefundType" runat="server" />
                        </div>
                    </div>
                    <div class="user_line clearfix">
                        <div class="user_label">
                            Refund Reason:
                        </div>
                        <div class="user_field">
                            <asp:DropDownList ID="refundReason" runat="server" Width="250px">
                                <asp:ListItem Text="-- Select Refund Reason --" Value="" />
                                <asp:ListItem Text="Duplicate" Value="Duplicate" />
                                <asp:ListItem Text="Chargeback" Value="Chargeback" />
                                <asp:ListItem Text="Changed-Mind" Value="Changed-Mind" />
                                <asp:ListItem Text="Fraud" Value="Fraud" />
                                <asp:ListItem Text="Stolen Card" Value="Stolen Card" />
                                <asp:ListItem Text="Unintended" Value="Unintended" />
                                <asp:ListItem Text="Other" Value="Other" />
                            </asp:DropDownList>
                            <asp:TextBox ID="refundReasonOther" runat="server" Width="250px" TextMode="MultiLine" MaxLength="99" />
                            <asp:CustomValidator ErrorMessage="<br />Please select a refund reason" ControlToValidate="refundReason" ValidationGroup="RefundDo" ClientValidationFunction="ValidateRequired_DropDown" Display="Dynamic" ID="CustomValidator30" runat="server" ValidateEmptyText="true" />
                        </div>
                    </div>
                </div>
                <asp:Panel ID="pnl_standalone" runat="server" Visible="false">
                    <div id="user_address" class="user_section">
                        <h2>Details Required only for Stand Alone Credit</h2>
                        This indicates that the charge is old, or not using the current system.
                        <br />This type of credit requires credit card information.
                        <br /><span style="color: DarkRed;">This information is read only and you should not need to change anything.
                        <br />If any of this information is not accurate, please contact IT</span>
                        <div class="user_line clearfix">
                            <div class="user_label">
                                Credit Card:
                            </div>
                            <div class="user_field">
                                <asp:TextBox ID="CardNumber" runat="server" Width="250px" ReadOnly="true" />
                                <asp:HiddenField ID="CardNumberFull" runat="server" />
                            </div>
                        </div>
                        <div class="user_line clearfix">
                            <div class="user_label">
                                Credit Exp:
                            </div>
                            <div class="user_field">
                                <asp:TextBox ID="CardMonth" runat="server" Width="25px" ReadOnly="true" />
                                <asp:TextBox ID="CardYear" runat="server" Width="25px" ReadOnly="true" />
                            </div>
                        </div>
                        <div class="user_line clearfix">
                            <div class="user_label">
                                Card Type:
                            </div>
                            <div class="user_field">
                                <asp:TextBox ID="CardType" runat="server" Width="75px" ReadOnly="true" />
                                <asp:HiddenField ID="CardTypeFull" runat="server" />
                            </div>
                        </div>
                        <div class="user_line clearfix">
                            <div class="user_label">
                                First Name:
                            </div>
                            <div class="user_field">
                                <asp:TextBox ID="FirstName" runat="server" Width="250px" ReadOnly="true" />
                            </div>
                        </div>
                        <div class="user_line clearfix">
                            <div class="user_label">
                                Last Name:
                            </div>
                            <div class="user_field">
                                <asp:TextBox ID="LastName" runat="server" Width="250px" ReadOnly="true" />
                            </div>
                        </div>
                        <div class="user_line clearfix">
                            <div class="user_label">
                                Email Address:
                            </div>
                            <div class="user_field">
                                <asp:TextBox ID="Email" runat="server" Width="250px" ReadOnly="true" />
                            </div>
                        </div>
                        <div class="user_line clearfix">
                            <div class="user_label">
                                Address 1:
                            </div>
                            <div class="user_field">
                                <asp:TextBox ID="Address1" runat="server" Width="250px" ReadOnly="true" />
                            </div>
                        </div>
                        <div class="user_line clearfix">
                            <div class="user_label">
                                Address 2:
                            </div>
                            <div class="user_field">
                                <asp:TextBox ID="Address2" runat="server" Width="250px" ReadOnly="true" />
                            </div>
                        </div>
                        <div class="user_line clearfix">
                            <div class="user_label">
                                Address 3:
                            </div>
                            <div class="user_field">
                                <asp:TextBox ID="Address3" runat="server" Width="250px" ReadOnly="true" />
                            </div>
                        </div>
                        <div class="user_line clearfix">
                            <div class="user_label">
                                City:
                            </div>
                            <div class="user_field">
                                <asp:TextBox ID="City" runat="server" Width="125px" ReadOnly="true" />
                            </div>
                        </div>
                        <div class="user_line clearfix">
                            <div class="user_label">
                                State:
                            </div>
                            <div class="user_field">
                                <asp:TextBox ID="ddlState" runat="server" Width="75px" ReadOnly="true" />
                            </div>
                        </div>
                        <div class="user_line clearfix">
                            <div class="user_label">
                                Zip:
                            </div>
                            <div class="user_field">
                                <asp:TextBox ID="Zip" runat="server" Width="75px" ReadOnly="true" />
                            </div>
                        </div>
                        <div class="user_line clearfix user_last">
                            <div class="user_label">
                                Country:
                            </div>
                            <div class="user_field">
                                <asp:TextBox ID="ddlCountry" runat="server" Width="75px" ReadOnly="true" />
                            </div>
                        </div>
                    </div>
                </asp:Panel>
                <div id="user_submit" class="user_section">
                    <h2></h2>
                    <div class="user_line clearfix user_last">
                        <div class="user_submit">
                            <asp:Button ID="btnRefundSubmit" runat="server"
                                Text="Process Refund/Credit"
                                OnClientClick="this.disabled = true; this.value = 'Processing...';$('#processing').show();" 
                                UseSubmitBehavior="false" 
                                OnClick="CreditTry" ValidationGroup="RefundDo" Enabled="false"
                                />
                            <asp:Button ID="btnRefundCancel" runat="server" Text="Cancel" Enabled="true" PostBackUrl="~/Search.aspx" />
                        </div>
                        <div id="processing" class="donate-alert" style="display: none;">Please wait while the refund processes.</div>
                    </div>
                </div>
            </asp:Panel>
            <asp:Panel ID="pnl_ReplyDetails" runat="server">
                <div id="Div1" class="user_section">
                    <h2>Response Details</h2>
                    <div class="user_line clearfix">
                        <div class="user_label">
                            Decision:
                        </div>
                        <div class="user_field">
                            <asp:Label ID="rplDecision" runat="server" />
                        </div>
                    </div>
                    <div class="user_line clearfix">
                        <div class="user_label">
                            Merchant Reference Code:
                        </div>
                        <div class="user_field">
                            <asp:Label ID="rplMerchantReferenceCode" runat="server" />
                        </div>
                    </div>
                    <div class="user_line clearfix">
                        <div class="user_label">
                            Reason Code:
                        </div>
                        <div class="user_field">
                            <asp:Label ID="rplReasonCode" runat="server" />
                        </div>
                    </div>
                    <div class="user_line clearfix">
                        <div class="user_label">
                            RequestID:
                        </div>
                        <div class="user_field">
                            <asp:Label ID="rplRequestID" runat="server" />
                        </div>
                    </div>
                    <div class="user_line clearfix">
                        <div class="user_label">
                            Request Token:
                        </div>
                        <div class="user_field">
                            <asp:Label ID="rplRequestToken" runat="server" />
                        </div>
                    </div>
                    <div class="user_line clearfix">
                        <div class="user_label">
                            Amount:
                        </div>
                        <div class="user_field">
                            <asp:Label ID="rplAmount" runat="server" />
                        </div>
                    </div>
                    <div class="user_line clearfix">
                        <div class="user_label">
                            Reason Code:
                        </div>
                        <div class="user_field">
                            <asp:Label ID="rplReasonCode2" runat="server" />
                        </div>
                    </div>
                    <div class="user_line clearfix">
                        <div class="user_label">
                            Reconciliation ID:
                        </div>
                        <div class="user_field">
                            <asp:Label ID="rplReconciliationID" runat="server" />
                        </div>
                    </div>
                    <div class="user_line clearfix">
                        <div class="user_label">
                            Request DateTime:
                        </div>
                        <div class="user_field">
                            <asp:Label ID="rplRequestDateTime" runat="server" />
                        </div>
                    </div>
                    <div class="user_line clearfix user_last">
                        <div class="user_label">
                            SQL Response:
                        </div>
                        <div class="user_field">
                            <asp:Label ID="rplResponse" runat="server" />
                        </div>
                    </div>
                </div>
            </asp:Panel>
            </asp:Panel>
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
                <asp:Label ID="sqlMsg" runat="server" Text="" />
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

