<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Advanced.aspx.cs" Inherits="APSIM.Builds.Portal.Advanced" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
       <asp:Label ID="Label1" runat="server" Text="JobID:"></asp:Label>
       <asp:TextBox ID="JobID" runat="server"></asp:TextBox>
       <br />
       <asp:Label ID="Label2" runat="server" Text="New status:"></asp:Label>
       <asp:TextBox ID="NewStatus" runat="server"></asp:TextBox>
       <asp:Button ID="Button1" runat="server" onclick="Button1_Click" 
          Text="Change status" />
    
    </div>
    </form>
</body>
</html>

