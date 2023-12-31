using Assimp;
using ClipperLib;
using CSharpFunctionalExtensions;
using SFML.System;
using Shared.DataStructures;
using SolidSimplification.Data;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace SolidSimplification.HelperMethods
{
    public static class HullGenerator
    {
        // The current implementation is hard coded to create hulls along the Z axis.
        // axis can be either 1 for X, 2 for Y, or 3 for Z, default to Z
        public static Result<List<LineSegment>> Generate(Scene scene, int axis, float alpha)
        {
            var output = new List<LineSegment>();
            var shapes = new List<List<IntPoint>>();

            foreach (var mesh in scene.Meshes)
            {
                var points = mesh.Vertices;

                foreach (var triangle in mesh.Faces)
                {
                    Vector3D p1, p2, p3;

                    if (axis == 1)
                    {
                        p1 = new Vector3D(
                            points[triangle.Indices[0]].Y,
                            points[triangle.Indices[0]].Z,
                            0);
                        p2 = new Vector3D(
                            points[triangle.Indices[1]].Y,
                            points[triangle.Indices[1]].Z,
                            0);
                        p3 = new Vector3D(
                            points[triangle.Indices[2]].Y,
                            points[triangle.Indices[2]].Z,
                            0);
                    }
                    else if (axis == 2)
                    {
                        p1 = new Vector3D(
                            points[triangle.Indices[0]].X,
                            points[triangle.Indices[0]].Z,
                            0);
                        p2 = new Vector3D(
                            points[triangle.Indices[1]].X,
                            points[triangle.Indices[1]].Z,
                            0);
                        p3 = new Vector3D(
                            points[triangle.Indices[2]].X,
                            points[triangle.Indices[2]].Z,
                            0);
                    }
                    else
                    {
                        p1 = new Vector3D(
                            points[triangle.Indices[0]].X,
                            points[triangle.Indices[0]].Y,
                            0);
                        p2 = new Vector3D(
                            points[triangle.Indices[1]].X,
                            points[triangle.Indices[1]].Y,
                            0);
                        p3 = new Vector3D(
                            points[triangle.Indices[2]].X,
                            points[triangle.Indices[2]].Y,
                            0);
                    }

                    if (p1 == p2 || p2 == p3 || p3 == p1)
                    {
                        continue;
                    }

                    var round1 = new IntPoint((long)Math.Round(p1.X * 100), (long)p1.Y * 100);
                    var round2 = new IntPoint((long)Math.Round(p2.X * 100), (long)p2.Y * 100);
                    var round3 = new IntPoint((long)Math.Round(p3.X * 100), (long)p3.Y * 100);

                    if ((round1.X == round2.X && round1.Y == round2.Y) || (round1.X == round3.X && round1.Y == round3.Y) ||
                        (round3.X == round2.X && round3.Y == round2.Y))
                    {
                        continue;
                    }

                    var order = new List<IntPoint>
                    {
                        round1,
                        round2,
                        round3,
                    }.OrderBy(p => p.X).ThenBy(p => p.Y).ToList();

                    // slope
                    var o1 = order[0];
                    var o2 = order[1];
                    var o3 = order[2];

                    var s1 = double.PositiveInfinity;
                    var s2 = double.PositiveInfinity;

                    if ((o2.X - o1.X) != 0)
                    {
                        s1 = (o2.Y - o1.Y) / (double)(o2.X - o1.X);
                    }

                    if ((o3.X - o1.X) != 0)
                    {
                        s2 = (o3.Y - o1.Y) / (double)(o3.X - o1.X);
                    }

                    // points have rounded to be colinear
                    if (s1 == s2)
                    {
                        continue;
                    }

                    if (!((o3.X - o1.X) == 0 || s2 > s1))
                    {
                        o2 = order[2];
                        o3 = order[1];
                    }

                    shapes.Add(new List<IntPoint>
                    {
                        o1,
                        o2,
                        o3,
                    });
                }

            }

            if (!shapes.Any())
            {
                return Result.Failure<List<LineSegment>>(
                    "Compressing the geometries resulted in no viable shapes to calculate a hull.");
            }

            var currentHull = new List<List<IntPoint>>();
            var clipper = new Clipper();
            foreach (var s in shapes)
            {
                clipper.AddPolygon(s, PolyType.ptSubject);
            }

            var clipSuccessful = true;
            try
            {
                clipper.Execute(ClipType.ctUnion, currentHull, PolyFillType.pftPositive, PolyFillType.pftPositive);
            }
            catch (Exception _)
            {
                clipSuccessful = false;
            }

            if (!clipSuccessful)
            {
                return Result.Failure<List<LineSegment>>("An error ocurred while attempting the clip operation.");
            }

            //Do aggregation opeation if alpha is valid
            /*if (alpha != 0)
            {
                var points1 = new List<PointF>();

                foreach (var hull in currentHull)
                {
                    foreach (var point in hull)
                    {
                        points1.Add(new PointF(point.X / 100f, point.Y / 100f));
                    }
                }

                var edges = AlphaShape.AlphaShape1(points1, alpha);
                foreach (var edge in edges)
                {
                    var start = new Vector2f(edge.A.X, edge.A.Y);
                    var end = new Vector2f(edge.B.X, edge.B.Y);
                    output.Add(new LineSegment(start, end));
                }
            }
            else
            {
            foreach (var hull in currentHull)
            {
                for (int i = 0; i < hull.Count() - 1; i++)
                {
                    var start = new Vector2f(hull[i].X / 100f, hull[i].Y / 100f);
                    var end = new Vector2f(hull[i + 1].X / 100f, hull[i + 1].Y / 100f);

                    output.Add(new LineSegment(start, end));
                }


                output.Add(new LineSegment(
                    new Vector2f(hull[0].X / 100f, hull[0].Y / 100f),
                    new Vector2f(hull.Last().X / 100f, hull.Last().Y / 100f)));
            }*/

            var tempnodes = new List<Node>();
            var nodes = new List<Node>();


            var count = 0;

            
            foreach (var hull in currentHull)
            {
                foreach (var point in hull)
                {
                    var temp = new Node((double)point.X / 100, (double)point.Y / 100);
                    if (!tempnodes.Contains(temp))
                    {
                        tempnodes.Add(temp);
                    }
                }
            }
            foreach (var node in tempnodes)
            {
                nodes.Add(new Node(node.x, node.y, count));
                count++;
            }
            count = 0;

            Hull.setConvexHull(nodes);
            var x = Hull.setConcaveHull(0, 20);

            foreach (var line in x)
            {
                var start = new Vector2f((float)line.nodes[0].x, (float)line.nodes[0].y);
                var end = new Vector2f((float)line.nodes[1].x, (float)line.nodes[1].y);
                output.Add(new LineSegment(start, end));
            }
            Hull.unused_nodes.Clear();
            Hull.hull_edges.Clear();
            Hull.hull_concave_edges.Clear();

            


            return output;
        }
    }
}