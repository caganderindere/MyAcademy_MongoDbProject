namespace Travel.Web.Settings
{
    public interface IDataBaseSettings
    {

        public string ConnectionString { get; set; }

        public string DatabaseName { get; set; }

        public string BannerColectionName { get; set; }

        public string RouteCollectionName { get; set; }

    }
}

