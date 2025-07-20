using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Specialized;

class Program
{
    static void Main()
    {
        try
        {
            // Create server and database objects in memory (no connection)
            Server server = new Server();
            Database db = new Database(server, "MyDatabase");

            // Create a table in memory
            Table table = new Table(db, "Employees", "dbo");

            // Add columns
            Column idColumn = new Column(table, "ID", DataType.Int);
            idColumn.Nullable = false;
            idColumn.Identity = true;
            idColumn.IdentityIncrement = 1;
            idColumn.IdentitySeed = 1;
            table.Columns.Add(idColumn);

            Column nameColumn = new Column(table, "Name", DataType.NVarChar(100));
            nameColumn.Nullable = false;
            table.Columns.Add(nameColumn);

            Column emailColumn = new Column(table, "Email", DataType.NVarChar(255));
            emailColumn.Nullable = true;
            table.Columns.Add(emailColumn);

            Column salaryColumn = new Column(table, "Salary", DataType.Decimal(10, 2));
            salaryColumn.Nullable = true;
            table.Columns.Add(salaryColumn);

            // Add primary key
            var primaryKey = new Microsoft.SqlServer.Management.Smo.Index(table, "PK_Employees");
            primaryKey.IndexKeyType = IndexKeyType.DriPrimaryKey;
            primaryKey.IndexedColumns.Add(new IndexedColumn(primaryKey, "ID"));
            table.Indexes.Add(primaryKey);

            // Create scripter
            Scripter scripter = new Scripter(server);
            scripter.Options.Indexes = true;
            scripter.Options.DriAll = true;
            scripter.Options.SchemaQualify = true;
            scripter.Options.DriAllConstraints = true;

            // Generate script for the table
            Console.WriteLine("-- Generated SQL Script (No Database Connection)");
            Console.WriteLine("-- Generated on: " + DateTime.Now);
            Console.WriteLine();

            StringCollection scripts = scripter.Script(new SqlSmoObject[] { table });

            foreach (string script in scripts)
            {
                Console.WriteLine(script);
                Console.WriteLine("GO");
                Console.WriteLine();
            }

            // Create a stored procedure in memory
            StoredProcedure sp = new StoredProcedure(db, "GetEmployeeById", "dbo");

            // Add parameter
            StoredProcedureParameter param = new StoredProcedureParameter(sp, "@EmployeeID", DataType.Int);
            sp.Parameters.Add(param);

            // Set procedure body
            sp.TextMode = false;
            sp.TextBody = @"
SELECT ID, Name, Email, Salary 
FROM dbo.Employees 
WHERE ID = @EmployeeID";

            // Generate script for stored procedure
            Console.WriteLine("-- Stored Procedure Script");
            StringCollection spScripts = scripter.Script(new SqlSmoObject[] { sp });

            foreach (string script in spScripts)
            {
                Console.WriteLine(script);
                Console.WriteLine("GO");
                Console.WriteLine();
            }

            // Create another table with foreign key relationship
            Table departmentTable = new Table(db, "Departments", "dbo");

            Column deptIdColumn = new Column(departmentTable, "DeptID", DataType.Int);
            deptIdColumn.Nullable = false;
            deptIdColumn.Identity = true;
            departmentTable.Columns.Add(deptIdColumn);

            Column deptNameColumn = new Column(departmentTable, "DeptName", DataType.NVarChar(50));
            deptNameColumn.Nullable = false;
            departmentTable.Columns.Add(deptNameColumn);

            // Add primary key to department table
            var deptPK = new Microsoft.SqlServer.Management.Smo.Index(departmentTable, "PK_Departments");
            deptPK.IndexKeyType = IndexKeyType.DriPrimaryKey;
            deptPK.IndexedColumns.Add(new IndexedColumn(deptPK, "DeptID"));
            departmentTable.Indexes.Add(deptPK);

            // Add department column to employee table
            Column deptColumn = new Column(table, "DepartmentID", DataType.Int);
            deptColumn.Nullable = true;
            table.Columns.Add(deptColumn);

            // Generate scripts for both tables
            Console.WriteLine("-- Department Table Script");
            StringCollection deptScripts = scripter.Script(new SqlSmoObject[] { departmentTable });

            foreach (string script in deptScripts)
            {
                Console.WriteLine(script);
                Console.WriteLine("GO");
                Console.WriteLine();
            }

            Console.WriteLine("-- Updated Employee Table Script");
            StringCollection updatedTableScripts = scripter.Script(new SqlSmoObject[] { table });

            foreach (string script in updatedTableScripts)
            {
                Console.WriteLine(script);
                Console.WriteLine("GO");
                Console.WriteLine();
            }

            Console.WriteLine("-- Script generation completed (no database connection required)");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }

        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
}