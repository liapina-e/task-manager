using System.Collections.Generic;
namespace Library_dop;
/// <summary>
/// Класс, сделанный для сравнения объектов типа Task по определенным параметрам.
/// </summary>
public class TaskComparer:IComparer<Task> 
{
    private readonly string _fieldToCompare;
    /// <summary>
    /// Конструктор с параметрами.
    /// </summary>
    /// <param name="fieldToCompare"></param>
    public TaskComparer(string fieldToCompare)
    {
        _fieldToCompare = fieldToCompare;
    }
    /// <summary>
    /// Метод сравнивает объекты по выбранному пользователем полю.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public int Compare(Task x, Task y)
    {
        switch (_fieldToCompare.ToLower())
        {
            case "id":
                return x.ID.CompareTo(y.ID);
            case "status":
                return string.Compare(x.Status, y.Status, StringComparison.Ordinal);
            case "description":
                return string.Compare(x.Description, y.Description, StringComparison.Ordinal);
            case "dateOfCreation": 
                return string.Compare(x.DateOfCreation.ToString(), y.DateOfCreation.ToString(), StringComparison.Ordinal);
            case "dateOfEdit":
                return string.Compare(x.DateOfEdit.ToString(), y.DateOfEdit.ToString(), StringComparison.Ordinal);
            default:
                throw new ArgumentException("Неверное поле для сравнения");
        }
    }
}