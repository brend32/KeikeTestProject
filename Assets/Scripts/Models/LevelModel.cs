using System;
using System.Threading;
using Configurations.JSON;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Services;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Localization;
using Utils;
using Zenject;

namespace Models
{
    public class LevelModel
    {
        public DrawingShapeModel Shape { get; }
        public Color Color { get; }
        public LocalizedAudioClip StartAudio { get; }

        [Inject]
        public LevelModel(LocalizedAudioClip startAudio, Color color, DrawingShapeModel shape)
        {
            StartAudio = startAudio;
            Color = color;
            Shape = shape;
        }
        
        public class Factory : IFactory<LevelJson, LevelModel>
        {
            private readonly DrawingShapeModel.Factory _shapeFactory;
            private readonly AudiosService _audiosService;

            [Inject]
            public Factory(DrawingShapeModel.Factory shapeFactory, AudiosService audiosService)
            {
                _shapeFactory = shapeFactory;
                _audiosService = audiosService;
            }

            public LevelModel Create(LevelJson json)
            {
                var shape = _shapeFactory.Create(json.Shape);
                var audio = _audiosService.GameStarters.GetClipOrDefault(json.StartAudioId);
                return new LevelModel(audio, json.Color, shape);
            }

            public async UniTask<LevelModel> Create(AssetReferenceT<TextAsset> reference, CancellationToken cancellationToken)
            {
                var asset = await reference.LoadAssetAsync().WithCancellation(cancellationToken);

                var json = JsonConvert.DeserializeObject<LevelJson>(asset.text);
                reference.Release();
            
                return Create(json);
            }
        }
    }
}