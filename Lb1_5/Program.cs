using System.Text;
using Lb1_5;

class WagonController {
    private WagonType[] wagonTypes = { // Типи вагонів
        new WagonType("Купе", 36, 1000),
        new WagonType("Плацкарт", 54, 500),
    };
    private TrainWagon[] Wagons; // Масив вагонів
    
    public WagonController(int wNum) {
        Wagons = new TrainWagon[wNum];
    }
    
    public WagonType[] WagonTypes {
        get => wagonTypes;
    }
    
    public TrainWagon this[int i] {
        // Індексатор вагонів
        get => Wagons[i];
        set {
            bool wrongType = true;
            foreach (WagonType wagonType in wagonTypes) {
                if(wagonType.Type == value.Type) {
                    wrongType = false;
                    break;
                }
            }
            if(wrongType)
                throw new InvalidTypeException($"Неправильний тип вагону: {value.ToString()}");
            foreach (TrainWagon wagon in Wagons) {
                if(wagon.Number == value.Number) {
                    throw new WagonNumberExistsException($"Вагон з таким номером вже існує: {value.Number}");
                }
            }
            if(value.SoldTickets > GetType(value.Type).NumberOfSeats)
                throw new InvalidSoldTicketsCount("Не може бути продано квитків більше, ніж є місць усього.");
            Wagons[i] = value;
        }
    }

    private WagonType GetType(string type) { // Отримати тип вагону з строки
        foreach (WagonType wagonType in wagonTypes) {
            if(wagonType.Type == type)
                return wagonType;
        }
        return wagonTypes[0];
    }

    public void SortByFreeSeats() {
        Wagons = Wagons.OrderByDescending(s => GetType(s.Type).NumberOfSeats-s.SoldTickets).ToArray();
    }
    
    public void printAll(bool printFree=false, bool printSeatsTotal=false, bool printMoney=false) {
        foreach (TrainWagon wagon in Wagons) {
            Console.Write(wagon.ToString());
            if(printSeatsTotal) {
                Console.Write($", {GetType(wagon.Type).NumberOfSeats} місць");
            }
            if(printFree) {
                int free = GetType(wagon.Type).NumberOfSeats - wagon.SoldTickets;
                Console.Write($", {free} вільних");
            }
            if(printMoney) {
                int money = GetType(wagon.Type).Price * wagon.SoldTickets;
                Console.Write($", {money}грн");
            }
            Console.WriteLine();
        }
    }
    
    public int mostFreeWagonIndex() { // Індекс найвільнішого вагону
        TrainWagon min = Wagons[0];
        int minIndex = 0;
        for(int i = 0; i < Wagons.Length; i++) {
            TrainWagon wagon = Wagons[i];
            if(wagon.SoldTickets < min.SoldTickets) {
                min = wagon;
                minIndex = i;
            }
        }
        return minIndex;
    }

    public bool canBeFreed() {
        TrainWagon min = Wagons[mostFreeWagonIndex()];
        
        Console.WriteLine($"Найвільніший вагон - {min.ToString()}");
        
        int soldTickets = min.SoldTickets;
        foreach (TrainWagon wagon in Wagons) {
            if(wagon.Type != min.Type)
                continue;
            if(wagon.Number == min.Number)
                continue;
            int free = GetType(wagon.Type).NumberOfSeats - wagon.SoldTickets;
            soldTickets -= free;
        }
        
        return soldTickets <= 0;
    }
    
    public void freeWagon() {
        int minIndex = mostFreeWagonIndex();
        WagonType type = GetType(Wagons[minIndex].Type);
        for(int i = 0; i < Wagons.Length; i++) {
            TrainWagon min = Wagons[minIndex];
            TrainWagon wagon = Wagons[i];
            if(wagon.Type != min.Type)
                continue;
            if(wagon.Number == min.Number)
                continue;
            int sold = wagon.SoldTickets;
            if(sold > min.SoldTickets) {
                sold = min.SoldTickets;
            }
            if(sold > type.NumberOfSeats-wagon.SoldTickets) {
                sold = type.NumberOfSeats-wagon.SoldTickets;
            }
            Wagons[minIndex].SoldTickets -= sold;
            Wagons[i].SoldTickets += sold;
            Console.WriteLine($"  Переміщено {sold} з {min.Number} у {wagon.Number}");
            if(Wagons[minIndex].SoldTickets <= 0)
                return;
        }
    }
}

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

class Program {
    private static int InputInt(string promptStr, string errorMessage) {
        Console.Write(promptStr);
        string valueStr = Console.ReadLine();
        int value;
        try {
            if(valueStr == null)
                throw new FormatException();
            value = int.Parse(valueStr);
        } catch (FormatException) {
            Console.WriteLine($"{errorMessage} (не число)!");
            return InputInt(promptStr, errorMessage);
        } catch (OverflowException) {
            Console.WriteLine($"{errorMessage} (число завелике)!");
            return InputInt(promptStr, errorMessage);
        }
        if(value < 0) {
            Console.WriteLine($"{errorMessage} (число менше 0)!");
            return InputInt(promptStr, errorMessage);
        }
        return value;
    }

    private static WagonController Input() {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        Console.InputEncoding = Encoding.GetEncoding("windows-1251");
        
        int wagonsCount = InputInt("Кількість вагонів: ", "Неправильна кількість вагонів");
        WagonController wagonController = new WagonController(wagonsCount);
        Console.WriteLine("Типи вагонів:");
        foreach (WagonType wagonType in wagonController.WagonTypes) {
            Console.WriteLine($"  {wagonType.ToString()}");
        }
        int i = 0;
        while (i < wagonsCount) {
            Console.WriteLine($"Введіть дані для вагону №{i+1}");
            Console.Write("  Тип вагону: ");
            string type = Console.ReadLine();
            int sold = InputInt("  Кількість проданих квитків: ", "Неправильна кількість проданих квитків");
            try {
                wagonController[i] = new TrainWagon(i+1, type, sold);
            } catch (Exception e) when (e is WagonNumberExistsException || e is InvalidTypeException || e is InvalidSoldTicketsCount) {
                Console.WriteLine(e.Message);
                continue;
            }
            i++;
        }
        return wagonController;
    }

    public static void Main(string[] args) {
        WagonController wagonController;
        if(args.Contains("--input")) {
            wagonController = Input();
        } else {
            wagonController = new WagonController(5);
            wagonController[0] = new TrainWagon(1, "Купе", 10);
            wagonController[1] = new TrainWagon(2, "Купе", 36);
            wagonController[2] = new TrainWagon(3, "Плацкарт", 20);
            wagonController[3] = new TrainWagon(4, "Купе", 20);
            wagonController[4] = new TrainWagon(5, "Плацкарт", 40);
        }
        
        
        Console.WriteLine("(а) Список вагонів, відсортованих за зменшенням кількості вільних місць:");
        wagonController.SortByFreeSeats();
        wagonController.printAll(printFree: true);
        Console.WriteLine();
        
        Console.WriteLine("(б) Повна інформація:");
        wagonController.printAll(printFree: true, printSeatsTotal: true, printMoney: true);
        Console.WriteLine();
        
        Console.Write("(в) ");
        if(wagonController.canBeFreed()) {
            Console.WriteLine("Можна звільнити...");
            wagonController.freeWagon();
            wagonController.printAll(printFree: true, printSeatsTotal: true, printMoney: true);
        } else {
            Console.WriteLine("Неможливо звільнити...");
        }
    }
}