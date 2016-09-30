using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace NeuroSim_TM
{
    public class CIOManager
    {
        //declaration of input\output files

        
        private string _InputFolderPath;
        public string InputFolderPath
        {
            private set
            {
                _InputFolderPath = value;
            }
            get { return _InputFolderPath;  }
        }

        private string _OutputFolderPath;
        public string OutputFolderPath
        {
            private set
            {
                _OutputFolderPath = value;
            }
            get { return _OutputFolderPath; }
        }
        private string PathTo_output_raster;

        private string PathTo_output_file_spiking_activity;
        private string PathTo_output_amount_of_active_connections;

        private string PathTo_output_x;
        private string PathTo_output_y;
        private string PathTo_output_z;
        private string PathTo_output_u;
        private string PathTo_output_M;

        private string PathTo_output_w;
        private string PathTo_output_w_initial;

        private string PathTo_exc_I_distribution;
        private string PathTo_inh_I_distribution;
        private string PathTo_p_sp_distribution;
        private string PathTo_synaptic_parameters_distribution;

        private string PathTo_output_connections;
        private string PathTo_output_connections_distribution;
        private string PathTo_output_coordinates;

        private string PathTo_output_info;

        private string PathTo_output_IBI;
        private string PathTo_output_burst_times;

        private string PathTo_input;
        private string PathTo_input_connections;
        private string PathTo_input_coordinates;
        private string PathTo_input_exc_I_distribution;
        private string PathTo_input_inh_I_distribution;
        private string PathTo_input_p_sp_distribution;
        private string PathTo_input_synaptic_parameters_distribution;
        

        StreamWriter output_raster;

        StreamWriter output_file_spiking_activity;
        StreamWriter output_amount_of_active_connections;

        StreamWriter output_x;
        StreamWriter output_y;
        StreamWriter output_z;
        StreamWriter output_u;
        StreamWriter output_M;

        StreamWriter output_w;
        StreamWriter output_w_initial;

        StreamWriter exc_I_distribution;
        StreamWriter inh_I_distribution;
        StreamWriter p_sp_distribution;
        StreamWriter synaptic_parameters_distribution;

        StreamWriter output_connections;
        StreamWriter output_connections_distribution;
        StreamWriter output_coordinates;

        StreamWriter output_info;

        StreamWriter output_IBI;
        StreamWriter output_burst_times;

        StreamReader input; // = fopen("input.txt", "r");
        StreamReader input_connections;
        StreamReader input_coordinates;
        StreamReader input_exc_I_distribution;
        StreamReader input_inh_I_distribution;
        StreamReader input_p_sp_distribution;
        StreamReader input_synaptic_parameters_distribution;
        
        public CIOManager(string path)
        {
          if (!File.Exists(path)) throw new Exception("Error! Input filed not found");

          InputFolderPath = path.Substring(0, path.LastIndexOf('\\'));


          OutputFolderPath = InputFolderPath;
          PathTo_input = path;
          //создать папку для вывода данных

        }


        public Dictionary<string, object> ReadInputFile()
        {
            if (!File.Exists(PathTo_input))
                return null;

            string[] lines = File.ReadAllLines(PathTo_input);
            if (lines.Length == 0) return null;

            Dictionary<string, object> result = new Dictionary<string, object>();
            
            int index_of_first_equal = 0;
            int index_of_first_space = 0;
            string key;
            string val;
            char[] equal_sign = new char[1]; equal_sign[0] = '=';
            char[] space_sign = new char[1]; space_sign[0] = ' ';

            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Length == 0) continue;

                //index_of_first_equal = lines[i].IndexOfAny(new char[] { '=' });
                index_of_first_equal = lines[i].IndexOfAny(equal_sign);
                if (index_of_first_equal <= 0) continue;

                key = lines[i].Substring(0, index_of_first_equal);

                //index_of_first_space = lines[i].IndexOfAny(new char[] { ' ' }, index_of_first_equal);
                index_of_first_space = lines[i].IndexOfAny(space_sign, index_of_first_equal);
                if (index_of_first_space > index_of_first_equal)
                    val = lines[i].Substring(index_of_first_equal + 1, index_of_first_space - index_of_first_equal).Trim();
                else val = lines[i].Substring(index_of_first_equal + 1).Trim();
                
                
                result.Add(key, val);
            }
            
            return result;
        }

        public void PrepareIOFiles()
        {
          output_raster = new StreamWriter(OutputFolderPath + "raster.txt");
          output_raster.Write("aa");
          output_raster.Close();
          output_file_spiking_activity = new StreamWriter("activity.txt");

          output_IBI = new StreamWriter("IBI.txt");
          output_burst_times = new StreamWriter("burst_times.txt");

          output_amount_of_active_connections = new StreamWriter("active_connections.txt");

          Directory.CreateDirectory(OutputFolderPath + "syn_resources_dynamics");
          output_x = new StreamWriter("syn_resources_dynamics/x.txt");
          output_y = new StreamWriter("syn_resources_dynamics/y.txt");
          output_z = new StreamWriter("syn_resources_dynamics/z.txt");
          output_u = new StreamWriter("syn_resources_dynamics/u.txt");

          output_w_initial = new StreamWriter("w_initital.txt");

          output_M = new StreamWriter("M.txt");


          if (Val.STDP_status != STDP.STDP_IS_OFF){
            Directory.CreateDirectory(OutputFolderPath + "stdp_w_dynamics");
            
          if (Val.layer_type == LayerType.BARBELL){
            Directory.CreateDirectory(OutputFolderPath + "stdp_w_dynamics");
            Directory.CreateDirectory(OutputFolderPath + "stdp_w_dynamics\\R");
            Directory.CreateDirectory(OutputFolderPath + "stdp_w_dynamics\\L");
          }
    }

          if (Val.STIMULATION_TYPE == StimType.STIM_TYPE_I_BG_GAUSSIAN || Val.STIMULATION_TYPE == StimType.STIM_TYPE_I_BG_TWO_VALUES)
          {
            exc_I_distribution = new StreamWriter("exc_I_distribution.txt");
            inh_I_distribution = new StreamWriter("inh_I_distribution.txt");
          } else
            p_sp_distribution = new StreamWriter("p_sp_distribution.txt");

            output_connections = new StreamWriter("connections.txt");
            output_connections_distribution = new StreamWriter("connections_per_neuron.txt");
            synaptic_parameters_distribution = new StreamWriter("synaptic_parameters_distribution.txt");
            output_coordinates = new StreamWriter("coordinates.txt");


            if (output_raster == null || output_file_spiking_activity == null)
                throw new Exception("ERROR: Failed to open raster or activity output file!");

            if (output_coordinates == null)
                throw new Exception("ERROR: Failed to open coordinates output file!");
              
            if (Val.USE_SAVED_COORDINATES == Coordinates.READ_COORDINATES_FROM_FILE)
            {
              if (! File.Exists("saved_coordinates.txt")) 
                throw new Exception("ERROR: Failed to open saved_coordinates.txt!");
              else input_coordinates =  new StreamReader("saved_coordinates.txt");
            }

          
        }

        public void ExportToFile(string filename, object value)
        {

        }

    }
}
