using System.Collections.Generic;
using System.Linq;
using Game.Areas.Interfaces;
using UnityEngine;

namespace Game.Areas
{
    public class AreaService : IAreaService
    {
        public bool IsPointInPolygon(Vector2 point, IEnumerable<Vector3> vertices)
        {
            var polygon = TransformVertices2D(vertices);
            int polygonLength = polygon.Length, i = 0;
            var inside = false;
            float pointX = point.x, pointY = point.y;
            var endPoint = polygon[polygonLength - 1];
            var endX = endPoint.x;
            var endY = endPoint.y;
            while (i < polygonLength)
            {
                var startX = endX;
                var startY = endY;
                endPoint = polygon[i++];
                endX = endPoint.x;
                endY = endPoint.y;
                inside ^= endY > pointY ^ startY > pointY &&
                          pointX - endX < (pointY - endY) * (startX - endX) / (startY - endY);
            }

            return inside;
        }

        public Mesh GenerateWalls(List<Vector3> areaVertices)
        {
            var sideVertices = new List<Vector3>();

            for (int i = 0; i < areaVertices.Count; i++)
            {
                var nextId = i == areaVertices.Count - 1 ? 0 : i + 1;
                sideVertices.Add(areaVertices[nextId]);
                sideVertices.Add(areaVertices[i] + Vector3.up);
                sideVertices.Add(areaVertices[i]);
                sideVertices.Add(areaVertices[nextId]);
                sideVertices.Add(areaVertices[nextId] + Vector3.up);
                sideVertices.Add(areaVertices[i] + Vector3.up);
            }

            var triangles = new int[sideVertices.Count];

            for (int i = 0; i < triangles.Length; i += 6)
            {
                triangles[i] = triangles[i + 3] = i;
                triangles[i + 1] = triangles[i + 5] = i + 1;
                triangles[i + 2] = i + 2;
                triangles[i + 4] = i + 4;
            }

            var msh = new Mesh();
            msh.SetVertices(sideVertices);
            msh.SetTriangles(triangles, 0);
            msh.RecalculateNormals();

            return msh;
        }

        public Mesh GenerateTop(List<Vector3> vertices)
        {
            Triangulator tr = new Triangulator(TransformVertices2D(vertices));
            int[] indices = tr.Triangulate();

            Mesh msh = new Mesh();
            msh.vertices = vertices.ToArray();
            msh.triangles = indices;
            msh.RecalculateNormals();

            return msh;
        }

        public Vector2[] TransformVertices2D(IEnumerable<Vector3> vertices) =>
            vertices.Select(vertex => new Vector2(vertex.x, vertex.z)).ToArray();

        public void DeformCharacterArea(AreaController areaController, List<Vector3> newAreaVertices)
        {
            int newAreaVerticesCount = newAreaVertices.Count;
            if (newAreaVerticesCount > 0)
            {
                List<Vector3> areaVertices = areaController.AreaVertices;
                int startPoint = GetClosestAreaVertice(newAreaVertices[0], areaVertices);
                int endPoint = GetClosestAreaVertice(newAreaVertices[newAreaVerticesCount - 1], areaVertices);

                List<Vector3> redundantVertices = new List<Vector3>();
                for (int i = startPoint; i != endPoint; i++)
                {
                    if (i == areaVertices.Count)
                    {
                        if (endPoint == 0)
                        {
                            break;
                        }

                        i = 0;
                    }

                    redundantVertices.Add(areaVertices[i]);
                }

                redundantVertices.Add(areaVertices[endPoint]);

                List<Vector3> tempAreaClockwise = new List<Vector3>(areaVertices);
                for (int i = 0; i < newAreaVerticesCount; i++)
                {
                    tempAreaClockwise.Insert(i + startPoint, newAreaVertices[i]);
                }

                tempAreaClockwise = tempAreaClockwise.Except(redundantVertices).ToList();
                float clockwiseArea = Mathf.Abs(tempAreaClockwise.Take(tempAreaClockwise.Count - 1).Select((p, i) =>
                    (tempAreaClockwise[i + 1].x - p.x) * (tempAreaClockwise[i + 1].z + p.z)).Sum() / 2f);

                redundantVertices.Clear();
                for (int i = startPoint; i != endPoint; i--)
                {
                    if (i == -1)
                    {
                        if (endPoint == areaVertices.Count - 1)
                        {
                            break;
                        }

                        i = areaVertices.Count - 1;
                    }

                    redundantVertices.Add(areaVertices[i]);
                }

                redundantVertices.Add(areaVertices[endPoint]);

                var tempAreaCounterclockwise = new List<Vector3>(areaVertices);
                for (int i = 0; i < newAreaVerticesCount; i++)
                {
                    tempAreaCounterclockwise.Insert(startPoint, newAreaVertices[i]);
                }

                tempAreaCounterclockwise = tempAreaCounterclockwise.Except(redundantVertices).ToList();
                var counterclockwiseArea = Mathf.Abs(tempAreaCounterclockwise.Take(tempAreaCounterclockwise.Count - 1)
                                                         .Select((p, i) =>
                                                             (tempAreaCounterclockwise[i + 1].x - p.x) *
                                                             (tempAreaCounterclockwise[i + 1].z + p.z)).Sum() /
                                                     2f);

                areaController.AreaVertices =
                    clockwiseArea > counterclockwiseArea ? tempAreaClockwise : tempAreaCounterclockwise;
            }

            areaController.UpdateArea();
        }

        private static int GetClosestAreaVertice(Vector3 fromPos, List<Vector3> vertices)
        {
            var closest = -1;
            var closestDist = Mathf.Infinity;
            for (int i = 0; i < vertices.Count; i++)
            {
                var dist = (vertices[i] - fromPos).magnitude;
                if (!(dist < closestDist)) continue;
                closest = i;
                closestDist = dist;
            }

            return closest;
        }
    }
}