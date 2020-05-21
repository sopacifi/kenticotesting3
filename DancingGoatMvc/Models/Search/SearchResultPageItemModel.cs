using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.Search;

namespace DancingGoat.Models.Search
{
    public class SearchResultPageItemModel : SearchResultItemModel
    {
        public string Type { get; set; }


        public SearchResultPageItemModel(SearchResultItem resultItem, TreeNode treeNode)
            : base(resultItem)
        {
            var className = treeNode.ClassName;
            var dataClassInfo = DataClassInfoProvider.GetDataClassInfo(className);

            Type = dataClassInfo.ClassDisplayName;
        }
    }
}