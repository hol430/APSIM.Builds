<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Validation.aspx.cs" Inherits="APSIM.Builds.Portal.Validation" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        <asp:Label ID="Label1" runat="server" Text="Version:"></asp:Label>
        <asp:DropDownList ID="versionList" runat="server" OnTextChanged="OnVersionListChanged" Height="20px" Width="131px" AutoPostBack="True">
        </asp:DropDownList>
        <asp:Label ID="Label2" runat="server" Text="Module:"></asp:Label>
        <asp:DropDownList ID="moduleList" runat="server" OnTextChanged="OnModuleListChanged" Height="17px" Width="148px" AutoPostBack="True">
        </asp:DropDownList>



        <asp:literal ID="literal" runat="server"></asp:literal>
            
    </div>
    </form>
</body>
</html>
