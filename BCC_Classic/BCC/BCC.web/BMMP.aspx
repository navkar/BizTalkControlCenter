<%@ Page Language="C#" MasterPageFile="~/template.master" AutoEventWireup="true" CodeFile="BMMP.aspx.cs" Inherits="MonitorPerformance" EnableViewState="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="leftContentPlaceHolder" Runat="Server">
    <table border="0" bgcolor="#E9E9E9" width="99%" cellpadding="0" cellspacing="0" align="left">
        <tr>
            <td align="center">
		        <asp:Label ID="lblCaption" skinID="PageCaption" runat="server"></asp:Label>
   	        </td>
        </tr>
        <tr>
                <td>
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
       <td>
	        <!-- Grid View for activity log -->
                <asp:SqlDataSource ID="ActivityDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:authStore %>"
                    SelectCommand="SELECT 'Recent activity' as Comment, [Message] FROM aspnet_WebEvent_Events WHERE EventTime = (select MAX(EventTime) FROM aspnet_WebEvent_Events WHERE EventType = 'BCC.Core.BCCWebAuditEvent' and EventCode = 100104)">
                </asp:SqlDataSource>

                <asp:GridView id="UserActivityGrid" SkinID="sideTable" Visible="true" runat="server" Width="100%" ForeColor="#333333"
                            CellPadding="4" CellSpacing="0" AutoGenerateColumns="False" DataSourceID="ActivityDataSource" 
                        ShowFooter="false" ShowHeader="false" Gridlines="Both" EmptyDataText="No Record(s) available">
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
        <tr>
            <td>
                    <table border="1" bgcolor="#E9E9E9" bordercolor="#C0C0C0" cellpadding="0" cellspacing="0" width="100%" align="left">
                        <tr align="left">
                            <td align="left" valign="bottom">Select&nbsp;:&nbsp;
                                <asp:LinkButton ID="btnSelectAll" runat="server" style="color:maroon;text-decoration:none;" OnClick="btnSelectAll_Click" Text="All" />,&nbsp;
                                <asp:LinkButton ID="btnDeselectAll" runat="server" style="color:maroon;text-decoration:none;" OnClick="btnDeSelectAll_Click" Text="None" />,&nbsp;
	                            <asp:LinkButton ID="btnRemove" runat="server" style="color:maroon;text-decoration:none;" OnClientClick="return confirm('Are you sure you want to remove?');" OnClick="btnRemove_Click" Text="Remove" />
                            </td>
                        </tr>
                    </table>
            </td>
        </tr>       
        <tr>
            <td>
		<table border="0" bordercolor="#B2B2B2" width="100%" cellpadding="0" cellspacing="0" align="center">
                <tr><td style="word-break:break-all;">
                                
                            <asp:GridView ID="gridPerfMon" runat="server" AutoGenerateEditButton="False"
                                AutoGenerateColumns="False" CellPadding="4" 
                                AllowSorting="true" 
                                AllowPaging="true" PageSize="15"
                                EmptyDataText="No record(s) found to set performance counters, use speedcode '303'."
                                GridLines="Both" width="100%"
                                OnSorting="gridPerfMon_OnSorting" 
                                OnRowDataBound="gridPerfMon_RowDataBound" 
                                OnRowEditing="gridPerfMon_RowEditing"   
                                OnRowCancelingEdit="gridPerfMon_RowCancelEditing"
                                OnRowUpdating="gridPerfMon_RowUpdating"
                                OnPageIndexChanging="gridPerfMon_PageIndexChanging"
                                >
                                <Columns>
                                    <asp:TemplateField>
                                        <ItemStyle Width="1%" Font-Italic="false" Wrap="true"/>
                                        <ItemTemplate>
                                            <asp:CheckBox ID="chkBoxNotify" runat="server" />
                                        </ItemTemplate>
                                    </asp:TemplateField>                                
                                    <asp:TemplateField HeaderText="Category Name" ControlStyle-Font-Bold="false" SortExpression="PerfCategory">
                                        <ItemTemplate>
                                            <asp:Label ID="lblCategory" runat="server" Text='<%# Bind("PerfCategory") %>'></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle Width="15%" Font-Italic="false" Wrap="false"/>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Performance Counter" ControlStyle-Font-Bold="false" SortExpression="PerfCounterName">
                                        <ItemTemplate>
                                            <asp:Label ID="lblCounter" runat="server" Text='<%# Bind("PerfCounterName") %>'></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle Width="15%" Font-Italic="false" Wrap="true"/>
                                    </asp:TemplateField> 
                                    <asp:TemplateField HeaderText="Instance Name" ControlStyle-Font-Bold="false" SortExpression="PerfInstance">
                                        <ItemTemplate>
                                            <asp:Label ID="lblInstance" runat="server" Text='<%# Bind("PerfInstance") %>'></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle Width="12%" Font-Italic="false" Wrap="true"/>
                                    </asp:TemplateField>                                                                                                                    
                                    <asp:TemplateField HeaderText="">
                                           <ItemTemplate>
                                                <bcc:AlertIndicator ID="alertStatus" Status='<%# Eval("StatusFlag").ToString() == "True" ? "Enabled" : "Disabled"  %>' runat="server" />
                                           </ItemTemplate>
                                           <ItemStyle HorizontalAlign="Left" Width="1%" Font-Italic="false" />
                                    </asp:TemplateField>                                    
                                    <asp:TemplateField HeaderText="Status" HeaderStyle-HorizontalAlign="Center" ControlStyle-Font-Bold="false" SortExpression="Status"> 
                                        <EditItemTemplate>
                                            <asp:DropDownList ID="ddlStatus" runat="server" SelectedValue='<%# Eval("StatusFlag")%>'>
                                                <asp:ListItem Text="Disable" Value="False"></asp:ListItem>
                                                <asp:ListItem Text="Enable" Value="True"></asp:ListItem>
                                            </asp:DropDownList>
                                        </EditItemTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="lblStatus" runat="server" Text='<%# Bind("StatusFlag") %>'></asp:Label>
                                        </ItemTemplate>
                                       <ItemStyle Width="5%" Font-Italic="false" Wrap="false"/>
                                    </asp:TemplateField>                                      
                                    
                                    <asp:TemplateField HeaderText="Interval" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" ControlStyle-Font-Bold="false" SortExpression="PollingInterval">
                                        <ItemTemplate>
                                             <asp:Label ID="lblPollingInterval" runat="server" Text='<%# Bind("PollingInterval", "{0} secs") %>'></asp:Label>
                                        </ItemTemplate>
                                        <EditItemTemplate>
                                            <asp:TextBox ID="tPollInterval" MaxLength="3" runat="Server" Text='<%# Bind("PollingInterval") %>' Columns="3" Width="50%"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="rfv" runat="server" Text="*" ControlToValidate="tPollInterval"></asp:RequiredFieldValidator>
                                        </EditItemTemplate>
                                        <ItemStyle Width="7%" Font-Italic="false" Wrap="true"/>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Updated on" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Center" ControlStyle-Font-Bold="false" SortExpression="LastUpdate">
                                        <ItemTemplate>
                                             <asp:Label ID="lblUpdatedOn" runat="server" Text='<%# Bind("LastUpdate", "{0:MMM-dd-yy hh:mm:ss tt}") %>'></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle Width="7%" Font-Italic="false" Wrap="true"/>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="" ShowHeader="False" HeaderStyle-HorizontalAlign="Left"> 
                                        <EditItemTemplate> 
                                            <asp:LinkButton ID="lnkUpdate" runat="server" CausesValidation="True" CommandName="Update" >
                                                <asp:Image ID="Image1" runat="server" ImageUrl="Images/ok.png" ToolTip="Update" />
                                            </asp:LinkButton> 
                                            <asp:LinkButton ID="lnkCancel" runat="server" CausesValidation="False" CommandName="Cancel">
                                                <asp:Image ID="Image2" runat="server" ImageUrl="Images/cancel.png" ToolTip="Cancel" />
                                            </asp:LinkButton> 
                                        </EditItemTemplate> 
                                        <ItemTemplate> 
                                            <asp:LinkButton ID="lnkEdit" runat="server" CausesValidation="False" CommandName="Edit" >
                                                    <asp:Image ID="Image3" runat="server" ImageAlign="Top" ImageUrl="Images/edit.png" ToolTip="Edit record" />
                                            </asp:LinkButton>
                                            <asp:LinkButton ID="lnkReport" runat="server" CausesValidation="False" CommandName="Report" >
                                                    <asp:Image ID="Image4" runat="server" ImageUrl="Images/report.png" ToolTip="View Chart" />
                                            </asp:LinkButton>  
                                        </ItemTemplate> 
                                        <ItemStyle Width="3%" Font-Italic="false" Wrap="false"/>                                        
                                    </asp:TemplateField>                                                                           
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
                            <asp:LinkButton ID="btnRemove2" runat="server" style="color:maroon;text-decoration:none;" OnClientClick="return confirm('Are you sure you want to remove?');" OnClick="btnRemove_Click" Text="Remove" />
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
                    <b>Monitor Performance</b><br />
                    Changes to the status or notification interval will take effect after <b>60 secs </b> by the BCC Agent service.
                    <br /><br />
                    <b>Creating a new monitoring parameter</b><br />
                    Use speedcode '303'.
                    <br /><br />
                    <b>Monitoring Timer Interval</b><br />
                    If the interval value is set to 10 seconds, this indicates that the performance counters value is read every 10 seconds. <br />
                    It is recommended to leave the interval value at <b>10 secs</b>, unless you want to explicitly set the value for your performance counter.
                    <br /><br />                    
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
                    SelectCommand="SELECT [Message] FROM aspnet_WebEvent_Events WHERE EventType = 'BCC.Core.BCCWebAuditEvent' and EventCode = 100104 ORDER BY EventTime DESC">
                </asp:SqlDataSource>

                <asp:GridView id="UserActivityGrid2" SkinID="sideTable" Visible="true" runat="server" Width="100%" ForeColor="#333333"
                            CellPadding="4" CellSpacing="0" AutoGenerateColumns="False" DataSourceID="ActivityDataSource2"
                        AllowPaging="True" PageSize="8" AllowSorting="True" Gridlines="Both" EmptyDataText="No Record(s) available">
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


