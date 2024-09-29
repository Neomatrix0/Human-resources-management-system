using System.Data.SQLite;
using Spectre.Console;

class Program
{
     // Metodo principale (entry point) del programma
    static void Main(string[] args)
    {
         // Inizializzazione del modello (Database)
        var db = new Database(); // Model
         // Inizializzazione della vista (View), che riceve il modello
        var view = new View(db); // View
        // Inizializzazione del controller, che collega il modello e la vista
        var controller = new Controller(db, view); // Controller
        controller.MainMenu(); // Menu principale dell'app

        
    }
}