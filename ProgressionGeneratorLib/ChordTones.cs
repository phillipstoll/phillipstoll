using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ProgressionGeneratorLib
{
    // TODO: code smell: class ChordTones is similar to class Chord
    public class ChordTones
    {
        private Tone[] _tones;
        public Tone[] Items { get { return _tones; } set { _tones = value; } }
        public string ShortString => string.Join(' ', Items.OrderBy(t => t.Height).Select(t => ExtensionMethods.Base10To12(t.Height)));

        public ChordTones(int[] fifthHeights, int transposeFH)
        {
            Items = fifthHeights.Select(fh => new Tone { FH = fh }).ToArray();
            Debug.Assert(Items[0].FH == 0);

            var transposeHeight = ExtensionMethods.ModulusOperator(transposeFH * 19, 12);
            foreach (var tone in Items)
            {
                // transposedFH
                tone.transposedFH = tone.FH + transposeFH;
                // normalHeight
                tone.normalHeight = tone.FH * 19 % 12;
                // transposedNormalHeight
                tone.transposedNormalHeight = transposeHeight + tone.normalHeight;
            }
            Debug.Assert(Items[0].transposedFH == transposeFH);
            Debug.Assert(Items[0].normalHeight == 0);
            Debug.Assert(Items[0].transposedNormalHeight == transposeHeight);
        }
        public ChordTones()
        {

        }
        public int IndexFirstFH => Enumerable.Range(0, Items.Length).Single(i => Items[i].FH == 0);
        public override string ToString()
        {
            return string.Join(' ', Items.OrderBy(t => t.Height).Select(t => ExtensionMethods.Base10To12(t.Height)));
        }
    }
}
