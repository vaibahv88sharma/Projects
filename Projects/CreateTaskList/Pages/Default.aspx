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
<br></br>
	<div>
		
	</div>	
<br></br>
	<div>
		<table>
		  <tr>
		    <td><b>Enter Task List Name:</b></td>
			<td>
				<select id="mySelect">
				    <option></option>
				</select>
			</td>
		    <td><input type="button" value="Create List" id="btn-new" /></td>
		  </tr>
		</table>
	</div>
<br></br>
    <div>
		<table>
		  <tr>
		    <td><b>Tasks will be created for:</b></td>
			<td>
				<input id="text2" type="text" name="itemName" placeholder="No Item Selected" readonly>
			</td>		
		    <td rowspan="3"><input type="button" value="Create List Items" id="btn-new-items" /></td>
		  </tr>
		  <tr>
		    <td><b>Select Site Supervisor of Tasks:</b></td>
		    <td>
				<select id="mySupervisorSelect">
				    <option></option>
				</select>				
			</td>			
		  </tr>		  
		  <tr>
		    <td><b>Select Category of Tasks:</b></td>
		    <td>
				<select id="myCategorySelect">
				    <option></option>
				</select>				
			</td>			
		  </tr>  		  
		</table>		
    </div>

</asp:Content>
