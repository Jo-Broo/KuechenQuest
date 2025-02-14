using KuechenQuestAPI.Models;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Crypto.Agreement.JPake;
using System.Text.Json;

namespace KuechenQuestAPI.Classes
{
    /// <summary>
    /// Represents the Connection to the Database
    /// </summary>
    public class Database
    {
        private MySqlConnection _connection;

        public Database(string connectionString)
        {
            this._connection = new MySqlConnection(connectionString);
        }
        #region User Functions
        /// <summary>
        /// Returns an empty DataPackage and no Error when the Credentials are correct
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public User? Login(string username, string password) 
        {
            User? user = new User();
            try
            {
                string sql = string.Format(@"select * from user where NAME = '{0}' AND PASSWORD = '{1}';",username,password);
                this._connection.Open();
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
                this._connection.Close();
            }
            catch (Exception ex)
            {
                user = null;
            }
            finally
            {
                // Datenbankverbindung schließen
                this._connection.Close();
            }

            return user;
        }
        /// <summary>
        /// Returns a DataPackage that contains every Achievment of a User
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public DataPackage GetAchievments(string username) { throw new NotImplementedException(); }

        public User? Register(string username, string email, string password)
        {
            User? user = null;
            try
            {
                string sql = string.Format(@"INSERT INTO 
                                                User(NAME,PASSWORD,EMAIL)
                                             VALUES 
                                                ('{0}','{1}','{2}');", username, password, email);
                this._connection.Open();
                this.ExecuteQuery(sql);
                this._connection.Close();
                user = this.GetUser(this.GetLastInsertedID());
            }
            catch (Exception)
            {
                user = null;
            }
            finally
            {
                this._connection.Close();
            }

            return user;
        }

        public User? GetUser(int id)
        {
            User? user = new User();
            try
            {
                string sql = string.Format(@"SELECT * FROM User WHERE ID = {0}",id);
                this._connection.Open();
                // Benutzer abfragen
                MySqlDataReader reader = this.ExecuteQuery(sql);
                while (reader.Read())
                {
                    user.ID = reader.GetInt32("ID");
                    user.NAME = reader.GetString("NAME");
                    user.LEVEL = reader.GetInt32("LEVEL");
                    user.XP = reader.GetInt32("XP");
                    user.EMAIL = reader.GetString("EMAIL");
                }
                this._connection.Close();
            }
            catch (Exception)
            {
                user = null;
            }
            finally
            {
                // Datenbankverbindung schließen
                this._connection.Close();
            }

            return user;
        }
        #endregion

        #region Rezept
        /// <summary>
        /// Returns a DataPackage with the corresponding Recipe if one is found
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DataPackage GetRecipe(int id) 
        {
            // Datenpaket erstellen
            DataPackage package = new DataPackage();
            try
            {
                // Datenbankverbindung öffnen

                #region Basisdaten
                // Basis Daten abfragen
                this._connection.Open();
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
                MySqlDataReader reader = this.ExecuteQuery(sql);
                
                // Ergebniss verarbeiten
                Recipe recipe = new Recipe();
                while (reader.Read())
                {
                    recipe.ID = reader.GetInt32("ID");
                    recipe.NAME = reader.GetString("NAME");
                    recipe.TIME = reader.GetInt32("TIME");
                    recipe.DIFFICULTY = reader.GetInt32("DIFFICULTY");
                    recipe.INSTRUCTIONS = reader.GetString("INSTRUCTIONS");
                    recipe.RATING = reader.GetInt32("RATING");
                    recipe.RATINGCOUNT = reader.GetInt32("RATINGCOUNT");
                    recipe.IMAGE = reader.GetString("IMAGE");
                }
                this._connection.Close();
                #endregion
                #region Utensilien
                this._connection.Open();
                string utensil_sql = string.Format(@"SELECT 
                                        ru.ID,
                                        ru.RECIPEID,
                                        ru.UTENSILID,
                                        u.NAME,
                                        ru.QUANTITY,
                                        u.IMAGE
                                    FROM Recipe_Utensil ru
                                    JOIN Utensil u ON ru.UTENSILID = u.ID
                                    WHERE ru.RECIPEID = {0};",id);
                reader = this.ExecuteQuery(utensil_sql);

                while (reader.Read()) 
                {
                    Utensil utensil = new Utensil();
                    utensil.ID = reader.GetInt32("ID");
                    utensil.NAME = reader.GetString("NAME");
                    utensil.QUANTITY = reader.GetFloat("QUANTITY");

                    recipe.Utensils.Add(utensil);
                }
                this._connection.Close();
                #endregion
                #region Zutaten
                this._connection.Open();
                string ingredient_sql = string.Format(@"SELECT 
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
                                    WHERE ri.RECIPEID = {0};",id);
                reader = this.ExecuteQuery(ingredient_sql);

                while (reader.Read())
                {
                    Ingredient ingredient = new Ingredient();
                    ingredient.ID = reader.GetInt32("ID");
                    ingredient.NAME = reader.GetString("NAME");
                    ingredient.QUANTITY = reader.GetFloat("QUANTITY");
                    ingredient.CATEGORY = reader.GetInt32("CATEGORY");

                    recipe.Ingredients.Add(ingredient);
                }
                this._connection.Close();
                #endregion
                package.Payload = recipe;
            }
            catch (Exception ex)
            {
                // Fehler bearbeitung 
                package.Payload = null;
                package.Error = ex.Message;
            }
            finally 
            { 
                // Datenbankverbindung schließen
                this._connection.Close(); 
            }

            // Datenpaket zurückgebens
            if (package.Payload == null) { package.Error = true; }
            return package;
        }
        /// <summary>
        /// Returns a DataPackage with all Recipes that are saved in the Database
        /// </summary>
        /// <returns></returns>
        public DataPackage GetAllRecipes() 
        {
            // Datenpaket erstellen
            DataPackage package = new DataPackage();
            try
            {
                // Datenbankverbindung öffnen

                #region Basisdaten
                // Basis Daten abfragen
                this._connection.Open();
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
                MySqlDataReader reader = this.ExecuteQuery(sql);

                // Ergebniss verarbeiten
                List<Recipe> recipes = new List<Recipe>();
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
                this._connection.Close();
                #endregion
                #region Utensilien/Zutaten
                foreach (Recipe recipe in recipes) 
                {
                    this._connection.Open();
                    string utensil_sql = string.Format(@"SELECT 
                                                            ru.ID,
                                                            ru.RECIPEID,
                                                            ru.UTENSILID,
                                                            u.NAME,
                                                            ru.QUANTITY,
                                                            u.IMAGE
                                                        FROM Recipe_Utensil ru
                                                        JOIN Utensil u ON ru.UTENSILID = u.ID
                                                        WHERE ru.RECIPEID = {0};",recipe.ID);
                    reader = this.ExecuteQuery(utensil_sql);
                    while (reader.Read())
                    {
                        Utensil utensil = new Utensil();
                        utensil.ID = reader.GetInt32("ID");
                        utensil.NAME = reader.GetString("NAME");
                        utensil.QUANTITY = reader.GetInt32("QUANTITY");

                        recipe.Utensils.Add(utensil);
                    }
                    this._connection.Close();
                    
                    this._connection.Open();
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
                                                            WHERE ri.RECIPEID = {0};",recipe.ID);
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

                    this._connection.Close();
                }
                #endregion
                package.Payload = recipes;
            }
            catch (Exception ex)
            {
                // Fehler bearbeitung 
                package.Payload = null;
                package.Error = ex.Message;
            }
            finally
            {
                // Datenbankverbindung schließen
                this._connection.Close();
            }

            // Datenpaket zurückgebens
            if (package.Payload == null) { package.Error = true; }
            return package;
        }
        /// <summary>
        /// Creates a Recipe in the Database with the given Recipe Objekt
        /// </summary>
        /// <param name="recipe"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public DataPackage CreateRecipe(Recipe recipe) 
        {
            DataPackage package = new DataPackage();

            try
            {
                if(recipe == new Recipe())
                {
                    throw new ArgumentException("Beim Parsen des Rezeptes ist ein Fehler aufgetreten.");
                }
                
                string sql = string.Format(@"INSERT INTO 
                                             Recipe(NAME, TIME, DIFFICULTY, INSTRUCTIONS, IMAGE)
                                             VALUES ('{0}',
                                                      {1},
                                                      {2},
                                                      '{3}',
                                                      '{4}');",recipe.NAME,recipe.TIME,recipe.DIFFICULTY,recipe.INSTRUCTIONS,recipe.IMAGE);
                this._connection.Open();
                MySqlDataReader reader = this.ExecuteQuery(sql);
                this._connection.Close();

                sql = string.Format(@"SELECT LAST_INSERT_ID() as 'ID';");
                this._connection.Open();
                reader = this.ExecuteQuery(sql);
                while (reader.Read()) 
                {
                    recipe.ID = reader.GetInt32("ID");
                }
                this._connection.Close();


                foreach (Utensil utensil in recipe.Utensils)
                {
                    string utensil_sql = string.Format(@"INSERT INTO 
                                                         Recipe_Utensil (RECIPEID, UTENSILID, QUANTITY)
                                                         VALUES ({0}, {1}, {2})", recipe.ID,utensil.ID,utensil.QUANTITY);

                    this._connection.Open();
                    this.ExecuteQuery(utensil_sql);
                    this._connection.Close();
                }

                foreach(Ingredient ingredient in recipe.Ingredients)
                {
                    string ingredient_sql = string.Format(@"INSERT INTO 
                                                            Recipe_Ingredient(RECIPEID, INGREDIENTID, QUANTITY)
                                                            VALUES ({0}, {1}, {2})",recipe.ID,ingredient.ID,ingredient.QUANTITY);

                    this._connection.Open();
                    this.ExecuteQuery(ingredient_sql);
                    this._connection.Close();
                }

                package.Payload = "";
            }
            catch (Exception ex)
            {
                // Fehler bearbeitung 
                package.Payload = null;
                package.Error = ex.Message;
            }
            finally
            {
                // Datenbankverbindung schließen
                this._connection.Close();
            }

            // Datenpaket zurückgebens
            if (package.Payload == null) { package.Error = true; }
            return package;
        }
        /// <summary>
        /// Deletes a Recipe with the given ID in the Database
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DataPackage DeleteRecipe(int id) 
        { 
            DataPackage package = new DataPackage();
            try
            {
                this._connection.Open();
                string sql = string.Format(@"DELETE FROM Recipe WHERE ID = {0}",id);
                this.ExecuteQuery(sql);
            }
            catch (Exception ex)
            {
                // Fehler bearbeitung 
                package.Payload = null;
                package.Error = ex.Message;
            }
            finally
            {
                // Datenbankverbindung schließen
                this._connection.Close();
            }

            // Datenpaket zurückgebens
            if (package.Payload == null) { package.Error = true; }
            return package;
        }
        /// <summary>
        /// Updates Information of a Recipe 
        /// </summary>
        /// <param name="recipe"></param>
        /// <returns></returns>
        public DataPackage UpdateRecipe(Recipe recipe) 
        { 
            DataPackage package = new DataPackage();
            try
            {
                string sql = string.Format(@"UPDATE Recipe 
                                             SET 
                                             NAME = '{0}',
                                             TIME = {1},
                                             DIFFICULTY = {2},
                                             INSTRUCTIONS = '{3}',
                                             RATING = {4},
                                             RATINGCOUNT = {5},
                                             IMAGE = {6}
                                             WHERE
                                             ID = {7};", recipe.NAME,recipe.TIME,recipe.DIFFICULTY,recipe.INSTRUCTIONS,recipe.RATING,recipe.RATINGCOUNT,recipe.IMAGE,recipe.ID);
                this._connection.Open();
                this.ExecuteQuery(sql);
                this._connection.Close();

                package.Payload = "";
            }
            catch (Exception ex)
            {
                // Fehler bearbeitung 
                package.Payload = null;
                package.Error = ex.Message;
            }
            finally
            {
                // Datenbankverbindung schließen
                this._connection.Close();
            }

            // Datenpaket zurückgebens
            if (package.Payload == null) { package.Error = true; }
            return package;
        }
        #endregion

        #region Utensil
        /// <summary>
        /// Returns a DataPackage with the corresponding Utensil if one is found
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DataPackage GetUtensil(int id) 
        {
            DataPackage package = new DataPackage();
            try
            {
                string sql = string.Format(@"SELECT 
                                             ID,
                                             NAME,
                                             IMAGE
                                             FROM Utensil
                                             WHERE ID = {0}",id);
                this._connection.Open();
                MySqlDataReader reader = this.ExecuteQuery(sql);
                while (reader.Read()) 
                {
                    Utensil utensil = new Utensil();
                    utensil.ID = reader.GetInt32("ID");
                    utensil.NAME = reader.GetString("NAME");
                    utensil.IMAGE = reader.GetString("image");
                    package.Payload = utensil;
                    break;
                }
                this._connection.Close();
            }
            catch (Exception ex)
            {
                // Fehler bearbeitung 
                package.Payload = null;
                package.Error = ex.Message;
            }
            finally
            {
                // Datenbankverbindung schließen
                this._connection.Close();
            }

            // Datenpaket zurückgebens
            if (package.Payload == null) { package.Error = true; }
            return package;
        }
        /// <summary>
        /// Returns a DataPackage with all Utensils that are saved in the Database
        /// </summary>
        /// <returns></returns>
        public DataPackage GetAllUtensils() 
        {
            DataPackage package = new DataPackage();
            try
            {
                List<Utensil> utensils = new List<Utensil>();
                string sql = string.Format(@"SELECT 
                                             ID,
                                             NAME,
                                             IMAGE
                                             FROM Utensil");
                this._connection.Open();
                
                MySqlDataReader reader = this.ExecuteQuery(sql);
                while (reader.Read())
                {
                    Utensil utensil = new Utensil();
                    utensil.ID = reader.GetInt32("ID");
                    utensil.NAME = reader.GetString("NAME");
                    utensil.IMAGE = reader.GetString("image");

                    utensils.Add(utensil);
                }
                this._connection.Close();
                package.Payload = utensils;
            }
            catch (Exception ex)
            {
                // Fehler bearbeitung 
                package.Payload = null;
                package.Error = ex.Message;
            }
            finally
            {
                // Datenbankverbindung schließen
                this._connection.Close();
            }

            // Datenpaket zurückgebens
            if (package.Payload == null) { package.Error = true; }
            return package;
        }
        /// <summary>
        /// Creates a Utensil in the Database with the given Recipe Objekt
        /// </summary>
        /// <param name="recipe"></param>
        /// <returns></returns>
        public DataPackage CreateUtensil(Utensil utensil) 
        {
            DataPackage package = new DataPackage();
            try
            {
                string sql = string.Format(@"INSERT INTO 
                                             Utensil(NAME,IMAGE)
                                             VALUES
                                             ('{0}',{1})",utensil.NAME,utensil.IMAGE);
                this._connection.Open();
                this.ExecuteQuery(sql);
                this._connection.Close();

                package.Payload = "";
            }
            catch (Exception ex)
            {
                // Fehler bearbeitung 
                package.Payload = null;
                package.Error = ex.Message;
            }
            finally
            {
                // Datenbankverbindung schließen
                this._connection.Close();
            }

            // Datenpaket zurückgebens
            if (package.Payload == null) { package.Error = true; }
            return package;
        }
        /// <summary>
        /// Deletes a Utensil with the given ID in the Database
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DataPackage DeleteUntensil(int id) 
        {
            DataPackage package = new DataPackage();
            try
            {
                string sql = string.Format(@"DELETE FROM Utensil WHERE ID = {0};",id);
                this._connection.Open();
                this.ExecuteQuery(sql);
                this._connection.Close();

                package.Payload = "";
            }
            catch (Exception ex)
            {
                // Fehler bearbeitung 
                package.Payload = null;
                package.Error = ex.Message;
            }
            finally
            {
                // Datenbankverbindung schließen
                this._connection.Close();
            }

            // Datenpaket zurückgebens
            if (package.Payload == null) { package.Error = true; }
            return package;
        }
        /// <summary>
        /// Updates Information of a Utensil 
        /// </summary>
        /// <param name="recipe"></param>
        /// <returns></returns>
        public DataPackage UpdateUtensil(Utensil utensil) 
        {
            DataPackage package = new DataPackage();
            try
            {
                string sql = string.Format(@"UPDATE Utensil
                                             SET NAME = '{0}',
                                             IMAGE = '{1}'
                                             WHERE ID = {2};",utensil.NAME,utensil.IMAGE,utensil.ID);
                this._connection.Open();
                this.ExecuteQuery(sql);
                this._connection.Close();

                package.Payload = "";
            }
            catch (Exception ex)
            {
                // Fehler bearbeitung 
                package.Payload = null;
                package.Error = ex.Message;
            }
            finally
            {
                // Datenbankverbindung schließen
                this._connection.Close();
            }

            // Datenpaket zurückgebens
            if (package.Payload == null) { package.Error = true; }
            return package;
        }
        #endregion

        #region Zutaten
        /// <summary>
        /// Returns a DataPackage with the corresponding Ingredient if one is found
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DataPackage GetIngredient(int id) 
        {
            DataPackage package = new DataPackage();
            try
            {
                string sql = string.Format(@"SELECT
                                             ID,
                                             NAME,
                                             IMAGE,
                                             CATEGORY
                                             FROM Ingredient
                                             WHERE ID = {0};",id);
                this._connection.Open();
                MySqlDataReader reader = this.ExecuteQuery(sql);
                while (reader.Read()) 
                {
                    Ingredient ingredient = new Ingredient();
                    ingredient.ID = reader.GetInt32("ID");
                    ingredient.NAME = reader.GetString("NAME");
                    ingredient.IMAGE = reader.GetString("IMAGE");
                    ingredient.CATEGORY = reader.GetInt32("CATEGORY");

                    package.Payload = ingredient;
                    break;
                }
                this._connection.Close();
            }
            catch (Exception ex)
            {
                // Fehler bearbeitung 
                package.Payload = null;
                package.Error = ex.Message;
            }
            finally
            {
                // Datenbankverbindung schließen
                this._connection.Close();
            }

            // Datenpaket zurückgebens
            if (package.Payload == null) { package.Error = true; }
            return package;
        }
        /// <summary>
        /// Returns a DataPackage with all Ingredients that are saved in the Database
        /// </summary>
        /// <returns></returns>
        public DataPackage GetAllIngredients() 
        {
            DataPackage package = new DataPackage();
            try
            {
                List<Ingredient> Ingredients = new List<Ingredient>();

                string sql = string.Format(@"SELECT
                                             ID,
                                             NAME,
                                             IMAGE,
                                             CATEGORY
                                             FROM Ingredient");
                this._connection.Open();
                MySqlDataReader reader = this.ExecuteQuery(sql);
                while (reader.Read())
                {
                    Ingredient ingredient = new Ingredient();
                    ingredient.ID = reader.GetInt32("ID");
                    ingredient.NAME = reader.GetString("NAME");
                    ingredient.IMAGE = reader.GetString("IMAGE");
                    ingredient.CATEGORY = reader.GetInt32("CATEGORY");

                    Ingredients.Add(ingredient);
                }
                this._connection.Close();

                package.Payload = Ingredients;
            }
            catch (Exception ex)
            {
                // Fehler bearbeitung 
                package.Payload = null;
                package.Error = ex.Message;
            }
            finally
            {
                // Datenbankverbindung schließen
                this._connection.Close();
            }

            // Datenpaket zurückgebens
            if (package.Payload == null) { package.Error = true; }
            return package;
        }
        /// <summary>
        /// Creates a Ingredient in the Database with the given Recipe Objekt
        /// </summary>
        /// <param name="recipe"></param>
        /// <returns></returns>
        public DataPackage CreateIngredient(Ingredient ingredient)
        {
            DataPackage package = new DataPackage();
            try
            {
                string sql = string.Format(@"INSERT INTO 
                                             Ingredient(NAME,IMAGE,CATEGORY)
                                             VALUES ('{0}','{1}',{2})",ingredient.NAME,ingredient.IMAGE,ingredient.CATEGORY);
                this._connection.Open();
                this.ExecuteQuery(sql);
                this._connection.Close();

                package.Payload = "";
            }
            catch (Exception ex)
            {
                // Fehler bearbeitung 
                package.Payload = null;
                package.Error = ex.Message;
            }
            finally
            {
                // Datenbankverbindung schließen
                this._connection.Close();
            }

            // Datenpaket zurückgebens
            if (package.Payload == null) { package.Error = true; }
            return package;
        }
        /// <summary>
        /// Deletes a Ingredient with the given ID in the Database
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DataPackage DeleteIngredient(int id) 
        {
            DataPackage package = new DataPackage();
            try
            {
                string sql = string.Format(@"DELETE FROM Ingredient WHERE ID = {0}",id);
                this._connection.Open();
                this.ExecuteQuery(sql);
                this._connection.Close();

                package.Payload = "";
            }
            catch (Exception ex)
            {
                // Fehler bearbeitung 
                package.Payload = null;
                package.Error = ex.Message;
            }
            finally
            {
                // Datenbankverbindung schließen
                this._connection.Close();
            }

            // Datenpaket zurückgebens
            if (package.Payload == null) { package.Error = true; }
            return package;
        }
        /// <summary>
        /// Updates Information of a Ingredient 
        /// </summary>
        /// <param name="recipe"></param>
        /// <returns></returns>
        public DataPackage UpdateIngredient(Ingredient ingredient) 
        {
            DataPackage package = new DataPackage();
            try
            {
                string sql = string.Format(@"UPDATE Ingredient 
                                             SET NAME = '{0}',
                                             CATEGORY = {1},
                                             IMAGE = '{2}'
                                             WHERE ID = {3}",ingredient.NAME,ingredient.CATEGORY,ingredient.IMAGE,ingredient.ID);
                this._connection.Open();
                this.ExecuteQuery(sql);
                this._connection.Close();

                package.Payload = "";
            }
            catch (Exception ex)
            {
                // Fehler bearbeitung 
                package.Payload = null;
                package.Error = ex.Message;
            }
            finally
            {
                // Datenbankverbindung schließen
                this._connection.Close();
            }

            // Datenpaket zurückgebens
            if (package.Payload == null) { package.Error = true; }
            return package;
        }
        #endregion
        /// <summary>
        /// Returns a DataPackage with all Difficulty Levels in the Database
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public DataPackage GetDifficulty() 
        {
            DataPackage package = new DataPackage();
            try
            {
                string sql = string.Format(@"SELECT NAME FROM Difficulty;");
                this._connection.Open();
                List<Difficulty> difficulties = new List<Difficulty>();
                MySqlDataReader reader = this.ExecuteQuery(sql);
                while (reader.Read())
                {
                    Difficulty difficulty = new Difficulty();
                    difficulty.ID = reader.GetInt32("ID");
                    difficulty.NAME = reader.GetString("NAME");

                    difficulties.Add(difficulty);
                }
                this._connection.Close();

                package.Payload = difficulties;
            }
            catch (Exception ex)
            {
                // Fehler bearbeitung 
                package.Payload = null;
                package.Error = ex.Message;
            }
            finally
            {
                // Datenbankverbindung schließen
                this._connection.Close();
            }

            // Datenpaket zurückgebens
            if (package.Payload == null) { package.Error = true; }
            return package;
        }
        /// <summary>
        /// Returns a DataPackage with all Categorys in the Database
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public DataPackage GetCategory() 
        {
            DataPackage package = new DataPackage();
            try
            {
                string sql = string.Format(@"SELECT NAME FROM Difficulty;");
                this._connection.Open();
                List<Category> categorys = new List<Category>();
                MySqlDataReader reader = this.ExecuteQuery(sql);
                while (reader.Read())
                {
                    Category category = new Category();
                    category.ID = reader.GetInt32("ID");
                    category.NAME = reader.GetString("NAME");

                    categorys.Add(category);
                }
                this._connection.Close();

                package.Payload = categorys;
            }
            catch (Exception ex)
            {
                // Fehler bearbeitung 
                package.Payload = null;
                package.Error = ex.Message;
            }
            finally
            {
                // Datenbankverbindung schließen
                this._connection.Close();
            }

            // Datenpaket zurückgebens
            if (package.Payload == null) { package.Error = true; }
            return package;
        }

        private MySqlDataReader ExecuteQuery(string sql)
        {
            using (MySqlCommand command = new MySqlCommand(sql, this._connection))
            {
                return command.ExecuteReader();
            }
        }

        private int GetLastInsertedID()
        {
            string sql = string.Format(@"SELECT LAST_INSERT_ID() as 'ID';");
            this._connection.Open();
            MySqlDataReader reader = this.ExecuteQuery(sql);
            int i = 0;
            while (reader.Read())
            {
                i = reader.GetInt32("ID");
                break;
            }
            this._connection.Close();

            return i;
        }
    }
}
