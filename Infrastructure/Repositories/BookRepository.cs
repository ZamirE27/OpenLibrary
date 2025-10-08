using Microsoft.EntityFrameworkCore;
using OpenLibrary.Infrastructure;
using OpenLibrary.Models;
using OpenLibrary.Repositories.Interfaces;

namespace OpenLibrary.Repositories;

public class BookRepository : IBookRepository<Book>
{
    private readonly AppDbContext _context;

    public BookRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<Book> CreateAsync(Book book)
    {
        _context.Books.Add(book);
        await _context.SaveChangesAsync();
        return book;
        
    }

    public async Task<IEnumerable<Book>> GetAllAsync()
    {
        return await _context.Books
            .ToListAsync();
    }

    public async Task<Book?> GetByTitleAsync(string Title)
    {
        return await _context.Books
            .AsNoTracking()
            .FirstOrDefaultAsync(book => book.Title == Title);
    }

    public async Task<Book?> GetByIdAsync(int id)
    {
        return await _context.Books.AsNoTracking().FirstOrDefaultAsync(book => book.Id == id);   
    }


    public async Task<Book> UpdateAsync(Book book)
    {
        _context.Books.Update(book);
        await  _context.SaveChangesAsync();
        return book;
    }

    public async Task<Book> DeleteAsync(Book book)
    {
        _context.Books.Remove(book);
        await _context.SaveChangesAsync();
        return book;
    }
}