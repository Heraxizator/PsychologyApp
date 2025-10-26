using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsychologyApp.Domain.Base.Constants;

public class Constants
{
    public const string QuotApiUrl = "https://api.forismatic.com/api/1.0/?method=getQuote&format=json";
    public const string ReviewEmailAdress = "m.a.sukhih@yandex.ru";
    public const int SmallBaseTimeout = 5000;
    public const int MiddleBaseTimeout = 10000;
    public const int LargeBaseTimeout = 15000;
}

