using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.Splines;
using UnityEngine.UI;

namespace Game
{
    [RequireComponent(typeof(RectTransform))]
    public class DrawDraggingZone : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        public UnityEvent OnFailed;
        public UnityEvent OnFinished;
        public UnityEvent OnStartedDragging;
        public UnityEvent OnDragging;
        
        public float Progress
        {
            get => _progress;
            set => SetProgress(value);
        }
        
        public Spline Spline { get; private set; }
        
        [SerializeField] private SplineContainer _splineContainer;
        [SerializeField] private int _splineIndex;
        [SerializeField] private float _maxDistance = 150f;
        [Header("How far the new point be in the path")]
        [SerializeField] private float _timeBoundDistance = 100f;
        [SerializeField] private float _finishSnapDistance = 20f;

        private float _timeBound;
        private float _progress;
        private bool _failed;
        
        private int _fingerId = -1;
        private RectTransform _splineTransform;
        private Vector3 _endPoint;
        private Vector2 _pointerOffset;

        private void Start()
        {
            SetSplineContainer(_splineContainer, _splineIndex);
        }

        private void OnEnable()
        {
            SetProgress(Progress);
        }

        public void SetSplineContainer(SplineContainer splineContainer, int splineIndex)
        {
            _splineContainer = splineContainer;
            _splineIndex = splineIndex;
            _splineTransform = splineContainer.GetComponent<RectTransform>();
            
            ChangeSpline(splineIndex);
        }

        public void ChangeSpline(int splineIndex)
        {
            Spline = _splineContainer.Splines[splineIndex];

            _endPoint = ToWorld(Spline.EvaluatePosition(1));
            _timeBound = _timeBoundDistance / Spline.GetLength();
        }

        public void ResetFingerState()
        {
            _fingerId = -1;
            _failed = false;
        }

        public void SetProgress(float progress)
        {
            _progress = progress;
            if (Spline == null)
                return;
            
            transform.position = ToWorld(Spline.EvaluatePosition(progress));
        }

        public Vector3 GetWorldPositionOnPath(float progress)
        {
            return ToWorld(Spline.EvaluatePosition(progress));
        }

        private Vector3 ToWorld(Vector3 local)
        {
            return _splineTransform.TransformPoint(local);
        }

        private bool InRange(Vector3 a, Vector3 b, float distance)
        {
            return (a - b).sqrMagnitude < distance * distance;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_fingerId != -1 || enabled == false)
                return;

            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(_splineTransform, eventData.position, null, out var worldPoint))
            {
                _pointerOffset = worldPoint - transform.position;
                _fingerId = eventData.pointerId;
                _failed = false;
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.pointerId != _fingerId) 
                return;
            
            _fingerId = -1;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_fingerId != eventData.pointerId || enabled == false)
                return;
            
            if (_failed)
                return;

            OnStartedDragging?.Invoke();
            
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_splineTransform, eventData.position - _pointerOffset, null, out var localPoint))
            {
                var worldPoint = ToWorld(localPoint);
                if (InRange(worldPoint, transform.position, _maxDistance) == false)
                {
                    _failed = true;
                    OnFailed?.Invoke();
                    return;
                }
                
                GetNearestPoint(Spline, (Vector3)localPoint, _timeBound, Progress, out var nearestPoint, out var t);

                var foundPoint = t < 1;
                var isProgression = t >= Progress;
                if (foundPoint && isProgression)
                {
                    var isCloseToPathEnd = 1 - _progress <= _timeBound;
                    if (InRange(transform.position, _endPoint, _finishSnapDistance) && isCloseToPathEnd)
                    {
                        Progress = 1;
                        OnFinished?.Invoke();
                    }
                    else
                    {
                        Progress = t;
                        OnDragging?.Invoke();
                    }
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, _maxDistance);

            if (Spline != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(_endPoint, _finishSnapDistance);
                
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(ToWorld(Spline.EvaluatePosition(_progress + _timeBound)), _finishSnapDistance);
            }
        }

        // Modified algorithm to account for progression limit
        // when finding the nearest point
        private struct Segment
        {
            public float start, length;

            public Segment(float start, float length)
            {
                this.start = start;
                this.length = length;
            }
        }
        
        public static float GetNearestPoint<T>(T spline,
            float3 point,
            float timeBound,
            float timePoint,
            out float3 nearest,
            out float t,
            int resolution = SplineUtility.PickResolutionDefault,
            int iterations = 2) where T : ISpline
        {
            float distance = float.PositiveInfinity;
            nearest = float.PositiveInfinity;
            Segment segment = new Segment(0f, 1f);
            t = 0f;
            int res = math.min(math.max(SplineUtility.PickResolutionMin, resolution), SplineUtility.PickResolutionMax);

            for (int i = 0, c = math.min(10, iterations); i < c; i++)
            {
                int segments = SplineUtility.GetSubdivisionCount(spline.GetLength() * segment.length, res);
                segment = GetNearestPoint(spline, point, segment, out distance, out nearest, out t, segments, timeBound, timePoint);
            }

            return distance;
        }
        
        private static Segment GetNearestPoint<T>(T spline,
            float3 point,
            Segment range,
            out float distance, out float3 nearest, out float time,
            int segments, float timeBound, float timePoint) where T : ISpline
        {
            distance = float.PositiveInfinity;
            nearest = float.PositiveInfinity;
            time = float.PositiveInfinity;
            Segment segment = new Segment(-1f, 0f);
            Segment segmentTemp = new Segment(-1f, 0f);

            float t0 = range.start;
            float3 a = spline.EvaluatePosition(t0);


            for (int i = 1; i < segments; i++)
            {
                float t1 = range.start + (range.length * (i / (segments - 1f)));
                float3 b = spline.EvaluatePosition(t1);
                var p = SplineMath.PointLineNearestPoint(point, a, b, out var lineParam);
                float dsqr = math.distancesq(p, point);

                if (dsqr < distance)
                {
                    segmentTemp.start = t0;
                    segmentTemp.length = t1 - t0;
                    var newTime = segmentTemp.start + segmentTemp.length * lineParam;
                    if (math.abs(newTime - timePoint) < timeBound)
                    {
                        segment.start = t0;
                        segment.length = t1 - t0;
                        time = newTime;
                    }
                    
                    distance = dsqr;

                    nearest = p;
                }

                t0 = t1;
                a = b;
            }

            distance = math.sqrt(distance);
            return segment;
        }
    }
}