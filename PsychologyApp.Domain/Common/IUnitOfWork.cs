﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsychologyApp.Domain.Common;

public interface IUnitOfWork
{
    Task<bool> Commit();
    Task Rollback();
}
