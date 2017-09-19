<%-- The following 4 lines are ASP.NET directives needed when using SharePoint components --%>

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
	<script type="text/javascript" src="../Scripts/App1.js"></script>
	<script type="text/javascript" src="../Scripts/App2.js"></script>
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
		<p id="message1">
		</p>
		<p id="message2">
		</p>
	<p id="message3">
		</p>
	
    </div>

<asp:Button ID="Button1" runat="server" Text="Create List"  OnClientClick="createSPList(); return false;" />
<asp:Button ID="Button2" runat="server" Text="Update List Item"  OnClientClick="Update(); return false;" />    
<asp:Button ID="Button2a" runat="server" Text="Add List Item"  OnClientClick="addListItem(); return false;" />
<asp:Button ID="Button3" runat="server" Text="Delete List"  OnClientClick="Test(); return false;" />    
<asp:Button ID="Button4a" runat="server" Text="Get List using SP.RequestExecutor.js"  OnClientClick="getListUsingSPRequestExecutorJS(); return false;" />
<asp:Button ID="Button4" runat="server" Text="Get List"  OnClientClick="getLists1(); return false;" />
<asp:Button ID="Button5" runat="server" Text="Get List"  OnClientClick="getLists(); return false;" />
<asp:Button ID="Button6" runat="server" Text="Get List"  OnClientClick="getListTitle(); return false;" />
<asp:Button ID="Button7" runat="server" Text="Get Lookup List"  OnClientClick="getLookUpList(); return false;" />

</asp:Content>
