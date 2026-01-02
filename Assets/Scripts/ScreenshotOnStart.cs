using System.IO;
using UnityEngine;

public class ScreenshotOnStart : MonoBehaviour
{
    [Header("Screenshot Settings")]
    [SerializeField] private string folderName = "Images";
    [SerializeField] private string fileName = "screenshot.png"; 
    [SerializeField] private int superSize = 1;

    private void Start()
    {
        TakeScreenshot();
    }

    private void TakeScreenshot()
    {
        string folderPath = Path.Combine(Application.dataPath, folderName);
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
        
        string filePath = Path.Combine(folderPath, fileName);
        
        ScreenCapture.CaptureScreenshot(filePath, superSize);

        Debug.Log($"Screenshot saved at: {filePath}");
    }
}