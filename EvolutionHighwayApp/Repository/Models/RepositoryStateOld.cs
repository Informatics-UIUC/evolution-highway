using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using Castle.MicroKernel.Registration;
using EvolutionHighwayApp.Converters;
using EvolutionHighwayApp.Display.Controllers;
using EvolutionHighwayApp.Events;
using EvolutionHighwayApp.Exceptions;
using EvolutionHighwayApp.Infrastructure;
using EvolutionHighwayApp.Infrastructure.EventBus;
using EvolutionHighwayApp.Models;
using EvolutionHighwayApp.ServiceLayer;
using EvolutionHighwayApp.Utils;

namespace EvolutionHighwayApp.Repository.Models
{
    public class RepositoryStateOld
    {
//        public IEHDataService Service { get; private set; }
//        public readonly Dictionary<RefGenome, List<RefChromosome>> RefGenomeMap;
//        public readonly Dictionary<RefChromosome, List<CompGenome>> RefChromosomeMap;
//        public static readonly IDictionary<string, IDictionary<string, double>> CompChromosomeLengths
//            = new Dictionary<string, IDictionary<string, double>>();
//
//        public readonly Dictionary<string, SmartObservableCollection<CustomTrackRegion>> CustomTrackMap; 
//
//        public DisplayController DisplayController { get; set; }
//
//        private readonly IEventPublisher _eventPublisher;
//
//        public RepositoryStateOld(IEventPublisher eventPublisher)
//        {
//            Debug.WriteLine("{0} instantiated", GetType().Name);
//
//            _eventPublisher = eventPublisher;
//            
//            RefGenomeMap = new Dictionary<RefGenome, List<RefChromosome>>();
//            RefChromosomeMap = new Dictionary<RefChromosome, List<CompGenome>>();
//            CustomTrackMap = new Dictionary<string, SmartObservableCollection<CustomTrackRegion>>();
//
//            eventPublisher.GetEvent<DataSourceChangedEvent>().Subscribe(e =>
//            {
//                if (!IoC.Container.Kernel.HasComponent(e.DataSourceUrl))
//                {
//                    Debug.WriteLine("Creating ServiceProxy for: {0}", e.DataSourceUrl);
//                    var service = EHDataService.CreateServiceProxy(new EndpointAddress(e.DataSourceUrl));
//                    IoC.Container.Register(Component.For<IEHDataService>().Instance(service).Named(e.DataSourceUrl).LifeStyle.Singleton);
//                }
//
//                Service = IoC.Container.Resolve<IEHDataService>(e.DataSourceUrl);
//
//                RefGenomeMap.ForEach(g => g.Value.Clear());
//                RefGenomeMap.Clear();
//
//                RefChromosomeMap.ForEach(c => c.Value.Clear());
//                RefChromosomeMap.Clear();
//
//                CompChromosomeLengths.ForEach(cl => cl.Value.Clear());
//                CompChromosomeLengths.Clear();
//
//                ClearCustomTracks();
//            });
//        }
//
//        public void LoadSelectionData(Action<RunWorkerCompletedEventArgs, object> loadCompletedCallback, 
//                                      Action beforeLoadCallback = null, object param = null)
//        {
//            LoadRefGenomes((rg, pg) =>
//            {
//                var genomes = RefGenomeMap.Keys;
//                LoadRefChromosomes(genomes, (rc, pc) =>
//                {
//                    var chromosomes = genomes.SelectMany(g => RefGenomeMap[g]).ToList();
//                    LoadCompGenomes(chromosomes, loadCompletedCallback);
//                });
////                LoadRefChromosomes(genomes, loadCompletedCallback);
//            }, beforeLoadCallback, param);
//        }
//
//        public void LoadRefGenomes(Action<RunWorkerCompletedEventArgs, object> loadCompletedCallback, 
//            Action beforeLoadCallback = null, object param = null)
//        {
//            _eventPublisher.Publish(new RefGenomesLoadingEvent { IsDoneLoading = false });
//
//            if (beforeLoadCallback != null)
//                beforeLoadCallback();
//
//            var worker = new BackgroundWorker { WorkerReportsProgress = false, WorkerSupportsCancellation = false };
//
//            worker.DoWork += (s, ea) =>
//            {
//                Exception error = null;
//                var mre = new ManualResetEvent(false);
//                Service.BeginListRefGenomes(asyncResult =>
//                {
//                    try
//                    {
//                        var genomes = Service.EndListRefGenomes(asyncResult).Select(g => new RefGenome(g.Name));
//                        genomes.ForEach(genome => RefGenomeMap.Add(genome, new List<RefChromosome>()));
//                    }
//                    catch(CommunicationException e)
//                    {
//                        error = new ServiceException("Error retrieving reference genomes", e);
//                    }
//                    finally
//                    {
//                        ((ManualResetEvent)asyncResult.AsyncState).Set();                        
//                    }
//                }, mre);
//                mre.WaitOne();
//
//                if (error != null)
//                    throw error;
//            };
//
//            worker.RunWorkerCompleted += (s, ea) =>
//                {
//                    _eventPublisher.Publish(new RefGenomesLoadingEvent { IsDoneLoading = true });
//                    loadCompletedCallback(ea, param);
//                };
//
//            worker.RunWorkerAsync();
//        }
//
//        public void LoadRefChromosomes(IEnumerable<RefGenome> genomes, 
//            Action<RunWorkerCompletedEventArgs, object> loadCompletedCallback, Action beforeLoadCallback = null, 
//            object param = null)
//        {
//            var genomesToLoad = genomes.Where(genome => RefGenomeMap[genome].IsEmpty()).ToList();
//            if (genomesToLoad.IsEmpty())
//            {
//                loadCompletedCallback(new RunWorkerCompletedEventArgs(null, null, false), param);
//                return;
//            }
//
//            _eventPublisher.Publish(new RefChromosomesLoadingEvent { IsDoneLoading = false });
//
//            if (beforeLoadCallback != null)
//                beforeLoadCallback();
//
//            var worker = new BackgroundWorker { WorkerReportsProgress = false, WorkerSupportsCancellation = false };
//            
//            worker.DoWork += (s, ea) =>
//            {
//                var errors = new List<Exception>();
//                var waitHandles = from genome in genomesToLoad
//                                  let mre = new ManualResetEvent(false)
//                                  let completed = Service.BeginListRefChromosomes(genome.Name,
//                                        asyncResult =>
//                                        {
//                                            try
//                                            {
//                                                var chromosomes = Service.EndListRefChromosomes(asyncResult)
//                                                    .Select(chromosome => new RefChromosome(chromosome.Name, chromosome.Length, genome))
//                                                    .ToList();
//                                                RefGenomeMap[genome].AddRange(chromosomes);
//                                                chromosomes.ForEach(chromosome => RefChromosomeMap.Add(chromosome, new List<CompGenome>()));
//                                            }
//                                            catch (CommunicationException e)
//                                            {
//                                                errors.Add(new ServiceException("Error loading reference chromosomes for genome: " + genome.Name, e));
//                                            }
//                                            finally
//                                            {
//                                                ((ManualResetEvent) asyncResult.AsyncState).Set();
//                                            }
//                                        }, mre).IsCompleted
//                                  where !completed
//                                  select mre;
//
//                waitHandles.All(w => w.WaitOne());
//
//                if (!errors.IsEmpty())
//                    throw new MultipleExceptions(errors);
//            };
//
//            worker.RunWorkerCompleted += (s, ea) =>
//                {
//                    _eventPublisher.Publish(new RefChromosomesLoadingEvent { IsDoneLoading = true });
//                    loadCompletedCallback(ea, param);
//                };
//
//            worker.RunWorkerAsync();
//        }
//
//        public void LoadCompGenomes(IEnumerable<RefChromosome> chromosomes, 
//            Action<RunWorkerCompletedEventArgs, object> loadCompletedCallback, Action beforeLoadCallback = null, 
//            object param = null)
//        {
//            var chromosomesToLoad = chromosomes.Where(chromosome => RefChromosomeMap[chromosome].IsEmpty()).ToList();
//            if (chromosomesToLoad.IsEmpty())
//            {
//                loadCompletedCallback(new RunWorkerCompletedEventArgs(null, null, false), param);
//                return;
//            }
//
//            _eventPublisher.Publish(new CompGenomesLoadingEvent { IsDoneLoading = false });
//
//            if (beforeLoadCallback != null)
//                beforeLoadCallback();
//
//            var worker = new BackgroundWorker { WorkerReportsProgress = false, WorkerSupportsCancellation = false };
//
//            worker.DoWork += (s, ea) =>
//            {
//                var errors = new List<Exception>();
//                var waitHandles = from chromosome in chromosomesToLoad
//                                  let mre = new ManualResetEvent(false)
//                                  let completed = Service.BeginListCompGenomes(chromosome.RefGenome.Name, chromosome.Name,
//                                        asyncResult =>
//                                        {
//                                            try
//                                            {
//                                                var genomes = Service.EndListCompGenomes(asyncResult)
//                                                    .Select(g => new CompGenome(g.SpeciesName, chromosome)).ToList();
//                                                RefChromosomeMap[chromosome].AddRange(genomes);
//                                            }
//                                            catch (CommunicationException e)
//                                            {
//                                                errors.Add(new ServiceException(string.Format("Error retrieving comparative genomes for genome: {0} chr: {1}", 
//                                                    chromosome.RefGenome.Name, chromosome.Name), e));
//                                            }
//                                            finally
//                                            {
//                                                ((ManualResetEvent) asyncResult.AsyncState).Set();
//                                            }
//                                        }, mre).IsCompleted
//                                  where !completed
//                                  select mre;
//
//                waitHandles.All(w => w.WaitOne());
//
////                var extraDataLoaded = new ManualResetEvent(false);
////                LoadCentromereRegions(chromosomesToLoad, (rc, pc) => {
////                    if (rc.Error != null)
////                        errors.Add(rc.Error);
////                    LoadHeterochromatinRegions(chromosomesToLoad, (rh, ph) => {
////                        if (rh.Error != null)
////                            errors.Add(rh.Error);
////                        LoadDensityFeatureData("AdjacencyScore", chromosomesToLoad, (rf, pf) => {
////                            if (rf.Error != null)
////                                errors.Add(rf.Error);
////                            extraDataLoaded.Set();
////                        });
////                    });
////                });
////                extraDataLoaded.WaitOne();
//
//                if (!errors.IsEmpty())
//                    throw new MultipleExceptions(errors);
//            };
//
//            worker.RunWorkerCompleted += (s, ea) =>
//                {
//                    _eventPublisher.Publish(new CompGenomesLoadingEvent { IsDoneLoading = true });
//                    loadCompletedCallback(ea, param);
//                };
//
//            worker.RunWorkerAsync();
//        }
//
//        public void LoadSyntenyBlocks(IEnumerable<CompGenome> compGenomes,
//            Action<RunWorkerCompletedEventArgs, object> loadCompletedCallback, Action beforeLoadCallback = null,
//            object param = null)
//        {
//            var compGenomesToLoad = compGenomes.Where(g => g.SyntenyBlocks.IsEmpty()).ToList();
//            if (compGenomesToLoad.IsEmpty())
//            {
//                loadCompletedCallback(new RunWorkerCompletedEventArgs(null, null, false), param);
//                return;
//            }
//
//            _eventPublisher.Publish(new SyntenyBlocksLoadingEvent { IsDoneLoading = false });
//
//            if (beforeLoadCallback != null)
//                beforeLoadCallback();
//
//            var worker = new BackgroundWorker { WorkerReportsProgress = false, WorkerSupportsCancellation = false };
//
//            worker.DoWork += (s, ea) =>
//            {
//                var errors = new List<Exception>();
//                var waitHandles = from compGen in compGenomesToLoad
//                                  let mre = new ManualResetEvent(false)
//                                  let completed = Service.BeginListSyntenyRegions(compGen.RefChromosome.RefGenome.Name, 
//                                        compGen.RefChromosome.Name, compGen.Name, asyncResult =>
//                                        {
//                                            try
//                                            {
//                                                var synBlocks = Service.EndListSyntenyRegions(asyncResult).Select(r =>
//                                                    new SyntenyRegion(r.Start, r.End, r.Chromosome, r.Label, compGen,
//                                                                      r.ModStart, r.ModEnd, r.Sign)).ToList();
//                                                compGen.SyntenyBlocks.AddRange(synBlocks);
//                                            }
//                                            catch (CommunicationException e)
//                                            {
//                                                errors.Add(new ServiceException(string.Format("Error retrieving synteny regions for genome: {0} chr: {1} compGen: {2}",
//                                                    compGen.RefChromosome.RefGenome.Name, compGen.RefChromosome.Name, compGen.Name), e));
//                                            }
//                                            finally
//                                            {
//                                                ((ManualResetEvent) asyncResult.AsyncState).Set();
//                                            }
//                                        }, mre).IsCompleted
//                                  where !completed
//                                  select mre;
//
//                waitHandles.All(w => w.WaitOne());
//
//                var extraDataLoaded = new ManualResetEvent(false);
//                LoadCompChrLengths(compGenomesToLoad, (rcl, pcl) =>
//                                                          {
//                                                              if (rcl.Error != null)
//                                                                  errors.Add(rcl.Error);
//                                                              extraDataLoaded.Set();
//                                                          });
//                extraDataLoaded.WaitOne();
//
//                // TODO: This needs to be done differently
//                if (ScaleConverter.DataMaximum == default(double))
//                {
//                    var maxBp = 0d;
//                    compGenomesToLoad.ForEach(g => g.SyntenyBlocks.ForEach(b => maxBp = Math.Max(maxBp, b.End)));
//                    ScaleConverter.DataMaximum = maxBp;
//                }
//
//                if (!errors.IsEmpty())
//                    throw new MultipleExceptions(errors);
//            };
//
//            worker.RunWorkerCompleted += (s, ea) =>
//                {
//                    _eventPublisher.Publish(new SyntenyBlocksLoadingEvent { IsDoneLoading = true });
//                    loadCompletedCallback(ea, param);
//                };
//
//            worker.RunWorkerAsync();
//        }
//
//        public void LoadCentromereRegions(IEnumerable<RefChromosome> chromosomes,
//            Action<RunWorkerCompletedEventArgs, object> loadCompletedCallback, Action beforeLoadCallback = null,
//            object param = null)
//        {
//            var chromosomesToLoad = chromosomes.Where(chromosome => chromosome.CentromereRegions.IsEmpty()).ToList();
//            if (chromosomesToLoad.IsEmpty())
//            {
//                loadCompletedCallback(new RunWorkerCompletedEventArgs(null, null, false), param);
//                return;
//            }
//
//            _eventPublisher.Publish(new CentromereRegionsLoadingEvent { IsDoneLoading = false });
//
//            if (beforeLoadCallback != null)
//                beforeLoadCallback();
//
//            var worker = new BackgroundWorker { WorkerReportsProgress = false, WorkerSupportsCancellation = false };
//
//            worker.DoWork += (s, ea) =>
//            {
//                var errors = new List<Exception>();
//                var waitHandles = from chromosome in chromosomesToLoad
//                                  let mre = new ManualResetEvent(false)
//                                  let completed = Service.BeginListCentromereRegions(chromosome.RefGenome.Name, chromosome.Name,
//                                        asyncResult =>
//                                        {
//                                            try
//                                            {
//                                                var regions = Service.EndListCentromereRegions(asyncResult)
//                                                    .Select(r => new CentromereRegion(r.Start, r.End, chromosome)).ToList();
//                                                chromosome.CentromereRegions.AddRange(regions);
//                                            }
//                                            catch (CommunicationException e)
//                                            {
//                                                errors.Add(new ServiceException(string.Format("Error retrieving centromere regions for genome: {0} chr: {1}",
//                                                    chromosome.RefGenome.Name, chromosome.Name), e));
//                                            }
//                                            finally
//                                            {
//                                                ((ManualResetEvent) asyncResult.AsyncState).Set();
//                                            }
//                                        }, mre).IsCompleted
//                                  where !completed
//                                  select mre;
//
//                waitHandles.All(w => w.WaitOne());
//
//                if (!errors.IsEmpty())
//                    throw new MultipleExceptions(errors);
//            };
//
//            worker.RunWorkerCompleted += (s, ea) =>
//            {
//                _eventPublisher.Publish(new CentromereRegionsLoadingEvent { IsDoneLoading = true });
//                loadCompletedCallback(ea, param);
//            };
//
//            worker.RunWorkerAsync();
//        }
//
//        public void LoadHeterochromatinRegions(IEnumerable<RefChromosome> chromosomes,
//            Action<RunWorkerCompletedEventArgs, object> loadCompletedCallback, Action beforeLoadCallback = null,
//            object param = null)
//        {
//            var chromosomesToLoad = chromosomes.Where(chromosome => chromosome.HeterochromatinRegions.IsEmpty()).ToList();
//            if (chromosomesToLoad.IsEmpty())
//            {
//                loadCompletedCallback(new RunWorkerCompletedEventArgs(null, null, false), param);
//                return;
//            }
//
//            _eventPublisher.Publish(new HeterochromatinRegionsLoadingEvent { IsDoneLoading = false });
//
//            if (beforeLoadCallback != null)
//                beforeLoadCallback();
//
//            var worker = new BackgroundWorker { WorkerReportsProgress = false, WorkerSupportsCancellation = false };
//
//            worker.DoWork += (s, ea) =>
//            {
//                var errors = new List<Exception>();
//                var waitHandles = from chromosome in chromosomesToLoad
//                                  let mre = new ManualResetEvent(false)
//                                  let completed = Service.BeginListHeterochromatinRegions(chromosome.RefGenome.Name, chromosome.Name,
//                                        asyncResult =>
//                                        {
//                                            try
//                                            {
//                                                var regions = Service.EndListHeterochromatinRegions(asyncResult)
//                                                    .Select(r => new HeterochromatinRegion(r.Start, r.End, chromosome)).ToList();
//                                                chromosome.HeterochromatinRegions.AddRange(regions);
//                                            }
//                                            catch (CommunicationException e)
//                                            {
//                                                errors.Add(new ServiceException(string.Format("Error retrieving heterochromatin regions for genome: {0} chr: {1}",
//                                                    chromosome.RefGenome.Name, chromosome.Name), e));
//                                            }
//                                            finally
//                                            {
//                                                ((ManualResetEvent) asyncResult.AsyncState).Set();
//                                            }
//                                        }, mre).IsCompleted
//                                  where !completed
//                                  select mre;
//
//                waitHandles.All(w => w.WaitOne());
//
//                if (!errors.IsEmpty())
//                    throw new MultipleExceptions(errors);
//            };
//
//            worker.RunWorkerCompleted += (s, ea) =>
//            {
//                _eventPublisher.Publish(new HeterochromatinRegionsLoadingEvent { IsDoneLoading = true });
//                loadCompletedCallback(ea, param);
//            };
//
//            worker.RunWorkerAsync();
//        }
//
//        public void LoadCompChrLengths(IEnumerable<CompGenome> compGenomes,
//            Action<RunWorkerCompletedEventArgs, object> loadCompletedCallback, Action beforeLoadCallback = null,
//            object param = null)
//        {
//            var compChrToLoad = compGenomes.Select(g => g.Name).Distinct().Where(n => !CompChromosomeLengths.ContainsKey(n)).ToList();
//            if (compChrToLoad.IsEmpty())
//            {
//                loadCompletedCallback(new RunWorkerCompletedEventArgs(null, null, false), param);
//                return;
//            }
//
//            _eventPublisher.Publish(new CompChrLengthsLoadingEvent { IsDoneLoading = false });
//
//            if (beforeLoadCallback != null)
//                beforeLoadCallback();
//
//            var worker = new BackgroundWorker { WorkerReportsProgress = false, WorkerSupportsCancellation = false };
//
//            worker.DoWork += (s, ea) =>
//            {
//                var errors = new List<Exception>();
//                var waitHandles = from compChr in compChrToLoad
//                                  let mre = new ManualResetEvent(false)
//                                  let completed = Service.BeginGetCompChrLengths(compChr,
//                                        asyncResult =>
//                                        {
//                                            try
//                                            {
//                                                var compChrLengths = Service.EndGetCompChrLengths(asyncResult)
//                                                    .ToDictionary(cl => cl.Chromosome, cl => cl.Length);
//                                                CompChromosomeLengths.Add(compChr, compChrLengths);
//                                            }
//                                            catch (CommunicationException e)
//                                            {
//                                                errors.Add(new ServiceException("Error retrieving comparative chromosome lengths for: " + compChr, e));
//                                            }
//                                            finally
//                                            {
//                                                ((ManualResetEvent) asyncResult.AsyncState).Set();
//                                            }
//                                        }, mre).IsCompleted
//                                  where !completed
//                                  select mre;
//
//                waitHandles.All(w => w.WaitOne());
//
//                if (!errors.IsEmpty())
//                    throw new MultipleExceptions(errors);
//            };
//
//            worker.RunWorkerCompleted += (s, ea) =>
//            {
//                _eventPublisher.Publish(new CompChrLengthsLoadingEvent { IsDoneLoading = true });
//                loadCompletedCallback(ea, param);
//            };
//
//            worker.RunWorkerAsync();
//        }
//
//        public void LoadDensityFeatureData(string featureName, IEnumerable<RefChromosome> refChromosomes,
//            Action<RunWorkerCompletedEventArgs, object> loadCompletedCallback, Action beforeLoadCallback = null,
//            object param = null)
//        {
//            var chromosomesToLoad = refChromosomes.Where(c => !c.FeatureDensityData.ContainsKey(featureName)).ToList();
//            if (chromosomesToLoad.IsEmpty())
//            {
//                loadCompletedCallback(new RunWorkerCompletedEventArgs(null, null, false), param);
//                return;
//            }
//
//            _eventPublisher.Publish(new FeatureLoadingEvent { FeatureName = featureName, IsDoneLoading = false });
//
//            if (beforeLoadCallback != null)
//                beforeLoadCallback();
//
//            var worker = new BackgroundWorker { WorkerReportsProgress = false, WorkerSupportsCancellation = false };
//
//            worker.DoWork += (s, ea) =>
//            {
//                var errors = new List<Exception>();
//                var waitHandles = from chromosome in chromosomesToLoad
//                                  let mre = new ManualResetEvent(false)
//                                  let completed = Service.BeginGetFeatureData(featureName, chromosome.RefGenome.Name, chromosome.Name,
//                                        asyncResult =>
//                                        {
//                                            try
//                                            {
//                                                var featureDensityData = Service.EndGetFeatureData(asyncResult)
//                                                    .Select(d => new FeatureDensity(d.RefStart, d.RefEnd, d.Score, d.CompGen)).ToList();
//                                                chromosome.FeatureDensityData.Add(featureName, featureDensityData);
//                                            }
//                                            catch (CommunicationException e)
//                                            {
//                                                errors.Add(new ServiceException(string.Format("Error retrieving feature density data for genome: {0} chr: {1}",
//                                                    chromosome.RefGenome.Name, chromosome.Name), e));
//                                            }
//                                            finally
//                                            {
//                                                ((ManualResetEvent) asyncResult.AsyncState).Set();
//                                            }
//                                        }, mre).IsCompleted
//                                  where !completed
//                                  select mre;
//
//                waitHandles.All(w => w.WaitOne());
//
//                if (!errors.IsEmpty())
//                    throw new MultipleExceptions(errors);
//            };
//
//            worker.RunWorkerCompleted += (s, ea) =>
//            {
//                _eventPublisher.Publish(new FeatureLoadingEvent { FeatureName = featureName, IsDoneLoading = true });
//                loadCompletedCallback(ea, param);
//            };
//
//            worker.RunWorkerAsync();
//        }
//
//        public void AddCustomTrackData(string trackData, char delimiter, bool append = false)
//        {
//            var lines = trackData.Split(new[] { '\n', '\r' });
//            var lineno = 0;
//
//            if (!append)
//                // Clear any existing custom tracks
//                ClearCustomTracks();
//
//            foreach (var line in lines.Where(line => !string.IsNullOrWhiteSpace(line)))
//            {
//                lineno++;
//
//                var parts = line.Split(delimiter);
//
//                if (parts.Length != 5 && parts.Length != 6)
//                    throw new ParseErrorException(line, lineno);
//
//                var genome = parts[0];
//                var chromosome = parts[1];
//                var label = parts[2];
//                var start = double.Parse(parts[3]);
//                var end = double.Parse(parts[4]);
//
//
//                var color = PredefinedColors.AllColors["Black"];
//                if (parts.Length == 6)
//                {
//                    var colorStr = parts[5];
//                    if (colorStr.StartsWith("#"))
//                        color = color.FromHexString(colorStr);
//                    else
//                        if (PredefinedColors.AllColors.ContainsKey(colorStr))
//                            color = PredefinedColors.AllColors[colorStr];
//                        else
//                            throw new ParseErrorException(colorStr, lineno);
//                }
//
//                var key = string.Format("<{0}><{1}>", genome, chromosome);
//
//                var trackRegion = new CustomTrackRegion(genome, chromosome, start, end, label, color);
//
//                if (!CustomTrackMap.ContainsKey(key))
//                    CustomTrackMap.Add(key, new SmartObservableCollection<CustomTrackRegion>());
//
//                var trackRegions = CustomTrackMap[key];
//                trackRegions.Add(trackRegion);
//            }
//        }
//
//        public void ClearCustomTracks()
//        {
//            CustomTrackMap.ForEach(kvp => kvp.Value.Clear());
//            CustomTrackMap.Clear();
//        }
    }
}
