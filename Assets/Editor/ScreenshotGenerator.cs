using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

public class ScreenshotGenerator
{
    [MenuItem("Tools/Generate Screenshot")]

    public static void GenerateScreenshot()
    {
        // Take a screenshot
        string filePath = Application.dataPath + "/Images/screenshot.png";
        int i = 1;
        while (File.Exists(filePath))
        {
            filePath = Application.dataPath + "/Images/screenshot-" + i + ".png";
            i++;
        }
        ScreenCapture.CaptureScreenshot(filePath);

        Debug.Log("Screenshot saved at " + filePath);
    }
}
