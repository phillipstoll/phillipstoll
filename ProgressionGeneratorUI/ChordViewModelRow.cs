using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ProgressionGeneratorUI;

/// <summary>
/// Represents a chord and voice leadings from this chord and other chords
/// </summary>
public partial class ChordViewModelRow : ObservableObject
{
    [ObservableProperty]
    private List<OctavePlacementViewModel> octavePlacements;
    [ObservableProperty]
    private List<OctavePlacementViewModel> octavePlacements2;
    // whenever HeightsSelection changes, update ChordViewModel.HeightsSelection. That's all you have to do.
    [ObservableProperty]
    private int heightsSelection = -1;
    private readonly Action<int> UpdateVisibilityAction;

    public ChordViewModelRow(List<OctavePlacementViewModel> octavePlacements, List<OctavePlacementViewModel> octavePlacements2, Action<int> updateVisibilityAction)
    {
        this.octavePlacements = octavePlacements;
        this.octavePlacements2 = octavePlacements2;
        UpdateVisibilityAction = updateVisibilityAction;
        this.PropertyChanged += ChordViewModelRow_PropertyChanged;
    }

    private void ChordViewModelRow_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == "HeightsSelection")
        {
            UpdateVisibilityAction(HeightsSelection);
        }
    }
}
