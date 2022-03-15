using System;

namespace GalacticMeltdown.InputProcessing;

public abstract class KeyHandler
{
    private bool _flushBuffer;
    
    protected KeyHandler(bool flushBuffer)
    {
        _flushBuffer = flushBuffer;
    }

    public virtual void HandleKey(ConsoleKey key)
    {
        if (_flushBuffer) FlushBuffer();
    }

    private void FlushBuffer()
    {
        while (Console.KeyAvailable) Console.ReadKey(true);
    }
}