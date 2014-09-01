<%@ Page Language="C#" MasterPageFile="~/template.master" AutoEventWireup="true" CodeFile="AAS.aspx.cs" Inherits="AgentSettings" 
EnableViewState="false" %>

<%@ Register TagPrefix="bcc" TagName="EditBox" Src="~/Controls/EditBox.ascx" %>
<%@ Reference Control="~/Controls/EditBox.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="leftContentPlaceHolder" Runat="Server">

<script type="text/javascript">

    /***********************************************
    * Local Time script- © Dynamic Drive (http://www.dynamicdrive.com)
    * This notice MUST stay intact for legal use
    * Visit http://www.dynamicdrive.com/ for this script and 100s more.
    ***********************************************/

    var weekdaystxt = ["Sun", "Mon", "Tues", "Wed", "Thurs", "Fri", "Sat"]

    function showLocalTime(container, servermode, offsetMinutes, displayversion) {
        if (!document.getElementById || !document.getElementById(container)) return
        this.container = document.getElementById(container)
        this.displayversion = displayversion
        var servertimestring = (servermode == "server-php") ? '<? print date("F d, Y H:i:s", time())?>' : (servermode == "server-ssi") ? '<!--#config timefmt="%B %d, %Y %H:%M:%S"--><!--#echo var="DATE_LOCAL" -->' : '<%= System.DateTime.UtcNow %>'
        this.localtime = this.serverdate = new Date(servertimestring)
        this.localtime.setTime(this.serverdate.getTime() + offsetMinutes * 60 * 1000) //add user offset to server time
        this.updateTime()
        this.updateContainer()
    }

    showLocalTime.prototype.updateTime = function () {
        var thisobj = this
        this.localtime.setSeconds(this.localtime.getSeconds() + 1)
        setTimeout(function () { thisobj.updateTime() }, 1000) //update time every second
    }

    showLocalTime.prototype.updateContainer = function () {
        var thisobj = this
        if (this.displayversion == "long")
            this.container.innerHTML = this.localtime.toLocaleString()
        else {
            var hour = this.localtime.getHours()
            var minutes = this.localtime.getMinutes()
            var seconds = this.localtime.getSeconds()
            var ampm = (hour >= 12) ? "PM" : "AM"
            var dayofweek = weekdaystxt[this.localtime.getDay()]
            this.container.innerHTML = hour + ":" + formatField(minutes) + ":" + formatField(seconds) + " " + " (" + dayofweek + ")"
            //this.container.innerHTML = formatField(hour, 1) + ":" + formatField(minutes) + ":" + formatField(seconds) + " " + ampm + " (" + dayofweek + ")"
        }
        setTimeout(function () { thisobj.updateContainer() }, 1000) //update container every second
    }

    function formatField(num, isHour) {
        if (typeof isHour != "undefined") { //if this is the hour field
            var hour = (num > 12) ? num - 12 : num
            return (hour == 0) ? 12 : hour
        }
        return (num <= 9) ? "0" + num : num//if this is minute or sec field
    }

</script>

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

      <table border="0"  width="95%" cellpadding="0" cellspacing="0" align="left">
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
                    SelectCommand="SELECT 'Recent activity' as Comment, [Message] FROM aspnet_WebEvent_Events WHERE EventTime = (select MAX(EventTime) FROM aspnet_WebEvent_Events WHERE EventType = 'BCC.Core.BCCWebAuditEvent' and EventCode = 100604)">
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
       <tr>
            <td align="center" valign="baseline">
            <!-- -->
            <asp:Panel ID="AgentPanel" runat="server" BorderStyle="Solid" BorderColor="#525252" BorderWidth="1" >
                <table border="0" bordercolor="#C0C0C0" width="75%" cellpadding="0" cellspacing="0" align="center">        
                <tr><td>&nbsp;</td></tr>
                <tr>
                    <td> 
                        <table border="1" bgcolor="#E9E9E9" bordercolor="silver" cellpadding="0" cellspacing="0" width="100%" align="left">
                            <tr align="left">
                                <td align="left" valign="bottom">Select&nbsp;:&nbsp;
                                    <asp:LinkButton ID="btnSelectAll" runat="server" style="color:maroon;text-decoration:none;" CausesValidation="false" OnClick="btnSelectAll_Click" Text="All" />,&nbsp;
                                    <asp:LinkButton ID="btnDeselectAll" runat="server" style="color:maroon;text-decoration:none;" CausesValidation="false" OnClick="btnDeSelectAll_Click" Text="None" />,&nbsp;
			                        <asp:LinkButton ID="btnStart" runat="server" style="color:green;text-decoration:none;" CausesValidation="false" OnClientClick="return confirm('Are you sure you want to start?');" OnClick="btnStart_Click" Text="Start" />,&nbsp;
                                    <asp:LinkButton ID="btnStop" runat="server" style="text-decoration:none;" CausesValidation="false" OnClientClick="return confirm('Are you sure you want to stop?');" OnClick="btnStop_Click" Text="Stop" />
                                </td>
                            </tr>
                        </table>

                    </td>
                </tr>
          
                <tr>
                    <td>
		                <table border="1" bordercolor="#C0C0C0" width="100%" cellpadding="0" cellspacing="0" align="center">
                        <tr colspan=2><td>
                                    <asp:GridView ID="gridBCCAgent" runat="server" AutoGenerateColumns="False" CellPadding="4" 
                                    EmptyDataText="Unable to locate BizTalk Control Center(BCC) - Agent Service"
                                        ForeColor="white" GridLines="Both" Width="100%" OnRowDataBound="gridBCCAgent_RowDataBound">
                                        <Columns>
                                            <asp:TemplateField>
                                                <ItemTemplate>
                                                    <asp:CheckBox ID="chkBoxService" runat="server" />
                                                </ItemTemplate>
                                                 <ItemStyle HorizontalAlign="Left" Width="2%" Font-Italic="false" />
                                            </asp:TemplateField>
                                            <asp:BoundField DataField="ServiceName" HeaderText="BCC Agent Windows Service">
                                                <ItemStyle HorizontalAlign="Left" Width="50%" Font-Italic="false" Wrap="true"/>
                                            </asp:BoundField>
                                            <asp:TemplateField HeaderText="">
                                               <ItemTemplate>
                                                    <bcc:AlertIndicator ID="alertStatus" Status='<%# Bind("ServiceStatus") %>' runat="server" />
                                               </ItemTemplate>
                                               <ItemStyle HorizontalAlign="Left" Width="3%" Font-Italic="false" />
                                            </asp:TemplateField>                                           
                                            <asp:BoundField DataField="ServiceStatus" HeaderText="Service Status" >
                                                <ItemStyle HorizontalAlign="Center" Width="30%" Font-Italic="false" Wrap="true"/>
                                            </asp:BoundField>
                                            </Columns>
                                            </asp:GridView>
                          </td></tr>
		                  </table>
                    </td>
                </tr>
                
                <tr>
                    <td>
                        <table border="1" bgcolor="#E9E9E9" bordercolor="#C0C0C0" cellpadding="0" cellspacing="0" width="100%" align="left">
                            <tr align="left">
                                <td align="left" valign="bottom">Select&nbsp;:&nbsp;
                                    <asp:LinkButton ID="btnSelectAll2" runat="server" style="color:maroon;text-decoration:none;" CausesValidation="false" OnClick="btnSelectAll_Click" Text="All" />,&nbsp;
                                    <asp:LinkButton ID="btnDeselectAll2" runat="server" style="color:maroon;text-decoration:none;" CausesValidation="false" OnClick="btnDeSelectAll_Click" Text="None" />,&nbsp;
			                        <asp:LinkButton ID="btnStart2" runat="server" style="color:green;text-decoration:none;" CausesValidation="false" OnClientClick="return confirm('Are you sure you want to start?');" OnClick="btnStart_Click" Text="Start" />,&nbsp;
                                    <asp:LinkButton ID="btnStop2" runat="server" style="text-decoration:none;" CausesValidation="false" OnClientClick="return confirm('Are you sure you want to stop?');" OnClick="btnStop_Click" Text="Stop" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
        
                <tr><td>&nbsp;</td></tr>

                <tr>
                    <td align="left">
                        <div align="left" style="border-style:groove;border-width:thin;border-color:#525252;background-color:#525252">
                            <asp:Label ID="Label5" Runat="server" Text="BCC Agent - Configuration settings" Width="100%" SkinID="PageHeader" />
                            <asp:PlaceHolder ID="phEmailConfig" runat="server" Visible="true" />
                        </div>
                    </td>
                </tr>
                <tr><td>&nbsp;</td></tr>                
                <tr>
                    <td align="center">
                        <asp:LinkButton BackColor="#525252" Font-Bold="true" ForeColor="White" BorderStyle="Outset" BorderWidth="2" ID="lnkSaveEmailProp" runat="server" style="text-decoration:none;" OnClientClick="return confirm('Are you sure you want to update?');" OnClick="SaveProperties_Click" Text="&nbsp;Update Agent Settings&nbsp;" />
                    </td>
                </tr>
                <tr><td>&nbsp;</td></tr>                
                </table>            
            </asp:Panel>
            <!-- -->
            </td>
        </tr>
        <tr>
            <td>&nbsp;</td>
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
		           <b>BizTalk Control Center (BCC) Agent </b><br /> 
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
		           <b>Agent Email Settings (Gmail account)</b><br />  <br />
                   <b>Web Email</b>: True <br />
		           <b>Email Host</b>: smtp.gmail.com <br /> 
                   <b>Email Port</b>: 587 <br /> 
                   <b>SSL Enabled</b>: True <br /> <br /> 
		           <b>Agent Email Settings (Domain account)</b><br />  <br />
                   <b>Web Email</b>: False <br />
		           <b>Email Host</b>: your.email.host <br /> 
                   <b>Email Port</b>: 25 <br /> 
                   <b>SSL Enabled</b>: False <br /> <br /> 
		           <b>Purge BCC data - default values</b><br />  <br />
		           <b>Keep performance data</b>: 7 days <br /> 
                   <b>Keep user notification data</b>: 30 days <br /> 
                   <b>Keep user activity data</b>: 30 days <br /> 
                   <b>UTC time</b>: <span id="timecontainer" style="background-color:Gray;color:White;font-weight:bold;" ></span> (24-hour) <br /> 
                    <script type="text/javascript">
                       new showLocalTime("timecontainer", "server-asp", 0, "short")
                    </script>
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
                        SelectCommand="SELECT [Message] FROM aspnet_WebEvent_Events WHERE EventType = 'BCC.Core.BCCWebAuditEvent' and EventCode = 100604 ORDER BY EventTime DESC">
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

