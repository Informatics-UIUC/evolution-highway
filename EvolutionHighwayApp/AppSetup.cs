using System;
using System.Diagnostics;
using Castle.MicroKernel.Registration;
using EvolutionHighwayApp.Converters;
using EvolutionHighwayApp.Events;
using EvolutionHighwayApp.Infrastructure;
using EvolutionHighwayApp.Infrastructure.EventBus;
using EvolutionHighwayApp.State;
using EvolutionHighwayApp.ViewModels;
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
                .Subscribe(e => ScaleConverter.DataMaximum = default(double));

            CompGenomeNameFormatToStringConverter.NameFormat = appSettings.CompGenomeNameFormat;
            eventPublisher.GetEvent<CompGenomeNameFormatChangedEvent>()
                .Subscribe(e => CompGenomeNameFormatToStringConverter.NameFormat = e.CompGenomeNameFormat);
        }

        private static void ContainerSetup()
        {
            IoC.Container.Register(
                Component.For<AppSettings>().LifeStyle.Singleton,
                Component.For<IEventPublisher>().ImplementedBy<EventAggregator>().LifeStyle.Singleton,
                Component.For<Repository>().LifeStyle.Singleton,
                Component.For<SelectionsController>().LifeStyle.Singleton,
                Component.For<MenuViewModel>().LifeStyle.Transient,
                Component.For<ToolbarViewModel>().LifeStyle.Transient,
                Component.For<DataSourceSelectorViewModel>().LifeStyle.Transient,
                Component.For<RefGenomeSelectorViewModel>().LifeStyle.Transient,
                Component.For<RefChromosomeSelectorViewModel>().LifeStyle.Transient,
                Component.For<CompGenomeSelectorViewModel>().LifeStyle.Transient,
                Component.For<RefGenomeCollectionViewModel>().LifeStyle.Transient,
                Component.For<RefChromosomeCollectionViewModel>().LifeStyle.Transient,
                Component.For<CompGenomeCollectionViewModel>().LifeStyle.Transient
            );
        }

        private static void ImageEncoderSetup()
        {
            Encoders.AddEncoder<PngEncoder>();
            Encoders.AddEncoder<JpegEncoder>();
        }
    }
}
