﻿using PsychologyApp.Presentation.ViewModels;

namespace PsychologyApp.Presentation.Modules.Practic.Techniques.Resize;

public class ResizeViewModel : BaseViewModel
{
    public ResizeViewModel()
    {

    }

    public ResizeViewModel(INavigation navigation)
    {
        ModuleName = "Практик";
        PageName = "Уменьши Это";

        Info = "Эта практика работает, потому что человеку свойственно терять интерес к предмету, который он оставляет далеко позади себя, когда тот теряет очертания и уменьшается, а расстояние между ним и наблюдателем быстро увеличивается. Не зря же говорят - с глаз долой, из сердца вон.";
        Finish = new Command(ToFinish);
        Theory = new Command(ToTheory);
        Navigation = navigation;
    }
}