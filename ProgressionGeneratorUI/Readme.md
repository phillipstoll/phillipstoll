ProgressionGeneratorUI.csproj handles chord progression input and generation. It is referenced by 

### Object Model
```
ChordViewModel
	List<OctavePlacementViewModel> octavePlacements;
	List<OctavePlacementViewModel> octavePlacements2;

Represents a pitch-height and harmonic connection
OctavePlacementViewModel
	HarmonicStruct harmonic
	HeightStruct height

Represents an octave placement of a pair of chords, including 1 voice leadings
OctavePlacementPair
	OctavePlacementViewModel octavePlacement

Represents an octave placement of a pair of chords, including the 2 best voice leadings
AlternatePair
	OctavePlacementPair[2] Pairs

Represents alternate octave placements of a chord pair
AlternatePairList
	List<AlternatePair> Items

Represents all chord pairs
CrossPair
	AlternatePairList[][] Item
```
