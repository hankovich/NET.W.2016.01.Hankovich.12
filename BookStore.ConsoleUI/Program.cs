using BookStore.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.ConsoleUI
{
    class Program
    {
        static void Main()
        {
            Book holyBible = new Book("God", "Holy Bible", int.MinValue, "Lovestory", 1372);
            Book oneNineEightFour = new Book("George Orwell", "1984", 1949, "Dystopian", 267);
            Book weChildrenFromBahnhofZoo = new Book("Christiane F.", "We Children from Bahnhof Zoo", 1980, "Biography", 300);
            Book harryPotterAndTheGobletOfFire = new Book("J. K. Rowling", "Harry Potter and the Goblet of Fire", 2000, "Fantasy", 636);

            BinaryStorage byn = new BinaryStorage("books.bin");
            XmlStorage xml = new XmlStorage("books.xml");
            BinarySerializationStorage bss = new BinarySerializationStorage("books.dat");
            
            BookListService service = new BookListService();

            service.AddBook(holyBible);
            service.AddBook(oneNineEightFour);
            service.AddBook(weChildrenFromBahnhofZoo);
            service.AddBook(harryPotterAndTheGobletOfFire);

            service.SaveTo(byn);
            service.SaveTo(xml);
            service.SaveTo(bss);

            var bookByTag = service.FindBookByTag(t => t.Genre == "Dystopian");
            Console.WriteLine(bookByTag);
            service.RemoveBook(bookByTag);
            bookByTag = service.FindBookByTag(t => t.Genre == "Dystopian");
            Console.WriteLine(bookByTag);

            service.LoadFrom(bss);

            service.SortBooksByTag((t1, t2) => t1.PublishingYear > t2.PublishingYear ? 1 : -1);

            foreach (var book in service)
            {
                Console.WriteLine($"{book}\n\n");
            }
            Console.ReadKey();
        }
    }
}
