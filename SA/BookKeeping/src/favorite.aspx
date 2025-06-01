<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="favorite.aspx.cs" Inherits="_BookKeeping.src.favorite" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>我的最愛食譜 - 食在當季</title>
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

        .header {
            text-align: center;
            margin-bottom: 30px;
            color: white;
            text-shadow: 2px 2px 4px rgba(0,0,0,0.3);
        }

        .header h1 {
            font-size: 2.5em;
            margin-bottom: 10px;
            display: flex;
            align-items: center;
            justify-content: center;
            gap: 15px;
        }

        .header .heart-icon {
            font-size: 1.2em;
            animation: heartbeat 1.5s ease-in-out infinite;
        }

        @keyframes heartbeat {
            0% { transform: scale(1); }
            50% { transform: scale(1.1); }
            100% { transform: scale(1); }
        }

        .back-btn {
            position: absolute;
            top: 20px;
            left: 20px;
            background: rgba(255, 255, 255, 0.2);
            border: 1px solid rgba(255, 255, 255, 0.3);
            border-radius: 10px;
            padding: 10px 20px;
            color: white;
            text-decoration: none;
            font-size: 1em;
            transition: all 0.3s ease;
        }

        .back-btn:hover {
            background: rgba(255, 255, 255, 0.3);
            transform: translateY(-2px);
        }

        .filter-container {
            background: rgba(255, 255, 255, 0.95);
            border-radius: 15px;
            padding: 25px;
            margin-bottom: 30px;
            box-shadow: 0 10px 30px rgba(0,0,0,0.1);
        }

        .filter-box {
            display: flex;
            gap: 15px;
            align-items: center;
            flex-wrap: wrap;
            justify-content: space-between;
        }

        .filter-left {
            display: flex;
            gap: 15px;
            align-items: center;
            flex-wrap: wrap;
        }

        .filter-input {
            flex: 1;
            min-width: 250px;
            padding: 12px 15px;
            border: 2px solid #e0e0e0;
            border-radius: 25px;
            font-size: 1em;
            outline: none;
            transition: border-color 0.3s ease;
        }

        .filter-input:focus {
            border-color: #667eea;
        }

        .filter-select {
            padding: 12px 15px;
            border: 2px solid #e0e0e0;
            border-radius: 25px;
            font-size: 1em;
            outline: none;
            background: white;
            cursor: pointer;
            transition: border-color 0.3s ease;
        }

        .filter-select:focus {
            border-color: #667eea;
        }

        .filter-btn {
            background: linear-gradient(145deg, #667eea, #764ba2);
            border: none;
            border-radius: 25px;
            padding: 12px 25px;
            color: white;
            font-size: 1em;
            cursor: pointer;
            transition: all 0.3s ease;
        }

        .filter-btn:hover {
            transform: translateY(-2px);
            box-shadow: 0 5px 15px rgba(102, 126, 234, 0.4);
        }

        .stats-container {
            background: rgba(255, 255, 255, 0.95);
            border-radius: 15px;
            padding: 20px;
            margin-bottom: 30px;
            box-shadow: 0 10px 30px rgba(0,0,0,0.1);
            text-align: center;
        }

        .stats-item {
            display: inline-block;
            margin: 0 20px;
            color: #666;
            font-size: 1.1em;
        }

        .stats-number {
            font-size: 1.5em;
            font-weight: bold;
            color: #667eea;
            display: block;
        }

        .recipe-container {
            background: rgba(255, 255, 255, 0.95);
            border-radius: 15px;
            padding: 30px;
            box-shadow: 0 10px 30px rgba(0,0,0,0.1);
        }

        .recipe-list {
            display: grid;
            grid-template-columns: repeat(auto-fill, minmax(350px, 1fr));
            gap: 25px;
            margin-top: 20px;
        }

        .recipe-card {
            background: white;
            border-radius: 15px;
            padding: 20px;
            box-shadow: 0 5px 20px rgba(0,0,0,0.1);
            transition: all 0.3s ease;
            position: relative;
            border: 3px solid transparent;
        }

        .recipe-card:hover {
            transform: translateY(-5px);
            box-shadow: 0 10px 30px rgba(0,0,0,0.15);
            border-color: #667eea;
        }

        .seasonal-badge {
            position: absolute;
            top: 15px;
            right: 15px;
            background: linear-gradient(145deg, #4ecdc4, #44a08d);
            color: white;
            padding: 8px 15px;
            border-radius: 20px;
            font-size: 0.9em;
            font-weight: bold;
            z-index: 10;
            box-shadow: 0 2px 8px rgba(0,0,0,0.2);
        }

        .recipe-image {
            width: 100%;
            height: 200px;
            border-radius: 10px;
            margin-bottom: 15px;
            object-fit: cover;
            background: #f0f0f0;
        }

        .recipe-image.placeholder, .image-placeholder {
            display: flex;
            align-items: center;
            justify-content: center;
            color: #999;
            font-size: 0.9em;
            background: linear-gradient(135deg, #f0f0f0, #e0e0e0);
        }

        .recipe-title {
            font-size: 1.3em;
            font-weight: bold;
            color: #333;
            margin-bottom: 10px;
            cursor: pointer;
            transition: color 0.3s ease;
        }

        .recipe-title:hover {
            color: #667eea;
        }

        .recipe-description {
            color: #666;
            margin-bottom: 15px;
            line-height: 1.4;
        }

        .recipe-ingredients {
            font-size: 0.9em;
            color: #777;
            margin-bottom: 15px;
        }

        .recipe-steps {
            background: #f8f9fa;
            padding: 15px;
            border-radius: 8px;
            margin-bottom: 15px;
            display: none;
            line-height: 1.5;
            color: #555;
        }

        .recipe-steps.show {
            display: block;
        }

        .recipe-actions {
            display: flex;
            gap: 10px;
            align-items: center;
            flex-wrap: wrap;
        }

        .remove-favorite-btn {
            background: linear-gradient(145deg, #ff6b6b, #ee5a24);
            border: none;
            border-radius: 20px;
            padding: 8px 15px;
            color: white;
            font-size: 0.9em;
            cursor: pointer;
            transition: all 0.3s ease;
        }

        .remove-favorite-btn:hover {
            transform: translateY(-2px);
            background: linear-gradient(145deg, #ee5a24, #d63031);
        }

        .comment-btn, .detail-btn, .share-btn {
            background: linear-gradient(145deg, #4ecdc4, #44a08d);
            border: none;
            border-radius: 20px;
            padding: 8px 15px;
            color: white;
            font-size: 0.9em;
            cursor: pointer;
            transition: all 0.3s ease;
        }

        .detail-btn {
            background: linear-gradient(145deg, #667eea, #764ba2);
        }

        .share-btn {
            background: linear-gradient(145deg, #ffa726, #ff8f00);
        }

        .comment-btn:hover, .detail-btn:hover, .share-btn:hover {
            transform: translateY(-2px);
        }

        .comment-section {
            margin-top: 15px;
            display: none;
            border-top: 1px solid #eee;
            padding-top: 15px;
        }

        .comment-section.show {
            display: block;
        }

        .comment-input {
            width: 100%;
            padding: 8px 12px;
            border: 1px solid #ddd;
            border-radius: 20px;
            margin-bottom: 10px;
            outline: none;
        }

        .comment-submit {
            background: #667eea;
            border: none;
            border-radius: 15px;
            padding: 6px 12px;
            color: white;
            font-size: 0.8em;
            cursor: pointer;
        }

        .comments-list {
            margin-top: 10px;
            max-height: 150px;
            overflow-y: auto;
        }

        .comment-item {
            background: #f8f9fa;
            padding: 8px 12px;
            border-radius: 8px;
            margin-bottom: 8px;
            font-size: 0.85em;
        }

        .comment-author {
            font-weight: bold;
            color: #667eea;
            margin-right: 8px;
        }

        .comment-date {
            color: #999;
            font-size: 0.75em;
            margin-left: 8px;
        }

        .empty-favorites {
            text-align: center;
            color: #666;
            font-size: 1.2em;
            margin-top: 50px;
            padding: 40px;
        }

        .empty-favorites .icon {
            font-size: 4em;
            margin-bottom: 20px;
            opacity: 0.5;
        }

        .browse-recipes-btn {
            background: linear-gradient(145deg, #667eea, #764ba2);
            border: none;
            border-radius: 25px;
            padding: 12px 25px;
            color: white;
            font-size: 1em;
            cursor: pointer;
            transition: all 0.3s ease;
            margin-top: 20px;
            text-decoration: none;
            display: inline-block;
        }

        .browse-recipes-btn:hover {
            transform: translateY(-2px);
            box-shadow: 0 5px 15px rgba(102, 126, 234, 0.4);
        }

        .bulk-actions {
            background: #f8f9fa;
            padding: 15px;
            border-radius: 10px;
            margin-bottom: 20px;
            display: none;
        }

        .bulk-actions.show {
            display: block;
        }

        .bulk-btn {
            background: #6c757d;
            border: none;
            border-radius: 15px;
            padding: 8px 15px;
            color: white;
            font-size: 0.9em;
            cursor: pointer;
            margin-right: 10px;
            transition: all 0.3s ease;
        }

        .bulk-btn:hover {
            transform: translateY(-2px);
        }

        .bulk-btn.delete {
            background: #dc3545;
        }

        .bulk-btn.export {
            background: #28a745;
        }

        .recipe-checkbox {
            position: absolute;
            top: 15px;
            left: 15px;
            z-index: 10;
            transform: scale(1.2);
        }

        @media (max-width: 768px) {
            .filter-box {
                flex-direction: column;
            }
            
            .filter-left {
                width: 100%;
            }
            
            .filter-input {
                min-width: 100%;
            }
            
            .recipe-list {
                grid-template-columns: 1fr;
            }
            
            .back-btn {
                position: relative;
                margin-bottom: 20px;
                display: inline-block;
            }

            .recipe-actions {
                justify-content: center;
            }

            .stats-item {
                display: block;
                margin: 10px 0;
            }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
        
        <a href="../src/main.aspx" class="back-btn">← 返回主頁</a>
        
        <div class="header">
            <h1>
                <span class="heart-icon">❤️</span>
                我的最愛食譜
                <span class="heart-icon">❤️</span>
            </h1>
            <p>您收藏的美味料理都在這裡</p>
        </div>

        <div class="stats-container">
            <div class="stats-item">
                <span class="stats-number">
                    <asp:Label ID="TotalFavoritesLabel" runat="server" Text="0"></asp:Label>
                </span>
                收藏總數
            </div>
            <div class="stats-item">
                <span class="stats-number">
                    <asp:Label ID="SeasonalFavoritesLabel" runat="server" Text="0"></asp:Label>
                </span>
                當季推薦
            </div>
            <div class="stats-item">
                <span class="stats-number">
                    <asp:Label ID="RecentFavoritesLabel" runat="server" Text="0"></asp:Label>
                </span>
                本月新增
            </div>
        </div>

        <div class="filter-container">
            <div class="filter-box">
                <div class="filter-left">
                    <asp:TextBox ID="FilterTextBox" runat="server" CssClass="filter-input" 
                        placeholder="搜尋我的最愛食譜"></asp:TextBox>
                    <asp:DropDownList ID="SortDropDown" runat="server" CssClass="filter-select">
                        <asp:ListItem Value="recent" Text="最近收藏"></asp:ListItem>
                        <asp:ListItem Value="name_asc" Text="名稱 A-Z"></asp:ListItem>
                        <asp:ListItem Value="name_desc" Text="名稱 Z-A"></asp:ListItem>
                        <asp:ListItem Value="seasonal" Text="當季推薦"></asp:ListItem>
                    </asp:DropDownList>
                    <asp:Button ID="FilterButton" runat="server" CssClass="filter-btn" 
                        Text="篩選" OnClick="FilterButton_Click" />
                </div>
                <div>
                    <button type="button" class="filter-btn" onclick="toggleBulkActions()">
                        🗂️ 批量管理
                    </button>
                </div>
            </div>
        </div>

        <div class="recipe-container">
            <div id="bulk-actions" class="bulk-actions">
                <button type="button" class="bulk-btn export" onclick="exportSelected()">
                    📤 匯出選取
                </button>
                <asp:Button ID="BulkDeleteButton" runat="server" CssClass="bulk-btn delete" 
                    Text="🗑️ 刪除選取" OnClick="BulkDeleteButton_Click" 
                    OnClientClick="return confirmBulkDelete();" />
                <button type="button" class="bulk-btn" onclick="selectAll()">
                    ☑️ 全選
                </button>
                <button type="button" class="bulk-btn" onclick="clearSelection()">
                    ⬜ 清除選取
                </button>
            </div>

            <asp:UpdatePanel ID="FavoriteUpdatePanel" runat="server">
                <ContentTemplate>
                    <asp:Repeater ID="FavoriteRepeater" runat="server" OnItemCommand="FavoriteRepeater_ItemCommand">
                        <HeaderTemplate>
                            <div class="recipe-list">
                        </HeaderTemplate>
                        <ItemTemplate>
                            <div class="recipe-card">
                                <input type="checkbox" class="recipe-checkbox" 
                                    name="selectedRecipes" value='<%# Eval("recipe_id") %>' 
                                    style="display: none;" />
                                
                                <asp:Panel ID="SeasonalBadgePanel" runat="server" 
                                    CssClass="seasonal-badge" 
                                    Visible='<%# (bool)Eval("IsSeasonal") %>'>
                                    🌟 當季推薦
                                </asp:Panel>
                                
                                <asp:Image ID="RecipeImage" runat="server" 
                                    ImageUrl='<%# Eval("image") %>' 
                                    CssClass="recipe-image"
                                    AlternateText='<%# Eval("title") %>'
                                    onerror="handleImageError(this);" />
                                
                                <div class="recipe-title" onclick="toggleDetails(<%# Eval("recipe_id") %>)">
                                    <%# Eval("title") %>
                                </div>
                                <div class="recipe-description"><%# Eval("description") %></div>
                                <div class="recipe-ingredients">
                                    <strong>食材：</strong><%# Eval("ingredients") %>
                                </div>
                                
                                <div id="recipe-steps-<%# Eval("recipe_id") %>" class="recipe-steps">
                                    <strong>製作步驟：</strong><br />
                                    <%# Eval("steps") %>
                                </div>
                                
                                <div class="recipe-actions">
                                    <asp:Button ID="RemoveFavoriteButton" runat="server" 
                                        CssClass="remove-favorite-btn"
                                        Text="💔 移除最愛"
                                        CommandName="RemoveFavorite" 
                                        CommandArgument='<%# Eval("recipe_id") %>'
                                        OnClientClick="return confirm('確定要從最愛中移除這個食譜嗎？');" />
                                    
                                    <button type="button" class="detail-btn" 
                                        onclick="toggleDetails(<%# Eval("recipe_id") %>)">
                                        📋 查看步驟
                                    </button>
                                    
                                    <button type="button" class="comment-btn" 
                                        onclick="toggleComments(<%# Eval("recipe_id") %>)">
                                        💬 留言
                                    </button>
                                    
                                    <button type="button" class="share-btn" 
                                        onclick="shareRecipe(<%# Eval("recipe_id") %>, '<%# Eval("title") %>')">
                                        📤 分享
                                    </button>
                                </div>
                                
                                <div id="comment-section-<%# Eval("recipe_id") %>" class="comment-section">
                                    <asp:TextBox ID="CommentTextBox" runat="server" CssClass="comment-input" 
                                        placeholder="寫下您的評論..." MaxLength="80"></asp:TextBox>
                                    <asp:Button ID="CommentSubmitButton" runat="server" CssClass="comment-submit"
                                        Text="發表" CommandName="AddComment" 
                                        CommandArgument='<%# Eval("recipe_id") %>' />
                                    
                                    <div class="comments-list">
                                        <asp:Repeater ID="CommentRepeater" runat="server" 
                                            DataSource='<%# Eval("Comments") %>'>
                                            <ItemTemplate>
                                                <div class="comment-item">
                                                    <span class="comment-author"><%# Eval("user_name") %></span>
                                                    <%# Eval("content") %>
                                                    <span class="comment-date"><%# Convert.ToDateTime(Eval("created_date")).ToString("MM/dd HH:mm") %></span>
                                                </div>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </div>
                                </div>
                            </div>
                        </ItemTemplate>
                        <FooterTemplate>
                            </div>
                        </FooterTemplate>
                    </asp:Repeater>
                    
                    <div id="Div1" class="empty-favorites" runat="server" visible="false">
                        <div class="icon">💔</div>
                        <h3>還沒有收藏任何食譜</h3>
                        <p>快去瀏覽食譜，找到您喜歡的料理加入最愛吧！</p>
                        <a href="browse_recipe.aspx" class="browse-recipes-btn">🔍 瀏覽食譜</a>
                    </div>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="FilterButton" EventName="Click" />
                    <asp:AsyncPostBackTrigger ControlID="BulkDeleteButton" EventName="Click" />
                </Triggers>
            </asp:UpdatePanel>
        </div>
    </form>

    <script type="text/javascript">
        function toggleComments(recipeId) {
            var section = document.getElementById('comment-section-' + recipeId);
            if (section.classList.contains('show')) {
                section.classList.remove('show');
            } else {
                section.classList.add('show');
            }
        }

        function toggleDetails(recipeId) {
            var stepsSection = document.getElementById('recipe-steps-' + recipeId);
            if (stepsSection.classList.contains('show')) {
                stepsSection.classList.remove('show');
            } else {
                stepsSection.classList.add('show');
            }
        }

        function handleImageError(img) {
            if (img.src.indexOf('default_recipe.jpg') === -1) {
                var basePath = window.location.pathname.substring(0, window.location.pathname.lastIndexOf('/'));
                if (basePath.endsWith('/src')) {
                    basePath = basePath.substring(0, basePath.lastIndexOf('/src'));
                }
                img.src = basePath + '/src/recipes/default_recipe.jpg';
            } else {
                img.style.display = 'none';
                var placeholder = img.nextElementSibling;
                if (!placeholder || !placeholder.classList.contains('image-placeholder')) {
                    placeholder = document.createElement('div');
                    placeholder.className = 'recipe-image placeholder image-placeholder';
                    placeholder.innerHTML = '📷 暫無圖片';
                    img.parentNode.insertBefore(placeholder, img.nextSibling);
                }
                placeholder.style.display = 'flex';
            }
        }

        function toggleBulkActions() {
            var bulkActions = document.getElementById('bulk-actions');
            var checkboxes = document.querySelectorAll('.recipe-checkbox');

            if (bulkActions.classList.contains('show')) {
                bulkActions.classList.remove('show');
                checkboxes.forEach(function (checkbox) {
                    checkbox.style.display = 'none';
                    checkbox.checked = false;
                });
            } else {
                bulkActions.classList.add('show');
                checkboxes.forEach(function (checkbox) {
                    checkbox.style.display = 'block';
                });
            }
        }

        function selectAll() {
            var checkboxes = document.querySelectorAll('.recipe-checkbox');
            checkboxes.forEach(function (checkbox) {
                checkbox.checked = true;
            });
        }

        function clearSelection() {
            var checkboxes = document.querySelectorAll('.recipe-checkbox');
            checkboxes.forEach(function (checkbox) {
                checkbox.checked = false;
            });
        }

        function confirmBulkDelete() {
            var selectedCount = document.querySelectorAll('.recipe-checkbox:checked').length;
            if (selectedCount === 0) {
                alert('請先選擇要刪除的食譜');
                return false;
            }
            return confirm('確定要刪除選取的 ' + selectedCount + ' 個食譜嗎？');
        }

        function exportSelected() {
            var selectedCheckboxes = document.querySelectorAll('.recipe-checkbox:checked');
            if (selectedCheckboxes.length === 0) {
                alert('請先選擇要匯出的食譜');
                return;
            }

            var recipeIds = [];
            selectedCheckboxes.forEach(function (checkbox) {
                recipeIds.push(checkbox.value);
            });

            // 這裡可以實作匯出功能
            alert('匯出功能開發中，已選取 ' + recipeIds.length + ' 個食譜');
        }

        function shareRecipe(recipeId, title) {
            var url = window.location.origin + '/src/recipe_detail.aspx?id=' + recipeId;
            var text = '我想分享這個美味食譜：' + title;

            if (navigator.share) {
                navigator.share({
                    title: title,
                    text: text,
                    url: url
                });
            } else {
                // 複製到剪貼簿
                var textArea = document.createElement('textarea');
                textArea.value = text + '\n' + url;
                document.body.appendChild(textArea);
                textArea.select();
                document.execCommand('copy');
                document.body.removeChild(textArea);
                alert('食譜連結已複製到剪貼簿！');
            }
        }
    </script>
</body>
</html>