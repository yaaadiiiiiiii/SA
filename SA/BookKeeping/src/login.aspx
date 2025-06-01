<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="login.aspx.cs" Inherits="_BookKeeping.login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>食在當季 - 登入</title>
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

        .login-container {
            display: flex;
            flex-direction: column;
            align-items: center;
            width: 100%;
            max-width: 450px;
            position: relative;
            z-index: 1;
        }

        .login-box {
            background: rgba(255, 255, 255, 0.95);
            backdrop-filter: blur(20px);
            border-radius: 20px;
            box-shadow: 0 20px 40px rgba(0, 0, 0, 0.1);
            padding: 40px;
            width: 100%;
            border: 1px solid rgba(255, 255, 255, 0.2);
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

        .login-header {
            text-align: center;
            margin-bottom: 30px;
        }

        .system-title {
            font-size: 2.5rem;
            font-weight: 700;
            background: linear-gradient(135deg, #667eea, #764ba2);
            -webkit-background-clip: text;
            -webkit-text-fill-color: transparent;
            background-clip: text;
            margin-bottom: 8px;
        }

        .system-subtitle {
            color: #666;
            font-size: 1rem;
            font-weight: 400;
        }

        .login-form {
            width: 100%;
        }

        .form-group {
            margin-bottom: 20px;
            position: relative;
        }

        .form-label {
            display: block;
            font-weight: 600;
            color: #444;
            margin-bottom: 8px;
            font-size: 0.95rem;
            transition: color 0.3s ease;
        }

        .form-group.focused .form-label {
            color: #667eea;
        }

        .form-control {
            width: 100%;
            padding: 12px 16px;
            border: 2px solid #e1e5e9;
            border-radius: 12px;
            font-size: 1rem;
            transition: all 0.3s ease;
            background: #fff;
            font-family: inherit;
        }

        .form-control:focus {
            outline: none;
            border-color: #667eea;
            box-shadow: 0 0 0 3px rgba(102, 126, 234, 0.1);
            transform: translateY(-2px);
        }

        .form-control::placeholder {
            color: #aaa;
        }

        .btn {
            display: inline-block;
            padding: 12px 24px;
            border: none;
            border-radius: 12px;
            font-size: 1rem;
            font-weight: 600;
            cursor: pointer;
            transition: all 0.3s ease;
            text-decoration: none;
            text-align: center;
            font-family: inherit;
        }

        .btn-primary {
            background: linear-gradient(135deg, #667eea, #764ba2);
            color: white;
        }

        .btn-primary:hover {
            transform: translateY(-2px);
            box-shadow: 0 10px 20px rgba(102, 126, 234, 0.3);
        }

        .btn-block {
            width: 100%;
            margin-top: 10px;
        }

        .form-actions {
            margin-top: 20px;
        }

        .form-footer {
            text-align: center;
            margin-top: 25px;
        }

        .link-group {
            display: flex;
            align-items: center;
            justify-content: center;
            flex-wrap: wrap;
        }

        .form-link {
            color: #667eea;
            text-decoration: none;
            font-weight: 500;
            transition: color 0.3s ease;
        }

        .form-link:hover {
            color: #764ba2;
            text-decoration: underline;
        }

        .separator {
            margin: 0 10px;
            color: #ccc;
        }

        .error-message {
            color: #e74c3c;
            font-size: 0.9rem;
            margin-top: 10px;
            display: block;
            text-align: center;
            font-weight: 500;
        }

        .system-info {
            text-align: center;
            margin-top: 20px;
            color: rgba(255, 255, 255, 0.8);
            font-size: 0.9rem;
        }

        /* 響應式設計 */
        @media (max-width: 768px) {
            .login-box {
                margin: 20px;
                padding: 30px 25px;
            }

            .system-title {
                font-size: 2rem;
            }

            .link-group {
                flex-direction: column;
                gap: 10px;
            }

            .separator {
                display: none;
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
    </style>
</head>
        
<body>
    <div class="login-container">
        <div class="login-box">
            <div class="login-header">
                <h1 class="system-title">食在當季</h1>
                <p class="system-subtitle">Recipe Management System</p>
            </div>
            
            <form id="form1" runat="server" class="login-form">
                <div class="form-group">
                    <label for="UserAcc" class="form-label">帳號</label>
                    <asp:TextBox 
                        ID="UserAcc" 
                        runat="server" 
                        CssClass="form-control" 
                        placeholder="請輸入帳號"
                        autocomplete="username">
                    </asp:TextBox>
                </div>

                <div class="form-group">
                    <label for="UserPwd" class="form-label">密碼</label>
                    <asp:TextBox 
                        ID="UserPwd" 
                        runat="server" 
                        TextMode="Password"
                        CssClass="form-control" 
                        placeholder="請輸入密碼"
                        autocomplete="current-password">
                    </asp:TextBox>
                </div>

                <asp:Label ID="state" runat="server" CssClass="error-message"></asp:Label>

                <div class="form-actions">
                    <asp:Button 
                        ID="LoginButton" 
                        runat="server" 
                        Text="登入" 
                        CssClass="btn btn-primary btn-block"
                        OnClick="Login_click" />
                </div>

                <div class="form-footer">
                    <div class="link-group">
                        <asp:LinkButton 
                            ID="RegisterButton" 
                            runat="server" 
                            Text="註冊新帳號" 
                            CssClass="form-link"
                            PostBackUrl="register.aspx" />
                    </div>
                </div>
            </form>
        </div>
        
        <div class="system-info">
            <p>&copy; 2024 食在當季系統. All rights reserved.</p>
        </div>
    </div>

    <script type="text/javascript">
        // 表單互動效果
        document.addEventListener('DOMContentLoaded', function () {
            var inputs = document.querySelectorAll('.form-control');
            inputs.forEach(function (input) {
                input.addEventListener('focus', function () {
                    this.parentElement.classList.add('focused');
                });

                input.addEventListener('blur', function () {
                    this.parentElement.classList.remove('focused');
                });
            });
        });
    </script>
</body>
</html>