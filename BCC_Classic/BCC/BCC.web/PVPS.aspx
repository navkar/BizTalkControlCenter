<%@ Page Language="C#"   ValidateRequest = "false" MasterPageFile="~/template.master" AutoEventWireup="true" CodeFile="PVPS.aspx.cs" Inherits="PartnerWebService" %>
<asp:Content ID="Content1" ContentPlaceHolderID="leftContentPlaceHolder" Runat="Server">

  <table border="0" bgcolor="#E9E9E9" bordercolor="#C0C0C0" width="100%" cellpadding="1" cellspacing="0" align="left">
        <tr>
	        <td colspan="2" align="center">
               <asp:Label ID="lblCaption" skinID="PageCaption" runat="server"></asp:Label>
   	        </td>
        </tr>
        <tr>
             <td colspan="2" width="100%" align="left" >
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
                SelectCommand="SELECT 'Recent activity' as Comment, [Message] FROM aspnet_WebEvent_Events WHERE EventTime = (select MAX(EventTime) FROM aspnet_WebEvent_Events WHERE EventType = 'BCC.Core.BCCWebAuditEvent' and EventCode = 100301)">
            </asp:SqlDataSource>

 	        <asp:GridView id="UserActivityGrid" Visible="false" runat="server" Width="100%" ForeColor="#333333"
                    CellPadding="4" CellSpacing="0" AutoGenerateColumns="False" DataSourceID="ActivityDataSource"
	                Gridlines="None" EmptyDataText="" ShowFooter="false" ShowHeader="false">
  	                <Columns>
  	                    <asp:BoundField DataField="Comment">
                              <ItemStyle Width="10%" Font-Italic="false" Wrap="true"/>
                        </asp:BoundField>
                        <asp:BoundField DataField="Message">
                              <ItemStyle Width="92%" Font-Italic="false" Wrap="true"/>
                        </asp:BoundField>
                    </Columns>
  	        </asp:GridView>
            </td>
        </tr>
        <!-- Grid View for activity log -->       
        <tr>
            <td>
            Choose Service:
            </td>
            <td align="left">
                <asp:DropDownList ID="dListServices" runat="server" AutoPostBack="true" 
				Font-Names="Verdana" BorderStyle="Groove" OnSelectedIndexChanged="dListService_SelectedIndexChanged" Width="70%">
                </asp:DropDownList>
	    </td>
        </tr>

        <tr>
            <td>
            URL:
            </td>
            <td align="left" width="80%">
                <asp:UpdatePanel ID="invokeEndpointPanel" runat="server" UpdateMode="Always">
		            <ContentTemplate>            
                        <asp:TextBox ID="endPoint" Width="70%" BorderStyle="Groove" runat="server"></asp:TextBox>
		                <asp:LinkButton ID="submit" runat="server" cssclass="linkConfig" Text="Invoke Endpoint" OnClientClick="return confirm('Are you ready to submit?');" OnClick="btnWebServiceCall_Click"/>
                    </ContentTemplate>
                </asp:UpdatePanel>
                <asp:UpdateProgress ID="evtMain" runat="server" DynamicLayout="true" DisplayAfter="1" AssociatedUpdatePanelID="invokeEndpointPanel" > 
                <ProgressTemplate>    
                <div align="left">
                    <div style="background-color:#E9E9E9;padding:6px;border:solid 1px black;width:90%;text-align:center;">
                        <span><i><b>Processing please wait...</b></i>
                        <asp:Image ID="img" runat="server" ImageUrl="~/Images/loading.gif" ImageAlign="AbsMiddle" />
                        </span>
                    </div>
                </div>
                </ProgressTemplate>
                </asp:UpdateProgress>        
            </td>
        </tr>

        <tr>
            <td width="5%">
            Request:
            </td>
            <td align="left" width="40%">
               <asp:TextBox ID="inpRequest" ForeColor="#5D7B9D" runat="server" BorderStyle="Groove" Wrap="True" TextMode="MultiLine" Rows="15" Columns="100"></asp:TextBox>
    	    </td>
        </tr>
      
            <tr>
                <td width="5%">
                    Response: &nbsp;
                </td>
                <td align="left" width="40%">
                    <asp:UpdatePanel ID="updateResponse" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>  
                        <asp:TextBox ID="outResponse" ForeColor="#5D7B9D" runat="server" BorderStyle="Groove" Wrap="True" TextMode="MultiLine" Rows="15" Columns="100" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="submit" EventName="Click" />
                    </Triggers>
                    </asp:UpdatePanel>
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
                        <b>Service</b> <br />
                        The Service specifies the type of operation you would want to invoke.<br />
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
                        <b>URL</b> <br />
                        The Service URL represents the end point to which the request shall be sent.<br />
                        The URLs will be <b>saved</b> by the system, when 'Invoke Endpoint' link is clicked.
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
                        <b>Request</b> <br />
                        Specify the XML request to be sent to the service invoked. Verfiy that the URL in the request is correct.<br />
                        The Request XML will be <b>saved</b> by the system, when 'Invoke Endpoint' link is clicked.
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
                        <b>Response</b> <br />
                        This text area captures the response returned from the invoked service. <br />
		    </div>
		    </div>
		    </div>
		    <!--- Shady code goes here -->
   	        </td>
       </tr>             
       </table>
</asp:Content>