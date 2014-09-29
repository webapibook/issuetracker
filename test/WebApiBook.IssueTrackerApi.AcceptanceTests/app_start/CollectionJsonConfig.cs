namespace WebApiBook.IssueTrackerApp.AcceptanceTests.SampleApi
{
    using System.Linq;
    using System.Reflection;
    using System.Web.Http;
    using CollectionJsonFormatter;
    using CollectionJsonFormatter.Attributes;
    using CollectionJsonFormatter.Extensions;

    public class CollectionJsonConfig
    {
        public static void Register(HttpConfiguration config)
        {
            RegisterAssemblyAttributes();
            RegisterFluidAttributes();
            RegisterQueries();
            config.Formatters.Add(new CollectionJsonMediaTypeFormatter());
        }

        private static void RegisterFluidAttributes()
        {
            // Register attributes here
            //var type = typeof(YourType);
            //type
            //    .RegisterAttribute<AddCollectionLink>(new AddCollectionLink("/href/to/location", "rel"))
            //    .RegisterAttribute<AddHref>(new AddHref("/href/to/location"))
            //    .RegisterAttribute<AddItemLink>(new AddItemLink("/href/to/location", "rel") { Prompt = "Prompt" })
            //    .RegisterAttribute<AddItemLink>(new AddItemLink("/href/to/location", "rel") { Prompt = "Prompt" })
            //    .RegisterAttribute<AddQuery>(new AddQuery("queryName"))
            //    .RegisterAttribute<AddTemplate>(new AddTemplate());
        }

        private static void RegisterQueries()
        {
            // Register queries
            //CollectionJsonConfiguration.RegisterQuery("queryName", new QueryProperty
            //{
            //    Href = "/href/to/location",
            //    Name = "queryName",
            //    Prompt = "Prompt",
            //    Rel = "rel",
            //    Data = new List<DataProperty>
            //    {
            //        new DataProperty
            //        {
            //            Name = "propertyName",
            //            Prompt = "propertyPrompt",
            //            Value = string.Empty // propertyValue
            //        }
            //    }
            //});
        }

        private static void RegisterAssemblyAttributes()
        {
            var types = Assembly.GetAssembly(typeof(CollectionJsonConfig))
                .GetTypes()
                .Where(t => t.GetCustomAttributes<CollectionJsonAttribute>().Any());

            foreach (var type in types)
            {
                var attributes = type.GetCustomAttributes();
                foreach (var attribute in attributes)
                {
                    type.RegisterAttribute(attribute);
                }

                var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => p.GetCustomAttribute<Prompt>() != null);
                foreach (var property in properties)
                {
                    var prompt = property.GetCustomAttribute<Prompt>();
                    type.RegisterAttribute<Prompt>(prompt, property.Name);
                }
            }
        }
    }
}