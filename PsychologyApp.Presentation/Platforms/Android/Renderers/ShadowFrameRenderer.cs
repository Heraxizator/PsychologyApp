using Android.Content;
using Microsoft.Maui.Controls.Handlers.Compatibility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsychologyApp.Presentation.Platforms.Android.Renderers;

public class ShadowFrameRenderer : FrameRenderer
{
    public ShadowFrameRenderer(Context context) : base(context)
    {
        Radius = 20;
        Elevation = 20;
    }
}
