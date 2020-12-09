using System;
using System.Collections.Generic;
using System.IO;

namespace ATMMachineLab
{
    class Program
    {
        static void Main(string[] args)
        {
            bool shouldContinue = true;
            var myATM = new ATM();
            while (shouldContinue)
            {
                var selection = PromptForAction();
                RunAccountAction(selection, myATM);
                Console.Clear();
                while (myATM.CurrentAccount != null)
                {
                    Console.WriteLine($"Welcome {myATM.CurrentAccount.Name}!\n");
                    var transaction = PromptForTransaction();
                    RunTransaction(transaction, myATM);
                    if (ShouldDoSomethingElse())
                    {
                        continue;
                    }
                }
                shouldContinue = AreYouSure();
            }
            myATM.UpdateRecords();
        }
        public static uint PromptForAction() 
        {
            while (true)
            {
                Console.WriteLine("What would you like to do:");
                Console.WriteLine("[1]: Login");
                Console.WriteLine("[2]: Sign Up");
                Console.WriteLine("[3]: Close App");
                if (uint.TryParse(Console.ReadLine(), out uint selection) && selection < 4)
                {
                    return selection;
                }
                else
                {
                    Console.WriteLine("Sorry, that is an invalid selection. Try again.");
                }
            }
        }

        public static void RunAccountAction(uint selection, ATM myATM)
        {
            if (selection == 1)
            {
                while (true)
                {
                    Console.WriteLine("Please enter your user name:");
                    string userName = Console.ReadLine();
                    Console.WriteLine("Please enter your password:");
                    string password = Console.ReadLine();
                    if (myATM.Login(userName, password))
                    {
                        Console.WriteLine($"Great! {myATM.CurrentAccount.Name} has been logged in!");
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Sorry, looks like that account does not exist");
                        continue;
                    }
                }  
            }
            if (selection == 2)
            {
                while (true)
                {
                    Console.WriteLine("Please enter your user name:");
                    var userName = Console.ReadLine();
                    Console.WriteLine("Please enter your password:");
                    var userPassword = Console.ReadLine();
                    Console.WriteLine("Please confirm your password");
                    if (Console.ReadLine() != userPassword)
                    {
                        Console.WriteLine("Sorry! That password does not match.");
                        continue;
                    }
                    if (myATM.Register(userName, userPassword))
                    {
                        Console.WriteLine($"Great! New Account Created for {userName}");
                        break;
                    }
                    else
                    {
                        Console.WriteLine($"I'm sorry, looks like there is an account already for {userName}");
                        continue;
                    }
                    
                }
                
            }
        }
        public static uint PromptForTransaction()
        {
            while (true)
            {
                Console.WriteLine("What would you like to do:");
                Console.WriteLine("[1]: Log Out");
                Console.WriteLine("[2]: Check Balance");
                Console.WriteLine("[3]: Deposit");
                Console.WriteLine("[4]: Withdraw");
                if (uint.TryParse(Console.ReadLine(), out uint selection) && selection < 5)
                {
                    return selection;
                }
                else
                {
                    Console.WriteLine("Sorry, that is an invalid selection. Try again.");
                }
            }
        }
        public static void RunTransaction(uint selection, ATM myATM)
        {
            switch (selection)
            {
                case 1:
                    myATM.Logout();
                    break;
                case 2:
                    myATM.CheckBalance();
                    break;
                case 3:
                    Console.WriteLine("How much would you like to deposit?");
                    if (uint.TryParse(Console.ReadLine(), out uint amount))
                    {
                        myATM.Deposit(amount);
                        Console.WriteLine($"Great. Deposit of ${amount} was successful.");
                        Console.WriteLine($"Your current balance is ${myATM.CurrentAccount.Balance}");
                    }
                    else
                    {
                        Console.WriteLine("Sorry, that is not a valid selection");
                    }
                    break;
                case 4:
                    while (true)
                    {
                        Console.WriteLine("How much would you like to withdraw?");
                        if (uint.TryParse(Console.ReadLine(), out uint Wamount))
                        {
                            if (myATM.Withdraw(Wamount))
                            {
                                Console.WriteLine($"Great. withdraw of ${Wamount} was successful.");
                                Console.WriteLine($"Your current balance is ${myATM.CurrentAccount.Balance}");
                                break;
                            }
                            else
                            {
                                Console.WriteLine("Sorry, you do not have sufficent funds.");
                                break;
                            }
                        }
                        else
                        {
                            Console.WriteLine("Sorry, that is not a valid selection");
                            continue;
                        }
                    }
                    break;
                default:
                    break;
            }
        }
        public static bool ShouldDoSomethingElse()
        {
            while (true)
            {
                Console.WriteLine("Would you like to do something else?\n[1]: Yes\n[2]: No");
                if (uint.TryParse(Console.ReadLine(), out uint continueSelection))
                {
                    if (continueSelection == 1)
                    {
                        Console.Clear();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    Console.WriteLine("Sorry, that is not a valid selection.");
                }
            }
        }
        public static bool AreYouSure()
        {
            while (true)
            {
                Console.WriteLine("Are you sure?\n[1]: Yes\n[2]: No");
                if (uint.TryParse(Console.ReadLine(), out uint continueSelection))
                {
                    if (continueSelection == 1)
                    {
                        Console.Clear();
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    Console.WriteLine("Sorry, that is not a valid selection.");
                }
            }
        }
    }

    public class ATM
    {
        public ATM()
        {
            ActiveAccounts = BuildAccountsFromText();
            CurrentAccount = null;
        }
        public Account CurrentAccount { get; set; }
        public List<Account> ActiveAccounts{ get; set; }
        public List<Account> BuildAccountsFromText()
        {
            var myAccounts = new List<Account>();
            string[] myData = File.ReadAllLines("../../../accounts.txt");
            for (int i = 0; i < myData.Length; i = i + 3)
            {
                var userName = myData[i];
                var userPassword = myData[i + 1];
                var stringBalance = myData[i + 2];
                double.TryParse(stringBalance, out double userBalance);
                myAccounts.Add(new Account(userName, userPassword, userBalance));
            }

            return myAccounts;
        }
        public void UpdateRecords()
        {
            foreach (Account account in ActiveAccounts)
            {
                var sw = new StreamWriter("../../../accounts.txt", false);
                using (sw)
                {
                    sw.WriteLine(account.Name);
                    sw.WriteLine(account.Password);
                    sw.WriteLine(account.Balance);
                }
                sw.Close();
                var totalString = File.ReadAllText("../../../accounts.txt");
                File.WriteAllText("../../../accounts.txt", totalString.TrimEnd());
            }
        }
        public void PrintAllAccounts()
        {
            foreach (Account account in ActiveAccounts)
            {
                Console.WriteLine($"{account.Name}");
                Console.WriteLine($"{account.Password}");
                Console.WriteLine($"{account.Balance}");
            }
        }
        public bool Register(string name, string password)
        {
            int accountsThatMatch = 0;
            foreach (Account account in ActiveAccounts)
            {
                if (account.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    accountsThatMatch ++;
                }
            }
            if (accountsThatMatch < 1)
            {
                var newAccount = new Account(name, password);
                CurrentAccount = newAccount;
                ActiveAccounts.Add(newAccount);
                return true;
            }
            else return false;


        }
        public bool Login(string userName, string password) 
        {
            foreach (Account account in ActiveAccounts)
            {
                if ((account.Name == userName) && (account.Password == password))
                {
                    var newAccount = new Account(account.Name, account.Password, account.Balance);
                    CurrentAccount = newAccount;
                    return true;
                }
            }
            return false;
        }
        public void Logout()
        {
            Console.WriteLine($"Thanks {CurrentAccount.Name}!\nLogging Out...");
            CurrentAccount = null;
        }
        public void CheckBalance()
        {
            Console.WriteLine($"Name: {CurrentAccount.Name}");
            Console.WriteLine($"Balance: ${CurrentAccount.Balance}");
        }
        public void Deposit(uint depositAmount)
        {
            CurrentAccount.Balance = CurrentAccount.Balance + depositAmount;
        }
        public bool Withdraw(uint withdrawAmount)
        {
            if (CurrentAccount.Balance > withdrawAmount)
            {
                CurrentAccount.Balance = CurrentAccount.Balance - withdrawAmount;
                return true;
            }
            return false;
        }
    }
    public class Account
    {
        public Account(string name, string password)
        {
            Name = name;
            Password = password;
            Balance = 0;
        }
        public Account(string name, string password, double balance)
        {
            Name = name;
            Password = password;
            Balance = balance;
        }
        public string Name { get; set; }
        public string Password { get; set; }
        public double Balance { get; set; }
    }
}
