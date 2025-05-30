<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="add_recipe.aspx.cs" Inherits="_BookKeeping.src.add_recipe" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>新增食譜 - 食在當季</title>
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
            padding: 20px;
        }

        .container {
            max-width: 800px;
            margin: 0 auto;
            background: rgba(255, 255, 255, 0.95);
            border-radius: 25px;
            padding: 40px;
            box-shadow: 0 20px 40px rgba(0,0,0,0.1);
        }

        .header {
            text-align: center;
            margin-bottom: 30px;
            color: #333;
        }

        .header h1 {
            font-size: 2.5em;
            margin-bottom: 10px;
            color: #667eea;
        }

        .form-group {
            margin-bottom: 25px;
        }

        .form-group label {
            display: block;
            margin-bottom: 8px;
            font-weight: bold;
            color: #333;
            font-size: 1.1em;
        }

        .form-control {
            width: 100%;
            padding: 12px 15px;
            border: 2px solid #e0e0e0;
            border-radius: 10px;
            font-size: 1em;
            transition: border-color 0.3s ease;
            font-family: 'Microsoft JhengHei', Arial, sans-serif;
        }

        .form-control:focus {
            outline: none;
            border-color: #667eea;
            box-shadow: 0 0 0 3px rgba(102, 126, 234, 0.1);
        }

        .textarea-large {
            min-height: 120px;
            resize: vertical;
        }

        .ingredients-section {
            background: #f8f9fa;
            border-radius: 15px;
            padding: 25px;
            margin-bottom: 25px;
        }

        .ingredients-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 20px;
        }

        .ingredients-header h3 {
            color: #333;
            margin: 0;
        }

        .add-ingredient-btn {
            background: linear-gradient(145deg, #4ecdc4, #44a08d);
            border: none;
            border-radius: 8px;
            padding: 8px 15px;
            color: white;
            font-size: 0.9em;
            cursor: pointer;
            transition: all 0.3s ease;
        }

        .add-ingredient-btn:hover {
            transform: translateY(-2px);
            box-shadow: 0 5px 15px rgba(78, 205, 196, 0.3);
        }

        .ingredient-row {
            display: flex;
            gap: 15px;
            margin-bottom: 15px;
            align-items: flex-end;
        }

        .ingredient-name {
            flex: 2;
        }

        .ingredient-quantity {
            flex: 1;
        }

        .remove-ingredient-btn {
            background: #ff6b6b;
            border: none;
            border-radius: 8px;
            padding: 12px 15px;
            color: white;
            cursor: pointer;
            transition: all 0.3s ease;
        }

        .remove-ingredient-btn:hover {
            background: #ee5a24;
            transform: translateY(-2px);
        }

        .file-upload-section {
            background: #f0f8ff;
            border: 2px dashed #667eea;
            border-radius: 15px;
            padding: 30px;
            text-align: center;
            margin-bottom: 25px;
            transition: all 0.3s ease;
        }

        .file-upload-section:hover {
            background: #e6f3ff;
        }

        .file-upload-icon {
            font-size: 3em;
            color: #667eea;
            margin-bottom: 15px;
        }

        .button-group {
            display: flex;
            gap: 20px;
            justify-content: center;
            margin-top: 30px;
            flex-wrap: wrap;
        }

        .btn-primary {
            background: linear-gradient(145deg, #667eea, #764ba2);
            border: none;
            border-radius: 15px;
            padding: 15px 40px;
            font-size: 1.2em;
            font-weight: bold;
            color: white;
            cursor: pointer;
            transition: all 0.3s ease;
            box-shadow: 0 8px 25px rgba(102, 126, 234, 0.3);
        }

        .btn-primary:hover {
            transform: translateY(-3px);
            box-shadow: 0 12px 35px rgba(102, 126, 234, 0.4);
        }

        .btn-secondary {
            background: #6c757d;
            border: none;
            border-radius: 15px;
            padding: 15px 40px;
            font-size: 1.2em;
            font-weight: bold;
            color: white;
            cursor: pointer;
            transition: all 0.3s ease;
        }

        .btn-secondary:hover {
            background: #5a6268;
            transform: translateY(-3px);
        }

        .btn-info {
            background: linear-gradient(145deg, #17a2b8, #138496);
            border: none;
            border-radius: 15px;
            padding: 15px 40px;
            font-size: 1.2em;
            font-weight: bold;
            color: white;
            cursor: pointer;
            transition: all 0.3s ease;
        }

        .btn-info:hover {
            transform: translateY(-3px);
            box-shadow: 0 12px 35px rgba(23, 162, 184, 0.4);
        }

        .alert {
            padding: 15px;
            border-radius: 10px;
            margin-bottom: 20px;
            font-weight: bold;
        }

        .alert-success {
            background: #d4edda;
            color: #155724;
            border: 1px solid #c3e6cb;
        }

        .alert-danger {
            background: #f8d7da;
            color: #721c24;
            border: 1px solid #f5c6cb;
        }

        @media (max-width: 768px) {
            .container {
                margin: 10px;
                padding: 20px;
            }

            .ingredient-row {
                flex-direction: column;
                gap: 10px;
            }

            .button-group {
                flex-direction: column;
                gap: 15px;
            }

            .header h1 {
                font-size: 2em;
            }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <div class="header">
                <h1>🍳 新增食譜</h1>
                <p>分享您的美味創作</p>
            </div>

            <!-- 訊息顯示區域 -->
            <asp:Panel ID="MessagePanel" runat="server" Visible="false">
                <asp:Label ID="MessageLabel" runat="server" CssClass="alert"></asp:Label>
            </asp:Panel>

            <!-- 食譜標題 -->
            <div class="form-group">
                <label for="RecipeTitleTextBox">食譜名稱 *</label>
                <asp:TextBox ID="RecipeTitleTextBox" runat="server" CssClass="form-control" 
                    placeholder="請輸入食譜名稱，例如：蒜泥白肉" MaxLength="45" required="true"></asp:TextBox>
            </div>

            <!-- 食譜描述 -->
            <div class="form-group">
                <label for="RecipeDescriptionTextBox">食譜描述 *</label>
                <asp:TextBox ID="RecipeDescriptionTextBox" runat="server" CssClass="form-control" 
                    placeholder="請簡單描述這道菜的特色，例如：清爽下飯的夏日涼菜" MaxLength="100" required="true"></asp:TextBox>
            </div>

            <!-- 製作步驟 -->
            <div class="form-group">
                <label for="RecipeStepsTextBox">製作步驟 *</label>
                <asp:TextBox ID="RecipeStepsTextBox" runat="server" CssClass="form-control textarea-large" 
                    TextMode="MultiLine" placeholder="請輸入詳細的製作步驟，例如：1.豬肉煮熟切片 2.蒜泥調味 3.淋上調料" 
                    MaxLength="500" required="true"></asp:TextBox>
            </div>

            <!-- 食材清單 -->
            <div class="ingredients-section">
                <div class="ingredients-header">
                    <h3>📝 食材清單</h3>
                    <button type="button" class="add-ingredient-btn" onclick="addIngredientRow(); return false;">
                        + 新增食材
                    </button>
                </div>
                
                <div id="ingredientsContainer">
                    <div class="ingredient-row">
                        <div class="ingredient-name">
                            <label>食材名稱</label>
                            <asp:TextBox ID="IngredientName1" runat="server" CssClass="form-control" 
                                placeholder="例如：豬五花肉" MaxLength="45"></asp:TextBox>
                        </div>
                        <div class="ingredient-quantity">
                            <label>用量</label>
                            <asp:TextBox ID="IngredientQuantity1" runat="server" CssClass="form-control" 
                                placeholder="例如：300g" MaxLength="10"></asp:TextBox>
                        </div>
                    </div>
                </div>
                
                <asp:HiddenField ID="IngredientCountHidden" runat="server" Value="1" />
            </div>

            <!-- 圖片上傳 -->
            <div class="form-group">
                <label>食譜圖片</label>
                <div class="file-upload-section">
                    <div class="file-upload-icon">📷</div>
                    <p>選擇或拖放食譜圖片</p>
                    <asp:FileUpload ID="RecipeImageUpload" runat="server" CssClass="form-control" 
                        accept="image/*" />
                    <small style="color: #666; margin-top: 10px; display: block;">
                        支援 JPG、PNG、GIF 格式，檔案大小限制 5MB
                    </small>
                </div>
            </div>

            <!-- 按鈕群組 -->
            <div class="button-group">
                <asp:Button ID="SaveRecipeBtn" runat="server" CssClass="btn-primary" 
                    Text="💾 儲存食譜" OnClick="SaveRecipeBtn_Click" />
                <asp:Button ID="BackToMainBtn" runat="server" CssClass="btn-info" 
                    Text="🏠 返回主頁面" OnClick="BackToMainBtn_Click" CausesValidation="false" />
                <asp:Button ID="CancelBtn" runat="server" CssClass="btn-secondary" 
                    Text="❌ 取消" OnClick="CancelBtn_Click" CausesValidation="false" />
            </div>
        </div>
    </form>

    <script type="text/javascript">
        let ingredientCount = 1;

        function addIngredientRow() {
            ingredientCount++;

            const container = document.getElementById('ingredientsContainer');
            const newRow = document.createElement('div');
            newRow.className = 'ingredient-row';
            newRow.innerHTML = `
                <div class="ingredient-name">
                    <label>食材名稱</label>
                    <input type="text" name="IngredientName${ingredientCount}" 
                           class="form-control" placeholder="例如：豬五花肉" maxlength="45" />
                </div>
                <div class="ingredient-quantity">
                    <label>用量</label>
                    <input type="text" name="IngredientQuantity${ingredientCount}" 
                           class="form-control" placeholder="例如：300g" maxlength="10" />
                </div>
                <button type="button" class="remove-ingredient-btn" onclick="removeIngredientRow(this)">
                    🗑️
                </button>
            `;

            container.appendChild(newRow);

            // 更新隱藏欄位的值
            document.getElementById('<%= IngredientCountHidden.ClientID %>').value = ingredientCount;
        }

        function removeIngredientRow(button) {
            const row = button.closest('.ingredient-row');
            row.remove();
        }

        // 表單驗證
        function validateForm() {
            const title = document.getElementById('<%= RecipeTitleTextBox.ClientID %>').value.trim();
            const description = document.getElementById('<%= RecipeDescriptionTextBox.ClientID %>').value.trim();
            const steps = document.getElementById('<%= RecipeStepsTextBox.ClientID %>').value.trim();
            
            if (!title || !description || !steps) {
                alert('請填寫所有必填欄位！');
                return false;
            }
            
            // 檢查至少要有一個食材
            const firstIngredientName = document.getElementById('<%= IngredientName1.ClientID %>').value.trim();
            if (!firstIngredientName) {
                alert('請至少新增一個食材！');
                return false;
            }
            
            return true;
        }

        // 在表單提交前進行驗證
        document.getElementById('<%= SaveRecipeBtn.ClientID %>').onclick = function (e) {
            if (!validateForm()) {
                e.preventDefault();
                return false;
            }
            return true;
        };

        // 確保表單提交時包含所有動態新增的食材
        document.getElementById('form1').onsubmit = function () {
            // 記錄當前的食材數量
            console.log('提交表單時的食材數量:', ingredientCount);
            return true;
        };
    </script>
</body>
</html>