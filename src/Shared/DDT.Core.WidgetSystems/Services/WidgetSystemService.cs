using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace DDT.Core.WidgetSystems.Services;

public interface IWidgetSystemService
{

}

public class WidgetSystemService : IWidgetSystemService
{

    public WidgetSystemService()
    {
    }

    private IConfiguration BuildConfiguration(string path)
    {
        var builder = new ConfigurationBuilder()
            .AddJsonFile(path);

        return builder.Build();
    }
}
