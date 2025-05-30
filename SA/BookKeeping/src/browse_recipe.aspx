<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="browse_recipe.aspx.cs" Inherits="_BookKeeping.src.browse_recipe" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>瀏覽食譜 - 食在當季</title>
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

        .search-container {
            background: rgba(255, 255, 255, 0.95);
            border-radius: 15px;
            padding: 25px;
            margin-bottom: 30px;
            box-shadow: 0 10px 30px rgba(0,0,0,0.1);
        }

        .search-box {
            display: flex;
            gap: 15px;
            align-items: center;
            flex-wrap: wrap;
        }

        .search-input {
            flex: 1;
            min-width: 300px;
            padding: 12px 15px;
            border: 2px solid #e0e0e0;
            border-radius: 25px;
            font-size: 1em;
            outline: none;
            transition: border-color 0.3s ease;
        }

        .search-input:focus {
            border-color: #667eea;
        }

        .search-btn {
            background: linear-gradient(145deg, #667eea, #764ba2);
            border: none;
            border-radius: 25px;
            padding: 12px 25px;
            color: white;
            font-size: 1em;
            cursor: pointer;
            transition: all 0.3s ease;
        }

        .search-btn:hover {
            transform: translateY(-2px);
            box-shadow: 0 5px 15px rgba(102, 126, 234, 0.4);
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
        }

        .recipe-card:hover {
            transform: translateY(-5px);
            box-shadow: 0 10px 30px rgba(0,0,0,0.15);
        }

        .seasonal-badge {
            position: absolute;
            top: 15px;
            right: 15px;
            background: linear-gradient(145deg, #4ecdc4, #44a08d);
            color: white;
            padding: 5px 12px;
            border-radius: 20px;
            font-size: 0.8em;
            font-weight: bold;
        }

        .recipe-image {
            width: 100%;
            height: 200px;
            background: #f0f0f0;
            border-radius: 10px;
            margin-bottom: 15px;
            display: flex;
            align-items: center;
            justify-content: center;
            color: #999;
            font-size: 0.9em;
        }

        .recipe-title {
            font-size: 1.3em;
            font-weight: bold;
            color: #333;
            margin-bottom: 10px;
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

        .recipe-actions {
            display: flex;
            gap: 10px;
            align-items: center;
        }

        .favorite-btn {
            background: linear-gradient(145deg, #ff6b6b, #ee5a24);
            border: none;
            border-radius: 20px;
            padding: 8px 15px;
            color: white;
            font-size: 0.9em;
            cursor: pointer;
            transition: all 0.3s ease;
        }

        .favorite-btn:hover {
            transform: translateY(-2px);
        }

        .favorite-btn.favorited {
            background: linear-gradient(145deg, #ffa726, #ff8f00);
        }

        .comment-btn {
            background: linear-gradient(145deg, #4ecdc4, #44a08d);
            border: none;
            border-radius: 20px;
            padding: 8px 15px;
            color: white;
            font-size: 0.9em;
            cursor: pointer;
            transition: all 0.3s ease;
        }

        .comment-btn:hover {
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

        .no-recipes {
            text-align: center;
            color: #666;
            font-size: 1.1em;
            margin-top: 50px;
        }

        @media (max-width: 768px) {
            .search-box {
                flex-direction: column;
            }
            
            .search-input {
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
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
        
        <a href="../src/main.aspx" class="back-btn">← 返回主頁</a>
        
        <div class="header">
            <h1>🔍 瀏覽食譜</h1>
            <p>搜尋您想要的美味料理</p>
        </div>

        <div class="search-container">
            <div class="search-box">
                <asp:TextBox ID="SearchTextBox" runat="server" CssClass="search-input" 
                    placeholder="搜尋食譜名稱、食材、月份(1-12)或季節(春季/夏季/秋季/冬季)..."></asp:TextBox>
                <asp:Button ID="SearchButton" runat="server" CssClass="search-btn" 
                    Text="搜尋" OnClick="SearchButton_Click" />
            </div>
        </div>

        <div class="recipe-container">
            <asp:UpdatePanel ID="RecipeUpdatePanel" runat="server">
                <ContentTemplate>
                    <asp:Repeater ID="RecipeRepeater" runat="server" OnItemCommand="RecipeRepeater_ItemCommand">
                        <HeaderTemplate>
                            <div class="recipe-list">
                        </HeaderTemplate>
                        <ItemTemplate>
                            <div class="recipe-card">
                                <%# Convert.ToBoolean(Eval("IsSeasonal")) ? "<div class=\"seasonal-badge\">當季推薦</div>" : "" %>
                                
                                <div class="recipe-image">
                                    <%# Eval("image") %>
                                </div>
                                
                                <div class="recipe-title"><%# Eval("title") %></div>
                                <div class="recipe-description"><%# Eval("description") %></div>
                                <div class="recipe-ingredients">
                                    <strong>食材：</strong><%# Eval("ingredients") %>
                                </div>
                                
                                <div class="recipe-actions">
                                    <asp:Button ID="FavoriteButton" runat="server" 
                                        CssClass='<%# Convert.ToBoolean(Eval("IsFavorited")) ? "favorite-btn favorited" : "favorite-btn" %>'
                                        Text='<%# Convert.ToBoolean(Eval("IsFavorited")) ? "★ 已收藏" : "☆ 加入最愛" %>'
                                        CommandName="ToggleFavorite" 
                                        CommandArgument='<%# Eval("recipe_id") %>' />
                                    
                                    <button type="button" class="comment-btn" 
                                        onclick="toggleComments(<%# Eval("recipe_id") %>)">
                                        💬 留言
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
                    
                    <asp:Label ID="NoRecipesLabel" runat="server" CssClass="no-recipes" 
                        Text="沒有找到符合條件的食譜" Visible="false"></asp:Label>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="SearchButton" EventName="Click" />
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
    </script>
</body>
</html>