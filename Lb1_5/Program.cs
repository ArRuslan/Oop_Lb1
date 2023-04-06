/*
Створити масив структур з інформацією про вагони поїзда: номер вагона, тип вагона, кількість проданих квитків.
І масив структур інформацією про різні типи вагонів: тип вагона, кількість місць, вартість квитка.
а) Вивести список вагонів, відсортувавши за зменшенням кількості вільних місць.
б) Для кожного вагона вказати кількість місць всього, кількість проданих квитків, кількість вільних місць, загальну суму виручених грошей.
в) Визначити, чи можна звільнити найвільніший вагон, розмістивши його пасажирів за іншими вагонами цього ж типу, і зробити це у разі підтвердження.
*/

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
    
    private int freeSeats(TrainWagon wagon) {
        return GetType(wagon.Type).NumberOfSeats-wagon.SoldTickets;
    }
    
    private TrainWagon[] SortedByFreeSeatsS() {
        TrainWagon[] wagons = (TrainWagon[])Wagons.Clone();
        for(int i = 0; i < wagons.Length-1; i++) {
            for(int j = 0; j < wagons.Length-i-1; j++) {
                if(freeSeats(wagons[j]) <= freeSeats(wagons[j+1])) {
                    (wagons[j], wagons[j + 1]) = (wagons[j+1], wagons[j]);
                }
            }
        }
        return wagons;
    }

    public TrainWagon[] SortedByFreeSeats() {
        //return SortedByFreeSeatsS();
        return Wagons.OrderByDescending(s => freeSeats(s)).ToArray();
    }
    
    private long TotalMoney() {
        long total = 0; 
        foreach (TrainWagon wagon in Wagons) {
            total += GetType(wagon.Type).Price * wagon.SoldTickets;
        }
        return total;
    }
    
    public void printAll(bool printFree=false, bool printSeatsTotal=false, bool printMoney=false, TrainWagon[] wagons=null) {
        if(wagons == null) wagons = Wagons;
        foreach (TrainWagon wagon in wagons) {
            Console.Write(wagon.ToString());
            if(printSeatsTotal) {
                Console.Write($", {GetType(wagon.Type).NumberOfSeats} місць");
            }
            if(printFree) {
                Console.Write($", {freeSeats(wagon)} вільних");
            }
            if(printMoney) {
                int money = GetType(wagon.Type).Price * wagon.SoldTickets;
                Console.Write($", {money}грн");
            }
            Console.WriteLine();
        }
        if(printMoney)
            Console.WriteLine($"Усього {TotalMoney()}грн");
    }
    
    public int mostFreeWagonIndex() { // Індекс найвільнішого вагону
        TrainWagon min = Wagons[0];
        int minIndex = 0;
        for(int i = 0; i < Wagons.Length; i++) {
            TrainWagon wagon = Wagons[i];
            //if(wagon.SoldTickets < 1) continue; // Вважати пусті вагони найвільнішими?
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
            wagonController = new WagonController(15);
            wagonController[0] = new TrainWagon(1, "Купе", 10);
            wagonController[1] = new TrainWagon(2, "Купе", 36);
            wagonController[2] = new TrainWagon(3, "Плацкарт", 20);
            wagonController[3] = new TrainWagon(4, "Купе", 20);
            wagonController[4] = new TrainWagon(5, "Плацкарт", 40);
            wagonController[5] = new TrainWagon(6, "Купе", 36);
            wagonController[6] = new TrainWagon(7, "Купе", 36);
            wagonController[7] = new TrainWagon(8, "Купе", 0);
            wagonController[8] = new TrainWagon(9, "Плацкарт", 1);
            wagonController[9] = new TrainWagon(10, "Плацкарт", 7);
            wagonController[10] = new TrainWagon(11, "Плацкарт", 54);
            wagonController[11] = new TrainWagon(12, "Плацкарт", 54);
            wagonController[12] = new TrainWagon(13, "Плацкарт", 5);
            wagonController[13] = new TrainWagon(14, "Плацкарт", 8);
            wagonController[14] = new TrainWagon(15, "Плацкарт", 25);
        }
        
        
        Console.WriteLine("(а) Список вагонів, відсортованих за зменшенням кількості вільних місць:");
        wagonController.printAll(printFree: true, wagons: wagonController.SortedByFreeSeats());
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