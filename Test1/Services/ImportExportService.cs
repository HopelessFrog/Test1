using System.IO;
using System.Windows;
using System.Windows.Documents;
using MaterialDesignExtensions.Controls;
using MaterialDesignExtensions.Controls;
using Test1.Consts;
using Test1.Models;
using Test1.Windows;

namespace Test1.Services;

public class ImportExportService
{
    public async Task ExportTestCases(List<TestCase> testCases)
    {
        var dialog = new Microsoft.Win32.SaveFileDialog();
        dialog.FileName = "testCases"; // Default file name
        dialog.DefaultExt = ".txt"; // Default file extension
        dialog.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension


        bool? result = dialog.ShowDialog();

        if (result == true)
        {
            string filename = dialog.FileName;
            try
            {
                using (var writer = new StreamWriter(filename))
                {
                    foreach (var testCase in testCases)
                    {
                        writer.WriteLine(testCase.LeftBoundary.ToString("0.#########"));
                        writer.WriteLine(testCase.RightBoundary.ToString("0.#########"));
                        writer.WriteLine(testCase.IntegrationStep.ToString("0.#########"));
                        writer.WriteLine((int)testCase.Method);
                        writer.WriteLine(string.Join(",", testCase.Coefficients));

                        writer.WriteLine("----");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при экспорте! ");
            }
        }
    }

    public async Task<List<TestCase>> ImportTestCases()
    {
        var dialog = new Microsoft.Win32.OpenFileDialog();
        dialog.FileName = "testCases"; // Default file name
        dialog.DefaultExt = ".txt"; // Default file extension
        dialog.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension


        bool? result = dialog.ShowDialog();

        if (result == true)
        {
            string filename = dialog.FileName;
            var testCases = new List<TestCase>();
            try
            {
                using (var reader = new StreamReader(filename))
                {
                    while (!reader.EndOfStream)
                    {
                        var testCase = new TestCase();

                        if (!double.TryParse(reader.ReadLine(), out double leftBoundary))
                        {
                            continue;
                        }

                        testCase.LeftBoundary = leftBoundary;

                        if (!double.TryParse(reader.ReadLine(), out double rightBoundary))
                        {
                            continue;
                        }

                        testCase.RightBoundary = rightBoundary;

                        if (!double.TryParse(reader.ReadLine(), out double integrationStep))
                        {
                            continue;
                        }

                        testCase.IntegrationStep = integrationStep;

                        if (!Enum.TryParse(reader.ReadLine(), out IntegrationMehtod methodInt))
                        {
                            continue;
                        }

                        testCase.Method = (IntegrationMehtod)methodInt;
                        var coefficientsLine = reader.ReadLine();
                        testCase.Coefficients = coefficientsLine?.Split(',')
                            .Select(s => double.TryParse(s, out var coef) ? coef : double.NaN)
                            .Where(d => !double.IsNaN(d))
                            .ToList() ?? new List<double>();


                        testCases.Add(testCase);


                        reader.ReadLine();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при импорте! ");
            }

            return testCases;
        }
        return new List<TestCase>();
    }
    
}
