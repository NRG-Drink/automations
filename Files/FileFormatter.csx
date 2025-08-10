#r "System"

public class FileFormatter(string seriesKey, string episodeKey)
{
    public bool TryFormat(string fileName, out string formatted)
    {
        formatted = fileName;
        if (TryGetSE(fileName, out var _))
        {
            // No formatting needed.
            return true;
        }

        var isSeries = TryGetNumberAfter(fileName, seriesKey, out var serieNumber);
        var isEpisode = TryGetNumberAfter(fileName, episodeKey, out var episodeNumber);
        if (!isSeries && !isEpisode)
        {
            System.Console.WriteLine($"Error: Series or episode numbers could not be read from file-name: '{fileName}'");
            System.Console.WriteLine($"Series = {isSeries} [key: {seriesKey}], Episode = {isEpisode} [key: {episodeKey}]");
            return false;
        }

        formatted = Insert(fileName, serieNumber, episodeNumber);
        return true;
    }

    private string Insert(string input, int s, int e)
    {
        var index = input.IndexOf('-');
        var before = input[..index];
        var after = input[index..];

        return $"{before}- S{s:00}E{e:00} {after}";
    }

    private bool TryGetSE(string input, out (int, int) res)
    {
        res = (default, default);
        var wip = input;
        var index = input.IndexOf('S');
        while (index > 0)
        {
            wip = wip[index..];
            if (wip is ['S', var s1, var s2, 'E', var e1, var e2, ..])
            {
                if (int.TryParse([s1, s2], out var s)
                    && int.TryParse([e1, e2], out var e))
                {
                    res = (s, e);
                    return true;
                }
            }

            index = wip.IndexOf('S');
        }

        return false;
    }

    private bool TryGetNumberAfter(string input, string key, out int number)
    {
        number = default;
        var index = input.IndexOf(key);
        if (index is < 0)
        {
            return false;
        }

        index += key.Length;
        var split = input[index..];
        var cut = split
            .TakeWhile(e => char.IsWhiteSpace(e) || char.IsDigit(e))
            .Where(e => char.IsDigit(e));

        return int.TryParse([.. cut], out number);
    }
}