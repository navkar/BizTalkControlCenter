<%@ Page Debug="True" Language="C#" AutoEventWireup="true" EnableTheming="true" CodeFile="BMMS-F.aspx.cs" Inherits="IFrame" %>
<head runat="server" id="hdr" />
<div style="background-color:#E9E9E9" >
<form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" />

        <asp:Timer ID="RefreshTimer" runat="server" Interval="5000">
        </asp:Timer>

        <asp:UpdatePanel ID="UpdatePanel1" UpdateMode="Conditional" runat="server">
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="RefreshTimer" EventName="Tick" />
            </Triggers>

            <ContentTemplate>
            <asp:Panel ID="chartPanel" runat="server" Visible="true">
		        <table bgcolor="#E9E9E9" width="100%" border="0" cellspacing="0" cellpadding="0">
		           <tr>
                   <td>
			            <img src="http://chart.apis.google.com/chart?chco=00FF00,FFFF00,FF0000&#038;chs=320x150&#038;chd=t:<%=dt.Rows.Count%>&#038;cht=gom&#038;chl=<%=dt.Rows.Count%>&#038;&chds=0,<%=UpperLimitValue%>&chtt=BizTalk+Message+Load&chf=bg,lg,0,E9E9E9,0,E9E9E9,1" alt="BizTalk message load meter" /> 
		           </td>
		           <td>             
		                <img src="http://chart.apis.google.com/chart?chs=320x250&cht=bvs&chd=t:<%=chartData%>&chds=0,<%=UpperLimitValue%>&chxt=y,x&chm=N**,000000,0,-1,11&chtt=Messages+(per+t+sec)&chxt=y,x&chxr=0,0,<%=UpperLimitValue%>|1,1,10&chf=bg,lg,0,E9E9E9,0,E9E9E9,1" alt="Messages activity chart" /> 
		           </td>
		           <td>
			            <img src="http://chart.apis.google.com/chart?cht=p3&chd=t:<%=strMsgCount%>&chs=320x100&chl=<%=strMsgType%>&chco=<%=strMsgTypeColour%>&chtt=Message+Distribution&chf=bg,lg,0,E9E9E9,0,E9E9E9,1" alt="Message distribution chart" />
		           </td>
		           </tr>
		        </table>
		    </asp:Panel>
		    <asp:Panel ID="errorPanel" runat="server" Visible="false">
		            <table border="0" width="100%" bgcolor="#CECECE" cellpadding="0" cellspacing="1" align="left">
                    <tr>
                        <td width="2%" valign="top">
                            <asp:Image ID="errorImg" runat="server" ImageUrl="~/Images/error.png" Visible="true" />
		                </td>
		                <td width="95%">
		                    <asp:Label id="lblError" runat="server" visible="true" width="100%" />
		                </td>
		            </tr>
                    </table>
		    </asp:Panel>
            </ContentTemplate>
        </asp:UpdatePanel>
        <div style="position:relative; left:0; top:0; width:100">
	        <asp:LinkButton style="font-size:7pt;color:silver;text-decoration:none;" runat="server" ID="lbRefresh" OnClick="ToggleRefresh" Text="Refresh off" Font-Names="Courier New" Font-Size="Small"></asp:LinkButton>
	    </div>
</form>
</div>	    

