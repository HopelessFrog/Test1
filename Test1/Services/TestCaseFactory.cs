using Test1.Consts;
using Test1.Models;

namespace Test1.Services;

public class TestCaseFactory
{
    private Random _random = new Random();

    public async Task<TestCase> CreateTestCase(int degree, double leftBoundary, double rightBoundary,
        double integrationStep, IntegrationMehtod integrationMehtod)
    {
        var coefs = new List<double>();
        for (int i = 0; i < degree; i++)
        {
            coefs.Add(_random.NextDouble());
        }

        return new TestCase()
        {
            LeftBoundary = leftBoundary,
            RightBoundary = rightBoundary,
            IntegrationStep = integrationStep,
            Method = integrationMehtod,
            Coefficients = coefs
        };
    }

    public async Task<List<TestCase>> CreateTestCases(int step,int minDegree, int maxDegree, double leftBoundary,
        double rightBoundary, double integrationStep, IntegrationMehtod integrationMehtod)
    {
        var testCases = new List<TestCase>();
        for (int i = minDegree; i <= maxDegree; i+= step)
        {
            testCases.Add(await CreateTestCase(i, leftBoundary, rightBoundary, integrationStep, integrationMehtod));
        }
        return testCases;
    }
}