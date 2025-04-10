﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace HarmonicAnalysisLib
{
    public class VectorConverter
    {
        public static Vector<int> CreateVector(int x, int y, int z)
        {
            return new Vector<int>(new int[] { x, y, z, 0, 0, 0, 0, 0 });
        }

        ///<summary> ModulusOperator always returns a number between 0 and modulus-1 </summary>
        public static int ModulusOperator(int arg, int modulus)
        {
            return (arg < 0) ? ((arg % modulus) + modulus) % modulus : arg % modulus;
        }

        ///<summary> Quotient(*, 2) maps the series {-4, -3, -2, -1, 0, 1, 2, 3, 4} to {-2, -2, -1, -1, 0, 0, 1, 1, 2} </summary>
        public static int Quotient(int arg, int modulus)
        {
            return (arg < 0) ? (arg - modulus + 1) / modulus : arg / modulus;
        }

        public static int VectorToPitch(Vector<int> v)
        {
            return 12 * v[0] + 19 * v[1] + 28 * v[2];
        }

        public static int VectorToDistance(Vector<int> v)
        {
            return 12 * Math.Abs(v[0]) + 19 * Math.Abs(v[1]) + 28 * Math.Abs(v[2]);
        }

        public static Vector<int> PitchAndFifthToVector(int pitchHeight, int fifthHeight)
        {
            var z = 0;
            var y = fifthHeight;
            var x = (pitchHeight - 19 * y) / 12;
            Vector<int> vector = VectorConverter.CreateVector(x, y, z);
            if (VectorToPitch(vector) != pitchHeight)
            {
                return Vector<int>.AllBitsSet;
            }
            return vector;
        }

        public static Vector<int> HeightToNormalPosition(int pitchHeight)
        {
            int accidental = 0;
            var fifthHeight = HeightAndAccidentalToFifthHeight(pitchHeight, accidental);
            var vector = PitchAndFifthToVector(pitchHeight, fifthHeight);
            return vector + Normalize(vector);
        }

        // from HarmonicLaneAnalysis.Notes.xlsx
        static readonly (int height, int accidental, int fifthheight)[] Mapping = {
            new (1, 2, 19),
            new (6, 2, 18),
            new (11, 2, 17),
            new (4, 2, 16),
            new (9, 2, 15),
            new (2, 2, 14),
            new (7, 2, 13),
            new (0, 1, 12),
            new (5, 1, 11),
            new (10, 1, 10),
            new (3, 1, 9),
            new (8, 1, 8),
            new (1, 1, 7),
            new (6, 1, 6),
            new (11, 0, 5),
            new (4, 0, 4),
            new (9, 0, 3),
            new (2, 0, 2),
            new (7, 0, 1),
            new (0, 0, 0),
            new (5, 0, -1),
            new (10, -1, -2),
            new (3, -1, -3),
            new (8, -1, -4),
            new (1, -1, -5),
            new (6, -1, -6),
            new (11, -1, -7),
            new (4, -1, -8),
            new (9, -2, -9),
            new (2, -2, -10),
            new (7, -2, -11),
            new (0, -2, -12),
            new (5, -2, -13),
            new (10, -2, -14),
            new (3, -2, -15),
            };

        public static int HeightAndAccidentalToFifthHeight(int height, int accidental)
        {
            int pitchHeight = ModulusOperator(height, 12);
            if (!Mapping.Any(m => m.height == pitchHeight && m.accidental == accidental))
            {
                var x = Mapping.Skip(12).First();
                return Mapping.Skip(12).First(m => m.height == pitchHeight).fifthheight;
            }
            return Mapping.Single(m => m.height == pitchHeight && m.accidental == accidental).fifthheight;
        }

        public static int VectorToDeltaZ(Vector<int> source, Vector<int> target)
        {
            var delta = source - target;
            var alias = Aliases.Single(a => a == delta);
            var indexOf = Array.IndexOf(Aliases, alias);
            return AliasesOrigin - indexOf;
        }

        public static Vector<int> DeltaZToVector(Vector<int> source, int deltaZ)
        {
            var alias = Aliases[deltaZ + AliasesOrigin];
            return source + alias;
        }

        #region Aliases
        public static readonly Vector<int>[] Aliases =
        {
            new Vector<int>(new int[] { 14, 0, -6, 0, 0, 0, 0, 0 }),
            new Vector<int>(new int[] { 18, -4, -5, 0, 0, 0, 0, 0 }),
            new Vector<int>(new int[] { 3, 4, -4, 0, 0, 0, 0, 0 }),
            new Vector<int>(new int[] { 7, 0, -3, 0, 0, 0, 0, 0 }),
            new Vector<int>(new int[] { 11, -4, -2, 0, 0, 0, 0, 0 }),
            new Vector<int>(new int[] { -4, 4, -1, 0, 0, 0, 0, 0 }),
            new Vector<int>(new int[] { 0, 0, 0, 0, 0, 0, 0, 0 }),
            new Vector<int>(new int[] { 4, -4, 1, 0, 0, 0, 0, 0 }),
            new Vector<int>(new int[] { -11, 4, 2, 0, 0, 0, 0, 0 }),
            new Vector<int>(new int[] { -7, 0, 3, 0, 0, 0, 0, 0 }),
            new Vector<int>(new int[] { -3, -4, 4, 0, 0, 0, 0, 0 }),
            new Vector<int>(new int[] { -18, 4, 5, 0, 0, 0, 0, 0 }),
            new Vector<int>(new int[] { -14, 0, 6, 0, 0, 0, 0, 0 }),
        };
        public const int AliasesOrigin = 6;
        #endregion

        /// <summary>
        /// Normalize a position so z = 0
        /// </summary>
        /// <returns>normalized position</returns>
        public static Vector<int> NormalizeTo_z0(Vector<int> source)
        {
            return source + source[2] * Aliases[AliasesOrigin - 1];
        }
        public static Vector<int> Normalize(Vector<int> source)
        {
            return Normalize(source, Vector<int>.Zero);
        }
        public static bool IsNormalized(Vector<int> source)
        {
            return Normalize(source) == Vector<int>.Zero;
        }
        /// <summary>
        /// Normalize source position with respect to target position
        /// </summary>
        /// <remarks>Note: source + Normalize(source, target) is normalized with respect to target</remarks>
        /// <param name="source">source position</param>
        /// <param name="t">target position</param>
        /// <returns>offset</returns>
        public static Vector<int> Normalize(Vector<int> source, Vector<int> t)
        {
            var z1 = Aliases[AliasesOrigin - 1];
            var z3 = Aliases[AliasesOrigin + 3];
            var offset = Vector<int>.Zero;
            var s = source;
            var d = s - t;
            var quotientY = Quotient(d[1], 4);
            offset += -quotientY * z1;
            s += offset;
            d = s - t;
            var quotientZ = Quotient(d[2], 3);
            offset += -quotientZ * z3;
            s += -quotientZ * z3;
            d = s - t;
            return offset;
        }
        public static void DebugWriteVector(Vector<int> v)
        {
            Debug.Write($"{v[0],3} {v[1],2} {v[2],2} ");
        }
        public static string DebugFormatVector(Vector<int> v)
        {
            return $"{v[0],3} {v[1],2} {v[2],2}";
        }
        public static int Compare(Vector<int> x, Vector<int> y)
        {
            for (int i = 2; i >= 0; --i)
            {
                if (x[i] < y[i])
                {
                    return -1;
                }
                if (x[i] > y[i])
                {
                    return 1;
                }
            }
            return 0;
        }

        public static bool IsPositiveVector(Vector<int> delta)
        {
            return !(delta[2] < 0 || delta[2] == 0 & delta[1] < 0 || delta[2] == 0 & delta[1] == 0 && delta[0] < 0);
        }
        public static bool PositionClassEqual(Vector<int> x, Vector<int> y)
        {
            return x[1] == y[1] && x[2] == y[2];
        }
        /// <summary>
        /// Generate canonical fifth heights from a list of chord tone positions
        /// </summary>
        /// <remarks>correctly handles repeated chord tone positions</remarks>
        /// <param name="chords">chord tone positions</param>
        public static List<(int height, Vector<int> position, Vector<int> position_z0)> CanonicalFifthHeightOrder(List<(int height, Vector<int> position, Vector<int> position_z0)> chords)
        {
            // order items
            var count = chords.Count;
            var list = chords.Select(t => (height: t.height, position: t.position, position_z0: NormalizeTo_z0(t.position))).ToList();
            list = list.OrderBy(t => t.position_z0[1]).ThenBy(t => t.position_z0[0]).ToList();
            var best_index = -1;
            var best_sum = -1;
            for (var i = 0; i < count; ++i)
            {
                var sum = 0;
                for (var j = 0; j < count; ++j)
                {
                    var index0 = ModulusOperator(i + j, count);
                    var index1 = ModulusOperator(i + j + 1, count);
                    var diff = ModulusOperator(list[index1].position_z0[1] - list[index0].position_z0[1], 12);
                    sum += diff * (int)Math.Pow(10, j);
                }
                if (best_index == -1 || sum > best_sum)
                {
                    best_index = i;
                    best_sum = sum;
                };
            }
            list = Enumerable.Range(best_index, count).Select(i => list[ModulusOperator(i, count)]).ToList();
            // make y monotone increasing
            for (var i = 0; i < count - 1; ++i)
            {
                while (list[i + 1].position_z0[0] < list[i].position_z0[1])
                {
                    var position_z0 = list[i + 1].position_z0 + CreateVector(-19, 12, 0);
                    list[i + 1] = (list[i + 1].height, list[i + 1].position, position_z0);
                }
            }
            return list;
        }
    }
}
