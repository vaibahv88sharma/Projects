﻿<%-- The following 4 lines are ASP.NET directives needed when using SharePoint components --%>

<%@ Page Inherits="Microsoft.SharePoint.WebPartPages.WebPartPage, Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" MasterPageFile="~masterurl/default.master" Language="C#" %>

<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<%-- The markup and script in the following Content element will be placed in the <head> of the page --%>
<asp:Content ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
    <script type="text/javascript" src="https://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.9.1.min.js"></script>
    <script type="text/javascript" src="/_layouts/15/sp.runtime.js"></script>
    <script type="text/javascript" src="/_layouts/15/sp.js"></script>

    <!-- Add your CSS styles to the following file -->
    <link rel="Stylesheet" type="text/css" href="../Content/App.css" />

    <!-- Add your JavaScript to the following file -->
    <script type="text/javascript" src="../Scripts/App.js"></script>
    <script type="text/javascript" src="../Scripts/JSOMScript.js"></script>    
    <script type="text/javascript" src="../Scripts/JSONExceptionHandling.js"></script>     
    <script type="text/javascript" src="../Scripts/GlobalFunctions.js"></script> 
</asp:Content>

<%-- The markup in the following Content element will be placed in the TitleArea of the page --%>
<asp:Content ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server">
    Page Title
</asp:Content>

<%-- The markup and script in the following Content element will be placed in the <body> of the page --%>
<asp:Content ContentPlaceHolderID="PlaceHolderMain" runat="server">

    <div>
        <p id="message">
            <!-- The following content will be replaced with the user name when you run the app - see App.js -->
            initializing...
        </p>
    </div>
    <table>
    <tr>
    <td>
    <table>
        <tr>
            <td>Category Id</td>
            <td>
                <input type="text" id="CategoryId" class="c1"/>
            </td>
            </tr>
            <tr>
            <td>Category Name</td>
            <td>
                <input type="text" id="CategoryName" class="c1"/>
            </td>
            </tr>
            <tr>
            <td>
                <input type="button" value="New" id="btn-new" />
            </td>
            <td>
                <input type="button" value="Add" id="btn-add" />
            </td>
            <td>
                <input type="button" value="Update" id="btn-update" />
            </td>
            <td>
                <input type="button" value="Delete" id="btn-delete" />
            </td>
            <td>
                <input type="button" value="Find" id="btn-find" />
            </td>
        </tr> 
        <tr>
            <td>
                <input type="text" id="CategoryIdofNewList"/>
            </td>
            <td>
                <input type="button" value="Find" id="btn-findNewList" />
            </td>
        </tr>   
    </table>
    </td>
    <td>
    <table id="tblcategories">
 
    </table>
    </td>
    </tr>
</table>
<div id="dvMessage"></div>
</asp:Content>
