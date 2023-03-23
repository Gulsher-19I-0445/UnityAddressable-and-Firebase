using System.Collections.Generic;
using System.IO;
using Firebase.Extensions;
using RobinBird.FirebaseTools.Storage.Addressables;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

public class InitFirebaseStorage : MonoBehaviour
{
    void Awake()
    {
        Debug.Log($"Current cache: {Caching.defaultCache.path}");
        
        var cachePaths = new List<string>();
        Caching.GetAllCachePaths(cachePaths);
        foreach (var cachePath in cachePaths)
        {
            Debug.Log($"Cache path: {cachePath}");
        }
        

        //Caching.ClearCache();
        
        Addressables.ResourceManager.ResourceProviders.Add(new FirebaseStorageAssetBundleProvider());
        Addressables.ResourceManager.ResourceProviders.Add(new FirebaseStorageJsonAssetProvider());
        Addressables.ResourceManager.ResourceProviders.Add(new FirebaseStorageHashProvider());

        Addressables.InternalIdTransformFunc += FirebaseAddressablesCache.IdTransformFunc;

        Debug.Log("Getting download size");
        const string downloadAssetKey = "Assets/Scenes/DownloadSizeScene.unity";
        FirebaseAddressablesCache.PreWarmDependencies(downloadAssetKey,
            () =>
            {
                var handler = Addressables.GetDownloadSizeAsync(downloadAssetKey);
                
                handler.Completed += handle =>
                {
                    if (handle.Status == AsyncOperationStatus.Failed)
                    {
                        Debug.LogError($"Get Download size failed because of error: {handle.OperationException}");
                    }
                    else
                    {
                        Debug.Log($"Got download size of: {handle.Result}");
                    }
                
                    Addressables.DownloadDependenciesAsync(downloadAssetKey).Completed +=
                        operationHandle =>
                        {
                            var dependencyList = (List<IAssetBundleResource>)operationHandle.Result;
                            foreach (IAssetBundleResource resource in dependencyList)
                            {
                                AssetBundle assetBundle = resource.GetAssetBundle();
                                Debug.Log($"Downloaded dependency: {assetBundle}");
                            }
                        };
                };
            });

        // Make sure to continue on MAIN THREAD for addressables initialization
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available) {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                //   app = Firebase.FirebaseApp.DefaultInstance;

                Debug.Log("FIREBASE INIT FINISHED");
                FirebaseAddressablesManager.IsFirebaseSetupFinished = true;

                // Set a flag here to indicate whether Firebase is ready to use by your app.
            } else {
                UnityEngine.Debug.LogError(System.String.Format(
                    "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
        });

        
    }
}
