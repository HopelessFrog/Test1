using System.Diagnostics;
using System.Text;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using Test1.Consts;

namespace Test1.Models;

public partial class TestCase : ObservableObject
{
    private Process _process;

    public CancellationTokenSource Cts = new();

    public string ArgLine
    {
        get
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(
                $"{LeftBoundary.ToString("0." + new string('#', 339))}" +
                $" {RightBoundary.ToString("0." + new string('#', 339))}" +
                $" {IntegrationStep.ToString("0." + new string('#', 339))} {(int)Method + 1} ");
            foreach (var coef in Coefficients)
            {
                stringBuilder.Append(coef);
                stringBuilder.Append(" ");
            }

            return stringBuilder.ToString();
        }
    }

    private Stopwatch _stopwatch;
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsTime))]
    private bool _busy = false;

    [ObservableProperty] private string _resultText = "Не запускался";

    public int Degree => Coefficients.Count;

    [NotifyPropertyChangedFor(nameof(IsGone))]
    [NotifyPropertyChangedFor(nameof(IsPass))]
    [NotifyPropertyChangedFor(nameof(IsTime))]
    [ObservableProperty]
    private CaseState _state = CaseState.None;

    [ObservableProperty] private string time;
    public bool IsGone => State == CaseState.Pass || State == CaseState.Fail;
    
    public bool IsPass => State == CaseState.Pass;
    public double LeftBoundary { get; set; }
    public double RightBoundary { get; set; }
    public double IntegrationStep { get; set; }
    public IntegrationMehtod Method { get; set; }
    public List<double> Coefficients { get; set; }

    public bool IsTime => IsGone || Busy;
    

    [ObservableProperty] private bool _selected = false;
    [ObservableProperty] private bool result = false;

    public async Task Stop()
    {
        if (!_selected || !Busy)
            return;
        
        Cts.Cancel();
        _process?.Kill();
        _stopwatch?.Stop();
        Busy = false;
        State = CaseState.None;
        ResultText = "Программа прервана вручную";
        Cts = new();
    }

    public async Task Run(int delayTimeMiliseconds )
    {
        if (!_selected)
            return;
        State = CaseState.None;
        Busy = true;
        ResultText = "Исполняется";
        _process = new Process();
        _process.StartInfo.FileName = @"Integral3x.exe";
        _process.StartInfo.RedirectStandardInput = true;
        _process.StartInfo.RedirectStandardOutput = true;
        _process.StartInfo.CreateNoWindow = true;
        _process.StartInfo.UseShellExecute = false;
        _process.StartInfo.Arguments = ArgLine.Replace(".", ",");
        _process.Start();
        


        _process.StandardInput.Write(' ');

        var exited = false;
         _stopwatch = Stopwatch.StartNew();
        while (!_process.HasExited && _stopwatch.ElapsedMilliseconds < delayTimeMiliseconds)
        {
            Thread.Sleep(1);
            Time = _stopwatch.ElapsedMilliseconds.ToString();
            if (Cts.Token.IsCancellationRequested)  
            {
                return;   
            }
        }

        exited = _stopwatch.ElapsedMilliseconds < delayTimeMiliseconds;

        _stopwatch.Stop();


        if (exited)
        {
            ResultText = _process.StandardOutput.ReadLine();
            if (_process.ExitCode == 0)
            {
                State = CaseState.Pass;
            }
            else
            {
                State = CaseState.Fail;
            }
        }
        else
        {
            ResultText = $"Превышен временной лимит({delayTimeMiliseconds} мс)";
            _process.Kill();
            State = CaseState.Fail;
        }


        Busy = false;
        Result = true;
        _process.Dispose();
    }
}