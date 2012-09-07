using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Xml.Linq;
using System.Xml.XPath;
using EvolutionHighwayApp.Events;
using EvolutionHighwayApp.Infrastructure;
using EvolutionHighwayApp.Infrastructure.EventBus;
using EvolutionHighwayApp.Infrastructure.MVVM;

namespace EvolutionHighwayApp.Selection.ViewModels
{
    public class DataSourceSelectorViewModel : ViewModelBase
    {
        #region ViewModel Bindable Properties

        private IEnumerable<KeyValuePair<string, string>> _dataSources;
        public IEnumerable<KeyValuePair<string, string>> DataSources
        {
            get { return _dataSources; }
            set { NotifyPropertyChanged(() => DataSources, ref _dataSources, value); }
        }

        private bool _isEnabled;
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { NotifyPropertyChanged(() => IsEnabled, ref _isEnabled, value); }
        }

        #endregion

        private readonly IEventPublisher _eventPublisher; 
        private readonly IDisposable _loadingObserver;

        public DataSourceSelectorViewModel()
        {
            _eventPublisher = IoC.Container.Resolve<IEventPublisher>();

            if (!Application.Current.Resources.Contains("dataSourceConfigUrl"))
            {
                MessageBox.Show(
                    "dataSourceConfigUrl not specified. Please make sure your HTML includes a Silverlight 'initParams' " +
                    "parameter that sets 'dataSourceConfigUrl' to point to the URL of the data sources config file.");
                return;
            }

            var dataSourcesSrc = Application.Current.Resources["dataSourceConfigUrl"].ToString();
            var webClient = new WebClient();
            webClient.OpenReadCompleted += delegate(object o, OpenReadCompletedEventArgs ea)
            {
                if (ea.Error != null)
                {
                    MessageBox.Show(string.Format("Cannot load {0}:\n{1}", dataSourcesSrc, ea.Error.Message));
                    return;
                }

                using (var s = ea.Result)
                {
                    var docDataSources = XDocument.Load(s);
                    var xmlDataSources = docDataSources.XPathSelectElements("//datasource");

                    // ReSharper disable PossibleNullReferenceException
                    DataSources = xmlDataSources
                        .Where(ds => ds.Attribute("name") != null && ds.Attribute("serviceAddress") != null)
                        .Select(ds => new KeyValuePair<string, string>(
                                ds.Attribute("name").Value,
                                ds.Attribute("serviceAddress").Value));
                    // ReSharper restore PossibleNullReferenceException
                }
            };

            webClient.OpenReadAsync(new Uri(dataSourcesSrc, UriKind.RelativeOrAbsolute));

            _loadingObserver = _eventPublisher.GetEvent<LoadingEvent>()
                .ObserveOnDispatcher()
                .Subscribe(e => IsEnabled = e.IsDoneLoading);

            IsEnabled = true;
        }

        public void DataSourceSelectionChanged(string dataSourceUrl)
        {
            _eventPublisher.Publish(new DataSourceChangedEvent { DataSourceUrl = dataSourceUrl });
        }

        public override void Dispose()
        {
            base.Dispose();

            _loadingObserver.Dispose();
            _dataSources = null;
        }
    }
}
