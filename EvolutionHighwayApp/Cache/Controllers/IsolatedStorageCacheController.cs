using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization;

namespace EvolutionHighwayApp.Cache.Controllers
{
    public class IsolatedStorageCacheController<T>
    {
        private readonly string _folder;

        public IsolatedStorageCacheController(string folder)
        {
            Debug.WriteLine("{0} instantiated", GetType().Name);

            _folder = folder;

            using (var appStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (!appStore.DirectoryExists(folder))
                {
                    Debug.WriteLine("Creating folder {0} in isolated storage", folder);
                    appStore.CreateDirectory(folder);
                }
            }
        }

        public void Store(string filename, T obj)
        {
            //TODO: need to implement cache replacement mechanism
            Debug.WriteLine("Storing {0} in cache...", filename);
            var appStore = IsolatedStorageFile.GetUserStoreForApplication();
            using (var fileStream = appStore.OpenFile(filename, FileMode.Create))
            {
                var serializer = new DataContractSerializer(typeof(T));
                serializer.WriteObject(fileStream, obj);
            }
        }

        public T Retrieve(string filename)
        {
            Debug.WriteLine("Retrieving {0} from {1} cache...", typeof(T).Name, filename);

            var obj = default(T);
            var appStore = IsolatedStorageFile.GetUserStoreForApplication();
            if (appStore.FileExists(filename))
            {
                Debug.WriteLine("Cache hit for {0} in {1}", typeof(T).Name, filename);
                using (var fileStream = appStore.OpenFile(filename, FileMode.Open))
                {
                    var serializer = new DataContractSerializer(typeof(T));
                    obj = (T)serializer.ReadObject(fileStream);
                }
            }
            else
            {
                Debug.WriteLine("Cache miss for {0} in {1}", typeof(T).Name, filename);
            }

            return obj;
        }

        public void Clear()
        {
            Debug.WriteLine("Clearing cache...");
            using (var appStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                var pattern = Path.GetFileName("*");
                var files = appStore.GetFileNames(_folder + "/" + pattern);
                Debug.WriteLine("Removing cache files: " + files);
                foreach (var fileName in files)
                    appStore.DeleteFile(_folder + "/" + fileName);
            }
        }
    }
}
