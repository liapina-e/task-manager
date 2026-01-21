using System.ComponentModel;
using Telegram.Bot.Polling;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Extensions;
using Telegram.Bot.Types;
using Telegram.Bot.Exceptions;
using Telegram;
namespace Project_4_dop;
using Library_dop;
/// <summary>
/// В классе представлена работа всех методов для решения всей основной задачи.
/// </summary>
class Program
{
    /// <summary>
    /// В методе представлено меню для пользователя.
    /// </summary>
    public static void Menu()
    {
        Console.WriteLine("Введите номер пункта меню для запуска действия:");
        Console.WriteLine("1. Ввести данные из файла\n2. Просмотр всех задач\n3. Добавление новой задачи\n" +
                          "4. Изменение статуса задачи\n5. Удаление задачи\n6. Вывести данные в консоль\n7. Добавить/удалить зависимую задачу\n" +
                          "8. Работа с таблицей\n9. Вывести данные в файл\n10. Работа с проектом\n11. Отображение задач по проектам\n12. Отображение статистики по проектам\n13. Отслеживание прогресса задач\n14. Выход");
    }
    /// <summary>
    /// В методе Main обрабатываем все данные, введенные пользователем.
    /// Запускаем все методы, представленные в других классах.
    /// </summary>
    /// <param name="args"></param>
    static async System.Threading.Tasks.Task Main(string[] args)
    {
        ConsoleKeyInfo keyToExit = default;
        // Переменная flagOfDataAvailability отвечает за выполнение первого пункта меню. Т.е. чтоб входные данные были.
        // Без них дальнейшая программа работать не будет.
        bool flagOfDataAvailability = false;
        List<Task> tasks = new List<Task>();
        List<Project> projects = new List<Project>();
        // Подписываем метод на событие из встроенного класса для работы в фоновом режиме.
        BackgroundWorker worker = new BackgroundWorker();
        worker.DoWork += Methods.RemindAboutDeadline;
        // Включаем тг бота.
        TelegramBot bot = new TelegramBot();
        Console.WriteLine("Перед началом работы введите в бота \"ReminderAboutTasks\" start.");
        bot.StartAsync();
        // Программа работает, пока пользователь сам не захочет выйти, нажав клавишу Escape.
        do
        {
            Menu();
            string? input_ = Console.ReadLine();
            if (!int.TryParse(input_, out  int numb) || numb < 1 || numb > 14)
            {
                Console.WriteLine("Такого пункта меню не существует. Попробуйте еще раз.");
            }
            
            if (numb == 1)
            {
                Console.WriteLine("Выберите, из какого файла хотите извлечь данные:\n" +
                                  "1. CSV\n2. JSON");
                string? input = Console.ReadLine();
                if (!int.TryParse(input, out  int num) || num < 1 || num > 2)
                {
                    Console.WriteLine("Такого пункта не существует. Попробуйте еще раз.");
                    break;
                }
                Console.WriteLine("Введите путь к файлу:");
                string path = Console.ReadLine();

                if (num == 1)
                {
                    tasks = await Methods.ParseFromCSV(path);
                    flagOfDataAvailability = true;
                    Console.WriteLine("Нажми любую клавишу для вывода меню");
                }
                else 
                {
                    tasks = await Methods.ParseFromJSON(path);
                    flagOfDataAvailability = true;
                    Console.WriteLine("Нажми любую клавишу для вывода меню");
                }
            }
            // Ниже представлены все возможные действия, которые может совершить пользователь.
            else if (numb == 14)
            {
                // Перед выходом сохраняем данные в файл,выбранный пользователем.
                Console.WriteLine("Выберите, в какой файл вы хотите сохранить данные:\n" +
                                  "1. CSV\n2. JSON\n");
                string? input = Console.ReadLine();
                if (!int.TryParse(input, out  int number) || number < 1 || number > 2)
                {
                    Console.WriteLine("Такого пункта не существует. Попробуйте еще раз.");
                    break;
                }
                Console.WriteLine("Введите путь к файлу:");
                string path = Console.ReadLine();

                if (number == 1)
                {
                    await Methods.WriteToCsv(path, tasks);
                }
                else
                {
                    await Methods.WriteToJSON(path, tasks);
                }
                Console.WriteLine("Для выхода нажмите Escape....");
                
            }
            // Если выполнены первый пункт меню, то можно выбирать любой из следующих.

            if (flagOfDataAvailability)
            {
                if (numb == 2)
                {
                    Methods.ViewingTasks(tasks);
                    Console.WriteLine("Нажми любую клавишу для вывода меню");
                }
                else if (numb == 3)
                {
                    Task task = Methods.AddTask(tasks);
                    tasks.Add(task); ;
                    Console.WriteLine("Нажми любую клавишу для вывода меню");
                }
                else if (numb == 4)
                {
                    Console.WriteLine("Введите ID нужной задачи:");
                    int id = int.Parse(Console.ReadLine());
                    Methods.EditStatus(tasks, id);
                    Console.WriteLine("Нажми любую клавишу для вывода меню");
                }
                else if (numb == 5)
                {   
                    Console.WriteLine("Введите ID нужной задачи:");
                    int id = int.Parse(Console.ReadLine());
                    Methods.RemoveTask(tasks, id);
                    Console.WriteLine("Нажми любую клавишу для вывода меню");
                }
                else if (numb == 6)
                {
                    for (int i = 0; i < tasks.Count-1; i++)
                    {
                        Console.WriteLine(tasks[i].ToString());
                        Console.WriteLine("-------------------------");
                    }
                    Console.WriteLine(tasks[tasks.Count-1].ToString());
                    Console.WriteLine("Нажми любую клавишу для вывода меню");

                }
                else if (numb == 7)
                {
                    Console.WriteLine("Выберите:\n1.Добавить зависимость \n2.Удалить зависимость");
                    string input = Console.ReadLine();
                    int id1, id2;
                    while (true)
                    {
                        Console.WriteLine("Введите id задачи, к которой хотите внести/удалить зависимую:");
                        string _id1 = Console.ReadLine();
                        Console.WriteLine("Введите id задачи,которую хотите сделать/убрать зависимой:");
                        string _id2 = Console.ReadLine();
                        if (int.TryParse(_id1, out id1) && int.TryParse(_id2, out id2))
                        {
                            break;
                        }
                        Console.WriteLine("Повторите ввод.");
                    }

                    if (input == "1")
                    {
                        Methods.AddDependenceTask(tasks, id1, id2);
                    }
                    else if (input == "2")
                    {
                        Methods.DeleteDependenceTask(tasks, id1, id2);
                    }
                    else
                    {
                        Console.WriteLine("Такого пункта меню нет.");
                    }
                    Console.WriteLine("Нажми любую клавишу для вывода меню");
                }
                else if (numb == 8)
                {
                    Console.WriteLine("Выберите, что вы хотите сделать:\n" +
                                      "1. Отобразить задачи в виде таблицы\n" +
                                      "2. Интерактивная фильтрация в таблице и сортировка в таблице");
                    string input = Console.ReadLine();
                    if (input == "1")
                    {
                        Methods.OutputToTable(tasks);
                    }
                    else if (input == "2")
                    {
                        Methods.FilteringAndSorting(tasks);
                    }
                    else
                    {
                        Console.WriteLine("Такого пункта меню нет.");
                    }
                    Console.WriteLine("Нажми любую клавишу для вывода меню");

                }
                
                else if (numb == 9)
                {
                    Console.WriteLine("Выберите, в какой файл вы хотите сохранить данные:\n" +
                                      "1. CSV\n2. JSON\n");
                    int number;
                    while (true)
                    {
                        string? input = Console.ReadLine();
                        if (!int.TryParse(input, out number) || number < 1 || number > 2)
                        {
                            Console.WriteLine("Такого пункта не существует. Попробуйте еще раз.");
                        }
                        else
                        {
                            break;
                        }
                    }
                    
                    Console.WriteLine("Введите путь к файлу:");
                    string path = Console.ReadLine();

                    if (number == 1)
                    {
                        await Methods.WriteToCsv(path, tasks);
                    }
                    else
                    {
                        await Methods.WriteToJSON(path, tasks);
                    }
                    Console.WriteLine("Нажми любую клавишу для вывода меню");
                }
                else if (numb == 10)
                {
                    Console.WriteLine("Выберите, что вы хотите сделать:\n" +
                                      "1. Создать проект\n2. Удалить проект\n3. Переименовать проект\n4. Привязать задачу к проекту");
                    int number;
                    while(true)
                    {
                        string? input = Console.ReadLine();
                        if (!int.TryParse(input, out number) || number < 1 || number > 4)
                        {
                            Console.WriteLine("Такого пункта не существует. Попробуйте еще раз.");
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (number == 1)
                    {
                        Project project = Methods.CreateProject(tasks);
                        projects.Add(project);
                    }
                    else if (number == 2)
                    {
                        Methods.DeleteProject(projects);
                    }
                    else if(number == 3)
                    {
                        Methods.RenameProject(projects);
                    }
                    else
                    {
                        Methods.AddTaskToProj(tasks, projects);
                    }
                    Console.WriteLine("Нажми любую клавишу для вывода меню");
                }
                else if (numb == 11)
                {
                    Methods.DisplayTasksByProj(projects);
                    Console.WriteLine("Нажми любую клавишу для вывода меню");
                }
                else if (numb == 12)
                {
                    Methods.RewievStatisticsByProj(projects);
                    Console.WriteLine("Нажми любую клавишу для вывода меню");
                }
                else if (numb == 13)
                {
                    Console.WriteLine("Выберите, что вы хотите сделать:\n" +
                                      "1. Изменить процент выполнения задачи\n2. Отобразить прогресс");
                    int number;
                    while(true)
                    {
                        string? input = Console.ReadLine();
                        if (!int.TryParse(input, out number) || number < 1 || number > 2)
                        {
                            Console.WriteLine("Такого пункта не существует. Попробуйте еще раз.");
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (number == 1)
                    {
                        Methods.ChangeProgress(tasks);
                    }
                    else
                    {
                        Console.WriteLine("Введите id задачи:");
                        int id = int.Parse(Console.ReadLine());
                        foreach (Task task in tasks)
                        {
                            if (task.ID == id)
                            {
                                Methods.DisplayProgressBar(task);
                                break;
                            }
                        }
                    }
                    Console.WriteLine("Нажми любую клавишу для вывода меню");
                }
            }
            else
            {
                Console.WriteLine("Невозможно выполнить действие, т.к. данных нет.");
            }
            // Отправка напоминания через тг бот.
            await bot.SendRemindersAsync(tasks);
            // Работа метода в фоновом режиме.
            worker.RunWorkerAsync(tasks);

            keyToExit = Console.ReadKey();
        } while (keyToExit.Key != ConsoleKey.Escape);
    }
}