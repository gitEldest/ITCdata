﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITCdata
{
    public struct Experiment
    {
        public string title;
        public double slope;
        public double intercept;
        public double conc;
        public List<double> avgHeat;

        public Experiment(string name, double sl, double it, double injConc, List<double> injHeat) {
            title = name;
            slope = sl;
            intercept = it;
            conc = injConc;
            avgHeat = new List<double>(injHeat);
        }

    }
}
