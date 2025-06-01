<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ingredient_manage.aspx.cs" Inherits="_BookKeeping.ingredient_manage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>食材管理系統</title>
    <style>
        * {
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }

        body {
            font-family: 'Microsoft JhengHei', Arial, sans-serif;
            background: linear-gradient(135deg, #4CAF50 0%, #45a049 100%);
            min-height: 100vh;
            padding: 20px;
        }

        .container {
            max-width: 1200px;
            margin: 0 auto;
            background: rgba(255, 255, 255, 0.95);
            border-radius: 20px;
            padding: 30px;
            box-shadow: 0 20px 40px rgba(0,0,0,0.1);
        }

        .header {
            text-align: center;
            margin-bottom: 40px;
            color: #333;
        }

        .header h1 {
            font-size: 2.5em;
            margin-bottom: 10px;
            color: #4CAF50;
        }

        .function-tabs {
            display: flex;
            justify-content: center;
            margin-bottom: 30px;
            gap: 10px;
        }

        .tab-btn {
            background: #6c757d;
            border: none;
            border-radius: 10px;
            padding: 15px 30px;
            color: white;
            font-size: 1.1em;
            font-weight: bold;
            cursor: pointer;
            transition: all 0.3s ease;
        }

        .tab-btn.active {
            background: #4CAF50;
            transform: translateY(-2px);
        }

        .tab-btn:hover {
            background: #45a049;
            transform: translateY(-2px);
        }

        .tab-content {
            display: none;
            background: #f8f9fa;
            border-radius: 15px;
            padding: 30px;
            margin-bottom: 20px;
        }

        .tab-content.active {
            display: block;
        }

        .form-group {
            margin-bottom: 20px;
        }

        .form-group label {
            display: block;
            margin-bottom: 8px;
            font-weight: bold;
            color: #333;
        }

        .form-control {
            width: 100%;
            padding: 12px;
            border: 2px solid #ddd;
            border-radius: 8px;
            font-size: 1em;
            transition: border-color 0.3s ease;
        }

        .form-control:focus {
            border-color: #4CAF50;
            outline: none;
        }

        .btn-primary {
            background: linear-gradient(145deg, #4CAF50, #45a049);
            border: none;
            border-radius: 10px;
            padding: 12px 30px;
            color: white;
            font-size: 1.1em;
            font-weight: bold;
            cursor: pointer;
            transition: all 0.3s ease;
            margin-right: 10px;
        }

        .btn-primary:hover {
            transform: translateY(-2px);
            box-shadow: 0 8px 20px rgba(76, 175, 80, 0.3);
        }

        .btn-success {
            background: linear-gradient(145deg, #28a745, #20c997);
            border: none;
            border-radius: 10px;
            padding: 12px 30px;
            color: white;
            font-size: 1.1em;
            font-weight: bold;
            cursor: pointer;
            transition: all 0.3s ease;
            margin-right: 10px;
        }

        .btn-danger {
            background: linear-gradient(145deg, #dc3545, #c82333);
            border: none;
            border-radius: 10px;
            padding: 12px 30px;
            color: white;
            font-size: 1.1em;
            font-weight: bold;
            cursor: pointer;
            transition: all 0.3s ease;
            margin-right: 10px;
        }

        .btn-back {
            background: #6c757d;
            border: none;
            border-radius: 10px;
            padding: 10px 20px;
            color: white;
            font-size: 1em;
            cursor: pointer;
            transition: all 0.3s ease;
            position: absolute;
            top: 20px;
            left: 20px;
        }

        .btn-back:hover {
            background: #5a6268;
        }

        .message {
            padding: 15px;
            border-radius: 10px;
            margin-bottom: 20px;
            text-align: center;
            font-weight: bold;
        }

        .message.success {
            background: #d4edda;
            color: #155724;
            border: 1px solid #c3e6cb;
        }

        .message.error {
            background: #f8d7da;
            color: #721c24;
            border: 1px solid #f5c6cb;
        }

        .form-row {
            display: flex;
            gap: 20px;
            align-items: end;
        }

        .form-row .form-group {
            flex: 1;
        }

        .ingredient-preview {
            background: white;
            border-radius: 10px;
            padding: 15px;
            margin: 10px 0;
            border-left: 4px solid #4CAF50;
        }

        @media (max-width: 768px) {
            .function-tabs {
                flex-direction: column;
                align-items: center;
            }
            
            .tab-btn {
                width: 200px;
                margin-bottom: 10px;
            }

            .form-row {
                flex-direction: column;
            }
        }
    </style>
    <script>
        function showTab(tabName) {
            // 隱藏所有分頁內容
            var contents = document.querySelectorAll('.tab-content');
            contents.forEach(function (content) {
                content.classList.remove('active');
            });

            // 移除所有按鈕的 active 類別
            var buttons = document.querySelectorAll('.tab-btn');
            buttons.forEach(function (btn) {
                btn.classList.remove('active');
            });

            // 顯示選中的分頁內容
            var targetTab = document.getElementById(tabName + 'Tab');
            if (targetTab) {
                targetTab.classList.add('active');
            }

            // 為對應的按鈕加上 active 類別
            if (event && event.target) {
                event.target.classList.add('active');
            } else {
                var targetButton = document.querySelector(`button[onclick*="showTab('${tabName}')"]`);
                if (targetButton) {
                    targetButton.classList.add('active');
                }
            }

            // 儲存當前分頁狀態到 hidden field
            var hiddenField = document.getElementById('<%= CurrentTab.ClientID %>');
            if (hiddenField) {
                hiddenField.value = tabName;
            }
        }

        function confirmDelete() {
            return confirm('確定要刪除這個食材嗎？此操作無法復原。');
        }

        function showTabByName(tabName) {
            // 隱藏所有分頁內容
            var contents = document.querySelectorAll('.tab-content');
            contents.forEach(function (content) {
                content.classList.remove('active');
            });

            // 移除所有按鈕的 active 類別
            var buttons = document.querySelectorAll('.tab-btn');
            buttons.forEach(function (btn) {
                btn.classList.remove('active');
            });

            // 顯示選中的分頁內容
            var targetTab = document.getElementById(tabName + 'Tab');
            if (targetTab) {
                targetTab.classList.add('active');
            }

            // 為對應的按鈕加上 active 類別
            var targetButton = document.querySelector(`button[onclick*="showTab('${tabName}')"]`);
            if (targetButton) {
                targetButton.classList.add('active');
            }
        }

        // 初始化頁籤顯示
        function initializeTabs() {
            var hiddenField = document.getElementById('<%= CurrentTab.ClientID %>');
            var currentTab = hiddenField ? hiddenField.value : 'add';

            if (!currentTab || currentTab === '') {
                currentTab = 'add';
            }

            showTabByName(currentTab);
        }

        // 頁面完全載入後執行
        window.onload = function () {
            initializeTabs();
        };

        // 處理 UpdatePanel 的 partial postback
        function pageLoad() {
            initializeTabs();
        }

        // 註冊 UpdatePanel 的事件處理
        if (typeof (Sys) !== "undefined") {
            Sys.Application.add_load(pageLoad);
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
        <asp:Button ID="BackBtn" runat="server" CssClass="btn-back" Text="← 返回主頁" OnClick="BackBtn_Click" />
        
        <div class="container">
            <div class="header">
                <h1>🥬 食材管理系統</h1>
                <p>管理您的食譜食材配方</p>
            </div>

            <!-- 功能分頁按鈕 -->
            <div class="function-tabs">
                <button type="button" class="tab-btn" onclick="showTab('add')">新增食材</button>
                <button type="button" class="tab-btn" onclick="showTab('edit')">修改食材</button>
                <button type="button" class="tab-btn" onclick="showTab('delete')">刪除食材</button>
            </div>

            <asp:HiddenField ID="CurrentTab" runat="server" Value="add" />

            <!-- 新增食材分頁 -->
            <div id="addTab" class="tab-content">
                <h2>📝 新增食材</h2>
                
                <!-- 新增功能的訊息顯示區域 -->
                <asp:Panel ID="AddMessagePanel" runat="server" Visible="false">
                    <div id="addMessageDiv" runat="server" class="message">
                        <asp:Label ID="AddMessageLabel" runat="server"></asp:Label>
                    </div>
                </asp:Panel>
                
                <div class="form-group">
                    <label>選擇食譜：</label>
                    <asp:DropDownList ID="AddRecipeDropDown" runat="server" CssClass="form-control">
                    </asp:DropDownList>
                </div>
                <div class="form-row">
                    <div class="form-group">
                        <label>食材名稱：</label>
                        <asp:TextBox ID="AddFoodName" runat="server" CssClass="form-control" placeholder="請輸入食材名稱"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <label>數量：</label>
                        <asp:TextBox ID="AddQuantity" runat="server" CssClass="form-control" placeholder="如：200g、2顆、適量"></asp:TextBox>
                    </div>
                </div>
                <div class="form-group">
                    <asp:Button ID="SaveAddBtn" runat="server" CssClass="btn-success" Text="💾 新增食材" OnClick="SaveAddBtn_Click" />
                </div>
            </div>

            <!-- 修改食材分頁 -->
            <div id="editTab" class="tab-content">
                <h2>✏️ 修改食材</h2>
    
                <asp:UpdatePanel ID="EditUpdatePanel" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <!-- 修改功能的訊息顯示區域 -->
                        <asp:Panel ID="EditMessagePanel" runat="server" Visible="false">
                            <div id="editMessageDiv" runat="server" class="message">
                                <asp:Label ID="EditMessageLabel" runat="server"></asp:Label>
                            </div>
                        </asp:Panel>
                        
                        <div class="form-group">
                            <label>選擇要修改的食材：</label>
                            <asp:DropDownList ID="EditIngredientDropDown" runat="server" CssClass="form-control" 
                                AutoPostBack="true" OnSelectedIndexChanged="EditIngredientDropDown_SelectedIndexChanged">
                            </asp:DropDownList>
                        </div>
                        <div class="form-group">
                            <label>所屬食譜：</label>
                            <asp:DropDownList ID="EditRecipeDropDown" runat="server" CssClass="form-control">
                            </asp:DropDownList>
                        </div>
                        <div class="form-row">
                            <div class="form-group">
                                <label>食材名稱：</label>
                                <asp:TextBox ID="EditFoodName" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label>數量：</label>
                                <asp:TextBox ID="EditQuantity" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                        <div class="form-group">
                            <asp:Button ID="SaveEditBtn" runat="server" CssClass="btn-success" 
                                Text="💾 儲存修改" OnClick="SaveEditBtn_Click" />
                        </div>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="EditIngredientDropDown" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="SaveEditBtn" EventName="Click" />
                    </Triggers>
                </asp:UpdatePanel>
            </div>

            <!-- 刪除食材分頁 -->
            <div id="deleteTab" class="tab-content">
                <h2>🗑️ 刪除食材</h2>
    
                <asp:UpdatePanel ID="DeleteUpdatePanel" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <!-- 刪除功能的訊息顯示區域 -->
                        <asp:Panel ID="DeleteMessagePanel" runat="server" Visible="false">
                            <div id="deleteMessageDiv" runat="server" class="message">
                                <asp:Label ID="DeleteMessageLabel" runat="server"></asp:Label>
                            </div>
                        </asp:Panel>
                        
                        <div class="form-group">
                            <label>選擇要刪除的食材：</label>
                            <asp:DropDownList ID="DeleteIngredientDropDown" runat="server" CssClass="form-control" 
                                AutoPostBack="true" OnSelectedIndexChanged="DeleteIngredientDropDown_SelectedIndexChanged">
                            </asp:DropDownList>
                        </div>
                        <asp:Panel ID="DeletePreviewPanel" runat="server" Visible="false">
                            <div class="ingredient-preview">
                                <div class="ingredient-title">
                                    <asp:Label ID="DeletePreviewName" runat="server"></asp:Label>
                                </div>
                                <div class="ingredient-recipe">
                                    所屬食譜：<asp:Label ID="DeletePreviewRecipe" runat="server"></asp:Label>
                                </div>
                                <div class="ingredient-quantity">
                                    數量：<asp:Label ID="DeletePreviewQuantity" runat="server"></asp:Label>
                                </div>
                            </div>
                        </asp:Panel>
                        <div class="form-group">
                            <asp:Button ID="ConfirmDeleteBtn" runat="server" CssClass="btn-danger" 
                                Text="🗑️ 確認刪除" OnClick="ConfirmDeleteBtn_Click" 
                                OnClientClick="return confirmDelete();" />
                        </div>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="DeleteIngredientDropDown" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="ConfirmDeleteBtn" EventName="Click" />
                    </Triggers>
                </asp:UpdatePanel>
            </div>
        </div>
    </form>
</body>
</html>