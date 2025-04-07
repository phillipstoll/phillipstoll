using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ProgressionGeneratorLib
{
    public class Tone : IEqualityComparer<Tone>
    {
        // use tones.OrderBy(t => t.FH), get rid of indexFH
        //public int indexFH { get; set; }
        // Chord[][][]
        public int FH { get; set; }
        // Chord[][][] + transposeFH
        public int transposedFH { get; set; }
        // for Tone[0] normalHeight is 0
        public int normalHeight { get; set; }
        public int transposedNormalHeight { get; set; }
        public int spacedHeight { get; set; }
        // Chord[][][].ToHeight()
        public int transposedSpacedHeight { get; set; }
        public int index { get; set; }
        public int indexBand { get; set; }
        public int Height { get; set; }
        // (x, y, 0)
        public Vector3 Vect { get; set; }
        // 0, 1, 2, 3
        public int Band { get; set; }
        //public override bool Equals(object? obj)
        //{
        //    return ((Tone)obj).Vect == this.Vect;
        //}
        public Tone Clone()
        {
            return new Tone
            {
                //indexFH = this.indexFH,
                FH = FH,
                transposedFH = transposedFH,
                normalHeight = normalHeight,
                transposedNormalHeight = transposedNormalHeight,
                spacedHeight = spacedHeight,
                transposedSpacedHeight = transposedSpacedHeight,
                index = index,
                indexBand = indexBand,
                Height = Height,
                Vect = new Vector3(Vect.X, Vect.Y, Vect.Z),
                Band = Band,
            };
        }

        public bool Equals(Tone? x, Tone? y)
        {
            return x.Vect.Equals(y.Vect);
        }

        public int GetHashCode([DisallowNull] Tone obj)
        {
            return Vect.X.GetHashCode() ^ Vect.Y.GetHashCode() ^ Vect.Z.GetHashCode();
        }
    }

}
