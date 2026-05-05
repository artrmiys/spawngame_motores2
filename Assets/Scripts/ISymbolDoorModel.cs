using System.Collections.Generic;

public interface ISymbolDoorModel
{
    void RegisterAnswer(string symbol, string meaning);
    void RemoveAnswer(string symbol);
    bool IsAnswerComplete();
    bool CheckAnswers();
    bool HasAnswer(string symbol);
    string GetAnswer(string symbol);
    void Reset();
    List<string> GetSymbols();
    List<string> GetMeanings();
}
