#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;
using System.IO;

/// <summary>
/// 预制体字体快速更新 - 菜单工具
/// </summary>
public class PrefabFontUpdater
{
    [MenuItem("Tools/Font Manager/批量更新所有预制体字体...")]
    public static void OpenPrefabFontUpdater()
    {
        ProjectFontUpdater.ShowWindow();
    }

    [MenuItem("Tools/Font Manager/更新当前选中预制体的字体")]
    public static void UpdateSelectedPrefabFont()
    {
        if (Selection.activeObject == null)
        {
            EditorUtility.DisplayDialog("提示", "请先选中一个预制体文件", "确定");
            return;
        }

        GameObject prefab = Selection.activeObject as GameObject;
        if (prefab == null)
        {
            EditorUtility.DisplayDialog("提示", "请选择一个GameObject类型的预制体", "确定");
            return;
        }

        // 加载字体
        Font cnRegularFont = AssetDatabase.LoadAssetAtPath<Font>("Assets/Fonts/CN_Regular.ttf");
        if (cnRegularFont == null)
        {
            cnRegularFont = AssetDatabase.LoadAssetAtPath<Font>("Assets/Fonts/LXGWWenKaiGB-Regular.ttf");
        }

        TMP_FontAsset cnRegularTMP_Font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Fonts/CN_Regular_TMP.asset");

        if (cnRegularFont == null && cnRegularTMP_Font == null)
        {
            EditorUtility.DisplayDialog("错误",
                "未找到字体资源！\n\n" +
                "请确保以下文件存在：\n" +
                "- Assets/Fonts/CN_Regular.ttf (UGUI)\n" +
                "- Assets/Fonts/CN_Regular_TMP.asset (TMP)",
                "确定");
            return;
        }

        // 显示配置对话框
        bool updateUGUI = cnRegularFont != null;
        bool updateTMP = cnRegularTMP_Font != null;

        bool confirm = EditorUtility.DisplayDialog(
            "确认更新",
            $"更新预制体：{prefab.name}\n\n" +
            $"更新 UGUI Text: {(updateUGUI ? "是" : "否")}\n" +
            $"更新 TextMeshPro: {(updateTMP ? "是" : "否")}\n\n" +
            "是否继续？",
            "继续",
            "取消"
        );

        if (!confirm) return;

        // 更新预制体
        int uguiCount = 0;
        int tmpCount = 0;

        AssetDatabase.StartAssetEditing();
        try
        {
            // 更新UGUI Text
            if (updateUGUI && cnRegularFont != null)
            {
                Text[] texts = prefab.GetComponentsInChildren<Text>(true);
                foreach (Text text in texts)
                {
                    if (text.font != cnRegularFont)
                    {
                        Undo.RecordObject(text, "Update Font");
                        PrefabUtility.RecordPrefabInstancePropertyModifications(text);
                        text.font = cnRegularFont;
                        uguiCount++;
                    }
                }
            }

            // 更新TMP_Text
            if (updateTMP && cnRegularTMP_Font != null)
            {
                TMP_Text[] tmpTexts = prefab.GetComponentsInChildren<TMP_Text>(true);
                foreach (TMP_Text tmpText in tmpTexts)
                {
                    if (tmpText.font != cnRegularTMP_Font)
                    {
                        Undo.RecordObject(tmpText, "Update TMP Font");
                        PrefabUtility.RecordPrefabInstancePropertyModifications(tmpText);
                        tmpText.font = cnRegularTMP_Font;
                        tmpCount++;
                    }
                }
            }

            // 保存
            if (uguiCount > 0 || tmpCount > 0)
            {
                EditorUtility.SetDirty(prefab);
                AssetDatabase.SaveAssets();
            }

            EditorUtility.DisplayDialog("更新完成",
                $"预制体更新完成：{prefab.name}\n\n" +
                $"已更新 UGUI Text：{uguiCount}\n" +
                $"已更新 TMP_Text：{tmpCount}",
                "确定");

            Debug.Log($"[PrefabFontUpdater] 预制体 {prefab.name} 更新完成：UGUI={uguiCount}, TMP={tmpCount}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[PrefabFontUpdater] 更新失败：{e.Message}");
            EditorUtility.DisplayDialog("错误", $"更新失败：{e.Message}", "确定");
        }
        finally
        {
            AssetDatabase.StopAssetEditing();
            AssetDatabase.Refresh();
        }
    }

    [MenuItem("Tools/Font Manager/更新当前选中文件夹下所有预制体字体")]
    public static void UpdatePrefabsInSelectedFolder()
    {
        if (Selection.activeObject == null)
        {
            EditorUtility.DisplayDialog("提示", "请先选中一个文件夹", "确定");
            return;
        }

        string folderPath = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            EditorUtility.DisplayDialog("提示", "请选择一个文件夹", "确定");
            return;
        }

        // 查找文件夹中的所有预制体
        string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab", new[] { folderPath });
        if (prefabGuids.Length == 0)
        {
            EditorUtility.DisplayDialog("提示", $"文件夹中没有找到预制体文件\n路径：{folderPath}", "确定");
            return;
        }

        // 加载字体
        Font cnRegularFont = AssetDatabase.LoadAssetAtPath<Font>("Assets/Fonts/CN_Regular.ttf");
        if (cnRegularFont == null)
        {
            cnRegularFont = AssetDatabase.LoadAssetAtPath<Font>("Assets/Fonts/LXGWWenKaiGB-Regular.ttf");
        }

        TMP_FontAsset cnRegularTMP_Font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Fonts/CN_Regular_TMP.asset");

        if (cnRegularFont == null && cnRegularTMP_Font == null)
        {
            EditorUtility.DisplayDialog("错误",
                "未找到字体资源！\n\n" +
                "请确保以下文件存在：\n" +
                "- Assets/Fonts/CN_Regular.ttf (UGUI)\n" +
                "- Assets/Fonts/CN_Regular_TMP.asset (TMP)",
                "确定");
            return;
        }

        bool confirm = EditorUtility.DisplayDialog(
            "确认更新",
            $"将在以下文件夹中更新 {prefabGuids.Length} 个预制体：\n\n{folderPath}\n\n" +
            "是否继续？",
            "继续",
            "取消"
        );

        if (!confirm) return;

        // 批量更新
        int modifiedPrefabs = 0;
        int uguiCount = 0;
        int tmpCount = 0;

        AssetDatabase.StartAssetEditing();
        try
        {
            EditorUtility.DisplayProgressBar("更新预制体", "正在处理...", 0f);

            for (int i = 0; i < prefabGuids.Length; i++)
            {
                string prefabPath = AssetDatabase.GUIDToAssetPath(prefabGuids[i]);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

                if (prefab != null)
                {
                    bool prefabModified = false;

                    // 更新UGUI Text
                    if (cnRegularFont != null)
                    {
                        Text[] texts = prefab.GetComponentsInChildren<Text>(true);
                        foreach (Text text in texts)
                        {
                            if (text.font != cnRegularFont)
                            {
                                Undo.RecordObject(text, "Update Font");
                                PrefabUtility.RecordPrefabInstancePropertyModifications(text);
                                text.font = cnRegularFont;
                                prefabModified = true;
                                uguiCount++;
                            }
                        }
                    }

                    // 更新TMP_Text
                    if (cnRegularTMP_Font != null)
                    {
                        TMP_Text[] tmpTexts = prefab.GetComponentsInChildren<TMP_Text>(true);
                        foreach (TMP_Text tmpText in tmpTexts)
                        {
                            if (tmpText.font != cnRegularTMP_Font)
                            {
                                Undo.RecordObject(tmpText, "Update TMP Font");
                                PrefabUtility.RecordPrefabInstancePropertyModifications(tmpText);
                                tmpText.font = cnRegularTMP_Font;
                                prefabModified = true;
                                tmpCount++;
                            }
                        }
                    }

                    if (prefabModified)
                    {
                        EditorUtility.SetDirty(prefab);
                        modifiedPrefabs++;
                    }
                }

                float progress = (float)(i + 1) / prefabGuids.Length;
                EditorUtility.DisplayProgressBar("更新预制体", $"正在处理: {Path.GetFileName(prefabPath)}", progress);
            }

            AssetDatabase.SaveAssets();

            EditorUtility.DisplayDialog("更新完成",
                $"文件夹更新完成！\n\n" +
                $"文件夹：{folderPath}\n" +
                $"总预制体数：{prefabGuids.Length}\n" +
                $"已修改预制体：{modifiedPrefabs}\n" +
                $"已更新UGUI组件：{uguiCount}\n" +
                $"已更新TMP组件：{tmpCount}",
                "确定");

            Debug.Log($"[PrefabFontUpdater] 文件夹 {folderPath} 更新完成：预制体={modifiedPrefabs}/{prefabGuids.Length}, UGUI={uguiCount}, TMP={tmpCount}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[PrefabFontUpdater] 更新失败：{e.Message}");
            EditorUtility.DisplayDialog("错误", $"更新失败：{e.Message}", "确定");
        }
        finally
        {
            AssetDatabase.StopAssetEditing();
            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
        }
    }

    // 验证菜单项可用性
    [MenuItem("Tools/Font Manager/更新当前选中预制体的字体", true)]
    public static bool ValidateUpdateSelectedPrefab()
    {
        return Selection.activeObject != null && Selection.activeObject is GameObject;
    }

    [MenuItem("Tools/Font Manager/更新当前选中文件夹下所有预制体字体", true)]
    public static bool ValidateUpdatePrefabsInFolder()
    {
        if (Selection.activeObject == null) return false;
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        return AssetDatabase.IsValidFolder(path);
    }
}
#endif
