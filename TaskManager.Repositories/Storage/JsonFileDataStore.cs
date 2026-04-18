using System.Text.Json;
using System.Text.Json.Serialization;
using TaskManager.Repositories.Models;

namespace TaskManager.Repositories.Storage;

public sealed class JsonFileDataStore : IDataStore
{
    private readonly string _filePath;
    private readonly SemaphoreSlim _mutex = new(1, 1);
    private readonly JsonSerializerOptions _jsonOptions;

    private StorageSnapshot _snapshot = new();
    private bool _initialized;

    public JsonFileDataStore(string filePath)
    {
        _filePath = filePath;
        _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter() }
        };
    }

    public async Task InitializeAsync()
    {
        await _mutex.WaitAsync();
        try
        {
            if (_initialized)
                return;

            var directory = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            if (File.Exists(_filePath))
            {
                await using var stream = File.OpenRead(_filePath);
                var loaded = await JsonSerializer.DeserializeAsync<StorageSnapshot>(stream, _jsonOptions);
                _snapshot = loaded ?? new StorageSnapshot();
            }
            else
            {
                _snapshot = SeedDataFactory.CreateInitialSnapshot();
                await SaveUnsafeAsync();
            }

            _initialized = true;
        }
        finally
        {
            _mutex.Release();
        }
    }

    public async Task<IReadOnlyList<ProjectData>> GetProjectsAsync()
    {
        await EnsureInitializedAsync();
        await _mutex.WaitAsync();
        try
        {
            return _snapshot.Projects.Select(Clone).ToList();
        }
        finally
        {
            _mutex.Release();
        }
    }

    public async Task<ProjectData?> GetProjectByIdAsync(int id)
    {
        await EnsureInitializedAsync();
        await _mutex.WaitAsync();
        try
        {
            var project = _snapshot.Projects.FirstOrDefault(p => p.Id == id);
            return project is null ? null : Clone(project);
        }
        finally
        {
            _mutex.Release();
        }
    }

    public async Task<ProjectData> AddProjectAsync(ProjectData project)
    {
        ArgumentNullException.ThrowIfNull(project);
        await EnsureInitializedAsync();
        await _mutex.WaitAsync();
        try
        {
            var nextId = _snapshot.Projects.Count == 0 ? 1 : _snapshot.Projects.Max(p => p.Id) + 1;
            var created = new ProjectData(nextId, project.Name, project.Description, project.Type);
            _snapshot.Projects.Add(created);
            await SaveUnsafeAsync();
            return Clone(created);
        }
        finally
        {
            _mutex.Release();
        }
    }

    public async Task<bool> UpdateProjectAsync(ProjectData project)
    {
        ArgumentNullException.ThrowIfNull(project);
        await EnsureInitializedAsync();
        await _mutex.WaitAsync();
        try
        {
            var existing = _snapshot.Projects.FirstOrDefault(p => p.Id == project.Id);
            if (existing is null)
                return false;

            existing.Name = project.Name;
            existing.Description = project.Description;
            existing.Type = project.Type;
            await SaveUnsafeAsync();
            return true;
        }
        finally
        {
            _mutex.Release();
        }
    }

    public async Task<bool> DeleteProjectAsync(int id)
    {
        await EnsureInitializedAsync();
        await _mutex.WaitAsync();
        try
        {
            var index = _snapshot.Projects.FindIndex(p => p.Id == id);
            if (index < 0)
                return false;

            _snapshot.Projects.RemoveAt(index);
            _snapshot.Tasks.RemoveAll(t => t.ProjectId == id);
            await SaveUnsafeAsync();
            return true;
        }
        finally
        {
            _mutex.Release();
        }
    }

    public async Task<IReadOnlyList<TaskItemData>> GetTasksByProjectIdAsync(int projectId)
    {
        await EnsureInitializedAsync();
        await _mutex.WaitAsync();
        try
        {
            return _snapshot.Tasks.Where(t => t.ProjectId == projectId).Select(Clone).ToList();
        }
        finally
        {
            _mutex.Release();
        }
    }

    public async Task<TaskItemData?> GetTaskByIdAsync(int id)
    {
        await EnsureInitializedAsync();
        await _mutex.WaitAsync();
        try
        {
            var task = _snapshot.Tasks.FirstOrDefault(t => t.Id == id);
            return task is null ? null : Clone(task);
        }
        finally
        {
            _mutex.Release();
        }
    }

    public async Task<TaskItemData> AddTaskAsync(TaskItemData task)
    {
        ArgumentNullException.ThrowIfNull(task);
        await EnsureInitializedAsync();
        await _mutex.WaitAsync();
        try
        {
            if (!_snapshot.Projects.Any(p => p.Id == task.ProjectId))
                throw new InvalidOperationException($"Parent project {task.ProjectId} not found.");

            var nextId = _snapshot.Tasks.Count == 0 ? 1 : _snapshot.Tasks.Max(t => t.Id) + 1;
            var created = new TaskItemData(nextId, task.ProjectId, task.Title, task.Description,
                task.Priority, task.DueDate, task.IsCompleted);
            _snapshot.Tasks.Add(created);
            await SaveUnsafeAsync();
            return Clone(created);
        }
        finally
        {
            _mutex.Release();
        }
    }

    public async Task<bool> UpdateTaskAsync(TaskItemData task)
    {
        ArgumentNullException.ThrowIfNull(task);
        await EnsureInitializedAsync();
        await _mutex.WaitAsync();
        try
        {
            var existing = _snapshot.Tasks.FirstOrDefault(t => t.Id == task.Id);
            if (existing is null)
                return false;

            existing.Title = task.Title;
            existing.Description = task.Description;
            existing.Priority = task.Priority;
            existing.DueDate = task.DueDate;
            existing.IsCompleted = task.IsCompleted;
            await SaveUnsafeAsync();
            return true;
        }
        finally
        {
            _mutex.Release();
        }
    }

    public async Task<bool> DeleteTaskAsync(int id)
    {
        await EnsureInitializedAsync();
        await _mutex.WaitAsync();
        try
        {
            var index = _snapshot.Tasks.FindIndex(t => t.Id == id);
            if (index < 0)
                return false;

            _snapshot.Tasks.RemoveAt(index);
            await SaveUnsafeAsync();
            return true;
        }
        finally
        {
            _mutex.Release();
        }
    }

    private async Task EnsureInitializedAsync()
    {
        if (_initialized)
            return;
        await InitializeAsync();
    }

    private async Task SaveUnsafeAsync()
    {
        await using var stream = File.Create(_filePath);
        await JsonSerializer.SerializeAsync(stream, _snapshot, _jsonOptions);
    }

    private static ProjectData Clone(ProjectData p) =>
        new(p.Id, p.Name, p.Description, p.Type);

    private static TaskItemData Clone(TaskItemData t) =>
        new(t.Id, t.ProjectId, t.Title, t.Description, t.Priority, t.DueDate, t.IsCompleted);
}
