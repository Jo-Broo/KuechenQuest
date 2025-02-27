using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginUI.Components.Models
{
    public class Recipe
    {
        public int ID { get; set; }
        public string NAME { get; set; } = string.Empty;
        public int TIME { get; set; }
        public int DIFFICULTY { get; set; }
        public string INSTRUCTIONS { get; set; } = string.Empty;
        public int RATING { get; set; }
        public int RATINGCOUNT { get; set; }
        public int CREATEDBY { get; set; }
        public List<Ingredient> Ingredients { get; set; } = new List<Ingredient>();
        public List<Utensil> Utensils { get; set; } = new List<Utensil> { };
        public User User { get; set; } = new User();
        public string IMAGE { get; set; } = string.Empty;
    }
}
