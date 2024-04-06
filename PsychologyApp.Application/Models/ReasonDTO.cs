﻿using PsychologyApp.Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsychologyApp.Application.Models
{
    public class ReasonDTO : BaseDTO
    {
        public string? Title { get; set; }
        public string? Subtitle { get; set; }
        public string? Solution { get; set; }
    }
}
