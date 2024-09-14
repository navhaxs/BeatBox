using System.Collections.ObjectModel;
using System.Linq;
using Melanchall.DryWetMidi.Multimedia;
using ReactiveUI;

namespace DrumMachine;

public class ConfigViewModel : ReactiveObject
{
    public ConfigViewModel()
    {
        OutputDevice.GetAll().ToList().ForEach(x => OutputDevices.Add(x));
    }
    
    public ObservableCollection<OutputDevice> OutputDevices { get; } = new();
    private OutputDevice? _selectedOutputDevice;
    public OutputDevice? SelectedOutputDevice { get => _selectedOutputDevice; set => this.RaiseAndSetIfChanged(ref _selectedOutputDevice, value); }
}