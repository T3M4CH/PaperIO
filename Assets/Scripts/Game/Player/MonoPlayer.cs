using System;
using System.Collections.Generic;
using Game.Player.Interfaces;
using UnityEngine.AI;
using UnityEngine;
using Game.Areas;
using Zenject;

namespace Game.Player
{
    public class MonoPlayer : MonoBehaviour
    {
        [SerializeField] private Material material;
        [SerializeField] private Color color;
        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private float speed;

        private bool _isStrolling;
        private IJoystickInput _joystickInput;
        private AreaController _areaController;
        private TrailController _trailController;
        private List<MonoArea> _enemiesAreas = new();

        [Inject]
        private void Construct
        (
            IJoystickInput joystickInput,
            AreaController.Factory area,
            TrailController.Factory trail
        )
        {
            _joystickInput = joystickInput;
            _areaController = area.Create(transform, material, color);
            _trailController = trail.Create(transform, color);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                _areaController.AreaVertices.ForEach(x => print(x));
            }
            transform.Rotate(new Vector3(0, _joystickInput.Direction.x, 0));
            var forward = transform.TransformDirection(Vector3.forward);
            agent.velocity = forward * speed;

            if (_areaController.IsOutside)
            {
                if (!_trailController.CheckDistance()) return;
                _isStrolling = true;
                _trailController.Spawn();
                _areaController.AddVertice();
            }
            else
            {
                if (_isStrolling)
                {
                    _isStrolling = false;
                    _trailController.Clear();
                    var areaVertices = new List<Vector3>(_areaController.NewAreaVertices);
                    _areaController.FixVertices();

                    if (_enemiesAreas.Count > 0)
                    {
                        _enemiesAreas.ForEach(area => area.DeformArea(areaVertices));
                    }
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out MonoArea area))
            {
                _trailController.ChangeHeight(true);
                if (area != _areaController.MonoArea)
                {
                    _enemiesAreas.Add(area);
                }
            }

            if (other.TryGetComponent(out SphereCollider sphereCollider))
            {
                Time.timeScale = 0;
                Debug.Log("died");
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out MonoArea area))
            {
                _trailController.ChangeHeight(false);
            }
        }
    }
}