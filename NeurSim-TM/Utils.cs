using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroSim_TM
{
    public static class Utils
    {
        //****Function for generating a gaussian distribution

        /*
        1 - развернуть цикл
        2 - с самого начала установить s = - 6;
        3 - вместо трех подсчетов mena + s*sd сделать сохранение в переменную.
        4 - протестировать упрощение условий (дерево)
        5 - избавиться от 12 повторов (серьезно, зачем?)
        **/
        public static double gauss(double mean, double sd, double min, double max)
        {
            int i; //number of iterations in procedure

            double s;
            Random r = new Random();
            while (true)
            {
                s = 0;
                for (i = 0; i < 12; i++)
                {
                    s += r.NextDouble();
                    //s += ((double)rand() / (RAND_MAX));
                }
                s = s - 6;
                if (((mean + s * sd) > min) && ((mean + s * sd) < max))
                    return my_round(mean + s * sd);
            }
        }
        


        /*
         1 - сравнить с исходным условием
         2 - попробовать проверять на бит, отвечающий за знак
         3 - найти встроенный метод, сравнить с ним
         4 - трюки с математикой?
        */
        public static double my_theta(double X)
        {
            return (X > 0) ? 1 : 0;
            /*
            if (X > 0) return 1;
            else return 0;
            */
        }


        //****STDP rules, MULTIPLICATIVE or ADDITIVE
        /*
         1 - вставить метку для уменьшения числа проверок
         2 - если STDP Не меняется в процессе работы и эта функция используется в симуляции, заменить на делегат чтобы убрать лишнюю проверку. всегда можно вернуть если что
         3 - проверить величины delta-t / tau_plus_corr. быть может, экспоненту можно заменить на асимптоту
         4 - в конце заменить w+result на переменную, чтобы не суммировать дважды. может быть, упростить выражение или придумать замену. 
        */

        public static double dw(double w, double delta_t, STDP STDP_status, double A_plus, double A_minus, double tau_plus_corr, double tau_minus_corr)
        {
            double result = 0;
            if (STDP_status == STDP.STDP_IS_MULTIPLICATIVE)
            {
                if (delta_t > 0)
                    result = A_plus * (1 - w) * Math.Exp(-(delta_t) / tau_plus_corr);
                if (delta_t < 0)
                    result = -A_minus * w * Math.Exp((delta_t) / tau_minus_corr);
            }
            else if (STDP_status == STDP.STDP_IS_ADDITIVE)
            {
                if (delta_t > 0)
                    result = A_plus * Math.Exp(-(delta_t) / tau_plus_corr);
                if (delta_t < 0)
                    result = -A_minus * Math.Exp((delta_t) / tau_minus_corr);
            }
            if (w + result < 0.000001)   //boundary correction
                result = -w;
            else if (w + result > 1)
                result = 1 - w;
            return result;
        }

      public static double my_round(double input)
      {
        double tmp = Math.Truncate(input);
        int digits_n = (int)Math.Ceiling(Math.Log10(tmp));
        double res = Math.Round(input, 6 - digits_n);

        return res;
      }




        


}
}
