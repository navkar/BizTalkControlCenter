<%@ Page Language="C#" AutoEventWireup="true" CodeFile="BACD.aspx.cs" Inherits="BACD" %>

<html>
<head runat="server"> 
    <title></title>
    <link rel="stylesheet" type="text/css" href="./css/template.css" />
    <link rel="stylesheet" type="text/css" href="./css/help.css" />
    <link rel="stylesheet" type="text/css" href="./css/tag.css" />
</head>
<body>
    <form id="mainForm" runat="server">
    <table border="1" bgcolor="#525252" bordercolor="#525252" cellpadding="0" cellspacing="0" width="100%" align="left">
        <tr>
            <td colspan="2">
            <asp:SqlDataSource ID="ActivityDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:authStore %>"
                SelectCommand="SELECT 'Recent activity' as Comment, [Message] FROM aspnet_WebEvent_Events WHERE EventTime = (select MAX(EventTime) FROM aspnet_WebEvent_Events WHERE EventType = 'BCC.Core.BCCWebAuditEvent' and EventCode = 100210)">
            </asp:SqlDataSource>

 	        <asp:GridView id="UserActivityGrid" Visible="false" runat="server" Width="100%" ForeColor="#333333"
                    CellPadding="4" CellSpacing="0" AutoGenerateColumns="False" DataSourceID="ActivityDataSource"
	                Gridlines="None" EmptyDataText="" ShowFooter="false" ShowHeader="false">
  	                <Columns>
  	                    <asp:BoundField DataField="Comment">
                              <ItemStyle Width="15%" Font-Italic="false" Wrap="false"/>
                        </asp:BoundField>
                        <asp:BoundField DataField="Message">
                              <ItemStyle Width="85%" Font-Italic="false" Wrap="true"/>
                        </asp:BoundField>
                    </Columns>
  	        </asp:GridView>
        </td>
        </tr>
        <tr>
            <td align="left">
                <asp:Label skinID="PageHeader" ID="lblCaption" runat="server" Text="Configuration file view" />
   	    </td>
        </tr>
        <tr>
            <td align="center">
                <asp:TextBox BorderStyle="Groove" ID="displayArea" ReadOnly="true" runat="server" TextMode="MultiLine" Width="100%" Height="700px"></asp:TextBox>
             </td>
   	    </tr>
     </table>
    </form>
</body>
</html>
