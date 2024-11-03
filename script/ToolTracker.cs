using UnityEngine;
using TMPro;

public class ToolTracker : MonoBehaviour
{
    public static ToolTracker Instance { get; private set; }
    public TextMeshPro metricsDisplay; // HoloLens中显示距离和角度的文本

    private void Awake()
    {
        Instance = this;
    }

    public void UpdateToolPosition(WebSocketManager.ToolPositionData toolData)
    {
        Transform toothTransform = ModelManager.Instance.GetToothTransform();
        if (toothTransform == null) return;

        // 转换工具位置到世界坐标系
        Vector3 worldPosition = toothTransform.TransformPoint(toolData.position);
        Vector3 worldForward = toothTransform.TransformDirection(toolData.rotation * Vector3.forward);

        // 计算与目标路径的距离和角度
        var (distance, angle) = PathManager.Instance.CalculateToolMetrics(worldPosition, worldForward);

        // 更新显示
        UpdateMetricsDisplay(distance, angle);
    }

    private void UpdateMetricsDisplay(float distance, float angle)
    {
        if (metricsDisplay != null)
        {
            metricsDisplay.text = $"距离: {distance:F2}mm\n角度: {angle:F1}°";
        }
    }
} 