<%@ Page Language="C#" AutoEventWireup="true" CodeFile="KCTL-E.aspx.cs" Inherits="TaskEfforts" EnableEventValidation="false" Theme="Day" %>

<%@ Register assembly="RJS.Web.WebControl.PopCalendar.Net.2008" namespace="RJS.Web.WebControl" tagprefix="rjs" %>

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="header" runat="server">
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
    <td align="center">
        <asp:Label ID="headerCaption" runat="server" Width="100%" Text="Task efforts" SkinID="PageHeader"></asp:Label>
    </td>
</tr>
<tr>
    <td align="center">
          <asp:UpdatePanel ID="taskEffortPanel" runaat="server">
          <ContentTemplate>
  		                <table border="1" bordercolor="#525252" width="100%" cellpadding="0" cellspacing="0" align="left">
                            <thead align="left" >
                                <asp:Label ID="lblTaskEffortCaption" runat="server" SkinID="PageHeader" Width="100%" />
                            </thead>
                	        <tr>
                	            <td>   
                                        <asp:GridView ID="taskEffortView" AutoGenerateColumns ="False" runat="server" DataKeyNames="taskID" PageSize="10" 
                                            AllowPaging="True" ShowFooter="true" Width="100%" AllowSorting="true"
                                            OnSorting="taskEffortView_Sorting"
                                            OnRowDataBound="taskEffortView_RowDataBound"
                                            OnRowEditing="taskEffortView_RowEditing"   
                                            OnRowCancelingEdit="taskEffortView_RowCancelEditing"
                                            OnRowUpdating="taskEffortView_RowUpdating"
                                            OnRowDeleting="taskEffortView_RowDeleting"
                                            OnRowCommand="taskEffortView_RowCommand"                                            
                                            OnPageIndexChanging="taskEffortView_PageIndexChanging">
                                        <Columns>
                                            <asp:TemplateField HeaderText="User" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Center" ControlStyle-Font-Bold="false" SortExpression="taskRef">
                                            <ItemTemplate>
                                                    <asp:HiddenField ID="taskID" runat="server" Value='<%# DataBinder.Eval(Container.DataItem, "taskID")%>' />
                                                    <asp:HiddenField ID="taskRef" runat="server" Value='<%# DataBinder.Eval(Container.DataItem, "taskRef")%>' />
                                                    <asp:Label ID="lblAssignedToUserName" style="font-weight:bold;display:block;width:75px;text-decoration:none;" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "AssignedToUserName")%>' ToolTip='<%# DataBinder.Eval(Container.DataItem, "taskRef")%>' ></asp:Label>
                                            </ItemTemplate>
                                            <FooterTemplate>
                                                    <asp:Label ID="lblFooterUserName" style="font-weight:bold;display:block;width:75px;text-decoration:none;" runat="server" Text='<%# CurrentUser %>'></asp:Label>
                                            </FooterTemplate>
                                            <ItemStyle Width="15%" Wrap="false" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Date" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" ControlStyle-Font-Bold="false" SortExpression="taskDate">
                                                <ItemTemplate>
                                                        <asp:Label ID="lblTaskDate" style="font-weight:normal;display:block;width:75px;" runat="server" Tooltip='<%# Bind("taskDate") %>' Text='<%# Bind("taskDate","{0:MMM-dd-yy}") %>'></asp:Label>
                                                </ItemTemplate>
                                                <ItemStyle Width="15%" Wrap="true"/>
                                                <EditItemTemplate>
                                                    <asp:TextBox ID="tTaskDate" MaxLength="11" Width="50%" runat="Server" Text='<%# Bind("taskDate","{0:MMM-dd-yyyy}") %>' BorderStyle="Groove" ></asp:TextBox>
                                                    <rjs:PopCalendar Format="mmm dd yyyy" RequiredDate="True" Culture="en-US English (United States)" ID="taskDateCalendar" runat="server" Control="tTaskDate" Separator="-" />
                                                </EditItemTemplate>
                                                <FooterTemplate>
                                                    <asp:TextBox ID="tNewTaskDate" MaxLength="11" Width="60%" runat="Server" Text='<%# TodaysDate %>' BorderStyle="Groove" ></asp:TextBox>
                                                    <rjs:PopCalendar Format="mmm dd yyyy" RequiredDate="True" Culture="en-US English (United States)" ID="newTaskDateCalendar" runat="server" Control="tNewTaskDate" Separator="-" />
                                                </FooterTemplate>
                                                <FooterStyle BackColor="#C0C0C0" HorizontalAlign="Left" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Effort" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" ControlStyle-Font-Bold="false" SortExpression="taskDuration">
                                                <ItemTemplate>
                                                        <asp:Label ID="lblTaskHours" runat="server" Text='<%# Bind("taskDuration", "{0} hrs") %>'></asp:Label>
                                                </ItemTemplate>
                                                <ItemStyle Width="15%" Wrap="false"/>
                                                <EditItemTemplate>
                                                    <asp:TextBox ID="tTaskHours" MaxLength="3" Width="50%" CausesValidation="true" runat="Server" Text='<%# Bind("taskDuration") %>' BorderStyle="Groove" ></asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="rfv" runat="server" ToolTip="Required!" Text="*" ControlToValidate="tTaskHours"></asp:RequiredFieldValidator>
                                                </EditItemTemplate>
                                                <FooterTemplate>
                                                    <asp:TextBox ID="tNewTaskHours" MaxLength="3" Width="70%" CausesValidation="true" runat="Server" BorderStyle="Groove" Text="4.0" ></asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="rfv2" runat="server" ToolTip="Required!" Text="*" ControlToValidate="tNewTaskHours"></asp:RequiredFieldValidator>
                                                </FooterTemplate>
                                                <FooterStyle BackColor="#C0C0C0" HorizontalAlign="Left" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Status" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" ControlStyle-Font-Bold="false" SortExpression="TaskStatus">
                                                <ItemTemplate>
                                                        <asp:Label ID="lblTaskStatus" style="font-weight:normal;display:block;width:75px;" runat="server" Text='<%# Bind("taskStatus") %>'></asp:Label>
                                                </ItemTemplate>
                                                <ItemStyle Width="10%" Wrap="true"/>
                                                <EditItemTemplate>
                                                    <asp:DropDownList ID="ddlTaskStatus" runat="server" AppendDataBoundItems="false" DataTextField="taskStatus" DataValueField="taskStatus" DataSource="<%# TaskStatusList %>" SelectedValue='<%# Eval("taskStatus")%>'>
                                                    </asp:DropDownList>
                                                </EditItemTemplate>
                                                <FooterTemplate>
                                                    <asp:DropDownList ID="ddlNewTaskStatus" runat="server" AppendDataBoundItems="false" DataTextField="taskStatus" DataValueField="taskStatus" DataSource="<%# TaskStatusList %>">
                                                    </asp:DropDownList>
                                                </FooterTemplate>
                                                <FooterStyle BackColor="#C0C0C0" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Remark" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Center" ControlStyle-Font-Bold="false" SortExpression="taskRemark">
                                                <ItemTemplate>
                                                        <asp:Label ID="lblTaskRemark" runat="server" Text='<%# Bind("taskRemark") %>'></asp:Label>
                                                </ItemTemplate>
                                                 <EditItemTemplate>
                                                        <asp:TextBox ID="tTaskRemark" MaxLength="100" Width="90%" Text='<%# Bind("taskRemark") %>' CausesValidation="false" runat="Server" BorderStyle="Groove" ></asp:TextBox>
                                                </EditItemTemplate>
                                                <FooterTemplate>
                                                        <asp:TextBox ID="tNewTaskRemark" MaxLength="100" Width="95%" CausesValidation="false" runat="Server" BorderStyle="Groove" ></asp:TextBox>
                                                </FooterTemplate>
                                                <FooterStyle BackColor="#C0C0C0" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Select" HeaderStyle-HorizontalAlign="Left"> 
                                                <EditItemTemplate> 
                                                    <asp:LinkButton ID="lnkUpdate" runat="server" CausesValidation="True" CommandName="Update" >
                                                        <asp:Image ID="Image1" runat="server" ImageUrl="Images/ok.png" ToolTip="Update" />
                                                    </asp:LinkButton> 
                                                    <asp:LinkButton ID="lnkCancel" runat="server" CausesValidation="False" CommandName="Cancel">
                                                        <asp:Image ID="Image2" runat="server" ImageUrl="Images/cancel.png" ToolTip="Cancel" />
                                                    </asp:LinkButton> 
                                                </EditItemTemplate> 
                                                <ItemTemplate> 
                                                    <asp:LinkButton ID="lnkEdit" runat="server" CausesValidation="False" CommandName="Edit" >
                                                            <asp:Image ID="editImage" runat="server" ImageUrl="Images/edit.png" ToolTip="Edit" />
                                                    </asp:LinkButton>
                                                    <asp:LinkButton ID="lnkDelete" runat="server" CausesValidation="False" CommandName="Delete" OnClientClick="javascript:return confirm('Are you sure you want to delete the entry?');" >
                                                            <asp:Image ID="deleteImage" runat="server" ImageUrl="Images/delete.png" ToolTip="Delete" />
                                                    </asp:LinkButton>
                                                </ItemTemplate> 
                                                <ItemStyle Width="2%" Wrap="false"/>     
                                                <FooterTemplate>
                                                    <asp:LinkButton ID="lnkAdd2" runat="server" CausesValidation="True" CommandName="Add">
                                                            <asp:Image ID="addImage" runat="server" ImageUrl="Images/add.png" ToolTip="Add" />
                                                    </asp:LinkButton>                                                  
                                                </FooterTemplate>                                   
                                            </asp:TemplateField>    
                                        </Columns>
                                        <EmptyDataTemplate>
                                            <table border="1" cellpadding="2" cellspacing="0" width="100%" bgcolor="#FFFFCC">
                                                <thead>
                                                    <tr align="center" bgcolor="#525252">
                                                        <td> <asp:Label skinID="PageHeader" ID="Label6" Text="Date" runat="server"/></td>
                                                        <td> <asp:Label skinID="PageHeader" ID="Label7" Text="Effort" runat="server"/></td>
                                                        <td>  <asp:Label skinID="PageHeader" ID="Label8" Text="Status" runat="server"/></td>
                                                        <td>  <asp:Label skinID="PageHeader" ID="Label9" Text="Remark" runat="server"/></td>
                                                        <td>  <asp:Label skinID="PageHeader" ID="Label10" Text="Add new" runat="server"/></td>
                                                    </tr>
                                                </thead>
                                                <tr>
                                                    <td width="10%">
                                                        <asp:HiddenField ID="taskID" runat="server" Value='<%# DataBinder.Eval(Container.DataItem, "taskID")%>' />
                                                        <asp:TextBox ID="tDate" Width="75%" runat="server" Text="<%# TodaysDate %>" BorderStyle="Groove">
                                                        </asp:TextBox>
                                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ToolTip="Required!" Text="*" ControlToValidate="tDate">
                                                        </asp:RequiredFieldValidator>
                                                    </td>
                                                    <td width="10%">
                                                        <asp:TextBox ID="tEffort" Width="75%" runat="server" MaxLength="3" Text="4.0" BorderStyle="Groove">
                                                        </asp:TextBox>
                                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ToolTip="Required!" Text="*" ControlToValidate="tEffort">
                                                        </asp:RequiredFieldValidator>
                                                    </td>
                                                    <td>
                                                        <asp:DropDownList ID="ddlTaskStatus2" Width="90%" runat="server" DataTextField="taskStatus" DataValueField="taskStatus" DataSource="<%# TaskStatusList %>">
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td width="50%">
                                                        <asp:TextBox ID="tRemark" Width="90%" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "taskID")%>' BorderStyle="Groove">
                                                        </asp:TextBox>
                                                    </td>
                                                    <td align="center">
                                                        <asp:LinkButton ID="LinkButton1" runat="server" CausesValidation="True" CommandName="AddOnEmpty">
                                                                <asp:Image ID="Image6" runat="server" ImageUrl="images/add.png" ToolTip="Add" />
                                                        </asp:LinkButton> 
                                                    </td>
                                                </tr>
                                            </table>
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


