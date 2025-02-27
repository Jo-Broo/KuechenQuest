using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginUI.Components.Models
{
    public class Utensil
    {
        public int ID { get; set; }
        public string NAME { get; set; } = string.Empty;
        public float QUANTITY { get; set; }
        public string IMAGE { get; set; } = string.Empty;
    }
}
