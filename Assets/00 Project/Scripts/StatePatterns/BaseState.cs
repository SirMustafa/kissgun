using System;

public class BaseState : IState
{
    protected IDisposable _disposable;
    public virtual void Initialize()
    {
        
    }
    
    public virtual void Dispose()
    {
        if (_disposable != null)
            _disposable.Dispose();
    }
    
    public virtual void DoState()
    {
        
    }
}