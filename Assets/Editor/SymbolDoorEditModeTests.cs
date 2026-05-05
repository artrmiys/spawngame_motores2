#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class SymbolDoorEditModeTests
{
    [Test]
    public void DefaultBuilderCreatesThreeExpectedPairs()
    {
        SymbolDoorPuzzleData data = SymbolDoorPuzzleBuilder.CreateDefault().Build();

        Assert.AreEqual(3, data.Symbols.Count);
        Assert.AreEqual(3, data.Meanings.Count);
        Assert.AreEqual("POWER", data.CorrectPairs["PWR"]);
        Assert.AreEqual("TARGET", data.CorrectPairs["TGT"]);
        Assert.AreEqual("SHIELD", data.CorrectPairs["SHD"]);
    }

    [Test]
    public void BuilderSkipsInvalidAndDuplicatePairs()
    {
        SymbolDoorPuzzleData data = new SymbolDoorPuzzleBuilder()
            .AddPair("A", "One")
            .AddPair("", "Empty")
            .AddPair("B", "")
            .AddPair("A", "Duplicate")
            .Build();

        Assert.AreEqual(1, data.Symbols.Count);
        Assert.AreEqual(1, data.Meanings.Count);
        Assert.AreEqual("A", data.Symbols[0]);
        Assert.AreEqual("One", data.CorrectPairs["A"]);
    }

    [Test]
    public void ModelAcceptsOnlyCompleteCorrectAnswers()
    {
        var symbols = new List<string> { "A", "B", "C" };
        var meanings = new List<string> { "1", "2", "3" };
        var pairs = new Dictionary<string, string>
        {
            { "A", "1" },
            { "B", "2" },
            { "C", "3" }
        };

        var model = new SymbolDoorModel(symbols, meanings, pairs);

        model.RegisterAnswer("A", "1");
        model.RegisterAnswer("B", "2");
        Assert.IsFalse(model.IsAnswerComplete());
        Assert.IsFalse(model.CheckAnswers());

        model.RegisterAnswer("C", "3");
        Assert.IsTrue(model.IsAnswerComplete());
        Assert.IsTrue(model.CheckAnswers());

        model.RegisterAnswer("C", "2");
        Assert.IsTrue(model.IsAnswerComplete());
        Assert.IsFalse(model.CheckAnswers());
    }

    [Test]
    public void ViewBuildsScalableButtonTiles()
    {
        SymbolDoorPuzzleData data = SymbolDoorPuzzleBuilder.CreateDefault().Build();
        GameObject viewObject = new GameObject("SymbolDoorViewTest");
        GameObject controllerObject = new GameObject("SymbolDoorControllerTest");

        try
        {
            SymbolDoorView view = viewObject.AddComponent<SymbolDoorView>();
            SymbolDoorController controller = controllerObject.AddComponent<SymbolDoorController>();

            view.Setup(data.Symbols, data.Meanings, controller);

            CanvasScaler scaler = viewObject.GetComponent<CanvasScaler>();
            Assert.IsNotNull(scaler);
            Assert.AreEqual(CanvasScaler.ScaleMode.ScaleWithScreenSize, scaler.uiScaleMode);
            Assert.AreEqual(new Vector2(1080, 1920), scaler.referenceResolution);

            Transform main = viewObject.transform.Find("MainContainer");
            Assert.IsNotNull(main);
            AssertRectStretchesToParent(main.GetComponent<RectTransform>(), "MainContainer");

            Transform content = main.Find("ContentArea");
            Assert.IsNotNull(content);
            RectTransform contentRect = content.GetComponent<RectTransform>();
            Assert.AreEqual(new Vector2(0f, 0.18f), contentRect.anchorMin);
            Assert.AreEqual(new Vector2(1f, 0.84f), contentRect.anchorMax);

            Button[] buttons = viewObject.GetComponentsInChildren<Button>(true);
            Assert.AreEqual(8, buttons.Length);

            foreach (Button button in buttons)
            {
                Assert.IsNotNull(button.GetComponent<Image>(), button.name);
                Assert.IsNotNull(button.GetComponent<Outline>(), button.name);
                Assert.IsNotNull(button.GetComponent<Shadow>(), button.name);

                LayoutElement layout = button.GetComponent<LayoutElement>();
                Assert.IsNotNull(layout, button.name);
                Assert.GreaterOrEqual(layout.preferredHeight, 58f, button.name);

                Text label = button.GetComponentInChildren<Text>(true);
                Assert.IsNotNull(label, button.name);
                Assert.IsFalse(label.raycastTarget, button.name);
                AssertRectStretchesToParent(label.rectTransform, button.name);
            }

            Assert.IsTrue(HasText(viewObject, "COMBAT CORE REPAIR"));
            Assert.IsTrue(HasText(viewObject, "CORE\nOFFLINE"));
            Assert.IsTrue(HasText(viewObject, "SYSTEM FAULT"));
            Assert.IsTrue(HasText(viewObject, "REPAIR"));
            Assert.IsTrue(HasText(viewObject, "CLEAR"));
            Assert.IsFalse(HasText(viewObject, "SYMBOL DOOR"));
            Assert.IsFalse(HasText(viewObject, "LOCK\nDOOR"));
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(controllerObject);
            UnityEngine.Object.DestroyImmediate(viewObject);
        }
    }

    [Test]
    public void ViewSetupRebuildDoesNotDuplicateTiles()
    {
        SymbolDoorPuzzleData data = SymbolDoorPuzzleBuilder.CreateDefault().Build();
        GameObject viewObject = new GameObject("SymbolDoorViewRebuildTest");
        GameObject controllerObject = new GameObject("SymbolDoorControllerRebuildTest");

        try
        {
            SymbolDoorView view = viewObject.AddComponent<SymbolDoorView>();
            SymbolDoorController controller = controllerObject.AddComponent<SymbolDoorController>();

            view.Setup(data.Symbols, data.Meanings, controller);
            int childCount = viewObject.transform.childCount;

            view.Setup(data.Symbols, data.Meanings, controller);

            Assert.AreEqual(childCount, viewObject.transform.childCount);
            Assert.AreEqual(8, viewObject.GetComponentsInChildren<Button>(true).Length);
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(controllerObject);
            UnityEngine.Object.DestroyImmediate(viewObject);
        }
    }

    [Test]
    public void DoorOpenDelayUsesRealtimeWhileGameIsPaused()
    {
        GameObject doorObject = new GameObject("SymbolDoorRealtimeTest");

        try
        {
            SymbolDoor door = doorObject.AddComponent<SymbolDoor>();
            MethodInfo method = typeof(SymbolDoor).GetMethod("DoorOpenRoutine", BindingFlags.Instance | BindingFlags.NonPublic);

            Assert.IsNotNull(method);

            Time.timeScale = 0f;
            IEnumerator routine = (IEnumerator)method.Invoke(door, null);

            Assert.IsTrue(routine.MoveNext());
            Assert.IsInstanceOf<WaitForSecondsRealtime>(routine.Current);
        }
        finally
        {
            Time.timeScale = 1f;
            UnityEngine.Object.DestroyImmediate(doorObject);
        }
    }

    [Test]
    public void EventManagerBroadcastsAnswerCheckedResult()
    {
        bool? receivedResult = null;

        try
        {
            SymbolDoorEventManager.ClearAllListeners();
            SymbolDoorEventManager.OnAnswerChecked += result => receivedResult = result;

            SymbolDoorEventManager.RaiseAnswerChecked(true);

            Assert.IsTrue(receivedResult.HasValue);
            Assert.IsTrue(receivedResult.Value);
        }
        finally
        {
            SymbolDoorEventManager.ClearAllListeners();
        }
    }

    [Test]
    public void PowerUpSpawnIntervalIsHalfOfConfigured()
    {
        Assert.AreEqual(4f, PowerUpSpawner.GetEffectiveSpawnInterval(8f), 0.001f);
        Assert.AreEqual(1.5f, PowerUpSpawner.GetEffectiveSpawnInterval(3f), 0.001f);
    }

    [Test]
    public void WaveSpawnCountsAreFiftyPercentLonger()
    {
        Assert.AreEqual(8, WaveManager.GetWaveEnemySpawnCount(5, 1));
        Assert.AreEqual(11, WaveManager.GetWaveEnemySpawnCount(5, 2));
        Assert.AreEqual(14, WaveManager.GetWaveEnemySpawnCount(5, 3));
    }

    private static void AssertRectStretchesToParent(RectTransform rect, string name)
    {
        Assert.IsNotNull(rect, name);
        Assert.AreEqual(Vector2.zero, rect.anchorMin, name);
        Assert.AreEqual(Vector2.one, rect.anchorMax, name);
    }

    private static bool HasText(GameObject root, string expected)
    {
        foreach (Text text in root.GetComponentsInChildren<Text>(true))
        {
            if (text != null && text.text == expected)
                return true;
        }

        return false;
    }
}

public static class SymbolDoorAutomatedTestRunner
{
    public static void RunAllFromCommandLine()
    {
        var testClass = new SymbolDoorEditModeTests();
        var tests = new (string Name, Action Run)[]
        {
            (nameof(SymbolDoorEditModeTests.DefaultBuilderCreatesThreeExpectedPairs), testClass.DefaultBuilderCreatesThreeExpectedPairs),
            (nameof(SymbolDoorEditModeTests.BuilderSkipsInvalidAndDuplicatePairs), testClass.BuilderSkipsInvalidAndDuplicatePairs),
            (nameof(SymbolDoorEditModeTests.ModelAcceptsOnlyCompleteCorrectAnswers), testClass.ModelAcceptsOnlyCompleteCorrectAnswers),
            (nameof(SymbolDoorEditModeTests.ViewBuildsScalableButtonTiles), testClass.ViewBuildsScalableButtonTiles),
            (nameof(SymbolDoorEditModeTests.ViewSetupRebuildDoesNotDuplicateTiles), testClass.ViewSetupRebuildDoesNotDuplicateTiles),
            (nameof(SymbolDoorEditModeTests.DoorOpenDelayUsesRealtimeWhileGameIsPaused), testClass.DoorOpenDelayUsesRealtimeWhileGameIsPaused),
            (nameof(SymbolDoorEditModeTests.EventManagerBroadcastsAnswerCheckedResult), testClass.EventManagerBroadcastsAnswerCheckedResult),
            (nameof(SymbolDoorEditModeTests.PowerUpSpawnIntervalIsHalfOfConfigured), testClass.PowerUpSpawnIntervalIsHalfOfConfigured),
            (nameof(SymbolDoorEditModeTests.WaveSpawnCountsAreFiftyPercentLonger), testClass.WaveSpawnCountsAreFiftyPercentLonger)
        };

        var lines = new List<string>();
        int failed = 0;

        foreach (var test in tests)
        {
            try
            {
                test.Run();
                lines.Add("PASS " + test.Name);
                Debug.Log("[SymbolDoorAutomatedTestRunner] PASS " + test.Name);
            }
            catch (Exception ex)
            {
                failed++;
                lines.Add("FAIL " + test.Name + ": " + ex);
                Debug.LogException(ex);
            }
        }

        string resultPath = GetCommandLineValue("-symbolDoorTestResults");
        if (string.IsNullOrEmpty(resultPath))
            resultPath = Path.Combine(Application.dataPath, "..", "Temp", "SymbolDoorAutomatedTestResults.txt");

        try
        {
            string resultDirectory = Path.GetDirectoryName(resultPath);
            if (!string.IsNullOrEmpty(resultDirectory))
                Directory.CreateDirectory(resultDirectory);

            File.WriteAllText(resultPath, string.Join(Environment.NewLine, lines));
            Debug.Log("[SymbolDoorAutomatedTestRunner] Results written: " + resultPath);
        }
        catch (Exception ex)
        {
            failed++;
            Debug.LogException(ex);
        }

        if (Application.isBatchMode)
            EditorApplication.Exit(failed == 0 ? 0 : 1);
    }

    private static string GetCommandLineValue(string key)
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
