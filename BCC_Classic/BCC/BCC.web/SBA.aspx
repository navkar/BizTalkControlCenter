<%@ Page Language="C#" MasterPageFile="~/template.master" AutoEventWireup="true" CodeFile="SBA.aspx.cs" Inherits="BTSearch" EnableViewState="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="leftContentPlaceHolder" runat="Server">

    <table border="0" bgcolor="#E9E9E9" bordercolor="#5D7B9D" width="100%" cellpadding="0" cellspacing="0" align="left">
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
                        SelectCommand="SELECT 'Recent activity' as Comment, [Message] FROM aspnet_WebEvent_Events WHERE EventTime = (select MAX(EventTime) FROM aspnet_WebEvent_Events WHERE EventType = 'BCC.Core.BCCWebAuditEvent' and EventCode = 100402)">
                    </asp:SqlDataSource>

 	                <asp:GridView id="UserActivityGrid" Visible="false" runat="server" Width="100%" ForeColor="#333333"
                            CellPadding="4" CellSpacing="0" AutoGenerateColumns="False" DataSourceID="ActivityDataSource"
	                        Gridlines="None" EmptyDataText="" ShowFooter="false" ShowHeader="false">
  	                        <Columns>
  	                            <asp:BoundField DataField="Comment">
                                      <ItemStyle Width="10%" Font-Italic="false" Wrap="true" />
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
               <asp:Panel ID="tablePanel" runat="server" defaultbutton="btnFilter">
                    <table border="1" bgcolor="#E9E9E9" bordercolor="#C0C0C0" cellpadding="0" cellspacing="0" width="100%" align="left">
                        <tr align="left">
                            <td align="left" valign="middle" width="10%">
                                <asp:TextBox id="txtSearchKey" runat="server" Width="98%" BorderStyle="Groove" Tooltip="Enter a search term" />
                            </td>
                            <td align="left" valign="middle" width="60%">
                                <asp:LinkButton ID="btnFilter" runat="server" CssClass="linkConfig" OnClick="btnFilter_Click">
                                        <asp:Image ImageAlign="Middle" ID="searchImg" runat="server" ImageUrl="~/Images/search-icon.gif" />
                                </asp:LinkButton>
                                (search history : <%=searchTerms%> ...)&nbsp;[<%=count%>]                            
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
		        <table border="1" bordercolor="#E9E9E9" width="100%" cellpadding="0" cellspacing="0" align="center">
                        <tr colspan="2">
                        <td style="word-break:break-all;">
		                <asp:GridView ID="gvSearch" runat="server" AutoGenerateColumns="False" CellPadding="4" AllowPaging="true"
                            GridLines="Both" Width="100%" PageSize="15" EmptyDataText="No record(s) found."
                            OnPageIndexChanging="gvSearch_PageIndexChanging"
                            OnRowDataBound="gvSearch_RowDataBound">
                            <Columns>
			                    <asp:BoundField DataField="Name" HeaderText="BizTalk artifact Name">
			                        <ItemStyle Width="30%" Font-Italic="false" Wrap="true"/>
		                        </asp:BoundField>
                                <asp:BoundField DataField="Status" HeaderText="Status">
  			                        <ItemStyle Width="10%" Font-Italic="false" Wrap="true"/>
		                        </asp:BoundField>
                                <asp:BoundField DataField="Data" HeaderText="Additional information">
                                     <ItemStyle Width="60%" Font-Italic="false" Wrap="true"/>
		                        </asp:BoundField>
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
    <table border="0" bordercolor="#C0C0C0" width="100%" cellpadding="0" cellspacing="2" align="center">
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
                    <b>Search and you shall seek</b><br />
                    Use this module to search for various BizTalk artifacts, for e.g. Orchestrations, Send ports, Receive locations etc.
                    <br /><br />
                    The search history can be used to search various artifacts quickly and effectively.
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
                        SelectCommand="SELECT [Message] FROM aspnet_WebEvent_Events WHERE EventType = 'BCC.Core.BCCWebAuditEvent' and EventCode = 100402 ORDER BY EventTime DESC">
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