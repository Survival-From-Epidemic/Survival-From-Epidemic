using System;
using UnityEngine;

namespace _root.PomaTest
{
    [Serializable]
    public class Line
    {
        public Vector3 from;
        public Vector3 to;

        public Line(Vector3 from, Vector3 to)
        {
            this.from = from;
            this.to = to;
        }

        private static int Ccw(Vector3 p1, Vector3 p2, Vector3 p3)
        {
            return (p1.x * p2.z + p2.x * p3.z + p3.x * p1.z - (p1.z * p2.x + p2.z * p3.x + p3.z * p1.x)) switch
            {
                > 0 => 1,
                0 => 0,
                _ => -1
            };
        }

        private static bool BiggerThen(Vector3 a, Vector3 b) => a.x > b.x || (a.x >= b.x && a.y > b.y);

        private bool IsIntersectWith(Line line)
        {
            // 각 라인을 정점으로 변환
            var (p1, p2, p3, p4) = (line.from, line.to, from, to);

            // CCW 계산으로 여부 판정
            var l1 = Ccw(p1, p2, p3) * Ccw(p1, p2, p4);
            var l2 = Ccw(p3, p4, p1) * Ccw(p3, p4, p2);

            // 두 직선이 일직선 상에 존재 여부 확인
            if (l1 != 0 || l2 != 0) return l1 <= 0 && l2 <= 0;

            // 비교를 일반화하기 위한 점 위치 변경 (더 가깝거나 등)
            if (BiggerThen(p1, p2)) (p1, p2) = (p2, p1);
            if (BiggerThen(p3, p4)) (p3, p4) = (p4, p3);

            return !BiggerThen(p3, p2) && !BiggerThen(p1, p4); // 두 선분이 포개어져 있는지 확인
        }

        public Vector3? GetCross(Line line)
        {
            if (!IsIntersectWith(line)) return null;

            var (x1, y1, x2, y2, x3, y3, x4, y4) = (from.x, from.z, to.x, to.z, line.from.x, line.from.z, line.to.x, line.to.z);

            var m = (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4);
            if (m == 0) return null;

            var (k1, k2) = (x1 * y2 - y1 * x2, x3 * y4 - y3 * x4);

            var crossX = (k1 * (x3 - x4) - (x1 - x2) * k2) / m;
            var crossY = (k1 * (y3 - y4) - (y1 - y2) * k2) / m;

            // var (m1, m2) = (
            //     (line.to.y - line.from.y) / (line.to.x - line.from.x),
            //     (to.y - from.y) / (to.x - from.x)
            //     );
            // // if (a1 - a2 <= 0.01f) return null; // 두 직선 평행
            //
            // var crossX = (line.from.x * m1 - line.from.y - line.to.x * m2 + line.to.y) / (m1 - m2);
            // var crossY = m1 * (crossX - line.from.x) + line.from.y;

            return new Vector3(crossX, 0, crossY);
        }
    }
}