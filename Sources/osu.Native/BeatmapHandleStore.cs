using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading;
using osu.Game.Beatmaps;

namespace osu.Native;

internal class BeatmapHandleStore
{
    private readonly ConcurrentDictionary<int, WorkingBeatmap> _store = new();
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
    public void Release(int id) => _store.Remove(id, out _);
}