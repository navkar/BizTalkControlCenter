<%@ Page Language="C#" MasterPageFile="~/template.master" AutoEventWireup="true" CodeFile="DSI.aspx.cs" Inherits="ScalabilityView" EnableViewState="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="leftContentPlaceHolder" runat="Server">

    <table border="0" bgcolor="#E9E9E9" width="99%" cellpadding="0" cellspacing="0" align="left">
        <tr>
            <td align="center">
		        <asp:Label ID="lblCaption" skinID="PageCaption" runat="server"></asp:Label>
   	        </td>
        </tr>
        <tr>
                <td>
                    <table border="0" width="100%" bgcolor="#CECECE" cellpadding="0" cellspacing="0" align="left">
                    <tr>
                        <td width="2%" valign="top">
                            <asp:Image ID="errorImg" runat="server" ImageUrl="~/Images/error.png" Visible="false" />
		                </td>
		                <td width="95%">
		                    <asp:Label ID="lblError" runat="server" Visible="false" Width="100%"></asp:Label>
		                </td>
		            </tr>
                    </table>
                </td>
        </tr>
       <tr>
       <td>
	        <!-- Grid View for activity log -->
                <asp:SqlDataSource ID="ActivityDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:authStore %>"
                    SelectCommand="SELECT 'Recent activity' as Comment, [Message] FROM aspnet_WebEvent_Events WHERE EventTime = (select MAX(EventTime) FROM aspnet_WebEvent_Events WHERE EventType = 'BCC.Core.BCCWebAuditEvent' and EventCode = 100505)">
                </asp:SqlDataSource>

                <asp:GridView id="UserActivityGrid" SkinID="sideTable" Visible="true" runat="server" Width="100%" ForeColor="#333333"
                            CellPadding="4" CellSpacing="0" AutoGenerateColumns="False" DataSourceID="ActivityDataSource" 
                        ShowFooter="false" ShowHeader="false" Gridlines="Both" EmptyDataText="">
  	                    <Columns>
  	                        <asp:BoundField DataField="Comment">
                                  <ItemStyle Width="10%" Font-Italic="false" Wrap="false" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Message">
                                  <ItemStyle Width="90%" Font-Italic="false" Wrap="true"/>
                            </asp:BoundField>
                        </Columns>
                </asp:GridView>
        </td>
       </tr>
        <tr>
            <td> 
               <asp:Panel ID="tablePanel" runat="server" defaultbutton="btnFilter">
                <table border="1" bgcolor="#E9E9E9" bordercolor="#C0C0C0" cellpadding="0" cellspacing="0" width="100%" align="left">
                    <tr align="left">
                        <td align="left" valign="bottom">
                            <asp:TextBox id="txtSearchKey" runat="server" BorderStyle="Groove" Tooltip="BizTalk Application Name" AutoCompleteType="Search" />&nbsp;
                            <asp:LinkButton ID="btnFilter" runat="server" style="visibility:hidden;display:none;text-decoration:none;" OnClick="btnFilter_Click" Text="Search" />
                            (Application List : <%=searchTerms%> ...) <%=testData%> [<%=count%>]
                        </td>
			            <td align="right" width="1%">
			                 <asp:LinkButton ID="btnExportToExcel" runat="server" style="color:teal;text-decoration:none;" OnClick="btnExportToExcel_Click" >
				                <asp:Image ID="excel" runat="server" ImageUrl="~/Images/Excel-16.gif" />
			                 </asp:LinkButton>
                        </td>
                    </tr>
                </table>
               </asp:Panel>
            </td>
        </tr>
        <tr>
            <td>
	        <table border="0" bordercolor="#CECECE" width="100%" cellpadding="0" cellspacing="0" align="left">
                <tr>
                    <td valign="top" width="70%" align="left" style="word-break:break-all;">
		            <asp:GridView ID="gridScaleIndex" runat="server" AutoGenerateColumns="False" 
		            CellPadding="4" ForeColor="white" GridLines="Both" Width="100%" 
		            OnRowDataBound="gridScaleIndex_RowDataBound"
                     OnRowCancelingEdit="gridScaleIndex_RowCancelingEdit" 
                     OnRowEditing="gridScaleIndex_RowEditing" 
                     OnRowUpdating="gridScaleIndex_RowUpdating">
                        <Columns>
                           <asp:BoundField ReadOnly="true" DataField="ArtifactType" HeaderText="Artifact Type">
                                 <ItemStyle Width="10%" Font-Italic="false" Wrap="false" />
		                    </asp:BoundField>	                    
                            <asp:BoundField ReadOnly="true" DataField="ArtifactName" HeaderText="Artifact Name">
                                 <ItemStyle Width="40%" Font-Italic="false" Wrap="true" />
		                    </asp:BoundField>
		                    <asp:TemplateField HeaderText="Associated Host" HeaderStyle-HorizontalAlign="Center" ControlStyle-Font-Bold="false"> 
                               <EditItemTemplate>
                                    <asp:DropDownList ID="hostDropDown"  DataTextField="HandlerName" DataValueField="Id" Runat="server"></asp:DropDownList>
                               </EditItemTemplate>
                               <ItemTemplate>
                                    <asp:Label Id="lblHandler" Text='<%# Eval("ArtifactHost") %>' Runat="server"></asp:Label>
                               </ItemTemplate>
                            </asp:TemplateField>
		                     <asp:TemplateField HeaderText="Select" ShowHeader="False" HeaderStyle-HorizontalAlign="Left" Visible=false> 
                                <EditItemTemplate> 
                                    <asp:LinkButton ID="lbkUpdate" runat="server" CausesValidation="True" CommandName="Update" Text="Update"></asp:LinkButton> 
                                    <asp:LinkButton ID="lnkCancel" runat="server" CausesValidation="False" CommandName="Cancel" Text="Cancel"></asp:LinkButton> 
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:LinkButton ID="lnkEdit" runat="server" CausesValidation="False" CommandName="Edit" Text="Edit"></asp:LinkButton> 
                                </ItemTemplate> 
                            </asp:TemplateField> 
                        </Columns>
                    </asp:GridView>
                    </td>
                    <td align="center" valign="top" width="30%" align="center" style="word-break:break-all;">
                        <asp:Panel ID="chartPanel" runat="server" Visible="false">
                           <table border="0" bordercolor="#CECECE" width="100%" cellpadding="0" cellspacing="0" align="left">
                            <tr>
                                <td>
                                    <asp:Label ID="chartHeader" runat="server" Visible="false" Width="100%"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Chart id="hostChart" runat="server" Palette="Light" BackColor="#E9E9E9" Width="270px" Height="200px" BorderDashStyle="Solid" BackGradientStyle="None" BorderWidth="2" BorderColor="181, 64, 1">
                                        <titles>
                                            <asp:Title Font="Verdana, 9.5pt, style=Bold" Name="heading" Text=" " ForeColor="26, 59, 105"></asp:Title>
                                        </titles>
                                        <legends>
                                            <asp:Legend LegendStyle="Table" TitleFont="Verdana, 8.5pt" BackColor="Transparent" Font="Verdana, 7pt"  Enabled="True" Name="Default" BorderWidth="1">
                                                <position y="5.5423727" height="8.283567" width="89.43796" x="4.82481766"></position>
                                            </asp:Legend>
                                        </legends>
                                        <series>
                                            <asp:Series  XValueType="Int32" Name="Actual" BorderColor="180, 26, 59, 105" Legend="Default">
                                            </asp:Series>
                                            <asp:Series XValueType="Int32" Name="Expected" BorderColor="180, 26, 59, 105" Legend="Default">
                                            </asp:Series>
                                        </series>
                                      
                                        <chartareas>
                                            <asp:ChartArea Name="chartArea1" BorderColor="64, 64, 64, 64" BackSecondaryColor="#E9E9E9" 
                                                            BackColor="White" ShadowColor="Transparent" BackGradientStyle=TopBottom>
                                                <axisy linecolor="64, 64, 64, 64" labelautofitmaxfontsize="8" Title="Host count" >
                                                    <LabelStyle font="Verdana, 8.25pt" IsEndLabelVisible="False" IntervalOffsetType=Number Interval=NotSet />
                                                    <MajorGrid linecolor="64, 64, 64, 64" />
                                                </axisy>
                                                <axisx LineColor="64, 64, 64, 64"  labelautofitmaxfontsize="8" Title="BizTalk Artifacts">
                                                    <LabelStyle font="Verdana, 8.25pt" IsEndLabelVisible="False" format="" />
                                                    <MajorGrid linecolor="64, 64, 64, 64" />
                                                </axisx>
                                            </asp:ChartArea>
                                        </chartareas>
                                    </asp:Chart>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="chartFooter" runat="server" Visible="false" Width="100%"></asp:Label>
                                </td>
                            </tr>
                           </table>
                        </asp:Panel>
                    </td>
                </tr>
		        </table>
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
                    <b>Compute Scalability Index</b><br />
                    The index value ranges between 0.0 - 1.0, where 1.0 denotes the highest level of scalability. It is recommended to have
                    a scalability index around 0.6.
                    <br /><br />
                    <b>How is Scalability index determined?</b><br />
                    The scalability index is computed based on an algorithm which uses the number of hosts assigned to each BizTalk artifact.
                    <br /><br />
                    <b>How can I improve the index?</b><br />
                    It is recommended to assign several different hosts to various BizTalk artifacts by observing the graph.
                    <br /><br />
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
       <td>
	        <!-- Grid View for activity log -->
                <asp:SqlDataSource ID="ActivityDataSource2" runat="server" ConnectionString="<%$ ConnectionStrings:authStore %>"
                    SelectCommand="SELECT [Message] FROM aspnet_WebEvent_Events WHERE EventType = 'BCC.Core.BCCWebAuditEvent' and EventCode = 100505 ORDER BY EventTime DESC">
                </asp:SqlDataSource>

                <asp:GridView id="UserActivityGrid2" SkinID="sideTable" Visible="true" runat="server" Width="100%" ForeColor="#333333"
                            CellPadding="4" CellSpacing="0" AutoGenerateColumns="False" DataSourceID="ActivityDataSource2"
                        AllowPaging="True" PageSize="10" AllowSorting="True" Gridlines="Both" EmptyDataText="">
                        <Columns>
                                <asp:BoundField DataField="Message" HeaderText="Scalability index history" SortExpression="Message">
                                      <ItemStyle Width="30%" Font-Italic="false" Wrap="true"/>
                                </asp:BoundField>
                            </Columns>
                </asp:GridView>
        </td>
       </tr>
       </table>
</asp:Content>
