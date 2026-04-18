using TaskManager.Repositories.Storage;

namespace TaskManager.Services;

internal sealed class StorageInitializer : IStorageInitializer
{
    private readonly IDataStore _dataStore;

    public StorageInitializer(IDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public Task InitializeAsync() => _dataStore.InitializeAsync();
}
