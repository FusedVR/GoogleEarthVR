using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using System.IO;

[InitializeOnLoad]
public class FirstInstallActionsRunner : Editor
{
    static FirstInstallActionsRunner()
    {
        RunActions();
    }

    static void DisplayWelcomeDialog()
    {
        string message = "Thank you for downloading the WRLD Unity SDK!\n\nPlease register for an API key to get started.";

        if (EditorUtility.DisplayDialog("WRLD", message, "Get Key", "Later"))
        {
            Application.OpenURL("https://wrld3d.com/register?utm_source=UnitySDK&utm_medium=UnityEditor");
        }
    }

    static void ShadowDialog()
    {
        string message = "Your current Shadow Distance settings are below recommendations for WRLD Maps.\n(Shadow Distances need to be increased for shadows to be visible)\n\nWould you like increase them?\n\n(To revert go to: Edit > Project Settings > Quallity)";

        if (EditorUtility.DisplayDialog("WRLD - Shadow Distance Settings", message, "Increase", "Skip"))
        {
            QualitySettings.shadowDistance = Wrld.Constants.RecommendedShadowDistance;
        }
    }

    private static void PerformBuildTargetSwitch()
    {
        if (!EditorApplication.isUpdating)
        {
            if (EditorUserBuildSettings.activeBuildTarget != m_desiredBuildTarget)
            {
                EditorUserBuildSettings.SwitchActiveBuildTarget(m_desiredBuildTarget);
                Debug.LogFormat("Current Settings not supported. Changed Build Target to: {0:G}", m_desiredBuildTarget);
                EditorApplication.update -= PerformBuildTargetSwitch;
            }
        }
    }

    public static void ChangeBuildTarget()
    {
        BuildTarget currentTarget = EditorUserBuildSettings.activeBuildTarget;
        BuildTarget newTarget = currentTarget;

        switch (currentTarget)
        {
            case BuildTarget.StandaloneWindows:
            {
                newTarget = BuildTarget.StandaloneWindows64;
                break;
            }
            case BuildTarget.StandaloneOSXIntel:
            case BuildTarget.StandaloneOSXUniversal:
            {
                newTarget = BuildTarget.StandaloneOSXIntel64;
                break;
            }
            default:
                break;
        }

        if (currentTarget != newTarget)
        {
            m_desiredBuildTarget = newTarget;
            EditorApplication.update += PerformBuildTargetSwitch;
        }
    }

    public static void RunActions()
    {
        const string WelcomeGuardFile = "Assets/Wrld/Version/WelcomeConfig.txt";
        const string ShadowGuardFile = "Assets/Wrld/Version/ShadowConfig.txt";
        
        if (!UnityEditorInternal.InternalEditorUtility.inBatchMode)
        {
            if (!File.Exists(WelcomeGuardFile))
            {
                DisplayWelcomeDialog();
                ChangeBuildTarget();
                CopyPlatformResources.RenameFiles();

                StreamWriter file = File.CreateText(WelcomeGuardFile);
                file.WriteLine("Delete this to get welcome message again.");
                file.Close();
                AssetDatabase.Refresh();
            }

            if (!File.Exists(ShadowGuardFile) && (QualitySettings.shadowDistance < Wrld.Constants.RecommendedShadowDistance))
            {
                ShadowDialog();

                var file = File.CreateText(ShadowGuardFile);
                file.WriteLine("Delete This to get shadow settings messages again.");
                file.Close();
                AssetDatabase.Refresh();
            }
        }
    }

    private static BuildTarget m_desiredBuildTarget;
}
