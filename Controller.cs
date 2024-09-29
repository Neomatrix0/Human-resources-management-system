using System.Data.SQLite;
using Spectre.Console;
// classe Controller funge da collegamento tra il Model Database e View
// gestice la logica dell'applicazione
class Controller
{
    private Database _db; // Riferimento al modello
    private View _view; // Riferimento alla vista

    // Costruttore della classe Controller
    public Controller(Database db, View view)
    {
        _db = db; // Inizializzazione del riferimento al modello
        _view = view; // Inizializzazione del riferimento alla vista
    }
    
  // Metodo principale che gestisce il menu tramite spectre console  richiamato dalla funzione MonstraMenuPrincipale
    public void MainMenu()
    {
        while (true)
        {
            // Chiede alla View di mostrare il menu e restituisce la scelta dell'utente
            var input = _view.MostraMenuPrincipale();

            // Esegui le operazioni in base alla scelta dell'utente
            if (input == "Aggiungi Dipendente")
            {
                AggiungiDipendente(); // Aggiunta di un utente
            }
            else if (input == "Mostra Dipendenti")
            {
                MostraDipendenti(); // Visualizzazione degli utenti
            }
            else if (input == "Rimuovi Dipendente")
            {
                RimuoviDipendente();    // Rimuove un dipendente
            }
            else if (input == "Cerca Dipendente")
            {
                CercaDipendente(); // Cerca un dipendente tramite email
            }
            else if (input == "Modifica Dipendente")
            {
                ModificaDipendente(); // Modifica un dipendente
            }
            else if (input == "Ordina stipendi")
            {
                OrdinaStipendi(); // Ordina i dipendenti per stipendio
            }
            else if (input == "Aggiungi indicatori")
            {
                AggiungiIndicatoriDipendente(); // Aggiungi indicatori fatturato e presenze a un dipendente
            }
            else if (input == "Tasso di presenza")
            {
                TassoDiPresenza(); // Calcola e visualizza il tasso di presenza in percentuale
            }
            else if (input == "Valutazione per fatturato")
            {
                ValutazioneFatturatoProdotto(); // Valutazione dei dipendenti per fatturato
            }
            else if (input == "Incidenza percentuale")
            {
                IncidenzaPercentuale(); // Da completare
            }
            else if (input == "Esci")
            {
                break; // Uscita dal programma
            }
        }
    }
 
// il metodo AggiungiDipendente gestisce l'intero processo di raccolta dei dati per un nuovo dipendente
// verifica la validità delle informazioni e inserisce il nuovo dipendente nel database del sistema.
    private void AggiungiDipendente()
    {
        Console.WriteLine("Inserisci il nome:"); // Richiesta del nome dell'utente
        var nome = _view.GetInput(); // Lettura del nome dell'utente
        Console.WriteLine("Inserisci il cognome:");
        var cognome = _view.GetInput();
        Console.WriteLine("Inserisci la data di nascita DD/MM/YYYY:");
        var dataDiNascitaString = _view.GetInput();
        Console.WriteLine("Inserisci la mail aziendale:");
        var mail = _view.GetInput();
         // Mostra le mansioni disponibili recuperate dal database per consentire all'utente di scegliere
        var mansioni = _db.MostraMansioni();
         // Ottiene la lista delle mansioni dal database
        foreach (var mansione in mansioni)
        {
            // Stampa ogni mansione con il suo ID e altri dettagli per visionare l'id di quale mansione  aggiungere
            Console.WriteLine($"ID: {mansione.Id}, Titolo: {mansione.Titolo}, Stipendio: {mansione.Stipendio}");
        }

        Console.WriteLine("Scegli tra le mansioni disponibili per id:");
        var mansioneInput = _view.GetInput();
        // Verifica se l'input della mansione è un intero valido
        if (int.TryParse(mansioneInput, out int mansioneId))
        {
            var mansioneid = Console.ReadLine();
            // Verifica se la data di nascita inserita è valida nel formato "dd/MM/yyyy"
            if (DateTime.TryParseExact(dataDiNascitaString, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataDiNascita))
            {
                // Aggiunta del dipendente nel database con nome, cognome, data di nascita, email e ID della mansione
                _db.AggiungiDipendente(nome, cognome, dataDiNascita, mail, mansioneId);
                Console.WriteLine("Dipendente aggiunto con successo."); // Aggiunta del dipendente al database
            }
            else
            {
                Console.WriteLine("Formato data di nascita non valido. Riprova.");
            }

        }
        else
        {
            Console.WriteLine("ID mansione non valido. Riprova.");
        }
    }

// il metodo RimuoviDipendente gestisce il processo di eliminazione di un dipendente dal database
//fornendo all'utente un elenco dei dipendenti verificando l'input e rimuovendo il dipendente selezionato in modo sicuro
    private void RimuoviDipendente()
    {
        // Mostra l'elenco dei dipendenti con il loro ID, nome e cognome
        Console.WriteLine("Elenco dei dipendenti:");
        var dipendentiConId = _db.GetDipendentiConId();
        // Itera attraverso i dipendenti e visualizza le loro informazioni
        foreach (var dipendente in dipendentiConId)
        {
            Console.WriteLine(dipendente);
        }
        Console.WriteLine("Inserisci l'ID del dipendente da rimuovere:");
        try
        {
            // Usa Convert.ToInt32 per convertire l'input dell'utnete (ID) in un intero
            var dipendenteId = Convert.ToInt32(Console.ReadLine());

            // Prova a rimuovere il dipendente con l'ID specificato
            bool successo = _db.RimuoviDipendente(dipendenteId);
            if (successo)
            {
                Console.WriteLine("Dipendente rimosso con successo.");
            }
            else
            {
                Console.WriteLine("Dipendente non trovato o ID non valido.");
            }
        }
        catch (FormatException)
        {
            // Gestisce l'errore se l'input non è un numero valido
            Console.WriteLine("ID non valido. Inserisci un numero.");
        }

    }

    //Il metodo CercaDipendente consente di cercare un dipendente nel database
    // utilizzando la sua email aziendale come chiave di ricerca.ad esempio nome.cognome@gmail.com
    private void CercaDipendente()
    {
        Console.WriteLine("Cerca il dipendente usando la sua mail aziendale:");
        // Lettura dell'input dell'utente
        var cercaMail = _view.GetInput();
        // Cerca il dipendente nel database usando la mail fornita
        var dipendente = _db.CercaDipendentePerMail(cercaMail);
        // Se il dipendente viene trovato
        if (dipendente != null)
        {
            // Crea una nuova tabella per visualizzare i dati del dipendente
            var table = new Table();
            table.AddColumn("Nome");
            table.AddColumn("Cognome");
            table.AddColumn("Data di Nascita");
            table.AddColumn("Mansione");
            table.AddColumn("Stipendio");
            table.AddColumn("Fatturato");
            table.AddColumn("Presenze");
            table.AddColumn("Email");

            // Aggiungi i dati del dipendente alla tabella come una nuova riga
            table.AddRow(
                dipendente.Nome,
                dipendente.Cognome,
                dipendente.DataDiNascita,
                dipendente.Mansione.Titolo,
                dipendente.Stipendio.ToString(),
                dipendente.Statistiche.Fatturato.ToString(),
                dipendente.Statistiche.Presenze.ToString(),
                dipendente.Mail
            );

            // Mostra la tabella con i dettagli del dipendente 
            AnsiConsole.Write(table);

        }
        else
        {
            // Messaggio se il dipendente non viene trovato
            Console.WriteLine("Dipendente non trovato con questa email.");
        }

    }

    // Il metodo MostraDipendenti visualizza a schermo l'elenco completo dei dipendenti con tutti i dati correlati
    private void MostraDipendenti()
    {
        var dipendenti = _db.GetUsers(); // Lettura di tutti i dipendenti dal database
        var table = new Table();
        // Aggiungere le colonne alla tabella
        table.AddColumn("Nome");
        table.AddColumn("Cognome");
        table.AddColumn("Data di Nascita");
        table.AddColumn("Mansione");
        table.AddColumn("Stipendio annuale");
        table.AddColumn("Fatturato");
        table.AddColumn("Presenze");
        table.AddColumn("Email aziendale");
        //  _view.MostraDipendenti(dipendenti); // Visualizzazione degli utenti

        // Itera attraverso ogni dipendente recuperato dal database
        foreach (var dipendente in dipendenti)
        {
            table.AddRow(
                dipendente.Nome,                            // Nome del dipendente
                dipendente.Cognome,                         // Cognome del dipendente
                dipendente.DataDiNascita,                   // Data di nascita
                dipendente.Mansione.Titolo,                 // Nome della mansione
                $"{dipendente.Stipendio}",                  // stipendio
                $"{dipendente.Statistiche.Fatturato}",      // Fatturato generato dal dipendente
                $"{dipendente.Statistiche.Presenze}",        // Giorni di presenza del dipendente
                dipendente.Mail                             // mail 
            );
        }


        AnsiConsole.Write(table);
    }

    // Il metodo TassoDiPresenza è utilizzato per calcolare e visualizzare il tasso di presenza dei dipendenti in percentuale 
    //rispetto al numero totale di giorni lavorativi in un anno (250 giorni).
    private void TassoDiPresenza()
    {
        Console.WriteLine("\nDi seguito l'elenco con il tasso di presenza per ogni dipendente su 250 giorni lavorativi equivalente ad 1 anno\n");
        int giorniLavorativiTotali = 250; // Numero di giorni lavorativi in un anno

        try
        {
            var dipendenti = _db.GetUsers(); // Ottieni la lista dei dipendenti dal database

            // Crea una tabella per visualizzare i risultati
            var table = new Table();
            table.AddColumn("Dipendente");
            table.AddColumn("Tasso di Presenza (%)");

            // Ordina i dipendenti dal tasso di presenza più alto al più basso
            dipendenti.Sort((y, x) => x.Statistiche.Presenze.CompareTo(y.Statistiche.Presenze));

            foreach (var dipendente in dipendenti)
            {
                int presenze = dipendente.Statistiche.Presenze;

                // Calcolo del tasso di presenza
                double tassoPresenza = ((double)presenze / giorniLavorativiTotali) * 100;
                tassoPresenza = Math.Round(tassoPresenza, 2); // Arrotonda il risultato a due cifre decimali

                // Aggiungi una riga alla tabella con il nome del dipendente e il tasso di presenza
                table.AddRow($"{dipendente.Nome} {dipendente.Cognome}", $"{tassoPresenza}%");
            }

            // Visualizza la tabella nella console
            AnsiConsole.Write(table);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Errore generale: {e.Message}");
        }
    }

    // metodo che divide in 2 gruppi i dipendenti in base al fatturato 
    //il primo gruppo sono i più performanti e l'ultimo gruppo i meno performanti
    // mostra anche il 15% dei dipendenti meno performanti
    private void ValutazioneFatturatoProdotto()
    {
        Console.WriteLine("\nDivide i dipendenti in 2 gruppi in base al fatturato prodotto");

        var dipendenti = _db.GetUsers(); // Ottiene la lista dei dipendenti dal database

        // Ordina i dipendenti per fatturato in ordine decrescente (dal più alto al più basso)
        dipendenti.Sort((x, y) => y.Statistiche.Fatturato.CompareTo(x.Statistiche.Fatturato));

        // Divide i dipendenti in due gruppi: i migliori e i peggiori
        int split = dipendenti.Count / 2;
        List<Dipendente> gruppoMigliori = dipendenti.GetRange(0, split);
        List<Dipendente> gruppoPeggiori = dipendenti.GetRange(split, dipendenti.Count - split);

        // Crea le tabelle per visualizzare i risultati
        var tableMigliori = new Table();
        tableMigliori.AddColumn("Dipendente");
        tableMigliori.AddColumn("Fatturato");

        var tablePeggiori = new Table();
        tablePeggiori.AddColumn("Dipendente");
        tablePeggiori.AddColumn("Fatturato");

        var tablePeggiori15 = new Table();
        tablePeggiori15.AddColumn("Dipendente");
        tablePeggiori15.AddColumn("Fatturato");

        // Aggiungi i dipendenti con fatturato più alto nella prima tabella
        foreach (var dipendente in gruppoMigliori)
        {
            tableMigliori.AddRow($"{dipendente.Nome} {dipendente.Cognome}", $"{dipendente.Statistiche.Fatturato}");
        }

        // Aggiungi i dipendenti con fatturato più basso nella seconda tabella
        foreach (var dipendente in gruppoPeggiori)
        {
            tablePeggiori.AddRow($"{dipendente.Nome} {dipendente.Cognome}", $"{dipendente.Statistiche.Fatturato}");
        }

        // Mostra la tabella dei dipendenti con fatturato più alto
        Console.WriteLine("\nGruppo con il fatturato più alto:\n");
        AnsiConsole.Write(tableMigliori);

        // Mostra la tabella dei dipendenti con fatturato più basso
        Console.WriteLine("\nGruppo con il fatturato più basso:\n");
        AnsiConsole.Write(tablePeggiori);

        // Ordina il gruppo con fatturato più basso per trovare il 15% delle performance peggiori
        gruppoPeggiori.Sort((x, y) => x.Statistiche.Fatturato.CompareTo(y.Statistiche.Fatturato));

        // Calcola il 15% dei dipendenti con fatturato più basso
        int index = (15 * gruppoPeggiori.Count) / 100;

        // Se il 15% è 0 ma ci sono dipendenti, mostra almeno un dipendente
        if (index == 0 && gruppoPeggiori.Count > 0)
        {
            index = 1;
        }

        // Aggiungi i dipendenti con il peggior 15% di fatturato alla terza tabella
        for (int i = 0; i < index; i++)
        {
            var dipendente = gruppoPeggiori[i];
            tablePeggiori15.AddRow($"{dipendente.Nome} {dipendente.Cognome}", $"{dipendente.Statistiche.Fatturato}");
        }

        // Mostra il 15% dei dipendenti con fatturato più basso
        Console.WriteLine("\nDi seguito il 15% delle performance peggiori per fatturato\n");
        AnsiConsole.Write(tablePeggiori15);
    }

    // metodo che mostra l'incidenza percentuale dello stipendi osul fatturato
    // il metodo è da costruire
    public void IncidenzaPercentuale()
    {
        /* Console.WriteLine("Calcolo dell'incidenza percentuale dello stipendio in rapporto al fatturato.");

         // Recupera i dipendenti dal database
         var dipendenti = _db.GetUsers();

         // Verifica se ci sono dipendenti nel database
         if (dipendenti.Count == 0)
         {
             Console.WriteLine("Non ci sono dipendenti nel sistema.");
             return;
         }

         // Chiedi l'inserimento del fatturato totale, potrebbe essere ottenuto anche da un altro metodo
         Console.WriteLine("Inserisci il fatturato totale dell'azienda:");
         double fatturatoTotale = Convert.ToDouble(Console.ReadLine());

         // Crea una tabella per visualizzare i dati
         var table = new Table();
         table.AddColumn("Nome");
         table.AddColumn("Cognome");
         table.AddColumn("Data di Nascita");
         table.AddColumn("Mansione");
         table.AddColumn("Stipendio");
         table.AddColumn("Incidenza sul fatturato (%)");
         table.AddColumn("Fatturato prodotto");
         table.AddColumn("Presenze");

         // Ordina i dipendenti per stipendio in ordine discendente
         dipendenti.Sort((x, y) => y.Stipendio.CompareTo(x.Stipendio));

         // Itera sui dipendenti e calcola l'incidenza percentuale
         foreach (var dipendente in dipendenti)
         {
             double stipendio = dipendente.Stipendio;

             // Calcola l'incidenza percentuale dello stipendio rispetto al fatturato totale
             double incidenza = (stipendio / fatturatoTotale) * 100;
             double incidenzaPercentuale = Math.Round(incidenza, 2); // Arrotonda a 2 cifre decimali

             // Aggiungi i dati del dipendente alla tabella
             table.AddRow(
                 dipendente.Nome,
                 dipendente.Cognome,
                 dipendente.DataDiNascita,
                 dipendente.Mansione.Titolo,
                 stipendio.ToString(),
                 $"{incidenzaPercentuale}%",
                 dipendente.Statistiche.Fatturato.ToString(),
                 dipendente.Statistiche.Presenze.ToString()
             );
         }

         // Mostra la tabella nella console
         AnsiConsole.Write(table); */
    }


    // metodo per aggiungere fatturato e presenze da assegnare al dipendente
    private void AggiungiIndicatoriDipendente()
    {
        Console.WriteLine("Elenco dei dipendenti:");

        // Recupera i dipendenti con ID
        var dipendentiConId = _db.GetUsers(); // Ottieni la lista dei dipendenti

        // Crea una tabella per visualizzare i dipendenti
        var table = new Table();
        table.AddColumn("ID");
        table.AddColumn("Nome");
        table.AddColumn("Cognome");
        table.AddColumn("Mansione");
        table.AddColumn("Stipendio");
        table.AddColumn("Fatturato");
        table.AddColumn("Presenze");

        // Aggiungi le righe con i dati dei dipendenti
        foreach (var dipendente in dipendentiConId)
        {
            table.AddRow(
                dipendente.Id.ToString(), // Visualizza l'ID del dipendente
                dipendente.Nome,
                dipendente.Cognome,
                dipendente.Mansione.Titolo,
                dipendente.Stipendio.ToString(),
                dipendente.Statistiche.Fatturato.ToString(),
                dipendente.Statistiche.Presenze.ToString()
            );
        }

        // Mostra la tabella
        AnsiConsole.Write(table);

        // Chiedi all'utente l'ID del dipendente
        Console.WriteLine("Inserisci l'ID del dipendente per aggiungere indicatori:");
        int dipendenteId = Convert.ToInt32(Console.ReadLine());

        // Chiedi i nuovi valori per fatturato e presenze
        Console.WriteLine("Inserisci il fatturato del dipendente:");
        double fatturato = Convert.ToDouble(Console.ReadLine());

        Console.WriteLine("Inserisci il numero di presenze del dipendente:");
        int presenze = Convert.ToInt32(Console.ReadLine());

        // Chiamata al metodo AggiungiIndicatori del Database
        _db.AggiungiIndicatori(dipendenteId, fatturato, presenze);

        Console.WriteLine("Indicatori aggiunti con successo.");
    }



    // da rivedere
    private void AggiornaIndicatoriDipendente()
    {
        // Mostra elenco dipendenti con ID
        Console.WriteLine("Elenco dei dipendenti:");
        var dipendentiConId = _db.GetDipendentiConId(); // Ottieni la lista dei dipendenti con ID
        foreach (var dipendente in dipendentiConId)
        {
            Console.WriteLine(dipendente); // Mostra ID e nome di ogni dipendente
        }

        // Richiedi l'ID del dipendente per il quale aggiornare gli indicatori
        Console.WriteLine("Inserisci l'ID del dipendente per aggiornare gli indicatori:");
        int dipendenteId = Convert.ToInt32(Console.ReadLine()); // Legge l'ID del dipendente

        // Chiedi i nuovi valori di fatturato e presenze
        Console.WriteLine("Inserisci il nuovo fatturato del dipendente:");
        double nuovoFatturato = Convert.ToDouble(Console.ReadLine()); // Legge il nuovo fatturato

        Console.WriteLine("Inserisci il nuovo numero di presenze del dipendente:");
        int nuovePresenze = Convert.ToInt32(Console.ReadLine()); // Legge il nuovo numero di presenze

        // Chiamata al metodo AggiornaIndicatori del Database
        _db.AggiornaIndicatori(dipendenteId, nuovoFatturato, nuovePresenze);

        // Conferma che gli indicatori sono stati aggiornati
        Console.WriteLine("Indicatori aggiornati con successo.");
    }

    // metodo che ordina nel terminale stipendi dal più alto al più basso
    private void OrdinaStipendi()
    {
        // Recupera i dipendenti dal database
        var dipendenti = _db.GetUsers();

        var table = new Table();

        table.AddColumn("Nome");
        table.AddColumn("Cognome");
        table.AddColumn("Stipendio");
        table.AddColumn("Mansione");
        table.AddColumn("Fatturato");
        table.AddColumn("Presenze");

        // Algoritmo bubble sort per ordinare i dipendenti in base allo stipendio in ordine discendente
        for (int i = 0; i < dipendenti.Count - 1; i++)
        {
            for (int j = 0; j < dipendenti.Count - i - 1; j++)
            {
                if (dipendenti[j].Stipendio < dipendenti[j + 1].Stipendio)
                {
                    // Scambia i dipendenti se lo stipendio del primo è inferiore a quello del successivo
                    var temp = dipendenti[j];
                    dipendenti[j] = dipendenti[j + 1];
                    dipendenti[j + 1] = temp;
                }
            }
        }

        foreach (var dipendente in dipendenti)
        {
            table.AddRow(
                dipendente.Nome,
                dipendente.Cognome,
                dipendente.Stipendio.ToString(),
                dipendente.Mansione.Titolo.ToString(),
                dipendente.Statistiche.Fatturato.ToString(),
                dipendente.Statistiche.Presenze.ToString()
            );
        }

        // Mostra la tabella ordinata
        Console.WriteLine("\nDipendenti ordinati per stipendio (dal più alto al più basso):\n");
        AnsiConsole.Write(table);
    }



    private void ModificaDipendente()
    {
        try
        {
            Console.WriteLine("Elenco dei dipendenti:");
            var dipendentiConId = _db.GetDipendentiConId();

            // Mostra l'elenco dei dipendenti con ID
            foreach (var dipendente in dipendentiConId)
            {
                Console.WriteLine(dipendente);
            }

            // Richiedi l'ID del dipendente
            Console.WriteLine("Inserisci l'ID del dipendente da modificare:");
            int dipendenteId = Convert.ToInt32(_view.GetInput());

            // Menu di selezione per i campi da modificare
            var inserimento = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                .Title("MODIFICA DIPENDENTE")
                .PageSize(10)
                .MoreChoicesText("[grey](Move up and down to reveal more)[/]")
                .AddChoices(new[] {
                "Cambia nome", "Cambia cognome", "Cambia data di nascita formato DD/MM/YYYY",
                "Cambia mansione", "Cambia stipendio", "Cambia fatturato", "Cambia giorni di presenza", "Cambia mail","Aggiorna indicatori", "Esci",
                }));

            string campoDaModificare = "";
            string nuovoValore = "";

            // Switch case per la modifica del campo selezionato
            switch (inserimento)
            {
                case "Cambia nome":
                    Console.WriteLine("Inserisci il nuovo nome");
                    campoDaModificare = "nome";
                    nuovoValore = Console.ReadLine().Trim();
                    break;

                case "Cambia cognome":
                    Console.WriteLine("Inserisci il nuovo cognome");
                    campoDaModificare = "cognome";
                    nuovoValore = Console.ReadLine().Trim();
                    break;

                case "Cambia data di nascita formato DD/MM/YYYY":
                    Console.WriteLine("Inserisci nuova data di nascita");
                    // Legge l'input dell'utente rimuove eventuali spazi vuoti e lo converte in un oggetto DateTime
                    // Utilizza il formato "dd/MM/yyyy" per garantire che la data sia inserita correttamente (giorno/mese/anno)
                    DateTime dataDiNascita = DateTime.ParseExact(Console.ReadLine().Trim(), "dd/MM/yyyy", null);
                    campoDaModificare = "dataDiNascita";
                    // Converte l'oggetto DateTime in una stringa formattata per essere compatibile con SQLite
                    nuovoValore = dataDiNascita.ToString("yyyy-MM-dd");  // Formattazione per SQLite
                    break;

                case "Cambia mansione":
                    Console.WriteLine("Inserisci la nuova mansione (ID):");
                    int mansioneId = Convert.ToInt32(Console.ReadLine().Trim());
                    campoDaModificare = "mansioneId";
                    nuovoValore = mansioneId.ToString();
                    break;

                case "Cambia stipendio":
                    Console.WriteLine("Inserisci il nuovo stipendio:");
                    campoDaModificare = "stipendio";
                    nuovoValore = Console.ReadLine().Trim();
                    break;

                case "Cambia fatturato":
                    Console.WriteLine("Inserisci il nuovo fatturato:");
                    campoDaModificare = "fatturato";
                    nuovoValore = Console.ReadLine().Trim();
                    break;

                case "Cambia giorni di presenza":
                    Console.WriteLine("Inserisci il numero di giorni di presenze:");
                    campoDaModificare = "presenze";
                    nuovoValore = Console.ReadLine().Trim();
                    break;

                case "Cambia mail":
                    Console.WriteLine("Inserisci il nuovo indirizzo email aziendale:");
                    campoDaModificare = "mail";
                    nuovoValore = Console.ReadLine().Trim();
                    break;



                case "Esci":
                    Console.WriteLine("\nL'applicazione si sta per chiudere\n");
                    return;

                default:
                    Console.WriteLine("\nScelta errata. Prego scegliere tra le opzioni disponibili\n");
                    return;
            }

            // Modifica il campo nel database
            bool successo = _db.ModificaDipendente(dipendenteId, campoDaModificare, nuovoValore);

            if (successo)
            {
                Console.WriteLine($"{campoDaModificare} aggiornato con successo per il dipendente con ID {dipendenteId}.");
            }
            else
            {
                Console.WriteLine("Errore durante la modifica del dipendente. Verifica l'ID o il campo inserito.");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Errore non trattato: {e.Message}");
            Console.WriteLine($"CODICE ERRORE: {e.HResult}");
        }
    }




}