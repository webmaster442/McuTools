using System.Linq;
using System.Windows.Controls;

namespace MCalculator
{
    internal enum Actions
    {
        None,
        Add,
        Multiply,
        Divide,
        Subtract
    }

    internal class Unit
    {
        public string Name { get; set; }
        public double Ratio { get; set; }
        public Actions Action { get; set; }
        public double offset { get; set; }

        public Unit() { }
        public Unit(string Name, double Ratio, Actions Action = Actions.None, double offset = 0)
        {
            this.Name = Name;
            this.Ratio = Ratio;
            this.Action = Action;
            this.offset = offset;
        }
    }

    internal class UnitConverterLogic
    {
        private Unit[] Distance, Flow, Area, Aceleration, Speed, Power, Pressure, Mass, Volume, Time, Temperature;

        public UnitConverterLogic()
        {
            Distance = new Unit[]
            {
                new Unit("Meter", 1, Actions.None),
                new Unit("Feet", 3.280839895, Actions.Multiply),
                new Unit("Inch", 39.37007874, Actions.Multiply),
                new Unit("Yard", 1.093613298, Actions.Multiply),
                new Unit("LightYear", 3.241e-17, Actions.Multiply),
                new Unit("Parsec", 3.241e-17, Actions.Multiply),
                new Unit("Miles", 0.000621371, Actions.Multiply),
                new Unit("Furlong", 0.00497097, Actions.Multiply)
            };
            Flow = new Unit[]
            {
                new Unit("Liters/second",1, Actions.None),
                new Unit("Liters/minute" ,60 , Actions.Multiply),
                new Unit("Litres/hour" ,3600 , Actions.Multiply),
                new Unit("Gallons/hour" ,951.0193885 , Actions.Multiply),
                new Unit("Gallons/minute",15.85032314 , Actions.Multiply),
                new Unit("Gallons/second" ,0.264172052 , Actions.Multiply),
                new Unit("Meters3/hour" ,39878 , Actions.Multiply),
                new Unit("Meters3/minute", 0.06 , Actions.Multiply),
                new Unit("Meters3/second", 0.001 , Actions.Multiply)
            };
            Area = new Unit[]
            {
                new Unit("Meters2" , 1, Actions.None),
                new Unit("Acres" , 0.000247104, Actions.Multiply),
                new Unit("Feet2" , 10.76391042, Actions.Multiply),
                new Unit("Hectares" , 0.0001, Actions.Multiply),
                new Unit("Inches2" , 1550.0031, Actions.Multiply),
                new Unit("Miles2" , 3.86E-07 , Actions.Multiply),
                new Unit("Yards2" , 1.195990046, Actions.Multiply) 
            };
            Aceleration = new Unit[]
            {
                new Unit("Meters/sec2" , 1, Actions.None),
                new Unit("Feet/sec2" , 3.280839895, Actions.Multiply),
                new Unit("Gravity" , 0.101971621, Actions.Multiply),  
                new Unit("Inches/sec2" , 39.37007874 , Actions.Multiply)
            };
            Speed = new Unit[]
            {
                 new Unit("Meters/second" , 1, Actions.None),
                 new Unit("Meters/minute" , 60, Actions.Multiply),
                 new Unit("Meters/hour" , 3600, Actions.Multiply),
                 new Unit("Feet/second" , 3.280839895, Actions.Multiply),
                 new Unit("Feet/minute" , 196.8503937, Actions.Multiply),
                 new Unit("Feet/hour" , 11811.02362, Actions.Multiply),
                 new Unit("Knots" , 1.943844492, Actions.Multiply),
                 new Unit("Mach" , 0.003016955, Actions.Multiply),
                 new Unit("Miles/second" , 0.000621371, Actions.Multiply),
                 new Unit("Miles/minute" , 0.037282272, Actions.Multiply),
                 new Unit("Miles/hour" , 2.236936292, Actions.Multiply)
            };
            Power = new Unit[]
            {
                new Unit("Watt" , 1, Actions.None),
                new Unit("Horsepower" , 0.001341022, Actions.Multiply)
            };
            Pressure = new Unit[]
            {
                new Unit("Pascals" ,  1, Actions.None),
                new Unit("Atmospheres" , 9.87E-06, Actions.Multiply),
                new Unit("Bars" , 1.00E-05, Actions.Multiply),
                new Unit("Pounds/Foot2" , 0.020885434 , Actions.Multiply),
                new Unit("Pounds/Inch2" , 0.000145038, Actions.Multiply),
                new Unit("Tons/Foot2" , 1.04E-05 , Actions.Multiply),
                new Unit("Tons/Inch2" , 7.25E-08, Actions.Multiply),
                new Unit("Kilograms/Meter2" , 0.101971621, Actions.Multiply)
            };
            Mass = new Unit[]
            {
                new Unit("Grams" , 1, Actions.None),
                new Unit("Carats" , 5, Actions.Multiply),
                new Unit("Grains" , 15.43235835, Actions.Multiply),
                new Unit("Ounces" , 0.035273962, Actions.Multiply),
                new Unit("Pennyweights" , 0.643014931, Actions.Multiply),
                new Unit("Pounds" , 0.002204623, Actions.Multiply),
                new Unit("Stones" , 0.000157473, Actions.Multiply),
                new Unit("Tons" , 1.00E-06, Actions.Multiply) 
            };
            Volume = new Unit[]
            {
                new Unit("Litres" , 1, Actions.None),
                new Unit("Inches3" , 61.02374409, Actions.Multiply),
                new Unit("Feet3" , 0.035314667, Actions.Multiply),
                new Unit("Yards3" ,  0.001307951, Actions.Multiply),
                new Unit("Cups" , 4.226752838, Actions.Multiply),
                new Unit("Gallons" , 0.219969152, Actions.Multiply),
                new Unit("Meters3" , 0.001, Actions.Multiply),
                new Unit("Ounces" , 35.19506424, Actions.Multiply),
                new Unit("Pints" , 2.113376419, Actions.Multiply),
                new Unit("Quarts" , 1.056688209, Actions.Multiply),
                new Unit("Tablespoons" , 67.6280454, Actions.Multiply),
                new Unit("Teaspoons" , 202.8841362, Actions.Multiply) 
            };
            Time = new Unit[]
            {
                new Unit("Second" , 1, Actions.None),
                new Unit("Minute" , 60 , Actions.Divide),
                new Unit("Hour" , 3600, Actions.Divide),
                new Unit("Day" , 86400, Actions.Divide)
            };
            Temperature = new Unit[]
            {
                new Unit("Celsius", 1, Actions.None),
                new Unit("Kelvin", 273.15, Actions.Add)
            };
        }

        public double Convert2Standard(Unit u, double value)
        {
            double res = 0;
            switch (u.Action)
            {
                case Actions.Multiply:
                    res = value / u.Ratio;
                    break;
                case Actions.Add:
                    res = value - u.Ratio;
                    break;
                case Actions.Divide:
                    res = value * u.Ratio;
                    break;
                case Actions.Subtract:
                    res = value + u.Ratio;
                    break;
                default:
                    return value;
            }
            return res;
        }

        public double Convert2Unit(Unit u, double stdval)
        {
            double res = 0;
            switch (u.Action)
            {
                case Actions.Multiply:
                    res = stdval * u.Ratio;
                    break;
                case Actions.Divide:
                    res = stdval / u.Ratio;
                    break;
                case Actions.Add:
                    res = stdval + u.Ratio;
                    break;
                case Actions.Subtract:
                    res = stdval - u.Ratio;
                    break;
                default:
                    return stdval;
            }
            return res;
        }

        public double Convert(string source, string destination, Unit[] category, double value)
        {
            Unit Src, Dst;
            Src = (from i in category where string.Compare(i.Name, source, true) == 0 select i).FirstOrDefault();
            Dst = (from i in category where string.Compare(i.Name, destination, true) == 0 select i).FirstOrDefault();

            if (Src == null || Dst == null) return double.NaN;

            double stdval = Convert2Standard(Src, value);
            return Convert2Unit(Dst, stdval);
        }

        private TreeViewItem ListCategory(Unit[] Array, string Header)
        {
            TreeViewItem itm = new TreeViewItem();
            itm.Header = Header;
            var ordered = (from i in Array orderby i.Name ascending select i).ToArray();
            foreach (var unit in ordered)
            {
                TreeViewItem i = new TreeViewItem();
                i.Header = unit.Name;
                itm.Items.Add(i);
            }
            itm.IsExpanded = true;
            return itm;

        }

        public void FillTreeview(TreeView Control)
        {
            Control.Items.Add(ListCategory(Distance, "Distance"));
            Control.Items.Add(ListCategory(Flow, "Flow"));
            Control.Items.Add(ListCategory(Area, "Area"));
            Control.Items.Add(ListCategory(Aceleration, "Aceleration"));
            Control.Items.Add(ListCategory(Speed, "Speed"));
            Control.Items.Add(ListCategory(Power, "Power"));
            Control.Items.Add(ListCategory(Pressure, "Pressure"));
            Control.Items.Add(ListCategory(Mass, "Mass"));
            Control.Items.Add(ListCategory(Volume, "Volume"));
            Control.Items.Add(ListCategory(Time, "Time"));
            Control.Items.Add(ListCategory(Temperature, "Temperature"));
        }

        public Unit[] TreeviewItemToCategory(TreeViewItem item)
        {
            TreeViewItem i = null;

            if (item.Items.Count == 0)
            {
                object parent = item.Parent;
                if (parent != null && (parent is TreeViewItem))
                {
                    i = (TreeViewItem)parent;
                }
            }
            else i = item;

            if (i == null) return null;

            switch (i.Header.ToString())
            {
                case "Distance":
                    return Distance;
                case "Flow":
                    return Flow;
                case "Area":
                    return Area;
                case "Aceleration":
                    return Aceleration;
                case "Speed":
                    return Speed;
                case "Power":
                    return Power;
                case "Pressure":
                    return Pressure;
                case "Mass":
                    return Mass;
                case "Volume":
                    return Volume;
                case "Time":
                    return Time;
                case "Temperature":
                    return Temperature;
                default:
                    return null;
            }
        }
    }
}
