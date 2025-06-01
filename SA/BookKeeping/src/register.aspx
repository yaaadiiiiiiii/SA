<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="register.aspx.cs" Inherits="_BookKeeping.register" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>食在當季 - 註冊</title>
    <style>
        * {
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }

        body {
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            min-height: 100vh;
            display: flex;
            align-items: center;
            justify-content: center;
            position: relative;
            overflow-x: hidden;
        }

        /* 背景裝飾動畫 */
        body::before {
            content: '';
            position: absolute;
            top: -50%;
            left: -50%;
            width: 200%;
            height: 200%;
            background: radial-gradient(circle, rgba(255,255,255,0.1) 1px, transparent 1px);
            background-size: 50px 50px;
            animation: backgroundMove 20s linear infinite;
            z-index: 0;
        }

        @keyframes backgroundMove {
            0% { transform: translate(0, 0); }
            100% { transform: translate(50px, 50px); }
        }

        .LogBody {
            display: flex;
            flex-direction: column;
            align-items: center;
            min-height: 100vh;
            width: 100%;
            position: relative;
            z-index: 1;
            padding: 20px;
        }

        .LogForm {
            background: rgba(255, 255, 255, 0.95);
            backdrop-filter: blur(20px);
            border-radius: 20px;
            box-shadow: 0 20px 40px rgba(0, 0, 0, 0.1);
            padding: 40px;
            width: 100%;
            max-width: 500px;
            border: 1px solid rgba(255, 255, 255, 0.2);
            animation: slideInUp 0.6s ease-out;
        }

        @keyframes slideInUp {
            from {
                opacity: 0;
                transform: translateY(30px);
            }
            to {
                opacity: 1;
                transform: translateY(0);
            }
        }

        .RegTitle {
            font-size: 2.5rem;
            font-weight: 700;
            background: linear-gradient(135deg, #667eea, #764ba2);
            -webkit-background-clip: text;
            -webkit-text-fill-color: transparent;
            background-clip: text;
            text-align: center;
            margin-bottom: 30px;
        }

        .RegText {
            width: 100%;
        }

        .RegTextBlock {
            margin-bottom: 25px;
        }

        .RegTextBlock p {
            margin-bottom: 15px;
            font-weight: 600;
            color: #444;
            font-size: 0.95rem;
            display: flex;
            flex-direction: column;
            gap: 8px;
            transition: color 0.3s ease;
        }

        .RegTextBlock p:focus-within {
            color: #667eea;
        }

        .TextBoxStyle {
            width: 100% !important;
            max-width: 100% !important;
            padding: 12px 16px !important;
            border: 2px solid #e1e5e9 !important;
            border-radius: 12px !important;
            font-size: 1rem !important;
            transition: all 0.3s ease !important;
            background: #fff !important;
            font-family: inherit !important;
            margin: 0 !important;
            height: auto !important;
        }

        .TextBoxStyle:focus {
            outline: none !important;
            border-color: #667eea !important;
            box-shadow: 0 0 0 3px rgba(102, 126, 234, 0.1) !important;
            transform: translateY(-2px) !important;
        }

        .TextBoxStyle::placeholder {
            color: #aaa !important;
        }

        .RegButton {
            display: flex;
            gap: 15px;
            margin-top: 25px;
            justify-content: center;
        }

        .ButtonStyle3 {
            padding: 12px 24px !important;
            border: none !important;
            border-radius: 12px !important;
            font-size: 1rem !important;
            font-weight: 600 !important;
            cursor: pointer !important;
            transition: all 0.3s ease !important;
            text-decoration: none !important;
            text-align: center !important;
            font-family: inherit !important;
            min-width: 120px !important;
            height: auto !important;
        }

        .ButtonSize1 {
            /* 按鈕尺寸由 ButtonStyle3 控制 */
        }

        /* 返回按鈕樣式 */
        .RegButton .ButtonStyle3:first-child {
            background: #f8f9fa !important;
            color: #667eea !important;
            border: 2px solid #667eea !important;
        }

        .RegButton .ButtonStyle3:first-child:hover {
            background: #667eea !important;
            color: white !important;
            transform: translateY(-2px) !important;
        }

        /* 確認按鈕樣式 */
        .RegButton .ButtonStyle3:last-child {
            background: linear-gradient(135deg, #667eea, #764ba2) !important;
            color: white !important;
        }

        .RegButton .ButtonStyle3:last-child:hover {
            transform: translateY(-2px) !important;
            box-shadow: 0 10px 20px rgba(102, 126, 234, 0.3) !important;
        }

        /* 隱藏欄位 */
        input[type="hidden"] {
            display: none;
        }

        /* 覆蓋層樣式（如果需要） */
        #overlay {
            display: none;
        }

        .system-info {
            text-align: center;
            margin-top: 20px;
            color: rgba(255, 255, 255, 0.8);
            font-size: 0.9rem;
        }

        /* 表單驗證樣式 */
        .validation-error {
            color: #e74c3c;
            font-size: 0.85rem;
            margin-top: 5px;
            display: block;
        }

        /* 響應式設計 */
        @media (max-width: 768px) {
            .LogBody {
                padding: 15px;
            }

            .LogForm {
                padding: 30px 25px;
                max-width: none;
            }

            .RegTitle {
                font-size: 2rem;
            }

            .RegButton {
                flex-direction: column;
                gap: 10px;
            }

            .ButtonStyle3 {
                width: 100% !important;
            }
        }

        /* 美化滾動條 */
        ::-webkit-scrollbar {
            width: 8px;
        }

        ::-webkit-scrollbar-track {
            background: rgba(255, 255, 255, 0.1);
            border-radius: 4px;
        }

        ::-webkit-scrollbar-thumb {
            background: rgba(102, 126, 234, 0.5);
            border-radius: 4px;
        }

        ::-webkit-scrollbar-thumb:hover {
            background: rgba(102, 126, 234, 0.7);
        }

        /* 載入動畫 */
        .fade-in {
            animation: fadeIn 0.6s ease-out;
        }

        @keyframes fadeIn {
            from {
                opacity: 0;
                transform: translateY(20px);
            }
            to {
                opacity: 1;
                transform: translateY(0);
            }
        }
    </style>
</head>
<body onload="restoreFormData()">
    <div class="LogBody">
        <form class="LogForm" id="form1" runat="server">
            <h1 class="RegTitle">註冊帳號</h1>
            <div class="RegText">
                <div class="RegTextBlock">
                    <p>
                       暱稱
                        <asp:TextBox 
                            class="TextBoxStyle" 
                            ID="RegNickname" 
                            runat="server" 
                            PlaceHolder="請輸入暱稱">
                        </asp:TextBox>
                    </p>
                    
                    <p>
                        帳號
                        <asp:TextBox 
                            class="TextBoxStyle" 
                            ID="RegAcc" 
                            runat="server" 
                            PlaceHolder="請輸入帳號(限英文或數字)" 
                            ValidationGroup="register">
                        </asp:TextBox>
                    </p>
                    
                    <p>
                        密碼
                        <asp:TextBox 
                            class="TextBoxStyle" 
                            ID="RegPwd" 
                            runat="server" 
                            TextMode="Password"
                            PlaceHolder="請輸入6~10位密碼(限英文或數字)" 
                            OnTextChanged="TextBox4_TextChanged">
                        </asp:TextBox>
                    </p>

                    <asp:HiddenField ID="HiddenAccount" runat="server" />
                    <asp:HiddenField ID="HiddenPassword" runat="server" />
                    <asp:HiddenField ID="HiddenField1" runat="server" />

                    <p>
                        確認密碼
                        <asp:TextBox 
                            class="TextBoxStyle" 
                            ID="ReRegPwd" 
                            runat="server" 
                            TextMode="Password"
                            PlaceHolder="請再次輸入密碼">
                        </asp:TextBox>
                    </p>
                </div>       
                
                <div class="RegButton">
                    <asp:Button 
                        class="ButtonStyle3 ButtonSize1" 
                        ID="Button1" 
                        runat="server" 
                        Text="返回"  
                        OnClick="Button1_Click" 
                        PostBackUrl="login.aspx"
                        CausesValidation="false" /> 
                        
                    <asp:Button 
                        class="ButtonStyle3 ButtonSize1" 
                        ID="Button2" 
                        runat="server" 
                        OnClick="Button2_Click" 
                        OnClientClick="storeDate();" 
                        Text="確認註冊" 
                        ValidationGroup="register" />
                </div>
            </div>
            <div id="overlay"></div>
        </form>
        
        <div class="system-info">
            <p>&copy; 2024 食在當季系統. All rights reserved.</p>
        </div>
    </div>

    <script type="text/javascript">
        // 存储表单数据到本地存储
        function storeFormData() {
            try {
                var regNickname = document.getElementById("<%=RegNickname.ClientID%>").value;
                var regAcc = document.getElementById("<%=RegAcc.ClientID%>").value;
                var regPwd = document.getElementById("<%=RegPwd.ClientID%>").value;
                var reRegPwd = document.getElementById("<%=ReRegPwd.ClientID%>").value;

                var formData = {
                    regNickname: regNickname,
                    regAcc: regAcc,
                    regPwd: regPwd,
                    reRegPwd: reRegPwd
                };

                localStorage.setItem("formData", JSON.stringify(formData));
            } catch (e) {
                console.log("無法存取 localStorage:", e);
            }
        }

        // 確保函數名稱正確
        function storeDate() {
            storeFormData();
        }

        // 恢复表单数据
        function restoreFormData() {
            try {
                var formData = localStorage.getItem("formData");
                if (formData) {
                    formData = JSON.parse(formData);

                    var regNickname = document.getElementById("<%=RegNickname.ClientID%>");
                    var regAcc = document.getElementById("<%=RegAcc.ClientID%>");
                    var regPwd = document.getElementById("<%=RegPwd.ClientID%>");
                    var reRegPwd = document.getElementById("<%=ReRegPwd.ClientID%>");

                    if (regNickname && formData.regNickname) regNickname.value = formData.regNickname;
                    if (regAcc && formData.regAcc) regAcc.value = formData.regAcc;
                    if (regPwd && formData.regPwd) regPwd.value = formData.regPwd;
                    if (reRegPwd && formData.reRegPwd) reRegPwd.value = formData.reRegPwd;
                }
            } catch(e) {
                console.log("無法讀取 localStorage:", e);
            }
        }

        // 表單互動效果
        document.addEventListener('DOMContentLoaded', function() {
            var inputs = document.querySelectorAll('.TextBoxStyle');
            inputs.forEach(function(input) {
                input.addEventListener('focus', function() {
                    this.closest('p').style.color = '#667eea';
                });
                
                input.addEventListener('blur', function() {
                    this.closest('p').style.color = '#444';
                });
            });
        });

    </script>
</body>
</html>