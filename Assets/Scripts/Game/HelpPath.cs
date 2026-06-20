using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Splines;

namespace Game
{
    public class HelpPath : MonoBehaviour
    {
        public Spline Spline { get; private set; }
        
        [SerializeField] private Sprite _normalSprite;
        [SerializeField] private Sprite _pathEnd;
        [SerializeField] private SplineContainer _splineContainer;
        [SerializeField] private int _splineIndex;
        [SerializeField] private HelpPathItem _itemPrefab;
        [SerializeField] private float _chainDelay = 0.05f;
        [SerializeField, Min(1)] private float _spacing = 150f;

#if UNITY_EDITOR
        [Space]
        [Header("Editor")]
        [SerializeField] public bool __preview;
#endif

        private float _step;
        private Transform _splineTransform;
        private ObjectPool<HelpPathItem> _pool;
        private readonly List<HelpPathItem> _items = new();
        private Coroutine _animation;

        private void Awake()
        {
            if (_pool == null)
            {
                _pool = new ObjectPool<HelpPathItem>(() =>
                    {
                        if (this == null)
                            return null;
                        
                        return Instantiate(_itemPrefab, transform);
                    },
                    actionOnDestroy: item => DestroyImmediate(item.gameObject),
                    actionOnGet: item => item.gameObject.SetActive(true),
                    actionOnRelease: item =>
                    {
                        if (item)
                        {
                            item.gameObject.SetActive(false);
                        }
                    });
            }

            if (_splineContainer)
            {
                SetSplineContainer(_splineContainer, _splineIndex);
            }
        }

        private void OnEnable()
        {
            CreatePath();
        }

        private void OnDisable()
        {
            ClearPath();
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
                _step = 1 / (Spline.GetLength() / _spacing);
            }
            
            CreatePath();
        }

        public void CreatePath()
        {
            ClearPath();
            
            var amount = Mathf.CeilToInt(1 / _step) + 1;
            var i = 0;
            for (float p = 0; i < amount; p += _step, i++)
            {
                var point = Spline.EvaluatePosition(Mathf.Clamp01(p));
                var localPoint = (Vector3)point;
                if (_splineTransform)
                {
                    localPoint = transform.InverseTransformPoint(_splineTransform.TransformPoint(point));
                }
                
                var item = _pool.Get();
                item.transform.localPosition = localPoint;
                _items.Add(item);

                var isLastPoint = i == amount - 1;
                item.SetImage(isLastPoint ? _pathEnd : _normalSprite);
            } 
        }

        [ContextMenu("Play Animation")]
        public void PlayPathAnimation()
        {
            AnimatePath();
        }

        public Coroutine AnimatePath()
        {
            StopAnimation();
            return _animation = StartCoroutine(Animation());
        }

        public void StopAnimation()
        {
            if (_animation != null)
            {
                StopCoroutine(_animation);
                _animation = null;
            }
        }

        private IEnumerator Animation()
        {
            foreach (var item in _items)
            {
                item.PrepareAnimation();
            }
            
            foreach (var item in _items)
            {
                item.PlayAnimation();
                yield return new WaitForSeconds(_chainDelay);
            }
        }

        public void ClearPath()
        {
            StopAnimation();
            if (_pool == null)
                return;
            
            foreach (var item in _items)
            {
                _pool.Release(item);
            }
            _items.Clear();
        }
        
#if UNITY_EDITOR
        public void OnValidate()
        {
            if (Application.isPlaying)
                return;
            
            if (UnityEditor.EditorApplication.isCompiling || UnityEditor.EditorApplication.isUpdating)
                return;
            
            if (__preview)
            {
                Invoke(nameof(Awake), 0.1f);
            }
            else
            {
                Invoke(nameof(_ClearPreview), 0.1f);
            }
        }

        public void _SyncPreviewInstant()
        {
            if (__preview)
            {
                Awake();
            }
            else
            {
                _ClearPreview();
            } 
        }

        public void _ClearPreview()
        {
            ClearPath();
            if (_pool != null)
            {
                _pool.Clear();
                _pool = null;
            }

            if (transform.childCount > 0)
            {
                while (transform.childCount > 0)
                {
                    UnityEditor.Undo.DestroyObjectImmediate(transform.GetChild(0).gameObject);
                }
            }
        }
#endif
    }
}