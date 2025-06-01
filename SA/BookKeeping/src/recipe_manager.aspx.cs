using MySql.Data.MySqlClient;
using System;
using System.Configuration;
using System.Data;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace _BookKeeping
{
    public partial class recipe_manager : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;
        private string imageUploadPath = "~/src/recipes/";

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
                CurrentTab.Value = "add";

                // 確保圖片上傳目錄存在
                EnsureImageDirectory();
            }
        }

        protected void BackBtn_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/src/manager_main.aspx");
        }

        private void EnsureImageDirectory()
        {
            string physicalPath = Server.MapPath(imageUploadPath);
            if (!Directory.Exists(physicalPath))
            {
                Directory.CreateDirectory(physicalPath);
            }
        }

        #region 新增食譜功能
        protected void SaveAddBtn_Click(object sender, EventArgs e)
        {
            CurrentTab.Value = "add";
            try
            {
                string title = AddTitle.Text.Trim();
                string description = AddDescription.Text.Trim();
                string steps = AddSteps.Text.Trim();
                string imageFileName = "";

                if (string.IsNullOrEmpty(title))
                {
                    ShowAddMessage("請輸入食譜標題", "error");
                    return;
                }

                if (string.IsNullOrEmpty(description))
                {
                    ShowAddMessage("請輸入食譜描述", "error");
                    return;
                }

                if (string.IsNullOrEmpty(steps))
                {
                    ShowAddMessage("請輸入製作步驟", "error");
                    return;
                }

                // 檢查是否已存在相同標題的食譜
                if (IsRecipeExist(title))
                {
                    ShowAddMessage("已存在相同標題的食譜，請更換標題", "error");
                    return;
                }

                // 處理圖片上傳
                if (AddImageUpload.HasFile)
                {
                    imageFileName = UploadImage(AddImageUpload, "add");
                    if (string.IsNullOrEmpty(imageFileName))
                    {
                        return; // 上傳失敗，錯誤訊息已在 UploadImage 方法中顯示
                    }
                }
                else
                {
                    ShowAddMessage("請選擇食譜圖片", "error");
                    return;
                }

                string sql = @"INSERT INTO recipe (title, description, steps, image) 
                              VALUES (@title, @description, @steps, @image)";

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@title", title);
                        cmd.Parameters.AddWithValue("@description", description);
                        cmd.Parameters.AddWithValue("@steps", steps);
                        cmd.Parameters.AddWithValue("@image", imageFileName);

                        int result = cmd.ExecuteNonQuery();
                        if (result > 0)
                        {
                            ShowAddMessage("食譜新增成功！", "success");
                            ClearAddForm();
                            LoadRecipeDropDowns();
                        }
                        else
                        {
                            ShowAddMessage("食譜新增失敗，請再試一次", "error");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowAddMessage($"新增食譜時發生錯誤：{ex.Message}", "error");
            }
        }

        private bool IsRecipeExist(string title)
        {
            string sql = "SELECT COUNT(*) FROM recipe WHERE LOWER(TRIM(title)) = LOWER(TRIM(@title))";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@title", title);
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
        }

        private string UploadImage(FileUpload fileUpload, string operation)
        {
            try
            {
                if (!fileUpload.HasFile || fileUpload.PostedFile == null)
                {
                    if (operation == "add")
                        ShowAddMessage("請選擇要上傳的圖片檔案", "error");
                    else if (operation == "edit")
                        ShowEditMessage("請選擇要上傳的圖片檔案", "error");
                    return "";
                }

                // 檢查檔案大小是否為 0
                if (fileUpload.PostedFile.ContentLength == 0)
                {
                    if (operation == "add")
                        ShowAddMessage("選擇的圖片檔案是空的，請重新選擇", "error");
                    else if (operation == "edit")
                        ShowEditMessage("選擇的圖片檔案是空的，請重新選擇", "error");
                    return "";
                }

                string fileName = fileUpload.FileName;
                string fileExtension = Path.GetExtension(fileName).ToLower();

                // 檢查檔案類型
                if (fileExtension != ".jpg" && fileExtension != ".jpeg" &&
                    fileExtension != ".png" && fileExtension != ".gif")
                {
                    if (operation == "add")
                        ShowAddMessage("只支援 JPG、PNG、GIF 格式的圖片", "error");
                    else if (operation == "edit")
                        ShowEditMessage("只支援 JPG、PNG、GIF 格式的圖片", "error");
                    return "";
                }

                // 檢查檔案大小 (限制為 5MB)
                if (fileUpload.PostedFile.ContentLength > 5 * 1024 * 1024)
                {
                    if (operation == "add")
                        ShowAddMessage("圖片檔案大小不能超過 5MB", "error");
                    else if (operation == "edit")
                        ShowEditMessage("圖片檔案大小不能超過 5MB", "error");
                    return "";
                }

                // 確保上傳目錄存在
                EnsureImageDirectory();

                // 生成唯一檔名
                string uniqueFileName = Guid.NewGuid().ToString() + fileExtension;
                string physicalPath = Server.MapPath(imageUploadPath + uniqueFileName);

                // 儲存檔案
                fileUpload.SaveAs(physicalPath);

                // 驗證檔案是否真的被儲存
                if (!File.Exists(physicalPath))
                {
                    if (operation == "add")
                        ShowAddMessage("圖片儲存失敗，請重試", "error");
                    else if (operation == "edit")
                        ShowEditMessage("圖片儲存失敗，請重試", "error");
                    return "";
                }

                return uniqueFileName;
            }
            catch (Exception ex)
            {
                if (operation == "add")
                    ShowAddMessage($"圖片上傳失敗：{ex.Message}", "error");
                else if (operation == "edit")
                    ShowEditMessage($"圖片上傳失敗：{ex.Message}", "error");
                return "";
            }
        }

        private void ClearAddForm()
        {
            AddTitle.Text = "";
            AddDescription.Text = "";
            AddSteps.Text = "";
        }
        #endregion

        #region 修改食譜功能
        protected void EditRecipeDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            CurrentTab.Value = "edit";

            if (EditRecipeDropDown.SelectedValue != "")
            {
                LoadRecipeForEdit(int.Parse(EditRecipeDropDown.SelectedValue));
            }
            else
            {
                ClearEditForm();
            }
        }

        private void LoadRecipeForEdit(int recipeId)
        {
            string sql = "SELECT * FROM recipe WHERE recipe_id = @recipe_id";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@recipe_id", recipeId);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            EditTitle.Text = reader["title"].ToString();
                            EditDescription.Text = reader["description"].ToString();
                            EditSteps.Text = reader["steps"].ToString();

                            string imageFileName = reader["image"].ToString();
                            CurrentImageLabel.Text = imageFileName;

                            // 設定圖片路徑和顯示
                            if (!string.IsNullOrEmpty(imageFileName))
                            {
                                CurrentImage.ImageUrl = imageUploadPath + imageFileName;
                                CurrentImage.Visible = true;
                            }
                            else
                            {
                                CurrentImage.Visible = false;
                                CurrentImageLabel.Text = "無圖片";
                            }
                        }
                    }
                }
            }
        }

        private void ClearEditForm()
        {
            EditTitle.Text = "";
            EditDescription.Text = "";
            EditSteps.Text = "";
            CurrentImageLabel.Text = "";
            CurrentImage.Visible = false;
            CurrentImage.ImageUrl = ""; // 清空圖片路徑
        }

        protected void SaveEditBtn_Click(object sender, EventArgs e)
        {
            CurrentTab.Value = "edit";

            try
            {
                if (EditRecipeDropDown.SelectedValue == "")
                {
                    ShowEditMessage("請選擇要修改的食譜", "error");
                    return;
                }

                int recipeId = int.Parse(EditRecipeDropDown.SelectedValue);
                string title = EditTitle.Text.Trim();
                string description = EditDescription.Text.Trim();
                string steps = EditSteps.Text.Trim();
                string imageFileName = CurrentImageLabel.Text; // 預設使用原有圖片

                if (string.IsNullOrEmpty(title))
                {
                    ShowEditMessage("請輸入食譜標題", "error");
                    return;
                }

                if (string.IsNullOrEmpty(description))
                {
                    ShowEditMessage("請輸入食譜描述", "error");
                    return;
                }

                if (string.IsNullOrEmpty(steps))
                {
                    ShowEditMessage("請輸入製作步驟", "error");
                    return;
                }

                // 先取得原始資料進行比較
                var originalData = GetOriginalRecipeData(recipeId);
                if (originalData == null)
                {
                    ShowEditMessage("找不到要修改的食譜資料", "error");
                    return;
                }

                // 如果標題有變更，才需要檢查重複
                bool needCheckDuplicate = (originalData.Value.Item1 != title);

                if (needCheckDuplicate && IsRecipeExistForEdit(title, recipeId))
                {
                    ShowEditMessage("已存在相同標題的食譜", "error");
                    return;
                }

                // 處理圖片上傳
                string oldImageFileName = imageFileName; // 保存舊檔名用於刪除

                if (EditImageUpload.HasFile)
                {
                    // 檢查檔案是否真的有內容
                    if (EditImageUpload.PostedFile != null && EditImageUpload.PostedFile.ContentLength > 0)
                    {
                        string newImageFileName = UploadImage(EditImageUpload, "edit");
                        if (!string.IsNullOrEmpty(newImageFileName))
                        {
                            imageFileName = newImageFileName;
                            // 上傳成功後才刪除舊圖片
                            if (!string.IsNullOrEmpty(oldImageFileName) && oldImageFileName != newImageFileName)
                            {
                                DeleteOldImage(oldImageFileName);
                            }
                        }
                        else
                        {
                            return; // 上傳失敗，錯誤訊息已在 UploadImage 方法中顯示
                        }
                    }
                    else
                    {
                        ShowEditMessage("選擇的圖片檔案無效，請重新選擇", "error");
                        return;
                    }
                }

                // 更新資料庫
                string sql = @"UPDATE recipe SET title = @title, description = @description, 
                      steps = @steps, image = @image WHERE recipe_id = @recipe_id";

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@title", title);
                        cmd.Parameters.AddWithValue("@description", description);
                        cmd.Parameters.AddWithValue("@steps", steps);
                        cmd.Parameters.AddWithValue("@image", imageFileName);
                        cmd.Parameters.AddWithValue("@recipe_id", recipeId);

                        int result = cmd.ExecuteNonQuery();
                        if (result > 0)
                        {
                            // 修改成功後清空所有欄位
                            LoadRecipeDropDowns(); // 重新載入下拉選單
                            ClearEditForm(); // 清空表單欄位
                            EditRecipeDropDown.SelectedIndex = 0; // 回到"請選擇要修改的食譜"

                            ShowEditMessage("食譜修改成功！", "success");

                            // 額外的 JavaScript 確保所有欄位都被清空
                            string clearAllFieldsScript = @"
                                setTimeout(function() {
                                    // 清空所有文字欄位
                                    document.getElementById('" + EditTitle.ClientID + @"').value = '';
                                    document.getElementById('" + EditDescription.ClientID + @"').value = '';
                                    document.getElementById('" + EditSteps.ClientID + @"').value = '';
                                    document.getElementById('" + CurrentImageLabel.ClientID + @"').textContent = '';
            
                                    // 隱藏圖片
                                    var currentImage = document.getElementById('" + CurrentImage.ClientID + @"');
                                    if (currentImage) {
                                        currentImage.style.display = 'none';
                                        currentImage.src = '';
                                    }
            
                                    // 重設下拉選單
                                    document.getElementById('" + EditRecipeDropDown.ClientID + @"').selectedIndex = 0;
            
                                    // 清空檔案上傳控件的顯示文字
                                    var editFileLabel = document.getElementById('editFileLabel');
                                    if (editFileLabel) {
                                        editFileLabel.textContent = '點擊選擇新圖片檔案 (JPG, PNG, GIF)';
                                        editFileLabel.classList.remove('has-file');
                                    }
            
                                    // 清空檔案上傳控件
                                    var fileUpload = document.getElementById('" + EditImageUpload.ClientID + @"');
                                    if (fileUpload) {
                                        fileUpload.value = '';
                                    }
                                }, 100);
                            ";
                            ClientScript.RegisterStartupScript(this.GetType(), "ClearAllEditFields", clearAllFieldsScript, true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowEditMessage($"修改食譜時發生錯誤：{ex.Message}", "error");
            }
        }

        private bool IsRecipeExistForEdit(string title, int excludeRecipeId)
        {
            string sql = @"SELECT COUNT(*) FROM recipe 
                          WHERE LOWER(TRIM(title)) = LOWER(TRIM(@title)) 
                          AND recipe_id != @recipe_id";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@title", title);
                    cmd.Parameters.AddWithValue("@recipe_id", excludeRecipeId);

                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
        }

        private (string, string, string, string)? GetOriginalRecipeData(int recipeId)
        {
            string sql = "SELECT title, description, steps, image FROM recipe WHERE recipe_id = @recipe_id";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@recipe_id", recipeId);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return (
                                reader["title"].ToString().Trim(),
                                reader["description"].ToString().Trim(),
                                reader["steps"].ToString().Trim(),
                                reader["image"].ToString().Trim()
                            );
                        }
                    }
                }
            }
            return null;
        }

        private void DeleteOldImage(string fileName)
        {
            try
            {
                if (!string.IsNullOrEmpty(fileName))
                {
                    string physicalPath = Server.MapPath(imageUploadPath + fileName);
                    if (File.Exists(physicalPath))
                    {
                        File.Delete(physicalPath);
                    }
                }
            }
            catch
            {
                // 靜默處理刪除錯誤，不影響主要功能
            }
        }
        #endregion

        #region 刪除食譜功能
        protected void DeleteRecipeDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            CurrentTab.Value = "delete";

            if (DeleteRecipeDropDown.SelectedValue != "")
            {
                LoadRecipeForDelete(int.Parse(DeleteRecipeDropDown.SelectedValue));
                DeletePreviewPanel.Visible = true;
            }
            else
            {
                DeletePreviewPanel.Visible = false;
            }
        }

        private void LoadRecipeForDelete(int recipeId)
        {
            string sql = "SELECT * FROM recipe WHERE recipe_id = @recipe_id";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@recipe_id", recipeId);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            DeletePreviewTitle.Text = reader["title"].ToString();
                            DeletePreviewDescription.Text = reader["description"].ToString();
                            DeletePreviewSteps.Text = reader["steps"].ToString();

                            string imageFileName = reader["image"].ToString();
                            DeletePreviewImageName.Text = imageFileName;

                            // 設定圖片路徑
                            if (!string.IsNullOrEmpty(imageFileName))
                            {
                                DeletePreviewImage.ImageUrl = imageUploadPath + imageFileName;
                                DeletePreviewImage.Visible = true;
                            }
                            else
                            {
                                DeletePreviewImage.Visible = false;
                            }
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
                if (DeleteRecipeDropDown.SelectedValue == "")
                {
                    ShowDeleteMessage("請選擇要刪除的食譜", "error");
                    return;
                }

                int recipeId = int.Parse(DeleteRecipeDropDown.SelectedValue);

                // 先取得要刪除的食譜資訊
                string deletedTitle = "";
                string deletedImage = "";
                string getRecipeInfoSql = "SELECT title, image FROM recipe WHERE recipe_id = @recipe_id";

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(getRecipeInfoSql, conn))
                    {
                        cmd.Parameters.AddWithValue("@recipe_id", recipeId);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                deletedTitle = reader["title"].ToString();
                                deletedImage = reader["image"].ToString();
                            }
                        }
                    }
                }

                // 開始交易
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    MySqlTransaction transaction = conn.BeginTransaction();

                    try
                    {
                        // 先刪除相關的食材
                        string deleteIngredientsSql = "DELETE FROM ingredient WHERE recipe_id = @recipe_id";
                        using (MySqlCommand cmd = new MySqlCommand(deleteIngredientsSql, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@recipe_id", recipeId);
                            cmd.ExecuteNonQuery();
                        }

                        // 刪除相關的評論
                        string deleteCommentsSql = "DELETE FROM comment WHERE recipe_id = @recipe_id";
                        using (MySqlCommand cmd = new MySqlCommand(deleteCommentsSql, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@recipe_id", recipeId);
                            cmd.ExecuteNonQuery();
                        }

                        // 刪除相關的收藏
                        string deleteFavoritesSql = "DELETE FROM favorite WHERE recipe_id = @recipe_id";
                        using (MySqlCommand cmd = new MySqlCommand(deleteFavoritesSql, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@recipe_id", recipeId);
                            cmd.ExecuteNonQuery();
                        }

                        // 最後刪除食譜
                        string deleteRecipeSql = "DELETE FROM recipe WHERE recipe_id = @recipe_id";
                        using (MySqlCommand cmd = new MySqlCommand(deleteRecipeSql, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@recipe_id", recipeId);
                            int result = cmd.ExecuteNonQuery();

                            if (result > 0)
                            {
                                transaction.Commit();

                                // 刪除圖片檔案
                                DeleteOldImage(deletedImage);

                                LoadRecipeDropDowns();
                                DeletePreviewPanel.Visible = false;
                                DeleteRecipeDropDown.SelectedIndex = 0;
                                CurrentTab.Value = "delete";
                                ShowDeleteMessage($"食譜「{deletedTitle}」及其相關資料刪除成功！", "success");
                            }
                            else
                            {
                                transaction.Rollback();
                                ShowDeleteMessage("食譜刪除失敗，請再試一次", "error");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw ex;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowDeleteMessage($"刪除食譜時發生錯誤：{ex.Message}", "error");
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

                        // 修改食譜下拉選單
                        EditRecipeDropDown.DataSource = dt;
                        EditRecipeDropDown.DataTextField = "title";
                        EditRecipeDropDown.DataValueField = "recipe_id";
                        EditRecipeDropDown.DataBind();
                        EditRecipeDropDown.Items.Insert(0, new ListItem("請選擇要修改的食譜", ""));

                        // 刪除食譜下拉選單
                        DeleteRecipeDropDown.DataSource = dt;
                        DeleteRecipeDropDown.DataTextField = "title";
                        DeleteRecipeDropDown.DataValueField = "recipe_id";
                        DeleteRecipeDropDown.DataBind();
                        DeleteRecipeDropDown.Items.Insert(0, new ListItem("請選擇要刪除的食譜", ""));
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

            // 因為使用 PostBackTrigger，所以使用 ClientScript 而不是 ScriptManager
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

            ClientScript.RegisterStartupScript(this.GetType(), "ShowEditMessage_" + DateTime.Now.Ticks, script, true);
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