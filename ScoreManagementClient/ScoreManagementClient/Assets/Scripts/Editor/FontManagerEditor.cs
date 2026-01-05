using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// FontManager编辑器扩展 - 提供批量配置字体的功能（支持UGUI Text和TextMeshPro）
/// </summary>
public class FontManagerEditor
{
    #region UGUI Text 配置

    [MenuItem("Tools/Font Manager/配置当前场景所有UGUI Text使用CN_Regular字体")]
    public static void ConfigureAllTextsInScene()
    {
        Font cnRegular = AssetDatabase.LoadAssetAtPath<Font>("Assets/Fonts/CN_Regular.ttf");
        
        if (cnRegular == null)
        {
            EditorUtility.DisplayDialog("字体未找到", 
                "未找到 CN_Regular 字体文件！\n\n" +
                "请确保：\n" +
                "1. 已下载霞鹜文楷GB字体\n" +
                "2. 将字体文件导入到 Assets/Fonts/ 目录\n" +
                "3. 将字体文件重命名为 CN_Regular.ttf", 
                "确定");
            return;
        }

        Text[] allTexts = GameObject.FindObjectsOfType<Text>(true);
        int count = 0;

        foreach (Text text in allTexts)
        {
            if (text.font != cnRegular)
            {
                Undo.RecordObject(text, "Change Font");
                text.font = cnRegular;
                EditorUtility.SetDirty(text);
                count++;
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog("配置完成", 
            $"已配置 {count} 个UGUI Text组件使用CN_Regular字体。\n\n" +
            $"总Text组件数：{allTexts.Length}", 
            "确定");

        Debug.Log($"[FontManager] 已配置 {count} 个UGUI Text组件使用CN_Regular字体");
    }

    [MenuItem("Tools/Font Manager/重置当前场景所有UGUI Text字体为默认")]
    public static void ResetAllTextsInScene()
    {
        Font defaultFont = Resources.GetBuiltinResource<Font>("Arial.ttf");
        Text[] allTexts = GameObject.FindObjectsOfType<Text>(true);
        int count = 0;

        foreach (Text text in allTexts)
        {
            if (text.font != defaultFont)
            {
                Undo.RecordObject(text, "Reset Font");
                text.font = defaultFont;
                EditorUtility.SetDirty(text);
                count++;
            }
        }

        AssetDatabase.SaveAssets();

        EditorUtility.DisplayDialog("重置完成", 
            $"已重置 {count} 个UGUI Text组件的字体为默认字体。\n\n" +
            $"总Text组件数：{allTexts.Length}", 
            "确定");

        Debug.Log($"[FontManager] 已重置 {count} 个UGUI Text组件的字体为默认字体");
    }

    [MenuItem("Tools/Font Manager/查找当前场景中未使用CN_Regular字体的Text")]
    public static void FindTextsWithoutCnRegular()
    {
        Font cnRegular = AssetDatabase.LoadAssetAtPath<Font>("Assets/Fonts/CN_Regular.ttf");
        
        if (cnRegular == null)
        {
            Debug.LogWarning("[FontManager] 未找到 CN_Regular 字体文件");
            return;
        }

        Text[] allTexts = GameObject.FindObjectsOfType<Text>(true);
        List<Text> nonCnRegularTexts = new List<Text>();

        foreach (Text text in allTexts)
        {
            if (text.font != cnRegular)
            {
                nonCnRegularTexts.Add(text);
            }
        }

        if (nonCnRegularTexts.Count == 0)
        {
            EditorUtility.DisplayDialog("检查结果", 
                $"所有 {allTexts.Length} 个UGUI Text组件都已使用CN_Regular字体。", 
                "确定");
        }
        else
        {
            string message = $"共 {allTexts.Length} 个UGUI Text组件\n\n" +
                           $"未使用CN_Regular字体的Text：{nonCnRegularTexts.Count} 个\n\n";
            
            // 显示前10个未使用CN_Regular字体的Text
            int showCount = Mathf.Min(nonCnRegularTexts.Count, 10);
            for (int i = 0; i < showCount; i++)
            {
                Text text = nonCnRegularTexts[i];
                message += $"- {GetGameObjectPath(text.transform)}\n";
            }
            
            if (nonCnRegularTexts.Count > 10)
            {
                message += $"\n... 还有 {nonCnRegularTexts.Count - 10} 个";
            }

            EditorUtility.DisplayDialog("检查结果", message, "确定");
            
            // 在Hierarchy中选择所有未使用CN_Regular字体的Text对象
            Selection.objects = new Object[showCount];
            for (int i = 0; i < showCount; i++)
            {
                Selection.objects[i] = nonCnRegularTexts[i].gameObject;
            }
        }

        Debug.Log($"[FontManager] 检查完成：{nonCnRegularTexts.Count}/{allTexts.Length} 个UGUI Text未使用CN_Regular字体");
    }

    #endregion

    #region TextMeshPro 配置

    [MenuItem("Tools/Font Manager/配置当前场景所有TMP_Text使用CN_Regular字体")]
    public static void ConfigureAllTMP_TextsInScene()
    {
        TMP_FontAsset cnRegularTMP = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Fonts/CN_Regular_TMP.asset");
        
        if (cnRegularTMP == null)
        {
            int result = EditorUtility.DisplayDialogComplex(
                "TMP字体未找到",
                "未找到 CN_Regular_TMP 字体资产！\n\n" +
                "可以选择：\n" +
                "1. 查看创建指南（推荐）\n" +
                "2. 取消操作",
                "查看创建指南",
                "取消",
                "使用TMP默认字体"
            );

            if (result == 0)
            {
                // 查看创建指南
                ShowTMPFontCreationGuide();
            }
            else if (result == 2)
            {
                // 使用TMP默认字体，继续执行
                Debug.LogWarning("[FontManager] 使用TMP默认字体（Liberation Sans SDF）");
                return;
            }
            return;
        }

        TMP_Text[] allTMPTexts = GameObject.FindObjectsOfType<TMP_Text>(true);
        int count = 0;

        foreach (TMP_Text text in allTMPTexts)
        {
            if (text.font != cnRegularTMP)
            {
                Undo.RecordObject(text, "Change TMP Font");
                text.font = cnRegularTMP;
                EditorUtility.SetDirty(text);
                count++;
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog("配置完成", 
            $"已配置 {count} 个TMP_Text组件使用CN_Regular_TMP字体。\n\n" +
            $"总TMP_Text组件数：{allTMPTexts.Length}", 
            "确定");

        Debug.Log($"[FontManager] 已配置 {count} 个TMP_Text组件使用CN_Regular_TMP字体");
    }

    [MenuItem("Tools/Font Manager/重置当前场景所有TMP_Text字体为默认")]
    public static void ResetAllTMP_TextsInScene()
    {
        TMP_FontAsset defaultFont = Resources.GetBuiltinResource<TMP_FontAsset>("LiberationSans SDF");
        TMP_Text[] allTMPTexts = GameObject.FindObjectsOfType<TMP_Text>(true);
        int count = 0;

        foreach (TMP_Text text in allTMPTexts)
        {
            if (text.font != defaultFont)
            {
                Undo.RecordObject(text, "Reset TMP Font");
                text.font = defaultFont;
                EditorUtility.SetDirty(text);
                count++;
            }
        }

        AssetDatabase.SaveAssets();

        EditorUtility.DisplayDialog("重置完成", 
            $"已重置 {count} 个TMP_Text组件的字体为默认字体。\n\n" +
            $"总TMP_Text组件数：{allTMPTexts.Length}", 
            "确定");

        Debug.Log($"[FontManager] 已重置 {count} 个TMP_Text组件的字体为默认字体");
    }

    [MenuItem("Tools/Font Manager/查找当前场景中未使用CN_Regular_TMP字体的TMP_Text")]
    public static void FindTMP_TextsWithoutCnRegular()
    {
        TMP_FontAsset cnRegularTMP = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Fonts/CN_Regular_TMP.asset");
        
        if (cnRegularTMP == null)
        {
            Debug.LogWarning("[FontManager] 未找到 CN_Regular_TMP 字体资产");
            return;
        }

        TMP_Text[] allTMPTexts = GameObject.FindObjectsOfType<TMP_Text>(true);
        List<TMP_Text> nonCnRegularTexts = new List<TMP_Text>();

        foreach (TMP_Text text in allTMPTexts)
        {
            if (text.font != cnRegularTMP)
            {
                nonCnRegularTexts.Add(text);
            }
        }

        if (nonCnRegularTexts.Count == 0)
        {
            EditorUtility.DisplayDialog("检查结果", 
                $"所有 {allTMPTexts.Length} 个TMP_Text组件都已使用CN_Regular_TMP字体。", 
                "确定");
        }
        else
        {
            string message = $"共 {allTMPTexts.Length} 个TMP_Text组件\n\n" +
                           $"未使用CN_Regular_TMP字体的TMP_Text：{nonCnRegularTexts.Count} 个\n\n";
            
            // 显示前10个未使用CN_Regular_TMP字体的TMP_Text
            int showCount = Mathf.Min(nonCnRegularTexts.Count, 10);
            for (int i = 0; i < showCount; i++)
            {
                TMP_Text text = nonCnRegularTexts[i];
                message += $"- {GetGameObjectPath(text.transform)}\n";
            }
            
            if (nonCnRegularTexts.Count > 10)
            {
                message += $"\n... 还有 {nonCnRegularTexts.Count - 10} 个";
            }

            EditorUtility.DisplayDialog("检查结果", message, "确定");
            
            // 在Hierarchy中选择所有未使用CN_Regular_TMP字体的TMP_Text对象
            Selection.objects = new Object[showCount];
            for (int i = 0; i < showCount; i++)
            {
                Selection.objects[i] = nonCnRegularTexts[i].gameObject;
            }
        }

        Debug.Log($"[FontManager] 检查完成：{nonCnRegularTexts.Count}/{allTMPTexts.Length} 个TMP_Text未使用CN_Regular_TMP字体");
    }

    [MenuItem("Tools/Font Manager/显示TMP字体创建指南")]
    public static void ShowTMPFontCreationGuide()
    {
        string guide = @"
# TextMeshPro字体创建指南

## 步骤1：下载并导入字体文件

1. 下载霞鹜文楷GB字体：
   - 猫啃网：https://www.maoken.com/freefonts/16864.html
   - GitHub：https://github.com/lxgw/LxgwWenkaiGB/releases/tag/v1.521

2. 将字体文件（.ttf或.otf）复制到：
   Assets/Fonts/

## 步骤2：创建TextMeshPro Font Asset

### 方法一：使用Unity编辑器（推荐）

1. 在Project窗口中，找到导入的字体文件（如：LXGWWenKaiGB-Regular.ttf）
2. 右键字体文件 -> Create -> TextMeshPro -> Font Asset
3. 在弹出的Font Asset Creator窗口中：
   - Source Font File：选择你的字体文件
   - Font Name：CN_Regular
   - Sampling Point Size：保持默认（通常36）
   - Character Set：选择 Custom 或 Custom Range
   - Atlas Resolution：512x512 或 1024x1024（根据需要）
4. 点击 Generate Font Asset

### 方法二：使用TMP Settings

1. 打开 Window -> TextMeshPro -> Font Asset Creator
2. 配置同上
3. 点击 Generate Font Asset

## 步骤3：保存和命名

1. 生成的Font Asset会保存在 Fonts 目录下
2. 将其重命名为：CN_Regular_TMP.asset
3. 如果需要粗体，重复上述步骤，命名为：CN_Bold_TMP.asset

## 步骤4：配置使用

现在可以：
1. 在FontManager组件中拖入CN_Regular_TMP
2. 使用菜单：Tools -> Font Manager -> 配置当前场景所有TMP_Text使用CN_Regular字体

## 注意事项

1. **字符集选择**：如果需要完整的中文支持，建议使用 Custom Range
2. **图集大小**：较大的图集支持更多字符，但占用更多内存
3. **字符范围**：常用的中文范围：U+4E00-U+9FFF
4. **优化**：根据项目实际需求，只包含需要的字符以减少文件大小

## 常见问题

Q: 中文字符显示为方块？
A: 检查Font Asset的Character Set是否包含了中文字符

Q: 字体显示模糊？
A: 尝试增加Sampling Point Size或提高Atlas Resolution
";
        Debug.Log(guide);
        EditorUtility.DisplayDialog("TMP字体创建指南", 
            "指南已在Console中显示。\n\n" +
            "提示：\n" +
            "1. 下载字体并导入到 Assets/Fonts/\n" +
            "2. 右键字体文件 -> Create -> TextMeshPro -> Font Asset\n" +
            "3. 重命名为 CN_Regular_TMP.asset\n" +
            "4. 使用Tools菜单配置场景字体", 
            "确定");
    }

    #endregion

    #region  通用配置
    [MenuItem("Tools/Font Manager/配置所有文本组件（UGUI和TMP）")]
    public static void ConfigureAllTextComponentsInScene()
    {
        ConfigureAllTextsInScene();
        ConfigureAllTMP_TextsInScene();
    }

    [MenuItem("Tools/Font Manager/打开字体配置说明")]
    public static void OpenFontGuide()
    {
        string guidePath = "Assets/Fonts/字体配置说明.md";
        TextAsset guide = AssetDatabase.LoadAssetAtPath<TextAsset>(guidePath);
        
        if (guide != null)
        {
            Debug.Log(guide.text);
        }
        else
        {
            Debug.LogWarning("未找到字体配置说明文件");
        }
    }

    [MenuItem("Tools/Font Manager/打开字体文件夹")]
    public static void OpenFontsFolder()
    {
        // 检查Fonts文件夹是否存在
        if (!AssetDatabase.IsValidFolder("Assets/Fonts"))
        {
            AssetDatabase.CreateFolder("Assets", "Fonts");
        }

        // 在Project窗口中选中Fonts文件夹
        Object folder = AssetDatabase.LoadAssetAtPath<Object>("Assets/Fonts");
        Selection.activeObject = folder;
        EditorGUIUtility.PingObject(folder);
    }

    private static string GetGameObjectPath(Transform transform)
    {
        string path = transform.name;
        while (transform.parent != null)
        {
            transform = transform.parent;
            path = transform.name + "/" + path;
        }
        return path;
    }
    
    #endregion
}
