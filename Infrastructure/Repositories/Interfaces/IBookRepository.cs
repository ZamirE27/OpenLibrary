using OpenLibrary.Models;

namespace OpenLibrary.Repositories.Interfaces;

public interface IBookRepository<T> where T : Book
{
    Task<T> CreateAsync(T book);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByTitleAsync(string Title);
    Task<T> GetByIdAsync(int id);
    Task<T> UpdateAsync(T book);
    Task<T> DeleteAsync(T book);
}