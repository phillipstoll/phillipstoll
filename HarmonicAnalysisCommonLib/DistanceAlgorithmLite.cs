﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonicAnalysisCommonLib.Quarantine;

namespace HarmonicAnalysisCommonLib;
public class DistanceAlgorithmLite : IDistanceAlgorithmLite
{
    public double Pairwise
    {
        get;
    }
    public double Average
    {
        get;
    }
    public double Weighted
    {
        get;
    }
    public double CompoundPairwise
    {
        get;
    }
    public double CompoundAverage
    {
        get;
    }
    public double CompoundWeighted
    {
        get;
    }
    public DistanceAlgorithmLite(double pairwise, double average, double weighted, double compoundPairwise, double compoundAverage, double compoundWeighted)
    {
        Pairwise = pairwise;
        Average = average;
        Weighted = weighted;
        CompoundPairwise = compoundPairwise;
        CompoundAverage = compoundAverage;
        CompoundWeighted = compoundWeighted;
    }

    public DistanceAlgorithmLite(IDistanceAlgorithmLite d)
    {
        Pairwise = d.Pairwise;
        Average = d.Average;
        Weighted = d.Weighted;
        CompoundPairwise = d.CompoundPairwise;
        CompoundAverage = d.CompoundAverage;
        CompoundWeighted = d.CompoundWeighted;
    }

    public double DistanceTypeToDouble(DistanceType type)
    {
        switch (type)
        {
            case DistanceType.Pairwise:
                return Pairwise;
            case DistanceType.Average:
                return Average;
            case DistanceType.Weighted:
                return Weighted;
            case DistanceType.CompoundPairwise:
                return CompoundPairwise;
            case DistanceType.CompoundAverage:
                return CompoundAverage;
            case DistanceType.CompoundWeighted:
                return CompoundWeighted;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }

}
