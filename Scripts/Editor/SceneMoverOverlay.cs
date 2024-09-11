using UnityEditor;
using UnityEditor.Overlays;
using UnityEditor.SceneManagement;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[Overlay(typeof(SceneView), "Scene Mover")]
public class SceneMoverOverlay : Overlay
{
    // UI 요소 루트
    private VisualElement _root;

    // 씬 개수 (사용자가 설정할 수 있음)
    private uint _sceneCount;

    // 씬 복귀를 위한 키
    private const string RevertSceneKey = "test_revert_scene_path";
    // 선택된 씬의 경로를 저장하는 키
    private const string SelectSceneKey = "test_select_scene_path";
    // 씬 개수 저장 키
    private const string SceneCountKey = "SceneCount";

    // 프로젝트별 고유 키 생성 (프로젝트 경로를 해시로 사용)
    private string GetProjectSpecificKey(string key)
    {
        string projectPath = Application.dataPath;
        string projectSpecificKey = projectPath.GetHashCode().ToString();
        return $"{projectSpecificKey}_{key}";
    }

    // 테스트가 끝난 후 원래 씬으로 돌아가기 위한 경로 저장 및 로드
    string RevertScenePath
    {
        get { return EditorPrefs.GetString(GetProjectSpecificKey(RevertSceneKey), string.Empty); }
        set { EditorPrefs.SetString(GetProjectSpecificKey(RevertSceneKey), value); }
    }

    // 특정 번호의 선택된 씬 경로를 가져옴
    public string GetSelectScenePath(int num)
    {
        return EditorPrefs.GetString(GetProjectSpecificKey(SelectSceneKey + num), string.Empty);
    }

    // 특정 번호의 선택된 씬 경로를 저장
    public void SetSelectScenePath(int num, string value)
    {
        EditorPrefs.SetString(GetProjectSpecificKey(SelectSceneKey + num), value);
    }

    // 오버레이 패널 콘텐츠 생성
    public override VisualElement CreatePanelContent()
    {
        _root = new VisualElement
        {
            style = { width = 100 } // 오버레이 너비 설정
        };
        CreateRootPanel(); // 기본 UI 구성 생성
        return _root;

        // 루트 패널에 UI 요소 생성
        void CreateRootPanel()
        {
            // 씬 개수 설정을 위한 접이식 UI (Foldout)
            Foldout foldout = new Foldout
            {
                text = "Scene Count",
                value = false, // 접힌 상태로 시작
                style = {
                    marginTop = -5,
                    marginBottom = -6,
                    width = 100
                }
            };
            foldout.Add(CreateField_SceneCount()); // 씬 개수 입력 필드 추가
            _root.Add(foldout);

            // 씬 개수만큼 선택된 씬 UI 요소 추가
            for (int i = 0; i < _sceneCount; i++)
            {
                _root.Add(CreateField_SelectScene(i)); // 씬 선택 필드
                _root.Add(CreateButtons_PlayAndOpen(i)); // 재생 및 열기 버튼
            }
        }

        // 씬 개수를 입력받는 TextField 생성
        TextField CreateField_SceneCount()
        {
            TextField t = new TextField
            {
                style = {
                    marginTop = -4,
                    marginBottom = 7,
                    width = 80
                }
            };
            // 저장된 씬 개수를 불러오고 기본값은 1로 설정
            _sceneCount = (uint)EditorPrefs.GetInt(SceneCountKey, 1);
            t.value = _sceneCount.ToString();

            // 씬 개수 값 변경 시 이벤트 처리
            t.RegisterValueChangedCallback(e =>
            {
                if (uint.TryParse(e.newValue, out uint result))
                {
                    EditorPrefs.SetInt(SceneCountKey, (int)result);
                    _sceneCount = result;
                    t.value = result.ToString();

                    // 씬 개수 변경에 따라 UI를 다시 생성
                    _root.Clear();
                    CreateRootPanel();
                }
                else
                {
                    t.value = 1.ToString(); // 유효하지 않은 값일 경우 기본값 1로 설정
                }
            });

            return t;
        }

        // 특정 씬을 선택할 수 있는 ObjectField 생성
        ObjectField CreateField_SelectScene(int num)
        {
            ObjectField o = new ObjectField
            {
                style =
                {
                    flexGrow = 1,
                    minWidth = 0,
                    marginTop = 2,
                    marginBottom = 0,
                    width = 95,
                    minHeight = 18,
                    maxHeight = 18
                },
                objectType = typeof(SceneAsset)
            };

            // 저장된 씬 경로를 불러와서 ObjectField 값으로 설정
            o.value = AssetDatabase.LoadAssetAtPath<SceneAsset>(GetSelectScenePath(num));

            // 씬 선택 시 씬 경로 저장
            o.RegisterValueChangedCallback(e =>
            {
                SetSelectScenePath(num, AssetDatabase.GetAssetPath(e.newValue as SceneAsset));
            });

            return o;
        }

        // 씬을 재생하거나 여는 버튼 생성
        VisualElement CreateButtons_PlayAndOpen(int num)
        {
            VisualElement buttons = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Row,
                    justifyContent = Justify.SpaceBetween, // 양 끝 정렬
                    minWidth = 100,
                    flexGrow = 1
                }
            };

            buttons.Add(CreateButton_PlayStartScene(num)); // 재생 버튼
            buttons.Add(CreateButton_OpenSelectScene(num)); // 열기 버튼
            return buttons;
        }

        // 씬을 재생하는 버튼 생성
        Button CreateButton_PlayStartScene(int num)
        {
            Button b = new Button
            {
                text = "Play",
                style = { flexGrow = 1, flexBasis = 0, width = 50 }
            };
            b.AddToClassList("unity-toolbar-button");
            b.clicked += () =>
            {
                PlayStartScene(num);
            };
            return b;
        }

        // 씬을 여는 버튼 생성
        Button CreateButton_OpenSelectScene(int num)
        {
            Button b = new Button
            {
                text = "Open",
                style = { flexGrow = 1, flexBasis = 0, width = 50 }
            };
            b.AddToClassList("unity-toolbar-button");
            b.clicked += () =>
            {
                OpenSelectScene(num);
            };
            return b;
        }
    }

    // 선택된 씬을 재생
    void PlayStartScene(int num)
    {
        if (AssetDatabase.LoadAssetAtPath<SceneAsset>(GetSelectScenePath(num)) == null)
        {
            return; // 씬이 없으면 아무 작업도 하지 않음
        }

        // 현재 활성화된 씬 경로를 저장
        RevertScenePath = EditorSceneManager.GetActiveScene().path;

        // 선택된 씬을 열고 플레이 모드로 진입
        OpenScene(GetSelectScenePath(num));
        EditorApplication.EnterPlaymode();
    }

    // 선택된 씬을 열기
    void OpenSelectScene(int num)
    {
        if (AssetDatabase.LoadAssetAtPath<SceneAsset>(GetSelectScenePath(num)) == null)
        {
            return; // 씬이 없으면 아무 작업도 하지 않음
        }

        OpenScene(GetSelectScenePath(num));
    }

    // 씬을 열기 전에 수정된 씬이 있다면 저장 여부 확인
    void OpenScene(string scenePath)
    {
        if (EditorSceneManager.GetActiveScene().isDirty)
        {
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        }

        // 지정된 씬 경로를 열기
        EditorSceneManager.OpenScene(scenePath);
    }

    // 오버레이 생성 시 플레이 모드 상태 변경 이벤트 리스너 등록
    public override void OnCreated()
    {
        base.OnCreated();
        EditorApplication.playModeStateChanged += RevertScene;
    }

    // 플레이 모드 종료 후 원래 씬으로 복귀
    void RevertScene(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredEditMode)
        {
            if (string.IsNullOrEmpty(RevertScenePath))
            {
                return; // 복귀할 씬이 없으면 종료
            }

            OpenScene(RevertScenePath);
            RevertScenePath = string.Empty; // 복귀 후 경로 초기화

            EditorApplication.playModeStateChanged -= RevertScene;
        }
    }
}
