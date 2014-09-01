<%@ Page Language="C#" MasterPageFile="~/template.master" AutoEventWireup="true" CodeFile="KCTL.aspx.cs" 
Inherits="TaskList" EnableViewState="true" EnableEventValidation="false" %>

<%@ Register assembly="RJS.Web.WebControl.PopCalendar.Net.2008" namespace="RJS.Web.WebControl" tagprefix="rjs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="leftContentPlaceHolder" Runat="Server">

<script language="javascript" type="text/javascript">
</script>


    <table border="0" bordercolor="#5D7B9D" width="99%" cellpadding="0" cellspacing="1" align="left">
        <tr>
            <td align="center">
		    <asp:Label ID="lblCaption" skinID="PageCaption" runat="server"></asp:Label>
   	        </td>
        </tr>
        <tr>
             <td width="100%" align="left" >
                <asp:UpdatePanel id="infoPanel" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <table border="0" width="100%" bgcolor="#CECECE" cellpadding="0" cellspacing="0" align="left">
                    <tr>
                        <td width="2%" valign="top">
                            <asp:Image ID="errorImg" runat="server" ToolTip="Error" ImageUrl="~/Images/task-error.png" Visible="false" />
                            <asp:Image ID="successImg" runat="server" ToolTip="Success" ImageUrl="~/Images/task-success.png" Visible="false" />
		                </td>
		                <td width="95%">
		                    <asp:Label ID="lblInfo" runat="server" Visible="false" Width="100%"></asp:Label>
		                </td>
		            </tr>
                    </table>
                </ContentTemplate>
                <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="taskView" EventName="RowUpdating" />
                        <asp:AsyncPostBackTrigger ControlID="taskView" EventName="RowCreated" />
                        <asp:AsyncPostBackTrigger ControlID="projectView" EventName="RowCreated" />
                        <asp:AsyncPostBackTrigger ControlID="taskView" EventName="RowDeleted" />
                </Triggers>                
                </asp:UpdatePanel>
	        </td>
        </tr> 
    	<tr>
        <td>
            <table border="1" style="width:100%;color:#333333;border-collapse:collapse;background-color:#FFFFCC">
            <tr>
                <td align="center">
                    <asp:LinkButton ID="lnkViewTasks" runat="server" cssclass="blockView" Text="Tasks" ToolTip="Displays all tasks assigned to all users" CommandName="ViewTasks" OnClick="btnView_Click"/>
                </td>
                <td align="center">
                    <asp:LinkButton ID="lnkViewProjects" runat="server" cssclass="blockView" Text="Projects" ToolTip="Displays all the projects" CommandName="ViewProjects" OnClick="btnView_Click"/>
                </td>
                <td align="center">
                    <asp:LinkButton ID="lnkViewEnvironments" runat="server" cssclass="blockView" Text="Environments" ToolTip="Display all the environments" CommandName="ViewEnvironments" OnClick="btnView_Click"/>                
                </td>
                <td align="center">
                    <asp:LinkButton ID="lnkArchivedTasks" runat="server" cssclass="blockView" Text="Archives" ToolTip="Display all the archived tasks" CommandName="ViewArchivedTasks" OnClick="btnView_Click"/>                
                </td>
                <td align="center">
                    <asp:LinkButton ID="lnkReports" runat="server" cssclass="blockView" Text="Reports" ToolTip="View Task Reports" CommandName="ViewReports" OnClick="btnView_Click"/>                
                </td>
                <td width="45%" align="right">
                          <asp:UpdateProgress runat="server" ID="PageUpdateProgress">
                                <ProgressTemplate>
                                      <img src="images/loading.gif" alt="Processing" />
                                </ProgressTemplate>
                          </asp:UpdateProgress>
                </td>
            </tr>
            </table>
        </td>
        </tr>
        <tr><td>&nbsp;</td></tr>
    	<tr>
            <td>
                   <asp:UpdatePanel id="multiViewPanel" runat="server">
                   <ContentTemplate>
                    <asp:MultiView runat="server" ID="tasksView" OnActiveViewChanged="tasksView_ActiveViewChanged" >
                        <asp:View ID="vTasks" runat="server">
                            <asp:DropDownList ID="ddlOptions" ToolTip="select your choice" runat="server" OnSelectedIndexChanged="ddlOptions_OnSelectedIndexChanged" AutoPostBack="true">
                                <asp:ListItem Text="My Tasks" Value="1"></asp:ListItem>
                                <asp:ListItem Text="All Tasks" Value="2"></asp:ListItem>
                                <asp:ListItem Text="Completed Tasks" Value="3"></asp:ListItem>
                            </asp:DropDownList>

                            <asp:GridView ID="taskView" runat="server" AutoGenerateEditButton="False"
                                        AutoGenerateColumns="False" CellPadding="0" 
                                        AllowPaging="true" PageSize="15" AllowSorting="true"
                                        GridLines="Both" ShowFooter="true" ShowHeader="true"
                                        OnSorting="taskView_OnSort"
                                        OnRowDataBound="taskView_RowDataBound"
                                        OnRowEditing="taskView_RowEditing"   
                                        OnRowCancelingEdit="taskView_RowCancelEditing"
                                        OnRowUpdating="taskView_RowUpdating"
                                        OnRowDeleting="taskView_RowDeleting"
                                        OnPageIndexChanging="taskView_PageIndexChanging" 
                                        OnRowCommand="taskView_RowCommand" >
                                        <Columns>
                                            <asp:TemplateField HeaderText="Priority" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" ControlStyle-Font-Bold="false" SortExpression="taskPriority">
                                                <ItemTemplate>
                                                        <asp:Label ID="lblTaskPriority" style="font-weight:bold;display:block;width:50px;" runat="server" Text='<%# Bind("taskPriority") %>'></asp:Label>
                                                </ItemTemplate>
                                                <EditItemTemplate>
                                                    <asp:DropDownList ID="ddlPriority" runat="server" SelectedValue='<%# Eval("taskPriority")%>' Width="100%">
                                                        <asp:ListItem Text="1" Value="1"></asp:ListItem>
                                                        <asp:ListItem Text="2" Value="2"></asp:ListItem>
                                                        <asp:ListItem Text="3" Value="3"></asp:ListItem>
                                                    </asp:DropDownList>
                                                </EditItemTemplate>
                                                <ItemStyle Width="3%" Wrap="false"/>
                                                <FooterStyle BackColor="#C0C0C0" HorizontalAlign="Left" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Task" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Center" ControlStyle-Font-Bold="false" SortExpression="taskRef">
                                                <ItemTemplate>
                                                        <asp:HiddenField ID="taskID" runat="server" Value='<%# DataBinder.Eval(Container.DataItem, "taskID")%>' />
                                                        <asp:HiddenField ID="taskRef" runat="server" Value='<%# DataBinder.Eval(Container.DataItem, "taskRef")%>' />
                                                        <asp:Label ID="lblTaskTitle" runat="server" Text='<%# FormatTaskName( DataBinder.Eval(Container.DataItem, "taskRef") , DataBinder.Eval(Container.DataItem, "taskTitle") ) %>'></asp:Label>
                                                </ItemTemplate>
                                                <EditItemTemplate>
                                                    <asp:HiddenField ID="taskID" runat="server" Value='<%# DataBinder.Eval(Container.DataItem, "taskID")%>' />
                                                    <asp:HiddenField ID="taskRef" runat="server" Value='<%# DataBinder.Eval(Container.DataItem, "taskRef")%>' />
                                                    <asp:TextBox ID="tTaskTitle" CausesValidation="true" Width="90%" runat="Server" Text='<%# Bind("taskTitle") %>' BorderStyle="Groove" ></asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="tv1" runat="server" ToolTip="Required!" Text="*" ControlToValidate="tTaskTitle"></asp:RequiredFieldValidator>
                                                </EditItemTemplate>
                                                <ItemStyle Width="30%" Wrap="false"/>
                                                <FooterTemplate>
                                                    <asp:TextBox BorderStyle="Groove" Width="90%" ID="tNewTaskTitle" runat="server" Text="Specify a new item"></asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="tv2" runat="server" ToolTip="Required!" Text="*" ControlToValidate="tNewTaskTitle"></asp:RequiredFieldValidator>
                                                </FooterTemplate>
                                                <FooterStyle BackColor="#C0C0C0" Width="15%" HorizontalAlign="Left" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Project" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Center" ControlStyle-Font-Bold="false" SortExpression="projectName">
                                                <ItemTemplate>
                                                        <asp:Label ID="lblProjectName" runat="server" Text='<%# Bind("projectName") %>'></asp:Label>
                                                </ItemTemplate>
                                                <EditItemTemplate>
                                                    <asp:DropDownList ID="ddlProjectName" Width="100%" runat="server" DataTextField="ProjectName" DataValueField="ProjectName" DataSource="<%# Projects %>" SelectedValue='<%# Eval("projectName")%>'>
                                                    </asp:DropDownList>
                                                </EditItemTemplate>
                                                <ItemStyle Width="5%" Wrap="false"/>
                                                <FooterTemplate>
                                                    <asp:DropDownList ID="ddlNewProjectName" Width="100%" runat="server" DataTextField="ProjectName" DataValueField="ProjectName" DataSource="<%# Projects %>">
                                                    </asp:DropDownList>
                                                </FooterTemplate>
                                                <FooterStyle BackColor="#C0C0C0" HorizontalAlign="Left" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Recurring" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" ControlStyle-Font-Bold="false" SortExpression="TaskType">
                                                <ItemTemplate>
                                                        <asp:Label ID="lblTaskType" style="font-weight:normal;display:block;width:90px;" runat="server" Text='<%# Bind("taskType") %>'></asp:Label>
                                                </ItemTemplate>
                                                <EditItemTemplate>
                                                    <asp:DropDownList ID="ddlTaskType" runat="server" AppendDataBoundItems="false" DataTextField="taskType" DataValueField="taskType" DataSource="<%# TaskTypeList %>" SelectedValue='<%# Eval("taskType")%>'>
                                                    </asp:DropDownList>
                                                </EditItemTemplate>
                                                <ItemStyle Width="5%" Wrap="true"/>
                                                <FooterTemplate>
                                                    <asp:DropDownList ID="ddlNewTaskType" Width="100%" runat="server" AppendDataBoundItems="false" DataTextField="taskType" DataValueField="taskType" DataSource="<%# TaskTypeList %>" >
                                                    </asp:DropDownList>
                                                </FooterTemplate>
                                                <FooterStyle BackColor="#C0C0C0" HorizontalAlign="Left" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Due On" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" ControlStyle-Font-Bold="false" SortExpression="taskDueDate">
                                                <ItemTemplate>
                                                        <asp:Label ID="lblTaskDueDate" style="font-weight:normal;display:block;width:75px;" runat="server" Text='<%# Bind("taskDueDate","{0:MMM-dd-yy}") %>'></asp:Label>
                                                </ItemTemplate>
                                                <EditItemTemplate>
                                                    <asp:TextBox style="font-weight:normal;width:35px;" ID="tTaskDueDate" MaxLength="8" Width="60%" CausesValidation="true" runat="Server" Text='<%# Bind("taskDueDate","{0:MM/dd/yy}") %>' BorderStyle="Groove" ></asp:TextBox>
                                                    <asp:CompareValidator ID="dd1" runat="server" Operator="DataTypeCheck" Type="Date" ControlToValidate="tTaskDueDate" Text="*"></asp:CompareValidator>
                                                    <asp:RequiredFieldValidator ID="dd2" runat="server" ToolTip="Required!" Text="*"  ControlToValidate="tTaskDueDate"></asp:RequiredFieldValidator>
                                                </EditItemTemplate>
                                                <ItemStyle Width="8%" Wrap="false"/>
                                                <FooterTemplate>
                                                </FooterTemplate>
                                                <FooterStyle BackColor="#C0C0C0" HorizontalAlign="Left" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Status" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" ControlStyle-Font-Bold="false" SortExpression="TaskStatus">
                                                <ItemTemplate>
                                                        <asp:Label ID="lblTaskStatus" style="font-weight:normal;display:block;width:75px;" runat="server" Text='<%# Bind("taskStatus") %>'></asp:Label>
                                                </ItemTemplate>
                                                <ItemStyle Width="4%" Wrap="true"/>
                                                <FooterTemplate>
                                                </FooterTemplate>
                                                <FooterStyle BackColor="#C0C0C0" HorizontalAlign="Left" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Assigned" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" ControlStyle-Font-Bold="false" SortExpression="AssignToUserName">
                                                <ItemTemplate>
                                                        <asp:Label ID="lblAssignedToUser" runat="server" Text='<%# Bind("AssignToUserName") %>'></asp:Label>
                                                </ItemTemplate>
                                                <EditItemTemplate>
                                                    <asp:DropDownList ID="ddlUsers" runat="server" AppendDataBoundItems="true" DataTextField="userName" DataValueField="userName" DataSource="<%# UserList %>" SelectedValue='<%# Bind("AssignToUserName")%>'>
                                                        <asp:ListItem Text="Not assigned" Value="" />
                                                    </asp:DropDownList>
                                                </EditItemTemplate>
                                                <ItemStyle Width="5%" Wrap="false"/>
                                                <FooterTemplate>
                                                </FooterTemplate>
                                                <FooterStyle BackColor="#C0C0C0" HorizontalAlign="Left" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Hours" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" ControlStyle-Font-Bold="false" SortExpression="taskDuration">
                                                <ItemTemplate>
                                                        <asp:Label ID="lblTaskHours" runat="server" Text='<%# Bind("taskDuration", "{0} hrs") %>'></asp:Label>
                                                </ItemTemplate>
                                                <ItemStyle Width="3%" Wrap="false"/>
                                                <FooterTemplate>
                                                </FooterTemplate>
                                                <FooterStyle BackColor="#C0C0C0" HorizontalAlign="Left" />
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
                                                            <asp:Image ID="Image3" runat="server" ImageUrl="Images/edit.png" ToolTip="Edit" />
                                                    </asp:LinkButton>
                                                    <asp:LinkButton ID="lnkEffort" runat="server" CausesValidation="False" CommandName="TaskEffort" >
                                                            <asp:Image ID="Image4" runat="server" ImageUrl="Images/task-details.png" ToolTip="Show task efforts" />
                                                    </asp:LinkButton>
                                                    <asp:LinkButton ID="lnkComplete" Visible="false" runat="server" CausesValidation="False" CommandName="TaskComplete" OnClientClick="javascript:return confirm('You are marking the task as Complete, have you entered the hours spent?');" >
                                                            <asp:Image ID="Image5" runat="server" ImageUrl="Images/task-ok.png" ToolTip="Mark task as complete" />
                                                    </asp:LinkButton>
                                                    <asp:LinkButton ID="lnkDelete" runat="server" CausesValidation="False" CommandName="Delete" OnClientClick="javascript:return confirm('Are you sure you want to archive the entry?');" >
                                                            <asp:Image ID="Image6" runat="server" ImageUrl="Images/delete.png" ToolTip="Archive Task" />
                                                    </asp:LinkButton>
                                                </ItemTemplate> 
                                                <ItemStyle Width="2%" Wrap="false"/>     
                                                <FooterTemplate>
                                                    <asp:LinkButton ID="lnkAdd2" runat="server" CausesValidation="True" CommandName="Add">
                                                            <asp:Image ID="ImageAdd2" runat="server" ImageUrl="Images/add.png" ToolTip="Add" />
                                                    </asp:LinkButton>                                                  
                                                </FooterTemplate>                                   
                                            </asp:TemplateField>    
                                        </Columns>
                                        
                                        <EmptyDataTemplate>
                                            <table border="1" cellpadding="2" cellspacing="0" width="100%" bgcolor="#FFFFCC">
                                                <thead>
                                                    <tr align="center" bgcolor="#525252">
                                                        <td> <asp:Label skinID="PageHeader" ID="lblEnv" Text="Task" runat="server"/></td>
                                                        <td> <asp:Label skinID="PageHeader" ID="lblEnv2" Text="Priority" runat="server"/></td>
                                                        <td>  <asp:Label skinID="PageHeader" ID="lblEnv3" Text="Project" runat="server"/></td>
                                                        <td>  <asp:Label skinID="PageHeader" ID="lblEnv4" Text="Recurring" runat="server"/></td>
                                                        <td>  <asp:Label skinID="PageHeader" ID="lblEnv5" Text="Add new" runat="server"/></td>
                                                    </tr>
                                                </thead>
                                                <tr>
                                                    <td>
                                                        <asp:TextBox ID="tNewTaskName" Width="90%" runat="server" Text="Specify a new item" BorderStyle="Groove">
                                                        </asp:TextBox>
                                                        <asp:RequiredFieldValidator ID="pv6" runat="server" ToolTip="Required!" Text="*" ControlToValidate="tNewTaskName">
                                                        </asp:RequiredFieldValidator>
                                                    </td>
                                                    <td>
                                                        <asp:DropDownList ID="ddlPriority" runat="server" Width="100%">
                                                            <asp:ListItem Text="1 (High)" Value="1"></asp:ListItem>
                                                            <asp:ListItem Text="2 (Med)" Value="2"></asp:ListItem>
                                                            <asp:ListItem Text="3 (Low)" Value="3"></asp:ListItem>
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td>
                                                        <asp:DropDownList ID="ddlNewProjectName" Width="100%" runat="server" DataTextField="ProjectName" DataValueField="ProjectName" DataSource="<%# Projects %>">
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td>
                                                        <asp:DropDownList ID="ddlNewTaskType" Width="100%" runat="server" AppendDataBoundItems="false" DataTextField="taskType" DataValueField="taskType" DataSource="<%# TaskTypeList %>" >
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td align="center">
                                                        <asp:LinkButton ID="lnkAdd2" runat="server" CausesValidation="True" CommandName="AddOnEmpty">
                                                                <asp:Image ID="ImageAdd2" runat="server" ImageUrl="images/add.png" ToolTip="Add" />
                                                        </asp:LinkButton> 
                                                    </td>
                                                </tr>
                                            </table>
                                        </EmptyDataTemplate>
                                        <EmptyDataRowStyle HorizontalAlign="Left" BackColor="#C0C0C0" />
                                    </asp:GridView>
                        </asp:View>
                        <asp:View ID="vProjects" runat="server">
                            <table border="0" width="100%" cellpadding="0" cellspacing="0" align="left">
                                <tr>
                                <td>
    		                        <table border="1" bordercolor="#525252" width="100%" cellpadding="0" cellspacing="0" align="left">
                                        <thead align="left" >
                                            <asp:Label style="background-color:#525252;color:gold;" ID="Label1" Text="Projects" runat="server" Width="100%" />
                                        </thead>
                	                    <tr>
                	                        <td>
                                                <asp:GridView ID="projectView" runat="server" AutoGenerateEditButton="False"
                                        AutoGenerateColumns="False" CellPadding="0" 
                                        AllowPaging="true" PageSize="10"
                                        GridLines="Both" ShowFooter="true" ShowHeader="true"
                                        OnRowDataBound="projectView_RowDataBound"
                                        OnRowEditing="projectView_RowEditing"   
                                        OnRowCancelingEdit="projectView_RowCancelEditing"
                                        OnRowUpdating="projectView_RowUpdating"
                                        OnRowDeleting="projectView_RowUDeleting"
                                        OnPageIndexChanging="projectView_PageIndexChanging" 
                                        OnRowCommand="projectView_RowCommand" >
                                        <Columns>
                                            <asp:TemplateField HeaderText="Priority" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" ControlStyle-Font-Bold="false" SortExpression="projectPriority">
                                                <ItemTemplate>
                                                     <asp:Label ID="lblProjectPriority" style="font-weight:bold;display:block;width:50px;" runat="server" Text='<%# Bind("projectPriority") %>'></asp:Label>
                                                </ItemTemplate>
                                                <EditItemTemplate>
                                                    <asp:DropDownList ID="ddlPriority" runat="server" SelectedValue='<%# Eval("projectPriority")%>'>
                                                        <asp:ListItem Text="1 (High)" Value="1"></asp:ListItem>
                                                        <asp:ListItem Text="2 (Med)" Value="2"></asp:ListItem>
                                                        <asp:ListItem Text="3 (Low)" Value="3"></asp:ListItem>
                                                    </asp:DropDownList>
                                                </EditItemTemplate>
                                                <ItemStyle Width="3%" Wrap="false"/>
                                                <FooterTemplate>
                                                    <asp:DropDownList ID="ddlNewPriority" runat="server">
                                                        <asp:ListItem Text="1 (High)" Value="1"></asp:ListItem>
                                                        <asp:ListItem Text="2 (Med)" Value="2"></asp:ListItem>
                                                        <asp:ListItem Text="3 (Low)" Value="3"></asp:ListItem>
                                                    </asp:DropDownList>
                                                </FooterTemplate>
                                                <FooterStyle BackColor="#C0C0C0" HorizontalAlign="Left" />
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Project Name" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Center" ControlStyle-Font-Bold="false" SortExpression="projectName">
                                                <ItemTemplate>
                                                     <asp:HiddenField ID="projectID" runat="server" Value='<%# DataBinder.Eval(Container.DataItem, "projectID")%>' />
                                                     <asp:Label ID="lblProjectName" runat="server" Text='<%# Bind("projectName") %>'></asp:Label>
                                                </ItemTemplate>
                                                <EditItemTemplate>
                                                    <asp:HiddenField ID="projectID" runat="server" Value='<%# DataBinder.Eval(Container.DataItem, "projectID")%>' />
                                                    <asp:TextBox ID="tProjectName" MaxLength="25" Width="90%" runat="Server" Text='<%# Bind("projectName") %>' BorderStyle="Groove" ></asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="rfv" runat="server" Text="*" ControlToValidate="tProjectName"></asp:RequiredFieldValidator>
                                                </EditItemTemplate>
                                                <ItemStyle Width="75%" Wrap="false"/>
                                                <FooterTemplate>
                                                    <asp:TextBox BorderStyle="Groove" MaxLength="25" Width="90%" ID="tNewProjectName" runat="server" Text="Specify a new item"></asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="pv1" runat="server" ToolTip="Required!" Text="*" ControlToValidate="tNewProjectName">
                                                    </asp:RequiredFieldValidator>
                                                </FooterTemplate>
                                                <FooterStyle BackColor="#C0C0C0" HorizontalAlign="Left" />
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Environment" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" ControlStyle-Font-Bold="false" SortExpression="environment">
                                                <ItemTemplate>
                                                     <asp:Label ID="lblEnvName" runat="server" Text='<%# Bind("envName") %>'></asp:Label>
                                                </ItemTemplate>
                                                <EditItemTemplate>
                                                    <asp:DropDownList ID="ddlEnvName" runat="server" DataTextField="EnvName" DataValueField="EnvName" DataSource="<%# Environments %>" SelectedValue='<%# Eval("envName")%>'>
                                                    </asp:DropDownList>
                                                </EditItemTemplate>
                                                <ItemStyle Width="10%" Wrap="false"/>
                                                <FooterTemplate>
                                                    <asp:DropDownList ID="ddlNewEnvName" runat="server" Width="100%" DataTextField="EnvName" DataValueField="EnvName" DataSource="<%# Environments %>">
                                                    </asp:DropDownList>
                                                </FooterTemplate>
                                                <FooterStyle BackColor="#C0C0C0" HorizontalAlign="Left" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Select" HeaderStyle-HorizontalAlign="Center"> 
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
                                                            <asp:Image ID="Image3" runat="server" ImageUrl="Images/edit.png" ToolTip="Edit" />
                                                    </asp:LinkButton>
                                                    <asp:LinkButton ID="lnkDelete" runat="server" CausesValidation="False" CommandName="Delete" OnClientClick="javascript:return confirm('Are you sure you want to remove the entry?');" >
                                                            <asp:Image ID="Image4" runat="server" ImageUrl="Images/delete.png" ToolTip="Archive" />
                                                    </asp:LinkButton>
                                                    <asp:LinkButton ID="lnkReport" runat="server" CausesValidation="False" CommandName="Report" >
                                                            <asp:Image ID="Image7" runat="server" ImageUrl="Images/report.png" ToolTip="View Project Report" />
                                                    </asp:LinkButton>  
                                                </ItemTemplate> 
                                                <ItemStyle Width="5%" Wrap="false"/>     
                                                <FooterTemplate>
                                                    <asp:LinkButton ID="lnkAdd2" runat="server" CausesValidation="True" CommandName="Add">
                                                            <asp:Image ID="ImageAdd2" runat="server" ImageUrl="Images/add.png" ToolTip="Add" />
                                                    </asp:LinkButton>                                                  
                                                </FooterTemplate>                                   
                                            </asp:TemplateField>    
                                        </Columns>
                                        <EmptyDataTemplate>
                                         <table border="1" cellpadding="2" cellspacing="0" width="100%" bgcolor="#FFFFCC">
                                             <thead>
                                                 <tr align="center" bgcolor="#525252">
                                                     <td> <asp:Label skinID="PageHeader" ID="lblEnv" Text="Project Name" runat="server"/></td>
                                                     <td> <asp:Label skinID="PageHeader" ID="lblEnv2" Text="Priority" runat="server"/></td>
                                                     <td>  <asp:Label skinID="PageHeader" ID="lblEnv3" Text="Environment Name" runat="server"/></td>
                                                     <td>  <asp:Label skinID="PageHeader" ID="lblEnv4" Text="Add new" runat="server"/></td>
                                                 </tr>
                                             </thead>
                                             <tr>
                                                 <td>
                                                    <asp:TextBox ID="tNewProjectName" MaxLength="25" Width="90%" runat="server" Text="Specify a new item" BorderStyle="Groove">
                                                    </asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="pv6" runat="server" ToolTip="Required!" ErrorMessage="<img src='images/indicator/ball_redS.gif'>" ControlToValidate="tNewProjectName">
                                                    </asp:RequiredFieldValidator>
                                                 </td>
                                                 <td>
                                                    <asp:DropDownList ID="ddlNewPriority" runat="server" Width="100%">
                                                        <asp:ListItem Text="1 (High)" Value="1"></asp:ListItem>
                                                        <asp:ListItem Text="2 (Med)" Value="2"></asp:ListItem>
                                                        <asp:ListItem Text="3 (Low)" Value="3"></asp:ListItem>
                                                    </asp:DropDownList>
                                                 </td>
                                                 <td>
                                                    <asp:DropDownList ID="ddlNewEnvName" runat="server" Width="100%" DataTextField="EnvName" DataValueField="EnvName" DataSource="<%# Environments %>">
                                                    </asp:DropDownList>
                                                 </td>
                                                 <td align="center">
                                                    <asp:LinkButton ID="lnkAdd2" runat="server" CausesValidation="True" CommandName="AddOnEmpty">
                                                            <asp:Image ID="ImageAdd2" runat="server" ImageUrl="images/add.png" ToolTip="Add" />
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
                                    </td>
                                </tr>
                               <tr>
                                    <td>&nbsp;</td>
                                </tr>
                                <tr>
                                    <td>&nbsp;</td>
                                </tr>
                                <tr>
                                    <td>
    		                        <table border="1" bordercolor="#525252" width="100%" cellpadding="0" cellspacing="0" align="left">
                                        <thead align="left" >
                                            <asp:Label style="background-color:#525252;color:gold;" ID="Label2" Text="Archived projects" runat="server" Width="100%" />
                                        </thead>
                	                    <tr>
                	                        <td>
                                                <asp:GridView ID="archivedProjects" runat="server"
                                        AutoGenerateEditButton="False" SkinID="archived"
                                        AutoGenerateColumns="False" CellPadding="0" CellSpacing="0" 
                                        AllowPaging="true" PageSize="7" 
                                        GridLines="Both" ShowFooter="false" ShowHeader="true"
                                        OnRowDataBound="archivedProjects_RowDataBound"
                                        OnPageIndexChanging="archivedProjects_PageIndexChanging" 
                                        OnRowCommand="archivedProjects_RowCommand">
                                        <Columns>
                                            <asp:TemplateField HeaderText="Project Name" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Center" ControlStyle-Font-Bold="false" SortExpression="projectName">
                                                <ItemTemplate>
                                                     <asp:HiddenField ID="projectID" runat="server" Value='<%# DataBinder.Eval(Container.DataItem, "projectID")%>' />
                                                     <asp:Label ID="lblProjectName" runat="server" Text='<%# Bind("projectName") %>'></asp:Label>
                                                </ItemTemplate>
                                                <ItemStyle Width="35%" Wrap="false" Font-Italic="true"/>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="" HeaderStyle-HorizontalAlign="Left"> 
                                                <ItemTemplate> 
                                                    <asp:LinkButton ID="lnkActivate" runat="server" CausesValidation="False" CommandName="Activate" OnClientClick="javascript:return confirm('Are you sure you want to activate the entry?');" >
                                                            <asp:Image ID="Image4" runat="server" ImageUrl="Images/task-ok.png" ToolTip="Activate Item" />
                                                    </asp:LinkButton>
                                                </ItemTemplate> 
                                                <ItemStyle Width="1%" Wrap="false" Font-Italic="true" />     
                                            </asp:TemplateField>    
                                        </Columns>  
                                        <EmptyDataTemplate>
                                        <i>0 record(s) found</i>
                                        </EmptyDataTemplate>                                  
                                        </asp:GridView>
                                            </td>
                                        </tr>
                                    </table>
                                    </td>
                                </tr>
                            </table>
                        </asp:View>
                        <asp:View ID="vEnvironments" runat="server">
                            <table border="0" width="100%" cellpadding="0" cellspacing="0" align="left">
                                <tr>
                                    <td>
    		                             <table border="0" bordercolor="#525252" width="100%" cellpadding="0" cellspacing="0" align="left">
                                        <thead align="left" >
                                            <asp:Label style="background-color:#525252;color:gold;" ID="Label11" Text="Environments" runat="server" Width="100%" />
                                        </thead>
                	                    <tr>
                	                        <td>
                                                <asp:GridView ID="environmentView" runat="server" AutoGenerateEditButton="False"
                                        AutoGenerateColumns="False" CellPadding="0" CellSpacing="0" 
                                        AllowPaging="true" PageSize="10" 
                                        GridLines="Both" ShowFooter="true" ShowHeader="true"
                                        OnRowDataBound="environmentView_RowDataBound"
                                        OnRowEditing="environmentView_RowEditing"   
                                        OnRowCancelingEdit="environmentView_RowCancelEditing"
                                        OnRowUpdating="environmentView_RowUpdating"
                                        OnRowDeleting="environmentView_RowUDeleting"
                                        OnPageIndexChanging="environmentView_PageIndexChanging" 
                                        OnRowCommand="environmentView_RowCommand" >
                                        <Columns>
                                            <asp:TemplateField HeaderText="Environment Name" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Center" ControlStyle-Font-Bold="false" SortExpression="envName">
                                                <ItemTemplate>
                                                     <asp:HiddenField ID="envID" runat="server" Value='<%# DataBinder.Eval(Container.DataItem, "envID")%>' />
                                                     <asp:Label ID="lblEnvName" runat="server" Text='<%# Bind("envName") %>'></asp:Label>
                                                </ItemTemplate>
                                                <EditItemTemplate>
                                                    <asp:HiddenField ID="envID" runat="server" Value='<%# DataBinder.Eval(Container.DataItem, "envID")%>' />
                                                    <asp:TextBox ID="tEnvName" Width="85%" runat="Server" Text='<%# Bind("envName") %>' BorderStyle="Groove" ></asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="rfv" runat="server" Text="*" ControlToValidate="tEnvName"></asp:RequiredFieldValidator>
                                                </EditItemTemplate>
                                                <ItemStyle Width="25%" Wrap="false"/>
                                                <FooterTemplate>
                                                    <asp:TextBox BorderStyle="Groove" Width="85%" ID="tNewEnvName" runat="server" Text="TEST"></asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="validator" runat="server" ToolTip="Required!" ErrorMessage="*" ControlToValidate="tNewEnvName">
                                                    </asp:RequiredFieldValidator>
                                                </FooterTemplate>
                                                <FooterStyle BackColor="#C0C0C0" HorizontalAlign="Left" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Machine Name" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Center" ControlStyle-Font-Bold="false" SortExpression="machineName">
                                                <ItemTemplate>
                                                     <asp:Label ID="lblMachineName" runat="server" Text='<%# Bind("machineName") %>'></asp:Label>
                                                </ItemTemplate>
                                                <EditItemTemplate>
                                                    <asp:TextBox ID="tMachineName" Width="85%" runat="Server" Text='<%# Bind("machineName") %>' BorderStyle="Groove"></asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="rfv2" runat="server" Text="*" ControlToValidate="tMachineName"></asp:RequiredFieldValidator>
                                                </EditItemTemplate>
                                                <ItemStyle Width="25%" Wrap="false"/>
                                                <FooterTemplate>
                                                    <asp:TextBox BorderStyle="Groove" Width="85%" ID="tNewMachineName" runat="server" Text="(local)"></asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="validator2" runat="server" ToolTip="Required!" ErrorMessage="*" ControlToValidate="tNewMachineName">
                                                    </asp:RequiredFieldValidator>
                                                </FooterTemplate>
                                                <FooterStyle BackColor="#C0C0C0" HorizontalAlign="Left" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Database Name" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Center" ControlStyle-Font-Bold="false" SortExpression="databaseName">
                                                <ItemTemplate>
                                                     <asp:Label ID="lblDatabaseName" runat="server" Text='<%# Bind("databaseName") %>'></asp:Label>
                                                </ItemTemplate>
                                                <EditItemTemplate>
                                                    <asp:TextBox ID="tDatabaseName" Width="85%" runat="Server" Text='<%# Bind("databaseName") %>' BorderStyle="Groove"></asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="rfv3" runat="server" Text="*" ControlToValidate="tDatabaseName"></asp:RequiredFieldValidator>
                                                </EditItemTemplate>
                                                <ItemStyle Width="25%" Wrap="false"/>
                                                <FooterTemplate>
                                                    <asp:TextBox BorderStyle="Groove" Width="85%" ID="tNewDatabaseName" runat="server" Text="BizTalkMgmtDb"></asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="validator3" runat="server" ToolTip="Required!" ErrorMessage="*" ControlToValidate="tNewDatabaseName">
                                                    </asp:RequiredFieldValidator>
                                                </FooterTemplate>
                                                <FooterStyle BackColor="#C0C0C0" HorizontalAlign="Left" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Remark" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Center" ControlStyle-Font-Bold="false" SortExpression="envDescription">
                                                <ItemTemplate>
                                                     <asp:Label ID="lblEnvDesc" runat="server" Text='<%# Bind("envDescription") %>'></asp:Label>
                                                </ItemTemplate>
                                                <EditItemTemplate>
                                                    <asp:TextBox ID="tEnvDesc" Width="85%" runat="Server" Text='<%# Bind("envDescription") %>' BorderStyle="Groove"></asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="rfv4" runat="server" Text="*" ControlToValidate="tEnvDesc"></asp:RequiredFieldValidator>
                                                </EditItemTemplate>
                                                <ItemStyle Width="20%" Wrap="false"/>
                                                <FooterTemplate>
                                                    <asp:TextBox BorderStyle="Groove" Width="85%" ID="tNewEnvDesc" runat="server" Text="default"></asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="validator4" runat="server" ToolTip="Required!" ErrorMessage="*" ControlToValidate="tNewEnvDesc">
                                                    </asp:RequiredFieldValidator>
                                                </FooterTemplate>
                                                <FooterStyle BackColor="#C0C0C0" HorizontalAlign="Left" />
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
                                                            <asp:Image ID="Image3" runat="server" ImageUrl="Images/edit.png" ToolTip="Edit" />
                                                    </asp:LinkButton>
                                                    <asp:LinkButton ID="lnkDelete" runat="server" CausesValidation="False" CommandName="Delete" OnClientClick="javascript:return confirm('Are you sure you want to archive the entry?');" >
                                                            <asp:Image ID="Image4" runat="server" ImageUrl="Images/delete.png" ToolTip="De-activate Item" />
                                                    </asp:LinkButton>
                                                </ItemTemplate> 
                                                <ItemStyle Width="5%" Wrap="false"/>     
                                                <FooterTemplate>
                                                    <asp:LinkButton ID="lnkAdd2" runat="server" CausesValidation="True" CommandName="Add">
                                                            <asp:Image ID="ImageAdd2" runat="server" ImageUrl="Images/add.png" ToolTip="Add" />
                                                    </asp:LinkButton>                                                  
                                                </FooterTemplate>                                   
                                            </asp:TemplateField>    
                                        </Columns>
                                        <EmptyDataTemplate>
                                         <table border="1" cellpadding="2" cellspacing="0" width="100%" bgcolor="#FFFFCC">
                                             <thead>
                                                 <tr align="center" bgcolor="#525252">
                                                     <td> <asp:Label skinID="PageHeader" ID="lblEnv" Text="Environment Name" runat="server"/></td>
                                                     <td> <asp:Label skinID="PageHeader" ID="Label6" Text="Machine Name" runat="server"/></td>
                                                     <td> <asp:Label skinID="PageHeader" ID="Label7" Text="Database Name" runat="server"/></td>
                                                     <td> <asp:Label skinID="PageHeader" ID="lblEnv2" Text="Remark" runat="server"/></td>
                                                     <td>  <asp:Label skinID="PageHeader" ID="lblEnv3" Text="Add" runat="server"/></td>
                                                 </tr>
                                             </thead>
                                             <tr>
                                                 <td>
                                                    <asp:TextBox ID="tNewEnvName" Width="85%" runat="server" Text="DEV" BorderStyle="Groove">
                                                    </asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="validator" runat="server" ToolTip="Required!" ErrorMessage="<img src='images/indicator/ball_redS.gif'>" ControlToValidate="tNewEnvName">
                                                    </asp:RequiredFieldValidator>
                                                 </td>
                                                 <td>
                                                    <asp:TextBox ID="tNewMachineName" Width="85%" runat="server" Text="(local)" BorderStyle="Groove">
                                                    </asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="validator2" runat="server" ToolTip="Required!" ErrorMessage="<img src='images/indicator/ball_redS.gif'>" ControlToValidate="tNewMachineName">
                                                    </asp:RequiredFieldValidator>
                                                 </td>
                                                 <td>
                                                    <asp:TextBox ID="tNewDatabaseName" Width="85%" runat="server" Text="BizTalkMgmtDb" BorderStyle="Groove">
                                                    </asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="validator3" runat="server" ToolTip="Required!" ErrorMessage="<img src='images/indicator/ball_redS.gif'>" ControlToValidate="tNewDatabaseName">
                                                    </asp:RequiredFieldValidator>
                                                 </td>
                                                 <td>
                                                    <asp:TextBox ID="tNewEnvDesc" Width="85%" runat="server" Text="Development box" BorderStyle="Groove">
                                                    </asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="validator4" runat="server" ToolTip="Required!" ErrorMessage="<img src='images/indicator/ball_redS.gif'>" ControlToValidate="tNewEnvDesc">
                                                    </asp:RequiredFieldValidator>
                                                 </td>
                                                 <td align="center">
                                                    <asp:LinkButton ID="lnkAdd2" runat="server" CausesValidation="True" CommandName="AddOnEmpty">
                                                            <asp:Image ID="ImageAdd2" runat="server" ImageUrl="images/add.png" ToolTip="Add" />
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
                                    </td>
                                </tr>
                                <tr>
                                    <td>&nbsp;</td>
                                </tr>
                                <tr>
                                    <td>&nbsp;</td>
                                </tr>
                                <tr>
                                    <td>
    		                        <table border="1" bordercolor="#525252" width="100%" cellpadding="0" cellspacing="0" align="left">
                                        <thead align="left" >
                                            <asp:Label style="background-color:#525252;color:gold;" ID="Label3" Text="Archived Environments" runat="server" Width="100%" />
                                        </thead>
                	                    <tr>
                	                        <td>
                                        <asp:GridView ID="archivedEnvironments" runat="server"
                                        AutoGenerateEditButton="False" SkinID="archived"
                                        AutoGenerateColumns="False" CellPadding="0" CellSpacing="0" 
                                        AllowPaging="true" PageSize="7" 
                                        GridLines="Both" ShowFooter="false" ShowHeader="true"
                                        OnRowDataBound="archivedEnvironments_RowDataBound"
                                        OnPageIndexChanging="archivedEnvironments_PageIndexChanging" 
                                        OnRowCommand="archivedEnvironments_RowCommand">
                                        <Columns>
                                            <asp:TemplateField HeaderText="Environment Name" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Center" ControlStyle-Font-Bold="false" SortExpression="envName">
                                                <ItemTemplate>
                                                     <asp:HiddenField ID="envID" runat="server" Value='<%# DataBinder.Eval(Container.DataItem, "envID")%>' />
                                                     <asp:Label ID="lblEnvName" runat="server" Text='<%# Bind("envName") %>'></asp:Label>
                                                </ItemTemplate>
                                                <ItemStyle Width="25%" Wrap="false" Font-Italic="true"/>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Environment Description" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Center" ControlStyle-Font-Bold="false" SortExpression="envDescription">
                                                <ItemTemplate>
                                                     <asp:Label ID="lblEnvDesc" runat="server" Text='<%# Bind("envDescription") %>'></asp:Label>
                                                </ItemTemplate>
                                                <ItemStyle Width="50%" Wrap="false" Font-Italic="true" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="" HeaderStyle-HorizontalAlign="Left"> 
                                                <ItemTemplate> 
                                                    <asp:LinkButton ID="lnkActivate" runat="server" CausesValidation="False" CommandName="Activate" OnClientClick="javascript:return confirm('Are you sure you want to activate the entry?');" >
                                                            <asp:Image ID="Image4" runat="server" ImageUrl="Images/task-ok.png" ToolTip="Activate Item" />
                                                    </asp:LinkButton>
                                                </ItemTemplate> 
                                                <ItemStyle Width="1%" Wrap="false" Font-Italic="true" />     
                                            </asp:TemplateField>    
                                        </Columns> 
                                        <EmptyDataTemplate>
                                        <i>0 records(0) found</i>
                                        </EmptyDataTemplate>                                   
                                        </asp:GridView>
                                            </td>
                                        </tr>
                                     </table>
                                    </td>
                                </tr>
                            </table>
                        </asp:View>
                        <asp:View ID="vArchivedTasks" runat="server">
    		                        <table border="1" bordercolor="#525252" width="100%" cellpadding="0" cellspacing="0" align="left">
                                        <thead align="left" >
                                            <asp:Label style="background-color:#525252;color:gold;" ID="Label4" Text="Archived Tasks" runat="server" Width="100%" />
                                        </thead>
                	                    <tr>
                	                        <td>
                                                <asp:GridView ID="archivedTasks" runat="server" AutoGenerateEditButton="False"
                                                AutoGenerateColumns="False" CellPadding="0" SkinID="archived"
                                                AllowPaging="true" PageSize="25"
                                                GridLines="Both" ShowHeader="true"
                                                OnRowDataBound="archivedTasks_RowDataBound"
                                                OnPageIndexChanging="archivedTasks_PageIndexChanging" 
                                                OnRowCommand="archivedTasks_RowCommand" >
                                                <Columns>
                                                    <asp:TemplateField HeaderText="Task" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Center" ControlStyle-Font-Bold="false" SortExpression="task">
                                                        <ItemTemplate>
                                                             <asp:HiddenField ID="hTaskID" runat="server" Value='<%# DataBinder.Eval(Container.DataItem, "taskID")%>' />
                                                             <asp:Label ID="lblTaskTitle" runat="server" Text='<%# FormatTaskName( DataBinder.Eval(Container.DataItem, "taskRef") , DataBinder.Eval(Container.DataItem, "taskTitle") ) %>'></asp:Label>
                                                        </ItemTemplate>
                                                        <ItemStyle Width="40%" Font-Italic="true" Wrap="false"/>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Hours" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Center" ControlStyle-Font-Bold="false" SortExpression="task">
                                                        <ItemTemplate>
                                                             <asp:Label ID="lblEffortHrs" runat="server" Text='<%#  Bind("taskDuration") %>' ToolTip="Hours spent on the task"></asp:Label>
                                                        </ItemTemplate>
                                                        <ItemStyle Width="5%" Font-Italic="true" Wrap="false"/>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Project" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Center" ControlStyle-Font-Bold="false" SortExpression="project">
                                                        <ItemTemplate>
                                                             <asp:Label ID="lblProjectName" runat="server" Text='<%# Bind("projectName") %>'></asp:Label>
                                                        </ItemTemplate>
                                                        <ItemStyle Width="20%" Font-Italic="true" Wrap="false"/>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Status" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" ControlStyle-Font-Bold="false" SortExpression="TaskStatus">
                                                        <ItemTemplate>
                                                             <asp:Label ID="lblTaskStatus" style="font-weight:normal;display:block;width:75px;" runat="server" Text='<%# Bind("taskStatus") %>'></asp:Label>
                                                        </ItemTemplate>
                                                        <ItemStyle Width="10%" Font-Italic="true" Wrap="true"/>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Recurring" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" ControlStyle-Font-Bold="false" SortExpression="TaskType">
                                                        <ItemTemplate>
                                                             <asp:Label ID="lblTaskType" style="font-weight:normal;display:block;width:90px;" runat="server" Text='<%# Bind("taskType") %>'></asp:Label>
                                                        </ItemTemplate>
                                                        <ItemStyle Width="10%" Font-Italic="true" Wrap="true"/>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Updated by" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" ControlStyle-Font-Bold="false" SortExpression="assignedTo">
                                                        <ItemTemplate>
                                                                <asp:Label ID="lblUpdatedbyUserName" runat="server" Text='<%# Bind("UpdatedbyUserName") %>'></asp:Label>
                                                        </ItemTemplate>
                                                        <ItemStyle Width="10%" Wrap="false"/>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="" HeaderStyle-HorizontalAlign="Left"> 
                                                        <ItemTemplate> 
                                                            <asp:LinkButton ID="lnkArchive" runat="server" CausesValidation="False" CommandName="Archive" OnClientClick="javascript:return confirm('Are you sure you want to delete the entry?');" >
                                                                    <asp:Image ID="Image4" runat="server" ImageUrl="Images/delete.png" ToolTip="Delete Task" />
                                                            </asp:LinkButton>
                                                        </ItemTemplate> 
                                                        <ItemStyle Width="1%" Wrap="false" Font-Italic="true" />     
                                                    </asp:TemplateField>   
                                                </Columns>
                                                <EmptyDataTemplate>
                                                <i>0 record(s) found.</i>
                                                </EmptyDataTemplate>
                                            </asp:GridView>             
                                            </td>
                                        </tr>
                                    </table>
                        </asp:View>
                        <asp:View ID="vTaskReport" runat="server">
    		                        <table border="1" bordercolor="#525252" width="100%" cellpadding="1" cellspacing="0" align="left">
                                        <tr style="background-color:#525252">
                                             <td align="left">
                                                <asp:Label ID="headerCaption" runat="server" Width="100%" Text="User Task Report" SkinID="PageHeader"></asp:Label>
                                            </td>
                                            <td align="right" colspan="2">
                                                <asp:LinkButton ID="btnExportToExcel" runat="server" style="color:teal;text-decoration:none;" OnClick="btnExportToExcel_Click" Visible="false" >
                                                    <asp:Image ID="excel" runat="server" ImageAlign="Middle" ImageUrl="~/Images/Excel-16.gif" />
                                                </asp:LinkButton>    
                                            </td>
                                        </tr>
                                        <tr>
                                            <td width="27%">
                                                Start date: <asp:TextBox ID="tStartDate" Width="40%" MaxLength="11" runat="server" BorderStyle="Groove"  />
                                               <rjs:PopCalendar Format="mmm dd yyyy" RequiredDate="True" Culture="en-US English (United States)" ID="startDateCalendar" runat="server" Control="tStartDate" Separator="-" />
                                            </td>
                                            <td width="25%">
                                                End date: <asp:TextBox ID="tEndDate" Width="40%"  MaxLength="11" runat="server" BorderStyle="Groove"  />
                                                <rjs:PopCalendar Format="mmm dd yyyy" RequiredDate="True" Culture="en-US English (United States)" ID="endDateCalendar" runat="server" Control="tEndDate" Separator="-" />
                                            </td>
                                            <td>
                                                <asp:LinkButton ID="lnkViewReport" runat="server" cssclass="blockView" Text="Show Report" ToolTip="Default task report is for past 5 days" CommandName="ViewReports" OnClick="lnkViewReport_Click"/>
                                            </td>
                                        </tr>
                	                    <tr>
                	                        <td colspan="3">
                                                <asp:GridView ID="reportView" runat="server" AutoGenerateEditButton="False"
                                                AutoGenerateColumns="False" CellPadding="0"  
                                                AllowPaging="true" AllowSorting="true" PageSize="25"
                                                GridLines="Both" ShowHeader="true" ShowFooter="true"
                                                OnRowDataBound="taskReport_RowDataBound"
                                                OnSorting="taskReport_OnSort"
                                                OnPageIndexChanging="taskReport_PageIndexChanging" >
                                                <Columns>
                                                    <asp:TemplateField HeaderText="Priority" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" ControlStyle-Font-Bold="false" SortExpression="taskPriority">
                                                        <ItemTemplate>
                                                                <asp:Label ID="lblTaskPriority" style="font-weight:bold;display:block;width:50px;" runat="server" Text='<%# Bind("taskPriority") %>'></asp:Label>
                                                        </ItemTemplate>
                                                        <ItemStyle Width="3%" Wrap="false"/>
                                                        <FooterStyle BackColor="#C0C0C0" HorizontalAlign="Left" />
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Task" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Center" ControlStyle-Font-Bold="false" SortExpression="taskRef">
                                                        <ItemTemplate>
                                                                <asp:Label ID="lblTaskTitle" runat="server" Text='<%# FormatTaskName( DataBinder.Eval(Container.DataItem, "taskRef") , DataBinder.Eval(Container.DataItem, "taskTitle") ) %>'></asp:Label>
                                                        </ItemTemplate>
                                                        <ItemStyle Width="80%" Wrap="false"/>
                                                        <FooterTemplate>
                                                        </FooterTemplate>
                                                        <FooterStyle BackColor="#C0C0C0" HorizontalAlign="Left" />
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Project" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Center" ControlStyle-Font-Bold="false" SortExpression="ProjectName">
                                                        <ItemTemplate>
                                                                <asp:Label ID="lblProjectName" runat="server" Text='<%# Bind("ProjectName") %>'></asp:Label>
                                                        </ItemTemplate>
                                                        <ItemStyle Width="5%" Wrap="false"/>
                                                        <FooterTemplate>
                                                        </FooterTemplate>
                                                        <FooterStyle BackColor="#C0C0C0"  HorizontalAlign="Left" />
                                                    </asp:TemplateField>
                                                     <asp:TemplateField HeaderText="Due On" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" ControlStyle-Font-Bold="false" SortExpression="taskDueDate">
                                                        <ItemTemplate>
                                                                <asp:Label ID="lblTaskDueDate" style="font-weight:normal;display:block;width:75px;" runat="server" Text='<%# Bind("taskDueDate","{0:MMM-dd-yy}") %>'></asp:Label>
                                                        </ItemTemplate>
                                                        <ItemStyle Width="4%" Wrap="true"/>
                                                        <FooterTemplate>
                                                        </FooterTemplate>
                                                        <FooterStyle BackColor="#C0C0C0" HorizontalAlign="Left" />
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Status" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" ControlStyle-Font-Bold="false" SortExpression="TaskStatus">
                                                        <ItemTemplate>
                                                                <asp:Label ID="lblTaskStatus" style="font-weight:normal;display:block;width:75px;" runat="server" Text='<%# Bind("taskStatus") %>'></asp:Label>
                                                        </ItemTemplate>
                                                        <ItemStyle Width="3%" Wrap="true"/>
                                                        <FooterTemplate>
                                                        </FooterTemplate>
                                                        <FooterStyle BackColor="#C0C0C0" HorizontalAlign="Left" />
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Hours" ItemStyle-HorizontalAlign="Right" HeaderStyle-HorizontalAlign="Center" ControlStyle-Font-Bold="false" SortExpression="TotalTaskHours">
                                                        <ItemTemplate>
                                                                <asp:HiddenField ID="hTaskHours" runat="server" Value='<%# Bind("TotalTaskHours") %>' />
                                                                <asp:Label ID="lblTaskHours" runat="server" Text='<%# Bind("TotalTaskHours", "{0} hrs&nbsp;&nbsp;") %>'></asp:Label>
                                                        </ItemTemplate>
                                                        <ItemStyle Width="2%" Wrap="false"/>
                                                        <FooterTemplate>
                                                                <asp:Label ID="lblEffortSum" runat="server" ></asp:Label>
                                                        </FooterTemplate>
                                                        <FooterStyle BackColor="#C0C0C0" Wrap="false" HorizontalAlign="Right" />
                                                    </asp:TemplateField>
                                                </Columns>
                                                <EmptyDataTemplate>
                                                    <div>No record(s) found.</div>
                                                </EmptyDataTemplate>
                                            </asp:GridView>             
                                            </td>
                                        </tr>
                                    </table>                        
                        </asp:View>
                    </asp:MultiView>
                </ContentTemplate>
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
		               <b>Personal Task Manager</b><br />
		               Use this module to manage your tasks.<br /><br />
                       NOTE: A user cannot edit/delete efforts for the tasks NOT assigned to him.<br />
                       <br />
                       <b>Priority Legend</b><br />
                       <table border="1" style="color:#333333;border-collapse:collapse;background-color:#FFFFCC;border-color:#525252">
                           <tr>
                                <td align="center">
                                       <span id="s1" title="High Priority" style="color:White;background-color:Orange;font-family:Verdana;font-size:10pt;font-weight:normal;font-weight:bold;display:block;width:50px;">1</span>
                               </td>
                               <td>
                                      High Priority  
                               </td>
                           </tr>
                           <tr>
                                <td align="center">
                                       <span id="s2" title="Medium Priority" style="color:White;background-color:RoyalBlue;font-family:Verdana;font-size:10pt;font-weight:normal;font-weight:bold;display:block;width:50px;">2</span>
                               </td>
                               <td>
                                        Medium Priority
                               </td>
                           </tr>
                           <tr>
                                <td align="center">
                                        <span id="s3" title="Low Priority" style="color:White;background-color:Teal;font-family:Verdana;font-size:10pt;font-weight:normal;font-weight:bold;display:block;width:50px;">3</span>
                               </td>
                               <td>
                                        Low Priority
                               </td>
                           </tr>
                       </table>
		        </div>
		        </div>
		        </div>
		        <!--- Shady code goes here -->
   	            </td>
           </tr>
       <tr>
            <td align="center">
            
            <asp:SqlDataSource ID="taskSource" runat="server" ConnectionString="<%$ ConnectionStrings:authStore %>"
                SelectCommand="select Taskstatus, COUNT(*) as 'Count' from  [BCCDB].[dbo].[bcc_TaskList] where showFlag = 1 group by taskStatus">
            </asp:SqlDataSource>
            
            <asp:chart id="taskChart" DataSourceID="taskSource" runat="server" Visible="false" Palette="Light" BackColor="#E9E9E9" 
                    OnPostPaint="taskChart_PostPaint"
                    Height="250px" Width="320px" 
                    BorderDashStyle="Solid" BorderWidth="0" BorderColor="Black">
				<titles>
					<asp:Title Font="Verdana, 12pt" Text="Tasks" Name="taskChartTitle" ForeColor="Gold" BackColor="#525252"></asp:Title>
				</titles>
                <series>
                    <asp:Series Name="Default" ChartType="Pie" BorderColor="180, 26, 59, 105" Color="220, 65, 140, 240">
                    </asp:Series>
                </series>
                <chartareas>
                    <asp:ChartArea Name="ChartArea1" BorderColor="64, 64, 64, 64" BorderDashStyle="Solid" BackSecondaryColor="Transparent" BackColor="#FFFFCC" ShadowColor="Transparent">
                    <area3dstyle Rotation="0" />
                    <axisy LineColor="64, 64, 64, 64">
	                    <LabelStyle Font="Verdana, 8pt, style=Bold" />
	                    <MajorGrid LineColor="64, 64, 64, 64" />
                    </axisy>
                    <axisx LineColor="64, 64, 64, 64">
	                    <LabelStyle Font="Verdana, 8pt, style=Bold" />
	                    <MajorGrid LineColor="64, 64, 64, 64" />
                    </axisx>
                    </asp:ChartArea>
                </chartareas>
            </asp:chart>
            
   	        </td>
       </tr>   
    </table>
</asp:Content>