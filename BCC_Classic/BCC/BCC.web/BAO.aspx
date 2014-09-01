<%@ Page Language="C#" MasterPageFile="~/template.master" AutoEventWireup="true"
    CodeFile="BAO.aspx.cs" Inherits="Orchestration" EnableViewState="false" EnableEventValidation="False" %>
    
<%@ Register TagPrefix="uc" TagName="Search" Src="~/Controls/SearchControl.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="leftContentPlaceHolder" runat="Server">
    <table border="0" bordercolor="#5D7B9D" width="99%" cellpadding="0" cellspacing="0" align="center">
        <tr>
            <td align="center">
		    <asp:Label ID="lblCaption" skinID="PageCaption" runat="server"></asp:Label>
   	        </td>
        </tr>
       <tr>
             <td width="100%" align="left" >
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
                SelectCommand="SELECT 'Recent activity' as Comment, [Message] FROM aspnet_WebEvent_Events WHERE EventTime = (select MAX(EventTime) FROM aspnet_WebEvent_Events WHERE EventType = 'BCC.Core.BCCWebAuditEvent' and EventCode = 100203)">
            </asp:SqlDataSource>

 	        <asp:GridView id="UserActivityGrid" Visible="false" runat="server" Width="100%" ForeColor="#333333"
                    CellPadding="4" CellSpacing="0" AutoGenerateColumns="False" DataSourceID="ActivityDataSource"
	                Gridlines="None" EmptyDataText="" ShowFooter="false" ShowHeader="false">
  	                <Columns>
  	                    <asp:BoundField DataField="Comment">
                              <ItemStyle Width="10%" Font-Italic="false" Wrap="false" />
                        </asp:BoundField>
                        <asp:BoundField DataField="Message">
                              <ItemStyle Width="90%" Font-Italic="false" Wrap="true"/>
                        </asp:BoundField>
                    </Columns>
  	        </asp:GridView>
        </td>
        </tr>
        <!-- Grid View for activity log -->
         <tr>
            <td> 
               <asp:Panel ID="tablePanel" runat="server">
                <table border="1" bgcolor="#E9E9E9" bordercolor="#C0C0C0" cellpadding="0" cellspacing="0" width="100%" align="left">
                    <tr align="left">
                        <td align="left" valign="bottom"colspan="2" >
		                    <uc:Search ID="search" runat="Server" /> 
                        </td>
                    </tr>                
                    <tr align="left">
                        <td align="left" valign="bottom">Select&nbsp;:&nbsp;
                            <asp:LinkButton ID="btnSelectAll" runat="server" style="color:maroon;text-decoration:none;" OnClick="btnSelectAll_Click" Text="All" />,&nbsp;
                            <asp:LinkButton ID="btnDeselectAll" runat="server" style="color:maroon;text-decoration:none;" OnClick="btnDeSelectAll_Click" Text="None" />,&nbsp;
			                <asp:LinkButton ID="btnStart" runat="server" style="color:green;text-decoration:none;" OnClientClick="return confirm('Are you sure you want to start?');" OnClick="btnStart_Click" Text="Start" />,&nbsp;
                            <asp:LinkButton ID="btnStop" runat="server" style="color:DodgerBlue;text-decoration:none;" OnClientClick="return confirm('Are you sure you want to unenlist?');" OnClick="btnStop_Click" Text="Unenlist" />
                        </td>
			            <td align="right" width="1%">
			                <asp:LinkButton ID="btnExportToExcel" runat="server" style="color:teal;text-decoration:none;" OnClick="btnExportToExcel_Click" >
				                <asp:Image ID="excel" runat="server" ImageUrl="~/Images/Excel-16.gif" />
			                </asp:LinkButton>
			            </td>
                    </tr>
                </table>
		        </asp:Panel>
            </td>
        </tr>
        <tr>
            <td>
              <asp:Panel id="masterPanel" Visible="True" runat="server">
		            <table border="1" bordercolor="#C0C0C0" width="100%" cellpadding="0" cellspacing="0" align="center">
                            <tr colspan=2>
                                <td style="word-break:break-all;">
		                            <asp:GridView ID="gridOdx" runat="server" AutoGenerateColumns="False" AllowSorting="True" 
		                            PageSize="10" AllowPaging="true"
		                          OnSorting="OnSort" CellPadding="4"
                                  GridLines="Both" Width="100%" 
                                  OnPageIndexChanging="gridOdx_PageIndexChanging"
                                  OnRowDataBound="gridOdx_RowDataBound">
                               <Columns>
                                    <asp:TemplateField>
                                        <ItemStyle Width="1%" Font-Italic="false" Wrap="true"/>
                                        <ItemTemplate>
                                            <asp:CheckBox ID="chkBoxOdx" runat="server" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="BizTalk Orchestration" SortExpression="Name"  >
                                        <ItemStyle Width="32%" Font-Italic="false" Wrap="true"/>
                                        <ItemTemplate>
                                            <asp:LinkButton Style="color:teal;text-decoration:none;" Text='<%# Eval("Name") %>' ID="btnDetail" runat="server" OnClick="btnDetail_Click" /> 
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="">
                                           <ItemTemplate>
                                                <bcc:AlertIndicator ID="alertStatus" Status='<%# Bind("Status") %>' runat="server" />
                                           </ItemTemplate>
                                           <ItemStyle HorizontalAlign="Left" Width="2%" Font-Italic="false" />
                                    </asp:TemplateField>                                    
                                    <asp:BoundField DataField="Status" HeaderText="Status" SortExpression="Status">
                                          <ItemStyle Width="6%" Font-Italic="false" Wrap="false"/>
                                    </asp:BoundField>
                                    <asp:BoundField DataField="AssemblyName" HeaderText="Assembly" SortExpression="AssemblyName">
                                          <ItemStyle Width="19%" Font-Italic="false" Wrap="true"/>
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Version" HeaderText="V" SortExpression="Version">
                                          <ItemStyle Width="4%" Font-Italic="false" Wrap="false"/>
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Application" HeaderText="Application" SortExpression="Application">
                                          <ItemStyle Width="12%" Font-Italic="false" Wrap="true"/>
                                    </asp:BoundField>
                                </Columns>
                            </asp:GridView>
                                </td>
                            </tr>
		            </table>
              </asp:Panel>
              
              <asp:Panel id="emptyPanel" Visible="False" Runat="server">
                  <br />
			        <!--- Shady code goes here -->
                    <div class="shiftcontainer" align="center">
                    <div class="shadowcontainer">
                    <div class="innerdiv" align="left">
                            <b>No orchestration(s) were found.</b><br /><br />
                                The BizTalk Applications specified in the user profile does contain any Orchestrations(s). <br /><br />
                                Go to 'Administration > System Settings' or use speedcode <b>603</b>.<br /><br />
		                    <br />
		            </div>
		            </div>
		            </div>
		            <!--- Shady code goes here -->
		          <br />
              </asp:Panel>

    		  <asp:Panel id="detailPanel" Visible="False" runat="server" defaultbutton="btnGoBack">
		            <table border="1" bordercolor="#C0C0C0" width="100%" cellpadding="0" cellspacing="0" align="center">
		    <tr>
		        <td colspan="2">
                    <asp:GridView ID="gridOdxDetail" runat="server" AutoGenerateColumns="False" CellPadding="4"
                        ForeColor="#333333" GridLines="Both" Width="100%" OnRowDataBound="gridOdxDetail_RowDataBound">
                        <Columns>
                            <asp:BoundField DataField="PropertyName" HeaderText="Property Name" />
                            <asp:BoundField DataField="PropertyValue" HeaderText="Property Value" />
                        </Columns>
                    </asp:GridView>
		        </td>
		    </tr>
		    <tr>
		        <td colspan="2">
		            <asp:GridView ID="PortView" runat="server" AutoGenerateColumns="False" CellPadding="4"
                        ForeColor="#333333" GridLines="Both" Width="100%"  OnRowDataBound="PortView_RowDataBound">
                        <Columns>
                            <asp:BoundField DataField="PhysicalPortName" HeaderText="Physical Port Name" />
                            <asp:BoundField DataField="LogicalPortName" HeaderText="Logical Port Name" />
                            <asp:BoundField DataField="PortBinding" HeaderText="Binding" />
                            <asp:BoundField DataField="PortOperation" HeaderText="Operation" />
                            <asp:BoundField DataField="PortDirection" HeaderText="Direction" />
                            <asp:BoundField DataField="PortParent" HeaderText="Parent Port" />
                            <asp:BoundField DataField="PortStatus" HeaderText="Status" />
                        </Columns>
                    </asp:GridView>
		        </td>
		    </tr>
    		<tr>
    		<td colspan=2>
		        <asp:LinkButton ID="btnGoBack" runat="server" style="color:teal;text-decoration:none;" OnClick="btnGoBack_Click" Text="Orchestration View" />
		    </td>
		</tr>

		</table>
		      </asp:Panel>
            </td>
        </tr>
        <tr>
            <td>
                <table border="1" bgcolor="#E9E9E9" bordercolor="#C0C0C0" cellpadding="0" cellspacing="0" width="100%" align="left">
                    <tr align="left">
                        <td align="left" valign="bottom">Select&nbsp;:&nbsp;
                            <asp:LinkButton ID="btnSelectAll2" runat="server" style="color:maroon;text-decoration:none;" OnClick="btnSelectAll_Click" Text="All" />,&nbsp;
                            <asp:LinkButton ID="btnDeselectAll2" runat="server" style="color:maroon;text-decoration:none;" OnClick="btnDeSelectAll_Click" Text="None" />,&nbsp;
			    <asp:LinkButton ID="btnStart2" runat="server" style="color:green;text-decoration:none;" OnClientClick="return confirm('Are you sure you want to start?');" OnClick="btnStart_Click" Text="Start" />,&nbsp;
                            <asp:LinkButton ID="btnStop2" runat="server" style="color:DodgerBlue;text-decoration:none;" OnClientClick="return confirm('Are you sure you want to unenlist?');" OnClick="btnStop_Click" Text="Unenlist" />
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
        <tr>
            <td width="*" bgcolor="#E9E9E9"  align="center" >
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
                    You must enlist dependent orchestrations, send ports and receive locations before you can successfully start the Orchestration.<br /><br />
            <b>Adding new BizTalk Applications</b> <br /> 
            If you are expecting to see additional orchestrations and are not seeing them, you might want to add additional <b>BizTalk Applications</b> into the user profile, use speedcode <b>603</b>.
             <br /><br /> 
            <asp:DataList ID="dlOAppList" runat="server" skinID="sideList" >
                <HeaderTemplate>
                Configured BizTalk Applications
                </HeaderTemplate>
                <ItemTemplate>
                <%#Container.DataItem%>
                </ItemTemplate>
            </asp:DataList>
            <br />                                 
            The Control center automatically terminates instances when the Orchestration is unenlisted.<br /><br />
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
	            <!-- Grid View for orchestration activity -->
                <asp:SqlDataSource ID="sqlDS" runat="server" ConnectionString="<%$ ConnectionStrings:bizTalk %>">
                </asp:SqlDataSource>

 	            <asp:GridView id="odxInfo" Visible="true" runat="server" Width="100%" ForeColor="#333333"
                        CellPadding="4" CellSpacing="0" AutoGenerateColumns="False" DataSourceID="sqlDS"
	                    Gridlines="Both" SkinID="sideTable" EmptyDataText="No activity detected">
  	                <Columns>
                            <asp:BoundField DataField="Orchestration" HeaderText="Orchestration" >
                                  <ItemStyle Width="50%" Font-Italic="false" Wrap="true"/>
                            </asp:BoundField>
                            <asp:BoundField DataField="State" HeaderText="State" >
                                  <ItemStyle Width="30%" Font-Italic="false" Wrap="true"/>
                            </asp:BoundField>
                            <asp:BoundField DataField="Count" HeaderText="Count" >
                                  <ItemStyle HorizontalAlign="Center" Width="20%" Font-Italic="false" Wrap="true"/>
                            </asp:BoundField>
                        </Columns>
  	            </asp:GridView>
  	            <!-- Grid View for orchestration activity -->
  	        </td>
        </tr>
    </table>
</asp:Content>