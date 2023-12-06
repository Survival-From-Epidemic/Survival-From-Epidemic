using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _root.PomaTest
{
    [Serializable]
    public class Wall
    {
        public Vector3[] physicalPoints;
        public Vector3[] points;
        public Vector3 physicalScale;
        public Vector3 center;
        public Line[] physicalLines;
        public Line[] lines;

        public Wall(Vector3[] physicalPoints)
        {
            this.physicalPoints = physicalPoints;
        }

        public Wall Update(float playerSize)
        {
            physicalScale = new Vector3(physicalPoints[0].x - physicalPoints[3].x, 0, physicalPoints[0].z - physicalPoints[1].z);
            center = new Vector3(Mathf.Abs(physicalScale.x) / 2, 0, Mathf.Abs(physicalScale.z) / 2) + physicalPoints[3];
            points = physicalPoints
                .Select(p =>
                {
                    var pivot = p - center;
                    return new Vector3(
                        p.x + (pivot.x > 0 ? playerSize : -playerSize),
                        p.y + (pivot.y > 0 ? playerSize : -playerSize),
                        p.z + (pivot.z > 0 ? playerSize : -playerSize)
                    );
                })
                .ToArray();

            physicalLines = new[]
            {
                new Line(physicalPoints[0], physicalPoints[1]),
                new Line(physicalPoints[2], physicalPoints[3]),
                new Line(physicalPoints[1], physicalPoints[3]),
                new Line(physicalPoints[0], physicalPoints[2])
            };

            lines = new[]
            {
                new Line(points[0], points[1]),
                new Line(points[2], points[3]),
                new Line(points[1], points[3]),
                new Line(points[0], points[2])
            };

            return this;
        }

        public List<CrossedLine> LineCheck(Line line) => (
            from l in lines
            let cross = line.GetCross(l)
            where cross != null
            orderby (cross.Value - line.@from).sqrMagnitude
            select new CrossedLine
            {
                cross = cross.Value,
                line = l
            }
        ).ToList();
        // if (crosses.Count <= 0) return null;
        //
        // foreach (var (value, l) in crosses)
        // {
        //     Gizmos.color = Color.yellow;
        //     Gizmos.DrawLine(l.from, l.to);
        // }
        //
        // return crosses[^1];
    }
}