<%@ Page Language="C#" MasterPageFile="~/template.master" AutoEventWireup="true" CodeFile="PVPC.aspx.cs" Inherits="PerfCounters" %>


<asp:Content ID="Content1" ContentPlaceHolderID="leftContentPlaceHolder" Runat="Server">
    <table border="0" bgcolor="#E9E9E9" width="99%" cellpadding="0" cellspacing="0" align="left">
        <tr>
            <td align="center">
		        <asp:Label ID="lblCaption" skinID="PageCaption" runat="server"></asp:Label>
   	        </td>
        </tr>
        <tr>
                <td>
                    <asp:UpdatePanel ID="updateResponse" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>  
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
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="btnFilter" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="btnClearCache" EventName="Click" />
                    </Triggers>
                    </asp:UpdatePanel>                     
                </td>
        </tr>
       <tr>
       <td>
	        <!-- Grid View for activity log -->
                <asp:SqlDataSource ID="ActivityDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:authStore %>"
                    SelectCommand="SELECT 'Recent activity' as Comment, [Message] FROM aspnet_WebEvent_Events WHERE EventTime = (select MAX(EventTime) FROM aspnet_WebEvent_Events WHERE EventType = 'BCC.Core.BCCWebAuditEvent' and EventCode = 100303)">
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
            <td align="left">
                <asp:UpdatePanel ID="upPerfGrid" runat="server" UpdateMode="Conditional">
                <ContentTemplate> 
                <table border="1" bgcolor="#E9E9E9" bordercolor="#C0C0C0" cellpadding="0" cellspacing="0" width="100%" align="left">
                <tr>
                    <td align="left" width="12%">
                    Category Name :
                    </td>
                    <td align="left" width="30%">
                        <asp:TextBox ID="filterExpr" runat="server" Width="97%" BorderStyle="Groove" ToolTip="Specify Category Name" /> 
                    </td>
                    <td align="left" width="1%" valign="middle">
                    <asp:Linkbutton ID="btnFilter" OnClick="btnFilter_Click" runat="server" CssClass="linkConfig">
                        <asp:Image ToolTip="Apply Filter" ImageAlign="Middle" ID="searchImg" runat="server" ImageUrl="~/Images/search-icon.gif" />
                    </asp:Linkbutton>
                    </td>
                    <td align="left" width="50%" valign="middle">    
                        (For e.g. 'BizTalk' or 'Send Adapter')
                        <asp:Linkbutton ID="btnClearCache" OnClientClick="return confirm('Are you sure want to clear the Performance counter cache?');" OnClick="btnCache_Click" runat="server" CssClass="linkConfig">Clear cache</asp:Linkbutton>
                    </td>                                    
                </tr>
                </table>
                </ContentTemplate>
                </asp:UpdatePanel>  
                <asp:UpdateProgress ID="upPanel" runat="server" DynamicLayout="true"> 
                 <ProgressTemplate> 
                    <div align="center">
                        <div style="background-color:#E9E9E9;padding:10px;border:solid 1px black;width:50%;text-align:center;">
                            <span><i><b>Loading performance counters...</b></i>
                                <asp:Image ID="img" runat="server" ImageUrl="~/Images/loading.gif" ImageAlign="AbsMiddle" />
                            </span>
                        </div>
                    </div>
                 </ProgressTemplate> 
                </asp:UpdateProgress>                 
                <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                <ContentTemplate>                
		        <table border="0" bordercolor="#B2B2B2" width="100%" cellpadding="0" cellspacing="0" align="center">
                        <tr><td style="word-break:break-all;">
                                        
                                    <asp:GridView ID="gridPerfCounters" runat="server" AutoGenerateEditButton="False"
                                        AutoGenerateColumns="False" CellPadding="4" Visible="true" 
                                        AllowSorting="true" OnSorting="gridPerfCounters_OnSorting" 
                                        AllowPaging="true" PageSize="15"
                                        EmptyDataText="Specify a category and look for performance counters. Use speedcode '104' to start performance counters."
                                        GridLines="Both" width="100%"
                                        OnRowDataBound="gridPerfCounters_RowDataBound" 
                                        OnRowCommand="gridPerfCounters_RowCommand"
                                        OnPageIndexChanging="gridPerfCounters_PageIndexChanging">
                                        <Columns>
                                            <asp:BoundField DataField="PerfCategoryName" HeaderText="Category Name" ReadOnly="true" SortExpression="PerfCategoryName">
                                                <ItemStyle Width="20%" Font-Italic="false" Wrap="true"/>
                                            </asp:BoundField>
                                            <asp:BoundField DataField="PerfCategoryDesc" HeaderText="Description" ReadOnly="true" SortExpression="PerfCategoryDesc">
                                                <ItemStyle Width="30%" Font-Italic="false" Wrap="true"/>
                                            </asp:BoundField>                                            
                                            <asp:TemplateField HeaderText="Instance Name" HeaderStyle-HorizontalAlign="Center" SortExpression="PerfInstanceName">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblInstanceName" style="font-weight:normal;display:block;width:100%;text-decoration:none" runat="server" Text='<%# Bind("PerfInstanceName") %>'></asp:Label>
                                                </ItemTemplate>
                                                <ItemStyle Width="18%" HorizontalAlign="Center" Font-Italic="false" Wrap="true"/>
                                            </asp:TemplateField>                                                                                                                                                                                                   
                                            <asp:BoundField DataField="PerfCounterName" HeaderText="Counter Name" ReadOnly="true" SortExpression="PerfCounterName">
                                                <ItemStyle Width="18%" Font-Italic="false" Wrap="false"/>
                                            </asp:BoundField>
                                            <asp:TemplateField HeaderText="Select" ShowHeader="False" HeaderStyle-HorizontalAlign="Left"> 
                                                <ItemTemplate> 
                                                    <asp:LinkButton ID="lnkMonitor" runat="server" CausesValidation="False" CommandName="Monitor" Text="Monitor" CssClass="linkConfig"></asp:LinkButton> 
                                                </ItemTemplate> 
                                                <ItemStyle Width="7%" Font-Italic="false" Wrap="false"/>                                        
                                            </asp:TemplateField>                                     
                                        </Columns>
                                    </asp:GridView>
                        </td></tr>
		        </table>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="btnFilter" EventName="Click" />
                    <asp:AsyncPostBackTrigger ControlID="btnClearCache" EventName="Click" />
                </Triggers>                
                </asp:UpdatePanel>   		        
            </td>
        </tr>
        <tr>
             <td>
                &nbsp;
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
                    <b>Performance Counters</b><br />
                    Search for a specific category of performance counter and monitor.
                    Performance counter shall be in effect after <b>60 secs </b> by the BCC Agent service.
                    <br /><br />
                    <b>Performance Counter - Instance Name</b><br />
                    You must choose an instance name, in order for the performance counter to get data. If the instance is <b>unavailable</b>, this means 
                    that the performance counter instance has not been started.
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
                    SelectCommand="SELECT [Message] FROM aspnet_WebEvent_Events WHERE EventType = 'BCC.Core.BCCWebAuditEvent' and EventCode = 100303 ORDER BY EventTime DESC">
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


