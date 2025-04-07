using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarmonicAnalysisLib
{
    public class GroupMapLib
    {
        public static (int Frame, int VerticalFrame, int HorizontalFrame) frameCounts = new(0, 0, 0);
        public static GroupMap InitGroupMap(Group frame, List<Group> Groups, bool initialize = true)
        {
            switch (frame.Type)
            {
                case GroupType.Frame:
                    List<Group> groups = frame.FrameToVerticalGroups(Groups);
                    frameCounts.Frame += groups.Count;
                    var map = new GroupMap(GroupMapType.GroupMap, frame, groups, initialize);
                    return map;
                case GroupType.VerticalFrame:
                    List<Group> verticalGroups = frame.VerticalFrameToVerticalGroups(Groups);
                    frameCounts.VerticalFrame += verticalGroups.Count;
                    map = new GroupMap(GroupMapType.VerticalGroupMap, frame, verticalGroups, initialize);
                    return map;
                case GroupType.HorizontalFrame:
                    List<Group> horizGroups = frame.HorizontalFrameToHorizontalGroups(Groups);
                    frameCounts.HorizontalFrame += horizGroups.Count;
                    map = new GroupMap(GroupMapType.HorizontalGroupMap, frame, horizGroups, initialize);
                    return map;
                default:
                    throw new ArgumentException("group must be a Frame, VerticalFrame, or HorizontalFrame", nameof(frame));
            }
        }
    }
}
