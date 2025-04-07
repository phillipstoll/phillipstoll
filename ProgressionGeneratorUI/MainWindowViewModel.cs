using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows.Input;
using HarmonicAnalysisLib;
using System.Configuration;
using NoteInputLib;

namespace ProgressionGeneratorUI;

/// <summary>
/// Handles the collection of chords and pairs of chords
/// </summary>
public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty]
    private List<int> transpositions;
    [ObservableProperty]
    private int transpositionSelection;
    [ObservableProperty]
    private ObservableCollection<ChordViewModel> chordParams /* [iChord] */;
    [ObservableProperty]
    private bool isTopVisible;
    [ObservableProperty]
    private bool isMiddleVisible;
    [ObservableProperty]
    private bool isBottomVisible;
    [ObservableProperty]
    private List<string> distanceTypes;
    [ObservableProperty]
    private int distanceTypeSelection;
    private CrossPair crossPair;
    private Manager manager { get; }
    private readonly Action UpdateVisibilityAction;
    private const bool display_all_pairs = true;
    private HarmonicAnalysisCommonLib.Quarantine.DistanceType distanceType;

    public MainWindowViewModel()
    {
        FileAddCommand = new RelayCommand(FileAdd);
        FileDeleteCommand = new RelayCommand(FileDelete);
        transpositions = Enumerable.Range(-5, 11).ToList();
        transpositionSelection = 5;
        chordParams = new ObservableCollection<ChordViewModel>();
        isTopVisible = true;
        isMiddleVisible = true;
        isBottomVisible = true;
        distanceTypes = new List<string> { "P", "A", "W", "cp", "ca", "cw", };
        distanceTypeSelection = 3;
        crossPair = new CrossPair();
        manager = new Manager();
        UpdateVisibilityAction = UpdateVisibility;
        for (var i = 0; i < 3; i++)
        {
            AddChordParam();
        }
        UpdateDistanceType();
        PropertyChanged += MainWindowViewModel_PropertyChanged;
    }

    private void MainWindowViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case "TranspositionSelection":
                int transposition = TranspositionSelection == -1 ? 0 : Transpositions[TranspositionSelection];
                foreach (var chord in ChordParams)
                {
                    chord.Transposition = transposition;
                }
                UpdateVoiceLeading();
                UpdateHarmonicParts();
                break;
            case "IsTopVisible":
            case "IsMiddleVisible":
            case "IsBottomVisible":
                foreach (var chord in ChordParams)
                {
                    chord.TopVisible = IsTopVisible ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
                    chord.MiddleVisible = IsMiddleVisible ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
                    chord.BottomVisible = IsBottomVisible ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
                }
                break;
            case "DistanceTypeSelection":
                UpdateDistanceType();
                UpdateHarmonicParts();
                break;
        }
    }

    private void ChordParam_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (sender is ChordViewModel chord)
        {
            int indexOf = ChordParams.IndexOf(chord);
            if (indexOf == -1) throw new Exception();
            int count = ChordParams.Count;
            //Debug.WriteLine(e.PropertyName);
            switch (e.PropertyName)
            {
                case "HeightsSelection":
                    break;
                case "ChordSelection":
                case "PhaseSelection":
                    UpdateVoiceLeading();
                    UpdateHarmonicParts();
                    break;
            }
        }
    }

    private void UpdateVoiceLeading()
    {
        if (ChordParams.All(chord => chord.SelectionMade()))
        {
            crossPair = new CrossPair(ChordParams);

            if (display_all_pairs)
            {
                for (var first = 0; first < ChordParams.Count - 1; first++)
                {
                    for (var second = first + 1; second < ChordParams.Count; second++)
                    {
                        List<OctavePlacementViewModel> octavePlacements2 = new List<OctavePlacementViewModel>();
                        for (var j = 0; j < ChordParams[first].OctavePlacements2.Count / 2; j++)
                        {
                            octavePlacements2.Add(crossPair.Items[first][second].Items[j].Pairs[1].octavePlacement);
                            octavePlacements2.Add(crossPair.Items[first][second].Items[j].Pairs[0].octavePlacement);
                        }
                        // first and second are swapped because the delta is attached to the second chord instead of the first chord
                        ChordParams[second].ChordViewModelRows[first].OctavePlacements2 = octavePlacements2;
                    }
                }
            }

            for (var i = 0; i < ChordParams.Count - 1; i++)
            {
                List<OctavePlacementViewModel> octavePlacements2 = new List<OctavePlacementViewModel>();
                for (var j = 0; j < ChordParams[i].OctavePlacements2.Count / 2; j++)
                {
                    octavePlacements2.Add(crossPair.Items[i][i + 1].Items[j].Pairs[1].octavePlacement);
                    octavePlacements2.Add(crossPair.Items[i][i + 1].Items[j].Pairs[0].octavePlacement);
                }
                // place delta on first chord of the chord pair
                //ChordParams[i].OctavePlacements2 = octavePlacements2;
                // place delta on second chord of the chord pair
                if (i + 1 < ChordParams.Count)
                {
                    ChordParams[i + 1].OctavePlacements2 = octavePlacements2;
                }
            }
        }
        else
        {
            for (var i = 0; i < ChordParams.Count; i++)
            {
                for (var j = 0; j < ChordParams[i].OctavePlacements2.Count; j++)
                {
                    ChordParams[i].OctavePlacements2[j].Clear();
                }
            }
        }
    }

    private void UpdateHarmonicParts()
    {
        if (ChordParams.All(chord => chord.SelectionMade()))
        {
            if (display_all_pairs)
            {
                for (var chordI = 0; chordI < ChordParams.Count - 1; chordI++)
                {
                    for (var chordJ = chordI + 1; chordJ < ChordParams.Count; chordJ++)
                    {
                        var countI = ChordParams[chordI].OctavePlacements.Count / 2;
                        var countJ = ChordParams[chordJ].OctavePlacements.Count / 2;
                        var count = Math.Max(countI, countJ);
                        // first and second are swapped because the delta is attached to the second chord instead of the first chord
                        //var octavePlacements = ChordParams[chordJ].ChordViewModelRows[chordI].OctavePlacements;
                        var octavePlacements2 = ChordParams[chordI].ChordViewModelRows[chordJ].OctavePlacements2;
                        for (var inversion = 0; inversion < count/*octavePlacements2.Count / 2*/; inversion++)
                        {
                            // the OctavePlacementViewModels to be updated
                            var A = ChordParams[chordI].OctavePlacements[inversion * 2 + 1];
                            var a = ChordParams[chordI].ChordViewModelRows[chordI].OctavePlacements[inversion * 2 + 1];
                            var B = ChordParams[chordJ].OctavePlacements[inversion * 2 + 1];
                            var b = ChordParams[chordJ].ChordViewModelRows[chordJ].OctavePlacements[inversion * 2 + 1];
                            for (var alternate = 0; alternate < crossPair.Items[chordI][chordJ].Items[inversion].Pairs.Length; alternate++)
                            {
                                var pair = crossPair.Items[chordI][chordJ].Items[inversion].Pairs[alternate];
                                var delta = pair.octavePlacement.Height.Values;
                                var myString = pair.octavePlacement.Height.MyString;
                                var c = ChordParams[chordJ].ChordViewModelRows[chordI].OctavePlacements2[inversion * 2];
                                if (c.Height.MyString != myString)
                                {
                                    c = ChordParams[chordJ].ChordViewModelRows[chordI].OctavePlacements2[inversion * 2 + 1];
                                }
                                if (c.Height.MyString != myString)
                                {
                                    throw new Exception();
                                }

                                if (chordJ == chordI + 1)
                                {
                                    var C = ChordParams[chordJ].OctavePlacements2[inversion * 2];
                                    if (C.Height.MyString != myString)
                                    {
                                        C = ChordParams[chordJ].OctavePlacements2[inversion * 2 + 1];
                                    }
                                    if (C.Height.MyString != myString)
                                    {
                                        throw new Exception();
                                    }
                                }
                                var s = string.Join(" ",
                                    Enumerable.Range(0, countI).Select(j =>
                                        a.Height.chordTones.Items[j].Height));
                                var t = string.Join(" ",
                                    Enumerable.Range(0, countI).Select(j =>
                                        b.Height.chordTones.Items[j].Height
                                        + delta[j]));
                                var progressionString = string.Join(", ", new string[] { s, t });
                                var noteList = NoteInputLib.StringInput.ReadStringInput(progressionString, ',');
                                PitchPattern[][] progressions = NoteListConverter.Convert(noteList);
                                manager.Process(progressions[0/*index*/]);

                                var fdis = manager.SegmentLites[0].FrameStructs[0].GroupMap.Distance;
                                a.Harmonic = new HarmonicStruct(new HarmonicAnalysisCommonLib.DistanceAlgorithmLite(fdis), distanceType);

                                var dist = manager.SegmentLites[0].Distance;
                                var visible = c.Harmonic.Visible;
                                c.Harmonic = new HarmonicStruct(new HarmonicAnalysisCommonLib.DistanceAlgorithmLite(dist), distanceType);
                                c.Harmonic.Visible = visible;
                            }
                        }
                    }
                }
            }
        }
    }
    // hide elements
    private void UpdateVisibility()
    {
        for (var iChord = 0; iChord < ChordParams.Count; iChord++)
        {
            var chordParam = ChordParams[iChord];
            if (!chordParam.SelectionMade())
            {
                continue;
            }
            var opvm = chordParam.OctavePlacements;
            var opvm2 = chordParam.OctavePlacements2;
            for (var i = 0; i < opvm.Count / 2; i++)
            {
                if (chordParam.HeightsSelection == -1 || i * 2 + 1 == chordParam.HeightsSelection)
                {
                    opvm[i * 2 + 1].Harmonic.Visible = true;
                    opvm[i * 2 + 1].Height.Visible = true;
                    opvm2[i * 2].Harmonic.Visible = true;
                    opvm2[i * 2].Height.Visible = true;
                    opvm2[i * 2 + 1].Harmonic.Visible = true;
                    opvm2[i * 2 + 1].Height.Visible = true;
                    //foreach (var row in chordParam.ChordViewModelRows)
                    //{
                    //    if (row.OctavePlacements.Count == 0) continue;
                    //    row.OctavePlacements[i * 2 + 1].Harmonic.Visible = true;
                    //    row.OctavePlacements[i * 2 + 1].Height.Visible = true;
                    //    row.OctavePlacements2[i * 2].Harmonic.Visible = true;
                    //    row.OctavePlacements2[i * 2].Height.Visible = true;
                    //    row.OctavePlacements2[i * 2 + 1].Harmonic.Visible = true;
                    //    row.OctavePlacements2[i * 2 + 1].Height.Visible = true;
                    //}
                }
                else
                {
                    opvm[i * 2 + 1].Harmonic.Visible = false;
                    opvm[i * 2 + 1].Height.Visible = false;
                    opvm2[i * 2].Harmonic.Visible = false;
                    opvm2[i * 2].Height.Visible = false;
                    opvm2[i * 2 + 1].Harmonic.Visible = false;
                    opvm2[i * 2 + 1].Height.Visible = false;
                    //foreach (var row in chordParam.ChordViewModelRows)
                    //{
                    //    if (row.OctavePlacements.Count == 0) continue;
                    //    row.OctavePlacements[i * 2 + 1].Harmonic.Visible = false;
                    //    row.OctavePlacements[i * 2 + 1].Height.Visible = false;
                    //    row.OctavePlacements2[i * 2].Harmonic.Visible = false;
                    //    row.OctavePlacements2[i * 2].Height.Visible = false;
                    //    row.OctavePlacements2[i * 2 + 1].Harmonic.Visible = false;
                    //    row.OctavePlacements2[i * 2 + 1].Height.Visible = false;
                    //}
                }
            }
        }
    }

    private void UpdateDistanceType()
    {
        distanceType = (HarmonicAnalysisCommonLib.Quarantine.DistanceType)Enum.ToObject(
                            typeof(HarmonicAnalysisCommonLib.Quarantine.DistanceType), DistanceTypeSelection + 1);
    }

    private void AddChordParam()
    {
        int transposition = TranspositionSelection == -1 ? 0 : Transpositions[TranspositionSelection];
        var chordParam = new ChordViewModel(UpdateVisibilityAction)
        {
            Transposition = transposition,
        };
        chordParam.PropertyChanged += ChordParam_PropertyChanged;
        ChordParams.Add(chordParam);
        UpdateChordCount();
    }
    private void RemoveChordParam()
    {
        if (ChordParams.Count > 1)
        {
            ChordParams[ChordParams.Count - 1].PropertyChanged -= ChordParam_PropertyChanged;
            ChordParams.RemoveAt(ChordParams.Count - 1);
        }
        UpdateChordCount();
    }
    /// <summary>
    /// Called whenever the number of chords changes
    /// </summary>
    private void UpdateChordCount()
    {
        foreach (var chord in ChordParams)
        {
            chord.ChordCountIndex = new ChordCountIndexStruct(ChordParams.Count, ChordParams.IndexOf(chord));
        }
    }
    #region Commands
    public ICommand FileAddCommand { get; }
    public ICommand FileDeleteCommand { get; }
    private void FileAdd()
    {
        AddChordParam();
    }
    private void FileDelete()
    {
        RemoveChordParam();
    }
    #endregion
}
