<%@ Page Language="C#" AutoEventWireup="true" CodeFile="DSOV-R.aspx.cs" Inherits="OrchDocView" %>

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="header" runat="server">
    <link href="./css/template.css" type="text/css" rel="stylesheet" />
    <script src="VwdCmsSplitterBar.js" type="text/javascript"></script>
</head>
<body bgcolor="#E9E9E9">
<form id="reportForm" runat="server">

<table border="0" bgcolor="#525252" width="100%" cellpadding="0" cellspacing="0" align="center">
<tr>
    <td align="left">
        &nbsp;
        <asp:Label ID="OdxViewHeader" runat="server" Visible="true" Width="100%" ForeColor="Gold" Font-Bold="true" Font-Names="Verdana" Font-Size="10pt"></asp:Label>
    </td>
</tr>
</table>

<div style="margin:0px;padding:0px;width:100%;overflow:hidden;"> 
	<table border="0" cellpadding="0" cellspacing="0" style="width:100%;height:100%;border:solid 3px silver;" bgcolor="white">
		<tr>
			<td runat="server" id="tdRef1" align="left" valign="top"> 
				<div runat="server" id="divRef1" style="width:100%;height:750px;overflow:auto;padding:0px;margin:0px;">
                    <asp:PlaceHolder ID="leftSideView" runat="server" Visible="true" />
				</div>			
			</td>	
			<td id="tdMid1" style="height:200px;width:6px;"></td>
			<td id="tdEdit1" align="left" valign="top" style="padding:0px 0px 0px 0px;">
                <div runat="server" id="divRight" style="width:100%;height:750px;overflow:auto;padding:3px;margin:0px;"> 
                    <asp:PlaceHolder ID="rightSideView" runat="server" Visible="true" />
                </div>
       		</td>
		</tr>
	</table>
</div>

<bcc:SplitterBar runat="server" ID="vsbSplitter" 
	LeftResizeTargets="tdRef1;divRef1"
	MinWidth="250" 
	BackgroundColor="silver" 
	BackgroundColorLimit="firebrick"
	BackgroundColorHilite="steelblue"
	BackgroundColorResizing="teal"
    DynamicResizing="true"
	style="background-image:url(vsplitter.gif);
		background-position:center center;
		background-repeat:no-repeat;           
		border-left:solid 1px black;
        border-right:solid 1px black;">
</bcc:SplitterBar>

</form>
</body>
</html>


