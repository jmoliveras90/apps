namespace Trello.Selenium.UI
{
    public interface IBoard 
    {
        public ICollection<Column> Columns { get; }
    }
}
