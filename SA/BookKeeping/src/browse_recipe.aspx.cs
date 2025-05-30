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
                // 嘗試多個可能的路徑
                string[] possiblePaths = {
                    Server.MapPath("~/season_ingredients.json"),
                    Server.MapPath("~/src/season_ingredients.json"),
                    Server.MapPath("../season_ingredients.json")
                };

                string jsonContent = "";
                string foundPath = "";

                foreach (string path in possiblePaths)
                {
                    System.Diagnostics.Debug.WriteLine("嘗試路徑: " + path);
                    if (File.Exists(path))
                    {
                        jsonContent = File.ReadAllText(path);
                        foundPath = path;
                        System.Diagnostics.Debug.WriteLine("找到檔案: " + path);
                        break;
                    }
                }

                if (!string.IsNullOrEmpty(jsonContent))
                {
                    seasonalIngredients = JsonConvert.DeserializeObject<List<SeasonalIngredient>>(jsonContent);
                    System.Diagnostics.Debug.WriteLine("成功載入 " + seasonalIngredients.Count + " 個季節食材");

                    // 顯示當前月份的食材（除錯用）
                    int currentMonth = DateTime.Now.Month;
                    string currentSeason = GetCurrentSeason(currentMonth);
                    System.Diagnostics.Debug.WriteLine("當前月份: " + currentMonth + ", 當前季節: " + currentSeason);

                    var currentMonthIngredients = seasonalIngredients.Where(si => si.Month == currentMonth).ToList();
                    System.Diagnostics.Debug.WriteLine("當月食材: " + string.Join(", ", currentMonthIngredients.Select(i => i.Name)));

                    var currentSeasonIngredients = seasonalIngredients.Where(si => si.Season == currentSeason).ToList();
                    System.Diagnostics.Debug.WriteLine("當季食材: " + string.Join(", ", currentSeasonIngredients.Select(i => i.Name)));
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("無法找到 season_ingredients.json 檔案");
                    seasonalIngredients = new List<SeasonalIngredient>();

                    // 建立測試用的季節食材資料
                    CreateTestSeasonalIngredients();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("LoadSeasonalIngredients 錯誤: " + ex.Message);
                seasonalIngredients = new List<SeasonalIngredient>();

                // 建立測試用的季節食材資料
                CreateTestSeasonalIngredients();
            }
        }

        // 建立測試用的季節食材資料
        private void CreateTestSeasonalIngredients()
        {
            int currentMonth = DateTime.Now.Month;
            seasonalIngredients = new List<SeasonalIngredient>();

            // 根據當前月份建立測試資料
            switch (currentMonth)
            {
                case 1:
                case 2:
                case 12: // 冬季
                    seasonalIngredients.AddRange(new[] {
                        new SeasonalIngredient { Month = currentMonth, Season = "冬季", Name = "高麗菜" },
                        new SeasonalIngredient { Month = currentMonth, Season = "冬季", Name = "白蘿蔔" },
                        new SeasonalIngredient { Month = currentMonth, Season = "冬季", Name = "菠菜" },
                        new SeasonalIngredient { Month = currentMonth, Season = "冬季", Name = "花椰菜" }
                    });
                    break;
                case 3:
                case 4:
                case 5: // 春季
                    seasonalIngredients.AddRange(new[] {
                        new SeasonalIngredient { Month = currentMonth, Season = "春季", Name = "蘆筍" },
                        new SeasonalIngredient { Month = currentMonth, Season = "春季", Name = "番茄" },
                        new SeasonalIngredient { Month = currentMonth, Season = "春季", Name = "小黃瓜" },
                        new SeasonalIngredient { Month = currentMonth, Season = "春季", Name = "茄子" }
                    });
                    break;
                case 6:
                case 7:
                case 8: // 夏季
                    seasonalIngredients.AddRange(new[] {
                        new SeasonalIngredient { Month = currentMonth, Season = "夏季", Name = "苦瓜" },
                        new SeasonalIngredient { Month = currentMonth, Season = "夏季", Name = "茄子" },
                        new SeasonalIngredient { Month = currentMonth, Season = "夏季", Name = "絲瓜" },
                        new SeasonalIngredient { Month = currentMonth, Season = "夏季", Name = "空心菜" }
                    });
                    break;
                case 9:
                case 10:
                case 11: // 秋季
                    seasonalIngredients.AddRange(new[] {
                        new SeasonalIngredient { Month = currentMonth, Season = "秋季", Name = "南瓜" },
                        new SeasonalIngredient { Month = currentMonth, Season = "秋季", Name = "地瓜" },
                        new SeasonalIngredient { Month = currentMonth, Season = "秋季", Name = "芋頭" },
                        new SeasonalIngredient { Month = currentMonth, Season = "秋季", Name = "高麗菜" }
                    });
                    break;
            }

            System.Diagnostics.Debug.WriteLine("建立測試用季節食材: " + string.Join(", ", seasonalIngredients.Select(i => i.Name)));
        }

        private void LoadRecipes(string searchTerm = "")
        {
            try
            {
                List<RecipeData> recipes = GetRecipesFromDatabase(searchTerm);

                // 按當季食材優先排序
                recipes = recipes.OrderByDescending(r => r.IsSeasonal).ThenBy(r => r.title).ToList();

                // 除錯：顯示有多少當季食譜
                int seasonalCount = recipes.Count(r => r.IsSeasonal);
                System.Diagnostics.Debug.WriteLine("找到 " + seasonalCount + " 個當季食譜，總計 " + recipes.Count + " 個食譜");

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
                System.Diagnostics.Debug.WriteLine("LoadRecipes 錯誤: " + ex.Message);
                NoRecipesLabel.Text = "載入食譜時發生錯誤：" + ex.Message;
                NoRecipesLabel.Visible = true;
            }
        }

        private List<RecipeData> GetRecipesFromDatabase(string searchTerm = "")
        {
            List<RecipeData> recipes = new List<RecipeData>();
            string currentUserId = Session["UserID"].ToString();

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
                            string imageFromDB = reader.GetString("image");
                            string imagePath = ProcessImagePath(imageFromDB);
                            string ingredients = reader.GetString("ingredients");
                            string title = reader.GetString("title");

                            bool isSeasonal = IsRecipeSeasonal(ingredients, title);

                            // 詳細除錯資訊
                            System.Diagnostics.Debug.WriteLine("=== 食譜分析 ===");
                            System.Diagnostics.Debug.WriteLine("食譜名稱: " + title);
                            System.Diagnostics.Debug.WriteLine("食材內容: " + ingredients);
                            System.Diagnostics.Debug.WriteLine("是否當季: " + isSeasonal);
                            System.Diagnostics.Debug.WriteLine("================");

                            var recipe = new RecipeData
                            {
                                recipe_id = reader.GetInt32("recipe_id"),
                                title = title,
                                description = reader.GetString("description"),
                                steps = reader.GetString("steps"),
                                image = imagePath,
                                ingredients = ingredients,
                                IsFavorited = reader.GetInt32("is_favorited") == 1,
                                IsSeasonal = isSeasonal,
                                Comments = new List<CommentData>()
                            };
                            recipes.Add(recipe);
                        }
                    }
                }

                // 載入每個食譜的評論
                foreach (var recipe in recipes)
                {
                    recipe.Comments = GetRecipeComments(conn, recipe.recipe_id);
                }
            }

            return recipes;
        }

        private string ProcessImagePath(string imageFromDB)
        {
            if (string.IsNullOrEmpty(imageFromDB) || string.IsNullOrWhiteSpace(imageFromDB))
            {
                return "~/src/recipes/default_recipe.jpg";
            }

            if (imageFromDB.StartsWith("src/recipes/"))
            {
                return "~/" + imageFromDB;
            }

            if (!imageFromDB.Contains("/") && !string.IsNullOrEmpty(imageFromDB))
            {
                return "~/src/recipes/" + imageFromDB;
            }

            if (string.IsNullOrEmpty(imageFromDB) || imageFromDB.Trim() == "")
            {
                return "~/src/recipes/default_recipe.jpg";
            }

            return "~/" + imageFromDB.TrimStart('/');
        }

        private string BuildSearchWhereClause(string searchTerm, List<MySqlParameter> parameters)
        {
            searchTerm = searchTerm.Trim();

            if (int.TryParse(searchTerm, out int month) && month >= 1 && month <= 12)
            {
                return BuildSeasonalSearchQuery(month, "", parameters);
            }

            if (searchTerm.Contains("季"))
            {
                return BuildSeasonalSearchQuery(0, searchTerm, parameters);
            }

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

            return " WHERE 1=0";
        }

        // 簡化且強化的季節判斷方法
        private bool IsRecipeSeasonal(string ingredients, string title)
        {
            if (seasonalIngredients == null || seasonalIngredients.Count == 0)
            {
                System.Diagnostics.Debug.WriteLine("季節食材清單為空");
                return false;
            }

            if (string.IsNullOrEmpty(ingredients))
            {
                System.Diagnostics.Debug.WriteLine("食材清單為空");
                return false;
            }

            int currentMonth = DateTime.Now.Month;
            string currentSeason = GetCurrentSeason(currentMonth);

            // 獲取當月和當季的所有食材
            var currentSeasonalFoods = seasonalIngredients
                .Where(si => si.Month == currentMonth || si.Season == currentSeason)
                .Select(si => si.Name)
                .Distinct()
                .ToList();

            System.Diagnostics.Debug.WriteLine("檢查食譜: " + title);
            System.Diagnostics.Debug.WriteLine("當前季節食材: " + string.Join(", ", currentSeasonalFoods));
            System.Diagnostics.Debug.WriteLine("食譜食材: " + ingredients);

            // 簡化的匹配邏輯
            foreach (var seasonalFood in currentSeasonalFoods)
            {
                // 直接檢查是否包含
                if (ingredients.Contains(seasonalFood))
                {
                    System.Diagnostics.Debug.WriteLine("找到匹配的季節食材: " + seasonalFood);
                    return true;
                }

                // 檢查食譜標題是否包含季節食材
                if (title.Contains(seasonalFood))
                {
                    System.Diagnostics.Debug.WriteLine("標題包含季節食材: " + seasonalFood);
                    return true;
                }
            }

            // 更寬鬆的匹配：檢查關鍵字
            foreach (var seasonalFood in currentSeasonalFoods)
            {
                // 如果季節食材名稱長度 >= 2，檢查前兩個字
                if (seasonalFood.Length >= 2)
                {
                    string keyWord = seasonalFood.Substring(0, 2);
                    if (ingredients.Contains(keyWord) || title.Contains(keyWord))
                    {
                        System.Diagnostics.Debug.WriteLine("找到關鍵字匹配: " + keyWord + " (來自 " + seasonalFood + ")");
                        return true;
                    }
                }
            }

            System.Diagnostics.Debug.WriteLine("未找到匹配的季節食材");
            return false;
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

            if (!int.TryParse(e.CommandArgument.ToString(), out int recipeId))
            {
                return;
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

                    string checkQuery = "SELECT COUNT(*) FROM favorite WHERE user_id = @userId AND recipe_id = @recipeId";
                    using (MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@userId", userId);
                        checkCmd.Parameters.AddWithValue("@recipeId", recipeId);

                        int count = Convert.ToInt32(checkCmd.ExecuteScalar());

                        if (count > 0)
                        {
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
                System.Diagnostics.Debug.WriteLine("ToggleFavorite 錯誤: " + ex.Message);
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
                System.Diagnostics.Debug.WriteLine("AddComment 錯誤: " + ex.Message);
            }
        }
    }
}