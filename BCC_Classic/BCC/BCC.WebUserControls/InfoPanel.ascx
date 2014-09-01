<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="InfoPanel.ascx.cs" Inherits="BCC.WebUserControls.InfoPanel" %>

<table border="0" width="100%" bgcolor="#CECECE" cellpadding="0" cellspacing="0" align="left">
<tr>
    <td width="2%" valign="top">
        <asp:Image ID="errorImg" runat="server" ImageUrl="~/Images/error.png" Visible="false" />
        <asp:Image ID="infoImg" runat="server" ImageUrl="~/Images/info.png" Visible="false" />
    </td>
    <td width="95%">
        <asp:Label ID="lblMessage" runat="server" Visible="false" Width="100%"></asp:Label>
    </td>
</tr>
</table>
