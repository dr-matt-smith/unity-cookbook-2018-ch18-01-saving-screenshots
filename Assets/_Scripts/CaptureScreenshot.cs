using System.Collections;
using UnityEngine;
using System.IO;

/* ----------------------------------------
 * class to demonstrate how to take screenshots at runtime
 * using two different methods: Unity's built-in CaptureScreenshot function
 * and a combination of ReadPixels, EncodeToJPG (or EncodeToPNG) and WriteAllBytes.
 */
public class CaptureScreenshot : MonoBehaviour 
{
    // String variable for a prefix for the screenshot's name 
    public string prefix = "Screenshot";

    // A selector for the screenshot taking method
    public enum CaptureMethod {
        SCREENSHOT_PNG,
        READ_PIXELS_PNG,
        READ_PIXELS_JPG
    };

    // Set initial value for 'method' as 'captureScreenshotPng'
    public CaptureMethod captureMethod = CaptureMethod.SCREENSHOT_PNG;

    // Int value by which to increase the resolution of screenshots taken via CaptureScreenshot method
    public int captureScreenshotScale = 1;  

    // A slider from 0 to 100 from which to set JPG quality
    [Range(0, 100)]

    // Int variable for JPG quality
    public int jpgQuality = 75;

    // Private Texture2D variable where to storage image capture via ReadPixels
    private Texture2D texture;

    // Int variable for the screen's width
    private int sw;

    // Int variable for the screen's height
    private int sh;

    // Rect variable for the image capture area
    private Rect sRect;

    // String for date and time the screenshot was taken, to be added to the image file's name
    string date;

    /* ----------------------------------------
     * At beginning of frame record the screen height, width
     * and create a Rect object (sRect) for use by the byte-writing methods
     */
    void Start()
    {
        // record screen width / height
        sw = Screen.width;
        sh = Screen.height;
        // create rect to match current screen size
        sRect = new Rect(0,0,sw,sh);
    }

    /* ----------------------------------------
     * During Update, detect if the 'P' key was pressed, 
     * call method TakeShot
     */
    void  Update (){
        if (Input.GetKeyDown (KeyCode.P)){
            // IF 'P' is pressed on the keyboard, THEN call method to record screenshot
            TakeShot();
        }
    }

    /* ----------------------------------------
     * we want to record screenshot NOW
     * call appropriate built-in or custom method
     * based on user preferences
     */
    private void TakeShot()
    {
        // create a string of the date and date NOW - just as we are about to save screenshot
        date = System.DateTime.Now.ToString("_d-MMM-yyyy-HH-mm-ss-f");

        // IF selected method is 'captureScreenshotPng'...
        if (CaptureMethod.SCREENSHOT_PNG == captureMethod){
            // THEN use Unity's CaptureScreenshot function to capture screenshot, increased by 'captureScreenshotScale';
            ScreenCapture.CaptureScreenshot(prefix + date + ".png", captureScreenshotScale);    
        } else {
            // ELSE, call co-routine to use one of the read-pixels methods
            StartCoroutine(ReadPixels());
        }

    }
    
    /* ----------------------------------------
     * A coroutine where the screenshot is taken according to the preferences 
     * set by the user 
     */
    IEnumerator  ReadPixels (){
        // bytes array for converting pixels to image format
        byte[] bytes;
    
        // Wait for the end of the frame, so GUI elements are included in the screenshot
        yield return new WaitForEndOfFrame();

        // Create new Texture2D variable of the same size as the image capture area
        texture = new Texture2D (sw,sh,TextureFormat.RGB24,false);  

        // Read Pixels from the capture area
        texture.ReadPixels(sRect,0,0);

        // Apply pixels read com capture area into 'texture'
        texture.Apply();
        
        // IF selected method is 'ReadPixelsJpg'...
        switch(captureMethod){
            case CaptureMethod.READ_PIXELS_JPG:
                // store as bytes the texture encoded to JPG (using 'jpgQuality' as quality settings) 
                bytes = texture.EncodeToJPG(jpgQuality);
                WriteBytesToFile(bytes, ".jpg");
                break;

            case CaptureMethod.READ_PIXELS_PNG:
            default:
                // store as bytes the texture encoded to PNG
                bytes = texture.EncodeToPNG();
                WriteBytesToFile(bytes, ".png");
                break;
        }
    }

    /* ----------------------------------------
     * given screen data (bytes) and text file extension (format)
     * write the bytes to a file with approppriate path/name
     */
    void WriteBytesToFile(byte[] bytes, string fileExtension)
    {
        // Destroy 'texture' 
        Destroy (texture);

        // in form:
        // Screenshot_29-Jan-2018-07-33-26-6.png
        string filename = prefix + date + fileExtension;

        // Write bytes as file named after 'prefix", 'date' and 'format' strings
        string path = Application.dataPath;
        path = Path.Combine(path, "..");
        path = Path.Combine(path, filename);

        File.WriteAllBytes(path, bytes);
    }
} 

