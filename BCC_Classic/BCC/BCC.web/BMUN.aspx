<%@ Page Language="C#" MasterPageFile="~/template.master" AutoEventWireup="true" CodeFile="BMUN.aspx.cs" Inherits="UserNotifications" %>
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
                    SelectCommand="SELECT 'Recent activity' as Comment, [Message] FROM aspnet_WebEvent_Events WHERE EventTime = (select MAX(EventTime) FROM aspnet_WebEvent_Events WHERE EventType = 'BCC.Core.BCCWebAuditEvent' and EventCode = 100103)">
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
                                
                            <asp:GridView ID="gridNotifView" runat="server" AutoGenerateEditButton="False"
                                AutoGenerateColumns="False" CellPadding="4" 
                                AllowSorting="true" OnSorting="gridNotifView_OnSorting" 
                                AllowPaging="true" PageSize="15"
                                EmptyDataText="No record(s) found to set notification alerts."
                                GridLines="Both" width="100%"
                                OnRowDataBound="gridNotifView_RowDataBound" 
                                OnRowEditing="gridNotifView_RowEditing"   
                                OnRowCancelingEdit="gridNotifView_RowCancelEditing"
                                OnRowUpdating="gridNotifView_RowUpdating"
                                OnPageIndexChanging="gridNotifView_PageIndexChanging"
                                >
                                <Columns>
                                    <asp:TemplateField>
                                        <ItemStyle Width="1%" Font-Italic="false" Wrap="true"/>
                                        <ItemTemplate>
                                            <asp:CheckBox ID="chkBoxNotify" runat="server" />
                                        </ItemTemplate>
                                    </asp:TemplateField>                                
                                    <asp:TemplateField HeaderText="Artifact Type" ControlStyle-Font-Bold="false" SortExpression="ArtifactType">
                                        <ItemTemplate>
                                            <asp:Label ID="lblArtifactType" runat="server" Text='<%# Bind("ArtifactType") %>'></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle Width="8%" Font-Italic="false" Wrap="false"/>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Name" ControlStyle-Font-Bold="false" SortExpression="ArtifactName">
                                        <ItemTemplate>
                                            <asp:Label ID="lblArtifactName" runat="server" Text='<%# Bind("ArtifactName") %>'></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle Width="18%" Font-Italic="false" Wrap="true"/>
                                    </asp:TemplateField>                                                                                
                                    <asp:BoundField DataField="ArtifactDesc" HeaderText="Description" ReadOnly="true" SortExpression="ArtifactDesc">
                                        <ItemStyle Width="12%" Font-Italic="false" Wrap="true"/>
                                    </asp:BoundField>
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
                                    
                                    <asp:TemplateField HeaderText="Notify" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" ControlStyle-Font-Bold="false" SortExpression="PollingInterval">
                                        <ItemTemplate>
                                             <asp:Label ID="lblPollingInterval" runat="server" Text='<%# Bind("PollingInterval", "{0} secs") %>'></asp:Label>
                                        </ItemTemplate>
                                        <EditItemTemplate>
                                            <asp:TextBox ID="tPollInterval" runat="Server" Text='<%# Bind("PollingInterval") %>' Columns="3" BorderStyle="Groove" Width="50%"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="rfv" runat="server" Text="*" ControlToValidate="tPollInterval"></asp:RequiredFieldValidator>
                                        </EditItemTemplate>
                                        <ItemStyle Width="6%" Font-Italic="false" Wrap="true"/>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="LastUpdate" HeaderText="Updated on" ReadOnly="true" SortExpression="LastUpdate">
                                        <ItemStyle Width="7%" Font-Italic="false" Wrap="false"/>
                                    </asp:BoundField>
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
                    <b>User Notifications</b><br />
                    Changes to the status or notification interval will take effect after <b>60 secs </b> by the BCC Agent service.
                    <br /><br />
                    <b>Notification Interval</b><br />
                    It is recommended to leave the interval value at <b>10 secs</b>, unless you want to explicitly set the value for your artifact.
                    <br /><br />                    
                    <b>Notification Email</b><br />
                    To configure Email ids, use speedcode '603'.
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
                    SelectCommand="SELECT [Message] FROM aspnet_WebEvent_Events WHERE EventType = 'BCC.Core.BCCWebAuditEvent' and EventCode = 100103 ORDER BY EventTime DESC">
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


