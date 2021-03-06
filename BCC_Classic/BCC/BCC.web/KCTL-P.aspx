<%@ Page Language="C#" AutoEventWireup="true" CodeFile="KCTL-P.aspx.cs" Inherits="ProjectTasksReport" EnableEventValidation="false" Theme="Day" %>
<%@ Register assembly="RJS.Web.WebControl.PopCalendar.Net.2008" namespace="RJS.Web.WebControl" tagprefix="rjs" %>
<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="header" runat="server">
    <title>BizTalk Control Center (BCC) - Project Report</title>
    <link rel="stylesheet" type="text/css" href="./css/template.css" />
    <script language="javascript" type="text/javascript">

        function HighlightON(obj) {
            if (obj != null) {
                obj.originalClassName = obj.className;
                obj.className = obj.className + 'HL';
            }
        }

        function HighlightOFF(obj) {
            if (obj != null) {
                obj.className = obj.originalClassName;
            }
        }

    </script>
</head>
<body bgcolor="#E9E9E9">
<form id="reportForm" runat="server">

<asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>

<table border="0" bgcolor="#525252" width="100%" cellpadding="0" cellspacing="0" align="center">
<tr>
    <td align="left">
        <asp:Label ID="headerCaption" runat="server" Width="100%" Text="Project Task Report" SkinID="PageHeader"></asp:Label>
    </td>
    <td align="right">
        <asp:LinkButton ID="btnExportToExcel" runat="server" style="color:teal;text-decoration:none;" OnClick="btnExportToExcel_Click" >
            <asp:Image ID="excel" runat="server" ImageAlign="Middle" ImageUrl="~/Images/Excel-16.gif" />
        </asp:LinkButton>    
    </td>
</tr>
<tr>
    <td align="center" colspan="2">
          <asp:UpdatePanel ID="taskEffortPanel" runat="server">
          <ContentTemplate>
  		                <table border="1" bordercolor="#525252" width="100%" cellpadding="0" cellspacing="0" align="left">
                            <tr bgcolor="#E9E9E9">
                                <td width="25%">
                                    <asp:Label ID="Label1" runat="server" Text="Start date:" Width="35%" />
                                    <asp:TextBox ID="tStartDate" Width="35%" MaxLength="11" runat="server" BorderStyle="Groove"  />
                                    <rjs:PopCalendar Format="mmm dd yyyy" RequiredDate="True" Culture="en-US English (United States)" ID="startDateCalendar" runat="server" Control="tStartDate" ShowToday="true" Separator="-" />      
                                </td>
                                <td width="25%">
                                    <asp:Label ID="Label2" runat="server" Text="End date:" Width="35%" />
                                    <asp:TextBox ID="tEndDate" Width="35%"  MaxLength="11" runat="server" BorderStyle="Groove"  />
                                    <rjs:PopCalendar Format="mmm dd yyyy" RequiredDate="True" Culture="en-US English (United States)" ID="endDateCalendar" runat="server" Control="tEndDate" ShowToday="true" BorderStyle="Solid" BorderColor="Black" BackColor="Yellow" Separator="-" />
                                </td>
                                <td>
                                    <asp:LinkButton ID="lnkViewReport" runat="server" cssclass="blockView" Text="Show Report" ToolTip="Default report is for past 30 days" CommandName="ViewReports" OnClick="lnkViewReport_Click"/>
                                </td>
                            </tr>
                	        <tr>
                	            <td colspan="3">   
                                        <asp:GridView ID="projectEffortView" AutoGenerateColumns ="False" runat="server" PageSize="10" 
                                            AllowPaging="True" ShowFooter="true" Width="100%" AllowSorting="true"
                                            OnSorting="taskEffortView_Sorting"
                                            OnRowDataBound="taskEffortView_RowDataBound"
                                            OnPageIndexChanging="taskEffortView_PageIndexChanging">
                                        <Columns>
                                            <asp:TemplateField HeaderText="User" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Center" ControlStyle-Font-Bold="false" SortExpression="userName">
                                            <ItemTemplate>
                                                    <asp:Label ID="lblAssignedToUserName" style="font-weight:bold;display:block;width:75px;text-decoration:none;" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "UserName")%>' ></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle Width="5%" Wrap="false" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Priority" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" ControlStyle-Font-Bold="false" SortExpression="taskPriority">
                                                <ItemTemplate>
                                                        <asp:Label ID="lblTaskPriority" style="font-weight:bold;display:block;width:50px;" runat="server" Text='<%# Bind("taskPriority") %>'></asp:Label>
                                                </ItemTemplate>
                                                <ItemStyle Width="3%" Wrap="false"/>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Task" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Center" ControlStyle-Font-Bold="false" SortExpression="taskRef">
                                                <ItemTemplate>
                                                        <asp:Label ID="lblTaskTitle" runat="server" Text='<%# FormatTaskName( DataBinder.Eval(Container.DataItem, "taskRef") , DataBinder.Eval(Container.DataItem, "task") ) %>'></asp:Label>
                                                </ItemTemplate>
                                                <ItemStyle Width="45%" Wrap="false"/>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Status" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" ControlStyle-Font-Bold="false" SortExpression="TaskStatus">
                                                <ItemTemplate>
                                                        <asp:Label ID="lblTaskStatus" style="font-weight:normal;display:block;width:75px;" runat="server" Text='<%# Bind("taskStatus") %>'></asp:Label>
                                                </ItemTemplate>
                                                <ItemStyle Width="5%" Wrap="true"/>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Hours" ItemStyle-HorizontalAlign="Right" HeaderStyle-HorizontalAlign="Center" ControlStyle-Font-Bold="false" SortExpression="hoursSpent">
                                                <ItemTemplate>
                                                        <asp:HiddenField ID="hTaskHours" runat="server" Value='<%# Bind("hoursSpent") %>' />
                                                        <asp:Label ID="lblTaskHours" runat="server" Text='<%# Bind("hoursSpent", "{0} hrs &nbsp;&nbsp;") %>'></asp:Label>
                                                </ItemTemplate>
                                                <ItemStyle Width="3%" Wrap="false"/>
                                                <FooterTemplate>
                                                        <asp:Label ID="lblEffortSum" runat="server" Width="100%"></asp:Label>
                                                </FooterTemplate>
                                                <FooterStyle Width="3%" BackColor="#C0C0C0" Wrap="false" />
                                            </asp:TemplateField>
                                        </Columns>
                                        <EmptyDataTemplate>
                                            <asp:Label ID="empty" runat="server" Text="No records found." Width="100%" />
                                        </EmptyDataTemplate>
                                        <EmptyDataRowStyle HorizontalAlign="Left" BackColor="#C0C0C0" />
                                        </asp:GridView>
                                </td>
                            </tr>
                        </table>                                                                             
        </ContentTemplate>
        </asp:UpdatePanel>
    </td>
</tr>
</table>
</form>
</body>
</html>


