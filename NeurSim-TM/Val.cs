using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace NeuroSim_TM
{
    public enum Type { INHIBITORY = 0,
                       EXCITATORY = 1 };
    public enum StimType { STIM_TYPE_I_BG_GAUSSIAN = 1,
                           STIM_TYPE_I_BG_TWO_VALUES = 2,
                           STIM_TYPE_P_SP_GAUSSIAN = 3,
                           STIM_TYPE_P_SP_TWO_VALUES = 4 };
    public enum Topology { GENERATE_TOPOLOGY = 0,
                           READ_TOPOLOGY_FROM_FILE = 1 };
    public enum StimulationData { GENERATE_STIMULATION_DATA = 0,
                                  READ_STIMULATION_DATA_FROM_FILE = 1 };
    public enum SynapticData { GENERATE_SYNAPTIC_DATA = 0,
                               READ_SYNAPTIC_DATA_FROM_FILE = 1 };
    public enum Coordinates { GENERATE_COORDINATES = 0,
                              READ_COORDINATES_FROM_FILE = 1 };
    public enum STDP { STDP_IS_OFF = 0,
                       STDP_IS_MULTIPLICATIVE = 1,
                       STDP_IS_ADDITIVE = 2 };
    public enum Homeostasis { HOMEOSTASIS_IS_OFF = 0,
                               HOMEOSTASIS_IS_ON = 1 };
    //public const int MAXIMUM_NUMBER_ OF_NEURONS = 1000000;
    public enum LayerType { BINOMIAL = 0,
                            UNIFORM = 1,
                            SQUARE_LATTICE = 2,
                            STRIPED = 3,
                            BELL = 4,
                            RAMP = 5,
                            DOUBLE_RAMP = 6,
                            BARBELL = 7 };
    public enum NeuronModel { PERFECT_INTEGRATE_AND_FIRE = 0,
                              LEAKY_INTEGRATE_AND_FIRE = 1 };

    public static class Val
    {


        private static Dictionary<string, object> _input_pars;
        public static Dictionary<string, object> input_pars
        {
           get { return _input_pars; }
           private set { _input_pars = new Dictionary<string, object>(value); }
        }

        #region variables
        public const int MAX_NUMBER_OF_NEURONS = 1000000;
        public static Topology USE_SAVED_TOPOLOGY { get; set; }
        public static Coordinates USE_SAVED_COORDINATES;
        public static StimulationData USE_SAVED_STIMULATION_DATA;
        public static SynapticData USE_SAVED_SYNAPTIC_PARAMETERS;
        
        // setting default values in case we don't find them in a file
        public static int N;        //number of neurons
        public static double dt;   //simulation time step
        public static double INH_NEURONS_FRACTION;  //fraction of inhibitory neurons among N

        public static NeuronModel NEURON_MODEL; //membrane potential model of a neuron. 1 for Leaky-integrate-and-fire(LIF), 0 for Perfect Integrate-and-Fire(PIF)

        //LIF model default parameters
        public static double TAU_M;  //ms, membrane potential V relaxation constant
        public static double R_IN;   //GOhm, membrane resistance
        public static double V_REST; //mV, resting potential
        public static double V_TH;   //mV, threshold potential
        public static double V_RESET;   //mV, threshold potential
        public static LayerType layer_type;
        //PIF model addition
        public static double C_M; //pF, membrane capacitance

        //T_M model default parameters (mean values of synaptic parameters)
        public static double[][] Avg_A;//pA
        public static double[][] Avg_U;
        public static double[][] Avg_tau_rec;//ms
        public static double[][] Avg_tau_facil;//ms
        public static double TAU_I; //ms

        public static double SYN_RESOURCES_OUTPUT_PERIOD; //ms

        public static StimType STIMULATION_TYPE; //stimulation type, 1 for background currents, 2 for spontaneous spiking

        //stimulation default parameters for background currents
        public static double I_BG_MEAN; //pA
        public static double I_BG_SD; //pA
        public static double I_BG_MIN; //pA
        public static double I_BG_MAX; //pA

        public static double I_BG_1; //pA
        public static double I_BG_2; //pA
        public static double FRACTION_OF_NEURONS_WITH_I_BG_1;

        public static double I_BG_NOISE_SD;
        public static int BG_CURRENT_NOISE_MODE;

        //default parameters for the case of stochastic stimulation
        public static double P_SP_MEAN;
        public static double P_SP_SD;
        public static double P_SP_MIN;
        public static double P_SP_MAX;

        public static double P_SP_1;
        public static double P_SP_2;
        public static double FRACTION_OF_NEURONS_WITH_P_SP_1;

        //STDP parameters
        public static STDP STDP_status;
        public static double A_plus;
        public static double A_minus;
        public static double w_initial;
        public static double tau_plus_corr;
        public static double tau_minus_corr;
        public static double W_OUTPUT_PERIOD;


        //topology parameters
        public static double lambda; //characteristic length of a connection within an exponential distribution in spatially-dependent topology
        public static double tau_delay; //base axonal delay
        public static double SPIKE_SPEED; //speed of spike propagating along the axon. All connections are considered straight lines.
        public static double max_conn_length; //maximum length of a connection in spatially-dependent topology

        public static double P_CON; //probability of connecting two neurons in case of BINOMIAL topology
        public static int FORCE_BINOMIAL_TOPOLOGY;
        //Homeostasis parameters
        public static int HOMEOSTASIS_status;
        public static double b;
        public static double M;
        public static double M_max;

        //stim disabling protocol
        public static int stim_disable_protocol; //(1 for turning stimulation off when overcoming the activity threshold
                                   // 2 for turning stimulation off at a certain time)
        public static int pb_number;            //number of burst to which the stimulation disabling protocol is applied
        public static double activity_threshold; //activity threshold of the pb_number-th burst, at which the stimulation disabling protocol is applied
        public static double stim_off_time; //(ms - moment when to turn off the stimulation in case stim_disable_protocol = 2)

        public static double inh_off_time; //ms - moment in time when to set membrane potential V to V_rest until the simulation end. If set to -1 - mechanism is turend OFF

        //simulation parameters
        public static double AVG_TIME; //ms, activity averaging time
        public static int SIM_TIME; //ms, simulation duration
        public static double burst_threshold; //activity level for detecting a burst
        #endregion

        public static void Init(Dictionary<string, object> pars)
        {

            SetDefaultValues();
            ParseInputParameters(pars);
            input_pars = pars;
        }

        private static void SetDefaultValues()
        {
            N = 500;        //number of neurons
            dt = 0.01;   //simulation time step
            INH_NEURONS_FRACTION = 0.2;  //fraction of inhibitory neurons among N

            NEURON_MODEL = NeuronModel.LEAKY_INTEGRATE_AND_FIRE; //membrane potential model of a neuron. 1 for Leaky-integrate-and-fire(LIF), 0 for Perfect Integrate-and-Fire(PIF)

            //LIF model default parameters
            TAU_M = 30;  //ms, membrane potential V relaxation constant
            R_IN = 1;   //GOhm, membrane resistance
            V_REST = 0; //mV, resting potential
            V_TH = 15;   //mV, threshold potential
            V_RESET = 13.5;   //mV, threshold potential

            //PIF model addition
            C_M = 20; //pF, membrane capacitance

            //T_M model default parameters (mean values of synaptic parameters)
            Avg_A = new double[2][];
            Avg_A[0] = new double[2]; Avg_A[1] = new double[2];
            Avg_A[0][0] = -72; Avg_A[0][1] = -72;
            Avg_A[0][0] = 54; Avg_A[0][1] = 38;

            Avg_U = new double[2][];
            Avg_U[0] = new double[2]; Avg_U[1] = new double[2];
            Avg_U[0][0] = 0.04; Avg_U[0][1] = 0.04;
            Avg_U[0][0] = 0.5; Avg_U[0][1] = 0.5;

            Avg_tau_rec = new double[2][];
            Avg_tau_rec[0] = new double[2]; Avg_tau_rec[1] = new double[2];
            Avg_tau_rec[0][0] = 100; Avg_tau_rec[0][1] = 100;
            Avg_tau_rec[0][0] = 800; Avg_tau_rec[0][1] = 800;

            Avg_tau_facil = new double[2][];
            Avg_tau_facil[0] = new double[2]; Avg_tau_facil[1] = new double[2];
            Avg_tau_facil[0][0] = 1000; Avg_tau_facil[0][1] = 1000;
            Avg_tau_facil[0][0] = 0; Avg_tau_facil[0][1] = 0;

            TAU_I = 3; //ms

            SYN_RESOURCES_OUTPUT_PERIOD = 10; //ms

            STIMULATION_TYPE = StimType.STIM_TYPE_I_BG_GAUSSIAN; //stimulation type, 1 for background currents, 2 for spontaneous spiking

            //stimulation default parameters for background currents
            I_BG_MEAN = 7.7; //pA
            I_BG_SD = 4.9; //pA
            I_BG_MIN = 0; //pA
            I_BG_MAX = 20; //pA

            I_BG_1 = 22; //pA
            I_BG_2 = 9; //pA
            FRACTION_OF_NEURONS_WITH_I_BG_1 = 0.25;

            I_BG_NOISE_SD = 0;

            //default parameters for the case of stochastic stimulation
            P_SP_MEAN = 0.00002;
            P_SP_SD = 0.000005;
            P_SP_MIN = 0;
            P_SP_MAX = 0.001;

            P_SP_1 = 1.5 * P_SP_MEAN;
            P_SP_2 = 0.6 * P_SP_MEAN;
            FRACTION_OF_NEURONS_WITH_P_SP_1 = 0.25;

            //STDP parameters
            STDP_status = STDP.STDP_IS_OFF;
            A_plus = 0.01;
            A_minus = 0.01;
            w_initial = 0.5;
            tau_plus_corr = 20;
            tau_minus_corr = 20;
            W_OUTPUT_PERIOD = 500;


            //topology parameters
            lambda = 0.01; //characteristic length of a connection within an exponential distribution in spatiadouble lly-dependent topology
            tau_delay = 0.2; //base axonal delay
            SPIKE_SPEED = 0.2; //speed of spike propagating along the axon. All connections are considered straight lines.
            max_conn_length = 1.4142; //maximum length of a connection in spatially-dependent topology

            P_CON = 0.1; //probability of connecting two neurons in case of BINOMIAL topology

            //Homeostasis parameters
            HOMEOSTASIS_status = 0;
            b = 1 / (double)N;
            M = 1;
            M_max = 1;

            //stim disabling protocol
            stim_disable_protocol = 0; //(1 for turning stimulation off when overcoming the activity threshold
                                       // 2 for turning stimulation off at a certain time)
            pb_number = 2;            //number of burst to which the stimulation disabling protocol is applied
            activity_threshold = 0.2; //activity threshold of the pb_number-th burst, at which the stimulation disabling protocol is applied
            stim_off_time = 1400; //(ms - moment when to turn off the stimulation in case stim_disable_protocol = 2)

            inh_off_time = -1; //ms - moment in time when to set membrane potential V to V_rest until the simulation end. If set to -1 - mechanism is turend OFF

            //simulation parameters
            AVG_TIME = 2; //ms, activity averaging time
            SIM_TIME = 3000; //ms, simulation duration
            burst_threshold = 0.1; //activity level for detecting a burst
        }

        private static void ParseInputParameters(Dictionary<string, object> pars)
        {
            NumberFormatInfo provider = new NumberFormatInfo();
            provider.NumberDecimalSeparator = ".";

            foreach (var I in pars)
            {
                switch (I.Key)
                {
                    case "N": N = Convert.ToInt32(I.Value); break;
                    case "dt": dt = Convert.ToDouble(I.Value, provider); break;
                    
                    case "Inh_neurons_fraction": INH_NEURONS_FRACTION = Convert.ToDouble(I.Value, provider); break;

                    case "neuron_model":
                        {
                            int tmp = Convert.ToInt32(I.Value);
                            switch (tmp)
                            {
                                case 0: NEURON_MODEL = NeuronModel.PERFECT_INTEGRATE_AND_FIRE; break;
                                case 1: NEURON_MODEL = NeuronModel.LEAKY_INTEGRATE_AND_FIRE; break;
                                default: throw new Exception("Error! Wrong NeuronModel value in input.txt");
                            }
                            break;
                        }

                    case "TAU_M": TAU_M = Convert.ToDouble(I.Value, provider); break;
                    case "R_IN": R_IN = Convert.ToDouble(I.Value, provider); break;
                    case "V_REST": V_REST = Convert.ToDouble(I.Value, provider); break;
                    case "V_TH": V_TH = Convert.ToDouble(I.Value, provider); break;
                    case "V_RESET": V_RESET = Convert.ToDouble(I.Value, provider); break;

                    case "spatial_layer_type":
                        {
                            int tmp = Convert.ToInt32(I.Value);
                            switch (tmp)
                            {
                                case 0: layer_type = LayerType.BINOMIAL; break;
                                case 1: layer_type = LayerType.UNIFORM; break;
                                case 2: layer_type = LayerType.SQUARE_LATTICE; break;
                                case 3: layer_type = LayerType.STRIPED; break;
                                case 4: layer_type = LayerType.BELL; break;
                                case 5: layer_type = LayerType.RAMP; break;
                                case 6: layer_type = LayerType.DOUBLE_RAMP; break;
                                case 7: layer_type = LayerType.BARBELL; break;
                                default: throw new Exception("Error! Wrong layer_type value in input.txt file");
                            }
                            break;
                        }

                    case "C_M": C_M = Convert.ToDouble(I.Value, provider); break;

                    case "Aee": Avg_A[1][1] = Convert.ToDouble(I.Value, provider); break;
                    case "Aei": Avg_A[1][0] = Convert.ToDouble(I.Value, provider); break;
                    case "Aie": Avg_A[0][1] = Convert.ToDouble(I.Value, provider); break;
                    case "Aii": Avg_A[0][0] = Convert.ToDouble(I.Value, provider); break;

                    case "tau_I": TAU_I = Convert.ToDouble(I.Value, provider); break;

                    case "I_bg_mean": I_BG_MEAN = Convert.ToDouble(I.Value, provider); break;
                    case "I_bg_sd": I_BG_SD = Convert.ToDouble(I.Value, provider); break;
                    case "I_bg_min": I_BG_MIN = Convert.ToDouble(I.Value, provider); break;
                    case "I_bg_max": I_BG_MAX = Convert.ToDouble(I.Value, provider); break;

                    case "I_bg_1": I_BG_1 = Convert.ToDouble(I.Value, provider); break;
                    case "I_bg_2": I_BG_2 = Convert.ToDouble(I.Value, provider); break;
                    case "Fraction_of_neurons_with_I_bg_1": FRACTION_OF_NEURONS_WITH_I_BG_1 = Convert.ToDouble(I.Value, provider); break;

                    case "I_bg_noise_mode": BG_CURRENT_NOISE_MODE = Convert.ToInt32(I.Value); break;
                    case "I_bg_noise_sd": I_BG_NOISE_SD = Convert.ToDouble(I.Value, provider); break;

                    case "P_sp_mean": P_SP_MEAN = Convert.ToDouble(I.Value, provider); break;
                    case "P_sp_sd": P_SP_SD = Convert.ToDouble(I.Value, provider); break;
                    case "P_sp_min": P_SP_MIN = Convert.ToDouble(I.Value, provider); break;
                    case "P_sp_max": P_SP_MAX = Convert.ToDouble(I.Value, provider); break;

                    case "P_sp_1": P_SP_1 = Convert.ToDouble(I.Value, provider); break;
                    case "P_sp_2": P_SP_2 = Convert.ToDouble(I.Value, provider); break;
                    case "Fraction_of_neurons_with_P_sp_1": FRACTION_OF_NEURONS_WITH_P_SP_1 = Convert.ToDouble(I.Value, provider); break;

                    case "STDP_status":
                        {
                            int val = Convert.ToInt32(I.Value);
                            switch (val)
                            {
                                case 0: STDP_status = STDP.STDP_IS_OFF; break;
                                case 1: STDP_status = STDP.STDP_IS_MULTIPLICATIVE; break;
                                case 2: STDP_status = STDP.STDP_IS_ADDITIVE; break;
                                default: throw new Exception("STDP_Status - wrong value in input.txt");
                            }
                            break;
                        }

                    case "A_plus": A_plus = Convert.ToDouble(I.Value, provider); break;
                    case "A_minus": A_minus = Convert.ToDouble(I.Value, provider); break;
                    case "w_initial": w_initial = Convert.ToDouble(I.Value, provider); break;
                    case "tau_plus_corr": tau_plus_corr = Convert.ToDouble(I.Value, provider); break;
                    case "tau_minus_corr": tau_minus_corr = Convert.ToDouble(I.Value, provider); break;
                    case "W_OUTPUT_PERIOD": W_OUTPUT_PERIOD = Convert.ToDouble(I.Value, provider); break;
                    //-------

                    case "lambda": lambda = Convert.ToDouble(I.Value, provider); break;
                    case "tau_delay": tau_delay = Convert.ToDouble(I.Value, provider); break;
                    case "spike_speed": SPIKE_SPEED = Convert.ToDouble(I.Value, provider); break;
                    case "max_conn_length": max_conn_length = Convert.ToDouble(I.Value, provider); break;

                    case "P_con": P_CON = Convert.ToDouble(I.Value, provider); break;
                    case "force_binomial_topology": FORCE_BINOMIAL_TOPOLOGY = Convert.ToInt32(I.Value); break;

                    case "Homeostasis_status": HOMEOSTASIS_status = Convert.ToInt32(I.Value); break;
                    case "b": b = Convert.ToDouble(I.Value, provider); break;
                    case "M_max": M_max = Convert.ToDouble(I.Value, provider); break;


                    case "stim_disable_protocol": stim_disable_protocol = Convert.ToInt32(I.Value); break;
                    case "pb_number": pb_number = Convert.ToInt32(I.Value); break;
                    case "activity_threshold": activity_threshold = Convert.ToDouble(I.Value, provider); break;
                    case "stim_off_time": stim_off_time = Convert.ToDouble(I.Value, provider); break;

                    case "inh_off_time": inh_off_time = Convert.ToDouble(I.Value, provider); break;


                    case "AVG_TIME": AVG_TIME = Convert.ToDouble(I.Value, provider); break;
                    case "SIM_TIME": SIM_TIME = Convert.ToInt32(I.Value); break;
                    case "burst_threshold": burst_threshold = Convert.ToDouble(I.Value, provider); break;





                    case "Uee": Avg_U[1][1] = Convert.ToDouble(I.Value, provider); break;
                    case "Uei": Avg_U[1][0] = Convert.ToDouble(I.Value, provider); break;
                    case "Uie": Avg_U[0][1] = Convert.ToDouble(I.Value, provider); break;
                    case "Uii": Avg_U[0][0] = Convert.ToDouble(I.Value, provider); break;

                    case "tau_rec_ee": Avg_tau_rec[1][1] = Convert.ToDouble(I.Value, provider); break;
                    case "tau_rec_ei": Avg_tau_rec[1][0] = Convert.ToDouble(I.Value, provider); break;
                    case "tau_rec_ie": Avg_tau_rec[0][1] = Convert.ToDouble(I.Value, provider); break;
                    case "tau_rec_ii": Avg_tau_rec[0][0] = Convert.ToDouble(I.Value, provider); break;

                    case "tau_facil_ee": Avg_tau_facil[1][1] = Convert.ToDouble(I.Value, provider); break;
                    case "tau_facil_ei": Avg_tau_facil[1][0] = Convert.ToDouble(I.Value, provider); break;
                    case "tau_facil_ie": Avg_tau_facil[0][1] = Convert.ToDouble(I.Value, provider); break;
                    case "tau_facil_ii": Avg_tau_facil[0][0] = Convert.ToDouble(I.Value, provider); break;

                    case "Stimulation_type":
                        {
                            int value = Convert.ToInt32(I.Value);
                            switch (value)
                            {
                                case 1: STIMULATION_TYPE = StimType.STIM_TYPE_I_BG_GAUSSIAN; break;
                                case 2: STIMULATION_TYPE = StimType.STIM_TYPE_I_BG_TWO_VALUES; break;
                                case 3: STIMULATION_TYPE = StimType.STIM_TYPE_P_SP_GAUSSIAN; break;
                                case 4: STIMULATION_TYPE = StimType.STIM_TYPE_P_SP_TWO_VALUES; break;
                            };
                            break;
                        }
                        
                    case "use_saved_topology":
                        {
                            switch (Convert.ToInt32(I.Value))
                            {
                                case 0: USE_SAVED_TOPOLOGY = Topology.GENERATE_TOPOLOGY; break;
                                case 1: USE_SAVED_TOPOLOGY = Topology.READ_TOPOLOGY_FROM_FILE; break;
                                default: throw new Exception("Error! Wrong topology value in input.txt");
                            }
                            break;
                        }
                    
                    case "use_saved_stimulation_data":
                        {
                            switch (Convert.ToInt32(I.Value))
                            {
                                case 0: USE_SAVED_STIMULATION_DATA = StimulationData.GENERATE_STIMULATION_DATA; break;
                                case 1: USE_SAVED_STIMULATION_DATA = StimulationData.READ_STIMULATION_DATA_FROM_FILE; break;
                                default: throw new Exception("Error! Wrong topoly value in input.txt");
                            }
                            break;
                        }
                   
                    case "use_saved_synaptic_parameters":
                        {
                            switch (Convert.ToInt32(I.Value))
                            {
                                case 0: USE_SAVED_SYNAPTIC_PARAMETERS = SynapticData.GENERATE_SYNAPTIC_DATA; break;
                                case 1: USE_SAVED_SYNAPTIC_PARAMETERS = SynapticData.READ_SYNAPTIC_DATA_FROM_FILE; break;
                                default: throw new Exception("Error! Wrong synaptic data value in input.txt");
                            }
                            break;
                        }

                    case "use_saved_coordinates":
                        {
                            switch (Convert.ToInt32(I.Value))
                            {
                                case 0: USE_SAVED_COORDINATES = Coordinates.GENERATE_COORDINATES; break;
                                case 1: USE_SAVED_COORDINATES = Coordinates.READ_COORDINATES_FROM_FILE; break;
                                default: throw new Exception("Error! Wrong coordinates data value in input.txt");
                            }
                            break;
                        }
                }
            }
        }
        }
}
