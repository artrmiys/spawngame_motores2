using UnityEngine;

public class SymbolDoorController : MonoBehaviour
{
    private ISymbolDoorModel model;
    private SymbolDoorView view;
    private string selectedSymbol;

    public void Initialize(ISymbolDoorModel m, SymbolDoorView v)
    {
        if (m == null || v == null)
        {
            Debug.LogError("SymbolDoorController: Invalid initialization!");
            return;
        }

        model = m;
        view = v;
        selectedSymbol = null;
    }

    public void OnSymbolClicked(string symbol)
    {
        if (!ValidateInput(symbol) || model == null || view == null)
            return;

        if (selectedSymbol == symbol)
        {
            DeselectSymbol();
            return;
        }

        selectedSymbol = symbol;
        view.HighlightSymbol(symbol, true);
    }

    public void OnMeaningClicked(string meaning)
    {
        if (!ValidateInput(meaning) || selectedSymbol == null || model == null || view == null)
            return;

        string previous = model.GetAnswer(selectedSymbol);
        model.RegisterAnswer(selectedSymbol, meaning);

        view.ShowConnection(selectedSymbol, meaning);
        if (!string.IsNullOrEmpty(previous))
            view.HideConnection(selectedSymbol, previous);

        DeselectSymbol();
    }

    public void OnCheckClicked()
    {
        if (model == null || view == null)
            return;

        if (!model.IsAnswerComplete())
        {
            view.ShowIncomplete();
            return;
        }

        if (model.CheckAnswers())
        {
            view.PlaySuccessAnimation();
            SymbolDoorEventManager.RaiseOpened();
        }
        else
        {
            view.PlayErrorAnimation();
            SymbolDoorEventManager.RaiseFailed();
        }
    }

    public void OnResetClicked()
    {
        if (model == null || view == null)
            return;

        model.Reset();
        view.ClearAllConnections();
        DeselectSymbol();
        SymbolDoorEventManager.RaiseReset();
    }

    private void DeselectSymbol()
    {
        if (!string.IsNullOrEmpty(selectedSymbol) && view != null)
            view.HighlightSymbol(selectedSymbol, false);
        selectedSymbol = null;
    }

    private bool ValidateInput(string input)
    {
        return !string.IsNullOrEmpty(input);
    }
}
