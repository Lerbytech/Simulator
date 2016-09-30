using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;



namespace NeuroSim_TM
{

    class Program
    {
        static void Main(string[] args)
      {

            CController Kernel = new CController(@"C:\Users\Admin\Desktop\NeurSim-TM\input.txt");
           
            l1:
            string[] S = Kernel.GetModeDescription();
            for (int i = 0; i < S.Length; i++) Console.WriteLine(S[i]);

            Console.WriteLine("Start simulation?> (y/n)");
            string answer = Console.ReadLine();
            if (String.Compare(answer, "y") == 0) Kernel.StartSimulation();
            else if (String.Compare(answer, "n") == 0) { Console.WriteLine("Shutting down..."); return; }
                 else { Console.Clear(); goto l1; }



        }
    }
}
