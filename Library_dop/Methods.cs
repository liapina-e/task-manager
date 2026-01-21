using System.ComponentModel;
using System.Globalization;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Spectre.Console;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
namespace Library_dop;
/// <summary>
/// В статическом классе Methods представлены все необходимые для решения задач методы.
/// </summary>
public static class Methods
{
    /// <summary>
    /// В методе считываем все данные из CSV файла в строковый массив.
    /// Проверяем путь к файлу на корректность.
    /// Метод считывает файл асинхронно.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    /// <exception cref="FileNotFoundException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public static async Task<string[]> DataEntryFromCSV(string path)
    {
        string[] str = [];
        try
        {
            // Создаем массив строк, состоящий из данных из файла.
            // Если файла не существует или он соостоит только из некорректынх данных, то обрабатываем исключения.
            if (!File.Exists(path))
            {
                throw new FileNotFoundException();
            }

            if (path.Length == 0 || path is null)
            {
                throw new ArgumentException();
            }

            str = await File.ReadAllLinesAsync(path);
            return str;
        }
        catch (FileNotFoundException)
        {
            Console.Write("Входной Файл на диске отсутствует.");
            Console.WriteLine("Необходимо перезапустить программу.");
            throw;
        }
        catch (ArgumentException)
        {
            Console.Write("Корректных данных в файле нет.");
            Console.WriteLine("Необходимо перезапустить программу.");
            throw;
        }
        catch (IOException)
        {
            Console.Write("Проблемы с путем к файлу.");
            Console.WriteLine("Необходимо перезапустить программу.");
            throw;
        }
    }
    /// <summary>
    /// В методе обрабатываем строковый массив, полученный из метода DataEntryFromCSV.
    /// Проверяем файл на корректность.
    /// Метод возвращает коллекцию объектов типа Task.
    /// Метод считывает файл асинхронно.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    /// <exception cref="FileNotFoundException"></exception>
    public static async Task<List<Task>> ParseFromCSV(string path)
    {
        string[] str = await DataEntryFromCSV(path);
        try
        {
            if (str[0] != "ID,Status,Priority,Description,Deadline")
            {
                throw new FileNotFoundException();
            }
        }
        
        catch(FileNotFoundException)

        {
            Console.WriteLine("Шапка не соответствует файлу.");
            Console.WriteLine("Попробуйте еще раз.");
        }
        List<Task> tasks = new List<Task>();
        foreach (string s in str[1..])
        {
            string[] fields = s.Split(',');
            Task task = new Task(int.Parse(fields[0]), fields[1], fields[2], fields[3], fields[4]);
            tasks.Add(task);
        }
        Console.WriteLine("Данные из файла загружены.");
        return tasks;
    }
    /// <summary>
    /// В методе обрабатываем JSON файл.
    /// Проверяем путь к JSON файлу на корректность.
    /// Метод возвращает коллекцию объектов типа Task.
    /// Метод считывает файл асинхронно.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    /// <exception cref="FileNotFoundException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public static async Task<List<Task>?> ParseFromJSON(string path)
    {
        try
        {
            // Если файла не существует или он соостоит только из некорректынх данных, то обрабатываем исключения.
            if (!File.Exists(path))
            {
                throw new FileNotFoundException();
            }

            if (path.Length == 0 || path is null)
            {
                throw new ArgumentException();
            }
            string json = await File.ReadAllTextAsync(path);
            // Десериализация JSON в динамический объект, так как сразу коллекцию мы считать не можем.
            dynamic _tasks = JsonConvert.DeserializeObject<dynamic>(json);
            List<Task> tasks = new List<Task>();
            foreach (dynamic t in _tasks.elements)
            {
                Task task = new Task(int.Parse((string)t.ID), (string)t.Status, (string)t.Priority, (string)t.Description, (string)t.Deadline);
                tasks.Add(task);
            }
            Console.WriteLine("Данные из файла загружены.");
            return tasks;

        }
        catch (FileNotFoundException)
        {
            Console.Write("Входной Файл на диске отсутствует.");
            Console.WriteLine("Необходимо перезапустить программу.");
            throw;
        }
        catch (ArgumentException)
        {
            Console.Write("Корректных данных в файле нет.");
            Console.WriteLine("Необходимо перезапустить программу.");
            throw;
        }
        catch (IOException)
        {
            Console.Write("Проблемы с путем к файлу.");
            Console.WriteLine("Необходимо перезапустить программу.");
            throw;
        }
    }
    /// <summary>
    /// В методе выводим все задачи. на экран.
    /// Если дедлайн просрочен, выводим красным цветом.
    /// </summary>
    /// <param name="tasks"></param>
    public static void ViewingTasks(List<Task> tasks)
    {
        foreach (Task task in tasks)
        {
            if (DateTime.Now > task.Deadline)
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.White;
            }
            Console.WriteLine(task.ToString());
        }
        // Console.ForegroundColor = ConsoleColor.White;
        Console.ResetColor();
    }
    /// <summary>
    /// В методе добавляем новую задачу в нашу коллекцию.
    /// </summary>
    /// <param name="tasks"></param>
    /// <returns></returns>
    public static Task AddTask(List<Task> tasks)
    {
        Console.WriteLine("Введите описание задачи:");
        string description = Console.ReadLine();
        string priority, deadline;
        // Циклы будут идти, пока пользователь не введет корректные данные.
        while (true)
        {
            Console.WriteLine("Введите приоритет задачи:");
            priority = Console.ReadLine();
            if (priority == "Низкий" || priority == "Средний" || priority == "Высокий")
            {
                break;
            }
            Console.WriteLine("Такой приоритет нельзя поставить.");
        }
        while (true)
        {
            Console.WriteLine("Введите дедлайн задачи:");
            deadline = Console.ReadLine();
            if (DateTime.TryParseExact(deadline, "dd MMMM yyyy HH:mm:ss",new CultureInfo("ru-RU"), DateTimeStyles.None,out DateTime deadlineDate))
            {
                break;
            }
            Console.WriteLine("Такой дедлайн нельзя поставить.");
        }

        Task task = new Task(tasks.Count + 1,"TODO", priority, description, deadline);
        return task;
    }
    /// <summary>
    /// В методе меняем статус заадчи.
    /// </summary>
    /// <param name="tasks"></param>
    /// <param name="id"></param>
    public static void EditStatus(List<Task> tasks, int id)
    {
        string status;
        while (true)
        {
            Console.WriteLine("Введите новый статус задачи:");
            status = Console.ReadLine();
            if (status == "TODO" || status == "IN_PROGRESS" || status == "DONE")
            {
                break;
            }
            Console.WriteLine("Такой статус нельзя поставить.");
        }
        foreach (Task task in tasks)
        {
            if (task.ID == id)
            {
                // Проверяем, можем ли мы поставить статус завершения, т.к. у задачи могут быть зависимые.
                if (status == "DONE")
                {
                    if (task.CanBeDone())
                    {
                        task.Status = status;
                        Console.WriteLine("Статус задачи изменен.");
                        return;
                    }
                    else
                    {
                        Console.WriteLine("Не все зависимые задачи выполнены. Нельзя изменить статус на DONE.");
                        return;
                    }
                }
                else
                {
                    task.Status = status;
                    return;
                }
            }
            
        }
        Console.WriteLine("Такого id не существует.");
    }
    /// <summary>
    /// В методе удаляем задачу по введеному пользователем id.
    /// </summary>
    /// <param name="tasks"></param>
    /// <param name="id"></param>
    public static void RemoveTask(List<Task> tasks, int id)
    {
        if (tasks.Count == 0 || tasks == null)
        {
            Console.WriteLine("Задач нет. Создайте их.");
            return;
        }
        foreach (Task task in tasks)
        {
            if (task.ID == id)
            {
                tasks.Remove(task);
                Console.WriteLine("Задача удалена.");
                return;
            }
        }
        Console.WriteLine("Такого id не существует.");
    }
    /// <summary>
    /// В методе записываем нашу коллекцию в CSV файл.
    /// Записываем в файл асинхронно.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="tasks"></param>
    public static async System.Threading.Tasks.Task WriteToCsv(string path, List<Task> tasks)
    {
        try
        {
            using (StreamWriter writer = new StreamWriter(path))
            {
                await writer.WriteLineAsync("ID,Status,Priority,Description,Deadline");

                foreach (Task task in tasks)
                {
                    await writer.WriteLineAsync($"{task.ID.ToString()},{task.Status},{task.Priority},{task.Description},{task.Deadline.ToString()}");
                }
                Console.WriteLine("Данные записаны в CSV файл.");
            }

        }
        catch (IOException)
        {
            Console.Write("Проблемы с открытием файла");
            Console.WriteLine("Необходимо перезапустить программу.");
            throw;
        }
    }
    /// <summary>
    /// В методе записываем нашу коллекцию в JSON файл.
    /// Записываем в файл асинхронно.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="tasks"></param>
    public static async System.Threading.Tasks.Task WriteToJSON(string path, List<Task> tasks)
    {
        try
        {
            string json = JsonConvert.SerializeObject(tasks, Formatting.Indented);

            await File.WriteAllTextAsync(path, json);
            Console.WriteLine("Данные записаны в JSON файл.");

        }
        catch (IOException)
        {
            Console.Write("Проблемы с открытием файла");
            Console.WriteLine("Необходимо перезапустить программу.");
            throw;
        }
    }
    /// <summary>
    /// Мутод выводит нашу коллекцию в виде таблицы.
    /// </summary>
    /// <param name="tasks"></param>
    public static void OutputToTable(List<Task> tasks)
    {
        Table table = new Table();
        table.AddColumn("ID");
        table.AddColumn("Статус");
        table.AddColumn("Приоритет");
        table.AddColumn("Описание задачи");
        table.AddColumn("Дата создания");
        table.AddColumn("Дата изменения");
        table.AddColumn("Дедлайн");
        DateTime time = DateTime.Now;   // Переменная для обозначения времени в данный момент.
        for (int i = 0; i < tasks.Count; i++)
        {
            Task task = tasks[i];
            // Добавляем изменения цвета у просроченных задач.
            if (task.Deadline > time)
            {
                table.AddRow(task.ID.ToString(), task.Status, task.Priority, task.Description,task.Deadline.ToString());
            }
            else
            {
                table.AddRow(new Markup($"[red]{task.ID}[/]"), new Markup($"[red]{task.Status}[/]"),
                    new Markup($"[red]{task.Priority}[/]"), new Markup($"[red]{task.Description}[/]"), new Markup($"[red]{task.Deadline}[/]"));
            }
        }
        
        AnsiConsole.Write(table);
    }
    /// <summary>
    /// В методе фильтруем и сортируем данные в таблице, пока пользователь сам не захочет выйти.
    /// </summary>
    /// <param name="tasks"></param>
    public static void FilteringAndSorting(List<Task> tasks)
    {
        // Коллекция, с которой будем работать, чтоб не менять исходную.
        List<Task> filteredTasks = tasks;
        string menu = "Выберете пункт:\n1. Отфильтровать по id\n2. Отфильтровать по статусу\n.3. Отфильтровать по приоритету\n4. Отфильтровать по описанию\n" +
                      "5. Отфильтровать по дате создания\n6. Отфильтровать по дате изменения\n7. Отсортировать по id\n8. Отсортировать по статусу\n9. Отсортировать по приоритету\n10. Отсортировать по описанию\n" +
                      "11. Отсортировать по дате создания\n12. Отсортировать по дате изменения\n13. Выйти";
        Console.WriteLine(menu);
        // Цикл идет, пока пользователь не захочет выйти.
        while (true)
        {
            int num =int.Parse(Console.ReadLine());
            if (num == 1)
            {
                Console.WriteLine("Введите значения, которые хотите оставить");
                Console.WriteLine("Для выхода введите stop");
                List<int> list = new List<int>();
                string input = null;
                while (true)
                {
                    input = Console.ReadLine();
                    if (int.TryParse(input, out int num2))
                    {
                        list.Add(num2);
                    }
                    else
                    {
                        if (input == "stop")
                        {
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Повторите ввод.");
                        }
                    }
                }
                foreach (Task task in filteredTasks)
                {
                    if (list.Contains(task.ID) == false)
                    {
                        filteredTasks.Remove(task);
                    }
                }
            }
            else if (num > 1 && num <= 3)
            {
                Console.WriteLine("Введите значения, которые хотите оставить");
                Console.WriteLine("Для выхода введите stop");
                List<string> list = new List<string>();
                string input = null;
                while (true)
                {
                    input = Console.ReadLine();
                    if (input == "stop")
                    {
                        break;
                    }

                    list.Add(input);

                }

                if (num == 2)
                {
                    foreach (Task task in filteredTasks)
                    {
                        if (list.Contains(task.Status) == false)
                        {
                            filteredTasks.Remove(task);
                        }
                    }
                }
                else if (num == 3)
                {
                    foreach (Task task in filteredTasks)
                    {
                        if (list.Contains(task.Status) == false)
                        {
                            filteredTasks.Remove(task);
                        }
                    }
                }

                else if (num == 4)
                {
                    // Проверяем, есть ли в описании задачи слово, введеное пользователем.
                    foreach (Task task in filteredTasks)
                    {
                        bool flag = false;
                        foreach (string word in list)
                        {
                            if (task.Description.Contains(word))
                            {
                                flag = true;
                            }
                        }

                        if (flag == false)
                        {
                            filteredTasks.Remove(task);
                        }
                    }
                }
            }
            else if (num == 5)
            {
                Console.WriteLine("Введите день, по которому фильтровать в виде например \"01 Март\":");
                string input = Console.ReadLine();
                foreach (Task task in filteredTasks)
                {
                    if (task.DateOfCreation.ToString().Contains(input) == false)
                    {
                        filteredTasks.Remove(task);
                    }
                }
            }
            else if (num == 6)
            {
                Console.WriteLine("Введите день, по которому фильтровать в виде например \"01 Март\":");
                string input = Console.ReadLine();
                foreach (Task task in filteredTasks)
                {
                    if (task.DateOfEdit.ToString().Contains(input) == false)
                    {
                        filteredTasks.Remove(task);
                    }
                }
            }
            else if (num > 6 && num < 13)
            {
                string fieldToCompare = null;
                switch (num)
                {
                    case 7: fieldToCompare = "id"; break;
                    case 8: fieldToCompare = "status"; break;
                    case 9:
                    {
                        for (int i = 0; i < tasks.Count-1; i++)
                        {
                            if (tasks[i].Status != tasks[i + 1].Status)
                            {
                                if ((tasks[i].Status == "Низкий" &&
                                     (tasks[i + 1].Status == "Средний" || tasks[i + 1].Status == "Высокий"))
                                    || (tasks[i].Status == "Средний" && tasks[i + 1].Status == "Высокий"))
                                {
                                    (tasks[i], tasks[i+1]) = (tasks[i+1], tasks[i]);
                                }
                            }
                        }
                        break;
                    } ;
                    case 10: fieldToCompare = "description"; break;
                    case 11: fieldToCompare = "dateOfCreation"; break;
                    case 12: fieldToCompare = "dateOfEdit"; break;

                }

                if (num != 9)
                {
                    TaskComparer tc = new TaskComparer(fieldToCompare);
                    filteredTasks.Sort(tc.Compare);
                }
                
            }
            else if (num == 13)
            {
                break;
            }
            else
            {
                Console.WriteLine("Такого пункта меню не существует.");
            }
            OutputToTable(filteredTasks);
        }
    }
    /// <summary>
    /// В методе добавляем зависимые задачи.
    /// </summary>
    /// <param name="tasks"></param>
    /// <param name="id1"></param>
    /// <param name="id2"></param>
    public static void AddDependenceTask(List<Task> tasks, int id1, int id2)
    {
        foreach (Task task1 in tasks)
        {
            if (task1.ID == id1)
            {
                foreach (Task task2 in tasks)
                {
                    if (task2.ID == id2)
                    {
                        task1.AddDependency(task2);
                        Console.WriteLine($"Задача {id2} добавлена в зависимость.");
                        return;
                    }
                }
            }
        }
        Console.WriteLine("Введены неверные id задач. Попробуйте снова.");
    }
    /// <summary>
    /// В методе удаляем зависимые задачи.
    /// </summary>
    /// <param name="tasks"></param>
    /// <param name="id1"></param>
    /// <param name="id2"></param>
    public static void DeleteDependenceTask(List<Task> tasks, int id1, int id2)
    {
        foreach (Task task1 in tasks)
        {
            if (task1.ID == id1)
            {
                foreach (Task task2 in tasks)
                {
                    if (task2.ID == id2)
                    {
                        task1.DeleteDependency(task2);
                        Console.WriteLine($"Задача {id2} удвлена.");
                        return;
                    }
                }
            }
        }
        Console.WriteLine("Введены неверные id задач. Попробуйте снова.");
    }
    /// <summary>
    /// Метод напоминает о приближающихся дедлайнах.
    /// Метод работает в фоновом режиме.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="doWorkEventArgs"></param>
    public static void RemindAboutDeadline(object? sender, DoWorkEventArgs doWorkEventArgs)
    {
        // Передаем список в качестве аргумента, чтоб метод работал корректно в фоновом режиме.
        List<Task> tasks = doWorkEventArgs.Argument as List<Task>;
        DateTime time = DateTime.Now;
        foreach (Task task in tasks)
        {
            if (task.Deadline > time)
            {
                TimeSpan remainingTime= task.Deadline - time;
                if (remainingTime.Days == 1)
                {
                    Console.WriteLine($"У вас остался один день на выполнение задачи {task.Description}");
                }
            }
        }
    }
    /// <summary>
    /// Метод для вывода напоминания о дедлайне задачи через тг бот.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="doWorkEventArgs"></param>
    /// <param name="bot"></param>
    /// <param name="chat"></param>
    public static void RemindAboutDeadlineInTg(object? sender, DoWorkEventArgs doWorkEventArgs, ITelegramBotClient bot, Chat chat)
    {
        // Передаем список в качестве аргумента, чтоб метод работал корректно в фоновом режиме.
        List<Task> tasks = doWorkEventArgs.Argument as List<Task>;
        DateTime time = DateTime.Now;
        foreach (Task task in tasks)
        {
            if (task.Deadline > time)
            {
                TimeSpan remainingTime= task.Deadline - time;
                if (remainingTime.Days == 1)
                {
                    bot.SendMessage(chat.Id, $"У вас остался один день на выполнение задачи: {task.Description}") ;
                }
            }
        }
    }
    /// <summary>
    /// Метод для создания нового проекта.
    /// </summary>
    /// <param name="tasks"></param>
    /// <returns></returns>
    public static Project CreateProject(List<Task> tasks)
    {
        Console.WriteLine("Введите имя проекта:");
        string name = Console.ReadLine();
        List<int> ids = new List<int>();
        while (true)
        {
            Console.WriteLine("Введите id задач, которые внести в проект:\n Для выхода введите -1");
            string input = Console.ReadLine();
            if (input == "-1")
            {
                break;
            }
            else
            {
                if (int.TryParse(input, out int id))
                {
                    ids.Add(id);
                }
            }
        }
        List<Task> tasksForProj = new List<Task>();
        foreach (Task task in tasks)
        {
            if (ids.Contains(task.ID))
            {
                tasksForProj.Add(task);
            }
        }
        Project project = new Project(name, tasksForProj);
        return project;
    }
    /// <summary>
    /// Метод для удаления проекта из коллекции проектов.
    /// </summary>
    /// <param name="projects"></param>
    public static void DeleteProject(List<Project> projects)
    {
        if (projects == null || projects.Count == 0)
        {
            Console.WriteLine("Проектов нет. Создайте его.");
            return;
        }
        Console.WriteLine("Введите название проекта:");
        string name = Console.ReadLine();
        foreach (Project project in projects)
        {
            if (project.Name == name)
            {
                projects.Remove(project);
                return;
            }
        }
        Console.WriteLine("Проекта с таким именем нет.");
    }
    /// <summary>
    /// Метод для переименовывания проекта.
    /// </summary>
    /// <param name="projects"></param>
    public static void RenameProject(List<Project> projects)
    {
        // Проверяем, что проекты существуют.
        if (projects == null || projects.Count == 0)
        {
            Console.WriteLine("Проектов нет. Создайте его.");
            return;
        }
        Console.WriteLine("Введите имя проекта, которое хотите поменять:");
        string name = Console.ReadLine();
        Console.WriteLine("Введите новое имя проекта:");
        string newName = Console.ReadLine();
        foreach (Project project in projects)
        {
            if (project.Name == name)
            {
                project.Rename(name);
                Console.WriteLine("Имя проекта изменено.");
                return;
            }
        }
        Console.WriteLine("Проекта с таким именем нет.");
    }
    /// <summary>
    /// Метод для добавления задачи в проект.
    /// </summary>
    /// <param name="tasks"></param>
    /// <param name="projects"></param>
    public static void AddTaskToProj(List<Task> tasks, List<Project> projects)
    {
        Console.WriteLine("Введите имя проекта, которое хотите поменять:");
        string name = Console.ReadLine();
        Console.WriteLine("Введите id задачи, которую хотите добавить к проекту:");
        int id = int.Parse(Console.ReadLine());
        Project proj = null;
        foreach (Project project in projects)
        {
            if (project.Name == name)
            {
                proj = project;
                break;
            }
        }

        if (proj == null)
        {
            Console.WriteLine("Проекта с таким именем нет. Попробуйте еще раз.");
            return;
        }

        foreach (Task task in tasks)
        {
            if (task.ID == id)
            {
                proj.AddTask(task);
                Console.WriteLine($"Задача добавлена к проекту {name}");
                return;
            }
        }
        Console.WriteLine("Задачи с таким id нет.");
    }
    /// <summary>
    /// Метод для вывода всех задач проекта на экран.
    /// </summary>
    /// <param name="projects"></param>
    public static void DisplayTasksByProj(List<Project> projects)
    {
        // Проверяем, что проекты существуют.
        if (projects == null || projects.Count == 0)
        {
            Console.WriteLine("Проектов нет. Создайте его.");
            return;
        }
        foreach (Project project in projects)
        {
            Console.WriteLine(project.Name + ":");
            Console.WriteLine(project.ReviewTasks());
        }
    }
    /// <summary>
    /// Метод для представления статистики по проектам.
    /// </summary>
    /// <param name="projects"></param>
    public static void RewievStatisticsByProj(List<Project> projects)
    {
        // Проверяем, что проекты существуют.
        if (projects == null || projects.Count == 0)
        {
            Console.WriteLine("Проектов нет. Создайте его.");
            return;
        }
        foreach (Project project in projects)
        {
            Console.WriteLine(project.Name + ":");
            project.ReviewStatistics();
        }
    }
    /// <summary>
    /// Метод для отображения задачи в виде progress bar.
    /// </summary>
    /// <param name="task"></param>
    public static void DisplayProgressBar(Task task)
    {
        int percent = task.percentOfDone;
        int progress = (int)task.percentOfDone/2;   //Переменная уменьшенного процента задачи для вывода корректного соотношения в progress bar.
        Console.Write("[");
        // Ввыводим progress-bar в определенном цвете, в зависимости от кол-ва выполнения задачи.
        if (percent <= 30)
        {
            Console.ForegroundColor = ConsoleColor.Red;
        }
        else if (percent > 30 && percent <= 80)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Green;
        }
        for (int i = 0; i <= progress; i++)
        {
            Console.Write($"=");
        }
        Console.ResetColor();

        for (int i = (progress + 1); i <= 50; i++)
        {
            Console.Write($" ");
        }
        Console.Write("]");
        Console.Write($" {percent} %\n");
    }
    /// <summary>
    /// Метод для изменения прогресса задачи.
    /// </summary>
    /// <param name="tasks"></param>
    public static void ChangeProgress(List<Task> tasks)
    {
        Console.WriteLine("Введите id задачи:");
        int id;
        while (true)
        {
            string input = Console.ReadLine();
            if (int.TryParse(input, out id))
            {
                break;
            }
            else
            {
                Console.WriteLine("Повторите ввод.");
            }

        }
        Console.WriteLine("Введите процент выполнения задачи:");
        int percent = int.Parse(Console.ReadLine());
        foreach (Task task in tasks)
        {
            if (task.ID == id)
            {
                task.PercentOfDone(percent);
                return;
            }
        }
        Console.WriteLine("Задачи с таким id нет.");
        
    }
}