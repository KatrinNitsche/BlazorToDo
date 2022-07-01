using CollaborateSoftware.MyLittleHelpers.Backend.Data;
using System.Collections.Generic;

namespace CollaborateSoftware.MyLittleHelpers.Pages
{
    public partial class Categories
    {
        public IEnumerable<Category> CategoryList { get; set; }
        public string SearchTerm { get; set; }
        public string SortingColumn { get; set; }
        public string SortingDirection { get; set; } = "Asc";

        protected void AddCategory()
        {

        }

        protected async void SortByColumn(string columnName)
        {

        }

        protected async void DeleteCategory(int id)
        {

        }

        protected async void EditCategory(int id)
        {

        }
    }
}
