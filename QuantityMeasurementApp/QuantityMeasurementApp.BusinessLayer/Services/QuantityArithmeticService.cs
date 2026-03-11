using QuantityMeasurementApp.BusinessLayer.Factories;
using QuantityMeasurementApp.ModelLayer.Entity;
using QuantityMeasurementApp.ModelLayer.Enums;
using System;
using System.Collections.Generic;
using System.Runtime.Intrinsics.Arm;
using System.Text;

namespace QuantityMeasurementApp.BusinessLayer.Services
{
    public interface IQuantityArithmeticService
    {
        Quantity<T> AddUnit<T>(Quantity<T> q1, Quantity<T> q2) where T : struct, Enum;
        Quantity<T> AddToSpecificUnit<T>(Quantity<T> q1, Quantity<T> q2, T targetUnit) where T : struct, Enum;
        Quantity<T> SubtractUnit<T>(Quantity<T> q1, Quantity<T> q2, T targetUnit) where T : struct, Enum;
        double DivideUnit<T>(Quantity<T> q1, Quantity<T> q2) where T : struct, Enum;
    }

    public class QuantityArithmeticService : IQuantityArithmeticService
    {
        private readonly UnitAdapterFactory _adapterFactory;
        private readonly QuantityValidationService _validator;

        public QuantityArithmeticService(
            UnitAdapterFactory adapterFactory,
            QuantityValidationService validator)
        {
            _adapterFactory = adapterFactory;
            _validator = validator;
        }

        private double PerformBaseOperation<U> (
            Quantity<U> q1,
            Quantity<U> q2,
            ArithmeticOperation op) where U : struct, Enum
        {
            var adapter1 = _adapterFactory.CreateAdapter(q1.Unit);
            var adapter2 = _adapterFactory.CreateAdapter(q2.Unit);

            double aBase = adapter1.ConvertToBaseUnit(q1.Value);
            double bBase = adapter2.ConvertToBaseUnit(q2.Value);

            return op switch
            {
                ArithmeticOperation.Add => aBase + bBase,
                ArithmeticOperation.Subtract => aBase - bBase,
                ArithmeticOperation.Divide => bBase == 0.0
                    ? throw new DivideByZeroException("Cannot divide by zero quantity.")
                    : aBase / bBase,
                _ => throw new ArgumentOutOfRangeException(nameof(op), "Unsupported arithmetic operation.")
            };
        }

        public Quantity<T> AddUnit<T>(Quantity<T> q1, Quantity<T> q2) where T : struct, Enum
        {
            _validator.ValidateNotNull(q1, nameof(q1));
            _validator.ValidateNotNull(q2, nameof(q2));
            _validator.ValidateSameCategory(q1, q2);

            double baseResult = PerformBaseOperation(q1, q2, ArithmeticOperation.Add);

            var targetAdapter = _adapterFactory.CreateAdapter(q1.Unit);
            double valueInTarget = targetAdapter.ConvertFromBaseUnit(baseResult);

            return new Quantity<T>(valueInTarget, q1.Unit);
        }

        public Quantity<T> AddToSpecificUnit<T>(Quantity<T> q1,Quantity<T> q2,T targetUnit) where T : struct, Enum
        {
            _validator.ValidateNotNull(q1, nameof(q1));
            _validator.ValidateNotNull(q2, nameof(q2));
            _validator.ValidateEnumUnit(targetUnit, nameof(targetUnit));
            _validator.ValidateSameCategory(q1, q2);
            double baseResult = PerformBaseOperation(q1, q2, ArithmeticOperation.Add);

            var targetAdapter = _adapterFactory.CreateAdapter(targetUnit);
            double valueInTarget = targetAdapter.ConvertFromBaseUnit(baseResult);

            return new Quantity<T>(valueInTarget, targetUnit);
        }

        public Quantity<T> SubtractUnit<T>(Quantity<T> q1,Quantity<T> q2,T targetUnit) where T : struct, Enum
        {
            _validator.ValidateNotNull(q1, nameof(q1));
            _validator.ValidateNotNull(q2, nameof(q2));
            _validator.ValidateEnumUnit(targetUnit, nameof(targetUnit));
            _validator.ValidateSameCategory(q1, q2);

            double baseResult = PerformBaseOperation(q1, q2, ArithmeticOperation.Subtract);

            var targetAdapter = _adapterFactory.CreateAdapter(targetUnit);
            double valueInTarget = targetAdapter.ConvertFromBaseUnit(baseResult);

            return new Quantity<T>(valueInTarget, targetUnit);
        }

        public double DivideUnit<T>(Quantity<T> q1, Quantity<T> q2) where T : struct, Enum
        {
            _validator.ValidateNotNull(q1, nameof(q1));
            _validator.ValidateNotNull(q2, nameof(q2));
            _validator.ValidateSameCategory(q1, q2);

            if (Math.Abs(q2.Value) < 1e-10)
                throw new DivideByZeroException("Cannot divide by zero quantity.");

            double baseResult = PerformBaseOperation(q1, q2, ArithmeticOperation.Divide);
            return baseResult;
        }
    }
}
