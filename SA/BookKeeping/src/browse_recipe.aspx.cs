using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace _BookKeeping.src
{
    public partial class browse_recipe : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;
        private List<SeasonalIngredient> seasonalIngredients;

        public class SeasonalIngredient
        {
            public int Month { get; set; }
            public string Season { get; set; }
            public string Name { get; set; }
        }

        public class RecipeData
        {
            public int recipe_id { get; set; }
            public string title { get; set; }
            public string description { get; set; }
            public string steps { get; set; }
            public string image { get; set; }
            public string ingredients { get; set; }
            public bool IsSeasonal { get; set; }
            public bool IsFavorited { get; set; }
            public List<CommentData> Comments { get; set; }
        }

        public class CommentData
        {
            public int comment_id { get; set; }
            public string user_id { get; set; }
            public string user_name { get; set; }
            public int recipe_id { get; set; }
            public string content { get; set; }
            public DateTime created_date { get; set; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // 檢查用戶是否已登入
            if (Session["UserID"] == null)
            {
                Response.Redirect("~/src/login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadSeasonalIngredients();
                LoadRecipes();
            }
        }

        private void LoadSeasonalIngredients()
        {
            try
            {
                string jsonPath = Server.MapPath("~/season_ingredients.json");
                if (File.Exists(jsonPath))
                {
                    string jsonContent = File.ReadAllText(jsonPath);
                    seasonalIngredients = JsonConvert.DeserializeObject<List<SeasonalIngredient>>(jsonContent);
                }
                else
                {
                    seasonalIngredients = new List<SeasonalIngredient>();
                }
            }
            catch (Exception ex)
            {
                // 記錄錯誤，使用空清單
                System.Diagnostics.Debug.WriteLine("LoadSeasonalIngredients error: " + ex.Message);
                seasonalIngredients = new List<SeasonalIngredient>();
            }
        }

        private void LoadRecipes(string searchTerm = "")
        {
            try
            {
                List<RecipeData> recipes = GetRecipesFromDatabase(searchTerm);

                // 按當季食材優先排序
                recipes = recipes.OrderByDescending(r => r.IsSeasonal).ThenBy(r => r.title).ToList();

                if (recipes.Count > 0)
                {
                    RecipeRepeater.DataSource = recipes;
                    RecipeRepeater.DataBind();
                    NoRecipesLabel.Visible = false;
                }
                else
                {
                    RecipeRepeater.DataSource = null;
                    RecipeRepeater.DataBind();
                    NoRecipesLabel.Visible = true;
                }
            }
            catch (Exception ex)
            {
                // 處理錯誤
                NoRecipesLabel.Text = "載入食譜時發生錯誤：" + ex.Message;
                NoRecipesLabel.Visible = true;
            }
        }

        private List<RecipeData> GetRecipesFromDatabase(string searchTerm = "")
        {
            List<RecipeData> recipes = new List<RecipeData>();
            string currentUserId = Session["UserID"].ToString();
            int currentMonth = DateTime.Now.Month;
            string currentSeason = GetCurrentSeason(currentMonth);

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                string baseQuery = @"
            SELECT DISTINCT r.recipe_id, r.title, r.description, r.steps, r.image,
                   COALESCE(GROUP_CONCAT(CONCAT(i.food_name, '(', i.quantity, ')') SEPARATOR ', '), '') as ingredients,
                   CASE WHEN f.recipe_id IS NOT NULL THEN 1 ELSE 0 END as is_favorited
            FROM recipe r
            LEFT JOIN ingredient i ON r.recipe_id = i.recipe_id
            LEFT JOIN favorite f ON r.recipe_id = f.recipe_id AND f.user_id = @userId";

                string whereClause = "";
                List<MySqlParameter> parameters = new List<MySqlParameter>
        {
            new MySqlParameter("@userId", currentUserId)
        };

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    whereClause = BuildSearchWhereClause(searchTerm, parameters);
                }

                string fullQuery = baseQuery + whereClause + " GROUP BY r.recipe_id, r.title, r.description, r.steps, r.image, f.recipe_id";

                using (MySqlCommand cmd = new MySqlCommand(fullQuery, conn))
                {
                    cmd.Parameters.AddRange(parameters.ToArray());

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var recipe = new RecipeData
                            {
                                recipe_id = reader.GetInt32("recipe_id"),
                                title = reader.GetString("title"),
                                description = reader.GetString("description"),
                                steps = reader.GetString("steps"),
                                image = "~/"+reader.GetString("image"),
                                ingredients = reader.GetString("ingredients"), // 移除 IsDBNull 檢查，因為已用 COALESCE 處理
                                IsFavorited = reader.GetInt32("is_favorited") == 1,
                                IsSeasonal = IsRecipeSeasonal(reader.GetString("ingredients")),
                                Comments = new List<CommentData>()
                            };
                            recipes.Add(recipe);
                        }
                    }
                }

                // 載入每個食譜的評論 - 修正：在 using 區塊內調用
                foreach (var recipe in recipes)
                {
                    recipe.Comments = GetRecipeComments(conn, recipe.recipe_id);
                }
            } // 連線在這裡才會被釋放

            return recipes;
        }

        private string BuildSearchWhereClause(string searchTerm, List<MySqlParameter> parameters)
        {
            searchTerm = searchTerm.Trim();

            // 檢查是否為月份搜尋 (1-12)
            if (int.TryParse(searchTerm, out int month) && month >= 1 && month <= 12)
            {
                return BuildSeasonalSearchQuery(month, "", parameters);
            }

            // 檢查是否為季節搜尋
            if (searchTerm.Contains("季"))
            {
                return BuildSeasonalSearchQuery(0, searchTerm, parameters);
            }

            // 一般搜尋 (食譜名稱或食材)
            parameters.Add(new MySqlParameter("@searchTerm", "%" + searchTerm + "%"));
            return " WHERE (r.title LIKE @searchTerm OR i.food_name LIKE @searchTerm)";
        }

        private string BuildSeasonalSearchQuery(int month, string season, List<MySqlParameter> parameters)
        {
            List<string> seasonalFoods = new List<string>();

            if (month > 0)
            {
                seasonalFoods = seasonalIngredients
                    .Where(si => si.Month == month)
                    .Select(si => si.Name)
                    .ToList();
            }
            else if (!string.IsNullOrEmpty(season))
            {
                seasonalFoods = seasonalIngredients
                    .Where(si => si.Season == season)
                    .Select(si => si.Name)
                    .Distinct()
                    .ToList();
            }

            if (seasonalFoods.Count > 0)
            {
                string foodConditions = string.Join(" OR ",
                    seasonalFoods.Select((food, index) =>
                    {
                        string paramName = "@seasonalFood" + index;
                        parameters.Add(new MySqlParameter(paramName, food));
                        return "i.food_name = " + paramName;
                    }));

                return " WHERE (" + foodConditions + ")";
            }

            return " WHERE 1=0"; // 沒有找到當季食材，返回空結果
        }

        private bool IsRecipeSeasonal(string ingredients)
        {
            if (string.IsNullOrEmpty(ingredients) || seasonalIngredients == null)
                return false;

            int currentMonth = DateTime.Now.Month;
            var currentSeasonalFoods = seasonalIngredients
                .Where(si => si.Month == currentMonth)
                .Select(si => si.Name)
                .ToList();

            return currentSeasonalFoods.Any(food => ingredients.Contains(food));
        }

        private List<CommentData> GetRecipeComments(MySqlConnection conn, int recipeId)
        {
            List<CommentData> comments = new List<CommentData>();

            string query = @"
                SELECT c.comment_id, c.user_id, u.user_name, c.recipe_id, c.content, c.created_date
                FROM comment c
                JOIN user u ON c.user_id = u.user_id
                WHERE c.recipe_id = @recipeId
                ORDER BY c.created_date DESC
                LIMIT 5";

            using (MySqlCommand cmd = new MySqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@recipeId", recipeId);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        comments.Add(new CommentData
                        {
                            comment_id = reader.GetInt32("comment_id"),
                            user_id = reader.GetString("user_id"),
                            user_name = reader.GetString("user_name"),
                            recipe_id = reader.GetInt32("recipe_id"),
                            content = reader.GetString("content"),
                            created_date = reader.GetDateTime("created_date")
                        });
                    }
                }
            }

            return comments;
        }

        private string GetCurrentSeason(int month)
        {
            switch (month)
            {
                case 12:
                case 1:
                case 2:
                    return "冬季";
                case 3:
                case 4:
                case 5:
                    return "春季";
                case 6:
                case 7:
                case 8:
                    return "夏季";
                case 9:
                case 10:
                case 11:
                    return "秋季";
                default:
                    return "春季";
            }
        }

        protected void SearchButton_Click(object sender, EventArgs e)
        {
            LoadSeasonalIngredients();
            LoadRecipes(SearchTextBox.Text);
        }

        protected void RecipeRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (Session["UserID"] == null) return;

            string currentUserId = Session["UserID"].ToString();

            // Fix: Convert CommandArgument to int properly
            if (!int.TryParse(e.CommandArgument.ToString(), out int recipeId))
            {
                return; // Invalid recipe ID
            }

            if (e.CommandName == "ToggleFavorite")
            {
                ToggleFavorite(currentUserId, recipeId);
            }
            else if (e.CommandName == "AddComment")
            {
                var commentTextBox = e.Item.FindControl("CommentTextBox") as TextBox;
                if (commentTextBox != null && !string.IsNullOrEmpty(commentTextBox.Text.Trim()))
                {
                    AddComment(currentUserId, recipeId, commentTextBox.Text.Trim());
                    commentTextBox.Text = "";
                }
            }

            // 重新載入食譜以更新狀態
            LoadSeasonalIngredients();
            LoadRecipes(SearchTextBox.Text);
        }

        private void ToggleFavorite(string userId, int recipeId)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    // 檢查是否已收藏
                    string checkQuery = "SELECT COUNT(*) FROM favorite WHERE user_id = @userId AND recipe_id = @recipeId";
                    using (MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@userId", userId);
                        checkCmd.Parameters.AddWithValue("@recipeId", recipeId);

                        int count = Convert.ToInt32(checkCmd.ExecuteScalar());

                        if (count > 0)
                        {
                            // 移除收藏
                            string deleteQuery = "DELETE FROM favorite WHERE user_id = @userId AND recipe_id = @recipeId";
                            using (MySqlCommand deleteCmd = new MySqlCommand(deleteQuery, conn))
                            {
                                deleteCmd.Parameters.AddWithValue("@userId", userId);
                                deleteCmd.Parameters.AddWithValue("@recipeId", recipeId);
                                deleteCmd.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            // 新增收藏
                            string insertQuery = "INSERT INTO favorite (user_id, recipe_id, add_date) VALUES (@userId, @recipeId, @addDate)";
                            using (MySqlCommand insertCmd = new MySqlCommand(insertQuery, conn))
                            {
                                insertCmd.Parameters.AddWithValue("@userId", userId);
                                insertCmd.Parameters.AddWithValue("@recipeId", recipeId);
                                insertCmd.Parameters.AddWithValue("@addDate", DateTime.Now);
                                insertCmd.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // 處理錯誤 - Log the error for debugging
                System.Diagnostics.Debug.WriteLine("ToggleFavorite error: " + ex.Message);
            }
        }

        private void AddComment(string userId, int recipeId, string content)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    string insertQuery = "INSERT INTO comment (user_id, recipe_id, content, created_date) VALUES (@userId, @recipeId, @content, @createdDate)";
                    using (MySqlCommand cmd = new MySqlCommand(insertQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@userId", userId);
                        cmd.Parameters.AddWithValue("@recipeId", recipeId);
                        cmd.Parameters.AddWithValue("@content", content);
                        cmd.Parameters.AddWithValue("@createdDate", DateTime.Now);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                // 處理錯誤 - Log the error for debugging
                System.Diagnostics.Debug.WriteLine("AddComment error: " + ex.Message);
            }
        }
    }
}