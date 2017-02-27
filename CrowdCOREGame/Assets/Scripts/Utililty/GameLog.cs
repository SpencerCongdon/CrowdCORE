using UnityEngine;

/// <summary>
/// A logging system that allows selective logging based on categories.
/// </summary>
public class GameLog : Singleton<GameLog>
{

    public enum Category
    {
        General,    // After General, try to keep this in alphabetical order
        Audio,
        Camera,
        Crowd,
        Lighting,
        Surfer,
        UI
    }

    [SerializeField] private bool logAllWarnings;

    // Category information
    [SerializeField] private bool logAudio;
    [SerializeField] private Color audioColour;

    [SerializeField] private bool logCamera;
    [SerializeField] private Color cameraColour;

    [SerializeField] private bool logCrowd;
    [SerializeField] private Color crowdColour;

    [SerializeField] private bool logLighting;
    [SerializeField] private Color lightingColour;

    [SerializeField] private bool logSurfer;
    [SerializeField] private Color surferColour;

    [SerializeField] private bool logUI;
    [SerializeField] private Color uiColour;

    #region Public Logging Functions
    public static void Log(object message, Category category = Category.General)
    {
        if(IsLoggingCategory(category)) Debug.Log(FormatCategoryMessage(category, (string)message));
    }

    public static void LogFormat(object message, Category category = Category.General, params object[] args)
    {
        if (IsLoggingCategory(category)) Debug.LogFormat(FormatCategoryMessage(category, (string)message), args);
    }

    public static void LogWarning(object message, Category category = Category.General)
    {
        if (Instance.logAllWarnings || IsLoggingCategory(category)) Debug.LogWarning(FormatCategoryMessage(category, (string)message));
    }

    public static void LogWarningFormat(object message, Category category = Category.General, params object[] args)
    {
        if (Instance.logAllWarnings || IsLoggingCategory(category)) Debug.LogWarningFormat(FormatCategoryMessage(category, (string)message), args);
    }

    public static void LogError(object message, Category category = Category.General)
    {
        // Always log errors for all categories
        Debug.LogError(FormatCategoryMessage(category, (string)message));
    }

    public static void LogErrorFormat(object message, Category category = Category.General, params object[] args)
    {
        // Always log errors for all categories
        Debug.LogErrorFormat(FormatCategoryMessage(category, (string)message), args);
    }
    #endregion // Public Logging Functions

    #region Private Functions
    private static bool IsLoggingCategory(Category category)
    {
        switch(category)
        {
            case Category.General:
                return true;
            case Category.Audio:
                return Instance.logAudio;
            case Category.Camera:
                return Instance.logCamera;
            case Category.Crowd:
                return Instance.logCrowd;
            case Category.Lighting:
                return Instance.logLighting;
            case Category.Surfer:
                return Instance.logSurfer;
            default:
                return false;
        }
    }

    private static string FormatCategoryMessage(Category category, string message)
    {
        string categoryName = "General";
        Color colour = Color.black;

        switch(category)
        {
            case Category.Audio:
                categoryName = "Audio";
                colour = Instance.audioColour;
                break;
            case Category.Camera:
                categoryName = "Camera";
                colour = Instance.cameraColour;
                break;
            case Category.Crowd:
                categoryName = "Crowd";
                colour = Instance.crowdColour;
                break;
            case Category.Lighting:
                categoryName = "Lights";
                colour = Instance.lightingColour;
                break;
            case Category.Surfer:
                categoryName = "Player";
                colour = Instance.surferColour;
                break;
            default:
                // Just use the default settings
                break;
        }
        string colourHex = GetHexForColour(colour);
        return "[<color=" + colourHex + ">" + categoryName + "</color>] " + message;
    }

    private static string GetHexForColour(Color color)
    {
        return string.Format("#{0:X2}{1:X2}{2:X2}ff",
            (int)(color.r * 255),
            (int)(color.g * 255),
            (int)(color.b * 255));
    }
    #endregion // Private Helpers
}
