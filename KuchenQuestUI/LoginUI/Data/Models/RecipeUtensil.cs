using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginUI.Data.Models
{
    public class RecipeUtensil
    {
        public int Id { get; set; }
        public int RecipeId { get; set; }
        public int UtensilId { get; set; }
        public int Quantity { get; set; }
        public Recipe? Recipe { get; set; }
        public Utensil? Utensil { get; set; }
    }
}
