namespace PsychologyApp.Application.Conversation.Companion;

/// <summary>
/// The model offered for download. Sizes and SHA-256 come from the Hugging Face file listing; the installer refuses anything that does not match.
/// Swap this manifest to change the model: bump <see cref="LocalModelManifest.Id"/> so existing installs are treated as outdated.
/// </summary>
public static class LocalModelCatalog
{
    private const string Repo = "https://huggingface.co/Arm/gemma-3-1b-instruct-onnx-genai-int4-emb-int8/resolve/main/";

    public static LocalModelManifest Default { get; } = new(
        "gemma-3-1b-it-onnx-int4",
        "Gemma 3 1B (instruct, int4)",
        "https://ai.google.dev/gemma/terms",
        [
            new("genai_config.json", Repo + "genai_config.json", 1249),
            new("chat_template.jinja", Repo + "chat_template.jinja", 1532),
            new("special_tokens_map.json", Repo + "special_tokens_map.json", 662),
            new("tokenizer_config.json", Repo + "tokenizer_config.json", 1155361),
            new("tokenizer.json", Repo + "tokenizer.json", 33384568, "4667f2089529e8e7657cfb6d1c19910ae71ff5f28aa7ab2ff2763330affad795"),
            new("model.onnx", Repo + "model.onnx", 334842, "1be3eb5396e36b3fb65ce4d2cc9762994c2df93b002594d4f8f0eb56cbaddd15"),
            new("model.onnx.data", Repo + "model.onnx.data", 865140736, "049bdd9071704cd7e3400002ca0339bf07e52c857555943e15437a54453df4cc")
        ],
        // Evaluated on 21 realistic messages (see docs/local-llm.md): English replies are usable, Russian replies from a 1B model are
        // often ungrammatical or invented. Add "ru" only after a Russian-capable model passes tools/PsychologyApp.LlmEval and a native-speaker review.
        Languages: ["en"],
        // The 1B weights need roughly 1.5 GB resident; keep generous headroom for the rest of the app.
        MinTotalMemoryBytes: 3L * 1024 * 1024 * 1024);
}
