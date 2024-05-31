using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDT.Core.WidgetSystems.Bases;

public interface IDataSourceCoreSettings
{
    string Name { get; }
}

public class DataSourceCoreSettings : IDataSourceCoreSettings
{
    public string Name { get; set; }
}

public interface IDataSourceOption : IEntity
{
    string Type { get; }
    IDataSourceCoreSettings CoreSettings { get; }

}

public class DataSourceOption : IDataSourceOption
{
    public Guid Id { get; set; }
    public string Type { get; set; }
    public IDataSourceCoreSettings CoreSettings { get; set; }
}
