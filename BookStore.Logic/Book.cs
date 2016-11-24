using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Logic
{
    public interface IBookListStorage
    {
        IEnumerable<Book> ReadBooks();

        void WriteBooks(IEnumerable<Book> collection);
    }

    public class Book : IEquatable<Book>, IComparable, IComparable<Book>
    {
        #region Properties

        private string author;
        private string title;
        private string genre;
        private int numberOfPages;

        public string Author
        {
            get
            {
                return author;
            }
            set
            {
                if(string.IsNullOrEmpty(value))
                    throw new ArgumentException("Enter correct string");
                author = value;
            } 
        }
        public string Title {
            get
            {
                return title;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentException("Enter correct string");
                title = value;
            }
        }
        public int PublishingYear { get; set; }
        public string Genre {
            get
            {
                return genre;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentException("Enter correct string");
                genre = value;
            }
        }
        public int NumberOfPages {
            get
            {
                return numberOfPages;
            }
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Number of pages must be greater than 0");
                numberOfPages = value;
            }
        }

        #endregion
        #region Object methods
        public override string ToString()
        {
            return $"Author: {Author}\nTitle: {Title}\nPublishing Year: {PublishingYear}\nGenre: {Genre}\nNumber Of Pages: {NumberOfPages}";
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((Book) obj);
        }

        public override int GetHashCode()
        {
            return RsHash(ToString());
        }
        #endregion
        public Book(string author, string title, int publishingYear, string genre, int numberOfPages)
        {
            Author = author;
            Title = title;
            PublishingYear = publishingYear;
            Genre = genre;
            NumberOfPages = numberOfPages;
        }
        #region Interfaces methods
        public bool Equals(Book other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return ToString().Equals(other.ToString());
        }

        public int CompareTo(Book other)
        {
            if (other == null)
                throw new ArgumentNullException($"{nameof(other)} is null");
            int ret;
            ret = string.Compare(Author, other.Author, StringComparison.Ordinal);
            if (ret != 0)
                return ret;
            ret = string.Compare(Title, other.Title, StringComparison.Ordinal);
            if (ret != 0)
                return ret;
            ret = PublishingYear.CompareTo(other.PublishingYear);
            if (ret != 0)
                return ret;
            ret = string.Compare(Genre, other.Genre, StringComparison.Ordinal);
            if (ret != 0)
                return ret;
            ret = NumberOfPages.CompareTo(other.NumberOfPages);
            return ret;
        }

        public int CompareTo(object obj)
        {
            if (!(obj is Book))
                throw new ArgumentException($"{nameof(obj)} is not a book");
            return CompareTo((Book) obj);
        }
        #endregion
        #region Private methods
        private static int RsHash(string str)
        {
            const int b = 378551;
            int a = 63689;
            int hash = 0;

            foreach (char t in str)
            {
                hash = hash * a + t;
                a *= b;
            }
            return hash;
        }
        #endregion
    }

    public class BookListService: IEnumerable<Book>
    {
        public List<Book> books; 
        /*AddBook (добавить книгу, если такой книги нет, в противном случае выбросить исключение); 
          RemoveBook (удалить книгу, если она есть, в противном случае выбросить исключение); 
          FindBookByTag (найти книгу по заданному критерию); 
          SortBooksByTag (отсортировать список книг по заданному критерию). 
         */

        public BookListService()
        {
            books = new List<Book>();
        }

        public BookListService(IEnumerable<Book> storage):this()
        {
            if (storage == null)
                throw new ArgumentNullException($"{nameof(storage)} must be not null");
            foreach (var book in storage)
            {
                books.Add(book);
            }
        }

        public void AddBook(Book item)
        {
            if(item == null)
                throw new ArgumentNullException($"{nameof(item)} must be not null");

            if (books.Contains(item))
                throw new ArgumentException("There is such book in a collection");
            books.Add(item);
        }

        public void RemoveBook(Book item)
        {
            if (item == null)
                throw new ArgumentNullException($"{nameof(item)} must be not null");

            if (!books.Contains(item))
                throw new ArgumentException("There isn't such book in a collection");
            books.Remove(item);
        }

        public Book FindBookByTag(Predicate<Book> tag)
        {
            if (tag == null)
                throw new ArgumentNullException($"{nameof(tag)} must be not null");

            return books.Find(tag);
        }

        public void SortBooksByTag(Comparison<Book> cmp)
        {
            if (cmp == null)
                throw new ArgumentNullException($"{nameof(cmp)} must be not null");

            books.Sort(cmp);
        }

        public void SaveTo(IBookListStorage storage)
        {
           if (storage == null)
                throw new ArgumentNullException($"{nameof(storage)} must be not null");

            storage.WriteBooks(books);
        }

        public void LoadFrom(IBookListStorage storage)
        {
            if (storage == null)
                throw new ArgumentNullException($"{nameof(storage)} must be not null");

            books = new List<Book>(storage.ReadBooks());
        }

        public IEnumerator<Book> GetEnumerator()
        {
            return books.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class BinaryStorage : IBookListStorage
    {
        public readonly string path;

        public BinaryStorage(string currentPath)
        {
            path = currentPath;
        }

        public IEnumerable<Book> ReadBooks()
        {
            List<Book> storage = new List<Book>();
            using (var read = new BinaryReader(new FileStream(path, FileMode.Open)))
            {
                while (read.BaseStream.Position != read.BaseStream.Length)
                {
                    string author = read.ReadString();
                    string title = read.ReadString();
                    int publishingYear = read.ReadInt32();
                    string genre = read.ReadString();
                    int numberOfPages = read.ReadInt32();
                    storage.Add(new Book(author, title, publishingYear, genre, numberOfPages));
                }
            }
            return storage;
        }

        public void WriteBooks(IEnumerable<Book> collection)
        {
            using (var write = new BinaryWriter(new FileStream(path, FileMode.OpenOrCreate)))
            {
                foreach (var book in collection)
                {
                    write.Write(book.Author);
                    write.Write(book.Title);
                    write.Write(book.PublishingYear);
                    write.Write(book.Genre);
                    write.Write(book.NumberOfPages);
                }
            }
        }
    }
}
