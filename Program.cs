using System;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Timers;

namespace BookAppOnline
{
    
    public class Books 
    {
        public static List<Books> books1 = new List<Books>();
        public string bookName {  get; set; }
        public string bookAutor {  get; set; }
        public string bookGaner { get; set; }
        public bool isAvailable { get; set; }

        public Books(string bookName, string bookAutor, string bookGaner, bool isAvailable)
        {
            this.bookName = bookName;
            this.bookAutor = bookAutor;
            this.bookGaner = bookGaner;
            this.isAvailable = isAvailable;
        }
    }
    public class Library
    {
        public void AddBook()
        {          
            Console.WriteLine("Enter book name.");
            Console.Write(">>> ");
            string booknameus = Console.ReadLine();
            Console.WriteLine("Enter book's autor name.");
            Console.Write(">>> ");
            string bookautorus = Console.ReadLine();
            Console.WriteLine("Enter book's ganer.");
            Console.Write(">>> ");
            string bookganerus = Console.ReadLine();
            Books books = new Books(booknameus, bookautorus, bookganerus, true);
            Books.books1.Add(books);

        }
        public void ReserveBook(string bookName)
        {
            Console.WriteLine("Enter book name.");
            Console.Write(">>> ");
            bookName = Console.ReadLine();

            var bookQuerty = from reser in Books.books1
                             where reser.bookName == bookName && reser.isAvailable == true
                             select reser;
            var foundreser = bookQuerty.FirstOrDefault();


        }
        public void RemoveBook()
        {
            
        }
        public void GetAvailableBooks()
        {
            var bookQuerty = from avalible in Books.books1
                             where avalible.isAvailable == true
                             select avalible;
            var foundAval = bookQuerty.FirstOrDefault();

            if (foundAval != null) 
            {
                Console.WriteLine($"Book Name: {foundAval.bookName}");
                Console.WriteLine($"Author: {foundAval.bookAutor}");
                Console.WriteLine($"Genre: {foundAval.bookGaner}");
                Console.WriteLine($"Available: {foundAval.isAvailable}");
            }
        }
        public void GetInfo(string bookName)
        {
            var bookQuery = from book in Books.books1
                            where book.bookName == bookName
                            select book;
            var foundBook = bookQuery.FirstOrDefault();

            if (foundBook != null)
            {
                Console.WriteLine($"Book Name: {foundBook.bookName}");
                Console.WriteLine($"Author: {foundBook.bookAutor}");
                Console.WriteLine($"Genre: {foundBook.bookGaner}");
                Console.WriteLine($"Available: {foundBook.isAvailable}");
            }
            else
            {
                Console.WriteLine("Book not found!");
            }
            
        }
    }
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public UserRoles Role { get; set; }
    }

    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }        
    }

    class Program
    {
        
        static User adminUser;
        static void Main()
        {
            try
            {
                
                Console.Title = "Library - <<Loading...>>";
                Console.WriteLine("Loading...");
                using (var context = new ApplicationDbContext())
                {
                    context.Database.CreateIfNotExists();
                    if (!context.Users.Any(u => u.Role == UserRoles.Admin))
                    {
                        string adminUsername = "Library's Admin";
                        string adminPassword = "Admin123"; 
                        string adminPasswordHash = HashPassword(adminPassword);

                        var adminUser = new User
                        {
                            Username = adminUsername,
                            PasswordHash = adminPasswordHash,
                            Role = UserRoles.Admin
                        };

                        context.Users.Add(adminUser);
                        context.SaveChanges();
                    }
                }

                Console.Title = "Library - <<Hub>>";

                while (true)
                {
                    Console.Clear();
                    Console.WriteLine("Welcome to our library!");
                    Console.WriteLine(new string('-', 50));
                    Console.WriteLine("Please select.");
                    Console.WriteLine();
                    Console.WriteLine("1 - Register\n2 - Login\n0 - Exit");
                    Console.WriteLine(new string('-', 50));
                    Console.Write(">>> ");

                    sbyte choice = sbyte.Parse(Console.ReadLine());

                    switch (choice)
                    {
                        case 0:
                            int frequency = 2000; 
                            int duration = 3000;
                            Console.Beep(frequency, duration);
                            Console.BackgroundColor = ConsoleColor.DarkGray;
                            Console.CursorVisible = false;
                            Console.WriteLine(new string('-', 50));
                            Console.WriteLine("Thank you for using our app!");
                            Console.WriteLine(new string('-', 50));
                            Thread.Sleep(1000);
                            Environment.Exit(0);
                            break;
                        case 1:
                            Register();
                            break;
                        case 2:
                            Login();
                            break;
                        default:
                            Console.WriteLine("Invalid choice. Press Enter to continue.");
                            Console.ReadLine();
                            break;
                    }
                }
            }
            catch(FormatException) 
            {
                Console.Clear();
                Console.WriteLine("Invalid choice.");
                Thread.Sleep(700);
                Main();
            }

        }

        static void Register()
        {
            Console.Title = "Library - <<Registration-Menu>>";
            Console.Clear();
            Console.WriteLine("Register.");
            Console.WriteLine(new string('-', 50));
            Console.WriteLine("Enter your UserName.");
            Console.Write(">>> ");
            string username = Console.ReadLine();
            Console.WriteLine("Enter your password. ");
            Console.Write(">>> ");
            string password = Console.ReadLine();
            
            using (var context = new ApplicationDbContext())
            {
                if (context.Users.Any(u => u.Username == username))
                {
                    Console.WriteLine(new string('↓', 50));
                    Console.WriteLine("User with this username already exists!");
                    Console.WriteLine("Press 'ENTER' to continue.\t To back to MainMenu, press 'M'.\nTo exit - press 'ESC'.");
                    Console.WriteLine(">>> ");
                    ConsoleKeyInfo keyInfo = Console.ReadKey();
                    if(keyInfo.Key == ConsoleKey.Enter)
                    {
                        Register();
                    }
                    else if(keyInfo.Key == ConsoleKey.Escape)
                    {
                        Console.WriteLine(new string('-', 50));
                        Console.WriteLine("Thank you for using our app!");
                        Console.WriteLine(new string('-', 50));
                        Thread.Sleep(1000);
                        Environment.Exit(0);
                    }
                    else if( keyInfo.Key == ConsoleKey.M)
                    {
                        Console.Clear();
                        Console.WriteLine("Loading...");
                        Thread.Sleep(1000);
                        Main();
                    }
                    else
                    {
                        Console.WriteLine("Unkown command! Return to Menu.");
                        Console.WriteLine(new string('-', 50));
                        Console.WriteLine("Loading...");
                        Thread.Sleep(2000);
                        Main();
                    }
                }
                else
                {
                    string passwordHash = HashPassword(password);

                    var user = new User
                    {
                        Username = username,
                        PasswordHash = passwordHash,
                        Role = UserRoles.User
                    };

                    context.Users.Add(user);
                    context.SaveChanges();
                    Console.WriteLine("Registration successful. Press Enter to continue.");
                    Console.WriteLine(new string ('-', 50));
                    Console.ReadLine();
                }
            }
        }
        static void Login()
        {
            
            Console.Clear();
            Console.WriteLine("Login");
            Console.Write("Enter username: ");
            string username = Console.ReadLine();
            Console.Write("Enter password: ");
            string password = Console.ReadLine();

            using (var context = new ApplicationDbContext())
            {
                var user = context.Users.FirstOrDefault(u => u.Username == username);

                if (user != null && VerifyPassword(password, user.PasswordHash))
                {
                    Console.CursorVisible = false;
                    Console.Clear();

                    Console.WriteLine("Login successful.");
                    Thread.Sleep(1000);
                    Console.Clear();
                    Console.CursorVisible = true;

                    if (user.Role == UserRoles.Admin)
                    {
                        AdminLogin();
                    }
                    else if(user.Role == UserRoles.User)
                    {
                        Console.Clear();
                        Console.WriteLine("Welcome!");
                        Console.WriteLine("You are logged in as a regular user.");
                        Console.WriteLine(new string('_', 50));
                        Console.WriteLine("Please choose one of the options.");
                        Console.WriteLine();
                        Console.WriteLine("1 - Enter to your personal account.");
                        Console.WriteLine("2 - Go to the chat.");
                        Console.WriteLine("3 - Add book to the library.");
                        Console.WriteLine("4 - View book by title");

                        sbyte userChooseans = sbyte.Parse(Console.ReadLine());

                        switch (userChooseans)
                        {
                            case 1:
                                Console.WriteLine($"Welcome, {username}!");
                                Console.WriteLine(new string('_', 50));
                                Console.WriteLine("Change your password - 1");
                                Console.WriteLine("Change your username - 2");

                                byte userchoose = byte.Parse(Console.ReadLine());

                                switch (userchoose)
                                {
                                    case 1:
                                        ChangePassword();
                                        break;
                                }
                                break;
                            case 2:
                                break;
                            case 3:
                                break;
                            case 4:
                                break;
                        }
                    }
                    else if (user.Role == UserRoles.BlackList)
                    {
                       BlackBan();
                    }

                }
                else
                {
                    Console.WriteLine("Invalid username or password. Press Enter to continue.");
                    Console.ReadLine();
                }
            }
        }
        static void BlackBan()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("You are in the Black-List!");
            Console.ResetColor();
            Thread.Sleep(3000);
            Main();
        }
       
        static void AdminLogin()
        {
            Console.Title = "Library - <<Admin Panel>>";
            Console.WriteLine("Welcome!");
            Console.WriteLine("You are logged in as an administrator.");
            Console.WriteLine(new string('_', 50));

            Console.WriteLine("1 - Change User Status");
            Console.WriteLine("2 - Show All Users");
            Console.WriteLine("3 - ");

           
            Console.WriteLine("0 - To Exit");



            sbyte AdminAns = sbyte.Parse(Console.ReadLine());

            switch (AdminAns)
            {
                case 0:
                    Environment.Exit(0);
                    break;
                case 1:
                    Console.Clear();
                    Console.WriteLine("Change User Status");
                    Console.Write("Enter username of the user whose status you want to change: ");
                    string targetUsername = Console.ReadLine(); // Измените имя переменной на "targetUsername"

                    using (var targetContext = new ApplicationDbContext())
                    {
                        var targetUser = targetContext.Users.FirstOrDefault(u => u.Username == targetUsername); // Измените имя переменной на "targetUser"

                        if (targetUser != null && targetUser != adminUser)
                        {
                            Console.WriteLine($"Current status of {targetUser.Username}: {targetUser.Role}");
                            Console.Write("Enter the new status (Admin/User/BlackList): ");
                            string newRoleString = Console.ReadLine();

                            if (Enum.TryParse(newRoleString, true, out UserRoles newRole))
                            {
                                targetUser.Role = newRole;
                                targetContext.SaveChanges();
                                Console.WriteLine($"Status of {targetUser.Username} successfully changed to {newRole}");
                            }
                            else
                            {
                                Console.WriteLine("Invalid status. Status not changed.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("User not found or you are trying to change the status of the admin account. Status not changed.");
                        }

                        Console.WriteLine("Press Enter to continue.");
                        Console.ReadLine();
                    }
                    break;
                case 2:
                    ShowAllUsers();
                    break;
            }
        }
        static void ShowAllUsers()
        {
            using (var context = new ApplicationDbContext())
            {
                var users = context.Users.ToList(); 

                if (users.Count > 0)
                {
                    Console.WriteLine("List of all users:");
                    foreach (var user in users)
                    {
                        Console.WriteLine($"Username: {user.Username}, Role: {user.Role}");
                    }
                }
                else
                {
                    Console.WriteLine("No users found in the database.");
                }

                Console.WriteLine("Press Enter to continue.");
                Console.ReadLine();
            }
        }
        static void ChangePassword()
        {
            Console.Clear();
            Console.Title = "Library - <<ChangePassword Menu>>";
            Console.WriteLine("Change Password");
            Console.WriteLine(new string('_', 50));
            Console.Write("Enter username: ");
            string username = Console.ReadLine();
            Console.Write("Enter old password: ");
            string oldPassword = Console.ReadLine();
            Console.Write("Enter new password: ");
            string newPassword = Console.ReadLine();

            using (var context = new ApplicationDbContext())
            {
                var user = context.Users.FirstOrDefault(u => u.Username == username);

                if (user != null && VerifyPassword(oldPassword, user.PasswordHash))
                {
                    string newPasswordHash = HashPassword(newPassword);
                    user.PasswordHash = newPasswordHash;
                    context.SaveChanges();
                    Console.WriteLine("Password successfully changed. Press Enter to continue.");
                    Console.ReadLine();
                }
                else
                {
                    Console.WriteLine("Old password is incorrect or user not found. Press Enter to continue.");
                    Console.ReadLine();
                }
            }
        }

        static void DeleteAccount()
        {
            Console.Clear();
            Console.WriteLine("Delete Account");
            Console.Write("Enter username: ");
            string username = Console.ReadLine();

            using (var context = new ApplicationDbContext())
            {
                var user = context.Users.FirstOrDefault(u => u.Username == username);

                if (user != null)
                {
                    Console.Write("Are you sure you want to delete the account? (yes/no): ");
                    string confirmation = Console.ReadLine();

                    if (confirmation.Equals("yes", StringComparison.OrdinalIgnoreCase))
                    {
                        context.Users.Remove(user);
                        context.SaveChanges();
                        Console.WriteLine("Account deleted. Press Enter to continue.");
                        Console.ReadLine();
                    }
                    else
                    {
                        Console.WriteLine("Account not deleted. Press Enter to continue.");
                        Console.ReadLine();
                    }
                }
                else
                {
                    Console.WriteLine("User not found. Press Enter to continue.");
                    Console.ReadLine();
                }
            }
        }

        static string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hash = sha256.ComputeHash(bytes);

                return BitConverter.ToString(hash).Replace("-", string.Empty);
            }
        }

        static bool VerifyPassword(string enteredPassword, string passwordHash)
        {
            string enteredPasswordHash = HashPassword(enteredPassword);
            return enteredPasswordHash == passwordHash;
        }
    }
    /// <summary>
    /// USER TAGS, LO LAGAAT!!!
    /// </summary>
    [Flags]
    public enum UserRoles
    {
        None = 0,
        Admin = 1,
        User = 2,
        BlackList = 3,
        ReadOnly = 4
    }
}