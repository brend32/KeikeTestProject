using System;
using System.Collections;
using UnityEngine;
using Utils;

namespace Animations
{
    public class SpinAnimation : MonoBehaviour
    {
        [SerializeField] private float _speedDegrees = 20f;

        public void Update()
        {
            transform.rotation *= Quaternion.Euler(0, 0, _speedDegrees * Time.deltaTime);
        }
    }
}