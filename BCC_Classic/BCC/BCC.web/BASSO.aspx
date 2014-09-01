<%@ Page Language="C#" MasterPageFile="~/template.master" AutoEventWireup="true" CodeFile="BASSO.aspx.cs" Inherits="SSOConfigView" EnableViewState="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="leftContentPlaceHolder" runat="Server">

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
                    SelectCommand="SELECT 'Recent activity' as Comment, [Message] FROM aspnet_WebEvent_Events WHERE EventTime = (select MAX(EventTime) FROM aspnet_WebEvent_Events WHERE EventType = 'BCC.Core.BCCWebAuditEvent' and EventCode = 100211)">
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
               <asp:Panel ID="tablePanel" runat="server">
                <table border="1" bgcolor="#E9E9E9" bordercolor="#C0C0C0" cellpadding="0" cellspacing="0" width="100%" align="left">
                    <tr align="left">
                        <td align="left" valign="bottom">
                            <asp:TextBox id="txtSearchKey" runat="server" BorderStyle="Groove" Tooltip="Application name to create in SSO DB" Visible="false" />
                            <asp:LinkButton ID="btnFilter" runat="server" style="visibility:hidden;display:none;text-decoration:none;" OnClick="btnFilter_Click" Text="Search" />
                            SSO applications : <%=searchTerms%> <%=testData%> [<%=count%>]
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
		        <table border="0" bordercolor="#5D7B9D" width="80%" cellpadding="0" cellspacing="0" align="left">
                <tr>
                    <td colspan="2" align="left">
		            <asp:GridView ID="gridAppView" runat="server" AutoGenerateColumns="False" CellPadding="4"
                        ForeColor="white" GridLines="Both" Width="100%">
                        <Columns>
			                <asp:BoundField DataField="AppUserAccount" HeaderText="Application (User Account)">
			                    <ItemStyle Wrap="false" Width="20%" />
                             </asp:BoundField>
                            <asp:BoundField DataField="AppAdminAccount" HeaderText="Application (Administrator Account)">
                                   <ItemStyle Wrap="false" Width="20%" />
                             </asp:BoundField>
                             <asp:BoundField DataField="Description" HeaderText="Description" >
                                <ItemStyle Wrap="true" Width="50%" />
                             </asp:BoundField>
                            <asp:BoundField DataField="ContactInfo" HeaderText="Contact Info" >
                                <ItemStyle Wrap="false" Width="10%" />
                             </asp:BoundField>
                        </Columns>
                    </asp:GridView>
                    </td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td colspan="2">
		            <asp:GridView ID="gridSSO" runat="server" EnableTheming="true" AutoGenerateColumns="False" CellPadding="4" 
		                         ForeColor="white" GridLines="Both" Width="100%" Visible="true"
		                         DataKeyNames="Application" ShowFooter="False"
                                 OnRowCancelingEdit="gridSSO_RowCancelingEdit" 
                                 OnRowDataBound="gridSSO_RowDataBound" 
                                 OnRowEditing="gridSSO_RowEditing" 
                                 OnRowUpdating="gridSSO_RowUpdating"  
                                 OnRowCommand="gridSSO_RowCommand" 
                                 OnRowDeleting="gridSSO_RowDeleting">
                        <Columns>
                            <asp:BoundField ReadOnly="True" HeaderText="Application" InsertVisible="False" DataField="Application" >
                            </asp:BoundField>
                            <asp:TemplateField HeaderText="Key Name" HeaderStyle-HorizontalAlign="Center" ControlStyle-Font-Bold="false"> 
                               <EditItemTemplate> 
                                    <asp:Label ID="txtKeyName" runat="server" Text='<%# Bind("KeyName") %>'></asp:Label>
                               </EditItemTemplate> 
                               <ItemTemplate> 
                                    <asp:Label ID="lblKeyName" runat="server" Text='<%# Bind("KeyName") %>'></asp:Label> 
                               </ItemTemplate>
                               <ItemStyle Width="35%" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Key Value" HeaderStyle-HorizontalAlign="Center" ControlStyle-Font-Bold="false"> 
                               <EditItemTemplate> 
                                    <asp:TextBox Width="95%" ID="txtKeyValue" Runat="server" Text='<%# Bind("KeyValue") %>'></asp:TextBox> 
                               </EditItemTemplate> 
                               <ItemTemplate> 
                                    <asp:Label ID="lblKeyValue" Runat="server" Text='<%# Bind("KeyValue") %>'></asp:Label> 
                               </ItemTemplate>
                                <ItemStyle Width="35%" />
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
                                            <asp:Image ID="Image3" runat="server" ImageAlign="Baseline" ImageUrl="Images/edit.png" ToolTip="Edit record" />
                                    </asp:LinkButton>
                                </ItemTemplate> 
                                 <ItemStyle Width="5%" Wrap="false" />
                            </asp:TemplateField> 
                           
                        </Columns>
                    </asp:GridView>
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
                    <b>SSO Configuration</b><br />
                    Use this module to view the various BizTalk configuration parameters in the SSO DB (table name - SSOX_ApplicationInfo). Click on the list of applications or create a new one.
                    <br /><br />
                    <b>Creating a new SSO Application</b><br />
                    Select Deployment > Configuration or type speedcode '502' to create a new SSO configuration application. 
                    <br /><br />
                    <b>Add/Delete keys to an existing SSO Application</b><br />
                    In order to add/delete keys to an existing SSO, type speedcode '502' and specify a new SSO configuration application with the existing and the new keys.
                    <br /> <br />
                    <asp:DataList ID="dlSSOAppList" runat="server" skinID="sideList" >
                        <HeaderTemplate>
                        Configured SSO Applications
                        </HeaderTemplate>
                        <ItemTemplate>
                        <%#Container.DataItem%>
                        </ItemTemplate>
                    </asp:DataList>
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
	        <!-- Grid View for activity log -->
                <asp:SqlDataSource ID="ActivityDataSource2" runat="server" ConnectionString="<%$ ConnectionStrings:authStore %>"
                    SelectCommand="SELECT [Message] FROM aspnet_WebEvent_Events WHERE EventType = 'BCC.Core.BCCWebAuditEvent' and EventCode = 100211 ORDER BY EventTime DESC">
                </asp:SqlDataSource>

                <asp:GridView id="UserActivityGrid2" SkinID="sideTable" Visible="true" runat="server" Width="100%" ForeColor="#333333"
                            CellPadding="4" CellSpacing="0" AutoGenerateColumns="False" DataSourceID="ActivityDataSource2"
                        AllowPaging="True" PageSize="5" AllowSorting="True" Gridlines="Both" EmptyDataText="No Record(s) available">
                        <Columns>
                                <asp:BoundField DataField="Message" HeaderText="Change history" SortExpression="Message">
                                      <ItemStyle Width="30%" Font-Italic="false" Wrap="true"/>
                                </asp:BoundField>
                            </Columns>
                </asp:GridView>
        </td>
       </tr>
       </table>
</asp:Content>
