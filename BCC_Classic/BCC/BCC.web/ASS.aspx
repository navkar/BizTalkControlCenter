<%@ Page Language="C#" MasterPageFile="~/template.master" AutoEventWireup="true" CodeFile="ASS.aspx.cs" Inherits="SystemSettings" 
EnableViewState="true" %>

<%@ Register Assembly="BCC.Controls" Namespace="BCC.Controls" TagPrefix="bcc" %>

<%@ Register TagPrefix="bcc" TagName="CollectionView" Src="~/Controls/CollectionView.ascx" %>
<%@ Reference Control="~/Controls/CollectionView.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="leftContentPlaceHolder" Runat="Server">

<script type="text/javascript">
function SetThemeCookie(mood)
{
   if (mood == 'formal')
   {	
	document.cookie = "theme=night";  
   }
   else if (mood == 'casual')	
   {
	document.cookie = "theme=day";  
   }
   else if (mood == 'retro')	
   {	
	document.cookie = "theme=retro";  
   }
   else
   {
	document.cookie = "theme=day"; // this is casual theme.
   }

   return;

}

</script>

      <table border="0" width="95%" cellpadding="0" cellspacing="0" align="left">
        <tr>
            <td align="center">
		        <asp:Label ID="lblCaption" skinID="PageCaption" runat="server"></asp:Label>
   	        </td>
        </tr>
        <tr>
            <td align="left" >
                <table border="0" width="100%" bgcolor="CECECE" cellpadding="0" cellspacing="0" align="left">
                <tr>
                    <td width="2%">
                        <asp:Image ID="errorImg" runat="server" ImageUrl="~/Images/error.png" ImageAlign="Baseline" Visible="false" />
                        <asp:Image ID="okImg" runat="server" ImageUrl="~/Images/ok.png" Visible="false" />
		            </td>
		            <td width="95%">
		                <asp:Label ID="lblStatus" runat="server" Visible="false" Width="100%"></asp:Label>
		            </td>
		        </tr>
                </table>
   	        </td>
        </tr>
        <!-- Grid View for activity log -->
        <tr>
            <td>
                <asp:SqlDataSource ID="ActivityDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:authStore %>"
                    SelectCommand="SELECT 'Recent activity' as Comment, [Message] FROM aspnet_WebEvent_Events WHERE EventTime = (select MAX(EventTime) FROM aspnet_WebEvent_Events WHERE EventType = 'BCC.Core.BCCWebAuditEvent' and EventCode = 100603)">
                </asp:SqlDataSource>

 	            <asp:GridView id="UserActivityGrid" Visible="false" runat="server" Width="100%" ForeColor="#333333"
                        CellPadding="4" CellSpacing="0" AutoGenerateColumns="False" DataSourceID="ActivityDataSource"
	                    Gridlines="None" EmptyDataText="" ShowFooter="false" ShowHeader="false">
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
        <!-- Grid View for activity log ends -->
        <tr>
            <td>&nbsp;</td>
        </tr>
        <tr>
             <td>
               <asp:Panel ID="themePanel" runat="server" GroupingText="Theme settings" Visible="false">
                <div id="themeDiv" runat="server" visible="false">
                  <table border="0" width="60%" cellpadding="0" cellspacing="0" align="left">
                    <tr>
                        <td>
                            <asp:Label ID="Label1" runat="server"></asp:Label>
                        </td>
                        <td align="center">
                             <asp:LinkButton BackColor="#525252" ForeColor="White" BorderStyle="Outset" BorderWidth="2" ID="btnMood" runat="server" style="text-decoration:none;" OnClientClick="javascript:SetThemeCookie('casual')" OnClick="btnMood_Click" Text="&nbsp;Casual Theme&nbsp;" />
                        </td>
                        <td align="center">    
                            <asp:LinkButton BackColor="#5D7B9D" ForeColor="White" BorderStyle="Outset" BorderWidth="2" ID="btnMood2" runat="server" style="text-decoration:none;" OnClientClick="javascript:SetThemeCookie('formal')" OnClick="btnMood_Click" Text="&nbsp;Formal Theme&nbsp;" />
                        </td>
                        <td align="center">                      
                            <asp:LinkButton BackColor="#7C1818" ForeColor="White" BorderStyle="Outset" BorderWidth="2" ID="btnMood3" runat="server" style="text-decoration:none;" OnClientClick="javascript:SetThemeCookie('retro')" OnClick="btnMood_Click" Text="&nbsp;Retro Theme&nbsp;" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            &nbsp;
                        </td>
                        <td align="center">
                            <asp:Label ID="lblTheme" runat="server" Visible="false" Width="100%"></asp:Label>
                        </td>
                        <td align="center">    
                            <asp:Label ID="lblTheme2" runat="server" Visible="false" Width="100%"></asp:Label>
                        </td>
                        <td align="center">                      
                            <asp:Label ID="lblTheme3" runat="server" Visible="false" Width="100%"></asp:Label>
                        </td>
                    </tr>
                   </table>
                </div>
                </asp:Panel>
            </td>
        </tr>
        <tr>
            <td>
            <asp:Label ID="subTitle" runat="server" Text="User Profile" /> 
            <asp:Label ID="Label2" Font-Bold="true" EnableTheming="false" runat="server" Text="(Note: Your profile is derived from DefaultProfile.xml)" />
            <hr />
            </td>
        </tr>
        <tr>
            <td align="left" style="word-break:break-all;">
            <asp:Panel ID="userProfilePanel" runat="server">
              	<asp:GridView ID="gridUserProfile" runat="server" AutoGenerateColumns="False" CellPadding="4"
              	        AllowSorting="false" AllowPaging="false" PageSize="10" 
                        ForeColor="white" GridLines="Both" Width="100%" >
                    <Columns>
                           <asp:BoundField DataField="IsFilterEnabled" HeaderText="Is Filter Enabled?" >
                                    <ItemStyle Width="15%" Font-Italic="false" Wrap="false" />
                           </asp:BoundField>
                           <asp:BoundField DataField="IsProfileActive" HeaderText="Is Profile Active?"  >
                                    <ItemStyle Width="15%" Font-Italic="false" Wrap="false" />
                           </asp:BoundField>                           
                           <asp:BoundField DataField="LastAccessed" HeaderText="Last accessed"  >
                                    <ItemStyle Width="15%" Font-Italic="false" Wrap="false" />
                           </asp:BoundField>      
                          <asp:BoundField DataField="LastUpdated" HeaderText="Last updated"  >
                                    <ItemStyle Width="15%" Font-Italic="false" Wrap="false" />
                           </asp:BoundField>      
                          <asp:BoundField DataField="Created" HeaderText="Created"  >
                                    <ItemStyle Width="15%" Font-Italic="false" Wrap="false" />
                           </asp:BoundField>
                          <asp:BoundField DataField="UserTheme" HeaderText="Current theme"  >
                                    <ItemStyle Width="15%" Font-Italic="false" Wrap="false" />
                           </asp:BoundField>                                                                                     
                    </Columns>
        	    </asp:GridView>
       	    </asp:Panel>
             <br />
             <asp:LinkButton ID="lnkRefreshProfile" runat="server" cssclass="linkConfig" Text="Refresh user profile" OnClick="btnRefreshProfile_Click"/>
            </td>
        </tr>
       <tr>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="subTitle2" runat="server" Text="User Profile Configuration" />
                <asp:Label ID="subTitle3" Font-Bold="true" EnableTheming="false" runat="server" Text="(Warning: use extreme caution while changing the key values)" />
            <hr />
            </td>
        </tr>
        <tr>
             <td>
                <table border="0" cellpadding="0" cellspacing="0" align="left" bordercolor="#C0C0C0">
                    <tr>
                        <td width="30%">
                                <asp:Dropdownlist runat="server" ID="ddlModule" AutoPostBack="true" OnSelectedIndexChanged="ddlModule_SelectedIndexChanged">
                                <asp:ListItem Value="000" Text="Select a speedcode"></asp:ListItem>
                                </asp:Dropdownlist>
                        </td>
                        <td width="20%">
                                <asp:Dropdownlist runat="server" ID="ddlKey" >
                                <asp:ListItem Value="000" Text="Select a key"></asp:ListItem>
                                </asp:Dropdownlist>
                        </td>
                        <td>
                                <asp:LinkButton ID="btnViewConfig" runat="server" cssclass="linkConfig" Text="View configuration" OnClick="btnViewConfig_Click"/>
                        </td>
                    </tr>
                    <tr>
                        <td>&nbsp;
                        </td>
                        <td>&nbsp;
                        </td>
                        <td>&nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td align="center" colspan="2">
                            <div align="center" style="border-style:groove;border-width:thin;border-color:#525252">
                                <asp:Label style="background-color:#525252;color:gold;" ID="Label6" Text="User Profile Configuration" runat="server" Width="100%" />
                                <bcc:CollectionView ID="moduleConfiguration" runat="server" Visible="false" OnCollectionViewEvent="moduleConfiguration_CollectionViewEvent" />
                            </div>
                        </td>
                        <td>&nbsp;
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
		           <b>BizTalk Control Center (BCC) System Settings </b><br /> 
		           The agent takes around 60 seconds to enable all the BizTalk Artifact user notifications. To enable/disable notifications use speedcode <b>'103'</b>.<br />
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
            <td align="center">
				    <!--- Shady code goes here -->
            <div class="shiftcontainer">
            <div class="shadowcontainer300">
            <div class="innerdiv" align="left">
		           <b>User Profile Configuration changes</b><br />  <br />
		           <b>Step 1:</b> Select the 'speedcode' and the appropriate 'key' from the dropdown list and click 'View configuration'.<br /> 
        		   <b>Step 2:</b> You can now make required changes to the values.<br /> 
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
                        SelectCommand="SELECT [Message] FROM aspnet_WebEvent_Events WHERE EventType = 'BCC.Core.BCCWebAuditEvent' and EventCode = 100603 ORDER BY EventTime DESC">
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

