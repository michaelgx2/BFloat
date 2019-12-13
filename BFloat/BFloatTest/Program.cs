using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bfloat = BFloat;

namespace BFloatTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //bfloat b1 = 152300;
            //bfloat b2 = new BFloat(1.2411, 26+15);
            //bfloat b3 = 23523423423423234234568.0;
            //Console.WriteLine(
            //    $"{b1.ToString(BFloatStringFormat.UseLetters)}\n{b2.ToString(BFloatStringFormat.UseLetters)}\n{b3.ToString(BFloatStringFormat.UseLetters)}");
            bfloat b = 1.523;
            for (int i = 0; i < 1000; i++)
            {
                b *= 10;
                //Console.WriteLine(b.ToString(BFloatStringFormat.UseLetters));
                Console.WriteLine(b.ToString(BFloatStringFormat.ChineseAndLetters));
            }
            
            Console.ReadKey();
        }
    }
}
