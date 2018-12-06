<%@ Page Title="Reporting Dashboard" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<%@ MasterType TypeName="MasterPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" Runat="Server">
    <link href="css/content_general.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Master_Content" Runat="Server">
    <script type="text/javascript">
    </script>
    <div id="default_container">
        Dashboard
        <div>
            Donation Query
        </div>
        <div>
            Donation Details
        </div>
        <div>
            Donation Entry
        </div>
        <div>
            Donation Stats
        </div>
        <div>
            Donor Services Stats
        </div>
        <div>
            <asp:Label ID="Label1" runat="server" />
        </div>
        <asp:Panel ID="Panel1" runat="server" Visible="false">
            <div style="color: Red;font-weight: bold">
                <br />
                <h3>IMPORTANT NOTICE</h3>
                <br />If you are a Manager+ you will see a "Client" menu with a lot of reports listed.
                <br />A lot of those reports are not yet activated and are not yet finished if they are even there.
                <br />Unless it is a report you have been told about, don't expect it to be working properly.
                <br />This message is only displayed for Managers, Administrators, and Sys Admins.
            </div>
        </asp:Panel>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder2" Runat="Server">
    <div>
    </div>
</asp:Content>

