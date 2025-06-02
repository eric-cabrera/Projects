namespace Assurity.AgentPortal.Utilities.WordWrap;

using System.Text;

public static class WordWrap
{
    public static string Wrap(string input, int charLimit)
    {
        string[] words = input.Split(' ');
        StringBuilder newSentence = new();
        string line = string.Empty;

        foreach (string word in words)
        {
            if ((line + word).Length > charLimit)
            {
                newSentence.AppendLine(line.Trim());
                line = string.Empty;
            }

            line += string.Format("{0} ", word);
        }

        if (line.Length > 0)
        {
            newSentence.AppendLine(line);
        }

        return newSentence.ToString();
    }
}
