<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="recipe_manage.aspx.cs" Inherits="_BookKeeping.recipe_manage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>食譜管理系統</title>
    <style>
        * {
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }

        body {
            font-family: 'Microsoft JhengHei', Arial, sans-serif;
            background: linear-gradient(135deg, #FF6B6B 0%, #FF8E53 100%);
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
            color: #FF6B6B;
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
            background: #FF6B6B;
            transform: translateY(-2px);
        }

        .tab-btn:hover {
            background: #FF8E53;
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
            border-color: #FF6B6B;
            outline: none;
        }

        .form-control-textarea {
            height: 100px;
            resize: vertical;
        }

        .form-control-steps {
            height: 150px;
            resize: vertical;
        }

        .btn-primary {
            background: linear-gradient(145deg, #FF6B6B, #FF8E53);
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
            box-shadow: 0 8px 20px rgba(255, 107, 107, 0.3);
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

        .recipe-preview {
            background: white;
            border-radius: 10px;
            padding: 20px;
            margin: 15px 0;
            border-left: 4px solid #FF6B6B;
            box-shadow: 0 2px 10px rgba(0,0,0,0.1);
        }

        .recipe-preview h4 {
            color: #FF6B6B;
            margin-bottom: 10px;
            font-size: 1.3em;
        }

        .recipe-preview .description {
            color: #666;
            margin-bottom: 10px;
            font-style: italic;
        }

        .recipe-preview .steps {
            background: #f8f9fa;
            padding: 10px;
            border-radius: 5px;
            white-space: pre-line;
            margin-bottom: 10px;
        }

        .recipe-preview .image-info {
            color: #888;
            font-size: 0.9em;
        }

        .file-upload-wrapper {
            position: relative;
            display: inline-block;
            width: 100%;
        }

        .file-upload-input {
            position: absolute;
            opacity: 0;
            width: 100%;
            height: 100%;
            cursor: pointer;
        }

        .file-upload-label {
            display: block;
            padding: 12px;
            border: 2px dashed #ddd;
            border-radius: 8px;
            text-align: center;
            background: #f8f9fa;
            cursor: pointer;
            transition: all 0.3s ease;
        }

        .file-upload-label:hover {
            border-color: #FF6B6B;
            background: #fff;
        }

        .file-upload-label.has-file {
            border-color: #28a745;
            background: #d4edda;
            color: #155724;
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
            return confirm('確定要刪除這個食譜嗎？此操作無法復原，且相關的食材也會一併刪除。');
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

        // 處理檔案上傳顯示
        function handleFileSelect(input, labelId) {
            var label = document.getElementById(labelId);
            if (input.files && input.files[0]) {
                label.textContent = '已選擇檔案: ' + input.files[0].name;
                label.classList.add('has-file');
            } else {
                label.textContent = '點擊選擇圖片檔案 (JPG, PNG, GIF)';
                label.classList.remove('has-file');
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
                <h1>📖 食譜管理系統</h1>
                <p>管理您的美味食譜集合</p>
            </div>

            <!-- 功能分頁按鈕 -->
            <div class="function-tabs">
                <button type="button" class="tab-btn" onclick="showTab('add')">新增食譜</button>
                <button type="button" class="tab-btn" onclick="showTab('edit')">修改食譜</button>
                <button type="button" class="tab-btn" onclick="showTab('delete')">刪除食譜</button>
            </div>

            <asp:HiddenField ID="CurrentTab" runat="server" Value="add" />

            <!-- 新增食譜分頁 -->
            <div id="addTab" class="tab-content">
                <h2>📝 新增食譜</h2>
                
                <!-- 新增功能的訊息顯示區域 -->
                <asp:Panel ID="AddMessagePanel" runat="server" Visible="false">
                    <div id="addMessageDiv" runat="server" class="message">
                        <asp:Label ID="AddMessageLabel" runat="server"></asp:Label>
                    </div>
                </asp:Panel>
                
                <div class="form-group">
                    <label>食譜標題：</label>
                    <asp:TextBox ID="AddTitle" runat="server" CssClass="form-control" placeholder="請輸入食譜標題"></asp:TextBox>
                </div>
                <div class="form-group">
                    <label>食譜描述：</label>
                    <asp:TextBox ID="AddDescription" runat="server" CssClass="form-control form-control-textarea" 
                        TextMode="MultiLine" placeholder="請輸入食譜描述"></asp:TextBox>
                </div>
                <div class="form-group">
                    <label>製作步驟：</label>
                    <asp:TextBox ID="AddSteps" runat="server" CssClass="form-control form-control-steps" 
                        TextMode="MultiLine" placeholder="請輸入製作步驟，可換行描述"></asp:TextBox>
                </div>
                <div class="form-group">
                    <label>食譜圖片：</label>
                    <div class="file-upload-wrapper">
                        <asp:FileUpload ID="AddImageUpload" runat="server" CssClass="file-upload-input" 
                            onchange="handleFileSelect(this, 'addFileLabel')" accept="image/*" />
                        <label id="addFileLabel" class="file-upload-label">點擊選擇圖片檔案 (JPG, PNG, GIF)</label>
                    </div>
                </div>
                <div class="form-group">
                    <asp:Button ID="SaveAddBtn" runat="server" CssClass="btn-success" Text="💾 新增食譜" OnClick="SaveAddBtn_Click" />
                </div>
            </div>

            <!-- 修改食譜分頁 -->
            <div id="editTab" class="tab-content">
                <h2>✏️ 修改食譜</h2>

                <asp:UpdatePanel ID="EditUpdatePanel" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <!-- 修改功能的訊息顯示區域 -->
                        <asp:Panel ID="EditMessagePanel" runat="server" Visible="false">
                            <div id="editMessageDiv" runat="server" class="message">
                                <asp:Label ID="EditMessageLabel" runat="server"></asp:Label>
                            </div>
                        </asp:Panel>
            
                        <div class="form-group">
                            <label>選擇要修改的食譜：</label>
                            <asp:DropDownList ID="EditRecipeDropDown" runat="server" CssClass="form-control" 
                                AutoPostBack="true" OnSelectedIndexChanged="EditRecipeDropDown_SelectedIndexChanged">
                            </asp:DropDownList>
                        </div>
                        <div class="form-group">
                            <label>食譜標題：</label>
                            <asp:TextBox ID="EditTitle" runat="server" CssClass="form-control"></asp:TextBox>
                        </div>
                        <div class="form-group">
                            <label>食譜描述：</label>
                            <asp:TextBox ID="EditDescription" runat="server" CssClass="form-control form-control-textarea" 
                                TextMode="MultiLine"></asp:TextBox>
                        </div>
                        <div class="form-group">
                            <label>製作步驟：</label>
                            <asp:TextBox ID="EditSteps" runat="server" CssClass="form-control form-control-steps" 
                                TextMode="MultiLine"></asp:TextBox>
                        </div>
                        <div class="form-group">
                            <label>更換食譜圖片：(可選，不選擇則保持原圖片)</label>
                            <div class="file-upload-wrapper">
                                <asp:FileUpload ID="EditImageUpload" runat="server" CssClass="file-upload-input" 
                                    onchange="handleFileSelect(this, 'editFileLabel')" accept="image/*" />
                                <label id="editFileLabel" class="file-upload-label">點擊選擇新圖片檔案 (JPG, PNG, GIF)</label>
                            </div>
                            <!-- 顯示目前圖片區域 -->
                            <div class="current-image-preview" style="margin-top: 15px;">
                                <strong>目前圖片：</strong><br />
                                <asp:Image ID="CurrentImage" runat="server" 
                                    style="max-width: 300px; max-height: 200px; border-radius: 8px; box-shadow: 0 2px 8px rgba(0,0,0,0.1); margin-top: 8px;" 
                                    AlternateText="目前食譜圖片" />
                                <div style="margin-top: 5px; color: #666; font-size: 0.9em;">
                                    檔案名稱：<asp:Label ID="CurrentImageLabel" runat="server"></asp:Label>
                                </div>
                            </div>
                        </div>
                        <div class="form-group">
                            <asp:Button ID="SaveEditBtn" runat="server" CssClass="btn-success" 
                                Text="💾 儲存修改" OnClick="SaveEditBtn_Click" />
                        </div>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="EditRecipeDropDown" EventName="SelectedIndexChanged" />
                        <asp:PostBackTrigger ControlID="SaveEditBtn" />
                    </Triggers>
                </asp:UpdatePanel>
            </div>

            <!-- 刪除食譜分頁 -->
            <div id="deleteTab" class="tab-content">
                <h2>🗑️ 刪除食譜</h2>

                <asp:UpdatePanel ID="DeleteUpdatePanel" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <!-- 刪除功能的訊息顯示區域 -->
                        <asp:Panel ID="DeleteMessagePanel" runat="server" Visible="false">
                            <div id="deleteMessageDiv" runat="server" class="message">
                                <asp:Label ID="DeleteMessageLabel" runat="server"></asp:Label>
                            </div>
                        </asp:Panel>
            
                        <div class="form-group">
                            <label>選擇要刪除的食譜：</label>
                            <asp:DropDownList ID="DeleteRecipeDropDown" runat="server" CssClass="form-control" 
                                AutoPostBack="true" OnSelectedIndexChanged="DeleteRecipeDropDown_SelectedIndexChanged">
                            </asp:DropDownList>
                        </div>
                        <asp:Panel ID="DeletePreviewPanel" runat="server" Visible="false">
                            <div class="recipe-preview">
                                <h4><asp:Label ID="DeletePreviewTitle" runat="server"></asp:Label></h4>
                                <div class="description">
                                    <asp:Label ID="DeletePreviewDescription" runat="server"></asp:Label>
                                </div>
                                <div class="steps">
                                    <strong>製作步驟：</strong><br />
                                    <asp:Label ID="DeletePreviewSteps" runat="server"></asp:Label>
                                </div>
                                <!-- 新增圖片顯示區域 -->
                                <div class="recipe-image-preview" style="margin: 15px 0;">
                                    <strong>食譜圖片：</strong><br />
                                    <asp:Image ID="DeletePreviewImage" runat="server" 
                                        style="max-width: 300px; max-height: 200px; border-radius: 8px; box-shadow: 0 2px 8px rgba(0,0,0,0.1); margin-top: 8px;" 
                                        AlternateText="食譜圖片" />
                                    <div class="image-info" style="margin-top: 5px;">
                                        檔案名稱：<asp:Label ID="DeletePreviewImageName" runat="server"></asp:Label>
                                    </div>
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
                        <asp:AsyncPostBackTrigger ControlID="DeleteRecipeDropDown" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="ConfirmDeleteBtn" EventName="Click" />
                    </Triggers>
                </asp:UpdatePanel>
            </div>
        </div>
    </form>
</body>
</html>