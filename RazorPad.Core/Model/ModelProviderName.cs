namespace RazorPad
{
    public class ModelProviderName : SimpleName
    {
        public const string NameSuffix = "ModelProvider";

        public ModelProviderName(object objectOrType)
            : base(objectOrType, NameSuffix)
        {
        }

        public ModelProviderName(string name)
            : base(name, NameSuffix)
        {
        }
    }

    public class ModelProviderFactoryName : SimpleName
    {
        public const string NameSuffix = "ModelProviderFactory";

        public ModelProviderFactoryName(object objectOrType)
            : base(objectOrType, NameSuffix)
        {
        }

        public ModelProviderFactoryName(string name)
            : base(name, NameSuffix)
        {
        }
    }
}