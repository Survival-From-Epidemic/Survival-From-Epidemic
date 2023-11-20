using System;

namespace _root.Scripts.Managers.UI
{
    public enum UIElements
    {
        SignIn,
        SignUp,
        GameStart,
        GameResult,
        InGameMenu,
        InGame
    }

    public enum GameEndType
    {
        Win,
        Banbal,
        Authority
    }

    public enum UIScenes
    {
        MainScene,
        GameScene
    }

    public static class EnumUtils
    {
        public static string GetString(this UIScenes type)
        {
            return type switch
            {
                UIScenes.MainScene => "World",
                UIScenes.GameScene => "World",
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }
    }
}