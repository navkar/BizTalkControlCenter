<%@ Page Language="C#" MasterPageFile="~/template.master" AutoEventWireup="true"
    CodeFile="PVSJ.aspx.cs" Inherits="PartnerJobs"
	EnableEventValidation="False" EnableViewState="false" %>
	
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
                SelectCommand="SELECT 'Recent activity' as Comment, [Message] FROM aspnet_WebEvent_Events WHERE EventTime = (select MAX(EventTime) FROM aspnet_WebEvent_Events WHERE EventType = 'BCC.Core.BCCWebAuditEvent' and EventCode = 100302)">
            </asp:SqlDataSource>

 	        <asp:GridView id="UserActivityGrid" Visible="false" runat="server" Width="100%" ForeColor="#333333"
                    CellPadding="4" CellSpacing="0" AutoGenerateColumns="False" DataSourceID="ActivityDataSource"
	                Gridlines="None" EmptyDataText="No record(s) found" ShowFooter="false" ShowHeader="false">
  	                <Columns>
  	                    <asp:BoundField DataField="Comment">
                              <ItemStyle Width="12%" Font-Italic="false" Wrap="true"/>
                        </asp:BoundField>
                        <asp:BoundField DataField="Message">
                              <ItemStyle Width="88%" Font-Italic="false" Wrap="true"/>
                        </asp:BoundField>
                    </Columns>
  	        </asp:GridView>
        </td>
        </tr>
        <!-- Grid View for activity log -->
        <tr>
            <td>
		        <asp:Panel ID="tablePanel" runat="server" >
                        <table border="1" bgcolor="#E9E9E9" bordercolor="#B2B2B2" cellpadding="0" cellspacing="0" width="100%" align="left">
                            <tr align="left">
                                <td align="left" valign="bottom">Select&nbsp;:&nbsp;
                                    <asp:LinkButton ID="btnSelectAll" runat="server" style="color:maroon;text-decoration:none;" OnClick="btnSelectAll_Click" Text="All" />,&nbsp;
                                    <asp:LinkButton ID="btnDeselectAll" runat="server" style="color:maroon;text-decoration:none;" OnClick="btnDeSelectAll_Click" Text="None" />,&nbsp;
			                        <asp:LinkButton ID="btnStart" runat="server" style="color:green;text-decoration:none;" OnClientClick="return confirm('Are you sure you want to start?');" OnClick="btnStart_Click" Text="Start" />,&nbsp;
                                    <asp:LinkButton ID="btnStop" runat="server" style="text-decoration:none;" OnClientClick="return confirm('Are you sure you want to stop?');" OnClick="btnStop_Click" Text="Stop" />,&nbsp;
                                    <asp:LinkButton ID="btnEnableDisable" runat="server" style="color:green;text-decoration:none;" OnClientClick="return confirm('Are you sure you want to do this?');" OnClick="btnEnableDisable_Click" Text="Toggle (Enable/Disable)" />
                                </td>
			                    <td align="right" width="1%">
			                        <asp:LinkButton ID="btnExportToExcel" runat="server" style="color:teal;text-decoration:none;" OnClick="btnExportToExcel_Click" >
				                            <asp:Image ID="excel" runat="server" ImageUrl="~/Images/Excel-16.gif" />
			                        </asp:LinkButton>
			                    </td>
                            </tr>
                            <tr align="left">
                                <td align="left" valign="middle" colspan="2">
                                    <uc:Search ID="search" runat="Server" /> 
                                </td>
                            </tr>
                        </table>
		        </asp:Panel>
            </td>
        </tr>
         <tr>
            <td>
		<table border="1" bordercolor="#B2B2B2" width="100%" cellpadding="0" cellspacing="0" align="center">
                <tr><td style="word-break:break-all;">
                            <asp:GridView ID="gridSQLJobs" runat="server" AutoGenerateColumns="False" CellPadding="4" EmptyDataText="No record(s) found."
                                ForeColor="#333333" GridLines="Both" width="100%" PageSize="20" OnRowDataBound="gridSQLJobs_RowDataBound">
                                <Columns>
                                    <asp:TemplateField>
                                        <ItemStyle Width="1%" Font-Italic="false" Wrap="true"/>
                                        <ItemTemplate>
                                            <asp:CheckBox ID="chkJobID" runat="server" />
                                            <asp:HiddenField ID="jobConnectionString" runat="server" Value='<%# DataBinder.Eval(Container.DataItem, "JobServerConnectionString")%>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="JobName" HeaderText="Job Name">
                                        <ItemStyle Width="37%" Font-Italic="false" Wrap="true"/>
                                    </asp:BoundField>
                                    <asp:TemplateField HeaderText="Active?" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" ControlStyle-Font-Bold="false" SortExpression="IsJobEnabled">
                                        <ItemTemplate>
                                                <asp:Label ID="lblIsJobEnabled" CausesValidation="false" style="font-weight:bold;display:block;width:50px;text-decoration:none" runat="server" Text='<%# Bind("IsJobEnabled") %>' />
                                        </ItemTemplate>
                                        <ItemStyle Width="9%" Font-Italic="false" Wrap="false" />
                                    </asp:TemplateField>                                    
                                                                
                                    <asp:TemplateField HeaderText="Status" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" ControlStyle-Font-Bold="false" SortExpression="status">
                                        <ItemTemplate>
                                                <asp:Label ID="lblJobStatus" CausesValidation="false" style="font-weight:bold;display:block;width:75px;text-decoration:none" runat="server" Text='<%# Bind("JobExecutionStatus") %>' />
                                        </ItemTemplate>
                                        <ItemStyle Width="10%" Wrap="false" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Result" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" ControlStyle-Font-Bold="false" SortExpression="JobResult">
                                        <ItemTemplate>
                                                <asp:Label ID="lblJobResult" CausesValidation="false" style="font-weight:bold;display:block;width:75px;text-decoration:none" runat="server" Text='<%# Bind("JobResult") %>' />
                                        </ItemTemplate>
                                        <ItemStyle Width="10%" Font-Italic="false" Wrap="false" />
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Last Run" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" ControlStyle-Font-Bold="false" SortExpression="LastRunDate">
                                        <ItemTemplate>
                                                <asp:Label ID="lblLastRun" CausesValidation="false" style="font-weight:bold;display:block;width:75px;text-decoration:none" runat="server" Text='<%# Bind("LastRunDate") %>' />
                                        </ItemTemplate>
                                        <ItemStyle Width="7%" Font-Italic="false" Wrap="false" />
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Next Run" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" ControlStyle-Font-Bold="false" SortExpression="NextRunDate">
                                        <ItemTemplate>
                                                <asp:Label ID="lblNextRun" CausesValidation="false" style="font-weight:bold;display:block;width:75px;text-decoration:none" runat="server" Text='<%# Bind("NextRunDate") %>' />
                                        </ItemTemplate>
                                        <ItemStyle Width="7%" Font-Italic="false" Wrap="false" />
                                    </asp:TemplateField>

                                    <asp:BoundField DataField="JobServer" HeaderText="Server">
                                        <ItemStyle Width="5%" Font-Italic="false" Wrap="false"/>
                                    </asp:BoundField>
                                </Columns>
                            </asp:GridView>
                </td></tr>
		</table>
            </td>
        </tr>
         <tr>
            <td> 
                <table border="1" bgcolor="#E9E9E9" bordercolor="#B2B2B2" cellpadding="0" cellspacing="0" width="100%" align="left">
                    <tr align="left">
                        <td align="left" valign="bottom">Select&nbsp;:&nbsp;
                            <asp:LinkButton ID="btnSelectAll2" runat="server" style="color:maroon;text-decoration:none;" OnClick="btnSelectAll_Click" Text="All" />,&nbsp;
                            <asp:LinkButton ID="btnDeselectAll2" runat="server" style="color:maroon;text-decoration:none;" OnClick="btnDeSelectAll_Click" Text="None" />,&nbsp;
			                <asp:LinkButton ID="btnStart2" runat="server" style="color:green;text-decoration:none;" OnClientClick="return confirm('Are you sure you want to start?');" OnClick="btnStart_Click" Text="Start" />,&nbsp;
                            <asp:LinkButton ID="btnStop2" runat="server" style="text-decoration:none;" OnClientClick="return confirm('Are you sure you want to stop?');" OnClick="btnStop_Click" Text="Stop" />,&nbsp;
                            <asp:LinkButton ID="btnEnableDisable2" runat="server" style="color:green;text-decoration:none;" OnClientClick="return confirm('Are you sure you want to do this?');" OnClick="btnEnableDisable_Click" Text="Toggle (Enable/Disable)" />
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
             <b>SQL Jobs</b> <br />
	            The user profile controls the list of jobs which can be started/stopped. <br />
                <br />
             <b>Notes</b><br />
             SQL Jobs from multiple servers is supported. You would need to modify the user profile to do it. <br />
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
           <td style="word-break:break-all;">
	            <!-- Grid View for activity log -->
                    <asp:SqlDataSource ID="ActivityDataSource2" runat="server" ConnectionString="<%$ ConnectionStrings:authStore %>"
                        SelectCommand="SELECT [Message] FROM aspnet_WebEvent_Events WHERE EventType = 'BCC.Core.BCCWebAuditEvent' and EventCode = 100302 ORDER BY EventTime DESC">
                    </asp:SqlDataSource>

                    <asp:GridView id="UserActivityGrid2" SkinID="sideTable" Visible="true" runat="server" Width="100%" ForeColor="#333333"
                                CellPadding="4" CellSpacing="0" AutoGenerateColumns="False" DataSourceID="ActivityDataSource2"
                            AllowPaging="True" PageSize="10" AllowSorting="True" Gridlines="Both" EmptyDataText="No record(s) found">
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