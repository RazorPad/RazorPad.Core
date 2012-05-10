using System.Collections.Generic;
using System.ComponentModel.Composition;
using NLog;
using RazorPad.Providers;

namespace RazorPad
{
    [Export]
    public class ModelProviders
    {
        protected static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public static ModelProviders Current
        {
            get { return _current; }
            set { _current = value; }
        }
        private static volatile ModelProviders _current = new ModelProviders();

        public static IModelProviderFactory DefaultFactory
        {
            get
            {
                if(_defaultFactory == null)
                {
                    Log.Debug("No default provider factory registered - falling back to JsonModelProviderFactory");
                    _defaultFactory = new JsonModelProvider.JsonModelProviderFactory();
                }

                return _defaultFactory;
            }
            set
            {
                _defaultFactory = value;
                
                if(_defaultFactory != null)
                    Log.Debug("Default Model Provider Factory set to {0}", _defaultFactory.GetType().Name);
            }
        }
        private static volatile IModelProviderFactory _defaultFactory;

        public IEnumerable<KeyValuePair<string, IModelProviderFactory>> Providers
        {
            get { return _providers; }
        }
        private readonly IDictionary<string, IModelProviderFactory> _providers;


        [ImportingConstructor]
        public ModelProviders([ImportMany]IEnumerable<IModelProviderFactory> factories = null)
        {
            _providers = new Dictionary<string, IModelProviderFactory>();

            var providerFactories = factories ?? new IModelProviderFactory[] {
                                        new BasicModelProvider.BasicModelProviderFactory(), 
                                        new JsonModelProvider.JsonModelProviderFactory(),
                                    };

            foreach (var factory in providerFactories)
                Add(factory);
        }

        public void Add(IModelProviderFactory factory)
        {
            var factoryTypeName = factory.GetType().Name;
            var providerNickname = GetProviderDictionaryKey(factoryTypeName);
            
            if (_providers.ContainsKey(providerNickname))
            {
                Log.Debug("Model Provider already registered for {0} -- skipping...", providerNickname);
                return;
            }

            _providers.Add(providerNickname, factory);

            Log.Info("Added model provider factory {0} ({1})", 
                     providerNickname, factoryTypeName);
        }

        public IModelProvider Create(string provider)
        {
            Log.Debug("Creating model provider for {0}...", provider);

            IModelProviderFactory factory;

            var providerNickname = GetProviderDictionaryKey(provider);

            if (_providers.TryGetValue(providerNickname, out factory) && factory != null)
            {
                Log.Debug("Found registered model provider {0}", factory.GetType().Name);
            }
            else
            {
                Log.Warn("Could not find {0} -- falling back to default model provider factory",
                         providerNickname);

                factory = DefaultFactory;
            }

            Log.Debug("Creating {0}...", factory.GetType().Name);

            return factory.Create();
        }

        private static string GetProviderDictionaryKey(string typeName)
        {
            string providerNickname = new ModelProviderFactoryName(typeName);
            return providerNickname.ToLower();
        }
    }
}