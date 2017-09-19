<%@ Page language="C#"   Inherits="Microsoft.SharePoint.Publishing.PublishingLayoutPage,Microsoft.SharePoint.Publishing,Version=15.0.0.0,Culture=neutral,PublicKeyToken=71e9bce111e9429c" %>
  
 <%@ Register Tagprefix="SharePointWebControls" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
 <%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
 <%@ Register Tagprefix="PublishingWebControls" Namespace="Microsoft.SharePoint.Publishing.WebControls" Assembly="Microsoft.SharePoint.Publishing, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
 <%@ Register Tagprefix="PublishingNavigation" Namespace="Microsoft.SharePoint.Publishing.Navigation" Assembly="Microsoft.SharePoint.Publishing, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
  
 <asp:Content ContentPlaceholderID="PlaceHolderPageTitle" runat="server">
     <SharePointWebControls:FieldValue id="PageTitle" FieldName="Title" runat="server"/>
 </asp:Content>
 <asp:Content ContentPlaceholderID="PlaceHolderMain" runat="server">
  
 <ui>
      
 <li ><span style="color:red" >Article Author : </span></li><SharePointWebControls:TextField ID="Author" FieldName="5511034a-38c6-4861-9d00-1633cc111ca3" runat="server"></SharePointWebControls:TextField>
 <li ><span style="color:red" >Article Body : </span></li><SharePointWebControls:NoteField ID="Body" FieldName="93bb1031-87a2-4f84-bddb-3a9d94d197d1" runat="server"></SharePointWebControls:NoteField>
 <li ><span style="color:red" >Tags : </span></li><SharePointWebControls:CheckBoxChoiceField ID="Tags" FieldName="9736f0e4-9c76-4ff7-bf33-1547ac53553c" runat="server"></SharePointWebControls:CheckBoxChoiceField>
  
 </ui>
  
 </asp:Content>