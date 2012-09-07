using System;
using System.Diagnostics;
using Castle.MicroKernel.Registration;
using EvolutionHighwayApp.Converters;
using EvolutionHighwayApp.Display.Controllers;
using EvolutionHighwayApp.Events;
using EvolutionHighwayApp.Infrastructure;
using EvolutionHighwayApp.Infrastructure.EventBus;
using EvolutionHighwayApp.Repository.Controllers;
using EvolutionHighwayApp.Selection.Controllers;
using ImageTools.IO;
using ImageTools.IO.Jpeg;
using ImageTools.IO.Png;

namespace EvolutionHighwayApp
{
    public static class AppSetup
    {
        public static void Setup()
        {
            Debug.WriteLine("Performing app setup");
            ContainerSetup();
            ImageEncoderSetup();

            var appSettings = IoC.Container.Resolve<AppSettings>();
            var eventPublisher = IoC.Container.Resolve<IEventPublisher>();

            ScaleConverter.DisplayMaximum = appSettings.DisplaySize;
            eventPublisher.GetEvent<DisplaySizeChangedEvent>()
                .Subscribe(e => ScaleConverter.DisplayMaximum = e.DisplaySize);

            eventPublisher.GetEvent<DataSourceChangedEvent>()
                .Subscribe(e => ScaleConverter.DataMaximum = null);

            CompGenomeNameFormatToStringConverter.NameFormat = appSettings.CompGenomeNameFormat;
            eventPublisher.GetEvent<CompGenomeNameFormatChangedEvent>()
                .Subscribe(e => CompGenomeNameFormatToStringConverter.NameFormat = e.CompGenomeNameFormat);
        }

        private static void ContainerSetup()
        {
            IoC.Container.Register(
                Component.For<IEventPublisher>().ImplementedBy<EventAggregator>().LifeStyle.Singleton,
                Component.For<IRepositoryController>().ImplementedBy<RepositoryController>().LifeStyle.Singleton,
                Component.For<ISelectionController>().ImplementedBy<SelectionController>().LifeStyle.Singleton,
                Component.For<IDisplayController>().ImplementedBy<DisplayController>().LifeStyle.Singleton,
                Component.For<AppSettings>().LifeStyle.Singleton
            );
        }

        private static void ImageEncoderSetup()
        {
            Encoders.AddEncoder<PngEncoder>();
            Encoders.AddEncoder<JpegEncoder>();
        }
    }
}
