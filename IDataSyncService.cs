namespace PlantApp
{
    public interface IDataSyncService
    {
        void GetContentFromRepository();
        void GetLocalContent();
        string ContentParsing(string content);
        bool IsSomethingNewToUpdate();
        void UpdateLocalContent();
    }
}
