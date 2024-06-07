using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

class ExhibitionRecord
{
    public string ExhibitionName { get; set; }
    public string ArtistSurname { get; set; }
    public DateTime VisitDate { get; set; }
    public int VisitorsCount { get; set; }
    public string Comment { get; set; }
}

class ExhibitionManager
{
    private const string FilePath = "exhibition_database.txt";

    public List<ExhibitionRecord> ReadRecords()
    {
        List<ExhibitionRecord> records = new List<ExhibitionRecord>();
        try
        {
            string[] lines = File.ReadAllLines(FilePath);
            foreach (string line in lines)
            {
                string[] parts = line.Split(',');
                records.Add(new ExhibitionRecord
                {
                    ExhibitionName = parts[0],
                    ArtistSurname = parts[1],
                    VisitDate = DateTime.Parse(parts[2]),
                    VisitorsCount = int.Parse(parts[3]),
                    Comment = parts[4]
                });
            }
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine("Файл бази даних не знайдено.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Помилка при читанні бази даних: {ex.Message}");
        }
        return records;
    }

    public void WriteRecords(List<ExhibitionRecord> records)
    {
        try
        {
            using (StreamWriter writer = new StreamWriter(FilePath))
            {
                foreach (var record in records)
                {
                    writer.WriteLine($"{record.ExhibitionName},{record.ArtistSurname},{record.VisitDate},{record.VisitorsCount},{record.Comment}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Помилка при записі в базу даних: {ex.Message}");
        }
    }

    public void AddRecord(List<ExhibitionRecord> records, ExhibitionRecord newRecord)
    {
        records.Add(newRecord);
        WriteRecords(records);
    }

    public void EditRecord(List<ExhibitionRecord> records, int index, ExhibitionRecord editedRecord)
    {
        records[index] = editedRecord;
        WriteRecords(records);
    }

    public void DeleteRecord(List<ExhibitionRecord> records, int index)
    {
        records.RemoveAt(index);
        WriteRecords(records);
    }

    public void DisplayRecords(List<ExhibitionRecord> records)
    {
        foreach (var record in records)
        {
            Console.WriteLine($"Назва виставки: {record.ExhibitionName}");
            Console.WriteLine($"Прізвище художника: {record.ArtistSurname}");
            Console.WriteLine($"Дата візиту: {record.VisitDate}");
            Console.WriteLine($"Кількість відвідувачів: {record.VisitorsCount}");
            Console.WriteLine($"Коментар: {record.Comment}");
            Console.WriteLine();
        }
    }

    public int CalculateTotalVisitors(List<ExhibitionRecord> records)
    {
        return records.Sum(r => r.VisitorsCount);
    }

    public DateTime GetDayWithLeastVisitors(List<ExhibitionRecord> records)
    {
        var groupedByDate = records.GroupBy(r => r.VisitDate.Date);
        var minVisitorsDay = groupedByDate.OrderBy(g => g.Sum(r => r.VisitorsCount)).FirstOrDefault();
        return minVisitorsDay.Key;
    }

    public List<string> GetCommentsWithKeyword(List<ExhibitionRecord> records, string keyword)
    {
        return records.Where(r => r.Comment.Contains(keyword)).Select(r => r.Comment).ToList();
    }
}

class Program
{
    static void Main(string[] args)
    {
        ExhibitionManager exhibitionManager = new ExhibitionManager();
        List<ExhibitionRecord> records = exhibitionManager.ReadRecords();

        char choice;
        do
        {
            DisplayMenu();
            choice = Console.ReadKey().KeyChar;
            Console.WriteLine();
            ProcessChoice(choice, exhibitionManager, records);
        } while (char.ToLower(choice) != 'q');
    }

    static void DisplayMenu()
    {
        Console.WriteLine("Меню:");
        Console.WriteLine("1. Додати запис");
        Console.WriteLine("2. Редагувати запис");
        Console.WriteLine("3. Видалити запис");
        Console.WriteLine("4. Показати записи");
        Console.WriteLine("5. Підрахувати загальну кількість відвідувачів");
        Console.WriteLine("6. Знайти день з найменшою кількістю відвідувачів");
        Console.WriteLine("7. Пошук коментарів за ключовим словом");
        Console.WriteLine("Q. Вихід");
        Console.WriteLine("Введіть варіант:");
    }

    static void ProcessChoice(char choice, ExhibitionManager exhibitionManager, List<ExhibitionRecord> records)
    {
        switch (choice)
        {
            case '1':
                AddRecord(exhibitionManager, records);
                break;
            case '2':
                EditRecord(exhibitionManager, records);
                break;
            case '3':
                DeleteRecord(exhibitionManager, records);
                break;
            case '4':
                DisplayRecords(exhibitionManager, records);
                break;
            case '5':
                CalculateTotalVisitors(exhibitionManager, records);
                break;
            case '6':
                FindDayWithLeastVisitors(exhibitionManager, records);
                break;
            case '7':
                SearchCommentsByKeyword(exhibitionManager, records);
                break;
            case 'q':
                Console.WriteLine("Дякую за використання програми!");
                break;
            default:
                Console.WriteLine("Неправильний вибір!");
                break;
        }
    }

    static void AddRecord(ExhibitionManager exhibitionManager, List<ExhibitionRecord> records)
    {
        ExhibitionRecord newRecord = new ExhibitionRecord();
        Console.WriteLine("Введіть назву виставки:");
        newRecord.ExhibitionName = Console.ReadLine();
        Console.WriteLine("Введіть прізвище художника:");
        newRecord.ArtistSurname = Console.ReadLine();
        Console.WriteLine("Введіть дату візиту (у форматі yyyy-MM-dd):");
        if (DateTime.TryParse(Console.ReadLine(), out DateTime visitDate))
        {
            newRecord.VisitDate = visitDate;
        }
        else
        {
            Console.WriteLine("Неправильний формат дати.");
            return;
        }
        Console.WriteLine("Введіть кількість відвідувачів:");
        if (int.TryParse(Console.ReadLine(), out int visitorsCount))
        {
            newRecord.VisitorsCount = visitorsCount;
        }
        else
        {
            Console.WriteLine("Неправильний формат кількості відвідувачів.");
            return;
        }
        Console.WriteLine("Введіть коментар:");
        newRecord.Comment = Console.ReadLine();

        exhibitionManager.AddRecord(records, newRecord);
        Console.WriteLine("Запис успішно додано!");
    }

    static void EditRecord(ExhibitionManager exhibitionManager, List<ExhibitionRecord> records)
    {
        Console.WriteLine("Введіть індекс запису, який бажаєте відредагувати:");
        if (int.TryParse(Console.ReadLine(), out int index) && index >= 0 && index < records.Count)
        {
            ExhibitionRecord editedRecord = new ExhibitionRecord();
            Console.WriteLine("Введіть назву виставки:");
            editedRecord.ExhibitionName = Console.ReadLine();
            Console.WriteLine("Введіть прізвище художника:");
            editedRecord.ArtistSurname = Console.ReadLine();
            Console.WriteLine("Введіть дату візиту (у форматі yyyy-MM-dd):");
            if (DateTime.TryParse(Console.ReadLine(), out DateTime visitDate))
            {
                editedRecord.VisitDate = visitDate;
            }
            else
            {
                Console.WriteLine("Неправильний формат дати.");
                return;
            }
            Console.WriteLine("Введіть кількість відвідувачів:");
            if (int.TryParse(Console.ReadLine(), out int visitorsCount))
            {
                editedRecord.VisitorsCount = visitorsCount;
            }
            else
            {
                Console.WriteLine("Неправильний формат кількості відвідувачів.");
                return;
            }
            Console.WriteLine("Введіть коментар:");
            editedRecord.Comment = Console.ReadLine();

            exhibitionManager.EditRecord(records, index, editedRecord);
            Console.WriteLine("Запис успішно відредаговано!");
        }
        else
        {
            Console.WriteLine("Неправильний індекс запису.");
        }
    }

    static void DeleteRecord(ExhibitionManager exhibitionManager, List<ExhibitionRecord> records)
    {
        Console.WriteLine("Введіть індекс запису, який бажаєте видалити:");
        if (int.TryParse(Console.ReadLine(), out int index) && index >= 0 && index < records.Count)
        {
            exhibitionManager.DeleteRecord(records, index);
            Console.WriteLine("Запис успішно видалено!");
        }
        else
        {
            Console.WriteLine("Неправильний індекс запису.");
        }
    }

    static void DisplayRecords(ExhibitionManager exhibitionManager, List<ExhibitionRecord> records)
    {
        exhibitionManager.DisplayRecords(records);
    }

    static void CalculateTotalVisitors(ExhibitionManager exhibitionManager, List<ExhibitionRecord> records)
    {
        int totalVisitors = exhibitionManager.CalculateTotalVisitors(records);
        Console.WriteLine($"Загальна кількість відвідувачів: {totalVisitors}");
    }

    static void FindDayWithLeastVisitors(ExhibitionManager exhibitionManager, List<ExhibitionRecord> records)
    {
        DateTime dayWithLeastVisitors = exhibitionManager.GetDayWithLeastVisitors(records);
        Console.WriteLine($"День з найменшою кількістю відвідувачів: {dayWithLeastVisitors.ToShortDateString()}");
    }

    static void SearchCommentsByKeyword(ExhibitionManager exhibitionManager, List<ExhibitionRecord> records)
    {
        Console.WriteLine("Введіть ключове слово для пошуку:");
        string keyword = Console.ReadLine();
        List<string> comments = exhibitionManager.GetCommentsWithKeyword(records, keyword);
        if (comments.Any())
        {
            Console.WriteLine($"Знайдено коментарі за ключовим словом '{keyword}':");
            foreach (var comment in comments)
            {
                Console.WriteLine(comment);
            }
        }
        else
        {
            Console.WriteLine($"Коментарі за ключовим словом '{keyword}' не знайдено.");
        }
    }
}
