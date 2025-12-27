using UnityEngine;

public class MathWave : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private int numPoints = 50;
    [SerializeField] private float maxX = 20f;
    [SerializeField] private float speed = 1f;
    [SerializeField] private float frequency = 1f;

    private void Update() => UpdateWave();

    private void UpdateWave()
    {
        Vector3 playerPos = transform.position;
        lineRenderer.positionCount = numPoints;

        for (int i = 0; i < numPoints; i++)
        {
            float t = i / (float)(numPoints - 1);
            float x = Mathf.Lerp(-maxX, maxX, t);
            float y = Mathf.Cos(x + Time.time * speed);
            lineRenderer.SetPosition(i, playerPos + new Vector3(x, y, 0));
        }
    }
}