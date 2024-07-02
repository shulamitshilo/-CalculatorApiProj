using System;

namespace CalculatorApi.Services
{
    public class CalculationService
    {
        public decimal PerformCalculation(decimal number1, decimal number2, string operation)
        {
            try
            {
                switch (operation.ToLower())
                {
                    case "add":
                        return checked(number1 + number2);
                    case "subtract":
                        return checked(number1 - number2);
                    case "multiply":
                        return checked(number1 * number2);
                    case "divide":
                        if (number2 == 0)
                            throw new InvalidOperationException("Division by zero is not allowed.");
                        return checked(number1 / number2);
                    default:
                        throw new InvalidOperationException("Invalid operation specified.");
                }
            }
            catch (OverflowException)
            {
                throw new InvalidOperationException("One or both numbers are out of valid range.");
            }
        }
    }

}
