using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using Castle.MicroKernel.Registration;
using EvolutionHighwayApp.Converters;
using EvolutionHighwayApp.Events;
using EvolutionHighwayApp.Infrastructure;
using EvolutionHighwayApp.Infrastructure.EventBus;
using EvolutionHighwayApp.Models;
using EvolutionHighwayApp.Repository.Models;
using EvolutionHighwayApp.ServiceLayer;
using EvolutionHighwayApp.Utils;

namespace EvolutionHighwayApp.Repository.Controllers
{
    public class RepositoryController : IRepositoryController
    {
        private readonly IEventPublisher _eventPublisher;
        private readonly RepositoryState _repositoryState;
        private IEHDataService _dataService;

        public RepositoryController(IEventPublisher eventPublisher)
        {
            Debug.WriteLine("{0} instantiated", GetType().Name);

            _eventPublisher = eventPublisher;
            _repositoryState = new RepositoryState();

            eventPublisher.GetEvent<DataSourceChangedEvent>().Subscribe(e =>
            {
                if (!IoC.Container.Kernel.HasComponent(e.DataSourceUrl))
                {
                    Debug.WriteLine("Creating ServiceProxy for: {0}", e.DataSourceUrl);
                    var service = EHDataService.CreateServiceProxy(new EndpointAddress(e.DataSourceUrl));
                    IoC.Container.Register(Component.For<IEHDataService>().Instance(service).Named(e.DataSourceUrl).LifeStyle.Singleton);
                }

                _dataService = IoC.Container.Resolve<IEHDataService>(e.DataSourceUrl);

                // clear the memory cache
                _repositoryState.Clear();
            });
        }

        public Dictionary<string, SmartObservableCollection<CustomTrackRegion>> CustomTrackMap
        {
            get { return _repositoryState.CustomTrackMap; }
        }

        public void AddCustomTrackData(string trackData, char delimiter, bool append = false)
        {
            _repositoryState.AddCustomTrackData(trackData, delimiter, append);
        }

        public void ClearCustomTracks()
        {
            _repositoryState.ClearCustomTracks();
        }

        public void GetRefGenomes(Action<ActionCompletedEventArgs<List<RefGenome>>> successCallback, Action<ActionFailingEventArgs<object>> failureCallback = null, Action beforeLoadCallback = null)
        {
            Debug.WriteLine("GetRefGenomes()");

            #region 1. check if already loaded in memory cache
            if (_repositoryState.RefGenomes != null)
            {
                successCallback(new ActionCompletedEventArgs<List<RefGenome>>(_repositoryState.RefGenomes));
                return;
            } 
            #endregion
            
            // TODO: 2. check if exists in isolated storage cache

            #region 3. load from web service
            _eventPublisher.Publish(new RefGenomesLoadingEvent { IsDoneLoading = false });

            if (beforeLoadCallback != null)
                beforeLoadCallback();

            var worker = new BackgroundWorker { WorkerReportsProgress = false, WorkerSupportsCancellation = false };

            worker.DoWork += (s, ea) =>
            {
                Debug.WriteLine("LoadRefGenomes");
                bool retry;

                do
                {
                    retry = false;

                    var mre = new ManualResetEvent(false);
                    _dataService.BeginListRefGenomes(asyncResult =>
                    {
                        try
                        {
                            var genomes = _dataService.EndListRefGenomes(asyncResult).Select(g => new RefGenome(g.Name)).ToList();
                            _repositoryState.RefGenomes = genomes;
                            ea.Result = genomes;
                        }
                        catch (CommunicationException e)
                        {
                            UISynchronizationContext.Instance.InvokeSynchronously(
                                delegate
                                {
                                    var errorMsg = string.Format("{0}{2}{1}{2}{2}Press OK if you want to retry, or Cancel otherwise.", 
                                        "Error loading reference genomes", e.Message, Environment.NewLine);
                                    var mbResult = MessageBox.Show(errorMsg, "Error", MessageBoxButton.OKCancel);
                                    retry = mbResult == MessageBoxResult.OK;
                                });

                            if (!retry) ea.Cancel = true;
                        }
                        finally
                        {
                            ((ManualResetEvent)asyncResult.AsyncState).Set();
                        }
                    }, mre);

                    mre.WaitOne();
                } while (retry);
            };

            worker.RunWorkerCompleted += (s, ea) =>
            {
                _eventPublisher.Publish(new RefGenomesLoadingEvent { IsDoneLoading = true });
                if (!ea.Cancelled)
                    successCallback(new ActionCompletedEventArgs<List<RefGenome>>((List<RefGenome>)ea.Result));
            };

            worker.RunWorkerAsync();
            #endregion
        }

        public void GetRefChromosomes(ICollection<RefGenome> refGenomes, Action<ActionCompletedEventArgs<List<RefChromosome>>> successCallback, Action<ActionFailingEventArgs<RefGenome>> failureCallback = null, Action beforeLoadCallback = null)
        {
            Debug.WriteLine("GetRefChromosomes()");

            #region 1. check if already loaded in memory cache
            var genomesToLoad = refGenomes.Where(g => g.RefChromosomes == null).ToList();
            if (genomesToLoad.IsEmpty())
            {
                successCallback(new ActionCompletedEventArgs<List<RefChromosome>>(
                    refGenomes.SelectMany(g => g.RefChromosomes).ToList()));
                return;
            }
            #endregion

            // TODO: 2. check if exists in isolated storage cache

            #region 3. load from web service
            _eventPublisher.Publish(new RefChromosomesLoadingEvent { IsDoneLoading = false });

            if (beforeLoadCallback != null)
                beforeLoadCallback();

            var worker = new BackgroundWorker { WorkerReportsProgress = false, WorkerSupportsCancellation = false };

            worker.DoWork += (s, ea) =>
            {
                Debug.WriteLine("LoadRefChromosomes");
                bool retry;

                do
                {
                    retry = false;

                    var waitHandles = from genome in genomesToLoad.ToList()
                                      let mre = new ManualResetEvent(false)
                                      let completed = _dataService.BeginListRefChromosomes(genome.Name,
                                            asyncResult =>
                                            {
                                                try
                                                {
                                                    var chromosomes = _dataService.EndListRefChromosomes(asyncResult)
                                                        .Select(chromosome => new RefChromosome(chromosome.Name, chromosome.Length, genome))
                                                        .ToList();
                                                    genome.RefChromosomes = chromosomes;
                                                    genomesToLoad.Remove(genome);
                                                }
                                                catch (CommunicationException e)
                                                {
                                                    UISynchronizationContext.Instance.InvokeSynchronously(
                                                        delegate
                                                            {
                                                                var errorMsg = string.Format("{0}{2}{1}{2}{2}Press OK if you want to retry, or Cancel to skip.", 
                                                                    "Error loading chromosomes for " + genome, e.Message, Environment.NewLine);
                                                                var mbResult = MessageBox.Show(errorMsg, "Error", MessageBoxButton.OKCancel);
                                                                retry = mbResult == MessageBoxResult.OK;
                                                            });

                                                    if (!retry) genomesToLoad.Remove(genome);
                                                }
                                                finally
                                                {
                                                    ((ManualResetEvent)asyncResult.AsyncState).Set();
                                                }
                                            }, mre).IsCompleted
                                      where !completed
                                      select mre;

                    waitHandles.All(w => w.WaitOne());
                } while (!genomesToLoad.IsEmpty());

                ea.Result = refGenomes.Where(g => g.RefChromosomes != null).SelectMany(g => g.RefChromosomes).ToList();
            };

            worker.RunWorkerCompleted += (s, ea) =>
            {
                _eventPublisher.Publish(new RefChromosomesLoadingEvent { IsDoneLoading = true });
                successCallback(new ActionCompletedEventArgs<List<RefChromosome>>((List<RefChromosome>)ea.Result));
            };

            worker.RunWorkerAsync();

            #endregion
        }

        public void GetCompGenomes(ICollection<RefChromosome> refChromosomes, Action<ActionCompletedEventArgs<List<CompGenome>>> successCallback, Action<ActionFailingEventArgs<RefChromosome>> failureCallback = null, Action beforeLoadCallback = null)
        {
            Debug.WriteLine("GetCompGenomes()");

            #region 1. check if already loaded in memory cache
            var chromosomesToLoad = refChromosomes.Where(c => c.CompGenomes == null).ToList();
            if (chromosomesToLoad.IsEmpty())
            {
                successCallback(new ActionCompletedEventArgs<List<CompGenome>>(
                    refChromosomes.SelectMany(c => c.CompGenomes).ToList()));
                return;
            }
            #endregion

            // TODO: 2. check if exists in isolated storage cache

            #region 3. load from web service
            _eventPublisher.Publish(new CompGenomesLoadingEvent { IsDoneLoading = false });

            if (beforeLoadCallback != null)
                beforeLoadCallback();

            var worker = new BackgroundWorker { WorkerReportsProgress = false, WorkerSupportsCancellation = false };

            worker.DoWork += (s, ea) =>
            {
                Debug.WriteLine("LoadCompGenomes");
                var chromosomesLoaded = new List<RefChromosome>();
                bool retry;

                do
                {
                    retry = false;

                    var waitHandles = from chromosome in chromosomesToLoad.ToList()
                                      let mre = new ManualResetEvent(false)
                                      let completed = _dataService.BeginListCompGenomes(chromosome.Genome.Name, chromosome.Name,
                                            asyncResult =>
                                            {
                                                try
                                                {
                                                    var compGenomes = _dataService.EndListCompGenomes(asyncResult)
                                                        .Select(genome => new CompGenome(genome.SpeciesName, chromosome))
                                                        .ToList();
                                                    chromosome.CompGenomes = compGenomes;
                                                    chromosomesToLoad.Remove(chromosome);
                                                    chromosomesLoaded.Add(chromosome);
                                                }
                                                catch (CommunicationException e)
                                                {
                                                    UISynchronizationContext.Instance.InvokeSynchronously(
                                                        delegate
                                                        {
                                                            var errorMsg = string.Format("{0}{2}{1}{2}{2}Press OK if you want to retry, or Cancel to skip.", 
                                                                string.Format("Error loading comp genomes for {0} chr {1}", chromosome.Genome.Name, chromosome.Name),
                                                                e.Message, Environment.NewLine);
                                                            var mbResult = MessageBox.Show(errorMsg, "Error", MessageBoxButton.OKCancel);
                                                            retry = mbResult == MessageBoxResult.OK;
                                                        });

                                                    if (!retry) chromosomesToLoad.Remove(chromosome);
                                                }
                                                finally
                                                {
                                                    ((ManualResetEvent)asyncResult.AsyncState).Set();
                                                }
                                            }, mre).IsCompleted
                                      where !completed
                                      select mre;

                    waitHandles.All(w => w.WaitOne());
                } while (!chromosomesToLoad.IsEmpty());

                ea.Result = refChromosomes.Where(c => c.CompGenomes != null).SelectMany(c => c.CompGenomes).ToList();
            };

            worker.RunWorkerCompleted += (s, ea) =>
            {
                _eventPublisher.Publish(new CompGenomesLoadingEvent { IsDoneLoading = true });
                successCallback(new ActionCompletedEventArgs<List<CompGenome>>((List<CompGenome>)ea.Result));
            };

            worker.RunWorkerAsync();

            #endregion
        }

        public void GetSyntenyBlocks(ICollection<CompGenome> compGenomes, Action<ActionCompletedEventArgs<List<SyntenyRegion>>> successCallback, Action<ActionFailingEventArgs<CompGenome>> failureCallback = null, Action beforeLoadCallback = null)
        {
            Debug.WriteLine("GetSyntenyBlocks()");

            #region 1. check if already loaded in memory cache
            var compGenomesToLoad = compGenomes.Where(g => g.SyntenyBlocks == null).ToList();
            if (compGenomesToLoad.IsEmpty())
            {
                successCallback(new ActionCompletedEventArgs<List<SyntenyRegion>>(
                    compGenomes.SelectMany(g => g.SyntenyBlocks).ToList()));
                return;
            }
            #endregion

            // TODO: 2. check if exists in isolated storage cache

            #region 3. load from web service
            _eventPublisher.Publish(new SyntenyBlocksLoadingEvent { IsDoneLoading = false });

            if (beforeLoadCallback != null)
                beforeLoadCallback();

            var worker = new BackgroundWorker { WorkerReportsProgress = false, WorkerSupportsCancellation = false };

            worker.DoWork += (s, ea) =>
            {
                Debug.WriteLine("LoadSyntenyBlocks");
                var compGenomesLoaded = new List<CompGenome>();
                bool retry;

                do
                {
                    retry = false;

                    var waitHandles = from compGen in compGenomesToLoad.ToList()
                                      let mre = new ManualResetEvent(false)
                                      let completed = _dataService.BeginListSyntenyRegions(compGen.RefChromosome.Genome.Name,
                                            compGen.RefChromosome.Name, compGen.Name, asyncResult =>
                                            {
                                                try
                                                {
                                                    var synBlocks = _dataService.EndListSyntenyRegions(asyncResult).Select(r =>
                                                        new SyntenyRegion(r.Start, r.End, r.Chromosome, r.Label, compGen,
                                                                          r.ModStart, r.ModEnd, r.Sign)).ToList();
                                                    compGen.SyntenyBlocks = synBlocks;
                                                    compGenomesToLoad.Remove(compGen);
                                                    compGenomesLoaded.Add(compGen);
                                                }
                                                catch (CommunicationException e)
                                                {
                                                    UISynchronizationContext.Instance.InvokeSynchronously(
                                                        delegate
                                                        {
                                                            var errorMsg = string.Format("{0}{2}{1}{2}{2}Press OK if you want to retry, or Cancel to skip.",
                                                                string.Format("Error loading synteny blocks for {0} chr {1} ({2})", compGen.RefChromosome.Genome.Name, compGen.RefChromosome.Name, compGen.Name),
                                                                e.Message, Environment.NewLine);
                                                            var mbResult = MessageBox.Show(errorMsg, "Error", MessageBoxButton.OKCancel);
                                                            retry = mbResult == MessageBoxResult.OK;
                                                        });

                                                    if (!retry) compGenomesToLoad.Remove(compGen);
                                                }
                                                finally
                                                {
                                                    ((ManualResetEvent)asyncResult.AsyncState).Set();
                                                }
                                            }, mre).IsCompleted
                                      where !completed
                                      select mre;

                    waitHandles.All(w => w.WaitOne());
                } while (retry);

                var synBlocksLoaded = compGenomes.Where(g => g.SyntenyBlocks != null).SelectMany(g => g.SyntenyBlocks).ToList();

                var extraDataLoaded = new ManualResetEvent(false);
                LoadCompChrLengths(compGenomesLoaded, (rcl, pcl) => extraDataLoaded.Set());
                extraDataLoaded.WaitOne();

                // TODO: This needs to be done differently
                if (!ScaleConverter.DataMaximum.HasValue)
                    ScaleConverter.DataMaximum = synBlocksLoaded.Max(b => b.End);

                ea.Result = synBlocksLoaded;
            };

            worker.RunWorkerCompleted += (s, ea) =>
            {
                _eventPublisher.Publish(new SyntenyBlocksLoadingEvent { IsDoneLoading = true });
                successCallback(new ActionCompletedEventArgs<List<SyntenyRegion>>((List<SyntenyRegion>)ea.Result));
            };

            worker.RunWorkerAsync();
            #endregion
        }

        public void GetCentromereRegions(ICollection<RefChromosome> refChromosomes, Action<ActionCompletedEventArgs<List<CentromereRegion>>> successCallback, Action<ActionFailingEventArgs<RefChromosome>> failureCallback = null, Action beforeLoadCallback = null)
        {
            Debug.WriteLine("GetCentromereRegions()");
            var chromosomesToLoad = refChromosomes.Where(chromosome => chromosome.CentromereRegions == null).ToList();
            if (chromosomesToLoad.IsEmpty())
            {
                successCallback(new ActionCompletedEventArgs<List<CentromereRegion>>(
                    refChromosomes.SelectMany(c => c.CentromereRegions).ToList()));
                return;
            }

            _eventPublisher.Publish(new CentromereRegionsLoadingEvent { IsDoneLoading = false });

            if (beforeLoadCallback != null)
                beforeLoadCallback();

            var worker = new BackgroundWorker { WorkerReportsProgress = false, WorkerSupportsCancellation = false };

            worker.DoWork += (s, ea) =>
            {
                Debug.WriteLine("LoadCentromereRegions");
                bool retry;

                do
                {
                    retry = false;

                    var waitHandles = from chromosome in chromosomesToLoad.ToList()
                                      let mre = new ManualResetEvent(false)
                                      let completed = _dataService.BeginListCentromereRegions(chromosome.Genome.Name, chromosome.Name,
                                            asyncResult =>
                                            {
                                                try
                                                {
                                                    var regions = _dataService.EndListCentromereRegions(asyncResult)
                                                        .Select(r => new CentromereRegion(r.Start, r.End, chromosome)).ToList();
                                                    chromosome.CentromereRegions = regions;
                                                    chromosomesToLoad.Remove(chromosome);
                                                }
                                                catch (CommunicationException e)
                                                {
                                                    UISynchronizationContext.Instance.InvokeSynchronously(
                                                        delegate
                                                        {
                                                            var errorMsg = string.Format("{0}{2}{1}{2}{2}Press OK if you want to retry, or Cancel to skip.",
                                                                string.Format("Error loading centromeres for {0} chr {1}", chromosome.Genome.Name, chromosome.Name),
                                                                e.Message, Environment.NewLine);
                                                            var mbResult = MessageBox.Show(errorMsg, "Error", MessageBoxButton.OKCancel);
                                                            retry = mbResult == MessageBoxResult.OK;
                                                        });

                                                    if (!retry) chromosomesToLoad.Remove(chromosome);
                                                }
                                                finally
                                                {
                                                    ((ManualResetEvent)asyncResult.AsyncState).Set();
                                                }
                                            }, mre).IsCompleted
                                      where !completed
                                      select mre;

                    waitHandles.All(w => w.WaitOne());
                } while (retry);

                ea.Result = refChromosomes.Where(c => c.CentromereRegions != null).SelectMany(c => c.CentromereRegions).ToList();
            };

            worker.RunWorkerCompleted += (s, ea) =>
            {
                _eventPublisher.Publish(new CentromereRegionsLoadingEvent { IsDoneLoading = true });
                successCallback(new ActionCompletedEventArgs<List<CentromereRegion>>((List<CentromereRegion>)ea.Result));
            };

            worker.RunWorkerAsync();
        }

        public void GetHeterochromatinRegions(ICollection<RefChromosome> refChromosomes, Action<ActionCompletedEventArgs<List<HeterochromatinRegion>>> successCallback, Action<ActionFailingEventArgs<RefChromosome>> failureCallback = null, Action beforeLoadCallback = null)
        {
            Debug.WriteLine("GetHeterochromatinRegions()");
            var chromosomesToLoad = refChromosomes.Where(chromosome => chromosome.HeterochromatinRegions == null).ToList();
            if (chromosomesToLoad.IsEmpty())
            {
                successCallback(new ActionCompletedEventArgs<List<HeterochromatinRegion>>(
                    refChromosomes.SelectMany(c => c.HeterochromatinRegions).ToList()));
                return;
            }

            _eventPublisher.Publish(new HeterochromatinRegionsLoadingEvent { IsDoneLoading = false });

            if (beforeLoadCallback != null)
                beforeLoadCallback();

            var worker = new BackgroundWorker { WorkerReportsProgress = false, WorkerSupportsCancellation = false };

            worker.DoWork += (s, ea) =>
            {
                Debug.WriteLine("LoadHeterochromatinRegions");
                bool retry;

                do
                {
                    retry = false;

                    var waitHandles = from chromosome in chromosomesToLoad.ToList()
                                      let mre = new ManualResetEvent(false)
                                      let completed = _dataService.BeginListHeterochromatinRegions(chromosome.Genome.Name, chromosome.Name,
                                            asyncResult =>
                                            {
                                                try
                                                {
                                                    var regions = _dataService.EndListHeterochromatinRegions(asyncResult)
                                                        .Select(r => new HeterochromatinRegion(r.Start, r.End, chromosome)).ToList();
                                                    chromosome.HeterochromatinRegions = regions;
                                                    chromosomesToLoad.Remove(chromosome);
                                                }
                                                catch (CommunicationException e)
                                                {
                                                    UISynchronizationContext.Instance.InvokeSynchronously(
                                                        delegate
                                                        {
                                                            var errorMsg = string.Format("{0}{2}{1}{2}{2}Press OK if you want to retry, or Cancel to skip.",
                                                                string.Format("Error loading heterochromatin for {0} chr {1}", chromosome.Genome.Name, chromosome.Name),
                                                                e.Message, Environment.NewLine);
                                                            var mbResult = MessageBox.Show(errorMsg, "Error", MessageBoxButton.OKCancel);
                                                            retry = mbResult == MessageBoxResult.OK;
                                                        });

                                                    if (!retry) chromosomesToLoad.Remove(chromosome);
                                                }
                                                finally
                                                {
                                                    ((ManualResetEvent)asyncResult.AsyncState).Set();
                                                }
                                            }, mre).IsCompleted
                                      where !completed
                                      select mre;

                    waitHandles.All(w => w.WaitOne());
                } while (retry);

                ea.Result = refChromosomes.Where(c => c.HeterochromatinRegions != null).SelectMany(c => c.HeterochromatinRegions).ToList();
            };

            worker.RunWorkerCompleted += (s, ea) =>
            {
                _eventPublisher.Publish(new HeterochromatinRegionsLoadingEvent { IsDoneLoading = true });
                successCallback(new ActionCompletedEventArgs<List<HeterochromatinRegion>>((List<HeterochromatinRegion>)ea.Result));
            };

            worker.RunWorkerAsync();
        }



        private void LoadCompChrLengths(ICollection<CompGenome> compGenomes,
            Action<RunWorkerCompletedEventArgs, object> loadCompletedCallback, Action beforeLoadCallback = null,
            object param = null)
        {
            var compChrToLoad = compGenomes.Select(g => g.Name).Distinct().Where(n => !RepositoryState.CompChromosomeLengths.ContainsKey(n)).ToList();
            if (compChrToLoad.IsEmpty())
            {
                loadCompletedCallback(new RunWorkerCompletedEventArgs(null, null, false), param);
                return;
            }

            _eventPublisher.Publish(new CompChrLengthsLoadingEvent { IsDoneLoading = false });

            if (beforeLoadCallback != null)
                beforeLoadCallback();

            var worker = new BackgroundWorker { WorkerReportsProgress = false, WorkerSupportsCancellation = false };

            worker.DoWork += (s, ea) =>
            {
                var waitHandles = from compChr in compChrToLoad
                                  let mre = new ManualResetEvent(false)
                                  let completed = _dataService.BeginGetCompChrLengths(compChr,
                                        asyncResult =>
                                        {
                                            try
                                            {
                                                var compChrLengths = _dataService.EndGetCompChrLengths(asyncResult)
                                                    .ToDictionary(cl => cl.Chromosome, cl => cl.Length);
                                                RepositoryState.CompChromosomeLengths.Add(compChr, compChrLengths);
                                            }
                                            finally
                                            {
                                                ((ManualResetEvent)asyncResult.AsyncState).Set();
                                            }
                                        }, mre).IsCompleted
                                  where !completed
                                  select mre;

                waitHandles.All(w => w.WaitOne());
            };

            worker.RunWorkerCompleted += (s, ea) =>
            {
                _eventPublisher.Publish(new CompChrLengthsLoadingEvent { IsDoneLoading = true });
                loadCompletedCallback(ea, param);
            };

            worker.RunWorkerAsync();
        }
    }
}
