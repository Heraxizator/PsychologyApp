namespace PsychologyApp.Presentation.ViewModels;

public partial class BaseViewModel
{
    protected void SetDefault()
    {
        IsInit = false;
        IsFail = false;
        IsDone = false;
        IsCreated = false;
    }

    public void SetDone()
    {
        SetDefault();
        IsDone = true;
    }

    public void SetInit()
    {
        SetDefault();
        IsInit = true;
    }

    public void SetFail()
    {
        SetDefault();
        IsFail = true;
    }

    public void SetCreated()
    {
        SetDefault();
        IsCreated = true;
    }

    protected void CancelProgress()
    {
        if (IsInit)
        {
            SetDone();
        }
    }
}
