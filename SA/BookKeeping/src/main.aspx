<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="main.aspx.cs" Inherits="_BookKeeping.main" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>食在當季 - 食譜推薦系統</title>
    <style>
        * {
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }

        body {
            font-family: 'Microsoft JhengHei', Arial, sans-serif;
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            min-height: 100vh;
            display: flex;
            flex-direction: column;
            align-items: center;
            padding: 20px;
        }

        .header {
            text-align: center;
            margin-bottom: 50px;
            color: white;
            text-shadow: 2px 2px 4px rgba(0,0,0,0.3);
        }

        .header h1 {
            font-size: 3.5em;
            margin-bottom: 10px;
            font-weight: bold;
        }

        .header p {
            font-size: 1.3em;
            opacity: 0.9;
        }

        .welcome-section {
            background: rgba(255, 255, 255, 0.15);
            backdrop-filter: blur(10px);
            border-radius: 20px;
            padding: 20px 30px;
            margin-bottom: 40px;
            border: 1px solid rgba(255, 255, 255, 0.2);
        }

        .welcome-text {
            color: white;
            font-size: 1.2em;
            text-align: center;
        }

        .main-container {
            background: rgba(255, 255, 255, 0.95);
            border-radius: 25px;
            padding: 50px;
            box-shadow: 0 20px 40px rgba(0,0,0,0.1);
            max-width: 800px;
            width: 100%;
        }

        .function-buttons {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
            gap: 30px;
            margin-top: 30px;
        }

        .function-btn {
            background: linear-gradient(145deg, #ff6b6b, #ee5a24);
            border: none;
            border-radius: 20px;
            padding: 30px 20px;
            font-size: 1.3em;
            font-weight: bold;
            color: white;
            cursor: pointer;
            transition: all 0.3s ease;
            box-shadow: 0 8px 25px rgba(255, 107, 107, 0.3);
            position: relative;
            overflow: hidden;
        }

        .function-btn:nth-child(2) {
            background: linear-gradient(145deg, #4ecdc4, #44a08d);
            box-shadow: 0 8px 25px rgba(78, 205, 196, 0.3);
        }

        .function-btn:nth-child(3) {
            background: linear-gradient(145deg, #feca57, #ff9ff3);
            box-shadow: 0 8px 25px rgba(254, 202, 87, 0.3);
        }

        .function-btn:hover {
            transform: translateY(-5px);
            box-shadow: 0 15px 35px rgba(0,0,0,0.2);
        }

        .function-btn:active {
            transform: translateY(-2px);
        }

        .function-btn::before {
            content: '';
            position: absolute;
            top: 0;
            left: -100%;
            width: 100%;
            height: 100%;
            background: linear-gradient(90deg, transparent, rgba(255,255,255,0.3), transparent);
            transition: left 0.5s;
        }

        .function-btn:hover::before {
            left: 100%;
        }

        .btn-icon {
            font-size: 2em;
            margin-bottom: 10px;
            display: block;
        }

        .logout-section {
            margin-top: 40px;
            text-align: center;
        }

        .logout-btn {
            background: rgba(108, 117, 125, 0.8);
            border: none;
            border-radius: 25px;
            padding: 12px 30px;
            color: white;
            font-size: 1em;
            cursor: pointer;
            transition: all 0.3s ease;
        }

        .logout-btn:hover {
            background: rgba(108, 117, 125, 1);
            transform: translateY(-2px);
        }

        @media (max-width: 768px) {
            .header h1 {
                font-size: 2.5em;
            }
            
            .main-container {
                padding: 30px 20px;
            }
            
            .function-buttons {
                grid-template-columns: 1fr;
                gap: 20px;
            }
            
            body {
                padding: 10px;
            }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="header">
            <h1>🍳 食在當季</h1>
            <p>食譜推薦系統</p>
        </div>

        <div class="welcome-section">
            <div class="welcome-text">
                歡迎回來，<asp:Label ID="UserNameLabel" runat="server" Text="用戶"></asp:Label>！
                <br />今天想要嘗試什麼美味料理呢？
            </div>
        </div>

        <div class="main-container">
            <div class="function-buttons">
                <asp:Button ID="AddRecipeBtn" runat="server" CssClass="function-btn" 
                    Text='新增食譜' 
                    OnClick="AddRecipeBtn_Click" />
                
                <asp:Button ID="BrowseRecipeBtn" runat="server" CssClass="function-btn" 
                    Text='瀏覽食譜' 
                    OnClick="BrowseRecipeBtn_Click" />
                
                <asp:Button ID="FavoriteBtn" runat="server" CssClass="function-btn" 
                    Text='我的最愛' 
                    OnClick="FavoriteBtn_Click" />
            </div>

            <div class="logout-section">
                <asp:Button ID="LogoutBtn" runat="server" CssClass="logout-btn" 
                    Text="登出" OnClick="LogoutBtn_Click" />
            </div>
        </div>
    </form>
</body>
</html>