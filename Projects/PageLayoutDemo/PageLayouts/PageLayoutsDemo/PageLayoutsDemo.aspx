<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>


<%@ Page language="C#"   Inherits="PageLayoutDemo.PageLayouts.PageLayoutsDemo.PageLayoutsDemoCode, $SharePoint.Project.AssemblyFullName$" %>
 <%@ Register Tagprefix="SharePointWebControls" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
 <%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
 <%@ Register Tagprefix="PublishingWebControls" Namespace="Microsoft.SharePoint.Publishing.WebControls" Assembly="Microsoft.SharePoint.Publishing, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
 <%@ Register Tagprefix="PublishingNavigation" Namespace="Microsoft.SharePoint.Publishing.Navigation" Assembly="Microsoft.SharePoint.Publishing, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
  
 <asp:Content ContentPlaceholderID="PlaceHolderPageTitle" runat="server">
     <SharePointWebControls:FieldValue id="PageTitle" FieldName="Title" runat="server"/>
 </asp:Content>


 <asp:Content ContentPlaceholderID="PlaceHolderMain" runat="server"> 
      
         <WebPartPages:SPProxyWebPartManager runat="server" id="ProxyWebPartManager"></WebPartPages:SPProxyWebPartManager>
 
    <table cellpadding="1" cellspacing="0" border="0" width="100%">
        <PublishingWebControls:EditModePanel runat=server id="ContentEditModePanel" SuppressTag="True">
            <tr>
                <td width="50%" valign="Top">
                    <SharePointWebControls:TextField ID="TextField2" FieldName="Title" runat="server">
                        </SharePointWebControls:TextField>
                </td>
                <td width="50%" valign="Top">
                    <PublishingWebControls:RichImageField ID="RichImageField1" FieldName="PublishingRollupImage" runat="server">
                        </PublishingWebControls:RichImageField>
                </td>
            </tr>
        </PublishingWebControls:EditModePanel>
 
        <tr>
            <td width="50%" valign="Top">
                <PublishingWebControls:RichHtmlField ID="RichHtmlField2" FieldName="PublishingPageContent" runat="server">
                </PublishingWebControls:RichHtmlField>
            </td>
            <td width="50%" valign="Top">
                <div class="webpart_zone">
                    <WebPartPages:WebPartZone ID="TopZone" runat="server" Title="Top"
                            AllowPersonalization="False" AllowCustomization="False" AllowLayoutChange="False">
                        <ZoneTemplate></ZoneTemplate></WebPartPages:WebPartZone>
                </div>
 
                <div class="webpart_zone">
                    <WebPartPages:WebPartZone ID="BottomZone" runat="server" Title="Bottom"
                            AllowPersonalization="False">
                        <ZoneTemplate></ZoneTemplate></WebPartPages:WebPartZone>
                </div>
            </td>
        </tr>

        

    </table>


        <PublishingWebControls:EditModePanel runat="server" id="EditModePanel1" PageDisplayMode="Display">
             <ui>      
             <li ><span style="color:red" >Article Author : </span></li><SharePointWebControls:TextField ID="TextField1" FieldName="5511034a-38c6-4861-9d00-1633cc111ca3" runat="server"></SharePointWebControls:TextField>
             <li ><span style="color:red" >Article Body : </span></li><SharePointWebControls:NoteField ID="NoteField1" FieldName="93bb1031-87a2-4f84-bddb-3a9d94d197d1" runat="server"></SharePointWebControls:NoteField>
             <li ><span style="color:red" >Tags : </span></li><SharePointWebControls:CheckBoxChoiceField ID="CheckBoxChoiceField1" FieldName="9736f0e4-9c76-4ff7-bf33-1547ac53553c" runat="server"></SharePointWebControls:CheckBoxChoiceField>  
             </ui>
  
        <SharePointWebControls:FieldValue ID="FieldValue1" FieldName="PublishingPageContent" runat="server"/>
        <br/><asp:TextBox runat="server" id="TextBox1"></asp:TextBox>
        <asp:Button runat="server" Text="Get Time" id="Button1" OnClick="GetTimeButton_Click"></asp:Button>
        <asp:Button runat="server" Text="Click Me" id="Button2" OnClick="SayHelloButton_Click"></asp:Button>
    </PublishingWebControls:EditModePanel>
            
    <PublishingWebControls:EditModePanel runat="server" id="EditModePanel2" PageDisplayMode="Edit">
        <PublishingWebControls:RichHtmlField FieldName="PublishingPageContent" runat="server" id="RichHtmlField1"/>
    </PublishingWebControls:EditModePanel>
    
  <div> <asp:Label runat="server" ID="Label1" Text="Page Not Loaded"></asp:Label>  </div>
       <div> <asp:Label runat="server" ID="Label2" Text="Page Not Loaded"></asp:Label>  </div>

 </asp:Content>

