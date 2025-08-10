#load "FileFormatter.csx"
#r "System"
using System.IO;
using System.Linq;
using System;
#nullable enable

// run: dotnet script .\FormatEpisodes

var dir = Path.Combine("F:", "1Serien", "Mannezimmer");
var seriesKey = "Staffel";
// var episodeKey = "Episode";
var episodeKey = "Folge";

var formatter = new FileFormatter(seriesKey, episodeKey);

var files = Directory.EnumerateFiles(dir, "*", SearchOption.AllDirectories);
var executionMode = ExecutionMode.None;
foreach (var file in files)
{
    var fileName = Path.GetFileName(file);
    var isFormatted = formatter.TryFormat(fileName, out var formatted);

    if (fileName == formatted)
    {
        System.Console.WriteLine($"No formatting needed for: '{fileName}'");
        continue;
    }

    Console.WriteLine($"actual: {fileName}");
    Console.WriteLine($"rename: {formatted}");

    while (executionMode is ExecutionMode.None)
    {
        executionMode = AskUser();
    }

    try
    {
        if (executionMode is ExecutionMode.Break)
        {
            System.Console.WriteLine(" -> Exit");
            break;
        }
        else if (executionMode is ExecutionMode.Skip)
        {
            System.Console.WriteLine(" -> Skip");
            continue;
        }
        else if (executionMode is ExecutionMode.Process or ExecutionMode.ProcessAll)
        {
            System.Console.WriteLine(" -> Process");
            var dir = Path.GetDirectoryName(file);
            var newFileName = Path.Combine(dir ?? ".\\", formatted);
            File.Move(file, newFileName);
        }
    }
    finally
    {
        if (executionMode is not ExecutionMode.ProcessAll)
        {
            executionMode = ExecutionMode.None;
        }
    }

}

private ExecutionMode AskUser()
{
    System.Console.Write($"process [p]? process-all [pa]? skip [s]? exit?: ");
    var userInput = Console.ReadLine() ?? string.Empty;

    return userInput switch
    {
        "exit" => ExecutionMode.Break,
        "skip" or "s" => ExecutionMode.Skip,
        "process" or "p" => ExecutionMode.Process,
        "process-all" or "pa" => ExecutionMode.ProcessAll,
        _ => ExecutionMode.None
    };
}

public enum ExecutionMode { None, Break, Skip, Process, ProcessAll }
