<%@ Page Language="C#" MasterPageFile="~/template.master" AutoEventWireup="true"
    ValidateRequest="false" CodeFile="DVC.aspx.cs" Inherits="VersionControl" Title="" %>
<asp:Content ID="Content1" ContentPlaceHolderID="leftContentPlaceHolder" Runat="Server">
     <table border="0"  width="100%" cellpadding="0" cellspacing="0" align="left">
        <tr>
            <td align="center">
		        <asp:Label ID="lblCaption" skinID="PageCaption" runat="server"></asp:Label>
   	        </td>
        </tr>
        <tr>
            <td>
                <table border="0" width="100%" bgcolor="#CECECE" cellpadding="0" cellspacing="0" align="left">
                <tr>
                    <td width="2%" valign="top">
                        <asp:Image ID="errorImg" runat="server" ImageUrl="~/Images/error.png" Visible="false" />
	                </td>
	                <td width="95%">
	                    <asp:Label ID="lblError" runat="server" Visible="false" Width="100%"></asp:Label>
	                </td>
	            </tr>
                </table>
            </td>
        </tr>
        <!-- Grid View for activity log -->
        <tr>
            <td colspan="2">
            <asp:SqlDataSource ID="ActivityDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:authStore %>"
                SelectCommand="SELECT 'Recent activity' as Comment, [Message] FROM aspnet_WebEvent_Events WHERE EventTime = (select MAX(EventTime) FROM aspnet_WebEvent_Events WHERE EventType = 'BCC.Core.BCCWebAuditEvent' and EventCode = 100504)">
            </asp:SqlDataSource>

 	        <asp:GridView id="UserActivityGrid" Visible="false" runat="server" Width="100%" ForeColor="#333333"
                    CellPadding="4" CellSpacing="0" AutoGenerateColumns="False" DataSourceID="ActivityDataSource"
	                Gridlines="None" EmptyDataText="" ShowFooter="false" ShowHeader="false">
  	                <Columns>
  	                    <asp:BoundField DataField="Comment">
                              <ItemStyle Width="10%" Font-Italic="false" Wrap="true"/>
                        </asp:BoundField>
                        <asp:BoundField DataField="Message">
                              <ItemStyle Width="92%" Font-Italic="false" Wrap="true"/>
                        </asp:BoundField>
                    </Columns>
  	        </asp:GridView>
            </td>
        </tr>
        </table>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="rightContentPlaceHolder" runat="Server">
    <table border="0" bordercolor="#5D7B9D" width="100%" cellpadding="0" cellspacing="2" align="center">
        <tr>
            <td align="center">
		    <asp:Label ID="subCaption" skinID="PageCaption" runat="server"></asp:Label>
   	        </td>
        </tr>
       <tr>
            <td align="center">
				    <!--- Shady code goes here -->
            <div class="shiftcontainer">
            <div class="shadowcontainer300">
            <div class="innerdiv" align="left">
                    <b>Notes</b><br />
                    This module manages versioning based deployment in BizTalk. <br /><br />
                    <br /><br />
		            <br />
		    </div>
		    </div>
		    </div>
		    <!--- Shady code goes here -->
   	        </td>
       </tr>
       <tr>
            <td align="center">&nbsp;
   	        </td>
       </tr>
   </table>
</asp:Content>
