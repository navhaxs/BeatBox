using ReactiveUI;

namespace DrumMachine.Models;

public class TimeSignature : ReactiveObject
{
    private int _beats = 3;
    public int Beats
    {
        get => _beats; set => this.RaiseAndSetIfChanged(ref _beats, value);
    }
    
    private int _subdivision = 3;
    public int Subdivision {
        get => _subdivision; set => this.RaiseAndSetIfChanged(ref _subdivision, value);
    }
}