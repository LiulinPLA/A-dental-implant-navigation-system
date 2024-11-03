using Microsoft.MixedReality.QR;
using Microsoft.MixedReality.Toolkit;
using UnityEngine;
using System;

public class QRCodeManager : MonoBehaviour
{
    public static QRCodeManager Instance { get; private set; }
    private QRCodeWatcher qrWatcher;
    public event Action<Vector3, Quaternion> OnQRCodeDetected;

    private void Awake()
    {
        Instance = this;
        InitializeQRTracking();
    }

    private async void InitializeQRTracking()
    {
        if (QRCodeWatcher.IsSupported())
        {
            try
            {
                await QRCodeWatcher.RequestAccessAsync();
                qrWatcher = new QRCodeWatcher();
                qrWatcher.Added += QRCodeWatcher_Added;
                qrWatcher.Start();
            }
            catch (Exception ex)
            {
                Debug.LogError($"QR初始化失败: {ex.Message}");
            }
        }
    }

    private void QRCodeWatcher_Added(object sender, QRCode qrCode)
    {
        // 获取QR码的位置和旋转
        Vector3 position = qrCode.Coordinate.Position;
        Quaternion rotation = qrCode.Coordinate.Rotation;
        
        OnQRCodeDetected?.Invoke(position, rotation);
        
        // 检测到QR码后启动WebSocket连接
        WebSocketManager.Instance.Connect();
    }
} 