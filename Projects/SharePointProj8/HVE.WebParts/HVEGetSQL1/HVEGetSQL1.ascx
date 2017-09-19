<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="HVEGetSQL1.ascx.cs" Inherits="Proj1.HVE.WebParts.HVEGetSQL1.HVEGetSQL1" %>



<asp:GridView ID="GridView1" runat="server">
</asp:GridView>


<asp:Repeater ID="Repeater1" runat="server">
    <HeaderTemplate>
        <div style="font-weight: bold;">Top searches</div>        
    </HeaderTemplate>
    <ItemTemplate>  
        <%--<asp:Label   runat="server" Text='<%#Eval("queryString")%>'></asp:Label>--%>
        <a href='<%#Eval("queryString")%>' title='<%#Eval("queryString")%>'><%#Eval("queryString")%></a>
    </ItemTemplate>
</asp:Repeater>
<br />
<asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="Button" />
<br />
<asp:Label ID="Label1" runat="server" Text="Label"></asp:Label>
<p>
    &nbsp;</p>
<asp:Label ID="Label2" runat="server" Text="Label"></asp:Label>

<asp:Label ID="Label3" runat="server" Text="Label"></asp:Label>
<asp:Label ID="Label4" runat="server" Text="Label"></asp:Label>
<asp:Label ID="Label5" runat="server" Text="Label"></asp:Label>
<asp:Label ID="Label7" runat="server" Text="Label"></asp:Label>
<asp:Label ID="Label6" runat="server" Text="Label"></asp:Label>
<asp:Label ID="Label8" runat="server" Text="Label"></asp:Label>
<asp:Label ID="Label9" runat="server" Text="Label"></asp:Label>
<asp:Label ID="Label10" runat="server" Text="Label"></asp:Label>

<div id ="Table1">
    <ul>
        <li id ="Row1">><%#Eval("queryString")%></li>
    </ul>
</div>
<div>
<asp:Literal ID="LiteralText" runat="server" Text="This is example of Literal"></asp:Literal>
    </div>
<div>
    <asp:Label ID="Label11" runat="server" Text='<%#Eval("linkVal")%>'></asp:Label>
</div>

