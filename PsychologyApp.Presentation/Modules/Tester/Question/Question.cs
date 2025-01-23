using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileHelper.ViewModels.TestViewModels;

public class Question
{
    public int Number { get; set; }
    public List<Answer> Answers { get; set; } = [];
}
