using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
//using System.Windows.Media.Media3D;

namespace ProgressionGeneratorLib
{
    public static class ExtensionMethods
    {
        public static int[] StringToInts(this string str)
        {
            return str.Split().Select(s => int.Parse(s)).ToArray();
        }

        ///<summary> ModulusOperator always returns a number between 0 and modulus-1 </summary>
        public static int ModulusOperator(int arg, int modulus)
        {
            return arg < 0 ? (arg % modulus + modulus) % modulus : arg % modulus;
        }

        ///<summary> Quotient(*, 2) maps the series {-4, -3, -2, -1, 0, 1, 2, 3, 4} to {-2, -2, -1, -1, 0, 0, 1, 1, 2} </summary>
        public static int Quotient(int arg, int modulus)
        {
            return arg < 0 ? (arg - modulus + 1) / modulus : arg / modulus;
        }

        public static string Base10To12(int value)
        {
            // input  -13 -12 -11 -10 -9 -8 -7 -6 -5 -4 -3 -2 -1 0 1 2 3 4 5 6 7 8 9 10 11 12 13
            // output -11 -10  -B  -A -9 -8 -7 -6 -5 -4 -3 -2 -1 0 1 2 3 4 5 6 7 8 9  A  B 10 11
            return (value < 0 ? "-" : string.Empty) +
                (Math.Abs(value % 12) + 16 * Math.Abs(value / 12)).ToString("X1");
        }

        /// <summary>
        /// Convert pitch height to 1/3 octave band and modulus
        /// </summary>
        /// <param name="height">pitch height</param>
        /// <param name="bandReferenceHeight">center pitch height of band number 0</param>
        /// <param name="band">1/3 octave band</param>
        /// <param name="modulus">modulus</param>
        /// <param name="octaveOffset">add octave offet to pitch height to normalize the pitch height to band number 0, 1, 2, 3</param>
        public static void HeightToBand(int height, int bandReferenceHeight, out int band, out int modulus, out int octaveOffset)
        {
            band = Quotient(height - bandReferenceHeight + 1, 3);
            modulus = ModulusOperator(height - bandReferenceHeight + 1, 3);
            octaveOffset = Quotient(band, 4);
        }
    }
}
