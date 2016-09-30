using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroSim_TM
{
    public class CNeuronManager
    {
      public CNeuron[] Neurons;
        
      private int INH_COUNT;
      private int EXC_COUNT;
            
      public CNeuronManager()
      {
          INH_COUNT = 0;
          EXC_COUNT = 0;
      }

      public void CreateNeurons()
      {
        CreateDummies();
        LoadTopologyData();
        LoadStimulationData();
        LoadCoordinates();
        LoadTopology();
        // 1119 - 1239  skipped
        EstablishConnections();


        if (Val.USE_SAVED_STIMULATION_DATA == StimulationData.READ_STIMULATION_DATA_FROM_FILE)
          throw new Exception("Not implemented yet bunch of shit!");

      }

      private void CreateDummies()
      {
        int N = Val.N;
        double v_rest = Val.V_REST;
        double v_reset = Val.V_RESET;

        Neurons = new CNeuron[N];
        for (int i = 0; i < N; i++)
        {
            Neurons[i] = new CNeuron();
            Neurons[i].id = i;
            Neurons[i].x = -1;
            Neurons[i].y = -1;
            Neurons[i].num_of_outcoming_connections = 0;
            Neurons[i].V = v_rest;
            Neurons[i].V_rest = v_rest;
            Neurons[i].V_reset = v_rest; ;
            Neurons[i].ref_time_left = 0;
            Neurons[i].id = i;
            Neurons[i].last_spiked_at = -100;
            Neurons[i].in_conn = new List<CSynapse>();
            Neurons[i].out_conn = new List<CSynapse>();
        }
      }

      private void LoadTopologyData()
      {

          if (Val.USE_SAVED_TOPOLOGY == Topology.GENERATE_TOPOLOGY)
          {
              Random R = new Random();
              Neurons[0].type = (int)Type.EXCITATORY;
              EXC_COUNT++;

              for (int i = 1; i < Neurons.Length; i++)
              {
                  if (R.NextDouble() >= Val.INH_NEURONS_FRACTION)
                  {
                      Neurons[i].type = (int)Type.EXCITATORY;
                      EXC_COUNT++;
                  }
                  else
                  {
                      Neurons[i].type = (int)Type.INHIBITORY;
                      INH_COUNT++;
                  }
              }
          }
          else
          {
              throw new Exception("NOT IMPLEMENTED YET!!!");
          }


      }

      private void LoadStimulationData()
      {
        if (Val.USE_SAVED_STIMULATION_DATA == StimulationData.GENERATE_STIMULATION_DATA)
        {
          for (int i = 0; i < Val.N; i++)
          {
            switch (Val.STIMULATION_TYPE)
            {
              case StimType.STIM_TYPE_I_BG_GAUSSIAN:
                {
                  Neurons[i].I_b = Utils.gauss(Val.I_BG_MEAN, Val.I_BG_SD, Val.I_BG_MIN, Val.I_BG_MAX);
                  /*
                  if (Neurons[i].type == (int)Type.EXCITATORY)
                    fprintf(exc_I_distribution, "%d %g\n", i, Neurons[i].I_b);
                  else
                    fprintf(inh_I_distribution, "%d %g\n", -i, Neurons[i].I_b);*/
                  break;
                }
              case StimType.STIM_TYPE_I_BG_TWO_VALUES:
                {
                  if (i < Val.FRACTION_OF_NEURONS_WITH_I_BG_1 * (double)Val.N)
                    Neurons[i].I_b = Val.I_BG_1;
                  else Neurons[i].I_b = Val.I_BG_2;
                  /*
                  if (Neurons[i].type == (int)Type.EXCITATORY)
                    fprintf(exc_I_distribution, "%d %g\n", i, Neurons[i].I_b);
                  else
                    fprintf(inh_I_distribution, "%d %g\n", -i, Neurons[i].I_b);
                   * */
                  break;
                }
              case StimType.STIM_TYPE_P_SP_GAUSSIAN:
                {
                  Neurons[i].I_b = 0;
                  Neurons[i].p_sp = Utils.gauss(Val.P_SP_MEAN, Val.P_SP_SD, Val.P_SP_MIN, Val.P_SP_MAX);
                  //fprintf(p_sp_distribution, "%d %g\n", i, Neurons[i].p_sp);
                  break;
                }
              case StimType.STIM_TYPE_P_SP_TWO_VALUES:
                {
                  Neurons[i].I_b = 0;
                  if (i < Val.FRACTION_OF_NEURONS_WITH_P_SP_1 * (double)Val.N)
                    Neurons[i].p_sp = Val.P_SP_1;
                  else Neurons[i].p_sp = Val.P_SP_2;
                  //fprintf(p_sp_distribution, "%d %g\n", i, Neurons[i].p_sp);
                  break;
                }
            }

            
          }
        }
        else throw new Exception("Not implemented yet!");
      }

      private void LoadCoordinates()
      {
        if (Val.USE_SAVED_COORDINATES == Coordinates.GENERATE_COORDINATES)
          GenerateCoordinates();
        else
        {
          throw new Exception("Not implemented yet!");
        }
      }

      private void GenerateCoordinates()
      {
        double u,v;
        double x,y;
        Random R = new Random();
        int list_i = 0;
        int random_i = 0;
        int N_rest = Val.N;

        int[] tmp_list = new int[Val.MAX_NUMBER_OF_NEURONS];
        for (int i = 0; i < Val.N; i++) tmp_list[i] = i;

        for (int i = 0; i < Val.N; i++)
        {
          switch (Val.layer_type)
          {

            case LayerType.BINOMIAL:
              {
                Neurons[i].x = 0;
                Neurons[i].y = 0;
                break;
              }

            case LayerType.UNIFORM:
              {
                Neurons[i].x = Utils.my_round(R.NextDouble());
                Neurons[i].y = Utils.my_round(R.NextDouble());
                break;
              }

            case LayerType.SQUARE_LATTICE:
              {
                list_i = (int)Math.Floor((double)N_rest * R.NextDouble());
                random_i = tmp_list[list_i];
                Neurons[i].y = Utils.my_round(1 / Math.Sqrt(Val.N) * Math.Floor(random_i / Math.Sqrt(Val.N)));
                Neurons[i].x = Utils.my_round((random_i - Neurons[i].y * Val.N) / Math.Sqrt(Val.N));
                if (Neurons[i].x < 0.0000001) x = 0;
                break;
              }

            case LayerType.STRIPED:
              {
                if (R.NextDouble() < 0.1)
                {
                  u = 0.4 * R.NextDouble();
                  if (u < 0.2) Neurons[i].x = u + 0.2;
                  else Neurons[i].x = u + 0.4;
                }
                else
                {
                  u = 0.6 * R.NextDouble();
                  if (u < 0.2) Neurons[i].x = u;
                  else if (u < 0.4) Neurons[i].x = u + 0.2;
                  else Neurons[i].x = u + 0.4;
                }

                Neurons[i].x = Utils.my_round(Neurons[i].x);
                Neurons[i].y = Utils.my_round(R.NextDouble());
                break;
              }

            case LayerType.BELL:
              {
                Neurons[i].x = Utils.gauss(0.5, 0.25, 0, 1);
                Neurons[i].y = Utils.gauss(0.5, 0.25, 0, 1);
                break;
              }

            case LayerType.RAMP:
              {
                while (true)
                {
                  u = R.NextDouble();
                  v = R.NextDouble();
                  if (u <= v) break;
                }

                Neurons[i].x = Utils.my_round(v);
                Neurons[i].y = Utils.my_round(R.NextDouble());

                break;
              }

            case LayerType.DOUBLE_RAMP:
              {
                while (true)
                {
                  u = R.NextDouble();
                  v = R.NextDouble();
                  if (u <= v) break;
                }

                Neurons[i].y = Utils.my_round(R.NextDouble());

                if (Neurons[i].y < 0.5) Neurons[i].x = v;
                else Neurons[i].x = 1 - v;

                Neurons[i].x = Utils.my_round(Neurons[i].x);

                break;

              }

            case LayerType.BARBELL:
              {
                if (i < 0.4 * Val.N)
                {
                  while (true)
                  {
                    u = R.NextDouble();
                    v = R.NextDouble();
                    if (Math.Sqrt(Math.Pow(u - 0.2, 2) + Math.Pow(v - 0.2, 2)) < 0.2 && u + v < 0.664)
                    {
                      Neurons[i].x = u;
                      Neurons[i].y = v;
                      break;
                    }
                  }
                }
                else if (i < 0.8 * Val.N)
                {
                  while (true)
                  {
                    u = R.NextDouble();
                    v = R.NextDouble();
                    if (Math.Sqrt(Math.Pow(u - 0.8, 2) + Math.Pow(v - 0.8, 2)) < 0.2 && u + v > 1.336)
                    {
                      Neurons[i].x = u;
                      Neurons[i].y = v;
                      break;
                    }
                  }
                }
                else
                {
                  u = R.NextDouble();
                  v = R.NextDouble();
                  Neurons[i].x = 0.282 + 0.1 * (3.36 * u + v);
                  Neurons[i].y = 0.382 + 0.1 * (3.36 * u - v);
                }
                Neurons[i].x = Utils.my_round(Neurons[i].x);
                Neurons[i].y = Utils.my_round(Neurons[i].y);
                break;
              }

          }

          for (int j = list_i; j < N_rest - 1; j++)
            tmp_list[j] = tmp_list[j + 1];
          N_rest--;
        }
      }

      private void LoadTopology()
      {
        if (Val.USE_SAVED_TOPOLOGY == Topology.READ_TOPOLOGY_FROM_FILE)
        {

        }
      }

      private void EstablishConnections()
      {
        CSynapse tmpSynapse = new CSynapse();

        int N_COUNT = 0;

        if (Val.USE_SAVED_TOPOLOGY == Topology.READ_TOPOLOGY_FROM_FILE) ;

        for (int i = 0; i < Val.N; i++)
        {
          tmpSynapse = Neurons[i].out_conn[0];
          int pre_syn_type = Neurons[i].type;
          int CONN_PER_NEURON_COUNT = 0;


          //looking over every other neuron in the network and checking whether we create a connection
        //(if we read topology from file we still check if given probability value tmp_prob satisfies our connection occurrence condidtion)
        for (j=0;j<N;j++){
          if (j != i && ((Val.USE_SAVED_TOPOLOGY == Topology.READ_TOPOLOGY_FROM_FILE && i == Math.Abs(Neurons[i].pre_num) && j == abs(post_num)) || (TOPOLOGY_LOAD_TYPE == GENERATE_TOPOLOGY))){










        }
      }
    }
}
