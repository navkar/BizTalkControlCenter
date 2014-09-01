<%@ Page Language="C#" MasterPageFile="~/template.master" AutoEventWireup="true" CodeFile="AMUR.aspx.cs" Inherits="UserRoles" EnableViewState="true"  %>

<asp:Content ID="Content1" ContentPlaceHolderID="leftContentPlaceHolder" Runat="Server">
      <table border="0" width="100%" cellpadding="0" cellspacing="0" align="left">
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
                        <asp:Image ID="errorImg" runat="server" ImageUrl="~/Images/error.png" Visible="false" />
                        <asp:Image ID="userImg" runat="server" ImageUrl="~/Images/user-accept.png" Visible="false" />
                        <asp:Image ID="okImg" runat="server" ImageUrl="~/Images/ok.png" Visible="false" />
		            </td>
		            <td width="98%">
		                <asp:Label ID="lblStatus" runat="server" Visible="false" Width="100%"></asp:Label>
		            </td>
		        </tr>
                </table>
   	    </td>
        </tr>
        <!-- Grid View for activity log -->
        <tr>
            <td colspan="2">
            <asp:SqlDataSource ID="ActivityDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:authStore %>"
                SelectCommand="SELECT 'Recent activity' as Comment, [Message] FROM aspnet_WebEvent_Events WHERE EventTime = (select MAX(EventTime) FROM aspnet_WebEvent_Events WHERE EventType = 'BCC.Core.BCCWebAuditEvent' and (EventCode = 100601 OR EventCode = 100100))">
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
            <td> 
               <asp:Panel ID="tablePanel" runat="server">
                <table border="1" bgcolor="#E9E9E9" bordercolor="#5D7B9D" cellpadding="0" cellspacing="0" width="100%" align="left">
                    <tr align="left">
                    <td>
                               <table border="0" width="100%" cellpadding="0" cellspacing="0" align="left">
                                <tr>
                                <td align="left" width="20%">
                                    <asp:TextBox id="tUserName" width="97%" ForeColor="#5D7B9D" runat="server" BorderStyle="Groove" Tooltip="Enter a search term" AutoCompleteType="Search" />
                                </td>
                                <td align="left" width="1%">
                                   <asp:LinkButton ID="btnLookup" OnClick="btnLookup_Click" runat="server" cssclass="linkConfig">
                                        <asp:Image ToolTip="Locate a user" ID="searchImg" runat="server" ImageUrl="~/Images/search-icon.gif" />
                                    </asp:LinkButton>
                                </td>
                                <td align="left" width="80%">
                                    [Specify a user name to view role(s), for e.g. shiva]
                                </td>
                                </tr>
                            </table>	
                    </td>
                    </tr>
                </table>
               </asp:Panel>
            </td>
        </tr>
       <tr>
       <td valign="top">
            <!-- Roles Panel -->
                    <asp:Panel ID="rolesPanel" Visible="false" runat="server">
                    <table border="0" bgcolor="#E9E9E9" bordercolor="#5D7B9D" cellpadding="0" cellspacing="0" width="100%" align="left">
                        <tr>
                            <td>
                                &nbsp;
                            </td>
                        </tr>
                        <tr align="left">
                            <td width="2%">
                                &nbsp;
                            </td>
                            <td align="left" valign="bottom">
                                <!-- Inner Table -->
                                <table align="left" width="70%" border="1" bgcolor="#E9E9E9" bordercolor="#5D7B9D" cellpadding="0" cellspacing="0"  >
                                <tr>
                                    <td align="center"><asp:Label id="Label1" Text="User Role List" Runat="server" /></td>
                                </tr>
                                <tr>
                                    <td align="left">
                                             <asp:CheckBoxList ID="UserRoleList" CellPadding="5" CellSpacing="5" RepeatDirection="Vertical" RepeatLayout="Flow" TextAlign="Right" Runat="server" />
                                    </td>
                                </tr>
                                <tr>
                                    <td align="left">
                                               <asp:Label id="lblEmail" EnableTheming="false" Text="User email" AssociatedControlID="tbEmail" Runat="server" />
                                               <asp:TextBox id="tbEmail" BorderStyle="Groove" Width="50%" Runat="server" ForeColor="#5D7B9D" />           
                                               <asp:RequiredFieldValidator ID="rv" runat="server" ControlToValidate="tbEmail" Text="required!"></asp:RequiredFieldValidator>
                                               <asp:RegularExpressionValidator ID="regEmail" ControlToValidate="tbEmail" Text="Email is invalid"
                                                    ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" Runat="server" />    
                                    </td>
                                </tr>
                                <tr>
                                    <td align="left">
                                               <asp:Label id="lblUserLocked" EnableTheming="false" Text="Unlock user" AssociatedControlID="chkLocked" Runat="server" />
                                               <asp:CheckBox ID="chkLocked" runat="server" Enabled="false" ToolTip="You must check the box to un-lock a user." /> (You must check the box to un-lock a user)
                                    </td>
                                </tr>
                                <tr>
                                    <td align="center" valign="middle">
                                        <asp:LinkButton ID="updateBtn" style="background-color:#5D7B9D;font-weight:bold;display:block;width:75px;" OnClick="btnUpdate_Click" text="" runat="server">
                                            <asp:Image ID="imgRemoveUser" runat="server" ImageUrl="Images/user-accept.png" ToolTip="Update user properties" />
                                        </asp:LinkButton>
                                    </td>
                                </tr>
                                </table>
                                <!-- Inner Table -->            
                            </td>	
                        </tr>
                    </table>
                    </asp:Panel>
            <!-- Roles Panel -->
            </td>
        </tr>
        <tr>
            <td>&nbsp;</td>
        </tr>
            <tr>
                <td>
                        <asp:GridView id="UserGrid" runat="server" Width="100%" ForeColor="#333333"
                        CellPadding="4" CellSpacing="0" ShowFooter="true"
                        AutoGenerateEditButton="false"
                        AutoGenerateColumns="False"
                        OnRowDeleting="UserGrid_RowDeleting"
                        OnRowDataBound="UserGrid_RowDataBound"
                        OnRowCommand="UserGrid_RowCommand"
                        Gridlines="Both">
                            <Columns>
                                    <asp:TemplateField HeaderText="User Name" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" ControlStyle-Font-Bold="false" SortExpression="userName">
                                        <ItemTemplate>
                                                <asp:LinkButton ID="lnkUser" CausesValidation="false" CommandName="UpdateUser" style="font-weight:bold;display:block;width:75px;text-decoration:none" runat="server" Text='<%# Bind("UserName") %>'></asp:LinkButton>
                                        </ItemTemplate>
                                        <ItemStyle Width="15%" Wrap="false" />
                                        <FooterTemplate>
                                            <asp:TextBox MaxLength="25" BorderStyle="Groove" Width="85%" ID="tNewUserName" runat="server" Text="new user"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="rv1" runat="server" ToolTip="Required!" Text="*" ControlToValidate="tNewUserName"></asp:RequiredFieldValidator>
                                        </FooterTemplate>
                                        <FooterStyle Width="15%" Wrap="false" /> 
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Email" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Center" ControlStyle-Font-Bold="false" SortExpression="userEmail">
                                        <ItemTemplate>
                                                <asp:Label ID="lblEmail" runat="server" Text='<%# Bind("Email") %>'></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle Width="10%" Wrap="false" />
                                        <FooterTemplate>
                                            <asp:TextBox MaxLength="35" BorderStyle="Groove" Width="85%" ID="tNewUserEmail" runat="server" Text="member@email.com"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="rv2" runat="server" ToolTip="Required!" Text="*" ControlToValidate="tNewUserEmail"></asp:RequiredFieldValidator>
                                            <asp:RegularExpressionValidator ID="regEmail" ControlToValidate="tNewUserEmail" Text="Email is invalid"
                                                    ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" Runat="server" />   
                                        </FooterTemplate>
                                        <FooterStyle Width="10%" Wrap="false" /> 
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Locked" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" ControlStyle-Font-Bold="false" SortExpression="locked">
                                        <ItemTemplate>
                                                <asp:Label ID="lblLocked" runat="server" Text='<%# Bind("IsLockedOut") %>'></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle Width="5%" Wrap="true" />
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="LastLoginDate" HeaderText="Last Login Date" DataFormatString="{0:MMM-dd-yy hh:mm:ss}"/>
                                    <asp:BoundField DataField="CreationDate" HeaderText="Creation Date" DataFormatString="{0:MMM-dd-yy hh:mm:ss}"  />
                                    <asp:BoundField DataField="LastPasswordChangedDate" HeaderText="Password Date" DataFormatString="{0:MMM-dd-yy hh:mm:ss}" />
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                        <asp:LinkButton ID="lnkRemoveUser" runat="server" CommandName="Delete" OnClientClick="javascript:return confirm('Are you sure you want to delete the user?');">
                                                <asp:Image ID="imgRemoveUser" runat="server" ImageUrl="Images/user-remove.png" ToolTip="Remove User" />
                                        </asp:LinkButton>
                                        </ItemTemplate>
                                        <ItemStyle Width="1%" />
                                        <FooterTemplate>
                                            <asp:LinkButton ID="lnkAdd" runat="server" CausesValidation="True" CommandName="Add">
                                                    <asp:Image ID="ImageAdd" runat="server" ImageUrl="Images/user-add.png" ToolTip="Add" />
                                            </asp:LinkButton>                                                  
                                        </FooterTemplate>        
                                    </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
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
                    <b>User Operations</b><br /><br />
                    <b>Create User:</b>To create a new user, specify a user name in the text field and click 'Search'. Next specify the roles and click 'Update user...'<br /><br />
                    <b>Update User:</b>To update a user, specify a user name in the text field and click 'Search'. Next specify the roles and click 'Update user...'<br /><br />
                    <b>How to reset password for a user?</b><br />You can delete and then re-create a user to reset password.<br /><br />
		    </div> 
		    </div>
		    </div>
        		    <!--- Shady code goes here -->
   	        </td>
       </tr>
       <tr>
            <td>&nbsp;</td>
       </tr>
       <tr>
            <td align="center">
            
            <asp:SqlDataSource ID="userRoleDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:authStore %>"
                SelectCommand="select u.UserName as UserName, count(r.RoleName) as RoleCount from [BCCDB].[dbo].[aspnet_Users] u, [BCCDB].[dbo].[aspnet_Roles] r, [BCCDB].[dbo].[aspnet_UsersInRoles] ur where u.UserId = ur.UserId and r.RoleId = ur.RoleId group by u.UserName">
            </asp:SqlDataSource>
            
            <asp:CHART DataSourceID="userRoleDataSource" id="userRoleChart" runat="server" Palette="Light" BackColor="#E9E9E9" Height="200px" Width="320px" BorderDashStyle="Solid" BorderWidth="2" BorderColor="26, 59, 105">
                <series>
                    <asp:Series Name="Default" ChartType="Doughnut" BorderColor="180, 26, 59, 105" Color="220, 65, 140, 240"></asp:Series>
                </series>
				<titles>
					<asp:Title Font="Verdana, 12pt" Text="Users" Name="taskChartTitle" ForeColor="Gold" BackColor="#525252"></asp:Title>
				</titles>                
                <chartareas>
                    <asp:ChartArea Name="ChartArea1" BorderColor="64, 64, 64, 64" BackSecondaryColor="#E9E9E9" BackColor="#FFFFCC" BorderWidth="0">
                    <area3dstyle Rotation="0" />
                    <axisy LineColor="64, 64, 64, 64">
	                    <LabelStyle Font="Verdana, 8pt, style=Bold" />
	                    <MajorGrid LineColor="64, 64, 64, 64" />
                    </axisy>
                    <axisx LineColor="64, 64, 64, 64">
	                    <LabelStyle Font="Verdana, 8pt, style=Bold" />
	                    <MajorGrid LineColor="64, 64, 64, 64" />
                    </axisx>
                    </asp:ChartArea>
                </chartareas>
            </asp:CHART>
            
   	        </td>
       </tr>           
    </table>
</asp:Content>