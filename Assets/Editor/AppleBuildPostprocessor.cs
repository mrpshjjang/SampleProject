#if UNITY_IOS
using System.IO;
using UnityEditor;
using UnityEditor.iOS.Xcode;
using UnityEditor.Callbacks;
//using AppleAuth.Editor;


/// <summary>
/// Apple Build Post Processor
/// </summary>
public static class AppleBuildPostprocessor
{
    [PostProcessBuildAttribute(int.MaxValue)]
    public static void OnPostProcessBuild(BuildTarget buildTarget, string pathToBuildProject)
    {
        if (buildTarget != BuildTarget.iOS) return;
        string projectPath = pathToBuildProject + "/Unity-iPhone.xcodeproj/project.pbxproj";

        PBXProject pbxProject = new PBXProject();
        pbxProject.ReadFromFile(projectPath);

        // UnityMain
        string unityMainTargetGuid = pbxProject.GetUnityMainTargetGuid();
        pbxProject.AddFrameworkToProject(unityMainTargetGuid, "UnityFramework.framework", false);
        pbxProject.SetBuildProperty(unityMainTargetGuid, "GCC_ENABLE_OBJC_EXCEPTIONS", "YES");
        pbxProject.SetBuildProperty(unityMainTargetGuid, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "YES");
        pbxProject.SetBuildProperty(unityMainTargetGuid, "ENABLE_BITCODE", "NO");
        //pbxProject.(target, "CLANSetBuildPropertyG_ENABLE_MODULES", "YES");

        // Unity Framework
        var unityFrameworkTargetGuid = pbxProject.GetUnityFrameworkTargetGuid();
        pbxProject.SetBuildProperty(unityFrameworkTargetGuid, "ENABLE_BITCODE", "NO");
        pbxProject.AddBuildProperty(unityFrameworkTargetGuid, "OTHER_LDFLAGS", "-ObjC");
        pbxProject.AddBuildProperty(unityFrameworkTargetGuid, "OTHER_LDFLAGS", "-lc++");
        pbxProject.SetBuildProperty(unityFrameworkTargetGuid, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "NO");

        // TODO : Naver Lounge : 사용할때 주석 제거
        // pbxProject.AddFrameworkToProject(unityFrameworkTargetGuid, "SafariServices.framework", false);
        // pbxProject.AddFrameworkToProject(unityFrameworkTargetGuid, "NaverLogin.framework", false);

        // TODO : LIAPP IOS 적용을 위한 Xcode 세팅 : 사용할때 주석 제거
        // pbxProject.AddFrameworkToProject(unityFrameworkTargetGuid, "Security.framework", false);
        // pbxProject.AddFrameworkToProject(unityFrameworkTargetGuid, "UIKit.framework", false);
        // pbxProject.AddFrameworkToProject(unityFrameworkTargetGuid, "CFNetwork.framework", false);
        // pbxProject.AddFrameworkToProject(unityFrameworkTargetGuid, "SystemConfiguration.framework", false);
        // pbxProject.AddFrameworkToProject(unityFrameworkTargetGuid, "libLiapp.a", false);

        // AddPushNotification
        AddPushNotification(pbxProject, projectPath, unityMainTargetGuid);

        //AddSignInApple(pathToBuildProject);

        pbxProject.WriteToFile(projectPath);

        // Info.plist
        SetInfoPlist(pathToBuildProject);

        FixPodFile(pathToBuildProject);
    }

    private static void SetInfoPlist(string pathToBuildProject)
    {
        var plistPath = Path.Combine(pathToBuildProject, "Info.plist");
        PlistDocument plistDoc = new PlistDocument();
        plistDoc.ReadFromFile(plistPath);
        if (plistDoc.root != null)
        {
            plistDoc.root.SetString("NSCameraUsageDescription", "This app uses the camera to take pictures.");
            plistDoc.root.SetString("NSPhotoLibraryUsageDescription", "This app uses the photo album to import pictures.");
            plistDoc.WriteToFile(plistPath);
        }
    }

    private static void AddPushNotification(PBXProject project, string xCodeProjectPath, string xCodeTarget)
    {
        string entitlementsFileName = "Unity-iPhone.entitlements";

#if UNITY_2019_3_OR_NEWER
        var projectCapabilityManager = new ProjectCapabilityManager(xCodeProjectPath, entitlementsFileName, null, project.GetUnityMainTargetGuid());
#else
        var projectCapabilityManager = new ProjectCapabilityManager(xCodeProjectPath, entitlementsFileName, PBXProject.GetUnityTargetName());
#endif

#if __DEV
        projectCapabilityManager.AddPushNotifications(true);
#else
        projectCapabilityManager.AddPushNotifications(false);
#endif

        projectCapabilityManager.WriteToFile();
    }

    private static void AddSignInApple(string path)
    {
//         var projectPath = PBXProject.GetPBXProjectPath(path);
//
//         // Adds entitlement depending on the Unity version used
// #if UNITY_2019_3_OR_NEWER
//         var project = new PBXProject();
//         project.ReadFromString(System.IO.File.ReadAllText(projectPath));
//         var manager = new ProjectCapabilityManager(projectPath, "Unity-iPhone.entitlements", null, project.GetUnityMainTargetGuid());
//         manager.AddSignInWithAppleWithCompatibility(project.GetUnityFrameworkTargetGuid());
//         manager.WriteToFile();
// #else
//         var manager = new ProjectCapabilityManager(projectPath, "Unity-iPhone.entitlements", PBXProject.GetUnityTargetName());
//         manager.AddSignInWithAppleWithCompatibility();
//         manager.WriteToFile();
// #endif
    }

    /// <summary>
    /// xcode 14.3 빌드 에러관련 추가 스크립트
    /// </summary>
    /// <param name="buildPath"></param>
    private static void FixPodFile(string buildPath)
    {
        using var sw = File.AppendText(buildPath + "/Podfile");
        sw.WriteLine("post_install do |installer|");
        sw.WriteLine("\tinstaller.pods_project.targets.each do |target|");
        sw.WriteLine("\t\ttarget.build_configurations.each do |config|");
        //sw.WriteLine("\t\t\tconfig.build_settings['IPHONEOS_DEPLOYMENT_TARGET'] = '11.0'");
        sw.WriteLine("\t\t\tconfig.build_settings['EXPANDED_CODE_SIGN_IDENTITY'] = \"\"");
        sw.WriteLine("\t\t\tconfig.build_settings['CODE_SIGNING_REQUIRED'] = \"NO\"");
        sw.WriteLine("\t\t\tconfig.build_settings['CODE_SIGNING_ALLOWED'] = \"NO\"");
        sw.WriteLine("\t\tend");
        sw.WriteLine("\tend");
        sw.WriteLine("end");
    }
}
#endif

