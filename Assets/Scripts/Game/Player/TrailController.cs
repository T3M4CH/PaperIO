using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Game.Player
{
    public class TrailController
    {
        public TrailController(MemoryPool<SphereCollider> colliders, Color color, TrailRenderer trailRenderer, Transform transform)
        {
            _colliders = colliders;
            _playerTransform = transform;
            _trailRenderer = Object.Instantiate(trailRenderer, transform);
            _trailRenderer.startColor = color;
            _trailRenderer.endColor = color;
            _trailRenderer.emitting = false;
        }

        private readonly List<SphereCollider> _activeColliders = new();
        private readonly IMemoryPool<SphereCollider> _colliders;
        private readonly Transform _playerTransform;
        private readonly TrailRenderer _trailRenderer;

        public void Spawn()
        {
            var instance = _colliders.Spawn();
            instance.center = _playerTransform.position;
            instance.radius = 0.1f;
            _activeColliders.Add(instance);
            if (_activeColliders.Count <= 1)
            {
                SetTrailActive();
            }
        }

        public bool CheckDistance()
        {
            if (_activeColliders.Count > 0)
            {
                if ((_playerTransform.position - _activeColliders[^1].center).magnitude > 0.3f)
                {
                    if (_activeColliders.Count > 5)
                    {
                        _activeColliders[^5].enabled = true;
                    }
                    
                    return true;
                }
                else
                {
                    return false;
                }
            }
            
            return true;
        }

        private void SetTrailActive()
        {
            if (!_trailRenderer.emitting)
            {
                _trailRenderer.emitting = true;
            }
        }

        public void Clear()
        {
            _trailRenderer.Clear();
            _trailRenderer.emitting = false;
            _activeColliders.ForEach(coll =>
            {
                coll.enabled = false;
                _colliders.Despawn(coll);
            });
            _activeColliders.Clear();
            
        }

        public void ChangeHeight(bool higher)
        {
            if (higher)
            {
                _trailRenderer.transform.localPosition = new Vector3(0, .6f, 0);
            }
            else
            {
                _trailRenderer.transform.localPosition = new Vector3(0, -.5f,0);
            }
        }

        public class Factory : PlaceholderFactory<Transform, Color, TrailController>
        {
        }
    }
}