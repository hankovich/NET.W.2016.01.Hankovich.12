using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Logic
{
    public class BinaryStorage : IBookListStorage
    {
        public readonly string path;

        /// <summary>
        /// Get/create new binary storage for books
        /// </summary>
        /// <param name="currentPath">Path to binary file</param>
        public BinaryStorage(string currentPath)
        {
            path = currentPath;
        }

        /// <summary>
        /// Read all books from the binary file
        /// </summary>
        /// <returns>Collection of books</returns>
        /// <exception cref="FileNotFoundException">Throws when there isn't file with <see cref="path"/> name</exception>
        /// <exception cref="IOException">Throws when is something wrong with you</exception>
        /// <exception cref="ArgumentException">Throws when is something wrong with you</exception>
        /// <exception cref="ArgumentNullException">Throws when is something wrong with you</exception>
        /// <exception cref="ObjectDisposedException">Throws when is something wrong with you</exception>
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

        /// <summary>
        /// Write all books to the binary file
        /// </summary>
        /// <param name="collection">Book collection to write</param>
        /// <exception cref="FileNotFoundException">Throws when there isn't file with <see cref="path"/> name</exception>
        /// <exception cref="IOException">Throws when is something wrong with you</exception>
        /// <exception cref="ArgumentException">Throws when is something wrong with you</exception>
        /// <exception cref="ArgumentNullException">Throws when is something wrong with you</exception>
        /// <exception cref="ObjectDisposedException">Throws when is something wrong with you</exception>
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
