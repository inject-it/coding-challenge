using System.Collections.Generic;
using System.Linq;
using System;

namespace ConstructionLine.CodingChallenge
{
    public class SearchEngine
    {
        private readonly List<Shirt> _shirts;
        private readonly IDictionary<Guid, List<Shirt>> _shirtLookup = new Dictionary<Guid, List<Shirt>>();

        public SearchEngine(List<Shirt> shirts)
        {
            _shirts = shirts;

            // Filter all colours and sizes into a pre-processed dictionary for fast lookups e.g. red -> (set of red shirts)
            Color.All.ForEach(color => _shirtLookup.Add(color.Id, _shirts.Where(shirt => shirt.Color.Id == color.Id).ToList()));
            Size.All.ForEach(size => _shirtLookup.Add(size.Id, _shirts.Where(shirt => shirt.Size.Id == size.Id).ToList()));
        }

        public SearchResults Search(SearchOptions options)
        {          
            var setOfRequiredColors = options.Colors.Any()
                ? options.Colors.SelectMany(color => _shirtLookup[color.Id])
                : Color.All.SelectMany(color => _shirtLookup[color.Id]);

            var setOfRequiredSizes = options.Sizes.Any()
                ? options.Sizes.SelectMany(size => _shirtLookup[size.Id])
                : Size.All.SelectMany(size => _shirtLookup[size.Id]);

            var setOfAllMatchingShirts = setOfRequiredColors.Intersect(setOfRequiredSizes).ToList();
      
            return new SearchResults
            {
                Shirts = setOfAllMatchingShirts,
                ColorCounts = Color.All.Select(color => new ColorCount { Color = color, Count = setOfAllMatchingShirts.Select(o => o.Color.Id).Count(o => o == color.Id) }).ToList(),
                SizeCounts = Size.All.Select(size => new SizeCount { Size = size, Count = setOfAllMatchingShirts.Select(o => o.Size.Id).Count(o => o == size.Id) }).ToList()
            };
        }
    }
}