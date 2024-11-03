using UnityEngine;

public class PathManager : MonoBehaviour
{
    public static PathManager Instance { get; private set; }
    private LineRenderer pathLine;
    private Vector3 startPoint;
    private Vector3 endPoint;

    private void Awake()
    {
        Instance = this;
        InitializeLine();
    }

    private void InitializeLine()
    {
        GameObject lineObj = new GameObject("PathLine");
        pathLine = lineObj.AddComponent<LineRenderer>();
        pathLine.material = new Material(Shader.Find("Sprites/Default"));
        pathLine.startWidth = 0.001f;
        pathLine.endWidth = 0.001f;
        pathLine.positionCount = 2;
    }

    public void SetPath(WebSocketManager.PathData pathData)
    {
        Transform toothTransform = ModelManager.Instance.GetToothTransform();
        if (toothTransform == null) return;

        // 转换到世界坐标系
        startPoint = toothTransform.TransformPoint(pathData.startPoint);
        endPoint = toothTransform.TransformPoint(pathData.endPoint);

        // 更新路径显示
        pathLine.SetPosition(0, startPoint);
        pathLine.SetPosition(1, endPoint);
    }

    public (float distance, float angle) CalculateToolMetrics(Vector3 toolPosition, Vector3 toolForward)
    {
        // 计算工具到路径的最短距离
        Vector3 pathDirection = (endPoint - startPoint).normalized;
        Vector3 toolToPath = toolPosition - startPoint;
        Vector3 projection = Vector3.Project(toolToPath, pathDirection);
        float distance = Vector3.Distance(toolPosition, startPoint + projection);

        // 计算工具方向与路径的夹角
        float angle = Vector3.Angle(toolForward, pathDirection);

        return (distance, angle);
    }
} 