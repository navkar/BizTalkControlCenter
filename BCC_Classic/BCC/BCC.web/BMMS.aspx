<%@ Page Language="C#" MasterPageFile="~/template.master" AutoEventWireup="true" CodeFile="BMMS.aspx.cs" Inherits="BiztalkMessages" EnableViewState="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="leftContentPlaceHolder" runat="Server">

    <table border="0" bgcolor="#E9E9E9" bordercolor="#5D7B9D" width="100%" cellpadding="0" cellspacing="0" align="center">
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
	    <tr>
	       <td align="left" style="background-color:#E9E9E9">
                <iframe frameborder="1" src="BMMS-F.aspx" width="100%" height="310">
     	        </iframe>
           </td>
        <tr>
         <tr>
            <td>
                <asp:Label ID="lblTotalMessages" runat="server" Height="100%" Width="100%"></asp:Label>
	    </td>
        </tr>
        <tr>
        <td>
               <table border="0" bordercolor="#5D7B9D" width="100%" cellpadding="2" cellspacing="2" align="center">
               <tr>
                <td>
                    <div class="shiftcontainer">
                    <div class="shadowcontainer">
                    <div class="innerdiv">
                        <b>BizTalk Messages Load Meter</b> <br />
                        This indicates the message processing load in BizTalk server at any given point in time.<br />
                    </div>
                    </div>
                    </div>
                </td>
                <td>
                    <div class="shiftcontainer">
                    <div class="shadowcontainer">
                    <div class="innerdiv">
                        <b>Messages processed per 't' sec.</b> <br />
                        This metric indicates the number of messages processed by BizTalk within a given time interval.<br />
                    </div>
                    </div>
                    </div>
                </td>
                <td>
                    <div class="shiftcontainer">
                    <div class="shadowcontainer">
                    <div class="innerdiv">
                        <b>Message Distribution</b> <br />
                        This metric indicates the current state of various messages being processed.<br />
                    </div>
                    </div>
                    </div>
                </td>
                </tr> 
                </table>
        </td>
        </tr>
    </table>
</asp:Content>
