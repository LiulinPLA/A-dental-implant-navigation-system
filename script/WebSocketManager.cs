using UnityEngine;
using WebSocketSharp;
using System;
using Newtonsoft.Json;

public class WebSocketManager : MonoBehaviour
{
    public static WebSocketManager Instance { get; private set; }
    private WebSocket webSocket;
    private string serverUrl = "ws://your-server-url:port";

    [Serializable]
    public class ModelData
    {
        public string modelType; // "tooth" or "tool"
        public string modelUrl;
        public Vector3 position;
        public Quaternion rotation;
    }

    [Serializable]
    public class PathData
    {
        public Vector3 startPoint;
        public Vector3 endPoint;
    }

    [Serializable]
    public class ToolPositionData
    {
        public Vector3 position;
        public Quaternion rotation;
    }

    private void Awake()
    {
        Instance = this;
    }

    public void Connect()
    {
        webSocket = new WebSocket(serverUrl);
        
        webSocket.OnMessage += (sender, e) =>
        {
            ProcessMessage(e.Data);
        };

        webSocket.OnOpen += (sender, e) =>
        {
            Debug.Log("WebSocket Connected");
            RequestModels();
        };

        webSocket.Connect();
    }

    private void ProcessMessage(string message)
    {
        try
        {
            // 根据消息类型处理不同的数据
            if (message.Contains("modelData"))
            {
                ModelData modelData = JsonConvert.DeserializeObject<ModelData>(message);
                ModelManager.Instance.LoadModel(modelData);
            }
            else if (message.Contains("pathData"))
            {
                PathData pathData = JsonConvert.DeserializeObject<PathData>(message);
                PathManager.Instance.SetPath(pathData);
            }
            else if (message.Contains("toolPosition"))
            {
                ToolPositionData toolData = JsonConvert.DeserializeObject<ToolPositionData>(message);
                ToolTracker.Instance.UpdateToolPosition(toolData);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"消息处理错误: {ex.Message}");
        }
    }

    private void RequestModels()
    {
        webSocket.Send("requestModels");
    }

    private void OnDestroy()
    {
        if (webSocket != null && webSocket.IsAlive)
        {
            webSocket.Close();
        }
    }
} 