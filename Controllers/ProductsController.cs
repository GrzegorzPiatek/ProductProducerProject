using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PWProject.Migrations;
using PWProject.Models;
using AlcoholType = PWProject.Models.AlcoholType;

namespace PWProject.Controllers
{
    public class ProductsController : Controller
    {
        private readonly DataContext _context;

        public ProductsController(DataContext context)
        {
            _context = context;
        }

        // GET: Products
        public IActionResult Index()
        {
            var sortColumn = Request.Query["sortColumn"];
            var sortOrder = Request.Query["sortOrder"];
            var filterValue = Request.Query["filterValue"];
            var alcoholType = Request.Query["alcoholType"];
            // Build the query using LINQ methods and execute the query only when the results need to be materialized.
            var products = GetProductsFromDatabase(filterValue, sortColumn, sortOrder, alcoholType);
            // Return the filtered and sorted products to the view.
            var listProducts = products.ToList();
            return View(listProducts);
        }

        private IQueryable<Product> GetProductsFromDatabase(string filterValue, string sortColumn, string sortOrder, string alcoholType)
        {
            // Build the query using LINQ methods.
            var query = _context.Products
                .Include(p => p.Producer)
                .AsQueryable();
            if (!string.IsNullOrEmpty(filterValue))
            {
                query = query.Where(p => p.Name.Contains(filterValue) || p.Description.Contains(filterValue) || p.Producer.Name.Contains(filterValue));
            }
            if (!string.IsNullOrEmpty(sortColumn))
            {
                if (sortColumn == "Name")
                {
                    if (sortOrder == "asc")
                    {
                        query = query.OrderBy(p => p.Name);
                    }
                    else
                    {
                        query = query.OrderByDescending(p => p.Name);
                    }
                }
                else if (sortColumn == "Producer.Name")
                {
                    if (sortOrder == "asc")
                    {
                        query = query.OrderBy(p => p.Producer.Name);
                    }
                    else
                    {
                        query = query.OrderByDescending(p => p.Producer.Name);
                    }
                }
            }
            if (alcoholType != "" && alcoholType != null)
            {
                AlcoholType selectedAlcoholType;

                // Try to parse the filterValue as an AlcoholType enum value.
                // If it's not a valid AlcoholType value, set it to the default value.
                Enum.TryParse(alcoholType, out selectedAlcoholType);
                query = query.Where(p => p.AlcoholType == selectedAlcoholType);
            }
            ViewData["AlcoholTypes"] = Enum.GetValues(typeof(AlcoholType)).Cast<AlcoholType>();

            // Execute the query and materialize the results.
            return query;
        }

        private IQueryable<Product> ApplyAlcoholTypeFiltering(IQueryable<Product> products, string? alcoholType)
        {

            if (alcoholType != null)
            {
                AlcoholType selectedAlcoholType;

                // Try to parse the filterValue as an AlcoholType enum value.
                // If it's not a valid AlcoholType value, set it to the default value.
                if (!Enum.TryParse(alcoholType, out selectedAlcoholType))
                {
                    alcoholType = default;
                }
                products = products.Where(p => p.AlcoholType.Equals(alcoholType));
            }

            return products;
        }

        //// GET: Products
        //public IActionResult Index()
        //{
        //    var sortColumn = Request.Query["sortColumn"];
        //    var filterValue = Request.Query["filterValue"];
        //    AlcoholType alcoholType;
        //    List<Product> products = new List<Product>();
        //    try
        //    {
        //        alcoholType = (AlcoholType)Enum.Parse(typeof(AlcoholType), filterValue);
        //    }
        //    catch (ArgumentException)
        //    {
        //        alcoholType = default;
        //    }
        //    if (!string.IsNullOrEmpty(sortColumn))
        //    {
        //        if (sortColumn == "Name")
        //        {
        //            products = _context.Products
        //                .Include(p => p.Producer)
        //                .OrderBy(p => p.Name)
        //                .ToList();
        //        }
        //        else if (sortColumn == "Producer.Name")
        //        {
        //            products = _context.Products
        //                .Include(p => p.Producer)
        //                .OrderBy(p => p.Producer.Name)
        //                .ToList();
        //        }
        //    }

        //    if (!string.IsNullOrEmpty(filterValue))
        //    {
        //        products = _context.Products
        //            .Include(p => p.Producer)
        //            .Where(
        //                p => p.Name.Contains(filterValue) ||
        //                p.Producer.Name.Contains(filterValue) ||
        //                p.AlcoholType.Equals(alcoholType)
        //            )
        //            .ToList();
        //    }

        //    if (!products.Any())
        //    {
        //        products = _context.Products
        //            .Include(p => p.Producer)
        //            .ToList();
        //    }

        //    return View(products);
        //}

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            ViewBag.Producers = _context.Producers.Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Name }).ToList();

            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Producer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            ViewBag.Producers = _context.Producers.Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Name }).ToList();
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            product.Producer = _context.Producers.Find(product.ProducerId);

            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Producers = _context.Producers.Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Name }).ToList();
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ViewBag.Producers = _context.Producers.Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Name }).ToList();

            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["ProducerId"] = new SelectList(_context.Producers, "Id", "Id", product.ProducerId);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,ProducerId, AlcoholType")] Product product)
        {
            ViewBag.Producers = _context.Producers.Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Name }).ToList();

            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Producer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Products == null)
            {
                return Problem("Entity set 'DataContext.Products'  is null.");
            }
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
