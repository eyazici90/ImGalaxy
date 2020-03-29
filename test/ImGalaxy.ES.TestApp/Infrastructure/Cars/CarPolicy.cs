using ImGalaxy.ES.Core;
using ImGalaxy.ES.TestApp.Domain.Cars;
using System;

namespace ImGalaxy.ES.TestApp.Infrastructure.Cars
{
    public class CarPolicy : ICarPolicy
    {
        public IExecutionResult Apply(ModelYearCannotBeAboveThan policy) 
        {
            policy.ThrowsIf(p => p.ModelYear > 2019, new Exception("model cannot be above than 2019"));

            return ExecutionResult.Success;
        }

        public IExecutionResult Apply(CannotBeAddedMoreThanTwoItems policy)
        {
            policy.ThrowsIf(_ => policy.Car.CarItems.Count == 2, new Exception("You cannot add more than 2 items to single car"));
            return ExecutionResult.Success;
        }
    }
}
