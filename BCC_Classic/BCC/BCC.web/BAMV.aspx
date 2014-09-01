<%@ Page Language="C#" MasterPageFile="~/template.master" AutoEventWireup="true"
    CodeFile="BAMV.aspx.cs" Inherits="BiztalkMessageView" 
    EnableViewState="false" %>

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
                SelectCommand="SELECT 'Recent activity' as Comment, [Message] FROM aspnet_WebEvent_Events WHERE EventTime = (select MAX(EventTime) FROM aspnet_WebEvent_Events WHERE EventType = 'BCC.Core.BCCWebAuditEvent' and EventCode = 100204)">
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
            <td>
                <table border="1" bgcolor="#E9E9E9" bordercolor="#C0C0C0" cellpadding="0" cellspacing="0" width="100%" align="left">
                    <tr align="left">
                        <td align="left" valign="bottom">Select&nbsp;:&nbsp;
                            <asp:LinkButton ID="btnSelectAll" runat="server" style="color:maroon;text-decoration:none;" OnClick="btnSelectAll_Click" Text="All" />,&nbsp;
                            <asp:LinkButton ID="btnDeselectAll" runat="server" style="color:maroon;text-decoration:none;" OnClick="btnDeSelectAll_Click" Text="None" />,&nbsp;
			                <asp:LinkButton ID="btnResume" runat="server" style="color:green;text-decoration:none;" OnClientClick="return confirm('Are you sure you want to resume?');" OnClick="btnResume_Click" Text="Resume" />,&nbsp;
                            <asp:LinkButton ID="btnTerminate" runat="server" style="color:red;text-decoration:none;" OnClientClick="return confirm('Are you sure you want to terminate?');" OnClick="btnTerminate_Click" Text="Terminate" />,&nbsp;
            			    [<%=totalMessages %>]
                        </td>
			<td align="right" width="1%">
				<asp:LinkButton ID="btnExportToExcel" runat="server" style="color:teal;text-decoration:none;" OnClick="btnExportToExcel_Click" >
					<asp:Image ID="excel" runat="server" ImageUrl="~/Images/Excel-16.gif" />
				</asp:LinkButton>
			</td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>
		    <asp:UpdateProgress ID="updateView" runat="server" DynamicLayout="true" DisplayAfter="1" AssociatedUpdatePanelID="msgViewPanel">
            <ProgressTemplate>    
            <div align="left">
                <div style="background-color:#E9E9E9;padding:6px;border:solid 1px black;width:98%;text-align:center;">
                    <span><i><b>Processing please wait...</b></i>
                    <asp:Image ID="img" runat="server" ImageUrl="~/Images/loading.gif" ImageAlign="AbsMiddle" />
                    </span>
                </div>
            </div>
            </ProgressTemplate>
            </asp:UpdateProgress>  		    
            <asp:UpdatePanel ID="msgViewPanel" runat="server" RenderMode="Block" UpdateMode="Conditional">
		    <ContentTemplate>
		    <table border="1" bordercolor="#C0C0C0" width="100%" cellpadding="0" cellspacing="0" align="center">
                <tr><td>
                <asp:GridView ID="gridBTMessages" runat="server" AutoGenerateColumns="False" CellPadding="4"
                    ForeColor="#333333" GridLines="Both" Width="100%" PageSize="25"
		            EmptyDataText="No data found"
        		    AllowPaging="True" AllowSorting="True"
                    OnRowDataBound="gridBTMessages_RowDataBound" 
		            OnPageIndexChanging="gridBTMessages_PageIndexChanging"
                    OnSorting="gridBTMessages_Sorting">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:CheckBox ID="chkBoxMessage" runat="server"/>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="MessageID" HeaderText="Message ID" />
                        <asp:BoundField DataField="Status" HeaderText="Status" SortExpression="Status" />
                        <asp:BoundField DataField="InstanceID" HeaderText="Instance ID" />
                        <asp:BoundField DataField="CreationTime" HeaderText="Creation time" SortExpression="CreationTime" />
                        <asp:BoundField DataField="Application" HeaderText="Application" SortExpression="Application" />
                        <asp:BoundField DataField="HostName" HeaderText="Host Name" SortExpression="HostName" />
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:HyperLink ID="link" runat="server" NavigateUrl='<%# Eval("MessageID", "BAMV-D.aspx?ID={0}") %>'
                                    Target="_blank" style="color:teal;text-decoration:none;" Text="View"></asp:HyperLink>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
                </td></tr>
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
			                <asp:LinkButton ID="btnResume2" runat="server" style="color:green;text-decoration:none;" OnClientClick="return confirm('Are you sure you want to resume?');" OnClick="btnResume_Click" Text="Resume" />,&nbsp;
                            <asp:LinkButton ID="btnTerminate2" runat="server" style="color:red;text-decoration:none;" OnClientClick="return confirm('Are you sure you want to terminate?');" OnClick="btnTerminate_Click" Text="Terminate" />,&nbsp;
			                [<%=totalMessages %>]
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
		    <asp:Label ID="lblSubCaption" skinID="PageCaption" runat="server"></asp:Label>
   	        </td>
        </tr>
       <tr>
            <td align="center">&nbsp;
   	        </td>
       </tr>        
       <tr>
            <td align="center">
				    <!--- Shady code goes here -->
            <div class="shiftcontainer">
            <div class="shadowcontainer300">
            <div class="innerdiv" align="left">
            <b>Adding new BizTalk Applications</b> <br /> 
            If you are expecting to view messages and are not seeing them, you might want to add additional <b>BizTalk Applications</b> into the user profile, use speedcode <b>603</b>.
            <br /><br />
            <asp:DataList ID="dlBTAppList" runat="server" skinID="sideList" >
                <HeaderTemplate>
                Configured BizTalk Applications
                </HeaderTemplate>
                <ItemTemplate>
                <%#Container.DataItem%>
                </ItemTemplate>
            </asp:DataList>
            <br />
            <b>How to monitor a BizTalk Application for Suspended messages?</b> <br />
             Click the 'Enable monitoring' link and use speedcode <b>103</b> to view the list of all monitored artifacts.<br /><br />        
            <b>Maximum message view limit:</b>
		    <asp:Label ID="lblMsgLimit" runat="server" style="background-color:Gray;color:White;font-weight:bold;" Visible="false" Width="18%"></asp:Label>
            <br />Use speedcode '603' to change it.
            <br />
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
            <td>
                <!-- Inner Table -->
                <table align="left" border="1" width="100%" cellpadding="0" cellspacing="0" bgcolor="#FFFFCC" style="color:#333333;border-collapse:collapse;">
                <tr>
                    <td align="center" style="color:#FDD017;background-color:#525252;font-weight:bold;">Message View Monitoring</td>
                </tr>
                <tr>
                    <td align="left" valign="middle">
                        <asp:Image ID="user" ToolTip="Enable Monitoring" ImageAlign="Left" ImageUrl="~\Images\OK-small.png" runat="server" />
                        <asp:LinkButton ID="updateBtn" style="color:teal;text-decoration:none;" OnClick="btnMonitor_Click" OnClientClick="alert('Use speedcode \'103\' to view the list of monitored artifacts.');return true;" text="Enable Monitoring" runat="server">
                        </asp:LinkButton>
                    </td>
                </tr>
                </table>
                <!-- Inner Table -->               
            </td>
       </tr>   
       <tr>
            <td align="center">&nbsp;
   	        </td>
       </tr>
       <tr align="center" valign="middle">
            <td width="100%" align="left" style="word-break:break-all;">
                <!-- GridView for messaging activity -->
                <asp:GridView SkinID="sideTable" id="gridMsgInfo" Visible="true" runat="server" Width="100%" ForeColor="#333333"
                        CellPadding="4" CellSpacing="0" AutoGenerateColumns="False" Gridlines="Both" EmptyDataText="">
                     <Columns>
                            <asp:BoundField DataField="MessageState" HeaderText="Message State" >
                                  <ItemStyle Width="80%" Font-Italic="false" Wrap="true"/>
                            </asp:BoundField>
                            <asp:BoundField DataField="Count" HeaderText="Count" >
                                  <ItemStyle HorizontalAlign="Center" Width="20%" Font-Italic="false" Wrap="true"/>
                            </asp:BoundField>
                     </Columns>
                </asp:GridView>
                <!-- GridView for messaging activity -->
  	        </td>
        </tr>
       <tr>
            <td align="center">&nbsp;
   	        </td>
       </tr>
       <tr>
           <td>
	            <!-- Grid View for activity log -->
                    <asp:SqlDataSource ID="ActivityDataSource2" runat="server" ConnectionString="<%$ ConnectionStrings:authStore %>"
                        SelectCommand="SELECT [Message] FROM aspnet_WebEvent_Events WHERE EventType = 'BCC.Core.BCCWebAuditEvent' and EventCode = 100204 ORDER BY EventTime DESC">
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
