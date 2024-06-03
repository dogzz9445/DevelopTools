using DDT.Core.Contracts.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DDT.Core.Services;

public class JsonFileStorageService : IStorageService
{
    public void Delete(string filename)
    {
        if (string.IsNullOrEmpty(filename))
            return;

        if (File.Exists(filename))
        {
            // FIXME:
            // 접근권한 문제 시 수정
            File.Delete(filename);
        }
    }

    public async Task<StorageHandleArgs<T>> ReadAsync<T>(string filename)
    {
        StorageHandleArgs<T> args = new StorageHandleArgs<T>();
        if (!File.Exists(filename))
            return default;

        using var stream = File.OpenRead(filename);
        var json = await JsonSerializer.DeserializeAsync<T>(stream,
            new JsonSerializerOptions()
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
            });
        args.Data = () => json;
        return args;
    }

    public async Task<StorageHandleArgs<T>> SaveAsync<T>(string filename, T content)
    {
        var directoryName = Path.GetDirectoryName(filename);
        if (!string.IsNullOrEmpty(directoryName) &&
            !Directory.Exists(directoryName))
            Directory.CreateDirectory(directoryName);

        StorageHandleArgs<T> args = new StorageHandleArgs<T>();

        using var stream = File.OpenWrite(filename);
        await JsonSerializer.SerializeAsync<T>(stream, content,
            new JsonSerializerOptions()
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
                WriteIndented = true,
            });
        return args;
    }
}
