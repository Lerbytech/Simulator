using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroSim_TM
{
    public class CNeuron
    {
        public int type; //inhibitory or excitatory
        public int id;
        public double x; //coordinates of a neuron, set to (0,0) for BINOMIAL topology, otherwise described by LAYER_TYPE
        public double y;
        public int num_of_outcoming_connections; //amount of outcoming connections of a neuron, stored to be easily accessed without re-calculating
        public double V;                 //mV, membrane potential of a neuron
        public double V_rest;            //mV, resting value of V
        public double V_reset;           //mV, resetting value of V, V is set to this value after emitting a spike
        public double I_b;               //pA, background current value
        public double I_b_init;          //pA, initital value of background current, as the I_b itself can be decreased by the homeostasis mechanism, if the latter is turned on
        public double p_sp;              //probability of emitting a spike on each step of simulation
        public double ref_time_left;     //0 until neuron emits spike, then 3 or 2 ms of excitatory and inhibitory neurons respectively
        public double last_spiked_at;    //time of the last emitted spike, used for STDP rules
        public List<CSynapse> out_conn;  //list for output synapses
        public List<CSynapse> in_conn;   //list for input synapses

        public CNeuron()
        {
            out_conn = new List<CSynapse>();
            in_conn = new List<CSynapse>();
        }

        public CNeuron(int id) : base()
        {
            this.id = id;
        }
    }
}