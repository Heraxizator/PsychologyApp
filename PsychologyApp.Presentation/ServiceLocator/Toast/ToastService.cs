using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Core.Views;
using PsychologyApp.Presentation.Base.ServiceLocator;

namespace PsychologyApp.Presentation.Base.ServiceLocator;

public class ToastService : IToastService
{
    public void LongToast(string message)
    {
        IToast toast = Toast.Make(message, ToastDuration.Long);

        toast.Show();
    }

    public void ShortToast(string message)
    {
        IToast toast = Toast.Make(message, ToastDuration.Short);

        toast.Show();
    }
}
