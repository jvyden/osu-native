using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading;
using osu.Game.Beatmaps;

namespace osu.Native;

internal class BeatmapHandleStore
{
    private readonly Dictionary<int, WorkingBeatmap> _store = new();
    private int _idIncrement = 1;

    public int Store(WorkingBeatmap beatmap)
    {
        int id = Interlocked.Increment(ref _idIncrement);
        _store[id] = beatmap;
        return id;
    }

    [Pure]
    public WorkingBeatmap? Get(int id) => _store.GetValueOrDefault(id);

    public bool Has(int id) => _store.ContainsKey(id);
    public bool Has(WorkingBeatmap map) => _store.ContainsValue(map);
    public void Release(int id) => _store.Remove(id);
}