using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEngine;
using UnityEditor;

public class GameEditorWindow : OdinMenuEditorWindow
{
    private SkillEditorWindow skillEditor;


    [MenuItem("Tools/ÓÎÏ·±à¼­Æ÷")]
    public static GameEditorWindow ShowAssetBundleWindow()
    {
        GameEditorWindow window = GetWindow<GameEditorWindow>();
        window.position = GUIHelper.GetEditorWindowRect().AlignCenter(1050, 650);
        window.titleContent = new GUIContent("ÓÎÏ·±à¼­Æ÷");
        window.ForceMenuTreeRebuild();
        return window;
    }

    protected override OdinMenuTree BuildMenuTree()
    {
        OdinMenuTree menuTree = new OdinMenuTree(supportsMultiSelect: false)
        {
            { "Home",null,EditorIcons.House},
            { "¼¼ÄÜ±à¼­Æ÷",this.skillEditor ?? CreateSkillEditor(),EditorIcons.UnityLogo},
            { "¶Ô»°±à¼­Æ÷",this,EditorIcons.UnityLogo},
        };


        return menuTree;
    }

    public SkillEditorWindow CreateSkillEditor()
    {
        if (skillEditor == null)
        {
            skillEditor = ScriptableObject.CreateInstance<SkillEditorWindow>();
        }
        return skillEditor;
    }
}
