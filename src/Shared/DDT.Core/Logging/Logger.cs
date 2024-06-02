using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDT.Core;

public class Logger
{
    public static new Logger _instace = new Logger();
    public static Logger Instance
    {
        get
        {
            return _instace;
        }
    }
    
    public event EventHandler<string> LogEvent;

    public void Log(string message)
    {
        LogEvent?.Invoke(this, message);
    }
}
