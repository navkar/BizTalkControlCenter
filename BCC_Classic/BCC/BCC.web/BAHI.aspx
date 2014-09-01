<%@ Page Language="C#" MasterPageFile="~/template.master" AutoEventWireup="true" 
	CodeFile="BAHI.aspx.cs" Inherits="Host" Title="Host Instance" EnableViewState="false"%>
	
<asp:Content ID="Content1" ContentPlaceHolderID="leftContentPlaceHolder" Runat="Server">

    <table border="0" bordercolor="#C0C0C0" width="99%" cellpadding="0" cellspacing="0" align="left">
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
        <tr>
        <!-- Grid View for activity log -->
        <tr>
            <td colspan="2">
            <asp:SqlDataSource ID="ActivityDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:authStore %>"
                SelectCommand="SELECT 'Recent activity' as Comment, [Message] FROM aspnet_WebEvent_Events WHERE EventTime = (select MAX(EventTime) FROM aspnet_WebEvent_Events WHERE EventType = 'BCC.Core.BCCWebAuditEvent' and EventCode = 100202)">
            </asp:SqlDataSource>

 	        <asp:GridView id="UserActivityGrid" Visible="false" runat="server" Width="100%" ForeColor="#333333"
                    CellPadding="4" CellSpacing="0" AutoGenerateColumns="False" DataSourceID="ActivityDataSource"
	                Gridlines="None" EmptyDataText="" ShowFooter="false" ShowHeader="false">
  	                <Columns>
  	                    <asp:BoundField DataField="Comment">
                              <ItemStyle Width="10%" Font-Italic="false" Wrap="false"/>
                        </asp:BoundField>
                        <asp:BoundField DataField="Message">
                              <ItemStyle Width="92%" Font-Italic="false" Wrap="true"/>
                        </asp:BoundField>
                    </Columns>
  	        </asp:GridView>
        </td>
        </tr>
        <!-- Grid View for activity log -->
        <td width="50%">
        <!-- Update Panel starts here -->
     
        <table border="0" bordercolor="#C0C0C0" width="95%" cellpadding="0" cellspacing="0" align="left">
         <tr>
            <td>
                <table border="1" bgcolor="#E9E9E9" bordercolor="#C0C0C0" cellpadding="0" cellspacing="0" width="100%" align="left">
                    <tr align="left">
                        <td align="left" valign="bottom">Select&nbsp;:&nbsp;
                            <asp:LinkButton ID="btnSelectAll" runat="server" style="color:maroon;text-decoration:none;" OnClick="btnSelectAll_Click" Text="All" />,&nbsp;
                            <asp:LinkButton ID="btnDeselectAll" runat="server" style="color:maroon;text-decoration:none;" OnClick="btnDeSelectAll_Click" Text="None" />,&nbsp;
			                <asp:LinkButton ID="btnEnable" runat="server" style="color:green;text-decoration:none;" OnClientClick="return confirm('Are you sure you want to start?');" OnClick="btnEnable_Click" Text="Start" />,&nbsp;
                            <asp:LinkButton ID="btnDisable" runat="server" style="text-decoration:none;" OnClientClick="return confirm('Are you sure you want to stop?');" OnClick="btnDisable_Click" Text="Stop" />,&nbsp;
                            <asp:LinkButton ID="btnMonitor" runat="server" style="color:green;text-decoration:none;" OnClientClick="return confirm('Are you sure you want to monitor?');" OnClick="btnMonitor_Click" Text="Monitor" />                                    
                        </td>
                    </tr>
                </table>
            </td>
        </tr>

        <tr>
        <td>
            <asp:UpdatePanel ID="upHosts" runat="server" UpdateMode="Conditional">
            <ContentTemplate>               
		    <table border="1" bordercolor="#C0C0C0" width="100%" cellpadding="0" cellspacing="0" align="center">
            <tr colspan=2>
		    <td>
                    <asp:GridView ID="gridHost" runat="server" AutoGenerateColumns="False" CellPadding="4"
                        ForeColor="#333333" AllowSorting="false" GridLines="Both" Width="100%" OnRowDataBound="gridHost_RowDataBound">
                        <Columns>
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:CheckBox ID="chkBoxHost" runat="server" />
                                </ItemTemplate>
                                 <ItemStyle HorizontalAlign="Left" Width="2%" Font-Italic="false" />
                                </asp:TemplateField>
                                <asp:BoundField DataField="MachineName" HeaderText="Machine name" SortExpression="MachineName" />
                                <asp:BoundField DataField="HostName" HeaderText="BizTalk host name" SortExpression="HostName" />
                                <asp:BoundField DataField="HostType" HeaderText="Type" SortExpression="HostType" />
                                <asp:BoundField DataField="IsDisabled" HeaderText="Disabled?" SortExpression="IsDisabled">
                                    <ItemStyle HorizontalAlign="Center" Width="10%" Font-Italic="false" />
                                </asp:BoundField>                                
                                <asp:TemplateField HeaderText="">
                               <ItemTemplate>
                                    <bcc:AlertIndicator ID="alertStatus" Status='<%# Bind("Status") %>' runat="server" />
                               </ItemTemplate>
                               <ItemStyle HorizontalAlign="Left" Width="3%" Font-Italic="false" />
                            </asp:TemplateField>                          
                            <asp:BoundField DataField="Status" HeaderText="Status" SortExpression="Status">
                                <ItemStyle HorizontalAlign="Center" Width="10%" Font-Italic="false" />
                            </asp:BoundField>
                        </Columns>
                    </asp:GridView>
            </td>
		    </tr>
		    </table>
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
            			    <asp:LinkButton ID="btnEnable2" runat="server" style="color:green;text-decoration:none;" OnClientClick="return confirm('Are you sure you want to start?');" OnClick="btnEnable_Click" Text="Start" />,&nbsp;
                            <asp:LinkButton ID="btnDisable2" runat="server" style="text-decoration:none;" OnClientClick="return confirm('Are you sure you want to stop?');" OnClick="btnDisable_Click" Text="Stop" />,&nbsp;
                            <asp:LinkButton ID="btnMonitor2" runat="server" style="color:green;text-decoration:none;" OnClientClick="return confirm('Are you sure you want to monitor?');" OnClick="btnMonitor_Click" Text="Monitor" />                                                                
                        </td>
                    </tr>
                </table>
            </td>
        </tr>

        </table>
         
        <!-- Update Panel ends here -->
        </td>
        
        <td width="40%" align="center" valign="baseline">
                

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
            <td width="50%" align="center" valign="top">
                
                    <div class="shiftcontainer">
                    <div class="shadowcontainer">
                    <div class="innerdiv" align="left">
                        A Host is a logical container for BizTalk objects and only one instance of a specific host can exist on each server. <br />
                        <br />
                        Host instances are the physical containers of BizTalk Server objects. You create a host instance when you map a server to a host. <br />
                        <br />
                        <b>Notes</b> <br /> 
                        Hosts which are of the type 'Isolated', have a status as 'Unknown' and cannot be started or stopped. It's listed for completeness.<br />
                        <br />
                        <b>How to monitor a Host instance?</b> <br />
                        Simply check the host instance and click 'Monitor', use Speedcode <b>103</b> to view the list of all monitored artifacts.<br />        
                    </div>
                    </div>
                    </div>

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
                        SelectCommand="SELECT [Message] FROM aspnet_WebEvent_Events WHERE EventType = 'BCC.Core.BCCWebAuditEvent' and EventCode = 100202 ORDER BY EventTime DESC">
                    </asp:SqlDataSource>

                    <asp:GridView id="UserActivityGrid2" SkinID="sideTable" Visible="true" runat="server" Width="100%" ForeColor="#333333"
                                CellPadding="4" CellSpacing="0" AutoGenerateColumns="False" DataSourceID="ActivityDataSource2"
                            AllowPaging="True" PageSize="10" AllowSorting="True" Gridlines="Both" EmptyDataText="No Record(s) available">
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
