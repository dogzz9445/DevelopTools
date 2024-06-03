using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DDT.Core.WidgetSystems.Contracts.Services;

namespace DDT.Core.WidgetSystems.Services;

public class WebRequestSecretService : ISecretService
{

    public async Task<string> GetSecretAsync()
    {
        var request = WebRequest.Create("https://SampleurlSecretAsync.com/");

        request.ContentType = "application/json";
        var resopnse = await request.GetRequestStreamAsync();
        
        // FIXME:


        return resopnse.ToString();
    }
}
