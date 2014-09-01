<%@ Page Language="C#" MasterPageFile="~/template.master" AutoEventWireup="true" CodeFile="AUA.aspx.cs" Inherits="UserTrace" EnableViewState="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="leftContentPlaceHolder" Runat="Server">
      <table border="0" width="99%" cellpadding="0" cellspacing="0" align="left">
        <tr>
            <td align="center">
		<asp:Label ID="lblCaption" skinID="PageCaption" runat="server"></asp:Label>
   	    </td>
        </tr>
        <tr>
            <td align="left" >
                <table border="0" width="100%" bgcolor="CECECE" cellpadding="0" cellspacing="0" align="left">
                <tr>
                    <td width="2%">
                        <asp:Image ID="errorImg" runat="server" ImageUrl="~/Images/error.png" ImageAlign="Baseline" Visible="false" />
		            </td>
		            <td width="95%">
		                <asp:Label ID="lblStatus" runat="server" Visible="false" Width="100%"></asp:Label>
		            </td>
		        </tr>
                </table>
   	    </td>
        </tr>

       <tr>
            <td valign="top">

  <table border="0" bordercolor="#5D7B9D" width="100%" cellpadding="0" cellspacing="0" align="left">
  <tr>
  <td style="word-break:break-all;">
	<!-- Grid View for activity log -->
        <asp:SqlDataSource ID="ActivityDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:authStore %>"
            SelectCommand="SELECT EventTime, EventType, EventSequence, EventCode, Message, MachineName, ExceptionType, Details FROM aspnet_WebEvent_Events where EventType = 'BCC.Core.BCCWebAuditEvent' ORDER BY EventTime DESC">
        </asp:SqlDataSource>

 	<asp:GridView id="UserActivityGrid" Visible="false" runat="server" Width="100%" ForeColor="#333333"
                CellPadding="4" CellSpacing="0" AutoGenerateColumns="False" DataSourceID="ActivityDataSource"
	        AllowPaging="True" PageSize="20" AllowSorting="True" Gridlines="Both" EmptyDataText="No Record(s) available">
  	        <Columns>
                    <asp:BoundField DataField="EventTime" HeaderText="Time" SortExpression="EventTime" DataFormatString="{0:MMM-dd-yyyy hh:mm:ss tt}">
                           <ItemStyle ForeColor="Teal" Width="5%" Font-Italic="false" Wrap="true"/>
                    </asp:BoundField>
                    <asp:BoundField DataField="EventSequence" HeaderText="Seq" SortExpression="EventSequence">
                          <ItemStyle Width="4%" Font-Italic="false" Wrap="true"/>
                    </asp:BoundField>
                    <asp:BoundField DataField="EventCode" HeaderText="Code" SortExpression="EventCode">
                          <ItemStyle Width="5%" Font-Italic="false" Wrap="true"/>
                    </asp:BoundField>
                    <asp:BoundField DataField="MachineName" HeaderText="Machine Name" SortExpression="MachineName">
                          <ItemStyle Width="5%" Font-Italic="false" Wrap="true"/>
                    </asp:BoundField>
                    <asp:BoundField DataField="Message" HeaderText="Message" SortExpression="Message">
                          <ItemStyle Width="24%" Font-Italic="false" Wrap="true"/>
                    </asp:BoundField>
                    <asp:BoundField DataField="Details" HeaderText="Details" SortExpression="Details">
                           <ItemStyle Width="50%" Font-Italic="false" Wrap="true"/>
                    </asp:BoundField>
                </Columns>
  	</asp:GridView>
        <!-- Grid View for activity log -->
  </td></tr>
  </table>
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
		            This section lists the recent activity of the logged-in user.<br />
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
       <tr align="center" valign="middle">
            <td width="100%" align="left" style="word-break:break-all;">
            <!-- Grid View for UserSummaryGrid activity -->
            <asp:SqlDataSource ID="UserDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:authStore %>" >
            </asp:SqlDataSource>

            <asp:GridView id="UserSummaryGrid" SkinID="sideTable" Visible="true" runat="server" Width="100%" ForeColor="#333333"
                    CellPadding="4" CellSpacing="0" AutoGenerateColumns="False" DataSourceID="UserDataSource"
                AllowPaging="True" PageSize="10" AllowSorting="True" Gridlines="Both" EmptyDataText="No Record(s) available">
                <Columns>
                        <asp:BoundField DataField="EventCode" HeaderText="Code" SortExpression="EventCode">
                              <ItemStyle Width="20%" Font-Italic="false" Wrap="true"/>
                        </asp:BoundField>
                        <asp:BoundField DataField="Message" HeaderText="Message" SortExpression="Message">
                              <ItemStyle Width="80%" Font-Italic="false" Wrap="true"/>
                        </asp:BoundField>
                    </Columns>
            </asp:GridView>
            <!-- Grid View for UserSummaryGrid activity -->
  	        </td>
        </tr>
    </table>
</asp:Content>
