using LibraryApi.Models.Books;
using System.Threading.Tasks;

namespace LibraryApi
{
    public interface IBookCommands
    {
        Task RemoveBookFromInventory(int id);
        Task<bool> UpdateBookGenre(int id, string genre);
        Task<GetBookDetailsResponse> AddBookToInventory(PostBookRequest request);
    }
}