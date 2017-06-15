using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace дипломная_работа.Model
{
    public class Country
    {
        public string Code;
        public string Name;
        public string Name_rus;
        public List<Town> Towns = new List<Town>();
        public Country()
        {

        }
    }
}
