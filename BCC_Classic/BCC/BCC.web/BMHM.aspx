<%@ Page Language="C#" MasterPageFile="~/template.master" AutoEventWireup="true" CodeFile="BMHM.aspx.cs" Inherits="HealthMatrix" %>

<%@ Register TagPrefix="bcc" TagName="CollectionView" Src="~/Controls/CollectionView.ascx" %>
<%@ Reference Control="~/Controls/CollectionView.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="leftContentPlaceHolder" Runat="Server">
     <table border="0"  width="100%" cellpadding="0" cellspacing="0" align="left">
        <tr>
            <td align="center">
               <asp:Label skinid="PageCaption" ID="lblCaption" runat="server" ></asp:Label>
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
             <td width="100%" valign="top">
  	    
        <table border="0" cellpadding="7" cellspacing="7" width="100%" bordercolor="#5D7B9D" align="left">
        <!-- 1st row -->
		<tr>
            <td valign="top">
    		    <table border="1" bordercolor="#525252" width="100%" cellpadding="0" cellspacing="0" align="left">
                    <thead align="center" >
                        <asp:Label style="background-color:#525252;color:gold;" ID="Label5" Text="Database Status" runat="server" Width="100%" />
                    </thead>
                	<tr>
                	    <td>
	       		        <asp:GridView ID="gridDBStatus" runat="server" AutoGenerateColumns="False" CellPadding="4"
                                    ForeColor="white" GridLines="Both" Width="100%"
                                    AllowPaging="true" PageSize="5" 
                                 OnPageIndexChanging="gridDB_PageIndexChanging"  
                                 >
                                <Columns>
                                    <asp:BoundField DataField="ServerName" HeaderText="DB Server" />
                                    <asp:BoundField DataField="DBName" HeaderText="Database" />
                                    <asp:TemplateField HeaderText="">
                                       <ItemTemplate>
                                            <bcc:AlertIndicator ID="alertStatus" Status='<%# Bind("Status") %>' runat="server" />
                                       </ItemTemplate>
                                       <ItemStyle Width="5%" HorizontalAlign="Left" />
                                    </asp:TemplateField>                                       
                                </Columns>
                	      </asp:GridView>
                          </td>
                      </tr>
		        </table>
            </td>
		    <td valign="top">
		      <table border="1" bordercolor="#525252" width="100%" cellpadding="0" cellspacing="0" align="left">
                    <thead align="center" >
                        <asp:Label style="background-color:#525252;color:gold;" ID="Label4" Text="BizTalk Host Instances" runat="server" Width="100%" />
                    </thead>
	              <tr><td>
	              
	                        <asp:GridView ID="gridHost" runat="server" AutoGenerateColumns="False" CellPadding="4"
                                ForeColor="#333333" GridLines="Both" Width="100%"
                                  AllowPaging="true" PageSize="5" 
                                 OnPageIndexChanging="gridHost_PageIndexChanging"  
                                     >
                                <Columns>
                                    <asp:BoundField DataField="MachineName" HeaderText="Machine" ItemStyle-Width="40%" />
                                    <asp:TemplateField HeaderText="Host Instance" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Center" ControlStyle-Font-Bold="false" >
                                        <ItemTemplate>
                                                <asp:LinkButton ID="lnkPort" style="font-weight:normal;display:block;width:150px;text-decoration:none" runat="server" ToolTip='<%# Bind("HostType", "Host type is {0}") %>' Text='<%# Bind("HostName") %>' OnClick="lnkHost_Click"></asp:LinkButton>
                                        </ItemTemplate>
                                        <ItemStyle Width="55%" Font-Italic="false" Wrap="true"/>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="">
                                       <ItemTemplate>
                                            <bcc:AlertIndicator ID="alertStatus" Status='<%# Bind("Status") %>' runat="server" />
                                       </ItemTemplate>
                                       <ItemStyle Width="5%" HorizontalAlign="Left" />
                                    </asp:TemplateField>                                       
                                </Columns>
                            </asp:GridView>
                       </td></tr>
		    </table>
		    </td>

            <!------------------------------------------ -->
		    <td valign="top"> 
			<table border="1" bordercolor="#525252" width="100%" height="50%" cellpadding="0" cellspacing="0" align="left">
                    <thead align="center" >
                        <asp:Label style="background-color:#525252;color:gold;" ID="Label3" Text="Windows Services" runat="server" Width="100%" />
                    </thead>
	                <tr><td>
		
                      	    <asp:GridView ID="gridServices" runat="server" AutoGenerateColumns="False" CellPadding="4"
                                ForeColor="white" GridLines="Both" Width="100%"
                                    AllowPaging="true" PageSize="5" 
                                    OnPageIndexChanging="gridServices_PageIndexChanging"                                     
                                 >
                            <Columns>
                                    <asp:TemplateField HeaderText="Service Name" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Center" ControlStyle-Font-Bold="false" >
                                        <ItemTemplate>
                                                <asp:LinkButton ID="lnkPort" style="font-weight:normal;display:block;width:200px;text-decoration:none" runat="server" ToolTip='<%# Bind("ServiceStatus", "Service is {0}") %>' Text='<%# Bind("ServiceName") %>' OnClick="lnkService_Click"></asp:LinkButton>
                                        </ItemTemplate>
                                        <ItemStyle Width="95%" Font-Italic="false" Wrap="true"/>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="">
                                       <ItemTemplate>
                                            <bcc:AlertIndicator ID="alertStatus" Status='<%# Bind("ServiceStatus") %>' runat="server" />
                                       </ItemTemplate>
                                       <ItemStyle  Width="5%" HorizontalAlign="Left" />
                                    </asp:TemplateField>                                    
                            </Columns>
                	    </asp:GridView>

                	</td></tr>
			</table>
		    </td>	
		    <!------------------------------------------ -->	 
         </tr>
         <!-- 2nd row --> 
         <tr>
             <td valign="top">
                <table border="1" bordercolor="#525252" width="100%" cellpadding="0" cellspacing="0" align="left">
                    <thead align="center" >
                        <asp:Label style="background-color:#525252;color:gold;" ID="Label2" Text="BizTalk Receive Ports" runat="server" Width="100%" />
                    </thead>
    	                <tr>
                        <td>
                                 <asp:GridView ID="gridReceivePort" runat="server" AutoGenerateColumns="False" CellPadding="4"
                                    ForeColor="#333333" GridLines="Both" Width="100%"
                                    AllowPaging="true" PageSize="10" 
                                    OnPageIndexChanging="gridReceivePort_PageIndexChanging"                                     
                                     >
                                    <Columns>
                                    <asp:TemplateField HeaderText="Receive Location" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Center" ControlStyle-Font-Bold="false" >
                                        <ItemTemplate>
                                                <asp:LinkButton ID="lnkPort" style="font-weight:normal;display:block;width:300px;text-decoration:none" runat="server" ToolTip='<%# Bind("Application", "Application:{0}") %>' Text='<%# Bind("ReceivePortLocation") %>' OnClick="lnkReceivePort_Click"></asp:LinkButton>
                                        </ItemTemplate>
                                        <ItemStyle Width="95%" Font-Italic="false" Wrap="true"/>
                                    </asp:TemplateField>
                                        <asp:TemplateField HeaderText="">
                                           <ItemTemplate>
                                                <bcc:AlertIndicator ID="alertStatus" Status='<%# Bind("Status") %>' runat="server" />
                                           </ItemTemplate>
                                           <ItemStyle  Width="5%" HorizontalAlign="Left" />
                                        </asp:TemplateField>                                         
                                 
                                    </Columns>
                                </asp:GridView>

    	                </td></tr>
                </table>
             </td>
             <td valign="top">
			 <table border="1" bordercolor="#525252" width="100%" cellpadding="0" cellspacing="0" align="left">
                    <thead align="center" >
                        <asp:Label style="background-color:#525252;color:gold;" ID="Label1" Text="BizTalk Send Ports" runat="server" Width="100%" />
                    </thead>
                    <tr>
	                    <td style="word-break:break-all;">
                           <asp:GridView ID="gridSendPort" runat="server" 
                                AutoGenerateColumns="False" CellPadding="4"
                                Width="50%" ForeColor="#333333" GridLines="Both" 
                                AllowPaging="true" PageSize="10" 
                                OnPageIndexChanging="gridSendPort_PageIndexChanging" 
                                >
                                <Columns>
                                    <asp:TemplateField HeaderText="Send Port" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Center" ControlStyle-Font-Bold="false" >
                                        <ItemTemplate>
                                                <asp:LinkButton ID="lnkPort" style="font-weight:normal;display:block;width:300px;text-decoration:none" runat="server" ToolTip='<%# Bind("Application", "Application:{0}") %>' Text='<%# Bind("SendPortName") %>' OnClick="lnkSendPort_Click"></asp:LinkButton>
                                        </ItemTemplate>
                                        <ItemStyle Width="95%" Font-Italic="false" Wrap="true"/>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="">
                                       <ItemTemplate>
                                            <bcc:AlertIndicator ID="alertStatus" Status='<%# Bind("Status") %>' runat="server" />
                                       </ItemTemplate>
                                       <ItemStyle  Width="5%" HorizontalAlign="Left" />
                                    </asp:TemplateField>  
                                                                      
                                </Columns>
                            </asp:GridView>
	                    </td>
	                    
	                </tr>
			    </table>
             </td>
             <td valign="top">
			        <table border="1" bordercolor="#525252" width="100%" cellpadding="0" cellspacing="0" align="left">
                    <thead align="center" >
                        <asp:Label style="background-color:#525252;color:gold;" ID="lblInfo" Text="BizTalk Message Activity" runat="server" Width="100%" />
                    </thead>
                    <tr>
	                    <td style="word-break:break-all;">
	                        <!-- GridView for messaging activity -->
 	                        <asp:GridView id="gridMsgInfo" Visible="true" runat="server" Width="100%" ForeColor="#333333" OnRowDataBound="gridMsgInfo_RowDataBound"
                                    CellPadding="4" CellSpacing="0" AutoGenerateColumns="False" Gridlines="Both" EmptyDataText="">
  	                             <Columns>
                                        <asp:TemplateField HeaderText="Message State" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Center" ControlStyle-Font-Bold="false" >
                                            <ItemTemplate>
                                                    <asp:LinkButton ID="lnkMessageState" style="font-weight:normal;display:block;width:110px;text-decoration:none" runat="server" Tooltip='<%# Bind("Count","Count:{0}") %>' Text='<%# Bind("MessageState") %>' OnClick="lnkMsg_Click"></asp:LinkButton>
                                            </ItemTemplate>
                                            <ItemStyle Width="80%" Font-Italic="false" Wrap="false"/>
                                         </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Count" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" ControlStyle-Font-Bold="false" >
                                            <ItemTemplate>
                                                    <asp:LinkButton ID="lnkMsgCount" style="font-weight:normal;display:block;width:50px;text-decoration:none" runat="server" Text='<%# Bind("Count") %>' OnClick="lnkMsg_Click"></asp:LinkButton>
                                            </ItemTemplate>
                                            <ItemStyle Width="80%" Font-Italic="false" Wrap="false"/>
                                         </asp:TemplateField>


                                 </Columns>
  	                        </asp:GridView>
  	                        <!-- GridView for messaging activity -->
	                    </td>
	                </tr>
			        </table>
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
		           <b>Health Matrix</b> lists out the status of various BizTalk artifacts. Use this for reporting and monitoring purposes.<br />
		           <br />
		           <b>Adding/removing BizTalk applications</b> <br />
		           Use speedcode '603' to add/remove BizTalk applications.
                   <br /><br />

                   <b>Default application: </b> <asp:Label ID="lblDefaultBizTalkApp" EnableTheming="false" runat="server"> </asp:Label> <br />
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
       </table>
       <div align="center" style="border-style:groove;border-width:thin;border-color:#525252">
            <asp:Label style="background-color:#525252;color:gold;" ID="Label6" Text="Personal Reminders" runat="server" Width="100%" />
            <bcc:CollectionView ID="reminderCheckList" runat="server" OnCollectionViewEvent="reminderCheckList_ViewEvent" />
       </div>

       

</asp:Content>
