<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Bob.aspx.cs" Inherits="APSIM.Builds.Portal.Bob" %>
<%@ Register assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" namespace="System.Web.UI.DataVisualization.Charting" tagprefix="asp" %>

<!DOCTYPE html>


<html xmlns="http://www.w3.org/1999/xhtml">
    <head runat="server">
        <title/>
        <style>
            h1 {font-family: arial;}
            table {font-family: arial;font-size:90%;}
            #inactive-warning {color: red;}
            </style>
    </head>
    <body>
        <form id="form1" runat="server">
            <div>
                <h1>APSIM Build system</h1>
                <h2 id="inactive-warning">Note: the build system for APSIM is being moved to <a href="https://github.com/APSIMInitiative">GitHub</a>. Bob is not accepting new builds at this stage.</h2>
            </div>
            <p style="margin-left: 0px; margin-bottom: 19px">
                <asp:Button ID="UploadButton" runat="server" onclick="UploadButton_Click" enabled="false"
                            Text="Upload job" />
     Number of rows:
                <asp:TextBox ID="NumRowsTextBox" runat="server" AutoPostBack="True" 
                             ontextchanged="NumRowsTextBox_TextChanged">100</asp:TextBox>

                <asp:CheckBox ID="Passes" runat="server" 
                              oncheckedchanged="Passes_CheckedChanged" Text="Only show Passes" 
                              AutoPostBack="True" />
            </p>
            <p>
                <asp:Chart ID="Chart1" runat="server" Height="185px" Width="467px">

                    <series>

                        <asp:Series Name="Series1" Color="Red" ChartType="StackedColumn" />
                        <asp:Series ChartArea="ChartArea1" ChartType="StackedColumn" 
                                    Color="0, 192, 0" Name="Series2">
                        </asp:Series>
                    </series>
                    <chartareas>
                        <asp:ChartArea Name="ChartArea1"/>
                    </chartareas>
                    <Titles>
                        <asp:Title Name="Title1" Text="Total number of commits">
                        </asp:Title>
                    </Titles>
                </asp:Chart>
                <asp:Chart ID="Chart2" runat="server" Height="185px" Width="467px">

                    <series>
                        <asp:Series Name="Series1" Color="0, 192, 0" />
                    </series>
                    <chartareas>
                        <asp:ChartArea Name="ChartArea1" />
                    </chartareas>
                    <Titles>
                        <asp:Title Name="Title1" 
                                   Text="El-Greeno: Percentage of green patches ( &gt; 9 patches)">
                        </asp:Title>
                        <asp:Title Alignment="BottomCenter" Docking="Bottom" Name="Title3" 
                                   Text="Num of patches submitted in brackets"/>
                    </Titles>
                </asp:Chart>
                <asp:GridView ID="GridView" runat="server" 
                              CellPadding="4" ForeColor="#333333" 
                              GridLines="None" AutoGenerateColumns="False">
                    <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                    <Columns>
                        <asp:BoundField DataField="Action" HeaderText="Action" HtmlEncode="False"  />
                        <asp:BoundField DataField="User" HeaderText="User" />
                        <asp:BoundField DataField="PatchFile" HeaderText="Patch file" HtmlEncode="False" />
                        <asp:BoundField DataField="Description" HeaderText="Description"/>
                        <asp:BoundField DataField="Task" HeaderText="Task" HtmlEncode="False" />
                        <asp:BoundField DataField="Status" HeaderText="Status" HtmlEncode="False" />
                        <asp:BoundField DataField="StartTime" HeaderText="Start time"/>
                        <asp:BoundField DataField="Duration" HeaderText="Duration"/>
                        <asp:BoundField DataField="Revision" HeaderText="Revision" HtmlEncode="False" />
                        <asp:BoundField DataField="Links" HeaderText="Links" HtmlEncode="False" />

                    </Columns>
                    <EditRowStyle BackColor="#999999" />
                    <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                    <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                    <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                    <RowStyle BackColor="#F7F6F3" ForeColor="#333333" VerticalAlign="Top" />
                    <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                    <SortedAscendingCellStyle BackColor="#E9E7E2" />
                    <SortedAscendingHeaderStyle BackColor="#506C8C" />
                    <SortedDescendingCellStyle BackColor="#FFFDF8" />
                    <SortedDescendingHeaderStyle BackColor="#6F8DAE" />
                </asp:GridView>

            </p>
            <p>
            </p>
            <p>
            </p>
            <p>
            </p>
        </form>
    </body>
</html>
