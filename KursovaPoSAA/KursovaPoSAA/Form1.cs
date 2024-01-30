using System.Text;
using System.Text.Json;
using System.Windows.Forms;
using System.Xml.Linq;

namespace KursovaPoSAA
{
    public partial class Form1 : Form
    {
        private List<Table> tables = new List<Table>();
        private string storagePath;
        public Form1()
        {
            InitializeComponent();
        }

        private void RunCommandBtn_Click(object sender, EventArgs e)
        {
            string line = CommandTextBox.Text
                .Replace("\r\n", string.Empty);

            string command = line
                                .Substring(0, line.IndexOf(" "));

            line = line
                        .Replace(command, string.Empty)
                        .TrimStart();

            switch (command.ToLower())
            {
                case "createtable":
                    {
                        string tableName = line.Substring(0, line.IndexOf('(')); //Sample

                        FindIfTableAlreadyExists(tableName);

                        List<Property> properties = new List<Property>();

                        line = line
                                .Replace(tableName, string.Empty)
                                .TrimStart('(')
                                .TrimEnd(')'); //Id:int, Name:string, BirthDate:date default “01.01.2022”

                        string[] tableData = line.Split(", ");

                        foreach (string s in tableData)
                        {
                            Property property = Property.CreateProperty(s);
                            properties.Add(property);
                        }

                        ListViewItem item = new ListViewItem();
                        item.Text = tableName;
                        listView1.Items.Add(item);

                        tables.Add(new Table(tableName, properties));

                        break;
                    }
                case "droptable":
                    {
                        foreach (Table table in tables)
                        {
                            if (table.Name == line)
                            {
                                tables.Remove(table);
                                listView1.Items.Remove(listView1.SelectedItems[0]);
                                break;
                            }
                        }
                        break;
                    }
                case "select":
                    {
                        bool distinct = false;
                        if (line.ToLower().Contains("distinct"))
                        {
                            line = line
                                        .Replace("distinct", string.Empty, StringComparison.OrdinalIgnoreCase)
                                        .TrimStart();

                            distinct = true;
                        }
                        CQueue<string> subQueries = SplitQueries(line);

                        string selectData = subQueries.Dequeue();
                        //first subQuery would always be select

                        string[] properties = selectData
                        .Substring(0, selectData.ToLower().IndexOf("from"))
                        .Trim()
                        .Split(new string[] { ", ", " ", "," }, StringSplitOptions.RemoveEmptyEntries);
                        //date for which rows/columns we want. * -> all or specific ones

                        string tableName = selectData
                                                .Substring(selectData.ToLower().IndexOf("from") + "from".Length)
                                                .Trim();

                        Table? t = Table.FindTableByName(tables, tableName);

                        if (t is null)
                            throw new InvalidOperationException($"Couldn't find table {tableName}!");

                        while (subQueries.Any())
                        {
                            string subQuery = subQueries.Dequeue();

                            if (subQuery.ToLower().Contains("where"))
                            {
                                Tree tree = new Tree(t);
                                CQueue<string> queue = FillQueue(subQuery);
                                tree = tree.ConstructTree(queue);
                                t = tree.ProcessTree("select");

                            }
                            else if (subQuery.ToLower().Contains("order"))
                            {
                                string[] data = subQuery.Split(' ');
                                string orderType = data[0];
                                string orderByParam = data[1];

                                t = t.Order(orderType, orderByParam);

                            }

                        }
                        t = t.Select(properties);

                        if (distinct is true)
                            t = t.Distinct();

                        dataGridView1.Rows.Clear();
                        dataGridView1.Columns.Clear();
                        dataGridView1.Refresh();
                        DisplayDataInGridView(t);

                        break;
                    }
                case "insert":
                    {
                        string tableParams = line.Substring(0, line.ToLower().IndexOf("values")); // INTO Sample
                        string insertData = line.Replace(tableParams, string.Empty).TrimStart(); // VALUES (1, \"Иван\", \"10.10.1010\")
                                                                                                 // VALUES (1, \"Иван\", \"10.10.1010\"),(2, \"Goro\", \"10.12.2020\")

                        tableParams = tableParams
                            .Replace(tableParams.Substring(0, tableParams.IndexOf(' ')), string.Empty)
                            .TrimStart()
                            .TrimEnd(); // Sample \\ Sample (Id, Name)

                        string tableName = string.Empty;
                        string[]? insertParams = null; // if empty => no default data expected

                        insertData = insertData
                            .Replace(insertData.Substring(0, insertData.IndexOf(' ')), string.Empty);

                        GetTableNameAndInsertParams(tableParams, out tableName, out insertParams);
                        CQueue<CQueue<object>> queue = TableData(insertData);

                        Table? table = Table.FindTableByName(tables, tableName);

                        if (table is null)
                            throw new InvalidOperationException("Couldn't find the table!");

                        while (queue.Any())
                        {
                            CQueue<object> currentRowData = queue.Dequeue();

                            while (currentRowData.Any())
                            {
                                foreach (Property property in table.Properties)
                                {
                                    if (insertParams is null || insertParams.Contains(property.Name))
                                        property.InsertData(currentRowData.Dequeue());
                                    else
                                        // Skip properties not in insertParams
                                        property.InsertData(property.DefaultValue
                                            ??
                                            throw new InvalidOperationException("Default Value is null!"));
                                }

                            }

                        }

                        break;
                    }
                case "delete":
                    {
                        CQueue<string> subQueries = SplitQueries(line);
                        //From Table
                        //Where Id > 5

                        string deleteData = subQueries.Dequeue();

                        string[] properties = deleteData
                        .Substring(0, deleteData.ToLower().IndexOf("from"))
                        .Trim()
                        .Split(new string[] { ", ", " ", "," }, StringSplitOptions.RemoveEmptyEntries);
                        //date for which rows/columns we want. * -> all or specific ones

                        string tableName = deleteData
                                                .Substring(deleteData.ToLower().IndexOf("from") + "from".Length)
                                                .Trim();

                        Table? t = Table.FindTableByName(tables, tableName);

                        if (t is null)
                            throw new InvalidOperationException($"Couldn't find table {tableName}!");

                        if (properties.Length > 0)
                        {
                            if (properties[0].ToLower().Contains("all"))
                                foreach (var prop in t.Properties)
                                    prop.Values.Clear();
                        }
                        else
                        {
                            while (subQueries.Any())
                            {
                                string subQuery = subQueries.Dequeue();

                                if (subQuery.ToLower().Contains("where"))
                                {
                                    Tree tree = new Tree(t);
                                    CQueue<string> queue = FillQueue(subQuery);
                                    tree = tree.ConstructTree(queue);
                                    t = tree.ProcessTree("delete");

                                }
                                else if (subQuery.ToLower().Contains("order"))
                                {
                                    string[] data = subQuery.Split(' ');
                                    string orderType = data[0];
                                    string orderByParam = data[1];

                                    t = t.Order(orderType, orderByParam);

                                }
                            }
                        }
                        dataGridView1.Rows.Clear();
                        dataGridView1.Columns.Clear();
                        dataGridView1.Refresh();
                        DisplayDataInGridView(t);
                        break;
                    }

            }


            CommandTextBox.Text = string.Empty;
        }

        private static CQueue<string> FillQueue(string line)
        {
            string[] input = line.Split(' ');
            string[] keywords = { "WHERE", "AND", "OR", "NOT", "Where", "And", "Or", "Not" };

            CQueue<string> queue = new CQueue<string>();
            string text = string.Empty;

            foreach (string s in input)
            {
                if (keywords.Contains(s))
                {
                    if (text.Length != 0)
                        queue.Enqueue(text.Trim());

                    queue.Enqueue(s);
                    text = string.Empty;

                }
                else
                    text += s + " ";
            }
            if (text.Length != 0)
                queue.Enqueue(text.Trim());

            return queue;

        }

        private static CQueue<object> RowData(string s)
        {
            CQueue<object> queue = new();

            string[] rowData = s
            .TrimStart()
            .TrimStart('(')
            .TrimEnd(')')
            .Replace(" ", string.Empty)
            .Replace("\"", string.Empty)
            .Replace("\"", string.Empty)
            .Split(',');

            foreach (string str in rowData)
                queue.Enqueue(ParseValue(str));

            return queue;
        }

        private static CQueue<CQueue<object>> TableData(string insertData)
        {
            CQueue<CQueue<object>> queue = new();

            while (insertData.Length > 0)
            {
                int openParenIndex = insertData.IndexOf('(');
                int closeParenIndex = insertData.IndexOf(')');

                if (openParenIndex == -1 || closeParenIndex == -1)
                {
                    break; // Exit the loop if no more valid data is found
                }

                string s = insertData.Substring(openParenIndex, closeParenIndex - openParenIndex + 1);
                insertData = insertData.Remove(openParenIndex, closeParenIndex - openParenIndex + 1);

                CQueue<object> row = RowData(s);
                queue.Enqueue(row);
            }

            return queue;
        }

        private static void GetTableNameAndInsertParams(string tableParams, out string tableName, out string[]? insertParams)
        {
            tableName = string.Empty;
            insertParams = null;
            if (tableParams.Contains(' '))
            {
                tableName = tableParams.Substring(0, tableParams.IndexOf(' '));
                insertParams = tableParams
                                    .Replace(tableName, string.Empty)
                                    .TrimStart()
                                    .TrimStart('(')
                                    .TrimEnd(')')
                                    .Replace(" ", string.Empty)
                                    .Split(',');
            }
            else
            {
                tableName = tableParams;
            }
        }
        public static object ParseValue(string value)
        {
            // Add more data type checks as needed
            if (int.TryParse(value, out int intValue))
            {
                return intValue;
            }
            else if (DateOnly.TryParse(value, out DateOnly dateTimeValue))
            {
                return dateTimeValue;
            }
            else
            {
                return value.Replace("\"", string.Empty);
            }
        }
        private void DisplayDataInGridView(Table table)
        {
            foreach (Property property in table.Properties)
                dataGridView1.Columns.Add(property.Name, property.Name);

            for (int i = 0; i < table.Properties[0].Values.Count; i++) // Assuming all properties have the same number of values
            {
                List<object> rowValues = new List<object>();

                foreach (Property property in table.Properties)
                {
                    rowValues.Add(property.Values[i]);
                }

                dataGridView1.Rows.Add(rowValues.ToArray());
                TableDataLbl.Text = $"Retrieving details on table {table.Name} ... Done => {table.Properties[0].Values.Count} rows.";

            }
        }
        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();
            dataGridView1.Refresh();
            string tableName = listView1.SelectedItems[0].SubItems[0].Text;


            Table? table = Table.FindTableByName(tables, tableName);

            if (table is null)
                throw new InvalidOperationException("Table doesnt exist!");

            DisplayDataInGridView(table);

        }
        private static CQueue<string> SplitQueries(string line)
        {
            CQueue<string> queue = new CQueue<string>();
            string[] words = line.Split(' ');
            string[] keywords = { "SELECT", "DELETE", "WHERE", "ORDERBY", "ORDERBYDESC" };
            string text = string.Empty;

            for (int i = 0; i < words.Length; i++)
            {
                text += words[i] + " ";
                if (i < words.Length - 1 && keywords.Contains(words[i + 1], StringComparer.OrdinalIgnoreCase))
                {
                    queue.Enqueue(text.Trim());
                    text = string.Empty;
                }
            }
            queue.Enqueue(text.Trim());

            return queue;
        }

        private void FindIfTableAlreadyExists(string tableName)
        {
            foreach (Table table in tables)
                if (table.Name == tableName)
                    throw new Exception($"Touble {tableName} already exists!");
        }
        private Table? GetTable(string tableName)
        {
            foreach (Table table in tables)
                if (table.Name == tableName)
                    return table;

            return null;
        }
        private void RefreshListView()
        {
            listView1.Items.Clear();

            foreach (var table in tables)
            {
                ListViewItem item = new ListViewItem();
                item.Text = table.Name;
                listView1.Items.Add(item);
            }
        }


        public void SaveAllTables()
        {
            if (!Directory.Exists(storagePath))
            {
                Directory.CreateDirectory(storagePath);
            }

            foreach (var table in tables)
            {
                string filePath = Path.Combine(storagePath, $"{table.Name}.csv");
                SaveTableToFile(table, filePath);
            }
        }

        public void SaveTableToFile(Table table, string filePath)
        {
            using (StreamWriter sw = new StreamWriter(filePath))
            {
                // Write table name
                sw.WriteLine(table.Name);

                // Write properties
                foreach (var property in table.Properties)
                {
                    property.Serialize(sw);
                }
            }
        }
        public void LoadAllTables(params string[] filePaths)
        {
            foreach (var filePath in filePaths)
            {
                Table loadedTable = LoadTableFromFile(filePath);
                if (loadedTable != null)
                {
                    tables.Add(loadedTable);
                }
            }

            // Refresh the ListView or any other UI elements as needed
            RefreshListView();
        }
        //When a method has params, you can call it with zero or more arguments of the specified type
        //, and the compiler automatically packages them into an array.
        //If a method does not use params, you must explicitly provide an array or an instance of the specified type.
        public Table LoadTableFromFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                try
                {
                    Table table = new Table("", new List<Property>());

                    using (StreamReader sr = new StreamReader(filePath))
                    {
                        // Read table name
                        table.Name = sr.ReadLine();

                        // Read properties
                        while (!sr.EndOfStream)
                        {
                            Property property = new Property();
                            property.Deserialize(sr);
                            table.Properties.Add(property);
                        }
                    }

                    return table;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading table from file: {ex.Message}");
                    return null;
                }
            }
            else
            {
                return null; // File doesn't exist
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e) //save all button
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "All files (*.*)|*.*";
                saveFileDialog.Title = "Save File";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    storagePath = saveFileDialog.FileName;
                    SaveAllTables(); // Replace with your actual save method
                }
            }
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e) //delete all button
        {
            try
            {
                if (Directory.Exists(storagePath))
                {
                    // Ask for confirmation before deleting
                    DialogResult confirmationResult = MessageBox.Show("Are you sure you want to delete all files?", "Delete Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    if (confirmationResult == DialogResult.Yes)
                    {
                        // Delete all files in the directory
                        string[] files = Directory.GetFiles(storagePath, "*.csv");
                        foreach (string file in files)
                        {
                            System.IO.File.Delete(file);
                        }

                        // Delete the entire directory
                        Directory.Delete(storagePath, true);
                        tables.Clear();


                        // Optionally, recreate the directory if needed
                        // Directory.CreateDirectory(storagePath);
                    }
                    // If the user selects "No" in the initial confirmation, no further action is taken
                    RefreshListView();
                }
                else
                {
                    throw new DirectoryNotFoundException($"Directory does not exist: {storagePath}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting files: {ex.Message}");
            }
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e) //open file button
        {
            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
            {
                folderBrowserDialog.Description = "Select Folder";

                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedFolderPath = folderBrowserDialog.SelectedPath;
                    storagePath = selectedFolderPath;

                    // Get a list of CSV files in the selected folder
                    string[] csvFiles = Directory.GetFiles(selectedFolderPath, "*.csv");

                    // Call LoadAllTables with the list of file paths
                    LoadAllTables(csvFiles);
                }
            }
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e) //save changes button
        {
            // Assuming tables have been modified during the application execution

            // Ask the user for confirmation
            DialogResult result = MessageBox.Show("Do you want to save changes?", "Confirmation", MessageBoxButtons.YesNo);

            if (result == DialogResult.Yes)
            {
                if (storagePath != null && Directory.Exists(storagePath))
                {
                    foreach (var table in tables)
                    {
                        // Get the file path for the current table
                        string filePath = Path.Combine(storagePath, $"{table.Name}.csv");

                        // Save the current table to the file
                        SaveTableToFile(table, filePath);
                    }

                    MessageBox.Show("Changes saved successfully.", "Success", MessageBoxButtons.OK);
                }
                else
                {
                    MessageBox.Show("Error: Storage path is not set or does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                // Determine the clicked item
                ListViewItem clickedItem = listView1.GetItemAt(e.X, e.Y);

                // If an item is clicked, show the context menu
                if (clickedItem != null)
                {
                    contextMenuStrip1.Show(listView1, e.Location);


                }
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                string tableName = listView1.SelectedItems[0].Text;
                Table? table = Table.FindTableByName(tables, tableName);

                if (table is not null)
                {
                    using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                    {
                        saveFileDialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
                        saveFileDialog.Title = "Save CSV File";
                        saveFileDialog.FileName = $"{tableName}.csv"; // Default file name based on the table name

                        if (saveFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            storagePath = saveFileDialog.FileName;

                            try
                            {
                                using (StreamWriter sw = new StreamWriter(storagePath))
                                {
                                    // Write table name
                                    sw.WriteLine(table.Name);

                                    // Write properties
                                    foreach (var property in table.Properties)
                                    {
                                        property.Serialize(sw);
                                    }
                                }

                                MessageBox.Show($"Table '{tableName}' saved successfully.", "Table Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"Error saving table: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a table to save.", "No Table Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string tableName = listView1.SelectedItems[0].Text;

            DialogResult result = MessageBox.Show($"Are you sure you want to delete the table '{tableName}'?", "Delete Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                Table? tableToRemove = Table.FindTableByName(tables, tableName);

                if (tableToRemove is not null)
                {
                    tables.Remove(tableToRemove);
                    listView1.Items.Remove(listView1.SelectedItems[0]);

                    string filePath = Path.Combine(storagePath, $"{tableName}.csv");

                    try
                    {
                        if (File.Exists(filePath))
                            File.Delete(filePath);
                        else
                            MessageBox.Show($"File not found: {filePath}", "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error deleting file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                MessageBox.Show($"Table '{tableName}' deleted successfully.", "Table Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            RefreshListView();  // Refresh the ListView after deletion
        }
    }

}




