using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDT.Core.WidgetSystems.WPF.Bases;

public static class NameGenerationHelper
{
    public static string GenerateUniqueName(string baseName, List<string> usedNames)
    {
        for (int i = 1; i <= 1000; i++)
        {
            string res = $"{baseName} {i}";
            if (!usedNames.Contains(res))
            {
                return res;
            }
        }
        return $"{baseName} 1000";
    }

    public static string GenerateCopyName(string itemName, List<string> usedNames)
    {
        return GenerateUniqueName($"{itemName} Copy", usedNames);
    }
}
