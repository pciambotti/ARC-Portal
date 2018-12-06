<%@ Page Title="User Profile" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="User_Profile.aspx.cs" Inherits="User_Profile" %>

<%@ MasterType TypeName="MasterPage" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="Server">
    <link href="css/portal_all.css" rel="stylesheet" type="text/css" />
    <link href="css/portal_standard.css" rel="stylesheet" type="text/css" />
    <link href="css/user.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
    /* css for timepicker */
    .wrap {display: inline-block;}
    .ui-timepicker-div .ui-widget-header { margin-bottom: 8px; }
    .ui-timepicker-div dl { text-align: left; }
    .ui-timepicker-div dl dt { height: 25px; margin-bottom: -25px; }
    .ui-timepicker-div dl dd { margin: 0 10px 10px 65px; }
    #ui-timepicker-div td { font-size: 50%; }
    .ui-tpicker-grid-label { background: none; border: none; margin: 0; padding: 0; }
    .ui-widget{font-size: 12px;}
    .ui-button-text, .ui-button, .ui-button-text {font-size: 10px !important;}

    .ui-selectmenu-menu, .ui-selectmenu, .ui-selectmenu-text {font-size: 10px !important;}
    .ui-slider,ui-slider-horizontal,ui-slider-vertical {font-size: 10px !important;}
    
    #ui-datepicker-div{font-size: 12px;}
    #ui-datepicker-div{margin-left: 30px;}

    </style>
    <script src="js/jquery-gh-custom-objects.js" type="text/javascript"></script>
    <link href="js/jquery-ui-1.11.4.start/jquery-ui.css" rel="stylesheet" type="text/css" />
    <link href="css/start/jquery.ui.selectmenu.css" rel="stylesheet" type="text/css" />
    <link href="js/jquery.countdown.css" rel="stylesheet" type="text/css" />
    <link href="js/jquery.multiselect.css" rel="stylesheet" type="text/css" />
    <link href="js/jquery.multiselect.filter.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="js/jquery.js"></script>
    <script type="text/javascript" src="js/jquery-ui-1.11.4.start/jquery-ui.js"></script>
    <script src="js/jquery.multiselect.js" type="text/javascript"></script>
    <script src="js/jquery.multiselect.filter.js" type="text/javascript"></script>
    <script src="js/jquery.plugin.min.js" type="text/javascript"></script>
    <script src="js/jquery-ui-timepicker-addon.js" type="text/javascript"></script>
    <script src="js/jquery.countdown.min.js" type="text/javascript"></script>
    <script type="text/javascript">
        function OnLoad_Function() {
            // Turn all Submit Buttons to jQuery themed buttons
            GH_Buttons();
            GH_DropDown();
            // GH_DatePicker();
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
        function passwordShow(tgl,inpt) {
            // tgl is what we click to show/hide the password
            // inpt is the input field we are changing from PASSWORD to TEXT and back again
            //alert($(tgl).html());
            if ($(tgl).html() == "Show") {
                $("#ctl00_Master_Content_" + inpt).attr("type", "text");
                $(tgl).html("Hide");
                // $(tgl).click(function (e) { e.preventDefault(); });
                // $(tgl).removeAttr('href');
                setTimeout(function () { passwordShow(tgl, inpt); }, 2000);
            } else {
                $("#ctl00_Master_Content_" + inpt).attr("type", "password");
                $(tgl).attr("href", "#");
                $(tgl).html("Show");
            }
        }
    </script>
    <style type="text/css">
        .default_left
        {
	        display: inline-block;
	        width: 500px;
            vertical-align: top;
        }
        .default_right
        {
	        display: inline-block;
	        width: 500px;
            vertical-align: top;
        }
        .profile_details
        {
	        width: 500px;
            vertical-align: top;
        }
        .profile_password
        {
	        width: 400px;
	        margin-left: 5px;
            vertical-align: top;
        }
        .profile_log
        {
	        width: 400px;
	        margin-left: 5px;
            margin-top: 10px;
            vertical-align: top;
        }
        .portal_validation
        {
            color: darkred;
        }
        .portal_validation ul
        {
        	margin-left: 15px;        	
        }
        .user_submit
        {
        	text-align: center;
        }
        .user_field_text1
        {
        	width: 250px;
        }
        .validation_summary
        {
            color: darkred;
        }
        .validation_summary ul
        {
            margin-left: 15px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Master_Content" runat="Server">
    <script type="text/javascript">
        document.body.onload = function () { OnLoad_Function(); }
    </script>
    <div id="default_container">
        <div class="default_left">
            <div class="profile_details">
                <asp:UpdatePanel ID="updatePanel3" runat="server">
                    <ContentTemplate>
                        <asp:Panel ID="pnl_UserDetails" runat="server">
                            <div id="view_credentials" class="user_section">
                                <h2>User Credentials [View-Mode]</h2>
                                <div class="user_line clearfix">
                                    <div class="user_label">
                                        Role:
                                    </div>
                                    <div class="user_field">
                                        <asp:Label ID="lblRole" runat="server" Text=""></asp:Label>
                                    </div>
                                </div>
                                <div class="user_line clearfix">
                                    <div class="user_label">
                                        Email:
                                    </div>
                                    <div class="user_field">
                                        <asp:Label ID="lblEmail" runat="server" Text=""></asp:Label>
                                    </div>
                                </div>
                            </div>
                            <div id="Div2" class="user_section">
                                <h2>User Details</h2>
                                <div class="user_line clearfix">
                                    <div class="user_label">
                                        Prefix:
                                    </div>
                                    <div class="user_field">
                                        <asp:Label ID="lblPrefix" runat="server" Text=""></asp:Label>
                                    </div>
                                </div>
                                <div class="user_line clearfix">
                                    <div class="user_label">
                                        First Name:
                                    </div>
                                    <div class="user_field">
                                        <asp:Label ID="lblFirstName" runat="server" Text=""></asp:Label>
                                    </div>
                                </div>
                                <div class="user_line clearfix">
                                    <div class="user_label">
                                        Middle Name/Initial:
                                    </div>
                                    <div class="user_field">
                                        <asp:Label ID="lblMiddleName" runat="server" Text=""></asp:Label>
                                    </div>
                                </div>
                                <div class="user_line clearfix">
                                    <div class="user_label">
                                        Last Name:
                                    </div>
                                    <div class="user_field">
                                        <asp:Label ID="lblLastName" runat="server" Text=""></asp:Label>
                                    </div>
                                </div>
                                <div class="user_line clearfix user_last">
                                    <div class="user_label">
                                        Suffix:
                                    </div>
                                    <div class="user_field">
                                        <asp:Label ID="lblSuffix" runat="server" Text=""></asp:Label>
                                    </div>
                                </div>
                            </div>
                            <div id="Div3" class="user_section">
                                <h2>User Contact</h2>
                                <div class="user_line clearfix user_last">
                                    <div class="user_label">
                                        Phone:
                                    </div>
                                    <div class="user_field user_phone">
                                        <asp:Label ID="lblPhone" runat="server" Text=""></asp:Label>
                                    </div>
                                </div>
                            </div>
                            <div id="Div4" class="user_section">
                                <h2>User Address</h2>
                                <div class="user_line clearfix">
                                    <div class="user_label">
                                        Address 1:
                                    </div>
                                    <div class="user_field">
                                        <asp:Label ID="lblAddress1" runat="server" Text=""></asp:Label>
                                    </div>
                                </div>
                                <div class="user_line clearfix">
                                    <div class="user_label">
                                        Address 2:
                                    </div>
                                    <div class="user_field">
                                        <asp:Label ID="lblAddress2" runat="server" Text=""></asp:Label>
                                    </div>
                                </div>
                                <div class="user_line clearfix">
                                    <div class="user_label">
                                        Address 3:
                                    </div>
                                    <div class="user_field">
                                        <asp:Label ID="lblAddress3" runat="server" Text=""></asp:Label>
                                    </div>
                                </div>
                                <div class="user_line clearfix">
                                    <div class="user_label">
                                        City:
                                    </div>
                                    <div class="user_field">
                                        <asp:Label ID="lblCity" runat="server" Text=""></asp:Label>
                                    </div>
                                </div>
                                <div class="user_line clearfix">
                                    <div class="user_label">
                                        State:
                                    </div>
                                    <div class="user_field">
                                        <asp:Label ID="lblState" runat="server" Text=""></asp:Label>
                                    </div>
                                </div>
                                <div class="user_line clearfix">
                                    <div class="user_label">
                                        Zip:
                                    </div>
                                    <div class="user_field">
                                        <asp:Label ID="lblZip" runat="server" Text=""></asp:Label>
                                    </div>
                                </div>
                                <div class="user_line clearfix">
                                    <div class="user_label">
                                        Country:
                                    </div>
                                    <div class="user_field">
                                        <asp:Label ID="lblCountry" runat="server" Text=""></asp:Label>
                                    </div>
                                </div>
                            </div>
                            <div id="Div5" class="user_section">
                                <h2>
                                </h2>
                                <div class="user_line clearfix">
                                    <div class="user_label">
                                        User Notes:
                                    </div>
                                    <div class="user_field">
                                        <asp:Label ID="lblNotes" runat="server" Text=""></asp:Label>
                                    </div>
                                </div>
                                <div class="user_line clearfix user_last">
                                    <div class="user_submit">
                                        <asp:Button ID="btnEditData" runat="server" Text="Edit Data" OnClick="User_Details_Edit" />
                                    </div>
                                </div>
                            </div>
                        </asp:Panel>
                        <asp:Panel ID="pnl_UserEdit" runat="server" Visible="false">
                            <div id="user_credentials" class="user_section">
                                <h2>User Credentials [Edit-Mode]</h2>
                                <div class="user_line clearfix">
                                    <div class="user_label">
                                        Role:
                                    </div>
                                    <div class="user_field">
                                        <asp:Label ID="lblRole2" runat="server" Text=""></asp:Label>
                                    </div>
                                </div>
                                <div class="user_line clearfix">
                                    <div class="user_label">
                                        Email:
                                    </div>
                                    <div class="user_field">
                                        <asp:Label ID="lblEmail2" runat="server" Text=""></asp:Label>
                                    </div>
                                </div>
                            </div>
                            <div id="user_details" class="user_section">
                                <h2>
                                    User Details</h2>
                                <div class="user_line clearfix">
                                    <div class="user_label">
                                        Prefix:
                                    </div>
                                    <div class="user_field">
                                        <asp:DropDownList ID="ddlPrefix" runat="server">
                                            <asp:ListItem Value="" Text="Prefix" />
                                            <asp:ListItem Value="Mr." Text="Mr." />
                                            <asp:ListItem Value="Mrs." Text="Mrs." />
                                            <asp:ListItem Value="Ms." Text="Ms." />
                                            <asp:ListItem Value="Miss" Text="Miss" />
                                            <asp:ListItem Value="Prof" Text="Prof" />
                                            <asp:ListItem Value="Ph.D." Text="Ph.D." />
                                            <asp:ListItem Value="Capt" Text="Capt" />
                                            <asp:ListItem Value="Lt." Text="Lt." />
                                            <asp:ListItem Value="D.D.S." Text="D.D.S." />
                                            <asp:ListItem Value="Rev." Text="Rev." />
                                            <asp:ListItem Value="Dr." Text="Dr." />
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <div class="user_line clearfix">
                                    <div class="user_label">
                                        First Name:
                                    </div>
                                    <div class="user_field">
                                        <asp:TextBox ID="FirstName" runat="server" />
                                        <asp:RequiredFieldValidator ValidationGroup="UserEdit" ID="RequiredFieldValidator7"
                                            runat="server" ControlToValidate="FirstName" Text="*" ErrorMessage="First Name is required" SetFocusOnError="false" />
                                    </div>
                                </div>
                                <div class="user_line clearfix">
                                    <div class="user_label">
                                        Middle Name/Initial:
                                    </div>
                                    <div class="user_field">
                                        <asp:TextBox ID="MiddleName" runat="server" />
                                    </div>
                                </div>
                                <div class="user_line clearfix">
                                    <div class="user_label">
                                        Last Name:
                                    </div>
                                    <div class="user_field">
                                        <asp:TextBox ID="LastName" runat="server" />
                                        <asp:RequiredFieldValidator ValidationGroup="UserEdit" ID="RequiredFieldValidator8"
                                            runat="server" ControlToValidate="LastName" Text="*" ErrorMessage="Last Name is required" SetFocusOnError="false" />
                                    </div>
                                </div>
                                <div class="user_line clearfix user_last">
                                    <div class="user_label">
                                        Suffix:
                                    </div>
                                    <div class="user_field">
                                        <asp:DropDownList ID="ddlSuffix" runat="server">
                                            <asp:ListItem Value="" Text="Suffix" />
                                            <asp:ListItem Value="Jr." Text="Jr." />
                                            <asp:ListItem Value="Sr." Text="Sr." />
                                            <asp:ListItem Value="I" Text="I" />
                                            <asp:ListItem Value="II" Text="II" />
                                            <asp:ListItem Value="III" Text="III" />
                                            <asp:ListItem Value="IV" Text="IV" />
                                            <asp:ListItem Value="V" Text="V" />
                                        </asp:DropDownList>
                                    </div>
                                </div>
                            </div>
                            <div id="user_contact" class="user_section">
                                <h2>
                                    User Contact</h2>
                                <div class="user_line clearfix user_last">
                                    <div class="user_label">
                                        Phone:
                                    </div>
                                    <div class="user_field user_phone">
                                        <asp:TextBox ID="AreaCode" runat="server" MaxLength="3" />
                                        <asp:TextBox ID="LocalNumber1" runat="server" MaxLength="3" />
                                        <asp:TextBox ID="LocalNumber2" runat="server" MaxLength="4" />
                                        <asp:RegularExpressionValidator ValidationGroup="UserEdit" ID="RegularExpressionValidator1" runat="server"
                                            Text="*" ErrorMessage="If entering phone, first part must be 3 digits"
                                            ValidationExpression="\w{3,3}"
                                            ControlToValidate="AreaCode"
                                         />
                                        <asp:RegularExpressionValidator ValidationGroup="UserEdit" ID="RegularExpressionValidator2" runat="server"
                                            Text="*" ErrorMessage="If entering phone, second part must be 3 digits"
                                            ValidationExpression="\w{3,3}"
                                            ControlToValidate="LocalNumber1"
                                         />
                                        <asp:RegularExpressionValidator ValidationGroup="UserEdit" ID="RegularExpressionValidator3" runat="server"
                                            Text="*" ErrorMessage="If entering phone, third part must be 4 digits"
                                            ValidationExpression="\w{4,4}"
                                            ControlToValidate="LocalNumber2"
                                         />
                                    </div>
                                </div>
                            </div>
                            <div id="user_address" class="user_section">
                                <h2>
                                    User Address</h2>
                                <div class="user_line clearfix">
                                    <div class="user_label">
                                        Address 1:
                                    </div>
                                    <div class="user_field">
                                        <asp:TextBox ID="Address1" runat="server" Width="250px" />
                                    </div>
                                </div>
                                <div class="user_line clearfix">
                                    <div class="user_label">
                                        Address 2:
                                    </div>
                                    <div class="user_field">
                                        <asp:TextBox ID="Address2" runat="server" Width="250px" />
                                    </div>
                                </div>
                                <div class="user_line clearfix">
                                    <div class="user_label">
                                        Address 3:
                                    </div>
                                    <div class="user_field">
                                        <asp:TextBox ID="Address3" runat="server" Width="250px" />
                                    </div>
                                </div>
                                <div class="user_line clearfix">
                                    <div class="user_label">
                                        City:
                                    </div>
                                    <div class="user_field">
                                        <asp:TextBox ID="City" runat="server" Width="250px" />
                                    </div>
                                </div>
                                <div class="user_line clearfix">
                                    <div class="user_label">
                                        State:
                                    </div>
                                    <div class="user_field">
                                        <asp:DropDownList ID="ddlState" runat="server" Width="250px">
                                            <asp:ListItem Value="State" Text="State" />
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <div class="user_line clearfix">
                                    <div class="user_label">
                                        Zip:
                                    </div>
                                    <div class="user_field">
                                        <asp:TextBox ID="Zip" runat="server" />
                                    </div>
                                </div>
                                <div class="user_line clearfix user_last">
                                    <div class="user_label">
                                        Country:
                                    </div>
                                    <div class="user_field">
                                        <asp:DropDownList ID="ddlCountry" runat="server" Width="300px">
                                            <asp:ListItem Value="Country" Text="Country" />
                                        </asp:DropDownList>
                                    </div>
                                </div>
                            </div>
                            <div id="user_submit" class="user_section">
                                <h2></h2>
                                <div class="user_line clearfix user_last">
                                    <div class="user_submit">
                                        <asp:Button ID="Button5" runat="server" Text="Save Changes" OnClick="User_Details_Submit" ValidationGroup="UserEdit" />
                                        <asp:Button ID="Button6" runat="server" Text="Cancel" OnClick="User_Details_Cancel" />
                                    </div>
                                </div>
                            </div>
                        </asp:Panel>
                        <asp:Panel ID="pnl_UserMessage" runat="server" Visible="false">
                            <asp:Label ID="lbl_UserMessage" runat="server" Text="" />
                            <div class="portal_validation">
                                <asp:ValidationSummary HeaderText="Validation failed, please see below:" ID="ValidationSummary2" runat="server" ValidationGroup="UserEdit" />
                            </div>
                        </asp:Panel>
                        <script type="text/javascript">
                            Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(function (evt, args) { OnLoad_Function(); });
                        </script> 
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </div>
        <div class="default_right">
            <div class="profile_password">
                <asp:UpdatePanel ID="updatePanel1" runat="server">
                    <ContentTemplate>
                        <asp:Panel ID="pnl_PasswordResetStart" runat="server">
                            <asp:Button ID="btnChangePassword" runat="server" Text="Change my Password" OnClick="Password_Reset_Start" />
                        </asp:Panel>
                        <div id="password_reset" class="user_section" runat="server" visible="false">
                            <h2>!! Password Reset Required !!</h2>
                            <div class="user_line clearfix">
                                <div style="color: darkred;font-weight: bold;">
                                    You have a default password,<br /> or your password has been reset by an admin.
                                    <br />You must change your password before you can use this site.
                                </div>
                            </div>
                        </div>
                        <asp:Panel ID="pnl_PasswordResetConfirm" runat="server" Visible="false">
                            <div class="user_section">
                                <h2>Change Password</h2>
                                <div style="margin: 5px;display: inline-block;">
                                    <div>
                                        <div style="display: inline-block;">
                                            <asp:Label ID="Label1" runat="server" Text="Current Password" Width="125px" />
                                            <asp:TextBox ID="Password_Current" runat="server" TextMode="Password" />
                                            <a href="#" onclick="passwordShow(this,'Password_Current');return false;">Show</a>
                                        </div>
                                        <div style="display: inline-block;width: 5px;height: 14px;overflow: hidden;color: darkred;">
                                            <asp:RequiredFieldValidator ControlToValidate="Password_Current" Text="*" ErrorMessage="Current password is required" ID="RequiredFieldValidator4" runat="server" ValidationGroup="PasswordReset" Display="Dynamic" SetFocusOnError="true" />
                                        </div>
                                    </div>
                                    <div style="margin-top: 5px;">
                                        <div style="display: inline-block;">
                                            <asp:Label ID="Label2" runat="server" Text="New Password" Width="125px" />
                                            <asp:TextBox ID="Password_New" runat="server" TextMode="Password" />
                                            <a href="#" onclick="passwordShow(this,'Password_New');return false;">Show</a>
                                        </div>
                                        <div style="display: inline-block;width: 5px;height: 14px;overflow: hidden;color: darkred;">
                                            <asp:RequiredFieldValidator ControlToValidate="Password_New" Text="*" ErrorMessage="New password is required" ID="RequiredFieldValidator5" runat="server" ValidationGroup="PasswordReset" Display="Dynamic" SetFocusOnError="true" />
                                            <asp:CompareValidator ControlToValidate="Password_New" ControlToCompare="Password_Current" Operator="NotEqual" Text="*" ErrorMessage="New password can not match current" ID="CompareValidator2" runat="server" ValidationGroup="PasswordReset" Display="Dynamic" />
                                            <asp:RegularExpressionValidator Text="*" ErrorMessage="Password strength failed:<ul style='margin-left: 25px;'><li>Has at least 2 uppercase letters</li><li>Has 1 special character</li><li>Has at least 2 digits</li><li>Has at least 3 lowercase letters</li><li>Is between 6 and 20 characters long</li></ul>" ID="RegularExpressionValidator5" runat="server" ControlToValidate="Password_New" ValidationExpression="^(?=.*[A-Z].*[A-Z])(?=.*[!@#$&*])(?=.*[0-9].*[0-9])(?=.*[a-z].*[a-z].*[a-z]).{6,20}$" Display="Dynamic" ValidationGroup="PasswordReset" />
                                            <asp:RegularExpressionValidator Text="*" ErrorMessage="Password must have at least 2 uppercase letters" ID="RegularExpressUSE [master]

DECLARE @sp_exportdate datetime = '2017-10-09'-- 00:00:00'
/*
DECLARE @sp_exportdate datetime = '2017-10-09'-- 00:00:00'

DELETE FROM [aabeach].[dbo].[tblEmailExport]
WHERE [strexportdate] = @sp_exportdate

EXECUTE [aabeach].[dbo].[stp_Do_Email_Export]
	@sp_exportdate = '2017-10-09'


*/


--INSERT INTO [dbo].[tblEmailExport_New]
SELECT
[e].[strEmail]
,[e].[strFirstName]
,[e].[strLastName]
,[e].[intID]
,[e].[strServer]
,[e].[strTable]
,[e].[strExportDate]
FROM [aabeach].[dbo].[tblEmailExport] [e] WITH(NOLOCK)
WHERE 1=1
AND [e].[strexportdate] = @sp_exportdate
--AND [e].[strexportdate] = '2016-06-01 00:00:00.000'


                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                     GBLI   a u t o f a i l o v e r . a s p x . c s GBLIJ   o b j \ D e b u g \ \ T e m p o r a r y G e n e r a t e d F i l e _ 5 9 3 7 a 6 7 0 - 0 e 6 0 - 4 0 7 7 - 8 7 7 b - f 7 2 2 1 d a 3 d d a 1 . c s GBLIW   C : \ U s e r s \ C i a m b o t t i \ A p p D a t a \ L o c a l \ T e m p \ . N E T F r a m e w o r k , V e r s i o n = v 4 . 5 . A s s e m b l y A t t r i b u t e s . c s GBLI   C o n n e c t i o n . c s GBLI!   D o n a t e H I H v 2 0 1 7 V i s i t o r L o g . a s m x . c s GBLIJ   o b j \ D e b u g \ \ T e m p o r a r y G e n e r a t e d F i l e _ 0 3 6 C 0 B 5 B - 1 4 8 1 - 4 3 2 3 - 8 D 2 0 - 8 F 5 A D C B 2 3 D 9 2 . c s GBLIJ   o b j \ D e b u g \ \ T e m p o r a r y G e n e r a t e d F i l e _ E 7 A 7 1 F 7 3 - 0 F 8 D - 4 B 9 B - B 5 6 E - 8 E 7 0 B 1 0 B C 5 D 3 . c s GBLI   P r o p e r t i e s \ A s s e m b l y I n f o . c s DELIGBLIb   C : \ P r o g r a m   F i l e s   ( x 8 6 ) \ R e f e r e n c e   A s s e m b l i e s \ M i c r o s o f t \ F r a m e w o r k \ . N E T F r a m e w o r k \ v>
                
            </div>

        </div>
    </div>
    <div class="progressUpdate_wrap">
        <asp:AlwaysVisibleControlExtender ID="AlwaysVisibleControlExtender1" runat="server"
            TargetControlID="Panel4"
            VerticalSide="Middle"
            HorizontalSide="Left"
            HorizontalOffset="10"
            ScrollEffectDuration=".1"
            />        
        <asp:Panel ID="Panel4" runat="server">
            <asp:UpdateProgress ID="UpdateProgress1" rv 4 . 5 \ S y s t e m . W e b . d l l    g l o b a l        GBLIl   C : \ P r o g r a m   F i l e s   ( x 8 6 ) \ R e f e r e n c e   A s s e m b l i e s \ M i c r o s o f t \ F r a m e w o r k \ . N E T F r a m e w o r k \ v 4 . 5 \ S y s t e m . C o n f i g u r a t i o n . d l l    g l o b a l        GBLIc   C : \ P r o g r a m   F i l e s   ( x 8 6 ) \ R e f e r e n c e   A s s e m b l i e s \ M i c r o s o f t \ F r a m e w o r k \ . N E T F r a m e w o r k \ v 4 . 5 \ S y s t e m . D a t a . d l l    g l o b a l        GBLIf   C : \ P r o g r a m   F i l e s   ( x 8 6 ) \ R e f e r e n c e   A s s e m b l i e s \ M i c r o s o f t \ F r a m e w o r k \ . N E T F r a m e w o r k \ v 4 . 5 \ S y s t e m . D r a w i n g . d l l    g l o b a l        GBLIm   C : \ P r o g r a m   F i l e s   ( x 8 6 ) \ R e f e r e n c e   A s s e m b l i e s \ M i c r o s o f t \ F r a m e w o r k \ . N E T F r a m e w o r k \ v 4 . 5 \ S y s t e m . W e b . E x t e n s i o n s . d l l    g l o b a l        GBLIb   C : \ P r o g r a m   F i l e s   ( x 8 6 ) \ R e f e r e n c e   A s s e m b l i e s \ M i c r o s o f t \ F r a m e w o r k \ . N E T F r a m e w o r k \ v 4 . 5 \ S y s t e m . X m l . d l l    g l o b a l        GBLI^   C : \ P r o g r a m   F i l e s   ( x 8 6 ) \ R e f e r e n c e   A s s e m b l i e s \ M i c r o s o f t \ F r a m e w o r k \ . N E T F r a m e w o r k \ v 4 . 5 \ S y s t e m . d l l    g l o b a l        GBLIk   C : \ P r o g r a m   F i l e s   ( x 8 6 ) \ R e f e r e n c e   A s s e m b l i e s \ M i c r o s o f t \ F r a m e w o r k \ . N E T F r a m e w o r k \ v 4 . 5 \ S y s t e m . W e b . S e r v i c e s . d l l    g l o b a l        GBLIq   C : \ P r o g r a m   F i l e s   ( x 8 6 ) \ R e f e r e n c e   A s s e m b l i e s \ M i c r o s o f t \ F r a m e w o r k \ . N E T F r a m e w o r k \ v 4 . 5 \ S 