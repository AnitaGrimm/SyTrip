using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace дипломная_работа.Model
{
    public class Interval
    {
        int _from,
            _to;
        public int From
        {
            get
            {
                return _from;
            }
            set
            {
                if (value >= _to && value > 0)
                    _from = value;
            }
        }
        public int To
        {
            get
            {
                return _to;
            }
            set
            {
                if (value > 0)
                    _to = value;
            }
        }
        public Interval(int To, int From)
        {
            if (_to >= 0 && _from >= 0)
            {
                _to = To;
                _from = From;
            }
        }
        public Interval(): this(0,0) { }

    }
}
