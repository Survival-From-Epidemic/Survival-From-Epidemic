using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace _root.PomaTest
{
    public class PomaTest : MonoBehaviour
    {
        [SerializeField] private bool debug;
        [SerializeField] private GameObject player;
        [SerializeField] private List<Wall> walls;
        [SerializeField] private TextAsset json;
        [SerializeField] private GameObject anchor;

        [Space] [SerializeField] private float playerSize = 1f;
        [SerializeField] private float speed = 2;
        [SerializeField] private Vector3 nextPos;
        [SerializeField] private float calculateTime = 0.5f;
        [SerializeField] private Vector2 moveSpace;

        public List<CrossedLine> crossed;

        private void Start()
        {
            var data = JObject.Parse(json.text);
            walls = new List<Wall>();
            var wallsArray = (JArray)data["Walls"];
            // var playerScale = new Vector3(playerSize, 0, playerSize);
            foreach (JArray wall in wallsArray!)
            {
                walls.Add(new Wall(
                    (
                        from JObject position in wall
                        select new Vector3(position["x"]!.Value<float>(), position["y"]!.Value<float>(), position["z"]!.Value<float>())
                    ).ToArray()
                ).Update(playerSize));
            }

            StartCoroutine(Calculate());
            StartCoroutine(Go());
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireCube(transform.position, new Vector3(moveSpace.x, 0, moveSpace.y));

            if (walls != null && walls.Count > 0)
            {
                Gizmos.color = Color.blue;
                foreach (var wall in walls)
                {
                    Gizmos.DrawCube(wall.center, wall.physicalScale);
                    foreach (var line in wall.physicalLines)
                    {
                        Gizmos.DrawLine(line.from, line.to);
                    }
                }

                // Gizmos.color = Color.cyan;
                // foreach (var line in walls.SelectMany(wall => wall.lines))
                // {
                //     Gizmos.DrawLine(line.from, line.to);
                // }
            }

            var playerPos = player.transform.position;

            Gizmos.color = Color.green;
            Gizmos.DrawLine(playerPos, anchor.transform.position);

            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(playerPos, nextPos);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(nextPos, playerSize);
            // Gizmos.Draw(nextPos, playerSize);

            if (crossed != null && crossed.Count > 0)
            {
                foreach (var cross in crossed)
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawLine(cross.line.from, cross.line.to);
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawSphere(cross.cross, 0.5f);
                }
            }
        }

        private IEnumerator Calculate()
        {
            while (true)
            {
                GetNextPos();
                yield return new WaitForSeconds(calculateTime);
            }
        }

        private void GetNextPos()
        {
            if (debug) Debug.Log("Get Next Pos");
            if ((player.transform.position - anchor.transform.position).sqrMagnitude <= 0.01f)
            {
                if (debug) Debug.Log("Complete");
                return;
            }

            var line = new Line(player.transform.position, anchor.transform.position);

            var crossedList = new List<(Vector3, List<CrossedLine>)>();
            var pos = transform.position;

            foreach (var wall in walls)
            {
                var crossedLine = wall.LineCheck(line);
                if (crossedLine.Count > 0)
                {
                    var (_, castLine) = (crossedLine[0].cross, crossedLine[0].line);

                    var castFromLengthPoint = (castLine.from - line.to).sqrMagnitude;
                    var castToLengthPoint = (castLine.to - line.to).sqrMagnitude;

                    // var castFromLengthAnchor = (castLine.from - line.to).sqrMagnitude;
                    // var castToLengthAnchor = (castLine.to - line.to).sqrMagnitude;

                    var nextPosition = castFromLengthPoint <= castToLengthPoint ? castLine.from : castLine.to;
                    // 하늘색 선 만드는 것으로 해결
                    nextPosition += (nextPos - wall.center).normalized * (playerSize * 0.1f);
                    var (minX, maxX, minY, maxY) = (pos.x - moveSpace.x, pos.x + moveSpace.x, pos.y - moveSpace.y, pos.y + moveSpace.y);
                    if (nextPosition.x >= minX && nextPosition.x <= maxX && nextPosition.y >= minY && nextPosition.y <= maxY)
                    {
                        crossedList.Add((nextPosition, crossedLine));
                    }
                }
            }

            if (crossedList.Count > 0)
            {
                var cross = crossedList
                    .OrderBy(c => (c.Item1 - line.from).sqrMagnitude)
                    .First();
                nextPos = cross.Item1;
                crossed = cross.Item2;
                return;
            }

            if (debug) Debug.Log("Not Found Next Position");
            nextPos = line.to;
        }

        private IEnumerator Go()
        {
            // while (true)
            // {
            while (true)
            {
                player.transform.position = Vector3.MoveTowards(player.transform.position, nextPos, speed * Time.deltaTime);
                yield return null;
            }
        }
    }
}