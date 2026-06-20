using System.Collections.Generic;
using Configurations.JSON;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;
using Zenject;

namespace Models
{
    public class DrawingPathModel
    {
        public Spline Path { get; }

        [Inject]
        public DrawingPathModel(Spline path)
        {
            Path = path;
        }
        
        public class Factory : IFactory<DrawingPathJson, DrawingPathModel>
        {
            public DrawingPathModel Create(DrawingPathJson json)
            {
                return new DrawingPathModel(new Spline(json.Points, json.Closed));
            }
        }
    }
}