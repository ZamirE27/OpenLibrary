using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using OpenLibrary.Infrastructure.Services;
using OpenLibrary.Models;
using OpenLibrary.Repositories.Interfaces;

namespace OpenLibrary.Controllers;

public class BookController : Controller
{
    private readonly IBookRepository<Book> _bookRepository;
    private readonly OpenLibraryService _openLibraryService;

    public BookController(IBookRepository<Book> bookRepository, OpenLibraryService  openLibraryService)
    {
        _bookRepository = bookRepository;
        _openLibraryService = openLibraryService;
    }
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var books = await  _bookRepository.GetAllAsync();
        return View(books);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Book book)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return View(book);
            }
            await _bookRepository.CreateAsync(book);
            return RedirectToAction(nameof(Index));
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine($"Error: Could not create book: {e.Message}");
            throw;
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var bookId = await _bookRepository.GetByIdAsync(id);
        if (bookId == null)
        {
            return NotFound();
        }
        return View(bookId);  
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Book book)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return View(book);
            }
            var BookExists = await _bookRepository.GetByIdAsync(book.Id);
            if (BookExists == null)
            {
                ModelState.AddModelError(string.Empty, "Book not found");
                return View(book);
            }
            await _bookRepository.UpdateAsync(book);
            TempData["SuccessMessage"] = $"Book {book.Title} has been successfully Updated";
            return RedirectToAction(nameof(Index));
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine($"Error: Could not update book: {e.Message}");
            throw;
        }
    }

    [HttpGet]
    public async Task<IActionResult> Delete(Book book)
    {
        try
        {
            if (book.Id == null)
            {
                TempData["ErrorMessage"] = "Book not found";
                return RedirectToAction(nameof(Index));
            }
            await _bookRepository.DeleteAsync(book);
            TempData["SuccessMessage"] = $"Book {book.Title} has been successfully deleted";
            return RedirectToAction(nameof(Index));
        }   
        catch (HttpRequestException e)
        {
            Console.WriteLine($"Error: Could not delete book: {e.Message}");
            throw;
        }
    }

    [HttpGet]
    public async Task<IActionResult> Search()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Search(string Query)
    {
        if (string.IsNullOrWhiteSpace(Query))
        {
            ViewBag.Error = "Please enter a book title to search";
            return View();
        }
        var result = await _openLibraryService.SearchBookAync(Query);
        
        var localBooks = await _bookRepository.GetAllAsync();
        
        var localTitles = localBooks.Select(b => b.Title.Trim().ToLower()).ToHashSet();
        var bookMatch = new List<string>();
        if (result == null || result.Docs == null || result.Docs.Count == 0)
        {
            ViewBag.Error = $"Could not get the results for \"{Query}\".";
            return View();
        }

        foreach (var doc in result.Docs)
        {
            if (!string.IsNullOrWhiteSpace(doc.Title) && localTitles.Contains(doc.Title.Trim().ToLower()))
            {
                bookMatch.Add(doc.Title);
            }
        }

        if (bookMatch.Any())
        {
            ViewBag.Match = $"The book {bookMatch[0]} matches with one of our stored books";
        }
        return View(result);
    }
}