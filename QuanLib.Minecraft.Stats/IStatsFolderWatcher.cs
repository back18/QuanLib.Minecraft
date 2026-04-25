using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLib.Minecraft.Stats
{
    public interface IStatsFolderWatcher
    {
        public Task StartAsync(string statsFolder);

        public Task ReloadAsync();

        public void Stop();

        public Dictionary<Guid, PlayerStats> CloneStats();

        public Task<Dictionary<Guid, PlayerStats>> CloneStatsAsync();

        public IReadOnlyDictionary<Guid, PlayerStats> UseStats();

        public Task<IReadOnlyDictionary<Guid, PlayerStats>> UseStatsAsync();

        public void ReturnStats();
    }
}
