using UnityEditor;
using UnityEditor.Overlays;
using UnityEditor.SceneManagement;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[Overlay(typeof(SceneView), "Scene Mover")]
public class SceneMoverOverlay : Overlay
{
    private VisualElement _root;

    private uint _sceneCount;

    private const string RevertSceneKey = "test_revert_scene_path";
    private const string SelectSceneKey = "test_select_scene_path";
    private const string SceneCountKey = "SceneCount";

    private string GetProjectSpecificKey(string key)
    {
        // 프로젝트 경로를 포함시켜 고유 키 생성
        string projectPath = Application.dataPath;
        string projectSpecificKey = projectPath.GetHashCode().ToString();
        return $"{projectSpecificKey}_{key}";
    }

    // 테스트가 끝나고, 원래 씬으로 돌아오기 위해
    string RevertScenePath
    {
        get { return EditorPrefs.GetString(GetProjectSpecificKey(RevertSceneKey), string.Empty); }
        set { EditorPrefs.SetString(GetProjectSpecificKey(RevertSceneKey), value); }
    }

    public string GetSelectScenePath(int num)
    {
        return EditorPrefs.GetString(GetProjectSpecificKey(SelectSceneKey + num), string.Empty);
    }

    public void SetSelectScenePath(int num, string value)
    {
        EditorPrefs.SetString(GetProjectSpecificKey(SelectSceneKey + num), value);
    }

    public override VisualElement CreatePanelContent()
    {
        _root = new VisualElement();
        CreateRootPanel();
        return _root;

        void CreateRootPanel()
        {
            _root.Add(CreateField_SceneCount());
            for (int i = 0; i < _sceneCount; i++)
            {
                _root.Add(Create_Padding());
                _root.Add(CreateField_SelectScene(i));
                _root.Add(CreateButtons_PlayAndOpen(i));
            }
        }

        TextField CreateField_SceneCount()
        {
            TextField t = new TextField();
            _sceneCount = (uint)EditorPrefs.GetInt("SceneCount");
            t.value = _sceneCount.ToString();
            t.RegisterValueChangedCallback(e =>
            {
                if (uint.TryParse(e.newValue, out uint result))
                {
                    EditorPrefs.SetInt("SceneCount", (int)result);
                    _sceneCount = result;
                    t.value = result.ToString();

                    _root.Clear();
                    CreateRootPanel();
                }
                else
                {
                    t.value = 0.ToString();
                }
            });

            return t;
        }

        VisualElement Create_Padding()
        {
            VisualElement v = new VisualElement();
            v.style.paddingBottom = 10;
            return v;
        }

        ObjectField CreateField_SelectScene(int num)
        {
            ObjectField o = new ObjectField();
            o.style.minWidth = 100;
            o.style.maxWidth = 100;
            o.objectType = typeof(SceneAsset);
            o.value = AssetDatabase.LoadAssetAtPath<SceneAsset>(GetSelectScenePath(num));
            o.RegisterValueChangedCallback(e =>
            {
                SetSelectScenePath(num, AssetDatabase.GetAssetPath(e.newValue as SceneAsset));
            });
            return o;
        }

        VisualElement CreateButtons_PlayAndOpen(int num)
        {
            VisualElement buttons = new VisualElement();
            buttons.style.flexDirection = FlexDirection.Row;
            buttons.style.maxWidth = 100;
            buttons.Add(CreateButton_PlayStartScene(num));
            buttons.Add(CreateButton_OpenSelectScene(num));
            return buttons;
        }

        Button CreateButton_PlayStartScene(int num)
        {
            Button b = new Button();
            b.style.flexGrow = 1;
            b.text = "Play";
            b.AddToClassList("unity-toolbar-button");
            b.clicked += () =>
            {
                PlayStartScene(num);
            };
            return b;
        }

        Button CreateButton_OpenSelectScene(int num)
        {
            Button b = new Button();
            b.style.flexGrow = 1;
            b.text = "Open";
            b.AddToClassList("unity-toolbar-button");
            b.clicked += () =>
            {
                OpenSelectScene(num);
            };
            return b;
        }
    }

    void PlayStartScene(int num)
    {
        if (AssetDatabase.LoadAssetAtPath<SceneAsset>(GetSelectScenePath(num)) == null)
        {
            return;
        }


        RevertScenePath = EditorSceneManager.GetActiveScene().path;

        OpenScene(GetSelectScenePath(num));
        EditorApplication.EnterPlaymode();
    }

    void OpenSelectScene(int num)
    {
        if (AssetDatabase.LoadAssetAtPath<SceneAsset>(GetSelectScenePath(num)) == null)
        {
            return;
        }

        OpenScene(GetSelectScenePath(num));
    }

    void OpenScene(string scenePath)
    {
        if (EditorSceneManager.GetActiveScene().isDirty)
        {
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        }

        EditorSceneManager.OpenScene(scenePath);
    }

    public override void OnCreated()
    {
        base.OnCreated();
        EditorApplication.playModeStateChanged += RevertScene;
    }

    void RevertScene(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredEditMode)
        {
            if (string.IsNullOrEmpty(RevertScenePath))
            {
                return;
            }

            OpenScene(RevertScenePath);
            RevertScenePath = string.Empty;

            EditorApplication.playModeStateChanged -= RevertScene;
        }
    }
}
