using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Test1.Consts;
using Test1.Models;
using Test1.Services;

namespace Test1.ViewModels;

public partial class MainViewModel : ObservableValidator
{
    private TestCaseFactory _factory = new();
    
    private ImportExportService _importExport = new();

    private int _lowerDegree = 15;

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
            if(value > 100)
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
    private async Task GenerateTestCases()
    {
        TestCases = await _factory.CreateTestCases(Step,LowerDegree, UpperDegree, LeftBoundary, RightBoundary,
            IntegrationStep, IntegrationMehtod);
    }

    [RelayCommand]
    private async Task RunTests()
    {
        if (TestCases.Count == 0)
        {
            return;
        }

        foreach (var testCase in TestCases)
        {
            var cancel = testCase.Cts.Token;
            Task.Run(() => testCase.Run(waitingTime), cancel );
        }
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
    private async Task Export()
    {
        await _importExport.ExportTestCases(TestCases.Where(t => t.Selected).ToList());
    }
    [RelayCommand]
    private async Task Import()
    {
        TestCases = await  _importExport.ImportTestCases();
    }
}