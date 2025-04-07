using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HarmonicAnalysisCommonLib.Quarantine;
using HarmonicAnalysisCommonLib;
using ProgressionGeneratorLib;

namespace ProgressionGeneratorUI;

/// <summary>
/// Represents pitch-height and harmonic connection for a chord or chord pair
/// </summary>
/// <remarks>For chords, Height represents pitch heights. For chord pairs, Height represents pitch height deltas.</remarks>
public partial class OctavePlacementViewModel : ObservableObject
{
    [ObservableProperty]
    private HarmonicStruct harmonic;
    [ObservableProperty]
    private HeightStruct height;
    public override string ToString() => Height.MyString.Length == 0 && Harmonic.MyString.Length == 0 ? string.Empty : $"{Height} : {Harmonic}";
    public void Clear()
    {
        if (Harmonic.distance != HarmonicStruct.NullDistance)
        {
            Harmonic = new HarmonicStruct();
        }
        if (Height.ToString().Length != 0)
        {
            Height = new HeightStruct();
        }
    }
}
/// <summary>
/// Represents a harmonic connection
/// </summary>
public partial class HarmonicStruct : ObservableObject
{
    [ObservableProperty]
    private string myString;

    public HarmonicStruct()
    {
        MyString = string.Empty;
    }
    public HarmonicStruct(IDistanceAlgorithmLite distance, DistanceType distanceType = DistanceType.None)
    {
        this.distance = distance;
        harmonicString = distance == NullDistance ? string.Empty : $"{distance.DistanceTypeToDouble(distanceType):f0}";
        MyString = harmonicString;
    }

    public IDistanceAlgorithmLite distance = NullDistance;
    private bool visible = true;
    /// <summary>
    /// Visible is only used to set MyString
    /// </summary>
    public bool Visible
    {
        get => visible;
        set
        {
            visible = value;
            MyString = value ? harmonicString : string.Empty;
        }
    }
    public override string ToString() => myString;

    public static readonly IDistanceAlgorithmLite NullDistance = new DistanceAlgorithmLite(0, 0, 0, 0, 0, 0);
    private readonly string harmonicString = string.Empty;
}
/// <summary>
/// Represents a pitch-height connection
/// </summary>
public partial class HeightStruct : ObservableObject
{
    [ObservableProperty]
    private string myString;

    public HeightStruct()
    {
        MyString = heightString;
    }

    public HeightStruct(ChordTones chordTones)
    {
        this.chordTones = chordTones;
        heightString = chordTones.ShortString;
        MyString = heightString;
    }

    public HeightStruct(int[] values)
    {
        Values = values;
        heightString = string.Join(" ", Values);
        MyString = heightString;
    }

    public ChordTones chordTones = NullChordTones;
    public int[] Values = new int[0];
    private bool visible = true;
    /// <summary>
    /// Visible is only used to set MyString
    /// </summary>
    public bool Visible
    {
        get => visible;
        set
        {
            visible = value;
            MyString = value ? heightString : string.Empty;
        }
    }

    public override string ToString() => heightString;
    public static readonly ChordTones NullChordTones = new ChordTones { Items = new Tone[0] };

    private readonly string heightString = string.Empty;
}
