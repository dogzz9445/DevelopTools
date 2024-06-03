using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using DDT.Core.Contracts.Services;

namespace DDT.Core.Services;

public class JsonFileStorageService : IStorageService
{
    private static readonly JsonSerializerOptions OPTIONS = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter() }
    };

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
        var json = await JsonSerializer.DeserializeAsync<T>(stream, OPTIONS);
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
        await JsonSerializer.SerializeAsync<T>(stream, content, OPTIONS);
        return args;
    }
}
