using System.Collections.Generic;
using UnityEngine;

namespace Game.Areas.Interfaces
{
    public interface IAreaService
    {
        bool IsPointInPolygon(Vector2 point, IEnumerable<Vector3> vertices);

        void DeformCharacterArea(AreaController areaController, List<Vector3> newAreaVertices);

        Vector2[] TransformVertices2D(IEnumerable<Vector3> vertices);

        Mesh GenerateWalls(List<Vector3> areaVertices);
        Mesh GenerateTop(List<Vector3> areaVertices);

    }
}
