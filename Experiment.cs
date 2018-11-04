using System.Collections.Generic;

namespace ITCdata
{
    public struct Experiment
    {
        public string title;
        public double slope;
        public double intercept;
        public double conc;
        public double rSquared;
        public List<double> avgHeat;

        public Experiment(string name, double sl, double rS, double it, double injConc, List<double> injHeat) {
            title = name;
            slope = sl;
            intercept = it;
            conc = injConc;
            rSquared = rS;
            avgHeat = new List<double>(injHeat);
        }

    }
}
