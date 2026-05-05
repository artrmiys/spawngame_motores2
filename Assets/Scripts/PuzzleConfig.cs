using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SymbolPair
{
    public string symbol;
    public string meaning;
}

[CreateAssetMenu(fileName = "PuzzleConfig", menuName = "SymbolDoor/PuzzleConfig")]
public class PuzzleConfig : ScriptableObject
{
    [SerializeField] private List<SymbolPair> pairs = new();

    public List<string> GetSymbols()
    {
        List<string> result = new();
        foreach (var pair in pairs)
            result.Add(pair.symbol);
        return result;
    }

    public List<string> GetMeanings()
    {
        List<string> result = new();
        foreach (var pair in pairs)
            result.Add(pair.meaning);
        return result;
    }

    public Dictionary<string, string> GetPairs()
    {
        Dictionary<string, string> result = new();
        foreach (var pair in pairs)
            result[pair.symbol] = pair.meaning;
        return result;
    }

    public int GetPairCount() => pairs.Count;
}
