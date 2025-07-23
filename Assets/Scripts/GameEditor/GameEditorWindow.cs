using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEngine;
using UnityEditor;

public class GameEditorWindow : OdinMenuEditorWindow
{
    private SkillEditorWindow skillEditor;


    [MenuItem("Tools/��Ϸ�༭��")]
    public static GameEditorWindow ShowAssetBundleWindow()
    {
        GameEditorWindow window = GetWindow<GameEditorWindow>();
        window.position = GUIHelper.GetEditorWindowRect().AlignCenter(1050, 650);
        window.titleContent = new GUIContent("��Ϸ�༭��");
        window.ForceMenuTreeRebuild();
        return window;
    }

    protected override OdinMenuTree BuildMenuTree()
    {
        OdinMenuTree menuTree = new OdinMenuTree(supportsMultiSelect: false)
        {
            { "Home",null,EditorIcons.House},
            { "���ܱ༭��",this.skillEditor ?? CreateSkillEditor(),EditorIcons.UnityLogo},
            { "�Ի��༭��",this,EditorIcons.UnityLogo},
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
