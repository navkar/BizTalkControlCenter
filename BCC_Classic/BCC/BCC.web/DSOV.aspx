<%@ Page Language="C#" MasterPageFile="~/template.master" AutoEventWireup="true"
    CodeFile="DSOV.aspx.cs" Inherits="OrchDox" EnableViewState="false" EnableEventValidation="False" %>
    
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
                SelectCommand="SELECT 'Recent activity' as Comment, [Message] FROM aspnet_WebEvent_Events WHERE EventTime = (select MAX(EventTime) FROM aspnet_WebEvent_Events WHERE EventType = 'BCC.Core.BCCWebAuditEvent' and EventCode = 100701)">
            </asp:SqlDataSource>

 	        <asp:GridView id="UserActivityGrid" Visible="false" runat="server" Width="100%" ForeColor="#333333"
                    CellPadding="4" CellSpacing="0" AutoGenerateColumns="False" DataSourceID="ActivityDataSource"
	                Gridlines="None" EmptyDataText="" ShowFooter="false" ShowHeader="false">
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
        <!-- Grid View for activity log -->
        <tr>
            <td>
              <asp:Panel id="masterPanel" Visible="True" runat="server">
		            <table border="1" bordercolor="#C0C0C0" width="100%" cellpadding="0" cellspacing="0" align="center">
                            <tr colspan=2>
                                <td style="word-break:break-all;">
		                            <asp:GridView ID="gridOdx" runat="server" AutoGenerateColumns="False" AllowSorting="True" 
		                            PageSize="15" AllowPaging="true"
		                          OnSorting="OnSort" CellPadding="4"
                                  GridLines="Both" Width="100%" 
                                  OnPageIndexChanging="gridOdx_PageIndexChanging"
                                  OnRowDataBound="gridOdx_RowDataBound">
                               <Columns>
                                    <asp:BoundField DataField="Name" HeaderText="BizTalk Orchestration" SortExpression="Name">
                                          <ItemStyle Width="33%" Font-Italic="false" Wrap="true"/>
                                    </asp:BoundField>
                                    <asp:BoundField DataField="AssemblyName" HeaderText="Assembly" SortExpression="AssemblyName">
                                          <ItemStyle Width="19%" Font-Italic="false" Wrap="true"/>
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Version" HeaderText="V" SortExpression="Version">
                                          <ItemStyle Width="4%" Font-Italic="false" Wrap="false"/>
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Application" HeaderText="Application" SortExpression="Application">
                                          <ItemStyle Width="11%" Font-Italic="false" Wrap="true"/>
                                    </asp:BoundField>
                                    <asp:TemplateField HeaderText="Select" ShowHeader="False" HeaderStyle-HorizontalAlign="Left"> 
                                        <ItemTemplate> 
                                            <asp:LinkButton ID="lnkODox" runat="server" CausesValidation="False" CommandName="ViewDox" >
                                                    <asp:Image ID="Image1" runat="server" ImageUrl="Images/docview.png" ToolTip="View Orchestration documentation" />
                                            </asp:LinkButton>  
                                        </ItemTemplate> 
                                        <ItemStyle Width="4%" Font-Italic="false" HorizontalAlign="Center" Wrap="false"/>                                        
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                                </td>
                            </tr>
		            </table>
              </asp:Panel>
              
              <asp:Panel id="emptyPanel" Visible="False" Runat="server">
                  <br />
			        <!--- Shady code goes here -->
                    <div class="shiftcontainer" align="center">
                    <div class="shadowcontainer">
                    <div class="innerdiv" align="left">
                            <b>No orchestration(s) were found.</b><br /><br />
                                The BizTalk Applications specified in the user profile does contain any Orchestrations(s). <br /><br />
                                Go to 'Administration > System Settings' or use speedcode <b>603</b>.<br /><br />
		                    <br />
		            </div>
		            </div>
		            </div>
		            <!--- Shady code goes here -->
		          <br />
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
                    <b>Orchestration view & Source code</b><br />
                    Select an orchestration to view its flow and its associated source code.<br />
            <br />  
            <b>Orchestration code reviews</b><br />                               
            This module can be used for code reviews.<br /><br />

            <asp:DataList ID="dlOAppList" runat="server" skinID="sideList" >
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
                        SelectCommand="SELECT [Message] FROM aspnet_WebEvent_Events WHERE EventType = 'BCC.Core.BCCWebAuditEvent' and EventCode = 100701 ORDER BY EventTime DESC">
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