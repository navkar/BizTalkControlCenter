<%@ Control Language="C#" AutoEventWireup="false" CodeFile="SearchControl.ascx.cs" Inherits="SearchUserControl" %>

<table border="0" width="100%" cellpadding="0" cellspacing="0" align="left">
    <tr>
    <td align="left" width="25%">
        <asp:TextBox id="txtSearch" width="98%" Text="" runat="server" BorderStyle="Groove" Tooltip="Enter a keyword to search" />
    </td>
    <td align="left" width="1%">
       <asp:LinkButton ID="lnkSearch" OnClick="lnkSearch_Click" runat="server" cssclass="linkConfig">
            <asp:Image ToolTip="" ID="searchImg" runat="server" ImageUrl="~/Images/search-icon.gif" />
        </asp:LinkButton>
    </td>
    <td align="left" width="65%">
        <asp:Label ID="lblSearchKeywords" Text="" runat="server" />
    </td>
    </tr>
</table>	