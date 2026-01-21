namespace Library_dop;
/// <summary>
/// Класс, содержащий все необходимое для создания объекта проект.
/// </summary>
public class Project
{
    private string name;
    List<Task> tasks;
    /// <summary>
    /// Констрктор с параметрами.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="tasks"></param>
    public Project(string name, List<Task> tasks)
    {
        this.name = name;
        this.tasks = tasks;
    }
    // Свойства для имени проекта.
    public string Name { get => name; set => name = value; }
    // Свойства для списка задач в проекте.
    public List<Task> Tasks { get => tasks; set => tasks = value; }
    /// <summary>
    /// Метод добавления задачи в проект.
    /// </summary>
    /// <param name="task"></param>
    public void AddTask(Task task)
    {
        tasks.Add(task);
    }
    /// <summary>
    /// Метод удаления задачи из проекта.
    /// </summary>
    /// <param name="task"></param>
    public void RemoveTask(Task task)
    {
        tasks.Remove(task);
    }
    /// <summary>
    /// Метод для переименования проекта.
    /// </summary>
    /// <param name="newName"></param>
    public void Rename(string newName)
    {
        this.name = newName;
    }
    /// <summary>
    /// Метод для возврата строки со всеми задачами из проекта.
    /// </summary>
    /// <returns></returns>
    public string ReviewTasks()
    {
        string result = "";
        if (tasks.Count == 0 || tasks == null)
        {
            return $"У проекта {Name} нет задач.";
        }

        foreach (Task task in tasks)
        {
            result += task.ToString() + "\n";
        }
        return result;
    }
    /// <summary>
    /// Метод выводит статистику по проекту.
    /// </summary>
    public void ReviewStatistics()
    {
        Console.WriteLine("Кол-во задач: " + tasks.Count);
        int countOfDone = 0;
        foreach (Task task in tasks)
        {
            if (task.Status == "DONE")
            {
                countOfDone++;
            }
        }
        
        Console.WriteLine($"Кол-во выполненных задач: {countOfDone}");
        Console.WriteLine($"Процент выполнения проекта: {(countOfDone/tasks.Count)*100}%");
    }
    
}