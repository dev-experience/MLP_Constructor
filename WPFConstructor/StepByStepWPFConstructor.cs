using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WPFConstructor;
using ConstructorsDictionary = System.Collections.Generic
    .Dictionary<WPFConstructor
        .StepByStepToken, WPFConstructor
        .StepByStepWPFConstructor>;
namespace WPFConstructor
{
    public class StepByStepWPFConstructor
    {
        private readonly Grid targetContainer;
        private readonly StepByStepToken token;
        private readonly Constructor constructor;
        private static ConstructorsDictionary stepByStepConstructors =
            new ConstructorsDictionary();
        private Menu menu;
        private Panel footer;
        private Panel content;
        private RoutedEventHandler OnFileOpen;
        private RoutedEventHandler OnFileSave;
        /// <summary>
        /// Устанавливает новые габариты целевого представления
        /// </summary>
        /// <param name="menu">Размер блока верхнего меню</param>
        /// <param name="content">Размер блока содержимого</param>
        /// <param name="footer">Размер нижнего блока кнопок</param>
        public void Resize(RowDefinition menu,
            RowDefinition content,
            RowDefinition footer)
        {
            targetContainer.RowDefinitions.Clear();
            targetContainer.RowDefinitions.Add(menu ?? new RowDefinition());
            targetContainer.RowDefinitions.Add(content ?? new RowDefinition());
            targetContainer.RowDefinitions.Add(footer ?? new RowDefinition());
        }
        public void SetOnOpen(RoutedEventHandler onOpen)
        {
            OnFileOpen = onOpen;
        }
        public void SetOnSave(RoutedEventHandler onSave)
        {
            OnFileSave = onSave;
        }
        private void OnFileOpenClick(object sender, RoutedEventArgs e)
        {
            OnFileOpen?.Invoke(sender, e);
        }
        private void OnFileSaveClick(object sender, RoutedEventArgs e)
        {
            OnFileSave?.Invoke(sender, e);
        }

        /// <summary>
        /// Создает новый конструктор и отрисовывает его на целевом представлении. 
        /// </summary>
        /// <param name="targetContainer">Контейнер для отрисовки</param>
        /// <param name="stagesProvider">Поставщик этапов и шагов</param>
        /// <param name="dataContext">Данные, над которыми производятся манипуляции в конструкторе</param>
        /// <returns>Ассоциированный с этим конструктором токен, в котором содержатся данные</returns>
        public static StepByStepToken New(Grid targetContainer,
            StagesFactory stagesProvider, object dataContext = null)
        {
            var token = stagesProvider.Token;
            token.DataContext = dataContext;
            stepByStepConstructors.Add(token,
                new StepByStepWPFConstructor(targetContainer, stagesProvider, token));
            return token;
        }
        private StepByStepWPFConstructor(Grid targetContainer, StagesFactory stagesProvider, StepByStepToken token)
        {
            this.targetContainer = targetContainer;
            Resize(
                new RowDefinition() { Height = new GridLength(20, GridUnitType.Pixel) },
                new RowDefinition() { Height = new GridLength(5, GridUnitType.Star) },
                new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) }
                );
            this.token = token;
            constructor = new Constructor(stagesProvider, token);
            constructor.ContentChanged += OnEntryContentChanged;
            constructor.FooterChanged += OnFooterChanged;
            constructor.MenuChanged += OnMenuChanged;
            constructor.OnOpenFile += OnFileOpenClick;
            constructor.OnSaveFile += OnFileSaveClick;
            SetOnOpen((x, y) => SetShowContent(true));
            SetOnSave((x, y) => SetShowContent(false));
            constructor.Load();
        }
        public void Update()
        {

            constructor.Update();
        }
        public void Recheck()
        {
            constructor.Recheck();
        }
        public void SetShowContent(bool value)
        {
            constructor.ContentEnabled = value;
            constructor.Load();
        }
        private void RedrawMenu(Menu menu)
        {
            if (this.menu != null)
            {
                targetContainer.Children.Remove(this.menu);
            }
            Grid.SetRow(menu, 0);
            ReAddElement(menu);
            this.menu = menu;
        }
        private void RedrawFooter(Panel footer)
        {
            if (this.footer != null)
            {
                targetContainer.Children.Remove(this.footer);
            }
            Grid.SetRow(footer, 2);
            ReAddElement(footer);
            this.footer = footer;
        }
        private void RedrawContent(Panel content)
        {

            if (this.content != null)
            {
                targetContainer.Children.Remove(this.content);
            }
            Grid.SetRow(content, 1);
            ReAddElement(content);
            this.content = content;
        }
        private void ReAddElement(UIElement element)
        {
            if (targetContainer.Children.Contains(element))
            {
                targetContainer.Children.Remove(element);
            }
            targetContainer.Children.Add(element);
        }
        private void OnMenuChanged(object sender, UIElement args)
        {
            RedrawMenu(args as Menu);
        }

        private void OnFooterChanged(object sender, UIElement args)
        {
            RedrawFooter(args as Panel);
        }

        private void OnEntryContentChanged(object sender, UIElement args)
        {
            RedrawContent(args as Panel);
        }
    }
}

