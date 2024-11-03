using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Networking;
using System.Threading.Tasks;

public class ModelManager : MonoBehaviour
{
    public static ModelManager Instance { get; private set; }
    private GameObject toothModel;
    private GameObject toolModel;

    private void Awake()
    {
        Instance = this;
    }

    public async void LoadModel(WebSocketManager.ModelData modelData)
    {
        GameObject model = await LoadModelFromUrl(modelData.modelUrl);
        
        if (model != null)
        {
            model.transform.position = modelData.position;
            model.transform.rotation = modelData.rotation;

            if (modelData.modelType == "tooth")
            {
                if (toothModel != null) Destroy(toothModel);
                toothModel = model;
            }
            else if (modelData.modelType == "tool")
            {
                if (toolModel != null) Destroy(toolModel);
                toolModel = model;
            }
        }
    }

    private async Task<GameObject> LoadModelFromUrl(string url)
    {
        using (UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(url))
        {
            await www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"模型加载失败: {www.error}");
                return null;
            }

            AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(www);
            GameObject modelPrefab = bundle.LoadAsset<GameObject>("ModelPrefab");
            GameObject instance = Instantiate(modelPrefab);
            bundle.Unload(false);
            return instance;
        }
    }

    public Transform GetToothTransform()
    {
        return toothModel?.transform;
    }
} 