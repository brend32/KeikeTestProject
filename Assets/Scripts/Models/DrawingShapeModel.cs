using System.Collections.Generic;
using System.Linq;
using Configurations.JSON;
using Services;
using UnityEngine.AddressableAssets;
using Zenject;

namespace Models
{
    public class DrawingShapeModel
    {
        public AssetReferenceSprite Shape { get; }
        public IReadOnlyList<DrawingPathModel> Paths { get; }
        public float Width { get; }
        
        [Inject]
        public DrawingShapeModel(AssetReferenceSprite shape, IReadOnlyList<DrawingPathModel> paths, float width)
        {
            Shape = shape;
            Paths = paths;
            Width = width;
        }
        
        public class Factory : IFactory<DrawingShapeJson, DrawingShapeModel>
        {
            private readonly DrawingPathModel.Factory _pathFactory;
            private readonly ShapesService _shapesService;

            [Inject]
            public Factory(DrawingPathModel.Factory pathFactory, ShapesService shapesService)
            {
                _pathFactory = pathFactory;
                _shapesService = shapesService;
            }

            public DrawingShapeModel Create(DrawingShapeJson json)
            {
                var shapeReference = _shapesService.GetShape(json.ShapeAssetId);
                var paths = json.Paths.Select(_pathFactory.Create).ToList();
                return new DrawingShapeModel(shapeReference, paths, json.Width);
            }
        }
    }
}