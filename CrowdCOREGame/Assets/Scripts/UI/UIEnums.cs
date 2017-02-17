namespace CrowdCORE
{
    /// <summary>
    /// Enum: UIBackgroundState
    /// Used to determine what state our UIBackgroundScene should be in.
    /// </summary>
    public enum UIBackgroundState
    {
        None = 0,
        Focus1,
        Focus2
    }

    /// <summary>
    /// Enum: UIBackgroundAnimEvent
    /// Used to let the UIManager know when certain animation states have occured.
    /// </summary>
    public enum UIBackgroundAnimEvent
    {
        None = 0,
        Start,
        End
    }


    public enum UIScreenAnimState
    {
        None = 0,
        Intro,
        Outro
    }

    public enum UIScreenAnimEvent
    {
        None = 0,
        Start,
        End
    }

    /// <summary>
    /// Enum: UINavigationState
    /// Used To hook up navigation to our buttons.
    /// </summary>
    public enum UINavigationType
    {
        None = 0,
        Advance,
        Back
    }

    /// <summary>
    /// Enum: UIScreenId
    /// Used to uniquely identify which screen we are currently on.
    /// </summary>
    public enum UIScreenId
    {
        None = 0,
        MainMenu,
        Splash,
        Warnings,
        Settings,
        PlayerSelection,
        ResultsScreen,
        PauseMenu,
        Credits
    }

    public enum InputPlatform
    {
        Win_Xbox,
        Win_KB
    }

    public enum InputButton
    {
        None,
        A,
        B,
        X,
        Y,
        BACK,
        START,
        RB,
        LB,
        RT,
        LT,
        LStick,
        LStick_Left,
        LStick_Right,
        LStick_Up,
        LStick_Down,
        LStick_Press,
        RStick,
        RStick_Left,
        RStick_Right,
        RStick_Up,
        RStick_Down,
        RStick_Press,
        DPad,
        DPad_Left,
        DPad_Right,
        DPad_Up,
        DPad_Down
    }
}