using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroSim_TM
{
    public class CSynapse
    {
        public int pre_id; //id of a presynaptic neuron
        public int post_id; //id of a postsynaptic neuron
        public double I; //synaptic current

        public double w; //synaptic weight

        public double l; //length of a connection
        public double tau_delay; //axonal delay

        public double A; //synaptic current magnitude
        public double U; //supplementary synaptic magnitude
        public double u; //use of synaptic resources
        public double x; //recovered
        public double y; //active           states of synaptic resources
        public double z; //inactive

        public double tau_I; //decay constant
        public double tau_rec; //syn depression recovery time
        public double tau_facil; //u time constant
        //public CSynapse *next; //pointer on the next synapse in list
        public List<double> timers;

        public CSynapse()
        {
            timers = new List<double>();
        }
    }
}
