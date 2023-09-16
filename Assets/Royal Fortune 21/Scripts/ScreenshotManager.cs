using System;
using System.IO;
using UnityEngine;

[ExecuteAlways]
public class ScreenshotManager : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            // capture screen shot on left mouse button down
            string folderPath = "Assets/Screenshots/"; // the path of your project folder

            if (!Directory.Exists(folderPath)) // if this path does not exist yet
                Directory.CreateDirectory(folderPath);  // it will get created

            var screenshotName = "Screenshot_" +
                                    DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss") + // puts the current time right into the screenshot name
                                    ".png"; // put youre favorite data format here
            ScreenCapture.CaptureScreenshot(Path.Combine(folderPath, screenshotName), 1); // takes the sceenshot, the "2" is for the scaled resolution, you can put this to 600 but it will take really long to scale the image up
            Debug.Log(folderPath + screenshotName); // You get instant feedback in the console
        }
    }
}
