using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace BMI_Calculator
{
    public class UserData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }  
        public int Age { get; set; }
        public double Weight { get; set; }
        public double Height { get; set; }
        public double BMI { get; set; }
        public DateTime Timestamp { get; set; }

    }
}
