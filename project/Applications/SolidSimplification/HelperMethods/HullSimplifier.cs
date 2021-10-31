using Assimp;
using ClipperLib;
using CSharpFunctionalExtensions;
using SFML.System;
using Shared.DataStructures;
using SolidSimplification.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SolidSimplification.HelperMethods
{
    public static class HullSimplifier
    {
        // this function implements the Douglas-Peucker method for 2D simplification
        // given a list of lines, simplify them using parameter epsilon
        public static Result<List<LineSegment>> Simplify(List<LineSegment> lines, double epsilon)
        {
            if(lines.Count < 1)
            {
                throw new ArgumentOutOfRangeException("Not enough lines to simplify");
            }

            List<LineSegment> output = new List<LineSegment>();
            List<Vector2f> points = new List<Vector2f>();
            double dmax = 0;
            int index = 0;
            int end = 0;

            // retreive all points from lines
            foreach (var line in lines)
            {
                points.Add(line.Start);
                points.Add(line.End);
            }

            // remove duplicates and get last index
            //points = points.Distinct(new Comparer()).ToList();
            end = points.Count - 1;

            // find the point with max distance between start and end
            for (int i = 1; i < end - 1; i++)
            {
                double d = GetDistance(points[i], points[0], points[end]);

                if(d > dmax)
                {
                    index = i;
                    dmax = d;
                }
            }

            System.Diagnostics.Debug.Write(dmax);
            System.Diagnostics.Debug.Write("\n");

            // recursively simplify if distance greater than epsilon
            if (dmax > epsilon)
            {
                // split list at index
                List<Vector2f> points1 = points.Take(index + 1).ToList();
                List<Vector2f> points2 = points.Skip(index).ToList();

                // recursively simplify
                List<LineSegment> results1 = new List<LineSegment>();
                List<LineSegment> results2 = new List<LineSegment>();
                SimplifyRecursive(points1, epsilon, results1);
                SimplifyRecursive(points2, epsilon, results2);

                // build the result list
                output.AddRange(results1.Take(results1.Count - 1));
                output.AddRange(results2);
            }
            else
            {
                output.Clear();
                output.Add(new LineSegment(points[0],points[end]));
            }

            return output;
        }

        // this function implements the Douglas-Peucker method for 2D simplification
        // given a list of points, simplify them using parameter epsilon
        public static void SimplifyRecursive(List<Vector2f> points, double epsilon, List<LineSegment> output)
        {
            if (points.Count < 2)
            {
                throw new ArgumentOutOfRangeException("Not enough points to simplify");
            }

            double dmax = 0;
            int index = 0;
            int end = points.Count - 1;

            // find the point with max distance between start and end
            for (int i = 1; i < end - 1; i++)
            {
                double d = GetDistance(points[i], points[0], points[end]);

                if (d > dmax)
                {
                    index = i;
                    dmax = d;
                }
            }

            System.Diagnostics.Debug.Write(dmax);
            System.Diagnostics.Debug.Write("\n");

            // recursively simplify if distance greater than epsilon
            if (dmax > epsilon)
            {
                // split list at index
                List<Vector2f> points1 = points.Take(index + 1).ToList();
                List<Vector2f> points2 = points.Skip(index).ToList();

                // recursively simplify
                List<LineSegment> results1 = new List<LineSegment>();
                List<LineSegment> results2 = new List<LineSegment>();
                SimplifyRecursive(points1, epsilon, results1);
                SimplifyRecursive(points2, epsilon, results2);

                // build the result list
                output.AddRange(results1);
                output.AddRange(results2);
            }
            else
            {
                output.Clear();
                output.Add(new LineSegment(points[0], points[end]));
            }
        }

        // calculate the perpendicular distance from point to line
        public static double GetDistance(Vector2f point, Vector2f p1, Vector2f p2)
        {
            float dx = p2.X - p1.X;
            float dy = p2.Y - p1.Y;

            float m = (dx * dx) + (dy * dy);
            float u = ((point.X - p1.X) * dx + (point.Y - p1.Y) * dy) / m;

            if (u < 0)
            {
                dx = p1.X;
                dy = p1.Y;
            }
            else if (u > 1)
            {
                dx = p2.X;
                dy = p2.Y;
            }
            else
            {
                dx = p1.X + u * dx;
                dy = p1.Y + u * dy;
            }

            dx = point.X - dx;
            dy = point.Y - dy;

            return Math.Sqrt(dx * dx + dy * dy);
        }
    }

    // comparer for Vector2f that compares both X and Y
    class Comparer : IEqualityComparer<Vector2f>
    {
        public bool Equals(Vector2f x, Vector2f y)
        {
            return (x.X == y.X) && (x.Y == y.Y);
        }
        public int GetHashCode(Vector2f obj)
        {
            return obj.X.GetHashCode() + obj.Y.GetHashCode();
        }
    }
}
