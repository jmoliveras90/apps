namespace Trello.Selenium.UI.Interfaces
{
    public interface IColumn
    {        
        string Title { get; }

        ICollection<ICard> Cards { get; }
    }
}
