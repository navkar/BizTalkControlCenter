<%@ Page Language="C#" MasterPageFile="~/template.master" AutoEventWireup="true"
    CodeFile="PVMA.aspx.cs" Inherits="MessageAudit"
	EnableEventValidation="False" EnableViewState="true" %>
	
<%@ Register TagPrefix="uc" TagName="Search" Src="~/Controls/SearchControl.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="leftContentPlaceHolder" runat="Server">
   <table border="0" bordercolor="#5D7B9D" width="100%" cellpadding="0" cellspacing="0" align="center">
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
                SelectCommand="SELECT 'Recent activity' as Comment, [Message] FROM aspnet_WebEvent_Events WHERE EventTime = (select MAX(EventTime) FROM aspnet_WebEvent_Events WHERE EventType = 'BCC.Core.BCCWebAuditEvent' and EventCode = 100304)">
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
                                <td align="left" valign="middle" colspan="2">
                                    <uc:Search ID="search" runat="Server" /> 
                                </td>
                            </tr>
                        </table>
		        </asp:Panel>
            </td>
        </tr>
        <!-- Grid View for activity log -->
         <tr>
            <td>
		        <table border="1" bordercolor="#B2B2B2" width="100%" cellpadding="0" cellspacing="0" align="center">
                <tr><td style="word-break:break-all;">
                            <asp:GridView ID="gridBCCAudit" runat="server" AllowPaging="true" AllowSorting="true" AutoGenerateColumns="False" CellPadding="4" 
                                EmptyDataText="There were no audit record(s) found, to start a search."
                                ForeColor="#333333" GridLines="Both" width="100%" PageSize="50" 
                                OnSorting="gridBCCAudit_OnSort"
                                OnPageIndexChanging="gridBCCAudit_PageIndexChanging"
                                OnRowDataBound="gridBCCAudit_RowDataBound">
                                <Columns>
                                    <asp:TemplateField HeaderText="Session" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" ControlStyle-Font-Bold="false" SortExpression="SessionID">
                                        <ItemTemplate>
                                                <asp:Label ID="lblSessionID" CausesValidation="false" style="display:block;width:70px;text-decoration:none" runat="server" Text='<%# Bind("SessionID") %>' />
                                        </ItemTemplate>
                                        <ItemStyle Width="10%" Font-Italic="false" Wrap="true" />
                                    </asp:TemplateField>  

                                    <asp:TemplateField HeaderText="Message" ItemStyle-BackColor="BlanchedAlmond" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" ControlStyle-Font-Bold="false" SortExpression="MessageID">
                                        <ItemTemplate>
                                                <asp:HiddenField ID="auditMessage" runat="server" Value='' />
                                                <asp:LinkButton ID="lblMessageID" CausesValidation="false" style="font-weight:bold;display:block;width:70px;text-decoration:none" runat="server" Text='<%# Bind("MessageID") %>' />
                                        </ItemTemplate>
                                        <ItemStyle Width="10%" Font-Italic="false" Wrap="true" />
                                    </asp:TemplateField>  

                                    <asp:TemplateField HeaderText="Parent" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" ControlStyle-Font-Bold="false" SortExpression="parentService">
                                        <ItemTemplate>
                                                <asp:Label ID="lblParentService" CausesValidation="false" style="display:block;width:50px;text-decoration:none" runat="server" Text='<%# Bind("parentService") %>' />
                                        </ItemTemplate>
                                        <ItemStyle Width="9%" Font-Italic="false" Wrap="true" />
                                    </asp:TemplateField>                                    

                                    <asp:TemplateField HeaderText="Child" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" ControlStyle-Font-Bold="false" SortExpression="childService">
                                        <ItemTemplate>
                                                <asp:Label ID="lblChildService" CausesValidation="false" style="display:block;width:50px;text-decoration:none" runat="server" Text='<%# Bind("childService") %>' />
                                        </ItemTemplate>
                                        <ItemStyle Width="9%" Font-Italic="false" Wrap="true" />
                                    </asp:TemplateField>                                    

                                    <asp:TemplateField HeaderText="Initiator" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" ControlStyle-Font-Bold="false" SortExpression="initiator">
                                        <ItemTemplate>
                                                <asp:Label ID="lblInitiator" CausesValidation="false" style="display:block;width:75px;text-decoration:none" runat="server" Text='<%# Bind("initiator") %>' />
                                        </ItemTemplate>
                                        <ItemStyle Width="10%" Wrap="false" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Source" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" ControlStyle-Font-Bold="false" SortExpression="sourceSystem">
                                        <ItemTemplate>
                                                <asp:Label ID="lblSource" CausesValidation="false" style="display:block;width:75px;text-decoration:none" runat="server" Text='<%# Bind("sourceSystem") %>' />
                                        </ItemTemplate>
                                        <ItemStyle Width="10%" Wrap="false" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Target" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" ControlStyle-Font-Bold="false" SortExpression="targetSystem">
                                        <ItemTemplate>
                                                <asp:Label ID="lblTarget" CausesValidation="false" style="display:block;width:75px;text-decoration:none" runat="server" Text='<%# Bind("targetSystem") %>' />
                                        </ItemTemplate>
                                        <ItemStyle Width="10%" Wrap="false" />
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Step" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" ControlStyle-Font-Bold="false" SortExpression="processStep">
                                        <ItemTemplate>
                                                <asp:Label ID="lblProcessStep" CausesValidation="false" style="display:block;width:35px;text-decoration:none" runat="server" Text='<%# Bind("processStep") %>' />
                                        </ItemTemplate>
                                        <ItemStyle Width="3%" Font-Italic="false" Wrap="false" />
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Event" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" ControlStyle-Font-Bold="false" SortExpression="eventType">
                                        <ItemTemplate>
                                                <asp:Label ID="lblEventType" CausesValidation="false" style="font-weight:bold;display:block;width:75px;text-decoration:none" runat="server" Text='<%# Bind("EventType") %>' />
                                        </ItemTemplate>
                                        <ItemStyle Width="7%" Font-Italic="false" Wrap="true" />
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Workflow" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" ControlStyle-Font-Bold="false" SortExpression="workflowStatus">
                                        <ItemTemplate>
                                                <asp:Label ID="lblWorkflowStatus" CausesValidation="false" style="font-weight:bold;display:block;width:90px;text-decoration:none" runat="server" Text='<%# Bind("workflowStatus") %>' />
                                        </ItemTemplate>
                                        <ItemStyle Width="7%" Font-Italic="false" Wrap="false" />
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Logged" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" ControlStyle-Font-Bold="false" SortExpression="createdOn">
                                        <ItemTemplate>
                                                <asp:Label ID="lblCreatedOn" CausesValidation="false" style="font-weight:bold;display:block;width:86px;text-decoration:none" runat="server" Text='<%# Bind("createdOn") %>' />
                                        </ItemTemplate>
                                        <ItemStyle Width="7%" Font-Italic="false" Wrap="false" />
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Host" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" ControlStyle-Font-Bold="false" SortExpression="HostName">
                                        <ItemTemplate>
                                                <asp:Label ID="lblHostName" CausesValidation="false" style="display:block;width:60px;text-decoration:none" runat="server" Text='<%# Bind("HostName") %>' />
                                        </ItemTemplate>
                                        <ItemStyle Width="9%" Font-Italic="false" Wrap="true" />
                                    </asp:TemplateField>  
                                </Columns>
                            </asp:GridView>
                </td></tr>
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
             <b>BCC Auditing Framework</b> <br />
	            This module displays all the records from the BCC Audit table (bcc_MessageAudit).<br />
                <br />
             <b>Notes</b><br /><br />
             <b>Host</b> - The machine name.<br />
             <b>Session</b> - The transaction session ID.<br />
             <b>Message</b> - The unique message ID.<br />
             <b>Parent</b> - The parent process. <br />
             <b>Child</b> - The child process (can be same as parent).<br />
             <b>Initiator</b> - The initiator of the message. <br />
             <b>Source</b> - The message source. <br />
             <b>Target</b> - The message target. <br />
             <b>Step</b> - Message step.<br />
             <b>Event</b> - Message event.<br />
             <b>Workflow</b> - The outcome of the message.<br />             
             <b>Logged</b> - The date-time the entry was logged.<br />
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
                        SelectCommand="SELECT [Message] FROM aspnet_WebEvent_Events WHERE EventType = 'BCC.Core.BCCWebAuditEvent' and EventCode = 100304 ORDER BY EventTime DESC">
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