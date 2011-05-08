using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace EvolutionHighwayApp.Infrastructure.MVP
{
    public interface IView : IDisposable
    {
        void Hide();
        void Show();
    }

    public class View : UserControl, IView
    {
        private dynamic _presenter;

        private static readonly bool InDesignMode =
            (null == Application.Current) || Application.Current.GetType() == typeof(Application);

        protected TPresenter CreateAndInitializePresenter<TPresenter>()
        {
            if (InDesignMode)
            {
                return default(TPresenter);
            }

            _presenter = IoC.Container.Resolve<TPresenter>(new Dictionary<string, object> { { "view", this } });
            _presenter.Initialize();
            DataContext = _presenter.BindingModel;

            return _presenter;
        }

        public void Hide()
        {
            Visibility = Visibility.Collapsed;
        }

        public void Show()
        {
            Visibility = Visibility.Visible;
        }

        public void Dispose()
        {
            // naive implementation of Dispose, but then again, this is just a sample
            IoC.Container.Release(_presenter);
            CleanUpChildrenOf(this);
        }

        private static void CleanUpChildrenOf(object obj)
        {
            var dependencyObject = obj as DependencyObject;

            if (dependencyObject != null)
            {
                var count = VisualTreeHelper.GetChildrenCount(dependencyObject);

                for (int i = 0; i < count; i++)
                {
                    var child = VisualTreeHelper.GetChild(dependencyObject, i);

                    if (child != null)
                    {
                        DisposeIfDisposable(child);
                        StopIfProgressBar(child);
                        CleanUpChildrenOf(child);
                    }
                }
            }
        }

        private static void DisposeIfDisposable(DependencyObject child)
        {
            var disposable = child as IDisposable;
            if (disposable != null) disposable.Dispose();
        }

        private static void StopIfProgressBar(DependencyObject child)
        {
            var progressBar = child as ProgressBar;
            if (progressBar != null) progressBar.IsIndeterminate = false;
        }
    }
}
