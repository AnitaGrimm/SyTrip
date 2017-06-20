using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace дипломная_работа.Model
{
    public enum Cabin { COACH, PREMIUM_COACH, BUSINESS, FIRST, NONE }
    public class Querry: ObservableCollection<Stop>
    {
        double _budget;
        public Town NativeTown;
        public DateTime DateOfBigiinning1;
        public DateTime DateOfBigiinning2;
        public int AdultsCount = 0, ChildrenCount = 0, CeniorsCount = 0, InfantSeatCount = 0, InfantLapCount = 0;
        public int MaxDayCount = 0;
        public int RoomCount = 0;
        public bool IsOneHotel = false;
        public Cabin prefferedCabin = Cabin.COACH;
        public double Budget
        {
            get
            {
                return _budget;
            }
            set
            {
                if (value > 0)
                    _budget = value;
            }
        }
        public Querry(double Budget)
        {
            this.Budget = Budget;
        }
        public Querry(): this(0) { }
    }
}
