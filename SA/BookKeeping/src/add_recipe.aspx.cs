using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;

namespace _BookKeeping.src
{
    public partial class add_recipe : System.Web.UI.Page
    {
        // 資料庫連接字串 - 請根據您的實際設定修改
        private string connectionString = ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            // 檢查用戶是否已登入
            if (Session["UserID"] == null)
            {
                // 如果沒有登入，重導向到登入頁面
                Response.Redirect("~/src/login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                // 頁面首次載入時的初始化
                MessagePanel.Visible = false;
            }
        }

        protected void SaveRecipeBtn_Click(object sender, EventArgs e)
        {
            try
            {
                // 驗證必填欄位
                if (string.IsNullOrWhiteSpace(RecipeTitleTextBox.Text) ||
                    string.IsNullOrWhiteSpace(RecipeDescriptionTextBox.Text) ||
                    string.IsNullOrWhiteSpace(RecipeStepsTextBox.Text))
                {
                    ShowMessage("請填寫所有必填欄位！", "danger");
                    return;
                }

                // 處理圖片上傳
                string imagePath = "";
                if (RecipeImageUpload.HasFile)
                {
                    imagePath = ProcessImageUpload();
                    if (string.IsNullOrEmpty(imagePath))
                    {
                        ShowMessage("圖片上傳失敗，請重試！", "danger");
                        return;
                    }
                }
                else
                {
                    // 修改：如果沒有上傳圖片，使用預設圖片的相對路徑
                    imagePath = "src/recipes/default_recipe.jpg";
                }

                // 收集食材資料
                List<Ingredient> ingredients = CollectIngredients();
                if (ingredients.Count == 0)
                {
                    ShowMessage("請至少新增一個食材！", "danger");
                    return;
                }

                // 儲存到資料庫
                int newRecipeId = SaveRecipeToDatabase(imagePath, ingredients);

                if (newRecipeId > 0)
                {
                    ShowMessage("食譜新增成功！", "success");
                    // 清空表單
                    ClearForm();

                    // 3秒後自動跳轉回主頁面
                    ClientScript.RegisterStartupScript(this.GetType(), "redirect",
                        "setTimeout(function() { window.location = 'main.aspx'; }, 3000);", true);
                }
                else
                {
                    ShowMessage("食譜儲存失敗，請重試！", "danger");
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"發生錯誤：{ex.Message}", "danger");
                // 記錄錯誤到系統日誌
                System.Diagnostics.Debug.WriteLine($"SaveRecipe Error: {ex.Message}");
            }
        }

        private string ProcessImageUpload()
        {
            try
            {
                if (!RecipeImageUpload.HasFile)
                    return "";

                // 檢查檔案大小 (5MB限制)
                if (RecipeImageUpload.PostedFile.ContentLength > 5242880)
                {
                    ShowMessage("圖片檔案大小不能超過 5MB！", "danger");
                    return "";
                }

                // 檢查檔案類型
                string fileExtension = Path.GetExtension(RecipeImageUpload.FileName).ToLower();
                string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };

                if (!allowedExtensions.Contains(fileExtension))
                {
                    ShowMessage("只支援 JPG、PNG、GIF 格式的圖片！", "danger");
                    return "";
                }

                // 產生唯一檔名
                string fileName = $"recipe_{DateTime.Now:yyyyMMddHHmmss}_{Guid.NewGuid().ToString("N").Substring(0, 8)}{fileExtension}";

                // 修改：設定新的儲存路徑為 ~/src/recipes/
                string uploadFolder = Server.MapPath("~/src/recipes/");

                // 確保目錄存在
                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }

                string filePath = Path.Combine(uploadFolder, fileName);

                // 儲存檔案到本地端
                RecipeImageUpload.SaveAs(filePath);

                // 記錄成功訊息
                System.Diagnostics.Debug.WriteLine($"圖片已成功儲存到: {filePath}");

                // 修改：返回相對路徑，以便資料庫儲存和後續顯示
                // 這樣儲存在資料庫中的路徑格式為：src/recipes/filename.jpg
                return $"src/recipes/{fileName}";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Image Upload Error: {ex.Message}");
                ShowMessage($"圖片上傳錯誤: {ex.Message}", "danger");
                return "";
            }
        }
        private List<Ingredient> CollectIngredients()
        {
            List<Ingredient> ingredients = new List<Ingredient>();

            try
            {
                // 獲取食材數量
                int ingredientCount = 1;
                if (!string.IsNullOrEmpty(IngredientCountHidden.Value))
                {
                    ingredientCount = int.Parse(IngredientCountHidden.Value);
                }

                // 收集第一個食材（使用server control）
                string firstName = IngredientName1.Text.Trim();
                string firstQuantity = IngredientQuantity1.Text.Trim();

                if (!string.IsNullOrEmpty(firstName) && !string.IsNullOrEmpty(firstQuantity))
                {
                    ingredients.Add(new Ingredient { Name = firstName, Quantity = firstQuantity });
                }

                // 收集動態新增的食材
                // 修正：確保能正確收集所有動態新增的食材
                for (int i = 2; i <= ingredientCount; i++)
                {
                    string nameKey = $"IngredientName{i}";
                    string quantityKey = $"IngredientQuantity{i}";

                    // 從 Request.Form 獲取值
                    string name = Request.Form[nameKey];
                    string quantity = Request.Form[quantityKey];

                    // 除錯輸出
                    System.Diagnostics.Debug.WriteLine($"嘗試獲取食材 {i}: Name={name}, Quantity={quantity}");

                    if (!string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(quantity))
                    {
                        ingredients.Add(new Ingredient { Name = name.Trim(), Quantity = quantity.Trim() });
                        System.Diagnostics.Debug.WriteLine($"成功新增食材 {i}: {name} - {quantity}");
                    }
                }

                // 除錯輸出總數
                System.Diagnostics.Debug.WriteLine($"總共收集到 {ingredients.Count} 個食材");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Collect Ingredients Error: {ex.Message}");
                ShowMessage($"收集食材時發生錯誤: {ex.Message}", "danger");
            }

            return ingredients;
        }

        private int SaveRecipeToDatabase(string imagePath, List<Ingredient> ingredients)
        {
            int newRecipeId = 0;
            MySqlConnection connection = null;
            MySqlTransaction transaction = null;

            try
            {
                connection = new MySqlConnection(connectionString);
                connection.Open();
                transaction = connection.BeginTransaction();

                // 插入食譜主資料
                string recipeQuery = @"
                    INSERT INTO recipe (title, description, steps, image) 
                    VALUES (@title, @description, @steps, @image);
                    SELECT LAST_INSERT_ID();";

                using (MySqlCommand cmd = new MySqlCommand(recipeQuery, connection, transaction))
                {
                    cmd.Parameters.AddWithValue("@title", RecipeTitleTextBox.Text.Trim());
                    cmd.Parameters.AddWithValue("@description", RecipeDescriptionTextBox.Text.Trim());
                    cmd.Parameters.AddWithValue("@steps", RecipeStepsTextBox.Text.Trim());
                    cmd.Parameters.AddWithValue("@image", imagePath);

                    newRecipeId = Convert.ToInt32(cmd.ExecuteScalar());
                }

                // 插入食材資料
                if (newRecipeId > 0 && ingredients.Count > 0)
                {
                    string ingredientQuery = @"
                        INSERT INTO ingredient (food_name, quantity, recipe_id) 
                        VALUES (@food_name, @quantity, @recipe_id)";

                    foreach (var ingredient in ingredients)
                    {
                        using (MySqlCommand cmd = new MySqlCommand(ingredientQuery, connection, transaction))
                        {
                            cmd.Parameters.AddWithValue("@food_name", ingredient.Name);
                            cmd.Parameters.AddWithValue("@quantity", ingredient.Quantity);
                            cmd.Parameters.AddWithValue("@recipe_id", newRecipeId);
                            cmd.ExecuteNonQuery();

                            // 除錯輸出
                            System.Diagnostics.Debug.WriteLine($"已儲存食材: {ingredient.Name} - {ingredient.Quantity}");
                        }
                    }
                }

                // 提交交易
                transaction.Commit();
                System.Diagnostics.Debug.WriteLine($"食譜已成功儲存，ID: {newRecipeId}");
            }
            catch (Exception ex)
            {
                // 回滾交易
                transaction?.Rollback();
                throw new Exception($"資料庫操作失敗：{ex.Message}");
            }
            finally
            {
                connection?.Close();
            }

            return newRecipeId;
        }

        protected void CancelBtn_Click(object sender, EventArgs e)
        {
            // 返回主頁面
            Response.Redirect("~/src/main.aspx");
        }

        // 新增：返回主頁面按鈕事件
        protected void BackToMainBtn_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/src/main.aspx");
        }

        private void ShowMessage(string message, string type)
        {
            MessagePanel.Visible = true;
            MessageLabel.Text = message;
            MessageLabel.CssClass = $"alert alert-{type}";
        }

        private void ClearForm()
        {
            RecipeTitleTextBox.Text = "";
            RecipeDescriptionTextBox.Text = "";
            RecipeStepsTextBox.Text = "";
            IngredientName1.Text = "";
            IngredientQuantity1.Text = "";
            IngredientCountHidden.Value = "1";

            // 清空動態新增的食材欄位
            ClientScript.RegisterStartupScript(this.GetType(), "clearIngredients",
                "document.getElementById('ingredientsContainer').innerHTML = document.getElementById('ingredientsContainer').children[0].outerHTML;", true);
        }

        // 食材類別
        public class Ingredient
        {
            public string Name { get; set; }
            public string Quantity { get; set; }
        }
    }
}