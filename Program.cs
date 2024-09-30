using Spectre.Console;

class Program
{
    // Metodo principale (entry point) del programma
    static void Main(string[] args)
    {
        // Inizializzazione del contesto del database (Database estende DbContext di EF)
        using (var db = new Database()) // Model (DbContext)
        {
            // Inizializzazione della vista (View), che riceve il contesto del database
            var view = new View(db); // View
            
            // Inizializzazione del controller, che collega il modello (DbContext) e la vista
            var controller = new Controller(db, view); // Controller
            
            // Avvio del menu principale
            controller.MainMenu(); // Menu principale dell'app
        }
    }
}
