using System;
using System.Collections.Generic;
using System.Xml;
using System.Linq;
using System.Xml.Linq;

public class Program
{
	public enum CategoryType
	{
		Other,
		Easy,
		Intermediate,
		Hard
	}
	
	public class Item
	{
		public string Title { get; set; }
		public string Link { get; set; }
		public DateTime Date { get; set; }
		public CategoryType Category { get; set; }
	}
	
	public static void Main()
	{
		const string url = "https://www.reddit.com/r/dailyprogrammer/.rss";
		
		var xdoc = XDocument.Load(url);
		var items = from item in xdoc.Descendants("item")
			let title = item.Element("title").Value
			select new Item
			{
				Title = title,
				Link = item.Element("link").Value,
				Date = DateTime.Parse(item.Element("pubDate").Value),
				Category = ExtractCategory(title)
			};

		var sorted = items.OrderBy(x => x.Category);
		var array = Split(sorted);
		Print(array);
	}
	
	private static CategoryType ExtractCategory(string title)
	{
		var categories = new Dictionary<string, CategoryType>
		{
			{ "[Easy]", CategoryType.Easy },
			{ "[Intermediate]", CategoryType.Intermediate },
			{ "[Hard]", CategoryType.Hard }
		};
		return categories.FirstOrDefault(x => title.Contains(x.Key)).Value;	
	}
	
	private static void Print(Item[,] array)
	{
		Console.WriteLine("Other | Easy | Intermediate | Hard ");
		Console.WriteLine("------|------|--------------|------");
					
        for (int i = 0; i < array.GetLength(0); i++)
        {
            for (int j = 0; j < array.GetLength(1); j++)
			{
				var item = array[i, j];
				Console.Write("| " + (item != null ?
							  string.Format("[{0}]({1})", item.Title, item.Link)
							  : "   "));
			}
			Console.WriteLine(" |");
        }
	}
	
	private static Item[,] Split(IEnumerable<Item> items)
  {
		var splitedItems = items.GroupBy(j => j.Category).ToArray();
		var array = new Item[splitedItems.Max(x => x.Count()), splitedItems.Length];
		
		for (var i = 0; i < splitedItems.Length; i++)
			for (var j = 0; j < splitedItems.ElementAt(i).Count(); j++)
				array[j, i] = splitedItems.ElementAt(i).ElementAt(j);
		return array;
  } 
}
