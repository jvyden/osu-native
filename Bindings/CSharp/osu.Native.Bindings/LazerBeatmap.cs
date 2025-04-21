using System;

namespace osu.Native.Bindings;

public class LazerBeatmap : IDisposable
{
    internal readonly int Handle;

    internal LazerBeatmap(int handle)
    {
        this.Handle = handle;
    }

    public void Dispose()
    {
        Native.FreeBeatmap(Handle);
    }
}