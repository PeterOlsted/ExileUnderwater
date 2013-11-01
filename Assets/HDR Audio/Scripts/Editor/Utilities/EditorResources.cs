using UnityEditor;
using UnityEngine;

namespace HDRAudio
{
    public static class EditorResources
    {
        public static Texture Plus;
        public static Texture Minus;
        public static Texture White;
        public static Texture ChangingColor;
        public static Texture Background;
        public static Texture Up;
        public static Texture Down;

        public static void Reload()
        {

            if (Plus == null)
                Plus = LoadTexture("Plus");
            if (Minus == null)
                Minus = LoadTexture("Minus");
            if (Up == null)
                Up = LoadTexture("Up");
            if (Down == null)
                Down = LoadTexture("Down");
            if (Background == null)
                Background = LoadTexture("SelectedBackground");
            if (White == null)
                White = LoadTexture("White");
        }

        private static Texture LoadTexture(string name)
        {
            return Resources.Load(FolderSettings.IconPath + name, typeof (Texture)) as Texture;
        }
    }

    
    
}
