using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Areas
{
    public class MonoArea : MonoBehaviour
    {
        private AreaController _areaController;

        public void Initialize(AreaController areaController)
        {
            _areaController = areaController;
        }

        public void DeformArea(IEnumerable<Vector3> newValues)
        {
            var removeList = new List<Vector3>();
            foreach (var value in newValues)
            {
                if (_areaController.IsPointInPolygon(new Vector2(value.x, value.z)))
                {
                    Debug.Log("true");
                    removeList.Add(value);
                }
            }
            
            _areaController.AreaService.DeformCharacterArea(_areaController, removeList);
        }
    }
}