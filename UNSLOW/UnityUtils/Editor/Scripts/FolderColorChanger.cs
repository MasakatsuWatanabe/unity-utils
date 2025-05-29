using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UNSLOW.UnityUtils.Editor
{
    /// <summary>
    /// フォルダアイコンの色を変更するエディタ拡張
    /// href: https://tatsuya-koyama.com/devlog/unity/colored-project-view-2/
    /// </summary>
    [InitializeOnLoad]
    public static class FolderColorChanger
    {
        private static readonly string[] Names =
        {
            "Editor",
            "Scripts",
            "Materials",
            "Shaders",
            "Textures",
            "Fonts",
            "Prefabs",
            "Scenes",
            "Resources",
            "Animations",
            "Audio",
            "Physics",
            "Settings",
            "Models",
            "UI",
            "VFX Graph",
            "Visual Effect Graph",
        };

        private static readonly Dictionary<string, int> NameMap = new Dictionary<string, int>();

        static FolderColorChanger()
        {
            for (var i = 0; i < Names.Length; i++)
                NameMap.Add(Names[i], i);
            
            // プロジェクトウィンドウの描画イベントにメソッドを登録
            EditorApplication.projectWindowItemOnGUI += OnProjectWindowGUI;
        }

        private static void OnProjectWindowGUI(string guid, Rect selectionRect)
        {
            // GUIDからアセットのパスを取得
            var path = AssetDatabase.GUIDToAssetPath(guid);

            if (!AssetDatabase.IsValidFolder(path))
                return;

            var originalColor = GUI.color;
            GUI.color = GetColor(path);
            GUI.Box(selectionRect, string.Empty);
            GUI.color = originalColor;
        }
        
        private static Color GetColor(string path)
        {
            var folders = path.Split('/');
            
            for (var i = folders.Length - 1; i >= 0; i--)
            {
                if (!NameMap.TryGetValue(folders[i], out var index))
                    continue;

                var color = Color.HSVToRGB(index / (float)Names.Length, 1f, 1f);
                
                var mag = 2.5f;
                var level = folders.Length - i;
                color = new Color(color.r * mag, color.g * mag, color.b * mag, color.a / level);
                return color;
            }

            return new Color(0,0,0,0);
        }
    }
}
