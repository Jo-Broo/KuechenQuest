using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LoginUI.Data.Models;

namespace LoginUI.Data
{
        public class KuechenQuestDbContext : DbContext
        {

            public KuechenQuestDbContext(DbContextOptions<KuechenQuestDbContext> options) : base(options)
            {
            }

            public DbSet<Achievement> Achievements { get; set; }
            public DbSet<Category> Categories { get; set; }
            public DbSet<Difficulty> Difficulties { get; set; }
            public DbSet<Ingredient> Ingredients { get; set; }
            public DbSet<Recipe> Recipes { get; set; }
            public DbSet<RecipeIngredient> RecipeIngredients { get; set; }
            public DbSet<RecipeUtensil> RecipeUtensils { get; set; }
            public DbSet<User> Users { get; set; }
            public DbSet<UserAchievement> UserAchievements { get; set; }
            public DbSet<Utensil> Utensils { get; set; }
        }
}

