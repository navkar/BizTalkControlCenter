<%@ Page Language="C#" MasterPageFile="~/template.master" AutoEventWireup="true" CodeFile="BAP.aspx.cs" 
Inherits="Pipelines" EnableViewState="false"%>

<%@ Register TagPrefix="uc" TagName="Search" Src="~/Controls/SearchControl.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="leftContentPlaceHolder" Runat="Server">
    <table border="0" bordercolor="#C0C0C0" width="99%" cellpadding="0" cellspacing="0" align="center">
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
            <td>
		        <asp:Panel ID="tablePanel" runat="server">
                <table border="1" bgcolor="#E9E9E9" bordercolor="#C0C0C0" cellpadding="0" cellspacing="0" width="100%" align="left">
                    <tr align="left">
                       <td align="left" valign="bottom">&nbsp;
                       </td>
			           <td align="right" width="1%">
			                <asp:LinkButton ID="btnExportToExcel" runat="server" style="color:teal;text-decoration:none;" OnClick="btnExportToExcel_Click" >
				            <asp:Image ID="excel" runat="server" ImageUrl="~/Images/Excel-16.gif" />
			                </asp:LinkButton>
			            </td>
                    </tr>
                    <tr align="left">
                        <td align="left" valign="bottom"colspan="2" >
		                    <uc:Search ID="search" runat="Server" /> 
                        </td>
                    </tr>                    
                </table>
		</asp:Panel> 
            </td>
        </tr>
   
    	<tr>
            <td>
		<table border="1" bordercolor="#C0C0C0" width="100%" cellpadding="0" cellspacing="0" align="center">
        <tr>
		    <td>
                <asp:Panel ID="pipelinePanel" runat="server" Visible="true">
                    <table border="1" bordercolor="#C0C0C0" width="100%" cellpadding="0" cellspacing="0" align="center">
                        <tr>
                            <td style="word-break:break-all;">
                                <asp:GridView ID="gridPipeline" runat="server" 
                                AutoGenerateColumns="False" 
                                CellPadding="4" GridLines="Both" AllowPaging="true" PageSize="20" OnPageIndexChanging="gridPipeline_PageIndexChanging"
                                Width="100%" >
                                <Columns>
                                    <asp:BoundField DataField="FullName" HeaderText="Name">
                                        <ItemStyle Font-Italic="false" Wrap="true"/>
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Type" HeaderText="Type">
                                        <ItemStyle Font-Italic="false" Wrap="false"/>
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Assembly" HeaderText="Assembly">
                                        <ItemStyle Font-Italic="false" Wrap="true"/>
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Application" HeaderText="Application" />
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
                                <b>No pipeline(s) were found.</b><br /><br />
                                The BizTalk Applications specified in the user profile does not have any Pipeline(s). <br /><br />
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
            </td>
        </tr>
        <tr>
            <td> 
                <table border="1" bgcolor="#E9E9E9" bordercolor="#C0C0C0" cellpadding="0" cellspacing="0" width="100%" align="left">
                    <tr align="left">
                        <td align="left" valign="bottom">&nbsp;
                        </td>
			        <td align="right" width="1%">
			            <asp:LinkButton ID="btnExportToExcel2" runat="server" style="color:teal;text-decoration:none;" OnClick="btnExportToExcel_Click" >
				        <asp:Image ID="excel2" runat="server" ImageUrl="~/Images/Excel-16.gif" />
			            </asp:LinkButton>
			        </td>
                    </tr>
                </table>

            </td>
        </tr>
	<tr>
            <td width="*" bgcolor="#5D7B9D"  align="center" >
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
                    <b>BizTalk Pipeline</b><br />
                    A pipeline consists of several stages. Each stage contains one or more Microsoft .NET or COM components, and each component handles a particular part of message processing.
                    <br /><br />
            <b>Adding new BizTalk Applications</b> <br /> 
            If you are expecting to see additional pipelines and are not seeing them, you might want to add additional <b>BizTalk Applications</b> into the user profile, use speedcode <b>603</b>.
             <br /> <br />
            <asp:DataList ID="dlPAppList" runat="server" skinID="sideList" >
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
       </table>
</asp:Content>    
