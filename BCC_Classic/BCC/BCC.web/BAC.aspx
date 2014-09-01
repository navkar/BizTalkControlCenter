<%@ Page Language="C#" MasterPageFile="~/template.master" AutoEventWireup="true"
    ValidateRequest="false" CodeFile="BAC.aspx.cs" Inherits="ConfigFile" %>

<asp:Content ID="Content1" ContentPlaceHolderID="leftContentPlaceHolder" runat="Server">
      <table border="0" width="99%" cellpadding="0" cellspacing="0" align="left">
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
            <td colspan="2">
            <asp:SqlDataSource ID="ActivityDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:authStore %>"
                SelectCommand="SELECT 'Recent activity' as Comment, [Message] FROM aspnet_WebEvent_Events WHERE EventTime = (select MAX(EventTime) FROM aspnet_WebEvent_Events WHERE EventType = 'BCC.Core.BCCWebAuditEvent' and EventCode = 100210)">
            </asp:SqlDataSource>

 	        <asp:GridView id="UserActivityGrid" Visible="false" runat="server" Width="100%" ForeColor="#333333"
                    CellPadding="4" CellSpacing="0" AutoGenerateColumns="False" DataSourceID="ActivityDataSource"
	                Gridlines="None" EmptyDataText="" ShowFooter="false" ShowHeader="false">
  	                <Columns>
  	                    <asp:BoundField DataField="Comment">
                              <ItemStyle Width="10%" Font-Italic="false" Wrap="true"/>
                        </asp:BoundField>
                        <asp:BoundField DataField="Message">
                              <ItemStyle Width="92%" Font-Italic="false" Wrap="true"/>
                        </asp:BoundField>
                    </Columns>
  	        </asp:GridView>
        </td>
        </tr>
        <tr>
            <td width="100%" bgcolor="#E9E9E9">
                <table border="1" bgcolor="#E9E9E9" bordercolor="#C0C0C0" cellpadding="0" cellspacing="0" width="100%" align="left">
		        <tr>
		            <td align="left">
	                  <asp:PlaceHolder ID="linkButtonGroup" runat="server" Visible="true" />
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
            <td width="100%" valign="top">
            <asp:Panel ID="configFileListPanel" GroupingText="" Visible="false" runat="server" >
	          <table border="0" width="100%" bordercolor="#C0C0C0" cellpadding="0" cellspacing="0" align="left">
              <tr><td>
                <asp:GridView width="100%" ID="configGrid" runat="server" AutoGenerateColumns="False" 
                    CellPadding="4" ForeColor="#333333" GridLines="Both" EmptyDataText="Select an item from the dropdown for the list of files." 
                    PageSize="25"
                    AllowPaging="True" AllowSorting="True"
                    OnRowDataBound="configGrid_RowDataBound" 
		            OnPageIndexChanging="configGrid_PageIndexChanging"
                    OnSorting="configGrid_Sorting"
                    >
                    <PagerSettings Position="TopAndBottom" />
                    <Columns>
                        <asp:BoundField DataField="DirectoryName" HeaderText="Directory" SortExpression="DirectoryName">
                            <ItemStyle Wrap="false" Width="15%" />
                        </asp:BoundField>
                        <asp:BoundField DataField="Name" HeaderText="File" SortExpression="Name">
                            <ItemStyle Wrap="true" Width="35%" />
                        </asp:BoundField>                        
                        <asp:TemplateField HeaderText="">
                            <ItemTemplate>
                                <asp:HyperLink ID="hlnk" runat="server" NavigateUrl='<%# Eval("FullName", "BACD.aspx?name={0}") %>'
                                    Target="_blank" style="color:teal;text-decoration:none;" Text="">
                                    <asp:Image ID="imgDownload" runat="server" ToolTip="View file" ImageAlign="Middle" ImageUrl="~/Images/Downloads-icon.png" Visible="true" />
                                </asp:HyperLink>                                
                            </ItemTemplate>
                             <ItemStyle Wrap="false" Width="2%" />   
                        </asp:TemplateField>                                                 
                        <asp:TemplateField HeaderText="Created On" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Center" ControlStyle-Font-Bold="false" SortExpression="CreationTime">
                            <ItemTemplate>
                                    <asp:Label ID="lblCreatedOn" style="font-weight:normal;display:block;width:170px;" runat="server" Text='<%# Bind("CreationTime", "{0:MMM-dd-yy hh:mm:ss tt}") %>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Wrap="false" Width="12%" />
                        </asp:TemplateField>                                               
                        <asp:TemplateField HeaderText="Modified On" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Center" ControlStyle-Font-Bold="false" SortExpression="DateModified">
                            <ItemTemplate>
                                    <asp:Label ID="lblModifiedOn" style="font-weight:normal;display:block;width:170px;" runat="server" Text='<%# Bind("DateModified", "{0:MMM-dd-yy hh:mm:ss tt}") %>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Wrap="false" Width="12%" />
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
              </td></tr>
	          </table>
	        </asp:Panel>
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
                    Use this module to view the various BizTalk configuration files. For e.g. BTSNTSVC.exe.config.
                    <br /><br />
                    <asp:DataList ID="dlFileDirList" runat="server" skinID="sideList" >
                        <HeaderTemplate>
                        Configured directories
                        </HeaderTemplate>
                        <ItemTemplate>
                        <%#Container.DataItem%>
                        </ItemTemplate>
                    </asp:DataList>
                    <br />     
                    <b>Adding additional directories</b><br />
                    Use speedcode <b>603</b>, if you want to add additional directories.  
                    <br /><br />          
                    <b>File(s) modified today</b><br />
                        <asp:Label ID="lblCaptionToday" style="font-weight:normal;display:block;width:170px;" runat="server"></asp:Label> indicates file(s) modified today.
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
                        SelectCommand="SELECT [Message] FROM aspnet_WebEvent_Events WHERE EventType = 'BCC.Core.BCCWebAuditEvent' and EventCode = 100210 ORDER BY EventTime DESC">
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