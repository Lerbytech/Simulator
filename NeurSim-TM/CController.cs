using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroSim_TM
{
    public class CController
    {
        string path_to_inputFile;
        Dictionary<string, object> Pars;
        public CNeuronManager NM;
        public CSimulator Sim; 
        public CIOManager IO;

        public CController(string path)
        {
            path_to_inputFile = path;
            IO = new CIOManager(path);
            Pars = IO.ReadInputFile();
            NM = new CNeuronManager();
            //Sim = new CSimulator();
            
            Init();
        }

        public void Init()
        {   
            Val.Init(Pars);
        }

        public string[] GetModeDescription()
        {
            List<string> S = new List<string>();
            S.Add("********************NeuroSimTM-2.0 C#*****************");
            S.Add("Pure C version 12.07.2016");
            S.Add("Rewritten version 26.09.2016");

            S.Add(String.Empty);
            S.Add("Reading input file...");

            S.Add("Ready to start simulation with following parameters:");
            S.Add(String.Empty);
            S.Add(">>>> GENERAL <<<<");
            S.Add("N = " + Val.N.ToString());
            S.Add("INHIBITORY neurons fraction: " + Val.INH_NEURONS_FRACTION.ToString());

            S.Add(String.Empty);
            S.Add(">>>> NEURON MODEL <<<<");
            if (Val.NEURON_MODEL == NeuronModel.PERFECT_INTEGRATE_AND_FIRE)
            {
                S.Add("Neuron Model: Perfect Integrate-and-Fire (PIF)");
                S.Add("C_m = " + Val.C_M.ToString() + " pF");
            }
            else
            {
                S.Add("Neuron Model: Leaky Integrate-and-Fire (LIF)");
                S.Add("TAU_M = " + Val.TAU_M.ToString() + " ms");
                S.Add("R_in = " + Val.R_IN.ToString() + " GOhM");
            }
            S.Add("V_REST = " + Val.V_REST.ToString() + " mv");
            S.Add("V_TH = " + Val.V_TH.ToString() + " mv");

            S.Add(String.Empty);
            S.Add(">>>> STIMULATION <<<<");

            if (Val.STIMULATION_TYPE == StimType.STIM_TYPE_I_BG_GAUSSIAN || Val.STIMULATION_TYPE == StimType.STIM_TYPE_I_BG_TWO_VALUES)
            {
                S.Add("Stimulation type: Background currents");
                if (Val.USE_SAVED_STIMULATION_DATA == StimulationData.READ_STIMULATION_DATA_FROM_FILE)
                {
                    S.Add("Background currents will be read from file");
                }
                else
                {
                    S.Add("Background currents will be generated with following properties");
                    if (Val.STIMULATION_TYPE == StimType.STIM_TYPE_I_BG_GAUSSIAN)
                    {
                        S.Add("I_bg_mean = " + Val.I_BG_MEAN.ToString() + " pA");
                        S.Add("I_bg_sd =  " + Val.I_BG_SD.ToString() + " pA");
                        S.Add("I_bg_min = " + Val.I_BG_MIN.ToString() + " pA");
                        S.Add("I_bg_max = " + Val.I_BG_MAX.ToString() + " pA");
                    }
                    else
                    {
                        S.Add("I_bg_1 = " + Val.I_BG_1.ToString() + " pA");
                        S.Add("I_bg_2 = " + Val.I_BG_2.ToString() + " pA");
                        S.Add("Fraction_of_neurons_with_I_bg_1 = " + Val.FRACTION_OF_NEURONS_WITH_I_BG_1.ToString());
                    }
                }
            }
            else
            {
                S.Add("Stimulation type: Spontaneous spiking");
                if (Val.USE_SAVED_STIMULATION_DATA == StimulationData.READ_STIMULATION_DATA_FROM_FILE)
                {
                    S.Add("P_sp probabilities will be read from file");
                }
                else
                {
                    if (Val.STIMULATION_TYPE == StimType.STIM_TYPE_P_SP_GAUSSIAN)
                    {
                        S.Add("P_sp probabilities will be generated with following properties");
                        S.Add("P_SP_mean = " + Val.P_SP_MEAN.ToString());
                        S.Add("P_SP_sd = " + Val.P_SP_SD.ToString());
                        S.Add("P_SP_min = " + Val.P_SP_MIN.ToString());
                        S.Add("P_SP_max = " + Val.P_SP_MAX.ToString());
                    }
                    else
                    {
                        S.Add("p_sp_1 = " + Val.P_SP_1.ToString());
                        S.Add("p_sp_2 = " + Val.P_SP_2.ToString());
                        S.Add("Fraction_of_neurons_with_p_sp_1 = " + Val.FRACTION_OF_NEURONS_WITH_P_SP_1.ToString());
                    }
                }
            }

            S.Add(String.Empty);
            S.Add(">>>> TOPOLOGY <<<<");
            switch (Val.layer_type)
            {
                case LayerType.BINOMIAL: S.Add("Layer type: BINOMIAL"); break;
                case LayerType.UNIFORM: S.Add("Layer type: UNIFORM"); break;
                case LayerType.SQUARE_LATTICE: S.Add("Layer type: SQUARE_LATTICE"); break;
                case LayerType.BELL: S.Add("Layer type: BELL"); break;
                case LayerType.RAMP: S.Add("Layer type: RAMP"); break;
                case LayerType.DOUBLE_RAMP: S.Add("Layer type: DOUBLE RAMP"); break;
                case LayerType.BARBELL: S.Add("Layer type: BARBELL"); break;
                case LayerType.STRIPED: S.Add("Layer type: STRIPED"); break;
            }

            S.Add(String.Empty);
            if (Val.FORCE_BINOMIAL_TOPOLOGY == 1)
            {
                S.Add("FORCED TO BINOMIAL TOPOLOGY");
                S.Add("P_con = " + Val.P_CON);
            }

            if (Val.layer_type ==  LayerType.BINOMIAL)
                S.Add("P_con = " + Val.P_CON.ToString());
            else
            {
                if (Val.FORCE_BINOMIAL_TOPOLOGY != 1) S.Add("lambda = " + Val.lambda);
                S.Add("max_conn_length = " + Val.max_conn_length + " L");
                S.Add("spike_speed = " + Val.SPIKE_SPEED + " L/ms");
                S.Add("tau_delay = " + Val.tau_delay + " ms");
            }

            if (Val.USE_SAVED_TOPOLOGY == Topology.READ_TOPOLOGY_FROM_FILE || Val.USE_SAVED_SYNAPTIC_PARAMETERS == SynapticData.READ_SYNAPTIC_DATA_FROM_FILE ||
         Val.USE_SAVED_COORDINATES == Coordinates.READ_COORDINATES_FROM_FILE || Val.USE_SAVED_STIMULATION_DATA == StimulationData.READ_STIMULATION_DATA_FROM_FILE)
            {
                S.Add(">>>> USING SAVED DATA <<<<");

                if (Val.USE_SAVED_TOPOLOGY == Topology.READ_TOPOLOGY_FROM_FILE)
                {
                    S.Add("TOPOLOGY is being READ FROM FILE");
                }
                else
                {
                    // in case we don't read topology input file we generate it
                    S.Add("TOPOLOGY will be GENERATED for this run");
                }


                if (Val.USE_SAVED_SYNAPTIC_PARAMETERS == SynapticData.READ_SYNAPTIC_DATA_FROM_FILE)
                {
                    if (Val.USE_SAVED_TOPOLOGY != Topology.READ_TOPOLOGY_FROM_FILE)
                    {
                        S.Add("ERROR: Synaptic parameters cannot be read from file if topology is not read from file!");
                        throw new Exception("ERROR: Synaptic parameters cannot be read from file if topology is not read from file!");
                    }
                    S.Add("SYNAPTIC PARAMETERS distribution is being READ FROM FILE");
                }
                else
                {
                    // in case we don't read synaptic parameters from file we generate them
                    S.Add("SYNAPTIC PARAMETERS will be GENERATED for this run");
                }

                if (Val.USE_SAVED_COORDINATES == Coordinates.READ_COORDINATES_FROM_FILE)
                    S.Add("COORDINATES are being READ FROM FILE expecting ");
                else { S.Add("COORDINATES will be GENERATED with "); }

                switch (Val.layer_type)
                {
                    case LayerType.BINOMIAL: S.Add("layer type BINOMIAL"); break;
                    case LayerType.UNIFORM: S.Add("layer type UNIFORM"); break;
                    case LayerType.SQUARE_LATTICE: S.Add("layer type SQUARE_LATTICE"); break;
                    case LayerType.BELL: S.Add("layer type BELL"); break;
                    case LayerType.RAMP: S.Add("layer type RAMP"); break;
                    case LayerType.DOUBLE_RAMP: S.Add("layer type DOUBLE RAMP"); break;
                    case LayerType.BARBELL: S.Add("layer type BARBELL"); break;
                    case LayerType.STRIPED: S.Add("layer type STRIPED"); break;
                }

                if (Val.USE_SAVED_STIMULATION_DATA == StimulationData.READ_STIMULATION_DATA_FROM_FILE)
                    S.Add("STIMULATION DATA is being READ FROM FILE");
                else
                    S.Add("STIMULATION DATA will be GENERATED for this run");
            }

            S.Add(String.Empty);
            S.Add(">>>> ADDITIONAL MECHANISMS <<<<");

            if (Val.BG_CURRENT_NOISE_MODE == 0)
                S.Add("BG current noise is OFF");
            else
            {
                S.Add("BG current noise is ON");
                S.Add("I_bg_noise_sd = " + Val.I_BG_NOISE_SD.ToString() + " pA");
            }

            switch (Val.STDP_status)
            {
                case STDP.STDP_IS_OFF: S.Add("STDP status is OFF"); break;
                case STDP.STDP_IS_ADDITIVE: S.Add("STDP status is ADDITIVE"); break;
                case STDP.STDP_IS_MULTIPLICATIVE: S.Add("STDP status is MULTIPLICATIVE"); break;
            }

            if ( Val.HOMEOSTASIS_status == (int)Homeostasis.HOMEOSTASIS_IS_ON)
                S.Add("HOMEOSTASIS is ON");
            else
                S.Add("HOMEOSTASIS is OFF");

            if (Val.stim_disable_protocol != 0)
                S.Add("STIMULATION DISABLING protocol is ON");

            S.Add(String.Empty);
            if (Val.inh_off_time != -1)
                S.Add("Inhibitory neurons will be forced to V_REST on " + Val.inh_off_time.ToString() + " ms");

            S.Add(String.Empty);
            S.Add(">>>> SIMULATION PARAMETERS <<<<");
            S.Add("Burst detection theshold: " + Val.burst_threshold.ToString());
            S.Add("Time step dt = " +  Val.dt.ToString() + " ms");
            S.Add("Averaging time: " + Val.AVG_TIME.ToString() + " ms");
            S.Add("Simulation time: " + Val.SIM_TIME.ToString() + " ms");

            return S.ToArray();
        }

        public void StartSimulation()
        {
          IO.PrepareIOFiles();
          NM.CreateNeurons();  
          
          //for (int i = 0; i < Sim.Start();


        }

    }
}
