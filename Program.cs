using System;
using System.Diagnostics;
using System.Linq;
using BatchReplacer.Systems;

var benchmark = new Stopwatch();
benchmark.Start();
IConfigReader reader = new JsonConfigReader();
var (paths, filter, fileNameReplacement, contentReplacements) = reader.Read();
FilesParser filesParser = new(fileNameReplacement, contentReplacements.ToArray());
var filesFound = filesParser.GetFiles(filter, paths).ToArray();
Console.WriteLine($"Processing {filesFound.Length} files:");

await foreach (var (file, result) in filesParser.ParseFilesAsync(filesFound))
{
    Console.WriteLine($"[{file}] processed.");

    if (result.changed)
    {
        await filesParser.SaveContent(file, result.content);
        Console.WriteLine($"[{file}] updated.");
    }

    if (filesParser.ParseFileName(file, out var newName))
    {
        Console.WriteLine($"[{file}] renamed to [{newName}].");
    }
}

benchmark.Stop();
Console.WriteLine($"Completed in {benchmark.ElapsedMilliseconds} ms");