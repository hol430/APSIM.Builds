﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DeleteJob.aspx.cs" Inherits="APSIM.Builds.Portal.DeleteJob" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
       <asp:Label ID="Label" runat="server" 
          Text="Are you sure you want to delete job?"></asp:Label>
    
    </div>
    <p>
       <asp:Button ID="Yes" runat="server" onclick="Yes_Click" Text="Yes" />
       <asp:Button ID="No" runat="server" onclick="No_Click" Text="No" />
    </p>
    </form>
</body>
</html>
