using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PiggyBank.Models
{
    public interface ITagManager
    {
        Task<IEnumerable<Tag>> ListTags(int bookId);
        Task<Tag> CreateTag(Book book, Tag tag);
        Task<Tag> FindTag(int tagId, bool populateBook = false);
        Task<Tag> UpdateTag(Tag tag);
    }
}
