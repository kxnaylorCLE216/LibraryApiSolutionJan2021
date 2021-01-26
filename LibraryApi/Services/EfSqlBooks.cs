using AutoMapper;
using AutoMapper.QueryableExtensions;
using LibraryApi.Domain;
using LibraryApi.Models.Books;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryApi.Services
{
    public class EfSqlBooks : ILookupBooks, IBookCommands
    {
        private LibraryDataContext _context;
        private IMapper _mapper;
        private MapperConfiguration _config;

        public EfSqlBooks(LibraryDataContext context, IMapper mapper, MapperConfiguration config)
        {
            _context = context;
            _mapper = mapper;
            _config = config;
        }

        public async Task<GetBookDetailsResponse> GetBookById(int id)
        {
            var response = await _context.GetBooksInInventory()
                .ProjectTo<GetBookDetailsResponse>(_config)
                .Where(b => b.Id == id)
                .SingleOrDefaultAsync();
            return response;
        }

        public async Task<GetBooksResponse> GetBooks(string genre)
        {
            var response = new GetBooksResponse();
            var booksQuery = _context.GetBooksInInventory()
                .ProjectTo<GetBooksResponseItem>(_config);

            if (genre != "All")
            {
                booksQuery = booksQuery.Where(b => b.Genre == genre);
            }
            response.Data = await booksQuery.ToListAsync();
            response.NumberOfBooks = response.Data.Count;
            response.Genre = genre;

            return response;
        }

        public async Task RemoveBookFromInventory(int id)
        {
            var storedBook = await _context.GetBooksInInventory().SingleOrDefaultAsync(b => b.Id == id);
            if (storedBook != null)
            {
                storedBook.IsInInventory = false;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> UpdateBookGenre(int id, string genre)
        {
            var storedBook = await _context.GetBooksInInventory().SingleOrDefaultAsync(b => b.Id == id);
            if (storedBook != null)
            {
                storedBook.Genre = genre; // you need to validate it.
                await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<GetBookDetailsResponse> AddBookToInventory(PostBookRequest request)
        {
            // PostBookRequest -> Book
            var bookToAdd = _mapper.Map<Book>(request);
            _context.Books.Add(bookToAdd);
            await _context.SaveChangesAsync();

            // 3. Return:
            //    - Status Code 201 (Created)
            //    - Add a birth announcement. That is a location header with the URL of
            //      the newly created resource. e.g. Location: http://localhost:1337/books/42
            //    - It is super nice to just give them a copy of the newly created resource
            //      This must be exactly the same as they would get by following the location header.
            GetBookDetailsResponse response = _mapper.Map<GetBookDetailsResponse>(bookToAdd);

            return response;
            //return CreatedAtRoute("books#getbookbyid", new { id = response.Id }, response);
        }
    }
}