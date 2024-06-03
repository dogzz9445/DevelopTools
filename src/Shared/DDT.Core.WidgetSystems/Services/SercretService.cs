using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDT.Core.WidgetSystems.Contracts.Services;

namespace DDT.Core.WidgetSystems.Services;

public class SercretService : ISecretService
{
    public Task<string> GetSecretAsync()
    {
        throw new NotImplementedException();
    }
}
