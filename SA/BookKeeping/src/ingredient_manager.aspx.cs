using MySql.Data.MySqlClient;
using System;
using System.Configuration;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace _BookKeeping
{
    public partial class ingredient_manager : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            // 檢查用戶是否已登入且為管理者
            if (Session["UserID"] == null)
            {
                Response.Redirect("~/src/login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadRecipeDropDowns();
                LoadIngredientDropDowns();
                CurrentTab.Value = "add";
            }
        }

        protected void BackBtn_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/src/manager_main.aspx");
        }

        #region 新增食材功能
        protected void SaveAddBtn_Click(object sender, EventArgs e)
        {
            CurrentTab.Value = "add";
            try
            {
                if (AddRecipeDropDown.SelectedValue == "")
                {
                    ShowAddMessage("請選擇食譜", "error");
                    return;
                }

                string foodName = AddFoodName.Text.Trim();
                string quantity = AddQuantity.Text.Trim();
                int recipeId = int.Parse(AddRecipeDropDown.SelectedValue);

                if (string.IsNullOrEmpty(foodName))
                {
                    ShowAddMessage("請輸入食材名稱", "error");
                    return;
                }

                if (string.IsNullOrEmpty(quantity))
                {
                    ShowAddMessage("請輸入數量", "error");
                    return;
                }

                // 檢查是否已存在相同的食材在同一食譜中
                if (IsIngredientExist(foodName, recipeId))
                {
                    ShowAddMessage("此食譜中已存在相同的食材，請選擇修改功能", "error");
                    return;
                }

                string sql = @"INSERT INTO ingredient (food_name, quantity, recipe_id) 
                              VALUES (@food_name, @quantity, @recipe_id)";

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@food_name", foodName);
                        cmd.Parameters.AddWithValue("@quantity", quantity);
                        cmd.Parameters.AddWithValue("@recipe_id", recipeId);

                        int result = cmd.ExecuteNonQuery();
                        if (result > 0)
                        {
                            ShowAddMessage("食材新增成功！", "success");
                            ClearAddForm();
                            LoadIngredientDropDowns();
                        }
                        else
                        {
                            ShowAddMessage("食材新增失敗，請再試一次", "error");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowAddMessage($"新增食材時發生錯誤：{ex.Message}", "error");
            }
        }

        private bool IsIngredientExist(string foodName, int recipeId)
        {
            string sql = "SELECT COUNT(*) FROM ingredient WHERE LOWER(TRIM(food_name)) = LOWER(TRIM(@food_name)) AND recipe_id = @recipe_id";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@food_name", foodName);
                    cmd.Parameters.AddWithValue("@recipe_id", recipeId);

                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
        }

        private void ClearAddForm()
        {
            AddFoodName.Text = "";
            AddQuantity.Text = "";
            AddRecipeDropDown.SelectedIndex = 0;
        }
        #endregion

        #region 修改食材功能
        protected void EditIngredientDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            CurrentTab.Value = "edit";

            if (EditIngredientDropDown.SelectedValue != "")
            {
                LoadIngredientForEdit(int.Parse(EditIngredientDropDown.SelectedValue));
            }
            else
            {
                ClearEditForm();
            }
        }

        private void LoadIngredientForEdit(int ingredientId)
        {
            string sql = @"SELECT i.*, r.title as recipe_title 
                          FROM ingredient i 
                          INNER JOIN recipe r ON i.recipe_id = r.recipe_id 
                          WHERE i.ingredient_id = @ingredient_id";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@ingredient_id", ingredientId);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            EditFoodName.Text = reader["food_name"].ToString();
                            EditQuantity.Text = reader["quantity"].ToString();
                            EditRecipeDropDown.SelectedValue = reader["recipe_id"].ToString();
                        }
                    }
                }
            }
        }

        private void ClearEditForm()
        {
            EditFoodName.Text = "";
            EditQuantity.Text = "";
            EditRecipeDropDown.SelectedIndex = 0;
        }

        protected void SaveEditBtn_Click(object sender, EventArgs e)
        {
            CurrentTab.Value = "edit";

            try
            {
                if (EditIngredientDropDown.SelectedValue == "")
                {
                    ShowEditMessage("請選擇要修改的食材", "error");
                    return;
                }

                if (EditRecipeDropDown.SelectedValue == "")
                {
                    ShowEditMessage("請選擇食譜", "error");
                    return;
                }

                int ingredientId = int.Parse(EditIngredientDropDown.SelectedValue);
                string foodName = EditFoodName.Text.Trim();
                string quantity = EditQuantity.Text.Trim();
                int recipeId = int.Parse(EditRecipeDropDown.SelectedValue);

                if (string.IsNullOrEmpty(foodName))
                {
                    ShowEditMessage("請輸入食材名稱", "error");
                    return;
                }

                if (string.IsNullOrEmpty(quantity))
                {
                    ShowEditMessage("請輸入數量", "error");
                    return;
                }

                // 先取得原始資料進行比較
                var originalData = GetOriginalIngredientData(ingredientId);
                if (originalData == null)
                {
                    ShowEditMessage("找不到要修改的食材資料", "error");
                    return;
                }

                // 如果食材名稱或食譜有變更，才需要檢查重複
                bool needCheckDuplicate = (originalData.Value.Item1 != foodName || originalData.Value.Item2 != recipeId);

                if (needCheckDuplicate && IsIngredientExistForEdit(foodName, recipeId, ingredientId))
                {
                    ShowEditMessage("此食譜中已存在相同的食材名稱", "error");
                    return;
                }

                string sql = @"UPDATE ingredient SET food_name = @food_name, quantity = @quantity, 
                      recipe_id = @recipe_id WHERE ingredient_id = @ingredient_id";

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@food_name", foodName);
                        cmd.Parameters.AddWithValue("@quantity", quantity);
                        cmd.Parameters.AddWithValue("@recipe_id", recipeId);
                        cmd.Parameters.AddWithValue("@ingredient_id", ingredientId);

                        int result = cmd.ExecuteNonQuery();
                        if (result > 0)
                        {
                            LoadIngredientDropDowns();
                            ClearEditForm();
                            EditIngredientDropDown.SelectedIndex = 0;
                            CurrentTab.Value = "edit";
                            ShowEditMessage("食材修改成功！", "success");
                        }
                        else
                        {
                            ShowEditMessage("食材修改失敗，請再試一次", "error");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowEditMessage($"修改食材時發生錯誤：{ex.Message}", "error");
            }
        }

        private bool IsIngredientExistForEdit(string foodName, int recipeId, int excludeIngredientId)
        {
            string sql = @"SELECT COUNT(*) FROM ingredient 
                          WHERE LOWER(TRIM(food_name)) = LOWER(TRIM(@food_name)) 
                          AND recipe_id = @recipe_id 
                          AND ingredient_id != @ingredient_id";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@food_name", foodName);
                    cmd.Parameters.AddWithValue("@recipe_id", recipeId);
                    cmd.Parameters.AddWithValue("@ingredient_id", excludeIngredientId);

                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
        }

        private (string, int, string)? GetOriginalIngredientData(int ingredientId)
        {
            string sql = "SELECT food_name, recipe_id, quantity FROM ingredient WHERE ingredient_id = @ingredient_id";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@ingredient_id", ingredientId);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return (
                                reader["food_name"].ToString().Trim(),
                                Convert.ToInt32(reader["recipe_id"]),
                                reader["quantity"].ToString().Trim()
                            );
                        }
                    }
                }
            }
            return null;
        }
        #endregion

        #region 刪除食材功能
        protected void DeleteIngredientDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            CurrentTab.Value = "delete";

            if (DeleteIngredientDropDown.SelectedValue != "")
            {
                LoadIngredientForDelete(int.Parse(DeleteIngredientDropDown.SelectedValue));
                DeletePreviewPanel.Visible = true;
            }
            else
            {
                DeletePreviewPanel.Visible = false;
            }
        }

        private void LoadIngredientForDelete(int ingredientId)
        {
            string sql = @"SELECT i.*, r.title as recipe_title 
                          FROM ingredient i 
                          INNER JOIN recipe r ON i.recipe_id = r.recipe_id 
                          WHERE i.ingredient_id = @ingredient_id";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@ingredient_id", ingredientId);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            DeletePreviewName.Text = reader["food_name"].ToString();
                            DeletePreviewRecipe.Text = reader["recipe_title"].ToString();
                            DeletePreviewQuantity.Text = reader["quantity"].ToString();
                        }
                    }
                }
            }
        }

        protected void ConfirmDeleteBtn_Click(object sender, EventArgs e)
        {
            CurrentTab.Value = "delete";

            try
            {
                if (DeleteIngredientDropDown.SelectedValue == "")
                {
                    ShowDeleteMessage("請選擇要刪除的食材", "error");
                    return;
                }

                int ingredientId = int.Parse(DeleteIngredientDropDown.SelectedValue);

                // 先取得要刪除的食材名稱，用於成功訊息
                string deletedFoodName = "";
                string getNameSql = "SELECT food_name FROM ingredient WHERE ingredient_id = @ingredient_id";

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(getNameSql, conn))
                    {
                        cmd.Parameters.AddWithValue("@ingredient_id", ingredientId);
                        var result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            deletedFoodName = result.ToString();
                        }
                    }
                }

                string sql = "DELETE FROM ingredient WHERE ingredient_id = @ingredient_id";

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@ingredient_id", ingredientId);

                        int result = cmd.ExecuteNonQuery();
                        if (result > 0)
                        {
                            LoadIngredientDropDowns();
                            DeletePreviewPanel.Visible = false;
                            DeleteIngredientDropDown.SelectedIndex = 0;
                            CurrentTab.Value = "delete";
                            ShowDeleteMessage($"食材「{deletedFoodName}」刪除成功！", "success");
                        }
                        else
                        {
                            ShowDeleteMessage("食材刪除失敗，請再試一次", "error");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowDeleteMessage($"刪除食材時發生錯誤：{ex.Message}", "error");
            }
        }
        #endregion

        #region 資料載入方法
        private void LoadRecipeDropDowns()
        {
            string sql = "SELECT recipe_id, title FROM recipe ORDER BY title";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        DataTable dt = new DataTable();
                        dt.Load(reader);

                        // 新增食材下拉選單
                        AddRecipeDropDown.DataSource = dt;
                        AddRecipeDropDown.DataTextField = "title";
                        AddRecipeDropDown.DataValueField = "recipe_id";
                        AddRecipeDropDown.DataBind();
                        AddRecipeDropDown.Items.Insert(0, new ListItem("請選擇食譜", ""));

                        // 修改食材下拉選單
                        EditRecipeDropDown.DataSource = dt;
                        EditRecipeDropDown.DataTextField = "title";
                        EditRecipeDropDown.DataValueField = "recipe_id";
                        EditRecipeDropDown.DataBind();
                        EditRecipeDropDown.Items.Insert(0, new ListItem("請選擇食譜", ""));
                    }
                }
            }
        }

        private void LoadIngredientDropDowns()
        {
            string sql = @"SELECT i.ingredient_id, 
                          CONCAT(i.food_name, ' (', r.title, ')') as display_text 
                          FROM ingredient i 
                          INNER JOIN recipe r ON i.recipe_id = r.recipe_id 
                          ORDER BY r.title, i.food_name";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        DataTable dt = new DataTable();
                        dt.Load(reader);

                        // 修改食材下拉選單
                        EditIngredientDropDown.DataSource = dt;
                        EditIngredientDropDown.DataTextField = "display_text";
                        EditIngredientDropDown.DataValueField = "ingredient_id";
                        EditIngredientDropDown.DataBind();
                        EditIngredientDropDown.Items.Insert(0, new ListItem("請選擇要修改的食材", ""));

                        // 刪除食材下拉選單
                        DeleteIngredientDropDown.DataSource = dt;
                        DeleteIngredientDropDown.DataTextField = "display_text";
                        DeleteIngredientDropDown.DataValueField = "ingredient_id";
                        DeleteIngredientDropDown.DataBind();
                        DeleteIngredientDropDown.Items.Insert(0, new ListItem("請選擇要刪除的食材", ""));
                    }
                }
            }
        }
        #endregion

        #region 訊息顯示方法
        // 新增功能的訊息顯示
        private void ShowAddMessage(string message, string type)
        {
            AddMessageLabel.Text = message;

            if (type == "success")
            {
                addMessageDiv.Attributes["class"] = "message success";
            }
            else if (type == "error")
            {
                addMessageDiv.Attributes["class"] = "message error";
            }

            AddMessagePanel.Visible = true;

            string script = $@"
                function showAddMessage() {{
                    if (window.addMessageTimer) clearTimeout(window.addMessageTimer);
                    if (window.addHideTimer) clearTimeout(window.addHideTimer);

                    var messagePanel = document.getElementById('{AddMessagePanel.ClientID}');
                    if (messagePanel) {{
                        messagePanel.style.display = 'block';
                        messagePanel.style.opacity = '1';
                        messagePanel.scrollIntoView({{ behavior: 'smooth', block: 'center' }});
                        
                        window.addHideTimer = setTimeout(function() {{
                            if (messagePanel) {{
                                messagePanel.style.opacity = '0';
                                setTimeout(function() {{
                                    messagePanel.style.display = 'none';
                                    messagePanel.style.opacity = '1';
                                }}, 300);
                            }}
                        }}, 3000);
                    }}
                    
                    // 確保顯示正確的頁籤
                    setTimeout(function() {{
                        if (typeof showTabByName === 'function') {{
                            showTabByName('add');
                        }}
                    }}, 50);
                }}
                
                if (document.readyState === 'complete') {{
                    showAddMessage();
                }} else {{
                    window.addMessageTimer = setTimeout(showAddMessage, 100);
                }}";

            ClientScript.RegisterStartupScript(this.GetType(), "ShowAddMessage_" + DateTime.Now.Ticks, script, true);
        }

        // 修改功能的訊息顯示
        private void ShowEditMessage(string message, string type)
        {
            EditMessageLabel.Text = message;

            if (type == "success")
            {
                editMessageDiv.Attributes["class"] = "message success";
            }
            else if (type == "error")
            {
                editMessageDiv.Attributes["class"] = "message error";
            }

            EditMessagePanel.Visible = true;

            string script = $@"
                function showEditMessage() {{
                    if (window.editMessageTimer) clearTimeout(window.editMessageTimer);
                    if (window.editHideTimer) clearTimeout(window.editHideTimer);

                    var messagePanel = document.getElementById('{EditMessagePanel.ClientID}');
                    if (messagePanel) {{
                        messagePanel.style.display = 'block';
                        messagePanel.style.opacity = '1';
                        messagePanel.scrollIntoView({{ behavior: 'smooth', block: 'center' }});
                        
                        window.editHideTimer = setTimeout(function() {{
                            if (messagePanel) {{
                                messagePanel.style.opacity = '0';
                                setTimeout(function() {{
                                    messagePanel.style.display = 'none';
                                    messagePanel.style.opacity = '1';
                                }}, 300);
                            }}
                        }}, 3000);
                    }}
                    
                    // 確保顯示正確的頁籤
                    setTimeout(function() {{
                        if (typeof showTabByName === 'function') {{
                            showTabByName('edit');
                        }}
                    }}, 50);
                }}
                
                if (document.readyState === 'complete') {{
                    showEditMessage();
                }} else {{
                    window.editMessageTimer = setTimeout(showEditMessage, 100);
                }}";

            ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowEditMessage_" + DateTime.Now.Ticks, script, true);
        }

        // 刪除功能的訊息顯示
        private void ShowDeleteMessage(string message, string type)
        {
            DeleteMessageLabel.Text = message;

            if (type == "success")
            {
                deleteMessageDiv.Attributes["class"] = "message success";
            }
            else if (type == "error")
            {
                deleteMessageDiv.Attributes["class"] = "message error";
            }

            DeleteMessagePanel.Visible = true;

            string script = $@"
                function showDeleteMessage() {{
                    if (window.deleteMessageTimer) clearTimeout(window.deleteMessageTimer);
                    if (window.deleteHideTimer) clearTimeout(window.deleteHideTimer);

                    var messagePanel = document.getElementById('{DeleteMessagePanel.ClientID}');
                    if (messagePanel) {{
                        messagePanel.style.display = 'block';
                        messagePanel.style.opacity = '1';
                        messagePanel.scrollIntoView({{ behavior: 'smooth', block: 'center' }});
                        
                        window.deleteHideTimer = setTimeout(function() {{
                            if (messagePanel) {{
                                messagePanel.style.opacity = '0';
                                setTimeout(function() {{
                                    messagePanel.style.display = 'none';
                                    messagePanel.style.opacity = '1';
                                }}, 300);
                            }}
                        }}, 3000);
                    }}
                    
                    // 確保顯示正確的頁籤
                    setTimeout(function() {{
                        if (typeof showTabByName === 'function') {{
                            showTabByName('delete');
                        }}
                    }}, 50);
                }}
                
                if (document.readyState === 'complete') {{
                    showDeleteMessage();
                }} else {{
                    window.deleteMessageTimer = setTimeout(showDeleteMessage, 100);
                }}";

            ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowDeleteMessage_" + DateTime.Now.Ticks, script, true);
        }
        #endregion
    }
}