using Game.Areas;
using UnityEngine;
using Zenject;

namespace Game
{
    public class MonoEnemy : MonoBehaviour
    {
        [SerializeField] private Material material;
        [SerializeField] private Color color;
        
        [Inject]
        private void Construct
        (
            AreaController.Factory area
        )
        {
            area.Create(transform, material, color);
        }
    }
}
