using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace PiggyBank.Models
{
    public class TagManager : ITagManager
    {
        private IPiggyBankDbContext _dbContext;

        public TagManager(IPiggyBankDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Tag>> ListTags(int bookId)
        {
            var q = _dbContext.Tags.Where(b => b.Book.Id == bookId)
                .OrderBy(b => b.Name);
            return await q.ToListAsync();
        }

        public async Task<Tag> CreateTag(Book book, Tag tag)
        {
            if (book == null) throw new PiggyBankDataException("Book object is missing");
            if (tag == null) throw new PiggyBankDataException("Tag object is missing");

            tag.Book = book;
            PiggyBankUtility.CheckMandatory(tag);
            _dbContext.Tags.Add(tag);
            await _dbContext.SaveChangesAsync();

            return tag;
        }

        public async Task<Tag> FindTag(int tagId, bool populateBook = false)
        {
            var q = _dbContext.Tags.Where(b => b.Id == tagId);
            if (populateBook) { q = q.Include(b => b.Book); }
            var tags = await q.ToListAsync();
            if (!tags.Any()) throw new PiggyBankDataNotFoundException("Tag [" + tagId + "] cannot be found");
            return tags.First();
        }

        public async Task<Tag> UpdateTag(Tag tag)
        {
            if (tag == null) throw new PiggyBankDataException("Tag object is missing");
            Tag tagToUpdate = await FindTag(tag.Id);

            PiggyBankUtility.CheckMandatory(tag);
            PiggyBankUtility.UpdateModel(tagToUpdate, tag);
            await _dbContext.SaveChangesAsync();

            return tagToUpdate;
        }
    }
}
