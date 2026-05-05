using System.Collections.Generic;

public class SymbolDoorModel : ISymbolDoorModel
{
    private readonly List<string> symbols = new();
    private readonly List<string> meanings = new();
    private readonly Dictionary<string, string> correctPairs = new();
    private readonly Dictionary<string, string> playerAnswers = new();

    public SymbolDoorModel(List<string> syms, List<string> meanings, Dictionary<string, string> pairs)
    {
        if (syms != null) symbols.AddRange(syms);
        if (meanings != null) this.meanings.AddRange(meanings);
        if (pairs != null)
        {
            foreach (var pair in pairs)
                correctPairs[pair.Key] = pair.Value;
        }
    }

    public void RegisterAnswer(string symbol, string meaning)
    {
        if (string.IsNullOrEmpty(symbol) || string.IsNullOrEmpty(meaning))
            return;

        playerAnswers[symbol] = meaning;
    }

    public void RemoveAnswer(string symbol)
    {
        if (string.IsNullOrEmpty(symbol))
            return;

        playerAnswers.Remove(symbol);
    }

    public bool IsAnswerComplete()
    {
        return playerAnswers.Count == symbols.Count && symbols.Count > 0;
    }

    public bool CheckAnswers()
    {
        if (!IsAnswerComplete())
            return false;

        foreach (var symbol in symbols)
        {
            if (!playerAnswers.ContainsKey(symbol))
                return false;

            if (!correctPairs.ContainsKey(symbol))
                return false;

            if (correctPairs[symbol] != playerAnswers[symbol])
                return false;
        }

        return true;
    }

    public bool HasAnswer(string symbol)
    {
        return !string.IsNullOrEmpty(symbol) && playerAnswers.ContainsKey(symbol);
    }

    public string GetAnswer(string symbol)
    {
        return string.IsNullOrEmpty(symbol) || !playerAnswers.ContainsKey(symbol)
            ? null
            : playerAnswers[symbol];
    }

    public void Reset()
    {
        playerAnswers.Clear();
    }

    public List<string> GetSymbols() => new List<string>(symbols);
    public List<string> GetMeanings() => new List<string>(meanings);
}
