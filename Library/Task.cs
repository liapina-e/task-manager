using System.Globalization;
using System.Net.Http.Headers;

namespace Library;
/// <summary>
/// Класс, представляющий задачу со всеми необходимыми данными.
/// </summary>
public class Task
{
    private int _id;
    private string _status;
    private string _priority;
    string _description;
    List<Task> _dependencies;
    private DateTime _deadline;
    /// <summary>
    /// Конструктор с параметрами.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="status"></param>
    /// <param name="priority"></param>
    /// <param name="description"></param>
    /// <param name="deadline"></param>
    public Task(int id, string status, string priority, string description, string deadline)
    {
        ID = id;
        Status = status;
        Priority = priority;
        Description = description;
        DateOfCreation = DateTime.Now;
        DateOfEdit = DateTime.Now;
        string format = "dd MMMM yyyy HH:mm:ss";
        CultureInfo culture = new CultureInfo("ru-RU");
        Deadline = DateTime.ParseExact(deadline, format, culture);
    }
    /// <summary>
    /// Конструктор без параметров.
    /// </summary>
    public Task() {}
    // Свойства для id задачи.
    public int ID { get; set; }
    
    // Свойства для статуса задачи.
    public string Status
    {
        get => _status;
        set
        {
            _status = value;
            DateOfEdit = DateTime.Now;
        }
    }
    
    // Свойства для приоритета задачи.
    public string Priority
    {
        get => _priority;
        set
        {
            _priority = value;
            DateOfEdit = DateTime.Now;
        }
    }
    
    // Свойства для описания задачи.
    public string Description
    {
        get => _description;
        set
        {
            _description = value;
            DateOfEdit = DateTime.Now;
        }
    }
    
    // Свойства для даты создания задачи.
    public DateTime DateOfCreation { get; }
    
    // Свойства для даты изменения задачи.
    public DateTime DateOfEdit { get; set; }
    
    // Свойства для дедлайна задачи.
    public DateTime Deadline
    {
        get => _deadline;
        set
        {
            _deadline = value;
            DateOfEdit = DateTime.Now;
        }
        
    }
    /// <summary>
    /// В методе добавляем зависимую задачу.
    /// </summary>
    /// <param name="task"></param>
    public void AddDependency(Task task)
    {
        if (Status != "DONE" && _dependencies.Contains(task) == false && task.ID != ID)
        {
            if (WillCycle(task) == false)
            {
                _dependencies.Add(task);
                DateOfEdit = DateTime.Now;
            }
            else
            {
                Console.WriteLine("Добавление задачи создаст цикл.");
            }
            
        }
        else
        {
            Console.WriteLine("Задачу нельзя добавить в зависимость.");
        }
    }
    /// <summary>
    /// В методе удаляем зависимую задачу.
    /// </summary>
    /// <param name="task"></param>
    public void DeleteDependency(Task task)
    {
        if (_dependencies.Contains(task))
        {
            _dependencies.Remove(task);
            DateOfEdit = DateTime.Now;
        }
        else
        {
            Console.WriteLine("Задача не зависит от данной.");
        }
    }
    /// <summary>
    /// В методе проверяем можно ли завершить задачу.
    /// Метод возвращает правду/ложь.
    /// </summary>
    /// <returns></returns>
    public bool CanBeDone()
    {
        // Если не все зависимые от нее задачи выполнены, то не можем изменить ей статус на DONE.
        if (_dependencies != null && _dependencies.Count > 0)
        {
            foreach (Task dependency in _dependencies)
            {
                if (dependency.Status != "DONE")
                {
                    return false;
                }
            }
        }
        return true;
    }
    /// <summary>
    /// В методе проверяем не будет ли цикла.
    /// </summary>
    /// <param name="task"></param>
    /// <param name="visited"></param>
    /// <param name="now"></param>
    /// <returns></returns>
    public bool WillCycle(Task task, List<Task> visited = null, List<Task> now = null)
    {
        visited = visited ?? new List<Task>();  // Узлы, которые уже были посещены.
        now = now ?? new List<Task>();  // Узлы в текущем пути итерации.
        // Если найден цикл.
        if (now.Contains(task)) 
        {
            return true;
        }
        //Если узел был посещен, но цикл не найден.
        else if (visited.Contains(task))
        {
            return false;
        }
        
        visited.Add(task);
        now.Add(task);
        // В строках 179 - 184 использовалась GPT, т.к. рекурсия еще не была изучена на лекциях.
        // Используем рекурсию.
        foreach (Task dependency in task._dependencies)
        {
            if (WillCycle(dependency, visited, now))
            {
                return true;
            }
        }
        
        // Убираем задачу из текущего пути.
        now.Remove(task);
        return false;
        
    }
    
    /// <summary>
    /// Переопределенный метод ToString.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return $"ID: {ID}\nСтатус: {Status}\nПриоритет: {Priority}\nОписание: {Description}\nДедлайн: {Deadline.ToString()}";
    }
    
}