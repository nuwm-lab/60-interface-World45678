using System;

namespace AbstractInterfaceLab
{
    /// <summary>
    /// ✅ 1. ІНТЕРФЕЙС
    /// Визначає контракт: будь-який дріб повинен вміти встановлювати параметри,
    /// показувати себе і обчислювати значення.
    /// </summary>
    public interface IFraction
    {
        void SetCoefficients();
        void DisplayInfo();
        double Calculate(double x);
    }

    /// <summary>
    /// ✅ 2. АБСТРАКТНИЙ КЛАС
    /// Реалізує інтерфейс і додає загальну поведінку (конструктори/деструктори).
    /// </summary>
    public abstract class BaseFraction : IFraction
    {
        /// <summary>
        /// Точність для порівняння чисел (доступна спадкоємцям).
        /// </summary>
        protected const double Epsilon = 1e-12;

        // Конструктор базового класу
        public BaseFraction()
        {
            Console.WriteLine("-> Викликано конструктор абстрактного класу BaseFraction");
        }

        // Деструктор (Finalizer)
        ~BaseFraction()
        {
            Console.WriteLine("-> Викликано деструктор абстрактного класу BaseFraction");
        }

        // Абстрактні методи, які зобов'язані реалізувати спадкоємці
        public abstract void SetCoefficients();
        public abstract void DisplayInfo();
        public abstract double Calculate(double x);
    }

    /// <summary>
    /// ✅ Клас простого дробу (1 / (a*x)).
    /// Успадковується від BaseFraction.
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

        // Конструктор
        public SimpleFraction() : base() // Виклик базового конструктора
        {
            _coefficientA = 1.0;
            Console.WriteLine("-> Створено об'єкт SimpleFraction");
        }

        // Деструктор
        ~SimpleFraction()
        {
            Console.WriteLine("-> Видалено об'єкт SimpleFraction");
        }

        public override void SetCoefficients()
        {
            Console.WriteLine("--- Налаштування простого дробу ---");
            Console.Write("Введіть коефіцієнт 'a': ");
            double value;
            while (true)
            {
                if (double.TryParse(Console.ReadLine(), out value))
                {
                    try
                    {
                        CoefficientA = value;
                        break;
                    }
                    catch (ArgumentException ex)
                    {
                        Console.WriteLine($"Помилка: {ex.Message} Ще раз:");
                    }
                }
                else
                {
                    Console.WriteLine("Помилка. Введіть число:");
                }
            }
        }

        public override void DisplayInfo()
        {
            Console.WriteLine("\n[Тип: Простий дріб]");
            Console.WriteLine($"Формула: 1 / ({CoefficientA} * x)");
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
    /// ✅ Клас ланцюгового дробу.
    /// Успадковується від BaseFraction.
    /// </summary>
    public class ContinuedFraction : BaseFraction
    {
        private double _a1, _a2, _a3;

        // Конструктор
        public ContinuedFraction() : base()
        {
            Console.WriteLine("-> Створено об'єкт ContinuedFraction");
        }

        // Деструктор
        ~ContinuedFraction()
        {
            Console.WriteLine("-> Видалено об'єкт ContinuedFraction");
        }

        // Допоміжний метод (внутрішня логіка класу)
        private double ReadCoefficient(string name)
        {
            double value;
            while (true)
            {
                Console.Write($"Введіть коефіцієнт '{name}' (не 3): ");
                if (double.TryParse(Console.ReadLine(), out value))
                {
                    if (Math.Abs(value - 3.0) < Epsilon)
                        Console.WriteLine("Помилка: не може дорівнювати 3.");
                    else
                        return value;
                }
                else
                {
                    Console.WriteLine("Помилка. Введіть число.");
                }
            }
        }

        public override void SetCoefficients()
        {
            Console.WriteLine("\n--- Налаштування ланцюгового дробу ---");
            _a1 = ReadCoefficient("a1");
            _a2 = ReadCoefficient("a2");
            _a3 = ReadCoefficient("a3");
        }

        public override void DisplayInfo()
        {
            Console.WriteLine("\n[Тип: Ланцюговий дріб]");
            Console.WriteLine($"Коефіцієнти: a1={_a1}, a2={_a2}, a3={_a3}");
        }

        public override double Calculate(double x)
        {
            double inner = _a3 * x;
            if (Math.Abs(inner) < Epsilon) throw new DivideByZeroException("Внутрішній знаменник = 0");

            double middle = _a2 * x + (1.0 / inner);
            if (Math.Abs(middle) < Epsilon) throw new DivideByZeroException("Середній знаменник = 0");

            double outer = _a1 * x + (1.0 / middle);
            if (Math.Abs(outer) < Epsilon) throw new DivideByZeroException("Зовнішній знаменник = 0");

            return 1.0 / outer;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                // Використовуємо блок scope {}, щоб продемонструвати роботу деструкторів,
                // коли змінна виходить з області видимості.
                {
                    Console.WriteLine("\n--- Лабораторна: Абстрактні класи та Інтерфейси ---");
                    Console.WriteLine("1. Простий дріб");
                    Console.WriteLine("2. Ланцюговий дріб");
                    Console.WriteLine("0. Вихід");
                    Console.Write("Вибір: ");
                    string choice = Console.ReadLine();

                    if (choice == "0") break;

                    // !!! ДЕМОНСТРАЦІЯ РОБОТИ ІНТЕРФЕЙСУ !!!
                    // Ми створюємо змінну типу інтерфейсу IFraction.
                    // Вона може зберігати посилання на будь-який клас, що реалізує цей інтерфейс.
                    IFraction myFraction = null;

                    if (choice == "1")
                    {
                        myFraction = new SimpleFraction();
                    }
                    else if (choice == "2")
                    {
                        myFraction = new ContinuedFraction();
                    }
                    else
                    {
                        Console.WriteLine("Невірний вибір.");
                        continue;
                    }

                    try
                    {
                        // Виклик методів через інтерфейс
                        myFraction.SetCoefficients();
                        myFraction.DisplayInfo();

                        Console.Write("Введіть x: ");
                        if (double.TryParse(Console.ReadLine(), out double x))
                        {
                            double result = myFraction.Calculate(x);
                            Console.WriteLine($"✅ Результат: {result:F4}");
                        }
                        else
                        {
                            Console.WriteLine("Некоректний x.");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"❌ Помилка: {ex.Message}");
                    }

                    // Тут myFraction виходить з області видимості блоку, 
                    // але Garbage Collector спрацює не відразу.
                }

                // Примусовий виклик збирача сміття для демонстрації роботи Деструкторів (тільки для навчання!)
                GC.Collect();
                GC.WaitForPendingFinalizers();
                
                Console.WriteLine("\nНатисніть Enter для продовження...");
                Console.ReadLine();
            }
        }
    }
}
