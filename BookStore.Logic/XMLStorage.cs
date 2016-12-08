using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace BookStore.Logic
{
    public class XmlStorage: IBookListStorage
    {
        public readonly string path;
       
        /// <summary>
        /// Get/create new xml storage for books
        /// </summary>
        /// <param name="currentPath">Path to xml file</param>
        public XmlStorage(string currentPath)
        {
            path = currentPath;
        }

        /// <summary>
        /// Read all books from the xml file
        /// </summary>
        /// <returns>Collection of books</returns>
        /// <exception cref="FileNotFoundException">Throws when there isn't file with <see cref="path"/> name</exception>
        /// <exception cref="IOException">Throws when is something wrong with you</exception>
        /// <exception cref="ArgumentException">Throws when is something wrong with you</exception>
        /// <exception cref="ArgumentNullException">Throws when is something wrong with you</exception>
        /// <exception cref="ObjectDisposedException">Throws when is something wrong with you</exception>
        public IEnumerable<Book> ReadBooks()
        {
            XDocument doc = XDocument.Load(path);
            try
            {
                IEnumerable<XElement> xElements = doc.Root?.Elements();
                List<Book> storage =
                    xElements.Select(
                        el =>
                            new Book(el.Attribute("author").Value, el.Attribute("title").Value,
                                int.Parse(el.Attribute("publishingYear").Value), el.Attribute("genre").Value,
                                int.Parse(el.Attribute("numberOfPages").Value))).ToList();

                return storage;
            }
            catch (NullReferenceException)
            {
                return null;
            }           
        }

        /// <summary>
        /// Write all books to the xml file
        /// </summary>
        /// <param name="collection">Book collection to write</param>
        /// <exception cref="FileNotFoundException">Throws when there isn't file with <see cref="path"/> name</exception>
        /// <exception cref="IOException">Throws when is something wrong with you</exception>
        /// <exception cref="ArgumentException">Throws when is something wrong with you</exception>
        /// <exception cref="ArgumentNullException">Throws when is something wrong with you</exception>
        /// <exception cref="ObjectDisposedException">Throws when is something wrong with you</exception>
        public void WriteBooks(IEnumerable<Book> collection)
        {
            XDocument doc = new XDocument();
            XElement elem = new XElement("library");
            doc.Add(elem);
            foreach (var book in collection)
            {
                XElement bookElement = new XElement("book");
                XElement author = new XElement("author") {Value = book.Author};
                XElement title = new XElement("title") { Value = book.Title };
                XElement publishingYear = new XElement("publishingYear") { Value = book.PublishingYear.ToString() };
                XElement genre = new XElement("genre") { Value = book.Genre };
                XElement numberOfPages = new XElement("numberOfPages") { Value = book.NumberOfPages.ToString() };
                bookElement.Add(author);
                bookElement.Add(title);
                bookElement.Add(publishingYear);
                bookElement.Add(genre);
                bookElement.Add(numberOfPages);
                doc.Root?.Add(bookElement);
            }
            doc.Save(path);
        }
    }
}
