using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using RazorPad.Providers;

namespace RazorPad
{
    [Export]
    public class ModelProviders
    {
        public static ModelProviders Current
        {
            get { return _current; }
            set { _current = value; }
        }
        private static volatile ModelProviders _current = new ModelProviders();

        public static IModelProviderFactory DefaultFactory
        {
            get { return _defaultFactory = _defaultFactory ?? new JsonModelProvider.JsonModelProviderFactory(); }
            set { _defaultFactory = value; }
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
            var providerNickname = GetProviderDictionaryKey(factory.GetType().Name);
            if (!_providers.ContainsKey(providerNickname))
                _providers.Add(providerNickname, factory);
        }

        public IModelProvider Create(string provider)
        {
            IModelProviderFactory factory;

            var providerNickname = GetProviderDictionaryKey(provider);

            if (!_providers.TryGetValue(providerNickname, out factory) || factory == null)
            {
                Trace.TraceWarning("Could not find {0} -- falling back to default model provider factory",
                                   providerNickname);
                factory = DefaultFactory;
            }

            return factory.Create();
        }

        private static string GetProviderDictionaryKey(string typeName)
        {
            string providerNickname = new ModelProviderFactoryName(typeName);
            return providerNickname.ToLower();
        }
    }
}