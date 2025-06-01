using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Text;

namespace _BookKeeping.src
{
    public partial class favorite : System.Web.UI.Page
    {
        // 資料庫連接字串
        private string connectionString = ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadFavoriteRecipes();
                UpdateStatistics();
            }
        }

        private void LoadFavoriteRecipes()
        {
            try
            {
                // 檢查使用者是否已登入
                if (Session["UserId"] == null)
                {
                    Response.Redirect("~/login.aspx");
                    return;
                }

                string userId = Session["UserId"].ToString();
                string searchText = FilterTextBox.Text.Trim();
                string sortBy = SortDropDown.SelectedValue;

                DataTable favoriteRecipes = GetFavoriteRecipes(userId, searchText, sortBy);

                if (favoriteRecipes.Rows.Count > 0)
                {
                    // 為每筆資料增加額外的欄位
                    favoriteRecipes.Columns.Add("IsSeasonal", typeof(bool));
                    favoriteRecipes.Columns.Add("Comments", typeof(DataTable));
                    favoriteRecipes.Columns.Add("ingredients", typeof(string));

                    foreach (DataRow row in favoriteRecipes.Rows)
                    {
                        // 判斷是否為當季食譜 (這裡需要有 category 欄位，但資料庫中沒有)
                        // 暫時設為 false
                        row["IsSeasonal"] = false;

                        // 載入該食譜的評論
                        int recipeId;
                        if (int.TryParse(row["recipe_id"].ToString(), out recipeId))
                        {
                            row["Comments"] = GetRecipeComments(recipeId);
                            // 載入食材資訊
                            row["ingredients"] = GetRecipeIngredients(recipeId);
                        }
                    }

                    FavoriteRepeater.DataSource = favoriteRecipes;
                    FavoriteRepeater.DataBind();

                    // 隱藏空資料提示
                    if (FindControl("Div1") != null)
                        FindControl("Div1").Visible = false;
                }
                else
                {
                    // 顯示空資料提示
                    FavoriteRepeater.DataSource = null;
                    FavoriteRepeater.DataBind();

                    if (FindControl("Div1") != null)
                        FindControl("Div1").Visible = true;
                }
            }
            catch (Exception ex)
            {
                // 錯誤處理
                Response.Write("<script>alert('載入資料時發生錯誤：" + ex.Message + "');</script>");
            }
        }

        private DataTable GetFavoriteRecipes(string userId, string searchText, string sortBy)
        {
            DataTable dt = new DataTable();

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                // 以 recipe 為主表，用 favorite 來篩選已收藏的食譜
                string sql = @"
                    SELECT 
                        r.recipe_id,
                        r.title,
                        r.description,
                        r.steps,
                        r.image,
                        f.add_date as favorite_date,
                        u.user_name as author_name
                    FROM recipe r
                    INNER JOIN favorite f ON r.recipe_id = f.recipe_id
                    LEFT JOIN user u ON r.recipe_id = u.user_id
                    WHERE f.user_id = @UserId
                ";

                // 加入搜尋條件
                if (!string.IsNullOrEmpty(searchText))
                {
                    sql += " AND (r.title LIKE @SearchText OR r.description LIKE @SearchText)";
                }

                // 加入排序條件
                switch (sortBy)
                {
                    case "name_asc":
                        sql += " ORDER BY r.title ASC";
                        break;
                    case "name_desc":
                        sql += " ORDER BY r.title DESC";
                        break;
                    case "recent":
                    default:
                        sql += " ORDER BY f.add_date DESC";
                        break;
                }

                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);

                    if (!string.IsNullOrEmpty(searchText))
                    {
                        cmd.Parameters.AddWithValue("@SearchText", "%" + searchText + "%");
                    }

                    conn.Open();
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    adapter.Fill(dt);
                }
            }

            return dt;
        }

        private string GetRecipeIngredients(int recipeId)
        {
            StringBuilder ingredientsList = new StringBuilder();

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                string sql = @"
                    SELECT food_name, quantity
                    FROM ingredient
                    WHERE recipe_id = @RecipeId
                    ORDER BY ingredient_id
                ";

                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@RecipeId", recipeId);

                    conn.Open();
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (ingredientsList.Length > 0)
                                ingredientsList.Append("、");

                            ingredientsList.Append(reader["food_name"].ToString());
                            ingredientsList.Append(" ");
                            ingredientsList.Append(reader["quantity"].ToString());
                        }
                    }
                }
            }

            return ingredientsList.Length > 0 ? ingredientsList.ToString() : "暂无食材資訊";
        }

        private DataTable GetRecipeComments(int recipeId)
        {
            DataTable dt = new DataTable();

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                string sql = @"
                    SELECT 
                        c.content,
                        c.created_date,
                        u.user_name
                    FROM comment c
                    INNER JOIN user u ON c.user_id = u.user_id
                    WHERE c.recipe_id = @RecipeId
                    ORDER BY c.created_date DESC
                    LIMIT 5
                ";

                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@RecipeId", recipeId);

                    conn.Open();
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    adapter.Fill(dt);
                }
            }

            return dt;
        }

        private bool IsSeasonalRecipe(string category)
        {
            // 由於資料庫中沒有 category 欄位，暫時返回 false
            // 如果需要此功能，需要在 recipe 表中添加 category 欄位
            return false;
        }

        private void UpdateStatistics()
        {
            try
            {
                if (Session["UserId"] == null) return;

                string userId = Session["UserId"].ToString();

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    // 總收藏數
                    string totalSql = "SELECT COUNT(*) FROM favorite WHERE user_id = @UserId";
                    using (MySqlCommand cmd = new MySqlCommand(totalSql, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        int totalCount = Convert.ToInt32(cmd.ExecuteScalar());
                        TotalFavoritesLabel.Text = totalCount.ToString();
                    }

                    // 當季推薦數 (暫時設為 0，因為沒有 category 欄位)
                    SeasonalFavoritesLabel.Text = "0";

                    // 本月新增數
                    string recentSql = @"
                        SELECT COUNT(*) FROM favorite 
                        WHERE user_id = @UserId 
                        AND YEAR(add_date) = YEAR(CURDATE()) 
                        AND MONTH(add_date) = MONTH(CURDATE())
                    ";
                    using (MySqlCommand cmd = new MySqlCommand(recentSql, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        int recentCount = Convert.ToInt32(cmd.ExecuteScalar());
                        RecentFavoritesLabel.Text = recentCount.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                // 統計資訊載入失敗不影響主要功能
                Response.Write("<script>console.log('載入統計資訊時發生錯誤：" + ex.Message + "');</script>");
            }
        }

        protected void FilterButton_Click(object sender, EventArgs e)
        {
            LoadFavoriteRecipes();
        }

        protected void FavoriteRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            string recipeId = e.CommandArgument.ToString();

            switch (e.CommandName)
            {
                case "RemoveFavorite":
                    RemoveFromFavorites(recipeId);
                    break;
                case "AddComment":
                    AddComment(recipeId, e);
                    break;
            }
        }

        protected void BulkDeleteButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (Session["UserId"] == null) return;

                string userId = Session["UserId"].ToString();
                List<string> selectedIds = new List<string>();

                // 從前端 JavaScript 取得選中的食譜ID
                string selectedRecipes = Request.Form["selectedRecipes"];

                if (!string.IsNullOrEmpty(selectedRecipes))
                {
                    selectedIds.AddRange(selectedRecipes.Split(','));

                    if (selectedIds.Count > 0)
                    {
                        BulkRemoveFromFavorites(userId, selectedIds);
                        LoadFavoriteRecipes();
                        UpdateStatistics();

                        Response.Write("<script>alert('成功刪除 " + selectedIds.Count + " 個收藏項目');</script>");
                    }
                }
                else
                {
                    Response.Write("<script>alert('請選擇要刪除的項目');</script>");
                }
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('批量刪除時發生錯誤：" + ex.Message + "');</script>");
            }
        }

        private void RemoveFromFavorites(string recipeId)
        {
            try
            {
                if (Session["UserId"] == null) return;

                string userId = Session["UserId"].ToString();

                int recipeIdInt;
                if (!int.TryParse(recipeId, out recipeIdInt))
                {
                    Response.Write("<script>alert('食譜ID格式錯誤');</script>");
                    return;
                }

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    string sql = "DELETE FROM favorite WHERE user_id = @UserId AND recipe_id = @RecipeId";
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        cmd.Parameters.AddWithValue("@RecipeId", recipeIdInt);

                        conn.Open();
                        int result = cmd.ExecuteNonQuery();

                        if (result > 0)
                        {
                            LoadFavoriteRecipes();
                            UpdateStatistics();
                            Response.Write("<script>alert('已成功移除收藏');</script>");
                        }
                        else
                        {
                            Response.Write("<script>alert('移除收藏失敗');</script>");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('移除收藏時發生錯誤：" + ex.Message + "');</script>");
            }
        }

        private void BulkRemoveFromFavorites(string userId, List<string> recipeIds)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (MySqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        foreach (string recipeId in recipeIds)
                        {
                            int recipeIdInt;
                            if (int.TryParse(recipeId, out recipeIdInt))
                            {
                                string sql = "DELETE FROM favorite WHERE user_id = @UserId AND recipe_id = @RecipeId";
                                using (MySqlCommand cmd = new MySqlCommand(sql, conn, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@UserId", userId);
                                    cmd.Parameters.AddWithValue("@RecipeId", recipeIdInt);
                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        private void AddComment(string recipeId, RepeaterCommandEventArgs e)
        {
            try
            {
                if (Session["UserId"] == null) return;

                string userId = Session["UserId"].ToString();
                RepeaterItem item = e.Item;
                TextBox commentTextBox = item.FindControl("CommentTextBox") as TextBox;

                int recipeIdInt;
                if (!int.TryParse(recipeId, out recipeIdInt))
                {
                    Response.Write("<script>alert('食譜ID格式錯誤');</script>");
                    return;
                }

                if (commentTextBox != null && !string.IsNullOrEmpty(commentTextBox.Text.Trim()))
                {
                    string comment = commentTextBox.Text.Trim();

                    using (MySqlConnection conn = new MySqlConnection(connectionString))
                    {
                        string sql = @"
                            INSERT INTO comment (recipe_id, user_id, content, created_date) 
                            VALUES (@RecipeId, @UserId, @Content, @CreatedDate)
                        ";
                        using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                        {
                            cmd.Parameters.AddWithValue("@RecipeId", recipeIdInt);
                            cmd.Parameters.AddWithValue("@UserId", userId);
                            cmd.Parameters.AddWithValue("@Content", comment);
                            cmd.Parameters.AddWithValue("@CreatedDate", DateTime.Now);

                            conn.Open();
                            cmd.ExecuteNonQuery();
                        }
                    }

                    // 清空評論框並重新載入資料
                    commentTextBox.Text = "";
                    LoadFavoriteRecipes();

                    Response.Write("<script>alert('評論已新增');</script>");
                }
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('新增評論時發生錯誤：" + ex.Message + "');</script>");
            }
        }
    }
}