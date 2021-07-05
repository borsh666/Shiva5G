<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebFormClient.aspx.cs" Inherits="WebClient.WebFormClient" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>

     <script>
                function setHourglass() {
                    document.body.style.cursor = 'wait';
                }
    </script>
    <style type="text/css">
        .auto-style1 {
            position: absolute;
            top: 684px;
            left: 26px;
            z-index: 100;
             width: 119px;
            height: 54px;
        }
        .auto-style3 {
            position: absolute;
            top: 680px;
            left: 201px;
            z-index: 100;
            width: 119px;
            height: 54px;
        }
        .auto-style5 {
            position: absolute;
            top: 682px;
            left: 382px;
            z-index: 100;
           width: 119px;
            height: 54px;
        }
        .auto-style6 {
            position: absolute;
            top: 538px;
            left: 74px;
            z-index: 100;
            width: 90px;
            height: 18px;
        }
        .auto-style7 {
            position: absolute;
            top: 540px;
            left: 20px;
            z-index: 100;
        }
        .auto-style8 {
            position: absolute;
            top: 508px;
            left: 84px;
            z-index: 100;
        }
        .auto-style9 {
            position: absolute;
            top: 66px;
            left: 5px;
            z-index: 1;
            width: 531px;
            height: 702px;
        }
        .auto-style10 {
            position: absolute;
            top: 625px;
            left: 217px;
            z-index: 100;
            width: 101px;
        }
        .auto-style11 {
            position: absolute;
            top: 77px;
            left: 629px;
            z-index: 1;
        }
        .auto-style12 {
            position: absolute;
            top: 103px;
            left: 625px;
            z-index: 1;
            width: 565px;
            height: 168px;
        }
        .auto-style14 {
            position: absolute;
            top: 403px;
            left: 632px;
            z-index: 1;
            width: 554px;
            height: 247px;
        }
        .auto-style15 {
            position: absolute;
            top: 340px;
            left: 632px;
            z-index: 1;
            width: 120px;
            height: 52px;
        }
        .auto-style16 {
            position: absolute;
            top: 341px;
            left: 858px;
            z-index: 1;
            width: 321px;
        }
        .auto-style17 {
            position: absolute;
            top: 341px;
            left: 805px;
            z-index: 1;
        }
        .auto-style18 {
            position: absolute;
            top: 370px;
            left: 858px;
            z-index: 1;
            width: 318px;
        }
        .auto-style19 {
            position: absolute;
            top: 373px;
            left: 799px;
            z-index: 1;
            width: 61px;
            right: 538px;
        }
        .auto-style20 {
            position: absolute;
            top: 575px;
            left: 388px;
            z-index: 1;
            width: 119px;
            height: 54px;
        }
        .auto-style22 {
            position: absolute;
            top: 578px;
            left: 24px;
            z-index: 100;
            width: 119px;
            height: 53px;
            bottom: 521px;
        }
        .auto-style23 {
            position: absolute;
            top: 106px;
            left: 1211px;
            z-index: 1;
            width: 305px;
            height: 290px;
        }
    </style>
</head>
<body>   
        <form id="form1" runat="server">
        <div>
    
        </div>
           
            <asp:Button ID="ButtonIRFC" runat="server" CssClass="auto-style1" Text="IRFC" OnClick="ButtonIRFC_Click" Font-Bold="True"  />
            <asp:Button ID="ButtonSRF" runat="server" CssClass="auto-style3" Text="SRF" OnClick="ButtonSRF_Click" Font-Bold="True" />
            <asp:Button ID="ButtonSA" runat="server" CssClass="auto-style5" Text="SA" OnClick="ButtonSA_Click" Font-Bold="True" />
            <asp:TextBox ID="TextBox1" runat="server" CssClass="auto-style6" OnTextChanged="TextBox1_TextChanged"></asp:TextBox>
            <asp:Label ID="Label1" runat="server" CssClass="auto-style7" Text="SiteID" BackColor="#CCCCCC"></asp:Label>
            <asp:Label ID="Label2" runat="server" CssClass="auto-style8" Text="ex. SO1513" BackColor="#CCCCCC"></asp:Label>
            <asp:Label ID="Label3" runat="server" CssClass="auto-style10" Font-Names="Monotype Corsiva" Text="New Delhi Team" BackColor="#CCCCCC"></asp:Label>
            <asp:TextBox ID="TextBoxMailFrom" runat="server" CssClass="auto-style16"></asp:TextBox>
            <asp:TextBox ID="TextBoxMailSubject" runat="server" CssClass="auto-style18"></asp:TextBox>
            <asp:Button ID="ButtonSRF_2600" runat="server" CssClass="auto-style22" Font-Bold="True" Text="SRF 2600" OnClick="ButtonSRF_2600_Click" />
            <asp:Image ID="Image2" runat="server" CssClass="auto-style23" ImageAlign="Top" ImageUrl="~/viber_image_2020-03-27_09-31-02.jpg" />
            <asp:Image ID="Image1" runat="server" CssClass="auto-style9" ImageUrl="~/shiva391x480.gif" />
            <asp:Label ID="Label4" runat="server" CssClass="auto-style11" Text="News"></asp:Label>
            <asp:TextBox ID="TextBoxNewsBody" runat="server" CssClass="auto-style12" TextMode="MultiLine">Greetings! Shiva is on WEB already :)</asp:TextBox>
            <asp:TextBox ID="TextBoxMailBody" runat="server" CssClass="auto-style14" TextMode="MultiLine">Dear NDT,</asp:TextBox>
            <asp:Button ID="ButtonSendMail" runat="server" CssClass="auto-style15" OnClick="ButtonSendMail_Click" Text="Send Mail to NDT" Font-Bold="True" />
            <asp:Label ID="Label5" runat="server" CssClass="auto-style17" Text="From"></asp:Label>
            <p>
                &nbsp;</p>
            <asp:Label ID="Label6" runat="server" CssClass="auto-style19" Text="Subject"></asp:Label>
            <asp:Button ID="ButtonPSK" runat="server" CssClass="auto-style20" Font-Bold="True" OnClick="ButtonPSK_Click" Text="PSK" />
        </form>
   
</body>
</html>
