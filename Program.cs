using System;

namespace AbstractInterfaceLab
{
    /// <summary>
    /// Інтерфейс, що визначає контракт для роботи з дробами.
    /// </summary>
    public interface IFraction : IDisposable
    {
        /// <summary>
        /// Обчислює значення функції в точці x.
        /// </summary>
        double Calculate(double x);

        /// <summary>
        /// Виводить інформацію про дріб (формулу та коефіцієнти).
        /// </summary>
        void DisplayInfo();
    }

    /// <summary>
    /// Абстрактний базовий клас.
    /// Реалізує загальну логіку та патерн Dispose.
    /// </summary>
    public abstract class BaseFraction : IFraction
    {
        // Static readonly краще ніж const, якщо ми захочемо змінити це в майбутньому без перекоміляції клієнтів
        protected static readonly double Epsilon = 1e-12;
        private bool _disposed = false;

        protected BaseFraction()
        {
            // Конструктор чистий, без виводу в консоль
        }

        // Деструктор (фіналайзер) - залишено для виконання вимог методички,
        // але в реальному C# коді він рідко потрібен для керованих ресурсів.
        ~BaseFraction()
        {
            Dispose(false);
        }

        public abstract double Calculate(double x);
        public abstract void DisplayInfo();

        // Реалізація IDisposable
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Тут звільняємо керовані ресурси (якщо є)
                }
                // Тут звільняємо некеровані ресурси (якщо є)
                _disposed = true;
            }
        }
    }

    /// <summary>
    /// Клас простого дробу виду 1 / (a * x).
    /// </summary>
    public class SimpleFraction : BaseFraction
    {
        private double _coefficientA;

        public double CoefficientA
        {
            get => _coefficientA;
            set
            {
                if (Math.Abs(value) < Epsilon)
                    throw new ArgumentException("Коефіцієнт 'a' не може дорівнювати нулю.");
                _coefficientA = value;
            }
        }

        public SimpleFraction(double a)
        {
            CoefficientA = a; // Валідація спрацює через сетер
        }

        public override void DisplayInfo()
        {
            Console.WriteLine($"[Простий дріб] Формула: 1 / ({CoefficientA} * x)");
        }

        public override double Calculate(double x)
        {
            double denominator = CoefficientA * x;
            if (Math.Abs(denominator) < Epsilon)
                throw new DivideByZeroException("Знаменник (a*x) дорівнює нулю.");
            
            return 1.0 / denominator;
        }
    }

    /// <summary>
    /// Клас ланцюгового дробу.
    /// </summary>
    public class ContinuedFraction : BaseFraction
    {
        // Приватні поля
        private double _a1, _a2, _a3;

        // Публічні властивості тільки для читання (або з приватним сетом),
        // щоб гарантувати незмінність після створення (immutability), або контрольовану зміну.
        public double A1 
        { 
            get => _a1; 
            private set => ValidateAndSet(ref _a1, value, nameof(A1)); 
        }
        
        public double A2 
        { 
            get => _a2; 
            private set => ValidateAndSet(ref _a2, value, nameof(A2)); 
        }

        public double A3 
        { 
            get => _a3; 
            private set => ValidateAndSet(ref _a3, value, nameof(A3)); 
        }

        public ContinuedFraction(double a1, double a2, double a3)
        {
            A1 = a1;
            A2 = a2;
            A3 = a3;
        }

        // Допоміжний метод для валідації (DRY - Don't Repeat Yourself)
        private void ValidateAndSet(ref double field, double value, string paramName)
        {
            // Перевірка за умовою задачі: коефіцієнт не може дорівнювати 3
            if (Math.Abs(value - 3.0) < Epsilon)
                throw new ArgumentException($"Коефіцієнт {paramName} не може дорівнювати 3.");
            
            field = value;
        }

        public override void DisplayInfo()
        {
            Console.WriteLine($"[Ланцюговий дріб] Коефіцієнти: a1={A1}, a2={A2}, a3={A3}");
        }

        public override double Calculate(double x)
        {
            // Обчислення зсередини назовні
            double inner = A3 * x;
            if (Math.Abs(inner) < Epsilon) throw new DivideByZeroException("Внутрішній знаменник (A3*x) дорівнює 0.");

            double middleDenominator = A2 * x + (1.0 / inner);
            if (Math.Abs(middleDenominator) < Epsilon) throw new DivideByZeroException("Середній знаменник дорівнює 0.");

            double outerDenominator = A1 * x + (1.0 / middleDenominator);
            if (Math.Abs(outerDenominator) < Epsilon) throw new DivideByZeroException("Зовнішній знаменник дорівнює 0.");

            return 1.0 / outerDenominator;
        }
    }

    /// <summary>
    /// Статичний клас для взаємодії з користувачем (UI Logic).
    /// Відокремлює введення даних від бізнес-логіки.
    /// </summary>
    public static class InputHandler
    {
        public static double ReadDouble(string message, Func<double, bool> validator = null, string errorMessage = null)
        {
            while (true)
            {
                Console.Write(message);
                if (double.TryParse(Console.ReadLine(), out double result))
                {
                    if (validator != null && !validator(result))
                    {
                        Console.WriteLine($"Помилка: {errorMessage}");
                        continue;
                    }
                    return result;
                }
                Console.WriteLine("Некоректний формат числа. Спробуйте ще раз.");
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8; // Для коректного відображення кирилиці

            while (true)
            {
                Console.WriteLine("\n--- Лабораторна робота: Абстрактні класи та Інтерфейси ---");
                Console.WriteLine("1. Створити простий дріб");
                Console.WriteLine("2. Створити ланцюговий дріб");
                Console.WriteLine("0. Вихід");
                
                string choice = Console.ReadLine();
                if (choice == "0") break;

                IFraction fraction = null;

                try
                {
                    if (choice == "1")
                    {
                        // Логіка введення винесена з класу
                        double a = InputHandler.ReadDouble(
                            "Введіть коефіцієнт a (не 0): ", 
                            val => Math.Abs(val) > 1e-12, 
                            "Коефіцієнт не може бути нулем."
                        );
                        fraction = new SimpleFraction(a);
                    }
                    else if (choice == "2")
                    {
                        Console.WriteLine("Введіть коефіцієнти (заборонено значення 3):");
                        // Лямбда-вираз для перевірки умови != 3
                        Func<double, bool> notThree = val => Math.Abs(val - 3.0) > 1e-12;
                        string errorMsg = "Коефіцієнт не може дорівнювати 3.";

                        double a1 = InputHandler.ReadDouble("a1: ", notThree, errorMsg);
                        double a2 = InputHandler.ReadDouble("a2: ", notThree, errorMsg);
                        double a3 = InputHandler.ReadDouble("a3: ", notThree, errorMsg);

                        fraction = new ContinuedFraction(a1, a2, a3);
                    }
                    else
                    {
                        Console.WriteLine("Невірний вибір.");
                        continue;
                    }

                    // Використання об'єкта
                    fraction.DisplayInfo();

                    double x = InputHandler.ReadDouble("Введіть значення x для обчислення: ");
                    double result = fraction.Calculate(x);
                    Console.WriteLine($"✅ Результат: {result:F4}");

                }
                catch (DivideByZeroException ex)
                {
                    Console.WriteLine($"❌ Помилка математики: {ex.Message}");
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine($"❌ Помилка аргументів: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Непередбачена помилка: {ex.Message}");
                }
                finally
                {
                    // Явний виклик Dispose завдяки блоку finally (або використати using)
                    fraction?.Dispose(); 
                }
            }
        }
    }
}
