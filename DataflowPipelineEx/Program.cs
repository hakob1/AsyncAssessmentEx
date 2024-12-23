using System.Threading.Tasks.Dataflow;

class DataflowPipelineExample
{
    static async Task Main()
    {
            
        string inputFile = "C:\\Users\\user\\source\\repos\\AsyncAssessment\\DataflowPipelineEx\\input.txt";
        string outputFile = "C:\\Users\\user\\source\\repos\\AsyncAssessment\\DataflowPipelineEx\\output.txt";

        File.WriteAllText(outputFile, "");

        BufferBlock<string>? readBlock = new BufferBlock<string>();
        var transformBlock = new TransformBlock<string, string>(line =>
        {
            return line.ToUpperInvariant();
        },
        new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 4 });

        var writeBlock = new ActionBlock<string>(async line =>
        {
            using var sw = new StreamWriter(outputFile, append: true);
            await sw.WriteLineAsync(line);
        });

        readBlock.LinkTo(transformBlock, new DataflowLinkOptions { PropagateCompletion = true });
        transformBlock.LinkTo(writeBlock, new DataflowLinkOptions { PropagateCompletion = true });

        foreach (var line in File.ReadLines(inputFile))
        {
            await readBlock.SendAsync(line);
        }
        readBlock.Complete();

        await writeBlock.Completion;

        Console.WriteLine("Processing complete. Check output.txt for results.");
    }
}
