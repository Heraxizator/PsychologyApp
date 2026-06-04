namespace PsychologyApp.Infrastructure.Data.Sql;

internal static class EntitySqlMaps
{
    internal static readonly EntitySqlMap Technique = new()
    {
        Table = "Techniques",
        KeyColumn = "TechniqueId",
        InsertSql = """
            INSERT INTO Techniques (Number, Date, Header, Describtion, Subject, Author, Algorithm, Image, IsCompleted)
            VALUES (@Number, @Date, @Header, @Describtion, @Subject, @Author, @Algorithm, @Image, @IsCompleted);
            """,
        UpdateSql = """
            UPDATE Techniques SET
                Number = @Number,
                Date = @Date,
                Header = @Header,
                Describtion = @Describtion,
                Subject = @Subject,
                Author = @Author,
                Algorithm = @Algorithm,
                Image = @Image,
                IsCompleted = @IsCompleted
            WHERE TechniqueId = @TechniqueId;
            """,
        DeleteSql = "DELETE FROM Techniques WHERE TechniqueId = @TechniqueId;",
        SelectAllSql = "SELECT * FROM Techniques;",
        SelectByKeySql = "SELECT * FROM Techniques WHERE TechniqueId = @id;",
    };

    internal static readonly EntitySqlMap Quot = new()
    {
        Table = "Quots",
        KeyColumn = "QuotId",
        InsertSql = """
            INSERT INTO Quots (Title, Text, Theme, IsReaded, IsFavourite)
            VALUES (@Title, @Text, @Theme, @IsReaded, @IsFavourite);
            """,
        UpdateSql = """
            UPDATE Quots SET
                Title = @Title,
                Text = @Text,
                Theme = @Theme,
                IsReaded = @IsReaded,
                IsFavourite = @IsFavourite
            WHERE QuotId = @QuotId;
            """,
        DeleteSql = "DELETE FROM Quots WHERE QuotId = @QuotId;",
        SelectAllSql = "SELECT * FROM Quots;",
        SelectByKeySql = "SELECT * FROM Quots WHERE QuotId = @id;",
    };

    internal static readonly EntitySqlMap Reason = new()
    {
        Table = "Reasons",
        KeyColumn = "ReasonId",
        InsertSql = """
            INSERT INTO Reasons (Title, Subtitle, Solution)
            VALUES (@Title, @Subtitle, @Solution);
            """,
        UpdateSql = """
            UPDATE Reasons SET
                Title = @Title,
                Subtitle = @Subtitle,
                Solution = @Solution
            WHERE ReasonId = @ReasonId;
            """,
        DeleteSql = "DELETE FROM Reasons WHERE ReasonId = @ReasonId;",
        SelectAllSql = "SELECT * FROM Reasons;",
        SelectByKeySql = "SELECT * FROM Reasons WHERE ReasonId = @id;",
    };

    internal static readonly EntitySqlMap Statistic = new()
    {
        Table = "Statistics",
        KeyColumn = "StatisticId",
        InsertSql = """
            INSERT INTO Statistics (ModuleName, PageName, DateTime, SecondsDuration)
            VALUES (@ModuleName, @PageName, @DateTime, @SecondsDuration);
            """,
        UpdateSql = """
            UPDATE Statistics SET
                ModuleName = @ModuleName,
                PageName = @PageName,
                DateTime = @DateTime,
                SecondsDuration = @SecondsDuration
            WHERE StatisticId = @StatisticId;
            """,
        DeleteSql = "DELETE FROM Statistics WHERE StatisticId = @StatisticId;",
        SelectAllSql = "SELECT * FROM Statistics;",
        SelectByKeySql = "SELECT * FROM Statistics WHERE StatisticId = @id;",
    };
}
