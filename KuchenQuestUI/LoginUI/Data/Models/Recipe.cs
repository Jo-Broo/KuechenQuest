using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginUI.Data.Models
{
    public class Recipe
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Time { get; set; }
        public int Difficulty { get; set; }
        public string? Instructions { get; set; }
        public byte Rating { get; set; }
        public int RatingCount { get; set; }
        public string? Image { get; set; }
        public Difficulty? DifficultyRef { get; set; }
    }
}
