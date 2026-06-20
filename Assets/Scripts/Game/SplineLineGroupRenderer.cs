using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UI.Extensions;

namespace Game
{
    public class SplineLineGroupRenderer : MonoBehaviour
    {
        public IReadOnlyList<Spline> Splines => _splines;
        public List<int> Indexes => _indexes;
        
        [SerializeField] private UILineRendererList _lineRenderer;
        [SerializeField] private SplineContainer _splineContainer;
        [SerializeField] private List<int> _indexes;
        [Header("Amount of points per 100 units of lenght")]
        [Range(1, 100), SerializeField] private float _resolution = 5;

        private Transform LineTransform
        {
            get
            {
                if (_lineTransform)
                    return _lineTransform;
                return _lineTransform = _lineRenderer.transform;
            }
        }

        private Transform _lineTransform;
        private Transform _splineTransform;
        private readonly List<Spline> _splines = new();

        private void Awake()
        {
            if (_splineContainer)
            {
                SetSplineContainer(_splineContainer);
            }
        }

        private void OnEnable()
        {
            Render();
        }

        public void SetSplineContainer(SplineContainer splineContainer)
        {
            _splineContainer = splineContainer;
            _splines.Clear();
            foreach (var splineIndex in _indexes)
            {
                _splines.Add(splineContainer.Splines[splineIndex]);
            }
            _splineTransform = splineContainer.transform;
            Render();
        }

        public void Render()
        {
            _lineRenderer.ClearLines();
            
            if (Splines.Count == 0)
                return;
            
            foreach (var spline in Splines)
            {
                var points = new List<Vector2>();
                var step = 1 / (spline.GetLength() * _resolution / 100f);
                var i = 0;
                var amount = Mathf.CeilToInt(1 / step) + 1;
                for (float p = 0; i < amount; p += step, i++)
                {
                    var point = spline.EvaluatePosition(Mathf.Clamp01(p));
                    var localPoint = (Vector3)point;
                    if (_splineTransform)
                    {
                        localPoint = LineTransform.InverseTransformPoint(_splineTransform.TransformPoint(point));
                    }

                    points.Add(localPoint);
                }
                
                _lineRenderer.Lines.Add(points);
            }
            
            _lineRenderer.SetAllDirty();
        }
        
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (Application.isPlaying)
                return;
            
            Awake();
        }
#endif
    }
}