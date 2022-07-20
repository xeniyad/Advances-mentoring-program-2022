using System.Collections;
using System.Runtime.Serialization;
using CatalogService.Core.ProjectAggregate;

namespace CatalogService.Application.Categories.Models;

[CollectionDataContract(Name = "Categories", Namespace = "")]
    public sealed class CategoryList : IEnumerable<Category>
    {
        private readonly List<Category> _users = new List<Category>();

        public CategoryList()
        {
        }

        public CategoryList(IEnumerable<Category> collection)
        {
            _users = new List<Category>(collection);
        }

        internal void Add(Category item)
        {
            _users.Add(item);
        }

        public IEnumerator<Category> GetEnumerator() => _users.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _users.GetEnumerator();
    }
