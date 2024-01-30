using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace KursovaPoSAA
{    
    public class Table
    {
        public string Name { get; set; }
        public List<Property> Properties { get; set; }

        public Table(string Name, List<Property> Properties)
        {
            this.Name = Name;
            this.Properties = Properties;
        }
        public Table()
        {

        }
        public static Table? FindTableByName(List<Table> tables, string tableName)
        {
            foreach (Table t in tables)
            {
                if (t.Name == tableName)
                {
                    return t;
                }
            }
            return null;
        }
        public Table Select(string[] data)
        {
            List<Property> properties = new List<Property>();

            if (data[0] == "*")
                return new Table(Name, Properties);

            foreach (var prop in Properties)
                if (data.Contains(prop.Name))
                    properties.Add(prop);


            return new Table(Name, properties);
        }
        public Table Where(object o, string operation)
        {
            if (o is string condition)
            {
                List<Property> props = new();
                foreach (var prop in Properties)
                    props.Add(new Property(prop.Name));

                string[] data = condition.Replace("\"", string.Empty).Split(' ');

                string columnName = data[0];
                string sign = data[1];
                object valueToCompare = Form1.ParseValue(data[2]);

                Property? propertyToLookFor = this[columnName];

                if (propertyToLookFor is null)
                    throw new InvalidOperationException("Couldnt find the column!");

                int? index = GetColumnNameIndex(columnName);

                if (index is null)
                    throw new InvalidOperationException($"Column {columnName} doesnt exist!");


                for (int i = 0; i < Properties[0].Values.Count; i++)
                {
                    var row = GetRow(Properties, i);

                    if (Condition(sign, row[(int)index], valueToCompare))
                        for (int j = 0; j < row.Count; j++) //filling the newList
                            props[j].Values.Add(row[j]);
                }
                if (operation.ToLower() == "delete")
                    Delete(props);

                return new Table(Name, props);
            }
            else
            {
                List<Property> props = (List<Property>)o;
                if (operation.ToLower() == "delete")
                    Delete(props);

                return new Table(Name, props);
            }
        }
        public List<Property> And(List<object> list)
        {
            List<string> conditions = new();
            List<List<Property>> listOfSortedTables = new();

            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] is string stringObj)
                {
                    conditions.Add(stringObj);
                }
                else if (list[i] is List<Property> listObj)
                {
                    listOfSortedTables.Add(listObj);
                }
            }


            if(conditions.Count > 0)
            {
                List<Property> thisProp = new();
                foreach (var prop in Properties)
                    thisProp.Add(new Property(prop.Name));

                for (int i = 0; i < Properties[0].Values.Count; i++)
                {
                    bool conditionsMatch = true;
                    var row = GetRow(Properties, i);                    

                    foreach (string condition in conditions)
                    {
                        string[] data = condition.Replace("\"", string.Empty).Split(' ');

                        string columnName = data[0];
                        string sign = data[1];
                        object valueToCompare = Form1.ParseValue(data[2]);

                        Property? propertyToLookFor = this[columnName];

                        if (propertyToLookFor is null)
                            throw new InvalidOperationException("Couldnt find the column!");

                        int? index = GetColumnNameIndex(columnName);

                        if (index is null)
                            throw new InvalidOperationException($"Column {columnName} doesnt exist!");

                        if (!Condition(sign, row[(int)index], valueToCompare))
                        {
                            conditionsMatch = false;
                            break;
                        }                       
                    }
                    if (conditionsMatch is true)
                        for (int j = 0; j < row.Count; j++) //filling the newList
                            thisProp[j].Values.Add(row[j]);
                }
                listOfSortedTables.Add(thisProp);
            } //if there is a string condition

            //iterating every row, then iterating every condition and if all are true, add the row and repeat for then row

            Table table = new(Name, listOfSortedTables[0]);
            for (int i = 1; i < listOfSortedTables.Count; i++)
                table = table.IntersectWith(listOfSortedTables[i]);


            return table.Properties;

        }
        public List<Property> Or(List<object> list)
        {
            List<string> conditions = new();
            List<List<Property>> listOfSortedTables = new();

            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] is string stringObj)
                {
                    conditions.Add(stringObj);
                }
                else if (list[i] is List<Property> listObj)
                {
                    listOfSortedTables.Add(listObj);
                }
            }

            if (conditions.Count > 0)
            {
                List<Property> thisProp = new();
                foreach (var prop in Properties)
                    thisProp.Add(new Property(prop.Name));

                for (int i = 0; i < Properties[0].Values.Count; i++)
                {
                    var row = GetRow(Properties, i);

                    foreach (string condition in conditions)
                    {
                        string[] data = condition.Replace("\"", string.Empty).Split(' ');

                        string columnName = data[0];
                        string sign = data[1];
                        object valueToCompare = Form1.ParseValue(data[2]);

                        Property? propertyToLookFor = this[columnName];

                        if (propertyToLookFor is null)
                            throw new InvalidOperationException("Couldnt find the column!");

                        int? index = GetColumnNameIndex(columnName);

                        if (index is null)
                            throw new InvalidOperationException($"Column {columnName} doesnt exist!");

                        if (Condition(sign, row[(int)index], valueToCompare))
                        {
                            for (int j = 0; j < row.Count; j++) //filling the newList
                                thisProp[j].Values.Add(row[j]);

                            break;
                        }                      
                            
                    }
                    
                }
                listOfSortedTables.Add(thisProp);
            }
            //getting all rows that have match the condition, then adding that to listOfSortedTables


            Table table = new(Name, listOfSortedTables[0]);
            for (int i = 1; i < listOfSortedTables.Count; i++)
                table = table.Union(listOfSortedTables[i]);
            //Union returns all unique rows from both tables

            return table.Properties;
        }

        public static int CompareObjects(object obj1, object obj2)
        {
            if (obj1 is IComparable && obj2 is IComparable)
            {
                return ((IComparable)obj1).CompareTo(obj2);
            }
            else
            {
                // If objects are not comparable, handle the comparison based on your specific requirements.
                throw new ArgumentException("Objects are not comparable.");
            }
        }
        private bool Condition(string comparator, object comparing, object compared)
        {
            // firstObject is the one we have, the other is from the condition
            int comparedObjects = -1;
            bool result = false;

            switch (comparator)
            {
                case ">":
                    {
                        comparedObjects = comparedObjects = CompareObjects(comparing, compared);
                        if (comparedObjects == 1)
                            result = true;

                        break;
                    }
                case "<":
                    {
                        comparedObjects = CompareObjects(comparing, compared);
                        if (comparedObjects == -1)
                            result = true;

                        break;
                    }
                case ">=":
                    {
                        comparedObjects = CompareObjects(comparing, compared);
                        if (comparedObjects == 1 || comparedObjects == 0)
                            result = true;

                        break;
                    }
                case "<=":
                    {
                        comparedObjects = CompareObjects(comparing, compared);
                        if (comparedObjects == -1 || comparedObjects == 0)
                            result = true;

                        break;
                    }
                case "=":
                    {
                        comparedObjects = CompareObjects(comparing, compared);
                        if (comparedObjects == 0)
                            result = true;

                        break;
                    }
                case "<>":
                    {
                        comparedObjects = CompareObjects(comparing, compared);
                        if (comparedObjects != 0)
                            result = true;

                        break;
                    }

            }
            return result;
        }
        public Property? this[string name]
        {
            get
            {
                foreach (var prop in Properties)
                {
                    if (prop.Name == name)
                        return prop;
                }
                return null;
            }
        }
        private int? GetColumnNameIndex(string columnName)
        {
            for (int i = 0; i < Properties.Count; i++)
            {
                if (Properties[i].Name == columnName)
                    return i;
            }
            return null;
        }
        private List<object> GetRow(List<Property> properties, int rowIndex)
        {
            //Get a specific row from table
            List<object> list = new();

            foreach (var prop in properties)
                list.Add(prop.Values[rowIndex]);

            return list;
        }
        private static bool ListsMatch(List<object> list1, List<object> list2)
        {
            //Check if 2 rows match exactly
            return list1.SequenceEqual(list2);
        }

        public Table Distinct()
        {
            List<Property> properties = new();
            foreach (var prop in Properties)
                properties.Add(new Property(prop.Name));

            Table table = new Table(Name, properties);


            for (int i = 0; i < Properties[0].Values.Count; i++)
            {
                var row = GetRow(Properties, i);
                if (!table.Contains(row))
                    for (int j = 0; j < row.Count; j++)
                        table.Properties[j].Values.Add(row[j]);

            }
            return table;
        }
        public bool Contains(List<object> row)
        {
            for (int i = 0; i < Properties[0].Values.Count; i++)
            {
                var thisRow = GetRow(Properties, i);
                if (ListsMatch(row, thisRow))
                    return true;
            }
            return false;
        }

        public Table Order(string orderType, string orderByParam)
        {
            Property? propertyToOrder = this[orderByParam];
            if (propertyToOrder is null)
                throw new InvalidOperationException($"Column {orderByParam} doesnt exist");

            List<int> properOrder = Order(new List<object>(propertyToOrder.Values));

            if (orderType.ToLower() == "orderbydesc")
                properOrder.Reverse();

            foreach (var prop in Properties)
                ApplyOrder(prop.Values, properOrder);

            return this;
        }
        public static List<int> Order(List<object> list)
        {
            int n = list.Count;
            bool swapped;

            List<int> correctOrder = new();
            for (int i = 0; i < n; i++)
                correctOrder.Add(i);

            do
            {
                swapped = false;

                for (int i = 1; i < n; i++)
                {
                    if (CompareObjects(list[i - 1], list[i]) == 1)
                    {
                        object temp = list[i - 1];
                        list[i - 1] = list[i];
                        list[i] = temp;

                        int index = correctOrder[i - 1];
                        correctOrder[i - 1] = correctOrder[i];
                        correctOrder[i] = index;

                        swapped = true;
                    }
                }
                n--;
            } while (swapped);

            return correctOrder;
        }
        public void ApplyOrder(List<object> list, List<int> order)
        {
            List<object> temp = new List<object>(list.Count);

            for (int i = 0; i < order.Count; i++)
            {
                int newIndex = order[i];
                temp.Add(list[newIndex]);
            }

            // Copy the values back to the original list
            for (int i = 0; i < temp.Count; i++)
            {
                list[i] = temp[i];
            }
        }
        public Property? GetProperty(string propertyName)
        {
            foreach (Property property in Properties)
                if (property.Name == propertyName)
                    return property;

            return null;
        }      
        
        private Table Union(List<Property> list)
        {
            //get all unique elements from both tables          

            for (int i = 0; i < list[0].Values.Count; i++)
            {
                var row = GetRow(list, i);
                if (!this.Contains(row))
                {
                    for (int j = 0; j < row.Count; j++)
                        Properties[j].Values.Add(row[j]);
                }

            }

            return new Table(Name, Properties);
        }
        private Table IntersectWith(List<Property> list)
        {
            //get all rows that are present in both tables
            List<Property> final = new();
            foreach (var prop in Properties)
                final.Add(new Property(prop.Name));

            for (int i = 0; i < list[0].Values.Count; i++)
            {
                var row = GetRow(list, i);
                if (this.Contains(row))
                {
                    for (int j = 0; j < row.Count; j++)
                        final[j].Values.Add(row[j]);
                }

            }

            return new Table(Name, final);
        }
        private void Delete(List<Property> list)
        {
            Table _ = new Table(Name, list);

            for (int i = Properties[0].Values.Count - 1; i >= 0; i--)
            {
                var row = GetRow(Properties, i);

                if (_.Contains(row))
                {
                    for (int j = 0; j < Properties.Count; j++)
                    {
                        Properties[j].Values.RemoveAt(i);
                    }
                }
            }
        }
    }
}
