using System.Data.SQLite;
using Spectre.Console;
class View
{
    private Database _db; // Riferimento al modello Database

// Costruttore della classe View che riceve un oggetto 'Database' come argomento e lo assegna al campo privato _db.
    public View(Database db)
    {
        _db = db; // Inizializzazione del riferimento al modello Database
    }

      // Metodo per mostrare il menu principale e restituire la scelta dell'utente
    public string MostraMenuPrincipale()
    {
        // Genera e visualizza il menu con Spectre.Console
        var input = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
            .Title("GESTIONALE DIPENDENTI")
            .PageSize(8)
            .MoreChoicesText("[grey](Move up and down to reveal more)[/]")
            .AddChoices(new[] {
                "Aggiungi Dipendente", "Mostra Dipendenti", "Rimuovi Dipendente",
                "Cerca Dipendente", "Modifica Dipendente","Ordina stipendi","Aggiungi indicatori","Tasso di presenza","Valutazione per fatturato","Incidenza percentuale", "Esci",
            }));

        return input; // Restituisce la scelta dell'utente
    }


  // Metodo per mostrare la lista dei dipendenti (come stringhe) ricevuta in input
    public void MostraDipendenti(List<string> users)
    {
        foreach (var user in users)
        {
            Console.WriteLine(user); // Visualizzazione dei nomi dei dipendenti
        }
    }

// Metodo per raccogliere input dell'utente da console
    public string GetInput()
    {
        return Console.ReadLine();  // Ritorna il testo inserito dall'utente nella console
    }
}