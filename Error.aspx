<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage_Offline.master" AutoEventWireup="true" CodeFile="Error.aspx.cs" Inherits="Error" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" Runat="Server">
    <link href="../css/content_general.css" rel="stylesheet" type="text/css" />
    <link href="../css/user.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Master_Content" Runat="Server">
    <div id="default_container">
        <h2>Web Site Error</h2>
        There was a server error processing your request.
        <br />A message has been sent to the web master.
        <br />
        <br />You can try again if this is the first time the error has happened.
        <br />If you continue to experiences issues, you can contact the web master:
        <br /><a href="mailto:nciambotti@patriotllc.com">Web Master</a>
        <br />Use the home link at top to try again,
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder2" Runat="Server">
</asp:Content>

