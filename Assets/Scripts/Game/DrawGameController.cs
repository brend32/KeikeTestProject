using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Splines;

namespace Game
{
    public class DrawGameController : MonoBehaviour
    {
        public UnityEvent OnStartedGame;
        public UnityEvent OnFinishedGame;
        public UnityEvent OnFinishedPath;
        
        public Spline Spline => _splineContainer.Splines[_splineIndex];
        public SplineContainer Container => _splineContainer;

        [SerializeField] private GameStartSound _startSound;
        [SerializeField] private GameEndSound _endSound;
        [SerializeField] private SplineContainer _splineContainer;
        [SerializeField] private int _splineIndex;
        [SerializeField] private SplineLineGroupRenderer _completedLines;
        [SerializeField] private SplineLineRenderer _currentLine;
        [SerializeField] private DrawDraggingZone _draggingZone;
        [SerializeField] private HelpPath _helpPath;

        private void Awake()
        {
            _draggingZone.OnFinished.AddListener(CompletedPath);
            _draggingZone.OnDragging.AddListener(() => _currentLine.Progress = _draggingZone.Progress);
        }

        public void LoadSplines(List<Spline> splines)
        {
            _splineContainer.Splines = splines;
            _draggingZone.SetSplineContainer(_splineContainer, 0);
            _currentLine.SetSplineContainer(_splineContainer, 0);
            CompletedLinesUpTo(-1);
            _helpPath.ClearPath();
            
            SetPathProgress(0);
        }

        public async UniTask StartGame()
        {
            await _startSound.PlayAndWait();
            
            SetIndex(0);
            OnStartedGame?.Invoke();
        }

        public void SetIndex(int splineIndex)
        {
            _splineIndex = Mathf.Clamp(splineIndex, 0, _splineContainer.Splines.Count - 1);
            _draggingZone.SetSplineContainer(_splineContainer, _splineIndex);
            _draggingZone.ResetFingerState();
            _draggingZone.enabled = true;
            
            CompletedLinesUpTo(_splineIndex - 1);
            _currentLine.SetSplineContainer(_splineContainer, _splineIndex);
            
            _helpPath.SetSplineContainer(_splineContainer, _splineIndex);
            _helpPath.PlayPathAnimation();
            
            SetPathProgress(0);
        }

        public void SetPathProgress(float progress)
        {
            _draggingZone.SetProgress(progress);
            _currentLine.Progress = progress;
        }

        private void CompletedLinesUpTo(int index)
        {
            _completedLines.Indexes.Clear();
            for (int i = 0; i <= index; i++)
            {
                _completedLines.Indexes.Add(i);
            }
            _completedLines.SetSplineContainer(_splineContainer);
        }

        public async UniTask CompletedGame()
        {
            var token = destroyCancellationToken;
            
            CompletedLinesUpTo(_splineContainer.Splines.Count - 1);
            _draggingZone.enabled = false;
            _helpPath.ClearPath();
            
            OnFinishedGame?.Invoke();
            await _endSound.PlayAndWait();
        }
        
        public void CompletedPath()
        {
            Debug.Log("Completed Path");
            OnFinishedPath?.Invoke();
            
            if (_splineIndex < _splineContainer.Splines.Count - 1)
            {
                SetIndex(_splineIndex + 1);
            }
            else
            {
                CompletedGame().Forget();
            }
        }
    }
}