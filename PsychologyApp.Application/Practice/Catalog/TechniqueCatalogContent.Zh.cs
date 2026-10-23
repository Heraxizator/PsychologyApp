using PsychologyApp.Application.Models.Practice;
using PsychologyApp.Domain.Practice;

namespace PsychologyApp.Application.Practice.Catalog;

internal static class TechniqueCatalogContentZh
{
    internal static TheorySection[] Theory(string what, string when, string how, string example) =>
    [
        new("这是什么", what),
        new("什么时候有帮助", when),
        new("练习怎么做", how),
        new("示例", example)
    ];

    /// <summary>Common topics for a "problem" field: a start to tap and then add a word or two, not a full answer.</summary>
    private static readonly string[] ProblemTopics =
        ["和亲近的人吵架", "工作上的事", "和上司的冲突", "学习或考试", "健康", "钱", "孤独", "一次失败", "分手", "和父母的关系", "孩子", "搬家", "未来", "别人对我的批评"];

    private static readonly string[] BackupOptions =
        ["寻求帮助", "等一等再试", "另找办法", "降低对自己的要求", "休息一下", "和亲近的人聊聊", "去看专业人士", "找份兼职", "上一门课", "把任务拆成小步", "推迟决定", "从小处开始"];

    private static readonly string[] HarshThoughts =
        ["“我会把一切都搞砸”", "“他们会评判我”", "“我应付不了”", "“没有人需要我”", "“这是一场灾难”", "“什么都不会成功”", "“我不如别人”", "“总是事事不顺”", "“都是我的错”", "“我有问题”", "“这永远不会结束”", "“我必须完美”"];

    internal static readonly BuiltInTechniqueDefinition Spin = new(
        "练习", "旋转",
        [
            "1. 描述困扰你的那件事",
            "2. 说出主要的感受",
            "3. 给这种感受打分（0 到 10）"
        ],
        "日沃拉德·斯拉文斯基的“旋转”技巧有助于减轻痛苦记忆的情绪负荷。",
        TechniqueUiKind.Entry, "技巧 #1", "26.01.2023", "旋转", "三步减轻情绪负荷", "艰难时刻", "日沃拉德·斯拉文斯基", 5, "Autorenew",
        [
            new TechniqueEntrySeed("事件", "例如：昨天那段让你放不下的谈话", Suggestions: ProblemTopics, Role: EntryFieldRole.Problem),
            new TechniqueEntrySeed("主要感受", "委屈、恐惧、愤怒……", Role: EntryFieldRole.Emotion),
            new TechniqueEntrySeed("感受强度", "", EntryFieldKind.Rating0To10)
        ],
        Theory(
            "一个简单的中和技巧：说出事件、感受和它的强度。很多时候，仅仅觉察就能减轻负荷。",
            "当某个场景或想法反复出现，焦虑或怨恨挥之不去时。",
            "写下事件和感受，然后如实评估强度。如有需要，改天再做一次。",
            "事件：“同事无视了我的想法”。感受：委屈。评分：8 → 之后 5。"),
        null, null);

    internal static readonly BuiltInTechniqueDefinition Comparison = new(
        "练习", "重要性比较",
        [
            "1. 说出问题",
            "2. 过去在这方面什么最重要？",
            "3. 现在什么最重要？",
            "4. 将来什么会重要？",
            "5. 比较三个答案，记下有什么变化"
        ],
        "跨时间比较重要性，有助于把问题放到你整个人生的背景中去看。",
        TechniqueUiKind.Entry, "技巧 #2", "26.01.2023", "重要性比较", "当问题显得太大时", "问题的规模", "NLP", 7, "CompareArrows",
        [
            new TechniqueEntrySeed("问题", "现在什么让你觉得难以忍受？", Suggestions: ProblemTopics, Role: EntryFieldRole.Problem),
            new TechniqueEntrySeed("过去重要的", "例如：学业、感情、事业", Suggestions: ["学业", "家庭", "朋友", "健康", "事业", "爱好", "运动", "爱情", "旅行", "自由", "认可", "家"]),
            new TechniqueEntrySeed("现在重要的", "什么占据了你最多的注意力？", Suggestions: ["工作", "家庭", "健康", "钱", "人际关系", "休息", "孩子", "朋友", "学业", "睡眠", "内心平静", "自我成长"]),
            new TechniqueEntrySeed("将来重要的", "你想要奔向什么？", Suggestions: ["稳定", "家庭", "自由", "健康", "成长", "我爱的人", "属于自己的家", "喜欢的工作", "旅行", "平静", "孩子", "积蓄"]),
            new TechniqueEntrySeed("有什么变化？", "这个问题还主宰着一切吗？", Suggestions: ["问题只是生活的一部分", "重要的东西比想象的多", "这只是暂时的阶段", "感觉没那么尖锐了", "我感觉平静了些", "我看到自己以前也挺过来过", "出现了希望", "暂时还没有变化"])
        ],
        Theory(
            "这个技巧说明，问题的分量会随时间改变，很少会永远处于中心。",
            "当一个困难占满了整个视野，而且感觉会永远持续时。",
            "填写四个字段，然后比较：其他价值常常会让问题变小。",
            "问题：“没有工作”。过去：学业。现在：找工作。将来：稳定。领悟：这是一个阶段，不是整个人生。"),
        null, null);

    internal static readonly BuiltInTechniqueDefinition Polarity = new(
        "练习", "两极",
        [
            "1. 写下这件事中你喜欢的方面",
            "2. 写下你不喜欢的方面",
            "3. 对比每一组，留意其中的平衡"
        ],
        "两极方法帮助你看到事情的两面，减轻内心的紧张。",
        TechniqueUiKind.Polarity, "技巧 #3", "26.01.2023", "两极", "看到事情的两面", "不同的方面", "日沃拉德·斯拉文斯基", 5, "Contrast",
        null,
        Theory(
            "每件事都有吸引人和令人不快的一面。把两面都说出来，可以减少片面的执着。",
            "当你只看到坏的或只看到好的，感觉卡住了时。",
            "针对一个主题，添加“喜欢 / 不喜欢”的组合。比较两张清单，不评判谁对谁错。",
            "工作：喜欢收入，不喜欢作息。家：喜欢舒适，不喜欢邻居的噪音。"),
        "你不喜欢的", "你喜欢的");

    internal static readonly BuiltInTechniqueDefinition Paper = new(
        "练习", "一张纸",
        [
            "1. 把想法写在纸上（或下面的输入框里）",
            "2. 大声读一遍",
            "3. 把纸折起来、撕掉，或象征性地放下它"
        ],
        "把想法写在纸上，能减轻它们的情绪负荷。",
        TechniqueUiKind.Paper, "技巧 #4", "26.01.2023", "一张纸", "当想法反复打转时", "想法", "心理", 3, "Description",
        [new TechniqueEntrySeed("想法", "例如：“我会把演讲搞砸”", Suggestions: HarshThoughts)],
        Theory(
            "写下来能把想法从脑子里拿出来。毁掉这张纸，象征着放下。",
            "当同一句话在脑中转了好几个小时，影响睡眠或专注时。",
            "把想法倒出来，读一遍，然后毁掉这张纸——真的或在想象中。",
            "想法：“他们会觉得我没能力”。写下并烧掉这张纸后，焦虑常常会减轻。"),
        null, null);

    internal static readonly BuiltInTechniqueDefinition Future = new(
        "练习", "50 年后",
        [
            "1. 说出问题",
            "2. 评估 50 年后它的重要性（0 到 10）"
        ],
        "把问题放到更远的未来，能降低它在主观上的重要性。",
        TechniqueUiKind.Entry, "技巧 #5", "30.01.2023", "50 年后", "把问题放到未来去看", "问题的规模", "NLP", 2, "Schedule",
        [
            new TechniqueEntrySeed("问题", "今天什么让你觉得像一场灾难？", Suggestions: ProblemTopics, Role: EntryFieldRole.Problem),
            new TechniqueEntrySeed("50 年后的重要性", "", EntryFieldKind.Rating0To10)
        ],
        Theory(
            "想象一件事放到遥远的未来，常常会让它变得不那么尖锐——你能从更广的视角看待它。",
            "当一次挫折虽然是暂时的，却感觉像世界末日时。",
            "说出问题，并评估五十年后它还有多重要。分数通常会更低。",
            "问题：“开会没叫我”。50 年后：1–2 分（满分 10）。"),
        null, null);

    internal static readonly BuiltInTechniqueDefinition Hack = new(
        "练习", "白色光斑",
        [
            "1. 回想那件困扰你的事",
            "2. 把它想象成一个没有细节的白色光斑",
            "3. 一直保持这个光斑，直到紧张缓解"
        ],
        "去掉记忆中的细节，常常能减轻它的情绪负荷。",
        TechniqueUiKind.Entry, "技巧 #6", "30.01.2023", "白色光斑", "软化痛苦的记忆", "艰难时刻", "NLP", 3, "BlurOn",
        [new TechniqueEntrySeed("事件", "简单说：发生了什么，什么最让你痛", Suggestions: ProblemTopics, Role: EntryFieldRole.Problem)],
        Theory(
            "细节会加剧感受。白色光斑去掉了触发点，让神经系统喘口气。",
            "当过去的鲜明画面带着情绪向你袭来时。",
            "回想那件事，然后用一个单纯的白色光斑替换画面，保持 30–60 秒。",
            "伤害你的人的脸被一个白色光斑取代。呼吸平稳下来，心思回到当下。"),
        null, null);

    internal static readonly BuiltInTechniqueDefinition Experience = new(
        "练习", "经历调整",
        [
            "1. 选择一个要处理的领域",
            "2. 描述你对它的态度",
            "3. 说出相关的信念",
            "4. 想到过去的经历时，态度会怎样变化？",
            "5. 描述一次负面经历",
            "6. 给这次负面经历打分（−10…10）",
            "7. 什么样的积极经历可以替代它？",
            "8. 有了那样的经历，你的态度会怎样变化？",
            "9. 给新的经历打分（−10…10）"
        ],
        "EMO 帮助你用更有支持力的经历替代限制你的经历。",
        TechniqueUiKind.Entry, "技巧 #7", "08.02.2023", "经历调整", "改写限制你的经历", "艰难时刻", "菲利普·斯拉文斯基", 15, "Psychology",
        [
            new TechniqueEntrySeed("领域", "人际关系、工作、健康……", Suggestions: ["人际关系", "工作", "健康", "钱", "公开演讲", "家庭", "学业", "外貌", "友谊", "创造力", "运动", "事业"]),
            new TechniqueEntrySeed("对这个领域的态度", "你现在对它有什么感觉？", Suggestions: ["焦虑", "自我怀疑", "烦躁", "冷漠", "兴趣", "疲惫", "恐惧", "羞耻", "希望", "愤怒", "困惑", "平静"]),
            new TechniqueEntrySeed("相关的信念", "例如：“我不会成功的”", Suggestions: ["“我不会成功的”", "“我不够好”", "“他们不会接受我”", "“什么都不会改变”", "“我必须完美”", "“我运气不好”", "“我不配”", "“别人都比我好”", "“不允许犯错”", "“我年纪太大了”"]),
            new TechniqueEntrySeed("态度与过去的经历", "回想过去时，有什么变化？", Suggestions: ["变得更沉重", "变得更平静", "没有变化", "愤怒涌上来", "悲伤涌上来", "恐惧涌上来", "我想起自己挺过来了", "我想回避"]),
            new TechniqueEntrySeed("负面经历", "一件具体的事或一种模式"),
            new TechniqueEntrySeed("负面经历评分", "", EntryFieldKind.RatingNeg10To10),
            new TechniqueEntrySeed("积极的替代", "可能有什么支持性的经历？", Suggestions: ["我成功的一次", "我得到支持的一次", "我独自应对过来的一次", "我被表扬的一次", "我帮助别人的一次", "我学会新东西的一次", "我没有放弃的一次", "我被如实接纳的一次"]),
            new TechniqueEntrySeed("有了积极经历后的态度", "你对这个领域的看法会怎样变化？", Suggestions: ["出现希望", "更平静", "我想再试一次", "变得有趣", "出现信心", "变得更轻松", "我感到有力量", "还是很难相信"]),
            new TechniqueEntrySeed("新经历评分", "", EntryFieldKind.RatingNeg10To10)
        ],
        Theory(
            "信念常常建立在一次鲜明的负面经历上。有意识地替代它，可以改变你的态度。",
            "当“一直都是这样”，改变似乎不可能时。",
            "按顺序填写各项。比较两个评分——争取第二个更高。",
            "领域：公开演讲。负面：考试失利（−8）。替代：一次成功的发言（+6）。"),
        null, null);

    internal static readonly BuiltInTechniqueDefinition Copied = new(
        "练习", "重复说",
        [
            "1. 写下困扰你的那句话",
            "2. 大声重复，就像第一次听到一样",
            "3. 一直重复，直到情绪缓和"
        ],
        "“像第一次那样”重复描述问题的句子，可以降低情绪强度。",
        TechniqueUiKind.Copied, "技巧 #8", "21.01.2025", "重复说", "像第一次那样听这个问题", "问题的规模", "NLP", 2, "Repeat",
        [new TechniqueEntrySeed("句子", "我害怕失去工作", Suggestions: HarshThoughts)],
        Theory(
            "一句熟悉的话，慢慢地、平淡地、不带戏剧性地说出来，就会失去分量。",
            "当一个想法在脑中听起来像判决时。",
            "写下这句话，然后停顿着重复 5–10 遍，留意身体的感觉。",
            "“没有人需要我”——重复七遍后，常常会听起来有些荒谬，也轻了一些。"),
        null, null);

    internal static readonly BuiltInTechniqueDefinition Extend = new(
        "练习", "备用计划",
        [
            "1. 说出问题",
            "2. 写下三个现实的备选方案",
            "3. 留意有退路时焦虑如何变化"
        ],
        "备选方案能降低对单一结果的过度重视。",
        TechniqueUiKind.Entry, "技巧 #9", "21.01.2025", "备用计划", "用备用计划降低压力", "问题的规模", "NLP", 3, "Backup",
        [
            new TechniqueEntrySeed("问题", "如果主要计划失败了呢？", Suggestions: ProblemTopics, Role: EntryFieldRole.Problem),
            new TechniqueEntrySeed("备选方案 1", "例如：接一个自由职业项目", Suggestions: BackupOptions),
            new TechniqueEntrySeed("备选方案 2", "例如：学习相关岗位的技能", Suggestions: BackupOptions),
            new TechniqueEntrySeed("备选方案 3", "例如：暂时缩减开支", Suggestions: BackupOptions)
        ],
        Theory(
            "当事情显得非成即败时，头脑会紧绷。备选方案能找回选择的感觉。",
            "在面试、考试或失败会显得灾难性的事情之前。",
            "说出问题和三个可行的备用计划——哪怕很普通。",
            "问题：“拿不到这份工作”。备用计划：实习、找人推荐、去别的城市。"),
        null, null);

    internal static readonly BuiltInTechniqueDefinition Resize = new(
        "练习", "缩小它",
        [
            "1. 描述困扰你的事",
            "2. 把它想象成你面前的一个物体",
            "3. 把它缩小并推远，直到不再在意"
        ],
        "在想象中缩小并推远问题，能降低你对它的情绪关注。",
        TechniqueUiKind.Entry, "技巧 #10", "22.01.2025", "缩小它", "让问题变小、变远", "问题的规模", "NLP", 2, "ZoomOutMap",
        [new TechniqueEntrySeed("困扰你的事", "例如：让你睡不着的冲突", Suggestions: ProblemTopics, Role: EntryFieldRole.Problem)],
        Theory(
            "画面的大小和距离会影响它的分量。一个又小又远的东西，很少会牵动你的神经。",
            "当问题显得巨大，就在眼前时。",
            "想象一个画面，慢慢缩小它，把它推到地平线，持续 10–20 秒。",
            "一场争吵变成一个小点。呼吸放松，思绪清晰。"),
        null, null);

    internal static readonly BuiltInTechniqueDefinition Check = new(
        "练习", "检验一下",
        [
            "1. 描述让你反复纠结的事",
            "2. 有意停止去想它 1–2 分钟",
            "3. 留意身体和心情有什么变化"
        ],
        "过多的关注会放大重要性。短暂的停顿可以检验这个想法控制你的程度。",
        TechniqueUiKind.Entry, "技巧 #11", "22.01.2025", "检验一下", "看看放下会怎样", "问题的规模", "NLP", 2, "FactCheck",
        [new TechniqueEntrySeed("让你反复纠结的事", "哪个想法不断回来？", Suggestions: ProblemTopics, Role: EntryFieldRole.Problem)],
        Theory(
            "如果一个想法关乎生存，放下它会让人觉得危险。这个实验通常会表明，什么灾难都不会发生。",
            "当你停不下来反复想同一件事时。",
            "说出这个想法，然后温和地把注意力转到呼吸或周围的声音上。",
            "想法：“万一我病了怎么办”。停顿之后，焦虑常常从 7 降到 4。"),
        null, null);

    internal static readonly BuiltInTechniqueDefinition Observer = new(
        "练习", "观察者视角",
        [
            "1. 描述困扰你的情境",
            "2. 用第一人称写下感受",
            "3. 以中立观察者的身份看这个情境",
            "4. 换个视角后评估紧张程度"
        ],
        "转换感知视角，有助于从情绪高峰中退后一步，更全面地看待情境。",
        TechniqueUiKind.Entry, "技巧 #12", "12.06.2026", "观察者视角", "从情绪中退后一步，从外面看情境", "艰难时刻", "NLP", 4, "Visibility",
        [
            new TechniqueEntrySeed("情境", "发生了什么，或者你在脑中反复回放什么？", Suggestions: ProblemTopics, Role: EntryFieldRole.Problem),
            new TechniqueEntrySeed("第一人称的感受", "身处其中时，你有什么感受？", Role: EntryFieldRole.Emotion),
            new TechniqueEntrySeed("观察者看到的", "一个平静的旁观者会注意到什么？", Suggestions: ["两个人在争论", "一个疲惫、难过的人", "一个普通的工作场景", "每个人都在维护自己的立场", "一周后就会忘了", "人们互相误解了", "一个非常努力的人", "不愉快，但并不危险", "每个人都有自己的看法", "这个人需要休息"]),
            new TechniqueEntrySeed("现在的紧张程度", "", EntryFieldKind.Rating0To10)
        ],
        Theory(
            "身处一件事中时，情绪显得无法避免。观察者视角能拉开距离，又不否认你的体验。",
            "当情境让你招架不住、理性的道理也帮不上忙时。",
            "描述情境和感受，然后用第三人称重写——中立地，不加评判。",
            "争吵：身处其中——“他们不尊重我”。观察者：“两个人在大声说话”。紧张程度：8 → 5。"),
        null, null);

    internal static readonly BuiltInTechniqueDefinition Anchor = new(
        "练习", "资源锚",
        [
            "1. 回想一个你感到自信或平静的时刻",
            "2. 描述身体的感觉",
            "3. 选择一个简单的锚定手势",
            "4. 评估资源状态的强度"
        ],
        "锚把一个身体手势和资源状态连接起来，让你能更快地找回支撑。",
        TechniqueUiKind.Entry, "技巧 #13", "12.06.2026", "资源锚", "通过手势和回忆找回支撑", "艰难时刻", "NLP", 4, "SelfImprovement",
        [
            new TechniqueEntrySeed("资源回忆", "你什么时候感到强大、平静或快乐？", Suggestions: ["一次成功的发言", "一次海边度假", "亲近的人的拥抱", "完成了一件难事", "在大自然中散步", "通过了考试", "孩子出生", "赢得比赛", "和朋友度过的夜晚", "上司的表扬", "新工作的第一天", "一个不赶时间的早晨"]),
            new TechniqueEntrySeed("身体感觉", "你在哪里感觉到？温暖、轻盈、踏实……", Suggestions: ["胸口温暖", "轻盈", "肩膀舒展", "均匀的呼吸", "双腿稳稳的", "一个微笑", "平静的心", "挺直的背", "双手有力量", "放松的腹部"]),
            new TechniqueEntrySeed("手势或锚", "例如：握拳、触碰手腕", Suggestions: ["握拳", "触碰手腕", "拇指和食指相碰", "手放在心口", "深呼吸", "捏一下耳垂", "舒展肩膀", "抚摸手掌"]),
            new TechniqueEntrySeed("状态强度", "", EntryFieldKind.Rating0To10)
        ],
        Theory(
            "如果你多次重复“回忆 + 手势”这一组合，大脑会很快把手势和状态联系起来。",
            "在有压力的事情之前，或内心缺少支撑时。",
            "回想那个充满资源的时刻，加强身体感觉，再用一个简单的手势把它固定下来。",
            "资源：一次成功的发言。身体：胸口温暖。锚：触碰拇指。强度：8。"),
        null, null);

    internal static readonly BuiltInTechniqueDefinition Grounding = new(
        "练习", "着陆练习 5-4-3-2-1",
        [
            "1. 说出你能看到的 5 样东西",
            "2. 说出你身体能感觉到的 4 样东西",
            "3. 说出周围的 3 种声音",
            "4. 说出 2 种气味",
            "5. 说出 1 种味道或舒服的感觉"
        ],
        "感官着陆能把注意力从焦虑的想法转到当下。",
        TechniqueUiKind.Entry, "技巧 #14", "12.06.2026", "着陆练习 5-4-3-2-1", "通过感官回到当下", "艰难时刻", "心理", 3, "Nature",
        [
            new TechniqueEntrySeed("我看到的 5 样东西", "例如：窗户、杯子、书、台灯、手", Suggestions: ["窗户", "桌子", "手机", "电脑", "笔记本电脑", "显示器", "键盘", "杯子", "台灯", "手", "门", "书", "椅子", "墙", "画", "植物", "钟", "衣柜", "天空", "汽车"]),
            new TechniqueEntrySeed("我身体感觉到的 4 样东西", "例如：椅子、地板、衣服、空气的温度", Suggestions: ["椅子", "脚下的地板", "衣服", "凉意", "温暖", "手里的手机", "椅背", "皮肤上的布料", "胸口的呼吸", "风", "鼠标", "钥匙", "枕头", "手腕上的表"]),
            new TechniqueEntrySeed("3 种声音", "例如：街道、钟表、你的呼吸", Suggestions: ["街道", "钟", "呼吸", "说话声", "冰箱", "风", "安静", "电脑", "汽车声", "鸟叫", "音乐", "空调", "脚步声", "雨声"]),
            new TechniqueEntrySeed("2 种气味", "例如：咖啡、新鲜空气", Suggestions: ["咖啡", "新鲜空气", "香皂", "食物", "香水", "几乎没有", "茶", "面包", "书", "花", "护手霜", "木头"]),
            new TechniqueEntrySeed("1 种味道或舒服的感觉", "例如：口中的水、柔软的布料", Suggestions: ["水", "茶", "柔软的布料", "温暖的手掌", "口气清新", "咖啡", "薄荷口香糖", "巧克力", "一个苹果", "温暖的毯子"])
        ],
        Theory(
            "焦虑常常把你拉到过去或未来。通过感官逐一数出来，能把你带回此时此地。",
            "在惊恐、强烈焦虑、头晕或感觉与现实脱离时。",
            "慢慢地完成每一步，说出具体的感觉。每一项之间保持均匀呼吸。",
            "看到 5 样 → 感觉到 4 样 → 听到 3 种 → 呼吸平稳，思绪放慢。"),
        null, null);

    internal static readonly BuiltInTechniqueDefinition Breathing = new(
        "练习", "方块呼吸",
        [
            "1. 练习前评估紧张程度",
            "2. 吸气数 4 拍",
            "3. 屏息数 4 拍",
            "4. 呼气数 4 拍",
            "5. 屏息数 4 拍——重复 4 轮，然后评估紧张程度"
        ],
        "有节奏的呼吸能让神经系统平静下来，减轻身体的紧张。",
        TechniqueUiKind.Entry, "技巧 #15", "04.07.2026", "方块呼吸", "用呼吸节奏让身体平静", "艰难时刻", "心理", 3, "Air",
        [
            new TechniqueEntrySeed("练习前的紧张程度", "", EntryFieldKind.Rating0To10, Role: EntryFieldRole.IntensityBefore),
            new TechniqueEntrySeed("备注", "练习时，你在身体或呼吸上注意到了什么？", Suggestions: ["肩膀放松下来了", "呼吸变慢了", "身体暖和了", "很难保持停顿", "被想法分了心", "我平静了一些", "有点头晕", "心跳慢下来了", "想打哈欠", "没什么特别的"]),
            new TechniqueEntrySeed("练习后的紧张程度", "", EntryFieldKind.Rating0To10, Role: EntryFieldRole.IntensityAfter)
        ],
        Theory(
            "缓慢而有节奏的呼吸能降低交感神经的活动，帮助身体退出“战斗或逃跑”模式。",
            "在焦虑、惊恐、心跳加速、失眠或感到不堪重负时。",
            "舒服地坐好，用鼻子呼吸：吸 4 拍，屏 4 拍，呼 4 拍，屏 4 拍。重复 4 轮。",
            "焦虑 7/10 → 4 轮后 4/10。肩膀放松，脉搏变慢。"),
        null, null);

    internal static readonly BuiltInTechniqueDefinition SmallStep = new(
        "练习", "一小步",
        [
            "1. 描述什么让你觉得沉重，或阻碍你开始",
            "2. 选出今天最小、最现实的一步",
            "3. 写下你什么时候去做",
            "4. 写下这一步后，评估你的精力"
        ],
        "行为激活：一个小行动能找回前进和掌控的感觉。",
        TechniqueUiKind.Entry, "技巧 #16", "04.07.2026", "一小步", "用最小的行动找回动力", "艰难时刻", "CBT", 5, "DirectionsWalk",
        [
            new TechniqueEntrySeed("让你觉得沉重的事", "现在什么显得困难、无聊或不可能？", Suggestions: ["疲惫", "没精力", "要做的事太多", "无聊", "不敢开始", "什么都好像不重要", "没睡好", "没心情", "孤独", "拖延", "头疼", "不知道从哪里开始"], Role: EntryFieldRole.Problem),
            new TechniqueEntrySeed("今天最小的一步", "例如：起床、洗脸、发一条消息", Suggestions: ["起床", "洗把脸", "喝点水", "发一条消息", "出门走 5 分钟", "收拾一样东西", "打开窗户", "伸展 2 分钟", "洗个澡", "吃点东西", "给亲近的人打电话", "打开需要的文件", "写一份今天的计划", "洗一个杯子"]),
            new TechniqueEntrySeed("什么时候做", "一天中具体的时间或时刻", Suggestions: ["现在就做", "15 分钟后", "一小时后", "早上", "早饭后", "午饭后", "晚上", "睡前", "明天早上", "周末"]),
            new TechniqueEntrySeed("之后的精力", "", EntryFieldKind.Rating0To10)
        ],
        Theory(
            "在提不起劲时，大脑会等有了动力再行动。而小小的一步是通过行动产生动力，而不是反过来。",
            "当你没有精力、觉得一切都没意义，或者很难开始一天时。",
            "选一个 2–5 分钟就能完成的小步骤，不需要完美的计划。写下它和时间——这已经是在前进了。",
            "打扫房子 → 10:00 把一个杯子拿到厨房。精力：2 → 4。"),
        null, null);

    internal static readonly BuiltInTechniqueDefinition ThoughtRecord = new(
        "练习", "想法记录",
        [
            "1. 描述情境",
            "2. 写下自动想法",
            "3. 给情绪打分",
            "4. 写出一个更平衡的替代想法",
            "5. 再次给情绪打分"
        ],
        "结构化的认知行为疗法记录，帮助你看到情境、想法和情绪之间的联系。",
        TechniqueUiKind.Entry, "技巧 #17", "04.07.2026", "想法记录", "拆解一个想法，减轻情绪负荷", "艰难时刻", "CBT", 7, "EditNote",
        [
            new TechniqueEntrySeed("情境", "发生了什么？在哪里、什么时候？", Suggestions: ProblemTopics, Role: EntryFieldRole.Problem),
            new TechniqueEntrySeed("自动想法", "那一刻你在想什么？", Suggestions: HarshThoughts),
            new TechniqueEntrySeed("情绪", "", EntryFieldKind.Rating0To10, Role: EntryFieldRole.IntensityBefore),
            new TechniqueEntrySeed("替代想法", "还有什么可能是真的？更平衡的看法", Role: EntryFieldRole.Reframe),
            new TechniqueEntrySeed("现在的情绪", "", EntryFieldKind.Rating0To10, Role: EntryFieldRole.IntensityAfter)
        ],
        Theory(
            "自动想法听起来常常像事实，但其实是解读。写下来，就能看清并检验它们。",
            "在冲突、批评、失败之后，或某个想法反复出现时。",
            "描述情境和想法，给情绪打分，然后找一个替代想法——不是强行乐观，而是更准确的看法。",
            "情境：消息没有回复。想法：没有人需要我。替代：对方在忙。情绪：8 → 5。"),
        null, null);

    internal static readonly BuiltInTechniqueDefinition SelfCompassion = new(
        "练习", "对自己说温和的话",
        [
            "1. 写下你在为什么批评自己",
            "2. 想象一位好朋友这样说——你会怎么对他说？",
            "3. 对自己说同样支持的话",
            "4. 评估温暖或释然的程度"
        ],
        "自我关怀能减少严厉的自我批评，帮助你在犯错后恢复。",
        TechniqueUiKind.Entry, "技巧 #18", "04.07.2026", "对自己说温和的话", "用支持缓和自我批评", "艰难时刻", "CBT", 5, "Favorite",
        [
            new TechniqueEntrySeed("你批评自己的地方", "你对自己说什么，或者在责怪自己什么？", Suggestions: ["工作上出了错", "没做完", "对亲近的人发了脾气", "拿自己和别人比", "又在拖延", "我的外貌", "我的体重", "说错了话", "不是一个足够好的父母", "挣得太少", "没有遵守承诺", "太敏感"]),
            new TechniqueEntrySeed("你会对朋友说的话", "在这种情况下，你会怎样支持亲近的人？", Role: EntryFieldRole.Kindness),
            new TechniqueEntrySeed("对自己说的话", "同样的善意，送给你自己", Role: EntryFieldRole.SelfKindness),
            new TechniqueEntrySeed("温暖 / 释然", "", EntryFieldKind.Rating0To10)
        ],
        Theory(
            "自我批评很少能激励人——它更常助长羞耻和停滞。自我关怀并不免除责任，而是给予支持。",
            "在犯错之后、和别人比较时，或觉得自己不够好时。",
            "写下批评，然后像回应朋友那样回应自己——带着尊重，不轻描淡写。",
            "批评：我又把一切搞砸了。对朋友：你努力过了，这可以弥补。对自己：我可以从中学到东西。温暖：3 → 6。"),
        null, null);

    internal static readonly BuiltInTechniqueDefinition BodyScan = new(
        "练习", "它在身体的哪里？",
        [
            "1. 评估你现在的紧张程度",
            "2. 把注意力从头到脚慢慢扫过身体",
            "3. 找到感觉最强烈的地方",
            "4. 描述它：是什么样的，大还是小，暖还是冷",
            "5. 说出伴随它的情绪",
            "6. 慢慢呼气 5 次，就像每次呼气都穿过那个地方，然后再次评估紧张程度"
        ],
        "身体扫描：觉察情绪住在哪里，说出它的名字，让身体放松下来，不与感觉对抗。",
        TechniqueUiKind.Entry, "技巧 #19", "16.10.2026", "它在身体的哪里？", "觉察情绪在身体的哪里，并缓解紧张", "艰难时刻", "正念", 4, "Accessibility",
        [
            new TechniqueEntrySeed("现在的紧张程度", "", EntryFieldKind.Rating0To10, Role: EntryFieldRole.IntensityBefore),
            new TechniqueEntrySeed("你在身体的哪里感觉到它？", "例如：胸口、喉咙、肩膀", Suggestions: ["头", "额头", "眼睛", "下巴", "喉咙", "脖子", "肩膀", "胸口", "心脏", "腹部", "背", "腰", "手臂", "双手", "腿", "全身"]),
            new TechniqueEntrySeed("是什么样的感觉？", "例如：紧绷、沉重、哽住", Suggestions: ["紧绷", "沉重", "哽住", "压迫感", "紧张", "发热", "发冷", "颤抖", "刺麻", "跳痛", "空虚", "麻木", "疼痛", "难以描述"]),
            new TechniqueEntrySeed("伴随的是什么情绪？", "例如：焦虑、委屈、疲惫", Role: EntryFieldRole.Emotion),
            new TechniqueEntrySeed("练习后的紧张程度", "", EntryFieldKind.Rating0To10, Role: EntryFieldRole.IntensityAfter)
        ],
        Theory(
            "情绪会在身体里回响：焦虑像胸口发紧，委屈像喉咙哽住，疲惫像肩膀沉重。当我们觉察到感觉并说出情绪时，紧张通常会缓解。这正是正念课程中身体扫描的核心。",
            "当你感到焦虑或沉重，却说不清到底是什么感受时；当一次艰难的谈话或一天之后身体紧绷时。这个练习不解释疾病，也不能代替医生：如果某种感觉是新出现的、强烈的或一直不消退，请去看医生。",
            "舒服地坐好，把注意力从头顶慢慢移到双脚。在感觉最明显的地方停下。不要改变它，也不要对抗它：描述它，并说出情绪。然后慢慢呼几口气，就像每次呼气都穿过那个地方。",
            "紧张程度 6。位置：胸口。感觉：紧绷，像攥着的拳头。情绪：谈话前的焦虑。呼气 5 次后，紧绷感柔和了一些，紧张程度 4。"),
        null, null);

    internal static readonly BuiltInTechniqueDefinition Flashback = new(
        "练习", "被一段记忆淹没了",
        [
            "1. 评估它有多强烈",
            "2. 告诉自己：“这是回忆。那是过去，现在是今天”",
            "3. 说出你在哪里，现在是一天中的什么时候",
            "4. 感受脚下的支撑，握紧拳头再松开",
            "5. 说出你看到的三样东西",
            "6. 慢慢呼气五次，用冷水，然后再次评估"
        ],
        "闪回时的帮助：通过时间、地点、身体和感官，从回忆回到当下。",
        TechniqueUiKind.Entry, "技巧 #20", "16.10.2026", "被一段记忆淹没了", "当痛苦的记忆占据你时，回到当下", "艰难时刻", "创伤", 3, "MyLocation",
        [
            new TechniqueEntrySeed("有多强烈", "", EntryFieldKind.Rating0To10, Role: EntryFieldRole.IntensityBefore),
            new TechniqueEntrySeed("你在哪里，现在是什么时候", "例如：在家，晚上", Suggestions: ["在家", "在工作", "在外面", "在交通工具上", "在别人家", "早上", "下午", "晚上", "夜里"]),
            new TechniqueEntrySeed("你看到的三样东西", "例如：窗户、杯子、台灯", Suggestions: ["窗户", "桌子", "手机", "杯子", "台灯", "门", "椅子", "墙", "画", "植物", "钟", "天空"]),
            new TechniqueEntrySeed("现在有多强烈", "", EntryFieldKind.Rating0To10, Role: EntryFieldRole.IntensityAfter)
        ],
        Theory(
            "闪回是一种创伤记忆，经历时就像事情又在发生一样：带着同样的情绪和身体反应。大脑把“过去”和“现在”混在了一起。时间、地点、身体的支撑以及你看到和听到的东西，能帮助你回到当下。",
            "当痛苦事件的记忆占据你，画面闪现，感觉“我又回到了那里”时。频繁的闪回是创伤后反应的常见迹象，在创伤专业人士的治疗下效果很好。",
            "不要试图处理这段记忆：现在唯一的任务就是回来。告诉自己你在哪里、今天是几号，双脚用力踩地，说出你看到的东西，慢慢呼气，借助冷的东西。",
            "强度 8。“那是过去；现在是 2026 年，我在家，是晚上。”双脚踩在地上；我看到窗户、杯子、台灯。冷水。现在 4。"),
        null, null);

    internal static readonly BuiltInTechniqueDefinition Nightmare = new(
        "练习", "改写噩梦",
        [
            "1. 选一个反复出现的噩梦，开始时不要选最难的",
            "2. 简短描述，不写令人难受的细节",
            "3. 决定要改变什么：结局、地点、你自己、身边的人",
            "4. 完整写下新版本的梦，直到平静的结局",
            "5. 每天花 10 到 20 分钟，详细想象新版本",
            "6. 一两周后，看看噩梦有什么变化"
        ],
        "意象排练：在清醒时改写反复出现的噩梦，大脑会开始梦到新版本。",
        TechniqueUiKind.Entry, "技巧 #21", "16.10.2026", "改写噩梦", "让反复出现的噩梦变少、变弱", "睡眠", "创伤", 15, "NightsStay",
        [
            new TechniqueEntrySeed("这个噩梦有多难受", "", EntryFieldKind.Rating0To10, Role: EntryFieldRole.IntensityBefore),
            new TechniqueEntrySeed("噩梦的内容，简单说", "一两句话，不写令人难受的细节", Suggestions: ["有人在追我", "我逃不掉", "我又回到了那个可怕的地方", "我失去了亲近的人", "我喊不出来", "我在坠落", "我被攻击", "我迟到了，赶不上"]),
            new TechniqueEntrySeed("你要改变的", "想改什么都行：这是你的梦", Suggestions: ["结局", "地点", "我变得更强大", "有人来帮忙", "我能说话、能喊", "危险消失了", "一切变得安全", "我意识到这是梦"]),
            new TechniqueEntrySeed("新版本的梦", "从开头到平静的结局，完整描述", Suggestions: ["我意识到这是梦", "出现了保护我的人", "我到了一个安全的地方", "我平静地离开", "一切都结束得很好"]),
            new TechniqueEntrySeed("什么时候排练新版本", "每天 10 到 20 分钟", Suggestions: ["早上", "白天，休息的时候", "晚上，睡前两小时左右", "每天同一时间"]),
            new TechniqueEntrySeed("现在有多难受", "", EntryFieldKind.Rating0To10, Role: EntryFieldRole.IntensityAfter)
        ],
        Theory(
            "创伤或压力后反复出现的噩梦，是靠习惯维持的：大脑一遍遍播放同样的情节。在意象排练疗法中，人在清醒时改写情节，并每天在脑中排练新版本。研究表明，这能让噩梦变少、变弱，睡眠变好。",
            "适用于让你害怕入睡的反复噩梦。如果噩梦直接重现了创伤，哪怕简短描述都会带来强烈恐惧，请和创伤专业人士一起做这个练习。",
            "从一个较轻的噩梦开始。不要详细写出可怕的部分：几句话就够了。随你喜欢去改变梦；新版本不必符合现实。白天排练，不要在临睡前，每天 10 到 20 分钟。",
            "噩梦：有人在追我，我逃不掉。改变：我意识到这是梦，转过身，那只是一个影子；我平静地走回家。我每天下午排练一次。两周后，这个噩梦出现得少了。"),
        null, null);
}
