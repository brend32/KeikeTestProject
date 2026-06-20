using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UI.Extensions;

namespace Game
{
    public class SplineLineRenderer : MonoBehaviour
    {
        public Spline Spline { get; private set; }

        public float Progress
        {
            get => _progress;
            set
            {
                _progress = value;
                Render();
            }
        }
        
        [SerializeField] private UILineRendererList _lineRenderer;
        [SerializeField] private SplineContainer _splineContainer;
        [SerializeField] private int _splineIndex;
        [Header("Amount of points per 100 units of lenght")]
        [Range(1, 100), SerializeField] private float _resolution = 5;
        [Range(0, 1), SerializeField] private float _progress = 1f;
        [Range(0, 1), SerializeField] private float _endCompensation = 0.04f;
        
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
        private float _step;
        private Transform _splineTransform;
        private readonly List<Vector2> _points = new();

        private void Awake()
        {
            if (_splineContainer)
            {
                SetSplineContainer(_splineContainer, _splineIndex);
            }
        }

        private void OnEnable()
        {
            Render();
        }

        public void SetSplineContainer(SplineContainer splineContainer, int splineIndex)
        {
            _splineContainer = splineContainer;
            _splineIndex = splineIndex;
            _splineTransform = splineContainer.transform;
            SetSpline(splineContainer.Splines[splineIndex]);
        }

        public void SetSpline(Spline spline)
        {
            Spline = spline;
            if (Spline != null)
            {
                _step = 1 / (Spline.GetLength() * _resolution / 100f);
            }
            
            Render();
        }

        public void Render()
        {
            _lineRenderer.ClearLines();
            _points.Clear();
            
            if (Spline == null)
                return;

            var amount = Mathf.CeilToInt(1 / _step) + 1;
            var i = 0;
            for (float p = 0; i < amount; p += _step, i++)
            {
                var point = Spline.EvaluatePosition(Mathf.Clamp(p, 0, _progress - _endCompensation));
                var localPoint = (Vector3)point;
                if (_splineTransform)
                {
                    localPoint = LineTransform.InverseTransformPoint(_splineTransform.TransformPoint(point));
                }
                
                _points.Add(localPoint);
                
                if (p > _progress)
                    break;
            }
            
            _lineRenderer.Lines.Add(_points);
        
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