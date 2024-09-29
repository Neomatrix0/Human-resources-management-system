// classe che rappresenta la mansione di un dipendente con proprietà Id, Titolo e Stipendio
public class Mansione
{
    public int Id { get; set; }  // Proprietà 'Id' che rappresenta l'identificatore univoco della mansione
    public string Titolo { get; set; } // proprietà relativa al nome della mansione
    public double Stipendio { get; set; }    // proprietà relativa allo stipendio

    // Costruttore che accetta id titolo e stipendio
    // Questo costruttore viene usato quando abbiamo già un 'Id', probabilmente assegnato dal database
    public Mansione(int id, string titolo, double stipendio)
    {
        Id = id;
        Titolo = titolo;        // Assegna il titolo della mansione
        Stipendio = stipendio;  // Assegna lo stipendio della mansione
    }
    // Nuovo costruttore che accetta solo titolo e stipendio
    // Questo costruttore può essere usato quando l'Id non è necessario al momento della creazione
    public Mansione(string titolo, double stipendio)
    {
        Titolo = titolo;
        Stipendio = stipendio;
    }
}