<%@ Page Language="C#" AutoEventWireup="true" CodeFile="BMMP-R.aspx.cs" Inherits="PerformanceCounterReport" EnableEventValidation="false" %>

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="header" runat="server">
    <link href="./css/template.css" type="text/css" rel="stylesheet" />
    <script language="javascript" type="text/javascript">
    </script>
</head>
<body bgcolor="#E9E9E9">
<form id="reportForm" runat="server">

<asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>

<table border="0" bgcolor="#525252" width="100%" cellpadding="0" cellspacing="0" align="center">
<tr>
    <td align="center">
        <asp:Label ID="chartHeader" runat="server" Visible="true" Width="100%" ForeColor="Gold" Font-Bold="true" Font-Names="Verdana" Font-Size="10pt"></asp:Label>
    </td>
</tr>
<tr>
    <td align="center">
          <asp:UpdatePanel ID="chartUpdatePanel" runat="server">
          <ContentTemplate>
			<asp:CHART id="chartPerfCounter" runat="server" OnPostPaint="PostPaint_EmptyChart" Width="750px" Height="390px" BackColor="#FFFFCC" Palette="EarthTones" BorderDashStyle="Solid" BackGradientStyle="None" BorderWidth="2" BorderColor="26, 59, 105">
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
            <asp:Timer ID="chartTimer" runat="server" ontick="ChartTimer_Tick" Enabled="true">
            </asp:Timer>
        </ContentTemplate>
        </asp:UpdatePanel>
    </td>
</tr>
<tr>
    <td align="center">
        <asp:Label ID="chartFooter" runat="server" Visible="true" Width="100%" ForeColor="Gold" Font-Bold="true" Font-Names="Verdana" Font-Size="10pt" Text="Data Points"></asp:Label>
    </td>
</tr>
<tr>
    <td align="center">
        <asp:RadioButtonList ID="dataPoints" runat="server" RepeatDirection="Horizontal" ForeColor="Gold" Font-Bold="true" BackColor="#525252" 
            OnSelectedIndexChanged="dataPoints_OnSelectedIndexChanged" AutoPostBack="True" Font-Names="Verdana" Font-Size="10pt"  
            BorderStyle="None">
            <asp:ListItem Value="50" Selected="True">50</asp:ListItem>
            <asp:ListItem Value="100">100</asp:ListItem>
            <asp:ListItem Value="250">250</asp:ListItem>
            <asp:ListItem Value="500">500</asp:ListItem>
            <asp:ListItem Value="1000">1000</asp:ListItem>
        </asp:RadioButtonList>
    </td>
</tr>
</table>
</form>
</body>
</html>


