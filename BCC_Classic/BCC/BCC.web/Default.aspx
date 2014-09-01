<%@Page Language="C#" MasterPageFile="~/default.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Default" EnableViewState="false"%>

<%@ MasterType VirtualPath="~/default.master" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="leftContentPlaceHolder">
    <table border="0" bordercolor="#5D7B9D" width="100%" cellpadding="0" cellspacing="0" align="center">
        <tr>
            <td></td>
        </tr>
        <tr>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td>&nbsp;</td>
        </tr>   
        <tr>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td>&nbsp;</td>
        </tr>                      
        <tr>
            <td align="center">
                <strong><span class="spantag">Welcome to BizTalk Control Center</span></strong>
   	        </td>
        </tr>
 
        <tr>
            <td valign="top" width="100%"  align="center">
		        <table border="0" bordercolor="#5D7B9D" width="100%" cellpadding="0" cellspacing="0" align="center">
                <tr>
                    <td align="center">
                        <asp:Login ID="myLogin" runat="server" VisibleWhenLoggedIn="False" Orientation="Horizontal"
                        TextLayout="TextOnTop" BackColor="#E3EAEB" BorderColor="#5D7B9D" BorderPadding="4"
                        BorderStyle="Solid" BorderWidth="1px" Font-Names="Verdana" ForeColor="#333333"
                        CreateUserText=""
                        CreateUserUrl="" 
                        PasswordRecoveryUrl="" 
                        OnLoggingIn="Login_OnLoggingIn"
                        OnLoginError="Login_OnLoginError"
                        DisplayRememberMe="False">
                            <TitleTextStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                            <LoginButtonStyle BackColor="White" BorderColor="#C5BBAF" BorderStyle="Solid" 
			                            BorderWidth="1px" Font-Names="Verdana" Font-Size="0.8em" ForeColor="#1C5E55" />
                        </asp:Login>
                    </td>
                </tr>
		        <tr>
		            <td align=justify>
			            <asp:LinkButton ID="changePwd" runat="server" style="font-size:7pt;color:silver;text-decoration:none;" OnClick="BtnChangePwd_Click" Text="Change password" />
		            </td>
		        </tr>
		        <tr>
		            <td>&nbsp;</td>
		        </tr>
		        <tr>
		            <td>&nbsp;</td>
		        </tr>
		        <tr>
			        <td>
		                 <strong><span class="spantag"></span></strong>
			        </td>
		        </tr>
		        <tr>
		                <td align="center">
		                    <!--- Shady code goes here -->
                            <div class="shiftcontainer">
                            <div class="shadowcontainer">
                            <div class="innerdiv" align="left">
		                           <asp:Label ID="lblInfo" runat="server" style="font-size:8pt;color:#5D7B9D" Visible="false">
			                       </asp:Label><br />
		                           <asp:Label ID="lblLastActivity" runat="server" style="font-size:8pt;color:#5D7B9D" Visible="false">
			                       </asp:Label><br />
		                    </div>
		                    </div>
		                    </div>
		                    <!--- Shady code goes here -->
		                </td>
		        </tr>
		        </table>
            </td>
        </tr>
        <tr> <!-- last row --> 
            <td align="center">&nbsp;
           <asp:Panel id="changePwdPanel" Visible="False" runat="server">
	        <table  border="0" cellpadding="0" cellspacing="0">
   	        <tr>
                <td  align="center" colspan="2" valign="top">
                    <asp:ChangePassword ID="ChPwd" runat="server" ForeColor="#333333"
	                BackColor="#E3EAEB" BorderColor="#5D7B9D" BorderStyle="Solid" 
	                BorderWidth="1px" Font-Names="Verdana" BorderPadding="4" Width="100%"
  	                DisplayUserName="True"
	                OnCancelButtonClick="CancelButton_Click"
	                OnChangingPassword="ChangePassword_Click"
                    ContinueDestinationPageUrl="~/Login.aspx">

                   <ChangePasswordButtonStyle BackColor="White" BorderColor="#C5BBAF" BorderStyle="Solid" 
				              Font-Size="0.8em" BorderWidth="1px" Font-Names="Verdana" ForeColor="#1C5E55" />
                   <ContinueButtonStyle BackColor="#FFFBFF" BorderColor="#CCCCCC" 
                    BorderStyle="Solid" BorderWidth="1px" Font-Names="Verdana" 
                    Font-Size="0.8em" ForeColor="#284775" />
	                <CancelButtonStyle BackColor="#FFFBFF" BorderColor="#CCCCCC" 
                    BorderStyle="Solid" BorderWidth="1px" Font-Names="Verdana" 
                    Font-Size="0.8em" ForeColor="#284775" />
                   <TitleTextStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                   <PasswordHintStyle Font-Italic="True" ForeColor="#888888" />
                   <InstructionTextStyle Font-Italic="True" ForeColor="Black" />
               </asp:ChangePassword>
               <asp:Label ID="lblStatus" runat="server" Visible="false" Width="100%"></asp:Label>
               </td>
               </tr>
	        </table>
            </asp:Panel>
            </td>
            </tr>
        <tr>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td>&nbsp;</td>
        </tr>   
        <tr>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td>&nbsp;</td>
        </tr> 
    </table>
   
</asp:Content>
