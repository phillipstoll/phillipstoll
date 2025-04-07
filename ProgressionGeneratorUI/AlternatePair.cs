using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ProgressionGeneratorUI;

/// <summary>
/// Represents alternate octave placements of a pair of chords
/// </summary>
/// <remarks>includes 4 inversions and the 2 best voice leadings</remarks>
public class AlternatePairList : ObservableObject
{
    public List<AlternatePair> Items;

    public AlternatePairList(int itemCount)
    {
        Items = Enumerable.Range(0, itemCount).Select(i => new AlternatePair()).ToList();
    }
}
/// <summary>
/// Represents an octave placement of a pair of chords, including the 2 best voice leadings
/// </summary>
public class AlternatePair
{
    public const int PairCount = 2;
    public OctavePlacementPair[] Pairs = new OctavePlacementPair[PairCount];
    public void GetTopPairs()
    {
    }
    public static AlternatePair Create(ChordViewModel chord1, ChordViewModel chord2)
    {

        return new AlternatePair();
    }
    public override string ToString() => $"{Pairs[0]} : {Pairs[1]}";
}
