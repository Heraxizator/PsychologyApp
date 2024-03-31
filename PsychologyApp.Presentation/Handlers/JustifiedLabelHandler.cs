using Android.OS;
using Android.Text;
using Microsoft.Maui.Handlers;
using MobileHelperMaui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsychologyApp.Presentation.Handlers
{
    public class JustifiedLabelHandler : LabelHandler
    {
        private static readonly IPropertyMapper<JustifiedLabel, JustifiedLabelHandler> PropertyMapper = new PropertyMapper<JustifiedLabel, JustifiedLabelHandler>(Mapper)
        {
            [nameof(JustifiedLabel.JustifyText)] = MapJustificationMode
        };

        public JustifiedLabelHandler() : base(PropertyMapper)
        {
        }

        public static void MapJustificationMode(ILabelHandler handler, ILabel label)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O) // O == API level 26.0
            {
                handler.PlatformView.JustificationMode = JustificationMode.InterWord;
            }
        }
    }
}
