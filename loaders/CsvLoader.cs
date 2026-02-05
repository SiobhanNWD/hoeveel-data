using System.Globalization;

/*
    The CSV Loader
    - Reads a CSV file
    - Convert rows → strongly-typed objects
    - Return a List<T>
*/
public static class CsvLoader
{
    // Load<T> converts a CSV file into a list of objects using a mapping function
    // Call: CsvLoader.Load<T>(...) -> Decide what T is when calling it (e.g. Load<Province>)
    // string filePath -> Path to the CSV file on disk
    // Func<string[], T> map -> The function that defines how to convert one CSV row into an object
    // Returns: List<T> -> A list of mapped objects (Municipality, Province, PopulationRow, etc.)
    public static List<T> Load<T>(string filePath, Func<string[], T> map)
    {
        var lines = File.ReadAllLines(filePath);    //reads entire CSV file into memory, each line becomes one string in the array, e.g. lines[0] = "code,name,population", lines[1] = "JHB,Johannesburg,450200"
        var results = new List<T>();                //Creates an empty list that will hold all parsed objects

        for (int i = 1; i < lines.Length; i++)      //loops through all rows, starts at 1, not 0 to skip header
        {
            var columns = lines[i].Split(',');      //splits the csv line into columns at the "," and returns a string[] e.g. "JHB,Johannesburg,450200" -> columns[0] = "JHB", columns[1] = "Johannesburg", columns[2] = "450200"
            results.Add(map(columns));              //parses the column array to the mapping function, which converts the row into an object of type T
        }
        return results;     //results are returned as a List<T>
    }
}
