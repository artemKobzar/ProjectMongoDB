namespace ProjectMongoDB.DbContext
{
    public class DbSettings
    {
        public string? ConnectionString { get; set; }
        public string? DbName { get; set; }
        public string? UsersCollectionName { get; set; }
        public string? PassportUsersCollectionName { get; set; }
        public string? UsersImageCollectionName { get; set; }
    }
}
