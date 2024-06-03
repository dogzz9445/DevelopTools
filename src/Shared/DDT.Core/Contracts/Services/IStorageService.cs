﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDT.Core.Contracts.Services;

public enum StorageHandleCode
{
    None,
    Error,
    Null,
    Success,
}

public interface IStorageHandleArgs<T>
{
    StorageHandleCode Code { get; }
    Func<T> Data { get; }
    string Message { get; }
}

public class StorageHandleArgs<T> : IStorageHandleArgs<T>
{
    public StorageHandleCode Code { get; set; }
    public Func<T> Data { get; set; }
    public string Message { get; set; }
}

public interface IStorageService
{
    Task<StorageHandleArgs<T>> ReadAsync<T>(string filename);

    Task<StorageHandleArgs<T>> SaveAsync<T>(string filename, T content);

    void Delete(string filename);
}
