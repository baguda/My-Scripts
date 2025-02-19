using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using static UnityEditor.FilePathAttribute;
namespace CogSim
{
    /// <summary>
    /// Static Methods for Map Utility
    /// </summary>
public static class MapUtility
{
        public static HashSet<Vector3Int> GetTilesInRange(Vector3Int center, int range)
        {
            HashSet<Vector3Int> visionRange = new HashSet<Vector3Int>();

            // Iterate over a cube defined by the range
            for (int x = -range; x <= range; x++)
            {
                for (int y = -range; y <= range; y++)
                {
                    for (int z = -range; z <= range; z++)
                    {
                        Vector3Int offset = new Vector3Int(x, y, z);

                        // Check if the position is within the range (Manhattan distance)
                        if (IsWithinRange(center, center + offset, range))
                        {
                            visionRange.Add(center + offset);
                        }
                    }
                }
            }

            return visionRange;
        }

        // Helper method to check if a position is within the range (Manhattan distance)
        private static bool IsWithinRange(Vector3Int center, Vector3Int position, int range)
        {
            int distance = Mathf.Abs(position.x - center.x) +
                           Mathf.Abs(position.y - center.y) +
                           Mathf.Abs(position.z - center.z);

            return distance <= range;
        }


        /// <summary>
        /// Finds all tiles at a specific distance from a center point in a 3D grid.
        /// </summary>
        /// <param name="center">The center point from which to measure distance.</param>
        /// <param name="distance">The exact distance from the center to find tiles.</param>
        /// <returns>A list of Vector3Int positions representing tiles at the specified distance.</returns>
        public static List<Vector3Int> GetTilesInRangeMax(Vector3Int center, int distance)
        {
            List<Vector3Int> result = new List<Vector3Int>();

            // Loop through all possible positions within a cube of dimensions (2 * distance + 1) on each axis
            for (int x = -distance; x <= distance; x++)
            {
                for (int y = -distance; y <= distance; y++)
                {
                    for (int z = -distance; z <= distance; z++)
                    {
                        Vector3Int pos = new Vector3Int(center.x + x, center.y + y, center.z + z);

                        // Check if the Manhattan distance matches the given distance
                        if (Mathf.Abs(x) + Mathf.Abs(y) + Mathf.Abs(z) == distance)
                        {
                            result.Add(pos);
                        }
                    }
                }
            }

            return result;
        }


        /// <summary>
        /// Used to get a location "behind" a target location relative to a origin location
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="Target"></param>
        /// <param name="knockback"></param>
        /// <returns></returns>
        public static Vector3Int getPointRadialOutward(Vector3 origin, Vector3 Target, float knockback)
        {
            float angleTmp = (float)Math.Atan((Target.z - origin.z) / (Target.x - origin.x)) + ((Target.x - origin.x) < 0 ? 3.14f : 0f);
            var kb = new Vector3(knockback * (float)Math.Cos(angleTmp), 0, knockback * (float)Math.Sin(angleTmp));

            return (Target + kb).ToVector3Int();

        }


        public static IEnumerable<Vector3Int> coneArea(Vector3Int center, int radius, int theta_i, int theta_f)
        {
            foreach (var point in CircleArea(center - new Vector3Int(radius, 0, radius), new Vector3Int(2 * radius, 0, 2 * radius)))
            {
                if (center.DistanceTo(point) <= radius && inAngle(center, point, (float)theta_i * (3.14f / 180f), (float)theta_f * (3.14f / 180f))) yield return point;

            }

            //yield break;
        }

        public static bool inAngle(Vector3Int center, Vector3Int point, float theta_i, float theta_f)
        {
            var dz = (float)(point.z - center.z);
            var dx = (float)(point.x - center.x);
            var pt = Math.Atan(dz / dx) + (dx < 0 ? 3.14f : 0f);

            var dt = theta_f - theta_i;
            if (dt < 0)
            {
                if (pt > theta_i || pt < theta_f) return true;
            }
            else
            {
                if (pt > theta_i && pt < theta_f) return true;
            }
            return false;
        }

        public static float getDistance(Vector3Int startPoint, Vector3Int endPoint)
        {
            return (float)Math.Sqrt(((endPoint.x - startPoint.x) * (endPoint.x - startPoint.x)) + ((endPoint.y - startPoint.y) * (endPoint.y - startPoint.y)) + ((endPoint.z - startPoint.z) * (endPoint.z - startPoint.z)));
        }

        public static IEnumerable<Vector3Int> GetPointsOnLine(int x0, int y0, int x1, int y1)
        {
            bool steep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);
            if (steep)
            {
                int t;
                t = x0; // swap x0 and y0
                x0 = y0;
                y0 = t;
                t = x1; // swap x1 and y1
                x1 = y1;
                y1 = t;
            }
            if (x0 > x1)
            {
                int t;
                t = x0; // swap x0 and x1
                x0 = x1;
                x1 = t;
                t = y0; // swap y0 and y1
                y0 = y1;
                y1 = t;
            }
            int dx = x1 - x0;
            int dy = Math.Abs(y1 - y0);
            int error = dx / 2;
            int ystep = (y0 < y1) ? 1 : -1;
            int y = y0;
            for (int x = x0; x <= x1; x++)
            {
                yield return new Vector3Int((steep ? y : x), 0, (steep ? x : y));
                error = error - dy;
                if (error < 0)
                {
                    y += ystep;
                    error += dx;
                }
            }
            yield break;
        }

        public static IEnumerable<Vector3Int> pointsBrushed(Vector3Int location, int xVal, int zVal, bool pos, bool mirror)
        {
            /*
             * 
             * PM|it
             * 00|10
             * 01|11
             * 11|11
             * 10|01
             * 
             */

            if (xVal == 0 && zVal == 0) yield return location;
            int ci = 0, ct = 0, dx = xVal, dz = zVal;

            if (!pos || (pos && mirror)) ci = -1;
            if (pos || (!pos && mirror)) ct = 1;
            for (int xdx = ci * dx; xdx <= ct * dx; xdx++)
            {
                for (int zdz = ci * dz; zdz <= ct * dz; zdz++)
                {
                    yield return new Vector3Int(location.x + xdx, location.y, location.z + zdz);
                }
            }

        }

        public static IEnumerable<Vector3Int> rectArea(Vector3Int botLeftCorner, Vector3Int topRightCorner, bool hollow = false)
        {
            for (int dx = 0; dx < topRightCorner.x; dx++)
            {
                for (int dz = 0; dz < topRightCorner.z; dz++)
                {

                    if (!hollow) yield return new Vector3Int(botLeftCorner.x + dx, 0, botLeftCorner.z + dz);
                    else
                    {
                        if ((dx == 0 || dz == 0) || (dx == topRightCorner.x - 1 || dz == topRightCorner.z - 1)) yield return new Vector3Int(botLeftCorner.x + dx, 0, botLeftCorner.z + dz);
                    }
                }
            }
        }


        public static Vector3Int getRoomSize(HashSet<Vector3Int> roomCells, Vector3Int location)
        {
            int safety = 1000;
            bool run = true;
            int pdx = 0, ndx = 0, pdz = 0, ndz = 0, value = 0;
            do
            {
                value++;
                if (!roomCells.Contains(new Vector3Int(location.x, 0, location.z - value)) && ndz == 0)
                {
                    ndz = value;
                }
                if (!roomCells.Contains(new Vector3Int(location.x - value, 0, location.z)) && ndx == 0)
                {
                    ndx = value;
                }
                if (!roomCells.Contains(new Vector3Int(location.x, 0, location.z + value)) && pdz == 0)
                {
                    pdz = value;
                }
                if (!roomCells.Contains(new Vector3Int(location.x + value, 0, location.z)) && pdx == 0)
                {
                    pdx = value;
                }
                run = (pdx == 0 || ndx == 0 || pdz == 0 || ndz == 0) && value < safety;

            } while (run);
            return new Vector3Int(pdx + ndx + 1, 0, pdz + ndz + 1);
        }
        /// <summary>
        /// Scans cells out from location in x and z direction to find distance to room edge 
        /// </summary>
        /// <param name="roomCells"></param>
        /// <param name="location"></param>
        /// <returns>pdx,ndx,pdz,ndz.</returns>
        public static List<int> getRoomSizes(HashSet<Vector3Int> roomCells, Vector3Int location)
        {
            int safety = 1000;
            bool run = true;
            int pdx = 0, ndx = 0, pdz = 0, ndz = 0, value = 0;
            do
            {
                value++;
                if (!roomCells.Contains(new Vector3Int(location.x, 0, location.z - value)) && ndz == 0)
                {
                    ndz = value;
                }
                if (!roomCells.Contains(new Vector3Int(location.x - value, 0, location.z)) && ndx == 0)
                {
                    ndx = value;
                }
                if (!roomCells.Contains(new Vector3Int(location.x, 0, location.z + value)) && pdz == 0)
                {
                    pdz = value;
                }
                if (!roomCells.Contains(new Vector3Int(location.x + value, 0, location.z)) && pdx == 0)
                {
                    pdx = value;
                }
                run = (pdx == 0 || ndx == 0 || pdz == 0 || ndz == 0) && value < safety;

            } while (run);
            return new List<int>() { pdx, ndx, pdz, ndz };
        }

        public static IEnumerable<Vector3Int> drunkenPath(Vector3Int pointOne, Vector3Int pointTwo, int pathWidth, int Amplitude = 4, int Frequency = 4, int Phase = 1)
        {

            int dx = pointTwo.x - pointOne.x;
            int dz = (pointTwo.z - pointOne.z);
            var set = genLine(pointOne, pointTwo, Amplitude, Amplitude, true, true);

            List<Vector3Int> vals = new List<Vector3Int>() { pointOne };//(dx < 0 || dz < 0) ? new List<Vector3Int>() { pointTwo }:
            bool wrtx = Math.Abs(dx) >= Math.Abs(dz);
            Frequency = Frequency + UnityEngine.Random.Range(0, Phase);
            if (wrtx)
            {
                for (int ind = 1; ind < Frequency; ind++)
                {
                    if (dx < 0)
                    {
                        vals.Add(set.ToList().FindAll(s => s.x == pointOne.x - ind * (dx / Frequency)).RandomElement());
                    }
                    else
                    {
                        vals.Add(set.ToList().FindAll(s => s.x == pointOne.x + ind * (dx / Frequency)).RandomElement());
                    }
                }

            }
            else
            {
                for (int ind = 1; ind < Frequency; ind++)
                {

                    if (dz < 0)
                    {
                        vals.Add(set.ToList().FindAll(s => s.z == pointOne.z - ind * (dz / Frequency)).RandomElement());
                    }
                    else
                    {
                        vals.Add(set.ToList().FindAll(s => s.z == pointOne.z + ind * (dz / Frequency)).RandomElement());
                    }
                }
            }
            //vals.Add(pointTwo);
            vals.Add(pointTwo);// (dx < 0 || dz < 0) ? pointOne :
            for (int ind = 0; ind < vals.Count() - 1; ind++)
            {
                foreach (var loc in genLine(vals[ind], vals[ind + 1], pathWidth, pathWidth, true, true))
                {
                    //Prim.Log2("DP: " + loc.ToString());
                    yield return loc;
                }
            }


        }
        /// <summary>
        /// Returns a collection of points allocated between pointOne and pointTwo that will attempt to avoid points in Repulsives  
        /// </summary>
        public static IEnumerable<Vector3Int> soberPath(Vector3Int pointOne, Vector3Int pointTwo, int pathWidth, HashSet<Vector3Int> Repulsives, int Amplitude = 4, int Frequency = 4, int Phase = 1)
        {
            int dx = pointTwo.x - pointOne.x;
            int dz = pointTwo.z - pointOne.z;
            var set = genLine(pointOne, pointTwo, Amplitude, Amplitude, true, true);

            List<Vector3Int> vals = new List<Vector3Int>() { pointOne };
            bool wrtx = Math.Abs(dx) >= Math.Abs(dz);
            Frequency = Frequency + UnityEngine.Random.Range(0, Phase);
            if (wrtx)
            {
                for (int ind = 1; ind < Frequency; ind++)
                {
                    vals.Add(getPointFurthestFrom(set.ToHashSet(), Repulsives.Where(c => c.x == pointOne.x + ind * (dx / Frequency)).ToHashSet()));
                    //vals.Add(set.ToList().FindAll(s => s.x == pointOne.x + ind * (dx / Frequency)).RandomElement());

                }

            }
            else
            {
                for (int ind = 1; ind < Frequency; ind++)
                {
                    vals.Add(getPointFurthestFrom(set.ToHashSet(), Repulsives.Where(c => c.z == pointOne.z + ind * (dz / Frequency)).ToHashSet()));

                    //vals.Add(set.ToList().FindAll(s => s.z == pointOne.z + ind * (dz / Frequency)).RandomElement());

                }
            }
            vals.Add(pointTwo);

            for (int ind = 0; ind < vals.Count() - 1; ind++)
            {
                foreach (var loc in genLine(vals[ind], vals[ind + 1], pathWidth, pathWidth, true, true))
                {
                    //Prim.Log2("DP: " + loc.ToString());
                    yield return loc;
                }
            }


        }
        public static void RandomPointsInQuads(Vector3Int LotSize, out Vector3Int Q1, out Vector3Int Q2, out Vector3Int Q3, out Vector3Int Q4)
        {
            Q1 = rectArea(new Vector3Int(LotSize.x / 2, 0, LotSize.z / 2), new Vector3Int(LotSize.x / 2, 0, LotSize.z / 2)).RandomElement();
            Q2 = rectArea(new Vector3Int(0, 0, LotSize.z / 2), new Vector3Int(LotSize.x / 2, 0, LotSize.z / 2)).RandomElement();
            Q3 = rectArea(new Vector3Int(0, 0, 0), new Vector3Int(LotSize.x / 2, 0, LotSize.z / 2)).RandomElement();
            Q4 = rectArea(new Vector3Int(LotSize.x / 2, 0, 0), new Vector3Int(LotSize.x / 2, 0, LotSize.z / 2)).RandomElement();
        }
        public static HashSet<Vector3Int> RandomPointsInQuads(Vector3Int LotCorner, Vector3Int LotSize)
        {
            return new HashSet<Vector3Int>()
            {
            rectArea(new Vector3Int(LotCorner.x + LotSize.x / 2, 0, LotCorner.z+LotSize.z / 2), new Vector3Int(LotSize.x / 2, 0, LotSize.z / 2)).RandomElement(),

            rectArea(new Vector3Int(LotCorner.x +0, 0, LotCorner.z+LotSize.z / 2), new Vector3Int(LotSize.x / 2, 0, LotSize.z / 2)).RandomElement(),
            rectArea(new Vector3Int(LotCorner.x +0, 0, LotCorner.z+0), new Vector3Int(LotSize.x / 2, 0,LotSize.z / 2)).RandomElement(),
            rectArea(new Vector3Int(LotCorner.x +LotSize.x / 2, 0, LotCorner.z+0), new Vector3Int(LotSize.x / 2, 0, LotSize.z / 2)).RandomElement()
            };

        }
        public static IEnumerable<Vector3Int> CircleArea(Vector3Int origin, Vector3Int limit, bool hollow = false)
        {
            float h_coef = ((limit.x - origin.x) / 2) + origin.x;
            float k_coef = ((limit.z - origin.z) / 2) + origin.z;
            float A_coef = 1f / (float)((limit.x - h_coef) * (limit.x - h_coef));
            float B_coef = 1f / (float)((limit.z - k_coef) * (limit.z - k_coef));

            foreach (Vector3Int point in rectArea(origin, limit))
            {
                if ((A_coef * ((point.x - h_coef) * (point.x - h_coef))) + (B_coef * ((point.z - k_coef) * (point.z - k_coef))) <= 1)
                {
                    if (!hollow) yield return point;
                    else
                    {
                        if ((A_coef * ((point.x - h_coef) * (point.x - h_coef))) + (B_coef * ((point.z - k_coef) * (point.z - k_coef))) >= 0.9) yield return point;
                    }
                }
            }
        }
        public static IEnumerable<Vector3Int> CircleArea(Vector3Int center, int radx, int radz, bool hollow = false)
        {
            Vector3Int limit = new Vector3Int(center.x + radx, 0, center.z + radz);
            Vector3Int origin = new Vector3Int(center.x - radx, 0, center.z - radz);

            float h_coef = ((limit.x - origin.x) / 2) + origin.x;
            float k_coef = ((limit.z - origin.z) / 2) + origin.z;

            float A_coef = 1f / (float)((limit.x - h_coef) * (limit.x - h_coef));
            float B_coef = 1f / (float)((limit.z - k_coef) * (limit.z - k_coef));

            foreach (Vector3Int point in rectArea(origin, limit))
            {
                if ((A_coef * ((point.x - h_coef) * (point.x - h_coef))) + (B_coef * ((point.z - k_coef) * (point.z - k_coef))) <= 1)
                {
                    if (!hollow) yield return point;
                    else
                    {
                        if ((A_coef * ((point.x - h_coef) * (point.x - h_coef))) + (B_coef * ((point.z - k_coef) * (point.z - k_coef))) >= 0.9) yield return point;
                    }
                }
            }
        }
        public static IEnumerable<Vector3Int> genLine(Vector3Int pointOne, Vector3Int pointTwo, int radWidthx = 0, int radWidthz = 0, bool positive = true, bool mirrored = false)
        {
            foreach (Vector3Int point in GetPointsOnLine(pointOne.x, pointOne.z, pointTwo.x, pointTwo.z))
            {
                foreach (Vector3Int location in pointsBrushed(point, radWidthx, radWidthz, positive, mirrored))
                {
                    yield return location;
                }
            }
        }
        public static IEnumerable<Vector3Int> triArea(Vector3Int pointA, Vector3Int pointB, Vector3Int pointC, bool hollow = false, int radWidthx = 1, int radWidthz = 1, bool positive = true, bool mirrored = true)
        {
            if (!hollow)
            {
                Vector3Int botLeftCorner = new Vector3Int(Math.Min(pointA.x, Math.Min(pointB.x, pointC.x)), 0, Math.Min(pointA.z, Math.Min(pointB.z, pointC.z)));
                Vector3Int topRightCorner = new Vector3Int(Math.Max(pointA.x, Math.Max(pointB.x, pointC.x)), 0, Math.Max(pointA.z, Math.Max(pointB.z, pointC.z)));
                foreach (Vector3Int point in rectArea(botLeftCorner, topRightCorner)) // map.AllCells.ToList().FindAll(v => (v.x >= botLeftCorner.x) && (v.x <= topRightCorner.x) && (v.z >= botLeftCorner.z) && (v.z <= topRightCorner.z)))
                {
                    if (isInsideTriangle(pointA, pointB, pointC, point))
                    {
                        yield return point;
                    }
                }
            }
            else
            {
                for (int ind = 1; ind <= 3; ind++)
                {
                    if (ind == 1)
                    {
                        foreach (Vector3Int point in genLine(pointA, pointB, radWidthx, radWidthz, positive, mirrored))
                        {
                            yield return point;
                        }

                    }
                    if (ind == 2)
                    {
                        foreach (Vector3Int point in genLine(pointA, pointC, radWidthx, radWidthz, positive, mirrored))
                        {
                            yield return point;
                        }

                    }
                    if (ind == 3)
                    {
                        foreach (Vector3Int point in genLine(pointC, pointB, radWidthx, radWidthz, positive, mirrored))
                        {
                            yield return point;
                        }

                    }
                }
            }
        }
        public static double trigArea(Vector3Int pointA, Vector3Int pointB, Vector3Int pointC)
        {

            return Math.Abs((pointA.x * (pointB.z - pointC.z) + pointB.x * (pointC.z - pointA.z) + pointC.x * (pointA.z - pointB.z)) / 2.0);

        }
        public static bool isInsideTriangle(Vector3Int pointA, Vector3Int pointB, Vector3Int pointC, Vector3Int pointP)
        {
            return (trigArea(pointA, pointB, pointC) == trigArea(pointP, pointB, pointC) + trigArea(pointA, pointP, pointC) + trigArea(pointA, pointB, pointP));
        }

        public static IEnumerable<Vector3Int> ProxyNoised(HashSet<Vector3Int> SpaceCells, HashSet<Vector3Int> TargetCells, int radius, float Threshold, float Variance, bool inclusive = false)
        {
            HashSet<Vector3Int> HCirc = new HashSet<Vector3Int>();
            if (inclusive) SpaceCells.RemoveWhere(d => TargetCells.Contains(d));
            foreach (Vector3Int cell in SpaceCells)
            {
                HCirc = CircleArea(new Vector3Int(cell.x - radius, 0, cell.z - radius), new Vector3Int(2 * radius, 0, 2 * radius)).ToHashSet<Vector3Int>();
                var Overlap = (float)TargetCells.Intersect(HCirc).Count();
                var Total = (float)HCirc.Count();
                float ff = Overlap / Total;

                if (ff >= (UnityEngine.Random.value * Variance) + Threshold)
                {
                    yield return cell;
                }

            }
        }

        public static IEnumerable<Vector3Int> OutlineAreaCells(HashSet<Vector3Int> Input)
        {
            Vector3Int tmpPoint;

            foreach (Vector3Int point in Input)
            {
                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dz = -1; dz <= 1; dz++)
                    {

                        tmpPoint = new Vector3Int(point.x + dx, 0, point.z + dz);
                        if (!Input.Contains(tmpPoint))
                        {
                            yield return tmpPoint;
                        }
                    }
                }
            }
        }
        public static IEnumerable<Vector3Int> AreaEdgeCells(HashSet<Vector3Int> Input)
        {

            bool flag = false;
            foreach (Vector3Int point in Input)
            {
                flag = false;
                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dz = -1; dz <= 1; dz++)
                    {
                        if (!Input.Contains(new Vector3Int(point.x + dx, 0, point.z + dz)))
                        {
                            flag = true;
                        }
                    }
                }
                if (flag) yield return point;
            }
        }


        /// <summary>
        /// Finds the point in <paramref name="spaceCells"/> that is furthest away from all points in <paramref name="CellSet"/>.
        /// </summary>
        /// <param name="spaceCells">A set of points to evaluate.</param>
        /// <param name="CellSet">A set of reference points to measure distance against.</param>
        /// <returns>
        /// The point in <paramref name="spaceCells"/> that has the maximum minimum distance to any point in <paramref name="CellSet"/>.
        /// </returns>
        /// <remarks>
        /// This function calculates the minimum distance from each point in <paramref name="spaceCells"/> to all points in <paramref name="CellSet"/>.
        /// It then returns the point in <paramref name="spaceCells"/> with the largest minimum distance.
        /// </remarks>
        public static Vector3Int getPointFurthestFrom(HashSet<Vector3Int> spaceCells, HashSet<Vector3Int> CellSet)
        {
            Dictionary<Vector3Int, float> ds = new Dictionary<Vector3Int, float>();
            foreach (var cell in spaceCells)
            {
                ds.Add(cell, CellSet.Min(s => s.DistanceTo(cell)));

            }
            return ds.Where(c => c.Value == ds.Max(s => s.Value)).First().Key;
        }

        public static HashSet<Vector3Int> FloodFillAlgorithm(Vector3Int start, float targetValue, float[,] grid)
        {
            if (grid == null)
                throw new System.ArgumentNullException(nameof(grid));

            int gridWidth = grid.GetLength(0);
            int gridHeight = grid.GetLength(1);

            // Queue for BFS
            Queue<Vector3Int> queue = new Queue<Vector3Int>();
            // HashSet to store the filled positions
            HashSet<Vector3Int> filled = new HashSet<Vector3Int>();

            // Add the start position to the queue
            queue.Enqueue(start);
            filled.Add(start);

            while (queue.Count > 0)
            {
                Vector3Int current = queue.Dequeue();

                // Check neighbors (in 3D, but we're only checking 2D here since grid is 2D)
                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        Vector3Int neighbor = new Vector3Int(current.x + dx, current.y + dy, current.z);

                        // Check if neighbor is within bounds and matches the target value
                        if (neighbor.x >= 0 && neighbor.x < gridWidth &&
                            neighbor.y >= 0 && neighbor.y < gridHeight &&
                            !filled.Contains(neighbor) &&
                            Mathf.Approximately(grid[neighbor.x, neighbor.y], targetValue))
                        {
                            queue.Enqueue(neighbor);
                            filled.Add(neighbor);
                        }
                    }
                }
            }

            return filled;
        }


        public static Vector3 RelativeVectorTo(this Vector3Int position, Vector3Int destination)
        {
            return (destination - position).ToVector3();

        }
        public static HashSet<Vector3> RelativeVectorsTo(HashSet<Vector3Int> input, Vector3Int position)
        {
            HashSet<Vector3> relativeVectors = new HashSet<Vector3>();

            // Convert each Vector3Int to a Vector3 relative to the given position
            foreach (Vector3Int location in input)
            {
                relativeVectors.Add(position.RelativeVectorTo(location));
            }

            return relativeVectors;
        }
        public static HashSet<Vector3> RelativeVectorsToDirect(HashSet<Vector3Int> input, Vector3Int position)
        {
            HashSet<Vector3> relativeVectors = new HashSet<Vector3>();

            // Convert each Vector3Int to a Vector3 relative to the given position
            foreach (Vector3Int location in input)
            {
                // Calculate the vector from 'position' to 'location'
                Vector3 relativeVector = new Vector3(
                    location.x - position.x,
                    location.y - position.y,
                    location.z - position.z
                );
                
                relativeVectors.Add(relativeVector);
            }

            return relativeVectors;
        }


    }
    public struct Tensor
    {

    }
}

