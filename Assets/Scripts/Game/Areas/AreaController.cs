using System.Collections.Generic;
using Game.Areas.Interfaces;
using UnityEngine;
using Zenject;

namespace Game.Areas
{
    public class AreaController
    {
        public AreaController
        (
            IAreaService areaService,
            Transform transform,
            Material material,
            Color color
        )
        {
            AreaService = areaService;
            _playerTransform = transform;
            Initialize(material, color, PlayerPosition);
        }

        private MeshCollider _meshCollider;
        private MeshFilter _areaFilter;
        private MeshFilter _areaOutlineFilter;
        
        public readonly IAreaService AreaService;
        private readonly Transform _playerTransform;
        public readonly List<Vector3> NewAreaVertices = new();

        public void UpdateArea()
        {
            if (!_areaFilter) return;
            var areaMesh = AreaService.GenerateTop(AreaVertices);
            _areaFilter.mesh = areaMesh;
            _meshCollider.sharedMesh = areaMesh;
            
            var wallMesh = AreaService.GenerateWalls(AreaVertices);
            _areaOutlineFilter.mesh = wallMesh;
        }

        public bool IsPointInPolygon(Vector2 position)
        {
            return AreaService.IsPointInPolygon(position, AreaVertices);
        }

        public void AddVertice()
        {
            NewAreaVertices.Add(new Vector3(PlayerPosition.x,.5f,PlayerPosition.z));   
        }

        public void FixVertices()
        {
            AreaService.DeformCharacterArea(this,NewAreaVertices);
            NewAreaVertices.Clear();
        }

        private void Initialize(Material material, Color color, Vector3 spawnPosition)
        {
            var area = new GameObject("area");
            MonoArea = area.AddComponent<MonoArea>();
            MonoArea.Initialize(this);
            var areaTransform = area.transform;
            areaTransform.position += Vector3.up / 2;
            _areaFilter = area.gameObject.AddComponent<MeshFilter>();
            var areaMeshRend = area.gameObject.AddComponent<MeshRenderer>();
            _meshCollider = area.gameObject.AddComponent<MeshCollider>();
            areaMeshRend.material = material;
            areaMeshRend.material.color = color;

            var areaOutline = new GameObject("areaWall");
            var areaOutlineTransform = areaOutline.transform;
            areaOutlineTransform.position -= Vector3.up / 2;
            areaOutlineTransform.SetParent(areaTransform);
            _areaOutlineFilter = areaOutline.AddComponent<MeshFilter>();
            var areaOutlineMeshRend = areaOutline.AddComponent<MeshRenderer>();
            areaOutlineMeshRend.material = material;
            areaOutlineMeshRend.material.color = new Color(color.r * .7f, color.g * .7f, color.b * .7f);

            const float step = 360f / 45;
            for (int i = 0; i < 45; i++)
            {
                AreaVertices.Add(spawnPosition +
                                 Quaternion.Euler(new Vector3(0, step * i, 0)) * Vector3.forward * 3);
            }

            UpdateArea();
        }

        public MonoArea MonoArea { get; private set; }

        public bool IsOutside => !AreaService.IsPointInPolygon(new Vector2(PlayerPosition.x, PlayerPosition.z), AreaVertices);

        private Vector3 PlayerPosition => _playerTransform.position;
        public List<Vector3> AreaVertices { get; set; } = new();

        public class Factory : PlaceholderFactory<Transform, Material, Color, AreaController>
        {
        }
    }
}