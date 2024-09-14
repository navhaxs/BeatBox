using System;
using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using Melanchall.DryWetMidi.Multimedia;
using ReactiveUI;

namespace DrumMachine;

public partial class MainWindow : ReactiveWindow<BeatMachineViewModel>
{
    public MainWindow()
    {
        InitializeComponent();
        this.WhenActivated(disposable => { });
    }
}