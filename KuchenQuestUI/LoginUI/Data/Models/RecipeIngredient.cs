using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginUI.Data.Models
{
    public class RecipeIngredient
    {
        public int Id { get; set; }
        public int RecipeId { get; set; }
        public int IngredientId { get; set; }
        public float Quantity { get; set; }
        public Recipe? Recipe { get; set; }
        public Ingredient? Ingredient { get; set; }
    }
}
