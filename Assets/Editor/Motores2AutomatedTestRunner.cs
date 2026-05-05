#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

public static class Motores2AutomatedTestRunner
{
    public static void RunAllFromCommandLine()
    {
        Type[] testTypes =
        {
            typeof(SymbolDoorEditModeTests),
            typeof(VisualDesignEditModeTests)
        };

        var lines = new List<string>();
        int failed = 0;

        foreach (Type type in testTypes)
        {
            object instance = Activator.CreateInstance(type);
            IEnumerable<MethodInfo> methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .Where(method => method.GetParameters().Length == 0 && method.GetCustomAttributes(typeof(TestAttribute), true).Length > 0)
                .OrderBy(method => method.Name);

            foreach (MethodInfo method in methods)
            {
                string testName = type.Name + "." + method.Name;
                try
                {
                    method.Invoke(instance, null);
                    lines.Add("PASS " + testName);
                    Debug.Log("[Motores2AutomatedTestRunner] PASS " + testName);
                }
                catch (TargetInvocationException ex)
                {
                    failed++;
                    Exception inner = ex.InnerException ?? ex;
                    lines.Add("FAIL " + testName + ": " + inner);
                    Debug.LogException(inner);
                }
                catch (Exception ex)
                {
                    failed++;
                    lines.Add("FAIL " + testName + ": " + ex);
                    Debug.LogException(ex);
                }
            }
        }

        string resultPath = GetCommandLineValue("-motores2TestResults");
        if (string.IsNullOrEmpty(resultPath))
            resultPath = Path.Combine(Application.dataPath, "..", "Temp", "Motores2AutomatedTestResults.txt");

        WriteResults(resultPath, lines, ref failed);

        if (Application.isBatchMode)
            EditorApplication.Exit(failed == 0 ? 0 : 1);
    }

    static void WriteResults(string resultPath, List<string> lines, ref int failed)
    {
        try
        {
            string resultDirectory = Path.GetDirectoryName(resultPath);
            if (!string.IsNullOrEmpty(resultDirectory))
                Directory.CreateDirectory(resultDirectory);

            File.WriteAllText(resultPath, string.Join(Environment.NewLine, lines));
            Debug.Log("[Motores2AutomatedTestRunner] Results written: " + resultPath);
        }
        catch (Exception ex)
        {
            failed++;
            Debug.LogException(ex);
        }
    }

    static string GetCommandLineValue(string key)
    {
        string[] args = Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length - 1; i++)
        {
            if (args[i] == key)
                return args[i + 1];
        }

        return null;
    }
}
#endif
