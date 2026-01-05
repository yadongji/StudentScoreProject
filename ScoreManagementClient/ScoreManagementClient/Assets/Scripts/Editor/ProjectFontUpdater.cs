#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// 项目字体更新器 - 直接修改预制体中的字体（编辑器模式）
/// </summary>
public class ProjectFontUpdater : EditorWindow
{
    private Font cnRegularFont;
    private Font cnBoldFont;
    private TMP_FontAsset cnRegularTMP_Font;
    private TMP_FontAsset cnBoldTMP_Font;

    private bool updateUGUI = true;
    private bool updateTMP = true;
    private bool includeInactive = true;

    private int totalPrefabs = 0;
    private int modifiedPrefabs = 0;
    private int modifiedUGUIComponents = 0;
    private int modifiedTMPComponents = 0;

    private Vector2 scrollPosition;

    [MenuItem("Tools/Project Font Updater/打开字体更新工具")]
    public static void ShowWindow()
    {
        GetWindow<ProjectFontUpdater>("项目字体更新器");
    }

    private void OnGUI()
    {
        GUILayout.Label("项目字体更新器", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("此工具将直接修改项目中的所有预制体文件，操作不可撤销！", MessageType.Warning);
        
        EditorGUILayout.Space();

        // ==================== 字体资源选择 ====================
        GUILayout.Label("1. 选择字体资源", EditorStyles.boldLabel);
        
        // UGUI字体
        updateUGUI = EditorGUILayout.Toggle("更新 UGUI Text 组件", updateUGUI);
        if (updateUGUI)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            cnRegularFont = (Font)EditorGUILayout.ObjectField("CN_Regular (UGUI)", cnRegularFont, typeof(Font), false);
            cnBoldFont = (Font)EditorGUILayout.ObjectField("CN_Bold (UGUI, 可选)", cnBoldFont, typeof(Font), false);
            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.Space();

        // TextMeshPro字体
        updateTMP = EditorGUILayout.Toggle("更新 TextMeshPro 组件", updateTMP);
        if (updateTMP)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            cnRegularTMP_Font = (TMP_FontAsset)EditorGUILayout.ObjectField("CN_Regular_TMP (TMP)", cnRegularTMP_Font, typeof(TMP_FontAsset), false);
            cnBoldTMP_Font = (TMP_FontAsset)EditorGUILayout.ObjectField("CN_Bold_TMP (TMP, 可选)", cnBoldTMP_Font, typeof(TMP_FontAsset), false);
            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.Space();

        // ==================== 其他选项 ====================
        GUILayout.Label("2. 选项设置", EditorStyles.boldLabel);
        includeInactive = EditorGUILayout.Toggle("包含非激活的对象", includeInactive);

        EditorGUILayout.Space();

        // ==================== 操作按钮 ====================
        GUILayout.Label("3. 执行更新", EditorStyles.boldLabel);
        
        EditorGUI.BeginDisabledGroup(!CanUpdate());
        if (GUILayout.Button("更新所有预制体", GUILayout.Height(40)))
        {
            UpdateAllPrefabs();
        }
        EditorGUI.EndDisabledGroup();

        EditorGUILayout.Space(10);

        // ==================== 统计信息 ====================
        if (modifiedPrefabs > 0)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label("更新统计", EditorStyles.boldLabel);
            GUILayout.Label($"总预制体数：{totalPrefabs}");
            GUILayout.Label($"已修改预制体：{modifiedPrefabs}");
            if (updateUGUI)
            {
                GUILayout.Label($"已修改 UGUI Text 组件：{modifiedUGUIComponents}");
            }
            if (updateTMP)
            {
                GUILayout.Label($"已修改 TMP_Text 组件：{modifiedTMPComponents}");
            }
            EditorGUILayout.EndVertical();
        }

        // ==================== 快捷操作 ====================
        EditorGUILayout.Space(10);
        GUILayout.Label("快捷操作", EditorStyles.boldLabel);
        
        if (GUILayout.Button("自动加载字体资源"))
        {
            AutoLoadFonts();
        }

        if (GUILayout.Button("打开字体文件夹"))
        {
            OpenFontsFolder();
        }

        if (GUILayout.Button("刷新字体资源"))
        {
            AssetDatabase.Refresh();
        }
    }

    private bool CanUpdate()
    {
        if (updateUGUI && updateTMP)
        {
            return cnRegularFont != null && cnRegularTMP_Font != null;
        }
        if (updateUGUI)
        {
            return cnRegularFont != null;
        }
        if (updateTMP)
        {
            return cnRegularTMP_Font != null;
        }
        return false;
    }

    private void AutoLoadFonts()
    {
        // 自动加载UGUI字体
        if (cnRegularFont == null)
        {
            cnRegularFont = AssetDatabase.LoadAssetAtPath<Font>("Assets/Fonts/CN_Regular.ttf");
            if (cnRegularFont == null)
            {
                cnRegularFont = AssetDatabase.LoadAssetAtPath<Font>("Assets/Fonts/LXGWWenKaiGB-Regular.ttf");
            }
        }

        if (cnBoldFont == null)
        {
            cnBoldFont = AssetDatabase.LoadAssetAtPath<Font>("Assets/Fonts/CN_Bold.ttf");
        }

        // 自动加载TMP字体
        if (cnRegularTMP_Font == null)
        {
            cnRegularTMP_Font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Fonts/CN_Regular_TMP.asset");
        }

        if (cnBoldTMP_Font == null)
        {
            cnBoldTMP_Font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Fonts/CN_Bold_TMP.asset");
        }

        Repaint();
    }

    private void OpenFontsFolder()
    {
        if (!AssetDatabase.IsValidFolder("Assets/Fonts"))
        {
            AssetDatabase.CreateFolder("Assets", "Fonts");
        }

        Object folder = AssetDatabase.LoadAssetAtPath<Object>("Assets/Fonts");
        Selection.activeObject = folder;
        EditorGUIUtility.PingObject(folder);
    }

    private void UpdateAllPrefabs()
    {
        // 确认对话框
        bool confirm = EditorUtility.DisplayDialog(
            "确认更新",
            "此操作将修改项目中的所有预制体文件。\n\n" +
            "修改将直接影响使用这些预制体的所有场景。\n\n" +
            "是否继续？",
            "继续",
            "取消"
        );

        if (!confirm) return;

        // 开始批量操作
        AssetDatabase.StartAssetEditing();

        try
        {
            // 重置统计
            totalPrefabs = 0;
            modifiedPrefabs = 0;
            modifiedUGUIComponents = 0;
            modifiedTMPComponents = 0;

            // 查找所有预制体
            string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab");
            totalPrefabs = prefabGuids.Length;

            if (totalPrefabs == 0)
            {
                Debug.LogWarning("[ProjectFontUpdater] 未找到任何预制体文件");
                return;
            }

            // 显示进度条
            EditorUtility.DisplayProgressBar("更新预制体", "正在处理预制体...", 0f);

            for (int i = 0; i < prefabGuids.Length; i++)
            {
                string prefabPath = AssetDatabase.GUIDToAssetPath(prefabGuids[i]);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

                if (prefab != null)
                {
                    bool prefabModified = false;

                    // 更新UGUI Text组件
                    if (updateUGUI && cnRegularFont != null)
                    {
                        Text[] texts = prefab.GetComponentsInChildren<Text>(includeInactive);
                        foreach (Text text in texts)
                        {
                            if (text.font != cnRegularFont)
                            {
                                // 记录修改
                                Undo.RecordObject(text, "Update Font");
                                PrefabUtility.RecordPrefabInstancePropertyModifications(text);
                                text.font = cnRegularFont;
                                prefabModified = true;
                                modifiedUGUIComponents++;
                            }
                        }
                    }

                    // 更新TMP_Text组件
                    if (updateTMP && cnRegularTMP_Font != null)
                    {
                        TMP_Text[] tmpTexts = prefab.GetComponentsInChildren<TMP_Text>(includeInactive);
                        foreach (TMP_Text tmpText in tmpTexts)
                        {
                            if (tmpText.font != cnRegularTMP_Font)
                            {
                                // 记录修改
                                Undo.RecordObject(tmpText, "Update TMP Font");
                                PrefabUtility.RecordPrefabInstancePropertyModifications(tmpText);
                                tmpText.font = cnRegularTMP_Font;
                                prefabModified = true;
                                modifiedTMPComponents++;
                            }
                        }
                    }

                    // 如果预制体被修改，标记为脏
                    if (prefabModified)
                    {
                        EditorUtility.SetDirty(prefab);
                        modifiedPrefabs++;
                    }
                }

                // 更新进度条
                float progress = (float)(i + 1) / prefabGuids.Length;
                EditorUtility.DisplayProgressBar("更新预制体", $"正在处理: {Path.GetFileName(prefabPath)} ({i + 1}/{prefabGuids.Length})", progress);
            }

            // 保存所有修改
            AssetDatabase.SaveAssets();

            Debug.Log($"[ProjectFontUpdater] 更新完成！\n" +
                      $"总预制体数：{totalPrefabs}\n" +
                      $"已修改预制体：{modifiedPrefabs}\n" +
                      $"已修改UGUI组件：{modifiedUGUIComponents}\n" +
                      $"已修改TMP组件：{modifiedTMPComponents}");

            EditorUtility.DisplayDialog("更新完成",
                $"预制体更新完成！\n\n" +
                $"总预制体数：{totalPrefabs}\n" +
                $"已修改预制体：{modifiedPrefabs}\n" +
                $"已修改UGUI组件：{modifiedUGUIComponents}\n" +
                $"已修改TMP组件：{modifiedTMPComponents}",
                "确定");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[ProjectFontUpdater] 更新过程中发生错误：{e.Message}\n{e.StackTrace}");
            EditorUtility.DisplayDialog("错误", $"更新过程中发生错误：\n{e.Message}", "确定");
        }
        finally
        {
            // 结束批量操作
            AssetDatabase.StopAssetEditing();
            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
        }
    }
}
#endif
