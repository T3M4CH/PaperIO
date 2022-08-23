using Game.Areas;
using Game.Areas.Interfaces;
using Game.Player;
using UnityEngine;
using Zenject;

namespace Game.Installers
{
    public class GameInstaller : MonoInstaller
    {
        [SerializeField] private TrailRenderer trailRenderer;
        [SerializeField] private Joystick joystick;
        public override void InstallBindings()
        {
            Container
                .BindInterfacesTo<AreaService>()
                .AsSingle();
            
            Container
                .BindInterfacesTo<JoystickInput>()
                .AsSingle()
                .WithArguments(joystick);

            Container
                .BindInstance(trailRenderer)
                .AsSingle();

            var colliderInstance = new GameObject("collider").AddComponent<SphereCollider>();
            colliderInstance.enabled = false;

            Container
                .BindMemoryPool<SphereCollider, MemoryPool<SphereCollider>>()
                .WithInitialSize(10)
                .FromComponentInNewPrefab(colliderInstance)
                .UnderTransformGroup("SphereColliders");
            
            Container
                .BindFactory<Transform, Material, Color, AreaController, AreaController.Factory>();
            
            Container
                .BindFactory<Transform, Color,TrailController, TrailController.Factory>();
        }
    }
}