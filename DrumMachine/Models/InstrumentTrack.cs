using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using Melanchall.DryWetMidi.Common;
using ReactiveUI;

namespace DrumMachine.Models;

[DataContract]
public class InstrumentTrack: ReactiveObject
{
    private string _instrumentTrackName = "hh";
    
    [DataMember]
    public string InstrumentTrackName
    {
        get => _instrumentTrackName; set => this.RaiseAndSetIfChanged(ref _instrumentTrackName, value);
    }
    
    private bool _isMute = false;
    [DataMember]
    public bool IsMute
    {
        get => _isMute; set => this.RaiseAndSetIfChanged(ref _isMute, value);
    }

    private SevenBitNumber _noteNumber = SevenBitNumber.MinValue;
    [DataMember]
    public SevenBitNumber NoteNumber { get => _noteNumber; set => this.RaiseAndSetIfChanged(ref _noteNumber, value); }

    // TODO observable
    private ObservableCollection<Note> _beatGrid = new(Enumerable.Range(1, 16).Select(i => new Note()).ToList());

    public InstrumentTrack()
    {
    }

    public InstrumentTrack(string instrumentTrackName, SevenBitNumber noteNumber)
    {
        InstrumentTrackName = instrumentTrackName;
        NoteNumber = noteNumber;
    }

    [DataMember]
    public ObservableCollection<Note> BeatGrid
    {
        get => _beatGrid; set => this.RaiseAndSetIfChanged(ref _beatGrid, value);
    }
}