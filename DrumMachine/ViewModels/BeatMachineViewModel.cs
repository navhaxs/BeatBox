using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Runtime.Serialization;
using Avalonia.Threading;
using DrumMachine.Engine;
using DrumMachine.Models;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Multimedia;
using ReactiveUI;

namespace DrumMachine;

[DataContract]
public class BeatMachineViewModel : ReactiveObject
{
    private int _count;
    public BeatMachine BeatMachine { get; }

    public ReactiveCommand<String, Unit> TriggerNoteClick { get; }
    public BeatMachineViewModel()
    {
        _instrumentTracks = new List<InstrumentTrack>()
        {
            new(instrumentTrackName: "c", noteNumber: (SevenBitNumber)(byte)MidiConstants.GM1_Drums.Crash1),
            new(instrumentTrackName: "hh", noteNumber: (SevenBitNumber)(byte)MidiConstants.GM1_Drums.ClosedHiHat),
            new(instrumentTrackName: "sn", noteNumber: (SevenBitNumber)(byte)MidiConstants.GM1_Drums.Snare),
            new(instrumentTrackName: "k", noteNumber: (SevenBitNumber)(byte)MidiConstants.GM1_Drums.Bass_Drum)
        };
        
        BeatMachine = new(this);
        
        _currentBeatCount = this.WhenAnyValue(x => x.Count)
            .Select(x => x / BeatMachine.BEAT_SUBDIVISION)
            .ToProperty(this, x => x.CurrentBeatCount);
        
        _beatGrid = this.WhenAnyValue(x => x.CurrentBeatCount)
            .Select(currentBeat => new List<bool> {
                currentBeat == 0, currentBeat == 1, currentBeat == 2, currentBeat == 3,
            })
            .ToProperty(this, x => x.BeatGrid);
        
        _subbeatGrid = this.WhenAnyValue(x => x.Count)
            .Select(currentBeat => Enumerable.Repeat(false, BeatMachine.TIMER_RESOLUTION).Select((_, i) => i == currentBeat).ToList())
            .ToProperty(this, x => x.SubdivisionBeatGrid);

        TriggerNoteClick = ReactiveCommand.Create<String>((v) =>
        {
            SevenBitNumber noteNumber = (SevenBitNumber)(byte)Convert.ToInt32(v);
            BeatMachine.TriggerNote(noteNumber);
        });
    }

    public int Count
    {
        get => _count; set => this.RaiseAndSetIfChanged(ref _count, value);
    }

    
    // count
    private readonly ObservableAsPropertyHelper<int> _currentBeatCount;
    public int CurrentBeatCount => _currentBeatCount.Value;

    
    // bpm
    private readonly ObservableAsPropertyHelper<List<bool>> _beatGrid;
    public List<bool> BeatGrid => _beatGrid.Value;
    
    private readonly ObservableAsPropertyHelper<List<bool>> _subbeatGrid;
    public List<bool> SubdivisionBeatGrid => _subbeatGrid.Value;
    
    // instrument tracks
    private List<InstrumentTrack> _instrumentTracks;

    [DataMember]
    public List<InstrumentTrack> InstrumentTracks
    {
        get => _instrumentTracks;
        set => this.RaiseAndSetIfChanged(ref _instrumentTracks, value);
    }
    
    // config
    public ConfigViewModel ConfigViewModel { get; } = new();
}