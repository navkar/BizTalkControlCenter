<%@ Page Language="C#" MasterPageFile="~/template.master" AutoEventWireup="true" CodeFile="BASP.aspx.cs" 
Inherits="BiztalkSendPort" EnableViewState="false"%>

<%@ Register TagPrefix="uc" TagName="Search" Src="~/Controls/SearchControl.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="leftContentPlaceHolder" Runat="Server">
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
                SelectCommand="SELECT 'Recent activity' as Comment, [Message] FROM aspnet_WebEvent_Events WHERE EventTime = (select MAX(EventTime) FROM aspnet_WebEvent_Events WHERE EventType = 'BCC.Core.BCCWebAuditEvent' and EventCode = 100205)">
            </asp:SqlDataSource>

 	        <asp:GridView id="UserActivityGrid" Visible="false" runat="server" Width="100%" ForeColor="#333333"
                    CellPadding="4" CellSpacing="0" AutoGenerateColumns="False" DataSourceID="ActivityDataSource"
	                Gridlines="None" EmptyDataText="" ShowFooter="false" ShowHeader="false">
  	                <Columns>
  	                    <asp:BoundField DataField="Comment">
                              <ItemStyle Width="10%" Font-Italic="false" Wrap="true"/>
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
                                <td align="left" valign="bottom">Select&nbsp;:&nbsp;
                                    <asp:LinkButton ID="btnSelectAll" runat="server" style="color:maroon;text-decoration:none;" OnClick="btnSelectAll_Click" Text="All" />,&nbsp;
                                    <asp:LinkButton ID="btnDeselectAll" runat="server" style="color:maroon;text-decoration:none;" OnClick="btnDeSelectAll_Click" Text="None" />,&nbsp;
			                        <asp:LinkButton ID="btnStart" runat="server" style="color:green;text-decoration:none;" OnClientClick="return confirm('Are you sure you want to start?');" OnClick="btnStart_Click" Text="Start" />,&nbsp;
                                    <asp:LinkButton ID="btnStop" runat="server" style="text-decoration:none;" OnClientClick="return confirm('Are you sure you want to stop?');" OnClick="btnStop_Click" Text="Stop" />,&nbsp;
                                    <asp:LinkButton ID="btnMonitor" runat="server" style="color:green;text-decoration:none;" OnClientClick="return confirm('Are you sure you want to monitor?');" OnClick="btnMonitor_Click" Text="Monitor" />                                    
                                </td>
			                    <td align="right" width="1%">
			                         <asp:LinkButton ID="btnExportToExcel" runat="server" style="color:teal;text-decoration:none;" OnClick="btnExportToExcel_Click" >
				                    <asp:Image ID="excel" runat="server" ImageUrl="~/Images/Excel-16.gif" />
			                         </asp:LinkButton>
			                    </td>
                            </tr>
                            <tr align="left">
                                <td align="left" valign="bottom"colspan="2" >
		                            <uc:Search ID="search" runat="Server" /> 
                                </td>
                            </tr>                    
                        </table>
		        </asp:Panel> 
            </td>
        </tr>
   
    	<tr>
            <td>
            <asp:UpdatePanel ID="upSendPorts" runat="server" UpdateMode="Conditional">
                <ContentTemplate>	            
                  <asp:Panel ID="sendPortPanel" runat="server" Visible="true">
		            <table border="1" bordercolor="#C0C0C0" width="100%" cellpadding="0" cellspacing="0" align="center">
                    <tr>
		                <td style="word-break:break-all;">
                        <asp:GridView ID="gridSendPort" runat="server" AutoGenerateColumns="False"
                                    CellPadding="4" GridLines="Both" Width="100%" PageSize="20"
                                    AllowSorting="true" OnSorting="OnSort" AllowPaging="true"
                                    OnPageIndexChanging="gridSendPort_PageIndexChanging"
                                    OnRowDataBound="gridSendPort_RowDataBound">
                                    <Columns>
                                        <asp:TemplateField>
                                            <ItemTemplate>
                                                <asp:CheckBox ID="chkBoxSendPort" runat="server" />
                                            </ItemTemplate>
                                            <ItemStyle Width="2%" Font-Italic="false" Wrap="true"/>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="SendPortName" SortExpression="SendPortName" HeaderText="Send port">
                                            <ItemStyle Width="30%" Font-Italic="false" Wrap="true"/>
                                        </asp:BoundField>
                                        <asp:TemplateField HeaderText="">
                                               <ItemTemplate>
                                                    <bcc:AlertIndicator ID="alertStatus" Status='<%# Bind("Status") %>' runat="server" />
                                               </ItemTemplate>
                                               <ItemStyle HorizontalAlign="Left" Width="2%" Font-Italic="false" />
                                        </asp:TemplateField>                                          
                                        <asp:BoundField DataField="Status" SortExpression="Status" HeaderText="Status">
                                            <ItemStyle Width="7%" Font-Italic="false" Wrap="false"/>
                                        </asp:BoundField>
                                        <asp:BoundField DataField="URI" SortExpression="URI" HeaderText="Send port URI">
                                            <ItemStyle Width="30%" Wrap="true"/>
                                        </asp:BoundField>
                                        <asp:BoundField DataField="PortType" SortExpression="PortType" HeaderText="Port type">
                                            <ItemStyle Width="8%" Wrap="true"/>
                                        </asp:BoundField>
                                        <asp:BoundField DataField="TransportType" SortExpression="TransportType" HeaderText="Protocol">
                                            <ItemStyle Width="8%" Font-Italic="false" Wrap="true"/>
                                        </asp:BoundField>
                                        <asp:BoundField DataField="Application" SortExpression="Application" HeaderText="Application" >
                                            <ItemStyle Width="14%" Font-Italic="false" Wrap="false"/>
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
                                <b>No send port(s) were found.</b><br /><br />
                                The BizTalk Applications specified in the user profile does not have any Send port(s). <br /><br />
                                Go to 'Administration > System Settings' or use speedcode <b>603</b>.<br /><br />
		                        <br />
		                </div>
		                </div>
		                </div>
		                <!--- Shady code goes here -->
		              <br />
                  </asp:Panel> 
                </ContentTemplate>                  
            </asp:UpdatePanel>                
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
                            <asp:LinkButton ID="btnStop2" runat="server" style="text-decoration:none;" OnClientClick="return confirm('Are you sure you want to stop?');" OnClick="btnStop_Click" Text="Stop" />,&nbsp;
                            <asp:LinkButton ID="btnMonitor2" runat="server" style="color:green;text-decoration:none;" OnClientClick="return confirm('Are you sure you want to monitor?');" OnClick="btnMonitor_Click" Text="Monitor" />
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
            <b>Adding new BizTalk Applications</b> <br /> 
            If you are expecting to see additional send ports and are not seeing them, you might want to add additional <b>BizTalk Applications</b> into the user profile, use speedcode <b>603</b>.
             <br /> <br />
            <asp:DataList ID="dlSPAppList" runat="server" skinID="sideList" >
                <HeaderTemplate>
                Configured BizTalk Applications
                </HeaderTemplate>
                <ItemTemplate>
                <%#Container.DataItem%>
                </ItemTemplate>
            </asp:DataList>
            <br />                    
            <b>How to monitor a Send port?</b> <br />
             Simply check the send port and click 'Monitor', use Speedcode <b>103</b> to view the list of all monitored artifacts.<br />        
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
       <tr>
           <td style="word-break:break-all;">
	            <!-- Grid View for activity log -->
                    <asp:SqlDataSource ID="ActivityDataSource2" runat="server" ConnectionString="<%$ ConnectionStrings:authStore %>"
                        SelectCommand="SELECT [Message] FROM aspnet_WebEvent_Events WHERE EventType = 'BCC.Core.BCCWebAuditEvent' and EventCode = 100205 ORDER BY EventTime DESC">
                    </asp:SqlDataSource>

                    <asp:GridView id="UserActivityGrid2" SkinID="sideTable" Visible="true" runat="server" Width="100%" ForeColor="#333333"
                                CellPadding="4" CellSpacing="0" AutoGenerateColumns="False" DataSourceID="ActivityDataSource2"
                            AllowPaging="True" PageSize="5" AllowSorting="True" Gridlines="Both" EmptyDataText="No Record(s) available">
                            <Columns>
                                    <asp:BoundField DataField="Message" HeaderText="Recent Activity" SortExpression="Message">
                                          <ItemStyle Width="30%" Font-Italic="false" Wrap="true"/>
                                    </asp:BoundField>
                                </Columns>
                    </asp:GridView>
            </td>       
       </tr>       
       </table>
</asp:Content>
