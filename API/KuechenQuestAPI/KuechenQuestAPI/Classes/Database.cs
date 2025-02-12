using KuechenQuestAPI.Models;
using MySql.Data.MySqlClient;
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

        public DataPackage Login(string username, string password) { throw new NotImplementedException(); }
        
        public DataPackage GetRecipe(int id) 
        {
            // Datenpaket erstellen
            DataPackage package = new DataPackage();
            try
            {
                // Datenbankverbindung öffnen
                this._connection.Open();
                
                // Daten abfragen
                string sql = string.Format(@"SELECT * FROM RECIPE WHERE ID = {0}", id);
                MySqlDataReader reader = this.ExecuteQuery(sql);
                
                // Ergebniss verarbeiten
                while (reader.Read())
                {
                    Recipe recipe = new Recipe();
                    // Recipe auffüllen
                    package.Payload = recipe;
                }
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
            return package;
        }
        public DataPackage GetAllRecipes() { throw new NotImplementedException(); }
        public DataPackage CreateRecipe() { throw new NotImplementedException(); }
        public DataPackage DeleteRecipe() { throw new NotImplementedException(); }
        public DataPackage UpdateRecipe(Recipe recipe) { throw new NotImplementedException(); }

        public DataPackage GetUtensil(int id) { throw new NotImplementedException(); }
        public DataPackage GetAllUtensils() { throw new NotImplementedException(); }
        public DataPackage CreateUtensil() {  throw new NotImplementedException(); }
        public DataPackage DeleteUntensil() { throw new NotImplementedException(); }
        public DataPackage UpdateUtensil(Utensil utensil) { throw new NotImplementedException(); }

        public DataPackage GetIngredient(int id) { throw new NotImplementedException(); }
        public DataPackage GetAllIngredients() { throw new NotImplementedException(); }
        public DataPackage CreateIngredient() { throw new NotImplementedException(); }
        public DataPackage DeleteIngredient() { throw new NotImplementedException(); }
        public DataPackage UpdateIngredient() { throw new NotImplementedException(); }


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
