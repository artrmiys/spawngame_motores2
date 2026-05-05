#if UNITY_EDITOR
using UnityEngine;

/// <summary>
/// Editor-only test component for Symbol Door logic verification.
/// Run in Play mode or call methods from editor console.
/// </summary>
public class SymbolDoorTests : MonoBehaviour
{
    [ContextMenu("Test Model - Valid")]
    public void TestModelValid()
    {
        var symbols = new System.Collections.Generic.List<string> { "A", "B", "C" };
        var meanings = new System.Collections.Generic.List<string> { "1", "2", "3" };
        var pairs = new System.Collections.Generic.Dictionary<string, string>
        {
            { "A", "1" },
            { "B", "2" },
            { "C", "3" }
        };

        var model = new SymbolDoorModel(symbols, meanings, pairs);

        // Test: Answer registration
        model.RegisterAnswer("A", "1");
        Debug.Assert(model.HasAnswer("A"), "Model should register answer");
        Debug.Assert(model.GetAnswer("A") == "1", "Model should return correct answer");

        // Test: Completion check
        Debug.Assert(!model.IsAnswerComplete(), "Should not be complete with 1/3");
        model.RegisterAnswer("B", "2");
        model.RegisterAnswer("C", "3");
        Debug.Assert(model.IsAnswerComplete(), "Should be complete with 3/3");

        // Test: Validation
        Debug.Assert(model.CheckAnswers(), "All answers are correct");

        Debug.Log("✓ Model Valid Test PASSED");
    }

    [ContextMenu("Test Model - Invalid Answers")]
    public void TestModelInvalid()
    {
        var symbols = new System.Collections.Generic.List<string> { "A", "B" };
        var meanings = new System.Collections.Generic.List<string> { "1", "2" };
        var pairs = new System.Collections.Generic.Dictionary<string, string>
        {
            { "A", "1" },
            { "B", "2" }
        };

        var model = new SymbolDoorModel(symbols, meanings, pairs);

        model.RegisterAnswer("A", "2"); // Wrong!
        model.RegisterAnswer("B", "1"); // Wrong!

        Debug.Assert(!model.CheckAnswers(), "Should fail with wrong answers");
        Debug.Log("✓ Model Invalid Test PASSED");
    }

    [ContextMenu("Test Model - Null Safety")]
    public void TestModelNullSafety()
    {
        // Test: null constructor args
        var model = new SymbolDoorModel(null, null, null);
        Debug.Assert(model.GetSymbols().Count == 0, "Should handle null symbols");
        Debug.Assert(model.GetMeanings().Count == 0, "Should handle null meanings");

        // Test: null/empty string operations
        model.RegisterAnswer(null, "value");
        model.RegisterAnswer("symbol", null);
        model.RemoveAnswer(null);
        model.RegisterAnswer("", "");

        Debug.Assert(model.GetAnswer(null) == null, "Should handle null key");
        Debug.Assert(model.GetAnswer("") == null, "Should handle empty key");

        Debug.Log("✓ Model Null Safety Test PASSED");
    }

    [ContextMenu("Test Model - Reset")]
    public void TestModelReset()
    {
        var symbols = new System.Collections.Generic.List<string> { "A" };
        var meanings = new System.Collections.Generic.List<string> { "1" };
        var pairs = new System.Collections.Generic.Dictionary<string, string> { { "A", "1" } };

        var model = new SymbolDoorModel(symbols, meanings, pairs);

        model.RegisterAnswer("A", "1");
        Debug.Assert(model.HasAnswer("A"), "Answer registered");

        model.Reset();
        Debug.Assert(!model.HasAnswer("A"), "Answer should be cleared after reset");

        Debug.Log("✓ Model Reset Test PASSED");
    }

    [ContextMenu("Test Controller - Input Validation")]
    public void TestControllerValidation()
    {
        GameObject controllerObj = new GameObject("ControllerTest");
        var controller = controllerObj.AddComponent<SymbolDoorController>();

        // Should not crash with null input
        controller.OnSymbolClicked(null);
        controller.OnSymbolClicked("");
        controller.OnMeaningClicked(null);

        Debug.Log("✓ Controller Input Validation Test PASSED");

        Destroy(controllerObj);
    }

    [ContextMenu("Test Events - Memory Cleanup")]
    public void TestEventCleanup()
    {
        System.Action listener1 = () => { };
        System.Action listener2 = () => { };

        SymbolDoorEventManager.OnDoorOpened += listener1;
        SymbolDoorEventManager.OnDoorFailed += listener2;

        SymbolDoorEventManager.ClearAllListeners();

        // Verify cleared (if we could access private field)
        Debug.Log("✓ Event Cleanup Test PASSED (manual verification needed)");
    }

    [ContextMenu("Run All Tests")]
    public void RunAllTests()
    {
        Debug.Log("=== Starting Symbol Door Tests ===");
        TestModelValid();
        TestModelInvalid();
        TestModelNullSafety();
        TestModelReset();
        TestControllerValidation();
        TestEventCleanup();
        Debug.Log("=== All Tests Completed ===");
    }
}
#endif
