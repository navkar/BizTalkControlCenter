<%@ Page Language="C#" AutoEventWireup="true" CodeFile="BMUN-R.aspx.cs" Inherits="UserNotificationReport" %>

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="header" runat="server">
    <link href="./css/template.css" type="text/css" rel="stylesheet" />
    <script language="javascript" type="text/javascript">
    </script>
</head>
<body bgcolor="#E9E9E9">
<form id="reportForm" runat="server">

<table border="0" bgcolor="#525252" width="100%" cellpadding="0" cellspacing="0" align="center">
<tr>
    <td align="center">
        <asp:Label ID="chartHeader" runat="server" Visible="true" Width="100%" ForeColor="Gold" Font-Bold="true" Font-Names="Verdana" Font-Size="10pt"></asp:Label>
    </td>
</tr>
<tr>
    <td align="center">
			<asp:CHART id="artifactReport" runat="server" OnPostPaint="PostPaint_EmptyChart" Width="590px" Height="390px" BackColor="#FFFFCC" Palette="EarthTones" BorderDashStyle="Solid" BackGradientStyle="None" BorderWidth="2" BorderColor="26, 59, 105">
				<legends>
					<asp:Legend TitleFont="Verdana, 8pt, style=Bold" BackColor="Transparent" Font="Verdana, 8pt, style=Bold" IsTextAutoFit="False" Enabled="True" Name="Default"></asp:Legend>
				</legends>							
				<borderskin SkinStyle="None"></borderskin>
				<series>
				</series>
				<chartareas>
					<asp:ChartArea Name="ChartArea1" BorderColor="64, 64, 64, 64" BorderDashStyle="Solid" BackSecondaryColor="Transparent" BackColor="#FFFFCC" ShadowColor="Transparent" >
						<position Y="3" Height="92" Width="92" X="2"></position>
						<axisy LineColor="64, 64, 64, 64" LabelAutoFitMaxFontSize="8">
							<LabelStyle Font="Verdana, 8pt, style=Bold" />
							<MajorGrid LineColor="64, 64, 64, 64" />
						</axisy>
						<axisx LineColor="64, 64, 64, 64" LabelAutoFitMaxFontSize="8">
							<LabelStyle Font="Verdana, 8pt, style=Bold" />
							<MajorGrid LineColor="64, 64, 64, 64" />
                            
						</axisx>
					</asp:ChartArea>
				</chartareas>
			</asp:CHART>
    </td>
</tr>
<tr>
    <td align="center">
        <asp:Label ID="chartFooter" runat="server" Visible="true" Width="100%" ForeColor="Gold" Font-Bold="true" Font-Names="Verdana" Font-Size="10pt" Text="Daily view"></asp:Label>
    </td>
</tr>
<tr>
    <td align="center">
        <asp:RadioButtonList ID="frequency" runat="server" RepeatDirection="Horizontal" ForeColor="Gold" Font-Bold="true" BackColor="#525252" 
            OnSelectedIndexChanged="frequency_OnSelectedIndexChanged" AutoPostBack="True" Font-Names="Verdana" Font-Size="10pt"  
            BorderStyle="None">
            <asp:ListItem Value="DAILY" Selected="True">Daily</asp:ListItem>
            <asp:ListItem Value="WEEKLY">Weekly</asp:ListItem>
            <asp:ListItem Value="MONTHLY">Monthly</asp:ListItem>
        </asp:RadioButtonList>
    </td>
</tr>
</table>
</form>
</body>
</html>


