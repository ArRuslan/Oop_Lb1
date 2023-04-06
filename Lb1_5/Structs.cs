namespace Lb1_5;

struct WagonType {
    public string Type;
    public int NumberOfSeats;
    public int Price;
        
    public WagonType(string type, int numberOfSeats, int price) {
        Type = type;
        NumberOfSeats = numberOfSeats;
        Price = price;
    }
        
    public override string ToString() {
        return $"{Type}: {NumberOfSeats}м., {Price}грн";
    }
}

struct TrainWagon {
    public int Number;
    public string Type;
    public int SoldTickets;
        
    public TrainWagon(int number, string type, int soldTickets) {
        Number = number;
        Type = type;
        SoldTickets = soldTickets;
    }
        
    public override string ToString() {
        return $"Вагон №{Number}, {Type}, {SoldTickets} продано";
    }
}