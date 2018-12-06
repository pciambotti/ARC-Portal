<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="Login" %>

<%--<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>--%>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Portal - Access</title>
    <link href="css/login-box.css?v=<%=ghFunctions.portalVersion %>" rel="stylesheet" type="text/css" />
    <script src="js/jquery-1.7.1.min.js?v=<%=ghFunctions.portalVersion %>" type="text/javascript"></script>
    <link href="js/jquery.countdown.css?v=<%=ghFunctions.portalVersion %>" rel="stylesheet" type="text/css" />
    <script src="js/jquery.plugin.min.js?v=<%=ghFunctions.portalVersion %>" type="text/javascript"></script>
    <script src="js/jquery.countdown.min.js?v=<%=ghFunctions.portalVersion %>" type="text/javascript"></script>
    <script src="js/jquery-gh-custom-objects.js?v=<%=ghFunctions.portalVersion %>" type="text/javascript"></script>
    <script type="text/javascript">
        function OnLoad_Function() {
            $(document).ready(function() {
                var d = new Date();
                //date.getDate
                var date_str = ('0' + (d.getMonth() + 1)).substr(-2, 2)
                    + '/' + ('' + d.getDate()).substr(-2, 2)
                    + '/' + d.getFullYear()
                    + ' ' + d.getHours()
                    + ':' + d.getMinutes()
                    + ':' + d.getSeconds()
                    ;
                $("#<%=UserStart.ClientID %>").val(date_str);
            });
        }
        function ValidateEnd(btn,lbl) {
            $(document).ready(function() {
                var i;for (i = 0; i < Page_Validators.length; i++) {ValidatorValidate(Page_Validators[i]);}
                if (!Page_IsValid){
                    $(btn).val(lbl);
                }
            });
        }
    </script>
</head>
<body onload="javascript:OnLoad_Function();">
    <form id="form1" runat="server">
        <script type="text/javascript">
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
        <%--<asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server" />--%>
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
        <div class="masterContainer">
            <div id="pageContainer">
                <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                    <ContentTemplate>
                        <asp:Panel ID="Panel1" runat="server" Visible="true">
                            <div id="login-box">
                                <h2>Login</h2>
                                <div style="height: 35px;">
                                    Login to Portal - Reporting Module
                                </div>
                                <div style="height: 110px;">
                                    <div class="login-box-name" style="margin-top: 20px;">
                                        Email:
                                    </div>
                                    <div class="login-box-field" style="margin-top: 20px;">
                                        <asp:TextBox ID="Username" runat="server" CssClass="form-login" MaxLength="100" ForeColor="White" Font-Bold="true" BackColor="#1E4F8A" />
                                        <asp:RequiredFieldValidator ValidationGroup="ValidationSummary1" ID="RequiredFieldValidator1"
                                            runat="server" ControlToValidate="Username" Text="*" ErrorMessage="Username is required" />
                                    </div>
                                    <div class="login-box-name">
                                        Password:
                                    </div>
                                    <div class="login-box-field">
                                        <asp:TextBox ID="Password" runat="server" CssClass="form-login" TextMode="Password" ForeColor="White" Font-Bold="true" BackColor="#1E4F8A" MaxLength="40" />
                                        <asp:RequiredFieldValidator ValidationGroup="ValidationSummary1" ID="RequiredFieldValidator2"
                                            runat="server" ControlToValidate="Password" Text="*" ErrorMessage="Password is required" />
                                    </div>
                                </div>
                                <div class="login-box-options">
                                    <div style="float: left;width: 100px;">
                                        <asp:CheckBox ID="chkPersistCookie" runat="server" /> Remember Me
                                    </div>
                                    <asp:LinkButton ID="LinkButton1" runat="server" Style="margin-left: 30px;" OnClick="ForgotPassword_Click">Forgot password?</asp:LinkButton>
                                </div>
                                <br />
                                <asp:Button ID="Button1" runat="server" Text="LOGIN" CssClass="login-box-submit" ValidationGroup="ValidationSummary1" OnClick="Login_Submit" OnClientClick="this.value = 'Processing...';ValidateEnd(this,'LOGIN');" />
                                <div style="text-align: left;">
                                    <asp:ValidationSummary ID="ValidationSummary1" runat="server" ValidationGroup="ValidationSummary1" ForeColor="White" />
                                    <asp:HiddenField ID="UserStart" runat="server" />
                                </div>
                            </div>
                        </asp:Panel>
                        <asp:Panel ID="Panel2" runat="server" Visible="false">
                            <div id="retrieve-box">
                                <h2>Retrieve</h2>
                                <div style="height: 35px;">
                                Please enter your username or email; password resetting instructions will be emailed to the registered email address.
                                </div>
                                <div style="height: 110px;">
                                    <div class="login-box-name" style="margin-top: 20px;">
                                        Email:
                                    </div>
                                    <div class="login-box-field" style="margin-top: 20px;">
                                        <asp:TextBox ID="Username_Retrieve" runat="server" CssClass="form-retrieve" MaxLength="100" />
                                        <asp:RequiredFieldValidator ValidationGroup="ValidationSummary2" ID="RequiredFieldValidator3"
                                            runat="server" ControlToValidate="Username_Retrieve" Text="*" ErrorMessage="Username is required, hit cancel if you've changed your mind." />
                                    </div>
                                    <div>
                                        Your password is stored encrypted; retrieving indicates you will reset it.                            
                                    </div>
                                </div>
                                <div class="login-box-options">
                                    <div style="float: left;width: 100px;">
                                        &nbsp;
                                    </div>
                                    <asp:LinkButton ID="LinkButton2" runat="server" Style="margin-left: 30px;" OnClick="ForgotPassword_Cancel">Cancel</asp:LinkButton>
                                </div>
                                <br />
                                <asp:Button ID="Button2" runat="server" Text="" CssClass="login-box-retrieve" ValidationGroup="ValidationSummary2" OnClick="ForgotPassword_Submit" />
                                <div style="text-align: left;">
                                    <asp:ValidationSummary ID="ValidationSummary2" runat="server" ValidationGroup="ValidationSummary2" ForeColor="White" />
                                    <asp:Label ID="Label2" runat="server" Text="" />
                                </div>
                            </div>
                        </asp:Panel>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
            <div id="pageContainer2">
                <div id="news-box">
                    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                        <ContentTemplate>
                            <div id="news-content">
                                <div id="news_content_inner" runat="server" class="news-content-inner">
                                    Welcome to the Portal - Reporting Tool
                                    <br />Powered by Patriot Communications
                                    <br />
                                    <br />Enter your credentials to the left.
                                    <br />If you need a username and password, contact your account manager.
                                    <br />
                                    <br />If you have problems logging in, contact your account manager, or use the password retrieval tool.
                                    <br />
                                    <br />Mode: <asp:Label ID="Label3" runat="server" />
                                    <br />Database: <asp:Label ID="DBMode" runat="server" />
                                    <asp:Panel ID="Panel3" runat="server" Visible="false">
                                        <br />Test Links:
                                        <br /><asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/Default.aspx" Text="Default" />
                                        <br /><asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl="~/Event_Add.aspx" Text="Event_Add" />
                                        <br /><asp:HyperLink ID="HyperLink3" runat="server" NavigateUrl="~/Event_List.aspx" Text="Event_List" />
                                        <br /><asp:HyperLink ID="HyperLink4" runat="server" NavigateUrl="~/Event_Attendee_List.aspx" Text="Event_Attendee_List" />
                                        <br /><asp:HyperLink ID="HyperLink5" runat="server" NavigateUrl="~/Event_Attendee_Add.aspx" Text="Event_Attendee_Add" />
                                        <br /><asp:HyperLink ID="HyperLink6" runat="server" NavigateUrl="~/User_Add.aspx" Text="User_Add" />
                                        <br /><asp:HyperLink ID="HyperLink7" runat="server" NavigateUrl="~/User_List.aspx" Text="User_List" />
                                        <br /><asp:HyperLink ID="HyperLink8" runat="server" NavigateUrl="~/Error.aspx" Text="Error" />
                                        <br /><asp:HyperLink ID="HyperLink9" runat="server" NavigateUrl="~/SystemAdministrator.aspx" Text="SystemAdministrator" />
                                    </asp:Panel>
                                </div>
                                <div id="login_message" runat="server" class="news-content-inner">
                                    <div>
                                        <asp:Label ID="lblLoginMessage" runat="server" Text="" />
                                    </div>
                                </div>
                                <div id="maintenance_mode" runat="server" class="news-content-inner" visible="false">
                                    Portal currently in maintenance mode.
                                    <br /><span style="color: Red;">Regular users will NOT be have access.</span>
                                </div>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>
        </div>
        <div class="masterContainer">
            <div class="progressUpdate_wrap">
                <%--<asp:AlwaysVisibleControlExtender ID="AlwaysVisibleControlExtender1" runat="server"
                    TargetControlID="UpdateProgress1"
                    VerticalSide="Middle"
                    HorizontalSide="Left"
                    HorizontalOffset="10"
                    ScrollEffectDuration=".1"
                    />--%>
                          
                <%--Not showing on Chrome..
                http://blog.lavablast.com/post/2008/10/20/Gotcha-WebKit-(Safari-3-and-Google-Chrome)-Bug-with-ASPNET-AJAX.aspx--%>

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
        </div>
    </form>
</body>
</html>
