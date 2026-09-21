using System.Diagnostics;
using System.Text.Json;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using Microsoft.ML.Tokenizers;
using PsychologyApp.Application.Conversation.Companion;

// Usage: dotnet run -c Release --project tools/PsychologyApp.NluEval -- <e5-dir with model_qint8_avx512_vnni.onnx + sentencepiece.bpe.model> <nlu-dataset.json> [--protos 3]
// Compares the lexicon analyzer with nearest-example classification on multilingual-e5-small sentence embeddings.

if (args.Length < 2)
{
    Console.Error.WriteLine("Usage: NluEval <model-dir> <dataset.json> [--protos N]");
    return 1;
}

string modelDir = args[0];
int protoCount = 3;
int at = Array.IndexOf(args, "--protos");
if (at >= 0 && at + 1 < args.Length)
{
    protoCount = int.Parse(args[at + 1]);
}

Item[] items = JsonSerializer.Deserialize<Item[]>(File.ReadAllText(args[1]), new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;

using Embedder embedder = new(modelDir);
Stopwatch sw = Stopwatch.StartNew();
float[][] vectors = items.Select(i => embedder.Embed("query: " + i.Text)).ToArray();
Console.WriteLine($"Embedded {items.Length} texts in {sw.Elapsed.TotalSeconds:F1}s ({sw.Elapsed.TotalMilliseconds / items.Length:F0} ms each)");


// Held-out mode: every labelled example of the dataset is a prototype, the separate file is the (untuned) test set.
string? testFile = args.SkipWhile(a => a != "--test").Skip(1).FirstOrDefault();
if (testFile is not null)
{
    Item[] test = JsonSerializer.Deserialize<Item[]>(File.ReadAllText(testFile), new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
    LexiconSituationAnalyzer lex2 = new();
    int[] protoIdx = Enumerable.Range(0, items.Length).Where(i => items[i].Label != "Unknown").ToArray();
    int total = 0, lexOk = 0, nnOk = 0, hybridOk = 0, unknownAsUnknown = 0, unknownTotal = 0;
    List<string> lexMisses = [];
    List<string> hybridMisses = [];
    foreach (Item t in test)
    {
        float[] v = embedder.Embed("query: " + t.Text);
        (string label, double sim) = protoIdx.Select(p => (items[p].Label, Dot(v, vectors[p]))).MaxBy(x => x.Item2);
        string lexPred = lex2.Analyze(t.Text).Emotion.ToString();
        if (t.Label == "Unknown")
        {
            unknownTotal++;
            unknownAsUnknown += lexPred == "Unknown" ? 1 : 0;
            continue;
        }

        total++;
        lexOk += lexPred == t.Label ? 1 : 0;
        nnOk += label == t.Label ? 1 : 0;
        string hybridPred = lexPred != "Unknown" ? lexPred : label;
        hybridOk += hybridPred == t.Label ? 1 : 0;
        if (lexPred != t.Label)
        {
            lexMisses.Add($"[{t.Label} -> {lexPred}] {t.Text}");
        }

        if (hybridPred != t.Label)
        {
            hybridMisses.Add($"[{t.Label} -> {hybridPred}] {t.Text}");
        }
    }

    Console.WriteLine();
    Console.WriteLine($"HELD-OUT ({total} emotion messages, prototypes: {protoIdx.Length} dev examples)");
    Console.WriteLine($"Lexicon                 : {lexOk}/{total} = {(double)lexOk / total:P0}");
    Console.WriteLine($"Embeddings 1-NN         : {nnOk}/{total} = {(double)nnOk / total:P0}");
    Console.WriteLine($"Hybrid (lexicon, else 1-NN): {hybridOk}/{total} = {(double)hybridOk / total:P0}");
    Console.WriteLine($"Unknown messages kept 'Unknown' by lexicon: {unknownAsUnknown}/{unknownTotal}");
    Console.WriteLine("Lexicon misses:");
    lexMisses.ForEach(m => Console.WriteLine("  " + m));
    Console.WriteLine("Hybrid misses:");
    hybridMisses.ForEach(m => Console.WriteLine("  " + m));
    return 0;
}
// Prototypes: the first N examples of each emotion; everything else is test data.
HashSet<int> protoIndexes = [];
foreach (IGrouping<string, (Item Item, int Index)> group in items.Select((item, index) => (item, index)).Where(x => x.item.Label != "Unknown").GroupBy(x => x.item.Label))
{
    foreach ((Item _, int index) in group.Take(protoCount))
    {
        protoIndexes.Add(index);
    }
}

LexiconSituationAnalyzer lexicon = new();
List<int> emotionTest = Enumerable.Range(0, items.Length).Where(i => !protoIndexes.Contains(i) && items[i].Label != "Unknown").ToList();
List<int> unknownTest = Enumerable.Range(0, items.Length).Where(i => items[i].Label == "Unknown").ToList();

(string Label, double Sim) Nearest(int index)
{
    (string, double) best = ("", double.MinValue);
    foreach (int p in protoIndexes)
    {
        double sim = Dot(vectors[index], vectors[p]);
        if (sim > best.Item2)
        {
            best = (items[p].Label, sim);
        }
    }

    return best;
}

Dictionary<string, float[]> centroids = protoIndexes
    .GroupBy(p => items[p].Label)
    .ToDictionary(g => g.Key, g => Normalize(Average(g.Select(p => vectors[p]).ToList())));

string Centroid(int index) => centroids.MaxBy(c => Dot(vectors[index], c.Value)).Key;

int lex = 0, nn = 0, cen = 0, hybrid = 0;
List<string> nnMisses = [];
foreach (int i in emotionTest)
{
    string lexPred = lexicon.Analyze(items[i].Text).Emotion.ToString();
    (string nnPred, double sim) = Nearest(i);
    lex += lexPred == items[i].Label ? 1 : 0;
    nn += nnPred == items[i].Label ? 1 : 0;
    cen += Centroid(i) == items[i].Label ? 1 : 0;
    hybrid += (lexPred != "Unknown" ? lexPred : nnPred) == items[i].Label ? 1 : 0;
    if (nnPred != items[i].Label)
    {
        nnMisses.Add($"[{items[i].Label} -> {nnPred} {sim:F2}] {items[i].Text}");
    }
}

Console.WriteLine();
Console.WriteLine($"Prototypes per emotion: {protoCount}; test emotion messages: {emotionTest.Count}; unknown messages: {unknownTest.Count}");
Console.WriteLine($"Lexicon                : {lex}/{emotionTest.Count} = {(double)lex / emotionTest.Count:P0}");
Console.WriteLine($"Embeddings 1-NN        : {nn}/{emotionTest.Count} = {(double)nn / emotionTest.Count:P0}");
Console.WriteLine($"Embeddings centroid    : {cen}/{emotionTest.Count} = {(double)cen / emotionTest.Count:P0}");
Console.WriteLine($"Hybrid (lexicon, else 1-NN): {hybrid}/{emotionTest.Count} = {(double)hybrid / emotionTest.Count:P0}");

Console.WriteLine();
Console.WriteLine("Unknown detection by similarity threshold (below it = 'not sure'):");
foreach (double tau in new[] { 0.80, 0.82, 0.84, 0.86, 0.88, 0.90 })
{
    int unknownCaught = unknownTest.Count(i => Nearest(i).Sim < tau);
    int emotionLost = emotionTest.Count(i => Nearest(i).Sim < tau);
    Console.WriteLine($"  tau {tau:F2}: unknown caught {unknownCaught}/{unknownTest.Count}, emotion messages wrongly 'unknown' {emotionLost}/{emotionTest.Count}");
}

Console.WriteLine();
Console.WriteLine("Embedding misses:");
foreach (string miss in nnMisses)
{
    Console.WriteLine("  " + miss);
}

return 0;

static double Dot(float[] a, float[] b)
{
    double sum = 0;
    for (int i = 0; i < a.Length; i++)
    {
        sum += a[i] * b[i];
    }

    return sum;
}

static float[] Average(List<float[]> vectors)
{
    float[] result = new float[vectors[0].Length];
    foreach (float[] v in vectors)
    {
        for (int i = 0; i < v.Length; i++)
        {
            result[i] += v[i] / vectors.Count;
        }
    }

    return result;
}

static float[] Normalize(float[] v)
{
    double norm = Math.Sqrt(v.Sum(x => (double)x * x));
    return v.Select(x => (float)(x / norm)).ToArray();
}

internal sealed record Item(string Lang, string Label, string Text);

/// <summary>multilingual-e5-small: XLM-R tokenizer (SentencePiece with the fairseq id offset), mean pooling, L2 normalisation.</summary>
internal sealed class Embedder : IDisposable
{
    private readonly InferenceSession _session;
    private readonly SentencePieceTokenizer _tokenizer;
    private readonly bool _needsTokenTypes;

    public Embedder(string directory)
    {
        _session = new InferenceSession(Path.Combine(directory, "model_qint8_avx512_vnni.onnx"));
        using FileStream stream = File.OpenRead(Path.Combine(directory, "sentencepiece.bpe.model"));
        _tokenizer = SentencePieceTokenizer.Create(stream, addBeginningOfSentence: false, addEndOfSentence: false);
        _needsTokenTypes = _session.InputMetadata.ContainsKey("token_type_ids");
        Console.WriteLine("Model inputs: " + string.Join(", ", _session.InputMetadata.Keys));
    }

    public float[] Embed(string text)
    {
        // sentencepiece ids -> HF XLM-R ids: <s>=0 <pad>=1 </s>=2 <unk>=3, every other piece shifts by one.
        List<long> ids = [0];
        foreach (int id in _tokenizer.EncodeToIds(text))
        {
            ids.Add(id switch { 0 => 3, 1 => 0, 2 => 2, _ => id + 1 });
        }

        ids.Add(2);
        long[] inputIds = ids.ToArray();
        long[] mask = Enumerable.Repeat(1L, inputIds.Length).ToArray();
        int[] shape = [1, inputIds.Length];

        List<NamedOnnxValue> inputs =
        [
            NamedOnnxValue.CreateFromTensor("input_ids", new DenseTensor<long>(inputIds, shape)),
            NamedOnnxValue.CreateFromTensor("attention_mask", new DenseTensor<long>(mask, shape))
        ];
        if (_needsTokenTypes)
        {
            inputs.Add(NamedOnnxValue.CreateFromTensor("token_type_ids", new DenseTensor<long>(new long[inputIds.Length], shape)));
        }

        using IDisposableReadOnlyCollection<DisposableNamedOnnxValue> results = _session.Run(inputs);
        Tensor<float> hidden = results.First().AsTensor<float>();
        int dim = hidden.Dimensions[2];
        float[] pooled = new float[dim];
        for (int t = 0; t < inputIds.Length; t++)
        {
            for (int d = 0; d < dim; d++)
            {
                pooled[d] += hidden[0, t, d] / inputIds.Length;
            }
        }

        double norm = Math.Sqrt(pooled.Sum(x => (double)x * x));
        return pooled.Select(x => (float)(x / norm)).ToArray();
    }

    public void Dispose() => _session.Dispose();
}
