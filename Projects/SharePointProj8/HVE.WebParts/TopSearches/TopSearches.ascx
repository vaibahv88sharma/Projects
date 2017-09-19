﻿<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TopSearches.ascx.cs" Inherits="SharePointProj8.HVE.WebParts.TopSearches.TopSearches" %>



<h2>Top Searches</h2>
<div>
    <asp:Literal ID="LiteralText" runat="server" Text=""></asp:Literal>
    <asp:Literal ID="Literal1" runat="server" Text=""></asp:Literal>
</div>

<div id="div1">
</div>


<asp:GridView ID="GridView1" runat="server" OnRowDataBound="GridView1_RowDataBound">
</asp:GridView>
<asp:GridView ID="GridView2" runat="server" OnRowDataBound="GridView2_RowDataBound">
</asp:GridView>