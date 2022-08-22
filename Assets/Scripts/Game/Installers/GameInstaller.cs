using UnityEngine;
using Zenject;

namespace Game.Installers
{
    public class GameInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            var colliderInstance = new GameObject("collider").AddComponent<SphereCollider>();

            Container
                .BindMemoryPool<SphereCollider, MemoryPool<SphereCollider>>()
                .WithInitialSize(10)
                .FromComponentInNewPrefab(colliderInstance)
                .UnderTransformGroup("SphereColliders");

            Destroy(colliderInstance.gameObject);
        }
    }
}