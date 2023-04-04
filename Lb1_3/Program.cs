struct Address {
    public string City;
    public string Street;
    public string House;
    
    public Address(string city, string street, string house) {
        City = city;
        Street = street;
        House = house;
    }
    
    public string ToString() {
        return $"м.{City}, вул.{Street}, буд.{House}";
    }
}

struct Cat {
    public int Number;
    public string Name;
    public string Breed;
    public string OwnerFullName;
    public Address OwnerAddress;
    
    public Cat(int number, string name, string breed, string owner, Address address) {
        Number = number;
        Name = name;
        Breed = breed;
        OwnerFullName = owner;
        OwnerAddress = address;
    }
    
    public string ToString() {
        return $"Кіт #{Number} {Name} ({Breed}), Власник {OwnerFullName} ({OwnerAddress.ToString()})";
    }
}

class Program {
    public static void Main(string[] args) {
        Cat[] cats = new Cat[10];
        cats[0] = new Cat(1, "Test1", "Idk1", "Asd1 Qwe2 Zxc3", new Address("Харків", "BC", "123"));
        cats[1] = new Cat(2, "Test2", "Idk2", "Asd4 Qwe1 Zxc3", new Address("Харків", "BC", "123"));
        cats[2] = new Cat(3, "Test3", "Idk3", "Asd2 Qwe3 Zxc1", new Address("AB", "BC", "123"));
        cats[3] = new Cat(4, "Test4", "Idk4", "Asd2 Qwe1 Zxc3", new Address("Харків", "BC", "123"));
        cats[4] = new Cat(5, "Test5", "Idk5", "Asd1 Qwe2 Zxc3", new Address("Харків", "BC", "123"));
        cats[5] = new Cat(6, "Test6", "Idk6", "Asd1 Qwe3 Zxc2", new Address("Харків", "BC", "123"));
        cats[6] = new Cat(7, "Test7", "Idk7", "Asd3 Qwe1 Zxc2", new Address("AB", "BC", "123"));
        cats[7] = new Cat(8, "Test8", "Idk8", "Asd1 Qwe3 Zxc2", new Address("Харків", "BC", "123"));
        cats[8] = new Cat(9, "Test9", "Idk9", "Asd1 Qwe2 Zxc3", new Address("Харків", "BC", "123"));
        cats[9] = new Cat(10, "Test0", "Idk10", "Asd4 Qwe2 Zxc3", new Address("Харків", "BC", "123"));

        foreach (Cat cat in cats) {
            if(!cat.OwnerAddress.City.Contains("Харків"))
                Console.WriteLine(cat.ToString());
        }
    }
}