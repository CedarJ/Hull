using Shared.DataStructures;
using System;
using System.Collections.Generic;

namespace SolidSimplification.Data
{
    public class FlattenedGeometrySharedData
    {
        public Action<List<LineSegment>> SetVisuals { get; set; }
    }
}
