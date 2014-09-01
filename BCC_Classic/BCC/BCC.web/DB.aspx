<%@ Page Language="C#" MasterPageFile="~/template.master" AutoEventWireup="true"
    ValidateRequest="false" CodeFile="DB.aspx.cs" Inherits="DeploymentBindings" Title="Control Center Bindings" %>

<asp:Content ID="Content1" ContentPlaceHolderID="leftContentPlaceHolder" runat="Server">
        <table border="0" width="100%" cellpadding="0" cellspacing="0" align="left">
        <tr>
            <td align="center">
		        <asp:Label ID="lblCaption" skinID="PageCaption" runat="server"></asp:Label>
   	        </td>
        </tr>
       <tr>
             <td width="100%" align="left" >
                    <asp:UpdatePanel ID="upError" runat="server">
		            <ContentTemplate>
                        <table border="0" width="100%" bgcolor="#CECECE" cellpadding="0" cellspacing="0" align="left">
                        <tr>
                            <td width="2%" valign="top">
                                <asp:Image ID="errorImg" runat="server" ImageUrl="~/Images/error.png" Visible="false" />
                                <asp:Image ID="okImg" runat="server" Height="" Width="" ImageUrl="~/Images/ok.png" Visible="false" />
		                    </td>
		                    <td width="95%">
		                        <asp:Label ID="lblError" runat="server" Visible="false" Width="100%"></asp:Label>
		                    </td>
		                </tr>
                        </table>
                    </ContentTemplate>
                    <Triggers>
                      <asp:AsyncPostBackTrigger ControlID="btnImportBinding" EventName="Click" />
                    </Triggers>
                    </asp:UpdatePanel>  
	        </td>
        </tr> 
        <!-- Grid View for activity log -->
        <tr>
            <td >
            <asp:SqlDataSource ID="ActivityDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:authStore %>"
                SelectCommand="SELECT 'Recent activity' as Comment, [Message] FROM aspnet_WebEvent_Events WHERE EventTime = (select MAX(EventTime) FROM aspnet_WebEvent_Events WHERE EventType = 'BCC.Core.BCCWebAuditEvent' and EventCode = 100501)">
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
        <!-- Grid View for activity log ends -->
        
        <tr>
            <td width="100%" bgcolor="#E9E9E9">
                <table border="1" bgcolor="#E9E9E9" bordercolor="#C0C0C0" cellpadding="0" cellspacing="0" width="100%" align="left">
		        <tr>
		            <td align="left">
	                  <asp:PlaceHolder ID="linkButtonGroup" runat="server" Visible="true" />
		            </td>
		        </tr>
		        </table>
            </td>
        </tr>
        
        <tr>
        <td>
              <asp:UpdateProgress ID="upBindings" runat="server" DynamicLayout="true" DisplayAfter="0" AssociatedUpdatePanelID="linkPanel" > 
                 <ProgressTemplate> 
                    <div align="center">
                        <div style="background-color:#E9E9E9;padding:6px;border:solid 1px black;width:50%;text-align:center;">
                            <span><i><b>Processing please wait...</b></i>
                            <asp:Image ID="img" runat="server" ImageUrl="~/Images/loading.gif" ImageAlign="AbsMiddle" />
                            </span>
                        </div>
                    </div>
                 </ProgressTemplate> 
               </asp:UpdateProgress>
               
               <asp:UpdateProgress ID="upBindings2" runat="server" DynamicLayout="true" DisplayAfter="0" AssociatedUpdatePanelID="linkPanel2" > 
                 <ProgressTemplate> 
                    <div align="center">
                        <div style="background-color:#E9E9E9;padding:6px;border:solid 1px black;width:50%;text-align:center;">
                            <span><i><b>Processing please wait...</b></i>
                            <asp:Image ID="img2" runat="server" ImageUrl="~/Images/loading.gif" ImageAlign="AbsMiddle" />
                            </span>
                        </div>
                    </div>
                 </ProgressTemplate> 
               </asp:UpdateProgress>
        
        <asp:Panel id="bindingsPanel" Visible="False" runat="server" >
          <table border="0"  width="100%" cellpadding="0" cellspacing="0" align="left">
          <tr>
          <td>&nbsp;</td>
          </tr>
            <tr>
                <td width="100%" bgcolor="#E9E9E9">
                    <table border="0" bgcolor="#E9E9E9" bordercolor="#C0C0C0" cellpadding="0" cellspacing="0" width="100%" align="left">
		                <tr>
		                <td align="left" colspan="2">
		                       <asp:Label ID="lblSection" Width="100%" style="background-color:#525252;color:gold;" Visible="false" runat="server"></asp:Label>
   	                    </td>
   	                    </tr>
		                <tr>
		                    <td align="left">
		                        <!-- udpdate Panel starts here -->
		                        <asp:UpdatePanel ID="linkPanel" Visible="false" runat="server">
		                            <ContentTemplate>
                                     <div style="background-color:#FFFFCC">
		                                <asp:LinkButton ID="btnExportMSI" Text="MSI file" runat="server" cssclass="linkConfig" OnClick="btnExportMSI_Click" />&nbsp;
		                                <asp:LinkButton ID="btnExportBindings" Text="Binding file" runat="server" cssclass="linkConfig" OnClick="btnExportBindings_Click" />
	                                    <asp:HyperLink ID="bindingFileLink" runat="server" Target="_blank" cssclass="linkConfig"></asp:HyperLink>
                                      </div>
    		                         </ContentTemplate>
	  	                        </asp:UpdatePanel>
	  	                        
	  	                        <!-- udpdate Panel starts here -->
		                    </td>
		                </tr>
  	                </table>
                </td>
            </tr>
              <tr>
              <td>&nbsp;</td>
              </tr>
            <tr>
                <td align="left">
		            <asp:Label ID="lblSection2" Width="100%" style="background-color:#525252;color:gold;" Visible="false" runat="server"></asp:Label>
   	            </td>
            </tr>
            <tr>
                <td width="*" bgcolor="darkgrey"  align="center" >
   	            </td>
            </tr>
            <tr>
                <td width="100%" bgcolor="#E9E9E9">
                    <table border="1" bgcolor="#E9E9E9" bordercolor="#525252" cellpadding="0" cellspacing="0" width="100%" align="left">
		            <tr>
	                    <td colspan="2" align="left" width="100%">
	                            <asp:TextBox ID="tBindings" Visible="false" ForeColor="#5D7B9D" runat="server" BorderStyle="Groove" Wrap="True" TextMode="MultiLine" Width="99%" Rows="20" />
		                </td>
		            </tr>
		            <tr>
	                    <td align="left">
		                     <asp:UpdatePanel ID="linkPanel2" runat="server">
		                     <ContentTemplate>
                             <div style="background-color:#FFFFCC">
		      		                <asp:LinkButton ID="btnImportBinding" 
				                        Text="Import bindings" Visible="false"
						                runat="server" 
						                cssclass="linkConfig" 
						                OnClick="btnImportBindings_Click" OnClientClick="return confirm('Are you sure you want to proceed?');" />
                                        </div>
		                     </ContentTemplate>
		                     <Triggers>
		                          <asp:AsyncPostBackTrigger ControlID="btnImportBinding" EventName="Click" />
		                     </Triggers>
	  	                     </asp:UpdatePanel>
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
                    The following set of operations can be performed using this module. <br /><br />
                    <b>Export Bindings</b> - Choose the application and select the option.<br /><br />
                    <b>Export Applications</b> - Choose the application and select the option.<br /><br />
		            <b>Import Bindings</b> - Paste the bindings file XML and select 'Import Bindings'.<br /><br />
                    <asp:DataList ID="dlApplication" runat="server" skinID="sideList" >
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
           <td>
	            <!-- Grid View for activity log -->
                    <asp:SqlDataSource ID="ActivityDataSource2" runat="server" ConnectionString="<%$ ConnectionStrings:authStore %>"
                        SelectCommand="SELECT [Message] FROM aspnet_WebEvent_Events WHERE EventType = 'BCC.Core.BCCWebAuditEvent' and EventCode = 100501 ORDER BY EventTime DESC">
                    </asp:SqlDataSource>
               <asp:Xml ID="Xml1" runat="server"></asp:Xml>

                    <asp:GridView id="UserActivityGrid2" SkinID="sideTable" Visible="true" runat="server" Width="100%" ForeColor="#333333"
                                CellPadding="4" CellSpacing="0" AutoGenerateColumns="False" DataSourceID="ActivityDataSource2"
                            AllowPaging="True" PageSize="5" AllowSorting="True" Gridlines="Both" EmptyDataText="No record(s) available">
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