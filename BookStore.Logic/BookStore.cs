﻿using System;
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

    [Serializable]
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

        /// <summary>
        /// Add book to the storage
        /// </summary>
        /// <param name="item">Book to add</param>
        /// <exception cref="ArgumentNullException">Throws when item is null</exception>
        /// <exception cref="ArgumentException">Throws when you try to add the book to the storage twice</exception>
        public void AddBook(Book item)
        {
            if(item == null)
                throw new ArgumentNullException($"{nameof(item)} must be not null");

            if (books.Contains(item))
                throw new ArgumentException("There is such book in a collection");
            books.Add(item);
        }

        /// <summary>
        /// Remove book from the storage
        /// </summary>
        /// <param name="item">Book to remove</param>
        /// <exception cref="ArgumentNullException">Throws when item is null</exception>
        /// <exception cref="ArgumentException">Throws when you try to remove the missing book from the storage</exception>
        public void RemoveBook(Book item)
        {
            if (item == null)
                throw new ArgumentNullException($"{nameof(item)} must be not null");

            if (!books.Contains(item))
                throw new ArgumentException("There isn't such book in a collection");
            books.Remove(item);
        }

        /// <summary>
        /// Find book by tag
        /// </summary>
        /// <param name="tag">Predicate to find</param>
        /// <returns>Book if there are such book in the storage. nul, otherwise</returns>
        /// <exception cref="ArgumentNullException">Throws when tag is null</exception>
        public Book FindBookByTag(Predicate<Book> tag)
        {
            if (tag == null)
                throw new ArgumentNullException($"{nameof(tag)} must be not null");

            return books.Find(tag);
        }

        /// <summary>
        /// Sort book by tag
        /// </summary>
        /// <param name="cmp">Comparision to sort</param>
        /// <exception cref="ArgumentNullException">Throws when cmp is null</exception>
        public void SortBooksByTag(Comparison<Book> cmp)
        {
            if (cmp == null)
                throw new ArgumentNullException($"{nameof(cmp)} must be not null");

            books.Sort(cmp);
        }

        /// <summary>
        /// Save this storage to the repo
        /// </summary>
        /// <param name="repo">The repo to store</param>
        /// <exception cref="ArgumentNullException">Throws when repo is null</exception>
        public void SaveTo(IBookListStorage repo)
        {
           if (repo == null)
                throw new ArgumentNullException($"{nameof(repo)} must be not null");

            repo.WriteBooks(books);
        }

        /// <summary>
        /// Load books from the repo
        /// </summary>
        /// <param name="repo">The repo to load from</param>
        /// <exception cref="ArgumentNullException">Throws when repo is null</exception>
        public void LoadFrom(IBookListStorage repo)
        {
            if (repo == null)
                throw new ArgumentNullException($"{nameof(repo)} must be not null");

            books = new List<Book>(repo.ReadBooks());
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
}
