/* Rewired Constants
   This list was generated on 2/10/2017 10:25:06 PM
   The list applies to only the Rewired Input Manager from which it was generated.
   If you use a different Rewired Input Manager, you will have to generate a new list.
   If you make changes to the exported items in the Rewired Input Manager, you will need to regenerate this list.
*/

namespace RewiredConsts {
    public static class ACTION {
        // Default
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "Default", friendlyName = "For joining the game")]
        public const int JoinGame = 10;
        // Menu
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "For navigating menus", friendlyName = "Select a menu control")]
        public const int Accept = 0;
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "For navigating menus", friendlyName = "Return from an interface")]
        public const int Back = 1;
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "For navigating menus", friendlyName = "Advance to gameplay")]
        public const int StartGame = 9;
        // Gameplay
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "Actions during gameplay", friendlyName = "Move the surfer")]
        public const int MoveVertical = 2;
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "Actions during gameplay", friendlyName = "Move left an right in the crowd")]
        public const int MoveHorizontal = 3;
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "Actions during gameplay", friendlyName = "Punch with your arms")]
        public const int Punch = 4;
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "Actions during gameplay", friendlyName = "Kick with your legs")]
        public const int Kick = 5;
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "Actions during gameplay", friendlyName = "Rotate the surfer")]
        public const int RotateClockwise = 6;
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "Actions during gameplay", friendlyName = "Rotate the surfer")]
        public const int RotateCounterClockwise = 7;
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "Actions during gameplay", friendlyName = "Bounce the surfer")]
        public const int Hop = 8;
    }
    public static class Category {
        public const int Default = 0;
        public const int Assignment = 3;
        public const int Menu = 1;
        public const int Surfer = 2;
    }
    public static class Layout {
        public static class Joystick {
            public const int Default = 0;
        }
        public static class Keyboard {
            public const int Default = 0;
        }
        public static class Mouse {
            public const int Default = 0;
        }
        public static class CustomController {
            public const int Default = 0;
        }
    }
}
