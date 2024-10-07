using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace Sample.Package.Window.Editor
{
    /// <summary>
    /// 기술지원팀 패키지를 하나의 window에서 보여줍니다.
    /// </summary>
    public class SamplePackageWindow : EditorWindow
    {
        //--------------------------------------------------------------------------------//
        //-----------------------------------FIELD----------------------------------------//
        //--------------------------------------------------------------------------------//
        private static Dictionary<string, TabItem> _tabItems =
#if UNITY_2021_1_OR_NEWER
            new();
#else
         new Dictionary<string, TabItem>();
#endif

        private VisualElement _veContent;

        private Button _btnChangeLogInUpdate;
        private Button _btnSkipInUpdate;
        private Button _btnOpenPackageWindow;
        private Label _lbUpdate;

        private VisualElement _veUpdate;
        private const string UXML_FILE = "SamplePackageWindow.uxml";
        private const string BI_FILE = "sample_bi.png";
        //private const string INHOUSE_PATH = "Assets/Package/Editor";
        private const string INHOUSE_PATH = "Assets/_ThirdParty/PackageWindow/Editor";
        private const string PACKAGE_WINDOW_NAME = "com.sample.package.window";
        private const string PACKAGE_PATH = "Packages/" + PACKAGE_WINDOW_NAME + "/Editor";

        private StyleLength _defaultFontSize;
        private StyleLength _selectFontSize = 16;
        private StyleColor _defaultBackgroundColor;
        private StyleColor _selectBackgroundColor = new(new Color(1, 127f / 255f, 0, 0.4f));
        private Button _btnSelected;
        private string _versionSelected;
        private string _nameSelected;
        private const string SELECTED_KEY = "selectedKey";

        // private static string _name;
        private Dictionary<string, Button> _tabButtons = new();
        private Dictionary<string, Button> _tabButtonsByName = new();
        private Dictionary<string, PackageInfo> _packageInfos = new();
        private static SamplePackageWindow _instance;
        private static List<string> _versions = new();

        public VisualElement Content => _veContent;

        [InitializeOnLoadMethod]
        private static void OnLoadMethod()
        {
            EditorApplication.delayCall += Initialize;
        }

        //--------------------------------------------------------------------------------//
        //------------------------------------METHOD--------------------------------------//
        //--------------------------------------------------------------------------------//
        [MenuItem("Sample/Show Window %#&w")]
        public static void ShowSampleWindow()
        {
            _instance = GetWindow<SamplePackageWindow>();

            string path = Path.Combine(INHOUSE_PATH, BI_FILE);
            if (File.Exists(path) == false)
            {
                path = Path.Combine(PACKAGE_PATH, BI_FILE); //패키지로 설치되었을 때
            }

            var icon = AssetDatabase.LoadAssetAtPath<Texture>(path);
            _instance.titleContent = new GUIContent(icon);
        }

        /// <summary>
        /// <param name="visualElement">visualElement</param>를 window 화면의 content 영역에 보여줍니다.
        /// </summary>
        /// <param name="visualElement">window화면에 보여줄 화면</param>
        public static void SetContentVisualElement(VisualElement visualElement)
        {
            var wnd = GetWindow<SamplePackageWindow>();
            wnd.RepaintContentElement(visualElement);
        }

        public static VisualElement GetContentVisualElement()
        {
            var wnd = GetWindow<SamplePackageWindow>();
            return wnd.Content;
        }

        private static void Initialize()
        {
            foreach (TabItem item in _tabItems.Values.Where(x => x.HasInitialize))
            {
                item.Initialize();
            }
        }

        /// <summary>
        /// window에서 보여줄 패키지를 추가합니다.
        /// </summary>
        /// <param name="tabName">window의 tab에 표시될 이름입니다.</param>
        /// <param name="sourcePath">`.uxml` 혹은 `.asset(ScriptableObject)`의 경로입니다.</param>
        /// <param name="onInitialize">초기화가 필요한 경우 콜백 함수를 등록합니다. 예를들어  <param name="sourcePath">sourcePath</param>에 있는 `.asset`파일을 생성하는 로직일수도 있습니다.</param>
        public static void Add(string tabName, string sourcePath, Action onInitialize = null)
        {
            if (_tabItems.ContainsKey(tabName) == false)
            {
                var tabItem = new TabItem(sourcePath, onInitialize);
                _tabItems.Add(tabName, tabItem);

                CheckPackageJson(Assembly.GetCallingAssembly().Location, _tabItems[tabName]);
            }
        }

        /// <summary>
        /// window에서 보여줄 패키지를 추가합니다.
        /// </summary>
        /// <param name="tabName">window의 tab에 표시될 이름입니다.</param>
        /// <param name="onSelected">tab을 선택하면 실행할 콜백함수입니다.</param>
        public static void Add(string tabName, Action onSelected)
        {
            if (_tabItems.ContainsKey(tabName) == false)
            {
                var tabItem = new TabItem(onSelected);
                _tabItems.Add(tabName, tabItem);

                CheckPackageJson(Assembly.GetCallingAssembly().Location, _tabItems[tabName]);
            }
        }

        /// <summary>
        /// key에 해당하는 메뉴를 선택합니다.
        /// </summary>
        /// <param name="key"><see cref="Add"/> 메소드의 `tabName`파라미터입니다.</param>
        public static void Select(string key)
        {
            if (_instance._tabButtons.TryGetValue(key, out Button button))
            {
                _instance.RepaintContentElement(button?.name);
                _instance.SelectButton(button);
            }
        }

        private static void CheckPackageJson(string location, TabItem tabItem)
        {
            string dllFileName = Path.GetFileNameWithoutExtension(location); //dll파일 이름만 추출
            string packageCachePath = Path.Combine(Path.GetFullPath("."), "Library", "PackageCache"); //UPM통해 설치
            string[] files = Directory.GetFiles(packageCachePath, $"{dllFileName}.asmdef", SearchOption.AllDirectories);
            if (files.Length > 0)
            {
                foreach (string filePath in files)
                {
                    string directory = Path.GetDirectoryName(filePath);
                    string packageRoot = Path.GetDirectoryName(directory);
                    string jsonPath = Path.Combine(packageRoot, "package.json");
                    SetTabItemFromPackageJson(jsonPath, tabItem);
                }
            }
            else
            {
                //설치한 프로젝트에 해당 폴더가 없을 경우를 대비해서 폴더와 파일 존재여부를 체크
                string packageFolder = Path.Combine(Path.GetFullPath("."), "Assets", "Package");
                if (Directory.Exists(packageFolder) == false)
                {
                    SetEmpty(tabItem);
                    return;
                }

                string jsonPath = Path.Combine(packageFolder, "package.json");
                if (File.Exists(jsonPath) == false)
                {
                    SetEmpty(tabItem);
                    return;
                }

                SetTabItemFromPackageJson(jsonPath, tabItem);
            }
        }

        private static void SetEmpty(TabItem tabItem)
        {
            tabItem.SetInfo(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
        }

        private static void SetTabItemFromPackageJson(string jsonPath, TabItem tabItem)
        {
            var packageJson = JsonUtility.FromJson<PackageJson>(File.ReadAllText(jsonPath));
            tabItem.SetInfo(packageJson.name, packageJson.version, packageJson.documentationUrl, packageJson.changelogUrl, packageJson.repository.url);
        }

        private async void CreateGUI()
        {
            Initialize();

            string path = Path.Combine(INHOUSE_PATH, UXML_FILE);
            if (File.Exists(path) == false)
            {
                path = Path.Combine(PACKAGE_PATH, UXML_FILE); //패키지로 설치되었을 때
            }

            var visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(path);
            VisualElement windowRoot = visualTreeAsset.Instantiate();
            rootVisualElement.Add(windowRoot);

            var gbTab = windowRoot.Q<VisualElement>("gb_tab");
            const string KEY_HOME = "Home";
            _tabItems.Remove(KEY_HOME);
            Button btnHome = CreateTabButton(KEY_HOME, PACKAGE_WINDOW_NAME, gbTab);
            _defaultFontSize = btnHome.style.fontSize;
            _defaultBackgroundColor = btnHome.style.backgroundColor;

            foreach (KeyValuePair<string, TabItem> tabItem in _tabItems)
            {
                CreateTabButton(tabItem.Key, tabItem.Value.name, gbTab);
            }

            if (!_tabItems.ContainsKey(KEY_HOME))
            {
                _tabItems.Add(KEY_HOME, new TabItem(CreateHomeContent));
            }

            _veContent = windowRoot.Q<VisualElement>("ve_content");

            if (windowRoot != null)
            {
                _veUpdate = windowRoot.Q<VisualElement>("ve_update");

                _btnOpenPackageWindow = _veUpdate.Q<Button>("btn_open_package_window");
                _btnOpenPackageWindow?.RegisterCallback<ClickEvent>(OnClickOpenPackageWindow);
            }

            var isExist = false;
            if (EditorPrefs.HasKey(SELECTED_KEY))
            {
                string tabName = EditorPrefs.GetString(SELECTED_KEY);
                IEnumerable<VisualElement> emerable = gbTab.Children();
                foreach (VisualElement child in emerable)
                {
                    if (child.name.Equals(tabName))
                    {
                        RepaintContentElement(tabName);
                        SelectButton(child as Button);
                        isExist = true;
                        break;
                    }
                }
            }

            if (isExist == false)
            {
                RepaintContentElement(btnHome.name);
                SelectButton(btnHome);
            }

            //패키지들 업데이트 체크
            ListRequest packageList = Client.List();
            while (packageList.IsCompleted == false)
            {
                await Task.Yield();
            }

            foreach (PackageInfo package in packageList.Result)
            {
                if (package.name.Contains("com.sample"))
                {
                    PackageInfo packageInfo = PackageInfo.FindForAssetPath($"Packages/{package.name}");
                    _packageInfos.TryAdd(package.name, package);
                    bool isExistUpdate = TryGetLatestVersion(package.name, packageInfo.version, package.versions, out string latestVersion);
                    if (isExistUpdate)
                    {
                        if (_tabButtonsByName.TryGetValue(package.name, out Button button))
                        {
                            button.text = $"* {button.name}";
                        }
                    }
                }
            }
        }

        private async void CheckUpdateVersion()
        {
            if (_veUpdate == null)
            {
                return;
            }

            _veUpdate.style.display = DisplayStyle.None;

            if (string.IsNullOrEmpty(_nameSelected))
            {
                return;
            }

            while (_packageInfos.Count == 0)
            {
                await Task.Yield();
            }

            if (string.IsNullOrEmpty(_nameSelected)
                || _packageInfos.TryGetValue(_nameSelected, out PackageInfo packageInfo) == false)
            {
                return;
            }

            // string skipThisVersion = EditorPrefs.GetString(GetPrefsKey(_nameSelected), "0.0.0");
            VersionsInfo versionsInfo = packageInfo.versions;
            if (TryGetLatestVersion(_nameSelected, _versionSelected, versionsInfo, out string latestVersion))
            {
                if (_tabButtonsByName.TryGetValue(_nameSelected, out Button button))
                {
                    button.text = $"* {button.name}";
                }

                _lbUpdate.text = $"{button.name}의 최신버전 {latestVersion}이 업데이트 되었습니다. PackageManager 창에서 업데이트하세요.";
                _veUpdate.style.display = DisplayStyle.Flex;
            }
            else
            {
                _veUpdate.style.display = DisplayStyle.None;
            }
        }

        private bool TryGetLatestVersion(string name, string version, VersionsInfo versionsInfo, out string latestVersion)
        {
            latestVersion = string.Empty;

            if (string.IsNullOrEmpty(name)
                || name.Equals(PACKAGE_WINDOW_NAME))
            {
                return false;
            }

            _versions.Clear();
            string skipThisVersion = EditorPrefs.GetString(GetPrefsKey(name), "0.0.0");
            int index = -1;
            for (var i = 0; i < versionsInfo.all.Length; i++)
            {
                if (versionsInfo.all[i].Equals(version))
                {
                    index = i;
                }

                if (index != -1 && i > index)
                {
                    _versions.Add(versionsInfo.all[i]);
                }
            }

            var isExist = false;
            for (var i = 0; i < _versions.Count; i++)
            {
                //version에 하이픈이 있으면 preview라고 가정한다.
                if (_versions[i].Contains("-")
                    || _versions[i].Contains("pre"))
                {
                    continue;
                }

                //1개라도 있으면 업데이트할 버전이 존재하는 것
                isExist = true;
                break;
            }

            if (isExist && _versions.Count > 0)
            {
                if (skipThisVersion.Equals(_versions[^1]))
                {
                    return false;
                }
                else
                {
                    for (var i = _versions.Count - 1; i >= 0; i--)
                    {
                        //version에 하이픈이 있으면 preview라고 가정한다.
                        if (_versions[i].Contains("-")
                            || _versions[i].Contains("pre"))
                        {
                            continue;
                        }

                        //1개라도 있으면 업데이트할 버전이 존재하는 것
                        latestVersion = _versions[i];
                        return true;
                    }

                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private void OnClickedSkipThisVersion(ClickEvent evt)
        {
            EditorPrefs.SetString(GetPrefsKey(_nameSelected), _versions[^1]);
            _veUpdate.style.display = DisplayStyle.None;
            if (_tabButtonsByName.TryGetValue(_nameSelected, out Button button))
            {
                button.text = button.name.Replace("* ", "");
            }
        }

        private void OnClickOpenPackageWindow(ClickEvent evt)
        {
            // unity 2021
            var typePackageManagerWindow = Type.GetType("UnityEditor.PackageManager.UI.PackageManagerWindow,UnityEditor.PackageManagerUIModule");
            if (typePackageManagerWindow == null)
            {
                // unity 2022
                typePackageManagerWindow = Type.GetType("UnityEditor.PackageManager.UI.PackageManagerWindow,UnityEditor.CoreModule");
                if (typePackageManagerWindow == null)
                {
                    return;
                }
            }

            // 패키지 윈도우 오픈 이후 특정 패키지 선택
            MethodInfo methodInfo = typePackageManagerWindow.GetMethod("SelectPackageAndFilterStatic", BindingFlags.Static | BindingFlags.NonPublic);
            if (methodInfo == null)
            {
                return;
            }

            ParameterInfo[] parameterInfos = methodInfo.GetParameters().ToArray();
            var parameters = new object[parameterInfos.Length];
            for (var i = 0; i < parameterInfos.Length; i++)
            {
                parameters[i] = parameterInfos[i].DefaultValue;
            }

            // 첫번째 인자 : 패키지 이름
            parameters[0] = _nameSelected;
            methodInfo!.Invoke(null, parameters);
        }

        private string GetPrefsKey(string name)
        {
            return $"{name}_skip_this_version";
        }

        private void CreateHomeContent()
        {
            var label = new Label();
            label.style.color = new Color(127f / 255f, 127f / 255f, 127f / 255f, 1);
            SetContentVisualElement(label);
        }

        private Button CreateTabButton(string key, string name, VisualElement gbTab)
        {
            var button = new Button();
            button.name = button.text = key;
            gbTab.Add(button);
            button.RegisterCallback<ClickEvent>(OnTabClicked);
            _tabButtons.TryAdd(key, button);
            _tabButtonsByName.TryAdd(name, button);
            return button;
        }

        private void OnTabClicked(ClickEvent evt)
        {
            var button = evt.currentTarget as Button;
            RepaintContentElement(button?.name);
            SelectButton(button);
        }

        private void SelectButton(Button btnSelect)
        {
            if (btnSelect == null)
            {
                return;
            }

            if (_btnSelected != null)
            {
                _btnSelected.style.fontSize = _defaultFontSize;
                _btnSelected.style.backgroundColor = _defaultBackgroundColor;
            }

            btnSelect.style.fontSize = _selectFontSize;
            btnSelect.style.backgroundColor = _selectBackgroundColor;

            _btnSelected = btnSelect;
            EditorPrefs.SetString(SELECTED_KEY, btnSelect.name);
        }

        private void RepaintContentElement(VisualElement veChildContent)
        {
            _veContent.Clear();
            _veContent.Add(veChildContent);
        }

        private void RepaintContentElement(string tabName)
        {
            _veContent.Clear();

            if (_tabItems.Count == 0
                || string.IsNullOrEmpty(tabName))
            {
                return;
            }

            if (_tabItems.TryGetValue(tabName, out TabItem tabItem))
            {
                if (string.IsNullOrEmpty(tabItem.sourcePath))
                {
                    tabItem.onSelected?.Invoke();
                }
                else
                {
                    string extension = Path.GetExtension(tabItem.sourcePath);
                    if (extension.Equals(".uxml"))
                    {
                        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(tabItem.sourcePath);
                        VisualElement element = visualTree.Instantiate();
                        _veContent.Add(element);
                    }

                    if (extension.Equals(".asset"))
                    {
                        var so = AssetDatabase.LoadAssetAtPath<ScriptableObject>(tabItem.sourcePath);
                        var inspectorElement = new InspectorElement(so);
                        inspectorElement.style.flexGrow = 1;
                        _veContent.Add(inspectorElement);
                    }
                }

                _versionSelected = tabItem.version;
                _nameSelected = tabItem.name;

                CheckUpdateVersion();
            }
        }
    }

    internal class TabItem
    {
        /// <summary>
        /// tab을 클릭했을 때 sourcePath의 경로를 통해서 window에 바로 보여줍니다.
        /// </summary>
        internal string sourcePath;

        /// <summary>
        /// onSelected를 통해 다음 태스크를 커스터마이징합니다.
        /// </summary>
        internal Action onSelected;

        /// <summary>
        /// 매뉴얼 url
        /// </summary>
        internal string documentationUrl;

        /// <summary>
        /// CHANGELOG url
        /// </summary>
        internal string changelogUrl;

        /// <summary>
        /// github Issue url
        /// </summary>
        internal string issueeUrl;

        /// <summary>
        /// identifier 이름. ex)com.Sample.package.window
        /// </summary>
        internal string name;

        /// <summary>
        /// 패키지 version
        /// </summary>
        internal string version;

        /// <summary>
        /// 초기화
        /// </summary>
        private Action onInitialize;

        internal TabItem(string sourcePath, Action onInitialize = null)
        {
            this.onInitialize = onInitialize;
            this.sourcePath = sourcePath;
            onSelected = null;
        }

        internal TabItem(Action onSelected)
        {
            sourcePath = string.Empty;
            this.onSelected = onSelected;
        }

        internal void Initialize()
        {
            onInitialize?.Invoke();
            onInitialize = null;
        }

        internal void SetInfo(string name, string version, string documentationUrl, string changelogUrl, string gitUrl)
        {
            this.name = name;
            this.version = version;
            this.documentationUrl = documentationUrl;
            this.changelogUrl = changelogUrl;

            if (string.IsNullOrEmpty(gitUrl))
            {
                issueeUrl = string.Empty;
            }
            else
            {
                if (gitUrl.EndsWith(".git"))
                {
                    gitUrl = gitUrl.Substring(0, gitUrl.Length - 4);
                }

                issueeUrl = $"{gitUrl}/issues";
            }
        }

        internal bool HasInitialize => onInitialize != null;
    }

    [Serializable]
    internal class PackageJson
    {
        public string name;
        public string version;
        public string documentationUrl;
        public string changelogUrl;
        public PackageJsonRepository repository;
    }

    [Serializable]
    internal class PackageJsonRepository
    {
        public string type;
        public string url;
    }
}
