using System.Text.Json.Serialization;
using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Features.RunTechniqueSession;

namespace PsychologyApp.Presentation.Features.RunTechniqueSession.Serialization;

[JsonSourceGenerationOptions(PropertyNameCaseInsensitive = true)]
[JsonSerializable(typeof(EntryDraft))]
[JsonSerializable(typeof(PaperListDraft))]
[JsonSerializable(typeof(PolarityListDraft))]
public partial class SessionDraftJsonSerializerContext : JsonSerializerContext;
