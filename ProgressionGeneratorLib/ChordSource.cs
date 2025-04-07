using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ProgressionGeneratorLib
{
    public class ChordSource
    {
        public static readonly string[/*chord size*/][/*chord width*/][/*chord index*/] Chords = new string[][][]
        {
            new string[][] {
                new string[] { },
            },
            new string[][] {
                new string[] { },
                new string[] { "0" },
            },
            new string[][]
            {
                new string[] { },
                new string[] { "0 1" },
                new string[] { "0 2" },
                new string[] { "0 3" },
                new string[] { "0 4" },
                new string[] { "0 5" },
                new string[] { "0 6" },
            },
            new string[][] {
                new string[] { },
                new string[] { },

                new string[] {
                    "0 1 2"
                },

                new string[] {
                "0 1 3",
                "0 2 3",
                },

                new string[] {
                "0 1 4",
                "0 2 4",
                "0 3 4",
                },

                new string[] {
                "0 1 5",
                "0 2 5",
                "0 3 5",
                "0 4 5",
                },

                new string[] {
                "0 1 6",
                "0 2 6",
                "0 3 6",
                "0 4 6",
                "0 5 6",
                },

                new string[] {
                "0 2 7",
                "0 3 7",
                "0 4 7",
                },

                new string[] {
                "0 4 8",
                },
            },
            new string[][] {
                new string[] { },
                new string[] { },
                new string[] { },

                new string[] {
                "0 1 2 3",
                },

                new string[] {
                "0 1 2 4",
                "0 1 3 4",
                "0 2 3 4",
                },
                new string[] {
                "0 1 2 5",
                "0 1 3 5",
                "0 2 3 5",
                "0 2 4 5",
                "0 3 4 5",
                "0 1 4 5",
                },
                new string[] {
                "0 1 2 6",
                "0 1 3 6",
                "0 2 3 6",
                "0 2 4 6",
                "0 3 4 6",
                "0 3 5 6",
                "0 4 5 6",
                "0 1 4 6",
                "0 1 5 6",
                "0 2 5 6",
                },
                new string[] {
                "0 1 2 7",
                "0 1 3 7",
                "0 2 3 7",
                "0 2 4 7",
                "0 3 4 7",
                "0 3 5 7",
                "0 4 5 7",
                "0 4 6 7",
                "0 5 6 7",

                "0 1 4 7",
                "0 1 5 7",
                "0 2 5 7",
                "0 2 6 7",
                "0 3 6 7",
                "0 1 6 7",
                },
                new string[] {
                "0 2 4 8",
                "0 3 4 8",
                "0 3 5 8",
                "0 4 5 8",
                },
                new string[] {
                "0 4 5 9",
                "0 3 5 9",
                "0 3 6 9",
                "0 4 6 9",
                },
                new string[] {
                "0 4 6 10",
                },
            },
        };

        /// <summary>
        /// copied from TryBandMath.sln
        /// </summary>
        /// <param name="chordString">the chord</param>
        /// <param name="transposeFH">fifth height transposition</param>
        /// <param name="bandReferenceHeight">band reference height</param>
        /// <param name="spacing">chord spacing</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static List<ChordTones> GenerateChords(string chordString, int transposeFH, int bandReferenceHeight, ChordSpacing spacing)
        {
            //ChordTones.DumpHeader();
            //Debug.WriteLine($"  transposeFH {transposeFH} bandReferenceHeight {bandReferenceHeight}");

            var chordList = new List<ChordTones>();

            int[] indexes;
            switch (spacing)
            {
                case ChordSpacing.Closed2:
                case ChordSpacing.Closed22:
                    throw new NotImplementedException();
                case ChordSpacing.Closed232:
                    indexes = new int[] { 0, 2, 5, 7 };
                    break;
                case ChordSpacing.Open333:
                default:
                    throw new NotImplementedException();
            }
            for (var inversion = 0; inversion < 4; inversion++)
            {
                var stringToInts = chordString.StringToInts();
                var chord = new ChordTones(stringToInts, transposeFH);
                chordList.Add(chord);
                chord.Items = chord.Items.OrderBy(t => t.normalHeight).ToArray();
                var cloneTones = chord.Items;
                for (var i = 0; i < indexes.Length; i++)
                {
                    var idx = (indexes[i] + inversion) % 4;
                    var octave = (indexes[i] + inversion) / 4;
                    // spacedHeight
                    cloneTones[idx].spacedHeight = cloneTones[idx].normalHeight + 12 * octave;
                    // transposedSpacedHeight
                    cloneTones[idx].transposedSpacedHeight = cloneTones[idx].transposedNormalHeight + 12 * octave;
                    // index
                    cloneTones[idx].index = indexes[i] + inversion;
                }
                chord.Items = chord.Items.OrderBy(t => t.spacedHeight).ToArray();
                // Get the band
                var indexFirstFH = chord.IndexFirstFH;
                //
                int band;
                int modulus;
                int octaveOffset;
                ExtensionMethods.HeightToBand(chord.Items[indexFirstFH].transposedSpacedHeight, bandReferenceHeight, out band, out modulus, out octaveOffset);

                // indexBand
                var delta = band - chord.Items[indexFirstFH].index;
                for (var i = 0; i < chord.Items.Length; i++)
                {
                    chord.Items[i].indexBand = chord.Items[i].index + delta;
                }
                Debug.Assert(chord.Items[indexFirstFH].indexBand == band);

                // Height
                var quotient = ExtensionMethods.Quotient(chord.Items[0].indexBand, 4);
                for (var i = 0; i < chord.Items.Length; i++)
                {
                    chord.Items[i].Height = chord.Items[i].transposedSpacedHeight - 12 * quotient;
                }
                //chord.Dump();
                ExtensionMethods.HeightToBand(chord.Items[indexFirstFH].Height, bandReferenceHeight, out band, out modulus, out octaveOffset);
                //Debug.WriteLine($"  band({chord.Items[indexFirstFH].Height}) {band}");
                //
                //int delta_ = chord.Items[indexFirstFH].Height - (bandReferenceHeight + 3 * band);
                //if (!Deltas.ContainsKey(delta_))
                //    Deltas.Add(delta_, 0);
                //Deltas[delta_]++;
                //
            }
            chordList = chordList.OrderBy(c => c.Items[0].Height).ToList();

            return chordList;
        }
    }
}
