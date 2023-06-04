using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Bessett.SmartConsole;

namespace Bessett.CodeWriter.Tests.Tasks
{

    public class CoolType
    {
        public String Name { get; set; }

    }

    public class CoolType2
    {
        public String Nombre { get; set; }

        public CoolType2 Transform(CoolType value)
        {
            return new CoolType2()
            {
                Nombre = "No One"
            };
        }
        public static implicit operator CoolType2(CoolType value)
        {
            return new CoolType2()
            {
                Nombre = $"{value.Name}!"
            };
        }
    }

    [NoConfirmation]
    public class Test1:ConsoleTask
    {
        public override TaskResult StartTask()
        {
            try
            {
                var coolType = new CoolType();
                var coolType2 = new CoolType2();

                coolType.Name = "Bernouli";

                Console.WriteLine($"{coolType2.Transform(coolType).Nombre }");

                var check = Activator.CreateInstance(typeof(CoolType));

                check = coolType2.Transform(coolType);
                Console.WriteLine($"{check.GetType().GetProperty("Nombre").GetValue(check)}");
                coolType.Name = "James";

                var xCheck = coolType2.Transform(coolType);
                CoolType2 xxCheck = coolType;

                var xxxCheck = new CoolType2();
                xxxCheck = coolType;

                return TaskResult.Complete();
            }
            catch (Exception ex)
            {
                return TaskResult.Exception(ex);
            }


        }
    }
}
