using System;

namespace GalacticMeltdown.UserInterfaceRelated.InputProcessing;

public abstract class Controller
{
    private bool _flushBuffer;
    
    protected Controller(bool flushBuffer)
    {
        _flushBuffer = flushBuffer;
    }

    public virtual void HandleKey(ConsoleKeyInfo keyInfo)
    {
        if (_flushBuffer) FlushBuffer();
    }

    private void FlushBuffer()
    {
        while (Console.KeyAvailable) Console.ReadKey(true);
    }
}