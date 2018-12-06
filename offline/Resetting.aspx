<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage_Offline.master" AutoEventWireup="true" CodeFile="Resetting.aspx.cs" Inherits="offline_Resetting" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" Runat="Server">
    <link href="../css/content_general.css" rel="stylesheet" type="text/css" />
    <link href="../css/user.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Master_Content" Runat="Server">
    <div id="default_container">
        <h2>Password Resetting Tool</h2>
        <asp:Panel ID="panel_Reset" runat="server">
            <div class="user_box">
                <div class="user_standard600">
                    <div class="user_section">
                        <h2>Enter your information</h2>
                        <div class="user_line clearfix">
                            <div class="user_label">
                                Username
                            </div>
                            <div class="user_field">
                                <asp:TextBox ID="Username" runat="server" Width="300px" />
                                <asp:RequiredFieldValidator ValidationGroup="ValidationSummary1" ID="RequiredFieldValidator1"
                                    runat="server" ControlToValidate="UserName" Text="*" ErrorMessage="Username is required" SetFocusOnError="false" />
                            </div>
                        </div>
                        <div class="user_line clearfix">
                            <div class="user_label">
                                New Password
                            </div>
                            <div class="user_field">
                                <asp:TextBox ID="Password" runat="server" TextMode="Password" />
                                <asp:RequiredFieldValidator ValidationGroup="ValidationSummary1" ID="RequiredFieldValidator2"
                                    runat="server" ControlToValidate="Password" Text="*" ErrorMessage="Password is required" SetFocusOnError="false" />
                                <asp:PasswordStrength ID="PasswordStrength1" runat="server" TargetControlID="Password"
                                    PreferredPasswordLength="10" MinimumLowerCaseCharacters="2" MinimumUpperCaseCharacters="2"
                                    MinimumNumericCharacters="1" MinimumSymbolCharacters="1" />
                            </div>
                        </div>
                        <div class="user_line clearfix">
                            <div class="user_label">
                                Confirm Password
                            </div>
                            <div class="user_field">
                                <asp:TextBox ID="Password_Retype" runat="server" TextMode="Password" />
                                <asp:RequiredFieldValidator ValidationGroup="ValidationSummary1" ID="RequiredFieldValidator3"
                                    Display="Dynamic" runat="server" ControlToValidate="Password_Retype" Text="*"
                                    ErrorMessage="Must verify password" SetFocusOnError="false" />
                                <asp:CompareValidator ID="CompareValidator1" runat="server" ValidationGroup="ValidationSummary1"
                                    Text="*" ErrorMessage="You must re-type the correct password." ControlToValidate="Password_Retype"
                                    ControlToCompare="Password" SetFocusOnError="false" />
                            </div>
                        </div>
                        <div class="user_line clearfix">
                            <div class="user_submit">
                                <asp:Button ID="Button1" runat="server" Text="Reset Password" Width="125px" ValidationGroup="ValidationSummary1" OnClick="Resetting_Submit" />
                                <asp:Button ID="Button2" runat="server" Text="Cancel" Width="125px" />
                                <asp:HiddenField ID="HiddenField1" runat="server" />
                            </div>
                            <div>
                                <asp:ValidationSummary ID="ValidationSummary1" runat="server" ValidationGroup="ValidationSummary1"
                                ForeColor="DarkRed" DisplayMode="BulletList" ShowMessageBox="false" HeaderText="Validation failed" />
                            </div>
                        </div>
                        <div class="user_line clearfix user_last">
                            <div class="user_info">
                                <asp:Label ID="Label1" runat="server" Text="Label"></asp:Label>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div style="margin-bottom: 25px;">&nbsp;</div>
        </asp:Panel>
        <asp:Panel ID="panel_Done" runat="server">
            <div class="user_box">
                <div class="user_standard600">
                    <div class="user_section">
                        <h2>
                            <asp:Label ID="Label2" runat="server">
                                Success
                            </asp:Label>
                        </h2>
                        <div class="user_line user_info">
                            <asp:Label ID="Label3" runat="server">
                                Your password has been successfully reset.
                            </asp:Label>
                            <asp:Label ID="Label4" runat="server" Text="Label">
                                Please proceed to login screen.
                            </asp:Label>
                        </div>
                    </div>
                </div>
            </div>
        </asp:Panel>
        <asp:Panel ID="panel_Error" runat="server">
            <div class="user_box">
                <div class="user_standard600">
                    <div class="user_section">
                        <h2>
                            <asp:Label ID="lbl_ErrorTitle" runat="server">
                                Link Expired
                            </asp:Label>
                        </h2>
                        <div class="user_line user_info">
                            <asp:Label ID="lbl_Error1" runat="server">
                                The link you are attempting to use has already expired.
                            </asp:Label>
                            <asp:Label ID="lbl_Error2" runat="server" Text="Label">
                                Please visit the login page to reset your password.
                            </asp:Label>
                        </div>
                    </div>
                </div>
            </div>
        </asp:Panel>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder2" Runat="Server">
</asp:Content>

