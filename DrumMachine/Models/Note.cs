using System.Runtime.Serialization;
using Melanchall.DryWetMidi.Common;
using ReactiveUI;

namespace DrumMachine.Models;

[DataContract]
public class Note : ReactiveObject
{
    public SevenBitNumber Velocity = SevenBitNumber.MaxValue;
    public bool _isOn;
    
    [DataMember]
    public bool IsOn
    {
        get => _isOn; set => this.RaiseAndSetIfChanged(ref _isOn, value);
    }
}