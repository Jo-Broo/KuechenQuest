using KuechenQuestAPI.Models;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Crypto.Agreement.JPake;
using System.Reflection.PortableExecutable;
using System.Text.Json;

namespace KuechenQuestAPI.Classes
{
    public class Database
    {
        private MySqlConnection connection;

        public Database(string connectionString)
        {
            this.connection = new MySqlConnection(connectionString);
        }

        #region User Functions
        public User? Login(string username, string password) 
        {
            User? user = null;
            try
            {
                string sql = string.Format(@"select * from user where NAME = '{0}' AND PASSWORD = '{1}';",username,password);
                this.connection.Open();
                MySqlDataReader reader = this.ExecuteQuery(sql);
                while (reader.Read()) 
                {
                    user = new User();
                    user.ID = reader.GetInt32("ID");
                    user.NAME = reader.GetString("NAME");
                    user.LEVEL = reader.GetInt32("LEVEL");
                    user.XP = reader.GetInt32("XP");
                    user.EMAIL = reader.GetString("EMAIL");
                    break;
                }
                this.connection.Close();
            }
            catch (Exception)
            {
                user = null;
            }
            finally
            {
                this.connection.Close();
            }

            return user;
        }
        public User? Register(string username, string email, string password)
        {
            User? user = null;
            try
            {
                string sql = string.Format(@"INSERT INTO 
                                                User(NAME,PASSWORD,EMAIL)
                                             VALUES 
                                                ('{0}','{1}','{2}');", username, password, email);
                this.connection.Open();
                this.ExecuteQuery(sql);
                this.connection.Close();
                user = this.GetUserByID(this.GetLastInsertedID());
            }
            catch (Exception)
            {
                user = null;
            }
            finally
            {
                this.connection.Close();
            }

            return user;
        }
        public User? GetUserByID(int id)
        {
            User? user = new User();
            try
            {
                string sql = string.Format(@"SELECT * FROM User WHERE ID = {0}",id);
                this.connection.Open();
                MySqlDataReader reader = this.ExecuteQuery(sql);
                while (reader.Read())
                {
                    user.ID = reader.GetInt32("ID");
                    user.NAME = reader.GetString("NAME");
                    user.LEVEL = reader.GetInt32("LEVEL");
                    user.XP = reader.GetInt32("XP");
                    user.EMAIL = reader.GetString("EMAIL");
                }
                this.connection.Close();
            }
            catch (Exception)
            {
                user = null;
            }
            finally
            {
                this.connection.Close();
            }

            return user;
        }
        public List<Achievment> GetAchievments(int userid) 
        {
            List<Achievment> achievments = new List<Achievment>();
            try
            {
                string sql = string.Format(@"select * FROM user_achievement ua LEFT JOIN Achievement a on ua.AchievementID = a.ID where ua.UserID = {0}", userid);
                this.connection.Open();
                MySqlDataReader reader = this.ExecuteQuery(sql);
                while (reader.Read())
                {
                    Achievment achievment = new Achievment();
                    achievment.ID = reader.GetInt32("ID");
                    achievment.NAME = reader.GetString("NAME");
                    achievment.DESCRIPTION = reader.GetString("DESCRIPTION");
                    achievment.IMAGE = reader.GetString("IMAGE");
                    achievment.TIME = reader.GetDateTime("TIME");
                    achievments.Add(achievment);
                }
            }
            catch (Exception)
            {
                achievments = new List<Achievment>();
            }

            return achievments;
        }
        #endregion

        #region Rezept
        public Recipe? GetRecipeByID(int id)
        {
            Recipe? result = null;
            try
            {
                #region Rezeptdaten
                string sql = string.Format(@"SELECT 
                                                r.ID,
                                                r.NAME,
                                                r.TIME,
                                                r.DIFFICULTY,
                                                r.INSTRUCTIONS,
                                                r.RATING,
                                                r.RATINGCOUNT,
                                                r.IMAGE
                                            FROM Recipe r
                                            JOIN Difficulty d ON r.DIFFICULTY = d.ID
                                            WHERE r.ID = {0};", id);
                this.connection.Open();
                MySqlDataReader reader = this.ExecuteQuery(sql);
                result = new Recipe();
                while (reader.Read())
                {
                    result.ID = reader.GetInt32("ID");
                    result.NAME = reader.GetString("NAME");
                    result.TIME = reader.GetInt32("TIME");
                    result.DIFFICULTY = reader.GetInt32("DIFFICULTY");
                    result.INSTRUCTIONS = reader.GetString("INSTRUCTIONS");
                    result.RATING = reader.GetInt32("RATING");
                    result.RATINGCOUNT = reader.GetInt32("RATINGCOUNT");
                    result.IMAGE = reader.GetString("IMAGE");
                }
                this.connection.Close();
                #endregion
                #region Utensilien
                sql = string.Format(@"SELECT 
                                        ru.ID,
                                        ru.RECIPEID,
                                        ru.UTENSILID,
                                        u.NAME,
                                        ru.QUANTITY,
                                        u.IMAGE
                                    FROM Recipe_Utensil ru
                                    JOIN Utensil u ON ru.UTENSILID = u.ID
                                    WHERE ru.RECIPEID = {0};", id);
                this.connection.Open();
                reader = this.ExecuteQuery(sql);
                while (reader.Read())
                {
                    Utensil utensil = new Utensil();
                    utensil.ID = reader.GetInt32("ID");
                    utensil.NAME = reader.GetString("NAME");
                    utensil.QUANTITY = reader.GetFloat("QUANTITY");

                    result.Utensils.Add(utensil);
                }
                this.connection.Close();
                #endregion
                #region Zutaten
                sql = string.Format(@"SELECT 
                                        ri.ID,
                                        ri.RECIPEID,
                                        ri.INGREDIENTID,
                                        i.NAME,
                                        ri.QUANTITY,
                                        i.CATEGORY,
                                        i.IMAGE
                                    FROM Recipe_Ingredient ri
                                    JOIN Ingredient i ON ri.INGREDIENTID = i.ID
                                    JOIN Category c ON i.CATEGORY = c.ID
                                    WHERE ri.RECIPEID = {0};", id);
                this.connection.Open();
                reader = this.ExecuteQuery(sql);
                while (reader.Read())
                {
                    Ingredient ingredient = new Ingredient();
                    ingredient.ID = reader.GetInt32("ID");
                    ingredient.NAME = reader.GetString("NAME");
                    ingredient.QUANTITY = reader.GetFloat("QUANTITY");
                    ingredient.CATEGORY = reader.GetInt32("CATEGORY");

                    result.Ingredients.Add(ingredient);
                }
                this.connection.Close();
                #endregion
            }
            catch (Exception)
            {
                result = null;
            }

            return result;
        }
        public List<Recipe> GetAllRecipes()
        {
            List<Recipe> recipes = new List<Recipe>();
            try
            {
                string sql = string.Format(@"SELECT 
                                                r.ID,
                                                r.NAME,
                                                r.TIME,
                                                r.DIFFICULTY,
                                                r.INSTRUCTIONS,
                                                r.RATING,
                                                r.RATINGCOUNT,
                                                r.IMAGE
                                            FROM Recipe r
                                            JOIN Difficulty d ON r.DIFFICULTY = d.ID");
                this.connection.Open();
                MySqlDataReader reader = this.ExecuteQuery(sql);
                while (reader.Read())
                {
                    Recipe recipe = new Recipe();
                    recipe.ID = reader.GetInt32("ID");
                    recipe.NAME = reader.GetString("NAME");
                    recipe.TIME = reader.GetInt32("TIME");
                    recipe.DIFFICULTY = reader.GetInt32("DIFFICULTY");
                    recipe.INSTRUCTIONS = reader.GetString("INSTRUCTIONS");
                    recipe.RATING = reader.GetInt32("RATING");
                    recipe.RATINGCOUNT = reader.GetInt32("RATINGCOUNT");
                    recipe.IMAGE = reader.GetString("IMAGE");
                    recipes.Add(recipe);
                }

                foreach (Recipe recipe in recipes)
                {
                    this.connection.Open();
                    string utensil_sql = string.Format(@"SELECT 
                                                            ru.ID,
                                                            ru.RECIPEID,
                                                            ru.UTENSILID,
                                                            u.NAME,
                                                            ru.QUANTITY,
                                                            u.IMAGE
                                                        FROM Recipe_Utensil ru
                                                        JOIN Utensil u ON ru.UTENSILID = u.ID
                                                        WHERE ru.RECIPEID = {0};", recipe.ID);
                    reader = this.ExecuteQuery(utensil_sql);
                    while (reader.Read())
                    {
                        Utensil utensil = new Utensil();
                        utensil.ID = reader.GetInt32("ID");
                        utensil.NAME = reader.GetString("NAME");
                        utensil.QUANTITY = reader.GetInt32("QUANTITY");

                        recipe.Utensils.Add(utensil);
                    }
                    this.connection.Close();

                    this.connection.Open();
                    string ingredient_sql = string.Format(@"SELECT 
                                                                ri.ID,
                                                                ri.RECIPEID,
                                                                ri.INGREDIENTID,
                                                                i.NAME,
                                                                ri.QUANTITY,
                                                                c.ID as 'CID',
                                                                i.IMAGE
                                                            FROM Recipe_Ingredient ri
                                                            JOIN Ingredient i ON ri.INGREDIENTID = i.ID
                                                            JOIN Category c ON i.CATEGORY = c.ID
                                                            WHERE ri.RECIPEID = {0};", recipe.ID);
                    reader = this.ExecuteQuery(ingredient_sql);
                    while (reader.Read())
                    {
                        Ingredient ingredient = new Ingredient();
                        ingredient.ID = reader.GetInt32("ID");
                        ingredient.NAME = reader.GetString("NAME");
                        ingredient.QUANTITY = reader.GetFloat("QUANTITY");
                        ingredient.CATEGORY = reader.GetInt32("CID");

                        recipe.Ingredients.Add(ingredient);
                    }

                    this.connection.Close();
                }
            }
            catch (Exception)
            {
                recipes = new List<Recipe>();
            }

            return recipes;
        }
        public Recipe? CreateRecipe(Recipe recipe)
        {
            Recipe? result = null;
            try
            {
                // Rezept anlegen
                string sql = string.Format(@"INSERT INTO
                                             Recipe(NAME, TIME, DIFFICULTY, INSTRUCTIONS, IMAGE)
                                             VALUES ('{0}',
                                                      {1},
                                                      {2},
                                                      '{3}',
                                                      '{4}');", recipe.NAME, recipe.TIME, recipe.DIFFICULTY, recipe.INSTRUCTIONS, recipe.IMAGE);
                this.connection.Open();
                this.ExecuteQuery(sql);
                this.connection.Close();

                recipe.ID = this.GetLastInsertedID();

                // Zuaten speichern
                foreach(Utensil utensil in recipe.Utensils)
                {
                    sql = string.Format(@"INSERT INTO 
                                            Recipe_Utensil (RECIPEID, UTENSILID, QUANTITY)
                                            VALUES ({0}, {1}, {2})", recipe.ID, utensil.ID, utensil.QUANTITY);
                    this.connection.Open();
                    this.ExecuteQuery(sql);
                    this.connection.Close();
                }

                // Utensilien speichern
                foreach (Ingredient ingredient in recipe.Ingredients)
                {
                    sql = string.Format(@"INSERT INTO 
                                            Recipe_Ingredient(RECIPEID, INGREDIENTID, QUANTITY)
                                            VALUES ({0}, {1}, {2})", recipe.ID, ingredient.ID, ingredient.QUANTITY);
                    this.connection.Open();
                    this.ExecuteQuery(sql);
                    this.connection.Close();
                }

                result = this.GetRecipeByID(recipe.ID);
            }
            catch (Exception)
            {
                result = null;
            }

            return result;
        }
        public bool DeleteRecipeByID(int id)
        {
            bool result = false;
            try
            {
                string sql = string.Format(@"DELETE FROM Recipe WHERE ID = {0}", id);
                this.connection.Open();
                this.ExecuteQuery(sql);
                this.connection.Close();

                // Utensilien und Zutaten werden mit On Cascade automatisch gelöscht

                result = true;
            }
            catch (Exception)
            {
                result = false;
            }

            return result;
        }
        public Recipe? UpdateRecipe(Recipe recipe)
        {
            Recipe? result = null;
            try
            {
                // Das Rezept wird zuerst gelöscht
                this.DeleteRecipeByID(recipe.ID);
                // dann wird es neu angelegt
                this.CreateRecipe(recipe);
                // und dann wird es zurückgegeben
                result = this.GetRecipeByID(this.GetLastInsertedID());
            }
            catch (Exception)
            {
                result = null;
            }

            return result;
        }
        #endregion

        #region Utensil
        public Utensil? GetUtensilByID(int id) 
        {
            Utensil? result = null;
            try
            {
                string sql = string.Format(@"SELECT 
                                             ID,
                                             NAME,
                                             IMAGE
                                             FROM Utensil
                                             WHERE ID = {0}",id);
                this.connection.Open();
                MySqlDataReader reader = this.ExecuteQuery(sql);
                result = new Utensil();
                while (reader.Read())
                {
                    result.ID = reader.GetInt32("ID");
                    result.NAME = reader.GetString("NAME");
                    result.IMAGE = reader.GetString("image");
                    break;
                }
            }
            catch (Exception)
            {
                result = null;
            }

            return result;
        }
        public List<Utensil> GetAllUtensils() 
        {
            List<Utensil> result = new List<Utensil>();
            try
            {
                string sql = string.Format(@"SELECT 
                                             ID,
                                             NAME,
                                             IMAGE
                                             FROM Utensil");
                this.connection.Open();
                MySqlDataReader reader = this.ExecuteQuery(sql);
                while (reader.Read())
                {
                    Utensil utensil = new Utensil();
                    utensil.ID = reader.GetInt32("ID");
                    utensil.NAME = reader.GetString("NAME");
                    utensil.IMAGE = reader.GetString("image");

                    result.Add(utensil);
                }
                this.connection.Close();
            }
            catch (Exception)
            {
                result = new List<Utensil>();
            }

            return result;
        }
        public Utensil? CreateUtensil(Utensil utensil) 
        {
            Utensil? result = new Utensil();
            try
            {
                string sql = string.Format(@"INSERT INTO 
                                             Utensil(NAME,IMAGE)
                                             VALUES
                                             ('{0}',{1})", utensil.NAME, utensil.IMAGE);
                this.connection.Open();
                this.ExecuteQuery(sql);
                this.connection.Close();

                result = this.GetUtensilByID(this.GetLastInsertedID());
            }
            catch (Exception)
            {
                result = null;
            }

            return result;
        }
        public Utensil? UpdateUtensil(Utensil utensil) { 
            Utensil? result = null;
            try
            {
                string sql = string.Format(@"UPDATE Utensil
                                             SET
                                                NAME = '{0}',
                                                IMAGE = '{1}'
                                             WHERE
                                                ID = {2}",utensil.NAME,utensil.IMAGE,utensil.ID);
                this.connection.Open();
                this.ExecuteQuery(sql); 
                this.connection.Close();

                result = this.GetUtensilByID(utensil.ID);
            }
            catch (Exception)
            {
                result = null;
            }
            
            return result;
        }
        public bool DeleteUtensilByID(int id)
        {
            bool result = false;
            try
            {
                string sql = string.Format(@"DELETE FROM Utensil WHERE ID = {0};",id);
                this.connection.Open();
                this.ExecuteQuery(sql);
                this.connection.Close();

                result = true;
            }
            catch (Exception)
            {
                result = false;
            }

            return result;
        }
        #endregion

        #region Zutaten
        public Ingredient? GetIngredientByID(int id)
        {
            Ingredient? ingredient = new Ingredient();
            try
            {
                string sql = string.Format(@"SELECT ID,NAME,CATEGORY,IMAGE FROM Ingredient WHERE ID = {0}",id);
                this.connection.Open();
                MySqlDataReader reader = this.ExecuteQuery(sql);
                while (reader.Read())
                {
                    ingredient.ID = reader.GetInt32("ID");
                    ingredient.NAME = reader.GetString("NAME");
                    ingredient.CATEGORY = reader.GetInt32("CATEGORY");
                    ingredient.IMAGE = reader.GetString("IMAGE");
                    break;
                }
            }
            catch (Exception)
            {
                ingredient = null;
            }
            finally
            {
                this.connection.Close();
            }

            return ingredient;
        }
        public List<Ingredient> GetAllIngredients()
        {
            List<Ingredient> ingredients = new List<Ingredient>();
            try
            {
                string sql = string.Format(@"SELECT ID,NAME,CATEGORY,IMAGE FROM Ingredient;");
                this.connection.Open();
                MySqlDataReader reader = this.ExecuteQuery(sql);
                while (reader.Read())
                {
                    Ingredient ingredient = new Ingredient();
                    ingredient.ID = reader.GetInt32("ID");
                    ingredient.NAME = reader.GetString("NAME");
                    ingredient.CATEGORY = reader.GetInt32("CATEGORY");
                    ingredient.IMAGE = reader.GetString("IMAGE");

                    ingredients.Add(ingredient);
                }
                this.connection.Close();
            }
            catch (Exception)
            {
                ingredients = new List<Ingredient>();
            }

            return ingredients;
        }
        public Ingredient? CreateIngredient(Ingredient ingredient)
        {
            Ingredient? result = null;
            try
            {
                string sql = string.Format(@"INSERT INTO 
                                             Ingredient(NAME,IMAGE,CATEGORY)
                                             VALUES ('{0}','{1}',{2})", result.NAME, result.IMAGE, result.CATEGORY);
                this.connection.Open();
                this.ExecuteQuery(sql);
                this.connection.Close();

                result = this.GetIngredientByID(this.GetLastInsertedID());
            }
            catch (Exception)
            {
                result = null;
            }

            return result;
        }
        public Ingredient? UpdateIngredient(Ingredient ingredient)
        {
            Ingredient? result = null;
            try
            {
                string sql = string.Format(@"UPDATE Ingredient 
                                             SET NAME = '{0}',
                                             CATEGORY = {1},
                                             IMAGE = '{2}'
                                             WHERE ID = {3}", ingredient.NAME, ingredient.CATEGORY, ingredient.IMAGE, ingredient.ID);
                this.connection.Open();
                this.ExecuteQuery(sql);
                this.connection.Close();
            }
            catch (Exception)
            {
                result = null;
            }

            return result;
        }
        public bool DeleteIngredientByID(int id) { return false; }
        #endregion     
        
        public List<Difficulty> GetAllDifficultys() 
        {
            List<Difficulty> result = new List<Difficulty>();
            try
            {
                string sql = string.Format(@"SELECT ID, NAME FROM Difficulty");
                this.connection.Open();
                MySqlDataReader reader = this.ExecuteQuery(sql);
                while (reader.Read())
                {
                    Difficulty difficulty = new Difficulty();
                    difficulty.ID = reader.GetInt32("ID");
                    difficulty.NAME = reader.GetString("NAME");

                    result.Add(difficulty);
                }
                this.connection.Close();
            }
            catch (Exception)
            {
                result = new List<Difficulty>();
            }

            return result;
        }
        public List<Category> GetAllCategorys() 
        {
            List<Category> result = new List<Category>();
            try
            {
                string sql = string.Format(@"SELECT * FROM Category;");
                this.connection.Open();
                MySqlDataReader reader = this.ExecuteQuery(sql);
                while (reader.Read())
                {
                    Category category = new Category();
                    category.ID = reader.GetInt32("ID");
                    category.NAME = reader.GetString("NAME");

                    result.Add(category);
                }
                this.connection.Close();
            }
            catch (Exception)
            {
                result = new List<Category>();
            }

            return result;
        }

        private MySqlDataReader ExecuteQuery(string sql)
        {
            using (MySqlCommand command = new MySqlCommand(sql, this.connection))
            {
                return command.ExecuteReader();
            }
        }
        private int GetLastInsertedID()
        {
            string sql = string.Format(@"SELECT LAST_INSERT_ID() as 'ID';");
            this.connection.Open();
            MySqlDataReader reader = this.ExecuteQuery(sql);
            int i = 0;
            while (reader.Read())
            {
                i = reader.GetInt32("ID");
                break;
            }
            this.connection.Close();

            return i;
        }
    }
}
