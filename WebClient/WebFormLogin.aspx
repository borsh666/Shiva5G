<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebFormLogin.aspx.cs" Inherits="WebClient.WebFormLogin" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        .auto-style1 {
            position: absolute;
            top: 83px;
            left: 138px;
            z-index: 1;
        }
        .auto-style2 {
            position: absolute;
            top: 212px;
            left: 143px;
            z-index: 1;
           
        }
        .auto-style3 {
            position: absolute;
            top: 296px;
            left: 142px;
            z-index: 1;
           
        }
        .auto-style4 {
            position: absolute;
            top: 388px;
            left: 140px;
            z-index: 1;
        }
        .auto-style5 {
            position: absolute;
            top: 185px;
            left: 141px;
            z-index: 1;
        }
        .auto-style6 {
            position: absolute;
            top: 269px;
            left: 143px;
            z-index: 1;
            width: 41px;
            height: 18px;
        }
        .auto-style7 {
            position: absolute;
            top: 368px;
            left: 142px;
            z-index: 1;
        }
        .auto-style9 {
            height: 53px;
        }
        .auto-style10 {
            position: absolute;
            top: 283px;
            left: 357px;
            z-index: 1;
            width: 105px;
            height: 54px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server" class="auto-style9">
    <div>
    
    </div>
        <asp:Label ID="Label1" runat="server" CssClass="auto-style1" Font-Bold="True" Font-Size="XX-Large" Text="Shiva Registration"></asp:Label>

        <asp:TextBox ID="TextBoxEmail" runat="server" CssClass="auto-style3" OnTextChanged="TextBoxEmail_TextChanged"></asp:TextBox>
        <asp:TextBox ID="TextBoxPhone" runat="server" CssClass="auto-style4" OnTextChanged="TextBoxPhone_TextChanged"></asp:TextBox>
        <asp:Label ID="Label2" runat="server" CssClass="auto-style5" Text="Name and Surname"></asp:Label>
        <asp:Label ID="Label3" runat="server" CssClass="auto-style6" Text="Email"></asp:Label>
        <asp:Label ID="Label4" runat="server" CssClass="auto-style7" Text="Phone"></asp:Label>
        <asp:TextBox ID="TextBoxName" runat="server" CssClass="auto-style2" OnTextChanged="TextBoxName_TextChanged"></asp:TextBox>
        <asp:Button ID="ButtonSubmit" runat="server" CssClass="auto-style10" OnClick="ButtonSubmit_Click1" Text="Submit" />
    </form>
</body>
</html>
