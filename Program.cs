using System;

class Program
{
    static void Main(string[] args)
    {
        // 1.Create a new Tiger object
        Tiger myTiger = new Tiger();
        myTiger.Name = "Tony";

        // Call methods from the base class and interfaces
        myTiger.Eat();    // From Animal base class
        myTiger.Walk();   // From ICanWalk interface
        myTiger.Hunt();   // From ICanHunt interface

        // 2.Test our Snake 
        Snake mySnake = new Snake();
        mySnake.Name = "Sammy";
        mySnake.Eat();    // From Animal base class
        mySnake.Crawl();  // From ICanCrawl interface
        mySnake.Hunt();   // From ICanHunt interface
        mySnake.Swim();   // From ICanSwim interface

        // 3. Test our Dog
        Dog myDog = new Dog();
        myDog.Name = "Buster";
        
        myDog.Walk();
        myDog.Hunt();
        myDog.Jump();
        myDog.Crawl();
        myDog.Swim();      
    }
}

// 1. The main blueprint (Base Class)
public class Animal
{
    public string Name { get; set; }

    public void Eat()
    {
        Console.WriteLine($"{Name} is eating food.");
    }
}

// 2. Interfaces (Rulebooks for actions)
public interface ICanWalk
{
    void Walk();
}

public interface ICanHunt
{
    void Hunt();
}

public interface ICanCrawl
{
    void Crawl();
}

public interface ICanJump
{
    void Jump();
    
}
public interface ICanSwim
{
    void Swim();
}

// 1. The Tiger class
public class Tiger : Animal, ICanWalk, ICanHunt
{
    public void Walk()
    {
        Console.WriteLine($"{Name} the tiger is walking.");
    }

    public void Hunt()
    {
        Console.WriteLine($"{Name} the tiger is hunting.");
    }
}

// 2. The Snake class
public class Snake : Animal, ICanCrawl, ICanHunt, ICanSwim
{
    public void Crawl()
    {
        Console.WriteLine($"{Name} the snake is crawling.");
    }

    public void Hunt()
    {
        Console.WriteLine($"{Name} the snake is hunting.");
    }

    public void Swim()
    {
        Console.WriteLine($"{Name} the snake is swimming.");
    }
}

    public class Dog : Animal, ICanWalk, ICanHunt, ICanJump, ICanCrawl, ICanSwim
    {
        public void Walk()
        {
            Console.WriteLine($"{Name} the dog is walking.");
        }

        public void Hunt()
        {
            Console.WriteLine($"{Name} the dog is hunting.");
        }

        public void Jump()
        {
            Console.WriteLine($"{Name} the dog is jumping.");
        }

        public void Crawl()
        {
            Console.WriteLine($"{Name} the dog is crawling.");
        }

        public void Swim()
        {
            Console.WriteLine($"{Name} the dog is swimming.");
        }
    }

  

