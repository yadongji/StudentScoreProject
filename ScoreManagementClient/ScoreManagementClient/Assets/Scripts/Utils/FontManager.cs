using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 字体管理器 - 统一管理项目中所有文本的字体（支持UGUI Text和TextMeshPro）
/// </summary>
public class FontManager : MonoBehaviour
{
    [Header("UGUI字体资源")]
    [Tooltip("常规中文字体（UGUI）")]
    public Font CN_Regular;
    
    [Tooltip("粗体中文体（UGUI）")]
    public Font CN_Bold;
    
    [Header("TextMeshPro字体资源")]
    [Tooltip("常规中文字体（TextMeshPro）")]
    public TMP_FontAsset CN_Regular_TMP;
    
    [Tooltip("粗体中文体（TextMeshPro）")]
    public TMP_FontAsset CN_Bold_TMP;
    
    [Header("自动配置")]
    [Tooltip("是否在启动时自动配置所有Text组件")]
    public bool autoConfigureOnStart = true;
    
    [Tooltip("是否包括非活动的对象")]
    public bool includeInactive = false;

    private static FontManager _instance;
    public static FontManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<FontManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("FontManager");
                    _instance = go.AddComponent<FontManager>();
                    DontDestroyOnLoad(go);
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (autoConfigureOnStart)
        {
            ConfigureAllTexts();
            ConfigureAllTMP_Texts();
        }
    }

    /// <summary>
    /// 配置场景中所有的Text组件使用CN_Regular字体
    /// </summary>
    [ContextMenu("配置所有Text使用CN_Regular字体")]
    public void ConfigureAllTexts()
    {
        if (CN_Regular == null)
        {
            Debug.LogWarning("CN_Regular字体未设置！请在Inspector中设置字体资源。");
            return;
        }

        Text[] allTexts = FindObjectsOfType<Text>(includeInactive);
        int count = 0;

        foreach (Text text in allTexts)
        {
            if (text.font != CN_Regular)
            {
                text.font = CN_Regular;
                count++;
            }
        }

        Debug.Log($"已配置 {count} 个Text组件使用CN_Regular字体。");
    }

    /// <summary>
    /// 配置场景中所有的TMP_Text组件使用CN_Regular_TMP字体
    /// </summary>
    [ContextMenu("配置所有TMP_Text使用CN_Regular字体")]
    public void ConfigureAllTMP_Texts()
    {
        if (CN_Regular_TMP == null)
        {
            Debug.LogWarning("CN_Regular_TMP字体未设置！请在Inspector中设置TMP_FontAsset资源。");
            return;
        }

        TMP_Text[] allTMPTexts = FindObjectsOfType<TMP_Text>(includeInactive);
        int count = 0;

        foreach (TMP_Text text in allTMPTexts)
        {
            if (text.font != CN_Regular_TMP)
            {
                text.font = CN_Regular_TMP;
                count++;
            }
        }

        Debug.Log($"已配置 {count} 个TMP_Text组件使用CN_Regular_TMP字体。");
    }

    /// <summary>
    /// 配置所有文本组件（包括UGUI Text和TextMeshPro）
    /// </summary>
    [ContextMenu("配置所有文本组件使用中文字体")]
    public void ConfigureAllTextComponents()
    {
        ConfigureAllTexts();
        ConfigureAllTMP_Texts();
    }

    /// <summary>
    /// 重置所有Text组件的字体为默认字体
    /// </summary>
    [ContextMenu("重置所有Text字体为默认")]
    public void ResetAllTexts()
    {
        Text[] allTexts = FindObjectsOfType<Text>(includeInactive);
        int count = 0;

        foreach (Text text in allTexts)
        {
            if (text.font != Resources.GetBuiltinResource<Font>("Arial.ttf"))
            {
                text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                count++;
            }
        }

        Debug.Log($"已重置 {count} 个Text组件的字体为默认字体。");
    }

    /// <summary>
    /// 重置所有TMP_Text组件的字体为默认字体
    /// </summary>
    [ContextMenu("重置所有TMP_Text字体为默认")]
    public void ResetAllTMP_Texts()
    {
        TMP_Text[] allTMPTexts = FindObjectsOfType<TMP_Text>(includeInactive);
        int count = 0;

        foreach (TMP_Text text in allTMPTexts)
        {
            if (text.font != null && text.font.name != "Liberation Sans SDF")
            {
                // 获取默认的TMP_FontAsset
                TMP_FontAsset defaultFont = Resources.GetBuiltinResource<TMP_FontAsset>("LiberationSans SDF");
                if (defaultFont != null)
                {
                    text.font = defaultFont;
                    count++;
                }
            }
        }

        Debug.Log($"已重置 {count} 个TMP_Text组件的字体为默认字体。");
    }

    /// <summary>
    /// 为指定的Text组件设置字体
    /// </summary>
    /// <param name="text">Text组件</param>
    /// <param name="isBold">是否使用粗体</param>
    public void SetFont(Text text, bool isBold = false)
    {
        if (text == null) return;

        text.font = isBold ? (CN_Bold ?? CN_Regular) : CN_Regular;
    }

    /// <summary>
    /// 为指定的TMP_Text组件设置字体
    /// </summary>
    /// <param name="text">TMP_Text组件</param>
    /// <param name="isBold">是否使用粗体</param>
    public void SetFont(TMP_Text text, bool isBold = false)
    {
        if (text == null) return;

        text.font = isBold ? (CN_Bold_TMP ?? CN_Regular_TMP) : CN_Regular_TMP;
    }

    /// <summary>
    /// 加载字体资源（从Resources文件夹加载）
    /// </summary>
    public void LoadFontFromResources()
    {
        CN_Regular = Resources.Load<Font>("Fonts/CN_Regular");
        CN_Bold = Resources.Load<Font>("Fonts/CN_Bold");
        CN_Regular_TMP = Resources.Load<TMP_FontAsset>("Fonts/CN_Regular_TMP");
        CN_Bold_TMP = Resources.Load<TMP_FontAsset>("Fonts/CN_Bold_TMP");
        
        Debug.Log("字体资源加载完成");
    }
}
