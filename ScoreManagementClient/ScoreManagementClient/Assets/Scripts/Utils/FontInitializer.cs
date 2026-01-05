#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

/// <summary>
/// 字体初始化器 - 在编辑器启动时检测字体配置
/// </summary>
[InitializeOnLoad]
public class FontInitializer
{
    static FontInitializer()
    {
        // 延迟执行，确保编辑器完全加载
        EditorApplication.delayCall += CheckFontConfig;
    }

    private static void CheckFontConfig()
    {
        // 检查UGUI字体文件是否存在
        string[] fontPaths = new string[]
        {
            "Assets/Fonts/CN_Regular.ttf",
            "Assets/Fonts/CN_Regular.otf",
            "Assets/Fonts/LXGWWenKaiGB-Regular.ttf",
            "Assets/Fonts/LXGWWenKaiGB-Regular.otf"
        };

        bool fontExists = false;
        string foundFontPath = "";

        foreach (string path in fontPaths)
        {
            if (System.IO.File.Exists(System.IO.Path.Combine(Application.dataPath, path.Substring(7))))
            {
                fontExists = true;
                foundFontPath = path;
                break;
            }
        }

        // 检查TMP字体资产是否存在
        bool tmpFontExists = System.IO.File.Exists(System.IO.Path.Combine(Application.dataPath, "Fonts/CN_Regular_TMP.asset"));

        if (!fontExists || !tmpFontExists)
        {
            // 字体不存在，显示提示
            bool shouldShow = EditorPrefs.GetBool("FontInitializer_Shown", false);
            if (!shouldShow)
            {
                string missingFonts = "";
                if (!fontExists) missingFonts += "- UGUI字体 (CN_Regular.ttf)\n";
                if (!tmpFontExists) missingFonts += "- TextMeshPro字体资产 (CN_Regular_TMP.asset)\n";

                int result = EditorUtility.DisplayDialogComplex(
                    "中文字体未配置",
                    "检测到项目中尚未配置中文字体。\n\n" +
                    "缺失的字体：\n" + missingFonts +
                    "\n推荐使用：霞鹜文楷 GB（免费商用）\n" +
                    "授权：SIL Open Font License 1.1\n\n" +
                    "是否现在查看配置说明？",
                    "查看说明",
                    "不再提示",
                    "稍后配置"
                );

                if (result == 0)
                {
                    // 查看说明
                    OpenFontGuide();
                    EditorPrefs.SetBool("FontInitializer_Shown", true);
                }
                else if (result == 1)
                {
                    // 不再提示
                    EditorPrefs.SetBool("FontInitializer_Shown", true);
                }
                // result == 2 表示稍后配置，不做任何操作
            }
        }
        else
        {
            Debug.Log($"[FontInitializer] 检测到字体配置：\nUGUI: {foundFontPath}\nTMP: CN_Regular_TMP.asset");
        }
    }

    private static void OpenFontGuide()
    {
        string guidePath = "Assets/Fonts/字体配置说明.md";
        TextAsset guide = AssetDatabase.LoadAssetAtPath<TextAsset>(guidePath);
        
        if (guide != null)
        {
            // 在编辑器中显示配置说明
            TextWindow window = EditorWindow.GetWindow<TextWindow>("字体配置说明");
            window.content = guide.text;
            window.Show();
        }
        else
        {
            Debug.LogWarning("未找到字体配置说明文件");
        }
    }

    private class TextWindow : EditorWindow
    {
        public string content;
        private Vector2 scrollPosition;

        void OnGUI()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            EditorGUILayout.TextArea(content, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
            EditorGUILayout.EndScrollView();
        }
    }
}
#endif
