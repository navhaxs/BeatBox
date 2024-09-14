using System;
using Avalonia.Controls;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Multimedia;
using ReactiveUI;

namespace DrumMachine.Engine;

public class BeatMachine : ReactiveObject, IDisposable
{
    private static readonly object _lock = new();

    private int _bpm = 120;

    public int BeatsPerMinute
    {
        get => _bpm;
        set => this.RaiseAndSetIfChanged(ref _bpm, value);
    }

    private bool _isPlaying = true;

    public bool IsPlaying
    {
        get => _isPlaying;
        set => this.RaiseAndSetIfChanged(ref _isPlaying, value);
    }

    private MidiClock midiClock;
    private BeatMachineViewModel vm;

    public const int BEATS = 3;
    public const int BEAT_SUBDIVISION = 4;
    public const int TIMER_RESOLUTION = BEATS * BEAT_SUBDIVISION;

    public BeatMachine(BeatMachineViewModel vm)
    {
        this.vm = vm;
        if (Avalonia.Controls.Design.IsDesignMode)
        {
            return;
        }

        vm.WhenAnyValue(vm => vm.ConfigViewModel.SelectedOutputDevice)
            .Subscribe(x =>
            {
                lock (_lock)
                {
                    _outputDevice?.Dispose();

                    if (x != null)
                    {
                        _outputDevice = OutputDevice.GetByName(x.Name);
                        // TODO catch
                        _outputDevice.PrepareForEventsSending();
                    }
                    else
                    {
                        _outputDevice = null;
                    }
                }
            });

        // _outputDevice.EventSent += OnEventSent;

        midiClock = new MidiClock(false, new RegularPrecisionTickGenerator(),
            TimeSpan.FromMilliseconds((double)60_000L / BeatsPerMinute / BEAT_SUBDIVISION));

        midiClock.Ticked += (s, e) =>
        {
            vm.Count = (vm.Count + 1) % TIMER_RESOLUTION;

            foreach (var instrumentTrack in vm.InstrumentTracks)
            {
                var note = instrumentTrack.BeatGrid[vm.Count];
                if (note?.IsOn == true)
                {
                    TriggerNote(instrumentTrack.NoteNumber, note.Velocity);
                }
            }
        };

        this.WhenAnyValue(x => x.IsPlaying)
            .Subscribe(x =>
            {
                if (x)
                {
                    OnStartPlayback();
                }
                else
                {
                    midiClock.Stop();
                }
            });

        this.WhenAnyValue(x => x.BeatsPerMinute)
            .Subscribe(x =>
            {
                midiClock.Stop();
                midiClock = new MidiClock(false, new RegularPrecisionTickGenerator(),
                    TimeSpan.FromMilliseconds((double)60_000L / BeatsPerMinute / BEAT_SUBDIVISION));
                midiClock.Ticked += (s, e) =>
                {
                    vm.Count = (vm.Count + 1) % TIMER_RESOLUTION;

                    OnFireTick(vm.Count);
                };
                midiClock.Start();
            });

        if (IsPlaying)
        {
            OnStartPlayback();
        }
        else
        {
            midiClock.Stop();
        }
    }

    private void OnStartPlayback()
    {
        vm.Count = 0;
        midiClock.Start();
        OnFireTick(0);
    }

    private void OnFireTick(int count)
    {
        foreach (var instrumentTrack in vm.InstrumentTracks)
        {
            if (instrumentTrack.IsMute)
                continue;
            
            var note = instrumentTrack.BeatGrid[count];
            if (note?.IsOn == true)
            {
                TriggerNote(instrumentTrack.NoteNumber, note.Velocity);
            }
        }
    }

    public void TriggerNote(SevenBitNumber noteNumber, SevenBitNumber? velocity = null)
    {
        lock (_lock)
        {
            //_outputDevice?.SendEvent(new ProgramChangeEvent((SevenBitNumber)(byte)119) { Channel = (FourBitNumber)(byte)9 });

            _outputDevice?.SendEvent(new NoteOnEvent(noteNumber, velocity ?? SevenBitNumber.MaxValue)
                { Channel = (FourBitNumber)(byte)9 });
            _outputDevice?.SendEvent(new NoteOffEvent(noteNumber, velocity ?? SevenBitNumber.MaxValue)
                { Channel = (FourBitNumber)(byte)9 });
        }
    }

    private static IOutputDevice? _outputDevice;

    public void Dispose()
    {
        lock (_lock)
        {
            _outputDevice?.Dispose();
        }
    }
}