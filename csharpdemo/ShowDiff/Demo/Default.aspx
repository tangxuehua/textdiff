<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Demo._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Html Diff.Net</title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <h2>Text1</h2>
            <hr />
            <asp:Literal runat="server" ID="text1Literal"></asp:Literal>
            <h2>Text2</h2>
            <hr />
            <asp:Literal runat="server" ID="text2Literal"></asp:Literal>
            <h2>Diff Result</h2>
            <hr />
            <asp:Literal runat="server" ID="resultLiteral"></asp:Literal>
        </div>
    </form>
</body>
</html>
