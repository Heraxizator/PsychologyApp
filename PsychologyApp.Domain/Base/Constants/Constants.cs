using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsychologyApp.Domain.Base.Constants;

public class Constants
{
    /// <summary>Fallback only; prefer <see cref="PsychologyApp.Application.Configuration.AppSettings.ReviewEmailAddress"/> from appsettings.</summary>
    public const string ReviewEmailAddress = "";

    public const string DonateUrl = "https://yoomoney.ru/fundraise/17UP5E1QFCU.250123";

    public const string AliceUrl = "https://alice.yandex.ru/";
    public const int SmallBaseTimeout = 5000;
    public const int MiddleBaseTimeout = 10000;
    public const int LargeBaseTimeout = 15000;
}

