using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace дипломная_работа.Model
{
    public class Stop
    {
        Town _town;
        Interval _period;
        public Town Town {
            get{
            return _town;
            }
            set
            {
                _town = value;
            }
        }
        public Interval Period
        {
            get
            {
                return _period;
            }
            set
            {
                 _period = value;
            }
        }
        public Stop(Town Town, Interval Period)
        {
            _town = Town;
            _period = Period;
        }
        public Stop(Town Town) : this(Town, new Interval(1,1)) { }
        public Stop():this(new Town(), new Interval(1, 1)) { }
    }
}
