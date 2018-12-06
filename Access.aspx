<%@ Page Title="Portal - Access Restricted" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Access.aspx.cs" Inherits="Access" %>
<%@ MasterType TypeName="MasterPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" Runat="Server">
    <link href="css/content_general.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
    .portal_access_restricted
    {
    	width: 600px;
    	margin-bottom: 5px;
    	border: solid 1px white;
    	margin-right: 5px;
    	
    }
    .portal_access_restricted h3
    {
    	background-color: #80A2D0;
    	width: 600px;
    	text-align: left;
    	margin-bottom: 10px;
    	border-bottom: solid 1px white;
    	text-indent: 5px;
    }
    .portal_access_restricted_content
    {
    	margin-left: 5px;
    	margin-bottom: 5px;
    	color: DarkRed;
    	font-weight: bold;
    }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Master_Content" Runat="Server">
    <div id="default_container">
        <div class="portal_access_restricted">
            <h3>Authorization Restriction</h3>
            <div class="portal_access_restricted_content">
                You are not authorized to access the page you requested.
                <br />Please go back to the main menu and try again.
                <br />If you continue to receive this message, please contact your account representative.
                <br />
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder2" Runat="Server">
</asp:Content>

