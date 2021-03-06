<%@ Page Language="C#" MasterPageFile="~/template.master" AutoEventWireup="true" CodeFile="DV.aspx.cs" Inherits="DeploymentVerification" Title="" %>
<asp:Content ID="Content1" ContentPlaceHolderID="leftContentPlaceHolder" Runat="Server">
     <table border="0"  width="100%" cellpadding="0" cellspacing="0" align="left">
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
        <!-- Grid View for activity log -->
        <tr>
            <td colspan="2">
            <asp:SqlDataSource ID="ActivityDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:authStore %>"
                SelectCommand="SELECT 'Recent activity' as Comment, [Message] FROM aspnet_WebEvent_Events WHERE EventTime = (select MAX(EventTime) FROM aspnet_WebEvent_Events WHERE EventType = 'BCC.Core.BCCWebAuditEvent' and EventCode = 100504)">
            </asp:SqlDataSource>

 	        <asp:GridView id="UserActivityGrid" Visible="false" runat="server" Width="100%" ForeColor="#333333"
                    CellPadding="4" CellSpacing="0" AutoGenerateColumns="False" DataSourceID="ActivityDataSource"
	                Gridlines="None" EmptyDataText="" ShowFooter="false" ShowHeader="false">
  	                <Columns>
  	                    <asp:BoundField DataField="Comment">
                              <ItemStyle Width="15%" Font-Italic="false" Wrap="true"/>
                        </asp:BoundField>
                        <asp:BoundField DataField="Message">
                              <ItemStyle Width="85%" Font-Italic="false" Wrap="true"/>
                        </asp:BoundField>
                    </Columns>
  	        </asp:GridView>
        </td>
        </tr>
        <!-- Grid View for activity log -->
        <tr>
             <td width="100%" valign="top">
                    <!-- Orchestrations List --> 
		    <asp:Panel id="orchestrationPanel" Visible="True" runat="server" defaultbutton="btnGACFilter">
                    <table border="1" bgcolor="#E9E9E9" bordercolor="#5D7B9D" cellpadding="1" cellspacing="0" width="100%" align="left">
                    <tr align="left" width="100%">
                        <td valign="top" colspan="2">
			<table border="0" bordercolor="#5D7B9D" width="100%" cellpadding="0" cellspacing="0" align="left">
			<tr align="left">
                        <td align="left" width="90%" valign="bottom">
                            <table border="0" width="100%" cellpadding="0" cellspacing="0" align="left">
                                <tr>
                                <td align="left" width="30%">
                                    <asp:TextBox id="txtGACSearchKey" width="98%" Text="Microsoft.BizTalk.Edi" runat="server" BorderStyle="Groove" Tooltip="Enter a search term" AutoCompleteType="Search" />
                                </td>
                                <td align="left" width="1%">
                                   <asp:LinkButton ID="btnGACFilter" OnClick="btnGACFilter_Click" runat="server" cssclass="linkConfig">
                                        <asp:Image ToolTip="Apply Filter" ID="searchImg" runat="server" ImageUrl="~/Images/search-icon.gif" />
                                    </asp:LinkButton>
                                </td>
                                <td align="left" width="60%">
                                    (search: 'Microsoft.BizTalk.Edi')
                                </td>
                                </tr>
                            </table>			                        		
                        </td>
			<td align="right" width="1%">
			     <asp:LinkButton ID="btnExportToExcel2" runat="server" style="color:teal;text-decoration:none;" OnClick="btnExportToExcel2_Click" >
				<asp:Image ID="excel2" runat="server" ImageUrl="~/Images/Excel-16.gif" />
			     </asp:LinkButton>
			</td>
			</tr>
			</table>
	        </td>   
              </tr>
		     <tr>
             <td width="45%" valign="top">
                    <!-- Orchestration Grid Panel -->
                    <table border="0" bordercolor="#5D7B9D" width="100%" cellpadding="0" cellspacing="0" align="left">
                    <tr>
                    <td>
                            <asp:GridView ID="gridOdx" runat="server" AutoGenerateColumns="False" CellPadding="4"
                                Width="100%" OnRowDataBound="gridOdx_RowDataBound">
                                <Columns>
                                    <asp:BoundField DataField="Name" HeaderText="Orchestration" >
                                        <ItemStyle Width="90%" Font-Italic="false" Wrap="true"/>
		                            </asp:BoundField>
                                    <asp:BoundField DataField="Version" HeaderText="Version">
                                            <ItemStyle Width="10%" Font-Italic="false" Wrap="false"/>
		                            </asp:BoundField>
                                </Columns>
                            </asp:GridView>
                    </td>
                    </tr>
                    </table>
                     <!-- Orchestration Grid Panel -->
            </td>
			<td width="55%" valign="top">
                    <table border="0" bordercolor="#5D7B9D" width="100%" cellpadding="0" cellspacing="0" align="left">
                    <tr>
                        <td>
                            <asp:GridView ID="gridGAC" runat="server" AllowSorting="True" EmptyDataText="No record(s) found."
                            OnSorting="OnSort" AutoGenerateColumns="False" CellPadding="4" OnRowDataBound="gridGAC_RowDataBound">
                            <Columns>
                                <asp:BoundField DataField="Flag" HeaderText="">
                                    <ItemStyle Width="3%" Font-Italic="false" Wrap="false"/>
		                        </asp:BoundField>
                                <asp:BoundField DataField="GACAssemblies" HeaderText="Global Assembly Cache (GAC)" SortExpression="GACAssemblies">
                                    <ItemStyle Width="67%" Font-Italic="false" Wrap="true"/>
		                        </asp:BoundField>
                                <asp:BoundField DataField="DateModified" HeaderText="Date modified" SortExpression="DateModified">
                                    <ItemStyle Width="25%" Font-Italic="false" Wrap="false"/>
		                        </asp:BoundField>
                                <asp:BoundField DataField="Version" HeaderText="Version" SortExpression="Version">
                                    <ItemStyle Width="5%" Font-Italic="false" Wrap="false"/>
		                        </asp:BoundField>
                            </Columns>
                            </asp:GridView>
                        </td>
                    </tr>
                    </table>
			</td>
            </tr>
            </table>
		</asp:Panel>
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
                    The right side panel displays the date on which an Assembly was installed in the GAC. Use this to determine the version of the assembly in the GAC. The DLLs which are installed today are shown as <img src="Images/tick-circle-frame-icon.png" />. <br /><br />
                    The left side panel lists down the various Orchestrations along with version numbers. <br /><br />
		            Specify the <b>search string</b> to narrow down the Assemblies that you are interested in and finally verify deployment.<br />
             <br />
            <asp:DataList ID="dlAppList" runat="server" skinID="sideList" >
                <HeaderTemplate>
                Configured BizTalk Applications
                </HeaderTemplate>
                <ItemTemplate>
                <%#Container.DataItem%>
                </ItemTemplate>
            </asp:DataList>
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
                        SelectCommand="SELECT [Message] FROM aspnet_WebEvent_Events WHERE EventType = 'BCC.Core.BCCWebAuditEvent' and EventCode = 100504 ORDER BY EventTime DESC">
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