<%@ Page Language="C#" MasterPageFile="~/template.master" AutoEventWireup="true" EnableViewState="true"
    CodeFile="SEL.aspx.cs" Inherits="EventLogs" Title="Event Log Page" %>

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
            <td width="100%" bgcolor="#E9E9E9">
                <table border="1" bgcolor="#E9E9E9" bordercolor="#C0C0C0" cellpadding="0" cellspacing="0" width="100%" align="left">
		        <tr>
		            <td align="left">
                        <asp:UpdatePanel ID="linkBtnPanel" runat="server">
		                <ContentTemplate>		            
	                            <asp:PlaceHolder ID="linkButtonGroup" runat="server" Visible="true" />
	                    </ContentTemplate>
	                    </asp:UpdatePanel>
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
                <asp:UpdateProgress ID="evtMain" runat="server" DynamicLayout="true" DisplayAfter="1" AssociatedUpdatePanelID="linkBtnPanel" > 
                 <ProgressTemplate> 
                    <div align="center">
                        <div style="background-color:#E9E9E9;padding:6px;border:solid 1px black;width:50%;text-align:center;">
                            <span><i><b>Searching...</b></i>
                            <asp:Image ID="img" runat="server" ImageUrl="~/Images/loading.gif" ImageAlign="AbsMiddle" />
                            </span>
                        </div>
                    </div>
                 </ProgressTemplate> 
               </asp:UpdateProgress>
            </td>
        </tr>
       
        <tr>
           <td>
                <asp:UpdateProgress ID="evtViewProgress" runat="server" DynamicLayout="true" DisplayAfter="1" AssociatedUpdatePanelID="evtLogPanel" > 
                 <ProgressTemplate> 
                    <div align="center">
                        <div style="background-color:#E9E9E9;padding:6px;border:solid 1px black;width:50%;text-align:center;">
                            <span><i><b>Searching...</b></i>
                            <asp:Image ID="img2" runat="server" ImageUrl="~/Images/loading.gif" ImageAlign="AbsMiddle" />
                            </span>
                        </div>
                    </div>
                 </ProgressTemplate> 
               </asp:UpdateProgress>           
               <asp:UpdatePanel ID="evtLogPanel" runat="server">
		       <ContentTemplate>
                    <table border="0" bgcolor="#E9E9E9" width="100%" cellpadding="0" cellspacing="0">
                <tr>
                   <td bgcolor="#E9E9E9" style="word-break:break-all;" align="center" width="100%" valign="top">
                    <asp:Panel ID="eventLogPanel" GroupingText="" runat="server" Visible="false">
                    <table border="0" bgcolor="#E9E9E9" width="100%" cellpadding="0" cellspacing="0">
                        <tr>
                            <td align="left">
                                <table border="0" bgcolor="#E9E9E9" bordercolor="#C0C0C0" cellpadding="0" cellspacing="0" width="100%" align="left">
                                <tr>
                                    <td align="left" width="12%">
                                    Filter expression:
                                    </td>
                                    <td align="left" width="30%">
                                        <asp:TextBox ID="filterExpr" runat="server" Width="97%" BorderStyle="Groove" ToolTip="Specify a filter expression" /> 
                                    </td>
                                    <td align="left" width="1%" valign="middle">
                                    <asp:Linkbutton ID="btnFilter" OnClick="btnFilter_Click" runat="server" CssClass="linkConfig">
                                        <asp:Image ToolTip="Apply Filter" ImageAlign="Middle" ID="searchImg" runat="server" ImageUrl="~/Images/search-icon.gif" />
                                    </asp:Linkbutton>
                                    </td>
                                    <td align="left" width="50%" valign="middle">                                    
                                    Use Filter expression: Source LIKE '%BizTalk%' AND EntryType LIKE '%Error%'
                                    <asp:HiddenField ID="category" EnableViewState="true" runat="server" Visible="false" />
                                    <asp:HiddenField ID="rowFilter" EnableViewState="true" runat="server" Visible="false" />
                                    </td>                                    
                                </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:GridView ID="LogGrid" runat="server" PageSize="25" AutoGenerateColumns="False"
                                    CellPadding="4" Width="100%" 
                                     OnPageIndexChanging="LogGrid_PageIndexChanging"
                                     OnRowDataBound="LogGrid_RowDataBound" 
                                     AllowPaging="True" AllowSorting="True" 
                                      EmptyDataText="No records found. If you have specified a row filter, modify it and try again." 
		                             ForeColor="#333333" GridLines="Both">
                                    <Columns>
                                        <asp:BoundField DataField="EventID" HeaderText="Event ID">
                                                <ItemStyle HorizontalAlign="Left" Width="6%" Font-Italic="false" Wrap="false"/>
                                        </asp:BoundField>                            
                                        <asp:BoundField DataField="EntryType" HeaderText="Entry">
                                            <ItemStyle HorizontalAlign="Left" Width="7%" Font-Italic="false" Wrap="false"/>
                                        </asp:BoundField>
                                        <asp:BoundField DataField="Source" HeaderText="Source" >
                                            <ItemStyle HorizontalAlign="Left" Width="8%" Font-Italic="false" Wrap="true"/>
                                        </asp:BoundField>
                                        <asp:BoundField DataField="DateTime" HeaderText="Time" DataFormatString="{0:MMM-dd-yyyy hh:mm:ss tt}" >
                                            <ItemStyle HorizontalAlign="Left" Width="7%" Font-Italic="false" Wrap="true"/>
                                        </asp:BoundField>
                                        <asp:BoundField DataField="Message" HeaderText="Message">
                                            <ItemStyle HorizontalAlign="Left" Width="63%" Font-Italic="false" Wrap="true"/>
                                        </asp:BoundField>
                                    </Columns>
                                </asp:GridView>                            
                            </td>
                        </tr>                        
                    </table>
                    </asp:Panel>
                    </td>
                </tr>
                </table>
               </ContentTemplate>
               </asp:UpdatePanel>
            </td>
        </tr>
	<tr>
	    <td>
	        <asp:Label ID="lblStatus" runat="server" Text="Label"></asp:Label>
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
                    Use this module to search event log. You can add additional logs to search by modifying the profile settings.
                    <br />
                    <br />
                    <b>About Row filter expressions</b><br />
                    For Event id use expression EventID.<br />
                    For Entry type use expression EntryType.<br />
                    For Date time use expression DateTime.<br /><br />
                    <i style="background-color:Transparent"> DateTime > '1/27/2011 6:30:00 PM' and DateTime < '1/27/2011 8:30:00 PM' </i><br /><br />
                    For Message use expression Message.<br />
                    For Event id use expression EventID.<br />
                    <br />
                    <b>Event(s) logged today</b><br />
                        <span style="background-color:#CECECE;color:#008080"><%=System.DateTime.Now %></span> indicates todays events.
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
            <td>
                <!-- Inner Table -->
                <table align="left" border="1" width="100%" cellpadding="0" cellspacing="0" bgcolor="#FFFFCC" style="color:#333333;border-collapse:collapse;">
                <tr>
                    <td align="center" style="color:#FDD017;background-color:#525252;font-weight:bold;">Event Log Monitoring</td>
                </tr>
                <tr>
                    <td align="left">
                             <asp:CheckBoxList ID="eventLogList" CellPadding="5" CellSpacing="5" RepeatDirection="Vertical" RepeatLayout="Flow" TextAlign="Right" Runat="server" />
                    </td>
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
       <tr>
           <td style="word-break:break-all;">
	            <!-- Grid View for activity log -->
                    <asp:SqlDataSource ID="ActivityDataSource2" runat="server" ConnectionString="<%$ ConnectionStrings:authStore %>"
                        SelectCommand="SELECT [Message] FROM aspnet_WebEvent_Events WHERE EventType = 'BCC.Core.BCCWebAuditEvent' and EventCode = 100401 ORDER BY EventTime DESC">
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
