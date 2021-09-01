using SFML.Graphics;
using SFML.System;
using Shared.DataStructures;
using System.Collections.Generic;

namespace SolidSimplification.HelperMethods
{
    public static class VisualHelper
    {
        private static Color LineColour = new Color(0x5E, 0xA9, 0x45);

        public static List<Vertex[]> GetVisualsForLines(
            List<LineSegment> lineSegments,
            Vector2f halfScreenSize)
        {
            var output = new List<Vertex[]>();

            foreach (var line in lineSegments)
            {
                output.Add(new Vertex[]
                {
                    new Vertex(halfScreenSize + line.Start * 10, LineColour),
                    new Vertex(halfScreenSize + line.End * 10, LineColour),
                });
            }

            return output;
        }
    }
}
