<%@ Page Language="C#" MasterPageFile="~/template.master" AutoEventWireup="true" CodeFile="KCS.aspx.cs" 
Inherits="KnowledgeCenterSolutions" Title="BizTalk Send Ports" EnableViewState="false"%>

<asp:Content ID="Content1" ContentPlaceHolderID="leftContentPlaceHolder" Runat="Server">
    <table border="0" bordercolor="#5D7B9D" width="95%" cellpadding="0" cellspacing="0" align="left">
        <tr>
            <td align="center">
		    <asp:Label ID="lblCaption" skinID="PageCaption" runat="server"></asp:Label>
   	        </td>
        </tr>
       <tr>
             <td width="100%" align="left" >
                <table border="0" width="100%" cellpadding="0" cellspacing="0" align="left">
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

        <tr>
            <td>
		<asp:Panel ID="tablePanel" runat="server" defaultbutton="btnFilter">
                <table border="1" bgcolor="#E9E9E9" bordercolor="#5D7B9D" cellpadding="0" cellspacing="0" width="100%" align="left">
                    <tr align="left">
                        <td align="left" valign="bottom">Select&nbsp;:&nbsp;
                            <asp:LinkButton ID="btnSelectAll" runat="server" style="color:maroon;text-decoration:none;" OnClick="btnSelectAll_Click" Text="All" />,&nbsp;
                            <asp:LinkButton ID="btnDeselectAll" runat="server" style="color:maroon;text-decoration:none;" OnClick="btnDeSelectAll_Click" Text="None" />,&nbsp;
			    <asp:LinkButton ID="btnStart" runat="server" style="color:green;text-decoration:none;" OnClientClick="return confirm('Are you sure you want to start?');" OnClick="btnStart_Click" Text="Start" />,&nbsp;
                            <asp:LinkButton ID="btnStop" runat="server" style="text-decoration:none;" OnClientClick="return confirm('Are you sure you want to stop?');" OnClick="btnStop_Click" Text="Stop" />
                        </td>
			<td align="right" width="1%">
			     <asp:LinkButton ID="btnExportToExcel" runat="server" style="color:teal;text-decoration:none;" OnClick="btnExportToExcel_Click" >
				<asp:Image ID="excel" runat="server" ImageUrl="~/Images/Excel-16.gif" />
			     </asp:LinkButton>
			</td>
                    </tr>
                    <tr align="left">
                        <td align="left" valign="bottom" colspan="2">&nbsp;
		            <asp:TextBox id="txtSearchKey" runat="server" BorderStyle="Groove" Tooltip="enter a keyword like 4.0/FP/IPLAN" AutoCompleteType="Search" />&nbsp;
                            <asp:LinkButton ID="btnFilter" runat="server" style="visibility:hidden;display:none;text-decoration:none;" OnClick="btnFilter_Click" Text="Search" />
                            (keywords:  ...) [<%=count %>]
                        </td>
                    </tr>
                </table>
		</asp:Panel> 
            </td>
        </tr>
   
    	<tr>
            <td>
		<table border="0" bordercolor="#5D7B9D" width="100%" cellpadding="0" cellspacing="0" align="center">
                <tr>
		    <td>
		    </td>
		</tr>
		</table>
            </td>
        </tr>
        <tr>
            <td> 
                <table border="1" bgcolor="#E9E9E9" bordercolor="#5D7B9D" cellpadding="0" cellspacing="0" width="100%" align="left">
                    <tr align="left">
                        <td align="left" valign="bottom">Select&nbsp;:&nbsp;
                            <asp:LinkButton ID="btnSelectAll2" runat="server" style="color:maroon;text-decoration:none;" OnClick="btnSelectAll_Click" Text="All" />,&nbsp;
                            <asp:LinkButton ID="btnDeselectAll2" runat="server" style="color:maroon;text-decoration:none;" OnClick="btnDeSelectAll_Click" Text="None" />,&nbsp;
			    <asp:LinkButton ID="btnStart2" runat="server" style="color:green;text-decoration:none;" OnClientClick="return confirm('Are you sure you want to start?');" OnClick="btnStart_Click" Text="Start" />,&nbsp;
                            <asp:LinkButton ID="btnStop2" runat="server" style="text-decoration:none;" OnClientClick="return confirm('Are you sure you want to stop?');" OnClick="btnStop_Click" Text="Stop" />
                        </td>
			<td align="right" width="1%">
			     <asp:LinkButton ID="btnExportToExcel2" runat="server" style="color:teal;text-decoration:none;" OnClick="btnExportToExcel_Click" >
				<asp:Image ID="excel2" runat="server" ImageUrl="~/Images/Excel-16.gif" />
			     </asp:LinkButton>
			</td>
                    </tr>
                </table>

            </td>
        </tr>
    </table>
</asp:Content>

