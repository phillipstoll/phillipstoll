using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonicAnalysisLib;
using HarmonicAnalysisCommonLib;

namespace ProgressionGeneratorUI;

/// <summary>
/// Represents all chord pairs
/// </summary>
/// <remarks> A chord pair includes 4 inversions and 2 pitch height parts
/// </remarks>
public class CrossPair
{
    public AlternatePairList[][] Items /* [chordI][chordJ] */;

    public CrossPair()
    {
        Items = new AlternatePairList[0][];
    }
    public CrossPair(ObservableCollection<ChordViewModel> chordParams)
    {
        Items /* [chordI][chordJ] */ = Enumerable.Range(0, chordParams.Count).Select(i => new AlternatePairList[chordParams.Count]).ToArray();
        // create AlternatePairList
        // for each chord pair
        for (var chordI = 0; chordI < chordParams.Count - 1; chordI++)
        {
            for (var chordJ = chordI + 1; chordJ < chordParams.Count; chordJ++)
            {
                var countI = chordParams[chordI].OctavePlacements.Count / 2;
                var countJ = chordParams[chordJ].OctavePlacements.Count / 2;
                // handle triad and tetrad combination
                var alternatePairList = new AlternatePairList(Math.Max(countI, countJ));
                Items[chordI][chordJ] = alternatePairList;
                // initialize AlternatePair
                // inversion pairs
                for (var inversionI = 0; inversionI < countI; inversionI++)
                {
                    var bestOctaves = new List<(double weight, int[] delta, int inversion, int octave)>();
                    for (var inversionJ = 0; inversionJ < countJ; inversionJ++)
                    {
                        // create Pair
                        // find best octave offset
                        var octaves = new List<(double weight, int[] delta, int inversion, int octave)>();
                        for (var octave = -1; octave <= 1; octave++)
                        {
                            var delta = Enumerable.Range(0, countI).Select(i =>
                                - chordParams[chordI].OctavePlacements[inversionI * 2 + 1].Height.chordTones.Items[i].Height
                                + chordParams[chordJ].OctavePlacements[inversionJ * 2 + 1].Height.chordTones.Items[i].Height
                                + 12 * octave).ToArray();
                            octaves.Add(new (WeightedSum(delta), delta, inversionJ, octave));
                        }
                        var best = octaves.OrderBy(i => i.weight).First();
                        bestOctaves.Add(best);
                    }
                    // find 2 best inversions
                    var list = bestOctaves.OrderBy(i => i.weight).Take(AlternatePair.PairCount).ToList();
                    var pairs = new OctavePlacementPair[AlternatePair.PairCount];
                    for (var n = 0; n < list.Count; n++)
                    {
                        var i = list[n];
                        var octavePlacment = new OctavePlacementViewModel
                        {
                             Harmonic = new HarmonicStruct(HarmonicStruct.NullDistance),
                             Height = new HeightStruct(i.delta),
                        };
                        var pair = new OctavePlacementPair(i.inversion, i.octave, i.weight, octavePlacment);
                        pairs[n] = pair;
                    }
                    // order pairs
                    if (pairs[0].inversionIndex + 1 == pairs[1].inversionIndex || pairs[0].inversionIndex + 1 == pairs[1].inversionIndex + 4)
                    {
                        alternatePairList.Items[inversionI].Pairs[0] = pairs[0];
                        alternatePairList.Items[inversionI].Pairs[1] = pairs[1];
                    }
                    else if (pairs[1].inversionIndex + 1 == pairs[0].inversionIndex || pairs[1].inversionIndex + 1 == pairs[0].inversionIndex + 4)
                    {
                        alternatePairList.Items[inversionI].Pairs[0] = pairs[1];
                        alternatePairList.Items[inversionI].Pairs[1] = pairs[0];
                    }
                    else
                    {
                        // should never be reached
                        alternatePairList.Items[inversionI].Pairs[0] = pairs[0];
                        alternatePairList.Items[inversionI].Pairs[1] = pairs[1];
                    }
                }
            }
        }
    }
    private double WeightedSum(int[] delta)
    {
        var orderBy = delta.Select(d => Math.Abs(d)).OrderBy(d => d).ToList();
        return Enumerable.Range(0, orderBy.Count).Sum(i => orderBy[i] * (double)DistanceAlgorithm._weights[orderBy.Count][i]);
    }
}
