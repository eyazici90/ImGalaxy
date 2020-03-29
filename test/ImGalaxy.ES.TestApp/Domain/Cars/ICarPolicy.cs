using ImGalaxy.ES.Core; 

namespace ImGalaxy.ES.TestApp.Domain.Cars
{
    public interface ICarPolicy :
        IPolicy<ModelYearCannotBeAboveThan>,
        IPolicy<CannotBeAddedMoreThanTwoItems>
    {
    }
}
