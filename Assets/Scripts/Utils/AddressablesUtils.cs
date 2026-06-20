using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Utils
{
    public static class AddressablesUtils
    {
        public static T GetAsset<T>(this AssetReferenceT<T> reference) where T : Object
        {
            if (reference.IsValid())
                return reference.Asset as T;

            return null;
        }

        public static void Release(this AssetReference reference)
        {
            if (reference.IsValid())
                reference.ReleaseAsset();
        }
        
        public static AssetReferenceT<T> Copy<T>(this AssetReferenceT<T> assetReference)
            where T : Object
        {
            return new AssetReferenceT<T>(assetReference.AssetGUID)
            {
                SubObjectName = assetReference.SubObjectName
            };
        }
        
        public static AssetReferenceSprite Copy(this AssetReferenceSprite assetReference)
        {
            return new AssetReferenceSprite(assetReference.AssetGUID)
            {
                SubObjectName = assetReference.SubObjectName
            };
        }        
    }
}