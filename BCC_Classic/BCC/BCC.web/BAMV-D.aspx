<%@ Page Language="C#" AutoEventWireup="true" CodeFile="BAMV-D.aspx.cs" Inherits="BiztalkMessageDetail" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Message Detail</title>
    <link href="./css/template.css" type="text/css" rel="stylesheet" />
<script language="javascript" type="text/javascript">
// <!CDATA[

function txtAreaMessage_onclick() {

}

// ]]>
</script>


</head>
<body>
    <form id="form1" runat="server">
    <div>
    <asp:Panel id="msgDetailPanel" Visible="False" runat="server">
    <table border="0" bgcolor="#EFF3FE" bordercolor="#5D7B9D" width="95%" cellpadding="0" cellspacing="0" align="left">
        <tr>
            <td align="center">
		<asp:Label ID="lblCaption" skinID="PageCaption" runat="server"></asp:Label>
   	    </td>
        </tr>
        <tr>
            <td width="100%" align="center">
                <asp:Label ID="lblError" runat="server" Width="100%" ForeColor="Red"></asp:Label>
	    </td>
        </tr>
        <tr>
            <td width="100%" align="left">

		<table border="1" bordercolor="#5D7B9D" width="60%" cellpadding="0" cellspacing="0" align="center">
		<tr><td align="left"> Select:
                  <asp:LinkButton ID="btnResumeMessage" Style="text-decoration:none;" runat="server" Text="Resume" OnClientClick="return confirm('Are you sure you want to resume?');" OnClick="btnResumeMessage_Click" />,&nbsp;
                  <asp:LinkButton ID="btnTerminate" Style="text-decoration:none;" runat="server" Text="Terminate" OnClientClick="return confirm('Are you sure you want to terminate?');" OnClick="btnTerminate_Click" />,&nbsp;
                  <a href="" Style="text-decoration:none;" onclick="javascript:window.close();" >Close</a>
		</td></tr>
		<tr><td>
                    <asp:GridView ID="gridMsg" runat="server" AutoGenerateColumns="False" CellPadding="4"
                        ForeColor="#333333" OnRowDataBound="gridMsg_RowDataBound" GridLines="Both" Width="70%" >
                        <Columns>
                            <asp:BoundField DataField="MsgProperty" HeaderText="Message Property Name" />
                            <asp:BoundField DataField="MsgValue" HeaderText="Message Property Value" />
                        </Columns>
                    </asp:GridView>

		</td></tr>
		<tr><td width="50%" align="left">
		<%# GetMessageDetail() %>
		</td></tr>
		<tr><td align="left"> Select:
                  <asp:LinkButton ID="btnResumeMessage2" Style="text-decoration:none;" runat="server" Text="Resume" OnClientClick="return confirm('Are you sure you want to resume?');" OnClick="btnResumeMessage_Click" />,&nbsp;
                  <asp:LinkButton ID="btnTerminate2" Style="text-decoration:none;" runat="server" Text="Terminate" OnClientClick="return confirm('Are you sure you want to terminate?');" OnClick="btnTerminate_Click" />,&nbsp;
                  <a href="" Style="text-decoration:none;" onclick="javascript:window.close();" >Close</a>
		</td></tr>
		</table>
	    </td>
        </tr>
        </table>
    </div>
    </asp:Panel>
    </form>
</body>
</html>
