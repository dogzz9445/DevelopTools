﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDT.Core.WidgetSystems.Services;

public interface ISecretService
{
    public Task<string> GetSecretAsync();
}
