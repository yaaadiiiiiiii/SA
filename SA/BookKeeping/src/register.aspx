<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="register.aspx.cs" Inherits="_BookKeeping.register" %>


<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link rel="stylesheet" type="text/css" href="styles.css" />
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>註冊</title>
</head>
<body onload="restoreFormData()">
    <div class="LogBody">
    <form class="LogForm" id="form1" runat="server">
        <h1 class="RegTitle">註冊帳號</h1>
        <div class="RegText">
            <div class="RegTextBlock">
                <p>
                   暱稱
                    <asp:TextBox class="TextBoxStyle" ID="RegNickname" runat="server" Width="90px" Height="20px" style ="margin-bottom:5px; padding-left:10px;" PlaceHolder="請輸入暱稱"></asp:TextBox>
                </p>
                
                <p>帳號
                    <asp:TextBox class="TextBoxStyle" ID="RegAcc" runat="server" Width="250px" Height="20px" style ="margin-bottom:5px; padding-left:10px;" PlaceHolder="請輸入帳號(限英文或數字)" ValidationGroup="register"></asp:TextBox>
                </p>
                <p>密碼
                    <asp:TextBox class="TextBoxStyle" ID="RegPwd" runat="server" Width="250px" Height="20px" style ="margin-bottom:5px; padding-left:10px;" PlaceHolder="請輸入6~10位密碼(限英文或數字)" OnTextChanged="TextBox4_TextChanged"></asp:TextBox>
                </p>

                <asp:HiddenField ID="HiddenAccount" runat="server" />
                <asp:HiddenField ID="HiddenPassword" runat="server" />
                <asp:HiddenField ID="HiddenField1" runat="server" />

                <p>確認密碼
                    <asp:TextBox class="TextBoxStyle" ID="ReRegPwd" runat="server" Width="250px" Height="20px" style ="margin-bottom:5px; padding-left:10px;" PlaceHolder="請再次輸入密碼"></asp:TextBox>
                </p>
            </div>       
            <div class="RegButton">
                <asp:Button class="ButtonStyle3 ButtonSize1" ID="Button1" runat="server" Text="返回"  OnClick="Button1_Click" PostBackUrl="login.aspx"/> 
                <asp:Button class="ButtonStyle3 ButtonSize1" ID="Button2" runat="server" OnClick="Button2_Click" OnClientClick="storeDate();" Text="確認" ValidationGroup="register"  />
            </div>
        </div>
        <div id="overlay"></div>
    </form>
    </div>

    <script type="text/javascript">
        // 存储表单数据到本地存储
        function storeFormData() {
            var regNickname = document.getElementById("RegNickname").value;
            var regAcc = document.getElementById("RegAcc").value;
            var regPwd = document.getElementById("RegPwd").value;
            var reRegPwd = document.getElementById("ReRegPwd").value;

            var formData = {
                regNickname: regNickname,
                regAcc: regAcc,
                regPwd: regPwd,
                reRegPwd: reRegPwd,
            };

            localStorage.setItem("formData", JSON.stringify(formData));
        }

        // 恢复表单数据
        function restoreFormData() {
            var formData = localStorage.getItem("formData");
            if (formData) {
                formData = JSON.parse(formData);

                document.getElementById("RegNickname").value = formData.regNickname;
                document.getElementById("RegAcc").value = formData.regAcc;
                document.getElementById("RegPwd").value = formData.regPwd;
                document.getElementById("ReRegPwd").value = formData.reRegPwd;
            }
        }

        var currentDate = new Date('<%= DateTime.Now.ToString("yyyy-MM-dd") %>');
        currentDate.setDate(currentDate.getDate());
        var maxDate = currentDate.toISOString().split('T')[0];
        document.getElementById("BirthDate").max = maxDate;
   
    </script>
</body>
</html>
