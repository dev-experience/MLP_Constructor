using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ConstructorsDictionary = System.Collections.Generic
    .Dictionary<WPFConstructor
        .StepByStepToken, WPFConstructor
        .StepByStepWPFConstructor>;
namespace WPFConstructor
{
    public class StepByStepToken
    {
        private static ConstructorsDictionary stepByStepConstructors
            = new ConstructorsDictionary();
        static StepByStepToken()
        {
            stepByStepConstructors = typeof(StepByStepWPFConstructor)
                .GetField("stepByStepConstructors",
                BindingFlags.NonPublic | BindingFlags.Static)
                .GetValue(null) as ConstructorsDictionary;
        }
        private StepByStepWPFConstructor FindAssociatedConstructor()
        {
            return stepByStepConstructors[this];
        }
        /// <summary>
        /// Позволяет модифицировать конструктор, ассоциированный с этим токеном
        /// </summary>
        /// <param name="action">Действия, выполняемые над конструктором</param>
        /// <returns></returns>
        public StepByStepToken InteractWithConstructor(Action<StepByStepWPFConstructor> action)
        {
            action(FindAssociatedConstructor());
            return this;
        }
        /// <summary>
        /// Позволяет взаимодействовать с данными
        /// </summary>
        /// <typeparam name="T">Тип данных, помещенных в токен</typeparam>
        /// <param name="interaction">Действия, выполняемые над данными</param>
        /// <exception cref="InvalidCastException">Если тип данных не совпадает с переданным типом T</exception>
        public void InteractWithDataContext<T>(Action<T> interaction)
        {
            var data = (T)DataContext;
            interaction(data);
            DataContext = data;
        }

        /// <summary>
        /// Позволяет взаимодействовать с данным
        /// </summary>
        /// <typeparam name="TData">Тип данных, помещенных в токен</typeparam>
        /// <typeparam name="TOutput">Тип возвращаемого значения переданной функции</typeparam>
        /// <param name="interaction">Действия, выполняемые над данными</param>
        /// <returns>Результат выполнения переданной функции</returns>
        public TOutput InteractWithDataContext<TData, TOutput>(Func<TData, TOutput> interaction)
        {
            var data = (TData)DataContext;
            var output = interaction(data);
            DataContext = data;
            return output;

        }
        /// <summary>
        /// Получает данные, приведенные к типу Т
        /// </summary>
        /// <typeparam name="T">Тип переданных в токен данных</typeparam>
        /// <returns>Данные, приведенные к типу Т</returns>
        public T GetContext<T>()
        {
            return (T)DataContext;
        }
        private Guid wpfConstructorID;
        public static StepByStepToken New => new StepByStepToken();
        private StepByStepToken()
        {
            wpfConstructorID = Guid.NewGuid();
        }
        /// <summary>
        /// Данные, над которыми производятся манипуляции в конструкторе
        /// </summary>
        public object DataContext { get; set; }
        public override int GetHashCode()
        {
            return wpfConstructorID.GetHashCode();
        }
    }
}
