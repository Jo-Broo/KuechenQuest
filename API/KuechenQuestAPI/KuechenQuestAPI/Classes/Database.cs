using KuechenQuestAPI.Models;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Crypto.Agreement.JPake;
using System.Text.Json;

namespace KuechenQuestAPI.Classes
{
    public class Database
    {
        private MySqlConnection _connection;

        public Database(string connectionString)
        {
            this._connection = new MySqlConnection(connectionString);
        }

        public DataPackage Login(string username, string password) 
        {
            DataPackage package = new DataPackage();
            try
            {
                string sql = string.Format(@"select ID from user where NAME = '{0}' AND PASSWORD = '{1}';",username,password);
                this._connection.Open();
                MySqlDataReader reader = this.ExecuteQuery(sql);
                int i = 0;
                while (reader.Read()) 
                {
                    i = reader.GetInt32("ID");
                }
                this._connection.Close();

                if (i == 0) { package.Payload = null; }
                else { package.Payload = ""; }
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
        public DataPackage GetAchievments(string username) { throw new NotImplementedException(); }

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
                    //recipe.IMAGE = reader.GetString("IMAGE");
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
                    utensil.QUANTITY = reader.GetInt32("QUANTITY");

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
                                        c.NAME as 'CNAME',
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
                    ingredient.CATEGORY = reader.GetString("CNAME");

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
                    recipes.Add(recipe);
                    //recipe.IMAGE = reader.GetString("IMAGE");
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
                                                                c.NAME as 'CNAME',
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
                        ingredient.CATEGORY = reader.GetString("CNAME");

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
                                             Recipe(NAME, TIME, DIFFICULTY, INSTRUCTIONS)
                                             VALUES ('{0}',
                                                      {1},
                                                      {2},
                                                      '{3}');",recipe.NAME,recipe.TIME,recipe.DIFFICULTY,recipe.INSTRUCTIONS);
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
        public DataPackage UpdateRecipe(Recipe recipe) { throw new NotImplementedException(); }

        public DataPackage GetUtensil(int id) { throw new NotImplementedException(); }
        public DataPackage GetAllUtensils() { throw new NotImplementedException(); }
        public DataPackage CreateUtensil() {  throw new NotImplementedException(); }
        public DataPackage DeleteUntensil(int id) { throw new NotImplementedException(); }
        public DataPackage UpdateUtensil(Utensil utensil) { throw new NotImplementedException(); }

        public DataPackage GetIngredient(int id) { throw new NotImplementedException(); }
        public DataPackage GetAllIngredients() { throw new NotImplementedException(); }
        public DataPackage CreateIngredient() { throw new NotImplementedException(); }
        public DataPackage DeleteIngredient(int id) { throw new NotImplementedException(); }
        public DataPackage UpdateIngredient() { throw new NotImplementedException(); }

        public DataPackage GetDifficulty() { throw new NotImplementedException(); }


        public DataPackage GetUser(int UserID)
        {
            DataPackage package = new DataPackage();

            try
            {
                string query = string.Format(@"SELECT * FROM User WHERE ID = {0}", UserID);
                this._connection.Open();
                MySqlDataReader reader = this.ExecuteQuery(query);

                while (reader.Read())
                {
                    User user = new User();
                    user.ID = Convert.ToInt32(reader["ID"]);
                    user.NAME = reader["NAME"].ToString() ?? "";
                    user.LEVEL = Convert.ToInt32(reader["LEVEL"]);
                    user.XP = Convert.ToInt32(reader["XP"]);
                    user.EMAIL = reader["EMAIL"].ToString() ?? "";
                    package.Payload = user;

                    break;
                }
            }
            catch (Exception ex)
            {
                package.Payload = null;
                package.Error = true;
            }
            finally
            {
                this._connection.Close();
            }

            if(package.Payload == null) { package.Error = true; }

            return package;
        }

        private MySqlDataReader ExecuteQuery(string sql)
        {
            using (MySqlCommand command = new MySqlCommand(sql, this._connection))
            {
                return command.ExecuteReader();
            }
        }
    }
}
