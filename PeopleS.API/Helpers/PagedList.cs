using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace PeopleS.API.Helpers
{
    public class PagedList<T> : List<T>
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 5;
        public int TotalCount { get; set; }

        public PagedList(List<T> items, int count, int pageNumber)
        {
            TotalCount = count;
            CurrentPage = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)PageSize);
            this.AddRange(items);
        }

        public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source,
        int pageNumber)
        {
            var count = await source.CountAsync();
            var items = await source.Skip((pageNumber - 1) * 5 ).Take(5).ToListAsync();
            return new PagedList<T>(items, count, pageNumber);
        }
    }
}