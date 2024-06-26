﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDT.Core.WidgetSystems.Contracts.Services;

namespace DDT.Core.WidgetSystems.Services;

public class ConstSecretKeyForDevelopSercretService : ISecretService
{
    public async Task<string> GetSecretAsync()
    {
        await Task.Yield();

        return this.GetType().Assembly.ManifestModule.ModuleVersionId.ToString();
    }
}
