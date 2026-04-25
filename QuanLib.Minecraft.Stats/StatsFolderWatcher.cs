using Microsoft.Extensions.Logging;
using QuanLib.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLib.Minecraft.Stats
{
    public class StatsFolderWatcher(ILogger<StatsFolderWatcher>? logger = null) : IStatsFolderWatcher, IDisposable
    {
        private readonly ILogger<StatsFolderWatcher>? _logger = logger;
        private readonly Dictionary<Guid, PlayerStats> _stats = [];
        private readonly SemaphoreSlim _initSemaphore = new(1);
        private readonly SemaphoreSlim _watcherSemaphore = new(1);
        private readonly SemaphoreSlim _usageSemaphore = new(1);
        private string? _statsFolder;
        private FileSystemWatcher? _statsWatcher;
        private int _usageCount;

        public async Task StartAsync(string statsFolder)
        {
            ArgumentException.ThrowIfNullOrEmpty(statsFolder, nameof(statsFolder));
            ThrowHelper.DirectoryNotFound(statsFolder);

            await _initSemaphore.WaitAsync();
            try
            {
                if (_statsWatcher is not null)
                    throw new InvalidOperationException("Watcher is already running.");

                _statsFolder = statsFolder;
                _statsWatcher = new FileSystemWatcher(statsFolder)
                {
                    EnableRaisingEvents = true,
                    IncludeSubdirectories = false,
                    NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite,
                    Filter = "*.json"
                };

                _statsWatcher.Changed += OnStatsFileChanged;
                _statsWatcher.Deleted += OnStatsFileDeleted;
                _statsWatcher.Renamed += OnStatsFileRenamed;

                await ReloadAsync();
            }
            finally
            {
                _initSemaphore.Release();
            }
        }

        public async Task ReloadAsync()
        {
            await _watcherSemaphore.WaitAsync();
            try
            {
                if (!Directory.Exists(_statsFolder))
                    return;

                await WaitForUserReturnAsync();

                string[] files = Directory.GetFiles(_statsFolder, "*.json");
                _stats.Clear();

                foreach (string file in files)
                {
                    string fileName = Path.GetFileNameWithoutExtension(file);
                    if (!Guid.TryParse(fileName, out Guid uuid))
                        continue;
                    try
                    {
                        string json = await File.ReadAllTextAsync(file);
                        PlayerStats stats = PlayerStats.Parse(json);
                        _stats[uuid] = stats;
                    }
                    catch (Exception ex)
                    {
                        _logger?.LogError(ex, "Failed to read or parse stats file: {File}", file);
                    }
                }

                if (_logger?.IsEnabled(LogLevel.Information) == true)
                    _logger.LogInformation("Stats reloaded {Count} files from folder: {Folder}", _stats.Count, _statsFolder);
            }
            finally
            {
                _watcherSemaphore.Release();
            }
        }

        public void Stop()
        {
            Dispose();
        }

        public Dictionary<Guid, PlayerStats> CloneStats()
        {
            _watcherSemaphore.Wait();
            try
            {
                return new Dictionary<Guid, PlayerStats>(_stats);
            }
            finally
            {
                _watcherSemaphore.Release();
            }
        }

        public async Task<Dictionary<Guid, PlayerStats>> CloneStatsAsync()
        {
            await _watcherSemaphore.WaitAsync();
            try
            {
                return new Dictionary<Guid, PlayerStats>(_stats);
            }
            finally
            {
                _watcherSemaphore.Release();
            }
        }

        public IReadOnlyDictionary<Guid, PlayerStats> UseStats()
        {
            _watcherSemaphore.Wait();
            _usageSemaphore.Wait();
            try
            {
                _usageCount++;
                return _stats.AsReadOnly();
            }
            finally
            {
                _usageSemaphore.Release();
                _watcherSemaphore.Release();
            }
        }

        public async Task<IReadOnlyDictionary<Guid, PlayerStats>> UseStatsAsync()
        {
            await _watcherSemaphore.WaitAsync();
            await _usageSemaphore.WaitAsync();
            try
            {
                _usageCount++;
                return _stats.AsReadOnly();
            }
            finally
            {
                _usageSemaphore.Release();
                _watcherSemaphore.Release();
            }
        }

        public void ReturnStats()
        {
            _usageSemaphore.Wait();
            try
            {
                if (_usageCount > 0)
                    _usageCount--;
            }
            finally
            {
                _usageSemaphore.Release();
            }
        }

        private void OnStatsFileChanged(object sender, FileSystemEventArgs e)
        {
            _watcherSemaphore.Wait();
            try
            {
                string fullName = e.FullPath;
                string fileName = Path.GetFileNameWithoutExtension(fullName);
                if (!Guid.TryParse(fileName, out Guid uuid))
                    return;

                WaitForUserReturn();
                try
                {
                    string json = File.ReadAllText(fullName);
                    PlayerStats stats = PlayerStats.Parse(json);
                    _stats[uuid] = stats;

                    if (_logger?.IsEnabled(LogLevel.Debug) == true)
                        _logger.LogDebug("Stats file {File} updated, Length: {Length}", Path.GetFileName(fullName), json.Length);
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Failed to read or parse stats file: {File}", fullName);
                }
            }
            finally
            {
                _watcherSemaphore.Release();
            }
        }

        private void OnStatsFileDeleted(object sender, FileSystemEventArgs e)
        {
            _watcherSemaphore.Wait();
            try
            {
                string fullName = e.FullPath;
                string fileName = Path.GetFileNameWithoutExtension(fullName);
                if (!Guid.TryParse(fileName, out Guid uuid) || !_stats.ContainsKey(uuid))
                    return;

                WaitForUserReturn();
                _stats.Remove(uuid);

                if (_logger?.IsEnabled(LogLevel.Debug) == true)
                    _logger.LogDebug("Stats file {File} deleted.", Path.GetFileName(fullName));
            }
            finally
            {
                _watcherSemaphore.Release();
            }
        }

        private void OnStatsFileRenamed(object sender, RenamedEventArgs e)
        {
            _watcherSemaphore.Wait();
            try
            {
                if (!Guid.TryParse(Path.GetFileNameWithoutExtension(e.OldFullPath), out Guid oldUuid) ||
                    !Guid.TryParse(Path.GetFileNameWithoutExtension(e.FullPath), out Guid newUuid))
                    return;

                WaitForUserReturn();

                if (!_stats.ContainsKey(newUuid) && _stats.Remove(oldUuid, out var stats))
                {
                    _stats[newUuid] = stats;

                    if (_logger?.IsEnabled(LogLevel.Debug) == true)
                        _logger.LogDebug("Stats file renamed from {OldFile} to {NewFile}", Path.GetFileName(e.OldFullPath), Path.GetFileName(e.FullPath));
                }
            }
            finally
            {
                _watcherSemaphore.Release();
            }
        }

        private async Task WaitForUserReturnAsync()
        {
            while (_usageCount > 0)
                await Task.Delay(10);
        }

        private void WaitForUserReturn()
        {
            while (_usageCount > 0)
                Thread.Sleep(10);
        }

        public void Dispose()
        {
            if (_statsWatcher is not null)
            {
                _statsWatcher.Changed -= OnStatsFileChanged;
                _statsWatcher.Created -= OnStatsFileChanged;
                _statsWatcher.Deleted -= OnStatsFileDeleted;
                _statsWatcher.Renamed -= OnStatsFileRenamed;

                _statsWatcher.EnableRaisingEvents = false;
                _statsWatcher.Dispose();
            }

            _initSemaphore.Dispose();
            _watcherSemaphore.Dispose();
            _usageSemaphore.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
