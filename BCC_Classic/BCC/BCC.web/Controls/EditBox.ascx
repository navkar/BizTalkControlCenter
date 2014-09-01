<%@ Control ClassName="Controls_EditBox" Language="C#" AutoEventWireup="true" CodeFile="EditBox.ascx.cs" Inherits="Controls_EditBox" %>

<table cellspacing="1" cellpadding="1" border="1" style="width:100%;border-collapse:collapse;" bgcolor="#FFFFCC">
    <tr>
    <td align="left" width="35%" style="background-color:#525252;font-weight:bold;" valign="middle">
        <asp:Label skinID="PageHeader" ID="lblDisplayName" Text="dummy field" runat="server" /> <asp:HiddenField ID="lblName" runat="server" Value="dummy field" />
    </td>
    <td width="60%" valign="middle">
        <asp:TextBox id="tValue" width="98%" Text="dummy data" runat="server" BorderStyle="Groove"  />
    </td>
    <td width="5%" valign="middle">
        <asp:RequiredFieldValidator id="validator" runat="server" ToolTip="Required!" ErrorMessage="<img src='images/indicator/ball_redS.gif'>" ControlToValidate="tValue">
        </asp:RequiredFieldValidator>
    </td>
    </tr>
</table>