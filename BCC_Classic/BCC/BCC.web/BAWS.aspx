<%@ Page Language="C#" MasterPageFile="~/template.master" AutoEventWireup="true" CodeFile="BAWS.aspx.cs" Inherits="Service" Title="" EnableViewState="false"%>
<%-- Add content controls here --%>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="leftContentPlaceHolder">
    <table border="0" bordercolor="#5D7B9D" width="99%" cellpadding="0" cellspacing="0" align="left">
        <tr>
            <td colspan="2" align="center">
		    <asp:Label ID="lblCaption" skinID="PageCaption" runat="server"></asp:Label>
   	        </td>
        </tr>
        <tr>
             <td width="100%" align="left" colspan="2" >
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
                SelectCommand="SELECT 'Recent activity' as Comment, [Message] FROM aspnet_WebEvent_Events WHERE EventTime = (select MAX(EventTime) FROM aspnet_WebEvent_Events WHERE EventType = 'BCC.Core.BCCWebAuditEvent' and EventCode = 100201)">
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
        <tr>
            <td width="90%" align="center" valign="baseline">
            <!-- -->
            <table border="0" bordercolor="#C0C0C0" width="100%" cellpadding="0" cellspacing="0" align="left">        

            <tr>
                <td> 
                    <table border="1" bgcolor="#E9E9E9" bordercolor="silver" cellpadding="0" cellspacing="0" width="100%" align="left">
                        <tr align="left">
                            <td align="left" valign="bottom">Select&nbsp;:&nbsp;
                                <asp:LinkButton ID="btnSelectAll" runat="server" style="color:maroon;text-decoration:none;" OnClick="btnSelectAll_Click" Text="All" />,&nbsp;
                                <asp:LinkButton ID="btnDeselectAll" runat="server" style="color:maroon;text-decoration:none;" OnClick="btnDeSelectAll_Click" Text="None" />,&nbsp;
			                    <asp:LinkButton ID="btnStart" runat="server" style="color:green;text-decoration:none;" OnClientClick="return confirm('Are you sure you want to start?');" OnClick="btnStart_Click" Text="Start" />,&nbsp;
                                <asp:LinkButton ID="btnStop" runat="server" style="text-decoration:none;" OnClientClick="return confirm('Are you sure you want to stop?');" OnClick="btnStop_Click" Text="Stop" />
                            </td>
                        </tr>
                    </table>

                </td>
            </tr>
      
            <tr>
                <td>
		            <table border="1" bordercolor="#C0C0C0" width="100%" cellpadding="0" cellspacing="0" align="center">
                    <tr colspan=2><td>
                                <asp:GridView ID="gridServices" runat="server" AutoGenerateColumns="False" CellPadding="4" EmptyDataText="Unable to find Windows services. Check the Filter file."
                                    ForeColor="white" GridLines="Both" Width="100%" OnRowDataBound="gridServices_RowDataBound">
                                    <Columns>
                                        <asp:TemplateField>
                                            <ItemTemplate>
                                                <asp:CheckBox ID="chkBoxService" runat="server" />
                                            </ItemTemplate>
                                             <ItemStyle HorizontalAlign="Left" Width="2%" Font-Italic="false" />
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="ServiceName" HeaderText="Windows Service">
                                            <ItemStyle HorizontalAlign="Left" Width="75%" Font-Italic="false" Wrap="true"/>
                                        </asp:BoundField>
                                        <asp:TemplateField HeaderText="">
                                           <ItemTemplate>
                                                <bcc:AlertIndicator ID="alertStatus" Status='<%# Bind("ServiceStatus") %>' runat="server" />
                                           </ItemTemplate>
                                           <ItemStyle HorizontalAlign="Left" Width="3%" Font-Italic="false" />
                                        </asp:TemplateField>                                           
                                        <asp:BoundField DataField="ServiceStatus" HeaderText="Status" >
                                            <ItemStyle HorizontalAlign="Center" Width="15%" Font-Italic="false" Wrap="true"/>
                                        </asp:BoundField>
                                        </Columns>
                                        </asp:GridView>
                      </td></tr>
		              </table>
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
                                <asp:LinkButton ID="btnStop2" runat="server" style="text-decoration:none;" OnClientClick="return confirm('Are you sure you want to stop?');" OnClick="btnStop_Click" Text="Stop" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            
            </table>
            <!-- -->
            </td>
        </tr>
        <tr>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
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
                    <b>BizTalk Artifacts - Windows Services </b> <br />
                    The Filters control the list of Windows Services that are visible. You can add additional windows services to this list, by adding additional entries in the filter. <br />
                    <br />You can upload your custom filters in "Administration > System Settings" module. <br />
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
                        SelectCommand="SELECT [Message] FROM aspnet_WebEvent_Events WHERE EventType = 'BCC.Core.BCCWebAuditEvent' and EventCode = 100201 ORDER BY EventTime DESC">
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
