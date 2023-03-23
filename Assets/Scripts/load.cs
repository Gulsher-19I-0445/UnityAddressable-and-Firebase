using System.Collections;
using System.Collections.Generic;
using RobinBird.FirebaseTools.Storage.Addressables;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class load: MonoBehaviour {
// Addressables.ResourceManager.ResourceProviders.Add(new FirebaseStorageAssetBundleProvider());
// Addressables.ResourceManager.ResourceProviders.Add(new FirebaseStorageJsonAssetProvider());
// Addressables.ResourceManager.ResourceProviders.Add(new FirebaseStorageHashProvider());

// This requires Addressables >=1.75 and can be commented out for lower versions
//Addressables.InternalIdTransformFunc += FirebaseAddressablesCache.IdTransformFunc;

private GameObject s;
[SerializeField]
private AssetReference assetReference; [SerializeField] private AssetReferenceGameObject assetReferenceGameObject;

//[SerializeField] private AssetReferenceAudioClip assetReferenceAudioClip;

[SerializeField] private AssetLabelReference assetLabelReference;
private void Awake() {
   
}
private void Start() {
     Addressables.ResourceManager.ResourceProviders.Add(new FirebaseStorageAssetBundleProvider());
        Addressables.ResourceManager.ResourceProviders.Add(new FirebaseStorageJsonAssetProvider());
        Addressables.ResourceManager.ResourceProviders.Add(new FirebaseStorageHashProvider());

        Addressables.InternalIdTransformFunc += FirebaseAddressablesCache.IdTransformFunc;
        FirebaseAddressablesManager.IsFirebaseSetupFinished = true;
    Caching.ClearCache();
    StartCoroutine(LoadGameObjectAndMaterial());
}


IEnumerator LoadGameObjectAndMaterial()
{
    //Load a GameObject
    AsyncOperationHandle<GameObject> goHandle = Addressables.LoadAssetAsync<GameObject>(assetReferenceGameObject);
    yield return goHandle;
    
    if(goHandle.Status == AsyncOperationStatus.Succeeded)
    {
        GameObject obj = goHandle.Result;
        Instantiate(obj);
        //etc...
    }

   

    //Use this only when the objects are no longer needed
    // Addressables.Release(goHandle);
    // Addressables.Release(matHandle);
}
private void Update() {

if (Input.GetKeyDown(KeyCode.T)) {
//GameObject enemyPrefab = await Addressables.LoadAssetAsync<GameObject>(enemyType.toStringValue()).Task;



//Enemy enemy = Instantiate(enemyPrefab, enemiesContainer.transform.position, enemiesContainer.transform.rotation).GetComponent<Enemy>();
Addressables.LoadAssetAsync<GameObject>(assetReference).Completed+=
(asyncOperationHandle)=>{
    if(asyncOperationHandle.Status==AsyncOperationStatus.Succeeded){
        Instantiate(asyncOperationHandle.Result);
    }
    else{
        Debug.Log("Failed");
    }
};
// Addressables.DownloadAssetsAsync<GameObject> (assetLabelReference, (a) => {
//     s = (Instantiate(a) as GameObject);
//     //s.transform.localScale=new Vector3(30f,30f,30f);
//  });

}
}
}