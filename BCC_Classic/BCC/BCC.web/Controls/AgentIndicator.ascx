<%@ Control Language="C#" AutoEventWireup="true" CodeFile="AgentIndicator.ascx.cs" Inherits="Controls_AgentIndicator" %>

<table border="0" cellpadding="2" cellspacing="0" bgcolor="#FFFFCC">
    <tr>
        <td>
            <asp:Label skinID="AgentCaption" ID="agentName" runat="server" Text="" />
        </td>
        <td width="3%" >
            <bcc:AlertIndicator ID="agentStatus" runat="server" />
        </td>
    </tr>
</table>