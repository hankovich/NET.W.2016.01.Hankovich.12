using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Logic
{
    public class BinarySerializationStorage : IBookListStorage
    {
        public readonly string path;

        /// <summary>
        /// Get/create new serialization storage for books
        /// </summary>
        /// <param name="currentPath">Path to xml file</param>
        public BinarySerializationStorage(string currentPath)
        {
            if (currentPath == null)
                throw new ArgumentNullException(nameof(currentPath));
            path = currentPath;
        }

        /// <summary>
        /// Read books from repo
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Book> ReadBooks()
        {
            BinaryFormatter formatter = new BinaryFormatter();

            IEnumerable<Book> books;

            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
                 books = (IEnumerable<Book>)formatter.Deserialize(fs);

            return books;
        }

        /// <summary>
        /// Write books to repo
        /// </summary>
        /// <param name="collection">Collection of books to write</param>
        public void WriteBooks(IEnumerable<Book> collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            BinaryFormatter formatter = new BinaryFormatter();

            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
                formatter.Serialize(fs, collection);           
        }
    }
}
