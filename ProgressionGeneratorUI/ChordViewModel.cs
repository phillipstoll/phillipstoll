using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Documents;
using ProgressionGeneratorLib;
using HarmonicAnalysisCommonLib;
using System.Diagnostics;
using System.Windows;

namespace ProgressionGeneratorUI;

/// <summary>
/// Represents a chord
/// </summary>
/// <remarks>Represents an abstract or concrete chord, depending on whether the chord's parameters are fully specified</remarks>
public partial class ChordViewModel : ObservableObject
{
    [ObservableProperty]
    private List<int> phases;
    [ObservableProperty]
    private List<int> chordSizes;
    [ObservableProperty]
    private List<int> widths;
    [ObservableProperty]
    private List<string> chords;
    [ObservableProperty]
    private List<string> spacings;

    // one chord in a series
    // chord pitch heights and harmonic connections for 4 octave placements of the chord
    [ObservableProperty]
    private List<OctavePlacementViewModel> octavePlacements /* [8] */;
    // chord pair (previous and current chords) pitch height connection and harmonic connection for 4 octave placements of the chord plus 2 channels
    [ObservableProperty]
    private List<OctavePlacementViewModel> octavePlacements2 /* [8] */;

    // one chord in a set of all possible chord pairs
    [ObservableProperty]
    private List<ChordViewModelRow> chordViewModelRows /* [iSecondChord] same as [row] */;

    [ObservableProperty]
    private int phaseSelection = -1;
    [ObservableProperty]
    private int chordSizeSelection = -1;
    [ObservableProperty]
    private int widthSelection = -1;
    [ObservableProperty]
    private int chordSelection = -1;
    [ObservableProperty]
    private int spacingSelection = -1;
    [ObservableProperty]
    private int heightsSelection = -1;

    [ObservableProperty]
    private int transposition;
    // chord count and index is needed to populate the right number of rows
    [ObservableProperty]
    private ChordCountIndexStruct chordCountIndex;

    [ObservableProperty]
    private Visibility topVisible;
    [ObservableProperty]
    private Visibility middleVisible;
    [ObservableProperty]
    private Visibility bottomVisible;

    private readonly Action UpdateVisibilityAction;
    private readonly Action<int> UpdateHeightSelectionAction;

    // four inversions of chord
    private List<ChordTones> chordTonesList /* [i_chord_inversion] */;

    public ChordViewModel(Action updateVisibilityAction)
    {
        phases = new List<int> { 5, 4, 3, 2, 1, 0, -1, -2, -3, -4, -5, };
        phaseSelection = 5;
        chordSizes = new List<int> { 4, 3, 2, };
        widths = new List<int>();
        chords = new List<string>();
        spacings = new List<string>();
        octavePlacements = new List<OctavePlacementViewModel>();
        octavePlacements2 = new List<OctavePlacementViewModel>();
        chordViewModelRows = new List<ChordViewModelRow>();
        chordTonesList = new List<ChordTones>();
        chordCountIndex = new ChordCountIndexStruct(0, -1);
        UpdateVisibilityAction = updateVisibilityAction;
        UpdateHeightSelectionAction = UpdateHeightSelection;
        topVisible = Visibility.Visible;
        middleVisible = Visibility.Visible;
        bottomVisible = Visibility.Visible;
        PropertyChanged += ChordParam_PropertyChanged;
        DefaultSelection();
    }

    public bool SelectionMade() => PhaseSelection != -1 && ChordSizeSelection != -1 && WidthSelection != -1 && ChordSelection != -1;

    public override string ToString()
    {
        return $"Phase {PhaseSelection} Size {ChordSizeSelection} Width {WidthSelection} Chord Heights";
    }

    private void DefaultSelection()
    {
        ChordSizeSelection = 0;
    }

    private void ChordParam_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        ProcessPropertyChanged(e.PropertyName ?? string.Empty);
    }

    private void ProcessPropertyChanged(string PropertyName)
    {
        switch (PropertyName)
        {
            case "PhaseSelection":
                UpdateHeights();
                break;
            case "ChordSizeSelection":
                WidthSelection = -1;
                ChordSelection = -1;
                HeightsSelection = -1;
                UpdateWidths();
                UpdateChords();
                UpdateHeights();
                break;
            case "WidthSelection":
                ChordSelection = -1;
                HeightsSelection = -1;
                UpdateChords();
                UpdateHeights();
                // works, but I don't like the way it works
                //ChordSelection = 0;
                break;
            case "ChordSelection":
                HeightsSelection = -1;
                UpdateHeights();
                break;
            case "HeightsSelection":
                UpdateVisibilityAction();
                break;
            case "Transposition":
                UpdateHeights();
                break;
            case "ChordCountIndex":
                UpdateChordViewModelRows();
                break;
        }
    }
    private void UpdateWidths()
    {
        var chordSize_index = ChordSizeSelection == -1 ? ChordSizeSelection : ChordSizes[ChordSizeSelection];
        if (chordSize_index != -1)
        {
            var first = Enumerable.Range(0, ChordSource.Chords[chordSize_index].Length).First(i => ChordSource.Chords[chordSize_index][i].Length != 0);
            var last = Enumerable.Range(0, ChordSource.Chords[chordSize_index].Length).Last(i => ChordSource.Chords[chordSize_index][i].Length != 0);
            Widths = Enumerable.Range(first, last - first + 1).ToList();

            switch (chordSize_index)
            {
                case 4:
                    Spacings = new List<string> { "232", "323", };
                    SpacingSelection = 0;
                    break;
                case 3:
                    Spacings = new List<string> { "22", };
                    SpacingSelection = 0;
                    break;
                case 2:
                    Spacings = new List<string> { "2", };
                    SpacingSelection = 0;
                    break;
                case 1:
                    Spacings = new List<string>();
                    SpacingSelection = -1;
                    break;
            }
        }
        else if (Widths.Count != 0)
        {
            // Don't use Widths.Clear. Doesn't fire PropertyChanged.
            Widths = new List<int>();
        }
    }
    private void UpdateChords()
    {
        var chordSize_index = ChordSizeSelection == -1 ? ChordSizeSelection : ChordSizes[ChordSizeSelection];
        var width_index = WidthSelection == -1 ? WidthSelection : Widths[WidthSelection];
        if (chordSize_index != -1 && width_index != -1)
        {
            Chords = new List<string>(ChordSource.Chords[chordSize_index][width_index]);
        }
        else if (Chords.Count != 0)
        {
            Chords = new List<string>();
        }
    }
    private void UpdateHeights()
    {
        if (SelectionMade())
        {
            var chordString = Chords[ChordSelection];
            var phaseFH = Phases[PhaseSelection];
            var bandReferenceHeight = 0;
            chordTonesList = ChordSource.GenerateChords(chordString, Transposition + phaseFH, bandReferenceHeight, ChordSpacing.Closed232);
            //Heights = new List<string>(chordTonesList.Select(c => c.ShortString));
            var list = new List<OctavePlacementViewModel>();
            var list2 = new List<OctavePlacementViewModel>();
            var empty = new OctavePlacementViewModel
            {
                Harmonic = new HarmonicStruct(),
                Height = new HeightStruct(),
            };
            //var distance = new DistanceAlgorithmLite(100, 100, 100, 100, 100, 100);
            foreach (var chordTones in chordTonesList)
            {
                list.Add(empty);
                var n = new OctavePlacementViewModel
                {
                    Harmonic = new HarmonicStruct(HarmonicStruct.NullDistance),
                    Height = new HeightStruct(chordTones),
                };
                list.Add(n);
                list2.Add(empty);
                list2.Add(empty);
            }
            OctavePlacements = list;
            OctavePlacements2 = list2;

            for (var row = 0; row < ChordCountIndex.Count; row++)
            {
                if (row <= ChordCountIndex.Index)
                {
                    ChordViewModelRows[row].OctavePlacements = list;
                    ChordViewModelRows[row].OctavePlacements2 = list2;
                }
                else
                {
                    //ChordViewModelRows.Add(NullChordViewModelRow);
                }
            }
        }
        else if (OctavePlacements.Count != 0)
        {
            OctavePlacements = new List<OctavePlacementViewModel>();
            OctavePlacements2 = new List<OctavePlacementViewModel>();
            //Heights = new List<string>();
            chordTonesList.Clear();
        }
    }
    /// <summary>
    /// Called whenever the number of chords changes
    /// </summary>
    private void UpdateChordViewModelRows()
    {
        Debug.Assert(ChordViewModelRows.Count != ChordCountIndex.Count);
        ChordViewModelRows = new List<ChordViewModelRow>();
        for (var row = 0; row < ChordCountIndex.Count; row++)
        {
            if (row <= ChordCountIndex.Index)
            {
                ChordViewModelRows.Add(new ChordViewModelRow(OctavePlacements, OctavePlacements2, UpdateHeightSelectionAction));
            }
            else
            {
                ChordViewModelRows.Add(NullChordViewModelRow);
            }
        }
    }
    private void UpdateHeightSelection(int index)
    {
        HeightsSelection = index;
    }
    public static readonly ChordViewModelRow NullChordViewModelRow = new ChordViewModelRow(new List<OctavePlacementViewModel>(), new List<OctavePlacementViewModel>(), new Action<int>((i) => { }));
}
public class ChordCountIndexStruct
{
    public int Count;
    public int Index;

    public ChordCountIndexStruct(int count, int index)
    {
        Count = count;
        Index = index;
    }
}
