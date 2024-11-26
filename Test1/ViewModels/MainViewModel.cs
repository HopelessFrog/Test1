using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows;
using Test1.Consts;
using Test1.Models;
using Test1.Services;

namespace Test1.ViewModels;

public partial class MainViewModel : ObservableValidator
{
    private TestCaseFactory _factory = new();

    private ImportExportService _importExport = new();

    private Random _random = new Random();

    private int _lowerDegree = 15;

    [ObservableProperty]
    private int maxConcurrency = 3;

    [ObservableProperty]
    private ObservableCollection<Coef> coefs = new ObservableCollection<Coef>();

    public MainViewModel()
    {
        coefs.CollectionChanged += Coefs_CollectionChanged;
    }

    private void Coefs_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        OnPropertyChanged(nameof(Coefs));
    }

    public int LowerDegree
    {
        get => _lowerDegree;
        set
        {
            if (value < _upperDegree)
                SetProperty(ref _lowerDegree, value);
        }
    }

    private int _upperDegree = 100;

    public int UpperDegree
    {
        get => _upperDegree;
        set
        {
            if (value > _lowerDegree)
                SetProperty(ref _upperDegree, value);
            if (value > 100)
                SetProperty(ref _upperDegree, 100);
        }
    }

    private bool selectAll;
    public bool SelectAll
    {
        get { return selectAll; }
        set
        {
            if (SetProperty(ref selectAll, value))
            {
                SelectOrDeselectAll();
                OnPropertyChanged(nameof(TestCases));

            }
        }
    }
    private void SelectOrDeselectAll()
    {
        TestCases.ForEach(c => c.Selected = SelectAll);

        var qwe = TestCases;
    }

    [ObservableProperty] private int waitingTime = 10000;
    [ObservableProperty] private int step = 1;
    [ObservableProperty] private IntegrationMehtod _integrationMehtod = IntegrationMehtod.Simson;

    [ObservableProperty] private double _leftBoundary = 1;
    [ObservableProperty] private double _rightBoundary = 1000;
    [ObservableProperty] private double _integrationStep = 0.00006;
    [ObservableProperty] private List<TestCase> _testCases = new List<TestCase>();

    [RelayCommand]
    public async Task AddCoef()
    {
        Coefs.Add(new Coef() { Number = Coefs.Count + 1, Value = _random.NextDouble() });
    }

    [RelayCommand]
    private async Task GenerateTestCases()
    {
        if (Coefs.Count == 0)
        {
            MessageBox.Show("Сначала сгенерируйте коэфиценты");
            return;
        }
        if (Coefs.Count < UpperDegree)
        {
            MessageBox.Show("Коэфицентов не достаточно для данной степени полинома");
            return;
        }
        TestCases = await _factory.CreateTestCases(Step, LowerDegree, UpperDegree, LeftBoundary, RightBoundary,
            IntegrationStep, IntegrationMehtod, Coefs.ToList());
    }

    [RelayCommand]
    private async Task RunTests()
    {
        if (TestCases.Count == 0)
        {
            return;
        }

        SemaphoreSlim semaphore = new SemaphoreSlim(maxConcurrency);

        List<Task> runningTasks = new List<Task>();

        foreach (var testCase in TestCases)
        {
            await semaphore.WaitAsync();
            var cancel = testCase.Cts.Token;
            var task = Task.Run(async () =>
            {
                try
                {
                    testCase.Run(waitingTime);
                }
                finally
                {
                    semaphore.Release();
                }
            }, cancel);

            runningTasks.Add(task);
        }

        await Task.WhenAll(runningTasks);
    }

    [RelayCommand]
    private async Task StopTests()
    {
        if (TestCases.Count == 0)
        {
            return;
        }

        foreach (var testCase in TestCases)
        {
            testCase.Stop();
        }
    }

    [RelayCommand]
    private async Task GenerateCoefs()
    {
        var coefsss = new List<Coef>();
        for (int i = 0; i < UpperDegree; i++)
        {
            coefsss.Add(new() { Number = i + 1, Value = _random.NextDouble() });
        }

        Coefs.Clear();
        foreach (var coef in coefsss)
        {
            Coefs.Add(coef);
        }

    }

    [RelayCommand]
    private async Task Export()
    {
        await _importExport.ExportTestCases(TestCases.Where(t => t.Selected).ToList());
    }
    [RelayCommand]
    private async Task Import()
    {
        TestCases = await _importExport.ImportTestCases();
    }
}