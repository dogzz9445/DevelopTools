﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDT.Core.WidgetSystems.Services;

public class ModelVersionSecretService : ISecretService
{
    public async Task<string> GetSecretAsync()
    {
        await Task.Yield();

        return this.GetType().Assembly.ManifestModule.ModuleVersionId.ToString();
    }
}
