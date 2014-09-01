<%@ Page Language="C#" MasterPageFile="~/template.master" EnableViewState="true"
    ValidateRequest="false" CodeFile="DC.aspx.cs" Inherits="DeploymentConfiguration" Title="" %>

<asp:Content ID="Content1" ContentPlaceHolderID="leftContentPlaceHolder" runat="Server">

    <table border="0" bgcolor="#E9E9E9" width="99%" cellpadding="0" cellspacing="0" align="left">
        <tr>
            <td align="center">
		        <asp:Label ID="lblCaption" skinID="PageCaption" runat="server"></asp:Label>
   	        </td>
        </tr>
        <tr>
                <td>
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
                                <asp:AsyncPostBackTrigger ControlID="submit" EventName="Click" />
                    </Triggers>
                    </asp:UpdatePanel>                    
                </td>
        </tr>
       <tr>
       <td>
	        <!-- Grid View for activity log -->
                <asp:SqlDataSource ID="ActivityDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:authStore %>"
                    SelectCommand="SELECT 'Recent activity' as Comment, [Message] FROM aspnet_WebEvent_Events WHERE EventTime = (select MAX(EventTime) FROM aspnet_WebEvent_Events WHERE EventType = 'BCC.Core.BCCWebAuditEvent' and EventCode = 100502)">
                </asp:SqlDataSource>

                <asp:GridView id="UserActivityGrid" SkinID="sideTable" Visible="true" runat="server" Width="100%" ForeColor="#333333"
                            CellPadding="4" CellSpacing="0" AutoGenerateColumns="False" DataSourceID="ActivityDataSource" 
                        ShowFooter="false" ShowHeader="false" Gridlines="Both" EmptyDataText="No record(s) available">
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
            
               <asp:Panel ID="tablePanel" runat="server" >
                
                <table border="1" cellpadding="0" cellspacing="0" width="100%" bgcolor="#FFFFCC" bordercolor="#C0C0C0" >
                    <tr><td>
                    <asp:PlaceHolder ID="linkButtonGroup" runat="server" Visible="true" />
                    </td></tr>
                </table>
               </asp:Panel>
            </td>
        </tr>
        <tr>
            <td>
                <asp:UpdateProgress ID="updProgress" runat="server" DynamicLayout="true" DisplayAfter="0" AssociatedUpdatePanelID="upTextArea" > 
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
               
               <asp:Panel ID="pXmlConfig" runat="server" Visible="false" >
		        <table border="2" bordercolor="#525252" width="100%" cellpadding="0" cellspacing="0" align="left">
                <thead align="center" >
                    <asp:Label style="background-color:#525252;color:gold;" ID="lblXmlConfig" runat="server" Width="100%" />
                </thead>
                <tr>
                    <td colspan="2" align="left" width="99%">
                            <asp:TextBox ID="configXmlText" BackColor="white" Font-Bold="true" ForeColor="#5D7B9D" Width="99%" TextMode="MultiLine" Wrap="true" Rows="25" BorderStyle="Groove" runat="server"></asp:TextBox>
                    </td>
                </tr>              
		        </table>
                </asp:Panel>
                <!-- Update Panel starts here -->
                <asp:UpdatePanel ID="upTextArea" Visible="false" runat="server" >
		        <ContentTemplate>   
                    <table border="1" bordercolor="#C0C0C0" bgcolor="#FFFFCC" width="100%" cellpadding="0" cellspacing="0" align="left">
                    <tr>
                        <td>
                            <asp:LinkButton ID="submit" runat="server" OnClick="btnSubmit_Click" CssClass="linkConfig" Text="Save & Apply configuration" OnClientClick="return confirm('Would you like to Save & Apply configuration?');" />&nbsp;
                            <asp:LinkButton ID="save" runat="server" OnClick="btnSave_Click" CssClass="linkConfig" Text="Save configuration" OnClientClick="return confirm('Would you like to Save configuration?');" />&nbsp;
                            <asp:LinkButton ID="template" runat="server" OnClick="btnTemplate_Click" CssClass="linkConfig" Text="Get configuration template" />
                        </td>		                                                    
                    </tr>
                    </table>                        
                </ContentTemplate>
                <Triggers>
                    <asp:PostBackTrigger ControlID="template" />
                </Triggers>
                </asp:UpdatePanel>
                                       
                <!-- Update Panel ends here -->

            </td>
        </tr>
        <tr>
            <td>
            </td>
        </tr>
       <tr>
            <td align="center">
            <asp:Panel ID="adapterPanel" runat="server" Visible="true" GroupingText="Installed Adapters" >
                <asp:GridView id="adapterList" Visible="true" runat="server" Width="100%" ForeColor="#333333"
                            CellPadding="4" CellSpacing="0" AutoGenerateColumns="False"
                         Gridlines="Both" EmptyDataText="No record(s) available">
                        <Columns>
                                <asp:BoundField DataField="ProtocolName" HeaderText="Adapter (Protocol)" >
                                      <ItemStyle Width="20%" HorizontalAlign="Left" Font-Italic="false" Wrap="false"/>
                                </asp:BoundField>
                                <asp:BoundField DataField="DefaultSendHandler" HeaderText="Default Send Handler" >
                                      <ItemStyle Width="20%" HorizontalAlign="Left" Font-Italic="false" Wrap="true"/>
                                </asp:BoundField>
                                <asp:BoundField DataField="Capabilities" HeaderText="Adapter Capabilities" >
                                      <ItemStyle Width="60%" HorizontalAlign="Left" Font-Italic="false" Wrap="true"/>
                                </asp:BoundField>                                                                
                            </Columns>
                </asp:GridView>  
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
                    <b>Deployment - Host Configuration</b><br />
                    Use this module to create the various BizTalk Host and Host instances. 
                    <br /><br />
                    <b>Deployment - SSO Configuration</b><br />
                    Use this module to create the SSO application. Use speedcode '211' to view the newly created SSO Application.
                    <br /><br />
                    <b>NOTE:</b> An existing SSO Application shall be overwritten with new values. 
                    <br /><br />
                    <b>Installed Adapters</b> <br />
                    This table lists the various adapters and handlers, this can be helpful when you select HostConfiguration.
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
                    SelectCommand="SELECT [Message] FROM aspnet_WebEvent_Events WHERE EventType = 'BCC.Core.BCCWebAuditEvent' and EventCode = 100502 ORDER BY EventTime DESC">
                </asp:SqlDataSource>
           <asp:Xml ID="Xml1" runat="server"></asp:Xml>

                <asp:GridView id="UserActivityGrid2" SkinID="sideTable" Visible="true" runat="server" Width="100%" ForeColor="#333333"
                            CellPadding="4" CellSpacing="0" AutoGenerateColumns="False" DataSourceID="ActivityDataSource2"
                        AllowPaging="True" PageSize="10" AllowSorting="True" Gridlines="Both" EmptyDataText="No record(s) available">
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
