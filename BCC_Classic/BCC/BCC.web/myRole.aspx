<%@ Page Language="C#" %>
<%@ Import Namespace="System.Web.Security" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN"
    "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<script runat="server">

//string[] rolesArray;

public void Page_Load()
{
  if (!IsPostBack)
  {
    // Bind roles to GridView.

    try
    {
     //   Roles.CreateRole("BCCMember");
     //Roles.CreateRole("BCCDeploy");
     //Roles.CreateRole("BCCArtifact");
     //Roles.CreateRole("BCCAdmin");
     //Roles.AddUserToRole(@"admin", "BCCArtifact");
     //Roles.AddUserToRole(@"admin", "BCCDeploy");
     //Roles.AddUserToRole(@"admin", "BCCAdmin");
     //Roles.RemoveUserFromRole(@"admin", "BCCSupportAdmin");
     //Roles.RemoveUserFromRole(@"admin", "BCCSupportMember");
     //Roles.RemoveUserFromRole(@"admin", "BCCSupportSuperUser");

     //Roles.DeleteRole("BCCSupportAdmin");
     //Roles.DeleteRole("BCCSupportMember");
     //Roles.DeleteRole("BCCSupportSuperUser");


/*	 string roleName = "HUBSupportAdmin";      
	
         if (!Roles.RoleExists(roleName))
         {
	         Roles.CreateRole(roleName);
        	 Roles.AddUserToRole(@"AD-ENT\nkaramc", roleName);

         }
	 else
	 {
		Roles.RemoveUserFromRole(@"AD-ENT\nkaramc", roleName);
	        Roles.DeleteRole(roleName);
	 }
*/
/*
          for (int count = 1; count <= 10; count ++)
              {
		 if (!Roles.RoleExists("sampleRole"+count))
                 {
			 Roles.CreateRole("sampleRole" + count);
	        	 Roles.AddUserToRole(@"AD-ENT\nkaramc", "sampleRole" + count);
                 }
		 else
		 {
			Roles.RemoveUserFromRole(@"AD-ENT\nkaramc", "sampleRole"+count);
		        Roles.DeleteRole("sampleRole" + count);
		 }
              }
*/

	//rolesArray = Roles.GetRolesForUser();
    }
    catch
    {
      Msg.Text = "There is no current logged on user. Role information cannot be retrieved.";
      return;
    }

   // UserRolesGrid.Columns[0].HeaderText = "Roles for " + User.Identity.Name;
   // UserRolesGrid.DataSource = rolesArray;
   // UserRolesGrid.DataBind();

  //  if (rolesArray != null && rolesArray.Length == 0)
   // {
//	Msg.Text = "The current user is not in any role.";
   // }


  }
}

protected void RemoveRolesAndUsers(Object src, EventArgs e)
{
    //    string userName = tName.Text;
    //    string roleName = "HUBSupportAdmin"; 

    //Roles.RemoveUserFromRole(userName, roleName);

    //    roleName = "HUBSupportSuperUser";

    //Roles.RemoveUserFromRole(userName, roleName);

    //    roleName = "HUBSupportMember";      

    //Roles.RemoveUserFromRole(userName, roleName);

}


</script>
<html xmlns="http://www.w3.org/1999/xhtml" >
<head>
<title>Roles.....</title>
</head>
<body>

<form runat="server" id="PageForm">


  <h3>View <asp:LoginName ID="loginName" runat="server" BackColor="#507CD1" ForeColor="White" FormatString="Welcome {0}" /> Roles
  <br></br>
  <asp:Label id="Msg" ForeColor="maroon" runat="server" />
  </h3>

  <div>
	Name: <asp:TextBox id="tName" runat="server" /> 
	<asp:LinkButton id="remove" text="Remove User and Role" runat="server" onClick="RemoveRolesAndUsers" />
  </div>

  <table border="0" cellspacing="4">
    <tr>
      <td valign="top"><asp:GridView runat="server" CellPadding="4" id="UserRolesGrid" 
                                     AutoGenerateColumns="false" Gridlines="None" 
                                     CellSpacing="0" >
                         <HeaderStyle BackColor="navy" ForeColor="white" />
                         <Columns>
                           <asp:TemplateField HeaderText="Roles" >
                             <ItemTemplate>
                               <%# Container.DataItem.ToString() %>
                             </ItemTemplate>
                           </asp:TemplateField>
                         </Columns>
                       </asp:GridView></td>
    </tr>
  </table>

</form>

</body>
</html>

