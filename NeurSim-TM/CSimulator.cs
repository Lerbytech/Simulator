using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroSim_TM
{
  public class CSimulator
  {
    double averaging_timer;
    double synapse_resource_timer;
    int N_SP;
    int N_ACTIVE_CONN;
    double avg_x;
    double avg_y;
    double avg_z;
    double avg_u;
    double last_burst_time;
    int burst_flag;
    double burst_counter;
    double avg_activity;
    double burst_detection_timer;
    double M;

    public CSimulator(CNeuron[] Cells)
    {
      averaging_timer = Val.AVG_TIME;
      synapse_resource_timer = Val.dt;
      N_SP = 0;
      N_ACTIVE_CONN = 0;
      avg_x = 0;
      avg_y = 0;
      avg_z = 0;
      avg_u = 0;

      last_burst_time = 0;
      burst_flag = 0;
      burst_counter = 0;
      avg_activity = 0;
      burst_detection_timer = 0;

      if (Val.HOMEOSTASIS_status == (int)Homeostasis.HOMEOSTASIS_IS_ON)
        M = Val.M_max;
      else M = 1;

      for (int i = 0; i < Cells.Length; i++)
        Cells[i].I_b_init = Cells[i].I_b;

    }

    public void ProcessNeuron(CNeuron C)
      {
        C.I_b = C.I_b_init * M;
        if (Val.BG_CURRENT_NOISE_MODE == 1) C.I_b = Utils.gauss(0, Val.I_BG_NOISE_SD, -C.I_b, 1000 * C.I_b);

        for (int i = 0; i < C.in_conn.Count; i++)
        {
          stepSynapse(C.in_conn[i], Val.dt);
          if (C.in_conn[i].timers.Count != 0)
          {


          }
        }
      }


    ////****T_M model synapse step
    public void stepSynapse(CSynapse syn, double dt)
    {
      double old_y, old_z, old_u;

      old_y = syn.y;
      old_z = syn.z;
      old_u = syn.u;

      syn.x += dt * old_z / syn.tau_rec;
      syn.y -= dt * old_y / syn.tau_I; /// почему минус?!
      syn.z += dt * (old_y / syn.tau_I - old_z / syn.tau_rec);
      //для syn.z если раскрыть скобки, то можно сэкономить подсчеты, взяв от предыдущих величин значение.

      if (syn.tau_facil == 0) syn.u = syn.w;
      else syn.u -= dt * old_u / syn.tau_facil;

      syn.I = syn.A * syn.y;

      // двойная проверка, избыточно
      if (syn.x >= 1) syn.x = 0.9999999; if (syn.x <= 0) syn.x = 0.0000001;//correction
      if (syn.y >= 1) syn.y = 0.9999999; if (syn.y <= 0) syn.y = 0.0000001;
      if (syn.z >= 1) syn.z = 0.9999999; if (syn.z <= 0) syn.z = 0.0000001;
      if (syn.u >= 1) syn.u = 0.9999999; if (syn.u <= 0) syn.u = 0.0000001;
    }


    ////****LIF model step
    //public double stepV_LIF(CNeuron n, double dt, double R_IN, double TAU_M)
    //{
    //    double I_syn = 0;
    //     Sinc_synapse* tmp_input_synapse = n.in_conn;

    //    //a sum of all incoming synaptic currents
    //    while (tmp_input_synapse.syn_pointer != NULL)
    //    {
    //        I_syn += tmp_input_synapse.syn_pointer.I;
    //        tmp_input_synapse = tmp_input_synapse.next;
    //    }

    //    return (n.V + dt * (-n.V + (I_syn + n.I_b) * R_IN) / TAU_M);

    //}


    ////****PIF model step
    //public double stepV_PIF(CNeuron n, double dt, double C_M)
    //{
    //    double I_syn = 0;
    //    inc_synapse* tmp_input_synapse = n.in_conn;

    //    //a sum of all incoming synaptic currents
    //    while (tmp_input_synapse.syn_pointer != NULL)
    //    {
    //        I_syn += tmp_input_synapse.syn_pointer.I;
    //        tmp_input_synapse = tmp_input_synapse.next;
    //    }

    //    return (n.V + dt * (I_syn + n.I_b) / C_M);
    //}

    ////****Homeostasis model step
    ///*избавиться от минуса. сравнить
    //использовать трюк с поэтапными скобками
    //*/
    //double stepM(double M, double M_max, double TAU_M, double dt)
    //{
    //    return dt * (-(M - M_max) / TAU_M);
    //}

  }
}