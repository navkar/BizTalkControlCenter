<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CollectionView.ascx.cs" Inherits="Controls_CollectionView" %>

<asp:GridView ID="collectionView" runat="server" AutoGenerateEditButton="False"
    AutoGenerateColumns="False" CellPadding="0" 
    AllowPaging="false" PageSize="3"
    EmptyDataText="No configuration(s) found."
    GridLines="Both" width="100%" ShowFooter="true" ShowHeader="true"
    OnPageIndexChanging="collectionView_PageIndexChanging"
    OnRowDataBound="collectionView_RowDataBound"
    OnRowCommand="collectionView_RowCommand" >
    <Columns>
        <asp:TemplateField ControlStyle-Font-Bold="false" HeaderStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <asp:Label ID="lblCollectionItem" runat="server" Text="<%#Container.DataItem%>"></asp:Label>
            </ItemTemplate>
            <FooterTemplate>
                <asp:TextBox BorderStyle="Groove" Width="90%" ID="tbCollectionItem" runat="server" Text="Specify a new item"></asp:TextBox>
                <asp:RequiredFieldValidator ID="validator" runat="server" ToolTip="Required!" ErrorMessage="<img src='images/indicator/ball_redS.gif'>" ControlToValidate="tbCollectionItem">
                </asp:RequiredFieldValidator>
            </FooterTemplate>
            <ItemStyle Width="50%" Wrap="true" HorizontalAlign="Left" />
            <FooterStyle BackColor="#C0C0C0" HorizontalAlign="Left" />
        </asp:TemplateField>
        <asp:TemplateField HeaderText="#" HeaderStyle-HorizontalAlign="Center"> 
            <ItemTemplate>
                <asp:LinkButton ID="lnkRemove" runat="server" CausesValidation="False" CommandName="Remove" OnClientClick="javascript:return confirm('Are you sure you want to remove the entry?');" >
                        <asp:Image ID="ImageRemove" runat="server" ImageAlign="Middle" ImageUrl="../images/cancel.png" ToolTip="Remove" />
                </asp:LinkButton>
            </ItemTemplate> 
            <FooterTemplate>
                <asp:LinkButton ID="lnkAdd" runat="server" CausesValidation="True" CommandName="Add">
                        <asp:Image ID="ImageAdd" runat="server" ImageAlign="Middle" ImageUrl="../images/add.png" ToolTip="Add" />
                </asp:LinkButton>            
            </FooterTemplate>
            <ItemStyle Width="3%" Wrap="false"/>   
            <FooterStyle BackColor="#C0C0C0" />
        </asp:TemplateField>
    </Columns>
    <EmptyDataTemplate>
        <asp:TextBox ID="tbEmptyInsert" Width="85%" runat="server" Text="Specify a new item" BorderStyle="Groove">
        </asp:TextBox>
        <asp:RequiredFieldValidator ID="validator" runat="server" ToolTip="Required!" ErrorMessage="<img src='images/indicator/ball_redS.gif'>" ControlToValidate="tbEmptyInsert">
        </asp:RequiredFieldValidator>
        <asp:LinkButton ID="lnkAdd2" runat="server" CausesValidation="True" CommandName="AddOnEmpty">
                <asp:Image ID="ImageAdd2" runat="server" ImageAlign="Right" ImageUrl="../images/add.png" ToolTip="Add" />
        </asp:LinkButton>  
    </EmptyDataTemplate>
    <EmptyDataRowStyle HorizontalAlign="Left" BackColor="#C0C0C0" />
</asp:GridView>