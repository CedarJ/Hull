using SFML.Graphics;
using SFML.System;
using Shared.Core;
using Shared.DataStructures;
using Shared.Interfaces;
using Shared.ScreenConfig;
using SolidSimplification.Data;
using SolidSimplification.HelperMethods;
using System.Collections.Generic;

namespace SolidSimplification.Screens
{
    public class FlattenedGeometryVisualizerScreen : Screen
    {
        private List<Vertex[]> lines;
        private ScreenConfiguration configuration;

        public FlattenedGeometryVisualizerScreen(
            FlattenedGeometrySharedData dataProvider,
            IApplicationManager applicationManager)
        {
            dataProvider.SetVisuals = this.SetVisuals;
            configuration = applicationManager.GetScreenConfiguration();

            this.lines = new List<Vertex[]>();
        }

        private void SetVisuals(List<LineSegment> lineSegments)
        {
            // Clear any lines from the last hull.
            lines.Clear();

            // Create the visuals for the new line segments.
            lines = VisualHelper.GetVisualsForLines(
                lineSegments,
                new Vector2f(configuration.Width, configuration.Height));
        }

        public override void OnRender(RenderTarget target)
        {
            // Set the view of our target to allow camera movement for the visual screen.
            target.SetView(this.Camera.GetView());

            foreach (var line in lines)
            {
                target.Draw(line, PrimitiveType.Lines);
            }
        }
    }
}
