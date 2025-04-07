using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgressionGeneratorUI;

/// <summary>
/// Represents an octave placement of a pair of chords, including 1 voice leadings
/// </summary>
public partial class OctavePlacementPair
{
    // index of the inversion
    public int inversionIndex;
    // currently not used
    public int octaveOffset;
    // currently not used
    public double weightedSum;
    public OctavePlacementViewModel octavePlacement = new();

    public OctavePlacementPair(int inversionIndex, int octaveOffset, double weightedSum, OctavePlacementViewModel octavePlacement)
    {
        this.inversionIndex = inversionIndex;
        this.octaveOffset = octaveOffset;
        this.weightedSum = weightedSum;
        this.octavePlacement = octavePlacement;
    }

    public override string ToString() => $"{octavePlacement}";

    public static OctavePlacementPair[][] CreatePairs(int n)
    {
        return Enumerable.Range(0, n).Select(i => new OctavePlacementPair[n]).ToArray();

    }

}
