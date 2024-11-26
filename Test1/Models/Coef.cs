using CommunityToolkit.Mvvm.ComponentModel;

namespace Test1.Models
{
    public partial class Coef : ObservableObject
    {
        [ObservableProperty]

        private int number;

        [ObservableProperty]
        private double value;
    }
}
