using TestApp;

namespace ImGalaxy.ES.TestApp.Domain.Cars
{
    public class ModelYearCannotBeAboveThan
    {
        public int ModelYear { get; }
        public ModelYearCannotBeAboveThan(int modelYear)
        {
            ModelYear = modelYear;
        }

    }
}
