// classe Statistiche con proprietà relative al fatturato e alle presenze del dipendente
public class Statistiche{
// Proprietà 'Id' per identificare in modo univoco ogni record di statistiche
    public int Id{ get; set; }

    // Proprietà per memorizzare il fatturato generato da un dipendente
    public double Fatturato{ get; set; }

 // Proprietà per memorizzare il numero di presenze di un dipendente
    public int Presenze{ get; set; }

// costruttore classe Statistiche
// Riceve i valori iniziali per 'Fatturato' e 'Presenze' e li assegna alle rispettive proprietà
    public Statistiche(double fatturato,int presenze){
        
        Fatturato = fatturato; // Assegna il valore di fatturato alla proprietà Fatturato
        Presenze = presenze;
    }
}