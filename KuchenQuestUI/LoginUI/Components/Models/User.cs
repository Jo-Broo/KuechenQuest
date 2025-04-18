﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginUI.Components.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public byte Level { get; set; }
        public int XP { get; set; }
        public string? Email { get; set; }
        public string IMAGE { get; set; }
    }
}
